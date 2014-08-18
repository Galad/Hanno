using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.Testing.Autofixture;
using Hanno.ViewModels;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Hanno.Tests.ViewModels
{
	#region Customization
	public class RvvmCustomization : ICustomization
	{
		private readonly bool _emptyResult;
		private readonly bool _hasError;
		private readonly TimeSpan _timeout;

		public RvvmCustomization(bool emptyResult, bool hasError, uint timeout = 0)
		{
			_emptyResult = emptyResult;
			_hasError = hasError;
			_timeout = TimeSpan.FromTicks(timeout);
		}

		public void Customize(IFixture fixture)
		{
			fixture.Freeze(new Subject<System.Reactive.Unit>());
			fixture.Freeze(new Exception());
			fixture.Register<IObservable<System.Reactive.Unit>>(() => fixture.Create<Subject<System.Reactive.Unit>>());
			fixture.Register(() =>
			{
				bool hasBeenRun = false;
				return new ObservableViewModel<object>(ct =>
				{
					var t = GetValue(fixture, hasBeenRun);
					hasBeenRun = true;
					return t;
				},
				o => _emptyResult, fixture.Create<IObservable<System.Reactive.Unit>>(), _timeout, new CompositeDisposable());
			});
		}

		private Task<object> GetValue(IFixture fixture, bool hasBeenRun)
		{
			return Task.Run(() =>
				{
					if (_hasError && !hasBeenRun)
					{
						throw fixture.Create<Exception>();
					}
					return fixture.Create<object>();
				});
		}
	}

	public class RvvmCompositeCustomization : CompositeCustomization
	{
		public RvvmCompositeCustomization(bool emptyResult, bool hasError, uint timeout = 0)
			: base(new AutoMoqCustomization(), new RxCustomization(), new RvvmCustomization(emptyResult, hasError, timeout))
		{
		}
	}

	public class RvvmAutoDataAttribute : AutoDataAttribute
	{
		public RvvmAutoDataAttribute(bool emptyResult = false, bool hasError = false, uint timeout = 0)
			: base(new Fixture().Customize(new RvvmCompositeCustomization(emptyResult, hasError, timeout)))
		{
		}
	}

	public class RvvmInlineAutoDataAttribute : CompositeDataAttribute
	{
		public RvvmInlineAutoDataAttribute(bool emptyResult = false, uint timeout = 0, params object[] values)
			: base(new RvvmAutoDataAttribute(emptyResult, timeout: timeout), new InlineDataAttribute(values))
		{
		}
	}
	#endregion

	public class ObservableViewModelTest
	{
		[Theory, RvvmAutoData()]
		public async Task Refresh_ShouldReturnCorrectValue(
			[Frozen] ThrowingTestScheduler scheduler,
			[Frozen] object value,
			[Frozen] IViewModel parent,
			ObservableViewModel<object> sut)
		{
			//arrange
			var observer = scheduler.CreateObserver<ObservableViewModelNotification>();
			sut.Subscribe(observer);

			//act
			await sut.RefreshAsync();

			//assert
			var actual = observer.Values().ToArray();
			var expected = new[]
			{
				new ObservableViewModelNotification() {Status = ObservableViewModelStatus.Initialized, Value = null},
				new ObservableViewModelNotification() {Status = ObservableViewModelStatus.Updating, Value = null},
				new ObservableViewModelNotification() {Status = ObservableViewModelStatus.Value, Value = value}
			};
			actual.ShouldAllBeEquivalentTo(expected);
		}

		[Theory, RvvmAutoData(hasError: true)]
		public async Task Refresh_SourceYieldError_ShouldReturnCorrectValue(
			[Frozen] ThrowingTestScheduler scheduler,
			[Frozen] Exception error,
			[Frozen] IViewModel parent,
			ObservableViewModel<object> sut)
		{
			//arrange
			var observer = scheduler.CreateObserver<ObservableViewModelNotification>();
			sut.Subscribe(observer);

			//act
			await sut.RefreshAsync();

			//assert
			var actual = observer.Values().ToArray();
			var expected = new[]
			{
				new ObservableViewModelNotification() {Status = ObservableViewModelStatus.Initialized, Value = null},
				new ObservableViewModelNotification() {Status = ObservableViewModelStatus.Updating, Value = null},
				new ObservableViewModelNotification() {Status = ObservableViewModelStatus.Error, Value = error}
			};
			actual.ShouldAllBeEquivalentTo(expected);
		}

		[Theory, RvvmAutoData(emptyResult: true)]
		public async Task Refresh_SourceIsEmpty_ShouldReturnCorrectValue(
			[Frozen] ThrowingTestScheduler scheduler,
			[Frozen] IViewModel parent,
			ObservableViewModel<object> sut)
		{
			//arrange
			var observer = scheduler.CreateObserver<ObservableViewModelNotification>();
			sut.Subscribe(observer);

			//act
			await sut.RefreshAsync();

			//assert
			var actual = observer.Values().ToArray();
			var expected = new[]
			{
				new ObservableViewModelNotification() {Status = ObservableViewModelStatus.Initialized, Value = null},
				new ObservableViewModelNotification() {Status = ObservableViewModelStatus.Updating, Value = null},
				new ObservableViewModelNotification() {Status = ObservableViewModelStatus.Empty, Value = null}
			};
			actual.ShouldAllBeEquivalentTo(expected);
		}

		[Theory, RvvmAutoData(emptyResult: true)]
		public void Dispose_ShouldYieldCompleted(
			[Frozen] ThrowingTestScheduler scheduler,
			ObservableViewModel<object> sut)
		{
			//arrange
			var observer = scheduler.CreateObserver<ObservableViewModelNotification>();
			sut.Subscribe(observer);

			//act
			sut.Dispose();

			//assert
			observer.Completed().Should().HaveCount(1);
		}

		[Theory, RvvmAutoData(emptyResult: true, timeout: 1)]
		public async Task RefreshAsync_WithTimeout_ShouldThrow(
			[Frozen] ThrowingTestScheduler scheduler,
			ObservableViewModel<object> sut)
		{
			//arrange
			var observer = scheduler.CreateObserver<ObservableViewModelNotification>();
			sut.Subscribe(observer);

			//act
			await sut.RefreshAsync();

			//assert
			var actual = observer.Values().ToArray();
			var expected = new[]
			{
				new ObservableViewModelNotification() {Status = ObservableViewModelStatus.Initialized, Value = null},
				new ObservableViewModelNotification() {Status = ObservableViewModelStatus.Updating, Value = null},
				new ObservableViewModelNotification() {Status = ObservableViewModelStatus.Timeout, Value = null}
			};
			actual.ShouldAllBeEquivalentTo(expected);
		}

		[Theory, RvvmAutoData]
		public void CurrentValue_WhenInitialized_ShouldBeDefault(
			[Frozen] ThrowingTestScheduler scheduler,
			ObservableViewModel<object> sut)
		{
			//assert
			sut.CurrentValue.Should().BeNull();
		}

		[Theory, RvvmAutoData()]
		public async Task CurrentValue_SourceYieldValue_ShouldReturnCorrectValue(
			[Frozen] ThrowingTestScheduler scheduler,
			[Frozen] object expected,
			[Frozen] IViewModel parent,
			ObservableViewModel<object> sut)
		{
			//arrange
			var observer = scheduler.CreateObserver<ObservableViewModelNotification>();
			sut.Subscribe(observer);

			//act
			await sut.RefreshAsync();

			//assert
			var actual = sut.CurrentValue;
			actual.Should().Be(expected);
		}

		[Theory, RvvmAutoData(hasError: true)]
		public async Task CurrentValue_SourceYieldErrorThenValue_ShouldReturnCorrectValue(
			[Frozen] ThrowingTestScheduler scheduler,
			[Frozen] Exception error,
			[Frozen] IViewModel parent,
			[Frozen] object expected,
			ObservableViewModel<object> sut)
		{
			//arrange
			var observer = scheduler.CreateObserver<ObservableViewModelNotification>();
			sut.Subscribe(observer);

			//act
			//first refresh yield an error
			await sut.RefreshAsync();
			//second refresh yield a value
			await sut.RefreshAsync();

			//assert
			var actual = sut.CurrentValue;
			actual.Should().Be(expected);
		}

		[Theory, RvvmAutoData(hasError: true)]
		public async Task CurrentValue_SourceYieldError_ShouldReturnCorrectValue(
			[Frozen] ThrowingTestScheduler scheduler,
			[Frozen] Exception error,
			[Frozen] IViewModel parent,
			ObservableViewModel<object> sut)
		{
			//arrange
			var observer = scheduler.CreateObserver<ObservableViewModelNotification>();
			sut.Subscribe(observer);

			//act
			await sut.RefreshAsync();

			//assert
			var actual = sut.CurrentValue;
			actual.Should().BeNull();
		}
	}
}