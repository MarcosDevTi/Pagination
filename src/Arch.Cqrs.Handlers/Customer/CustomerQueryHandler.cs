using Arch.Cqrs.Client.Paging;
using Arch.Cqrs.Client.Query.Customer.Models;
using Arch.Cqrs.Client.Query.Customer.Queries;
using Arch.Infra.Data;
using Arch.Infra.Shared.Cqrs.Query;
using AutoMapper;

namespace Arch.Cqrs.Handlers.Customer
{
    public class CustomerQueryHandler :
        IQueryHandler<GetCustomersIndex, PagedResult<CustomerIndex>>,
        IQueryHandler<GetCustomerDetails, CustomerDetails>
    {
        private readonly ArchDbContext _architectureContext;

        public CustomerQueryHandler(ArchDbContext architectureContext)
        {
            _architectureContext = architectureContext;
        }

        public PagedResult<CustomerIndex> Handle(GetCustomersIndex query)

        {
            return _architectureContext.Customers
                //.Where(
                    //new SpecGeneric<Domain.Models.Customer>()
                        //.AddSpec(new OnlyGreaterThan18Years())
                        //.AddSpec(new EspecialCustomer())
                        //.AddSpec(new SearchCustomer().AddSearch(query.Search))
                        //.Build())
                .GetPagedResult(
                query.Paging);
        }

        public CustomerDetails Handle(GetCustomerDetails query)
        {
            return Mapper.Map<CustomerDetails>(_architectureContext.Customers.Find(query.Id));
        }
    }
}
