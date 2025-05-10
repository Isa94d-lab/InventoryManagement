using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Application.UI
{
    public class SupplierMenu
    {
        private readonly SupplierRepository _supplierRepository;
        public SupplierMenu(MySqlConnection connection)
        {
            _supplierRepository = new SupplierRepository(connection);
        }
        
        public void ShowMenu()
        {
            bool returnTo = false; 

            while (!returnTo)
            {
                Console.Clear();
                MainMenu.ShowHeader(" 🏭 SUPPLIER MENU ");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║                🏭 SUPPLIER MENU            ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ║       1️⃣  List Suppliers           📋       ║");
                Console.WriteLine("  ║       2️⃣  Create Supplier          ➕       ║");
                Console.WriteLine("  ║       3️⃣  Update Supplier          ✏️        ║");
                Console.WriteLine("  ║       4️⃣  Delete Supplier          ✖️        ║");
                Console.WriteLine("  ║       0️⃣  Return to Person Menu     ↩️       ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                switch (option)
                {
                    case "1":
                        ListSupplier().Wait();
                        break;
                    case "2":
                        CreateSupplier().Wait();
                        break;
                    case "3":
                        UpdateSupplier().Wait();
                        break;
                    case "4":
                        DeleteSupplier().Wait();
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

        private async Task ListSupplier()
        {
            Console.Clear();
            MainMenu.ShowHeader("SUPPLIERS LIST");

            try
            {
                var suppliers = await _supplierRepository.GetAllAsync();

                if (!suppliers.Any())
                {
                    MainMenu.ShowMessage("\nNo suppliers registered.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\n{0,-5} {1,-12} {2,-30} {3,-15} {4,-10}",
                        "ID", "Person ID", "Supplier Name", "Discount", "Pay Day");

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 95));
                    Console.ResetColor();

                    foreach (var supplier in suppliers)
                    {
                        string supplierName = supplier.Person?.FullName ?? "N/A";
                        if (supplierName.Length > 27)
                        {
                            supplierName = supplierName.Substring(0, 24) + "...";
                        }

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("{0,-5} {1,-12} {2,-30} {3,-15} {4,-10}",
                            supplier.Id,
                            supplier.PersonId,
                            supplierName,
                            supplier.Discount.ToString("C"),
                            supplier.PayDay);
                    }

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 95));
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error listing suppliers: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task CreateSupplier()
        {
            Console.Clear();
            MainMenu.ShowHeader("REGISTER NEW SUPPLIER");

            try
            {
                string personId = MainMenu.ReadText("\nPerson ID: ").Trim();
                if (string.IsNullOrEmpty(personId))
                {
                    MainMenu.ShowMessage("Person ID cannot be empty.", ConsoleColor.Red);
                    return;
                }

                decimal discount = MainMenu.ReadPositiveDecimal("Discount: ");
                if (discount <= 0)
                {
                    MainMenu.ShowMessage("Discount must be greater than zero.", ConsoleColor.Red);
                    return;
                }

                int payDay = MainMenu.ReadInteger("Pay Day (1-31): ");
                if (payDay < 1 || payDay > 31)
                {
                    MainMenu.ShowMessage("Pay Day must be between 1 and 31.", ConsoleColor.Red);
                    return;
                }

                var supplier = new Supplier
                {
                    PersonId = personId,
                    Discount = discount,
                    PayDay = payDay
                };

                Console.Clear();
                MainMenu.ShowHeader("SUPPLIER INFORMATION");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\nPerson ID: {personId}");
                Console.WriteLine($"Discount: {discount:C}");
                Console.WriteLine($"Pay Day: {payDay}");

                string confirm = MainMenu.ReadText("\nDo you want to register this supplier? (Y/N): ");
                if (confirm.ToUpper() == "Y")
                {
                    bool result = await _supplierRepository.InsertAsync(supplier);

                    if (result)
                    {
                        MainMenu.ShowMessage("\n✅ Supplier registered successfully.", ConsoleColor.Green);
                    }
                    else
                    {
                        MainMenu.ShowMessage("\n❌ Supplier registration failed.", ConsoleColor.Red);
                    }
                }
                else
                {
                    MainMenu.ShowMessage("\n⚠️ Operation cancelled.", ConsoleColor.Yellow);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error registering supplier: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task UpdateSupplier()
        {
            Console.Clear();
            MainMenu.ShowHeader("UPDATE SUPPLIER");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter supplier ID to update: ");
                var supplier = await _supplierRepository.GetByIdAsync(id);

                if (supplier == null)
                {
                    MainMenu.ShowMessage("\n❌ The supplier doesn't exist", ConsoleColor.Red);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nCurrent Information:");
                    Console.WriteLine($"Person ID: {supplier.PersonId}");
                    Console.WriteLine($"Discount: {supplier.Discount:C}");
                    Console.WriteLine($"Pay Day: {supplier.PayDay}");
                    Console.ResetColor();

                    // Update Discount
                    decimal newDiscount = MainMenu.ReadPositiveDecimal($"\nNew Discount [{supplier.Discount:C}]: ");
                    if (newDiscount != supplier.Discount)
                    {
                        if (newDiscount <= 0)
                        {
                            MainMenu.ShowMessage("Discount must be greater than zero.", ConsoleColor.Red);
                            return;
                        }
                        supplier.Discount = newDiscount;
                    }

                    // Update Pay Day
                    int newPayDay = MainMenu.ReadInteger($"\nNew Pay Day (1-31) [{supplier.PayDay}]: ");
                    if (newPayDay != supplier.PayDay)
                    {
                        if (newPayDay < 1 || newPayDay > 31)
                        {
                            MainMenu.ShowMessage("Pay Day must be between 1 and 31.", ConsoleColor.Red);
                            return;
                        }
                        supplier.PayDay = newPayDay;
                    }

                    Console.Clear();
                    MainMenu.ShowHeader("UPDATED SUPPLIER INFORMATION");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nPerson ID: {supplier.PersonId}");
                    Console.WriteLine($"Discount: {supplier.Discount:C}");
                    Console.WriteLine($"Pay Day: {supplier.PayDay}");
                    Console.ResetColor();

                    string confirm = MainMenu.ReadText("\nDo you want to save these changes? (Y/N): ");
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _supplierRepository.UpdateAsync(supplier);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Supplier updated successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to update the supplier.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error updating the supplier: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task DeleteSupplier()
        {
            Console.Clear();
            MainMenu.ShowHeader("DELETE SUPPLIER");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter the Supplier ID to delete: ");
                var supplier = await _supplierRepository.GetByIdAsync(id);

                if (supplier == null)
                {
                    MainMenu.ShowMessage("\n❌ The supplier does not exist.", ConsoleColor.Red);
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nSupplier Information:");
                    Console.WriteLine($"ID: {supplier.Id}");
                    Console.WriteLine($"EPS ID: {supplier.PersonId}");
                    Console.WriteLine($"Base Salary: {supplier.Discount:C}");
                    Console.WriteLine($"ARL ID: {supplier.PayDay}");
                    Console.ResetColor();
                    
                    string confirm = MainMenu.ReadText("\n⚠️ Are you sure you want to delete this supplier? (Y/N): ");
                    
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _supplierRepository.DeleteAsync(id);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Supplier deleted successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to delete the supplier.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error deleting the supplier: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}