using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hanno.Extensions;
using Moq;
using Moq.Language.Flow;
using Ploeh.Albedo;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Kernel;

namespace Hanno.Testing.Autofixture
{
	internal class MockTasksPostProcessor : ISpecimenBuilder
	{
		private readonly MockPostprocessor _postprocessor;

		public MockTasksPostProcessor(MockPostprocessor postprocessor)
		{
			if (postprocessor == null) throw new ArgumentNullException("postprocessor");
			_postprocessor = postprocessor;
		}

		public object Create(object request, ISpecimenContext context)
		{
			var specimen = _postprocessor.Create(request, context);
			if (specimen.GetType() == typeof (NoSpecimen))
			{
				return specimen;
			}
			var mockedType = GetMockedType(specimen);
			var methodsWithTasks = GetMethodsWithTasks(mockedType);
			var isAny = typeof (It).GetMethod("IsAny");
			foreach (var method in methodsWithTasks)
			{
				var mockSetupMethod = GetMockSetupMethod(mockedType, method.ReturnType);
				var mockReturnMethod = GetMockReturnsMethod(mockedType, method.ReturnType);
				var returnTaskType = GetReturnType(method.ReturnType);
				var returnValue = context.Resolve(returnTaskType);
				var returnTask = typeof (Task).GetMethod("FromResult")
				                              .MakeGenericMethod(returnTaskType)
				                              .Invoke(null, new object[] {returnValue});
				var parameters = method.GetParameters()
				                       .Select(info =>(Expression) Expression.Constant(isAny.MakeGenericMethod(info.ParameterType).Invoke(null, new object[] {})))
				                       .ToArray();
				var parameter = Expression.Parameter(mockedType);
				var body = Expression.Call(parameter, method, parameters);
				var lambda = Expression.Lambda(body, parameter);
				var setup = mockSetupMethod.Invoke(specimen, new object[] {lambda});
				mockReturnMethod.Invoke(setup, new object[] {returnTask});
			}
			return specimen;
		}
		
		private Type GetMockedType(object specimen)
		{
			var type = specimen.GetType();
			var genericTypeDefinition = type.GetGenericTypeDefinition();
			if (genericTypeDefinition != typeof(Mock<>))
			{
				throw new ArgumentException("The type is not a instance of Mock<T>", "specimen");
			}
			return type.GetGenericArguments()[0];
		}

		private Type GetReturnType(Type returnType)
		{
			if (returnType.GetGenericTypeDefinition() != typeof (Task<>))
			{
				throw new ArgumentException("The return type of the method is not a valid task", "returnType");
			}
			return returnType.GetGenericArguments()[0];
		}
		
		private MethodInfo GetMockSetupMethod(Type mockedType, Type returnType)
		{
			var funcType = typeof (Func<,>).MakeGenericType(mockedType, returnType);
			return typeof (Mock<>).MakeGenericType(mockedType).GetMethod("Setup", new[] {funcType});
		}

		private MethodInfo GetMockReturnsMethod(Type mockedType, Type returnType)
		{
			var setupType = typeof (ISetup<,>).MakeGenericType(mockedType, returnType);
			return setupType.GetMethod("Returns", new[] {returnType});
		}

		private IEnumerable<MethodInfo> GetMethodsWithTasks(Type type)
		{
			return type.GetMethods()
			           .Where(mi => mi.ReturnType.IsGenericType && mi.ReturnType.GetGenericTypeDefinition() == typeof (Task<>));
		}
	}
}
