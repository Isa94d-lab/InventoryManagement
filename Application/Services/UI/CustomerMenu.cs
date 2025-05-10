using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Application.UI
{
    public class CustomerMenu
    {
        private readonly CustomerRepository _customerRepository;
        public CustomerMenu(MySqlConnection connection)
        {
            _customerRepository = new CustomerRepository(connection);
        }
        
        public void ShowMenu()
        {
            bool returnTo = false; 

            while (!returnTo)
            {
                Console.Clear();
                MainMenu.ShowHeader(" 🧔‍♂️ CUSTOMER MENU ");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║               🧔‍♂️ CUSTOMER MENU             ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ║       1️⃣  List Customers           📋       ║");
                Console.WriteLine("  ║       2️⃣  Create Customer          ➕       ║");
                Console.WriteLine("  ║       3️⃣  Update Customer          ✏️        ║");
                Console.WriteLine("  ║       4️⃣  Delete Customer          ✖️        ║");
                Console.WriteLine("  ║       0️⃣  Return to Person Menu     ↩️       ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                switch (option)
                {
                    case "1":
                        ListCustomers().Wait();
                        break;
                    case "2":
                        CreateCustomer().Wait();
                        break;
                    case "3":
                        UpdateCustomer().Wait();
                        break;
                    case "4":
                        DeleteCustomer().Wait();
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

        private async Task ListCustomers()
        {
            Console.Clear();
            MainMenu.ShowHeader("CUSTOMERS LIST");

            try
            {
                var customers = await _customerRepository.GetAllAsync();

                if (!customers.Any())
                {
                    MainMenu.ShowMessage("\nNo customers registered.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\n{0,-5} {1,-12} {2,-30} {3,-15} {4,-15}",
                        "ID", "Person ID", "Customer Name", "Birthdate", "Purchase Date");

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 95));
                    Console.ResetColor();

                    foreach (var customer in customers)
                    {
                        string customerName = customer.Person?.FullName ?? "N/A";
                        if (customerName.Length > 27)
                        {
                            customerName = customerName.Substring(0, 24) + "...";
                        }

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("{0,-5} {1,-12} {2,-30} {3,-15} {4,-15}",
                            customer.Id,
                            customer.PersonId,
                            customerName,
                            customer.Birthdate.ToString("dd/MM/yyyy"),
                            customer.PurchaseDate.HasValue ? customer.PurchaseDate.Value.ToString("dd/MM/yyyy") : "N/A");
                    }

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 95));
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error listing customers: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task CreateCustomer()
        {
            Console.Clear();
            MainMenu.ShowHeader("REGISTER NEW CUSTOMER");

            try
            {
                string personId = MainMenu.ReadText("\nPerson ID: ").Trim();
                if (string.IsNullOrEmpty(personId))
                {
                    MainMenu.ShowMessage("Person ID cannot be empty.", ConsoleColor.Red);
                    return;
                }

                DateTime birthDate = MainMenu.ReadDate("Birthdate (DD/MM/YYYY): ");
                if (birthDate > DateTime.Now)
                {
                    MainMenu.ShowMessage("Birthdate cannot be in the future.", ConsoleColor.Red);
                    return;
                }

                DateTime purchaseDate = MainMenu.ReadDate("Purchase Date (DD/MM/YYYY): ");
                if (purchaseDate > DateTime.Now)
                {
                    MainMenu.ShowMessage("Purchase Date cannot be in the future.", ConsoleColor.Red);
                    return;
                }
               

                var customer = new Customer
                {
                    PersonId = personId,
                    Birthdate = birthDate,
                    PurchaseDate = purchaseDate
                };

                Console.Clear();
                MainMenu.ShowHeader("CUSTOMER INFORMATION");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\nPerson ID: {personId}");
                Console.WriteLine($"Birthdate: {birthDate:dd/MM/yyyy}");
                Console.WriteLine($"Purchase Date: {purchaseDate:dd/MM/yyyy}");

                string confirm = MainMenu.ReadText("\nDo you want to register this customer? (Y/N): ");
                if (confirm.ToUpper() == "Y")
                {
                    bool result = await _customerRepository.InsertAsync(customer);

                    if (result)
                    {
                        MainMenu.ShowMessage("\n✅ Customer registered successfully.", ConsoleColor.Green);
                    }
                    else
                    {
                        MainMenu.ShowMessage("\n❌ Customer registration failed.", ConsoleColor.Red);
                    }
                }
                else
                {
                    MainMenu.ShowMessage("\n⚠️ Operation cancelled.", ConsoleColor.Yellow);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error registering customer: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task UpdateCustomer()
        {
            Console.Clear();
            MainMenu.ShowHeader("UPDATE CUSTOMER");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter customer ID to update: ");
                var customer = await _customerRepository.GetByIdAsync(id);

                if (customer == null)
                {
                    MainMenu.ShowMessage("\n❌ The customer doesn't exist", ConsoleColor.Red);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nCurrent Information:");
                    Console.WriteLine($"Person ID: {customer.PersonId}");
                    Console.WriteLine($"Birthdate: {customer.Birthdate:dd/MM/yyyy}");
                    Console.WriteLine($"Purchase Date: {customer.PurchaseDate:dd/MM/yyyy}");
                    Console.ResetColor();

                    // Update Birthdate
                    DateTime birthdate = MainMenu.ReadDate($"\nNew Birthdate (DD/MM/YYYY) [{customer.Birthdate:dd/MM/yyyy}]: ");
                    if (birthdate != customer.Birthdate)
                    {
                        if (birthdate > DateTime.Now)
                        {
                            MainMenu.ShowMessage("Birthdate cannot be in the future.", ConsoleColor.Red);
                            return;
                        }
                        customer.Birthdate = birthdate;
                    }

                    // Update Purchase Date
                    DateTime purchaseDate = MainMenu.ReadDate($"\nNew Purchase Date (DD/MM/YYYY) [{customer.PurchaseDate:dd/MM/yyyy}]: ");
                    if (purchaseDate != customer.PurchaseDate)
                    {
                        if (purchaseDate > DateTime.Now)
                        {
                            MainMenu.ShowMessage("Purchase date cannot be in the future.", ConsoleColor.Red);
                            return;
                        }
                        customer.PurchaseDate = purchaseDate;
                    }

                    Console.Clear();
                    MainMenu.ShowHeader("UPDATED CUSTOMER INFORMATION");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nPerson ID: {customer.PersonId}");
                    Console.WriteLine($"Birthdate: {customer.Birthdate:dd/MM/yyyy}");
                    Console.WriteLine($"Purchase Date: {customer.PurchaseDate:dd/MM/yyyy}");
                    Console.ResetColor();

                    string confirm = MainMenu.ReadText("\nDo you want to save these changes? (Y/N): ");
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _customerRepository.UpdateAsync(customer);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Customer updated successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to update the customer.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error updating the customer: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task DeleteCustomer()
        {
            Console.Clear();
            MainMenu.ShowHeader("DELETE CUSTOMER");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter the customer ID to delete: ");
                var customer = await _customerRepository.GetByIdAsync(id);

                if (customer == null)
                {
                    MainMenu.ShowMessage("\n❌ The customer does not exist.", ConsoleColor.Red);
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nCustomer Information:");
                    Console.WriteLine($"ID: {customer.Id}");
                    Console.WriteLine($"Person ID: {customer.PersonId}");
                    Console.WriteLine($"Birthdate: {customer.Birthdate:dd/MM/yyyy}");
                    Console.WriteLine($"Purchase Date: {customer.PurchaseDate:dd/MM/yyyy}");
                    Console.ResetColor();
                    
                    string confirm = MainMenu.ReadText("\n⚠️ Are you sure you want to delete this customer? (Y/N): ");
                    
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _customerRepository.DeleteAsync(id);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Customer deleted successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to delete the customer.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error deleting the customer: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}