namespace Arch.Cqrs.Client.Command.Customer.Validation
{
    public class DeleteCustomerValidation : CustomerCommandValidation<DeleteCustomer>
    {
        public DeleteCustomerValidation()
        {
            ValidateId();
        }
    }
}
