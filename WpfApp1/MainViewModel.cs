using NHLZKM_SZTGUI.Application;
using NHLZKM_SZTGUI.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using NHLZKM_SZTGUI.Application;

namespace WpfApp1
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ITeamService _teamService;
        private readonly IAnnualBudgetService _budgetService;
        private readonly IBudgetItemService _itemService;
        private readonly IDialogService _dialog;
        private readonly IJsonImporterService _importer;
        public ICommand ImportJsonCommand { get; }



        public MainViewModel(
            ITeamService teamService,
            IAnnualBudgetService budgetService,
            IBudgetItemService itemService,
            IDialogService dialog,
            IJsonImporterService importer)
        {
            _teamService = teamService;
            _budgetService = budgetService;
            _itemService = itemService;
            _dialog = dialog;
            _importer = importer;

            LoadTeamsCommand = new RelayCommand(LoadTeams);
            SearchTeamsCommand = new RelayCommand(SearchTeams);
            NextPageCommand = new RelayCommand(NextPage);
            PrevPageCommand = new RelayCommand(PrevPage);
            CreateTeamCommand = new RelayCommand(CreateTeam);  

            LoadBudgetsCommand = new RelayCommand(LoadBudgets);
            UpdateAnnualBudgetCommand = new RelayCommand(UpdateAnnualBudget);

            LoadItemsCommand = new RelayCommand(LoadItems);
            CreateItemCommand = new RelayCommand(CreateBudgetItem);
            DeleteSelectedItemCommand = new RelayCommand(DeleteBudgetItem);

            GeneratePredictionReportCommand = new RelayCommand(GeneratePredictionReport);
            GenerateCategoryReportCommand = new RelayCommand(GenerateCategoryReport);

            ImportJsonCommand = new RelayCommand(ImportJson);
             void ImportJson()
            {
                if (_dialog.TryPickJson(out var path) && !string.IsNullOrWhiteSpace(path))
                {
                    try
                    {
                        _importer.ImportBudgetFromJson(path);

                        LoadTeams();

                        MessageBox.Show("Import sikeres.", "OK");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Import hiba: {ex.Message}", "Hiba");
                    }
                }
            }

        }


        public ObservableCollection<Team> Teams { get; set; } = new();
        public ICommand LoadTeamsCommand { get; }
        public ICommand SearchTeamsCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand CreateTeamCommand { get; }


        public string? SearchName { get; set; }
        public string? SearchHQ { get; set; }
        public string? SearchPrincipal { get; set; }
        public int? SearchWins { get; set; }
        public int? SearchYear { get; set; }
        public bool SearchExact { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? NewTeamPrincipal { get; set; }
        public string? NewTeamName { get; set; }
        public string? NewTeamHq { get; set; }

        public string PageDisplay => $"Page {Page}";

        private void CreateTeam()
        {
            if (string.IsNullOrWhiteSpace(NewTeamPrincipal) ||
                string.IsNullOrWhiteSpace(NewTeamName) ||
                string.IsNullOrWhiteSpace(NewTeamHq))
            {
                MessageBox.Show("Minden mezőt tölts ki (Principal, Name, HQ).", "Create Team",MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var team = new Team
                {
                    TeamPrincipal = NewTeamPrincipal!,
                    TeamName = NewTeamName!,
                    Headquarters = NewTeamHq!
                };

                _teamService.CreateTeam(team);

                MessageBox.Show("Sikeres létrehozás.", "OK");
                Teams.Clear();
                foreach (var t in _teamService.GetAllTeams()) Teams.Add(t);

                NewTeamPrincipal = NewTeamName = NewTeamHq = string.Empty;
                OnPropertyChanged(nameof(NewTeamPrincipal));
                OnPropertyChanged(nameof(NewTeamName));
                OnPropertyChanged(nameof(NewTeamHq));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba létrehozáskor: {ex.Message}", "Error");
            }
        }

        private void LoadTeams()
        {
            Teams.Clear();
            foreach (var t in _teamService.GetAllTeams())
            {
                Teams.Add(t);
            }
            MessageBox.Show($"Betöltve: {Teams.Count} csapat.", "Teams");
        }

        private void SearchTeams()
        {
            Teams.Clear();
            var result = _teamService.SearchTeams(
                SearchName, SearchHQ, SearchPrincipal,
                SearchWins, SearchYear, SearchExact,
                Page, PageSize);
            foreach (var t in result)
            {
                Teams.Add(t);
            }
        }

        private void NextPage() { Page++; SearchTeams(); }
        private void PrevPage() { if (Page > 1) { Page--; SearchTeams(); } }

        public ObservableCollection<AnnualBudget> Budgets { get; set; } = new();
        public ICommand LoadBudgetsCommand { get; }
        public ICommand UpdateAnnualBudgetCommand { get; }

        private AnnualBudget? _selectedBudget;
        public AnnualBudget? SelectedBudget { get => _selectedBudget; set { _selectedBudget = value; OnPropertyChanged(); } }

        public string? UpdateTeamName { get; set; }
        public int? UpdateYear { get; set; }

        private void LoadBudgets()
        {
            Budgets.Clear();
            foreach (var b in _budgetService.GetAllAnnualBudgets())
                Budgets.Add(b);
            MessageBox.Show($"Betöltve: {Budgets.Count} költségvetés.", "Budgets");
        }

        private void UpdateAnnualBudget()
        {
            if (SelectedBudget == null)
            {
                MessageBox.Show("Válassz ki egy költségvetést!", "Figyelem");
                return;
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(UpdateTeamName))
                    SelectedBudget.Team.TeamName = UpdateTeamName;
                if (UpdateYear.HasValue)
                    SelectedBudget.Year = UpdateYear.Value;

                _budgetService.UpdateAnnualBudget(SelectedBudget);
                MessageBox.Show("Annual budget updated.", "OK");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a frissítésnél: {ex.Message}", "Error");
            }
        }

        public ObservableCollection<BudgetItem> Items { get; set; } = new();
        public ICommand LoadItemsCommand { get; }
        public ICommand CreateItemCommand { get; }
        public ICommand DeleteSelectedItemCommand { get; }

        private BudgetItem? _selectedItem;
        public BudgetItem? SelectedItem { get => _selectedItem; set { _selectedItem = value; OnPropertyChanged(); } }

        private string? _newItemCategory;
        public string? NewItemCategory { get => _newItemCategory; set { _newItemCategory = value; OnPropertyChanged(); } }

        private decimal? _newItemAmount;
        public decimal? NewItemAmount { get => _newItemAmount; set { _newItemAmount = value; OnPropertyChanged(); } }

        private void LoadItems()
        {
            Items.Clear();
            foreach (var i in _itemService.GetAllBudgetItems())
                Items.Add(i);
            MessageBox.Show($"Betöltve: {Items.Count} tétel.", "Items");
        }

        private void CreateBudgetItem()
        {
            if (string.IsNullOrWhiteSpace(NewItemCategory) || !NewItemAmount.HasValue)
            {
                MessageBox.Show("Töltsd ki a Category és Amount mezőket!", "Hiányos adat");
                return;
            }

            try
            {
                var item = new BudgetItem { Category = NewItemCategory!, Amount = NewItemAmount.Value };
                _itemService.CreateBudgetItem(item);
                MessageBox.Show("Item created successfully.", "OK");
                LoadItems();
                NewItemCategory = null;
                NewItemAmount = null;
                OnPropertyChanged(nameof(NewItemCategory));
                OnPropertyChanged(nameof(NewItemAmount));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba létrehozáskor: {ex.Message}", "Error");
            }
        }

        private void DeleteBudgetItem()
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("Válassz ki egy tételt!", "Figyelem");
                return;
            }

            try
            {
                _itemService.DeleteBudgetItem(SelectedItem.Id);
                MessageBox.Show("Sikeres törlés.", "OK");
                LoadItems();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba törléskor: {ex.Message}", "Error");
            }
        }

        public string? PredictionTeamName { get; set; }
        public decimal? PlannedBudget { get; set; }
        public string? CategoryTeamName { get; set; }
        public int? CategoryYear { get; set; }

        public ICommand GeneratePredictionReportCommand { get; }
        public ICommand GenerateCategoryReportCommand { get; }

        private void GeneratePredictionReport()
        {
            if (string.IsNullOrWhiteSpace(PredictionTeamName) || !PlannedBudget.HasValue)
            {
                MessageBox.Show("Add meg a Team Name és Planned Budget értékeit!", "Hiányos adat");
                return;
            }

            try
            {
                _budgetService.GenerateBudgetPredictionReport(PredictionTeamName, PlannedBudget.Value);
                MessageBox.Show("Sikeres jelentés létrehozás.", "OK");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Report hiba: {ex.Message}", "Error");
            }
        }

        private void GenerateCategoryReport()
        {
            if (string.IsNullOrWhiteSpace(CategoryTeamName) || !CategoryYear.HasValue)
            {
                MessageBox.Show("Add meg a Team Name és Year mezőket!", "Hiányos adat");
                return;
            }

            try
            {
                _budgetService.GenerateCategoryReportForTeamAndYear(CategoryTeamName, CategoryYear.Value);
                MessageBox.Show("Sikeres Generálás.", "OK");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Report hiba: {ex.Message}", "Error");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object? parameter) => _execute();
        public event EventHandler? CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _executeAsync;
        private readonly Func<bool>? _canExecute;
        private bool _isExecuting;
        public AsyncRelayCommand(Func<Task> executeAsync, Func<bool>? canExecute = null)
        {
            _executeAsync = executeAsync;
            _canExecute = canExecute;
        }
        public bool CanExecute(object? parameter) => !_isExecuting && (_canExecute?.Invoke() ?? true);
        public async void Execute(object? parameter)
        {
            _isExecuting = true;
            RaiseCanExecuteChanged();
            try { await _executeAsync(); }
            finally { _isExecuting = false; RaiseCanExecuteChanged(); }
        }
        public event EventHandler? CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
   

}
