using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.ViewModels
{
    public class ObservableViewModelBuilderOptions<T> : IObservableViewModelBuilderOptions<T>
    {
        private readonly Action<IObservableViewModel> _saveViewModel;
        public Func<CancellationToken, Task<T>> Source { get; private set; }
        private Func<T, bool> _emptyPredicate = arg => false;
        private IObservable<Unit> _refreshOn = Observable.Empty<Unit>();
	    private TimeSpan _timeout = TimeSpan.Zero;

	    public ObservableViewModelBuilderOptions(Action<IObservableViewModel> saveViewModel, Func<CancellationToken, Task<T>> source)
        {
            if (saveViewModel == null) throw new ArgumentNullException("saveViewModel");
            if (source == null) throw new ArgumentNullException("source");
            _saveViewModel = saveViewModel;
            Source = source;
        }

        public IObservableViewModelBuilderOptions<T> EmptyPredicate(Func<T, bool> predicate)
        {
            _emptyPredicate = predicate;
            return this;
        }

        public IObservableViewModelBuilderOptions<T> RefreshOn<TRefresh>(IObservable<TRefresh> refreshTrigger)
        {
            _refreshOn = refreshTrigger.SelectUnit();
            return this;
        }

	    public IObservableViewModelBuilderOptions<T> Timeout(TimeSpan timeout)
	    {
		    _timeout = timeout;
		    return this;
	    }

	    public IObservableViewModel<T> ToViewModel()
	    {
		    var viewModel = new ObservableViewModel<T>(Source, _emptyPredicate, _refreshOn, _timeout, new CompositeDisposable());
            _saveViewModel(viewModel);
            return viewModel;
        }
    }
}