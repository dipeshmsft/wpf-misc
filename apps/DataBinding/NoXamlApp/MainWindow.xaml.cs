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

namespace NoXamlApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = _vm;
            InitializeComponent();
        }

        private ViewModel _vm = new();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _vm.TextBoxContent = "Source Changed";
        }
    }
}