using Microsoft.EntityFrameworkCore;
using NHLZKM_SZTGUI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHLZKM_SZTGUI.Persistence.MsSql
{
    public interface ITeamDataProvider
    {
        IEnumerable<Team> GetAllTeams();
        Team GetTeamByName(string name);
        void AddTeam(Team team);
        void UpdateTeam(Team team);
        void DeleteTeam(string name);
    }

    public class TeamDataProvider : ITeamDataProvider
    {
        private readonly FormulaOneDbContext context;

        public TeamDataProvider(FormulaOneDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Team> GetAllTeams()
        {
            return context.Teams.Include(t => t.AnnualBudgets).ToList();
        }

        public Team GetTeamByName(string name)
        {
            return context.Teams
                          .Include(t => t.AnnualBudgets)
                          .FirstOrDefault(t => t.TeamName == name);
        }

        public void AddTeam(Team team)
        {
            context.Teams.Add(team);
            context.SaveChanges();
        }

        public void UpdateTeam(Team team)
        {
            var existing = context.Teams.FirstOrDefault(t => t.Id == team.Id);
            if (existing != null)
            {
                existing.TeamName = team.TeamName;
                existing.Headquarters = team.Headquarters;
                existing.TeamPrincipal = team.TeamPrincipal;
                existing.ConstructorsChampionshipWins = team.ConstructorsChampionshipWins;
                context.SaveChanges();
            }
        }

        public void DeleteTeam(string name)
        {
            var team = context.Teams.FirstOrDefault(t => t.TeamName == name);
            if (team != null)
            {
                context.Teams.Remove(team);
                context.SaveChanges();
            }
        }
    }


}
