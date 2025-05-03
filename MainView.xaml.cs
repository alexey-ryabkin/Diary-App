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

namespace Diary_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        MainViewModel viewModel = new MainViewModel();
        public MainView()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            UIRow uIRow = (UIRow)button.DataContext;

            viewModel.CreateOrOpen(uIRow);

            viewModel.UpdateButton(uIRow._index);
        }

        private void ButtonPath_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}