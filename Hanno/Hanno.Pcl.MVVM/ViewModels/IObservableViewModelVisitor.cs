namespace Hanno.ViewModels
{
	public interface IObservableViewModelVisitor
	{
		void Visit<T>(ObservableViewModel<T> ovm);
	}
}