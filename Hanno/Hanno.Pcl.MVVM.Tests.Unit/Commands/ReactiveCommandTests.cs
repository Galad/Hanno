using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using FluentAssertions;
using Framework.Commands;
using Framework.Pcl.Rx;
using Framework.Testing.Autofixture;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Framework.Pcl.MVVM.Tests.Unit.Commands
{
    public class ReactiveCommandTests
    {
        [Theory, AutoMoqData]
        public void Sut_TestDefaultValues(
            Func<object, IReactiveValue<object>> factory)
        {
            //arrange
            var sut = new ReactiveMvvmCommand<object, object>(factory);

            //act

            //assert
            sut.Callback.Should().NotBeNull();
            sut.OnError.Should().NotBeNull();
            sut.CallbackScheduler.Should().NotBeNull();
            sut.OnErrorScheduler.Should().NotBeNull();
            sut.CanExecutePredicate(null).Should().BeTrue();
        }

        [Theory, RxInlineAutoData(true), RxInlineAutoData(false)]
        public void CanExecute_ShouldReturnCorrectValue(
            IReactiveValue<object> value,
            bool canExecute,
            object arg)
        {
            //arrange
            var sut = new ReactiveMvvmCommand<object, object>(o => value, canExecutePredicate: o => canExecute);
            //act
            var actual = sut.CanExecute(arg);

            //assert
            actual.Should().Be(canExecute);
        }

        [Theory, RxAutoData]
        public void CanExecute_ShouldBeCalledWithArg(
            IReactiveValue<object> value,
            object arg)
        {
            //arrange
            object actual = null;
            var sut = new ReactiveMvvmCommand<object, object>(o => value, canExecutePredicate: o =>
            {
                actual = o;
                return true;
            });

            //act
            sut.CanExecute(arg);

            //assert
            actual.Should().Be(arg);
        }

        [Theory, RxAutoData]
        public void Execute_ShouldBeCalledWithArg(
            IReactiveValue<object> value,
            object arg)
        {
            //arrange
            object actual = null;
            var sut = new ReactiveMvvmCommand<object, object>(o =>
            {
                actual = o;
                return value;
            });

            //act
            sut.Execute(arg);

            //assert
            actual.Should().Be(arg);
        }

        [Theory, AutoMoqData]
        public void Execute_ShouldExecuteObservable()
        {
            //arrange
            bool observableCalled = false;
            var value = ReactiveValue.Return(System.Reactive.Unit.Default, ImmediateScheduler.Instance)
                .Do(unit => observableCalled = true)
                .AsReactiveValue();
            var sut = new ReactiveMvvmCommand<object, System.Reactive.Unit>(o => value);

            //act
            sut.Execute(null);

            //assert
            observableCalled.Should().BeTrue();
        }

        [Theory, AutoMoqData]
        public void Execute_ShouldCallCallback(
            object expected)
        {
            //arrange
            object actual = null;
            var value = ReactiveValue.Return(expected, ImmediateScheduler.Instance);
            var sut = new ReactiveMvvmCommand<object, object>(o => value, o => actual = o, ImmediateScheduler.Instance);

            //act
            sut.Execute(null);

            //assert
            actual.Should().Be(expected);
        }

        [Theory, AutoMoqData]
        public void Execute_ShouldCallOnError(
            Exception expected)
        {
            //arrange
            Exception actual = null;
            var value = Observable.Throw<object>(expected, ImmediateScheduler.Instance).AsReactiveValue();
            var sut = new ReactiveMvvmCommand<object, object>(o => value, onError: o => actual = o, onErrorScheduler: ImmediateScheduler.Instance);

            //act
            sut.Execute(null);

            //assert
            actual.Should().Be(expected);
        }

        [Theory, AutoMoqData]
        public void Execute_ShouldRaiseCanExecuteChangedTwice(
            object obj)
        {
            //arrange
            var value = ReactiveValue.Return(obj, ImmediateScheduler.Instance);
            var sut = new ReactiveMvvmCommand<object, object>(_ => value);
            sut.MonitorEvents();


            //act
            sut.Execute(null);

            //assert
            sut.ShouldRaise("CanExecuteChanged").Should().HaveCount(2);
        }

        [Theory, RxAutoData]
        public void CanExecute_WhenExecutingCommand_ShouldBeFalse(
            object obj)
        {
            //arrange
            var canExecuteValues = new List<bool>();
            var value = ReactiveValue.Return(obj, ImmediateScheduler.Instance);
            var sut = new ReactiveMvvmCommand<object, object>(_ => value);
            sut.CanExecuteChanged += (sender, args) => canExecuteValues.Add(sut.CanExecute(null));

            //act
            sut.Execute(null);

            //assert
            canExecuteValues.First().Should().BeFalse();
        }

        [Theory, RxAutoData]
        public void CanExecute_AfterExecutingCommand_ShouldBeTrue(
            object obj)
        {
            //arrange
            var value = ReactiveValue.Return(obj, ImmediateScheduler.Instance);
            var sut = new ReactiveMvvmCommand<object, object>(_ => value);
            sut.Execute(null);

            //act
            var actual = sut.CanExecute(null);

            //assert
            actual.Should().BeTrue();
        }

        [Theory, RxAutoData]
        public void CanExecute_AfterExecutingCommand_ShouldBePreviousState(
            object obj)
        {
            //arrange
            var value = ReactiveValue.Return(obj, ImmediateScheduler.Instance);
            var sut = new ReactiveMvvmCommand<object, object>(_ => value, canExecutePredicate: o => false);
            sut.Execute(null);

            //act
            var actual = sut.CanExecute(null);

            //assert
            actual.Should().BeFalse();
        }
    }
}