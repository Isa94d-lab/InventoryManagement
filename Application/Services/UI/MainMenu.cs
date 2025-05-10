using System;
using System.Threading.Tasks;
using InventoryManagement.Infrastructure.Repositories;
using InventoryManagement.Infrastructure.Configuration;
using MySql.Data.MySqlClient;
using InventoryManagement.Application.UI;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.UI
{
    public class MainMenu
    {
        private readonly ProductsMenu _productsMenu;
        private readonly SaleMenu _saleMenu;
        private readonly PurchaseMenu _purchaseMenu;
        private readonly CashFlowMenu _cashFlowMenu;
        private readonly PlanMenu _planMenu;
        private readonly PersonMenu _personMenu;
        
        public MainMenu()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "📦 Inventory Management System";

            var connection = DatabaseConfig.GetConnection();
            _productsMenu = new ProductsMenu(connection);
            _saleMenu = new SaleMenu(connection);
            _purchaseMenu = new PurchaseMenu(connection);
            _cashFlowMenu = new CashFlowMenu(connection);
            _planMenu = new PlanMenu(connection);
            _personMenu = new PersonMenu();
        }

        public void ShowMenu()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                ShowHeader("📦 INVENTORY MANAGEMENT SYSTEM");

                // Menu border and title
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║                📋 MAIN MENU                ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");

                // Menu options with emojis
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ║     1️⃣  Products Management    🛒           ║");
                Console.WriteLine("  ║     2️⃣  Sales Management       💰           ║");
                Console.WriteLine("  ║     3️⃣  Purchases Management   📥           ║");
                Console.WriteLine("  ║     4️⃣  Cash Flow              💵           ║");
                Console.WriteLine("  ║     5️⃣  Promotional Plans      🎁           ║");
                Console.WriteLine("  ║     6️⃣  Person                 👤           ║");
                Console.WriteLine("  ║     0️⃣  Exit                   ❌           ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string option = ReadText("\n✨ Select an option: ");

                switch (option)
                {
                    case "1":
                        _productsMenu.ShowMenu();
                        break;
                    case "2":
                        _saleMenu.ShowMenu();
                        break;
                    case "3":
                        _purchaseMenu.ShowMenu();
                        break;
                    case "4":
                        _cashFlowMenu.ShowMenu();
                        break;
                    case "5":
                        _planMenu.ShowMenu();
                        break;
                    case "6":
                        _personMenu.ShowMenu();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        ShowMessage("⚠️ Invalid option. Please try again.", ConsoleColor.Red);
                        Console.ReadKey();
                        break;
                }
            }

            ShowMessage("\n👋 Thank you for using the application! Have a great day! 🌟", ConsoleColor.Green);
        }

        public static void ShowHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            string border = new string('═', title.Length + 4);
            Console.WriteLine($"╔{border}╗");
            Console.WriteLine($"║  {title}  ║");
            Console.WriteLine($"╚{border}╝");
            Console.ResetColor();
        }

        public static void ShowMessage(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"\n{message}");
            Console.ResetColor();
        }

        public static string ReadText(string prompt)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(prompt);
            Console.ResetColor();
            return Console.ReadLine() ?? "";
        }

        public static int ReadInteger(string prompt)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(prompt);
                Console.ResetColor();
                if (int.TryParse(Console.ReadLine(), out int value) && value >= 0)
                {
                    return value;
                }

                ShowMessage("❌ Error: Please enter a positive integer.", ConsoleColor.Red);
            }
        }

        public static decimal ReadPositiveDecimal(string prompt)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(prompt);
                Console.ResetColor();
                if (decimal.TryParse(Console.ReadLine(), out decimal value) && value >= 0)
                {
                    return value;
                }

                ShowMessage("❌ Error: Please enter a positive decimal number.", ConsoleColor.Red);
            }
        }

        public static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(prompt);
                Console.ResetColor();
                if (DateTime.TryParse(Console.ReadLine(), out DateTime date))
                {
                    return date;
                }

                ShowMessage("❌ Error: Please enter a valid date (DD/MM/YYYY).", ConsoleColor.Red);
            }
        }
    }
}