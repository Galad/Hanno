using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Reflection;
using Hanno.Concurrency;
using Microsoft.Reactive.Testing;
using Ploeh.AutoFixture;
using Hanno.Rx;
using Ploeh.AutoFixture.Kernel;

namespace Hanno.Testing.Autofixture
{
	public class RxCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			var testScheduler = new TestSchedulers();
			fixture.Register<IScheduler>(() => testScheduler);
			fixture.Register<ISchedulers>(() => testScheduler);
			fixture.Register<TestScheduler>(() => testScheduler);
			fixture.Register<ThrowingTestScheduler>(() => testScheduler);
			fixture.Register<IPriorityScheduler>(() => testScheduler);
			fixture.Register(() => testScheduler);
			fixture.Register<Unit>(() => Unit.Default);
			fixture.Customizations.Add(new ReplaySubjectSpecimenBuilder());
		}
	}

	public class ReplaySubjectSpecimenBuilder : Ploeh.AutoFixture.Kernel.ISpecimenBuilder
	{
		public object Create(object request, ISpecimenContext context)
		{
			var type = request as Type;
			if (type == null)
			{
				return new NoSpecimen(request);
			}
			if (type.GetTypeInfo().IsGenericType &&
			    type.GetTypeInfo().GetGenericTypeDefinition() == typeof (ReplaySubject<>))
			{
				return Activator.CreateInstance(type, 1, context.Resolve(typeof (IScheduler)));
			}
			return new NoSpecimen(request);
		}
	}
}