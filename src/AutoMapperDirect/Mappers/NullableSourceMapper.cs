using System;
using System.Linq.Expressions;
using AutoMapperDirect.Configuration;
using AutoMapperDirect.Execution;

namespace AutoMapperDirect.Mappers
{
    public class NullableSourceMapper : IObjectMapperInfo
    {
        public bool IsMatch(TypePair context) => context.SourceType.IsNullableType();

        public Expression MapExpression(IConfigurationProvider configurationProvider, ProfileMap profileMap,
            IMemberMap memberMap, Expression sourceExpression, Expression destExpression,
            Expression contextExpression) =>
                ExpressionBuilder.MapExpression(configurationProvider, profileMap,
                    new TypePair(Nullable.GetUnderlyingType(sourceExpression.Type), destExpression.Type),
                    Expression.Property(sourceExpression, sourceExpression.Type.GetDeclaredProperty("Value")),
                    contextExpression,
                    memberMap,
                    destExpression
                );

        public TypePair GetAssociatedTypes(TypePair initialTypes)
        {
            return new TypePair(Nullable.GetUnderlyingType(initialTypes.SourceType), initialTypes.DestinationType);
        }
    }
}