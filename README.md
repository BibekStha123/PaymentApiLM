# PaymentDetailApi

A .NET 9 Web API implementing **DDD**, **CQRS**, and **Domain Events** with a custom dispatcher.

---

## Domain Events — How It Works

Domain events are used to decouple side-effects (e.g. audit logging, notifications) from the core business logic. The implementation follows DDD principles with a custom dispatcher — no MediatR involvement.

---

### Components

| Component | Location | Responsibility |
|---|---|---|
| `DomainEvent` | `Domain/Common/` | Abstract base class all events inherit from |
| `IDomainEventHandler<T>` | `Domain/Common/` | Interface every handler must implement |
| `PaymentCreatedDomainEvent` | `Domain/Payment/Events/` | Concrete event, carries the `PaymentDetail` entity |
| `DomainEventDispatcher` | `Infrastructure/DomainEvents/` | Resolves and invokes handlers at runtime via DI |
| `PaymentCreatedAuditHandler` | `Infrastructure/EventHandlers/` | Concrete handler — reacts to `PaymentCreatedDomainEvent` |

---

### Flow

```
HTTP Request
    │
    ▼
CreatePaymentDetailCommandHandler
    │
    ├─ 1. new PaymentDetail(...)
    │         └─ constructor calls AddDomainEvent(new PaymentCreatedDomainEvent(this))
    │                 └─ event stored in private _domainEvents list on the entity
    │
    ├─ 2. _context.SaveChangesAsync()
    │         └─ payment saved to DB, payment.Id is now set
    │
    ├─ 3. events = payment.DomainEvents.ToList()
    │     payment.ClearEvents()            ← prevents double-dispatch
    │
    ├─ 4. dispatcher.Dispatch(events)
    │         └─ for each event:
    │               ├─ get runtime type  →  PaymentCreatedDomainEvent
    │               ├─ build DI key      →  IDomainEventHandler<PaymentCreatedDomainEvent>
    │               ├─ resolve handlers  →  [PaymentCreatedAuditHandler]  (from DI)
    │               └─ invoke Handle()   →  handler runs
    │
    └─ 5. _context.SaveChangesAsync()
              └─ saves any DB changes made inside the handler (e.g. audit log)
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

**4. Done.** The assembly scanner in `Program.cs` auto-registers all `IDomainEventHandler<T>` implementations — no manual DI registration needed.

---

### Rules

- **Events are raised inside the entity** — never from the command handler or a service.
- **Events are dispatched after `SaveChangesAsync`** — so the entity's `Id` is available to handlers.
- **Always clear events before dispatching** — prevents double-dispatch on a second save.
- **One handler per file** — keep side-effects isolated and independently testable.
