using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Navigation
{
	public interface INavigationService
	{
		Task NavigateBack(CancellationToken ct);
		bool CanNavigateBack { get; }
		INavigationHistory History { get; }

		IObservable<INavigationRequest> Navigating { get; }
		IObservable<INavigationRequest> Navigated { get; }
	}
}