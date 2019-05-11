using AutoMapper;

namespace Arch.Cqrs.Client.AutoMapper
{
    public interface ICustomMapper
    {
        void Map(IMapperConfigurationExpression config);
    }
}
