using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.Testing.Autofixture;
using Moq;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Hanno.Tests
{
	public class CompositeParallelInitializableTests
	{
		[Theory, AutoMoqData]
		public void Sut_ShouldBeInitializable(CompositeParallelInitializable sut)
		{
			sut.Should().BeAssignableTo<IInitializable>();
		}

		[Theory, AutoMoqData]
		public void Sut_VerifyConstructorGuardClauses(GuardClauseAssertion assertion)
		{
			assertion.VerifyConstructors<CompositeParallelInitializable>();
		}

		[Theory, AutoMoqData]
		public async Task Initialize_ShouldCallInnerIntialize(
			IEnumerable<Mock<IInitializable>> mockInitializable
			)
		{
			//arrange
			mockInitializable.ForEach(m => m.Setup(mm => mm.Initialize(It.IsAny<CancellationToken>()))
							.ReturnsDefaultTask()
							.Verifiable())
							.ToArray();
			var sut = new CompositeParallelInitializable(mockInitializable.Select(m => m.Object));

			//act
			await sut.Initialize(CancellationToken.None);

			//assert
			mockInitializable.ForEach(m => m.Verify()).ToArray();
		}
	}
}
