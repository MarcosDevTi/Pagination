using System;

namespace Arch.Cqrs.Client.Command.OrderItem
{
    public class DeleteOrderItem : Infra.Shared.Cqrs.Command.Command
    {
        public DeleteOrderItem(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }
    }
}
