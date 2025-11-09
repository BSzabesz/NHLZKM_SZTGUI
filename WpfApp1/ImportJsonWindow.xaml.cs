using Microsoft.Win32;
using System.Windows;

namespace WpfApp1
{
    public partial class ImportJsonWindow : Window
    {
        public string? SelectedPath { get; private set; }

        public ImportJsonWindow()
        {
            InitializeComponent();
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "JSON fájlok (*.json)|*.json|Minden fájl (*.*)|*.*",
                Title = "Válassz JSON fájlt"
            };
            if (ofd.ShowDialog() == true)
            {
                PathBox.Text = ofd.FileName;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PathBox.Text))
            {
                SelectedPath = PathBox.Text;
                DialogResult = true; 
            }
            else
            {
                MessageBox.Show("Adj meg egy fájl elérési utat.", "Figyelem",MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}

