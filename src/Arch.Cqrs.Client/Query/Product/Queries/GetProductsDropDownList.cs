using Arch.Cqrs.Client.Query.Product.Models;
using Arch.Infra.Shared.Cqrs.Query;
using System.Collections.Generic;

namespace Arch.Cqrs.Client.Query.Product.Queries
{
    public class GetProductsDropDownList : IQuery<IReadOnlyList<ProductDropDownItem>>
    {
        public string Seach { get; set; }
    }
}
