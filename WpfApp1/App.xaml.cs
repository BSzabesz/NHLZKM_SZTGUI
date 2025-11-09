using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NHLZKM_SZTGUI.Persistence.MsSql;
using NHLZKM_SZTGUI.Application;

namespace WpfApp1
{
    public partial class App : Application
    {
        private IHost _host = null!;
        public static IHost HostRef => ((App)Current)._host;

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((ctx, services) =>
                {
                    services.AddScoped<FormulaOneDbContext>();

                    services.AddSingleton<IAnnualBudgetDataProvider, AnnualBudgetDataProvider>();
                    services.AddSingleton<IBudgetItemDataProvider, BudgetItemDataProvider>();
                    services.AddSingleton<ITeamDataProvider, TeamDataProvider>();

                    services.AddSingleton<IAnnualBudgetService, AnnualBudgetService>();
                    services.AddSingleton<IBudgetItemService, BudgetItemService>();
                    services.AddSingleton<ITeamService, TeamService>();
                    services.AddSingleton<IJsonImporterService, JsonImporterService>();
                    services.AddSingleton<ImportJsonWindow>();
                    services.AddSingleton<IDialogService, DialogService>();

                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton<MainWindow>();
                })
                .Build();

            await _host.StartAsync();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.DataContext = _host.Services.GetRequiredService<MainViewModel>();
            MainWindow = mainWindow;
            mainWindow.Show();
        }

    }
}
