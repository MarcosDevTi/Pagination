using Arch.Cqrs.Client.AutoMapper;
using Arch.Cqrs.Client.Command.Customer.Validation;
using AutoMapper;
using FluentValidation.Results;
using System;

namespace Arch.Cqrs.Client.Command.Customer
{
    public class CreateCustomer : CustomerCommand, ICustomMapper
    {
        //ctor AutoMapper
        public CreateCustomer() { }
        public CreateCustomer(
            string firstName,
            string lastName,
            string email,
            DateTime birthDate,
            Guid? userId = null)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            BirthDate = birthDate;
        }

        public void Map(IMapperConfigurationExpression cfg) =>
            cfg.CreateMap<CreateCustomer, Domain.Models.Customer>()
                .ConstructUsing(c =>
                    new Domain.Models.Customer(
                        c.FirstName, c.LastName, c.Email, c.BirthDate))
                .IgnoreAllPropertiesWithAnInaccessibleSetter();

        public override bool IsValid()
        {
            ValidationResult = new CreateCustomerValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }
}
