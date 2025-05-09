using System;
using System.Linq;
using WSEIMS_HSZF_2024252.Model;

namespace WSEIMS_HSZF_2024252.Persistence.MsSql
{
    public interface IExpenseDataProvider
    {
        ExpensEntity? GetExpenseByCategoryAndAmount(string category, int amount);
        void AddExpense(ExpensEntity expense);
    }

    public class ExpenseDataProvider : IExpenseDataProvider
    {
        private readonly FormulaOneDbContext ctx;

        public ExpenseDataProvider(FormulaOneDbContext ctx)
        {
            this.ctx = ctx;
        }

        public ExpensEntity? GetExpenseByCategoryAndAmount(string category, int amount)
        {
            return ctx.Expenses.FirstOrDefault(e => e.category == category && e.amount == amount);
        }

        public void AddExpense(ExpensEntity expense)
        {
            ctx.Expenses.Add(expense);
            ctx.SaveChanges();
        }
    }
}
