using System;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapperDirect.Mappers.Internal;

namespace AutoMapperDirect.Mappers
{
    public class ArrayCopyMapper : ArrayMapper
    {
        private static readonly Expression<Action> ArrayCopyExpression = () => Array.Copy(default, default, default(long));
        private static readonly Expression<Func<Array, long>> ArrayLengthExpression = arr => arr.LongLength;

        private static readonly MethodInfo ArrayCopyMethod = ((MethodCallExpression)ArrayCopyExpression.Body).Method;
        private static readonly PropertyInfo ArrayLengthProperty = (PropertyInfo) ((MemberExpression)ArrayLengthExpression.Body).Member;

        public override bool IsMatch(TypePair context) =>
            context.DestinationType.IsArray
            && context.SourceType.IsArray
            && ElementTypeHelper.GetElementType(context.DestinationType) == ElementTypeHelper.GetElementType(context.SourceType)
            && ElementTypeHelper.GetElementType(context.SourceType).IsPrimitive();

        public override Expression MapExpression(IConfigurationProvider configurationProvider, ProfileMap profileMap,
            IMemberMap memberMap, Expression sourceExpression, Expression destExpression, Expression contextExpression)
        {
            var destElementType = ElementTypeHelper.GetElementType(destExpression.Type);
            var sourceElementType = ElementTypeHelper.GetElementType(sourceExpression.Type);

            if (configurationProvider.FindTypeMapFor(sourceElementType, destElementType) != null)
                return base.MapExpression(configurationProvider, profileMap, memberMap, sourceExpression, destExpression, contextExpression);

            var valueIfNullExpr = profileMap.AllowNullCollections
                ? (Expression) Expression.Constant(null, destExpression.Type)
                : Expression.NewArrayBounds(destElementType, Expression.Constant(0));

            var dest = Expression.Parameter(destExpression.Type, "destArray");
            var sourceLength = Expression.Parameter(ArrayLengthProperty.PropertyType, "sourceLength");
            var mapExpr = Expression.Block(
                new[] {dest, sourceLength},
                Expression.Assign(sourceLength, Expression.Property(sourceExpression, ArrayLengthProperty)),
                Expression.Assign(dest, Expression.NewArrayBounds(destElementType, sourceLength)),
                Expression.Call(ArrayCopyMethod, sourceExpression, dest, sourceLength),
                dest
            );

            return Expression.Condition(Expression.Equal(sourceExpression, Expression.Constant(null)), valueIfNullExpr, mapExpr);

        }
    }
}
