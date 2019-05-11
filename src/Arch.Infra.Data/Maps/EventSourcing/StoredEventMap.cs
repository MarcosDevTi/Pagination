using System.Data.Entity.ModelConfiguration;
using Arch.Domain.Core.Event;

namespace Arch.Infra.Data.Maps.EventSourcing
{
    public class StoredEventMap : EntityTypeConfiguration<StoredEvent>
    {
        public StoredEventMap()
        {
            Property(c => c.Timestamp)
                .HasColumnName("CreationDate");

            Property(c => c.MessageType)
                .HasColumnName("Action")
                .HasColumnType("varchar(100)");
        }
    }
}
