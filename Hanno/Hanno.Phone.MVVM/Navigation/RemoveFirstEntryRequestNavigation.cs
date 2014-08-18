using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Phone.Controls;

namespace Hanno.Navigation
{
	public class RemoveFirstEntryRequestNavigation : IRequestNavigation
	{
		private readonly IRequestNavigation _innerRequestNavigation;
		private readonly Uri _entryPointUri;
		private readonly PhoneApplicationFrame _applicationFrame;
		private readonly IScheduler _dispatcherScheduler;

		public RemoveFirstEntryRequestNavigation(
			IRequestNavigation innerRequestNavigation,
			IScheduler dispatcherScheduler,
			Uri entryPointUri,
			PhoneApplicationFrame applicationFrame)
		{
			if (innerRequestNavigation == null) throw new ArgumentNullException("innerRequestNavigation");
			if (dispatcherScheduler == null) throw new ArgumentNullException("dispatcherScheduler");
			if (entryPointUri == null) throw new ArgumentNullException("entryPointUri");
			if (applicationFrame == null) throw new ArgumentNullException("applicationFrame");
			_innerRequestNavigation = innerRequestNavigation;
			_entryPointUri = entryPointUri;
			_applicationFrame = applicationFrame;
			_dispatcherScheduler = dispatcherScheduler;
		}

		public async Task Navigate(CancellationToken ct, INavigationRequest request)
		{
			await _innerRequestNavigation.Navigate(ct, request);
			var firstEntry = _applicationFrame.BackStack.FirstOrDefault();
			if (firstEntry != null && firstEntry.Source.Equals(_entryPointUri))
			{
				await _dispatcherScheduler.Run(() => _applicationFrame.RemoveBackEntry());
			}
		}
	}
}