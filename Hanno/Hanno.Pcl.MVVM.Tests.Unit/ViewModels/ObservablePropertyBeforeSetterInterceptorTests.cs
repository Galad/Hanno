using System;
using System.Collections.Generic;
using Hanno.Testing.Autofixture;
using Hanno.ViewModels;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit2;
using Ploeh.AutoFixture;
using Xunit;
using FluentAssertions;
using Moq;

namespace Hanno.Tests.ViewModels
{
	public class ObservablePropertyBeforeSetterInterceptorTests
	{
		[Theory, AutoMoqData]
		public void Sut_IsObservableProperty(
		  ObservablePropertyBeforeSetterInterceptor<object> sut)
		{
			//arrange
			sut.Should().BeAssignableTo<IObservableProperty<object>>();
		}

		[Theory, AutoMoqData]
		public void Sut_ConstructorGuardClauses(
		  GuardClauseAssertion assertion)
		{
			assertion.VerifyConstructors<ObservablePropertyBeforeSetterInterceptor<object>>();
		}

		[Theory, AutoMoqData]
		public void OnNext_ShouldCallMethodsInCorrectOrder(
			IFixture fixture,
			object value,
			object oldValue)
		{
			//arrange
			var property = new Mock<IObservableProperty<object>>();
			property.SetupGet(p => p.Value).Returns(oldValue);
			var action = new Mock<Action<object, object>>();
			var sut = new ObservablePropertyBeforeSetterInterceptor<object>(property.Object, action.Object);
			var actual = new List<object>();
			action.Setup(a => a(oldValue, value)).Callback<object, object>((o, o1) => actual.Add(new Tuple<object, object>(o, o1)));
			property.Setup(o => o.OnNext(value)).Callback<object>(o => actual.Add(o));

			//act
			sut.OnNext(value);

			//assert
			var expected = new[] { new Tuple<object, object>(oldValue, value), value };
			AssertInvocation(actual, expected);
		}

		[Theory, AutoMoqData]
		public void Value_ShouldCallMethodsInCorrectOrder(
			IFixture fixture,
			object value,
			object oldValue)
		{
			//arrange
			var property = new Mock<IObservableProperty<object>>();
			property.SetupGet(p => p.Value).Returns(oldValue);
			var action = new Mock<Action<object, object>>();
			var sut = new ObservablePropertyBeforeSetterInterceptor<object>(property.Object, action.Object);
			var actual = new List<object>();
			action.Setup(a => a(oldValue, value)).Callback<object, object>((o, o1) => actual.Add(new Tuple<object, object>(o, o1)));
			property.SetupSet(o => o.Value).Callback(o => actual.Add(o));

			//act
			sut.Value = value;

			//assert
			var expected = new[] { new Tuple<object, object>(oldValue, value), value };
			AssertInvocation(actual, expected);
		}

		private void AssertInvocation(IList<object> expected, IList<object> actual)
		{
			var t = (Tuple<object, object>)expected[0];
			var t2 = (Tuple<object, object>)actual[0];
			Assert.Equal(t.Item1, t2.Item1);
			Assert.Equal(t.Item2, t2.Item2);
			Assert.Equal(expected[1], actual[1]);
		}  
	}
}