using Arch.Domain.Core;
using Arch.Domain.ValueObjects;
using System;
using System.Collections.Generic;

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
            Guid? id = null)
        {
            Id = id == null ? Guid.NewGuid() : id.Value;
            CreatedDate = DateTime.Now;
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = email;
            BirthDate = birthDate;
            //Orders = new List<Order>();
        }

        public void OrdersAdd(Order order) => Orders.Add(order);

        public void OrdersAdd(IEnumerable<Order> orders)
        {
            foreach (var order in orders) Orders.Add(order);
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public DateTime BirthDate { get; set; }
        public Address Address { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
