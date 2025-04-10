using Microsoft.Extensions.DependencyInjection;
using System;
using WSEIMS_HSZF_2024252.Model;
using WSEIMS_HSZF_2024252.Persistence.MsSql;

namespace WSEIMS_HSZF_2024252
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ctx = new FormulaOneDbContext();

            string rootDirectory = @"C:\Users\zsofi\source\repos\WSEIMS_HSZF_2024252\WSEIMS_HSZF_2024252.Model";  // A gyökér könyvtár

            // Az importáló osztály példányosítása
            var jsonImporter = new JsonImporter();

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
        }
    }
}