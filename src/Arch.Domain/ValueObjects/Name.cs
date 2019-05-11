using Arch.Domain.Core;

namespace Arch.Domain.ValueObjects
{
    public class Name: Entity
    {
        public Name(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
    }
}
