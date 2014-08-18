using System;
using System.Threading.Tasks;

namespace Hanno.ViewModels
{
    public interface IObservableViewModel : IObservable<ObservableViewModelNotification>
    {
        Task RefreshAsync();
	    void Refresh();
	    void Accept(IObservableViewModelVisitor visitor);
    }

	public interface IObservableViewModel<out T> : IObservableViewModel
	{
		T CurrentValue { get; }
	}
}