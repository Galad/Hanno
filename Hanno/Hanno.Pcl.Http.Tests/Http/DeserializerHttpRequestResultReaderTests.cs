using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.Testing.Autofixture;
using Hanno.Http;
using Hanno.Serialization;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Xunit2;
using Xunit;
using Ploeh.AutoFixture.Idioms;

namespace Hanno.Tests.Http
{
	#region Customization
	public class DeserializerHttpRequestResultReaderCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.Freeze<MemoryStream>(b => b.FromFactory(() => new MemoryStream()).OmitAutoProperties());
			fixture.Freeze<Mock<IHttpResponseContent>>(b =>
				b.Do(content => content.Setup(c => c.ReadStream(It.IsAny<CancellationToken>())).ReturnsTask(fixture.Create<MemoryStream>())));
			fixture.Customize<Mock<IHttpRequestResult>>(b =>
				b.Do(result => result.Setup(r => r.Content).Returns(fixture.Create<IHttpResponseContent>)));


		}
	}

	public class DeserializerHttpRequestResultReaderCompositeCustomization : CompositeCustomization
	{
		public DeserializerHttpRequestResultReaderCompositeCustomization()
			: base(new DeserializerHttpRequestResultReaderCustomization(), new AutoMoqCustomization())
		{
		}
	}

	public class DeserializerHttpRequestResultReaderAutoDataAttribute : AutoDataAttribute
	{
		public DeserializerHttpRequestResultReaderAutoDataAttribute()
			: base(new Fixture().Customize(new DeserializerHttpRequestResultReaderCompositeCustomization()))
		{
		}
	}

	public class DeserializerHttpRequestResultReaderInlineAutoDataAttribute : CompositeDataAttribute
	{
		public DeserializerHttpRequestResultReaderInlineAutoDataAttribute(params object[] values)
			: base(new InlineDataAttribute(values), new DeserializerHttpRequestResultReaderAutoDataAttribute())
		{
		}
	}
	#endregion
	public class DeserializerHttpRequestResultReaderTests
	{
		[Theory, AutoMoqData]
		public void Sut_ShouldBeHttpRequestResultReader(DeserializerHttpRequestResultReader sut)
		{
			sut.Should().BeAssignableTo<IHttpRequestResultReader>();
		}

		[Theory, AutoMoqData]
		public void Sut_VerifyGuardClauses(GuardClauseAssertion assertion)
		{
			assertion.VerifyConstructors<DeserializerHttpRequestResultReader>();
		}

		[Theory, DeserializerHttpRequestResultReaderAutoData]
		public async Task Read_ShouldReturnCorrectValue(
			[Frozen]Mock<IDeserializer> deserializer,
			IHttpRequestResult result,
			DeserializerHttpRequestResultReader sut,
			MemoryStream stream,
			object expected)
		{
			//arrange
			deserializer.Setup(d => d.Deserialize<object>(stream)).Returns(expected);

			//act
			var actual = await sut.Read<object>(result, CancellationToken.None);

			//assert
			actual.Should().Be(expected);
		}

		[Theory, DeserializerHttpRequestResultReaderAutoData]
		public async Task Read_ShouldDisposeStream(
			IHttpRequestResult result,
			DeserializerHttpRequestResultReader sut,
			MemoryStream stream,
			object expected)
		{
			//arrange

			//act
			await sut.Read<object>(result, CancellationToken.None);

			//assert
			stream.CanRead.Should().BeFalse();
		}

		[Theory, DeserializerHttpRequestResultReaderAutoData]
		public async Task Read_WhenExceptionIsThrown_ShouldDisposeStream(
			[Frozen]Mock<IDeserializer> deserializer,
			IHttpRequestResult result,
			DeserializerHttpRequestResultReader sut,
			MemoryStream stream,
			object expected)
		{
			//arrange
			deserializer.Setup(d => d.Deserialize<object>(stream)).Throws(new Exception());

			//act
			try
			{
				await sut.Read<object>(result, CancellationToken.None);
			}
			catch (Exception)
			{
			}
			

			//assert
			stream.CanRead.Should().BeFalse();
		}
	}
}
