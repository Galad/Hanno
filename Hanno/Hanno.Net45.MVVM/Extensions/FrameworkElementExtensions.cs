using System.Reactive.Concurrency;

namespace System.Windows
{
	public static class FrameworkElementExtensions
	{
		public static IScheduler GetDispatcher(this FrameworkElement frameworkElement)
		{
			return new DispatcherScheduler(frameworkElement.Dispatcher);
		}
	}
}