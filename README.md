# PaymentDetailApi

A .NET 9 Web API implementing **DDD**, **CQRS**, and **Domain Events** with a custom dispatcher.

---

## Domain Events — How It Works

Domain events are used to decouple side-effects (e.g. audit logging, notifications) from the core business logic. The implementation follows DDD principles with a custom dispatcher triggered automatically via a **MediatR Pipeline Behavior**.

---

### Components

| Component | Location | Responsibility |
|---|---|---|
| `DomainEvent` | `Domain/Common/` | Abstract base class all events inherit from |
| `IDomainEventHandler<T>` | `Domain/Common/` | Interface every handler must implement |
| `AggregateRoot` | `Domain/Common/` | Base class for entities; holds the private `_domainEvents` list |
| `PaymentCreatedDomainEvent` | `Domain/Payment/Events/` | Concrete event, carries the `PaymentDetail` entity |
| `DomainEventDispatcher` | `Infrastructure/DomainEvents/` | Resolves and invokes handlers at runtime via DI |
| `DomainEventDispatchBehavior` | `Application/Common/Behaviors/` | MediatR pipeline behavior — automatically runs after every command handler |
| `PaymentCreatedAuditHandler` | `Infrastructure/EventHandlers/` | Concrete handler — reacts to `PaymentCreatedDomainEvent` |

---

### How the Pieces Connect

#### The controller never calls the handler directly

The controller calls `_mediator.Send(command)`. MediatR does **not** call the command handler immediately — it first builds a **pipeline** of all registered `IPipelineBehavior<,>` implementations that match the request type.

`DomainEventDispatchBehavior` is registered in `Program.cs` as:
```csharp
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DomainEventDispatchBehavior<,>));
```

`CreatePaymentDetailCommand` implements `ICommand<int>`, which implements `IRequest<int>`. `DomainEventDispatchBehavior` has the constraint `where TRequest : ICommand<TResponse>`, so MediatR wraps every command with this behavior automatically.

#### What `next` is

MediatR passes a `RequestHandlerDelegate<TResponse> next` into the behavior's `Handle` method. This delegate, when called (`await next()`), executes the actual command handler. The behavior controls **when** the handler runs and can execute code **before and after** it — exactly like ASP.NET middleware wraps an HTTP request.

---

### Full Call Stack

```
POST /api/PaymentDetail
    │
    ▼
PaymentDetailController.PostPaymentDetails()
    │
    └─ _mediator.Send(command)
            │
            ▼  MediatR builds pipeline, calls behavior first
    DomainEventDispatchBehavior.Handle()
            │
            ├─ await next()   ← delegate that calls the real handler
            │       │
            │       ▼
            │   CreatePaymentDetailCommandHandler.Handle()
            │       ├─ new PaymentDetail(...)
            │       │       └─ constructor: AddDomainEvent(new PaymentCreatedDomainEvent(this))
            │       │                           └─ event stored in _domainEvents on the entity
            │       │
            │       └─ _context.SaveChangesAsync()
            │               └─ payment row inserted, payment.Id is now set
            │                  events are STILL on the entity (not dispatched yet)
            │
            ← control returns here after next() completes
            │
            ├─ scan _context.ChangeTracker for AggregateRoot entities with pending events
            ├─ collect all events, then clear them from entities (prevents double-dispatch)
            ├─ await dispatcher.Dispatch(events)
            │       └─ for each event:
            │             ├─ get runtime type  →  PaymentCreatedDomainEvent
            │             ├─ build DI key      →  IDomainEventHandler<PaymentCreatedDomainEvent>
            │             ├─ resolve handlers  →  [PaymentCreatedAuditHandler]  (from DI)
            │             └─ invoke Handle()   →  handler writes audit log row to context
            │
            └─ await _context.SaveChangesAsync()
                    └─ audit log row persisted to DB
```

---

### How the Dispatcher Resolves the Right Handler

The dispatcher uses **reflection + DI** — it never hard-codes any event or handler type:

```csharp
var eventType   = domainEvent.GetType();
// e.g. PaymentCreatedDomainEvent

var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
// builds → IDomainEventHandler<PaymentCreatedDomainEvent>  at runtime

var handlers = _serviceProvider.GetServices(handlerType);
// DI container returns all handlers registered under that key
```

This means the dispatcher code **never changes** regardless of how many events you add.

---

### Adding a New Domain Event

**1. Create the event** in `Domain/<Aggregate>/Events/`:
```csharp
public class PaymentFailedDomainEvent : DomainEvent
{
    public PaymentDetail PaymentDetails { get; }
    public PaymentFailedDomainEvent(PaymentDetail paymentDetails)
        => PaymentDetails = paymentDetails;
}
```

**2. Raise it inside the entity**:
```csharp
// inside PaymentDetail
public void MarkAsFailed()
{
    AddDomainEvent(new PaymentFailedDomainEvent(this));
}
```

**3. Create the handler** in `Infrastructure/EventHandlers/`:
```csharp
public class PaymentFailedHandler : IDomainEventHandler<PaymentFailedDomainEvent>
{
    public Task Handle(PaymentFailedDomainEvent domainEvent)
    {
        // react to the event
        return Task.CompletedTask;
    }
}
```

**4. Register it** in `Program.cs`:
```csharp
builder.Services.AddScoped<IDomainEventHandler<PaymentFailedDomainEvent>, PaymentFailedHandler>();
```

The behavior picks it up automatically — no changes needed to the dispatcher or the behavior.

---

### Rules

- **Events are raised inside the entity** — never from the command handler or a service.
- **Events are dispatched after the first `SaveChangesAsync`** — so the entity's `Id` is available to handlers.
- **Always clear events before dispatching** — prevents double-dispatch if `SaveChangesAsync` is called again.
- **The behavior only wraps `ICommand<T>`** — queries do not trigger event dispatch.
- **One handler per file** — keep side-effects isolated and independently testable.