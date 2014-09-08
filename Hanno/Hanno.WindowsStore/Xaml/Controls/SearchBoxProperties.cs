using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Windows.ApplicationModel.Search;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Hanno.Search;

namespace Hanno.WindowsStore.Xaml.Controls
{
	public class SearchBoxProperties
	{
		private static readonly ConditionalWeakTable<SearchBox, TypedEventHandler<SearchBox, SearchBoxSuggestionsRequestedEventArgs>> _eventHandlers;

		static SearchBoxProperties()
		{
			_eventHandlers = new ConditionalWeakTable<SearchBox, TypedEventHandler<SearchBox, SearchBoxSuggestionsRequestedEventArgs>>();
		}

		public static ISearchSuggestionProvider GetProvider(DependencyObject obj)
		{
			return (ISearchSuggestionProvider)obj.GetValue(ProviderProperty);
		}

		public static void SetProvider(DependencyObject obj, ISearchSuggestionProvider value)
		{
			obj.SetValue(ProviderProperty, value);
		}

		// Using a DependencyProperty as the backing store for Provider.  This enables animation, styling, binding, etc...

		public static readonly DependencyProperty ProviderProperty =
			DependencyProperty.RegisterAttached("Provider", typeof(ISearchSuggestionProvider), typeof(SearchBoxProperties), new PropertyMetadata(null, OnProviderChanged));

		private static void OnProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var searchBox = (SearchBox) d;
			CleanEventHandlers(searchBox);
			searchBox.SuggestionsRequested += searchBox_SuggestionsRequested;
		}

		private static void CleanEventHandlers(SearchBox searchBox)
		{
			TypedEventHandler<SearchBox, SearchBoxSuggestionsRequestedEventArgs> eventHandler;
			if (_eventHandlers.TryGetValue(searchBox, out eventHandler))
			{
				searchBox.SuggestionsRequested -= eventHandler;
			}
		}

		static void searchBox_SuggestionsRequested(SearchBox sender, SearchBoxSuggestionsRequestedEventArgs args)
		{
			var deferral = args.Request.GetDeferral();
			GetSuggestions(deferral, sender, args);
		}

		private static async void GetSuggestions(SearchSuggestionsRequestDeferral deferral, SearchBox sender, SearchBoxSuggestionsRequestedEventArgs args)
		{
			try
			{
				var provider = GetProvider(sender);
				var suggestions = await provider.GetSuggestions(CancellationToken.None, args.QueryText);
				var visitor = new AppendToSearchSuggestionCollectionVisitor(args.Request.SearchSuggestionCollection);
				foreach (var searchSuggestion in suggestions)
				{
					searchSuggestion.Accept(visitor);
				}
			}
			finally 
			{
				deferral.Complete();
			}
		}
	}

	

	
}