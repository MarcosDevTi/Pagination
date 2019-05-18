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
using Arch.Infra.Shared.Cqrs.Event;
using Arch.Infra.Shared.Cqrs.Query;
using Arch.Infra.Shared.EventSourcing;
using AutoMapper;

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
            var typeOriginal = _eventSourcingContext.StoredEvent.Where(x => 
                x.AggregateId == query.AggregateId)
                .OrderBy(d => d.Data)
                .ToList().Select( x =>
                 ReadToObject(x.Data, x.Assembly)
            ).ToList();
           
            return typeOriginal;
        }

        public static object ReadToObject(string json, string typeP)
        {
            var asm = typeof(CreateCustomer).Assembly;
            var type = asm.GetType(typeP);

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var ser = new DataContractJsonSerializer(type);
            var res = ser.ReadObject(ms) as object;
            ms.Close();

            Convert.ChangeType(res, type);
            return res;
        }

        
    }
}
