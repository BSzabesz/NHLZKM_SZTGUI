using Microsoft.EntityFrameworkCore;
using NHLZKM_SZTGUI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace NHLZKM_SZTGUI.Persistence.MsSql
{
    public interface IAnnualBudgetDataProvider
    {
        IEnumerable<AnnualBudget> GetAllBudgets();
        AnnualBudget GetBudget(int id);
        void AddBudget(AnnualBudget budget);
        void UpdateBudget(AnnualBudget budget);
        void DeleteBudget(int id);
        IEnumerable<AnnualBudget> GetBudgetsForTeam(string teamName);
        void ImportBudgetFromFile(string filePath);
    }

    public class AnnualBudgetDataProvider : IAnnualBudgetDataProvider
    {
        private readonly FormulaOneDbContext context;

        public AnnualBudgetDataProvider(FormulaOneDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<AnnualBudget> GetAllBudgets()
        {
            return context.AnnualBudgets
                          .Include(b => b.Team)
                          .Include(b => b.BudgetItems)
                              .ThenInclude(bi => bi.Subcategories)
                          .ToList();
        }

        public AnnualBudget GetBudget(int id)
        {
            return context.AnnualBudgets
                          .Include(b => b.Team)
                          .Include(b => b.BudgetItems)
                              .ThenInclude(bi => bi.Subcategories)
                          .FirstOrDefault(b => b.Id == id);
        }

        public void AddBudget(AnnualBudget budget)
        {
            context.AnnualBudgets.Add(budget);
            context.SaveChanges();
        }
        public void ImportBudgetFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("The specified file cannot be found.", filePath);

            var json = File.ReadAllText(filePath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            };

            var deserialized = JsonSerializer.Deserialize<AnnualBudget>(json, options);

            if (deserialized == null)
                throw new Exception("The JSON does not contain valid budget data.");

            if (deserialized.Team == null || string.IsNullOrWhiteSpace(deserialized.Team.TeamName))
                throw new Exception("Team name is missing.");

            if (deserialized.BudgetItems == null || deserialized.BudgetItems.Count == 0)
                throw new Exception("Budget items are missing.");

            var existingTeam = context.Teams.FirstOrDefault(t => t.TeamName == deserialized.Team.TeamName);
            if (existingTeam != null)
            {
                deserialized.TeamId = existingTeam.Id;
                deserialized.Team = existingTeam;
            }

            foreach (var item in deserialized.BudgetItems)
            {
                item.AnnualBudget = deserialized;

                if (item.Subcategories != null)
                {
                    foreach (var sub in item.Subcategories)
                    {
                        sub.BudgetItem = item;
                    }
                }
                else
                {
                    item.Subcategories = new List<SubcategoryItem>();
                }
            }

            context.AnnualBudgets.Add(deserialized);
            context.SaveChanges();
        }

        public void UpdateBudget(AnnualBudget budget)
        {
            var existing = context.AnnualBudgets
                                  .FirstOrDefault(b => b.Id == budget.Id);

            if (existing != null)
            {
                existing.Year = budget.Year;
                existing.TotalBudget = budget.TotalBudget;
                existing.TeamId = budget.TeamId;
                context.SaveChanges();
            }
        }

        public void DeleteBudget(int id)
        {
            var budget = context.AnnualBudgets.FirstOrDefault(b => b.Id == id);
            if (budget != null)
            {
                context.AnnualBudgets.Remove(budget);
                context.SaveChanges();
            }
        }

        public IEnumerable<AnnualBudget> GetBudgetsForTeam(string teamName)
        {
            return context.AnnualBudgets
                          .Include(b => b.Team)
                          .Include(b => b.BudgetItems)
                              .ThenInclude(bi => bi.Subcategories)
                          .Where(b => b.Team.TeamName == teamName)
                          .ToList();
        }
    }



}



