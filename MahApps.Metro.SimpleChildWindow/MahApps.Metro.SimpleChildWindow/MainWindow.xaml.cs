using System.Windows;
using MahApps.Metro.Controls;

namespace testproject
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : MetroWindow
  {
    public MainWindow() {
      this.InitializeComponent();
    }

    private void FirstTest_OnClick(object sender, RoutedEventArgs e) {
      this.child01.IsOpen = true;
    }

    private void SecTest_OnClick(object sender, RoutedEventArgs e) {
      this.child02.IsOpen = true;
    }

    private void ThirdTest_OnClick(object sender, RoutedEventArgs e) {
      this.child03.IsOpen = true;
    }

    private void CloseFirst_OnClick(object sender, RoutedEventArgs e) {
      this.child01.IsOpen = false;
    }

    private void CloseSec_OnClick(object sender, RoutedEventArgs e) {
      this.child02.IsOpen = false;
    }
  }
}