using System.Collections.Generic;
using Arch.Cqrs.Client.Query.Product.Models;
using Arch.Infra.Shared.Cqrs.Query;

namespace Arch.Cqrs.Client.Query.Product.Queries
{
    public class GetProductsIndex : IQuery<IReadOnlyList<ProductIndex>>
    {

    }
}
