using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Application.UI
{
    public class CountryMenu
    {
        private readonly CountryRepository _countryRepository;
        public CountryMenu(MySqlConnection connection)
        {
            _countryRepository = new CountryRepository(connection);
        }

        public void ShowMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool returnTo = false; 

            while (!returnTo)
            {
                Console.Clear();
                MainMenu.ShowHeader(" 🌎 COUNTRY MENU   ");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║               🌎 COUNTRY MENU              ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ║       1️⃣  List Countries           📋       ║");
                Console.WriteLine("  ║       2️⃣  Create Country          ➕        ║");
                Console.WriteLine("  ║       3️⃣  Update Country          ✏️         ║");
                Console.WriteLine("  ║       4️⃣  Delete Country          ✖️         ║");
                Console.WriteLine("  ║       0️⃣  Return to Location Menu     ↩️     ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                switch (option)
                {
                    case "1":
                        ListCountries().Wait();
                        break;
                    case "2":
                        CreateCountry().Wait();
                        break;
                    case "3":
                        UpdateCountry().Wait();
                        break;
                    case "4":
                        DeleteCountry().Wait();
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

        private async Task ListCountries()
        {
            Console.Clear();
            MainMenu.ShowHeader("COUNTRIES LIST");

            try
            {
                var countries = await _countryRepository.GetAllAsync();

                if (!countries.Any())
                {
                    MainMenu.ShowMessage("\nNo countries registered.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\n{0,-5} {1,-12}",
                        "ID", "Name");

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 95));
                    Console.ResetColor();

                    foreach (var country in countries)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("{0,-5} {1,-12}",
                            country.Id,
                            country.Name);
                    }

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 95));
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error listing countries: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task CreateCountry()
        {
            Console.Clear();
            MainMenu.ShowHeader("REGISTER NEW COUNTRY");

            try
            {
                string nameCountry = MainMenu.ReadText("\nName Country: ").Trim();
                if (string.IsNullOrEmpty(nameCountry))
                {
                    MainMenu.ShowMessage("Name Country cannot be empty.", ConsoleColor.Red);
                    return;
                }

                var country = new Country
                {
                    Name = nameCountry
                };

                Console.Clear();
                MainMenu.ShowHeader("COUNTRY INFORMATION");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\nCountry ID: {country.Id}");
                Console.WriteLine($"Name: {nameCountry}");

                string confirm = MainMenu.ReadText("\nDo you want to register this country? (Y/N): ");
                if (confirm.ToUpper() == "Y")
                {
                    bool result = await _countryRepository.InsertAsync(country);

                    if (result)
                    {
                        MainMenu.ShowMessage("\n✅ Country registered successfully.", ConsoleColor.Green);
                    }
                    else
                    {
                        MainMenu.ShowMessage("\n❌ Country registration failed.", ConsoleColor.Red);
                    }
                }
                else
                {
                    MainMenu.ShowMessage("\n⚠️ Operation cancelled.", ConsoleColor.Yellow);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error registering country: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task UpdateCountry()
        {
            Console.Clear();
            MainMenu.ShowHeader("UPDATE COUNTRY");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter country ID to update: ");
                var country = await _countryRepository.GetByIdAsync(id);

                if (country == null)
                {
                    MainMenu.ShowMessage("\n❌ The country doesn't exist", ConsoleColor.Red);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nCurrent Information:");
                    Console.WriteLine($"Country ID: {country.Id}");
                    Console.WriteLine($"Birthdate: {country.Name}");
                    Console.ResetColor();
                    
                    string nombre = MainMenu.ReadText($"Enter new name country ({country.Name}): ");
                    if (!string.IsNullOrWhiteSpace(nombre))
                    {
                        country.Name = nombre;
                    }

                    Console.Clear();
                    MainMenu.ShowHeader("UPDATED COUNTRY INFORMATION");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"Country ID: {country.Id}");
                    Console.WriteLine($"Birthdate: {country.Name}");
                    Console.ResetColor();

                    string confirm = MainMenu.ReadText("\nDo you want to save these changes? (Y/N): ");
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _countryRepository.UpdateAsync(country);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Country updated successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to update the country.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error updating the country: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task DeleteCountry()
        {
            Console.Clear();
            MainMenu.ShowHeader("DELETE COUNTRY");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter the country ID to delete: ");
                var country = await _countryRepository.GetByIdAsync(id);

                if (country == null)
                {
                    MainMenu.ShowMessage("\n❌ The country does not exist.", ConsoleColor.Red);
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nCountry Information:");
                    Console.WriteLine($"ID: {country.Id}");
                    Console.WriteLine($"Name: {country.Name}");
                    Console.ResetColor();
                    
                    string confirm = MainMenu.ReadText("\n⚠️ Are you sure you want to delete this country? (Y/N): ");
                    
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _countryRepository.DeleteAsync(id);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Country deleted successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to delete the country.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error deleting the country: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}