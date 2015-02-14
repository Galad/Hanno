using System;

namespace Hanno.CqrsInfrastructure
{
	public class CommandEventArgs
	{
		public CommandEventArgs(object command, object value)
		{
			if (command == null) throw new ArgumentNullException("command");
			if (value == null) throw new ArgumentNullException("value");
			Command = command;
			Value = value;
		}
		public object Command { get; private set; }
		public object Value { get; private set; }
	}
}