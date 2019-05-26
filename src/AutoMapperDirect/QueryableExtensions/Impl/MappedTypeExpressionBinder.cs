using System.Collections.Generic;
using System.Linq.Expressions;
using AutoMapperDirect.Configuration;

namespace AutoMapperDirect.QueryableExtensions.Impl
{
    public class MappedTypeExpressionBinder : IExpressionBinder
    {
        public bool IsMatch(PropertyMap propertyMap, TypeMap propertyTypeMap, ExpressionResolutionResult result) => 
            propertyTypeMap != null && propertyTypeMap.CustomMapExpression == null;

        public MemberAssignment Build(IConfigurationProvider configuration, PropertyMap propertyMap, TypeMap propertyTypeMap, ExpressionRequest request, ExpressionResolutionResult result, IDictionary<ExpressionRequest, int> typePairCount, LetPropertyMaps letPropertyMaps) 
            => BindMappedTypeExpression(configuration, propertyMap, request, result, typePairCount, letPropertyMaps);

        private static MemberAssignment BindMappedTypeExpression(IConfigurationProvider configuration, PropertyMap propertyMap, ExpressionRequest request, ExpressionResolutionResult result, IDictionary<ExpressionRequest, int> typePairCount, LetPropertyMaps letPropertyMaps)
        {
            var transformedExpression = configuration.ExpressionBuilder.CreateMapExpression(request, result.ResolutionExpression, typePairCount, letPropertyMaps);
            if(transformedExpression == null)
            {
                return null;
            }
            // Handles null source property so it will not create an object with possible non-nullable properties 
            // which would result in an exception.
            if (propertyMap.TypeMap.Profile.AllowNullDestinationValues && !propertyMap.AllowNull && !(result.ResolutionExpression is ParameterExpression) && !result.ResolutionExpression.Type.IsCollectionType())
            {
                transformedExpression = result.ResolutionExpression.IfNullElse(Expression.Constant(null, transformedExpression.Type), transformedExpression);
            }

            return Expression.Bind(propertyMap.DestinationMember, transformedExpression);
        }
    }
}