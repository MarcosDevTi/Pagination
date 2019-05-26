using System;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapperDirect.Internal;
using AutoMapperDirect.Mappers.Internal;

namespace AutoMapperDirect.Mappers
{
    public class UnderlyingTypeToEnumMapper : IObjectMapper
    {
        private static readonly MethodInfo EnumToObject = ExpressionFactory.Method(() => Enum.ToObject(typeof(object), null));

        public bool IsMatch(TypePair context)
        {
            var destEnumType = ElementTypeHelper.GetEnumerationType(context.DestinationType);

            return destEnumType != null && context.SourceType.IsAssignableFrom(Enum.GetUnderlyingType(destEnumType));
        }

        public Expression MapExpression(IConfigurationProvider configurationProvider, ProfileMap profileMap,
            IMemberMap memberMap, Expression sourceExpression, Expression destExpression,
            Expression contextExpression) =>
                ExpressionFactory.ToType(
                    Expression.Call(EnumToObject, Expression.Constant(destExpression.Type),
                        ExpressionFactory.ToObject(sourceExpression)),
                    destExpression.Type
                );
    }
}