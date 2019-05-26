using Arch.Cqrs.Client.AutoMapper;
using Arch.Infra.Shared.Grid;
using AutoMapper;
using System;

namespace Arch.Cqrs.Client.Query.Customer.Models
{
    public class CustomerIndex : ICustomMapper

    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }


        public bool Especial { get; set; }
        public void Map(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Domain.Models.Customer, CustomerIndex>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.FirstName))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.EmailAddress));



            cfg.CreateMap<CustomerIndex, Domain.Models.Customer>();
            //.ConstructUsing(x => new Domain.Models.Customer(
            //    x.FirstName, x.LastName, x.Email, x.BirthDate));

        }
    }

    public class CustomerIndexMapGrid : GridFluent<CustomerIndex>
    {
        public override void Configuration(GridFluent<CustomerIndex> builder)
        {
            builder.DisplayName(_ => _.Name, "Prémon");
        }
    }
}
