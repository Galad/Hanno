using System.Reactive.Linq;
using System.Threading;
using FluentAssertions;
using Hanno.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hanno.Testing.Autofixture;
using Hanno.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Hanno.Tests.ViewModels
{
	public class SampleValidable : Validable
	{
		public SampleValidable(IValidator validator, ISchedulers scheduler)
			: base(validator, scheduler)
		{
		}

		public IValidator ValidatorPublic { get { return Validator; } }

		public string Property
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string Property2
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public string Property3
		{
			get { return GetValue<string>(); }
			set { SetValue(value); }
		}

		public virtual Task<bool> SetValueAsyncPublic<T>(T value, string key, CancellationTokenSource cancellationTokenSource)
		{
			return SetValueAsync(value, key, cancellationTokenSource);
		}
	}

	#region Customization
	public class ValidableCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.Customize<SampleValidable>(c =>
				c.Without(s => s.Property)
				 .Without(s => s.Property2));
			fixture.Customize<Mock<IValidator>>(c =>
				c.Do(mock => mock.Setup(v => v.Validate(It.IsAny<object>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsTask(() => NoError.Value)));
		}
	}

	public class ValidableCompositeCustomization : CompositeCustomization
	{
		public ValidableCompositeCustomization()
			: base(new AutoMoqCustomization(), new ValidableCustomization(), new RxCustomization())
		{
		}
	}

	public class ValidableAutoDataAttribute : AutoDataAttribute
	{
		public ValidableAutoDataAttribute()
			: base(new Fixture().Customize(new ValidableCompositeCustomization()))
		{
		}
	}

	public class ValidableInlineAutoDataAttribute : CompositeDataAttribute
	{
		public ValidableInlineAutoDataAttribute(params object[] values)
			: base(new InlineDataAttribute(values), new ValidableAutoDataAttribute())
		{
		}
	}
	#endregion

	public class ValidableTests
	{
		[Theory(Timeout = 5000), ValidableAutoData]
		public async Task HasError_ShouldReturnCorrectValue(
			[Frozen] Mock<IValidator> validator,
			SampleValidable sut,
			string value)
		{
			//arrange
			validator.Setup(v => v.Validate(value, "Property", It.IsAny<CancellationToken>())).ReturnsTask(() => NoError.Value);

			//act
			await sut.SetValueAsyncPublic(value, "Property", new CancellationTokenSource());
			var actual = await sut.HasError(() => sut.Property);

			//assert
			actual.Should().Be(false);
		}

		[Theory(Timeout = 5000), ValidableAutoData]
		public async Task HasError_WithPropertyValidatedTwice_ShouldReturnTrue(
			[Frozen] Mock<IValidator> validator,
			SampleValidable sut,
			IValidationError error1,
			IValidationError error2,
			string value1,
			string value2)
		{
			//arrange
			validator.Setup(v => v.Validate(value1, "Property", It.IsAny<CancellationToken>())).ReturnsTask(error1);
			validator.Setup(v => v.Validate(value2, "Property", It.IsAny<CancellationToken>())).ReturnsTask(error2);

			//act
			await sut.SetValueAsyncPublic(value1, "Property", new CancellationTokenSource());
			await sut.SetValueAsyncPublic(value2, "Property", new CancellationTokenSource());
			var actual = await sut.HasError(() => sut.Property);

			//assert
			actual.Should().BeTrue();
		}

		[Theory(Timeout = 5000), ValidableAutoData]
		public async Task HasError_WithPropertyValidatedTwice_ShouldReturnFalse(
			[Frozen] Mock<IValidator> validator,
			SampleValidable sut,
			IValidationError error1,
			string value1,
			string value2)
		{
			//arrange
			validator.Setup(v => v.Validate(value1, "Property", It.IsAny<CancellationToken>())).ReturnsTask(error1);
			validator.Setup(v => v.Validate(value2, "Property", It.IsAny<CancellationToken>())).ReturnsTask(NoError.Value);

			//act
			await sut.SetValueAsyncPublic(value1, "Property", new CancellationTokenSource());
			await sut.SetValueAsyncPublic(value2, "Property", new CancellationTokenSource());
			var actual = await sut.HasError(() => sut.Property);

			//assert
			actual.Should().BeFalse();
		}

		[Theory, ValidableAutoData]
		public async Task Sut_WhenSettingProperty_ShouldRaiseBeginValidation(
			[Frozen] Mock<IValidator> validator,
			SampleValidable sut,
			string value,
			CancellationTokenSource cancellationTokenSource)
		{
			//arrange
			sut.MonitorEvents();

			//act
			await sut.SetValueAsyncPublic(value, "Property", cancellationTokenSource);

			//assert
			sut.ShouldRaise("BeginValidation");
		}

		[Theory, ValidableAutoData]
		public async Task Sut_WhenSettingProperty_ShouldRaiseErrorChanged(
			[Frozen] TestSchedulers schedulers,
			[Frozen] Mock<IValidator> validator,
			SampleValidable sut,
			string value)
		{
			//arrange
			validator.Setup(v => v.Validate(value, "Property", It.IsAny<CancellationToken>()))
					 .ReturnsTask(new ValidationError("message", null));
			sut.MonitorEvents();

			//act
			await sut.SetValueAsyncPublic(value, "Property", new CancellationTokenSource());

			//assert
			sut.ShouldRaise("ErrorsChanged");
		}

		#region HasErrorsData
		public static IEnumerable<object[]> HasErrorsData1
		{
			get
			{
				yield return new object[]
				{
					new[]
					{
						new ValidationError("error", null),
						NoError.Value,
						NoError.Value
					},
					new[] {"Property", "Property2", "Property3"},
					true
				};
			}
		}
		public static IEnumerable<object[]> HasErrorsData2
		{
			get
			{
				yield return new object[]
				{
					new[]
					{
						NoError.Value,
						NoError.Value,
						NoError.Value
					},
					new[] {"Property", "Property2", "Property3"},
					false
				};
			}
		}
		public static IEnumerable<object[]> HasErrorsData3
		{
			get
			{
				yield return new object[]
				{
					new[]
					{
						new ValidationError("error", null),
						new ValidationError("error2", null),
						new ValidationError("error3", null),
					},
					new[] {"Property", "Property2", "Property3"},
					true
				};
			}
		} 
		#endregion
	
		public class HasErrorsTestData : CompositeDataAttribute
		{
			public HasErrorsTestData(string property)
				: base(new PropertyDataAttribute(property), new ValidableAutoDataAttribute())
			{
			}
		}

		[Theory,
		HasErrorsTestData("HasErrorsData1"),
		HasErrorsTestData("HasErrorsData2"),
		HasErrorsTestData("HasErrorsData3"),
		]
		public async Task HasErrors_ShouldReturnCorrectValue(
			IValidationError[] errors,
			string[] properties,
			bool expected,
			[Frozen] Mock<IValidator> validator,
			SampleValidable sut,
			IFixture fixture)
		{
			//arrange
			var values = fixture.CreateMany<string>(properties.Length).ToArray();
			for (var i = 0; i < errors.Length; i++)
			{
				int i1 = i;
				validator.Setup(v => v.Validate(values[i1], properties[i1], It.IsAny<CancellationToken>())).ReturnsTask(() => errors[i1]);
			}

			//act
			for (var i = 0; i < errors.Length; i++)
			{
				await sut.SetValueAsyncPublic(values[i], properties[i], new CancellationTokenSource());
			}

			var actual = await sut.HasErrorsAsync();

			//assert
			actual.Should().Be(expected);
		}

		[Theory, ValidableAutoData]
		public void GetError_ShouldReturnCorrectValue(
			SampleValidable sut)
		{
			//arrange

			//act
			var actual = sut.GetError("Property");

			//assert
			actual.Should().Be(NoError.Value);
		}


		[Theory, ValidableAutoData]
		public async Task GetError_WithValidatedPropertyWithNoError_ShouldReturnCorrectValue(
			[Frozen] Mock<IValidator> validator,
			SampleValidable sut,
			string value)
		{
			//arrange
			validator.Setup(v => v.Validate(value, "Property", It.IsAny<CancellationToken>()))
			         .ReturnsTask(NoError.Value);
			//act
			await sut.SetValueAsyncPublic(value, "Property", new CancellationTokenSource());
			var actual = sut.GetError("Property");

			//assert
			actual.Should().Be(NoError.Value);
		}

		[Theory, ValidableAutoData]
		public async Task GetError_WithValidatedPropertyWithError_ShouldReturnCorrectValue(
			[Frozen] Mock<IValidator> validator,
			SampleValidable sut,
			IValidationError error,
			string value)
		{
			//arrange
			validator.Setup(v => v.Validate(value, "Property", It.IsAny<CancellationToken>()))
					 .ReturnsTask(error);
			//act
			await sut.SetValueAsyncPublic(value, "Property", new CancellationTokenSource());
			var actual = sut.GetError("Property");

			//assert
			actual.Should().Be(error);
		}
		[Theory, ValidableAutoData]
		public async Task GetError_WithValidatedTwicePropertyWithError_ShouldReturnCorrectValue(
			[Frozen] Mock<IValidator> validator,
			SampleValidable sut,
			IValidationError error1,
			IValidationError error2,
			string value1,
			string value2)
		{
			//arrange
			validator.Setup(v => v.Validate(value1, "Property", It.IsAny<CancellationToken>()))
					 .ReturnsTask(error1);
			validator.Setup(v => v.Validate(value2, "Property", It.IsAny<CancellationToken>()))
					 .ReturnsTask(error2);
			//act
			await sut.SetValueAsyncPublic(value1, "Property", new CancellationTokenSource());
			await sut.SetValueAsyncPublic(value2, "Property", new CancellationTokenSource());
			var actual = sut.GetError("Property");

			//assert
			actual.Should().Be(error2);
		}
	}
}
