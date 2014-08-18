using System.Runtime.CompilerServices;

namespace Hanno.Commands
{
    public interface ICommandBuilderProvider
    {
        ICommandBuilder Get(string name);
	    void AddVisitor(IMvvmCommandVisitor visitor);
	    void CopyVisitors(ICommandBuilderProvider otherCommandBuilderProvider);
    }

	public static class CommandBuilderProviderExtensions
	{
		public static ICommandBuilder Get(this ICommandBuilderProvider commandBuilderProvider, [CallerMemberName] string name = null)
		{
			return commandBuilderProvider.Get(name);
		}
	}
}