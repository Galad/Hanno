using System.Collections.Generic;

namespace Hanno.Mobile.Scheduler
{
	public interface IAlarmService
	{
		void Add(AlarmAction alarm);
		void Remove(string name);
		Scheduler.AlarmAction Find(string name);
		IEnumerable<AlarmAction> GetAlarms();
	}
}