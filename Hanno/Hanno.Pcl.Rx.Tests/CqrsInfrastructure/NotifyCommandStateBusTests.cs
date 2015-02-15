using System;
using System.Linq;
using Hanno.CqrsInfrastructure;
using Hanno.Extensions;
using Hanno.Testing.Autofixture;
using Microsoft.Reactive.Testing;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Xunit;
using Ploeh.AutoFixture;
using Xunit.Extensions;
using Xunit;
using FluentAssertions;
using Moq;
using System.Threading.Tasks;
using Ploeh.AutoFixture.Idioms;

namespace Hanno.Tests.CqrsInfrastructure
{
	public class NotifyCommandStateBusTests : ReactiveTest
	{
		#region Customization
		public class NotifyCommandStateBusCustomization : ICustomization
		{
			public void Customize(IFixture fixture)
			{
				fixture.Customize<Mock<IAsyncCommandBus>>(c => c.Do(m => m.Setup(bus => bus.ProcessCommand(It.IsAny<IAsyncCommand>())).ReturnsDefaultTask()));
			}
		}

		public class NotifyCommandStateBusCompositeCustomization : CompositeCustomization
		{
			public NotifyCommandStateBusCompositeCustomization()
				: base(new RxCustomization(), new AutoMoqCustomization(), new NotifyCommandStateBusCustomization())
			{
			}
		}

		public class NotifyCommandStateBusAutoDataAttribute : AutoDataAttribute
		{
			public NotifyCommandStateBusAutoDataAttribute()
				: base(new Fixture().Customize(new NotifyCommandStateBusCompositeCustomization()))
			{
			}
		}

		public class NotifyCommandStateBusInlineAutoDataAttribute : CompositeDataAttribute
		{
			public NotifyCommandStateBusInlineAutoDataAttribute(params object[] values)
				: base(new InlineDataAttribute(values), new NotifyCommandStateBusAutoDataAttribute())
			{
			}
		}
		#endregion

		[Theory, NotifyCommandStateBusAutoData]
		public void Sut_IsAsyncCommandBus(
		  NotifyCommandStateBus sut)
		{
			sut.IsAssignableTo<IAsyncCommandBus>();
		}

		[Theory, NotifyCommandStateBusAutoData]
		public void Sut_IsCommandStateEvents(
		  NotifyCommandStateBus sut)
		{
			sut.IsAssignableTo<ICommandStateEvents>();
		}

		[Theory, AutoMoqData]
		public void Sut_VerifyConstructorGuardClauses(GuardClauseAssertion assertion)
		{
			assertion.VerifyConstructors<NotifyCommandStateBus>();
		}

		[Theory, NotifyCommandStateBusAutoData]
		public async Task ProcessCommand_ShouldCallInnerProcessCommnad(
			[Frozen]Mock<IAsyncCommandBus> innerCommandBus,
		  NotifyCommandStateBus sut,
			IAsyncCommand command)
		{
			//arrange
			var verifiable = innerCommandBus.Setup(bus => bus.ProcessCommand(command)).ReturnsDefaultTaskVerifiable();

			//act
			await sut.ProcessCommand(command);

			//assert
			verifiable.Verify();
		}

		[Theory, NotifyCommandStateBusAutoData]
		public async Task ProcessCommand_ShouldNotifyCommandStarted(
			[Frozen]Mock<IAsyncCommandBus> innerCommandBus,
			NotifyCommandStateBus sut,
			IAsyncCommand command,
			[Frozen]TestScheduler scheduler)
		{
			//arrange
			var observer = scheduler.CreateObserver<IAsyncCommand>();
			sut.ObserveCommandStarted().Subscribe(observer);

			//act
			scheduler.AdvanceTo(200);
			await sut.ProcessCommand(command);

			//assert
			var expected = OnNext(200, command);
			observer.Messages.First().ShouldBeEquivalentTo(expected);
		}

		[Theory, NotifyCommandStateBusAutoData]
		public async Task ProcessCommand_ShouldNotifyCommandEnded(
			[Frozen]Mock<IAsyncCommandBus> innerCommandBus,
			NotifyCommandStateBus sut,
			IAsyncCommand command,
			[Frozen]TestScheduler scheduler)
		{
			//arrange
			var observer = scheduler.CreateObserver<IAsyncCommand>();
			innerCommandBus.Setup(bus => bus.ProcessCommand(It.IsAny<IAsyncCommand>()))
			.Returns(() =>
			{
				scheduler.AdvanceTo(300);
				return Task.FromResult(true);
			});
			sut.ObserveCommandEnded().Subscribe(observer);

			//act
			await sut.ProcessCommand(command);

			//assert
			var expected = OnNext(300, command);
			observer.Messages.First().ShouldBeEquivalentTo(expected);
		}

		[Theory, NotifyCommandStateBusAutoData]
		public async Task ProcessCommand_ShouldNotifyCommandError(
			[Frozen]Mock<IAsyncCommandBus> innerCommandBus,
			NotifyCommandStateBus sut,
			IAsyncCommand command,
			[Frozen]TestScheduler scheduler,
			Exception exception)
		{
			//arrange
			var observer = scheduler.CreateObserver<Tuple<IAsyncCommand, Exception>>();
			innerCommandBus.Setup(bus => bus.ProcessCommand(It.IsAny<IAsyncCommand>()))
			.Returns(() =>
			{
				scheduler.AdvanceTo(300);
				throw exception;
			});
			sut.ObserveCommandError().Subscribe(observer);

			//act
			try
			{
				await sut.ProcessCommand(command);
			}
			catch (Exception ex)
			{
				if (ex != exception)
				{
					throw;
				}
			}

			//assert
			var expected = OnNext(300, new Tuple<IAsyncCommand, Exception>(command, exception));
			observer.Messages.First().ShouldBeEquivalentTo(expected);
		}

		[Theory, NotifyCommandStateBusAutoData]
		public async Task ProcessCommand_WhenSubscribingDuringExecution_ShouldNotifyCommandStarted(
			[Frozen]Mock<IAsyncCommandBus> innerCommandBus,
			NotifyCommandStateBus sut,
			IAsyncCommand command,
			[Frozen]TestScheduler scheduler)
		{
			//arrange
			var observer = scheduler.CreateObserver<IAsyncCommand>();
			innerCommandBus.Setup(bus => bus.ProcessCommand(It.IsAny<IAsyncCommand>()))
			.Returns(() =>
			{
				sut.ObserveCommandStarted().Subscribe(observer);
				return Task.FromResult(true);
			});

			//act
			scheduler.AdvanceTo(200);
			await sut.ProcessCommand(command);
			scheduler.Start();

			//assert
			var expected = OnNext(201, command);
			observer.Messages.First().ShouldBeEquivalentTo(expected);
		}

		[Theory, NotifyCommandStateBusAutoData]
		public async Task ProcessCommand_WhenSubscribingAfterExecution_ShouldNotNotifyCommandStarted(
			[Frozen]Mock<IAsyncCommandBus> innerCommandBus,
			NotifyCommandStateBus sut,
			IAsyncCommand command,
			[Frozen]TestScheduler scheduler)
		{
			//arrange
			var observer = scheduler.CreateObserver<IAsyncCommand>();
			
			//act
			scheduler.AdvanceTo(200);
			await sut.ProcessCommand(command);
			sut.ObserveCommandStarted().Subscribe(observer);

			//assert
			observer.Values().Should().BeEmpty();
		}

	}
}