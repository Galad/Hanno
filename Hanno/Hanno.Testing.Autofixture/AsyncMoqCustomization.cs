using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hanno.Extensions;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Kernel;

namespace Hanno.Testing.Autofixture
{
	public class AsyncMoqCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.Register<MemoryStream>(() => new MemoryStream());
			//fixture.Customizations.Add(
			//	new AsyncMoqPostProcessor(
			//		new MockPostprocessor(
			//			new MethodInvoker(
			//				new MockConstructorQuery())));
			//fixture.Customizations.Add(new TaskSpecimenBuilder());
			//var relay = new MockRelay();
		}
	}

	//public class AsyncMoqPostProcessor : ISpecimenBuilder
	//{
	//	private readonly ISpecimenBuilder _innerBuilder;

	//	public AsyncMoqPostProcessor(ISpecimenBuilder innerBuilder)
	//	{
	//		_innerBuilder = innerBuilder;
	//	}

	//	public object Create(object request, ISpecimenContext context)
	//	{
	//		var obj = _innerBuilder.Create(request, context);
	//		if (obj.IsAssignableTo<NoSpecimen>())
	//		{
	//			return obj;
	//		}
	//		if (obj.IsAssignableTo<Mock>() && obj.GetType().IsGenericType)
	//		{
	//			Mock a;
	//			a.DefaultValue
	//		}
	//	}
	//}

	//public class AsyncMockRelay : ISpecimenBuilder
	//{
	//	private readonly ISpecimenBuilder _innerRelay;

	//	public AsyncMockRelay(ISpecimenBuilder innerRelay)
	//	{
	//		if (innerRelay == null) throw new ArgumentNullException("innerRelay");
	//		_innerRelay = innerRelay;
	//	}

	//	public object Create(object request, ISpecimenContext context)
	//	{
	//		var type = request as Type;
	//		if (type == null)
	//		{
	//			return new NoSpecimen(request);
	//		}
	//		if (!type.IsAssignableTo<Mock>())
	//		{
	//			return new NoSpecimen(request);
	//		}
	//		var result = _innerRelay.Create(request, context);
	//		if (result is NoSpecimen)
	//		{
	//			return result;
	//		}
	//		var mock = result as Mock;
	//		if (mock == null)
	//		{
	//			return result;
	//		}
	//		mock.GetType().GetMethod("Setup");
	//		return null;
	//	}
	//}

	//public class TaskSpecimenBuilder : ISpecimenBuilder
	//{
	//	public object Create(object request, ISpecimenContext context)
	//	{
	//		var type = request as Type;
	//		if (type == null)
	//		{
	//			return new NoSpecimen(request);
	//		}
	//		if (!type.IsAssignableTo<Task>())
	//		{
	//			return new NoSpecimen(request);
	//		}
	//		if (type == typeof(Task))
	//		{
	//			return Task.FromResult(new object());
	//		}
	//		var taskReturnType = type.GenericTypeArguments.First();
	//		var taskReturnResult = context.Resolve(taskReturnType);
	//		var task = typeof(Type).GetMethod("FromResult").MakeGenericMethod(taskReturnType).Invoke(null, new[] { taskReturnResult });
	//		return task;
	//	}
	//}
}
