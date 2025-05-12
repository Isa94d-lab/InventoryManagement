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
    public class SocialSecurityMenu
    {
        private readonly ArlMenu _arlMenu;
        private readonly EpsMenu _epsMenu;

        public SocialSecurityMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "📦 Inventory Management System";

            var connection = DatabaseConfig.GetConnection();
            _arlMenu = new ArlMenu(connection);
            _epsMenu = new EpsMenu(connection);
        }
        
        public void ShowMenu()
        {
            bool returnTo = false; 

            while (!returnTo)
            {
                Console.Clear();
                MainMenu.ShowHeader(" 🩺 SOCIAL SECURITY MENU ");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║          🩺 SOCIAL SECURITY MENU           ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ║       1️⃣  Arl                🪪              ║");
                Console.WriteLine("  ║       2️⃣  Eps                🧬             ║");
                Console.WriteLine("  ║       0️⃣  Return to Main Menu     ↩️         ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                switch (option)
                {
                    case "1":
                        _arlMenu.ShowMenu();
                        break;
                    case "2":
                        _epsMenu.ShowMenu();
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