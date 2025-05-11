using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Repositories;
using MySql.Data.MySqlClient;
using InventoryManagement.Application.UI;
namespace InventoryManagement.Application.UI
{
    public class SaleMenu
    {
        private readonly SaleRepository _saleRepository;
        private readonly ProductRepository _productRepository;
        private readonly PlanRepository _planRepository;
        
        public SaleMenu(MySqlConnection connection)
        {
            _saleRepository = new SaleRepository(connection);
            _productRepository = new ProductRepository(connection);
            _planRepository = new PlanRepository(connection);
        }
        
        public void ShowMenu()
        {
            bool returnTo = false;
            
            while (!returnTo)
            {
                Console.Clear();
                MainMenu.ShowHeader("💰 SALES MANAGEMENT");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║             📋 SALES MENU                  ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ║       1️⃣  List Sales              📋        ║");
                Console.WriteLine("  ║       2️⃣  Show Sale Details       🔍        ║");
                Console.WriteLine("  ║       3️⃣  Register New Sale       ➕        ║");
                Console.WriteLine("  ║       4️⃣  Delete Sale             ❌        ║");
                Console.WriteLine("  ║       5️⃣  Update Sale             ✏️         ║");
                Console.WriteLine("  ║       0️⃣  Return to Main Menu     ↩️         ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                switch (option)
                {
                    case "1":
                        ListSales().Wait();
                        break;
                    case "2":
                        ShowSaleDetail().Wait();
                        break;
                    case "3":
                        RegisterSale().Wait();
                        break;
                    case "4":
                        DeleteSale().Wait();
                        break;
                    case "5":
                        UpdateSale().Wait();
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
        
        private async Task ListSales()
        {
            Console.Clear();
            MainMenu.ShowHeader("LIST SALES");
            
            try
            {
                var sales = await _saleRepository.GetAllAsync();
                
                if (!sales.Any())
                {
                    MainMenu.ShowMessage("\nThere aren't sales.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.WriteLine("\n{0,-10} {1,-12} {2,-20} {3,-20} {4,-15}", 
                        "Invoice", "Date", "Customer", "Employee", "Total");
                    Console.WriteLine(new string('-', 80));
                    
                    foreach (var sale in sales)
                    {
                        Console.WriteLine("{0,-10} {1,-12} {2,-20} {3,-20} {4,-15}", 
                            sale.InvoiceId, 
                            sale.Date.ToString("dd/MM/yyyy"),
                            sale.Customer?.FullName.Length > 17 
                                ? sale.Customer.FullName.Substring(0, 17) + "..." 
                                : sale.Customer?.FullName,
                            sale.Employee?.FullName.Length > 17 
                                ? sale.Employee.FullName.Substring(0, 17) + "..." 
                                : sale.Employee?.FullName,
                            sale.Total.ToString("C"));
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError listing sales: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
        
        private async Task ShowSaleDetail()
        {
            Console.Clear();
            MainMenu.ShowHeader("SALE DETAIL");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter the invoice number: ");
                
                var sale = await _saleRepository.GetByIdAsync(id);
                
                if (sale == null)
                {
                    MainMenu.ShowMessage("\nThe sale doesn't exist.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.WriteLine("\nSALE INFORMATION:");
                    Console.WriteLine($"Invoice: {sale.InvoiceId}");
                    Console.WriteLine($"Date: {sale.Date:dd/MM/yyyy}");
                    Console.WriteLine($"Customer: {sale.Customer?.FullName}");
                    Console.WriteLine($"Employee: {sale.Employee?.FullName}");
                    
                    Console.WriteLine("\nPRODUCTS DETAILS:");
                    Console.WriteLine("{0,-20} {1,-10} {2,-15} {3,-15}", 
                        "Product", "Quantity", "Valor Unit.", "Subtotal");
                    Console.WriteLine(new string('-', 65));
                    
                    foreach (var detail in sale.Details)
                    {
                        Console.WriteLine("{0,-20} {1,-10} {2,-15} {3,-15}", 
                            detail.Product?.Name.Length > 17 
                                ? detail.Product.Name.Substring(0, 17) + "..." 
                                : detail.Product?.Name,
                            detail.Quantity,
                            detail.Cost.ToString("C"),
                            detail.Subtotal.ToString("C"));
                    }
                    
                    Console.WriteLine(new string('-', 65));
                    Console.WriteLine("{0,-47} {1,-15}", "TOTAL:", sale.Total.ToString("C"));
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError obtaining the sale: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
        
        private async Task RegisterSale()
        {
            Console.Clear();
            MainMenu.ShowHeader("NEW SALE");
            
            try
            {
                // Obtener planes promocionales vigentes
                var activePlans = await _planRepository.GetPlanesVigentesAsync();
                Dictionary<string, decimal> productDiscounts = new Dictionary<string, decimal>();
                
                if (activePlans.Any())
                {
                    MainMenu.ShowMessage("\nPROMOTIONAL PLANS AVAILABLE:", ConsoleColor.Green);
                    foreach (var plan in activePlans)
                    {
                        Console.WriteLine($"- {plan.Name}: {plan.Discount:P}");
                        
                        // Registrar descuentos por producto
                        foreach (var product in plan.Products)
                        {
                            productDiscounts[product.Id] = plan.Discount;
                        }
                    }
                }
                else
                {
                    MainMenu.ShowMessage("\nThere aren't promotional plans available.", ConsoleColor.Yellow);
                }
                
                // Capturar información general de la venta
                string customerId = MainMenu.ReadText("\nCustomer ID: ");
                string employeeId = MainMenu.ReadText("Employee ID: ");
                
                // Crear la venta
                var sale = new Sale
                {
                    CustomerPersonId = customerId,
                    EmployeePersonId = employeeId,
                    Date = DateTime.Now,
                    Details = new List<SaleDetail>()
                };
                
                // Capturar productos
                bool addMoreProducts = true;
                while (addMoreProducts)
                {
                    string productId = MainMenu.ReadText("\nProduct ID: ");
                    var product = await _productRepository.GetByIdAsync(productId);
                    
                    if (product == null)
                    {
                        MainMenu.ShowMessage("The product doesn't exist.", ConsoleColor.Red);
                        continue;
                    }
                    
                    if (product.Stock <= 0)
                    {
                        MainMenu.ShowMessage("There aren't stock available.", ConsoleColor.Red);
                        continue;
                    }
                    
                    int quantity = MainMenu.ReadInteger("Quantity: ");
                    if (quantity > product.Stock)
                    {
                        MainMenu.ShowMessage("The quantity exceed the available stock.", ConsoleColor.Red);
                        continue;
                    }
                    
                    decimal unitPrice = MainMenu.ReadPositiveDecimal("Unity Cost: ");
                    decimal discount = productDiscounts.ContainsKey(productId) ? productDiscounts[productId] : 0;
                    
                    var detail = new SaleDetail
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        Cost = unitPrice
                    };
                    
                    sale.Details.Add(detail);
                    
                    string addMore = MainMenu.ReadText("\nDo you want to add other product? (Y/N): ");
                    addMoreProducts = addMore.ToUpper() == "Y";
                }
                
                if (sale.Details.Any())
                {
                    try
                    {
                        bool result = await _saleRepository.InsertAsync(sale);
                        if (result)
                        {
                            MainMenu.ShowMessage("\nSale has been registered successfully! 🎉", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\nError registering the sale.", ConsoleColor.Red);
                        }
                    }
                    catch (MySql.Data.MySqlClient.MySqlException ex)
                    {
                        if (ex.Number == 1062) // Error de duplicado
                        {
                            MainMenu.ShowMessage("\nError: This sale ID already exists. Please try again.", ConsoleColor.Red);
                        }
                        else
                        {
                            throw; // Re-lanzar otras excepciones
                        }
                    }
                }
                else
                {
                    MainMenu.ShowMessage("\nNo products were added to the sale.", ConsoleColor.Yellow);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError registering the sale: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }


        private async Task DeleteSale()
        {
            Console.Clear();
            MainMenu.ShowHeader("DELETE SALE");

            try
            {
                int id = MainMenu.ReadInteger("\nEnter the sale ID to delete: ");
                var sale = await _saleRepository.GetByIdAsync(id);

                if (sale == null)
                {
                    MainMenu.ShowMessage("\n❌ The sale does not exist.", ConsoleColor.Red);
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nSale Information:");
                    Console.WriteLine($"ID: {sale.InvoiceId}");
                    Console.WriteLine($"Date: {sale.Date}");
                    Console.WriteLine($"Employee ID: {sale.EmployeePersonId}");
                    Console.WriteLine($"Customer ID: {sale.CustomerPersonId}");
                    Console.WriteLine($"Total: {sale.Total}");
                    Console.ResetColor();
                    
                    string confirm = MainMenu.ReadText("\n⚠️ Are you sure you want to delete this sale? (Y/N): ");
                    
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _saleRepository.DeleteAsync(id);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Sale deleted successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to delete the sale.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error deleting the sale: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task UpdateSale()
        {
            Console.Clear();
            MainMenu.ShowHeader("UPDATE SALE");

            try
            {
                int id = MainMenu.ReadInteger("\nEnter sale ID to update: ");
                var sale = await _saleRepository.GetByIdAsync(id);

                if (sale == null)
                {
                    MainMenu.ShowMessage("\n❌ The sale doesn't exist", ConsoleColor.Red);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nCurrent Information:");
                    Console.WriteLine($"ID: {sale.InvoiceId}");
                    Console.WriteLine($"Date: {sale.Date:dd/MM/yyyy}");
                    Console.WriteLine($"Employee ID: {sale.EmployeePersonId}");
                    Console.WriteLine($"Customer ID: {sale.CustomerPersonId}");
                    Console.ResetColor();

                    DateTime date = MainMenu.ReadDate($"\nNew Date (DD/MM/YYYY) [{sale.Date:dd/MM/yyyy}]: ");
                    if (date != sale.Date)
                    {
                        if (date > DateTime.Now)
                        {
                            MainMenu.ShowMessage("Date cannot be in the future.", ConsoleColor.Red);
                            return;
                        }
                        sale.Date = date;
                    }

                    string employeeId = MainMenu.ReadText($"Enter Employee ID ({sale.EmployeePersonId}): ");
                    if (!string.IsNullOrWhiteSpace(employeeId))
                    {
                        sale.EmployeePersonId = employeeId;
                    }

                    string customerId = MainMenu.ReadText($"Enter Customer ID ({sale.CustomerPersonId}): ");
                    if (!string.IsNullOrWhiteSpace(customerId))
                    {
                        sale.CustomerPersonId = customerId;
                    }

                    string confirm = MainMenu.ReadText("\nDo you want to save these changes? (Y/N): ");
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _saleRepository.UpdateAsync(sale);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Sale updated successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to update the sale.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error updating the sale: {ex.Message}", ConsoleColor.Red);
            }

             Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}