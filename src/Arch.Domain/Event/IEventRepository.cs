using System.Collections.Generic;

namespace Arch.Domain.Event
{
    public interface IEventRepository
    {
        void Save(Infra.Shared.Cqrs.Event.Event @event);
        IReadOnlyList<CustomerHistoryData> GetAllHistories();
    }
}
