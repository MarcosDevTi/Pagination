using System;

namespace Arch.Cqrs.Client.Event.OrderItem
{
    public class OrderItemDeleted : Infra.Shared.Cqrs.Event.Event
    {
        public OrderItemDeleted(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }
}
