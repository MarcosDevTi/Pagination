using System.Data.Entity.ModelConfiguration;
using Arch.Domain.Models;

namespace Arch.Infra.Data.Maps
{
    public class OrderMap : EntityTypeConfiguration<Order>
    {
        public OrderMap()
        {
           // Property(_ => _.CreatedDate).HasColumnType("datetime2");
            HasMany(o => o.OrderItems);
        }
        
    }
}
