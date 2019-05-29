using Arch.Cqrs.Client.Query.Product.Models;
using Arch.Cqrs.Client.Query.Product.Queries;
using Arch.Infra.Data;
using Arch.Infra.Shared.Cqrs.Query;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Collections.Generic;
using System.Linq;

namespace Arch.Cqrs.Handlers.Product
{
    public class ProductQueryHandler :
        IQueryHandler<GetProductsIndex, IReadOnlyList<ProductIndex>>,
        IQueryHandler<GetProductDetails, ProductDetails>,
        IQueryHandler<GetProductsDropDownList, IReadOnlyList<ProductDropDownItem>>
    {
        private readonly ArchDbContext _architectureContext;

        public ProductQueryHandler(ArchDbContext architectureContext)
        {
            _architectureContext = architectureContext;
        }

        public IReadOnlyList<ProductIndex> Handle(GetProductsIndex query)
        {
            return _architectureContext.Products
                .ProjectTo<ProductIndex>().ToList();
        }


        public ProductDetails Handle(GetProductDetails query)
        {
            return Mapper.Map<ProductDetails>(
                _architectureContext.Products.Find(query.Id));
        }

        public IReadOnlyList<ProductDropDownItem> Handle(GetProductsDropDownList query)
        {
            return _architectureContext.Products.Select(_ => new ProductDropDownItem
            {
                Id = _.Id,
                Name = _.Name
            }).ToList();
        }
    }
}
