using System.Data.Entity.ModelConfiguration;
using Arch.Domain.Models;

namespace Arch.Infra.Data.Maps
{
    public class OrderItemMap : EntityTypeConfiguration<OrderItem>
    {
        public OrderItemMap()
        {
            //Property(_ => _.CreatedDate).HasColumnType("datetime2");
            //HasOne(i => i.Product);
        }
    }
}
