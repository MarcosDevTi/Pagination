using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMapperDirect.Configuration;
using AutoMapperDirect.Mappers.Internal;

namespace AutoMapperDirect.Mappers
{
    public class EnumerableToDictionaryMapper : IObjectMapper
    {
        public bool IsMatch(TypePair context) => context.DestinationType.IsDictionaryType()
                                                 && context.SourceType.IsEnumerableType()
                                                 && !context.SourceType.IsDictionaryType();

        public Expression MapExpression(IConfigurationProvider configurationProvider, ProfileMap profileMap,
            IMemberMap memberMap, Expression sourceExpression, Expression destExpression, Expression contextExpression)
            =>
            CollectionMapperExpressionFactory.MapCollectionExpression(configurationProvider, profileMap, memberMap, sourceExpression, destExpression,
                contextExpression, typeof(Dictionary<,>), CollectionMapperExpressionFactory.MapItemExpr);
    }
}