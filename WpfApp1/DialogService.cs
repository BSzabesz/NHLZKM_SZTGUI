using System.Windows;

namespace WpfApp1
{
    public class DialogService : IDialogService
    {
        public bool TryPickJson(out string? path)
        {
            var dlg = new ImportJsonWindow
            {
                Owner = Application.Current?.MainWindow
            };

            var ok = dlg.ShowDialog() == true;
            path = ok ? dlg.SelectedPath : null;
            return ok;
        }
    }
}
