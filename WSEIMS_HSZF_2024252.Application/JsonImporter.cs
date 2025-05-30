using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using WSEIMS_HSZF_2024252.Model;
using WSEIMS_HSZF_2024252.Persistence.MsSql;

public class JsonImporter
{
    private readonly ITeamDataProvider dp;
    private readonly IBudgetDataProvider bp;

    public JsonImporter(ITeamDataProvider dp, IBudgetDataProvider bp)
    {
        this.dp = dp;
        this.bp = bp;
    }

    // Beolvasás egyetlen JSON fájlból
    public TeamEntity ReadJsonFile(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var team = JsonSerializer.Deserialize<TeamEntity>(json, options);
            return team;
        }
        catch (Exception)
        {
            return null;
        }
    }

    // Importálás JSON könyvtárból
    public List<TeamEntity> ImportTeamsFromJsonDirectory(string rootDirectory)
    {
        if (!Directory.Exists(rootDirectory))
            return null;

        var allTeams = new List<TeamEntity>();
        var yearDirectories = Directory.GetDirectories(rootDirectory);

        foreach (var yearDirectory in yearDirectories)
        {
            string folderName = Path.GetFileName(yearDirectory);
            if (!IsYearFolder(folderName)) continue;

            var files = Directory.GetFiles(yearDirectory, "*.json");

            foreach (var filePath in files)
            {
                var team = ReadJsonFile(filePath);
                if (team == null) continue;

                var existingTeam = dp.GetTeamWithBudgetAndExpenses(team.teamName, team.year);

                if (existingTeam == null)
                {
                    dp.Add(team);
                    allTeams.Add(team);
                }
                else
                {
                    if (team.budget?.expenses != null && team.budget.expenses.Any())
                    {
                        foreach (var newExpense in team.budget.expenses)
                        {
                            newExpense.BudgetId = existingTeam.budget.Id;
                            existingTeam.budget.expenses.Add(newExpense);
                        }
                        bp.Update(existingTeam.budget);
                        allTeams.Add(existingTeam);
                    }
                }
            }
        }

        return allTeams;
    }

    // Új könyvtárból importálás (csak fájlokat néz)
    public List<TeamEntity> ImportTeamsFromNEWDirectory(string rootDirectory)
    {
        if (!Directory.Exists(rootDirectory))
            return null;

        var allTeams = new List<TeamEntity>();

        var files = Directory.GetFiles(rootDirectory, "*.json");

        foreach (var filePath in files)
        {
            var team = ReadJsonFile(filePath);
            if (team == null) continue;

            var existingTeam = dp.GetTeamWithBudgetAndExpenses(team.teamName, team.year);

            if (existingTeam == null)
            {
                dp.Add(team);
                allTeams.Add(team);
            }
            else
            {
                if (team.budget?.expenses != null && team.budget.expenses.Any())
                {
                    foreach (var newExpense in team.budget.expenses)
                    {
                        newExpense.BudgetId = existingTeam.budget.Id;

                        bool isDuplicate = existingTeam.budget.expenses.Any(e =>
                            e.expenseDate == newExpense.expenseDate &&
                            e.amount == newExpense.amount &&
                            e.category == newExpense.category);

                        if (!isDuplicate)
                        {
                            existingTeam.budget.expenses.Add(newExpense);
                        }
                    }

                    bp.Update(existingTeam.budget);
                    allTeams.Add(existingTeam);
                }
            }
        }

        return allTeams;
    }

    private bool IsYearFolder(string folderName)
    {
        return folderName.Length == 4 && folderName.All(char.IsDigit);
    }
}
