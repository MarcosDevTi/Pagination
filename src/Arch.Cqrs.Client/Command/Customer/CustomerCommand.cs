﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Arch.Cqrs.Client.Command.Customer
{
    public abstract class CustomerCommand : Infra.Shared.Cqrs.Command.Command
    {
        [Required(ErrorMessage = "The FirstName is Required")]
        [MinLength(2)]
        [MaxLength(100)]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "The LastName is Required")]
        [MinLength(2)]
        [MaxLength(100)]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "The E-mail is Required")]
        [EmailAddress]
        [DisplayName("E-mail")]
        public string Email { get; set; }
        [Required(ErrorMessage = "The BirthDate is Required")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date, ErrorMessage = "Data em formato inválido")]
        [DisplayName("Birth Date")]
        public DateTime BirthDate { get; set; }
    }
}