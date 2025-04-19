using WSEIMS_HSZF_2024252.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WSEIMS_HSZF_2024252.Persistence.MsSql;
using System.Globalization;

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

        public List<TeamEntity> Search(string field, string value, string searchType)
        {
            var query = _context.Teams.AsQueryable();

            if (string.IsNullOrEmpty(value)) return new List<TeamEntity>();

            switch (field.ToLower())
            {
                case "name":
                    query = searchType == "e"
                        ? query.Where(t => t.teamName.ToLower() == value.ToLower())
                        : query.Where(t => t.teamName.ToLower().Contains(value.ToLower()));
                    break;

                case "year":
                    if (int.TryParse(value, out var year))
                        query = query.Where(t => t.year == year);
                    break;

                case "hq":
                    query = searchType == "e"
                        ? query.Where(t => t.headquarters.ToLower() == value.ToLower())
                        : query.Where(t => t.headquarters.ToLower().Contains(value.ToLower()));
                    break;

                case "principal":
                    query = searchType == "e"
                        ? query.Where(t => t.teamPrincipal.ToLower() == value.ToLower())
                        : query.Where(t => t.teamPrincipal.ToLower().Contains(value.ToLower()));
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
            var team = _context.Teams
                .Include(t => t.budget)
                    .ThenInclude(b => b.expenses)
                        .ThenInclude(e => e.subcategory)
                .FirstOrDefault(t => t.Id == id);

            if (team == null) return false;

            // Töröljük először a subcategory-ket
            foreach (var exp in team.budget?.expenses ?? new List<ExpensEntity>())
            {
                if (exp.subcategory != null)
                    _context.Subcategories.RemoveRange(exp.subcategory);
            }

            // Töröljük az expense-eket
            _context.Expenses.RemoveRange(team.budget?.expenses ?? new List<ExpensEntity>());

            // Töröljük a budget-et
            if (team.budget != null)
                _context.Budgets.Remove(team.budget);

            // Végül töröljük a csapatot
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

        public List<TeamEntity> ImportFromDirectory(string path)
        {
            var importer = new JsonImporter();
            return importer.ImportTeamsFromNEWDirectory(path);
        }

        public void GeneratePredictionReport(string teamName, double plannedBudget)
        {
            using var context = new FormulaOneDbContext();

            // Utolsó 2 év adatai adott csapathoz
            var recentBudgets = context.Budgets
                .Include(b => b.expenses)
                .Include(b => b.TeamEntity)
                .Where(b => b.TeamEntity.teamName.ToLower().Contains(teamName.ToLower()))
                .OrderByDescending(b => b.TeamEntity.year)
                .Take(2)
                .ToList();

            foreach(var data in recentBudgets.Where(p => p.TeamEntity.teamName.ToLower().Contains(teamName.ToLower())))
            {
                teamName = data.TeamEntity.teamName.ToString();
            }

            if (!recentBudgets.Any())
            {
                Console.WriteLine("Nincs elérhető adat a megadott csapathoz az elmúlt 2 évből.");
                Thread.Sleep(5000);
                return;
            }
            
            // Csoportosítás kategória szerint
            var categories = new[] { "Car", "Personnel", "Operations" };
            var categoryRatios = new Dictionary<string, List<double>>();

            foreach (var category in categories)
            {
                categoryRatios[category] = new List<double>();

                foreach (var budget in recentBudgets)
                {
                    var total = budget.expenses.Sum(e => e.amount);
                    if (total == 0) continue;

                    var categorySum = budget.expenses
                        .Where(e => e.category.Contains(category, StringComparison.OrdinalIgnoreCase))
                        .Sum(e => e.amount);

                    double ratio = Convert.ToDouble((double)categorySum / (double)total);
                    categoryRatios[category].Add(ratio);
                }
            }

            // Riport szöveg összeállítása
            var reportLines = new List<string>
        {
            $"Predicted Budget Allocation for: {teamName} ({DateTime.Now.Year + 1})",
            $"Total Planned Budget: ${plannedBudget.ToString("N0", CultureInfo.InvariantCulture)}",
            ""
        };

            foreach (var category in categories)
            {
                var ratios = categoryRatios[category];
                if (ratios.Count == 0) continue;

                var minRatio = ratios.Min();
                var maxRatio = ratios.Max();

                var minValue = Math.Round(minRatio * plannedBudget, 2);
                var maxValue = Math.Round(maxRatio * plannedBudget, 2);

                reportLines.Add($"{category}:");
                reportLines.Add($"  Minimum: ${minValue.ToString("N0", CultureInfo.InvariantCulture)}");
                reportLines.Add($"  Maximum: ${maxValue.ToString("N0", CultureInfo.InvariantCulture)}");
                reportLines.Add("");
            }

            // Fájlba írás
            var folder = "C:\\Users\\zsofi\\source\\repos\\WSEIMS_HSZF_2024252\\WSEIMS_HSZF_2024252.Model\\Reports";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = $"{teamName.Replace(" ", "_")}_{DateTime.Now.Year + 1}_Prediction.txt";
            var filePath = Path.Combine(folder, fileName);

            File.WriteAllLines(filePath, reportLines);

            Console.WriteLine($"Riport generálva: {filePath}");
            Thread.Sleep(5000);
        }
    }

}
