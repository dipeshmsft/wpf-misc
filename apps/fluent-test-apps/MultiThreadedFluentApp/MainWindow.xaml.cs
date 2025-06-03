using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultiThreadedFluentApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MTLightWindow_Click(object sender, RoutedEventArgs e)
        {
            tm = ThemeMode.Light;
            StartNewThreadedWindow();
        }

        private void MTDarkWindow_Click(object sender, RoutedEventArgs e)
        {
            tm = ThemeMode.Dark;
            StartNewThreadedWindow();
        }

        private void MTSystemWindow_Click(object sender, RoutedEventArgs e)
        {
            tm = ThemeMode.System;
            StartNewThreadedWindow();

        }

        private static void StartNewThreadedWindow()
        {
            var thread = new Thread(StartOnThread);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private static void StartOnThread(object? obj)
        {
            var window = new MainWindow
            {
                ThemeMode = tm
            };
            window.Show();
            System.Windows.Threading.Dispatcher.Run();
        }

        static ThemeMode tm;
    }
}