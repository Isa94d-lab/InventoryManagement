using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Application.UI
{
    public class PhoneNumberMenu
    {
        private readonly PersonTelephoneRepository _phoneRepository;
        private readonly PersonRepository _personRepository;
        
        public PhoneNumberMenu(MySqlConnection connection)
        {
            _phoneRepository = new PersonTelephoneRepository(connection);
            _personRepository = new PersonRepository(connection);
        }
        
        public void ShowMenu()
        {
            bool returnTo = false;
            
            while (!returnTo)
            {
                Console.Clear();
                MainMenu.ShowHeader("📱 PHONE NUMBER MANAGEMENT");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║           📋 PHONE NUMBER MENU             ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ║     1️⃣  List All Phone Numbers   📋         ║");
                Console.WriteLine("  ║     2️⃣  Show Phone Details      🔍          ║");
                Console.WriteLine("  ║     3️⃣  Add New Phone Number    ➕          ║");
                Console.WriteLine("  ║     4️⃣  Update Phone Number     ✏️           ║");
                Console.WriteLine("  ║     5️⃣  Delete Phone Number     🗑️           ║");
                Console.WriteLine("  ║     6️⃣  Search by Person        👤          ║");
                Console.WriteLine("  ║     0️⃣  Return to Main Menu     ↩️           ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                switch (option)
                {
                    case "1":
                        ListAllPhoneNumbers().Wait();
                        break;
                    case "2":
                        ShowPhoneDetails().Wait();
                        break;
                    case "3":
                        AddPhoneNumber().Wait();
                        break;
                    case "4":
                        UpdatePhoneNumber().Wait();
                        break;
                    case "5":
                        DeletePhoneNumber().Wait();
                        break;
                    case "6":
                        SearchByPerson().Wait();
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

        private async Task ListAllPhoneNumbers()
        {
            Console.Clear();
            MainMenu.ShowHeader("ALL PHONE NUMBERS");
            
            try
            {
                var phones = await _phoneRepository.GetAllAsync();
                
                if (!phones.Any())
                {
                    MainMenu.ShowMessage("\nNo phone numbers found.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.WriteLine("\n{0,-5} {1,-15} {2,-10} {3,-15}", 
                        "ID", "Number", "Type", "Person ID");
                    Console.WriteLine(new string('-', 50));
                    
                    foreach (var phone in phones)
                    {
                        var person = await _personRepository.GetByIdAsync(phone.PersonId);
                        string personName = person?.FullName ?? "Unknown";
                        
                        Console.WriteLine("{0,-5} {1,-15} {2,-10} {3,-15}", 
                            phone.Id,
                            phone.Number,
                            phone.PhoneType,
                            personName);
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError listing phone numbers: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }

        private async Task ShowPhoneDetails()
        {
            Console.Clear();
            MainMenu.ShowHeader("PHONE NUMBER DETAILS");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter phone ID: ");
                var phone = await _phoneRepository.GetByIdAsync(id);
                
                if (phone == null)
                {
                    MainMenu.ShowMessage("\nPhone number not found.", ConsoleColor.Yellow);
                }
                else
                {
                    var person = await _personRepository.GetByIdAsync(phone.PersonId);
                    
                    Console.WriteLine("\nPHONE NUMBER INFORMATION:");
                    Console.WriteLine($"ID: {phone.Id}");
                    Console.WriteLine($"Number: {phone.Number}");
                    Console.WriteLine($"Type: {phone.PhoneType}");
                    Console.WriteLine($"Person ID: {phone.PersonId}");
                    if (person != null)
                    {
                        Console.WriteLine($"Person Name: {person.FullName}");
                        Console.WriteLine($"Person Email: {person.Email}");
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError getting phone details: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }

        private async Task AddPhoneNumber()
        {
            Console.Clear();
            MainMenu.ShowHeader("ADD NEW PHONE NUMBER");
            
            try
            {
                string personId = MainMenu.ReadText("\nPerson ID: ");
                var person = await _personRepository.GetByIdAsync(personId);
                
                if (person == null)
                {
                    MainMenu.ShowMessage("\nPerson not found.", ConsoleColor.Yellow);
                    return;
                }
                
                Console.WriteLine($"\nAdding phone number for: {person.FullName}");
                
                string number = MainMenu.ReadText("Phone number: ");
                
                Console.WriteLine("\nSelect phone type:");
                Console.WriteLine("1. Fijo (Fixed)");
                Console.WriteLine("2. Movil (Mobile)");
                string typeOption = MainMenu.ReadText("\nEnter type option (1-2): ");
                string type = typeOption switch
                {
                    "1" => "Fijo",
                    "2" => "Movil",
                    _ => throw new ArgumentException("Invalid phone type option")
                };
                
                var phone = new PersonTelephone
                {
                    Number = number,
                    PersonId = personId,
                    PhoneType = type
                };
                
                bool result = await _phoneRepository.InsertAsync(phone);
                if (result)
                {
                    MainMenu.ShowMessage("\nPhone number added successfully.", ConsoleColor.Green);
                }
                else
                {
                    MainMenu.ShowMessage("\nCould not add phone number.", ConsoleColor.Red);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError adding phone number: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }

        private async Task UpdatePhoneNumber()
        {
            Console.Clear();
            MainMenu.ShowHeader("UPDATE PHONE NUMBER");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter phone ID to update: ");
                var phone = await _phoneRepository.GetByIdAsync(id);
                
                if (phone == null)
                {
                    MainMenu.ShowMessage("\nPhone number not found.", ConsoleColor.Yellow);
                    return;
                }
                
                var person = await _personRepository.GetByIdAsync(phone.PersonId);
                Console.WriteLine($"\nUpdating phone number for: {person?.FullName ?? "Unknown"}");
                Console.WriteLine($"Current number: {phone.Number}");
                Console.WriteLine($"Current type: {phone.PhoneType}");
                
                string number = MainMenu.ReadText("\nNew number (press Enter to keep current): ");
                
                Console.WriteLine("\nSelect new phone type (press Enter to keep current):");
                Console.WriteLine("1. Fijo (Fixed)");
                Console.WriteLine("2. Movil (Mobile)");
                string typeOption = MainMenu.ReadText("\nEnter type option (1-2): ");
                string type = string.IsNullOrEmpty(typeOption) ? phone.PhoneType : typeOption switch
                {
                    "1" => "Fijo",
                    "2" => "Movil",
                    _ => throw new ArgumentException("Invalid phone type option")
                };
                
                phone.Number = string.IsNullOrEmpty(number) ? phone.Number : number;
                phone.PhoneType = type;
                
                bool result = await _phoneRepository.UpdateAsync(phone);
                if (result)
                {
                    MainMenu.ShowMessage("\nPhone number updated successfully.", ConsoleColor.Green);
                }
                else
                {
                    MainMenu.ShowMessage("\nCould not update phone number.", ConsoleColor.Red);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError updating phone number: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }

        private async Task DeletePhoneNumber()
        {
            Console.Clear();
            MainMenu.ShowHeader("DELETE PHONE NUMBER");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter phone ID to delete: ");
                var phone = await _phoneRepository.GetByIdAsync(id);
                
                if (phone == null)
                {
                    MainMenu.ShowMessage("\nPhone number not found.", ConsoleColor.Yellow);
                    return;
                }
                
                var person = await _personRepository.GetByIdAsync(phone.PersonId);
                Console.WriteLine($"\nAre you sure you want to delete this phone number?");
                Console.WriteLine($"Number: {phone.Number}");
                Console.WriteLine($"Type: {phone.PhoneType}");
                Console.WriteLine($"Person: {person?.FullName ?? "Unknown"}");
                
                string confirm = MainMenu.ReadText("\nThis action cannot be undone (Y/N): ");
                if (confirm.ToUpper() == "Y")
                {
                    bool result = await _phoneRepository.DeleteAsync(id);
                    if (result)
                    {
                        MainMenu.ShowMessage("\nPhone number deleted successfully.", ConsoleColor.Green);
                    }
                    else
                    {
                        MainMenu.ShowMessage("\nCould not delete phone number.", ConsoleColor.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError deleting phone number: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }

        private async Task SearchByPerson()
        {
            Console.Clear();
            MainMenu.ShowHeader("SEARCH PHONE NUMBERS BY PERSON");
            
            try
            {
                string personId = MainMenu.ReadText("\nEnter person ID: ");
                var person = await _personRepository.GetByIdAsync(personId);
                
                if (person == null)
                {
                    MainMenu.ShowMessage("\nPerson not found.", ConsoleColor.Yellow);
                    return;
                }
                
                Console.WriteLine($"\nPhone numbers for: {person.FullName}");
                
                var phones = await _phoneRepository.GetAllAsync();
                var personPhones = phones.Where(p => p.PersonId == personId);
                
                if (!personPhones.Any())
                {
                    MainMenu.ShowMessage("\nNo phone numbers found for this person.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.WriteLine("\n{0,-5} {1,-15} {2,-10}", "ID", "Number", "Type");
                    Console.WriteLine(new string('-', 35));
                    
                    foreach (var phone in personPhones)
                    {
                        Console.WriteLine("{0,-5} {1,-15} {2,-10}", 
                            phone.Id,
                            phone.Number,
                            phone.PhoneType);
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError searching phone numbers: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}