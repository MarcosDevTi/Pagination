using Arch.Domain.Core.DomainNotifications;
using System.Web.Http;

namespace Arch.Api.Controllers
{
    public class BaseController : ApiController
    {
        private readonly IDomainNotification _notifications;

        public BaseController(IDomainNotification notifications)
        {
            _notifications = notifications;
        }

        public bool IsValidOperation()  
        {
            return (!_notifications.HasNotifications());
        }
    }
}
