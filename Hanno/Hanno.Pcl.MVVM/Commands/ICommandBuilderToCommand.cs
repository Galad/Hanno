using System.Windows.Input;

namespace Hanno.Commands
{
    public interface ICommandBuilderToCommand
    {
        ICommand ToCommand();
    }
}