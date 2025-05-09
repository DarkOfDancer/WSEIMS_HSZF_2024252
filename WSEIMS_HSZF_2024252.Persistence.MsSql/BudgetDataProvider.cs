using System;
using System.Linq;
using WSEIMS_HSZF_2024252.Model;

namespace WSEIMS_HSZF_2024252.Persistence.MsSql
{
    public interface IBudgetDataProvider
    {
        BudgetEntity? GetBudgetByTeamIdAndTotal(string teamId, int totalBudget);
        void AddBudget(BudgetEntity budget);
    }

    public class BudgetDataProvider : IBudgetDataProvider
    {
        private readonly FormulaOneDbContext ctx;

        public BudgetDataProvider(FormulaOneDbContext ctx)
        {
            this.ctx = ctx;
        }

        public BudgetEntity? GetBudgetByTeamIdAndTotal(string teamId, int totalBudget)
        {
            return ctx.Budgets.FirstOrDefault(b => b.TeamEntityId == teamId && b.totalBudget == totalBudget);
        }

        public void AddBudget(BudgetEntity budget)
        {
            ctx.Budgets.Add(budget);
            ctx.SaveChanges();
        }
    }
}
