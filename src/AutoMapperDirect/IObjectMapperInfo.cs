namespace AutoMapperDirect
{
    public interface IObjectMapperInfo : IObjectMapper
    {
        TypePair GetAssociatedTypes(TypePair initialTypes);
    }
}