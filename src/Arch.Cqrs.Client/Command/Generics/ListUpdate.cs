using Arch.Infra.Shared.Cqrs.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arch.Cqrs.Client.Command.Customer.Generics
{
    public class ListUpdate: ICommand
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public dynamic Value { get; set; }
        public string AssemblyViewModel { get; set; }
        public string AssemblyModel { get; set; }
    }
}