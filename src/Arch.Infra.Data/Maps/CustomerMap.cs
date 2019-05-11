using System.Data.Entity.ModelConfiguration;
using Arch.Domain.Models;

namespace Arch.Infra.Data.Maps
{
    public class CustomerMap : EntityTypeConfiguration<Customer>
    {
        public CustomerMap()
        {
            //builder.OwnsOne(s => s.Name, cm =>
            //{
            //    cm.Property(n => n.FirstName)
            //        .HasColumnName("FirstName")
            //        .IsRequired()
            //        .HasMaxLength(40);
            //    cm.Property(c => c.LastName)
            //        .HasColumnName("LastName")
            //        .IsRequired()
            //        .HasMaxLength(120);
            //});

            //builder.OwnsOne(s => s.Email, cm =>
            //{
            //    cm.Property(e => e.EmailAddress)
            //        .HasColumnName("Email")
            //        .IsRequired()
            //        .HasMaxLength(150);
            //});
            //builder.OwnsOne(s => s.Address, cm =>
            //{
            //    cm.Property(c => c.Street)
            //        .HasColumnName("Street")
            //        .HasMaxLength(120);
            //    cm.Property(c => c.Number)
            //        .HasColumnName("Number")
            //        .HasMaxLength(20);
            //    cm.Property(c => c.City)
            //        .HasColumnName("City")
            //        .HasMaxLength(100);
            //    cm.Property(c => c.ZipCode)
            //        .HasColumnName("ZipCode")
            //        .HasMaxLength(30);
            //});

            //builder.HasMany(o => o.Orders)
            //    .WithOne(c => c.Customer);
        }
    }
}
