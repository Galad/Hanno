using System.Collections.Generic;
using Microsoft.Phone.Scheduler;

namespace Hanno.Scheduler
{
	public interface ISchedulerActionService
	{
		void Add(ScheduledAction scheduledAction);
		ScheduledAction Find(string name);
		void Remove(string name);
		void Replace(ScheduledAction scheduledAction);
		IEnumerable<T> GetActions<T>() where T : ScheduledAction;
	}

	public class SchedulerActionService : ISchedulerActionService
	{
		public void Add(ScheduledAction scheduledAction)
		{
			ScheduledActionService.Add(scheduledAction);
		}

		public ScheduledAction Find(string name)
		{
			return ScheduledActionService.Find(name);
		}

		public void Remove(string name)
		{
			ScheduledActionService.Remove(name);
		}

		public void Replace(ScheduledAction scheduledAction)
		{
			ScheduledActionService.Replace(scheduledAction);
		}

		public IEnumerable<T> GetActions<T>() where T : ScheduledAction
		{
			return ScheduledActionService.GetActions<T>();
		}
	}
}