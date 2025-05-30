using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using WSEIMS_HSZF_2024252.Model;

namespace WSEIMS_HSZF_2024252.Persistence.MsSql
{
    public interface ITeamDataProvider
    {
        List<TeamEntity> GetAll();
        TeamEntity GetById(string id);
        bool Update(TeamEntity team);
        bool Delete(string id);
        bool Save(TeamEntity team);
        FormulaOneDbContext Context();
    }


    public class TeamDataProvider : ITeamDataProvider
    {
        public readonly FormulaOneDbContext _context;

        public TeamDataProvider(FormulaOneDbContext context)
        {
            this._context = context;
        }
        public FormulaOneDbContext Context()
        {
            return _context;
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
        public bool Delete(string id)
        {
            var team = _context.Teams
                .Include(t => t.budget)
                .ThenInclude(b => b.expenses)
                .ThenInclude(e => e.subcategory)
                .FirstOrDefault(t => t.Id == id);

            if (team == null) return false;

            // Töröljük a kapcsolt subcategory-kat
            foreach (var exp in team.budget?.expenses ?? new List<ExpensEntity>())
            {
                if (exp.subcategory != null)
                    _context.Subcategories.RemoveRange(exp.subcategory);
            }

            // Töröljük a kapcsolt költségeket
            _context.Expenses.RemoveRange(team.budget?.expenses ?? new List<ExpensEntity>());

            // Töröljük a költségvetést, ha van
            if (team.budget != null)
                _context.Budgets.Remove(team.budget);

            // Töröljük a csapatot
            _context.Teams.Remove(team);

            _context.SaveChanges();
            return true;
        }
        public bool Save(TeamEntity team)
        {
            if (team == null || string.IsNullOrWhiteSpace(team.Id))
                return false;

            var exists = _context.Teams.Any(t => t.Id == team.Id);
            if (exists)
                return false;

            _context.Teams.Add(team);
            _context.SaveChanges();
            return true;
        }
        public List<TeamEntity> GetAll()
        {
            return _context.Teams.ToList();
        }
    }
}
