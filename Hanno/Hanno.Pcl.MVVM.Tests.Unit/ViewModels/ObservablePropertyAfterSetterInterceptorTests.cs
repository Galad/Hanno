using System;
using System.Collections.Generic;
using FluentAssertions;
using Hanno.Testing.Autofixture;
using Hanno.ViewModels;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Idioms;
using Xunit;
using Xunit.Extensions;

namespace Hanno.Tests.ViewModels
{
	public class ObservablePropertyAfterSetterInterceptorTests
	{
		[Theory, AutoMoqData]
		public void Sut_IsObservableProperty(
			ObservablePropertyAfterSetterInterceptor<object> sut)
		{
			//arrange
			sut.Should().BeAssignableTo<IObservableProperty<object>>();
		}

		[Theory, AutoMoqData]
		public void Sut_ConstructorGuardClauses(
			GuardClauseAssertion assertion)
		{
			assertion.VerifyConstructors<ObservablePropertyAfterSetterInterceptor<object>>();
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
			var sut = new ObservablePropertyAfterSetterInterceptor<object>(property.Object, action.Object);
			var actual = new List<object>();
			action.Setup(a => a(oldValue, value)).Callback<object, object>((o, o1) => actual.Add(new Tuple<object, object>(o, o1)));
			property.Setup(o => o.OnNext(value)).Callback<object>(o => actual.Add(o));

			//act
			sut.OnNext(value);

			//assert
			var expected = new[] { value, new Tuple<object, object>(oldValue, value) };
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
			var sut = new ObservablePropertyAfterSetterInterceptor<object>(property.Object, action.Object);
			var actual = new List<object>();
			action.Setup(a => a(oldValue, value)).Callback<object, object>((o, o1) => actual.Add(new Tuple<object, object>(o, o1)));
			property.SetupSet(o => o.Value).Callback(o => actual.Add(o));
			//act
			sut.Value = value;

			//assert
			var expected = new[] { value, new Tuple<object, object>(oldValue, value) };
			AssertInvocation(actual, expected);
		}

		private void AssertInvocation(IList<object> expected, IList<object> actual)
		{
			Assert.Equal(expected[0], actual[0]);
			var t = (Tuple<object, object>) expected[1];
			var t2 = (Tuple<object, object>) actual[1];
			Assert.Equal(t.Item1, t2.Item1);
			Assert.Equal(t.Item2, t2.Item2);
		}  
	}
}