namespace Arch.Cqrs.Client.Command.Product.Validation
{
    public class CreateProductValidation : ProductCommandValidation<CreateProduct>
    {
        public CreateProductValidation()
        {
            ValidateName();
            ValidateDescription();
            ValidatePrice();
        }
    }
}
