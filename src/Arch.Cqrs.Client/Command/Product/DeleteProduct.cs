using System;
using Arch.Cqrs.Client.Command.Product.Validation;

namespace Arch.Cqrs.Client.Command.Product
{
    public class DeleteProduct : ProductCommand
    {
        public DeleteProduct(Guid id)
        {
            Id = id;
        }
    }
}
