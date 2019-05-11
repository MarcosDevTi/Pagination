using System;
using Arch.Cqrs.Client.AutoMapper;
using Arch.Cqrs.Client.Command.Customer.Validation;
using AutoMapper;

namespace Arch.Cqrs.Client.Command.Customer
{
    public class UpdateCustomer : CustomerCommand, ICustomMapper
    {
        public UpdateCustomer()
        {

        }
        public UpdateCustomer(Guid id, string firstName, string lastName, string email, DateTime birthDate, string street, string number, string city, string zipCode)
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
                    c.Street,
                    c.Number,
                    c.City,
                    c.ZipCode,
                    c.Id))
                .IgnoreAllPropertiesWithAnInaccessibleSetter();
        }
    }
}
