using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using WSEIMS_HSZF_2024252.Model;
using WSEIMS_HSZF_2024252.Persistence.MsSql;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using WSEIMS_HSZF_2024252.Model;
using WSEIMS_HSZF_2024252.Persistence.MsSql;

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
    // Beolvasás egyetlen JSON fájlból
    public TeamEntity ReadJsonFile(string filePath)
    {
        try
        {
            // JSON fájl beolvasása
            string json = File.ReadAllText(filePath);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // A JSON deszerializálása TeamEntity típusra
            var team = JsonSerializer.Deserialize<TeamEntity>(json, options);
            return team;
        }
        catch (Exception)
        {
            // Hiba esetén null visszaadása
            return null;
        }
    }

    // Importálás a JSON fájlokból és adatbázisba
    public List<TeamEntity> ImportTeamsFromJsonDirectory(string rootDirectory)
    {
        if (!Directory.Exists(rootDirectory))
        {
            return null;
        }

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

                using (var context = new FormulaOneDbContext())
                {
                    var existingTeam = context.Teams
                        .Include(t => t.budget)
                        .ThenInclude(b => b.expenses)
                        .FirstOrDefault(t => t.teamName == team.teamName && t.year == team.year);

                    if (existingTeam == null)
                    {
                        context.Teams.Add(team);
                        context.SaveChanges();
                        allTeams.Add(team);
                    }
                    else
                    {
                        if (team.budget?.expenses != null && team.budget.expenses.Any())
                        {
                            foreach (var newExpense in team.budget.expenses)
                            {
                                newExpense.BudgetId = existingTeam.budget.Id;

                                // (Opcionális) Ne adjuk hozzá, ha már létezik ugyanolyan költség
                                bool isDuplicate = existingTeam.budget.expenses.Any(e =>
                                    e.expenseDate == newExpense.expenseDate &&
                                    e.amount == newExpense.amount &&
                                    e.category == newExpense.category);

                                if (!isDuplicate)
                                {
                                    context.Expenses.Add(newExpense);
                                }
                            }

                            context.SaveChanges();
                            allTeams.Add(existingTeam);
                        }
                    }
                }
            }
        }

        return allTeams;
    }


    // Ellenőrizzük, hogy a mappa neve tartalmaz-e évet (4 számjegy)
    private bool IsYearFolder(string folderName)
    {
        // Ellenőrizzük, hogy a mappa neve 4 számjegyből áll (pl. 2023)
        return folderName.Length == 4 && folderName.All(char.IsDigit);
    }

    public bool UpdateTeam(TeamEntity updatedTeam)
    {
        using (var context = new FormulaOneDbContext())
        {
            var existingTeam = context.Teams
                .Include(t => t.budget)
                .ThenInclude(b => b.expenses)
                .FirstOrDefault(t => t.Id == updatedTeam.Id);

            if (existingTeam == null) return false;

            // Frissítjük az értékeket
            context.Entry(existingTeam).CurrentValues.SetValues(updatedTeam);

            // Frissítjük a költségvetést, ha van
            if (updatedTeam.budget != null)
            {
                if (existingTeam.budget != null)
                {
                    context.Entry(existingTeam.budget).CurrentValues.SetValues(updatedTeam.budget);
                }
                else
                {
                    existingTeam.budget = updatedTeam.budget;
                }

                // Expenses frissítése (egyszerűsített logika)
                existingTeam.budget.expenses = updatedTeam.budget.expenses;
            }

            context.SaveChanges();
            return true;
        }
    }

    // Csapat törlése ID alapján
    public bool DeleteTeam(string teamId)
    {
        using (var context = new FormulaOneDbContext())
        {
            var team = context.Teams
                .Include(t => t.budget)
                .ThenInclude(b => b.expenses)
                .FirstOrDefault(t => t.Id == teamId);

            if (team == null) return false;

            context.Teams.Remove(team);
            context.SaveChanges();
            return true;
        }
    }

    // Egy csapat lekérdezése ID alapján (pl. szerkesztéshez)
    public TeamEntity GetTeamById(string id)
    {
        using (var context = new FormulaOneDbContext())
        {
            return context.Teams
                .Include(t => t.budget)
                .ThenInclude(b => b.expenses)
                .FirstOrDefault(t => t.Id == id);
        }
    }

    // Összes csapat lekérdezése
    public List<TeamEntity> GetAllTeams()
    {
        using (var context = new FormulaOneDbContext())
        {
            return context.Teams
                .Include(t => t.budget)
                .ThenInclude(b => b.expenses)
                .OrderBy(t => t.year)
                .ThenBy(t => t.teamName)
                .ToList();
        }
    }

}


