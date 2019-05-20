using System;
using Arch.Domain.Core;

namespace Arch.Domain.ValueObjects
{
    public class Address: Entity
    {
        public Address(string street, string number, string city, string zipCode, Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
            //CreatedDate = DateTime.Now;
            Street = street;
            Number = number;
            City = city;
            ZipCode = zipCode;
        }

        public string Street { get; private set; }
        public string Number { get; private set; }
        public string City { get; private set; }
        public string ZipCode { get; private set; }
    }
}
