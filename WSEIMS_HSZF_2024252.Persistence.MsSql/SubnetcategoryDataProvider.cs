using System;
using System.Linq;
using WSEIMS_HSZF_2024252.Model;

namespace WSEIMS_HSZF_2024252.Persistence.MsSql
{
    public interface ISubcategoryDataProvider
    {
        SubcategoryEntity? GetSubcategoryByNameAndAmount(string name, int amount);
        void AddSubcategory(SubcategoryEntity subcategory);
    }

    public class SubcategoryDataProvider : ISubcategoryDataProvider
    {
        private readonly FormulaOneDbContext ctx;

        public SubcategoryDataProvider(FormulaOneDbContext ctx)
        {
            this.ctx = ctx;
        }

        public SubcategoryEntity? GetSubcategoryByNameAndAmount(string name, int amount)
        {
            return ctx.Subcategories.FirstOrDefault(s => s.name == name && s.amount == amount);
        }

        public void AddSubcategory(SubcategoryEntity subcategory)
        {
            ctx.Subcategories.Add(subcategory);
            ctx.SaveChanges();
        }
    }
}
