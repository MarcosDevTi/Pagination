using System.Data.Entity.ModelConfiguration;
using Arch.Domain.Models;

namespace Arch.Infra.Data.Maps
{
    public class OrderMap : EntityTypeConfiguration<Order>
    {
        public OrderMap()
        {
            HasMany(o => o.OrderItems);
        }
        
    }
}
