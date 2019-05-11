using Newtonsoft.Json;
using System;

namespace Arch.Domain.Core.Event
{
    public class StoredEvent : Infra.Shared.Cqrs.Event.Event
    {
        protected StoredEvent()
        {

        }
        public StoredEvent(Infra.Shared.Cqrs.Event.Event theEvent, string user)
        {
            var jsonEvent = JsonConvert.SerializeObject(theEvent);
            Id = Guid.NewGuid();
            AggregateId = theEvent.AggregateId;
            MessageType = theEvent.MessageType;
            Data = jsonEvent;
            User = user;
        }

        // EF Constructor
        protected StoredEvent(Infra.Shared.Cqrs.Event.Event @event) { }

        public Guid Id { get; private set; }

        public string Data { get; private set; }

        public string User { get; private set; }
    }
}
