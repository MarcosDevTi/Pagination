using System;
using System.Data.Entity;
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
        private readonly ArchDbContext _architectureContext;

        public CustomerCommandHandler(
            ArchDbContext architectureContext,
            IDomainNotification notifications,
            IEventRepository eventRepository) : base(architectureContext, notifications, eventRepository)
        {
            _architectureContext = architectureContext;
        }

        public void Handle(CreateCustomer command)
        {
            ValidateCommand(command);

            var customer = Mapper.Map<Domain.Models.Customer>(command);


            if (Db().Any(x => x.Email == customer.Email))
            {
                AddNotification(new DomainNotification(
                    command.Action, "The customer e-mail has already been taken."));
                return;
            }

            Db().Add(customer);
            command.AggregateId = customer.Id;
            Commit(Mapper.Map<CustomerCreated>(command));
        }

        public void Handle(UpdateCustomer command)
        {
            //validate command
            //convert obj
            //exists

            ValidateCommand(command);
            command.AggregateId = command.Id;

            var customer = Mapper.Map<Domain.Models.Customer>(command);
            var existingCustomer = Db().Any(x => x.Email == customer.Email && x.Id != command.Id);

            if (existingCustomer)
            {
                AddNotification(new DomainNotification(command.Action, "The customer e-mail has already been taken."));
            }

            _architectureContext.Entry(customer).State = EntityState.Modified;
            var teste = Mapper.Map<CustomerUpdated>(command);

            Commit(Mapper.Map<CustomerUpdated>(command));
        }

        public void Handle(DeleteCustomer command)
        {
            ValidateCommand(command);

            Db().Remove(GetById(command.Id));
            Commit(new CustomerDeleted(command.Id));
        }

        private Domain.Models.Customer GetById(Guid id) => Db().Find(id);
    }
}
