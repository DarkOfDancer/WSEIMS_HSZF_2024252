using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
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
                        allTeams.Add(team);
                        context.SaveChanges();
    
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

                            
                            allTeams.Add(existingTeam);
                            context.SaveChanges();

                        }
                    }
                }
            }
        }

        return allTeams;
    }

    public List<TeamEntity> ImportTeamsFromNEWDirectory(string rootDirectory)
    {
        if (!Directory.Exists(rootDirectory))
        {
            return null;
        }

        var allTeams = new List<TeamEntity>();

        // Csak a fájlokat olvassuk be a rootDirectory-ból, nem pedig almappákat
        var files = Directory.GetFiles(rootDirectory, "*.json");

        foreach (var filePath in files)
        {
            var team = ReadJsonFile(filePath);
            if (team == null) continue;

            using (var context = new FormulaOneDbContext())
            {
          
                // Ellenőrizzük, hogy létezik-e már a csapat a megfelelő évvel
                var existingTeam = context.Teams
                    .Include(t => t.budget)
                    .ThenInclude(b => b.expenses)
                    .FirstOrDefault(t => t.teamName == team.teamName && t.year == team.year);

                if (existingTeam == null)
                {
     
                    // Ha nem létezik, új csapatot adunk hozzá
                    context.Teams.Add(team);
                    context.SaveChanges();
                    allTeams.Add(team);
                }
                else
                {

                    // Ha létezik, hozzáadjuk az új költségeket, ha nem léteznek már
                    if (team.budget?.expenses != null && team.budget.expenses.Any())
                    {
                        foreach (var newExpense in team.budget.expenses)
                        {
                            newExpense.BudgetId = existingTeam.budget.Id;

                            // Ne adjuk hozzá, ha már létezik ugyanolyan költség
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

        return allTeams;
    }


    // Ellenőrizzük, hogy a mappa neve tartalmaz-e évet (4 számjegy)
    private bool IsYearFolder(string folderName)
    {
        // Ellenőrizzük, hogy a mappa neve 4 számjegyből áll (pl. 2023)
        return folderName.Length == 4 && folderName.All(char.IsDigit);
    }



}


