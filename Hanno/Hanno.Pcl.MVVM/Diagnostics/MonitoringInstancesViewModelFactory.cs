using System;
using System.Diagnostics;
using System.Linq;
using Hanno.ViewModels;

namespace Hanno.Diagnostics
{
	public class MonitoringInstancesViewModelFactory : IViewModelFactory, IDisposable
	{
		private readonly IViewModelFactory _innerViewModelFactory;
		private IInstanceMonitor _instanceMonitor;

		public MonitoringInstancesViewModelFactory(IViewModelFactory innerViewModelFactory, TimeSpan time)
		{
			if (innerViewModelFactory == null) throw new ArgumentNullException("innerViewModelFactory");
			_innerViewModelFactory = innerViewModelFactory;

			_instanceMonitor = InstanceTracker.MonitorByType<IViewModel>(time);
			_instanceMonitor.Update += _instanceMonitor_Update;
		}

		private void _instanceMonitor_Update(object sender, InstanceMonitoringEventArgs e)
		{
			if (e.Instances.Count == 0)
			{
				Debug.WriteLine("Info : No IViewModel instance remaining");
			}
			else
			{
				Debug.WriteLine("Info : {0} released IViewModel instances are alive :\n{1}", e.Instances.Count, string.Join("\n", e.Instances.Select(i => i.ToString())));
			}
		}

		public IViewModel ResolveViewModel(object request)
		{
			return _innerViewModelFactory.ResolveViewModel(request);
		}

		public void ReleaseViewModel(IViewModel viewModel)
		{
			_innerViewModelFactory.ReleaseViewModel(viewModel);
			InstanceTracker.Track(viewModel);
		}


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
				_instanceMonitor.Update -= _instanceMonitor_Update;
			}

			//Add disposition of unmanaged resources here

			_isDisposed = true;
		}

		~MonitoringInstancesViewModelFactory()
		{
			this.Dispose(false);
		}

		private bool _isDisposed;


	}
}