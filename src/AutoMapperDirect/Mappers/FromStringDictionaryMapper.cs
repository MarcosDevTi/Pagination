using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapperDirect.Execution;
using AutoMapperDirect.Internal;
using StringDictionary = System.Collections.Generic.IDictionary<string, object>;

namespace AutoMapperDirect.Mappers
{
    public class FromStringDictionaryMapper : IObjectMapper
    {
        private static readonly MethodInfo MapMethodInfo =
            typeof(FromStringDictionaryMapper).GetDeclaredMethod(nameof(Map));

        public bool IsMatch(TypePair context) => typeof(StringDictionary).IsAssignableFrom(context.SourceType);

        public Expression MapExpression(IConfigurationProvider configurationProvider, ProfileMap profileMap,
            IMemberMap memberMap, Expression sourceExpression, Expression destExpression,
            Expression contextExpression) =>
            Expression.Call(null,
                MapMethodInfo.MakeGenericMethod(destExpression.Type),
                sourceExpression,
                Expression.Condition(
                    Expression.Equal(ExpressionFactory.ToObject(destExpression), Expression.Constant(null)),
                    DelegateFactory.GenerateConstructorExpression(destExpression.Type),
                    destExpression),
                contextExpression,
                Expression.Constant(profileMap));

        private static TDestination Map<TDestination>(StringDictionary source, TDestination destination, ResolutionContext context, ProfileMap profileMap)
        {
            var destTypeDetails = profileMap.CreateTypeDetails(typeof(TDestination));
            var members = from name in source.Keys
                          join member in destTypeDetails.PublicWriteAccessors on name equals member.Name
                          select member;
            object boxedDestination = destination;
            foreach (var member in members)
            {
                var value = context.MapMember(member, source[member.Name], boxedDestination);
                member.SetMemberValue(boxedDestination, value);
            }
            return (TDestination) boxedDestination;
        }
    }
}