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
			this.Activated += TestChildWindow_Activated;
			this.Deactivated += TestChildWindow_Deactivated;
		}

		private void TestChildWindow_Activated(object sender, OnActiveChangedEventArgs e)
		{
			var w = e.OriginalSource as MetroWindow;
			if (w != null)
			{
			}
		}

		private void TestChildWindow_Deactivated(object sender, OnActiveChangedEventArgs e)
		{
			var w = e.OriginalSource as MetroWindow;
			if (w != null)
			{
			}
		}

		private void CloseSec_OnClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}