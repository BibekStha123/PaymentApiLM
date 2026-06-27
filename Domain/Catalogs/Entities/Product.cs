using PaymentDetailApi.Domain.Catalog.Events;
using PaymentDetailApi.Domain.Common;

namespace PaymentDetailApi.Domain.Catalog.Entities
{
    public class Product : AggregateRoot
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }
        public int Stock { get; private set; }
        public int CategoryId { get; private set; }
        public bool IsActive { get; private set; }

        private Product() { } // for EF Core materialization
        private Product(string name, string description, decimal price, int stock, int categoryId, bool isActive)
        {
            Validate(name, description, price, stock, categoryId);
            Name = name;
            Description = description;
            Price = price;
            Stock = stock;
            CategoryId = categoryId;
            IsActive = isActive;
        }
        public static Product Create(string name, string description, decimal price, int stock, int categoryId, bool isActive)
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

        private static void Validate(string name, string description, decimal price, int stock, int categoryId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name is required.", nameof(name));

            if (name.Length > 200)
                throw new ArgumentException("Product name must not exceed 200 characters.", nameof(name));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Product description is required.", nameof(description));

            if (price <= 0)
                throw new ArgumentException("Product price must be greater than zero.", nameof(price));

            if (stock < 0)
                throw new ArgumentException("Product stock cannot be negative.", nameof(stock));

            if (categoryId <= 0)
                throw new ArgumentException("A valid category is required.", nameof(categoryId));
        }
    }
}
