using System.Threading;
using Hanno.CqrsInfrastructure;
using Hanno.Testing.Autofixture;
using Ploeh.AutoFixture.Xunit2;
using Xunit;
using FluentAssertions;

namespace Hanno.Tests.CqrsInfrastructure
{
	public class AsyncCommandBaseTests
	{
		public class TestAsyncCommand : AsyncCommandBase
		{
			public TestAsyncCommand(CancellationToken ct) : base(ct)
			{
			}
		}

		[Theory, AutoMoqData]
		public void Sut_ShouldBeAsyncCommand(
			AsyncCommandBase sut)
		{
			sut.Should().BeAssignableTo<IAsyncCommand>();
		}

		[Theory, AutoData]
		public void CancellationToken_ShouldReturnCorrectValue(
			)
		{
			//arrange
			var source = new CancellationTokenSource();
			var sut = new TestAsyncCommand(source.Token);

			//act
			source.Cancel();

			//assert
			sut.CancellationToken.IsCancellationRequested.Should().BeTrue();
		}
	}
}