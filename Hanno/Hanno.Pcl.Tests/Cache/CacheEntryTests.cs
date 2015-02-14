using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hanno.Cache;
using Hanno.Testing.Autofixture;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Idioms;
using Xunit.Extensions;

namespace Hanno.Tests.Cache
{
	public class CacheEntryTests
	{
		[Theory, AutoMoqData]
		public void Sut_VerifyGuardClauses(Fixture fixture)
		{
			var assertion = new GuardClauseAssertion(
				fixture,
				new SkipSpecifiedParametersBehaviorExpectation(
				new CompositeBehaviorExpectation(
					new EmptyGuidBehaviorExpectation(),
					new NullReferenceBehaviorExpectation()),
				"value")
				);
			assertion.VerifyConstructors<CacheEntry<string>>();
		}
	}
}
