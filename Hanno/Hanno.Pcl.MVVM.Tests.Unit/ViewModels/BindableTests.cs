using System;
using System.ComponentModel;
using System.Linq.Expressions;
using FluentAssertions;
using Hanno.Testing.Autofixture;
using Hanno.ViewModels;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Hanno.Tests.ViewModels
{
	public class SampleBindable : Bindable
	{
		public SampleBindable(ISchedulers schedulers)
			: base(schedulers)
		{

		}
		public string Property
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public object Property2
		{
			get { return GetValue<object>(); }
			set { SetValue(value); }
		}

		public string GetPropertyWithFactory(Func<string> factory)
		{
			return GetValue(factory, "PropertyWithFactory");
		}

		public void CallInvalidate(params Expression<Func<object>>[] properties)
		{
			Invalidate(properties);
		}
	}

	[Trait("Category", "Hello")]
	public class BindableTests
	{
		[Theory, RxAutoData]
		public void TestProperty(
			SampleBindable sut,
			WritablePropertyAssertion assertion)
		{
			assertion.VerifyProperty(() => sut.Property);
		}

		[Theory, RxAutoData]
		public void SetProperty_WithNewValue_ShouldRaiseNotifyPropertyChanged(
			SampleBindable sut,
			string value1,
			string value2)
		{
			//arrange
			sut.Property = value1;
			sut.MonitorEvents();

			//act
			sut.Property = value2;

			//assert
			sut.ShouldRaise("PropertyChanged").WithArgs<PropertyChangedEventArgs>(arg => arg.PropertyName == "Property");
		}

		[Theory, RxAutoData]
		public void SetProperty_WithSameValue_ShouldNotRaiseNotifyPropertyChanged(
			SampleBindable sut,
			string value1)
		{
			//arrange
			sut.Property = value1;
			sut.MonitorEvents();

			//act
			sut.Property = value1;

			//assert
			sut.ShouldNotRaise("PropertyChanged");
		}

		[Theory, RxAutoData]
		public void Invalidate_ShouldResetValue(
		  SampleBindable sut,
			string value1,
			object value2)
		{
			//arrange
			sut.Property = value1;
			sut.Property2 = value2;

			//act
			sut.CallInvalidate(() => sut.Property, () => sut.Property2);

			//assert
			sut.Property.Should().Be(default(string));
			sut.Property2.Should().Be(default(object));
		}

		[Theory, RxAutoData]
		public void GetProperty_WithFactory_ShouldReturnCorrectValue(
		  SampleBindable sut,
			string expected)
		{
			//arrange

			//act
			var actual = sut.GetPropertyWithFactory(() => expected);

			//assert
			actual.Should().Be(expected);
		}
	}
}