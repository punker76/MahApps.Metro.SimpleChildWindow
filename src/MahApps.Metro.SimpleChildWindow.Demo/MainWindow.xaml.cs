using System.ComponentModel;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace MahApps.Metro.SimpleChildWindow.Demo
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : MetroWindow
	{
		public MainWindow()
		{
			this.InitializeComponent();
		}

		private void FirstTest_OnClick(object sender, RoutedEventArgs e)
		{
			this.child01.SetCurrentValue(ChildWindow.IsOpenProperty, true);
		}

		private async void SecTest_OnClick(object sender, RoutedEventArgs e)
		{
			await this.ShowChildWindowAsync(new TestChildWindow(), this.RootGrid);
		}

		private async void ThirdTest_OnClick(object sender, RoutedEventArgs e)
		{
			var result = await this.ShowChildWindowAsync<bool>(new CoolChildWindow() {IsModal = true, AllowMove = true}, ChildWindowManager.OverlayFillBehavior.FullWindow);
			if (result)
			{
				await this.ShowMessageAsync("ChildWindow Result", "He, you just clicked the 'Ok' button.");
			}
			else
			{
				await this.ShowMessageAsync("ChildWindow Result", "The dialog was canceled.");
			}
		}

		private void CloseFirst_OnClick(object sender, RoutedEventArgs e)
		{
			this.child01.Close();
		}

		private void Child01_OnClosing(object sender, CancelEventArgs e)
		{
			//e.Cancel = true; // don't close
		}

		private async void MovingTest_OnClick(object sender, RoutedEventArgs e)
        {
            var childWindow = new CoolChildWindow() {IsModal = true, AllowMove = true, VerticalContentAlignment = VerticalAlignment.Bottom};
            var result = await this.ShowChildWindowAsync<CloseReason>(childWindow, RootGrid);
            if (result == CloseReason.Cancel)
            {
                await this.ShowMessageAsync("ChildWindow Result", "The dialog was canceled.");
            }
        }
	}
}