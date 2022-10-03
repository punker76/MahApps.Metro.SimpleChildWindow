using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace MahApps.Metro.SimpleChildWindow.Demo
{
    /// <summary>
    /// Interaction logic for TestChildWindow.xaml
    /// </summary>
    public partial class TestChildWindow : ChildWindow
    {
        public TestChildWindow()
        {
            InitializeComponent();
        }

        private async void MessageButtonOnClick(object sender, RoutedEventArgs e)
        {
            await ((MetroWindow)Window.GetWindow(this)).ShowMessageAsync("Title", "Message");
        }

        private void CloseSec_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}