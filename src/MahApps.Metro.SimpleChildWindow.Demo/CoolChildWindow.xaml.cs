using System.Windows;
using MahApps.Metro.Controls;

namespace MahApps.Metro.SimpleChildWindow.Demo
{
	/// <summary>
	/// Interaction logic for CoolChildWindow.xaml
	/// </summary>
	public partial class CoolChildWindow : ChildWindow
	{
		public CoolChildWindow()
		{
			this.InitializeComponent();
		}

		private void OkButtonOnClick(object sender, RoutedEventArgs e)
		{
			this.Close(true);
		}

		private void CloseSec_OnClick(object sender, RoutedEventArgs e)
		{
			this.Close(false);
		}
	}
}