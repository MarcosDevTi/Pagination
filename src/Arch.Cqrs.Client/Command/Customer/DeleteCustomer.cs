using Arch.Cqrs.Client.Command.Customer.Validation;
using System;

namespace Arch.Cqrs.Client.Command.Customer
{
    public class DeleteCustomer : CustomerCommand
    {
        public DeleteCustomer(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}
