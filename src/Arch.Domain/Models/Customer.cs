using System;
using System.Collections.Generic;
using Arch.Domain.Core;
using Arch.Domain.ValueObjects;
using AutoMapper;

namespace Arch.Domain.Models
{
    public class Customer : Entity
    {
        public Customer() { } // Empty constructor for EF
        public Customer(
            string firstName,
            string lastName,
            string email,
            DateTime birthDate,
            Address address,
            Guid? id = null)
        {
            Id = id == null ? Guid.NewGuid() : id.Value;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            BirthDate = birthDate;
            Address = address;
            //Orders = new List<Order>();
        }

        public void OrdersAdd(Order order) => Orders.Add(order);

        public void OrdersAdd(IEnumerable<Order> orders)
        {
            foreach (var order in orders) Orders.Add(order);
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public DateTime BirthDate { get; private set; }
        public Address Address { get; private set; }
        public ICollection<Order> Orders { get; set; }
    }
}
