using System.Windows;

namespace MahApps.Metro.SimpleChildWindow
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

		private void CloseSec_OnClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}