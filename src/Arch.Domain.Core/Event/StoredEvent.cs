using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using Arch.Infra.Shared.EventSourcing;
using Newtonsoft.Json.Linq;

namespace Arch.Domain.Core.Event
{
    public class StoredEvent : Infra.Shared.Cqrs.Event.Event
    {
        protected StoredEvent()
        {

        }
        public StoredEvent(Infra.Shared.Cqrs.Event.Event theEvent, string user)
        {
            theEvent.When = DateTime.Now.ToString();
            theEvent.Who = user;
            Id = Guid.NewGuid();
            AggregateId = theEvent.AggregateId;
            Action = theEvent.Action;

                var ignoredMembers = GetAttributs(theEvent).Select(_ => _.Name);
                var eventSerialized = JsonConvert.SerializeObject(theEvent);
                var jo = JObject.Parse(eventSerialized);
                ignoredMembers.ToList().ForEach(_ => jo.Property(_).Remove());
                var jsonData = jo.ToString();

            Data = jsonData;
            User = user;
            Assembly = theEvent.GetType().FullName;
        }

        //public EntityEvent(Entity entity, string user)
        //{
        //    //var theEvent = new Entity();
        //    //theEvent.When = DateTime.Now.ToString();
        //    //theEvent.Who = user;
        //    //Id = Guid.NewGuid();
        //    //AggregateId = theEvent.AggregateId;
        //    //Action = theEvent.Action;

        //    var ignoredMembers = GetAttributs(theEvent).Select(_ => _.Name);
        //    var eventSerialized = JsonConvert.SerializeObject(theEvent);
        //    var jo = JObject.Parse(eventSerialized);
        //    ignoredMembers.ToList().ForEach(_ => jo.Property(_).Remove());
        //    var jsonData = jo.ToString();

        //    Data = jsonData;
        //    User = user;
        //    Assembly = theEvent.GetType().FullName;
        //}

        public List<MemberInfo> GetAttributs(object obj)
        {
            var tipoGen = typeof(SourceFluent<>).MakeGenericType(obj.GetType());

            var target = obj.GetType().Assembly;
            var assemblies = target.GetReferencedAssemblies()
                .Select(System.Reflection.Assembly.Load).ToList();
            assemblies.Add(target);

            var map = assemblies.SelectMany(_ => _.GetExportedTypes())
                .FirstOrDefault(_ => tipoGen.IsAssignableFrom(_));

            var tipo = (dynamic)Activator.CreateInstance(map);
            tipo.Configuration(tipo);

            return (List<MemberInfo>)tipo.Members;
        }



        public string Serialize(object obj, IEnumerable<string> ignoredMembers)
        {
            var ms = new MemoryStream();
            var ser = new DataContractJsonSerializer(obj.GetType());
            ser.WriteObject(ms, obj);

            byte[] json = ms.ToArray();
            ms.Close();

            var jsonObj = Encoding.UTF8.GetString(json, 0, json.Length);

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
