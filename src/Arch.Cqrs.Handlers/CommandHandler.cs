using System.Data.Entity;
using Arch.Domain.Core;
using Arch.Domain.Core.DomainNotifications;
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
                AddNotification(new DomainNotification(cmd.MessageType, error.ErrorMessage));
        }

        protected void Commit(Event evet)
        {
            if (_notifications.HasNotifications()) return;
            if (_architectureContext.SaveChanges() > 0)

                _eventRepository.Save(evet);
            else
            AddNotification(new DomainNotification("Commit", "We had a problem during saving your data."));
        }

        protected void AddNotification(DomainNotification notification)
        {
            _notifications.Add(notification);
        }
    }
}
