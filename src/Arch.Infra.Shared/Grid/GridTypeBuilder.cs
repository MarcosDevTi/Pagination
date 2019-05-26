namespace Arch.Infra.Shared.Grid
{
    public class GridTypeBuilder<T> where T : class
    {
        private readonly GridTypeBuilder<T> _gridTypeBuilder;
        public GridTypeBuilder<T> Builder() => _gridTypeBuilder;
    }
}
