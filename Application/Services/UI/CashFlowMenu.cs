using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Repositories;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;


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
                Console.WriteLine("  ║     4️⃣  Delete Cash Flow         ❌         ║");
                Console.WriteLine("  ║     5️⃣  Update Cash Flow         ✏️          ║");
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
                    case "4":
                        DeleteCashFlow().Wait();
                        break;
                    case "5":
                        UpdateCashFlow().Wait();
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

        
        private async Task DeleteCashFlow()
        {
            Console.Clear();
            MainMenu.ShowHeader("DELETE CASH FLOW");

            try
            {
                int id = MainMenu.ReadInteger("\nEnter the cash flow ID to delete: ");
                var cashFlow = await _cashFlowRepository.GetByIdAsync(id);

                if (cashFlow == null)
                {
                    MainMenu.ShowMessage("\n❌ The cash flow does not exist.", ConsoleColor.Red);
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nCash Flow Information:");
                    Console.WriteLine($"ID: {cashFlow.Id}");
                    Console.WriteLine($"Date: {cashFlow.Date}");
                    Console.WriteLine($"Cost: {cashFlow.Cost}");
                    Console.WriteLine($"Concept: {cashFlow.Concept}");
                    Console.ResetColor();
                    
                    string confirm = MainMenu.ReadText("\n⚠️ Are you sure you want to delete this cash flow? (Y/N): ");
                    
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _cashFlowRepository.DeleteAsync(id);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Cash flow deleted successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to delete the cash flow.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error deleting the cash flow: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task UpdateCashFlow()
        {
            Console.Clear();
            MainMenu.ShowHeader("UPDATE CASH FLOW");

            try
            {
                int id = MainMenu.ReadInteger("\nEnter cash flow ID to update: ");
                var cashFlow = await _cashFlowRepository.GetByIdAsync(id);

                if (cashFlow == null)
                {
                    MainMenu.ShowMessage("\n❌ The cash flow doesn't exist", ConsoleColor.Red);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nCurrent Information:");
                    Console.WriteLine($"ID: {cashFlow.Id}");
                    Console.WriteLine($"Birthdate: {cashFlow.Date:dd/MM/yyyy}");
                    Console.WriteLine($"Cost: {cashFlow.Cost}");
                    Console.WriteLine($"Concept: {cashFlow.Concept}");
                    Console.ResetColor();

                    // Update Date
                    DateTime date = MainMenu.ReadDate($"\nNew Date (DD/MM/YYYY) [{cashFlow.Date:dd/MM/yyyy}]: ");
                    if (date != cashFlow.Date)
                    {
                        if (date > DateTime.Now)
                        {
                            MainMenu.ShowMessage("Date cannot be in the future.", ConsoleColor.Red);
                            return;
                        }
                        cashFlow.Date = date;
                    }

                    int movementId = MainMenu.ReadInteger($"\nMovement Type [{cashFlow.MovementTypeId}]: ");
                    if (movementId != cashFlow.MovementTypeId)
                    {
                        if (movementId <= 0)
                        {
                            MainMenu.ShowMessage("Movement type ID must be greater than zero.", ConsoleColor.Red);
                            return;
                        }
                        cashFlow.MovementTypeId = movementId;
                    }

                    decimal cost = MainMenu.ReadPositiveDecimal($"\nNew Cost[{cashFlow.Cost:C}]: ");
                    if (cost != cashFlow.Cost)
                    {
                        if (cost <= 0)
                        {
                            MainMenu.ShowMessage("Cost must be greater than zero.", ConsoleColor.Red);
                            return;
                        }
                        cashFlow.Cost = cost;
                    }

                    string concepto = MainMenu.ReadText($"Enter new concept cash flow ({cashFlow.Concept}): ");
                    if (!string.IsNullOrWhiteSpace(concepto))
                    {
                        cashFlow.Concept = concepto;
                    }

                    string personId = MainMenu.ReadText($"\nPerson ID [{cashFlow.PersonId}]: ");
                    if (!string.IsNullOrWhiteSpace(personId))
                    {
                        cashFlow.PersonId = personId;
                    }

                    Console.Clear();
                    MainMenu.ShowHeader("UPDATED CASH FLOW INFORMATION");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"ID: {cashFlow.Id}");
                    Console.WriteLine($"\nDate: {cashFlow.Date:dd/MM/yyyy}");
                    Console.WriteLine($"Movement type ID: {cashFlow.MovementTypeId}");
                    Console.WriteLine($"Cost: {cashFlow.Cost:C}");
                    Console.WriteLine($"Concept: {cashFlow.Concept}");
                    Console.WriteLine($"Person ID: {cashFlow.PersonId}");
                    Console.ResetColor();

                    string confirm = MainMenu.ReadText("\nDo you want to save these changes? (Y/N): ");
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _cashFlowRepository.UpdateAsync(cashFlow);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Cash Flow updated successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to update the cashFlow.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error updating the cash flow: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}