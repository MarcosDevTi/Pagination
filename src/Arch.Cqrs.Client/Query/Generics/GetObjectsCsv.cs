using Arch.Infra.Shared.Cqrs.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.Cqrs.Client.Query.Generics
{
    public class GetObjectsCsv: IQuery<IEnumerable<object>>
    {
        public string Properties { get; set; }
        public string Order { get; set; }
        public string ViewModelAssemblyFullName { get; set; }
        public string ModelAssemblyFullName { get; set; }
    }
}
