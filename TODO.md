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

- [x] **Wire up role-based policy**
  - [x] API (`API/Program.cs`): `AdminOnly` policy registered (`RequireRole("Admin")`), applied per-action.
  - [x] Gateway (`PaymentDetailApi.Gateway/appsettings.json`): mirrored — `AdminOnly` policy added alongside `Authenticated`, applied to matching admin routes (products write ops, categories, orders list, payment-details list) so the edge enforces the same boundary as the API.
  - [ ] Decide + implement **Admin bootstrap** — nothing can create an Admin today (`User.cs:28` hardcodes `Role = "User"` on registration). Options: (a) DB seed on startup from config, (b) admin-only promotion endpoint gated by a setup secret or existing Admin, (c) defer. Until this is done, every `AdminOnly` endpoint is unreachable by anyone.
  - [x] Endpoint → policy mapping implemented:
    - Admin-only: `CategoriesController.CreateCategory`, `ProductsController.Post/.Patch/.Delete`, `OrdersController.Get` (list-all), `PaymentDetailsController.GetPaymentDetails()` (list-all)
    - Any authenticated user: `ProductsController.Get`, `CurrencyController.Get`, `OrdersController.Post`, `PaymentDetailsController.PostPaymentDetails`, `.GetMyCards`

- [ ] **Fix resource-ownership gap** (found while investigating; not fixed by role policy — still open)
  - [ ] `PaymentDetailsController.GetPaymentDetails(id)`, `GetPaymentDetailsByName(name)`, `DeletePaymentDetails(id)` — take an id/name and return/delete with **no check the resource belongs to the calling user**. Any logged-in user can currently view or delete another user's card by guessing/enumerating an id. Contrast with `GetMyCards`/`PostPaymentDetails`, which correctly scope via the `ClaimTypes.NameIdentifier` claim — use that pattern. (Deliberately not made Admin-only: that would just block legitimate self-service instead of fixing the ownership check.)
  - [ ] `OrdersController.Get` is now Admin-only (list-all is admin-grade data); still need a separate self-scoped "my orders" endpoint for regular users — see Orders section below.

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

### Improving DDD

Reviewed 2026-09-02. Tactical patterns (rich aggregates, factory methods enforcing invariants, value objects, encapsulated aggregate boundaries, pure domain events) are solidly and consistently applied. The gaps below are architectural/strategic, not a rewrite:

- [ ] **No repository abstraction** — every command handler injects the concrete `PaymentDetailsContext` (EF Core) directly (e.g. `CreateOrderCommandHandler`, `CreateProductCommandHandler`). Application should depend on a domain-owned abstraction (`IOrderRepository`, `IProductRepository`, ...) with EF as one implementation, not the concrete `DbContext` — right now Application → Infrastructure is a direct, concrete dependency.
- [ ] **Cross-aggregate mutation in a single transaction** — `CreateOrderCommandHandler` loads a `Product`, calls `RemoveStock` on it, *and* creates/saves a new `Order` in the same `SaveChanges` call. One transaction should touch one aggregate; stock vs. order consistency should go through domain events/eventual consistency instead of a handler reaching into two aggregates at once.
- [ ] **Namespace/folder drift** — folder `Domain/Catalogs` vs namespace `Domain.Catalog`, folder `Domain/Payments` vs namespace `Domain.Payment`, folder `Domain/Users` vs namespace `Domain.User`. Not a DDD violation by itself, but this kind of inconsistency erodes ubiquitous language discipline over time — pick one (plural, matching the folder) and rename.
- [ ] **`PaymentDetail` uses a bare `bool Active` + `Delete()`** instead of an explicit status concept — `Order` already shows the better pattern with `OrderStatus`. Consider a small status enum for consistency across aggregates.
- Not pursuing for now: full bounded contexts / context maps / anti-corruption layers — folders (Catalogs/Orders/Payments/Users/Shared) are logical modules sharing one `DbContext`, which is appropriate for a single-service app at this scale.

### Observability
- [ ] Structured logging (Serilog or similar) — currently just default `ILogger` usage in the global exception handler.
- [ ] Application-level health check endpoint (`/health`) on the API itself — the Gateway has YARP active health checks for load balancing, but that's routing-layer, not an app health endpoint consumers/orchestrators can hit.

### Cleanup / hygiene
- [ ] `PaymentDetailApi.sln` has pending local changes (`git status`) — confirm intentional before committing.
