using Azure;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using WSEIMS_HSZF_2024252.Application;
using WSEIMS_HSZF_2024252.Model;
using WSEIMS_HSZF_2024252.Persistence.MsSql;


namespace WSEIMS_HSZF_2024252
{
    internal class Program
    {
        static void Menu(JsonImporter jsonImporter, TeamService service)
        {

            while (true)
            {
                int currentPage = 1;

                Console.Clear();
                Console.WriteLine("===== Formula One Budget Console App =====");
                Console.WriteLine("1. Csapatok listázása (lapozás)");
                Console.WriteLine("2. Keresés");
                Console.WriteLine("3. Csapat frissítése");
                Console.WriteLine("4. Csapat törlése");
                Console.WriteLine("5. Fájl importálása");
                Console.WriteLine("6. Riport készítés");
                Console.WriteLine("7. Kilépés");
                Console.Write("Válassz egy opciót: ");

                var input = Console.ReadLine();
                Console.Clear();
                switch (input)
                {
                    case "1":
                        ShowTeams((p, s) => service.GetTeamsPaged(p, s), ref currentPage, 10);
                        break;
                    case "2":
                        Search(service,ref currentPage,10);
                        break;
                    case "3":
                        Update(service);
                        break;
                    case "4":
                        Delete(service);
                        break;
                    case "5":
                        Upload(service);
                        break;
                    case "6":
                        Report(service);
                        break;
                    case "7":
                        Console.WriteLine("Kilépés...");
                        return;
                    default:
                        Console.WriteLine("Érvénytelen választás. Nyomj meg egy gombot...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static async Task Report(TeamService service)
        {
            Console.Write("Add meg a csapat nevét: ");
            string teamName = Console.ReadLine();

            Console.Write("Add meg a tervezett költségvetést (szám): ");
            if (double.TryParse(Console.ReadLine(), out double budget))
            {
                service.GeneratePredictionReport(teamName, budget);
                Thread.Sleep(5000);
            }
            else
            {
                Console.WriteLine("Érvénytelen összeg.");
                Thread.Sleep(5000);
            }
            
        }
        static void ShowTeams(Func<int, int, List<TeamEntity>> dataProvider, ref int page, int size)
        {
            while (true)
            {
                Console.Clear();

                var teams = dataProvider(page, size);
                if (teams == null || teams.Count == 0)
                {
                    Console.WriteLine("Nincs megjeleníthető csapat.");
                    break;
                }

                Console.WriteLine($"--- {page}. oldal ---");
                foreach (var t in teams)
                {
                    Console.WriteLine($"{t.teamName} ({t.year}) - {t.headquarters} | {t.teamPrincipal}");
                }

                Console.WriteLine("N - Következő oldal | P - Előző oldal | Q - Vissza");
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.N && teams.Count == size) page++;
                else if (key == ConsoleKey.P && page > 1) page--;
                else if (key == ConsoleKey.Q) break;
            }
        }


        static void Search(TeamService service, ref int currentpage,int size)
        {
            Console.Clear();
            Console.WriteLine("Keresési mező (name, year, hq, principal, titles):");
            var field = Console.ReadLine();
            Console.Write("Keresési érték: ");
            var value = Console.ReadLine();
            Console.WriteLine("Keresési típus (equals / contains)(e/c):");
            string type = Console.ReadLine()?.ToLower();
            var results = service.Search(field, value,type);
            ShowTeams((p, s) => results.Skip((p - 1) * s).Take(s).ToList(), ref currentpage, size);
            Console.WriteLine("Enter a visszatéréshez...");
            Console.ReadLine();
        }

        static void Delete(TeamService service)
        {
            Console.Clear();
            Console.Write("Törlendő ID: ");
            var id = Console.ReadLine();
            var success = service.Delete(id);
            Console.WriteLine(success ? "Törölve." : "Nem található.");
            Console.ReadLine();
        }

        static void Update(TeamService service)
        {
            Console.Clear();
            Console.Write("Frissítendő ID: ");
            var id = Console.ReadLine();

            var team = service.GetById(id);
            if (team == null)
            {
                Console.WriteLine("Nem található.");
                Console.ReadLine();
                return;
            }

            Console.Write($"Név ({team.teamName}): ");
            var name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name)) team.teamName = name;

            Console.Write($"Főhadiszállás ({team.headquarters}): ");
            var hq = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(hq)) team.headquarters = hq;

            var success = service.Update(team);
            Console.WriteLine(success ? "Frissítve." : "Sikertelen.");
            Console.ReadLine();
        }

        static void Upload(TeamService service)
        {
           
                Console.Write("Add meg a könyvtár elérési útját: ");
                var path = Console.ReadLine();

                var importedTeams = service.ImportFromDirectory(path);
                Console.WriteLine($"{importedTeams.Count} csapat importálva.");

                Console.WriteLine("Nyomj meg egy gombot a folytatáshoz...");
                Console.ReadKey();
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
            var service = new TeamService();
            // Az importáló osztály példányosítása
            var jsonImporter = new JsonImporter();
            // A gyökér könyvtár
            string rootDirectory = @"C:\Users\zsofi\source\repos\WSEIMS_HSZF_2024252\WSEIMS_HSZF_2024252.Model";  
            
            StartUpload(rootDirectory,jsonImporter);
            Menu(jsonImporter,service);
            
        }
    }
}