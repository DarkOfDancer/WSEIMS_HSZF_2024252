using WSEIMS_HSZF_2024252.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WSEIMS_HSZF_2024252.Persistence.MsSql;

namespace WSEIMS_HSZF_2024252.Application
{
    public class TeamService
    {
        public List<TeamEntity> GetTeamsPaged(int page, int pageSize)
        {
            using var context = new FormulaOneDbContext();
            return context.Teams
                .Include(t => t.budget)
                .ThenInclude(b => b.expenses)
                .OrderBy(t => t.year)
                .ThenBy(t => t.teamName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public List<TeamEntity> SearchTeams(string field, string value, bool exactMatch)
        {
            using var context = new FormulaOneDbContext();
            var query = context.Teams.AsQueryable();

            switch (field)
            {
                case "name":
                    query = exactMatch ?
                        query.Where(t => t.teamName == value) :
                        query.Where(t => t.teamName.Contains(value));
                    break;

                case "year":
                    if (int.TryParse(value, out int year))
                        query = query.Where(t => t.year == year);
                    else return new List<TeamEntity>();
                    break;

                case "hq":
                    query = exactMatch ?
                        query.Where(t => t.headquarters == value) :
                        query.Where(t => t.headquarters.Contains(value));
                    break;

                case "principal":
                    query = exactMatch ?
                        query.Where(t => t.teamPrincipal == value) :
                        query.Where(t => t.teamPrincipal.Contains(value));
                    break;

                case "titles":
                    if (int.TryParse(value, out int titles))
                        query = query.Where(t => t.constructorsChampionshipWins == titles);
                    else return new List<TeamEntity>();
                    break;

                default:
                    return new List<TeamEntity>();
            }

            return query
                .Include(t => t.budget)
                .ThenInclude(b => b.expenses)
                .OrderBy(t => t.year)
                .ThenBy(t => t.teamName)
                .ToList();
        }

        public bool DeleteTeamById(string id)
        {
            using var context = new FormulaOneDbContext();
            var team = context.Teams.Find(id);
            if (team == null) return false;

            context.Teams.Remove(team);
            context.SaveChanges();
            return true;
        }

        public TeamEntity GetTeamById(string id)
        {
            using var context = new FormulaOneDbContext();
            return context.Teams
                .Include(t => t.budget)
                .ThenInclude(b => b.expenses)
                .FirstOrDefault(t => t.Id == id);
        }

        public bool UpdateTeam(TeamEntity updatedTeam)
        {
            using var context = new FormulaOneDbContext();
            var existingTeam = context.Teams
                .Include(t => t.budget)
                .ThenInclude(b => b.expenses)
                .FirstOrDefault(t => t.Id == updatedTeam.Id);

            if (existingTeam == null) return false;

            context.Entry(existingTeam).CurrentValues.SetValues(updatedTeam);

            if (updatedTeam.budget != null)
            {
                if (existingTeam.budget != null)
                {
                    context.Entry(existingTeam.budget).CurrentValues.SetValues(updatedTeam.budget);
                }
                else
                {
                    existingTeam.budget = updatedTeam.budget;
                }

                existingTeam.budget.expenses = updatedTeam.budget.expenses;
            }

            context.SaveChanges();
            return true;
        }
    }
}
