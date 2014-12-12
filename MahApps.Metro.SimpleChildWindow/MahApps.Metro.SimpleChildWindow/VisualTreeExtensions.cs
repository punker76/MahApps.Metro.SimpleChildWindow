using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace MahApps.Metro.SimpleChildWindow
{
	public static class VisualTreeExtensions
	{
		/// <summary>
		/// Finds the typed visual at dependency object visual tree.
		/// </summary>
		public static T FindVisualChild<T>(this DependencyObject obj, bool applyTemplateIfNeeded = false) where T : DependencyObject
		{
			return FindVisualChilds<T>(obj, applyTemplateIfNeeded).FirstOrDefault();
		}

		/// <summary>
		/// Finds the typed visual at dependency object visual tree.
		/// </summary>
		public static IEnumerable<T> FindVisualChilds<T>(this DependencyObject obj, bool applyTemplateIfNeeded = false) where T : DependencyObject
		{
			if (obj != null)
			{
				var childrenCount = VisualTreeHelper.GetChildrenCount(obj);
				if (childrenCount == 0 && applyTemplateIfNeeded)
				{
					var fe = obj as FrameworkElement;
					if (fe != null)
					{
						if (fe.ApplyTemplate())
						{
							childrenCount = VisualTreeHelper.GetChildrenCount(obj);
						}
					}
				}
				for (var i = 0; i < childrenCount; i++)
				{
					var child = VisualTreeHelper.GetChild(obj, i);
					if (child is T)
					{
						yield return (T)child;
					}
					var childOfChild = FindVisualChilds<T>(child, applyTemplateIfNeeded);
					foreach (var c in childOfChild)
					{
						yield return c;
					}
				}
			}
		}
	}
}