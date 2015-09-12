using System;
using System.Threading;
using System.Threading.Tasks;
using Hanno.CqrsInfrastructure;
using Hanno.Testing.Autofixture;
using Moq;
using Moq.Protected;
using Ploeh.AutoFixture.Xunit2;
using Xunit;
using FluentAssertions;

namespace Hanno.Tests.CqrsInfrastructure
{
	public class DelegateQueryHandlerTests
	{
		public class TestQuery : AsyncQueryBase<object>
		{
			public TestQuery(CancellationToken ct) : base(ct)
			{
			}
		}

		public class TestQueryHandler : DelegateQueryHandler<TestQuery, object>
		{
			protected override object ExecuteOverride(TestQuery command)
			{
				throw new System.NotImplementedException();
			}
		}

		[Theory, AutoData]
		public async Task Execute_ShouldReturnCorrectValue(
		  Mock<DelegateQueryHandler<TestQuery, object>> sut,
		  TestQuery query,
		  object expected)
		{
			//arrange
			sut.Protected().Setup<object>("ExecuteOverride", query).Returns(expected);

			//act
			var actual = await sut.Object.Execute(query);

			//assert
			actual.Should().Be(expected);
		}

		[Theory, AutoMoqData]
		public void Sut_ShouldBeAsyncQueryHandler(
		  DelegateQueryHandler<TestQuery, object> sut,
		  TestQuery query,
		  object expected)
		{
			sut.Should().BeAssignableTo<IAsyncQueryHandler<TestQuery, object>>();
		}
	}
}