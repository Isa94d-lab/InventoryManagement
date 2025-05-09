using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Application.UI
{
    public class PlanMenu
    {
        private readonly PlanRepository _planRepository;
        private readonly ProductRepository _productRepository;
        
        public PlanMenu(MySqlConnection connection)
        {
            _planRepository = new PlanRepository(connection);
            _productRepository = new ProductRepository(connection);
        }
        
        public void ShowMenu()
        {
            bool returnTo = false;
            
            while (!returnTo)
            {
                Console.Clear();
                MainMenu.ShowHeader("🎁 PROMOTIONAL PLANS");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\n╔════════════════════════════════════════════╗");
                Console.WriteLine("║             📋 PLANS MENU                 ║");
                Console.WriteLine("╠════════════════════════════════════════════╣");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("║ 1️⃣  List Plans              📋            ║");
                Console.WriteLine("║ 2️⃣  Details Plan            🔍            ║");
                Console.WriteLine("║ 3️⃣  Create new plan         ➕            ║");
                Console.WriteLine("║ 4️⃣  Update Plan             ✏️            ║");
                Console.WriteLine("║ 5️⃣  Delete Plan             🗑️            ║");
                Console.WriteLine("║ 6️⃣  Show current plans      ⏳            ║");
                Console.WriteLine("║ 0️⃣  Return to Main Menu     ↩️            ║");
                Console.WriteLine("╚════════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string option = MainMenu.ReadText("\n✨ Select an option: ");
                
                switch (option)
                {
                    case "1":
                        ListPlans().Wait();
                        break;
                    case "2":
                        ShowDetailPlan().Wait();
                        break;
                    case "3":
                        CreatePlan().Wait();
                        break;
                    case "4":
                        UpdatePlan().Wait();
                        break;
                    case "5":
                        DeletePlan().Wait();
                        break;
                    case "6":
                        CurrentPromotionalPlan().Wait();
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
        
        private async Task ListPlans()
        {
            Console.Clear();
            MainMenu.ShowHeader("PROMOTIONAL PLANS");
            
            try
            {
                var plans = await _planRepository.GetAllAsync();
                
                if (!plans.Any())
                {
                    MainMenu.ShowMessage("\nThere are no promotional plans.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.WriteLine("\n{0,-5} {1,-20} {2,-12} {3,-12} {4,-15} {5,-10}", 
                        "ID", "Name", "Start", "End", "Discount", "Status");
                    Console.WriteLine(new string('-', 80));
                    
                    foreach (var plan in plans)
                    {
                        bool available = plan.CurrentPromotionalPlan();
                        ConsoleColor color = available ? ConsoleColor.Green : ConsoleColor.Gray;
                        
                        Console.ForegroundColor = color;
                        Console.WriteLine("{0,-5} {1,-20} {2,-12} {3,-12} {4,-15} {5,-10}", 
                            plan.Id, 
                            plan.Name.Length > 17 ? plan.Name.Substring(0, 17) + "..." : plan.Name,
                            plan.StartDate.ToString("dd/MM/yyyy"),
                            plan.EndDate.ToString("dd/MM/yyyy"),
                            plan.Discount.ToString("P"),
                            available ? "AVAILABLE" : "INACTIVE");
                        Console.ResetColor();
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError listing plans: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
        
        private async Task ShowDetailPlan()
        {
            Console.Clear();
            MainMenu.ShowHeader("DETAIL PROMOTIONAL PLAN");
            
            try
            {
                int id = MainMenu.ReadInteger("\nRegister ID plan: ");
                
                var plan = await _planRepository.GetByIdAsync(id);
                
                if (plan == null)
                {
                    MainMenu.ShowMessage("\nThe plan doesn't exist.", ConsoleColor.Yellow);
                }
                else
                {
                    bool available = plan.CurrentPromotionalPlan();
                    
                    Console.WriteLine("\nPLAN DETAIL:");
                    Console.WriteLine($"ID: {plan.Id}");
                    Console.WriteLine($"Name: {plan.Name}");
                    Console.WriteLine($"Start Date: {plan.StartDate:dd/MM/yyyy}");
                    Console.WriteLine($"End Date: {plan.EndDate:dd/MM/yyyy}");
                    Console.WriteLine($"Discount: {plan.Discount:P}");
                    Console.WriteLine($"Status: {(available ? "AVAILABLE" : "INACTIVE")}");
                    
                    Console.WriteLine("\nPRODUCTS ON PROMOTION:");
                    
                    if (!plan.Products.Any())
                    {
                        MainMenu.ShowMessage("There aren't products in this plan.", ConsoleColor.Yellow);
                    }
                    else
                    {
                        Console.WriteLine("{0,-10} {1,-30} {2,-10}", 
                            "ID", "Name", "Stock");
                        Console.WriteLine(new string('-', 60));
                        
                        foreach (var product in plan.Products)
                        {
                            Console.WriteLine("{0,-10} {1,-30} {2,-10}", 
                                product.Id, 
                                product.Name.Length > 27 ? product.Name.Substring(0, 27) + "..." : product.Name,
                                product.Stock);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError in obtaining promotional plan: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
        
        private async Task CreatePlan()
        {
            Console.Clear();
            MainMenu.ShowHeader("CREATE NEW PROMOTIONAL PLAN");
            
            try
            {
                string name = MainMenu.ReadText("\nPlan Name: ");
                DateTime startDate = MainMenu.ReadDate("Start Date (DD/MM/AAAA): ");
                DateTime endDate = MainMenu.ReadDate("End Date (DD/MM/AAAA): ");
                
                // Validar que la fecha de fin sea posterior a la de inicio
                if (endDate < startDate)
                {
                    MainMenu.ShowMessage("\nError: The end date must be later than the start date..", ConsoleColor.Red);
                    Console.ReadKey();
                    return;
                }
                
                decimal discount = 0;
                while (true)
                {
                    Console.Write("Discount Percentage (0-100): ");
                    if (decimal.TryParse(Console.ReadLine(), out discount) && discount >= 0 && discount <= 100)
                    {
                        // Convertir de porcentaje a decimal (ej: 10% -> 0.1)
                        discount = discount / 100;
                        break;
                    }
                    
                    MainMenu.ShowMessage("Error: You must enter a number between 0 and 100.", ConsoleColor.Red);
                }
                
                var plan = new PromotionalPlan
                {
                    Name = name,
                    StartDate = startDate,
                    EndDate = endDate,
                    Discount = discount,
                    Products = new List<Product>()
                };
                
                // Agregar productos al plan
                await AgregarProductosAlPlan(plan);
                
                // Si no se agregaron productos, confirmar si se desea guardar el plan
                if (!plan.Products.Any())
                {
                    string confirm = MainMenu.ReadText("\nNo products were added to the plan, do you want to save it anyway (Y/N): ");
                    
                    if (confirm.ToUpper() != "S")
                    {
                        MainMenu.ShowMessage("\nOperation canceled.", ConsoleColor.Yellow);
                        Console.ReadKey();
                        return;
                    }
                }
                
                bool result = await _planRepository.InsertAsync(plan);
                
                if (result)
                {
                    MainMenu.ShowMessage("\nThe promotional plan has been created.", ConsoleColor.Green);
                }
                else
                {
                    MainMenu.ShowMessage("\nError creating promotional plan.", ConsoleColor.Red);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError creating promotional plan: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
        
        private async Task UpdatePlan()
        {
            Console.Clear();
            MainMenu.ShowHeader("MODIFY PROMOTIONAL PLAN");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter the ID to modify: ");
                
                var plan = await _planRepository.GetByIdAsync(id);
                
                if (plan == null)
                {
                    MainMenu.ShowMessage("\nThe plan doesn't exist.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.WriteLine($"\nCurrent Plan: {plan.Name}");
                    
                    string name = MainMenu.ReadText($"Enter new name({plan.Name}): ");
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        plan.Name = name;
                    }
                    
                    Console.Write($"Enter new start date ({plan.StartDate:dd/MM/yyyy}): ");
                    string startDateStr = Console.ReadLine() ?? "";
                    if (!string.IsNullOrWhiteSpace(startDateStr) && DateTime.TryParse(startDateStr, out DateTime startDate))
                    {
                        plan.StartDate = startDate;
                    }
                    
                    Console.Write($"Enter end date({plan.EndDate:dd/MM/yyyy}): ");
                    string endDateStr = Console.ReadLine() ?? "";
                    if (!string.IsNullOrWhiteSpace(endDateStr) && DateTime.TryParse(endDateStr, out DateTime endDate))
                    {
                        plan.EndDate = endDate;
                    }
                    
                    // Validar que la fecha de fin sea posterior a la de inicio
                    if (plan.EndDate < plan.StartDate)
                    {
                        MainMenu.ShowMessage("\nError: The end date must be later than the start date.", ConsoleColor.Red);
                        Console.ReadKey();
                        return;
                    }
                    
                    Console.Write($"Enter the new percentage discount ({plan.Discount:P0}): ");
                    string discountStr = Console.ReadLine() ?? "";
                    if (!string.IsNullOrWhiteSpace(discountStr) && decimal.TryParse(discountStr, out decimal discount) && discount >= 0 && discount <= 100)
                    {
                        plan.Discount = discount / 100; // Convertir de porcentaje a decimal
                    }
                    
                    // Preguntar si desea modificar los productos
                    string modifyProducts = MainMenu.ReadText("\n¿Do you want modify the plan products? (Y/N): ");
                    
                    if (modifyProducts.ToUpper() == "Y")
                    {
                        // Limpiar la lista de productos y agregar los nuevos
                        plan.Products.Clear();
                        await AgregarProductosAlPlan(plan);
                    }
                    
                    bool result = await _planRepository.UpdateAsync(plan);
                    
                    if (result)
                    {
                        MainMenu.ShowMessage("\nPromotional Plan has been updated.", ConsoleColor.Green);
                    }
                    else
                    {
                        MainMenu.ShowMessage("\nError updating promotional plan.", ConsoleColor.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError updating promotional plan: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
        
        private async Task DeletePlan()
        {
            Console.Clear();
            MainMenu.ShowHeader("DELETE PROMOTIONAL PLAN");
            
            try
            {
                int id = MainMenu.ReadInteger("\nEnter the ID to delete: ");
                
                var plan = await _planRepository.GetByIdAsync(id);
                
                if (plan == null)
                {
                    MainMenu.ShowMessage("\nThe doesn't exist.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.WriteLine($"\nPlan to delete: {plan.Name}");
                    Console.WriteLine($"Available: {plan.StartDate:dd/MM/yyyy} - {plan.EndDate:dd/MM/yyyy}");
                    Console.WriteLine($"Discount: {plan.Discount:P}");
                    Console.WriteLine($"Promotional products: {plan.Products.Count}");
                    
                    string confirmation = MainMenu.ReadText("\nAre you sure to delete the plan? (Y/N): ");
                    
                    if (confirmation.ToUpper() == "Y")
                    {
                        bool result = await _planRepository.DeleteAsync(id);
                        
                        if (result)
                        {
                            MainMenu.ShowMessage("\nPromotional plan has been deleted.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\nError deleting promotional plan.", ConsoleColor.Red);
                        }
                    }
                    else
                    {
                        MainMenu.ShowMessage("\nOperation Canceled.", ConsoleColor.Yellow);
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError deleting promotional plan: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
        
        private async Task CurrentPromotionalPlan()
        {
            Console.Clear();
            MainMenu.ShowHeader("AVAILABLE PROMOTIONAL PLANS");
            
            try
            {
                var plans = await _planRepository.GetPlanesVigentesAsync();
                
                if (!plans.Any())
                {
                    MainMenu.ShowMessage("\nThere aren't promotional plans.", ConsoleColor.Yellow);
                }
                else
                {
                    MainMenu.ShowMessage($"\nThere are {plans.Count()} promotional plans available.", ConsoleColor.Green);
                    
                    Console.WriteLine("\n{0,-5} {1,-20} {2,-12} {3,-12} {4,-15}", 
                        "ID", "Name", "Start", "End", "Discount");
                    Console.WriteLine(new string('-', 70));
                    
                    foreach (var plan in plans)
                    {
                        Console.WriteLine("{0,-5} {1,-20} {2,-12} {3,-12} {4,-15}", 
                            plan.Id, 
                            plan.Name.Length > 17 ? plan.Name.Substring(0, 17) + "..." : plan.Name,
                            plan.StartDate.ToString("dd/MM/yyyy"),
                            plan.EndDate.ToString("dd/MM/yyyy"),
                            plan.Discount.ToString("P"));
                        
                        // Mostrar productos del plan
                        Console.WriteLine("  Promotional Products:");
                        if (!plan.Products.Any())
                        {
                            Console.WriteLine("    There aren't products in this plan.");
                        }
                        else
                        {
                            foreach (var product in plan.Products)
                            {
                                Console.WriteLine($"    - {product.Id}: {product.Name}");
                            }
                        }
                        
                        Console.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError obtaining plans: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
        
        private async Task AgregarProductosAlPlan(PromotionalPlan plan)
        {
            bool addProducts = true;
            
            // Primero obtener todos los productos para mostrar una lista
            var allProducts = await _productRepository.GetAllAsync();
            
            if (!allProducts.Any())
            {
                MainMenu.ShowMessage("\nThere aren't available products in the plan.", ConsoleColor.Yellow);
                return;
            }
            
            while (addProducts)
            {
                Console.Clear();
                MainMenu.ShowHeader("ADD PRODUCTS");
                
                Console.WriteLine("\nAVAILABLE PRODUCTS:");
                Console.WriteLine("{0,-10} {1,-30} {2,-8}", "ID", "Name", "Stock");
                Console.WriteLine(new string('-', 50));
                
                foreach (var product in allProducts)
                {
                    // No mostrar productos que ya están en el plan
                    if (!plan.Products.Any(p => p.Id == product.Id))
                    {
                        Console.WriteLine("{0,-10} {1,-30} {2,-8}", 
                            product.Id, 
                            product.Name.Length > 27 ? product.Name.Substring(0, 27) + "..." : product.Name, 
                            product.Stock);
                    }
                }
                
                // Mostrar productos ya agregados
                if (plan.Products.Any())
                {
                    Console.WriteLine("\nProducts that have been added:");
                    foreach (var product in plan.Products)
                    {
                        Console.WriteLine($"- {product.Id}: {product.Name}");
                    }
                }
                
                string productId = MainMenu.ReadText("\nEnter the product ID to add (0 for finishing): ");
                
                if (productId == "0")
                {
                    addProducts = false;
                }
                else
                {
                    // Verificar si el producto ya está en el plan
                    if (plan.Products.Any(p => p.Id == productId))
                    {
                        MainMenu.ShowMessage("\nThe product has already added.", ConsoleColor.Yellow);
                        Console.ReadKey();
                        continue;
                    }
                    
                    // Verificar si el producto existe
                    var product = allProducts.FirstOrDefault(p => p.Id == productId);
                    if (product == null)
                    {
                        MainMenu.ShowMessage("\nEl producto no existe.", ConsoleColor.Yellow);
                        Console.ReadKey();
                        continue;
                    }
                    
                    // Agregar el producto al plan
                    plan.Products.Add(product);
                    MainMenu.ShowMessage($"\nProduct'{product.Name}' has been added.", ConsoleColor.Green);
                    
                    string continuar = MainMenu.ReadText("\nDo you want to add other product? (S/N): ");
                    addProducts = continuar.ToUpper() == "S";
                }
            }
        }
    }
}