using System;
using Arch.Cqrs.Client.AutoMapper;

namespace Arch.Cqrs.Client.Query.Product.Models
{
    public class ProductIndex : IMapFrom<Domain.Models.Product>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

    }
}
