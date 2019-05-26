
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapperDirect.Configuration;
using AutoMapperDirect.Internal;
using AutoMapperDirect.Mappers.Internal;

namespace AutoMapperDirect.Execution
{
    public static class ExpressionBuilder
    {
        private static readonly Expression<Func<IRuntimeMapper, ResolutionContext>> CreateContext =
            mapper => new ResolutionContext(mapper.DefaultContext.Options, mapper);

        private static readonly MethodInfo ContextMapMethod =
            ExpressionFactory.Method<ResolutionContext, object>(a => a.Map<object, object>(null, null, null)).GetGenericMethodDefinition();            

        public static Expression MapExpression(IConfigurationProvider configurationProvider,
            ProfileMap profileMap,
            TypePair typePair,
            Expression sourceParameter,
            Expression contextParameter,
            IMemberMap propertyMap = null, Expression destinationParameter = null)
        {
            if (destinationParameter == null)
                destinationParameter = Expression.Default(typePair.DestinationType);
            var typeMap = configurationProvider.ResolveTypeMap(typePair);
            if (typeMap != null)
            {
                if (!typeMap.HasDerivedTypesToInclude())
                {
                    typeMap.Seal(configurationProvider);

                    return typeMap.MapExpression != null
                        ? typeMap.MapExpression.ConvertReplaceParameters(sourceParameter, destinationParameter,
                            contextParameter)
                        : ContextMap(typePair, sourceParameter, contextParameter, destinationParameter, propertyMap);
                }
                return ContextMap(typePair, sourceParameter, contextParameter, destinationParameter, propertyMap);
            }
            var objectMapperExpression = ObjectMapperExpression(configurationProvider, profileMap, typePair,
                sourceParameter, contextParameter, propertyMap, destinationParameter);
            var nullCheckSource = NullCheckSource(profileMap, sourceParameter, destinationParameter, objectMapperExpression, propertyMap);
            return ExpressionFactory.ToType(nullCheckSource, typePair.DestinationType);
        }

        public static Expression NullCheckSource(ProfileMap profileMap,
            Expression sourceParameter,
            Expression destinationParameter,
            Expression objectMapperExpression,
            IMemberMap memberMap)
        {
            var declaredDestinationType = destinationParameter.Type;
            var destinationType = objectMapperExpression.Type;
            var defaultDestination = DefaultDestination(destinationType, declaredDestinationType, profileMap);
            var destination = memberMap == null
                ? destinationParameter.IfNullElse(defaultDestination, destinationParameter)
                : memberMap.UseDestinationValue ? destinationParameter : defaultDestination;
            var ifSourceNull = destinationParameter.Type.IsCollectionType() ? ClearDestinationCollection() : destination;
            return sourceParameter.IfNullElse(ifSourceNull, objectMapperExpression);
            Expression ClearDestinationCollection()
            {
                var destinationElementType = ElementTypeHelper.GetElementType(destinationParameter.Type);
                var destinationCollectionType = typeof(ICollection<>).MakeGenericType(destinationElementType);
                var destinationVariable = Expression.Variable(destinationCollectionType, "collectionDestination");
                var clear = Expression.Call(destinationVariable, destinationCollectionType.GetDeclaredMethod("Clear"));
                var isReadOnly = Expression.Property(destinationVariable, "IsReadOnly");
                return Expression.Block(new[] {destinationVariable},
                    Expression.Assign(destinationVariable, ExpressionFactory.ToType(destinationParameter, destinationCollectionType)),
                    Expression.Condition(Expression.OrElse(Expression.Equal(destinationVariable, Expression.Constant(null)), isReadOnly), Expression.Empty(), clear),
                    destination);
            }
        }

        private static Expression DefaultDestination(Type destinationType, Type declaredDestinationType, ProfileMap profileMap)
        {
            if(profileMap.AllowNullCollections || destinationType == typeof(string) || !destinationType.IsEnumerableType())
            {
                return Expression.Default(declaredDestinationType);
            }
            if(destinationType.IsArray)
            {
                var destinationElementType = destinationType.GetElementType();
                return Expression.NewArrayBounds(destinationElementType, Enumerable.Repeat(Expression.Constant(0), destinationType.GetArrayRank()));
            }
            return DelegateFactory.GenerateNonNullConstructorExpression(destinationType);
        }

        private static Expression ObjectMapperExpression(IConfigurationProvider configurationProvider,
            ProfileMap profileMap, TypePair typePair, Expression sourceParameter, Expression contextParameter,
            IMemberMap propertyMap, Expression destinationParameter)
        {
            var match = configurationProvider.FindMapper(typePair);
            if (match != null)
            {
                var mapperExpression = match.MapExpression(configurationProvider, profileMap, propertyMap,
                    sourceParameter, destinationParameter, contextParameter);
                return mapperExpression;
            }
            return ContextMap(typePair, sourceParameter, contextParameter, destinationParameter, propertyMap);
        }

        public static Expression ContextMap(TypePair typePair, Expression sourceParameter, Expression contextParameter,
            Expression destinationParameter, IMemberMap memberMap)
        {
            var mapMethod = ContextMapMethod.MakeGenericMethod(typePair.SourceType, typePair.DestinationType);
            return Expression.Call(contextParameter, mapMethod, sourceParameter, destinationParameter, Expression.Constant(memberMap, typeof(IMemberMap)));
        }

        public static ConditionalExpression CheckContext(TypeMap typeMap, Expression context)
        {
            if (typeMap.MaxDepth > 0 || typeMap.PreserveReferences)
            {
                var mapper = Expression.Property(context, "Mapper");
                return Expression.IfThen(Expression.Property(context, "IsDefault"), Expression.Assign(context, Expression.Invoke(CreateContext, mapper)));
            }
            return null;
        }

    }
}