using System;
using Arch.Cqrs.Client.AutoMapper;
using Arch.Cqrs.Client.Command.Customer;
using AutoMapper;

namespace Arch.Cqrs.Client.Event.Customer
{
    public class CustomerUpdated : Infra.Shared.Cqrs.Event.Event, ICustomMapper
    {
        public CustomerUpdated() { }
        public CustomerUpdated(
            Guid iggregateId, string firstName, string lastName, string email, string birthDate,
            string street, string number, string city, string zipCode)
        {
            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            BirthDate = birthDate;
            Street = street;
            Number = number;
            City = city;
            ZipCode = zipCode;
            AggregateId = iggregateId;
        }
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string BirthDate { get; set; }

        public string Street { get; set; }
        public string Number { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public void Map(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<UpdateCustomer, CustomerUpdated>()
                .ConstructUsing(x => new CustomerUpdated(
                    x.Id,
                x.FirstName,
                    x.LastName,
                    x.Email,
                    x.BirthDate.ToString(),
                    x.Street,
                    x.Number,
                    x.City,
                    x.ZipCode))
                .IgnoreAllPropertiesWithAnInaccessibleSetter();
        }
    }
}
