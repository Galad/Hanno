using FluentAssertions;
using Hanno.ViewModels;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Hanno.Tests.ViewModels
{
	public class NullReferenceEmptyPredicateVisitorTests
	{
		[Theory, RvvmAutoData]
		public void Sut_IsEmptyPredicateVisitor(
			NullReferenceEmptyPredicateVisitor sut)
		{
			sut.Should().BeAssignableTo<IObservableViewModelVisitor>();
		}

		[Theory, RvvmAutoData]
		public void Visit_WithNullValue_ShouldReturnTrue(
			NullReferenceEmptyPredicateVisitor sut,
			ObservableViewModel<object> ovm)
		{
			//arrange

			//act
			sut.Visit(ovm);
			var actual = ovm.EmptyPredicate(null);

			//assert
			actual.Should().BeTrue();
		}

		[Theory, RvvmAutoData]
		public void Visit_WithNotNullValue_ShouldReturnFalse(
			NullReferenceEmptyPredicateVisitor sut,
			ObservableViewModel<object> ovm)
		{
			//arrange

			//act
			sut.Visit(ovm);
			var actual = ovm.EmptyPredicate(new object());

			//assert
			actual.Should().BeFalse();
		}

		[Theory, RvvmAutoData]
		public void Visit_WithNotReferenceTypeValue_ShouldReturnFalse(
			NullReferenceEmptyPredicateVisitor sut,
			ObservableViewModel<int> ovm,
			int value)
		{
			//arrange

			//act
			sut.Visit(ovm);
			var actual = ovm.EmptyPredicate(value);

			//assert
			actual.Should().BeFalse();
		}

		[Theory, RvvmAutoData]
		public void Visit_WithNotReferencenullableTypeValue_ShouldReturnTrue(
			NullReferenceEmptyPredicateVisitor sut,
			ObservableViewModel<int?> ovm)
		{
			//arrange

			//act
			sut.Visit(ovm);
			var actual = ovm.EmptyPredicate(null);

			//assert
			actual.Should().BeTrue();
		}
	}
}