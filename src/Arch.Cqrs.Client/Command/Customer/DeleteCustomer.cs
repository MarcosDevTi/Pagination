using System;
using Arch.Cqrs.Client.Command.Customer.Validation;

namespace Arch.Cqrs.Client.Command.Customer
{
    public class DeleteCustomer : CustomerCommand
    {
        public DeleteCustomer(Guid id)
        {
            Id = id;
        }
        public override bool IsValid()
        {
            ValidationResult = new DeleteCustomerValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
