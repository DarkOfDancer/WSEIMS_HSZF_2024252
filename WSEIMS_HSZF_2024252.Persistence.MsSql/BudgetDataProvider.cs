using System;
using System.Linq;
using WSEIMS_HSZF_2024252.Model;

namespace WSEIMS_HSZF_2024252.Persistence.MsSql
{
    public interface IBudgetDataProvider
    {
        void AddBudget(BudgetEntity budget);
    }

    public class BudgetDataProvider : IBudgetDataProvider
    {
        private readonly FormulaOneDbContext ctx;

        public BudgetDataProvider(FormulaOneDbContext ctx)
        {
            this.ctx = ctx;
        }


        public void AddBudget(BudgetEntity budget)
        {
            ctx.Budgets.Add(budget);
            ctx.SaveChanges();
        }
    }
}
