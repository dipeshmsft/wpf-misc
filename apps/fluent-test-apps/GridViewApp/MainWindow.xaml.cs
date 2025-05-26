using System.Collections.ObjectModel;
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

namespace GridViewApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
    public class MainViewModel
    {
        public class Car
        {
            public string? Make { get; set; }

            public string? Model { get; set; }
        }

        public ObservableCollection<Car> Cars { get; set; } = [];

        public MainViewModel()
        {
            Cars.Add(new Car() { Make = "Toyota", Model = "Corolla" });
            Cars.Add(new Car() { Make = "Honda", Model = "Civic" });
        }
    }
}