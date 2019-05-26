using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapperDirect.Internal;
using AutoMapperDirect.Mappers.Internal;

namespace AutoMapperDirect.Mappers
{
    public class FlagsEnumMapper : IObjectMapper
    {
        private static readonly MethodInfo EnumParseMethod = ExpressionFactory.Method(() => Enum.Parse(null, null, true));

        public bool IsMatch(TypePair context)
        {
            var sourceEnumType = ElementTypeHelper.GetEnumerationType(context.SourceType);
            var destEnumType = ElementTypeHelper.GetEnumerationType(context.DestinationType);

            return sourceEnumType != null
                   && destEnumType != null
                   && sourceEnumType.GetCustomAttributes(typeof (FlagsAttribute), false).Any()
                   && destEnumType.GetCustomAttributes(typeof (FlagsAttribute), false).Any();
        }

        public Expression MapExpression(IConfigurationProvider configurationProvider, ProfileMap profileMap,
            IMemberMap memberMap, Expression sourceExpression, Expression destExpression,
            Expression contextExpression) =>
                ExpressionFactory.ToType(
                    Expression.Call(EnumParseMethod,
                        Expression.Constant(destExpression.Type),
                        Expression.Call(sourceExpression, sourceExpression.Type.GetDeclaredMethod("ToString", new Type[0])),
                        Expression.Constant(true)
                    ),
                    destExpression.Type
                );
    }
}