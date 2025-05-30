using WSEIMS_HSZF_2024252.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WSEIMS_HSZF_2024252.Persistence.MsSql;
using System.Globalization;
using WSEIMS_HSZF_2024252.Application;

namespace WSEIMS_HSZF_2024252.Application
{

        public interface ITeamService
        {
            List<TeamEntity> GetTeamsPaged(int page, int size);
            List<TeamEntity> Search(string field, string value, string searchType);
            bool Delete(string id);
            TeamEntity GetById(string id);
            bool Update(TeamEntity team);
            List<TeamEntity> ImportFromDirectory(string path);
        }
    public class TeamService : ITeamService
    {
        protected readonly ITeamDataProvider dp;

        public TeamService(ITeamDataProvider dp)
        {
            this.dp = dp;
        }

        public List<TeamEntity> GetTeamsPaged(int page, int size)
        {
            var teams = dp.GetAll();
            return teams
                .OrderBy(t => t.year)
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();
        }
        public List<TeamEntity> Search(string field, string value, string searchType)
        {
            var teams = dp.GetAll();

            if (string.IsNullOrEmpty(value))
                return new List<TeamEntity>();

            value = value.ToLower();

            switch (field.ToLower())
            {
                case "name":
                    return searchType == "e"
                        ? teams.Where(t => t.teamName?.ToLower() == value).ToList()
                        : teams.Where(t => t.teamName?.ToLower().Contains(value) == true).ToList();

                case "year":
                    if (int.TryParse(value, out var year))
                        return teams.Where(t => t.year == year).ToList();
                    break;

                case "hq":
                    return searchType == "e"
                        ? teams.Where(t => t.headquarters?.ToLower() == value).ToList()
                        : teams.Where(t => t.headquarters?.ToLower().Contains(value) == true).ToList();

                case "principal":
                    return searchType == "e"
                        ? teams.Where(t => t.teamPrincipal?.ToLower() == value).ToList()
                        : teams.Where(t => t.teamPrincipal?.ToLower().Contains(value) == true).ToList();

                case "titles":
                    if (int.TryParse(value, out var titles))
                        return teams.Where(t => t.constructorsChampionshipWins == titles).ToList();
                    break;
            }

            return new List<TeamEntity>();
        }
        public bool Delete(string id)
        {
            var team = dp.GetById(id);
            if (team == null) return false;

            return dp.Delete(id);
        }

        public TeamEntity GetById(string id)
        {
            return dp.GetById(id);
        }

        public bool Update(TeamEntity team)
        {
            var existing = dp.GetById(team.Id);
            if (existing == null) return false;

            // Itt lehetne további validáció/üzleti logika is
            return dp.Update(team);
        }

        public List<TeamEntity> ImportFromDirectory(string path)
        {
            var importer = new JsonImporter(dp);
            var importedTeams = importer.ImportTeamsFromNEWDirectory(path);
            foreach (var team in importedTeams)
            {
                dp.Save(team);
            }

            return importedTeams;
        }


    }

}
