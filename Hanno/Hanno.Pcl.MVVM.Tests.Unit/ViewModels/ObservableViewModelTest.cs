using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.Testing.Autofixture;
using Hanno.ViewModels;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;
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
			RegisterOvm<object>(fixture);
			RegisterOvm<object[]>(fixture);
			RegisterOvm<string[]>(fixture);
			RegisterOvm<int>(fixture);
			RegisterOvm<int?>(fixture);
		}

		private void RegisterOvm<T>(IFixture fixture)
		{
			fixture.Register(() =>
			{
				bool hasBeenRun = false;
				return new ObservableViewModel<T>(ct =>
				{
					var t = GetValue<T>(fixture, hasBeenRun);
					hasBeenRun = true;
					return t;
				},
					o => _emptyResult,
					fixture.Create<IObservable<System.Reactive.Unit>>(),
					_timeout,
					new CompositeDisposable(),
					fixture.Create<ISchedulers>());
			});
		}

		private Task<T> GetValue<T>(IFixture fixture, bool hasBeenRun)
		{
			if (_timeout.TotalMilliseconds > 0)
			{
				return Observable.Timer(_timeout.Add(TimeSpan.FromMilliseconds(10)), fixture.Create<IScheduler>())
								 .Select(_ => fixture.Create<T>())
				                 .ToTask();
			}
			if (_hasError && !hasBeenRun)
			{
				var tcs = new TaskCompletionSource<T>();
				tcs.SetException(fixture.Create<Exception>());
				return tcs.Task;
			}
			return Task.FromResult(fixture.Create<T>());
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

		[Theory, RvvmAutoData]
		public void RefreshOn_ShouldRefresh(
			[Frozen] ThrowingTestScheduler scheduler,
			[Frozen] Subject<Unit> refreshOn,
			[Frozen] object value,
			ObservableViewModel<object> sut)
		{
			//arrange
			var observer = scheduler.CreateObserver<ObservableViewModelNotification>();
			sut.Subscribe(observer);

			//act
			scheduler.Schedule(TimeSpan.FromTicks(201), () =>  refreshOn.OnNext(Unit.Default));
			scheduler.Start();

			//assert
			var expected = new ObservableViewModelNotification()
			{
				Status = ObservableViewModelStatus.Value,
				Value = value
			};
			observer.Values().Last().ShouldBeEquivalentTo(expected);
		}

		[Theory, RvvmAutoData]
		public void RefreshOn_WithOverlappingRefresh_ShouldCancelPreviousTask(
			[Frozen] TestSchedulers scheduler,
			[Frozen] Subject<Unit> refreshOn)
		{
			//arrange
			CancellationToken ct = CancellationToken.None;
			var i = 0;
			var sut = new ObservableViewModel<object>(c =>
			{
				if (++i == 1)
				{
					ct = c;
					refreshOn.OnNext(Unit.Default);
				}
				return Task.FromResult(new object());
			}, _ => false, refreshOn, TimeSpan.Zero, new CompositeDisposable(), scheduler);


			//act
			refreshOn.OnNext(Unit.Default);
			scheduler.Start();

			//assert
			ct.IsCancellationRequested.Should().BeTrue();
		}

		[Theory, RvvmAutoData]
		public void RefreshOn_WithOverlappingRefresh_ShouldReturnCorrectValue(
			[Frozen] TestSchedulers scheduler,
			[Frozen] Subject<Unit> refreshOn,
			object value)
		{
			//arrange
			var observer = scheduler.CreateObserver<ObservableViewModelNotification>();
			var i = 0;
			var sut = new ObservableViewModel<object>(c =>
			{
				if (++i == 1)
				{
					refreshOn.OnNext(Unit.Default);
				}
				return Task.FromResult(value);
			}, _ => false, refreshOn, TimeSpan.Zero, new CompositeDisposable(), scheduler);
			sut.Subscribe(observer);

			//act
			refreshOn.OnNext(Unit.Default);
			scheduler.Start();

			//assert
			var expected = new ObservableViewModelNotification()
			{
				Status = ObservableViewModelStatus.Value,
				Value = value
			};
			observer.Values().Last().ShouldBeEquivalentTo(expected);
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

		[Theory, RvvmAutoData(emptyResult: true, timeout: 50)]
		public async Task RefreshAsync_WithTimeout_ShouldThrow(
			[Frozen] ThrowingTestScheduler scheduler,
			ObservableViewModel<object> sut)
		{
			//arrange
			var observer = scheduler.CreateObserver<ObservableViewModelNotification>();
			sut.Subscribe(observer);
			scheduler.Schedule(TimeSpan.FromTicks(201), () => sut.Refresh());

			//act
			scheduler.Start();
			//await sut.RefreshAsync();

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

		[Theory, RvvmAutoData]
		public void Accept_ShouldCallVisitor(
			ObservableViewModel<object> sut,
			Mock<IObservableViewModelVisitor> visitor)
		{
			//act
			sut.Accept(visitor.Object);

			//assert
			visitor.Verify(v => v.Visit(sut));
		}

		[Theory, RvvmAutoData]
		public void Accept_GuardClauses(
			GuardClauseAssertion assertion)
		{
			assertion.Verify((ObservableViewModel<object> sut) => sut.Accept(default(IObservableViewModelVisitor)));
		}

		[Theory, RvvmAutoData]
		public async Task ChainEmptyPredicate_WhenPredicateReturnsTrue_OvmStateShouldBeEmpty(
			[Frozen] TestSchedulers schedulers,
			ObservableViewModel<object> sut)
		{
			//arrange
			Func<object, bool> predicate = o => true;
			var observer = schedulers.CreateObserver<ObservableViewModelNotification>();
			sut.ChainEmptyPredicate(predicate);
			sut.Subscribe(observer);

			//act
			await sut.RefreshAsync();

			//assert
			observer.Values().Last().Should().Match<ObservableViewModelNotification>(n => n.Status == ObservableViewModelStatus.Empty);
		}

		[Theory, RvvmAutoData]
		public async Task ChainEmptyPredicate_WhenPredicateReturnsFalse_OvmStateShouldBeValue(
			[Frozen] TestSchedulers schedulers,
			ObservableViewModel<object> sut)
		{
			//arrange
			Func<object, bool> predicate = o => false;
			var observer = schedulers.CreateObserver<ObservableViewModelNotification>();
			sut.ChainEmptyPredicate(predicate);
			sut.Subscribe(observer);

			//act
			await sut.RefreshAsync();

			//assert
			observer.Values().Last().Should().Match<ObservableViewModelNotification>(n => n.Status == ObservableViewModelStatus.Value);
		}

		[Theory, RvvmAutoData]
		public async Task ChainEmptyPredicate_WithManyPredicatesAndWhenPredicateReturnsTrue_OvmStateShouldBeEmpty(
			[Frozen] TestSchedulers schedulers,
			ObservableViewModel<object> sut)
		{
			//arrange
			Func<object, bool> predicate1 = o => false;
			Func<object, bool> predicate2 = o => false;
			Func<object, bool> predicate3 = o => true;
			var observer = schedulers.CreateObserver<ObservableViewModelNotification>();
			sut.ChainEmptyPredicate(predicate1);
			sut.ChainEmptyPredicate(predicate2);
			sut.ChainEmptyPredicate(predicate3);
			sut.Subscribe(observer);

			//act
			await sut.RefreshAsync();

			//assert
			observer.Values().Last().Should().Match<ObservableViewModelNotification>(n => n.Status == ObservableViewModelStatus.Empty);
		}
	}
}