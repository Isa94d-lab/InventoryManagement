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
    public class EmployeeMenu
    {
        private readonly EmployeeRepository _employeeRepository;

        public EmployeeMenu(MySqlConnection connection)
        {
            _employeeRepository = new EmployeeRepository(connection);
        }

        public void ShowMenu()
        {
            bool returnTo = false; 

            while (!returnTo)
            {
                Console.Clear();
                MainMenu.ShowHeader(" 🧑 EMPLOYEE MENU ");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║            🙍 EMPLOYEE MENU                ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ║       1️⃣  List Employees           📋       ║");
                Console.WriteLine("  ║       2️⃣  Create Employee          ➕       ║");
                Console.WriteLine("  ║       3️⃣  Update Employee          ✏️        ║");
                Console.WriteLine("  ║       4️⃣  Delete Employee          ✖️        ║");
                Console.WriteLine("  ║       0️⃣  Return to Person Menu     ↩️       ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                switch (option)
                {
                    case "1":
                        ListEmployees().Wait();
                        break;
                    case "2":
                        CreateEmployee().Wait();
                        break;
                    case "3":
                        UpdateEmployee().Wait();
                        break;
                    case "4":
                        DeleteEmployee().Wait();
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

        private async Task ListEmployees()
        {
            Console.Clear();
            MainMenu.ShowHeader("EMPLOYEES LIST");

            try
            {
                var employees = await _employeeRepository.GetAllAsync();

                if (!employees.Any())
                {
                    MainMenu.ShowMessage("\nNo employees registered.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\n{0,-5} {1,-12} {2,-30} {3,-15} {4,-10} {5,-10}",
                        "ID", "Join Date", "Employee Name", "Salary", "EPS ID", "ARL ID");

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 95));
                    Console.ResetColor();

                    foreach (var employee in employees)
                    {
                        string employeeName = employee.Person?.FullName ?? "N/A";
                        if (employeeName.Length > 27)
                        {
                            employeeName = employeeName.Substring(0, 24) + "...";
                        }

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("{0,-5} {1,-12} {2,-30} {3,-15} {4,-10} {5,-10}",
                            employee.Id,
                            employee.JoinDate.ToString("dd/MM/yyyy"),
                            employeeName,
                            employee.BaseSalary.ToString("C"),
                            employee.EpsId.ToString() ?? "N/A",
                            employee.ArlId.ToString() ?? "N/A");
                    }

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(new string('-', 95));
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError listing employees: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task CreateEmployee()
        {
            Console.Clear();
            MainMenu.ShowHeader("REGISTER NEW EMPLOYEE");

            try
            {
                string personId = MainMenu.ReadText("\nPerson ID: ").Trim();
                if (string.IsNullOrEmpty(personId))
                {
                    MainMenu.ShowMessage("Person ID cannot be empty.", ConsoleColor.Red);
                    return;
                }

                DateTime joinDate = MainMenu.ReadDate("Join Date (DD/MM/YYYY): ");
                if (joinDate > DateTime.Now)
                {
                    MainMenu.ShowMessage("Join date cannot be in the future.", ConsoleColor.Red);
                    return;
                }

                decimal baseSalary = MainMenu.ReadPositiveDecimal("Base Salary: ");
                if (baseSalary <= 0)
                {
                    MainMenu.ShowMessage("Base salary must be greater than zero.", ConsoleColor.Red);
                    return;
                }

                int epsId = MainMenu.ReadInteger("EPS ID: ");
                if (epsId <= 0)
                {
                    MainMenu.ShowMessage("EPS ID must be greater than zero.", ConsoleColor.Red);
                    return;
                }

                int arlId = MainMenu.ReadInteger("ARL ID: ");
                if (arlId <= 0)
                {
                    MainMenu.ShowMessage("ARL ID must be greater than zero.", ConsoleColor.Red);
                    return;
                }

                var employee = new Employee
                {
                    PersonId = personId,
                    JoinDate = joinDate,
                    BaseSalary = baseSalary,
                    EpsId = epsId,
                    ArlId = arlId
                };

                Console.Clear();
                MainMenu.ShowHeader("EMPLOYEE INFORMATION");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\nPerson ID: {personId}");
                Console.WriteLine($"Join Date: {joinDate:dd/MM/yyyy}");
                Console.WriteLine($"Base Salary: {baseSalary:C}");
                Console.WriteLine($"EPS ID: {epsId}");
                Console.WriteLine($"ARL ID: {arlId}");

                string confirm = MainMenu.ReadText("\nDo you want to register this employee? (Y/N): ");
                if (confirm.ToUpper() == "Y")
                {
                    bool result = await _employeeRepository.InsertAsync(employee);

                    if (result)
                    {
                        MainMenu.ShowMessage("\n✅ Employee registered successfully.", ConsoleColor.Green);
                    }
                    else
                    {
                        MainMenu.ShowMessage("\n❌ Employee registration failed.", ConsoleColor.Red);
                    }
                }
                else
                {
                    MainMenu.ShowMessage("\n⚠️ Operation cancelled.", ConsoleColor.Yellow);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\n❌ Error registering employee: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task UpdateEmployee()
        {
            Console.Clear();
            MainMenu.ShowHeader("UPDATE EMPLOYEE");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter employee ID to update: ");
                var employee = await _employeeRepository.GetByIdAsync(id);

                if (employee == null)
                {
                    MainMenu.ShowMessage("\n❌ The employee doesn't exist", ConsoleColor.Red);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nCurrent Information:");
                    Console.WriteLine($"Join Date: {employee.JoinDate:dd/MM/yyyy}");
                    Console.WriteLine($"Base Salary: {employee.BaseSalary:C}");
                    Console.WriteLine($"EPS ID: {employee.EpsId}");
                    Console.WriteLine($"ARL ID: {employee.ArlId}");
                    Console.ResetColor();

                    // Update Join Date
                    DateTime joinDate = MainMenu.ReadDate($"\nNew Join Date (DD/MM/YYYY) [{employee.JoinDate:dd/MM/yyyy}]: ");
                    if (joinDate != employee.JoinDate)
                    {
                        if (joinDate > DateTime.Now)
                        {
                            MainMenu.ShowMessage("Join date cannot be in the future.", ConsoleColor.Red);
                            return;
                        }
                        employee.JoinDate = joinDate;
                    }

                    // Update Base Salary
                    decimal newSalary = MainMenu.ReadPositiveDecimal($"\nNew Base Salary [{employee.BaseSalary:C}]: ");
                    if (newSalary != employee.BaseSalary)
                    {
                        if (newSalary <= 0)
                        {
                            MainMenu.ShowMessage("Base salary must be greater than zero.", ConsoleColor.Red);
                            return;
                        }
                        employee.BaseSalary = newSalary;
                    }

                    // Update EPS ID
                    int newEpsId = MainMenu.ReadInteger($"\nNew EPS ID [{employee.EpsId}]: ");
                    if (newEpsId != employee.EpsId)
                    {
                        if (newEpsId <= 0)
                        {
                            MainMenu.ShowMessage("EPS ID must be greater than zero.", ConsoleColor.Red);
                            return;
                        }
                        employee.EpsId = newEpsId;
                    }

                    // Update ARL ID
                    int newArlId = MainMenu.ReadInteger($"\nNew ARL ID [{employee.ArlId}]: ");
                    if (newArlId != employee.ArlId)
                    {
                        if (newArlId <= 0)
                        {
                            MainMenu.ShowMessage("ARL ID must be greater than zero.", ConsoleColor.Red);
                            return;
                        }
                        employee.ArlId = newArlId;
                    }

                    Console.Clear();
                    MainMenu.ShowHeader("UPDATED EMPLOYEE INFORMATION");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nJoin Date: {employee.JoinDate:dd/MM/yyyy}");
                    Console.WriteLine($"Base Salary: {employee.BaseSalary:C}");
                    Console.WriteLine($"EPS ID: {employee.EpsId}");
                    Console.WriteLine($"ARL ID: {employee.ArlId}");
                    Console.ResetColor();

                    string confirm = MainMenu.ReadText("\nDo you want to save these changes? (Y/N): ");
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _employeeRepository.UpdateAsync(employee);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Employee updated successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to update the employee.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error updating the employee: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }

        private async Task DeleteEmployee()
        {
            Console.Clear();
            MainMenu.ShowHeader("DELETE EMPLOYEE");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter the employee ID to delete: ");
                var employee = await _employeeRepository.GetByIdAsync(id);

                if (employee == null)
                {
                    MainMenu.ShowMessage("\n❌ The employee does not exist.", ConsoleColor.Red);
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"\nEmployee Information:");
                    Console.WriteLine($"ID: {employee.Id}");
                    Console.WriteLine($"Join Date: {employee.JoinDate:dd/MM/yyyy}");
                    Console.WriteLine($"Base Salary: {employee.BaseSalary:C}");
                    Console.WriteLine($"EPS ID: {employee.EpsId}");
                    Console.WriteLine($"ARL ID: {employee.ArlId}");
                    Console.ResetColor();
                    
                    string confirm = MainMenu.ReadText("\n⚠️ Are you sure you want to delete this employee? (Y/N): ");
                    
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _employeeRepository.DeleteAsync(id);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\n✅ Employee deleted successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\n❌ Failed to delete the employee.", ConsoleColor.Red);
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
                MainMenu.ShowMessage($"\n❌ Error deleting the employee: {ex.Message}", ConsoleColor.Red);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nPress any key to continue...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}