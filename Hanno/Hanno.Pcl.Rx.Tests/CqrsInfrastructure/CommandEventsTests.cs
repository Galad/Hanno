using System;
using System.Linq;
using System.Reactive.Subjects;
using FluentAssertions;
using Hanno.CqrsInfrastructure;
using Hanno.Testing.Autofixture;
using Microsoft.Reactive.Testing;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;
using System.Reactive.Concurrency;

namespace Hanno.Tests.CqrsInfrastructure
{
	#region Customization
	public class CommandEventsCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.Register<ISubject<CommandEventArgs>>(fixture.Create<ReplaySubject<CommandEventArgs>>);
		}
	}

	public class CommandEventsCompositeCustomization : CompositeCustomization
	{
		public CommandEventsCompositeCustomization()
			: base(new AutoMoqCustomization(), new CommandEventsCustomization(), new RxCustomization())
		{
		}
	}

	public class CommandEventsAutoDataAttribute : AutoDataAttribute
	{
		public CommandEventsAutoDataAttribute()
			: base(new Fixture().Customize(new CommandEventsCompositeCustomization()))
		{
		}
	}

	public class CommandEventsInlineAutoDataAttribute : CompositeDataAttribute
	{
		public CommandEventsInlineAutoDataAttribute(params object[] values)
			: base(new InlineDataAttribute(values), new CommandEventsAutoDataAttribute())
		{
		}
	}
	#endregion

	public class TestCommand : IAsyncCommandEvent<string>
	{
	}

	public class TestCommandWithMultipleEvents : IAsyncCommandEvent<int>, IAsyncCommandEvent<string>
	{
	}

	public class CommandEventsTests
	{
		[Theory, RxAutoData]
		public void ObserveCommandEvents_WhenNotifying_ShouldYieldValue(
			[Frozen]TestScheduler scheduler,
			CommandEvents sut,
			TestCommand command,
			string value)
		{
			//arrange
			scheduler.Schedule(TimeSpan.FromTicks(202), () => sut.NotifyEvent(command, value));

			//act
			var resultObserver = scheduler.Start(() => sut.ObserveCommandEvents<TestCommand, string>(command), 500);

			//assert
			resultObserver.AssertExceptions();
			resultObserver.Values().ShouldAllBeEquivalentTo(new[] { value });
		}

		[Theory, RxAutoData]
		public void ObserveCommandEvents_CommandHaveMultipleEventsWhenNotifying_ShouldYieldValue(
			[Frozen]TestScheduler scheduler,
			CommandEvents sut,
			TestCommandWithMultipleEvents command,
			string value)
		{
			//arrange
			scheduler.Schedule(TimeSpan.FromTicks(202), () => sut.NotifyEvent(command, value));

			//act
			var resultObserver = scheduler.Start(() => sut.ObserveCommandEvents<TestCommandWithMultipleEvents, string>(command), 500);

			//assert
			resultObserver.AssertExceptions();
			resultObserver.Values().ShouldAllBeEquivalentTo(new[] { value });
		}

		[Theory, RxAutoData]
		public void ObserveCommandEvents_WhenNotifyingOtherType_ShouldNotYieldValue(
			[Frozen]TestScheduler scheduler,
			CommandEvents sut,
			TestCommandWithMultipleEvents command,
			int value)
		{
			//arrange
			scheduler.Schedule(TimeSpan.FromTicks(202), () => sut.NotifyEvent(command, value));

			//act
			var resultObserver = scheduler.Start(() => sut.ObserveCommandEvents<TestCommandWithMultipleEvents, string>(command), 500);

			//assert
			resultObserver.AssertExceptions();
			resultObserver.Values().Should().BeEmpty();
		}

		[Theory, RxAutoData]
		public void NotifyError_ShouldCallSubjectOnError(
			[Frozen]TestScheduler scheduler,
			CommandEvents sut,
			TestCommand command,
			Exception error)
		{
			//arrange
			scheduler.Schedule(TimeSpan.FromTicks(202), () => sut.NotifyError(command, error));

			//act
			var resultObserver = scheduler.Start(() => sut.ObserveCommandEvents<TestCommand, string>(command), 500);

			//assert
			resultObserver.Errors().ShouldAllBeEquivalentTo(new[] { error });
		}

		[Theory, RxAutoData]
		public void NotifyComplete_ShouldCallSubjectOnComplete(
			[Frozen]TestScheduler scheduler,
			CommandEvents sut,
			TestCommand command)
		{
			//arrange
			scheduler.Schedule(TimeSpan.FromTicks(202), () => sut.NotifyComplete(command));

			//act
			var resultObserver = scheduler.Start(() => sut.ObserveCommandEvents<TestCommand, string>(command), 500);

			//assert
			resultObserver.AssertExceptions();
			resultObserver.Completed().Should().HaveCount(1);
		}

		[Theory, CommandEventsAutoData]
		public void ObserveCommandEvents_ShouldReturnCorrectValue(
			[Frozen]ThrowingTestScheduler scheduler,
			CommandEvents sut,
			TestCommand command,
			string value)
		{

			//arrange
			scheduler.Schedule(TimeSpan.FromTicks(202), () => sut.NotifyEvent(command, value));

			//act
			var result = scheduler.Start(() => sut.ObserveCommandEvents<TestCommand, string>());

			//assert
			result.AssertExceptions();
			result.Values().First().Should().Be(value);
		}

		[Theory, CommandEventsAutoData]
		public void ObserveCommandEvents_WithoutCommandAndWithMultipleEventsWhenNotifying_ShouldReturnCorrectValue(
		[Frozen]ThrowingTestScheduler scheduler,
		CommandEvents sut,
		TestCommandWithMultipleEvents command,
		string value)
		{

			//arrange
			scheduler.Schedule(TimeSpan.FromTicks(202), () => sut.NotifyEvent(command, value));

			//act
			var result = scheduler.Start(() => sut.ObserveCommandEvents<TestCommandWithMultipleEvents, string>());

			//assert
			result.AssertExceptions();
			result.Values().First().Should().Be(value);
		}

		[Theory, CommandEventsAutoData]
		public void ObserveCommandEvents_WithoutCommandAndWhenNotifyingOtherType_ShouldReturnCorrectValue(
		[Frozen]ThrowingTestScheduler scheduler,
		CommandEvents sut,
		TestCommandWithMultipleEvents command,
		string value)
		{

			//arrange
			scheduler.Schedule(TimeSpan.FromTicks(202), () => sut.NotifyEvent(command, value));

			//act
			var result = scheduler.Start(() => sut.ObserveCommandEvents<TestCommandWithMultipleEvents, int>());

			//assert
			result.AssertExceptions();
			result.Values().Should().BeEmpty();
		}
	}
}