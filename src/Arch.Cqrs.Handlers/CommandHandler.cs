using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using Arch.Cqrs.Client.Command.Customer;
using Arch.Domain.Core;
using Arch.Domain.Core.DomainNotifications;
using Arch.Domain.Core.Event;
using Arch.Domain.Event;
using Arch.Infra.Data;
using Arch.Infra.Shared.Cqrs.Command;
using Arch.Infra.Shared.Cqrs.Event;

namespace Arch.Cqrs.Handlers
{
    public abstract class CommandHandler<T> where T : Entity
    {
        private readonly ArchDbContext _architectureContext;
        private readonly IDomainNotification _notifications;
        private readonly IEventRepository _eventRepository;

        protected CommandHandler(ArchDbContext architectureContext, IDomainNotification notifications, IEventRepository eventRepository)
        {
            _architectureContext = architectureContext;
            _notifications = notifications;
            _eventRepository = eventRepository;
        }

        protected DbSet<T> Db() => _architectureContext.Set<T>();

        protected void ValidateCommand(Command cmd)
        {
            if (cmd.IsValid()) return;
            foreach (var error in cmd.ValidationResult.Errors)
                AddNotification(new DomainNotification(cmd.Action, error.ErrorMessage));
        }

        protected void Commit(Event evet)
        {


            if (_notifications.HasNotifications()) return;
            if (_architectureContext.SaveChanges() > 0)
            {
                
                
                var anterior = _eventRepository.GetLastEvent(evet.AggregateId);
                var objJson = anterior != null
                    ? ReadToObject(((StoredEvent) anterior).Data, ((StoredEvent) anterior).Assembly)
                    : null;
                _eventRepository.Save(evet, objJson);

                
            }

            
            else
            AddNotification(new DomainNotification("Commit", "We had a problem during saving your data."));
        }

        
        protected void AddNotification(DomainNotification notification)
        {
            _notifications.Add(notification);
        }


        public static object ReadToObject(string json, string typeP)
        {
            var asm = typeof(CreateCustomer).Assembly;
            var type = asm.GetType(typeP);

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var ser = new DataContractJsonSerializer(type);
            var res = ser.ReadObject(ms) as object;
            ms.Close();

            Convert.ChangeType(res, type);
            return res;
        }
    }
}
