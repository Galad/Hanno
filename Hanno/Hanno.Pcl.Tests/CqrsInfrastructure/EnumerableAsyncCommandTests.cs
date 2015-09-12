using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.CqrsInfrastructure;
using Hanno.Testing.Autofixture;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Hanno.Tests.CqrsInfrastructure
{
	public class EnumerableAsyncCommandTests
	{
		[Theory, AutoMoqData]
		public void Sut_IsAsyncCommand(EnumerableAsyncCommand<IAsyncCommand> sut)
		{
			sut.Should().BeAssignableTo<IAsyncCommand>();
		}

		[Theory, AutoMoqData]
		public void Sut_VerifyConstructor(GuardClauseAssertion assertion)
		{
			assertion.VerifyConstructors<EnumerableAsyncCommand<IAsyncCommand>>();
		}

		[Theory, AutoMoqData]
		public void Sut_Commands(
			[Frozen]IEnumerable<IAsyncCommand> commands,
			EnumerableAsyncCommand<IAsyncCommand> sut)
		{
			sut.Commands.Should().Equal(commands);
		}
	}
}
