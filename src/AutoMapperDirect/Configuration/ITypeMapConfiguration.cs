using System;

namespace AutoMapperDirect.Configuration
{
    public interface ITypeMapConfiguration
    {
        void Configure(TypeMap typeMap);
        Type SourceType { get; }
        Type DestinationType { get; }
        bool IsOpenGeneric { get; }
        TypePair Types { get; }
        ITypeMapConfiguration ReverseTypeMap { get; }
    }
}