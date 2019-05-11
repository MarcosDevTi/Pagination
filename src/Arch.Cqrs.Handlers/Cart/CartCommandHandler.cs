using System.Linq;
using Arch.Cqrs.Client.Command.Cart;
using Arch.Infra.Data;
using Arch.Infra.Shared.Cqrs.Command;

namespace Arch.Cqrs.Handlers.Cart
{
    public class CartCommandHandler :
        ICommandHandler<AddItemCart>
    {
        private readonly ArchDbContext _context;

        public CartCommandHandler(ArchDbContext context)
        {
            _context = context;
        }

        public void Handle(AddItemCart command)
        {
            var orderItem = _context.OrderItems.FirstOrDefault(x => x.Id == command.OrderItemId);
            orderItem.Qtd += command.Value;
            _context.OrderItems.Add(orderItem);
            _context.SaveChanges();
        }
    }
}
