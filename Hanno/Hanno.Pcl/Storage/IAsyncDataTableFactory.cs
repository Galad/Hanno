namespace Hanno.Storage
{
	public interface IAsyncDataTableFactory
	{
		IAsyncDataTable<TKey, TValue> GetAsyncTable<TKey, TValue>();
		void Release<TKey, TValue>(IAsyncDataTable<TKey, TValue> table);
	}
}