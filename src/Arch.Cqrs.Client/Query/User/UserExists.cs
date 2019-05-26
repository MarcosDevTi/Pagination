using Arch.Infra.Shared.Cqrs.Query;

namespace Arch.Cqrs.Client.Query.User
{
    public class UserExists : IQuery<bool>
    {
        public UserExists(string userName)
        {
            UserName = userName;
        }
        public string UserName { get; set; }
    }
}
