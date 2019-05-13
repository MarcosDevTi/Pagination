using System.Data.Entity.ModelConfiguration;
using Arch.Domain.Core.Event;

namespace Arch.Infra.Data.Maps.EventSourcing
{
    public class StoredEventMap : EntityTypeConfiguration<StoredEvent>
    {
        public StoredEventMap()
        {
            Property(c => c.When)
                .HasColumnName("When");

            Property(c => c.Action)
                .HasColumnName("Action")
                .HasColumnType("varchar(100)");
        }
    }
}
