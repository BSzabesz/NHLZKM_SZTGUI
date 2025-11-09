using NHLZKM_SZTGUI.Model;
using NHLZKM_SZTGUI.Persistence.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHLZKM_SZTGUI.Application
{
    public interface IAnnualBudgetService
    {
        void CreateAnnualBudgetFromFile(AnnualBudget annualBudget);
        void DeleteAnnualBudget(int id);
        void UpdateAnnualBudget(AnnualBudget annualBudget);
        IEnumerable<AnnualBudget> GetAllAnnualBudgets();
        AnnualBudget GetAnnualBudgetById(int id);
        public void GenerateCategoryReportForTeamAndYear(string teamName, int year);
        public void GenerateBudgetPredictionReport(string teamName, decimal plannedBudget);
        void ImportBudgetFromFile(string filePath);

    }



    public class AnnualBudgetService : IAnnualBudgetService
    {
        private readonly IAnnualBudgetDataProvider annualBudgetDataProvider;
        private readonly IBudgetItemDataProvider budgetItemDataProvider;

        public AnnualBudgetService(IAnnualBudgetDataProvider annualBudgetDataProvider)
        {
            this.annualBudgetDataProvider = annualBudgetDataProvider;
        }
        public void ImportBudgetFromFile(string filePath)
        {
            annualBudgetDataProvider.ImportBudgetFromFile(filePath);
        }

        public void CreateAnnualBudgetFromFile(AnnualBudget incomingBudget)
        {
            if (incomingBudget == null || string.IsNullOrEmpty(incomingBudget.Team.TeamName))
                throw new Exception("Invalid budget data.");

            var existing = annualBudgetDataProvider.GetBudget(incomingBudget.Id);

            if (existing == null)
            {
                annualBudgetDataProvider.AddBudget(incomingBudget);
            }
            else
            {
                foreach (var item in incomingBudget.BudgetItems)
                {
                    item.AnnualBudgetId = existing.Id;
                    budgetItemDataProvider.AddItem(item);
                }
            }
        }



        public void DeleteAnnualBudget(int id)
        {
            annualBudgetDataProvider.DeleteBudget(id);
        }

        public void UpdateAnnualBudget(AnnualBudget annualBudget)
        {
            annualBudgetDataProvider.UpdateBudget(annualBudget);
        }

        public IEnumerable<AnnualBudget> GetAllAnnualBudgets()
        {
            return annualBudgetDataProvider.GetAllBudgets();
        }

        public AnnualBudget GetAnnualBudgetById(int id)
        {
            return annualBudgetDataProvider.GetBudget(id);
        }

        public void GenerateBudgetPredictionReport(string teamName, decimal plannedBudget)
        {
            if (string.IsNullOrEmpty(teamName))
                throw new ArgumentException("Team name is required.");
            if (plannedBudget <= 0)
                throw new ArgumentException("Planned budget must be positive.");

            var budgets = annualBudgetDataProvider
                .GetBudgetsForTeam(teamName)
                .Where(b => b.Year >= DateTime.Now.Year - 2)
                .ToList();

            if (budgets.Count == 0)
                throw new Exception("No data found for the past two years.");

            decimal totalCar = 0;
            decimal totalPeople = 0;
            decimal totalOperations = 0;
            decimal totalOverall = 0;

            foreach (var budget in budgets)
            {
                foreach (var item in budget.BudgetItems)
                {
                    switch (item.Category.ToLower())
                    {
                        case "car":
                            totalCar += item.Amount;
                            break;
                        case "people":
                            totalPeople += item.Amount;
                            break;
                        case "operations":
                            totalOperations += item.Amount;
                            break;
                    }
                    totalOverall += item.Amount;
                }
            }

            if (totalOverall == 0)
                throw new Exception("Total historical budget is zero, cannot calculate ratios.");

            var carRatio = totalCar / totalOverall;
            var peopleRatio = totalPeople / totalOverall;
            var operationsRatio = totalOperations / totalOverall;

            Random rnd = new Random();
            decimal variation = 0.1m;

            string report = $"Budget Prediction Report for {teamName}\n";
            report += $"Planned Budget: {plannedBudget}\n\n";

            report += $"Category\tMin\t\tMax\n";

            decimal carMin = plannedBudget * carRatio * (1 - variation);
            decimal carMax = plannedBudget * carRatio * (1 + variation);
            decimal peopleMin = plannedBudget * peopleRatio * (1 - variation);
            decimal peopleMax = plannedBudget * peopleRatio * (1 + variation);
            decimal operationsMin = plannedBudget * operationsRatio * (1 - variation);
            decimal operationsMax = plannedBudget * operationsRatio * (1 + variation);

            report += $"Car\t\t{carMin:F2}\t{carMax:F2}\n";
            report += $"People\t\t{peopleMin:F2}\t{peopleMax:F2}\n";
            report += $"Operations\t{operationsMin:F2}\t{operationsMax:F2}\n";

            string directory = Path.Combine("Reports", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            Directory.CreateDirectory(directory);

            string filePath = Path.Combine(directory, $"{teamName}_BudgetPrediction.txt");
            File.WriteAllText(filePath, report);
        }
        public void GenerateCategoryReportForTeamAndYear(string teamName, int year)
        {
            if (string.IsNullOrEmpty(teamName))
                throw new ArgumentException("Team name is required.");

            var teamBudgets = annualBudgetDataProvider.GetBudgetsForTeam(teamName);

            var targetBudget = teamBudgets.FirstOrDefault(b => b.Year == year);

            if (targetBudget == null)
                throw new Exception($"No budget found for team '{teamName}' in year {year}.");

            var grouped = targetBudget.BudgetItems
                                      .GroupBy(item => item.Category)
                                      .Select(group => new
                                      {
                                          Category = group.Key,
                                          Total = group.Sum(item => item.Amount)
                                      })
                                      .ToList();

            string report = $"Category Budget Report for {teamName} - Year: {year}\n";
            report += "-------------------------------------------------------\n";

            foreach (var entry in grouped)
            {
                report += $"{entry.Category}: {entry.Total:F2}\n";
            }

            string directory = Path.Combine("Reports", "CategoryReports");
            Directory.CreateDirectory(directory);

            string fileName = $"{teamName}_{year}_CategoryReport.txt";
            string filePath = Path.Combine(directory, fileName);

            File.WriteAllText(filePath, report);
        }





    }

}
