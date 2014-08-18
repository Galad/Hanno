using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Hanno.Services
{
	public class AsyncMessageDialog : IAsyncMessageDialog, IDisposable
	{
		private readonly IScheduler _dispatcher;
		private readonly SemaphoreSlim _semaphore;

		public AsyncMessageDialog(IScheduler dispatcher)
		{
			if (dispatcher == null) throw new ArgumentNullException("dispatcher");
			_dispatcher = dispatcher;
			_semaphore = new SemaphoreSlim(1);
		}

		public async Task Show(CancellationToken ct, string title, string content)
		{
			var messageDialog = new MessageDialog(content, title);
			await _semaphore.WaitAsync(ct);
			try
			{
				await _dispatcher.Run(async () => await messageDialog.ShowAsync());
			}
			finally
			{
				_semaphore.Release();
			}
		}


		#region Dispose
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Dispose(bool disposing)
		{
			if (_isDisposed)
			{
				return;
			}

			if (disposing)
			{
				_semaphore.Dispose();
			}

			//Add disposition of unmanaged resources here

			_isDisposed = true;
		}

		~AsyncMessageDialog()
		{
			this.Dispose(false);
		}

		private bool _isDisposed; 
		#endregion
		
	}
}