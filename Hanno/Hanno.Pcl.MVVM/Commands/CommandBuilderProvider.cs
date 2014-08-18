using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Windows.Input;

namespace Hanno.Commands
{
	public class CommandBuilderProvider : ICommandBuilderProvider, IDisposable
	{
		private readonly ISchedulers _schedulers;
		private readonly Func<Action<ICommand>, ISchedulers, string, ICommandBuilder> _builderFactory;
		private readonly IDictionary<string, ICommand> _existingCommands = new Dictionary<string, ICommand>();
		private readonly CompositeDisposable _disposable = new CompositeDisposable();
		private readonly List<IMvvmCommandVisitor> _visitors;

		public CommandBuilderProvider(
			ISchedulers schedulers,
			Func<Action<ICommand>, ISchedulers, string, ICommandBuilder> builderFactory = null)
		{
			if (schedulers == null) throw new ArgumentNullException("schedulers");
			if (builderFactory == null)
			{
				builderFactory = (action, schedulers1, name) => new CommandBuilder(action, schedulers1, name);
			}
			_schedulers = schedulers;
			_builderFactory = builderFactory;
			_visitors = new List<IMvvmCommandVisitor>();
		}

		public ICommandBuilder Get(string name)
		{
			if (name == null) throw new ArgumentNullException("name");
			ICommandBuilder builder;
			if (_existingCommands.ContainsKey(name))
			{
				builder = new ExistingCommandBuilder(_existingCommands[name]);
			}
			else
			{
				builder = _builderFactory(command =>
				{
					var disposable = command as IDisposable;
					if (disposable != null) disposable.DisposeWith(_disposable);
					_existingCommands[name] = command;
					var mvvmCommand = command as IMvvmCommand;
					if (mvvmCommand != null)
					{
						foreach (var mvvmCommandVisitor in _visitors)
						{
							mvvmCommand.Accept(mvvmCommandVisitor);
						}
					}
				}, 
				_schedulers,
				name);
			}
			return builder;
		}

		public void AddVisitor(IMvvmCommandVisitor visitor)
		{
			_visitors.Add(visitor);
		}

		public void CopyVisitors(ICommandBuilderProvider otherCommandBuilderProvider)
		{
			foreach (var mvvmCommandVisitor in _visitors)
			{
				otherCommandBuilderProvider.AddVisitor(mvvmCommandVisitor);
			}
		}

		public void Dispose()
		{
			_disposable.Dispose();
			_existingCommands.Clear();
		}
	}
}