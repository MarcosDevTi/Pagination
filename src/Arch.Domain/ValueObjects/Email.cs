using Arch.Domain.Core;

namespace Arch.Domain.ValueObjects
{
    public class Email: Entity
    {
        public Email(string emailAddress)
        {
            EmailAddress = emailAddress;
        }

        public string EmailAddress { get; private set; }
    }
}
