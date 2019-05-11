using System;
using System.Linq;
using Arch.Cqrs.Client.Command.Customer;
using Arch.Cqrs.Client.Event.Customer;
using Arch.Domain.Core.DomainNotifications;
using Arch.Domain.Event;
using Arch.Infra.Data;
using Arch.Infra.Shared.Cqrs.Command;
using AutoMapper;

namespace Arch.Cqrs.Handlers.Customer
{
    public class CustomerCommandHandler : CommandHandler<Domain.Models.Customer>,
        ICommandHandler<CreateCustomer>,
        ICommandHandler<UpdateCustomer>,
        ICommandHandler<DeleteCustomer>
    {
        public CustomerCommandHandler(
            ArchDbContext architectureContext,
            IDomainNotification notifications,
            IEventRepository eventRepository) : base(architectureContext, notifications, eventRepository)
        {
        }

        public void Handle(CreateCustomer command)
        {
            ValidateCommand(command);

            var customer = Mapper.Map<Domain.Models.Customer>(command);


            if (Db().Any(x => x.Email == customer.Email))
            {
                AddNotification(new DomainNotification(
                    command.MessageType, "The customer e-mail has already been taken."));
                return;
            }

            Db().Add(customer);

            Commit(Mapper.Map<CustomerCreated>(command));
        }

        public void Handle(UpdateCustomer command)
        {
            //validate command
            //convert obj
            //exists

            ValidateCommand(command);

            var customer = Mapper.Map<Domain.Models.Customer>(command);
            var existingCustomer = Db().Any(x => x.Email == customer.Email);

            if (existingCustomer)
            {
                AddNotification(new DomainNotification(command.MessageType, "The customer e-mail has already been taken."));
            }

            //Db().Update(customer);

            Commit(Mapper.Map<CustomerUpdated>(command));
        }

        public void Handle(DeleteCustomer command)
        {
            ValidateCommand(command);

            Db().Remove(GetById(command.Id.Value));
            Commit(new CustomerDeleted(command.Id.Value));
        }

        private Domain.Models.Customer GetById(Guid id) => Db().Find(id);
    }
}
