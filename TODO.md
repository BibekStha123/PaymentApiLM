# TODO

Roadmap for hardening PaymentDetailApi into a more realistic, production-style project. Checked items were verified against the current codebase on 2026-08-30, not just assumed from past notes.

## Done

- [x] CQRS with MediatR (commands/queries per feature)
- [x] Domain events dispatched via `DomainEventDispatchBehavior` pipeline behavior (not manual per-handler)
- [x] Value objects: `Money`, `CardNumber`, `ExpirationDate`
- [x] FluentValidation validators + `ValidationBehavior` pipeline behavior (covers Users, Orders, Products, Categories, PaymentDetails)
- [x] Global exception handling middleware (`UseExceptionHandler`) in both API and Gateway
- [x] Order creation validates/deducts product stock (`product.RemoveStock` in `CreateOrderCommand`)
- [x] API Gateway (YARP) owns JWT auth, CORS, rate limiting at the edge; API keeps its own `[Authorize]`/JWT validation as defense-in-depth
- [x] Gateway load balancing with active health checks configured

## Next up

### Authorization

Investigated 2026-08-31. Current state: every controller only uses bare `[Authorize]` (valid token = full access). `User.Role` is captured and embedded in the JWT (`JwtTokenService.cs:29`) but **nothing checks it**, and `User.cs:28` hardcodes every new registration to `Role = "User"` — there is no path today that can ever produce an `"Admin"` account. So right now any authenticated user can call any endpoint, including catalog-mutation ones.

Two distinct problems to solve — role-based policy (what kind of user) and resource ownership (is this *your* resource) are separate mechanisms, don't conflate them:

- [ ] **Wire up role-based policy**
  - [ ] API (`API/Program.cs`): register a named policy, e.g. `AddAuthorization(o => o.AddPolicy("AdminOnly", p => p.RequireRole("Admin")))`, apply `[Authorize(Policy = "AdminOnly")]` (or `[Authorize(Roles = "Admin")]`) per-action (not whole-controller, since access levels are mixed within controllers).
  - [ ] Gateway (`PaymentDetailApi.Gateway/appsettings.json`): mirror it — add an `"AdminOnly"` `AuthorizationPolicy` alongside the existing `"Authenticated"` one, apply to the same routes, to keep the Gateway's defense-in-depth model consistent with the API.
  - [ ] Decide + implement **Admin bootstrap** — nothing can create an Admin today. Options to choose between: (a) DB seed on startup from config, (b) admin-only promotion endpoint gated by a setup secret or existing Admin, (c) defer bootstrapping and just wire the policy shape for now.
  - [ ] Proposed endpoint → policy mapping (pending confirmation):
    - Admin-only: `CategoriesController.CreateCategory`, `ProductsController.Post` (create), `.Patch` (stock), `.Delete`
    - Any authenticated user: `ProductsController.Get`, `CurrencyController.Get`, `OrdersController.Post`, `PaymentDetailsController.PostPaymentDetails`, `.GetMyCards`

- [ ] **Fix resource-ownership gap** (found while investigating; not fixed by role policy)
  - [ ] `PaymentDetailsController.GetPaymentDetails(id)`, `GetPaymentDetailsByName(name)`, `DeletePaymentDetails(id)` — take an id/name and return/delete with **no check the resource belongs to the calling user**. Any logged-in user can currently view or delete another user's card by guessing/enumerating an id. Contrast with `GetMyCards`/`PostPaymentDetails`, which correctly scope via the `ClaimTypes.NameIdentifier` claim — use that pattern.
  - [ ] `PaymentDetailsController.GetPaymentDetails()` (list-all, no id) — also returns everyone's payment details unfiltered; decide if this should be Admin-only or removed/replaced by `GetMyCards`.
  - [ ] `OrdersController.Get` — appears to return all orders with no user filter either; confirm and decide "my orders" vs. admin-wide list (mirrors the existing Orders TODO item below).

- [ ] **Refresh tokens** — login only issues a short-lived access token (`IJwtTokenService.GenerateToken`). Add a refresh token flow (issue + rotate + revoke) so clients aren't forced to re-login on expiry.

### Orders
- [ ] `GET /orders/{id}` — only list (`GET /orders`, cursor-paged) and `POST /orders` exist today.
- [ ] Expose `Order.Cancel()` — the domain method exists on the entity but no command/endpoint calls it.
- [ ] Consider a "my orders" filter vs. admin-wide list.

### Testing
- [ ] Replace `PaymentDetailApi.UnitTests/Class1Tests.cs` — it's a leftover scaffold test (`Class1`, payment-amount filtering) unrelated to the actual domain; delete or replace with real coverage.
- [ ] Unit tests for command/query handlers (Orders, Products, Categories, Currency, Users) — only `PaymentDetailTests.cs` and one integration test exist right now.
- [ ] Integration tests for the new stock-deduction path and validation pipeline.
- [ ] Tests for domain event dispatch (e.g. audit log written after `PaymentCreatedDomainEvent`).

### Observability
- [ ] Structured logging (Serilog or similar) — currently just default `ILogger` usage in the global exception handler.
- [ ] Application-level health check endpoint (`/health`) on the API itself — the Gateway has YARP active health checks for load balancing, but that's routing-layer, not an app health endpoint consumers/orchestrators can hit.

### Cleanup / hygiene
- [ ] `PaymentDetailApi.sln` has pending local changes (`git status`) — confirm intentional before committing.
