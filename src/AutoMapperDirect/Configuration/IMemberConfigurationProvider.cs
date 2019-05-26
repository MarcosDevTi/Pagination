namespace AutoMapperDirect.Configuration
{
    public interface IMemberConfigurationProvider
    {
        void ApplyConfiguration(IMemberConfigurationExpression memberConfigurationExpression);
    }
}