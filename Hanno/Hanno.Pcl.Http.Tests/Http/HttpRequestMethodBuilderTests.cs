using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using Hanno.Http;
using Hanno.Testing.Autofixture;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Hanno.Tests.Http
{
	public class HttpRequestMethodBuilderTests
	{
		[Theory, AutoData]
		public void Sut_ShouldBeHttpRequestMethodBuilder(
		  HttpRequestBuilderOptions sut)
		{
			sut.Should().BeAssignableTo<IHttpRequestMethodBuilder>();
		}

		[Theory, AutoData]
		public void BaseUri_ShouldReturnCorrectValue(
			[Frozen]Uri expected,
			HttpRequestBuilderOptions sut)
		{
			sut.BaseUri.Should().Be(expected);
		}

		[Theory, AutoData]
		public void Sut_WhenConstructedWithNotHttpUri_ShouldThrow()
		{
			Action action = () => new HttpRequestBuilderOptions(new Uri("protocol://address"));
			action.ShouldThrow<ArgumentException>();
		}

		#region Method
		[Theory, AutoData]
		public void Method_ShouldSetMethod(
		  HttpRequestBuilderOptions sut,
		  HttpMethod method)
		{
			//arrange

			//act
			sut.Method(method);

			//assert
			sut.HttpMethod.Should().Be(method);
		}

		[Theory, AutoData]
		public void Method_WithUndefinedMethod_ShouldThrow(
		  HttpRequestBuilderOptions sut)
		{
			//arrange
			var method = default(HttpMethod);

			//act
			Action action = () => sut.Method(method);

			//assert
			action.ShouldThrow<ArgumentException>();
		}


		[Theory, AutoData]
		public void Method_ShouldReturnSut(
		  HttpRequestBuilderOptions sut,
			HttpMethod method)
		{
			//arrange

			//act
			var actual = sut.Method(method);

			//assert
			actual.Should().Be(sut);
		}
		#endregion

		#region AppendPath
		[Theory, AutoData]
		public void AppendPath_ShouldAddPath(
		  HttpRequestBuilderOptions sut,
			string expected)
		{
			//arrange

			//act
			sut.AppendPath(expected);
			var actual = sut.PathFragments.Last();

			//assert
			actual.Should().Be(expected);
		}

		[Theory,
		InlineAutoData("path1/path2", "path1/path2"),
		InlineAutoData("/path1/path2", "path1/path2"),
		InlineAutoData("//path1/path2//", "path1/path2"),
		InlineAutoData("path1//path2", "path1/path2"),
		]
		public void AppendPath_WithPathSeparator_ShouldAddCorrectPath(
			string path,
			string expectedPath,
		  HttpRequestBuilderOptions sut
			)
		{
			//arrange

			//act
			sut.AppendPath(path);

			//assert
			var expected = expectedPath.Split('/');
			sut.PathFragments.ShouldAllBeEquivalentTo(expected);
		}

		[Theory, AutoData]
		public void AppendPath_ShouldReturnSut(
		  HttpRequestBuilderOptions sut,
			string expected)
		{
			//act
			var actual = sut.AppendPath(expected);

			//assert
			actual.Should().Be(sut);
		}

		[Theory,
		 InlineAutoData(""),
		 InlineAutoData((string)null)
		]
		public void AppendPath_WithEmptyStrings_ShouldThrow(
			string path,
			HttpRequestBuilderOptions sut
			)
		{
			//act
			Action action = () => sut.AppendPath(path);

			//assert
			action.ShouldThrow<ArgumentNullException>();
		}

		#endregion

		#region Parameters
		[Theory, AutoData]
		public void Parameter_ShouldAddParameter(
		  HttpRequestBuilderOptions sut,
			string key,
			string value)
		{
			//arrange

			//act
			sut.Parameter(key, value);

			//assert
			sut.Parameters.Should().Contain(new KeyValuePair<string, string>(key, value));
		}

		[Theory, AutoData]
		public void Parameter_WithEmptyKey_ShouldThrow(
			HttpRequestBuilderOptions sut,
			string value)
		{
			//arrange

			//act
			Action action = () => sut.Parameter("", value);

			//assert
			action.ShouldThrow<ArgumentNullException>();
		}

		[Theory, AutoData]
		public void Parameter_AddingSameKeyTwice_ShouldThrow(
		  HttpRequestBuilderOptions sut,
			string key,
			string value)
		{
			//arrange
			sut.Parameter(key, value);

			//act
			Action action = () => sut.Parameter(key, value);

			//assert
			action.ShouldThrow<ArgumentException>();
		}

		[Theory, AutoData]
		public void Parameter_ShouldReturnSut(
		  HttpRequestBuilderOptions sut,
			string key,
			string value)
		{
			//act
			var actual = sut.Parameter(key, value);

			//assert
			actual.Should().Be(sut);
		}
		#endregion

		#region PayloadParameters
		[Theory, AutoData]
		public void PayloadParameter_ShouldAddPayloadParameter(
		  HttpRequestBuilderOptions sut,
			string key,
			string value)
		{
			//arrange

			//act
			sut.PayloadParameter(key, value);

			//assert
			sut.PayloadParameters.Should().Contain(new KeyValuePair<string, string>(key, value));
		}

		[Theory, AutoData]
		public void PayloadParameter_WithEmptyKey_ShouldThrow(
			HttpRequestBuilderOptions sut,
			string value)
		{
			//arrange

			//act
			Action action = () => sut.PayloadParameter("", value);

			//assert
			action.ShouldThrow<ArgumentNullException>();
		}

		[Theory, AutoData]
		public void PayloadParameter_AddingSameKeyTwice_ShouldThrow(
		  HttpRequestBuilderOptions sut,
			string key,
			string value)
		{
			//arrange
			sut.PayloadParameter(key, value);

			//act
			Action action = () => sut.PayloadParameter(key, value);

			//assert
			action.ShouldThrow<ArgumentException>();
		}

		[Theory, AutoData]
		public void PayloadParameter_ShouldReturnSut(
		  HttpRequestBuilderOptions sut,
			string key,
			string value)
		{
			//act
			var actual = sut.PayloadParameter(key, value);

			//assert
			actual.Should().Be(sut);
		}
		#endregion

		#region Cookie
		[Theory, AutoMoqData]
		public void Cookie_ShouldAddCookie(
		  HttpRequestBuilderOptions sut,
			Uri uri)
		{
			//arrange
			var cookie = new Cookie();

			//act
			sut.Cookie(uri, cookie);

			//assert
			sut.Cookies.Should().Contain(new Tuple<Uri, Cookie>(uri, cookie));
		}

		[Theory, AutoMoqData]
		public void Cookie_ShouldReturnSut(
		  HttpRequestBuilderOptions sut,
			Uri uri)
		{
			//arrange
			var cookie = new Cookie();

			//act
			var actual = sut.Cookie(uri, cookie);

			//assert
			actual.Should().Be(sut);
		}
		#endregion

		#region Header
		[Theory, AutoData]
		public void Header_ShouldAddHeader(
		  HttpRequestBuilderOptions sut,
			string key,
			string value)
		{
			//arrange

			//act
			sut.Header(key, value);

			//assert
			sut.Headers.Should().Contain(new KeyValuePair<string, string>(key, value));
		}

		[Theory, AutoData]
		public void Header_ShouldReturnSut(
		  HttpRequestBuilderOptions sut,
			string key,
			string value)
		{
			//arrange

			//act
			var actual = sut.Header(key, value);

			//assert
			actual.Should().Be(sut);
		}
		#endregion

		#region Uri
		[Theory, AutoData]
		public void Uri_ShouldReturnCorrectValue(
			[Frozen] Uri baseUri,
		  HttpRequestBuilderOptions sut,
			string path,
			string path2,
			string key,
			string value)
		{
			//arrange
			//add reserved caracters to test escaping
			value += "?";
			key += "=";
			path += "?";
			path2 += "=";
			sut.AppendPath(path)
			   .AppendPath(path2)
			   .Parameter(key, value);

			//act
			var actual = sut.Uri;

			//assert
			var expected = string.Format("{0}/{1}/{2}?{3}={4}", baseUri.ToString().TrimEnd('/'), Uri.EscapeDataString(path), Uri.EscapeDataString(path2), Uri.EscapeDataString(key), Uri.EscapeDataString(value));
			actual.Should().Be(expected);
		}

		[Theory, AutoData]
		public void Uri_WithNoParameters_ShouldReturnCorrectValue(
			[Frozen] Uri baseUri,
		  HttpRequestBuilderOptions sut,
			string path,
			string path2)
		{
			//arrange
			sut.AppendPath(path)
			   .AppendPath(path2);

			//act
			var actual = sut.Uri;

			//assert
			var expected = string.Format("{0}/{1}/{2}", baseUri.ToString().TrimEnd('/'), path, path2);
			actual.Should().Be(expected);
		}
		#endregion

		#region ToRequest
		[Theory, AutoMoqData]
		public void ToRequest_ShouldReturnCorrectValue(
		  HttpRequestBuilderOptions sut,
			Mock<IHttpRequestBuilder> requestBuilder,
			IHttpRequest request)
		{
			//arrange
			requestBuilder.Setup(b => b.BuildRequest(sut)).Returns(request);

			//act
			var actual = sut.ToRequest(requestBuilder.Object);

			//assert
			actual.Should().Be(request);
		}
		#endregion

	}
}