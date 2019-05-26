using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using AutoMapperDirect.Configuration;
using AutoMapperDirect.Internal;
using AutoMapperDirect.Mappers.Internal;

namespace AutoMapperDirect.Execution
{
    public static class DelegateFactory
    {
        private static readonly LockingConcurrentDictionary<Type, Func<object>> CtorCache = new LockingConcurrentDictionary<Type, Func<object>>(GenerateConstructor);

        public static Func<object> CreateCtor(Type type) => CtorCache.GetOrAdd(type);

        private static Func<object> GenerateConstructor(Type type)
        {
            var ctorExpr = GenerateConstructorExpression(type);

            return Expression.Lambda<Func<object>>(Expression.Convert(ctorExpr, typeof(object))).Compile();
        }

        public static Expression GenerateConstructorExpression(Type type, ProfileMap configuration) =>
            configuration.AllowNullDestinationValues
                ? GenerateConstructorExpression(type)
                : GenerateNonNullConstructorExpression(type);

        public static Expression GenerateNonNullConstructorExpression(Type type) => type.IsValueType()
            ? Expression.Default(type)
            : (type == typeof(string)
                ? Expression.Constant(string.Empty)
                : GenerateConstructorExpression(type)
            );

        public static Expression GenerateConstructorExpression(Type type)
        {
            if (type.IsValueType())
            {
                return Expression.Default(type);
            }

            if (type == typeof(string))
            {
                return Expression.Constant(null, typeof(string));
            }

            if (type.IsInterface())
            {
                return
                    type.IsDictionaryType() ? CreateCollection(type, typeof(Dictionary<,>))
                    : type.IsReadOnlyDictionaryType() ? CreateReadOnlyCollection(type, typeof(ReadOnlyDictionary<,>))
                    : type.IsSetType() ? CreateCollection(type, typeof(HashSet<>))
                    : type.IsEnumerableType() ? CreateCollection(type, typeof(List<>))
                    : InvalidType(type, $"Cannot create an instance of interface type {type}.");
            }

            if (type.IsAbstract())
            {
                return InvalidType(type, $"Cannot create an instance of abstract type {type}.");
            }

            var constructors = type
                .GetDeclaredConstructors()
                .Where(ci => !ci.IsStatic);

            //find a ctor with only optional args
            var ctorWithOptionalArgs = constructors.FirstOrDefault(c => c.GetParameters().All(p => p.IsOptional));
            if (ctorWithOptionalArgs == null)
            {
                return InvalidType(type, $"{type} needs to have a constructor with 0 args or only optional args.");
            }
            //get all optional default values
            var args = ctorWithOptionalArgs
                .GetParameters()
                .Select(p => Expression.Constant(p.GetDefaultValue(), p.ParameterType)).ToArray();

            //create the ctor expression
            return Expression.New(ctorWithOptionalArgs, args);
        }

        private static Expression CreateCollection(Type type, Type collectionType)
        {
            var listType = collectionType.MakeGenericType(ElementTypeHelper.GetElementTypes(type, ElementTypeFlags.BreakKeyValuePair));
            if (type.IsAssignableFrom(listType))
                return ExpressionFactory.ToType(Expression.New(listType), type);

            return InvalidType(type, $"Cannot create an instance of interface type {type}.");
        }

        private static Expression CreateReadOnlyCollection(Type type, Type collectionType)
        {
            var listType = collectionType.MakeGenericType(ElementTypeHelper.GetElementTypes(type, ElementTypeFlags.BreakKeyValuePair));
            var ctor = listType.GetConstructors()[0];
            var innerType = ctor.GetParameters()[0].ParameterType;
            if (type.IsAssignableFrom(listType))
                return ExpressionFactory.ToType(Expression.New(ctor, GenerateConstructorExpression(innerType)), type);

            return InvalidType(type, $"Cannot create an instance of interface type {type}.");
        }

        private static Expression InvalidType(Type type, string message)
        {
            var ex = new ArgumentException(message, "type");
            return Expression.Block(Expression.Throw(Expression.Constant(ex)), Expression.Constant(null, type));
        }
    }
}