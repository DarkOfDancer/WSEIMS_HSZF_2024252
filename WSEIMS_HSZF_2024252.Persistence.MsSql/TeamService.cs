using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSEIMS_HSZF_2024252.Model;

namespace WSEIMS_HSZF_2024252.Persistence.MsSql
{
    public class TeamService
    {
        public List<TeamEntity> GetTeamsPaged(int pageNumber, int pageSize)
        {
            using var context = new FormulaOneDbContext();

            return context.Teams
                .OrderBy(t => t.year)
                .ThenBy(t => t.teamName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
    }

}
