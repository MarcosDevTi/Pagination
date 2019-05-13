using System;
using Arch.Infra.Shared.Cqrs.Command;

namespace Arch.Infra.Shared.Cqrs.Event
{
    public class Message: ICommand
    {
        public Message()
        {
            Action = GetType().Name;

        }

        public string Action { get; protected set; }
        public Guid AggregateId { get; set; }
        public string Who { get; set; }
    }
}
