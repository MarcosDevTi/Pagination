using System.Collections.Generic;
using Arch.Cqrs.Client.Query.OrderItem.Models;
using Arch.Infra.Shared.Cqrs.Query;

namespace Arch.Cqrs.Client.Query.OrderItem.Queries
{
    public class GetOrderItensIndex : IQuery<IReadOnlyList<OrderItemIndex>>
    {
    }
}
