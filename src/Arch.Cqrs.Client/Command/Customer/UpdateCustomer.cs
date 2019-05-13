using System;
using System.Security.Permissions;
using Arch.Cqrs.Client.AutoMapper;
using Arch.Cqrs.Client.Command.Customer.Validation;
using Arch.Domain.ValueObjects;
using AutoMapper;

namespace Arch.Cqrs.Client.Command.Customer
{
    public class UpdateCustomer : CustomerCommand, ICustomMapper
    {
        public UpdateCustomer()
        {

        }
        public Guid Id { get; set; }
        public UpdateAddress UpdateAddress { get; set; }
        public UpdateCustomer(Guid id, string firstName, string lastName, string email, DateTime birthDate, 
            string street, string number, string city, string zipCode)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            BirthDate = birthDate;
            Street = street;
            Number = number;
            City = city;
            ZipCode = zipCode;
            AggregateId = id;
        }

        public override bool IsValid()
        {
            ValidationResult = new UpdateCustomerValidation().Validate(this);
            return ValidationResult.IsValid;
        }

        public void Map(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<UpdateCustomer, Domain.Models.Customer>()
                .ConstructUsing(c => new Domain.Models.Customer(
                    c.FirstName,
                    c.LastName,
                    c.Email,
                    c.BirthDate,
                    Mapper.Map<Address>(c.UpdateAddress),
                    c.Id
                    ))
                .IgnoreAllPropertiesWithAnInaccessibleSetter();
        }
    }
}
