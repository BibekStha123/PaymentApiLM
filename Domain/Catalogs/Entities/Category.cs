using PaymentDetailApi.Domain.Common;

namespace PaymentDetailApi.Domain.Catalog.Entities
{
    public class Category : AggregateRoot
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }

        private Category(string name, string type)
        {
            Validate(name, type);
            Name = name;
            Type = type;
        }
        public static Category Create(string name, string type)
        {
            return new Category(name, type);
        }

        private static void Validate(string name, string type)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name is required.", nameof(name));

            if (name.Length > 100)
                throw new ArgumentException("Category name must not exceed 100 characters.", nameof(name));

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Category type is required.", nameof(type));

            if (type.Length > 50)
                throw new ArgumentException("Category type must not exceed 50 characters.", nameof(type));
        }
    }
}
