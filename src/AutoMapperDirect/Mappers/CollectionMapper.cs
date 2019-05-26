using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMapperDirect.Configuration;
using AutoMapperDirect.Mappers.Internal;

namespace AutoMapperDirect.Mappers
{
    public class CollectionMapper : EnumerableMapperBase
    {
        public override bool IsMatch(TypePair context) => context.SourceType.IsEnumerableType() && context.DestinationType.IsCollectionType();

        public override Expression MapExpression(IConfigurationProvider configurationProvider, ProfileMap profileMap,
            IMemberMap memberMap, Expression sourceExpression, Expression destExpression, Expression contextExpression)
            => CollectionMapperExpressionFactory.MapCollectionExpression(configurationProvider, profileMap, memberMap, sourceExpression, destExpression, contextExpression, typeof(List<>), CollectionMapperExpressionFactory.MapItemExpr);
    }
}