using PaymentDetailApi.Domain.Catalog.Events;
using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Shared.ValueObjects;

namespace PaymentDetailApi.Domain.Catalog.Entities
{
    public class Product : AggregateRoot
    {
        public string Name { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public Money Price { get; private set; } = null!;
        public int Stock { get; private set; }
        public Guid CategoryId { get; private set; }
        public bool IsActive { get; private set; }

        private Product() { } // for EF Core materialization
        private Product(string name, string description, Money price, int stock, Guid categoryId, bool isActive)
        {
            Validate(name, description, stock, categoryId);
            Name = name;
            Description = description;
            Price = price;
            Stock = stock;
            CategoryId = categoryId;
            IsActive = isActive;
        }
        public static Product Create(string name, string description, Money price, int stock, Guid categoryId, bool isActive)
        {
            return new Product(name, description, price, stock, categoryId, isActive);
        }

        public void AddStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity to add must be greater than zero.", nameof(quantity));

            Stock += quantity;

            AddDomainEvent(new ProductStockAddedDomainEvent(this, quantity));
        }

        public void RemoveStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity to remove must be greater than zero.", nameof(quantity));

            if (quantity > Stock)
                throw new InvalidOperationException($"Insufficient stock for product '{Name}'. Available: {Stock}, requested: {quantity}.");

            Stock -= quantity;

            AddDomainEvent(new ProductStockRemovedDomainEvent(this, quantity));
        }

        private static void Validate(string name, string description, int stock, Guid categoryId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name is required.", nameof(name));

            if (name.Length > 200)
                throw new ArgumentException("Product name must not exceed 200 characters.", nameof(name));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Product description is required.", nameof(description));

            // Price > 0 is already guaranteed by Money.Create().

            if (stock < 0)
                throw new ArgumentException("Product stock cannot be negative.", nameof(stock));

            if (categoryId == Guid.Empty)
                throw new ArgumentException("A valid category is required.", nameof(categoryId));
        }
    }
}
