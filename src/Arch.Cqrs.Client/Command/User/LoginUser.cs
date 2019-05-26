using Arch.Infra.Shared.Cqrs.Query;

namespace Arch.Cqrs.Client.Command.User
{
    public class LoginUser : IQuery<Domain.Models.User>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
