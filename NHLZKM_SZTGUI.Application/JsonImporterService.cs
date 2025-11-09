using NHLZKM_SZTGUI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NHLZKM_SZTGUI.Application;

namespace NHLZKM_SZTGUI.Application
{
    public interface IJsonImporterService
    {
        void ImportBudgetFromJson(string filePath);
    }

    public class JsonImporterService : IJsonImporterService
    {
        private readonly ITeamService teamService;
        private readonly IAnnualBudgetService annualBudgetService;

        public JsonImporterService(ITeamService teamService, IAnnualBudgetService annualBudgetService)
        {
            this.teamService = teamService;
            this.annualBudgetService = annualBudgetService;
        }

        public void ImportBudgetFromJson(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("JSON file not found.", filePath);

            string json = File.ReadAllText(filePath);

            var dto = JsonConvert.DeserializeObject<BudgetImportDto>(json);

            if (dto == null)
                throw new Exception("Failed to deserialize JSON.");

            var team = new Team
            {
                TeamName = dto.TeamName,
                Headquarters = dto.Headquarters,
                TeamPrincipal = dto.TeamPrincipal,
                ConstructorsChampionshipWins = dto.ConstructorsChampionshipWins
            };
            teamService.CreateTeam(team);

            var annualBudget = new AnnualBudget
            {
                Year = dto.Year,
                Team = team,
                BudgetItems = new List<BudgetItem>()
            };

            foreach (var expense in dto.Budget.Expenses)
            {
                var item = new BudgetItem
                {
                    Category = expense.Category,
                    Amount = expense.Amount,
                    ApprovalStatus = expense.ApprovalStatus,
                    ExpenseDate = expense.ExpenseDate
                };
                annualBudget.BudgetItems.Add(item);
            }

            annualBudgetService.CreateAnnualBudgetFromFile(annualBudget);
        }
    }
    public class BudgetImportDto
    {
        public string TeamName { get; set; }
        public int Year { get; set; }
        public string Headquarters { get; set; }
        public string TeamPrincipal { get; set; }
        public int ConstructorsChampionshipWins { get; set; }
        public BudgetDto Budget { get; set; }
    }

    public class BudgetDto
    {
        public decimal TotalBudget { get; set; }
        public List<ExpenseDto> Expenses { get; set; }
    }

    public class ExpenseDto
    {
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public string ApprovalStatus { get; set; }
        public DateTime ExpenseDate { get; set; }
        public List<SubcategoryDto> Subcategory { get; set; }
    }

    public class SubcategoryDto
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }


}
