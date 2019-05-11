using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Arch.Domain.Core
{
    public interface IUser
    {
        string Name { get; }
        Guid UserId();
        bool IsAuthenticated();
        IEnumerable<Claim> GetClaimsIdentity();
    }
}
