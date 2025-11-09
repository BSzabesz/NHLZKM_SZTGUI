using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHLZKM_SZTGUI.Model
{
    public class BudgetItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Category { get; set; }
        public decimal Amount { get; set; }
        public string? ApprovalStatus { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public int? AnnualBudgetId { get; set; }
        public AnnualBudget AnnualBudget { get; set; }

        public ICollection<SubcategoryItem> Subcategories { get; set; }
    }

    public class SubcategoryItem
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public decimal Amount { get; set; }

        public int BudgetItemId { get; set; }
        public BudgetItem BudgetItem { get; set; }
    }

}
