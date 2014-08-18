using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Hanno.Commands;
using Hanno.Navigation;

namespace Hanno.ViewModels
{
	public interface IViewModel
	{
		Task Load(CancellationToken cancellationToken);
		Task Unload(CancellationToken cancellationToken);
		void Initialize(INavigationRequest request);
		IViewModelServices Services { get; }
		CompositeDisposable ShortDisposables { get; }
		CompositeDisposable LongDisposables { get; }
		ICommandBuilderProvider CommandBuilderProvider { get; }
		IObservableViewModelBuilderProvider OvmBuilderProvider { get; }
	}
}