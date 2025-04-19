using WSEIMS_HSZF_2024252.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WSEIMS_HSZF_2024252.Persistence.MsSql;

namespace WSEIMS_HSZF_2024252.Application
{
    public class TeamService
    {
        private readonly FormulaOneDbContext _context;

        public TeamService()
        {
            _context = new FormulaOneDbContext();
        }

        public List<TeamEntity> GetTeamsPaged(int page, int size)
        {
            return _context.Teams
                .OrderBy(t => t.year)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();
        }

        public List<TeamEntity> Search(string field, string value)
        {
            var query = _context.Teams.AsQueryable();

            if (string.IsNullOrEmpty(value)) return new List<TeamEntity>();

            switch (field.ToLower())
            {
                case "name":
                    query =  query.Where(t => t.teamName.Contains(value));
                    break;
                case "year":
                    if (int.TryParse(value, out var year))
                        query = query.Where(t => t.year == year);
                    break;
                case "hq":
                    query = query.Where(t => t.headquarters.Contains(value));
                    break;
                case "principal":
                    query = query.Where(t => t.teamPrincipal.Contains(value));
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
            var team = _context.Teams
                .Include(t => t.budget)
                    .ThenInclude(b => b.expenses)
                        .ThenInclude(e => e.subcategory)
                .FirstOrDefault(t => t.Id == id);

            if (team == null) return false;

            // Töröljük először a subcategory-ket
            foreach (var exp in team.budget?.expenses ?? new List<ExpensEntity>())
            {
                if (exp.subcategory != null)
                    _context.Subcategories.RemoveRange(exp.subcategory);
            }

            // Töröljük az expense-eket
            _context.Expenses.RemoveRange(team.budget?.expenses ?? new List<ExpensEntity>());

            // Töröljük a budget-et
            if (team.budget != null)
                _context.Budgets.Remove(team.budget);

            // Végül töröljük a csapatot
            _context.Teams.Remove(team);

            _context.SaveChanges();
            return true;
        }



        public TeamEntity GetById(string id)
        {
            return _context.Teams.FirstOrDefault(t => t.Id == id);
        }

        public bool Update(TeamEntity team)
        {
            var existing = _context.Teams.Find(team.Id);
            if (existing == null) return false;

            _context.Entry(existing).CurrentValues.SetValues(team);
            _context.SaveChanges();
            return true;
        }

        public List<TeamEntity> ImportFromDirectory(string path)
        {
            var importer = new JsonImporter();
            return importer.ImportTeamsFromNEWDirectory(path);
        }
    }

}
