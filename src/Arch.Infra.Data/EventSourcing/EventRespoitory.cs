using System.Collections.Generic;
using System.Linq;
using Arch.Domain.Core;
using Arch.Domain.Core.Event;
using Arch.Domain.Event;
using Arch.Infra.Shared.Cqrs.Event;

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

        public void Save(Event @event)
        {
            _eventSourcingContext.StoredEvent.Add(new StoredEvent(@event, _user.Name));
            _eventSourcingContext.SaveChanges();
        }

        public IReadOnlyList<CustomerHistoryData> GetAllHistories()
        {
            return CustomerHistoryHelper.ToJavaScriptCustomerHistory(_eventSourcingContext.StoredEvent.ToList());
        }
    }
}
