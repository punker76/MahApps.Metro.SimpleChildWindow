using System.Windows;

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
            this.Close(CloseReason.Ok, true);
        }

        private void CloseSec_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close(CloseReason.Cancel, false);
        }
    }
}