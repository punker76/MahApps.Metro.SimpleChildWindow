using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MS.Internal;

namespace MahApps.Metro.SimpleChildWindow
{
  /// <summary>
  /// Interaction logic for ChildWindow.xaml
  /// </summary>
  public partial class ChildWindow : ContentControl
  {
    public static readonly DependencyProperty HeaderProperty =
      DependencyProperty.Register("Header", typeof(string), typeof(ChildWindow), new PropertyMetadata(default(string)));

    public string Header {
      get { return (string)this.GetValue(HeaderProperty); }
      set { this.SetValue(HeaderProperty, value); }
    }

    public static readonly DependencyProperty IsOpenProperty =
      DependencyProperty.Register("IsOpen", typeof(bool), typeof(ChildWindow), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, IsOpenedChanged));

    public bool IsOpen {
      get { return (bool)this.GetValue(IsOpenProperty); }
      set { this.SetValue(IsOpenProperty, value); }
    }

    private static void IsOpenedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e) {
      var childWindow = (ChildWindow)dependencyObject;
      VisualStateManager.GoToState(childWindow, (bool)e.NewValue == false ? "Hide" : "Show", true);
      if ((bool)e.NewValue) {
        Canvas.SetZIndex(childWindow, 1);
        var child = FindVisualChilds<UIElement>(childWindow, true).FirstOrDefault(c => c.Focusable);
        if (child != null) {
          child.IsVisibleChanged += (sender, args) => child.Focus();
        }
      }
      if (childWindow.IsOpenChanged != null) {
        childWindow.IsOpenChanged(childWindow, EventArgs.Empty);
      }
    }

    public event EventHandler IsOpenChanged;

    public static readonly DependencyProperty ChildWindowWidthProperty =
      DependencyProperty.Register("ChildWindowWidth",
                                  typeof(double),
                                  typeof(ChildWindow),
                                  new FrameworkPropertyMetadata(Double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure),
                                  new ValidateValueCallback(IsWidthHeightValid));

    public double ChildWindowWidth {
      get { return (double)this.GetValue(ChildWindowWidthProperty); }
      set { this.SetValue(ChildWindowWidthProperty, value); }
    }

    public static readonly DependencyProperty ChildWindowHeightProperty =
      DependencyProperty.Register("ChildWindowHeight",
                                  typeof(double),
                                  typeof(ChildWindow),
                                  new FrameworkPropertyMetadata(Double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure),
                                  new ValidateValueCallback(IsWidthHeightValid));

    private static bool IsWidthHeightValid(object value) {
      var v = (double)value;
      return (double.IsNaN(v)) || (v >= 0.0d && !double.IsPositiveInfinity(v));
    }

    public double ChildWindowHeight {
      get { return (double)this.GetValue(ChildWindowHeightProperty); }
      set { this.SetValue(ChildWindowHeightProperty, value); }
    }

    static ChildWindow() {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ChildWindow), new FrameworkPropertyMetadata(typeof(ChildWindow)));
    }

    /// <summary>
    /// Finds the typed visual at dependency object visual tree.
    /// </summary>
    public static T FindVisualChild<T>(DependencyObject obj, bool applyTemplateIfNeeded = false) where T : DependencyObject {
      return FindVisualChilds<T>(obj, applyTemplateIfNeeded).FirstOrDefault();
    }

    /// <summary>
    /// Finds the typed visual at dependency object visual tree.
    /// </summary>
    public static IEnumerable<T> FindVisualChilds<T>(DependencyObject obj, bool applyTemplateIfNeeded = false) where T : DependencyObject {
      if (obj != null) {
        var childrenCount = VisualTreeHelper.GetChildrenCount(obj);
        if (childrenCount == 0 && applyTemplateIfNeeded) {
          var fe = obj as FrameworkElement;
          if (fe != null) {
            if (fe.ApplyTemplate()) {
              childrenCount = VisualTreeHelper.GetChildrenCount(obj);
            }
          }
        }
        for (var i = 0; i < childrenCount; i++) {
          var child = VisualTreeHelper.GetChild(obj, i);
          if (child is T) {
            yield return (T)child;
          }
          var childOfChild = FindVisualChilds<T>(child, applyTemplateIfNeeded);
          foreach (var c in childOfChild) {
            yield return c;
          }
        }
      }
    }
  }
}