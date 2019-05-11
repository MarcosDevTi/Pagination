using Arch.Cqrs.Client.Paging;
using Arch.Cqrs.Client.Query.Customer.Models;
using Arch.Infra.Shared.Cqrs.Query;

namespace Arch.Cqrs.Client.Query.Customer.Queries
{
    public class GetCustomersIndex : IQuery<PagedResult<CustomerIndex>>
    {
        public GetCustomersIndex(
            Paging<CustomerIndex> paging,
            string search)
        {
            Paging = paging;
            Search = search ?? "";
        }

        public Paging<CustomerIndex> Paging { get; private set; }
        public string Search { get; private set; }
    }
}
