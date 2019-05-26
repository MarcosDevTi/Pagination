using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapperDirect.Internal;
using AutoMapperDirect.Mappers.Internal;

namespace AutoMapperDirect.Mappers
{
    public class StringToEnumMapper : IObjectMapper
    {
        public bool IsMatch(TypePair context) => context.SourceType == typeof(string) &&
                                                 ElementTypeHelper.GetEnumerationType(context.DestinationType) != null;

        public Expression MapExpression(IConfigurationProvider configurationProvider, ProfileMap profileMap,
            IMemberMap memberMap, Expression sourceExpression, Expression destExpression,
            Expression contextExpression)
        {
            var destinationType = destExpression.Type;
            var destinationEnumType = ElementTypeHelper.GetEnumerationType(destinationType);
            var enumParse = Expression.Call(typeof(Enum), "Parse", null, Expression.Constant(destinationEnumType),
                sourceExpression, Expression.Constant(true));
            var switchCases = new List<SwitchCase>();
            var enumNames = destinationEnumType.GetDeclaredMembers();
            foreach (var memberInfo in enumNames.Where(x => x.IsStatic()))
            {
                var attribute = memberInfo.GetCustomAttribute(typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                if (attribute?.Value != null)
                {
                    var switchCase = Expression.SwitchCase(
                        ExpressionFactory.ToType(Expression.Constant(Enum.ToObject(destinationEnumType, memberInfo.GetMemberValue(null))),
                            destinationType), Expression.Constant(attribute.Value));
                    switchCases.Add(switchCase);
                }
            }
            var equalsMethodInfo = ExpressionFactory.Method(() => StringCompareOrdinalIgnoreCase(null, null));
            var switchTable = switchCases.Count > 0
                ? Expression.Switch(sourceExpression, ExpressionFactory.ToType(enumParse, destinationType), equalsMethodInfo, switchCases)
                : ExpressionFactory.ToType(enumParse, destinationType);
            var isNullOrEmpty = Expression.Call(typeof(string), "IsNullOrEmpty", null, sourceExpression);
            return Expression.Condition(isNullOrEmpty, Expression.Default(destinationType), switchTable);
        }

        private static bool StringCompareOrdinalIgnoreCase(string x, string y)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(x, y);
        }
    }
}