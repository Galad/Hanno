using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Hanno.Annotations;

namespace Hanno.Xaml
{
	public interface IViewState<out T>
	{
		T CurrentState { get; }
	}

	public class ViewState<T> : IViewState<T>, INotifyPropertyChanged
	{
		private class StateDefinition
		{
			public double Offset;
			public T State;
		}
		private T _currentState;
		private T _portraitState;
		private T _defaultState;
		private List<StateDefinition> _landscapeStates;
		private bool _usePortrait;

		public ViewState(T defaultState)
		{
			Window.Current.CoreWindow.SizeChanged += Current_SizeChanged;
			//DisplayProperties.OrientationChanged += DisplayProperties_OrientationChanged;
		}

		//void DisplayProperties_OrientationChanged(object sender)
		//{
		//	throw new NotImplementedException();
		//}

		void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
		{
			SetCurrentState(e.Size);
		}

		private void SetCurrentState(Size size)
		{
			
		}

		public ViewState<T> Portrait(T portraitState)
		{
			_portraitState = portraitState;
			_usePortrait = true;
			return this;
		} 

		public ViewState<T> AddState(T state, double offset)

		public void Initialize(
			T defaultState,
			T portraitState,
			params Tuple<T, double>[] landscapeStates)
		{
			_defaultState = defaultState;
			_portraitState = portraitState;
			_landscapeStates = landscapeStates.OrderBy(t => t.Item1).Select(t => new StateDefinition() { Offset = t.Item2, State = t.Item1 }).ToList();
			SetCurrentState(new Size(Window.Current.Bounds.Width, Window.Current.Bounds.Height));
		}

		public T CurrentState
		{
			get { return _currentState; }
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}