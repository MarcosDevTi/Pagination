using System;

namespace Arch.Cqrs.Client.Event.OrderItem
{
    public class OrderItemUpdated : Infra.Shared.Cqrs.Event.Event
    {
        public OrderItemUpdated(Guid id, Guid productId, int qtd)
        {
            Id = id;
            ProductId = productId;
            Qtd = qtd;
        }
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; }
        public int Qtd { get; private set; }
    }
}
