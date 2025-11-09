using Microsoft.EntityFrameworkCore;
using NHLZKM_SZTGUI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHLZKM_SZTGUI.Persistence.MsSql
{
    public interface IBudgetItemDataProvider
    {
        IEnumerable<BudgetItem> GetAllItems();
        IEnumerable<BudgetItem> GetItemsByBudget(string teamName, int year);
        BudgetItem GetItemById(int id);
        void AddItem(BudgetItem item);
        void UpdateItem(BudgetItem item);
        void DeleteItem(int id);
    }


    public class BudgetItemDataProvider : IBudgetItemDataProvider
    {
        private readonly FormulaOneDbContext context;

        public BudgetItemDataProvider(FormulaOneDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<BudgetItem> GetAllItems()
        {
            return context.BudgetItems
                          .Include(bi => bi.AnnualBudget)
                          .Include(bi => bi.Subcategories)
                          .ToList();
        }

        public IEnumerable<BudgetItem> GetItemsByBudget(string teamName, int year)
        {
            return context.BudgetItems
                          .Include(bi => bi.AnnualBudget)
                              .ThenInclude(ab => ab.Team)
                          .Include(bi => bi.Subcategories)
                          .Where(bi => bi.AnnualBudget.Team.TeamName == teamName &&
                                       bi.AnnualBudget.Year == year)
                          .ToList();
        }

        public BudgetItem GetItemById(int id)
        {
            return context.BudgetItems
                          .Include(bi => bi.Subcategories)
                          .FirstOrDefault(bi => bi.Id == id);
        }

        public void AddItem(BudgetItem item)
        {
            context.BudgetItems.Add(item);
            context.SaveChanges();
        }

        public void UpdateItem(BudgetItem item)
        {
            var existing = context.BudgetItems
                                  .Include(bi => bi.Subcategories)
                                  .FirstOrDefault(bi => bi.Id == item.Id);
            if (existing != null)
            {
                existing.Category = item.Category;
                existing.Amount = item.Amount;
                existing.ExpenseDate = item.ExpenseDate;
                existing.ApprovalStatus = item.ApprovalStatus;
                context.SaveChanges();
            }
        }

        public void DeleteItem(int id)
        {
            var item = context.BudgetItems.FirstOrDefault(bi => bi.Id == id);
            if (item != null)
            {
                context.BudgetItems.Remove(item);
                context.SaveChanges();
            }
        }
    }


}
