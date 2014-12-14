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
using MahApps.Metro.Controls;
using MahApps.Metro.SimpleChildWindow.Utils;

namespace MahApps.Metro.SimpleChildWindow
{
	[TemplatePart(Name = PART_Overlay, Type = typeof(Grid))]
	[TemplatePart(Name = PART_Window, Type = typeof(Grid))]
	[TemplatePart(Name = PART_Header, Type = typeof(Grid))]
	[TemplatePart(Name = PART_HeaderThumb, Type = typeof(Thumb))]
	[TemplatePart(Name = PART_Icon, Type = typeof(ContentControl))]
	[TemplatePart(Name = PART_CloseButton, Type = typeof(Button))]
	public class ChildWindow : ContentControl
	{
		private const string PART_Overlay = "PART_Overlay";
		private const string HideStoryboard = "HideStoryboard";
		private const string PART_Window = "PART_Window";
		private const string PART_Header = "PART_Header";
		private const string PART_HeaderThumb = "PART_HeaderThumb";
		private const string PART_Icon = "PART_Icon";
		private const string PART_CloseButton = "PART_CloseButton";

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

		public static readonly DependencyProperty FocusedElementProperty
			= DependencyProperty.Register("FocusedElement",
										  typeof(FrameworkElement),
										  typeof(ChildWindow),
										  new UIPropertyMetadata(null));

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
		public int TitleBarHeight
		{
			get { return (int)GetValue(TitleBarHeightProperty); }
			set { SetValue(TitleBarHeightProperty, value); }
		}

		public Brush TitleBarBackground
		{
			get { return (Brush)this.GetValue(TitleBarBackgroundProperty); }
			set { this.SetValue(TitleBarBackgroundProperty, value); }
		}

		public Brush TitleForeground
		{
			get { return (Brush)this.GetValue(TitleForegroundProperty); }
			set { this.SetValue(TitleForegroundProperty, value); }
		}

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
		/// Gets/sets the icon content template to show a icon or something else.
		/// </summary>
		[Bindable(true)]
		public object Icon
		{
			get { return (object)this.GetValue(IconProperty); }
			set { this.SetValue(IconProperty, value); }
		}

		/// <summary>
		/// Gets/sets the icon content template to show a custom icon or something else.
		/// </summary>
		[Bindable(true)]
		public DataTemplate IconTemplate
		{
			get { return (DataTemplate)this.GetValue(IconTemplateProperty); }
			set { this.SetValue(IconTemplateProperty, value); }
		}

		/// <summary>
		/// Gets/sets if the close button is visible.
		/// </summary>
		public bool ShowCloseButton
		{
			get { return (bool)this.GetValue(ShowCloseButtonProperty); }
			set { this.SetValue(ShowCloseButtonProperty, value); }
		}

		[Bindable(true)]
		public Style CloseButtonStyle
		{
			get { return (Style)this.GetValue(CloseButtonStyleProperty); }
			set { this.SetValue(CloseButtonStyleProperty, value); }
		}

		/// <summary>
		/// Gets/sets the command that is executed when the Close Button is clicked.
		/// </summary>
		public ICommand CloseButtonCommand
		{
			get { return (ICommand)this.GetValue(CloseButtonCommandProperty); }
			set { this.SetValue(CloseButtonCommandProperty, value); }
		}

		/// <summary>
		/// Gets/sets the command parameter that is used by the CloseButtonCommand when the Close Button is clicked.
		/// </summary>
		public object CloseButtonCommandParameter
		{
			get { return (object)this.GetValue(CloseButtonCommandParameterProperty); }
			set { this.SetValue(CloseButtonCommandParameterProperty, value); }
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
			var elementToFocus = FocusedElement ?? this.FindChildren<UIElement>().FirstOrDefault(c => c.Focusable);
			if (elementToFocus != null)
			{
				elementToFocus.IsVisibleChanged += (sender, args) => elementToFocus.Focus();
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

		public FrameworkElement FocusedElement
		{
			get { return (FrameworkElement)this.GetValue(FocusedElementProperty); }
			set { this.SetValue(FocusedElementProperty, value); }
		}

		private string closeText;
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
			this.headerThumb = this.Template.FindName(PART_HeaderThumb, this) as Thumb;

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