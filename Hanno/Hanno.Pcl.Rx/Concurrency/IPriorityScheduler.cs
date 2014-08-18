using System.Reactive.Concurrency;

namespace Hanno.Concurrency
{
	public enum SchedulerPriority
	{
		Lowest,
		Low,
		Normal,
		High
	}

	public interface IPriorityScheduler : IScheduler
	{
		IScheduler SchedulerFromPriority(SchedulerPriority priority);
	}
}