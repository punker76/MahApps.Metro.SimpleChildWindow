using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using MahApps.Metro.SimpleChildWindow.Utils;

namespace MahApps.Metro.SimpleChildWindow.ValueConverter
{
	public class MessageBoxImageToImageSourceConverter : SingletonBase<MessageBoxImageToImageSourceConverter>, IValueConverter
	{
		private readonly Visual hand_stop_error;

		private MessageBoxImageToImageSourceConverter()
		{
			var resDict = new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro.SimpleChildWindow;component/Resources/Icons.xaml", UriKind.RelativeOrAbsolute) };
			this.hand_stop_error = resDict["appbar_noentry"] as Visual;
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is MessageBoxImage)
			{
				switch ((MessageBoxImage)value)
				{
					case MessageBoxImage.None:
						break;
					case MessageBoxImage.Hand:
						return this.hand_stop_error;
					case MessageBoxImage.Question:
						break;
					case MessageBoxImage.Exclamation:
						break;
					case MessageBoxImage.Asterisk:
						break;
				}
			}
			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return DependencyProperty.UnsetValue;
		}
	}
}