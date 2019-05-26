namespace AutoMapperDirect.Features
{
    public interface IMappingFeature
    {
        void Configure(TypeMap typeMap);
        IMappingFeature Reverse();
    }
}
