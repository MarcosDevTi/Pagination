using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Arch.Infra.Shared.EventSourcing
{
    public static class EventSourceExtensions
    {
        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            var baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }

        public static MemberInfo GetMember<T, TProperty>(this Expression<Func<T, TProperty>> expression)
        {
            var memberExp = RemoveUnary(expression.Body) as MemberExpression;

            if (memberExp == null)
            {
                return null;
            }

            var currentExpr = memberExp.Expression;

            while (true)
            {
                currentExpr = RemoveUnary(currentExpr);

                if (currentExpr != null && currentExpr.NodeType == ExpressionType.MemberAccess)
                {
                    currentExpr = ((MemberExpression)currentExpr).Expression;
                }
                else
                {
                    break;
                }
            }

            if (currentExpr == null || currentExpr.NodeType != ExpressionType.Parameter)
            {
                return null;
            }

            return memberExp.Member;
        }

        private static Expression RemoveUnary(Expression toUnwrap) =>
            toUnwrap is UnaryExpression expression ? expression.Operand : toUnwrap;
    }
}
