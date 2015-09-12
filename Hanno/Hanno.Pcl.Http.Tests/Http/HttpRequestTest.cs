using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.Http;
using Ploeh.AutoFixture.Xunit2;
using Xunit;
using Xunit;

namespace Hanno.Tests.Http
{
	[Trait("Category", "Hello")]
	public class HttpRequestTest
	{
		[Theory, AutoData]
		public async Task Execute_ShouldReturnCorrectValue()
		{
			//arrange
			var builder = new HttpRequestBuilder();
			var builderFactory = new HttpRequestBuilderFactory();

			var request = builderFactory.CreateRequestBuilder(new Uri("http://www.bing.com"))
										.Get()
										.AppendPath("search")
										.Parameter("q", "test")
										.ToRequest(builder);

			//act
			var result = await request.Execute(new CancellationToken());
			
			//assert
			result.StatusCode.Should().Be(HttpStatusCode.OK);
		}

		[Theory, AutoData]
		public void Execute_WithInvalidUri_ShouldThrow(string path)
		{
			//arrange
			var builder = new HttpRequestBuilder();
			var builderFactory = new HttpRequestBuilderFactory();

			var request = builderFactory.CreateRequestBuilder(new Uri("http://www.bing.com"))
										.Get()
										.AppendPath(path)
										.Parameter("q", "test")
										.ToRequest(builder);
			
			//act
			Func<Task> action = async () => await request.Execute(CancellationToken.None);
			
			//assert
			action.ShouldThrow<HttpRequestSuccessException>();
		}
	}
}
