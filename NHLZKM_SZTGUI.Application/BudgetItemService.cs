using NHLZKM_SZTGUI.Model;
using NHLZKM_SZTGUI.Persistence.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHLZKM_SZTGUI.Application
{
    public interface IBudgetItemService
    {
        void CreateBudgetItem(BudgetItem item);
        void DeleteBudgetItem(int id);
        void UpdateBudgetItem(BudgetItem item);
        IEnumerable<BudgetItem> GetAllBudgetItems();
        BudgetItem GetBudgetItemById(int id);
    }

    public class BudgetItemService : IBudgetItemService
    {
        private readonly IBudgetItemDataProvider budgetItemDataProvider;

        public BudgetItemService(IBudgetItemDataProvider budgetItemDataProvider)
        {
            this.budgetItemDataProvider = budgetItemDataProvider;
        }

        public void CreateBudgetItem(BudgetItem item)
        {
            if (string.IsNullOrEmpty(item.Category))
                throw new Exception("Category is required for budget item.");

            budgetItemDataProvider.AddItem(item);
        }

        public void DeleteBudgetItem(int id)
        {
            budgetItemDataProvider.DeleteItem(id);
        }

        public void UpdateBudgetItem(BudgetItem item)
        {
            budgetItemDataProvider.UpdateItem(item);
        }

        public IEnumerable<BudgetItem> GetAllBudgetItems()
        {
            return budgetItemDataProvider.GetAllItems();
        }

        public BudgetItem GetBudgetItemById(int id)
        {
            return budgetItemDataProvider.GetItemById(id);
        }
    }

}
