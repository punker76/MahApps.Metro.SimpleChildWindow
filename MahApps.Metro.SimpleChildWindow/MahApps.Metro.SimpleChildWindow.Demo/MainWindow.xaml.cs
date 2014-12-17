using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;

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
			this.child01.IsOpen = true;
		}

		private void SecTest_OnClick(object sender, RoutedEventArgs e)
		{
			this.child02.IsOpen = true;
		}

		private async void ThirdTest_OnClick(object sender, RoutedEventArgs e)
		{
			await this.ShowChildWindowAsync(new CoolChildWindow());
		}

		private void CloseFirst_OnClick(object sender, RoutedEventArgs e)
		{
			this.child01.IsOpen = false;
		}

		private void CloseSec_OnClick(object sender, RoutedEventArgs e)
		{
			this.child02.IsOpen = false;
		}
	}
}
