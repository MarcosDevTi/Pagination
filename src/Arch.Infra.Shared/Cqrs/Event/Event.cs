using System;

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
