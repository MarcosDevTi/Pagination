using System;
using Arch.Cqrs.Client.Query.Product.Models;
using Arch.Infra.Shared.Cqrs.Query;

namespace Arch.Cqrs.Client.Query.Product.Queries
{
    public class GetProductDetails : IQuery<ProductDetails>
    {
        public GetProductDetails(Guid? id)
        {
            Id = id;
        }

        public Guid? Id { get; private set; }
    }
}
