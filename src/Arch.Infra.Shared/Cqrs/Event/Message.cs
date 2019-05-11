using System;
using Arch.Infra.Shared.Cqrs.Command;

namespace Arch.Infra.Shared.Cqrs.Event
{
    public class Message : ICommand
    {
        public Message()
        {
            MessageType = GetType().Name;

        }

        public string MessageType { get; protected set; }
        public Guid AggregateId { get; protected set; }
    }
}
