using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMapperDirect.Configuration;
using AutoMapperDirect.Mappers.Internal;

namespace AutoMapperDirect.Mappers
{
    public class EnumerableMapper : EnumerableMapperBase
    {
        public override bool IsMatch(TypePair context) => (context.DestinationType.IsInterface() && context.DestinationType.IsEnumerableType() ||
                                                  context.DestinationType.IsListType())
                                                 && context.SourceType.IsEnumerableType();

        public override Expression MapExpression(IConfigurationProvider configurationProvider, ProfileMap profileMap,
            IMemberMap memberMap, Expression sourceExpression, Expression destExpression, Expression contextExpression)
        {
            if(destExpression.Type.IsInterface())
            {
                var listType = typeof(IList<>).MakeGenericType(ElementTypeHelper.GetElementType(destExpression.Type));
                destExpression = Expression.Convert(destExpression, listType);
            }
            return CollectionMapperExpressionFactory.MapCollectionExpression(configurationProvider, profileMap, memberMap, sourceExpression,
                destExpression, contextExpression, typeof(List<>), CollectionMapperExpressionFactory.MapItemExpr);
        }
    }
}