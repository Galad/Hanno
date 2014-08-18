using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Hanno.Services;

namespace Hanno.ViewModels
{
	public interface IObservableRegistrationService
	{
		IDisposable RegisterObservable<TObservable>(
			IObservable<TObservable> observable,
			Func<Exception, string> errorKey);
	}

	public class ObservableRegistrationService : IObservableRegistrationService
	{
		private readonly IAsyncMessageDialog _asyncMessageDialog;
		private readonly IStringResources _resources;
		private readonly Func<string, string> _errorTitleKeyProvider;
		private readonly Func<string, string> _errorContentKeyProvider;

		public ObservableRegistrationService(
			IAsyncMessageDialog asyncMessageDialog,
			IStringResources resources,
			Func<string, string> errorTitleKeyProvider,
			Func<string, string> errorContentKeyProvider)
		{
			if (asyncMessageDialog == null) throw new ArgumentNullException("asyncMessageDialog");
			if (resources == null) throw new ArgumentNullException("resources");
			if (errorTitleKeyProvider == null) throw new ArgumentNullException("errorTitleKeyProvider");
			if (errorContentKeyProvider == null) throw new ArgumentNullException("errorContentKeyProvider");
			_asyncMessageDialog = asyncMessageDialog;
			_resources = resources;
			_errorTitleKeyProvider = errorTitleKeyProvider;
			_errorContentKeyProvider = errorContentKeyProvider;
		}

		public IDisposable RegisterObservable<TObservable>(IObservable<TObservable> observable, Func<Exception, string> errorKey)
		{
			return observable
				.Catch((Exception ex) => Observable.FromAsync(async ct =>
				{
					var resourceKey = errorKey(ex);
					var title = _resources.GetResource(_errorTitleKeyProvider(resourceKey));
					var content = _resources.GetResource(_errorContentKeyProvider(resourceKey));
					await _asyncMessageDialog.Show(ct, title, content);
					return default(TObservable);
				}))
				.Subscribe(_ => { }, e => { }, () => { });
		}
	}

	public class DefaultObservableRegistrationService : ObservableRegistrationService
	{
		public DefaultObservableRegistrationService(
			IAsyncMessageDialog asyncMessageDialog,
			IStringResources resources) :
			base(asyncMessageDialog, resources, s => string.Format("{0}_Error_Title", s), s => string.Format("{0}_Error_Content", s))
		{
		}
	}
}