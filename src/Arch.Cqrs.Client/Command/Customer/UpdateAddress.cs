using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.Cqrs.Client.AutoMapper;
using Arch.Domain.ValueObjects;
using AutoMapper;

namespace Arch.Cqrs.Client.Command.Customer
{
    public class UpdateAddress: IMapTo<Address>
    {
        public UpdateAddress(Guid id, string street, string number, string city, string zipCode)
        {
            Id = id;
            Street = street;
            Number = number;
            City = city;
            ZipCode = zipCode;
        }
        public Guid Id { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
    }
}
