using System.Data.Entity.ModelConfiguration;
using Arch.Domain.Models;

namespace Arch.Infra.Data.Maps
{
    public class OrderItemMap : EntityTypeConfiguration<OrderItem>
    {
        public OrderItemMap()
        {
            //HasOne(i => i.Product);
        }
    }
}
