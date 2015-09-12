using System;
using Ploeh.AutoFixture.Kernel;
using Moq;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using Ploeh.AutoFixture.AutoMoq;
using Moq.Language.Flow;
using Moq.Language;
using System.Globalization;

namespace Hanno.Testing.Autofixture
{
    internal class StubReadonlyPropertiesCommand : ISpecimenCommand
    {
        public StubReadonlyPropertiesCommand()
        {
        }

        public void Execute(object specimen, ISpecimenContext context)
        {
            var mock = specimen as Mock;
            if (mock == null)
            {
                return;
            }
            var mockedType = GetMockedType(mock.GetType());
            var methods = GetMethods(mockedType);
            foreach (var method in methods)
            {
                var returnType = method.ReturnType;
                var methodInvocationLambda = MakeMethodInvocationLambda(mockedType, method, context);
                if (methodInvocationLambda != null)
                {
                    this.GetType()
                        .GetMethod("SetupMethod", BindingFlags.NonPublic | BindingFlags.Static)
                        .MakeGenericMethod(mockedType, returnType)
                        .Invoke(this, new object[] { mock, methodInvocationLambda, context });
                }
            }
        }

        private MethodInfo[] GetMethods(Type mockedType)
        {
            return mockedType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.CanRead && !p.CanWrite && p.GetMethod.IsAbstract)
                        .Select(p => p.GetMethod)
                        .ToArray();
        }

        private static Expression MakeMethodInvocationLambda(Type mockedType, MethodInfo method,
                                                              ISpecimenContext context)
        {
            var lambdaParam = Expression.Parameter(mockedType, "x");
            //e.g. "x.Method(It.IsAny<string>(), out parameter)" 
            var methodCall = Expression.Call(lambdaParam, method);
            //e.g. "x => x.Method(It.IsAny<string>(), out parameter)" 
            return Expression.Lambda(methodCall, lambdaParam);
        }

        private static Type GetMockedType(Type type)
        {
            return type.GetGenericArguments().Single();
        }

        private static void SetupMethod<TMock, TResult>(Mock<TMock> mock, Expression<Func<TMock, TResult>> methodCallExpression, ISpecimenContext context) where TMock : class
        {
            if (mock == null) throw new ArgumentNullException("mock");
            ReturnsUsingContext(mock.Setup(methodCallExpression), context);
        }

        internal static IReturnsResult<TMock> ReturnsUsingContext<TMock, TResult>(IReturns<TMock, TResult> setup,
            ISpecimenContext context)
            where TMock : class
        {
            return setup.Returns(() =>
            {
                var specimen = context.Resolve(typeof(TResult));


                // check if specimen is null but member is non-nullable value type 
                if (specimen == null && (default(TResult) != null))
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            "Tried to setup a member with a return type of {0}, but null was found instead.",
                            typeof(TResult)));


                // check if specimen can be safely converted to TResult 
                if (specimen != null && !(specimen is TResult))
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            "Tried to setup a member with a return type of {0}, but an instance of {1} was found instead.",
                            typeof(TResult),
                             specimen.GetType()));


                TResult result = (TResult)specimen;


                //"cache" value for future invocations 
                setup.Returns(result);
                return result;
            });
        }

    }
}