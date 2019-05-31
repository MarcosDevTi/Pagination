using System;

namespace Arch.Cqrs.Client.Command.Order
{
    public class AddProductToCart : Infra.Shared.Cqrs.Command.Command
    {
        public AddProductToCart(Guid productId, Guid userId)
        {
            ProductId = productId;
            UserId = userId;
        }

        public Guid ProductId { get; private set; }
        public Guid UserId { get; private set; }
    }
}
