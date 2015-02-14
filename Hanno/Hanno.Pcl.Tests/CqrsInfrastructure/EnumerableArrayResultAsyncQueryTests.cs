using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.CqrsInfrastructure;
using Hanno.Testing.Autofixture;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Hanno.Tests.CqrsInfrastructure
{
	public class EnumerableArrayResultAsyncQueryTests
	{
		[Theory, AutoMoqData]
		public void Sut_IsAsyncQuery(EnumerableArrayResultAsyncQuery<IAsyncQuery<object>, string> sut)
		{
			sut.Should().BeAssignableTo<IAsyncQuery<string[]>>();
		}

		[Theory, AutoMoqData]
		public void Sut_VerifyConstructor(GuardClauseAssertion assertion)
		{
			assertion.VerifyConstructors<EnumerableArrayResultAsyncQuery<IAsyncQuery<object>, string>>();
        }

		[Theory, AutoMoqData]
		public void Sut_Queries(
			[Frozen]IEnumerable<IAsyncQuery<object>> queries,
			EnumerableArrayResultAsyncQuery<IAsyncQuery<object>, string> sut)
		{
			sut.Queries.Should().Equal(queries);
		}
	}
}
