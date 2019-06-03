using Arch.Cqrs.Client.Command.Customer.Generics;
using Arch.Domain.Core;
using Arch.Domain.Core.DomainNotifications;
using Arch.Infra.Data;
using Arch.Infra.Shared.Cqrs.Command;
using Arch.Infra.Shared.EventSourcing;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Arch.Cqrs.Handlers.Generic
{
    public class ListCommandHandler :
        ICommandHandler<ListUpdate>
    {
        private readonly ArchDbContext _architectureContext;
        private readonly EventSourcingContext _eventSourcingContext;
        private readonly IDomainNotification _notifications;
        public ListCommandHandler(
            ArchDbContext architectureContext,
            EventSourcingContext eventSourcingContext,
            IDomainNotification notifications)
        {
            _architectureContext = architectureContext;
            _eventSourcingContext = eventSourcingContext;
            _notifications = notifications;
        }

        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
        public void Handle(ListUpdate command)
        {
            var typeModel = typeof(Domain.Models.Customer).Assembly.GetType(command.AssemblyModel);
            var objDatabase = _architectureContext.Set(typeModel).Find(command.Id);
            var objBeforeUpdate = ((Entity)objDatabase).Clone();

            PropertyInfo prop = objDatabase.GetType().GetProperty(command.Key, BindingFlags.Public | BindingFlags.Instance);

            var proOrig = prop.GetValue(objDatabase);

            if (proOrig.ToString() != command.Value.ToString())
            {
                if (null != prop && prop.CanWrite)
                {
                    if (prop.GetMethod.ReturnType == typeof(DateTime))
                    {
                        command.Value = Convert.ToDateTime(command.Value);
                    };

                    if (prop.GetMethod.ReturnType == typeof(int))
                    {
                        command.Value = Convert.ToInt32(command.Value);
                    };

                    prop.SetValue(objDatabase, command.Value, null);
                }

                var action = _architectureContext.Update((Entity)objDatabase);
                Commit((Entity)objDatabase, action, (Entity)objBeforeUpdate);
            }
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


        private void Commit(Entity entity, string action, Entity lastEntity = null)
        {
            if (_notifications.HasNotifications()) return;
            if (_architectureContext.SaveChanges() > 0)
            {
                var eventEntity = new EventEntity(action, entity, "Marcos", lastEntity);

                _eventSourcingContext.EventEntities.Add(eventEntity);
                _eventSourcingContext.SaveChanges();
            }
            else
                AddNotification(new DomainNotification("Commit", "We had a problem during saving your data."));
        }

        private void AddNotification(DomainNotification notification)
        {
            _notifications.Add(notification);
        }
    }
}
