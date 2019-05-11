using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.Infra.Shared.Cqrs;
using Arch.Infra.Shared.Cqrs.Command;
using Arch.Infra.Shared.Cqrs.Query;
using SimpleInjector;

namespace Arch.Infra.IoC
{
    public class Processor: IProcessor
    {
        private readonly Container _container;

        public Processor(Container container) =>
            _container = container;

        public void Send<TCommand>(TCommand command) where TCommand : Command =>
            GetHandle(typeof(ICommandHandler<>), command.GetType()).Handle((dynamic)command);

        public TResult Get<TResult>(IQuery<TResult> query) =>
            GetHandle(typeof(IQueryHandler<,>), query.GetType(), typeof(TResult)).Handle((dynamic)query);

        private dynamic GetHandle(Type handle, params Type[] types) =>
            _container.GetInstance(handle.MakeGenericType(types));
    }
}
