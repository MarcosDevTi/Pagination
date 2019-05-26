using System.Linq.Expressions;
using System.Reflection;

namespace AutoMapperDirect.Configuration
{
    public interface IPropertyMapConfiguration
    {
        void Configure(TypeMap typeMap);
        MemberInfo DestinationMember { get; }
        LambdaExpression SourceExpression { get; }
        LambdaExpression GetDestinationExpression();
        IPropertyMapConfiguration Reverse();
    }
}