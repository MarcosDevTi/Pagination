using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMapperDirect.Configuration;
using AutoMapperDirect.Mappers.Internal;

namespace AutoMapperDirect.Mappers
{
    public class HashSetMapper : IObjectMapper
    {
        public bool IsMatch(TypePair context)
            => context.SourceType.IsEnumerableType() && IsSetType(context.DestinationType);

        public Expression MapExpression(IConfigurationProvider configurationProvider, ProfileMap profileMap,
            IMemberMap memberMap, Expression sourceExpression, Expression destExpression, Expression contextExpression)
            => CollectionMapperExpressionFactory.MapCollectionExpression(configurationProvider, profileMap, memberMap, sourceExpression, destExpression, contextExpression, typeof(HashSet<>), CollectionMapperExpressionFactory.MapItemExpr);

        private static bool IsSetType(Type type) => type.IsSetType();
    }
}