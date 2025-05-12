using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Application.UI
{
    public class ArlMenu
    {
        private readonly ArlRepository _arlRepository;
        public ArlMenu(MySqlConnection connection)
        {
            _arlRepository = new ArlRepository(connection);
        }

        public void ShowMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool returnTo = false; 

            while (!returnTo)
            {
                Console.Clear();
                MainMenu.ShowHeader(" 🪪 ARL MENU   ");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║                 🪪  ARL MENU                ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ║       1️⃣  List ARL           📋             ║");
                Console.WriteLine("  ║       2️⃣  Create ARL          ➕            ║");
                Console.WriteLine("  ║       3️⃣  Update ARL          ✏️             ║");
                Console.WriteLine("  ║       4️⃣  Delete ARL          ✖️             ║");
                Console.WriteLine("  ║       0️⃣  Return to Location Menu     ↩️     ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                switch (option)
                {
                    case "1":
                        ListArl().Wait();
                        break;
                    case "2":
                        CreateArl().Wait();
                        break;
                    case "3":
                        UpdateArl().Wait();
                        break;
                    case "4":
                        DeleteArl().Wait();
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

        private async Task ListArl()
        {
            Console.Clear();
            MainMenu.ShowHeader("ARL LIST");

            try
            {
                var arls = await _arlRepository.GetAllAsync();

                if (!arls.Any())
                {
                    MainMenu.ShowMessage("\nNo arls registered.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\n{0,-5} {1,-12}",
                        "ID", "Name");

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 95));
                    Console.ResetColor();

                    foreach (var arl in arls)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("{0,-5} {1,-12}",
                            arl.Id,
                            arl.Name);
                    }

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 95));
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error listing arl: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task CreateArl()
        {
            Console.Clear();
            MainMenu.ShowHeader("REGISTER NEW ARL");

            try
            {
                string nameArl = MainMenu.ReadText("\nName ARL: ").Trim();
                if (string.IsNullOrEmpty(nameArl))
                {
                    MainMenu.ShowMessage("Name ARL cannot be empty.", ConsoleColor.Red);
                    return;
                }

                var arl = new Arl
                {
                    Name = nameArl
                };

                Console.Clear();
                MainMenu.ShowHeader("ARL INFORMATION");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Name: {nameArl}");

                string confirm = MainMenu.ReadText("\nDo you want to register this ARL? (Y/N): ");
                if (confirm.ToUpper() == "Y")
                {
                    bool result = await _arlRepository.InsertAsync(arl);

                    if (result)
                    {
                        MainMenu.ShowMessage("\n✅ ARL registered successfully.", ConsoleColor.Green);
                    }
                    else
                    {
                        MainMenu.ShowMessage("\n❌ ARL registration failed.", ConsoleColor.Red);
                    }
                }
                else
                {
                    MainMenu.ShowMessage("\n⚠️ Operation cancelled.", ConsoleColor.Yellow);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error registering ARL: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task UpdateArl()
        {
            Console.Clear();
            MainMenu.ShowHeader("UPDATE ARL");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter ARL ID to update: ");
                var arl = await _arlRepository.GetByIdAsync(id);

                if (arl == null)
                {
                    MainMenu.ShowMessage("\n❌ The ARL doesn't exist", ConsoleColor.Red);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nCurrent Information:");
                    Console.WriteLine($"ARL ID: {arl.Id}");
                    Console.WriteLine($"Name: {arl.Name}");
                    Console.ResetColor();
                    
                    string nombre = MainMenu.ReadText($"Enter new name ARL ({arl.Name}): ");
                    if (!string.IsNullOrWhiteSpace(nombre))
                    {
                        arl.Name = nombre;
                    }

                    Console.Clear();
                    MainMenu.ShowHeader("UPDATED ARL INFORMATION");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"ARL ID: {arl.Id}");
                    Console.WriteLine($"Name: {arl.Name}");
                    Console.ResetColor();

                    string confirm = MainMenu.ReadText("\nDo you want to save these changes? (Y/N): ");
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _arlRepository.UpdateAsync(arl);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ ARL updated successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to update the ARL.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error updating the ARL: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task DeleteArl()
        {
            Console.Clear();
            MainMenu.ShowHeader("DELETE ARL");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter the ARL ID to delete: ");
                var arl = await _arlRepository.GetByIdAsync(id);

                if (arl == null)
                {
                    MainMenu.ShowMessage("\n❌ The ARL does not exist.", ConsoleColor.Red);
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nARL Information:");
                    Console.WriteLine($"ID: {arl.Id}");
                    Console.WriteLine($"Name: {arl.Name}");
                    Console.ResetColor();
                    
                    string confirm = MainMenu.ReadText("\n⚠️ Are you sure you want to delete this ARL? (Y/N): ");
                    
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _arlRepository.DeleteAsync(id);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ ARL deleted successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to delete the ARL.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error deleting the ARL: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}