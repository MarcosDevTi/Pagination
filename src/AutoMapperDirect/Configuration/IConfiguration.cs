using System;
using System.Collections.Generic;

namespace AutoMapperDirect.Configuration
{
    public interface IConfiguration : IProfileConfiguration
    {
        Func<Type, object> ServiceCtor { get; }
        IEnumerable<IProfileConfiguration> Profiles { get; }
    }
}