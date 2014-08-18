using System;
using System.Linq;
using Ploeh.AutoFixture.Idioms;

namespace Ploeh.AutoFixture.Idioms
{
	internal class FilteredParametersBehaviorExpectation
	{
		public string[] ParameterNames { get; private set; }

		public FilteredParametersBehaviorExpectation(string[] parameterNames)
		{
			if (parameterNames == null) throw new ArgumentNullException("parameterNames");
			ParameterNames = parameterNames;
		}

		public bool ContainsParameters(IGuardClauseCommand command)
		{
			if (command == null) throw new ArgumentNullException("command");

			var expectedCommand = command as ReflectionExceptionUnwrappingCommand;
			MethodInvokeCommand methodCommand = null;
			if (expectedCommand == null)
			{
				methodCommand = command as MethodInvokeCommand;
			}
			else
			{
				methodCommand = expectedCommand.Command as MethodInvokeCommand;
			}

			if (methodCommand != null)
			{
				return ParameterNames.Any(p => p == methodCommand.ParameterInfo.Name);
			}
			return false;
		}
	}
}