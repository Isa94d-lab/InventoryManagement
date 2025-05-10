using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Repositories;
using InventoryManagement.Infrastructure.Configuration;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Application.UI
{
    public class LocationMenu
    {
        private readonly CityMenu _cityMenu;
        private readonly RegionMenu _regionMenu;
        private readonly CountryMenu _countryMenu;

        public LocationMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "📦 Inventory Management System";

            var connection = DatabaseConfig.GetConnection();
            _cityMenu = new CityMenu(connection);
            _countryMenu = new CountryMenu(connection);
            _regionMenu = new RegionMenu(connection);
        }
        
        public void ShowMenu()
        {
            bool returnTo = false; 

            while (!returnTo)
            {
                Console.Clear();
                MainMenu.ShowHeader(" 📍 LOCATION MENU ");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║                📍 LOCATION MENU            ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ║       1️⃣  Country           🌎              ║");
                Console.WriteLine("  ║       2️⃣  Region            🏞️               ║");
                Console.WriteLine("  ║       3️⃣  City              🗼              ║");
                Console.WriteLine("  ║       0️⃣  Return to Main Menu   ↩️           ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                switch (option)
                {
                    case "1":
                        _countryMenu.ShowMenu();
                        break;
                    case "2":
                        _regionMenu.ShowMenu();
                        break;
                    case "3":
                        _cityMenu.ShowMenu();
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
    }
}