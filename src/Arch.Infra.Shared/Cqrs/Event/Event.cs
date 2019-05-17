using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Arch.Infra.Shared.Cqrs.Event
{
    public abstract class Event : Message
    {
        public string When { get; set; }
        

        protected Event()
        {
            When = DateTime.Now.ToString();
        }

        
    }

    
}
