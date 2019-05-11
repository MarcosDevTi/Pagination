namespace Arch.Cqrs.Client.Command.Product.Validation
{
    public class DeleteProductValidation : ProductCommandValidation<DeleteProduct>
    {
        public DeleteProductValidation()
        {
            ValidateId();
        }
    }
}
