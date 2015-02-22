using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.CqrsInfrastructure;
using Hanno.Http;
using Hanno.Testing.Autofixture;
using Moq;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace Hanno.Tests.Http
{
	public class DefaultHttpRequestResolverTests
	{
		[Theory, AutoMoqData]
		public void Sut_IsHttpRequestResolver(DefaultHttpRequestResolver sut)
		{
			sut.Should().BeAssignableTo<IHttpRequestResolver>();
		}

		[Theory, AutoMoqData]
		public void Sut_VerifyConstructors(GuardClauseAssertion assertion)
		{
			assertion.VerifyConstructors<DefaultHttpRequestResolver>();
		}

		[Theory, AutoMoqData]
		public void Resolve_VerifyGuardClause(GuardClauseAssertion assertion, IAsyncQuery<string> query)
		{
			assertion.Verify<DefaultHttpRequestResolver>(d => d.ResolveHttpRequest(query));
		}

		[Theory, AutoMoqData]
		public async Task Resolve_ShouldReturnCorrectValue(
			[Frozen]Mock<IHttpRequestSpecificationResolver> mockSpecResolver,
			[Frozen]Mock<IHttpRequestBuilder> mockBuilder,
			[Frozen]Mock<IHttpRequestSpecificationsTransformer> mockTransformer,
            DefaultHttpRequestResolver sut,
			Mock<IHttpRequestBuilderOptions> options,
            IAsyncQuery<string> query,
			IHttpRequestSpecification specs,
			IHttpRequest expected
			)
		{
			//arrange
			mockSpecResolver.Setup(m => m.ResolveHttpRequestSpecification(query)).Returns(specs);
			mockTransformer.Setup(m => m.TransformHttpSpecificationsToHttpBuilderOptions(specs, query))
				.ReturnsTask(options.Object);
			options.Setup(m => m.ToRequest(mockBuilder.Object)).Returns(expected);

			//act
			var actual = await sut.ResolveHttpRequest(query);

			//assert
			actual.Should().Be(expected);
		}
	}
}
