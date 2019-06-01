using Arch.Cqrs.Client.Command.Customer.Generics;
using Arch.Infra.Data;
using Arch.Infra.Shared.Cqrs.Command;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Arch.Cqrs.Handlers.Generic
{
    public class ListCommandHandler :
        ICommandHandler<ListUpdate>
    {
        private readonly ArchDbContext _architectureContext;
        public ListCommandHandler(ArchDbContext architectureContext)
        {
            _architectureContext = architectureContext;
        }
        public void Handle(ListUpdate command)
        {
            var typeModel = typeof(Domain.Models.Customer).Assembly.GetType(command.AssemblyModel);
            var objDatabase = _architectureContext.Set(typeModel).Find(command.Id);
            PropertyInfo prop = objDatabase.GetType().GetProperty(command.Key, BindingFlags.Public | BindingFlags.Instance);

            if (null != prop && prop.CanWrite)
            {
                prop.SetValue(objDatabase, command.Value, null);
            }

            _architectureContext.Entry(objDatabase).State = EntityState.Modified;
            _architectureContext.SaveChanges();
        }

        private static IEnumerable<(PropertyInfo dest, PropertyInfo src)> GetSourceAndDest<T, T2>(PropertyInfo propViewModel)
        {
            var typePair = new TypePair(typeof(T), typeof(T2));
            var target = Mapper.Configuration.GetMapperFunc<T, T2>(typePair).Target;
            var props = (target as Closure)?.Constants.Where(_ => _.GetType() == typeof(PropertyMap));

            return props?.Select(_ =>
                (
                    (PropertyInfo)((dynamic)_).DestinationProperty,
                    (PropertyInfo)((dynamic)_).SourceMember
                )
            ).ToList();
        }
    }
}
