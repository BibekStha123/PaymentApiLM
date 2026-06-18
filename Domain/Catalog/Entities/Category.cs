using PaymentDetailApi.Domain.Common;

namespace PaymentDetailApi.Domain.Catalog.Entities
{
    public class Category : Entity
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }

        private Category(string name, string type)
        {
            Name = name;
            Type = type;
        }
        public static Category Create(string name, string type)
        {
            return new Category(name, type);
        }
    }
}
