using System;
using System.Linq;
using WSEIMS_HSZF_2024252.Model;

namespace WSEIMS_HSZF_2024252.Persistence.MsSql
{
    public interface ITeamDataProvider
    {

        FormulaOneDbContext Context();
    }

    public class TeamDataProvider : ITeamDataProvider
    {
        public readonly FormulaOneDbContext ctx;

        public TeamDataProvider(FormulaOneDbContext ctx)
        {
            this.ctx = ctx;
        }


        public FormulaOneDbContext Context()
        {
            return ctx; 
        }
    }
}
