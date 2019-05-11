using System;

namespace Arch.Cqrs.Client.Query.OrderItem.Models
{
    public class OrderItemIndex
    {
        public Guid Id { get; set; }
        public string Product { get; set; }
        public int Qtd { get; set; }
    }
}
