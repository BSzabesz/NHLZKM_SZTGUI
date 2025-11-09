using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHLZKM_SZTGUI.Model
{
    public class Team
    {
        [Key]
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string Headquarters { get; set; }
        public string TeamPrincipal { get; set; }
        public int ConstructorsChampionshipWins { get; set; }

        public ICollection<AnnualBudget> AnnualBudgets { get; set; }
    }


}
