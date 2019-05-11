using System;
using Arch.Infra.Shared.Cqrs.Event;
using FluentValidation.Results;

namespace Arch.Infra.Shared.Cqrs.Command
{
    public abstract class Command : Message
    {
        public DateTime Timestamp { get; private set; }
        public ValidationResult ValidationResult { get; set; }
        protected Command() => Timestamp = DateTime.Now;
        public abstract bool IsValid();
    }
}
