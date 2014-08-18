using System;

namespace Hanno.Mobile.Scheduler
{
	public class AlarmAction : ScheduledAction
	{
		public string Content { get; set; }
		public string Title { get; set; }
		public RecurrenceInterval RecurrencyType { get; set; }
	}

	public abstract class ScheduledAction
	{
		public virtual DateTimeOffset BeginTime { get; set; }
		public virtual DateTimeOffset ExpirationTime { get; set; }
		public bool IsEnabled { get; set; }
		public bool IsScheduled { get; set; }
		public string Name { get; set; }
	}

	public enum RecurrenceInterval
	{
		None = 0,
		Daily = 1,
		Weekly = 2,
		Monthly = 3,
		EndOfMonth = 4,
		Yearly = 5,
	}
}