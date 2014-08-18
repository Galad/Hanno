using System;
using FluentAssertions;
using Hanno.Http;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Hanno.Tests.Http
{
	public class HttpRequestBuilderFactoryTests
	{
		[Theory, AutoData]
		public void Sut_ShouldBeHttpRequestBuilderFactory(HttpRequestBuilderFactory sut)
		{
			sut.Should().BeAssignableTo<IHttpRequestBuilderFactory>();
		}

		[Theory, AutoData]
		public void CreateRequestBuilder_ShouldReturnCorrectValue(
		  HttpRequestBuilderFactory sut,
			Uri uri)
		{
			//arrange

			//act
			var actual = sut.CreateRequestBuilder(uri);

			//assert
			actual.Should().BeOfType<HttpRequestBuilderOptions>();
		}

		[Theory, AutoData]
		public void 
			CreateRequestBuilder_WithNotHttpUri_ShouldThrow(
			HttpRequestBuilderFactory sut)
		{
			//arrange
			var uri = new Uri("protocol://path");

			//act
			Action action = () => sut.CreateRequestBuilder(uri);

			//assert
			action.ShouldThrow<ArgumentException>();
		}
	}
}
