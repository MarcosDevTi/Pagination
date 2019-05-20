using System;
using System.Collections.Generic;
using Arch.Domain.Core;

namespace Arch.Domain.Event
{
    public interface IEventRepository
    {
        void Save(Infra.Shared.Cqrs.Event.Event @event, object data);
        void SaveEntity(Entity entity, Entity lastEntity);
        void SaveEntity(object entity);
        IReadOnlyList<CustomerHistoryData> GetAllHistories();
        Infra.Shared.Cqrs.Event.Event GetLastEvent(Guid aggregateId);
        IEnumerable<Infra.Shared.Cqrs.Event.Event> GetAllEvents(Guid aggregateId);
    }
}
