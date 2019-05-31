using System;

namespace Arch.Cqrs.Client.Command.Cart
{
    public class AddItemCart : Infra.Shared.Cqrs.Command.Command
    {
        public AddItemCart(Guid orderItemId, int value)
        {
            Value = value;
            OrderItemId = orderItemId;
        }

        public int Value { get; private set; }

        public Guid OrderItemId { get; private set; }
    }
}
