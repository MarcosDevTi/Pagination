namespace Arch.Infra.Shared.EventSourcing
{
    public class SourceTypeBuilder<TEntity> where TEntity : class
    {
        private readonly SourceTypeBuilder<TEntity> _sourceTypeBuilder;
        public SourceTypeBuilder<TEntity> Builder() => _sourceTypeBuilder;
    }
}
