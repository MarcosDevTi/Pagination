using Arch.Infra.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arch.Cqrs.Handlers.Generic
{
    public class Repository<T> where T: class
    {
        public IEnumerable<T> GetAll(string order)
        {
            var context = new ArchDbContext();
            return context.Set<T>().ToList().OrderBy(_ => _.GetType().GetProperty(order).GetValue(_, null));
        }
    }
}
