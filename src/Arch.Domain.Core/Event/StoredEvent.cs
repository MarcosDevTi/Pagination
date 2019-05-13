using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Arch.Domain.Core.Event
{
    public class StoredEvent : Infra.Shared.Cqrs.Event.Event
    {
        protected StoredEvent()
        {

        }
        public StoredEvent(Infra.Shared.Cqrs.Event.Event theEvent, string user)
        {
            //var props = anterior.GetType().GetProperties();

            theEvent.When = DateTime.Now.ToString();
            theEvent.Who = user;
            Id = Guid.NewGuid();
            AggregateId = theEvent.AggregateId;
            Action = theEvent.Action;
            Data = Serialize(theEvent);
            User = user;
            Assembly = theEvent.GetType().FullName;
        }



        public string Serialize(object obj)
        {
            var ms = new MemoryStream();
            var ser = new DataContractJsonSerializer(obj.GetType());
            ser.WriteObject(ms, obj);

            byte[] json = ms.ToArray();
            ms.Close();

            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        // EF Constructor
        protected StoredEvent(Infra.Shared.Cqrs.Event.Event @event) { }
        public string Assembly { get; private set; }
        public Guid Id { get; private set; }

        public string Data { get; private set; }

        public string User { get; private set; }
    }
}
