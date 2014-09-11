using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Search;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Hanno.Search;

namespace Hanno.Xaml.Controls
{
	public class SearchBoxProperties
	{
		private static readonly ConditionalWeakTable<SearchBox, TypedEventHandler<SearchBox, SearchBoxSuggestionsRequestedEventArgs>> _suggestionRequestedEventHandlers;
		private static readonly ConditionalWeakTable<SearchBox, TypedEventHandler<SearchBox, SearchBoxResultSuggestionChosenEventArgs>> _resultSuggestionChosenEventHandlers;
		private static readonly ConditionalWeakTable<SearchBox, TypedEventHandler<FrameworkElement, DataContextChangedEventArgs>> _dataContextChangedEventHandlers;

		static SearchBoxProperties()
		{
			_suggestionRequestedEventHandlers = new ConditionalWeakTable<SearchBox, TypedEventHandler<SearchBox, SearchBoxSuggestionsRequestedEventArgs>>();
			_resultSuggestionChosenEventHandlers = new ConditionalWeakTable<SearchBox, TypedEventHandler<SearchBox, SearchBoxResultSuggestionChosenEventArgs>>();
			_dataContextChangedEventHandlers = new ConditionalWeakTable<SearchBox, TypedEventHandler<FrameworkElement, DataContextChangedEventArgs>>();
		}

		public static ISearchSuggestionProvider GetProvider(DependencyObject obj)
		{
			return (ISearchSuggestionProvider) obj.GetValue(ProviderProperty);
		}

		public static void SetProvider(DependencyObject obj, ISearchSuggestionProvider value)
		{
			obj.SetValue(ProviderProperty, value);
		}

		// Using a DependencyProperty as the backing store for Provider.  This enables animation, styling, binding, etc...

		public static readonly DependencyProperty ProviderProperty =
			DependencyProperty.RegisterAttached("Provider", typeof (ISearchSuggestionProvider), typeof (SearchBoxProperties), new PropertyMetadata(null, OnProviderChanged));

		private static void OnProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var searchBox = (SearchBox) d;
			CleanSuggestionRequestedEventHandlers(searchBox);
			var eventHandler = new TypedEventHandler<SearchBox, SearchBoxSuggestionsRequestedEventArgs>(searchBox_SuggestionsRequested);
			_suggestionRequestedEventHandlers.Add(searchBox, eventHandler);
			searchBox.SuggestionsRequested += eventHandler;
		}

		private static void CleanSuggestionRequestedEventHandlers(SearchBox searchBox)
		{
			TypedEventHandler<SearchBox, SearchBoxSuggestionsRequestedEventArgs> eventHandler;
			if (_suggestionRequestedEventHandlers.TryGetValue(searchBox, out eventHandler))
			{
				searchBox.SuggestionsRequested -= eventHandler;
				_suggestionRequestedEventHandlers.Remove(searchBox);
			}
		}

		private static void searchBox_SuggestionsRequested(SearchBox sender, SearchBoxSuggestionsRequestedEventArgs args)
		{
			var deferral = args.Request.GetDeferral();
			GetSuggestions(deferral, sender, args);
		}

		private static async void GetSuggestions(SearchSuggestionsRequestDeferral deferral, SearchBox sender, SearchBoxSuggestionsRequestedEventArgs args)
		{
			try
			{
				var provider = GetProvider(sender);
				var query = args.QueryText;
				var suggestions = await Task.Run(() => provider.GetSuggestions(CancellationToken.None, query));
				var visitor = new AppendToSearchSuggestionCollectionVisitor(args.Request.SearchSuggestionCollection);
				foreach (var searchSuggestion in suggestions)
				{
					searchSuggestion.Accept(visitor);
				}
				deferral.Complete();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}



		public static ResultSuggestionCommands GetResultSuggestionChosenCommand(DependencyObject obj)
		{
			return (ResultSuggestionCommands)obj.GetValue(ResultSuggestionChosenCommandProperty);
		}

		public static void SetResultSuggestionChosenCommand(DependencyObject obj, ResultSuggestionCommands value)
		{
			obj.SetValue(ResultSuggestionChosenCommandProperty, value);
		}

// Using a DependencyProperty as the backing store for SuggestionCommand.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ResultSuggestionChosenCommandProperty =
			DependencyProperty.RegisterAttached("ResultSuggestionChosenCommand", typeof(ResultSuggestionCommands), typeof(SearchBoxProperties), new PropertyMetadata(null, OnResultSuggestionChosenCommandChanged));

		private static void OnResultSuggestionChosenCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var searchBox = (SearchBox) d;
			var command = (ResultSuggestionCommands) e.NewValue;
			CleanResultSuggestionChosenEventHandlers(searchBox);
			CleanDataContextChangedEventHandlers(searchBox);
			command.DataContext = searchBox.DataContext;
			var dataContextEventHandler = new TypedEventHandler<FrameworkElement, DataContextChangedEventArgs>((sender, args) => command.DataContext = args.NewValue);
			_dataContextChangedEventHandlers.Add(searchBox, dataContextEventHandler);
			searchBox.DataContextChanged += dataContextEventHandler;
			var eventHandler = new TypedEventHandler<SearchBox, SearchBoxResultSuggestionChosenEventArgs>(searchBox_ResultSuggestionChosen);
			_resultSuggestionChosenEventHandlers.Add(searchBox, eventHandler);
			searchBox.ResultSuggestionChosen += eventHandler;
		}

		private static void CleanDataContextChangedEventHandlers(SearchBox searchBox)
		{
			TypedEventHandler<FrameworkElement, DataContextChangedEventArgs> eventHandler;
			if (_dataContextChangedEventHandlers.TryGetValue(searchBox, out eventHandler))
			{
				searchBox.DataContextChanged -= eventHandler;
				_dataContextChangedEventHandlers.Remove(searchBox);
			}
		}

		static void searchBox_ResultSuggestionChosen(SearchBox sender, SearchBoxResultSuggestionChosenEventArgs args)
		{
			var command = GetResultSuggestionChosenCommand(sender);
			if (command != null && command.CanExecute(args))
			{
				command.Execute(args);
			}
		}

		private static void CleanResultSuggestionChosenEventHandlers(SearchBox searchBox)
		{
			TypedEventHandler<SearchBox, SearchBoxResultSuggestionChosenEventArgs> eventHandler;
			if (_resultSuggestionChosenEventHandlers.TryGetValue(searchBox, out eventHandler))
			{
				searchBox.ResultSuggestionChosen -= eventHandler;
				_resultSuggestionChosenEventHandlers.Remove(searchBox);
			}
		}
	}


}