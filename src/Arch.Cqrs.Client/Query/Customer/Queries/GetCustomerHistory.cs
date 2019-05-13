using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.Cqrs.Client.Query.Customer.Models;
using Arch.Domain.Core.Event;
using Arch.Infra.Shared.Cqrs.Query;

namespace Arch.Cqrs.Client.Query.Customer.Queries
{
    public class GetCustomerHistory: IQuery<IReadOnlyList<object>>
    {
        public Guid AggregateId { get; set; }
    }
}
