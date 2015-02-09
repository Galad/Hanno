using System;
using System.Linq;
using System.Linq.Expressions;
using Hanno.Extensions;
using Ploeh.AutoFixture.Idioms;

namespace Ploeh.AutoFixture.Idioms
{
    public static class GuardClauseAssertionExtensions
    {
        public static void Verify(this IIdiomaticAssertion assertion, Expression<Action> action)
        {
            assertion.Verify(((MethodCallExpression)action.Body).Method);
        }

        public static void Verify<T1>(this IIdiomaticAssertion assertion, Expression<Action<T1>> action)
        {
            assertion.Verify(((MethodCallExpression)action.Body).Method);
        }

        public static void Verify<TSut, T1, T2>(this IIdiomaticAssertion assertion, Expression<Action<TSut, T1, T2>> action)
        {
            assertion.Verify(((MethodCallExpression)action.Body).Method);
        }

        public static void VerifyProperty<T1>(this IIdiomaticAssertion assertion, Expression<Func<T1>> action)
        {
            assertion.Verify(action.GetMemberInfo());
        }

        public static void VerifyProperties(this IIdiomaticAssertion assertion, params Expression<Func<object>>[] actions)
        {
            var memberInfos = actions.Select(expression => expression.GetMemberInfo()).ToArray();
            assertion.Verify(memberInfos);
        }

		/// <summary>
		/// Verify guard clauses for the first constructor
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="assertion"></param>
		/// <param name="actions"></param>
	    public static void VerifyConstructors<T>(this IIdiomaticAssertion assertion)
		{
			var constructor = typeof (T).GetConstructors();
			assertion.Verify(constructor);
		}

		/// <summary>
		/// Verify guard clauses for the first constructor having parameters
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="assertion"></param>
		/// <param name="actions"></param>
		public static void VerifyConstructorHavingParameter<T>(this IIdiomaticAssertion assertion)
		{
			var constructor = typeof (T).GetConstructors().First(c => c.GetParameters().Any());
            assertion.Verify(constructor);
		}
    }
}
