using System.Reactive.Concurrency;

namespace Hanno.Concurrency
{
	public interface IPriorityScheduler : IScheduler
	{
		IScheduler High { get; }
		IScheduler Normal { get; }
		IScheduler Low { get; }
	}
}