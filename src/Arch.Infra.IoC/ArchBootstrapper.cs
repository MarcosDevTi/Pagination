using Arch.Cqrs.Client.AutoMapper;
using Arch.Cqrs.Client.Command.Customer;
using Arch.Cqrs.Handlers.Customer;
using Arch.Domain.Core.DomainNotifications;
using Arch.Domain.Event;
using Arch.Infra.Data;
using Arch.Infra.Data.EventSourcing;
using Arch.Infra.Shared.Cqrs;
using Arch.Infra.Shared.Cqrs.Extentions;
using SimpleInjector;

namespace Arch.Infra.IoC
{
    public class ArchBootstrapper
    {
        public static Container MyContainer { get; set; }

        public static void Register(Container container)
        {
            MyContainer = container;
            AutoMapperConfig.Register<CreateCustomer>();

            container.Register<IProcessor, Processor>(Lifestyle.Transient);
            container.Register<ArchDbContext>(Lifestyle.Scoped);
            container.Register<EventSourcingContext>(Lifestyle.Scoped);
            container.AddCqrs<CustomerCommandHandler>();
            container.Register<IDomainNotification, DomainNotificationHandler>(Lifestyle.Scoped);
            container.Register<IEventRepository, EventRespoitory>(Lifestyle.Transient);

        }
    }
}
