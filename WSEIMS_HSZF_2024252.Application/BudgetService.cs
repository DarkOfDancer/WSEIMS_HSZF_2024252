using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WSEIMS_HSZF_2024252.Model;
using WSEIMS_HSZF_2024252.Persistence.MsSql;

namespace WSEIMS_HSZF_2024252.Application
{
    public interface IBudgetService
    {
        List<BudgetEntity> GetAll();
        BudgetEntity GetById(string id);
        bool Add(BudgetEntity budget);
        bool Update(BudgetEntity budget);
        bool Delete(string id);
    }
    public class BudgetService : IBudgetService
    {
        private readonly FormulaOneDbContext _context;

        public BudgetService(FormulaOneDbContext context)
        {
            _context = context;
        }

        public List<BudgetEntity> GetAll()
        {
            return _context.Budgets
                .Include(b => b.expenses)
                .Include(b => b.TeamEntity)
                .ToList();
        }

        public BudgetEntity GetById(string id)
        {
            return _context.Budgets
                .Include(b => b.expenses)
                .Include(b => b.TeamEntity)
                .FirstOrDefault(b => b.Id == id);
        }

        public bool Add(BudgetEntity budget)
        {
            if (budget == null || string.IsNullOrWhiteSpace(budget.Id))
                return false;

            bool exists = _context.Budgets.Any(b => b.Id == budget.Id);
            if (exists)
                return false;

            _context.Budgets.Add(budget);
            _context.SaveChanges();
            return true;
        }

        public bool Update(BudgetEntity budget)
        {
            var existing = _context.Budgets.Find(budget.Id);
            if (existing == null)
                return false;

            _context.Entry(existing).CurrentValues.SetValues(budget);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(string id)
        {
            var budget = _context.Budgets
                .Include(b => b.expenses)
                .FirstOrDefault(b => b.Id == id);

            if (budget == null)
                return false;

            if (budget.expenses != null && budget.expenses.Any())
                _context.Expenses.RemoveRange(budget.expenses);

            _context.Budgets.Remove(budget);
            _context.SaveChanges();
            return true;
        }
    }
}
