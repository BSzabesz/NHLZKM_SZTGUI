using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHLZKM_SZTGUI.Model
{
    public class AnnualBudget
    {
        [Key]
        public int Id { get; set; }
        public int Year { get; set; }
        public decimal TotalBudget { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; }

        public ICollection<BudgetItem> BudgetItems { get; set; }
    }


}
