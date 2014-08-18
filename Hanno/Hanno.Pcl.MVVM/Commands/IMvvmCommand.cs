namespace Hanno.Commands
{
	public interface IMvvmCommand
	{
		void Accept(IMvvmCommandVisitor visitor);
	}
}