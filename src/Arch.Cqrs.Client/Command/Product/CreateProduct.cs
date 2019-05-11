using Arch.Cqrs.Client.Command.Product.Validation;
using FluentValidation.Results;

namespace Arch.Cqrs.Client.Command.Product
{
    public class CreateProduct : ProductCommand
    {
        public CreateProduct(){}

        protected CreateProduct(string name, string description, decimal price)
        {
            Name = name;
            Description = description;
            Price = price;
        }

        public override bool IsValid()
        {
            ValidationResult = new CreateProductValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
