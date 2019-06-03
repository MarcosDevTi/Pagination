using Arch.Cqrs.Client.Query.Customer.Models;
using Arch.Infra.Shared.Cqrs.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.Cqrs.Client.Query.Customer.Queries
{
    public class GetCustomersCsv: IQuery<IEnumerable<CustomerIndex>>
    {
        public string[] Properties { get; set; }
        public string Order { get; set; }
    }
}
