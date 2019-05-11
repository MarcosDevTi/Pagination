using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.Domain;
using Arch.Domain.Models;

namespace Arch.Infra.Data
{
    public class ArchDbContext: DbContext
    {
        public ArchDbContext():base("DefaultConnection")
        {
            
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
           
        //}


    }
}
