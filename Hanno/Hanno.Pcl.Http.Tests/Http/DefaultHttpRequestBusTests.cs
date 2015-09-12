using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.CqrsInfrastructure;
using Hanno.Http;
using Hanno.Testing.Autofixture;
using Moq;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Hanno.Tests.Http
{
	public class DefaultHttpRequestBusTests
	{
		public class TestCommand : AsyncCommandBase
		{
			public TestCommand() :base(CancellationToken.None)
			{

			}
		}

		public class TestQuery : AsyncQueryBase<object>
		{
			public TestQuery() : base(CancellationToken.None) { }
		}

		[Theory, AutoMoqData]
		public void Sut_ShouldBeAsyncCommandBus(DefaultHttpRequestBus sut)
		{
			sut.Should().BeAssignableTo<IAsyncCommandBus>();
		}

		[Theory, AutoMoqData]
		public void Sut_ShouldBeAsyncQueryBus(DefaultHttpRequestBus sut)
		{
			sut.Should().BeAssignableTo<IAsyncQueryBus>();
		}

		[Theory, AutoMoqData]
		public void Sut_VerifyConstructorGuardClauses(GuardClauseAssertion assertion)
		{
			assertion.VerifyConstructors<DefaultHttpRequestBus>();
        }

		[Theory, AutoMoqData]
		public async Task Sut_ProcessCommand_ShouldReadCorrectResponse(
			[Frozen]Mock<IHttpRequestResolver> mockResolver,
			[Frozen]Mock<IHttpRequestResultReaderResolver> mockReaderResolver,
			DefaultHttpRequestBus sut,
			Mock<IHttpRequest> mockRequest,
			Mock<IHttpRequestResult> mockResult,
			Mock<IHttpRequestResultReader> mockReader,
			TestCommand command)
		{
			//arrange
			mockRequest.Setup(m => m.Execute(It.IsAny<CancellationToken>())).ReturnsTask(mockResult.Object);
			mockResolver.Setup(m => m.ResolveHttpRequest(command)).ReturnsTask(mockRequest.Object);			
			mockReaderResolver.Setup(m => m.ResolveHttpRequestResultReader(command)).Returns(mockReader.Object);

			//act
			await sut.ProcessCommand(command);

			//assert
			mockReader.Verify(r => r.Read<NoResponse>(mockResult.Object, It.IsAny<CancellationToken>()));
		}

		[Theory, AutoMoqData]
		public async Task Sut_ProcessQuery_ShouldReturnCorrectValue(
			[Frozen]Mock<IHttpRequestResolver> mockResolver,
			[Frozen]Mock<IHttpRequestResultReaderResolver> mockReaderResolver,
			DefaultHttpRequestBus sut,
			Mock<IHttpRequest> mockRequest,
			Mock<IHttpRequestResult> mockResult,
			Mock<IHttpRequestResultReader> mockReader,
			TestQuery query,
			object expected)
		{
			//arrange
			mockRequest.Setup(m => m.Execute(It.IsAny<CancellationToken>())).ReturnsTask(mockResult.Object);
			mockResolver.Setup(m => m.ResolveHttpRequest(query)).ReturnsTask(mockRequest.Object);
			mockReaderResolver.Setup(m => m.ResolveHttpRequestResultReader(query)).Returns(mockReader.Object);
			mockReader.Setup(r => r.Read<object>(mockResult.Object, It.IsAny<CancellationToken>()))
				.ReturnsTask(expected);

			//act
			var actual = await sut.ProcessQuery<TestQuery, object>(query);

			//assert
			actual.Should().Be(expected);
		}
	}
}
