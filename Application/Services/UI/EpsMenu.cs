using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Application.UI
{
    public class EpsMenu
    {
        private readonly EpsRepository _epsRepository;
        public EpsMenu(MySqlConnection connection)
        {
            _epsRepository = new EpsRepository(connection);
        }

        public void ShowMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool returnTo = false; 

            while (!returnTo)
            {
                Console.Clear();
                MainMenu.ShowHeader("  🪪  EPS MENU   ");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║                 🪪  EPS MENU                ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ║       1️⃣  List EPS           📋             ║");
                Console.WriteLine("  ║       2️⃣  Create EPS          ➕            ║");
                Console.WriteLine("  ║       3️⃣  Update EPS          ✏️             ║");
                Console.WriteLine("  ║       4️⃣  Delete EPS          ✖️             ║");
                Console.WriteLine("  ║       0️⃣  Return to Location Menu     ↩️     ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                switch (option)
                {
                    case "1":
                        ListEps().Wait();
                        break;
                    case "2":
                        CreateEps().Wait();
                        break;
                    case "3":
                        UpdateEps().Wait();
                        break;
                    case "4":
                        DeleteEps().Wait();
                        break;
                    case "0":
                        returnTo = true;
                        break;
                    default:
                        MainMenu.ShowMessage("⚠️ Invalid option. Please try again.", ConsoleColor.Red);
                        Console.ReadKey();
                        break;
                }
            }

            MainMenu.ShowMessage("\n👋 Thank you for using the application! Have a great day! 🌟", ConsoleColor.Green);
        }

        private async Task ListEps()
        {
            Console.Clear();
            MainMenu.ShowHeader("ARL LIST");

            try
            {
                var eps = await _epsRepository.GetAllAsync();

                if (!eps.Any())
                {
                    MainMenu.ShowMessage("\nNo EPS registered.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\n{0,-5} {1,-12}",
                        "ID", "Name");

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 95));
                    Console.ResetColor();

                    foreach (var e in eps)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("{0,-5} {1,-12}",
                            e.Id,
                            e.Name);
                    }

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 95));
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error listing EPS: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task CreateEps()
        {
            Console.Clear();
            MainMenu.ShowHeader("REGISTER NEW EPS");

            try
            {
                string nameEps = MainMenu.ReadText("\nName EPS: ").Trim();
                if (string.IsNullOrEmpty(nameEps))
                {
                    MainMenu.ShowMessage("Name EPS cannot be empty.", ConsoleColor.Red);
                    return;
                }

                var e = new Eps
                {
                    Name = nameEps
                };

                Console.Clear();
                MainMenu.ShowHeader("EPS INFORMATION");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Name: {nameEps}");

                string confirm = MainMenu.ReadText("\nDo you want to register this EPS? (Y/N): ");
                if (confirm.ToUpper() == "Y")
                {
                    bool result = await _epsRepository.InsertAsync(e);

                    if (result)
                    {
                        MainMenu.ShowMessage("\n✅ EPS registered successfully.", ConsoleColor.Green);
                    }
                    else
                    {
                        MainMenu.ShowMessage("\n❌ EPS registration failed.", ConsoleColor.Red);
                    }
                }
                else
                {
                    MainMenu.ShowMessage("\n⚠️ Operation cancelled.", ConsoleColor.Yellow);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error registering EPS: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task UpdateEps()
        {
            Console.Clear();
            MainMenu.ShowHeader("UPDATE EPS");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter EPS ID to update: ");
                var e = await _epsRepository.GetByIdAsync(id);

                if (e == null)
                {
                    MainMenu.ShowMessage("\n❌ The EPS doesn't exist", ConsoleColor.Red);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nCurrent Information:");
                    Console.WriteLine($"EPS ID: {e.Id}");
                    Console.WriteLine($"Name: {e.Name}");
                    Console.ResetColor();
                    
                    string nombre = MainMenu.ReadText($"Enter new name EPS ({e.Name}): ");
                    if (!string.IsNullOrWhiteSpace(nombre))
                    {
                        e.Name = nombre;
                    }

                    Console.Clear();
                    MainMenu.ShowHeader("UPDATED EPS INFORMATION");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"EPS ID: {e.Id}");
                    Console.WriteLine($"Name: {e.Name}");
                    Console.ResetColor();

                    string confirm = MainMenu.ReadText("\nDo you want to save these changes? (Y/N): ");
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _epsRepository.UpdateAsync(e);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ EPS updated successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to update the EPS.", ConsoleColor.Red);
                        }
                    }
                    else
                    {
                        MainMenu.ShowMessage("\n⚠️ Update cancelled.", ConsoleColor.Yellow);
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error updating the EPS: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task DeleteEps()
        {
            Console.Clear();
            MainMenu.ShowHeader("DELETE EPS");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter the EPS ID to delete: ");
                var e = await _epsRepository.GetByIdAsync(id);

                if (e == null)
                {
                    MainMenu.ShowMessage("\n❌ The EPS does not exist.", ConsoleColor.Red);
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nEPS Information:");
                    Console.WriteLine($"ID: {e.Id}");
                    Console.WriteLine($"Name: {e.Name}");
                    Console.ResetColor();
                    
                    string confirm = MainMenu.ReadText("\n⚠️ Are you sure you want to delete this EPS? (Y/N): ");
                    
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _epsRepository.DeleteAsync(id);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ EPS deleted successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to delete the EPS.", ConsoleColor.Red);
                        }
                    }
                    else
                    {
                        MainMenu.ShowMessage("\n⚠️ Operation cancelled.", ConsoleColor.Yellow);
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error deleting the EPS: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}