using System.Data.Entity.ModelConfiguration;
using Arch.Domain.Models;

namespace Arch.Infra.Data.Maps
{
    public class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
            Property(p => p.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(80);
            Property(p => p.Description)
                .HasColumnName("Description")
                .IsRequired()
                .HasMaxLength(150);
            Property(p => p.Price)
                .HasColumnName("Price")
                .IsRequired();
        }
    }
}
