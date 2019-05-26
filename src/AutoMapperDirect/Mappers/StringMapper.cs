using System.Linq.Expressions;

namespace AutoMapperDirect.Mappers
{
    public class StringMapper : IObjectMapper
    {
        public bool IsMatch(TypePair context) => context.DestinationType == typeof(string) && context.SourceType != typeof(string);

        public Expression MapExpression(IConfigurationProvider configurationProvider, ProfileMap profileMap,
            IMemberMap memberMap, Expression sourceExpression, Expression destExpression, Expression contextExpression)
        {
            return Expression.Call(sourceExpression, typeof(object).GetDeclaredMethod("ToString"));
        }
    }
}