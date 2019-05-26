using Arch.Cqrs.Client.AutoMapper;
using Arch.Cqrs.Client.Command.Customer.Validation;
using AutoMapper;
using System;

namespace Arch.Cqrs.Client.Command.Customer
{
    public class UpdateCustomer : CustomerCommand, ICustomMapper
    {
        public UpdateCustomer()
        {

        }
        public Guid Id { get; set; }
        public UpdateAddress UpdateAddress { get; set; }
        public UpdateCustomer(Guid id, string firstName, string lastName, string email, DateTime birthDate)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            BirthDate = birthDate;
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
                    c.Id
                    ))
                .IgnoreAllPropertiesWithAnInaccessibleSetter();
        }
    }
}
