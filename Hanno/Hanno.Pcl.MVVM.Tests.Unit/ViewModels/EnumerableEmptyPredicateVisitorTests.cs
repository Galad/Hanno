using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Hanno.ViewModels;
using Ploeh.AutoFixture.Xunit;
using Ploeh.AutoFixture;
using Xunit.Extensions;
using Xunit;
using FluentAssertions;
using Moq;

namespace Hanno.Tests.ViewModels
{
	public class EnumerableEmptyPredicateVisitorTests
	{
		[Theory, RvvmAutoData]
		public void Sut_IsEmptyPredicateVisitor(
			EnumerableEmptyPredicateVisitor sut)
		{
			sut.Should().BeAssignableTo<IObservableViewModelVisitor>();
		}

		[Theory, RvvmAutoData]
		public void Visit_OvmPredicateShouldReturnTrue(
			EnumerableEmptyPredicateVisitor sut,
			ObservableViewModel<string[]> ovm)
		{
			//arrange
			
			//act
			sut.Visit(ovm);
			var actual = ovm.EmptyPredicate(new string[0]);

			//assert
			actual.Should().BeTrue();
		}

		[Theory, RvvmAutoData]
		public void Visit_WhenValueIsNotEmpty_OvmPredicateShouldReturnFalse(
			EnumerableEmptyPredicateVisitor sut,
			ObservableViewModel<string[]> ovm,
			string[] value)
		{
			//arrange

			//act
			sut.Visit(ovm);
			var actual = ovm.EmptyPredicate(value);

			//assert
			actual.Should().BeFalse();
		}

		[Theory, RvvmAutoData]
		public void Visit_WhenValueIsNotEnumerable_OvmPredicateShouldReturnFalse(
			EnumerableEmptyPredicateVisitor sut,
			ObservableViewModel<object> ovm)
		{
			//arrange

			//act
			sut.Visit(ovm);
			var actual = ovm.EmptyPredicate(new object());

			//assert
			actual.Should().BeFalse();
		}
	}
}