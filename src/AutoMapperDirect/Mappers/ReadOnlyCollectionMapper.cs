using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using AutoMapperDirect.Configuration;
using AutoMapperDirect.Mappers.Internal;

namespace AutoMapperDirect.Mappers
{
    public class ReadOnlyCollectionMapper : IObjectMapper
    {
        public bool IsMatch(TypePair context)
        {
            if (!(context.SourceType.IsEnumerableType() && context.DestinationType.IsGenericType()))
                return false;

            var genericType = context.DestinationType.GetGenericTypeDefinition();

            return genericType == typeof (ReadOnlyCollection<>);
        }

        public Expression MapExpression(IConfigurationProvider configurationProvider, ProfileMap profileMap,
            IMemberMap memberMap, Expression sourceExpression, Expression destExpression, Expression contextExpression)
        {
            var listType = typeof(List<>).MakeGenericType(ElementTypeHelper.GetElementType(destExpression.Type));
            var list = CollectionMapperExpressionFactory.MapCollectionExpression(configurationProvider, profileMap, memberMap, sourceExpression, Expression.Default(listType), contextExpression, typeof(List<>), CollectionMapperExpressionFactory.MapItemExpr);
            var dest = Expression.Variable(listType, "dest");

            var ctor = destExpression.Type.GetDeclaredConstructors()
                .First(ci => ci.GetParameters().Length == 1 && ci.GetParameters()[0].ParameterType.IsAssignableFrom(dest.Type));

            return Expression.Block(new[] { dest }, 
                Expression.Assign(dest, list), 
                Expression.Condition(Expression.NotEqual(dest, Expression.Default(listType)), 
                    Expression.New(ctor, dest), 
                    Expression.Default(destExpression.Type)));
        }
    }
}