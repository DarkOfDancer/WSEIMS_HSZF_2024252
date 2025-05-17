using System;
using System.Linq;
using WSEIMS_HSZF_2024252.Model;

namespace WSEIMS_HSZF_2024252.Persistence.MsSql
{
    public interface IExpenseDataProvider
    {
        void AddExpense(ExpensEntity expense);
    }

    public class ExpenseDataProvider : IExpenseDataProvider
    {
        private readonly FormulaOneDbContext ctx;

        public ExpenseDataProvider(FormulaOneDbContext ctx)
        {
            this.ctx = ctx;
        }



        public void AddExpense(ExpensEntity expense)
        {
            ctx.Expenses.Add(expense);
            ctx.SaveChanges();
        }
    }
}
