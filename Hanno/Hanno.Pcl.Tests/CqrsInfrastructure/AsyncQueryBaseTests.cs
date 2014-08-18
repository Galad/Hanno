using System.Threading;
using Hanno.CqrsInfrastructure;
using Hanno.Testing.Autofixture;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;
using FluentAssertions;

namespace Hanno.Tests.CqrsInfrastructure
{
	public class AsyncQueryBaseTests
	{
		public class TestAsyncQuery : AsyncQueryBase<object>
		{
			public TestAsyncQuery(CancellationToken ct) : base(ct)
			{
			}
		}

		[Theory, AutoMoqData]
		public void Sut_ShouldBeAsyncCommand(
			AsyncQueryBase<object> sut)
		{
			sut.Should().BeAssignableTo<IAsyncQuery<object>>();
		}

		[Theory, AutoData]
		public void CancellationToken_ShouldReturnCorrectValue(
			)
		{
			//arrange
			var source = new CancellationTokenSource();
			var sut = new TestAsyncQuery(source.Token);

			//act
			source.Cancel();

			//assert
			sut.CancellationToken.IsCancellationRequested.Should().BeTrue();
		}
	}
}