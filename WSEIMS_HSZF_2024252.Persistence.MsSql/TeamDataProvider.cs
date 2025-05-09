using System;
using System.Linq;
using WSEIMS_HSZF_2024252.Model;

namespace WSEIMS_HSZF_2024252.Persistence.MsSql
{
    public interface ITeamDataProvider
    {
        TeamEntity? GetTeamByNameAndYear(string teamName, int year);
        void AddTeam(TeamEntity team);
    }

    public class TeamDataProvider : ITeamDataProvider
    {
        private readonly FormulaOneDbContext ctx;

        public TeamDataProvider(FormulaOneDbContext ctx)
        {
            this.ctx = ctx;
        }

        public TeamEntity? GetTeamByNameAndYear(string teamName, int year)
        {
            return ctx.Teams.FirstOrDefault(t => t.teamName == teamName && t.year == year);
        }

        public void AddTeam(TeamEntity team)
        {
            ctx.Teams.Add(team);
            ctx.SaveChanges(); // Az adatbázisba mentés itt történik meg
        }
    }
}
