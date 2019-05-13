using System;
using Arch.Cqrs.Client.AutoMapper;
using Arch.Domain.ValueObjects;
using AutoMapper;

namespace Arch.Cqrs.Client.Query.Customer.Models
{
    public class CustomerIndex : ICustomMapper

    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }

        public string Street { get; set; }
        public string Number { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }

        public bool Especial { get; set; }
        public void Map(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Domain.Models.Customer, CustomerIndex>()
                .ForMember(d => d.Street, o => o.MapFrom(s => s.Address.Street))
                .ForMember(d => d.Number, o => o.MapFrom(s => s.Address.Number))
                .ForMember(d => d.City, o => o.MapFrom(s => s.Address.City))
                .ForMember(d => d.ZipCode, o => o.MapFrom(s => s.Address.ZipCode));

            cfg.CreateMap<CustomerIndex, Domain.Models.Customer>()
                .ConstructUsing(x => new Domain.Models.Customer(
                    x.FirstName, x.LastName, x.Email, x.BirthDate, new Address(x.Street, x.Number, x.City, x.ZipCode)));

        }
    }
}
