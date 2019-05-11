using System;

namespace Arch.Cqrs.Client.Event.Customer
{
    public class CustomerDeleted : Infra.Shared.Cqrs.Event.Event
    {
        public CustomerDeleted(Guid id)
        {
            Id = id;
            AggregateId = id;
        }

        public Guid Id { get; set; }
    }
}
