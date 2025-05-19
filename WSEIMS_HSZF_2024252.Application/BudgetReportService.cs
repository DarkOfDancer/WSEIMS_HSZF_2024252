using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSEIMS_HSZF_2024252.Persistence.MsSql
{
    public interface IBudgetReportService
    {
        void GeneratePredictionReport(string teamName, double plannedBudget);
        List<(string Category, double TotalAmount)> GetExpenseReportByYearAndTeam(string teamName, int year);

    }
    public class BudgetReportService : IBudgetReportService
    {
        private readonly ITeamDataProvider _context;

        public BudgetReportService(ITeamDataProvider context)
        {
            _context = context;
        }

        public void GeneratePredictionReport(string teamName, double plannedBudget)
        {
            var recentBudgets = _context.Context().Budgets
                .Include(b => b.expenses)
                .Include(b => b.TeamEntity)
                .Where(b => b.TeamEntity.teamName.ToLower().Contains(teamName.ToLower()))
                .OrderByDescending(b => b.TeamEntity.year)
                .Take(2)
                .ToList();

            foreach (var data in recentBudgets)
            {
                if (data.TeamEntity.teamName.ToLower().Contains(teamName.ToLower()))
                    teamName = data.TeamEntity.teamName;
            }

            if (!recentBudgets.Any())
            {
                Console.WriteLine("Nincs elérhető adat a megadott csapathoz az elmúlt 2 évből.");
                Thread.Sleep(5000);
                return;
            }

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

                    double ratio = (double)categorySum / total;
                    categoryRatios[category].Add(ratio);
                }
            }

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
            string relativePath = @"..\..\..\..\WSEIMS_HSZF_2024252.Model\\Reports";
            string folder = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = $"{teamName.Replace(" ", "_")}_{DateTime.Now.Year + 1}_Prediction.txt";
            var filePath = Path.Combine(folder, fileName);

            File.WriteAllLines(filePath, reportLines);

            Console.WriteLine($"Riport generálva: {filePath}");
            Thread.Sleep(2000);
        }

        public List<(string Category, double TotalAmount)> GetExpenseReportByYearAndTeam(string teamName, int year)
        {
            var team = _context.Context().Teams
                .Include(t => t.budget)
                .ThenInclude(b => b.expenses)
                .FirstOrDefault(t => t.teamName.ToLower().Contains(teamName.ToLower())
                                     && t.year == year);

            if (team?.budget?.expenses == null)
                return new List<(string, double)>();

            return team.budget.expenses
                .GroupBy(e => e.category)
                .Select(g => (g.Key, (double)g.Sum(e => e.amount)))
                .OrderByDescending(x => x.Item2)
                .ToList();
        }
    }
}
