using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Application.UI
{
    public class PurchaseMenu
    {
        private readonly PurchaseRepository _purchaseRepository;
        private readonly ProductRepository _productRepository;
        
        public PurchaseMenu(MySqlConnection connection)
        {
            _purchaseRepository = new PurchaseRepository(connection);
            _productRepository = new ProductRepository(connection);
        }
        
        public void ShowMenu()
        {
            bool returnTo = false;

            while (!returnTo)
            {
                Console.Clear();
                MainMenu.ShowHeader("📥 PURCHASE MANAGEMENT");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║               📋 PURCHASE MENU             ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ║       1️⃣  List Purchases          📋        ║");
                Console.WriteLine("  ║       2️⃣  View Purchase Details   🔍        ║");
                Console.WriteLine("  ║       3️⃣  Register New Purchase   ➕        ║");
                Console.WriteLine("  ║       4️⃣  Delete Purchase         ❌        ║");
                Console.WriteLine("  ║       5️⃣  Update Sale             ✏️         ║");
                Console.WriteLine("  ║       0️⃣  Return to Main Menu     ↩️         ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                switch (option)
                {
                    case "1":
                        ListPurchases().Wait();
                        break;
                    case "2":
                        ShowPurchaseDetail().Wait();
                        break;
                    case "3":
                        RegisterPurchase().Wait();
                        break;
                    case "4":
                        DeletePurchase().Wait();
                        break;
                    case "5":
                        UpdatePurchase().Wait();
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

        private async Task ListPurchases()
        {
            Console.Clear();
            MainMenu.ShowHeader("PURCHASES LIST");

            try
            {
                var purchases = await _purchaseRepository.GetAllAsync();

                if (!purchases.Any())
                {
                    MainMenu.ShowMessage("\nNo purchases registered.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\n{0,-5} {1,-12} {2,-20} {3,-20} {4,-15}",
                        "ID", "Date", "Supplier", "Employee", "Total");

                    Console.ForegroundColor = ConsoleColor.DarkMagenta; // Purple
                    Console.WriteLine(new string('-', 80));
                    Console.ResetColor();

                    foreach (var purchase in purchases)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("{0,-5} {1,-12} ", purchase.Id, purchase.Date.ToString("dd/MM/yyyy"));

                        Console.ForegroundColor = ConsoleColor.DarkRed; // Dark Red
                        Console.Write("{0,-20} ",
                            purchase.Supplier?.FullName.Length > 17
                                ? purchase.Supplier.FullName.Substring(0, 17) + "..."
                                : purchase.Supplier?.FullName);

                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.Write("{0,-20} ",
                            purchase.Employee?.FullName.Length > 17
                                ? purchase.Employee.FullName.Substring(0, 17) + "..."
                                : purchase.Employee?.FullName);

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("{0,-15}", purchase.Total.ToString("C"));

                        Console.ResetColor();
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError listing purchases: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task ShowPurchaseDetail()
        {
            Console.Clear();
            MainMenu.ShowHeader("PURCHASE DETAIL");

            try
            {
                int id = MainMenu.ReadInteger("\nEnter the purchase ID: ");
                var purchase = await _purchaseRepository.GetByIdAsync(id);

                if (purchase == null)
                {
                    MainMenu.ShowMessage("\nPurchase not found.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\nPURCHASE INFORMATION:");
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine($"ID: {purchase.Id}");
                    Console.WriteLine($"Document: {purchase.PurOrder}");
                    Console.WriteLine($"Date: {purchase.Date:dd/MM/yyyy}");
                    Console.WriteLine($"Supplier: {purchase.Supplier?.FullName}");
                    Console.WriteLine($"Employee: {purchase.Employee?.FullName}");
                    Console.ResetColor();

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\nPRODUCT DETAILS:");
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("{0,-20} {1,-10} {2,-15} {3,-15}",
                        "Product", "Quantity", "Unit Price", "Subtotal");
                    Console.WriteLine(new string('-', 65));
                    Console.ResetColor();

                    foreach (var detail in purchase.Details)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("{0,-20} {1,-10} {2,-15} {3,-15}",
                            detail.Product?.Name.Length > 17
                                ? detail.Product.Name.Substring(0, 17) + "..."
                                : detail.Product?.Name,
                            detail.Quantity,
                            detail.Cost.ToString("C"),
                            detail.Subtotal.ToString("C"));
                        Console.ResetColor();
                    }

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 65));
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("{0,-47} {1,-15}", "TOTAL:", purchase.Total.ToString("C"));
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError retrieving purchase detail: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task RegisterPurchase()
        {
            Console.Clear();
            MainMenu.ShowHeader("REGISTER NEW PURCHASE");

            try
            {
                string supplierId = MainMenu.ReadText("\nSupplier ID: ");
                string employeeId = MainMenu.ReadText("Employee ID: ");
                string purchaseDoc = MainMenu.ReadText("Purchase document: ");

                var purchase = new Purchase
                {
                    SupplierPersonId = supplierId,
                    EmployeePersonId = employeeId,
                    Date = DateTime.Now,
                    PurOrder = purchaseDoc,
                    Details = new List<PurchaseDetail>()
                };

                bool addMoreProducts = true;
                while (addMoreProducts)
                {
                    Console.Clear();
                    MainMenu.ShowHeader("ADD PRODUCT TO PURCHASE");

                    string productId = MainMenu.ReadText("\nProduct ID: ");
                    var product = await _productRepository.GetByIdAsync(productId);

                    if (product == null)
                    {
                        MainMenu.ShowMessage("\nProduct not found.", ConsoleColor.Yellow);
                        Console.ReadKey();
                        continue;
                    }

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"Product: {product.Name}");
                    Console.ResetColor();

                    int quantity = MainMenu.ReadInteger("Quantity: ");
                    decimal unitPrice = MainMenu.ReadPositiveDecimal("Unit price: ");

                    purchase.Details.Add(new PurchaseDetail
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        Cost = unitPrice,
                        Date = purchase.Date,
                        Product = product
                    });

                    string response = MainMenu.ReadText("\nAdd another product? (Y/N): ");
                    addMoreProducts = response.ToUpper() == "Y";
                }

                Console.Clear();
                MainMenu.ShowHeader("PURCHASE SUMMARY");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\nSupplier: {supplierId}");
                Console.WriteLine($"Employee: {employeeId}");
                Console.WriteLine($"Document: {purchaseDoc}");
                Console.WriteLine($"Date: {purchase.Date:dd/MM/yyyy}");

                Console.WriteLine("\nPRODUCTS:");
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("{0,-20} {1,-10} {2,-15} {3,-15}",
                    "Product", "Quantity", "Unit Price", "Subtotal");
                Console.WriteLine(new string('-', 65));
                Console.ResetColor();

                decimal total = 0;
                foreach (var detail in purchase.Details)
                {
                    decimal subtotal = detail.Quantity * detail.Cost;
                    total += subtotal;

                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("{0,-20} {1,-10} {2,-15} {3,-15}",
                        detail.Product?.Name.Length > 17
                            ? detail.Product?.Name.Substring(0, 17) + "..."
                            : detail.Product?.Name,
                        detail.Quantity,
                        detail.Cost.ToString("C"),
                        subtotal.ToString("C"));
                    Console.ResetColor();
                }

                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine(new string('-', 65));
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("{0,-47} {1,-15}", "TOTAL:", total.ToString("C"));
                Console.ResetColor();

                string confirm = MainMenu.ReadText("\nDo you want to register this purchase? (Y/N): ");
                if (confirm.ToUpper() == "Y")
                {
                    bool result = await _purchaseRepository.InsertAsync(purchase);

                    if (result)
                    {
                        MainMenu.ShowMessage("\nPurchase registered successfully.", ConsoleColor.Green);
                    }
                    else
                    {
                        MainMenu.ShowMessage("\nPurchase registration failed.", ConsoleColor.Red);
                    }
                }
                else
                {
                    MainMenu.ShowMessage("\nOperation cancelled.", ConsoleColor.Yellow);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError registering purchase: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }


        public async Task DeletePurchase()
        {
            Console.Clear();
            MainMenu.ShowHeader("DELETE PURCHASE");

            try
            {
                int id = MainMenu.ReadInteger("\nEnter the purchase ID to delete: ");
                var purchase = await _purchaseRepository.GetByIdAsync(id);

                if (purchase == null)
                {
                    MainMenu.ShowMessage("\n❌ The purchase does not exist.", ConsoleColor.Red);
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nPurchase Information:");
                    Console.WriteLine($"ID: {purchase.Id}");
                    Console.WriteLine($"Date: {purchase.Date}");
                    Console.WriteLine($"Employee ID: {purchase.EmployeePersonId}");
                    Console.WriteLine($"Order: {purchase.PurOrder}");
                    Console.WriteLine($"Total: {purchase.Total}");
                    Console.ResetColor();
                    
                    string confirm = MainMenu.ReadText("\n⚠️ Are you sure you want to delete this purchase? (Y/N): ");
                    
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _purchaseRepository.DeleteAsync(id);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Purchase deleted successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to delete the purchase.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error deleting the purchase: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task UpdatePurchase()
        {
            Console.Clear();
            MainMenu.ShowHeader("UPDATE SALE");

            try
            {
                int id = MainMenu.ReadInteger("\nEnter sale ID to update: ");
                var purchase = await _purchaseRepository.GetByIdAsync(id);

                if (purchase == null)
                {
                    MainMenu.ShowMessage("\n❌ The purchase doesn't exist", ConsoleColor.Red);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nCurrent Information:");
                    Console.WriteLine($"ID: {purchase.Id}");
                    Console.WriteLine($"Date: {purchase.Date:dd/MM/yyyy}");
                    Console.WriteLine($"Employee ID: {purchase.EmployeePersonId}");
                    Console.WriteLine($"Supplier ID: {purchase.SupplierPersonId}");
                    Console.WriteLine($"Customer ID: {purchase.PurOrder}");
                    Console.ResetColor();

                    DateTime date = MainMenu.ReadDate($"\nNew Date (DD/MM/YYYY) [{purchase.Date:dd/MM/yyyy}]: ");
                    if (date != purchase.Date)
                    {
                        if (date > DateTime.Now)
                        {
                            MainMenu.ShowMessage("Date cannot be in the future.", ConsoleColor.Red);
                            return;
                        }
                        purchase.Date = date;
                    }

                    string employeeId = MainMenu.ReadText($"Enter Employee ID ({purchase.EmployeePersonId}): ");
                    if (!string.IsNullOrWhiteSpace(employeeId))
                    {
                        purchase.EmployeePersonId = employeeId;
                    }

                    string supplierId = MainMenu.ReadText($"Enter Supplier ID ({purchase.SupplierPersonId}): ");
                    if (!string.IsNullOrWhiteSpace(supplierId))
                    {
                        purchase.SupplierPersonId = supplierId;
                    }

                    string purOrder = MainMenu.ReadText($"Enter Order ({purchase.PurOrder}): ");
                    if (!string.IsNullOrWhiteSpace(purOrder))
                    {
                        purchase.PurOrder = purOrder;
                    }

                    string confirm = MainMenu.ReadText("\nDo you want to save these changes? (Y/N): ");
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _purchaseRepository.UpdateAsync(purchase);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Purchase dated successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to update the purchase.", ConsoleColor.Red);
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