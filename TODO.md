# TODO

Roadmap for hardening PaymentDetailApi into a more realistic, production-style project. Refreshed 2026-09-07 with a senior-engineer pass focused on the **backend** — re-verified every item against current code, not carried over blindly.

## Done

- [x] CQRS with MediatR (commands/queries per feature)
- [x] Domain events dispatched via `DomainEventDispatchBehavior` pipeline behavior (not manual per-handler)
- [x] Value objects: `Money`, `CardNumber`, `ExpirationDate`
- [x] FluentValidation validators + `ValidationBehavior` pipeline behavior (covers Users, Orders, Products, Categories, PaymentDetails)
- [x] Global exception handling middleware (`UseExceptionHandler`) in both API and Gateway
- [x] Order creation validates/deducts product stock (`product.RemoveStock` in `CreateOrderCommand`)
- [x] API Gateway (YARP) owns JWT auth, CORS, rate limiting at the edge; API keeps its own `[Authorize]`/JWT validation as defense-in-depth
- [x] Gateway load balancing with active health checks configured
- [x] Passwords hashed with BCrypt (`RegisterUserCommand.cs:32`, `LoginUserCommand.cs:32`) — not plaintext/weak hash
- [x] Role-based `AdminOnly` policy wired in API + Gateway, applied per-action

## 🔴 Critical — do first

- [x] **Secrets committed to source control** — fixed 2026-09-07. `Jwt:Key` (API + Gateway) and `LMStudio:ApiKey` (API) moved to
- [ ] `dotnet user-secrets` (`UserSecretsId` added to both `.csproj`s); checked-in `appsettings.json` values replaced with 
- [ ] empty-string placeholders so a misconfigured prod deploy fails loudly instead of using a known key. 
- [ ] JWT key was rotated (freshly generated, not reused) since the old value is burned — it stays in git history regardless. 
- [ ] Verified end-to-end at runtime: registered a user, logged in, and called a protected endpoint (`GET /payment-details/my-cards`) — token issuance and validation both worked off the new secret-store key, and API/Gateway builds still succeed. **Not done:** purging the old key from git history (destructive rewrite — needs explicit user sign-off) and wiring real env-var/secret-manager values for non-local environments (no such environment exists yet).
- [ ] **Stock oversell race condition.** `CreateOrderCommandHandler` (`Application/Orders/Commands/CreateOrderCommand.cs`)
- [ ] reads a `Product`, calls `RemoveStock`, then saves — with no concurrency token and no transaction-level protection. 
- [ ] Two concurrent orders for the last unit of a product can both pass the in-memory check and both succeed, driving stock negative. Fix: add a `RowVersion`/`ConcurrencyCheck` column on `Product` (EF optimistic concurrency) and catch `DbUpdateConcurrencyException` to retry or fail the order cleanly; or move the decrement to a single atomic `UPDATE ... WHERE Stock >= @qty` guarded call.
- [ ] **Resource-ownership gap.** `PaymentDetailsController.GetPaymentDetails(id)`, `GetPaymentDetailsByName(name)`, `DeletePaymentDetails(id)` take an id/name with **no check it belongs to the calling user** — any authenticated user can view/delete another user's card by guessing an id. Fix is a per-resource ownership check inside the handler (pattern already correct in `GetMyCards`/`PostPaymentDetails` via `ClaimTypes.NameIdentifier`).
- [ ] **Admin bootstrap** — nothing can create an Admin today. `User.cs:28`(`Register`) hardcodes `Role = "User"`. Every `AdminOnly` endpoint (category/product writes, order list, payment-detail list) is unreachable by anyone, including the owner. Needs a decision: DB seed on startup from config, a setup-secret-gated promotion endpoint, or a one-time manual migration/seed.

## Authorization / Auth

- [ ] **Refresh tokens** — login only issues a short-lived access token (`IJwtTokenService.GenerateToken`). Add issue + rotate + revoke so clients aren't forced to re-login on expiry.
- [x] **"My orders" endpoint** — fixed 2026-09-08. Added `GET /orders/my-orders` (`GetOrdersByUserIdQuery`/`GetOrdersByUserIdQueryHandler`), same cursor-pagination shape as the Admin-only list, scoped via `ClaimTypes.NameIdentifier` like `GetMyCards`. No `AdminOnly` restriction — any authenticated user gets their own orders only.
- [ ] **Login throttling / lockout** — currently nothing in the API stops repeated failed login attempts beyond the Gateway's general rate limiter (which isn't login-specific). Consider a per-account or per-IP failed-attempt lockout for `LoginUserCommand`.

### Orders
- [x] `GET /orders/{id}` — fixed 2026-09-08. `GetOrderByIdQuery`/`GetOrderByIdQueryHandler` was a `throw new NotImplementedException()` stub; implemented it plus an ownership check (`RequestingUserId`/`IsAdmin` on the query — not found → 404 `KeyNotFoundException`, found but not yours and not Admin → 401 `UnauthorizedAccessException`, matching the existing exception-middleware mapping). Verified at runtime with two users: owner gets 200, non-owner gets 401, nonexistent id gets 404.
- [x] Expose `Order.Cancel()` — fixed 2026-09-09. `CancelOrderCommand`/`CancelOrderCommandHandler` (`Application/Orders/Commands/CancelOrderCommand.cs`), same ownership-or-admin check as `GetOrderByIdQueryHandler` (404 `KeyNotFoundException` if not found, 401 `UnauthorizedAccessException` if not owner/not admin). New `PATCH /orders/{id}/cancel` endpoint, 204 on success. `Order.Cancel()`'s existing `InvalidOperationException` (cancelling a delivered order) maps to 409 via the exception middleware, no extra handling needed. Verified at runtime: non-owner gets 401, nonexistent id gets 404, owner gets 204 and order status flips to `Cancelled`. Note: `Cancel()` has no guard against re-cancelling an already-cancelled order (only blocks from `Delivered`), so a second cancel silently no-ops — not fixed, flagged only.
- [ ] **Idempotent order creation** — `POST /orders` has no idempotency key; a client retry (network blip) can create duplicate orders and double-decrement stock. Worth an `Idempotency-Key` header + a short-lived dedup table, common in payment-adjacent APIs.

### Testing
- [ ] Replace `PaymentDetailApi.UnitTests/Class1Tests.cs` — leftover scaffold test unrelated to the actual domain; delete or replace.
- [ ] Unit tests for command/query handlers (Orders, Products, Categories, Currency, Users) — only `PaymentDetailTests.cs` and one integration test exist right now.
- [ ] Integration tests for the stock-deduction path (including the concurrency fix above once done) and the validation pipeline.
- [ ] Tests for domain event dispatch (e.g. audit log written after `PaymentCreatedDomainEvent`).
- [ ] Auth tests: ownership-check rejection, `AdminOnly` rejection for non-admins, expired-token handling.

### Improving DDD / Architecture
- [ ] **No repository abstraction + no explicit Unit of Work** — every command handler injects the concrete `PaymentDetailsContext` (EF Core) directly. Application should depend on domain-owned abstractions (`IOrderRepository`, `IProductRepository`, ...) with EF as one implementation, plus an `IUnitOfWork` (`SaveChangesAsync()`) so handlers can coordinate multiple repositories under one atomic commit without knowing about EF Core. Note: `DbContext` already behaves as an implicit Unit of Work today (one `SaveChangesAsync` call = one atomic commit across all tracked entities) — a standalone `IUnitOfWork` wrapper isn't worth adding on its own before repositories exist, since it'd just be a thin re-wrap with no behavioral change. Do both together, not UoW first.
- [ ] **Cross-aggregate mutation in a single transaction** — `CreateOrderCommandHandler` loads a `Product`, mutates it, *and* creates/saves a new `Order` in the same `SaveChanges` call. One transaction should touch one aggregate; stock vs. order consistency should go through domain events/eventual consistency instead.
- [ ] **Namespace/folder drift** — folder `Domain/Catalogs` vs namespace `Domain.Catalog`, folder `Domain/Payments` vs namespace `Domain.Payment`, folder `Domain/Users` vs namespace `Domain.User`. Pick one (plural, matching the folder) and rename.
- [ ] **`PaymentDetail` uses a bare `bool Active` + `Delete()`** instead of an explicit status concept — `Order` already shows the better pattern with `OrderStatus`. Consider a small status enum for consistency.
- Not pursuing for now: full bounded contexts / context maps / anti-corruption layers — a single `DbContext` is appropriate at this scale.

### Observability
- [ ] `/health` exists (`API/Program.cs:94`) but is a static `Results.Ok("Healthy")` — it doesn't actually check anything. Replace with `AddHealthChecks().AddDbContextCheck<PaymentDetailsContext>()` so it reflects real DB connectivity, and expose it in a machine-readable format (JSON) for orchestrators.
- [ ] Structured logging (Serilog or similar) — currently just default `ILogger` usage in the global exception handler. Add request/response logging with correlation IDs, especially useful once the Gateway is in front.
- [ ] Consider basic OpenTelemetry tracing given the Gateway → API hop — makes latency/error attribution across the two processes possible.

### Cleanup / hygiene
- [ ] **Dead/experimental AI-agent code in `PaymentDetailsController`** — a commented-out `Chat`/`ChatRequest` endpoint and an `"LMStudio"` `HttpClient` registered in `API/Program.cs` (pointing at `localhost:1234`) are leftover from an earlier experiment and unrelated to payment details. Either finish and move it to its own controller, or delete it — a commented-out endpoint with an injected-but-unused `IHttpClientFactory` in `PaymentDetailsController` is confusing for anyone reading the class.
- [ ] Add API versioning (`Asp.Versioning` or URL-segment versioning) before the client surface grows further — currently unversioned routes.
- [ ] `PaymentDetailApi.sln` — confirm no stray local changes before committing (recheck; last audit flagged this).

**Why:** User is building this out incrementally as a learning project (CQRS → domain events → RBAC), see [[project_cqrs_implementation]], [[project_roles_and_refresh_tokens]], [[project_feature_audit]]. This pass adds a security/correctness lens (secrets-in-git, stock race condition, idempotency) on top of the existing architecture backlog.
**How to apply:** Work top-down starting at 🔴 Critical. Explain the what/why before each edit and do one file/feature at a time per [[feedback_stepwise_explain_before_edit]].
