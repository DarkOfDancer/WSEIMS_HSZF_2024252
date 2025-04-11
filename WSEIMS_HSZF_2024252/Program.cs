using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using WSEIMS_HSZF_2024252.Model;
using WSEIMS_HSZF_2024252.Persistence.MsSql;


namespace WSEIMS_HSZF_2024252
{
    internal class Program
    {
        static void Menu(JsonImporter jsonImporter)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== Formula One Budget Console App =====");
                Console.WriteLine("1. Csapatok listázása (lapozás)");
                Console.WriteLine("2. Keresés");
                Console.WriteLine("3. Csapat frissítése");
                Console.WriteLine("4. Csapat törlése");
                Console.WriteLine("5. Fájl importálása");
                Console.WriteLine("6. Kilépés");
                Console.Write("Válassz egy opciót: ");

                var input = Console.ReadLine();
                Console.Clear();
                
            }
        }

        
        static async Task StartUpload(string rootDirectory,JsonImporter jsonImporter)
        {
            // JSON fájlok importálása és eredmény kiírása
            var resultMessage = jsonImporter.ImportTeamsFromJsonDirectory(rootDirectory);

            // Az importálás eredményének kiírása

            if (resultMessage == null)
            {
                Console.WriteLine("A könyvtár nem található, vagy egy hiba történt.");
            }
            else
            {
                Console.WriteLine($"Sikeresen importálva {resultMessage.Count} csapat.");
            }
            Thread.Sleep(5000);
        }
        static async Task Main(string[] args)
        {
            var ctx = new FormulaOneDbContext();
            // A gyökér könyvtár
            string rootDirectory = @"C:\Users\zsofi\source\repos\WSEIMS_HSZF_2024252\WSEIMS_HSZF_2024252.Model";  
            // Az importáló osztály példányosítása
            var jsonImporter = new JsonImporter();
            StartUpload(rootDirectory,jsonImporter);
            
            Menu(jsonImporter);
            var service = new TeamService();
            //var result = service.SearchTeams(field, value, exact);
        }
    }
}