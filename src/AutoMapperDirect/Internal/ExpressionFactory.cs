﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using AutoMapperDirect.Configuration;

namespace AutoMapperDirect.Internal
{
    public static class ExpressionFactory
    {
        public static LambdaExpression MemberAccessLambda(Type type, string propertyOrField) =>
            MemberAccessLambda(type.GetFieldOrProperty(propertyOrField));

        public static LambdaExpression MemberAccessLambda(MemberInfo propertyOrField)
        {
            var source = Expression.Parameter(propertyOrField.DeclaringType, "source");
            return Expression.Lambda(Expression.MakeMemberAccess(source, propertyOrField), source);
        }


        public static MemberExpression MemberAccesses(string members, Expression obj) =>
            (MemberExpression) ReflectionHelper.GetMemberPath(obj.Type, members).MemberAccesses(obj);

        public static Expression GetSetter(MemberExpression memberExpression)
        {
            var propertyOrField = memberExpression.Member;
            return ReflectionHelper.CanBeSet(propertyOrField) ?
                        Expression.MakeMemberAccess(memberExpression.Expression, propertyOrField) :
                        null;
        }

        public static MethodInfo Method<T>(Expression<Func<T>> expression) => GetExpressionBodyMethod(expression);

        public static MethodInfo Method<TType, TResult>(Expression<Func<TType, TResult>> expression) => GetExpressionBodyMethod(expression);

        private static MethodInfo GetExpressionBodyMethod(LambdaExpression expression) => ((MethodCallExpression) expression.Body).Method;

        public static Expression ForEach(Expression collection, ParameterExpression loopVar, Expression loopContent)
        {
            if(collection.Type.IsArray)
            {
                return ForEachArrayItem(collection, arrayItem => Expression.Block(new[] { loopVar }, Expression.Assign(loopVar, arrayItem), loopContent));
            }
            var getEnumerator = collection.Type.GetInheritedMethod("GetEnumerator");
            var getEnumeratorCall = Expression.Call(collection, getEnumerator);
            var enumeratorType = getEnumeratorCall.Type;
            var enumeratorVar = Expression.Variable(enumeratorType, "enumerator");
            var enumeratorAssign = Expression.Assign(enumeratorVar, getEnumeratorCall);

            var moveNext = enumeratorType.GetInheritedMethod("MoveNext");
            var moveNextCall = Expression.Call(enumeratorVar, moveNext);

            var breakLabel = Expression.Label("LoopBreak");

            var loop = Expression.Block(new[] { enumeratorVar },
                enumeratorAssign,
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.Equal(moveNextCall, Expression.Constant(true)),
                        Expression.Block(new[] { loopVar },
                            Expression.Assign(loopVar, ToType(Expression.Property(enumeratorVar, "Current"), loopVar.Type)),
                            loopContent
                        ),
                        Expression.Break(breakLabel)
                    ),
                breakLabel)
            );

            return loop;
        }

        public static Expression ForEachArrayItem(Expression array, Func<Expression, Expression> body)
        {
            var length = Expression.Property(array, "Length");
            return For(length, index => body(Expression.ArrayAccess(array, index)));
        }

        public static Expression For(Expression count, Func<Expression, Expression> body)
        {
            var breakLabel = Expression.Label("LoopBreak");
            var index = Expression.Variable(typeof(int), "sourceArrayIndex");
            var initialize = Expression.Assign(index, Expression.Constant(0, typeof(int)));
            var loop = Expression.Block(new[] { index },
                initialize,
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.LessThan(index, count),
                        Expression.Block(body(index), Expression.PostIncrementAssign(index)),
                        Expression.Break(breakLabel)
                    ),
                breakLabel)
            );
            return loop;
        }

        public static Expression ToObject(Expression expression) => ToType(expression, typeof(object));

        public static Expression ToType(Expression expression, Type type) => expression.Type == type ? expression : Expression.Convert(expression, type);

        public static Expression ReplaceParameters(LambdaExpression exp, params Expression[] replace)
        {
            var replaceExp = exp.Body;
            for (var i = 0; i < Math.Min(replace.Length, exp.Parameters.Count); i++)
                replaceExp = Replace(replaceExp, exp.Parameters[i], replace[i]);
            return replaceExp;
        }

        public static Expression ConvertReplaceParameters(LambdaExpression exp, params Expression[] replace)
        {
            var replaceExp = exp.Body;
            for (var i = 0; i < Math.Min(replace.Length, exp.Parameters.Count); i++)
                replaceExp = new ConvertingVisitor(exp.Parameters[i], replace[i]).Visit(replaceExp);
            return replaceExp;
        }

        public static Expression Replace(Expression exp, Expression old, Expression replace) => new ReplaceExpressionVisitor(old, replace).Visit(exp);

        public static LambdaExpression Concat(LambdaExpression expr, LambdaExpression concat) => (LambdaExpression)new ExpressionConcatVisitor(expr).Visit(concat);

        public static Expression NullCheck(Expression expression, Type destinationType)
        {
            var target = expression;
            Expression nullConditions = Expression.Constant(false);
            do
            {
                if(target is MemberExpression member)
                {
                    target = member.Expression;
                    NullCheck();
                }
                else if(target is MethodCallExpression methodCall)
                {
                    var isStatic = methodCall.Method.IsStatic;
                    if (isStatic)
                    {
                        var parameters = methodCall.Method.GetParameters();
                        if (parameters.Length == 0 || !methodCall.Method.Has<ExtensionAttribute>())
                        {
                            return expression;
                        }
                        target = methodCall.Arguments[0];
                    }
                    else
                    {
                        target = methodCall.Object;
                    }
                    NullCheck();
                }
                else if(target?.NodeType == ExpressionType.Parameter)
                {
                    var returnType = Nullable.GetUnderlyingType(destinationType) == expression.Type ? destinationType : expression.Type;
                    var nullCheck = Expression.Condition(nullConditions, Expression.Default(returnType), ToType(expression, returnType));
                    return nullCheck;
                }
                else
                {
                    return expression;
                }
            }
            while(true);
            void NullCheck()
            {
                if(target == null || target.Type.IsValueType())
                {
                    return;
                }
                nullConditions = Expression.OrElse(Expression.Equal(target, Expression.Constant(null, target.Type)), nullConditions);
            }
        }

        public static Expression IfNullElse(Expression expression, Expression then, Expression @else = null)
        {
            var nonNullElse = ToType(@else ?? Expression.Default(then.Type), then.Type);
            if(expression.Type.IsValueType() && !expression.Type.IsNullableType())
            {
                return nonNullElse;
            }
            return Expression.Condition(Expression.Equal(expression, Expression.Constant(null)), then, nonNullElse);
        }

        internal class ConvertingVisitor : ExpressionVisitor
        {
            private readonly Expression _newParam;
            private readonly ParameterExpression _oldParam;

            public ConvertingVisitor(ParameterExpression oldParam, Expression newParam)
            {
                _newParam = newParam;
                _oldParam = oldParam;
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Expression == _oldParam)
                {
                    node = Expression.MakeMemberAccess(ToType(_newParam, _oldParam.Type), node.Member);
                }

                return base.VisitMember(node);
            }

            protected override Expression VisitParameter(ParameterExpression node) => 
                node == _oldParam 
                    ? ToType(_newParam, _oldParam.Type) 
                    : base.VisitParameter(node);

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Object == _oldParam)
                {
                    node = Expression.Call(ToType(_newParam, _oldParam.Type), node.Method, node.Arguments);
                }

                return base.VisitMethodCall(node);
            }
        }

        internal class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldExpression;
            private readonly Expression _newExpression;

            public ReplaceExpressionVisitor(Expression oldExpression, Expression newExpression)
            {
                _oldExpression = oldExpression;
                _newExpression = newExpression;
            }

            public override Expression Visit(Expression node)
            {
                if (_oldExpression == node)
                    node = _newExpression;

                return base.Visit(node);
            }
        }

        internal class ExpressionConcatVisitor : ExpressionVisitor
        {
            private readonly LambdaExpression _overrideExpression;

            public ExpressionConcatVisitor(LambdaExpression overrideExpression) => _overrideExpression = overrideExpression;

            public override Expression Visit(Expression node)
            {
                if (_overrideExpression == null)
                    return node;
                if (node.NodeType != ExpressionType.Lambda && node.NodeType != ExpressionType.Parameter)
                {
                    var expression = node;
                    if (node.Type == typeof(object))
                        expression = Expression.Convert(node, _overrideExpression.Parameters[0].Type);

                    return ReplaceParameters(_overrideExpression, expression);
                }
                return base.Visit(node);
            }

            protected override Expression VisitLambda<T>(Expression<T> node) => Expression.Lambda(Visit(node.Body), node.Parameters);
        }
    }
}
