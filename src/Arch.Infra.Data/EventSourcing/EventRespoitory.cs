using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Arch.Domain.Core;
using Arch.Domain.Core.Event;
using Arch.Domain.Event;
using Arch.Infra.Shared.Cqrs.Event;
using Newtonsoft.Json;

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

        public void Save(Event @event, object data)
        {
            
            if (data != null)
            {
                var props = data.GetType().GetProperties().Select(x =>
                    new KeyValuePair<string, object>(x.Name, x.GetValue(data, new object[] { })));
                foreach (var prop in props)
                {
                    var getPropSave = @event.GetType().GetProperty(prop.Key)?.GetValue(@event, null);
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

        //private object GetValueProp(string)

        public IReadOnlyList<CustomerHistoryData> GetAllHistories()
        {
            //var aa = JsonConvert.SerializeObject("");
            return CustomerHistoryHelper.ToJavaScriptCustomerHistory(_eventSourcingContext.StoredEvent.ToList());
        }

        public Event GetLastEvent(Guid aggregateId)
        {
            return _eventSourcingContext.StoredEvent.Where(a => a.AggregateId == aggregateId).OrderByDescending(x => x.Data).FirstOrDefault();
        }
    }
}
