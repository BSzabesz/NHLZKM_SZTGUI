using NHLZKM_SZTGUI.Model;
using NHLZKM_SZTGUI.Persistence.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHLZKM_SZTGUI.Application
{
    public interface ITeamService
    {
        void CreateTeam(Team team);
        void DeleteTeam(string name);
        void UpdateTeam(Team team);
        IEnumerable<Team> GetAllTeams();
        IEnumerable<Team> SearchTeams(
            string teamName,
            string headquarters,
            string teamPrincipal,
            int? constructorsChampionships,
            int? year,
            bool exactMatch = false,
            int page = 1,
            int pageSize = 10);
    }

    public class TeamService : ITeamService
    {
        private readonly ITeamDataProvider teamDataProvider;

        public TeamService(ITeamDataProvider teamDataProvider)
        {
            this.teamDataProvider = teamDataProvider;
        }

        public void CreateTeam(Team team)
        {
            if (string.IsNullOrEmpty(team.TeamName))
                throw new Exception("Team name is required.");

            teamDataProvider.AddTeam(team);
        }

        public void DeleteTeam(string name)
        {
            teamDataProvider.DeleteTeam(name);
        }

        public void UpdateTeam(Team team)
        {
            teamDataProvider.UpdateTeam(team);
        }

        public IEnumerable<Team> GetAllTeams()
        {
            return teamDataProvider.GetAllTeams();
        }

        public IEnumerable<Team> SearchTeams(
            string teamName,
            string headquarters,
            string teamPrincipal,
            int? constructorsChampionships,
            int? year,
            bool exactMatch = false,
            int page = 1,
            int pageSize = 10)
        {
            var teams = teamDataProvider.GetAllTeams().AsQueryable();

            if (!string.IsNullOrEmpty(teamName))
            {
                teams = exactMatch
                    ? teams.Where(t => t.TeamName == teamName)
                    : teams.Where(t => t.TeamName.Contains(teamName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(headquarters))
            {
                teams = exactMatch
                    ? teams.Where(t => t.Headquarters == headquarters)
                    : teams.Where(t => t.Headquarters.Contains(headquarters, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(teamPrincipal))
            {
                teams = exactMatch
                    ? teams.Where(t => t.TeamPrincipal == teamPrincipal)
                    : teams.Where(t => t.TeamPrincipal.Contains(teamPrincipal, StringComparison.OrdinalIgnoreCase));
            }

            if (constructorsChampionships.HasValue)
            {
                teams = teams.Where(t => t.ConstructorsChampionshipWins == constructorsChampionships.Value);
            }

            if (year.HasValue)
            {
                teams = teams.Where(t => t.AnnualBudgets.Any(b => b.Year == year.Value));
            }

            return teams
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }
}
