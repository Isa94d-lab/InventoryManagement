using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Application.UI
{
    public class PersonMenu
    {
        //private readonly PersonRepository _personRepository;
        public PersonMenu(MySqlConnection connection)
        {
            //_personRepository = new PersonRepository(connection);
        }
        
        public void ShowMenu()
        {
            bool returnTo = false; 

            while (!returnTo)
            {
                Console.Clear();
                MainMenu.ShowHeader(" 👤 PERSON MENU ");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║                👤 PERSON MENU              ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ║       1️⃣  Customers           📋          ║");
                Console.WriteLine("  ║       2️⃣  Supplier          ➕            ║");
                Console.WriteLine("  ║       3️⃣  Employee         ✏️             ║");
                Console.WriteLine("  ║       0️⃣  Return to Main Menu     ↩️      ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                switch (option)
                {
                    case "1":
                        //_customerMenu.ShowMenu();
                        break;
                    case "2":
                        //_supplierMenu.ShowMenu();
                        break;
                    case "3":
                        //_employeeMenu.ShowMenu();
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