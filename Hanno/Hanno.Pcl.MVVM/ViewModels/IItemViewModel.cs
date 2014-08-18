using System.Reactive.Disposables;

namespace Hanno.ViewModels
{
	public interface IItemViewModel
	{
		CompositeDisposable Subscriptions { get; }
	}
}