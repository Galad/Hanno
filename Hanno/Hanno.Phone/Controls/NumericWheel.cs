using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Hanno.Controls
{
	public class NumericWheel : Control
	{
		public const string NormalState = "Normal";
		public const string RotatingState = "Spining";

		public double MinValue
		{
			get { return (double)GetValue(MinValueProperty); }
			set { SetValue(MinValueProperty, value); }
		}

		// Using a DependencyProperty as the backing store for MinValue.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MinValueProperty =
			DependencyProperty.Register("MinValue", typeof(double), typeof(NumericWheel), new PropertyMetadata(0d));
		
		public double MaxValue
		{
			get { return (double)GetValue(MaxValueProperty); }
			set { SetValue(MaxValueProperty, value); }
		}

		// Using a DependencyProperty as the backing store for MaxValue.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MaxValueProperty =
			DependencyProperty.Register("MaxValue", typeof(double), typeof(NumericWheel), new PropertyMetadata(1d));
		
		public double Value
		{
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(double), typeof(NumericWheel), new PropertyMetadata(0d));

		public NumericWheel()
		{
			DefaultStyleKey = typeof (NumericWheel);
			ManipulationStarted += NumericWheel_ManipulationStarted;
			ManipulationDelta += NumericWheel_ManipulationDelta;
			ManipulationCompleted += NumericWheel_ManipulationCompleted;
			SizeChanged += NumericWheel_SizeChanged;
		}

		void NumericWheel_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			VisualStateManager.GoToState(this, RotatingState, true);
		}

		void NumericWheel_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
		{
			VisualStateManager.GoToState((Control)sender, RotatingState, true);
		}

		void NumericWheel_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
		{
			VisualStateManager.GoToState((Control)sender, NormalState, true);
		}

		void NumericWheel_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
		{
			var angle = Math.Atan(e.CumulativeManipulation.Translation.Y/e.CumulativeManipulation.Translation.X);
			Value += (MaxValue - MinValue)*angle/(Math.PI*2);
		}
	}
}
