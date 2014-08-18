using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Hanno.Services;
using Hanno.Commands;
using Hanno.ViewModels;

namespace Hanno.Extensions
{
	public static class ViewModelExtensions
	{

		#region Observable properties

		public static IObservableProperty<T> GetObservableProperty<T, TViewModel>(this TViewModel viewModel, Func<IObservable<T>> source, [CallerMemberName] string name = null, bool longSubscribtion = false) where TViewModel : IViewModel, IBindable
		{
			if (viewModel == null) throw new ArgumentNullException("viewModel");
			if (source == null) throw new ArgumentNullException("source");
			if (name == null) throw new ArgumentNullException("name");
			var isNew = false;
			var property = viewModel.GetValue(() =>
			{
				var p = new ObservableProperty<T>(viewModel.Services.Schedulers);
				isNew = true;
				return p;
			}, name);
			if (!isNew)
			{
				return property;
			}
			var observable = source();
			var disposables = longSubscribtion ? viewModel.LongDisposables : viewModel.ShortDisposables;
			observable.Subscribe(property).DisposeWith(disposables);
			return property;
		}

		public static IObservableProperty<T> GetObservableProperty<T, TViewModel>(this TViewModel viewModel, [CallerMemberName] string name = null)
			where TViewModel : IViewModel, IBindable
		{
			if (viewModel == null) throw new ArgumentNullException("viewModel");
			if (name == null) throw new ArgumentNullException("name");
			var property = viewModel.GetValue(() => new ObservableProperty<T>(viewModel.Services.Schedulers), name);
			return property;
		}
		#endregion

		public static ICommandBuilder CommandBuilder<TViewModel>(this TViewModel viewModel, [CallerMemberName] string name = null) where TViewModel : IViewModel
		{
			return viewModel.CommandBuilderProvider
							.Get(name);
		}

		public static IObservableViewModelBuilder RvvmBuilder<TViewModel>(this TViewModel viewModel, [CallerMemberName] string name = null) where TViewModel : IViewModel
		{
			return viewModel.OvmBuilderProvider.Get(name);
		}

		public static IObservable<T> ObservableFromPool<T, TViewModel>(
			this TViewModel viewModel,
			Func<IObservable<T>> observableFactory,
			IScheduler scheduler,
			[CallerMemberName]string name = null,
			bool longSubscription = false) where TViewModel : IViewModel, IBindable
		{
			var observable = viewModel.GetValue(() =>
			{
				var subscriptions = longSubscription ? viewModel.LongDisposables : viewModel.ShortDisposables;
				var obs = observableFactory();
				var subject = new ReplaySubject<T>(1, scheduler);
				subject.DisposeWith(subscriptions);
				obs.Subscribe(subject).DisposeWith(subscriptions);
				return subject.AsObservable();
			}, name);
			return observable;
		}
	}

}
