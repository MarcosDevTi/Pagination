using Arch.Domain.Core;

namespace Arch.Domain.Models
{
    public class OrderItem : Entity
    {
        public OrderItem()
        {


        }
        public OrderItem(Product product)
        {
            GetId(Id);
            Product = product;
            Qtd = 1;
        }

        public Product Product { get; private set; }
        public int Qtd { get; set; }
    }
}
