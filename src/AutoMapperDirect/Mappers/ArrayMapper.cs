using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapperDirect.Configuration;
using AutoMapperDirect.Internal;
using AutoMapperDirect.Mappers.Internal;

namespace AutoMapperDirect.Mappers
{
    public class ArrayMapper : EnumerableMapperBase
    {
        public override bool IsMatch(TypePair context) => context.DestinationType.IsArray && context.SourceType.IsEnumerableType();

        public override Expression MapExpression(IConfigurationProvider configurationProvider, ProfileMap profileMap,
            IMemberMap memberMap, Expression sourceExpression, Expression destExpression, Expression contextExpression)
        {
            var sourceElementType = ElementTypeHelper.GetElementType(sourceExpression.Type);
            var destElementType = ElementTypeHelper.GetElementType(destExpression.Type);

            var itemExpr = CollectionMapperExpressionFactory.MapItemExpr(configurationProvider, profileMap, sourceExpression.Type, destExpression.Type, contextExpression, out ParameterExpression itemParam);

            //var count = source.Count();
            //var array = new TDestination[count];

            //int i = 0;
            //foreach (var item in source)
            //    array[i++] = newItemFunc(item, context);
            //return array;

            var countParam = Expression.Parameter(typeof(int), "count");
            var arrayParam = Expression.Parameter(destExpression.Type, "destinationArray");
            var indexParam = Expression.Parameter(typeof(int), "destinationArrayIndex");

            var actions = new List<Expression>();
            var parameters = new List<ParameterExpression> { countParam, arrayParam, indexParam };

            var countMethod = typeof(Enumerable)
                .GetTypeInfo()
                .DeclaredMethods
                .Single(mi => mi.Name == "Count" && mi.GetParameters().Length == 1)
                .MakeGenericMethod(sourceElementType);
            actions.Add(Expression.Assign(countParam, Expression.Call(countMethod, sourceExpression)));
            actions.Add(Expression.Assign(arrayParam, Expression.NewArrayBounds(destElementType, countParam)));
            actions.Add(Expression.Assign(indexParam, Expression.Constant(0)));
            actions.Add(ExpressionFactory.ForEach(sourceExpression, itemParam,
                Expression.Assign(Expression.ArrayAccess(arrayParam, Expression.PostIncrementAssign(indexParam)), itemExpr)
                ));
            actions.Add(arrayParam);

            return Expression.Block(parameters, actions);
        }
    }
}
