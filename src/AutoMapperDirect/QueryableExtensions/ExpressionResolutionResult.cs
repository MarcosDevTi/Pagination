using System;
using System.Linq.Expressions;

namespace AutoMapperDirect.QueryableExtensions
{
    public class ExpressionResolutionResult
    {
        public Expression ResolutionExpression { get; }
        public Type Type { get; }

        public ExpressionResolutionResult(Expression resolutionExpression, Type type)
        {
            ResolutionExpression = resolutionExpression;
            Type = type;
        }
    }
}