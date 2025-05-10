using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Application.UI
{
    public class CashFlowMenu
    {
        private readonly CashFlowRepository _cashFlowRepository;
        
        public CashFlowMenu(MySqlConnection connection)
        {
            _cashFlowRepository = new CashFlowRepository(connection);
        }
        
        public void ShowMenu()
        {
            bool returnTo = false;
            
            while (!returnTo)
            {
                Console.Clear();
                MainMenu.ShowHeader("💵 CASH FLOW MANAGEMENT");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║             📋 CASH FLOW MENU              ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ║     1️⃣  List Cash Flow           📋         ║");
                Console.WriteLine("  ║     2️⃣  Show Cash Flow Details   🔍         ║");
                Console.WriteLine("  ║     3️⃣  Register New Cash Flow   ➕         ║");
                Console.WriteLine("  ║     0️⃣  Return to Main Menu      ↩️          ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string option = MainMenu.ReadText("\n✨ Select an option: ");
                
                switch (option)
                {
                    case "1":
                        ListCashFlow().Wait();
                        break;
                    case "2":
                        ShowCashFlowDetail().Wait();
                        break;
                    case "3":
                        RegisterCashFlow().Wait();
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
        }
        
        private async Task ListCashFlow()
        {
            Console.Clear();
            MainMenu.ShowHeader("CASH FLOW LIST");
            
            try
            {
                var cashFlows = await _cashFlowRepository.GetAllAsync();
                
                if (!cashFlows.Any())
                {
                    MainMenu.ShowMessage("\nNo cash flow records found.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.WriteLine("\n{0,-5} {1,-12} {2,-20} {3,-15} {4,-15}", 
                        "ID", "Date", "Description", "Type", "Amount");
                    Console.WriteLine(new string('-', 80));
                    
                    foreach (var cashFlow in cashFlows)
                    {
                        Console.WriteLine("{0,-5} {1,-12} {2,-20} {3,-15} {4,-15}", 
                            cashFlow.Id, 
                            cashFlow.Date.ToString("dd/MM/yyyy"),
                            cashFlow.Concept.Length > 17 
                                ? cashFlow.Concept.Substring(0, 17) + "..." 
                                : cashFlow.Concept,
                            cashFlow.MovementNameType,
                            cashFlow.Cost.ToString("C"));
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError listing cash flow: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
        
        private async Task ShowCashFlowDetail()
        {
            Console.Clear();
            MainMenu.ShowHeader("CASH FLOW DETAIL");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter cash flow ID: ");
                
                var cashFlow = await _cashFlowRepository.GetByIdAsync(id);
                
                if (cashFlow == null)
                {
                    MainMenu.ShowMessage("\nCash flow record does not exist.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.WriteLine("\nCASH FLOW INFORMATION:");
                    Console.WriteLine($"ID: {cashFlow.Id}");
                    Console.WriteLine($"Date: {cashFlow.Date:dd/MM/yyyy}");
                    Console.WriteLine($"Concept: {cashFlow.Concept}");
                    Console.WriteLine($"Type Movement Id: {cashFlow.MovementTypeId}");
                    Console.WriteLine($"Cost: {cashFlow.Cost:C}");
                    Console.WriteLine($"Type Movement: {cashFlow.MovementType}");
                    Console.WriteLine($"ID Person: {cashFlow.PersonId}");
                    Console.WriteLine($"Movement Name: {cashFlow.MovementNameType}");
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError getting cash flow details: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
        
        private async Task RegisterCashFlow()
        {
            Console.Clear();
            MainMenu.ShowHeader("REGISTER NEW CASH FLOW");
            
            try
            {
                string concept = MainMenu.ReadText("\nConcept: ");
                int movementTypeId = MainMenu.ReadInteger("Type Movement Id: ");
                decimal cost = MainMenu.ReadPositiveDecimal("Cost: ");
                string movementNameType = MainMenu.ReadText("Movement Name Type: ");
                string movementType = MainMenu.ReadText("Movement Type: ");
                string personId = MainMenu.ReadText("Person Id: ");
                
                var cashFlow = new CashFlow
                {
                    Date = DateTime.Now,
                    Concept = concept,
                    MovementTypeId = movementTypeId,
                    Cost = cost,
                    PersonId = personId,
                    MovementNameType = movementNameType,
                    MovementType = movementType
                };
                
                bool result = await _cashFlowRepository.InsertAsync(cashFlow);
                if (result)
                {
                    MainMenu.ShowMessage("\nCash flow registered successfully.", ConsoleColor.Green);
                }
                else
                {
                    MainMenu.ShowMessage("\nCould not register cash flow.", ConsoleColor.Red);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError registering cash flow: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}