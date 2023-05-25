using RtspViewer.GUI.ViewModels;
using System.Windows;

namespace RtspViewer.GUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void FullScreenOption_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
        }

        private void FullScreenOption_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            WindowStyle = WindowStyle.SingleBorderWindow;
            WindowState = WindowState.Normal;
        }

        private void ExitOption_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void StatusBarOption_Checked(object sender, RoutedEventArgs e)
        {
            WindowStatusBar.Visibility = Visibility.Visible;
        }

        private void StatusBarOption_Unchecked(object sender, RoutedEventArgs e)
        {
            WindowStatusBar.Visibility = Visibility.Collapsed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StatusBarOption.Checked += StatusBarOption_Checked;
        }
    }
}