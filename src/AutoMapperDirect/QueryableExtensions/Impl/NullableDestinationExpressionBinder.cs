using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapperDirect.Configuration;
using AutoMapperDirect.Internal;

namespace AutoMapperDirect.QueryableExtensions.Impl
{
    public class NullableDestinationExpressionBinder : IExpressionBinder
    {
        public bool IsMatch(PropertyMap propertyMap, TypeMap propertyTypeMap, ExpressionResolutionResult result) =>
            propertyMap.DestinationType.IsNullableType() && !result.Type.IsNullableType();

        public MemberAssignment Build(IConfigurationProvider configuration, PropertyMap propertyMap, TypeMap propertyTypeMap, ExpressionRequest request, ExpressionResolutionResult result, IDictionary<ExpressionRequest, int> typePairCount, LetPropertyMaps letPropertyMaps) 
            => BindNullableExpression(propertyMap, result);

        private static MemberAssignment BindNullableExpression(PropertyMap propertyMap,
            ExpressionResolutionResult result)
        {
            var destinationType = propertyMap.DestinationType;
            var expressionToBind =
                result.ResolutionExpression.GetMembers().Aggregate(
                    ExpressionFactory.ToType(result.ResolutionExpression, destinationType),
                    (accumulator, current) => current.IfNullElse(Expression.Constant(null, destinationType), accumulator));
            return Expression.Bind(propertyMap.DestinationMember, expressionToBind);
        }
    }
}