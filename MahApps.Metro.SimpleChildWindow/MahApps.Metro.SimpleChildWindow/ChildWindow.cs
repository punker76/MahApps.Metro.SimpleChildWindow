using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace MahApps.Metro.SimpleChildWindow
{
	[TemplatePart(Name = PART_Overlay, Type = typeof(Grid))]
	[TemplatePart(Name = PART_Window, Type = typeof(Grid))]
	[TemplatePart(Name = PART_Header, Type = typeof(Grid))]
	[TemplatePart(Name = PART_HeaderThumb, Type = typeof(Thumb))]
	public class ChildWindow : ContentControl
	{
		private const string PART_Overlay = "PART_Overlay";
		private const string PART_Window = "PART_Window";
		private const string PART_Header = "PART_Header";
		private const string PART_HeaderThumb = "PART_HeaderThumb";

		public static readonly DependencyProperty ShowTitleBarProperty
			= DependencyProperty.Register("ShowTitleBar",
										  typeof(bool),
										  typeof(ChildWindow),
										  new PropertyMetadata(true));

		public static readonly DependencyProperty TitlebarHeightProperty
			= DependencyProperty.Register("TitlebarHeight",
										  typeof(int),
										  typeof(ChildWindow),
										  new PropertyMetadata(30));

		public static readonly DependencyProperty HeaderProperty
			= DependencyProperty.Register("Header",
										  typeof(string),
										  typeof(ChildWindow),
										  new PropertyMetadata(default(string)));

		public static readonly DependencyProperty IsOpenProperty
			= DependencyProperty.Register("IsOpen",
										  typeof(bool),
										  typeof(ChildWindow),
										  new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, IsOpenedChanged));

		public static readonly DependencyProperty ChildWindowWidthProperty
			= DependencyProperty.Register("ChildWindowWidth",
										  typeof(double),
										  typeof(ChildWindow),
										  new FrameworkPropertyMetadata(Double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure), IsWidthHeightValid);

		public static readonly DependencyProperty ChildWindowHeightProperty
			= DependencyProperty.Register("ChildWindowHeight",
										  typeof(double),
										  typeof(ChildWindow),
										  new FrameworkPropertyMetadata(Double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure), IsWidthHeightValid);

		public static readonly DependencyProperty ChildWindowImageProperty
			= DependencyProperty.Register("ChildWindowImage",
										  typeof(MessageBoxImage),
										  typeof(ChildWindow),
										  new FrameworkPropertyMetadata(MessageBoxImage.None, FrameworkPropertyMetadataOptions.AffectsMeasure));

		public static readonly DependencyProperty EnableDropShadowProperty
			= DependencyProperty.Register("EnableDropShadow",
										  typeof(bool),
										  typeof(ChildWindow),
										  new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

		private Storyboard hideStoryboard;

		/// <summary>
		/// An event that is raised when IsOpen changes.
		/// </summary>
		public static readonly RoutedEvent IsOpenChangedEvent
			= EventManager.RegisterRoutedEvent("IsOpenChanged",
											   RoutingStrategy.Bubble,
											   typeof(RoutedEventHandler),
											   typeof(ChildWindow));

		public event RoutedEventHandler IsOpenChanged
		{
			add { AddHandler(IsOpenChangedEvent, value); }
			remove { RemoveHandler(IsOpenChangedEvent, value); }
		}

		/// <summary>
		/// An event that is raised when the closing animation has finished.
		/// </summary>
		public static readonly RoutedEvent ClosingFinishedEvent
			= EventManager.RegisterRoutedEvent("ClosingFinished",
											   RoutingStrategy.Bubble,
											   typeof(RoutedEventHandler),
											   typeof(ChildWindow));

		public event RoutedEventHandler ClosingFinished
		{
			add { AddHandler(ClosingFinishedEvent, value); }
			remove { RemoveHandler(ClosingFinishedEvent, value); }
		}

		/// <summary>
		/// Gets/sets whether the TitleBar is visible or not.
		/// </summary>
		public bool ShowTitleBar
		{
			get { return (bool)GetValue(ShowTitleBarProperty); }
			set { SetValue(ShowTitleBarProperty, value); }
		}

		/// <summary>
		/// Gets/sets the TitleBar's height.
		/// </summary>
		public int TitlebarHeight
		{
			get { return (int)GetValue(TitlebarHeightProperty); }
			set { SetValue(TitlebarHeightProperty, value); }
		}

		public string Header
		{
			get { return (string)this.GetValue(HeaderProperty); }
			set { this.SetValue(HeaderProperty, value); }
		}

		public bool IsOpen
		{
			get { return (bool)this.GetValue(IsOpenProperty); }
			set { this.SetValue(IsOpenProperty, value); }
		}

		private static void IsOpenedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var childWindow = (ChildWindow)dependencyObject;

			Action openedChangedAction = () => {
				if (e.NewValue != e.OldValue)
				{
					if ((bool)e.NewValue)
					{
						if (childWindow.hideStoryboard != null)
						{
							// don't let the storyboard end it's completed event
							// otherwise it could be hidden on start
							childWindow.hideStoryboard.Completed -= childWindow.HideStoryboard_Completed;
						}

						Canvas.SetZIndex(childWindow, 1);
						var child = childWindow.FindVisualChilds<UIElement>(true).FirstOrDefault(c => c.Focusable);
						if (child != null)
						{
							child.IsVisibleChanged += (sender, args) => child.Focus();
						}
					}
					else
					{
						if (childWindow.hideStoryboard != null)
						{
							childWindow.hideStoryboard.Completed += childWindow.HideStoryboard_Completed;
						}
						else
						{
							childWindow.Hide();
						}
					}

					VisualStateManager.GoToState(childWindow, (bool)e.NewValue == false ? "Hide" : "Show", true);

					childWindow.RaiseEvent(new RoutedEventArgs(IsOpenChangedEvent));
				}
			};

			childWindow.Dispatcher.BeginInvoke(DispatcherPriority.Background, openedChangedAction);
		}

		private void HideStoryboard_Completed(object sender, EventArgs e)
		{
			this.hideStoryboard.Completed -= this.HideStoryboard_Completed;
			this.Hide();
		}

		private void Hide()
		{
			this.DataContext = null;
			this.RaiseEvent(new RoutedEventArgs(ClosingFinishedEvent));
		}

		public double ChildWindowWidth
		{
			get { return (double)this.GetValue(ChildWindowWidthProperty); }
			set { this.SetValue(ChildWindowWidthProperty, value); }
		}

		private static bool IsWidthHeightValid(object value)
		{
			var v = (double)value;
			return (double.IsNaN(v)) || (v >= 0.0d && !double.IsPositiveInfinity(v));
		}

		public double ChildWindowHeight
		{
			get { return (double)this.GetValue(ChildWindowHeightProperty); }
			set { this.SetValue(ChildWindowHeightProperty, value); }
		}

		public MessageBoxImage ChildWindowImage
		{
			get { return (MessageBoxImage)this.GetValue(ChildWindowImageProperty); }
			set { this.SetValue(ChildWindowImageProperty, value); }
		}

		public bool EnableDropShadow
		{
			get { return (bool)this.GetValue(EnableDropShadowProperty); }
			set { this.SetValue(EnableDropShadowProperty, value); }
		}

		static ChildWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ChildWindow), new FrameworkPropertyMetadata(typeof(ChildWindow)));
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.hideStoryboard = (Storyboard)GetTemplateChild("HideStoryboard");
		}

		protected override void OnPreviewKeyUp(System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Escape)
			{
				this.IsOpen = false;
				e.Handled = true;
			}
			base.OnPreviewKeyUp(e);
		}
	}
}