using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMapperDirect.Configuration;

namespace AutoMapperDirect.QueryableExtensions.Impl
{
    internal class NullableSourceExpressionBinder : IExpressionBinder
    {
        public MemberAssignment Build(IConfigurationProvider configuration, PropertyMap propertyMap, TypeMap propertyTypeMap, ExpressionRequest request, ExpressionResolutionResult result, IDictionary<ExpressionRequest, int> typePairCount, LetPropertyMaps letPropertyMaps)
        {
            var defaultDestination = Activator.CreateInstance(propertyMap.DestinationType);
            return Expression.Bind(propertyMap.DestinationMember, Expression.Coalesce(result.ResolutionExpression, Expression.Constant(defaultDestination)));
        }

        public bool IsMatch(PropertyMap propertyMap, TypeMap propertyTypeMap, ExpressionResolutionResult result) =>
            result.Type.IsNullableType() && !propertyMap.DestinationType.IsNullableType() && propertyMap.DestinationType.IsValueType();
    }
}