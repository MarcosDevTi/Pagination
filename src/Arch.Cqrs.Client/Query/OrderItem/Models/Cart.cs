using System.Collections.Generic;

namespace Arch.Cqrs.Client.Query.OrderItem.Models
{
    public class Cart
    {
        public IReadOnlyList<OrderItemIndex> OrderItens { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
