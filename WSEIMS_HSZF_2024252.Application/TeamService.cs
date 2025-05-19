using WSEIMS_HSZF_2024252.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WSEIMS_HSZF_2024252.Persistence.MsSql;
using System.Globalization;
using WSEIMS_HSZF_2024252.Application;

namespace WSEIMS_HSZF_2024252.Application
{

        public interface ITeamService
        {
            List<TeamEntity> GetTeamsPaged(int page, int size);
            List<TeamEntity> Search(string field, string value, string searchType);
            bool Delete(string id);
            TeamEntity GetById(string id);
            bool Update(TeamEntity team);
            List<TeamEntity> ImportFromDirectory(string path);
        }
    public class TeamService : ITeamService
    {
        private readonly ITeamDataProvider _context;

        public TeamService(ITeamDataProvider context)
        {
            _context = context;
        }

        public List<TeamEntity> GetTeamsPaged(int page, int size)
        {
            return _context.Context().Teams
                .OrderBy(t => t.year)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();
        }

        public List<TeamEntity> Search(string field, string value, string searchType)
        {
            var query = _context.Context().Teams.AsQueryable();

            if (string.IsNullOrEmpty(value)) return new List<TeamEntity>();

            switch (field.ToLower())
            {
                case "name":
                    query = searchType == "e"
                        ? query.Where(t => t.teamName.ToLower() == value.ToLower())
                        : query.Where(t => t.teamName.ToLower().Contains(value.ToLower()));
                    break;

                case "year":
                    if (int.TryParse(value, out var year))
                        query = query.Where(t => t.year == year);
                    break;

                case "hq":
                    query = searchType == "e"
                        ? query.Where(t => t.headquarters.ToLower() == value.ToLower())
                        : query.Where(t => t.headquarters.ToLower().Contains(value.ToLower()));
                    break;

                case "principal":
                    query = searchType == "e"
                        ? query.Where(t => t.teamPrincipal.ToLower() == value.ToLower())
                        : query.Where(t => t.teamPrincipal.ToLower().Contains(value.ToLower()));
                    break;

                case "titles":
                    if (int.TryParse(value, out var titles))
                        query = query.Where(t => t.constructorsChampionshipWins == titles);
                    break;
            }

            return query.ToList();
        }

        public bool Delete(string id)
        {
            var team = _context.Context().Teams
                .Include(t => t.budget)
                .ThenInclude(b => b.expenses)
                .ThenInclude(e => e.subcategory)
                .FirstOrDefault(t => t.Id == id);

            if (team == null) return false;

            foreach (var exp in team.budget?.expenses ?? new List<ExpensEntity>())
            {
                if (exp.subcategory != null)
                    _context.Context().Subcategories.RemoveRange(exp.subcategory);
            }

            _context.Context().Expenses.RemoveRange(team.budget?.expenses ?? new List<ExpensEntity>());
            if (team.budget != null)
                _context.Context().Remove(team.budget);

            _context.Context().Remove(team);
            _context.Context().SaveChanges();
            return true;
        }

        public TeamEntity GetById(string id)
        {
            return _context.Context().Teams.FirstOrDefault(t => t.Id == id);
        }

        public bool Update(TeamEntity team)
        {
            var existing = _context.Context().Teams.Find(team.Id);
            if (existing == null) return false;

            _context.Context().Entry(existing).CurrentValues.SetValues(team);
            _context.Context().SaveChanges();
            return true;
        }

        public List<TeamEntity> ImportFromDirectory(string path)
        {
            var importer = new JsonImporter();
            return importer.ImportTeamsFromNEWDirectory(path);
        }

       

    }

}
