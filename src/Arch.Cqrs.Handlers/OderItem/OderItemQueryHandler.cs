using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Arch.Cqrs.Client.Query.OrderItem.Models;
using Arch.Cqrs.Client.Query.OrderItem.Queries;
using Arch.Domain.Core;
using Arch.Infra.Data;
using Arch.Infra.Shared.Cqrs.Query;

namespace Arch.Cqrs.Handlers.OderItem
{
    public class OderItemQueryHandler :
        IQueryHandler<GetOrderItensIndex, IReadOnlyList<OrderItemIndex>>,
        IQueryHandler<GetCart, Client.Query.OrderItem.Models.Cart>
    {
        private readonly ArchDbContext _architectureContext;

        public OderItemQueryHandler(ArchDbContext architectureContext)
        {
            _architectureContext = architectureContext;
        }

        public IReadOnlyList<OrderItemIndex> Handle(GetOrderItensIndex query)
        {
            var ordersItens = from oi in _architectureContext.Orders
                    .Include(x => x.Customer)
                    .Include(x => x.OrderItems)
                              from o in oi.OrderItems
                              //where oi.Customer.Id == _user.UserId()
                              select o;

            return ordersItens.Include(x => x.Product)
                .Select(x => new OrderItemIndex
                {
                    Id = x.Id,
                    Product = $"{x.Product.Name}, Price:{x.Product.Price}",
                    Qtd = x.Qtd
                }).ToList();
        }

        public Client.Query.OrderItem.Models.Cart Handle(GetCart query)
        {
            var ordersItens = from oi in _architectureContext.Orders
                    .Include(x => x.Customer)
                    .Include(x => x.OrderItems)
                              from o in oi.OrderItems
                              //where oi.Customer.Id == _user.UserId()
                              select o;

            var result = ordersItens.Include(x => x.Product)
                .Select(x => new OrderItemIndex
                {
                    Id = x.Id,
                    Product = $"{x.Product.Name}, Price:{x.Product.Price}",
                    Qtd = x.Qtd
                }).ToList();

            return new Client.Query.OrderItem.Models.Cart
            {
                OrderItens = result,
                TotalPrice = ordersItens.Sum(x => x.Product.Price * x.Qtd)
            };
        }
    }
}
