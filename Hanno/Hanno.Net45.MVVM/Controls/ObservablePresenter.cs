using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Concurrency;
using Hanno.Rx;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Collections;
using Hanno.Rx.Extensions;
using Hanno.ViewModels;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
#endif

namespace Hanno.MVVM.Controls
{
	#if NETFX_CORE
	[ContentProperty(Name = "ValueTemplate")]
#else
	[ContentProperty("ValueTemplate")]
#endif
	public class ObservablePresenter : ContentControl
	{
		private readonly CompositeDisposable subscriptions = new CompositeDisposable();

		private static readonly IDictionary<ObservableViewModelStatus, DependencyProperty> TemplatesMapping;
#if !WINDOWS_PHONE
		private static readonly Dictionary<ObservableViewModelStatus, DependencyProperty> TemplateSelectorsMapping;
#endif

		static ObservablePresenter()
		{
			TemplatesMapping = new Dictionary<ObservableViewModelStatus, DependencyProperty>()
                {
                    {ObservableViewModelStatus.Initialized, InitializedTemplateProperty},
                    {ObservableViewModelStatus.Updating, UpdatingTemplateProperty},
                    {ObservableViewModelStatus.Value, ValueTemplateProperty},
                    {ObservableViewModelStatus.Empty, EmptyTemplateProperty},
                    {ObservableViewModelStatus.Error, ErrorTemplateProperty},
					{ObservableViewModelStatus.Timeout, ErrorTemplateProperty}
                };
#if !WINDOWS_PHONE
			TemplateSelectorsMapping = new Dictionary<ObservableViewModelStatus, DependencyProperty>()
                {
                    {ObservableViewModelStatus.Value, ValueTemplateSelectorProperty},
                    {ObservableViewModelStatus.Error, ErrorTemplateSelectorProperty}
                };
#endif
		}

		public ObservablePresenter()
		{
			DefaultStyleKey = typeof(ObservablePresenter);
		}
#if NETFX_CORE
		protected override void OnApplyTemplate()
#else
		public override void OnApplyTemplate()
#endif
		{
			base.OnApplyTemplate();
			this.ObserveLoaded()
				.Take(1)
				.Where(unit => ContentTemplate == null)
				.Subscribe(unit =>
					{
						Content = new ObservableModel()
							{
								Parent = DataContext
							};
						ContentTemplate = InitializedTemplate;
					});
		}

		~ObservablePresenter()
		{
			subscriptions.Dispose();
		}

		#region Properties
		public IObservable<object> Observable
		{
			get { return GetValue(ObservableProperty).Convert<object>(); }
			set { SetValue(ObservableProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ObservableProperty =
			DependencyProperty.Register("Observable", typeof(object), typeof(ObservablePresenter), new PropertyMetadata(null, (o, args) => ((ObservablePresenter)o).Subscribe()));


		public DataTemplate ValueTemplate
		{
			get { return (DataTemplate)GetValue(ValueTemplateProperty); }
			set { SetValue(ValueTemplateProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ValueTemplate.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ValueTemplateProperty =
			DependencyProperty.Register("ValueTemplate", typeof(DataTemplate), typeof(ObservablePresenter), new PropertyMetadata(null));



		public DataTemplate InitializedTemplate
		{
			get { return (DataTemplate)GetValue(InitializedTemplateProperty); }
			set { SetValue(InitializedTemplateProperty, value); }
		}

		// Using a DependencyProperty as the backing store for InitializedTemplate.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty InitializedTemplateProperty =
			DependencyProperty.Register("InitializedTemplate", typeof(DataTemplate), typeof(ObservablePresenter), new PropertyMetadata(null));



		public DataTemplate UpdatingTemplate
		{
			get { return (DataTemplate)GetValue(UpdatingTemplateProperty); }
			set { SetValue(UpdatingTemplateProperty, value); }
		}

		// Using a DependencyProperty as the backing store for UpdatingTemplate.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty UpdatingTemplateProperty =
			DependencyProperty.Register("UpdatingTemplate", typeof(DataTemplate), typeof(ObservablePresenter), new PropertyMetadata(null));



		public DataTemplate EmptyTemplate
		{
			get { return (DataTemplate)GetValue(EmptyTemplateProperty); }
			set { SetValue(EmptyTemplateProperty, value); }
		}

		// Using a DependencyProperty as the backing store for EmptyTemplate.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty EmptyTemplateProperty =
			DependencyProperty.Register("EmptyTemplate", typeof(DataTemplate), typeof(ObservablePresenter), new PropertyMetadata(null));



		public DataTemplate ErrorTemplate
		{
			get { return (DataTemplate)GetValue(ErrorTemplateProperty); }
			set { SetValue(ErrorTemplateProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ErrorTemplate.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ErrorTemplateProperty =
			DependencyProperty.Register("ErrorTemplate", typeof(DataTemplate), typeof(ObservablePresenter), new PropertyMetadata(null));



#if !WINDOWS_PHONE
		public DataTemplateSelector ValueTemplateSelector
		{
			get { return (DataTemplateSelector)GetValue(ValueTemplateSelectorProperty); }
			set { SetValue(ValueTemplateSelectorProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ValueTemplateSelector.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ValueTemplateSelectorProperty =
			DependencyProperty.Register("ValueTemplateSelector", typeof(DataTemplateSelector), typeof(ObservablePresenter), new PropertyMetadata(null));



		public DataTemplateSelector ErrorTemplateSelector
		{
			get { return (DataTemplateSelector)GetValue(ErrorTemplateSelectorProperty); }
			set { SetValue(ErrorTemplateSelectorProperty, value); }
		}


		// Using a DependencyProperty as the backing store for ErrorTemplateSelector.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ErrorTemplateSelectorProperty =
			DependencyProperty.Register("ErrorTemplateSelector", typeof(DataTemplateSelector), typeof(ObservablePresenter), new PropertyMetadata(null));

#endif

		#endregion

		private void Subscribe()
		{
			if (Observable != null)
			{
				Observable.ObserveOn(this.GetDispatcher())
						  .Select(GetValueAndTemplate)
						  .Subscribe(ApplyContent,
									 exception => ApplyContent(GetValueAndTemplate(new ObservableViewModelNotification()
										 {
											 Status = ObservableViewModelStatus.Error,
											 Value = exception
										 })),
									 () => subscriptions.Dispose())
						  .DisposeWith(subscriptions);
			}
		}

		private void ApplyContent(ValueAndTemplate valueAndTemplate)
		{
			Content = valueAndTemplate.Value;
			ContentTemplate = valueAndTemplate.Template;
		}

		private class ValueAndTemplate
		{
			public object Value;
			public DataTemplate Template;
		}

		private ValueAndTemplate GetValueAndTemplate(object o)
		{
			var rvvmValue = o as ObservableViewModelNotification;
			var valueAndTemplate = new ValueAndTemplate();
			var rvModel = new ObservableModel()
					{
						Parent = DataContext
					};
			valueAndTemplate.Value = rvModel;
			if (rvvmValue == null)
			{
				valueAndTemplate.Template = SelectValueTemplate(o);
				rvModel.Value = o;
			}
			else
			{
				var template = SelectTemplate(o, rvvmValue);
				valueAndTemplate.Template = template;
				rvModel.Value = rvvmValue.Value;
			}
			return valueAndTemplate;
		}

		private DataTemplate SelectTemplate(object o, ObservableViewModelNotification rvvmValue)
		{
#if WINDOWS_PHONE
			return (DataTemplate)GetValue(TemplatesMapping[rvvmValue.Status]);
#else
			DataTemplate template;
			if (TemplateSelectorsMapping.ContainsKey(rvvmValue.Status) && GetValue(TemplateSelectorsMapping[rvvmValue.Status]) != null)
			{
				template = ((DataTemplateSelector)GetValue(TemplateSelectorsMapping[rvvmValue.Status])).SelectTemplate(o, this);
			}
			else
			{
				template = (DataTemplate)GetValue(TemplatesMapping[rvvmValue.Status]);
			}
			return template;
#endif
		}

		private DataTemplate SelectValueTemplate(object o)
		{
#if WINDOWS_PHONE
			return ValueTemplate;
#else
			return ValueTemplateSelector == null ? ValueTemplate : ValueTemplateSelector.SelectTemplate(o, this);
#endif
		}
	}

	public class ObservableModel
	{
		public object Value { get; set; }
		public object Parent { get; set; }
	}
}