using System;
using System.Reactive.Disposables;
using FluentAssertions;
using Hanno.Testing.Autofixture;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Hanno.Pcl.Rx.Tests.Extensions
{
    public class DisposableExtensionsTests
    {
        [Theory, AutoMoqData]
        public void DisposeWith_ShouldAddSutToComposite(
            CompositeDisposable compositeDisposable,
          IDisposable sut)
        {
            //arrange

            //act
            sut.DisposeWith(compositeDisposable);

            //assert
            compositeDisposable.Should().Contain(sut);
        }

        [Theory, AutoMoqData]
        public void DisposeWithOfT_WithTIsDiposable_ShouldAddSutToComposite(
            CompositeDisposable compositeDisposable,
          IDisposable sut)
        {
            //arrange

            //act
            ((object)sut).DisposeWith(compositeDisposable);

            //assert
            compositeDisposable.Should().Contain(sut);
        }

        [Theory, AutoMoqData]
        public void DisposeWithOfT_WithTIsNotDiposable_ShouldNotAddSutToComposite(
            CompositeDisposable compositeDisposable,
            object sut)
        {
            //arrange

            //act
            sut.DisposeWith(compositeDisposable);

            //assert
            compositeDisposable.Should().NotContain(sut);
        }

        [Theory, AutoMoqData]
        public void DisposeWithOfT_WithTIsNotDiposable_ShouldReturnSut(
            CompositeDisposable compositeDisposable,
            object sut)
        {
            //arrange

            //act
            var actual = sut.DisposeWith(compositeDisposable);

            //assert
            actual.Should().Be(sut);
        }
    }
}