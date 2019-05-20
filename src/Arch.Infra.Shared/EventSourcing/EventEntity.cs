using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Arch.Infra.Shared.EventSourcing
{
    public class EventEntity
    {
        public EventEntity() { }
        public EventEntity(string action, dynamic entity, string who, object lastEntity = null)
        {
            Id = Guid.NewGuid();
            When = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            Action = action;
            Data = BuildJsonData(entity, lastEntity);
            AggregateId = entity.Id;
            Who = who;
            Assembly = entity.GetType().FullName;
        }

        public Guid Id { get; set; }
        public string When { get; set; }
        public string Action { get; set; }
        public string Data { get; set; }
        public Guid AggregateId { get; set; }
        public string Who { get; set; }

        public string Assembly { get; set; }

        public object ReadToObject(EventEntity entity, Type domainTypeAssembly)
        {
            var asm = domainTypeAssembly.Assembly;
            var type = asm.GetType(entity.Assembly);

            var memberProperties = GetAttributs(type).Item1;
            var memberNames = GetAttributs(type).Item2;

            var listIgnoredMembers = memberProperties?.Select(_ => _.Name);
            var jo = JObject.Parse(entity.Data);
            listIgnoredMembers?.ToList().ForEach(_ =>
            {
                if (_ != null) jo.Property(_)?.Remove();
            });

            if (jo.Property("Id") != null)
            {
                jo.Property("Id").Remove();
            }

            memberNames?.ToList().ForEach(_ =>
            {
                if (jo.Property(_.Key.Name) == null) return;
                var temp = jo.Property(_.Key.Name);
                jo.Property(_.Key.Name).Remove();
                jo.Add(_.Value, temp.Value);
            });

            jo.Add("Action", entity.Action);
            jo.Add("When", entity.When);
            jo.Add("Who", entity.Who);

            return jo;
        }

        private Tuple<List<MemberInfo>, Dictionary<MemberInfo, string>> GetAttributs(Type type)
        {
            var tipoGen = typeof(SourceFluent<>).MakeGenericType(type);

            var target = type.Assembly;
            var assemblies = target.GetReferencedAssemblies()
                .Select(System.Reflection.Assembly.Load).ToList();
            assemblies.Add(target);

            var map = assemblies.SelectMany(_ => _.GetExportedTypes())
                .FirstOrDefault(_ => tipoGen.IsAssignableFrom(_));

            var entityType = (dynamic)Activator.CreateInstance(map);
            entityType.Configuration(entityType);
            var membersName = (Dictionary<MemberInfo, string>)entityType.MembersDisplayName;
            var membersProperties = (List<MemberInfo>)entityType.Members;
            return new Tuple<List<MemberInfo>, Dictionary<MemberInfo, string>>(membersProperties, membersName);
        }

        private string BuildJsonData(object entity, object lastEntity = null)
        {
            var iso = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };
            var eventSerialized = JsonConvert.SerializeObject(entity, iso);
            var jo = JObject.Parse(eventSerialized);

            if (lastEntity != null)
            {
                var modifieds = GetMembersUpdated(entity, lastEntity);
                modifieds.ToList().ForEach(_ =>
                {
                    if (jo.Property(_) != null)
                        jo.Property(_).Remove();
                });
            }
            jo.Property("CreatedDate")?.Remove();
            jo.Property("Id")?.Remove();

            var data = JsonHelper.RemoveEmptyChildren(jo).ToString();

            return data;
        }

        private string[] GetMembersUpdated(object objOrig, object objModified)
        {
            var modifieds = GetChangedProperties(objOrig, objModified);
            return modifieds.ToArray();
        }

        private static List<string> GetChangedProperties(object a, object b) =>
                 a.GetType() != b.GetType()
                ? throw new InvalidOperationException("Objects of different Type")
                : ElaborateChangedProperties(a.GetType().GetProperties(), b.GetType().GetProperties(), a, b);

        private static List<string> ElaborateChangedProperties(PropertyInfo[] pA, PropertyInfo[] pB, object a, object b) =>
            (from info in pA
             let propValueA = info.GetValue(a, null)
             let propValueB = info.GetValue(b, null)
             where propValueA?.ToString() == propValueB?.ToString()
             select info.Name).ToList();

    }

    public static class JsonHelper
    {
        public static JToken RemoveEmptyChildren(JToken token)
        {
            if (token.Type == JTokenType.Object)
            {
                var copy = new JObject();
                foreach (var prop in token.Children<JProperty>())
                {
                    var child = prop.Value;
                    if (child.HasValues)
                    {
                        child = RemoveEmptyChildren(child);
                    }
                    if (!IsEmpty(child))
                    {
                        copy.Add(prop.Name, child);
                    }
                }
                return copy;
            }

            if (token.Type != JTokenType.Array) return token;
            {
                var copy = new JArray();
                foreach (var item in token.Children())
                {
                    var child = item;
                    if (child.HasValues)
                    {
                        child = RemoveEmptyChildren(child);
                    }
                    if (!IsEmpty(child))
                    {
                        copy.Add(child);
                    }
                }
                return copy;
            }
        }

        public static bool IsEmpty(JToken token) => token.Type == JTokenType.Null;

    }
}
