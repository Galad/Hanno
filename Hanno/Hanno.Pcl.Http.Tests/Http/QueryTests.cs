using System;
using System.Globalization;
using System.Threading;
using Hanno.CqrsInfrastructure;
using Hanno.Http;

namespace Hanno.Tests.Http
{
	public class TestQuery : AsyncQueryBase<string>
	{
		public TestQuery(CancellationToken ct)
			: base(ct)
		{
		}

		public int Id { get; set; }
	}

	public class TestSpec : BuildSynchronousHttpRequest<TestQuery>, IHttpRequestSpecification
	{
		public IHttpRequestBuilderOptions CreateSpecifiation(IHttpRequestBuilderFactory httpRequestMethodBuilder)
		{
			return httpRequestMethodBuilder.CreateRequestBuilder(new Uri("http://test.com"))
										   .Post()
										   .AppendPath("test")
										   .Parameter("hello", "test");
		}

		protected override IHttpRequestBuilderOptions BuildSynchronously(IHttpRequestBuilderOptions options, TestQuery query)
		{
			return options.Parameter("id", query.Id.ToString(CultureInfo.InvariantCulture));
		}
	}

}
