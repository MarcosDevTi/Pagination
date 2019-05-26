using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapperDirect.Mappers.Internal;

namespace AutoMapperDirect.Mappers
{
    public class ToStringDictionaryMapper : IObjectMapper
    {
        private static readonly MethodInfo MembersDictionaryMethodInfo =
            typeof(ToStringDictionaryMapper).GetDeclaredMethod(nameof(MembersDictionary));

        public bool IsMatch(TypePair context) => typeof(IDictionary<string, object>).IsAssignableFrom(context.DestinationType);

        public Expression MapExpression(IConfigurationProvider configurationProvider, ProfileMap profileMap,
            IMemberMap memberMap, Expression sourceExpression, Expression destExpression, Expression contextExpression)
            => CollectionMapperExpressionFactory.MapCollectionExpression(configurationProvider, profileMap, memberMap,
                Expression.Call(MembersDictionaryMethodInfo, sourceExpression, Expression.Constant(profileMap)), destExpression, contextExpression, typeof(Dictionary<,>),
                CollectionMapperExpressionFactory.MapKeyPairValueExpr);

        private static Dictionary<string, object> MembersDictionary(object source, ProfileMap profileMap)
        {
            var sourceTypeDetails = profileMap.CreateTypeDetails(source.GetType());
            var membersDictionary = sourceTypeDetails.PublicReadAccessors.ToDictionary(p => p.Name, p => p.GetMemberValue(source));
            return membersDictionary;
        }
    }
}