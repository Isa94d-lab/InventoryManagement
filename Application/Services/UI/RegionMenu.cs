using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Application.UI
{
    public class RegionMenu
    {
        private readonly RegionRepository _regionRepository;
        public RegionMenu(MySqlConnection connection)
        {
            _regionRepository = new RegionRepository(connection);
        }

        public void ShowMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool returnTo = false; 

            while (!returnTo)
            {
                Console.Clear();
                MainMenu.ShowHeader(" 🏞️  REGION MENU  ");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║              🏞️  REGION MENU                ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ║       1️⃣  List Region           📋          ║");
                Console.WriteLine("  ║       2️⃣  Create Region          ➕         ║");
                Console.WriteLine("  ║       3️⃣  Update Region          ✏️          ║");
                Console.WriteLine("  ║       4️⃣  Delete Region          ✖️          ║");
                Console.WriteLine("  ║       0️⃣  Return to Location Menu     ↩️     ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                switch (option)
                {
                    case "1":
                        ListRegion().Wait();
                        break;
                    case "2":
                        CreateRegion().Wait();
                        break;
                    case "3":
                        UpdateRegion().Wait();
                        break;
                    case "4":
                        DeleteRegion().Wait();
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

        private async Task ListRegion()
        {
            Console.Clear();
            MainMenu.ShowHeader("REGION LIST");

            try
            {
                var regions = await _regionRepository.GetAllAsync();

                if (!regions.Any())
                {
                    MainMenu.ShowMessage("\nNo regions registered.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\n{0,-5} {1,-12} {2,-30}",
                        "ID", "Name", "Country ID");

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 95));
                    Console.ResetColor();

                    foreach (var region in regions)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("{0,-5} {1,-12}",
                            region.Id,
                            region.Name,
                            region.CountryId);
                    }

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 95));
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error listing regions: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task CreateRegion()
        {
            Console.Clear();
            MainMenu.ShowHeader("REGISTER NEW REGION");

            try
            {
                string nameRegion = MainMenu.ReadText("\nName region: ").Trim();
                if (string.IsNullOrEmpty(nameRegion))
                {
                    MainMenu.ShowMessage("Name region cannot be empty.", ConsoleColor.Red);
                    return;
                }

                int countryId = MainMenu.ReadInteger("Country ID: ");
                if (countryId <= 0)
                {
                    MainMenu.ShowMessage("Country ID must be greater than zero.", ConsoleColor.Red);
                    return;
                }

                var region = new Region
                {
                    Name = nameRegion,
                    CountryId = countryId
                };

                Console.Clear();
                MainMenu.ShowHeader("REGION INFORMATION");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\nID: {region.Id}");
                Console.WriteLine($"Name: {nameRegion}");
                Console.WriteLine($"Country ID: {countryId}");

                string confirm = MainMenu.ReadText("\nDo you want to register this region? (Y/N): ");
                if (confirm.ToUpper() == "Y")
                {
                    bool result = await _regionRepository.InsertAsync(region);

                    if (result)
                    {
                        MainMenu.ShowMessage("\n✅ Region registered successfully.", ConsoleColor.Green);
                    }
                    else
                    {
                        MainMenu.ShowMessage("\n❌ Region registration failed.", ConsoleColor.Red);
                    }
                }
                else
                {
                    MainMenu.ShowMessage("\n⚠️ Operation cancelled.", ConsoleColor.Yellow);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error registering region: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task UpdateRegion()
        {
            Console.Clear();
            MainMenu.ShowHeader("UPDATE REGION");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter region ID to update: ");
                var region = await _regionRepository.GetByIdAsync(id);

                if (region == null)
                {
                    MainMenu.ShowMessage("\n❌ The region doesn't exist", ConsoleColor.Red);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nCurrent Information:");
                    Console.WriteLine($"ID: {region.Id}");
                    Console.WriteLine($"Name: {region.Name}");
                    Console.WriteLine($"Country ID: {region.CountryId}");
                    Console.ResetColor();
                    
                    string nombre = MainMenu.ReadText($"Enter new name region ({region.Name}): ");
                    if (!string.IsNullOrWhiteSpace(nombre))
                    {
                        region.Name = nombre;
                    }

                    int newCountryId = MainMenu.ReadInteger($"\nNew Country ID [{region.CountryId}]: ");
                    if (newCountryId != region.CountryId)
                    {
                        if (newCountryId <= 0)
                        {
                            MainMenu.ShowMessage("Country ID must be greater than zero.", ConsoleColor.Red);
                            return;
                        }
                        region.CountryId = newCountryId;
                    }

                    Console.Clear();
                    MainMenu.ShowHeader("UPDATED REGION INFORMATION");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"ID: {region.Id}");
                    Console.WriteLine($"Name: {region.Name}");
                    Console.WriteLine($"Name: {region.CountryId}");
                    Console.ResetColor();

                    string confirm = MainMenu.ReadText("\nDo you want to save these changes? (Y/N): ");
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _regionRepository.UpdateAsync(region);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Region updated successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to update the region.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error updating the region: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task DeleteRegion()
        {
            Console.Clear();
            MainMenu.ShowHeader("DELETE REGION");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter the region ID to delete: ");
                var region = await _regionRepository.GetByIdAsync(id);

                if (region == null)
                {
                    MainMenu.ShowMessage("\n❌ The region does not exist.", ConsoleColor.Red);
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nRegion Information:");
                    Console.WriteLine($"ID: {region.Id}");
                    Console.WriteLine($"Name: {region.Name}");
                    Console.WriteLine($"Country ID: {region.CountryId}");
                    Console.ResetColor();
                    
                    string confirm = MainMenu.ReadText("\n⚠️ Are you sure you want to delete this region? (Y/N): ");
                    
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _regionRepository.DeleteAsync(id);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Region deleted successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to delete the region.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error deleting the region: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}