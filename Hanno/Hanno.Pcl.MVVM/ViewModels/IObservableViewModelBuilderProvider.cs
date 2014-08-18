namespace Hanno.ViewModels
{
	public interface IObservableViewModelBuilderProvider
	{
		IObservableViewModelBuilder Get(string name);
		void AddVisitor(IObservableViewModelVisitor visitor);
		void CopyVisitors(IObservableViewModelBuilderProvider observableViewModelBuilderProvider);
	}
}