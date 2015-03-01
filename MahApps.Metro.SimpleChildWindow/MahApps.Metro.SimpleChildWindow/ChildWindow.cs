using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using MahApps.Metro.SimpleChildWindow.Utils;

namespace MahApps.Metro.SimpleChildWindow
{
	[TemplatePart(Name = PART_Overlay, Type = typeof(Grid))]
	[TemplatePart(Name = PART_Window, Type = typeof(Grid))]
	[TemplatePart(Name = PART_Header, Type = typeof(Grid))]
	[TemplatePart(Name = PART_HeaderThumb, Type = typeof(Thumb))]
	[TemplatePart(Name = PART_Icon, Type = typeof(ContentControl))]
	[TemplatePart(Name = PART_CloseButton, Type = typeof(Button))]
	[TemplatePart(Name = PART_Border, Type = typeof(Border))]
	[TemplatePart(Name = PART_Content, Type = typeof(ContentPresenter))]
	public class ChildWindow : ContentControl
	{
		private const string PART_Overlay = "PART_Overlay";
		private const string HideStoryboard = "HideStoryboard";
		private const string PART_Window = "PART_Window";
		private const string PART_Header = "PART_Header";
		private const string PART_HeaderThumb = "PART_HeaderThumb";
		private const string PART_Icon = "PART_Icon";
		private const string PART_CloseButton = "PART_CloseButton";
		private const string PART_Border = "PART_Border";
		private const string PART_Content = "PART_Content";

		public static readonly DependencyProperty AllowMoveProperty
			= DependencyProperty.Register("AllowMove",
										  typeof(bool),
										  typeof(ChildWindow),
										  new PropertyMetadata(default(bool)));

		public static readonly DependencyProperty IsModalProperty
			= DependencyProperty.Register("IsModal",
										  typeof(bool),
										  typeof(ChildWindow),
										  new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty OverlayBrushProperty
			= DependencyProperty.Register("OverlayBrush",
										  typeof(Brush),
										  typeof(ChildWindow),
										  new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty CloseOnOverlayProperty
			= DependencyProperty.Register("CloseOnOverlay",
										  typeof(bool),
										  typeof(ChildWindow),
										  new PropertyMetadata(default(bool)));

		public static readonly DependencyProperty ShowTitleBarProperty
			= DependencyProperty.Register("ShowTitleBar",
										  typeof(bool),
										  typeof(ChildWindow),
										  new PropertyMetadata(true));

		public static readonly DependencyProperty TitleBarHeightProperty
			= DependencyProperty.Register("TitleBarHeight",
										  typeof(int),
										  typeof(ChildWindow),
										  new PropertyMetadata(30));

		public static readonly DependencyProperty TitleBarBackgroundProperty
			= DependencyProperty.Register("TitleBarBackground",
										  typeof(Brush),
										  typeof(ChildWindow),
										  new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty TitleForegroundProperty
			= DependencyProperty.Register("TitleForeground",
										  typeof(Brush),
										  typeof(ChildWindow),
										  new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty TitleProperty
			= DependencyProperty.Register("Title",
										  typeof(string),
										  typeof(ChildWindow),
										  new PropertyMetadata(default(string)));

		/// <summary>
		/// DependencyProperty for <see cref="TitleFontSize" /> property.
		/// </summary>
		public static readonly DependencyProperty TitleFontSizeProperty
			= DependencyProperty.Register("TitleFontSize",
										  typeof(double),
										  typeof(ChildWindow),
										  new FrameworkPropertyMetadata(SystemFonts.CaptionFontSize, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));

		/// <summary>
		/// DependencyProperty for <see cref="TitleFontFamily" /> property.
		/// </summary>
		public static readonly DependencyProperty TitleFontFamilyProperty
			= DependencyProperty.Register("TitleFontFamily",
										  typeof(FontFamily),
										  typeof(ChildWindow),
										  new FrameworkPropertyMetadata(SystemFonts.CaptionFontFamily, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty IconProperty
			= DependencyProperty.Register("Icon",
										  typeof(object),
										  typeof(ChildWindow),
										  new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

		public static readonly DependencyProperty IconTemplateProperty
			= DependencyProperty.Register("IconTemplate",
										  typeof(DataTemplate),
										  typeof(ChildWindow),
										  new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

		public static readonly DependencyProperty ShowCloseButtonProperty
			= DependencyProperty.Register("ShowCloseButton",
										  typeof(bool),
										  typeof(ChildWindow),
										  new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

		public static readonly DependencyProperty CloseButtonStyleProperty
			= DependencyProperty.Register("CloseButtonStyle",
										  typeof(Style),
										  typeof(ChildWindow),
										  new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

		public static readonly DependencyProperty CloseButtonCommandProperty
			= DependencyProperty.Register("CloseButtonCommand",
										  typeof(ICommand),
										  typeof(ChildWindow),
										  new PropertyMetadata(default(ICommand)));

		public static readonly DependencyProperty CloseButtonCommandParameterProperty
			= DependencyProperty.Register("CloseButtonCommandParameter",
										  typeof(object),
										  typeof(ChildWindow),
										  new PropertyMetadata(null));

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

		public static readonly DependencyProperty AllowFocusElementProperty
			= DependencyProperty.Register("AllowFocusElement",
										  typeof(bool),
										  typeof(ChildWindow),
										  new PropertyMetadata(true));

		public static readonly DependencyProperty FocusedElementProperty
			= DependencyProperty.Register("FocusedElement",
										  typeof(FrameworkElement),
										  typeof(ChildWindow),
										  new UIPropertyMetadata(null));

		public static readonly DependencyProperty GlowBrushProperty
			= DependencyProperty.Register("GlowBrush",
										  typeof(SolidColorBrush),
										  typeof(ChildWindow),
										  new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

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
		/// Gets or sets a value indicating whether the child window can be moved inside the overlay container.
		/// </summary>
		public bool AllowMove
		{
			get { return (bool)this.GetValue(AllowMoveProperty); }
			set { this.SetValue(AllowMoveProperty, value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the child window is modal.
		/// </summary>
		public bool IsModal
		{
			get { return (bool)this.GetValue(IsModalProperty); }
			set { this.SetValue(IsModalProperty, value); }
		}

		/// <summary>
		/// Gets or sets the overlay brush.
		/// </summary>
		public Brush OverlayBrush
		{
			get { return (Brush)this.GetValue(OverlayBrushProperty); }
			set { this.SetValue(OverlayBrushProperty, value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the child window can be closed by clicking the overlay container.
		/// </summary>
		public bool CloseOnOverlay
		{
			get { return (bool)this.GetValue(CloseOnOverlayProperty); }
			set { this.SetValue(CloseOnOverlayProperty, value); }
		}

		/// <summary>
		/// Gets or sets whether the title bar is visible or not.
		/// </summary>
		public bool ShowTitleBar
		{
			get { return (bool)GetValue(ShowTitleBarProperty); }
			set { SetValue(ShowTitleBarProperty, value); }
		}

		/// <summary>
		/// Gets or sets the height of the title bar.
		/// </summary>
		public int TitleBarHeight
		{
			get { return (int)GetValue(TitleBarHeightProperty); }
			set { SetValue(TitleBarHeightProperty, value); }
		}

		/// <summary>
		/// Gets or sets the title bar background.
		/// </summary>
		public Brush TitleBarBackground
		{
			get { return (Brush)this.GetValue(TitleBarBackgroundProperty); }
			set { this.SetValue(TitleBarBackgroundProperty, value); }
		}

		/// <summary>
		/// Gets or sets the title foreground.
		/// </summary>
		public Brush TitleForeground
		{
			get { return (Brush)this.GetValue(TitleForegroundProperty); }
			set { this.SetValue(TitleForegroundProperty, value); }
		}

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		public string Title
		{
			get { return (string)this.GetValue(TitleProperty); }
			set { this.SetValue(TitleProperty, value); }
		}

		/// <summary> 
		/// The FontSize property specifies the size of the title.
		/// </summary>
		[TypeConverter(typeof(FontSizeConverter))]
		public double TitleFontSize
		{
			get { return (double)this.GetValue(TitleFontSizeProperty); }
			set { this.SetValue(TitleFontSizeProperty, value); }
		}

		/// <summary> 
		/// The FontFamily property specifies the font family of the title.
		/// </summary>
		[Bindable(true)]
		public FontFamily TitleFontFamily
		{
			get { return (FontFamily)this.GetValue(TitleFontFamilyProperty); }
			set { this.SetValue(TitleFontFamilyProperty, value); }
		}

		/// <summary>
		/// Gets or sets the icon content template to show a icon or something else.
		/// </summary>
		[Bindable(true)]
		public object Icon
		{
			get { return (object)this.GetValue(IconProperty); }
			set { this.SetValue(IconProperty, value); }
		}

		/// <summary>
		/// Gets or sets the icon content template to show a custom icon or something else.
		/// </summary>
		[Bindable(true)]
		public DataTemplate IconTemplate
		{
			get { return (DataTemplate)this.GetValue(IconTemplateProperty); }
			set { this.SetValue(IconTemplateProperty, value); }
		}

		/// <summary>
		/// Gets or sets if the close button is visible.
		/// </summary>
		public bool ShowCloseButton
		{
			get { return (bool)this.GetValue(ShowCloseButtonProperty); }
			set { this.SetValue(ShowCloseButtonProperty, value); }
		}

		/// <summary>
		/// Gets or sets the close button style.
		/// </summary>
		[Bindable(true)]
		public Style CloseButtonStyle
		{
			get { return (Style)this.GetValue(CloseButtonStyleProperty); }
			set { this.SetValue(CloseButtonStyleProperty, value); }
		}

		/// <summary>
		/// Gets or sets the command that is executed when the Close Button is clicked.
		/// </summary>
		public ICommand CloseButtonCommand
		{
			get { return (ICommand)this.GetValue(CloseButtonCommandProperty); }
			set { this.SetValue(CloseButtonCommandProperty, value); }
		}

		/// <summary>
		/// Gets or sets the command parameter that is used by the CloseButtonCommand when the Close Button is clicked.
		/// </summary>
		public object CloseButtonCommandParameter
		{
			get { return (object)this.GetValue(CloseButtonCommandParameterProperty); }
			set { this.SetValue(CloseButtonCommandParameterProperty, value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is open or closed.
		/// </summary>
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

						Panel.SetZIndex(childWindow, 1);

						childWindow.TryFocusElement();
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

					childWindow.RaiseEvent(new RoutedEventArgs(IsOpenChangedEvent, childWindow));
				}
			};

			childWindow.Dispatcher.BeginInvoke(DispatcherPriority.Background, openedChangedAction);
		}

		private void TryFocusElement()
		{
			if (this.AllowFocusElement)
			{
				var elementToFocus = FocusedElement ?? this.FindChildren<UIElement>().FirstOrDefault(c => c.Focusable);
				if (this.ShowCloseButton && this.closeButton != null && elementToFocus == null)
				{
					this.closeButton.Focusable = true;
					elementToFocus = this.closeButton;
				}
				if (elementToFocus != null)
				{
					DependencyPropertyChangedEventHandler eh = null;
					eh = (sender, args) => {
						elementToFocus.IsVisibleChanged -= eh;
						var focused = elementToFocus.Focus();
					};
					elementToFocus.IsVisibleChanged += eh;
				}
			}
		}

		private void HideStoryboard_Completed(object sender, EventArgs e)
		{
			this.hideStoryboard.Completed -= this.HideStoryboard_Completed;
			this.Hide();
		}

		private void Hide()
		{
			this.DataContext = null;
			this.RaiseEvent(new RoutedEventArgs(ClosingFinishedEvent, this));
		}

		/// <summary>
		/// Gets or sets the width of the child window.
		/// </summary>
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

		/// <summary>
		/// Gets or sets the height of the child window.
		/// </summary>
		public double ChildWindowHeight
		{
			get { return (double)this.GetValue(ChildWindowHeightProperty); }
			set { this.SetValue(ChildWindowHeightProperty, value); }
		}

		/// <summary>
		/// Gets or sets which image is shown on the left side of the window content.
		/// </summary>
		public MessageBoxImage ChildWindowImage
		{
			get { return (MessageBoxImage)this.GetValue(ChildWindowImageProperty); }
			set { this.SetValue(ChildWindowImageProperty, value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the window has a drop shadow (glow brush).
		/// </summary>
		public bool EnableDropShadow
		{
			get { return (bool)this.GetValue(EnableDropShadowProperty); }
			set { this.SetValue(EnableDropShadowProperty, value); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the child window should try focus an element.
		/// </summary>
		public bool AllowFocusElement
		{
			get { return (bool)this.GetValue(AllowFocusElementProperty); }
			set { this.SetValue(AllowFocusElementProperty, value); }
		}

		/// <summary>
		/// Gets or sets the focused element.
		/// </summary>
		public FrameworkElement FocusedElement
		{
			get { return (FrameworkElement)this.GetValue(FocusedElementProperty); }
			set { this.SetValue(FocusedElementProperty, value); }
		}

		/// <summary>
		/// Gets or sets the glow brush (drop shadow).
		/// </summary>
		public SolidColorBrush GlowBrush
		{
			get { return (SolidColorBrush)this.GetValue(GlowBrushProperty); }
			set { this.SetValue(GlowBrushProperty, value); }
		}

		private string closeText;

		/// <summary>
		/// Gets the close button tool tip.
		/// </summary>
		public string CloseButtonToolTip
		{
			get
			{
				if (string.IsNullOrEmpty(closeText))
				{
					closeText = GetCaption(905);
				}
				return closeText;
			}
		}

		private Storyboard hideStoryboard;
		private Thumb headerThumb;
		private Button closeButton;
		private TranslateTransform moveTransform = new TranslateTransform();
		private Grid partWindow;
		private Grid partOverlay;

		static ChildWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ChildWindow), new FrameworkPropertyMetadata(typeof(ChildWindow)));
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			// really necessary?
			if (this.Template == null)
			{
				return;
			}

			this.hideStoryboard = this.Template.FindName(HideStoryboard, this) as Storyboard;

			if (this.partOverlay != null)
			{
				this.partOverlay.MouseLeftButtonDown -= PartOverlayOnClose;
			}
			this.partOverlay = this.Template.FindName(PART_Overlay, this) as Grid;
			if (this.partOverlay != null)
			{
				this.partOverlay.MouseLeftButtonDown += PartOverlayOnClose;
			}

			this.partWindow = this.Template.FindName(PART_Window, this) as Grid;
			if (this.partWindow != null)
			{
				this.partWindow.RenderTransform = this.moveTransform;
			}

			if (this.headerThumb != null)
			{
				this.headerThumb.DragDelta -= new DragDeltaEventHandler(this.HeaderThumbDragDelta);
			}
			this.headerThumb = this.Template.FindName(PART_HeaderThumb, this) as Thumb;
			if (this.headerThumb != null && this.partWindow != null)
			{
				var allowDragging = this.partWindow.HorizontalAlignment != HorizontalAlignment.Stretch
									&& this.partWindow.VerticalAlignment != VerticalAlignment.Stretch;
				if (allowDragging)
				{
					this.headerThumb.DragDelta += new DragDeltaEventHandler(this.HeaderThumbDragDelta);
				}
			}

			if (this.closeButton != null)
			{
				this.closeButton.Click -= new RoutedEventHandler(this.Close);
			}
			this.closeButton = this.Template.FindName(PART_CloseButton, this) as Button;
			if (this.closeButton != null)
			{
				this.closeButton.Click += new RoutedEventHandler(this.Close);
			}
		}

		private void PartOverlayOnClose(object sender, MouseButtonEventArgs e)
		{
			if (Equals(e.OriginalSource, partOverlay) && this.CloseOnOverlay)
			{
				this.Close();
			}
		}

		private void HeaderThumbDragDelta(object sender, DragDeltaEventArgs e)
		{
			var horizontalChange = this.FlowDirection == FlowDirection.RightToLeft ? -e.HorizontalChange : e.HorizontalChange;
			ProcessMove(horizontalChange, e.VerticalChange);
		}

		private void ProcessMove(double x, double y)
		{
			this.moveTransform.X += x;
			this.moveTransform.Y += y;

			this.InvalidateArrange();
		}

		private void Close(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// Closes this instance.
		/// </summary>
		public bool Close()
		{
			if (this.CloseButtonCommand != null)
			{
				this.CloseButtonCommand.Execute(this.CloseButtonCommandParameter);
				this.CloseButtonCommand = null;
				this.CloseButtonCommandParameter = null;
			}

			this.IsOpen = false;

			return true;
		}

		protected override void OnPreviewKeyUp(System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == System.Windows.Input.Key.Escape)
			{
				e.Handled = this.Close();
			}
			base.OnPreviewKeyUp(e);
		}

		private SafeLibraryHandle user32 = null;

		private string GetCaption(int id)
		{
			if (user32 == null)
				user32 = UnsafeNativeMethods.LoadLibrary(Environment.SystemDirectory + "\\User32.dll");

			var sb = new StringBuilder(256);
			UnsafeNativeMethods.LoadString(user32, (uint)id, sb, sb.Capacity);
			return sb.ToString().Replace("&", "");
		}
	}
}