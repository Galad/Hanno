//Imported from  WinRTXamlToolkit
//http://winrtxamltoolkit.codeplex.com/
using System;
#if WINRT
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media.Animation;
using AnimationPropertyPath = System.String;
#elif WINDOWS_PHONE
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using AnimationPropertyPath = System.Windows.PropertyPath;
#endif
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Reactive;

namespace Hanno.Controls
{
	/// <summary>
	/// A path that represents a pie slice with a given
	/// Radius,
	/// StartAngle and
	/// EndAngle.
	/// </summary>
	public class PieSlice : Path
	{
		private bool _isUpdating;

		#region StartAngle
		/// <summary>
		/// The start angle property
		/// </summary>
		public static readonly DependencyProperty StartAngleProperty =
			DependencyProperty.Register(
				"StartAngle",
				typeof(double),
				typeof(PieSlice),
				new PropertyMetadata(
					0d,
					OnStartAngleChanged));

		/// <summary>
		/// Gets or sets the start angle.
		/// </summary>
		/// <value>
		/// The start angle.
		/// </value>
		public double StartAngle
		{
			get { return (double)GetValue(StartAngleProperty); }
			set { SetValue(StartAngleProperty, value); }
		}

		private static void OnStartAngleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var target = (PieSlice)sender;
			var oldStartAngle = (double)e.OldValue;
			var newStartAngle = (double)e.NewValue;
			target.OnStartAngleChanged(oldStartAngle, newStartAngle);
		}

		private void OnStartAngleChanged(double oldStartAngle, double newStartAngle)
		{
			UpdatePath(newStartAngle, EndAngle);
		}
		#endregion

		#region EndAngle
		/// <summary>
		/// The end angle property.
		/// </summary>
		public static readonly DependencyProperty EndAngleProperty =
			DependencyProperty.Register(
				"EndAngle",
				typeof(double),
				typeof(PieSlice),
				new PropertyMetadata(
					0d,
					OnEndAngleChanged));

		/// <summary>
		/// Gets or sets the end angle.
		/// </summary>
		/// <value>
		/// The end angle.
		/// </value>
		public double EndAngle
		{
			get { return (double)GetValue(EndAngleProperty); }
			set { SetValue(EndAngleProperty, value); }
		}

		private static void OnEndAngleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var target = (PieSlice)sender;
			var oldEndAngle = target.EndAngleInternal;
			var newEndAngle = (double)e.NewValue;
			target.OnEndAngleChanged(oldEndAngle, newEndAngle);
		}

		private IDisposable animationDisposable = null;
		private void OnEndAngleChanged(double oldEndAngle, double newEndAngle)
		{
			if (animationDisposable != null)
			{
				animationDisposable.Dispose();
			}
			var storyBoard = new Storyboard();
			var animation = new DoubleAnimation()
			{
				From = oldEndAngle,
				To = newEndAngle,
				Duration = new Duration(AnimationDuration),
#if WINRT
				EnableDependentAnimation = true,				
#endif
			};
			Storyboard.SetTargetProperty(storyBoard, GetAnimationTargetPropertyPath("EndAngleInternal"));
			Storyboard.SetTarget(storyBoard, this);
			storyBoard.Children.Add(animation);
			animationDisposable = Disposable.Create(() => storyBoard.Stop());
			//storyBoard.Begin();
			EndAngleInternal = newEndAngle;
		}

		private static AnimationPropertyPath GetAnimationTargetPropertyPath(string path)
		{
#if WINRT
			return path;
#elif WINDOWS_PHONE
			return new PropertyPath(path);
#endif
		}



		public double EndAngleInternal
		{
			get { return (double)GetValue(EndAngleInternalProperty); }
			set { SetValue(EndAngleInternalProperty, value); }
		}

		// Using a DependencyProperty as the backing store for EndAngleInternal.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty EndAngleInternalProperty =
			DependencyProperty.Register("EndAngleInternal", typeof(double), typeof(PieSlice), new PropertyMetadata(0d, OnEndAngleInternalChanged));

		private static void OnEndAngleInternalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var target = (PieSlice)d;
			target.UpdatePath(target.StartAngle, (double)e.NewValue);
		}


		#endregion

		#region Radius
		/// <summary>
		/// The radius property.
		/// </summary>
		public static readonly DependencyProperty RadiusProperty =
			DependencyProperty.Register(
				"Radius",
				typeof(double),
				typeof(PieSlice),
				new PropertyMetadata(
					0d,
					OnRadiusChanged));

		/// <summary>
		/// Gets or sets the radius of the pie slice.
		/// </summary>
		/// <value>
		/// The radius.
		/// </value>
		public double Radius
		{
			get { return (double)GetValue(RadiusProperty); }
			set { SetValue(RadiusProperty, value); }
		}

		private static void OnRadiusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var target = (PieSlice)sender;
			var oldRadius = (double)e.OldValue;
			var newRadius = (double)e.NewValue;
			target.OnRadiusChanged(oldRadius, newRadius);
		}

		private void OnRadiusChanged(double oldRadius, double newRadius)
		{
			this.Width = this.Height = 2 * Radius;
			UpdatePath(StartAngle, EndAngle);
		}
		#endregion

		#region StrokeThicknessInternal
		private double StrokeThicknessInternal
		{
			get { return (double)GetValue(StrokeThicknessInternalProperty); }
			set { SetValue(StrokeThicknessInternalProperty, value); }
		}

		// Using a DependencyProperty as the backing store for StrokeThicknessInternal.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty StrokeThicknessInternalProperty =
			DependencyProperty.Register("StrokeThicknessInternal", typeof(double), typeof(PieSlice), new PropertyMetadata(0d, OnStrokeThicknessInternalChanged));

		private static void OnStrokeThicknessInternalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((PieSlice)d).OnStrokeThicknessChanged(d, (double)e.NewValue);
		}
		#endregion

		#region Animation Duration

		public TimeSpan AnimationDuration
		{
			get { return (TimeSpan)GetValue(AnimationDurationProperty); }
			set { SetValue(AnimationDurationProperty, value); }
		}

		// Using a DependencyProperty as the backing store for AnimationDuration.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty AnimationDurationProperty =
			DependencyProperty.Register("AnimationDuration", typeof(TimeSpan), typeof(PieSlice), new PropertyMetadata(TimeSpan.Zero));


		#endregion


		/// <summary>
		/// Initializes a new instance of the <see cref="PieSlice" /> class.
		/// </summary>
		public PieSlice()
		{
			this.SizeChanged += OnSizeChanged;
			SetBinding(
				StrokeThicknessInternalProperty,
				new Binding()
				{
					Source = this,
					Path = new PropertyPath("StrokeThickness"),
					Mode = BindingMode.OneWay
				});
		}


		private void OnStrokeThicknessChanged(object sender, double e)
		{
			UpdatePath(StartAngle, EndAngle);
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdatePath(StartAngle, EndAngle);
		}

		/// <summary>
		/// Suspends path updates until EndUpdate is called;
		/// </summary>
		public void BeginUpdate()
		{
			_isUpdating = true;
		}

		/// <summary>
		/// Resumes immediate path updates every time a component property value changes. Updates the path.
		/// </summary>
		public void EndUpdate()
		{
			_isUpdating = false;
			UpdatePath(StartAngle, EndAngle);
		}

		private void UpdatePath(double startAngle, double endAngle)
		{
			var radius = this.Radius - this.StrokeThickness / 2;

			if (_isUpdating ||
				this.ActualWidth == 0 ||
				radius <= 0)
			{
				return;
			}

			var pathGeometry = new PathGeometry();
			var pathFigure = new PathFigure
			{
				StartPoint = new Point(radius, radius),
				IsClosed = true
			};

			// Starting Point
			var lineSegment =
				new LineSegment
				{
					Point = new Point(
						radius + Math.Sin(startAngle * Math.PI / 180) * radius,
						radius - Math.Cos(startAngle * Math.PI / 180) * radius)
				};

			// Arc
			var arcSegment = new ArcSegment();
			arcSegment.IsLargeArc = (endAngle - startAngle) >= 180.0;
			arcSegment.Point =
				new Point(
						radius + Math.Sin(endAngle * Math.PI / 180) * radius,
						radius - Math.Cos(endAngle * Math.PI / 180) * radius);
			arcSegment.Size = new Size(radius, radius);
			arcSegment.SweepDirection = SweepDirection.Clockwise;
			pathFigure.Segments.Add(lineSegment);
			pathFigure.Segments.Add(arcSegment);
			pathGeometry.Figures.Add(pathFigure);
			this.Data = pathGeometry;
			this.InvalidateArrange();
		}
	}
}