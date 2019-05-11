using Arch.Infra.Shared.Cqrs.Query;

namespace Arch.Infra.Shared.Cqrs
{
    public interface IProcessor
    {
        void Send<TCommand>(TCommand command) where TCommand : Command.Command;
        TResult Get<TResult>(IQuery<TResult> query);
    }
}
