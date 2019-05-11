using System;

namespace Arch.Cqrs.Client.Command.Order
{
    public class CreateOrder
    {
        public Guid CustomerId { get; set; }
    }
}
