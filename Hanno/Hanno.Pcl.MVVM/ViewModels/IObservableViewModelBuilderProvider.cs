namespace Hanno.ViewModels
{
	public interface IObservableViewModelBuilderProvider
	{
		IObservableViewModelBuilder Get(string name);
	}
}