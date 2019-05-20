using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.Domain.Core.Event;
using Arch.Infra.Data.EventSourcing;
using Arch.Infra.Shared.EventSourcing;

namespace Arch.Infra.Data
{
    public class EventSourcingContext : DbContext
    {
        public EventSourcingContext() : base("DefaultConnection")
        {
            
        }
        public DbSet<StoredEvent> StoredEvent { get; set; }
        public DbSet<EventEntity> EventEntities { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.ApplyConfiguration(new StoredEventMap());

        //    base.OnModelCreating(modelBuilder);
        //}
    }
}
