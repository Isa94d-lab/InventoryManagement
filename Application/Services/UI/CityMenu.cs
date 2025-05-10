using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Application.UI
{
    public class CityMenu
    {
        private readonly CityRepository _cityRepository;
        public CityMenu(MySqlConnection connection)
        {
            _cityRepository = new CityRepository(connection);
        }

        public void ShowMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool returnTo = false; 

            while (!returnTo)
            {
                Console.Clear();
                MainMenu.ShowHeader(" 🗼 CITY MENU   ");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║               🗼 CITY MENU                 ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ║       1️⃣  List City           📋            ║");
                Console.WriteLine("  ║       2️⃣  Create City          ➕           ║");
                Console.WriteLine("  ║       3️⃣  Update City          ✏️            ║");
                Console.WriteLine("  ║       4️⃣  Delete City          ✖️            ║");
                Console.WriteLine("  ║       0️⃣  Return to Location Menu     ↩️     ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                switch (option)
                {
                    case "1":
                        ListCity().Wait();
                        break;
                    case "2":
                        CreateCity().Wait();
                        break;
                    case "3":
                        UpdateCity().Wait();
                        break;
                    case "4":
                        DeleteCity().Wait();
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

        private async Task ListCity()
        {
            Console.Clear();
            MainMenu.ShowHeader("CITY LIST");

            try
            {
                var cities = await _cityRepository.GetAllAsync();

                if (!cities.Any())
                {
                    MainMenu.ShowMessage("\nNo cities registered.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\n{0,-5} {1,-12} {2,-30}",
                        "ID", "Name", "Region ID");

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 95));
                    Console.ResetColor();

                    foreach (var city in cities)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("{0,-5} {1,-12}",
                            city.Id,
                            city.Name,
                            city.RegionId);
                    }

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 95));
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error listing cities: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task CreateCity()
        {
            Console.Clear();
            MainMenu.ShowHeader("REGISTER NEW CITY");

            try
            {
                string nameCity = MainMenu.ReadText("\nName city: ").Trim();
                if (string.IsNullOrEmpty(nameCity))
                {
                    MainMenu.ShowMessage("Name city cannot be empty.", ConsoleColor.Red);
                    return;
                }

                int regionId = MainMenu.ReadInteger("City ID: ");
                if (regionId <= 0)
                {
                    MainMenu.ShowMessage("City ID must be greater than zero.", ConsoleColor.Red);
                    return;
                }

                var city = new City
                {
                    Name = nameCity,
                    RegionId = regionId
                };

                Console.Clear();
                MainMenu.ShowHeader("CITY INFORMATION");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\nID: {city.Id}");
                Console.WriteLine($"Name: {nameCity}");
                Console.WriteLine($"City ID: {regionId}");

                string confirm = MainMenu.ReadText("\nDo you want to register this city? (Y/N): ");
                if (confirm.ToUpper() == "Y")
                {
                    bool result = await _cityRepository.InsertAsync(city);

                    if (result)
                    {
                        MainMenu.ShowMessage("\n✅ City registered successfully.", ConsoleColor.Green);
                    }
                    else
                    {
                        MainMenu.ShowMessage("\n❌ City registration failed.", ConsoleColor.Red);
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

        private async Task UpdateCity()
        {
            Console.Clear();
            MainMenu.ShowHeader("UPDATE REGION");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter city ID to update: ");
                var city = await _cityRepository.GetByIdAsync(id);

                if (city == null)
                {
                    MainMenu.ShowMessage("\n❌ The city doesn't exist", ConsoleColor.Red);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nCurrent Information:");
                    Console.WriteLine($"ID: {city.Id}");
                    Console.WriteLine($"Name: {city.Name}");
                    Console.WriteLine($"Country ID: {city.RegionId}");
                    Console.ResetColor();
                    
                    string nombre = MainMenu.ReadText($"Enter new name city ({city.Name}): ");
                    if (!string.IsNullOrWhiteSpace(nombre))
                    {
                        city.Name = nombre;
                    }

                    int newRegionId = MainMenu.ReadInteger($"\nNew Region ID [{city.RegionId}]: ");
                    if (newRegionId != city.RegionId)
                    {
                        if (newRegionId <= 0)
                        {
                            MainMenu.ShowMessage("Region ID must be greater than zero.", ConsoleColor.Red);
                            return;
                        }
                        city.RegionId = newRegionId;
                    }

                    Console.Clear();
                    MainMenu.ShowHeader("UPDATED CITY INFORMATION");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"ID: {city.Id}");
                    Console.WriteLine($"Name: {city.Name}");
                    Console.WriteLine($"Name: {city.RegionId}");
                    Console.ResetColor();

                    string confirm = MainMenu.ReadText("\nDo you want to save these changes? (Y/N): ");
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _cityRepository.UpdateAsync(city);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ City updated successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to update the city.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error updating the city: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task DeleteCity()
        {
            Console.Clear();
            MainMenu.ShowHeader("DELETE COUNTRY");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter the region ID to delete: ");
                var city = await _cityRepository.GetByIdAsync(id);

                if (city == null)
                {
                    MainMenu.ShowMessage("\n❌ The city does not exist.", ConsoleColor.Red);
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nRegion Information:");
                    Console.WriteLine($"ID: {city.Id}");
                    Console.WriteLine($"Name: {city.Name}");
                    Console.WriteLine($"Name: {city.RegionId}");
                    Console.ResetColor();
                    
                    string confirm = MainMenu.ReadText("\n⚠️ Are you sure you want to delete this city? (Y/N): ");
                    
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _cityRepository.DeleteAsync(id);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ City deleted successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to delete the city.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error deleting the city: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}