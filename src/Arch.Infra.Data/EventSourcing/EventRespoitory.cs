using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Arch.Cqrs.Client.Event.Customer;
using Arch.Domain.Core;
using Arch.Domain.Core.Event;
using Arch.Domain.Event;
using Arch.Infra.Data.Maps;
using Arch.Infra.Shared.Cqrs.Event;
using Arch.Infra.Shared.EventSourcing;

namespace Arch.Infra.Data.EventSourcing
{
    public class EventRespoitory : IEventRepository
    {
        private readonly EventSourcingContext _eventSourcingContext;
        private readonly IUser _user;

        public EventRespoitory(EventSourcingContext eventSourcingContext, IUser user)
        {
            _eventSourcingContext = eventSourcingContext;
            _user = user;
        }
        //public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        //{
        //    var interfaceTypes = givenType.GetInterfaces();

        //    foreach (var it in interfaceTypes)
        //    {
        //        if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
        //            return true;
        //    }

        //    if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
        //        return true;

        //    Type baseType = givenType.BaseType;
        //    if (baseType == null) return false;

        //    return IsAssignableToGenericType(baseType, genericType);
        //}
        public SourceFluent<T> Cast<T>(object o) where T: class
        {
            return (SourceFluent<T>)o;
        }

        public List<MemberInfo> GetAttributs(object obj)
        {
            var tipoGen = typeof(SourceFluent<>).MakeGenericType(obj.GetType());

            var target = obj.GetType().Assembly;
            var assemblies = target.GetReferencedAssemblies()
                .Select(System.Reflection.Assembly.Load).ToList();
            assemblies.Add(target);

            var map = assemblies.SelectMany(_ => _.GetExportedTypes())
                .FirstOrDefault(_ => tipoGen.IsAssignableFrom(_));

            var tipo = (dynamic)Activator.CreateInstance(map);
            tipo.Configuration(tipo);

            return (List<MemberInfo>)tipo.Members;
        }


        public void Save(Event @event, object data)
        {
            //var ignoredMembersList = GetAttributs(@event).Select(_=>_.Name);
            var listIgnoredMembers = GetAttributs(@event).Select(_=>_.Name);

            var all = _eventSourcingContext.StoredEvent.Where(x => x.AggregateId == @event.AggregateId).OrderByDescending(x => x.Data).ToArray();

            if (data != null)
            {
                var props = data.GetType().GetProperties().Select(x =>
                        new KeyValuePair<string, object>(x.Name, x.GetValue(data, new object[] { })))
                    .Where(x => !listIgnoredMembers.Contains(x.Key));


                foreach (var prop in props)
                {
                    var getPropSave = @event.GetType().GetProperty(prop.Key)?.GetValue(@event, null);
                    if ((prop.Value == null && getPropSave != null) && LastChangeIsEqual(prop.Key, all))
                    {
                        var propEvent = @event.GetType().GetProperty(prop.Key, BindingFlags.Public | BindingFlags.Instance);
                        propEvent.SetValue(@event, null, null);
                    }

                    if ((prop.Value?.ToString() == getPropSave?.ToString() && (prop.Value != null && getPropSave != null) && prop.Key != "AggregateId" && prop.Key != "Who" && prop.Key != "id" ))
                    {
                        var propEvent = @event.GetType().GetProperty(prop.Key, BindingFlags.Public | BindingFlags.Instance);
                        if (null != propEvent && propEvent.CanWrite)
                        {
                            propEvent.SetValue(@event, null, null);
                        } 
                    }
                }
            }
            
            _eventSourcingContext.StoredEvent.Add(new StoredEvent(@event, _user.Name));
            _eventSourcingContext.SaveChanges();
        }

        private bool LastChangeIsEqual(string prop, StoredEvent[] events)
        {
            var list = events.Select(x => x.Data);
            foreach (var item in list)
            {
                var propEvent = item.GetType().GetProperty(prop, BindingFlags.Public | BindingFlags.Instance);
                if (false) return true;
            }

            return false;
        }

        public IReadOnlyList<CustomerHistoryData> GetAllHistories()
        {
            //var aa = JsonConvert.SerializeObject("");
            return CustomerHistoryHelper.ToJavaScriptCustomerHistory(_eventSourcingContext.StoredEvent.ToList());
        }

        public Event GetLastEvent(Guid aggregateId)
        {
            return _eventSourcingContext.StoredEvent.Where(a => a.AggregateId == aggregateId).OrderByDescending(x => x.Data).FirstOrDefault();
        }

        public IEnumerable<Event> GetAllEvents(Guid aggregateId)
        {
            return _eventSourcingContext.StoredEvent.Where(a => a.AggregateId == aggregateId).ToList();
        }
    }
}
