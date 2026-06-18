using PaymentDetailApi.Domain.Common;

namespace PaymentDetailApi.Domain.Catalog.Entities
{
    public class Product : Entity
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
            Name = name;
            Description = description;
            Price = price;
            Stock = stock;
            CategoryId = categoryId;
            IsActive = isActive;
        }
        public static Product Create(string name, string description, decimal price, int stock, int categoryId, bool isActive)
        {
            // Add validation logic here if needed
            return new Product(name, description, price, stock, categoryId, isActive);
        }
    }
}
