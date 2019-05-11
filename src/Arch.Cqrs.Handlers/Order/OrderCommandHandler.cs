using System.Linq;
using Arch.Cqrs.Client.Command.Order;
using Arch.Cqrs.Client.Event.Order;
using Arch.Domain.Core;
using Arch.Domain.Core.DomainNotifications;
using Arch.Domain.Event;
using Arch.Domain.Models;
using Arch.Infra.Data;
using Arch.Infra.Shared.Cqrs.Command;

namespace Arch.Cqrs.Handlers.Order
{
    public class OrderCommandHandler : CommandHandler<Domain.Models.Order>,
        ICommandHandler<AddProductToCart>
    {
        private readonly ArchDbContext _architectureContext;
        private readonly IUser _user;

        public OrderCommandHandler(ArchDbContext architectureContext, IDomainNotification notifications, IEventRepository eventRepository, IUser user) : base(architectureContext, notifications, eventRepository)
        {
            _architectureContext = architectureContext;
            _user = user;
        }
        public void Handle(AddProductToCart command)
        {
            var product = _architectureContext.Products.Find(command.ProductId);

            var order = _architectureContext.Orders
                     //.Include(x => x.OrderItems).ThenInclude(c => c.Product)
                     .FirstOrDefault(x => x.Customer.Id == _user.UserId() && x.Closed == false);

            if (order == null)
            {
                var customer = _architectureContext.Customers.Find(command.UserId);

                order = new Domain.Models.Order(customer);
                var orderItem = new OrderItem(product);
                order.OrderItems.Add(orderItem);
                Db().Add(order);
                Commit(new ProductAddedToCart(order.Id, orderItem.Id, command.ProductId));
            }
            else
            {
                var orderItem = order.OrderItems.FirstOrDefault(x => x.Product.Id == command.ProductId);
                order.AddOrUpdateItem(orderItem, product);

                //Db().Update(order);
                Commit(new ProductAddedToCart(order.Id, order.Id, command.ProductId));
            }
        }

    }
}
