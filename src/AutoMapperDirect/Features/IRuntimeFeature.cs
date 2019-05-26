namespace AutoMapperDirect.Features
{
    public interface IRuntimeFeature
    {
        void Seal(IConfigurationProvider configurationProvider);
    }
}