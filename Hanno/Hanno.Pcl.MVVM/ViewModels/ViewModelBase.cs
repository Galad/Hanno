using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Hanno.Commands;
using Hanno.Navigation;
using Hanno.Validation;

namespace Hanno.ViewModels
{
	public abstract class ViewModelBase : IViewModel, IDisposable, INotifyValidationError, IValidable, INotifyPropertyChanged, IBindable
	{
		private IObservableViewModelBuilderProvider _ovmBuilderProvider;
		private ICommandBuilderProvider _commandBuilderProvider;
		private List<IViewModel> _children;
		private IViewModelServices _services;
		private Validable _validable;
		public CompositeDisposable ShortDisposables { get; private set; }
		public CompositeDisposable LongDisposables { get; private set; }

		private Validable Validable
		{
			get
			{
				//ensure services are  created 
				return _validable ?? (_validable = new Validable(Services.Validator, Services.Schedulers));
			}
		}

		protected ViewModelBase(IViewModelServices services)
		{
			if (services == null) throw new ArgumentNullException("services");
			_services = services;
			_children = new List<IViewModel>();
			ViewModels = _children;
			ShortDisposables = new CompositeDisposable();
			LongDisposables = new CompositeDisposable();
		}

		protected virtual void DefineRules()
		{
		}

		public void Initialize(INavigationRequest navigationRequest)
		{
			if (navigationRequest == null) throw new ArgumentNullException("navigationRequest");
			NavigationRequest = navigationRequest;
			foreach (var viewModel in _children)
			{
				viewModel.Initialize(navigationRequest);
			}
			DefineRules();
			OnInitialized();
		}

		protected virtual void OnInitialized()
		{
		}

		public async Task Load(CancellationToken ct)
		{
			await Task.WhenAll(ViewModels.Select(vm => vm.Load(ct)));
			await OnLoaded(ct);
		}

		public async Task Unload(CancellationToken ct)
		{
			ShortDisposables.Dispose();
			await Task.WhenAll(ViewModels.Select(vm => vm.Unload(ct)));
			await OnUnloaded(ct);
		}

		protected virtual Task OnLoaded(CancellationToken ct)
		{
			return Task.FromResult(true);
		}

		protected virtual Task OnUnloaded(CancellationToken ct)
		{
			return Task.FromResult(true);
		}

		public IViewModelServices Services
		{
			get
			{
				return _services;
			}
		}

		public virtual IObservableViewModelBuilderProvider OvmBuilderProvider
		{
			get
			{
				if (_ovmBuilderProvider == null)
				{
					_ovmBuilderProvider = new ObservableViewModelBuilderProvider(
						() => this.Services.Schedulers,
						(action, s1, s2) => new ObservableViewModelBuilder(action, s1, s2, Services.Schedulers));
					((ObservableViewModelBuilderProvider) _ovmBuilderProvider).DisposeWith(LongDisposables);
				}
				return _ovmBuilderProvider;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				_ovmBuilderProvider = value;
			}
		}

		public virtual ICommandBuilderProvider CommandBuilderProvider
		{
			get
			{
				if (_commandBuilderProvider == null)
				{
					_commandBuilderProvider = new CommandBuilderProvider(Services.Schedulers);
					((CommandBuilderProvider)_commandBuilderProvider).DisposeWith(LongDisposables);
				}
				return _commandBuilderProvider;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				var disposable = value as IDisposable;
				if (disposable != null)
				{
					disposable.DisposeWith(LongDisposables);
				}
				_commandBuilderProvider = value;
			}
		}

		protected TViewModel RegisterChild<TViewModel>(TViewModel child) where TViewModel : IViewModel, IDisposable
		{
			_children.Add(child);
			LongDisposables.Add(child);
			CommandBuilderProvider.CopyVisitors(child.CommandBuilderProvider);
			OvmBuilderProvider.CopyVisitors(child.OvmBuilderProvider);
			if (NavigationRequest != null)
			{
				child.Initialize(NavigationRequest);
			}
			return child;
		}

		public IEnumerable<IViewModel> ViewModels { get; private set; }

		public virtual void Dispose()
		{
			LongDisposables.Dispose();
			ShortDisposables.Dispose();
		}

		#region Validable

		public event EventHandler<ValidationErrorEventArgs> ErrorsChanged
		{
			add { Validable.ErrorsChanged += value; }
			remove { Validable.ErrorsChanged -= value; }
		}

		public event EventHandler<ValidationErrorEventArgs> BeginValidation
		{
			add { Validable.BeginValidation += value; }
			remove { Validable.BeginValidation -= value; }
		}

		public IValidationError GetError(string propertyName)
		{
			return Validable.GetError(propertyName);
		}

		public Task<IValidationError> GetErrorAsync(string propertyName)
		{
			return Validable.GetErrorAsync(propertyName);
		}

		public Task<bool> HasErrorsAsync()
		{
			return Validable.HasErrorsAsync();
		}

		public Task<bool> HasErrorAsync(string propertyName)
		{
			return Validable.HasErrorAsync(propertyName);
		}

		public IValidator Validator
		{
			get { return Validable.Validator; }
		}
		#endregion

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { Validable.PropertyChanged += value; }
			remove { Validable.PropertyChanged -= value; }
		}

		#region Validable
		public T GetValue<T>(Func<T> factory = null, [CallerMemberName]string key = null)
		{
			return Validable.GetValue(factory, key);
		}

		public void Invalidate(string property)
		{
			Validable.Invalidate(property);
		}

		public bool SetValue<T>(T value, [CallerMemberName]string key = null)
		{
			return Validable.SetValue(value, key);
		}

		public void NotifyPropertyChanged([CallerMemberName]string propertyName = null)
		{
			Validable.NotifyPropertyChanged(propertyName);
		}

		public T GetValue<T>(Expression<Func<T>> key, Func<T> factory = null)
		{
			return Validable.GetValue(key, factory);
		}
		#endregion

		public INavigationRequest NavigationRequest { get; private set; }
	}
}