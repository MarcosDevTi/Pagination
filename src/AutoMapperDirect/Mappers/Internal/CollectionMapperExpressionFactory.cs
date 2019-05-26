using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapperDirect.Execution;
using AutoMapperDirect.Internal;

namespace AutoMapperDirect.Mappers.Internal
{
    public static class CollectionMapperExpressionFactory
    {
        public delegate Expression MapItem(IConfigurationProvider configurationProvider, ProfileMap profileMap,
            Type sourceType, Type destType, Expression contextParam,
            out ParameterExpression itemParam);

        public static Expression MapCollectionExpression(IConfigurationProvider configurationProvider,
            ProfileMap profileMap, IMemberMap memberMap, Expression sourceExpression, Expression destExpression,
            Expression contextExpression, Type ifInterfaceType, MapItem mapItem)
        {
            var passedDestination = Expression.Variable(destExpression.Type, "passedDestination");
            var newExpression = Expression.Variable(passedDestination.Type, "collectionDestination");
            var sourceElementType = ElementTypeHelper.GetElementType(sourceExpression.Type);

            var itemExpr = mapItem(configurationProvider, profileMap, sourceExpression.Type, passedDestination.Type,
                contextExpression, out ParameterExpression itemParam);

            var destinationElementType = itemExpr.Type;
            var destinationCollectionType = typeof(ICollection<>).MakeGenericType(destinationElementType);
            if (!destinationCollectionType.IsAssignableFrom(destExpression.Type))
                destinationCollectionType = typeof(IList);
            var addMethod = destinationCollectionType.GetDeclaredMethod("Add");

            Expression destination, assignNewExpression;

            UseDestinationValue();

            var addItems = ExpressionFactory.ForEach(sourceExpression, itemParam, Expression.Call(destination, addMethod, itemExpr));
            var mapExpr = Expression.Block(addItems, destination);

            var clearMethod = destinationCollectionType.GetDeclaredMethod("Clear");
            var checkNull =
                Expression.Block(new[] { newExpression, passedDestination },
                    Expression.Assign(passedDestination, destExpression),
                    assignNewExpression,
                    Expression.Call(destination, clearMethod),
                    mapExpr
                );
            if (memberMap != null)
                return checkNull;
            var elementTypeMap = configurationProvider.ResolveTypeMap(sourceElementType, destinationElementType);
            if (elementTypeMap == null)
                return checkNull;
            var checkContext = ExpressionBuilder.CheckContext(elementTypeMap, contextExpression);
            if (checkContext == null)
                return checkNull;
            return Expression.Block(checkContext, checkNull);
            void UseDestinationValue()
            {
                if(memberMap?.UseDestinationValue == true)
                {
                    destination = passedDestination;
                    assignNewExpression = Expression.Empty();
                }
                else
                {
                    destination = newExpression;
                    Expression createInstance = passedDestination.Type.NewExpr(ifInterfaceType);
                    var isReadOnly = Expression.Property(ExpressionFactory.ToType(passedDestination, destinationCollectionType), "IsReadOnly");
                    assignNewExpression = Expression.Assign(newExpression,
                        Expression.Condition(Expression.OrElse(Expression.Equal(passedDestination, Expression.Constant(null)), isReadOnly), ExpressionFactory.ToType(createInstance, passedDestination.Type), passedDestination));
                }
            }
        }

        private static Expression NewExpr(this Type baseType, Type ifInterfaceType)
        {
            var newExpr = baseType.IsInterface()
                ? Expression.New(
                    ifInterfaceType.MakeGenericType(ElementTypeHelper.GetElementTypes(baseType,
                        ElementTypeFlags.BreakKeyValuePair)))
                : DelegateFactory.GenerateConstructorExpression(baseType);
            return newExpr;
        }

        public static Expression MapItemExpr(IConfigurationProvider configurationProvider, ProfileMap profileMap, Type sourceType, Type destType, Expression contextParam, out ParameterExpression itemParam)
        {
            var sourceElementType = ElementTypeHelper.GetElementType(sourceType);
            var destElementType = ElementTypeHelper.GetElementType(destType);
            itemParam = Expression.Parameter(sourceElementType, "item");

            var typePair = new TypePair(sourceElementType, destElementType);

            var itemExpr = ExpressionBuilder.MapExpression(configurationProvider, profileMap, typePair, itemParam, contextParam);
            return ExpressionFactory.ToType(itemExpr, destElementType);
        }

        public static Expression MapKeyPairValueExpr(IConfigurationProvider configurationProvider, ProfileMap profileMap, Type sourceType, Type destType, Expression contextParam, out ParameterExpression itemParam)
        {
            var sourceElementTypes = ElementTypeHelper.GetElementTypes(sourceType, ElementTypeFlags.BreakKeyValuePair);
            var destElementTypes = ElementTypeHelper.GetElementTypes(destType, ElementTypeFlags.BreakKeyValuePair);

            var typePairKey = new TypePair(sourceElementTypes[0], destElementTypes[0]);
            var typePairValue = new TypePair(sourceElementTypes[1], destElementTypes[1]);

            var sourceElementType = typeof(KeyValuePair<,>).MakeGenericType(sourceElementTypes);
            itemParam = Expression.Parameter(sourceElementType, "item");
            var destElementType = typeof(KeyValuePair<,>).MakeGenericType(destElementTypes);

            var keyExpr = ExpressionBuilder.MapExpression(configurationProvider, profileMap, typePairKey,
                Expression.Property(itemParam, "Key"), contextParam);
            var valueExpr = ExpressionBuilder.MapExpression(configurationProvider, profileMap, typePairValue,
                Expression.Property(itemParam, "Value"), contextParam);
            var keyPair = Expression.New(destElementType.GetDeclaredConstructors().First(), keyExpr, valueExpr);
            return keyPair;
        }
    }
}