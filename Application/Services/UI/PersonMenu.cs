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
    public class PersonMenu
    {
        private readonly PersonRepository _personRepository;
        private readonly PersonTelephoneRepository _phoneRepository;
        
        public PersonMenu(MySqlConnection connection)
        {
            _personRepository = new PersonRepository(connection);
            _phoneRepository = new PersonTelephoneRepository(connection);
        }
        
        public void ShowMenu()
        {
            bool returnTo = false;
            
            while (!returnTo)
            {
                Console.Clear();
                MainMenu.ShowHeader("👥 PERSON MANAGEMENT");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ╔════════════════════════════════════════════╗");
                Console.WriteLine("  ║               📋 PERSON MENU               ║");
                Console.WriteLine("  ╠════════════════════════════════════════════╣");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ║     1️⃣  List Persons            📋          ║");
                Console.WriteLine("  ║     2️⃣  Show Person Details     🔍          ║");
                Console.WriteLine("  ║     3️⃣  Register New Person     ➕          ║");
                Console.WriteLine("  ║     4️⃣  Update Person           ✏️           ║");
                Console.WriteLine("  ║     5️⃣  Delete Person           🗑️           ║");
                Console.WriteLine("  ║     0️⃣  Return to Main Menu     ↩️           ║");
                Console.WriteLine("  ╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string option = MainMenu.ReadText("\n✨ Select an option: ");

                switch (option)
                {
                    case "1":
                        ListPersons().Wait();
                        break;
                    case "2":
                        ShowPersonDetail().Wait();
                        break;
                    case "3":
                        RegisterPerson().Wait();
                        break;
                    case "4":
                        UpdatePerson().Wait();
                        break;
                    case "5":
                        DeletePerson().Wait();
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

        private async Task ListPersons()
        {
            Console.Clear();
            MainMenu.ShowHeader("PERSONS LIST");
            
            try
            {
                var persons = await _personRepository.GetAllAsync();
                
                if (!persons.Any())
                {
                    MainMenu.ShowMessage("\nNo persons found.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.WriteLine("\n{0,-15} {1,-30} {2,-30} {3,-20}", 
                        "ID", "Name", "Email", "Document Type");
                    Console.WriteLine(new string('-', 95));
                    
                    foreach (var person in persons)
                    {
                        Console.WriteLine("{0,-15} {1,-30} {2,-30} {3,-20}", 
                            person.Id,
                            person.FullName,
                            person.Email,
                            person.DocumentTypeId);
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError listing persons: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }

        private async Task ShowPersonDetail()
        {
            Console.Clear();
            MainMenu.ShowHeader("PERSON DETAIL");
            
            try
            {
                string id = MainMenu.ReadText("\nEnter person ID: ");
                var person = await _personRepository.GetByIdAsync(id);
                
                if (person == null)
                {
                    MainMenu.ShowMessage("\nPerson not found.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.WriteLine("\nPERSON INFORMATION:");
                    Console.WriteLine($"ID: {person.Id}");
                    Console.WriteLine($"Name: {person.Name}");
                    Console.WriteLine($"Last Name: {person.LastName}");
                    Console.WriteLine($"Email: {person.Email}");
                    Console.WriteLine($"Document Type ID: {person.DocumentTypeId}");
                    Console.WriteLine($"Person Type ID: {person.PersonTypeId}");
                    Console.WriteLine($"City ID: {person.CityId}");

                    // Mostrar teléfonos asociados
                    var phones = await _phoneRepository.GetAllAsync();
                    var personPhones = phones.Where(p => p.PersonId == person.Id);
                    
                    if (personPhones.Any())
                    {
                        Console.WriteLine("\nPHONE NUMBERS:");
                        foreach (var phone in personPhones)
                        {
                            Console.WriteLine($"- {phone.Number} ({phone.PhoneType})");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError getting person details: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }

        private async Task RegisterPerson()
        {
            Console.Clear();
            MainMenu.ShowHeader("REGISTER NEW PERSON");
            
            try
            {
                string id = MainMenu.ReadText("\nID: ");
                string name = MainMenu.ReadText("Name: ");
                string lastName = MainMenu.ReadText("Last Name: ");
                string email = MainMenu.ReadText("Email: ");
                int documentTypeId = MainMenu.ReadInteger("Document Type ID: ");
                int personTypeId = MainMenu.ReadInteger("Person Type ID: ");
                int cityId = MainMenu.ReadInteger("City ID: ");
                
                var person = new Person
                {
                    Id = id,
                    Name = name,
                    LastName = lastName,
                    Email = email,
                    DocumentTypeId = documentTypeId,
                    PersonTypeId = personTypeId,
                    CityId = cityId
                };
                
                bool result = await _personRepository.InsertAsync(person);
                if (result)
                {
                    MainMenu.ShowMessage("\nPerson registered successfully.", ConsoleColor.Green);
                    
                    // Preguntar si desea agregar teléfonos
                    string addPhones = MainMenu.ReadText("\nDo you want to add phone numbers? (Y/N): ");
                    if (addPhones.ToUpper() == "Y")
                    {
                        await AddPhoneNumbers(person.Id);
                    }
                }
                else
                {
                    MainMenu.ShowMessage("\nCould not register person.", ConsoleColor.Red);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError registering person: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }

        private async Task UpdatePerson()
        {
            Console.Clear();
            MainMenu.ShowHeader("UPDATE PERSON");
            
            try
            {
                string id = MainMenu.ReadText("\nEnter person ID to update: ");
                var person = await _personRepository.GetByIdAsync(id);
                
                if (person == null)
                {
                    MainMenu.ShowMessage("\nPerson not found.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.WriteLine("\nCurrent information:");
                    Console.WriteLine($"Name: {person.Name}");
                    Console.WriteLine($"Last Name: {person.LastName}");
                    Console.WriteLine($"Email: {person.Email}");
                    Console.WriteLine($"Document Type ID: {person.DocumentTypeId}");
                    Console.WriteLine($"Person Type ID: {person.PersonTypeId}");
                    Console.WriteLine($"City ID: {person.CityId}");
                    
                    Console.WriteLine("\nEnter new information (press Enter to keep current value):");
                    
                    string name = MainMenu.ReadText($"Name [{person.Name}]: ");
                    string lastName = MainMenu.ReadText($"Last Name [{person.LastName}]: ");
                    string email = MainMenu.ReadText($"Email [{person.Email}]: ");
                    string docTypeId = MainMenu.ReadText($"Document Type ID [{person.DocumentTypeId}]: ");
                    string personTypeId = MainMenu.ReadText($"Person Type ID [{person.PersonTypeId}]: ");
                    string cityId = MainMenu.ReadText($"City ID [{person.CityId}]: ");
                    
                    person.Name = string.IsNullOrEmpty(name) ? person.Name : name;
                    person.LastName = string.IsNullOrEmpty(lastName) ? person.LastName : lastName;
                    person.Email = string.IsNullOrEmpty(email) ? person.Email : email;
                    person.DocumentTypeId = string.IsNullOrEmpty(docTypeId) ? person.DocumentTypeId : int.Parse(docTypeId);
                    person.PersonTypeId = string.IsNullOrEmpty(personTypeId) ? person.PersonTypeId : int.Parse(personTypeId);
                    person.CityId = string.IsNullOrEmpty(cityId) ? person.CityId : int.Parse(cityId);
                    
                    bool result = await _personRepository.UpdateAsync(person);
                    if (result)
                    {
                        MainMenu.ShowMessage("\nPerson updated successfully.", ConsoleColor.Green);
                    }
                    else
                    {
                        MainMenu.ShowMessage("\nCould not update person.", ConsoleColor.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError updating person: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }

        private async Task DeletePerson()
        {
            Console.Clear();
            MainMenu.ShowHeader("DELETE PERSON");
            
            try
            {
                string id = MainMenu.ReadText("\nEnter person ID to delete: ");
                var person = await _personRepository.GetByIdAsync(id);
                
                if (person == null)
                {
                    MainMenu.ShowMessage("\nPerson not found.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.WriteLine($"\nAre you sure you want to delete {person.FullName}?");
                    string confirm = MainMenu.ReadText("This action cannot be undone (Y/N): ");
                    
                    if (confirm.ToUpper() == "Y")
                    {
                        bool result = await _personRepository.DeleteAsync(id);
                        if (result)
                        {
                            MainMenu.ShowMessage("\nPerson deleted successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\nCould not delete person.", ConsoleColor.Red);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError deleting person: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }

        private async Task AddPhoneNumbers(string personId)
        {
            bool addMore = true;
            while (addMore)
            {
                string number = MainMenu.ReadText("\nPhone number: ");
                string type = MainMenu.ReadText("Phone type (Mobile(1)/Fixed(2)): ");
                
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
                
                string response = MainMenu.ReadText("\nAdd another phone number? (Y/N): ");
                addMore = response.ToUpper() == "Y";
            }
        }

        private async Task UpdatePhoneNumber(string personId)
        {
            await ListPhoneNumbers(personId);
            
            int phoneId = MainMenu.ReadInteger("\nEnter phone ID to update: ");
            var phone = await _phoneRepository.GetByIdAsync(phoneId);
            
            if (phone == null || phone.PersonId != personId)
            {
                MainMenu.ShowMessage("\nPhone number not found.", ConsoleColor.Yellow);
                return;
            }
            
            string number = MainMenu.ReadText($"New number [{phone.Number}]: ");
            string type = MainMenu.ReadText($"New type [{phone.PhoneType}]: ");
            
            phone.Number = string.IsNullOrEmpty(number) ? phone.Number : number;
            phone.PhoneType = string.IsNullOrEmpty(type) ? phone.PhoneType : type;
            
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

        private async Task DeletePhoneNumber(string personId)
        {
            await ListPhoneNumbers(personId);
            
            int phoneId = MainMenu.ReadInteger("\nEnter phone ID to delete: ");
            var phone = await _phoneRepository.GetByIdAsync(phoneId);
            
            if (phone == null || phone.PersonId != personId)
            {
                MainMenu.ShowMessage("\nPhone number not found.", ConsoleColor.Yellow);
                return;
            }
            
            string confirm = MainMenu.ReadText($"\nAre you sure you want to delete {phone.Number}? (Y/N): ");
            if (confirm.ToUpper() == "Y")
            {
                bool result = await _phoneRepository.DeleteAsync(phoneId);
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

        private async Task ListPhoneNumbers(string personId)
        {
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
    }
}