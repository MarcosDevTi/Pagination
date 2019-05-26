using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using AutoMapperDirect.Configuration;
using AutoMapperDirect.Internal;
using AutoMapperDirect.Mappers.Internal;

namespace AutoMapperDirect.Mappers
{
    public class ReadOnlyDictionaryMapper : IObjectMapper
    {
        public bool IsMatch(TypePair context)
        {
            if (!(context.SourceType.IsEnumerableType() && context.DestinationType.IsGenericType()))
                return false;

            var genericType = context.DestinationType.GetGenericTypeDefinition();

            return genericType == typeof(ReadOnlyDictionary<,>) || genericType == typeof(IReadOnlyDictionary<,>);
        }

        public Expression MapExpression(IConfigurationProvider configurationProvider, ProfileMap profileMap,
            IMemberMap memberMap, Expression sourceExpression, Expression destExpression, Expression contextExpression)
        {
            var dictionaryTypes = ElementTypeHelper.GetElementTypes(destExpression.Type, ElementTypeFlags.BreakKeyValuePair);
            var dictType = typeof(Dictionary<,>).MakeGenericType(dictionaryTypes);
            var dict = CollectionMapperExpressionFactory.MapCollectionExpression(configurationProvider, profileMap, memberMap, sourceExpression, Expression.Default(dictType), contextExpression, typeof(Dictionary<,>), CollectionMapperExpressionFactory.MapKeyPairValueExpr);
            var dest = Expression.Variable(dictType, "dest");

            var readOnlyDictType = destExpression.Type.IsInterface
                ? typeof(ReadOnlyDictionary<,>).MakeGenericType(dictionaryTypes)
                : destExpression.Type;

            var ctor = readOnlyDictType.GetDeclaredConstructors()
                .First(ci => ci.GetParameters().Length == 1 && ci.GetParameters()[0].ParameterType.IsAssignableFrom(dest.Type));

            return Expression.Block(new[] { dest }, 
                Expression.Assign(dest, dict), 
                Expression.Condition(Expression.NotEqual(dest, Expression.Default(dictType)), 
                    ExpressionFactory.ToType(Expression.New(ctor, dest), destExpression.Type), 
                    Expression.Default(destExpression.Type)));
        }
    }
}