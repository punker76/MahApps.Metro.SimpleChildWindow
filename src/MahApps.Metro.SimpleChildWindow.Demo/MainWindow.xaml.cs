using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow.Demo.Properties;

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
            if (e.OriginalSource is Button button)
            {
                // This dialog should be displayed only once!
                // So disable the button while the dialog is open.
                button.IsEnabled = false;
                await this.ShowChildWindowAsync(new TestChildWindow(), this.RootGrid).ConfigureAwait(true);
                button.IsEnabled = true;
            }
        }

        private async void ThirdTest_OnClick(object sender, RoutedEventArgs e)
        {
            var result = await this.ShowChildWindowAsync<bool>(new CoolChildWindow() { IsModal = true, AllowMove = true }, ChildWindowManager.OverlayFillBehavior.FullWindow);
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
            var childWindow = new CoolChildWindow() { IsModal = true, AllowMove = true, VerticalContentAlignment = VerticalAlignment.Top, HorizontalContentAlignment = HorizontalAlignment.Left };
            var result = await this.ShowChildWindowAsync<CloseReason>(childWindow, this.RootGrid);
            if (result == CloseReason.Cancel)
            {
                await this.ShowMessageAsync("ChildWindow Result", "The dialog was canceled.");
            }

            Settings.Default.Save();
        }
    }
}