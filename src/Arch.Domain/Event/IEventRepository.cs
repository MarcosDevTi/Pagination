using System;
using System.Collections.Generic;

namespace Arch.Domain.Event
{
    public interface IEventRepository
    {
        void Save(Infra.Shared.Cqrs.Event.Event @event, object data);
        IReadOnlyList<CustomerHistoryData> GetAllHistories();
        Infra.Shared.Cqrs.Event.Event GetLastEvent(Guid aggregateId);
        IEnumerable<Infra.Shared.Cqrs.Event.Event> GetAllEvents(Guid aggregateId);
    }
}
