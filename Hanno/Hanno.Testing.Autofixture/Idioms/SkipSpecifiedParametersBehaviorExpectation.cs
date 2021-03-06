using System;
using Ploeh.AutoFixture.Idioms;

namespace Ploeh.AutoFixture.Idioms
{
	public class SkipSpecifiedParametersBehaviorExpectation : IBehaviorExpectation
	{
		public IBehaviorExpectation InnerBehaviorExpectation { get; private set; }
		private readonly FilteredParametersBehaviorExpectation _filter;

		public SkipSpecifiedParametersBehaviorExpectation(
			IBehaviorExpectation innerBehaviorExpectation,
			params string[] parameterNames)
		{
			_filter = new FilteredParametersBehaviorExpectation(parameterNames);
			InnerBehaviorExpectation = innerBehaviorExpectation;
		}

		public void Verify(IGuardClauseCommand command)
		{
			if (command == null) throw new ArgumentNullException("command");
			if (_filter.ContainsParameters(command))
			{
				return;
			}
			InnerBehaviorExpectation.Verify(command);
		}
	}
}