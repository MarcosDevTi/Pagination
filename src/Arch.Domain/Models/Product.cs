using System;
using Arch.Domain.Core;

namespace Arch.Domain.Models
{
    public class Product : Entity
    {
        public Product()
        {

        }
        public Product(string name, string description, decimal price, Guid? id = null)
        {
            GetId(Id);
            Name = name;
            Description = description;
            Price = price;
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }
    }
}
