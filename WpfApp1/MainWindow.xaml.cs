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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NHLZKM_SZTGUI.Application;
using NHLZKM_SZTGUI.Model;
using NHLZKM_SZTGUI.Persistence.MsSql;
using System.Text.Json;


namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()

        {
            InitializeComponent();
            DataContext = App.HostRef.Services.GetRequiredService<MainViewModel>();

        }
    }
}