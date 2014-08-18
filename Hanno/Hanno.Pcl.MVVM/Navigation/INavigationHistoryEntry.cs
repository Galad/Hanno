namespace Hanno.Navigation
{
	public interface INavigationHistoryEntry
	{
		INavigationRequest Request { get; }
		/// <summary>
		/// Indicated if the page is actually created 
		/// </summary>
		bool IsAlive { get; }
	}
}