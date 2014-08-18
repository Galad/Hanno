using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hanno.ViewModels;

namespace Hanno.Navigation
{
	public class NavigationHistory : INavigationHistory
	{
		private List<INavigationHistoryEntry> _entries;
		private Dictionary<INavigationRequest, IViewModel> _viewModels;
		private readonly Func<CancellationToken, INavigationHistoryEntry, Task> _remove;
		private readonly Func<CancellationToken, Task> _clear;

		public NavigationHistory(
			Func<CancellationToken, INavigationHistoryEntry, Task> remove,
			Func<CancellationToken, Task> clear)
		{
			if (remove == null) throw new ArgumentNullException("remove");
			if (clear == null) throw new ArgumentNullException("clear");
			_remove = remove;
			_clear = clear;
			_entries = new List<INavigationHistoryEntry>();
			_viewModels = new Dictionary<INavigationRequest, IViewModel>();
		}

		public IReadOnlyList<INavigationHistoryEntry> Entries { get { return _entries; } }

		public IEnumerable<AliveNavigationHistoryEntry> AliveEntries
		{
			get { return _entries.OfType<AliveNavigationHistoryEntry>(); }
		}

		public async Task Remove(CancellationToken ct, INavigationHistoryEntry entry)
		{
			await _remove(ct, entry);
			_entries.Remove(entry);
			_viewModels.Remove(entry.Request);
		}

		public void InsertRequest(int index, INavigationRequest request)
		{
			if (request == null) throw new ArgumentNullException("request");
			if (index > _entries.Count - 1)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			_entries.Insert(index, new NotAliveNavigationHistoryEntry(request));
		}

		public void Append(AliveNavigationHistoryEntry entry)
		{
			_entries.Add(entry);
			_viewModels[entry.Request] = entry.ViewModel;
		}

		public async Task Clear(CancellationToken ct)
		{
			await _clear(ct);
		}


	}

	internal class NotAliveNavigationHistoryEntry : INavigationHistoryEntry
	{
		public NotAliveNavigationHistoryEntry(INavigationRequest request)
		{
			Request = request;
		}

		public INavigationRequest Request { get; private set; }
		public bool IsAlive
		{
			get { return false; }
		}
	}

	public class AliveNavigationHistoryEntry : INavigationHistoryEntry
	{
		public AliveNavigationHistoryEntry(INavigationRequest request, IViewModel viewModel)
		{
			ViewModel = viewModel;
			Request = request;
		}

		public INavigationRequest Request { get; private set; }
		public bool IsAlive { get { return true; } }
		public IViewModel ViewModel { get; private set; }
	}
}