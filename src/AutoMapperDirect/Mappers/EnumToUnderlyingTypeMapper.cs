using System;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapperDirect.Internal;
using AutoMapperDirect.Mappers.Internal;
using Convert = System.Convert;

namespace AutoMapperDirect.Mappers
{
    public class EnumToUnderlyingTypeMapper : IObjectMapper
    {
        private static readonly MethodInfo ChangeTypeMethod = ExpressionFactory.Method(() => Convert.ChangeType(null, typeof(object)));

        public bool IsMatch(TypePair context)
        {
            var sourceEnumType = ElementTypeHelper.GetEnumerationType(context.SourceType);

            return sourceEnumType != null && context.DestinationType.IsAssignableFrom(Enum.GetUnderlyingType(sourceEnumType));
        }

        public Expression MapExpression(IConfigurationProvider configurationProvider, ProfileMap profileMap,
            IMemberMap memberMap, Expression sourceExpression, Expression destExpression,
            Expression contextExpression) =>
                ExpressionFactory.ToType(
                    Expression.Call(ChangeTypeMethod, ExpressionFactory.ToObject(sourceExpression),
                        Expression.Constant(destExpression.Type)),
                    destExpression.Type
                );
    }
    
}