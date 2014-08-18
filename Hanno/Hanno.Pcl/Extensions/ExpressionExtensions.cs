using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Hanno.Extensions
{
    public static class ExpressionExtensions
    {
        [System.Diagnostics.DebuggerStepThrough]
        public static string GetPropertyName<T>(this Expression<Func<T>> expression)
        {
            var memberInfo = expression.GetMemberInfo();
            return memberInfo.Name;
        }

		public static string GetPropertyName<T, T2>(this Expression<Func<T, T2>> expression)
		{
			var memberInfo = expression.GetMemberInfo();
			return memberInfo.Name;
		}

        public static MemberInfo GetMemberInfo(this LambdaExpression expression)
        {
            MemberExpression memberExpression;

            var unaryExpression = expression.Body as UnaryExpression;
            if (unaryExpression != null)
            {
                memberExpression = (MemberExpression) unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)expression.Body;
            }

            var memberInfo = memberExpression.Member;
            return memberInfo;
        }
    }
}