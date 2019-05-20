using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using Arch.Cqrs.Client.Command.Customer;
using Arch.Cqrs.Client.Event;
using Arch.Cqrs.Client.Paging;
using Arch.Cqrs.Client.Query.Customer.Models;
using Arch.Cqrs.Client.Query.Customer.Queries;
using Arch.Domain.Core.Event;
using Arch.Domain.Event;
using Arch.Infra.Data;
using Arch.Infra.Data.EventSourcing;
using Arch.Infra.Shared.Cqrs.Event;
using Arch.Infra.Shared.Cqrs.Query;
using Arch.Infra.Shared.EventSourcing;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Arch.Cqrs.Handlers.Customer
{
    public class CustomerQueryHandler :
        IQueryHandler<GetCustomersIndex, PagedResult<CustomerIndex>>,
        IQueryHandler<GetCustomerDetails, CustomerDetails>,
        IQueryHandler<GetCustomerHistory, IReadOnlyList<object>>
    {
        private readonly ArchDbContext _architectureContext;
        private readonly EventSourcingContext _eventSourcingContext;

        public CustomerQueryHandler(ArchDbContext architectureContext, EventSourcingContext eventSourcingContext)
        {
            _architectureContext = architectureContext;
            _eventSourcingContext = eventSourcingContext;
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

        public IReadOnlyList<object> Handle(GetCustomerHistory query)
        {
            var typeOriginal = _eventSourcingContext.EventEntities.Where(x => 
                x.AggregateId == query.AggregateId)
                .OrderBy(d => d.When)
                .ToList().Select(_ => _.ReadToObject(_, typeof(Domain.Models.Customer))
            ).ToList();
           
            return typeOriginal;
        }
    }
}
