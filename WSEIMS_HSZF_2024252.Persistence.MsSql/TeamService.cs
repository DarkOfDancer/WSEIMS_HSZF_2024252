using WSEIMS_HSZF_2024252.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WSEIMS_HSZF_2024252.Persistence.MsSql;

namespace WSEIMS_HSZF_2024252.Application
{
    public class TeamService
    {
        private readonly FormulaOneDbContext _context;

        public TeamService()
        {
            _context = new FormulaOneDbContext();
        }

        public List<TeamEntity> GetTeamsPaged(int page, int size)
        {
            return _context.Teams
                .OrderBy(t => t.year)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();
        }

        public List<TeamEntity> Search(string field, string value, bool exact)
        {
            var query = _context.Teams.AsQueryable();

            if (string.IsNullOrEmpty(value)) return new List<TeamEntity>();

            switch (field.ToLower())
            {
                case "name":
                    query = exact ? query.Where(t => t.teamName == value) : query.Where(t => t.teamName.Contains(value));
                    break;
                case "year":
                    if (int.TryParse(value, out var year))
                        query = query.Where(t => t.year == year);
                    break;
                case "hq":
                    query = exact ? query.Where(t => t.headquarters == value) : query.Where(t => t.headquarters.Contains(value));
                    break;
                case "principal":
                    query = exact ? query.Where(t => t.teamPrincipal == value) : query.Where(t => t.teamPrincipal.Contains(value));
                    break;
                case "titles":
                    if (int.TryParse(value, out var titles))
                        query = query.Where(t => t.constructorsChampionshipWins == titles);
                    break;
            }

            return query.ToList();
        }

        public bool Delete(string id)
        {
            var team = _context.Teams.Find(id);
            if (team == null) return false;

            _context.Teams.Remove(team);
            _context.SaveChanges();
            return true;
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
    }

}
