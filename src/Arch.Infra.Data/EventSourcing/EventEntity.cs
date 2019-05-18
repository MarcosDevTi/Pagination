using System;
using System.Globalization;

namespace Arch.Infra.Data.EventSourcing
{
    public class EventEntity
    {
        public string When { get; set; }
        public string Action { get; protected set; }
        public Guid AggregateId { get; set; }
        public string Who { get; set; }
    }
}
