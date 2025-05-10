using System;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Application.UI
{
    public class ProductsMenu
    {
        private readonly ProductRepository _productRepository;
        
        public ProductsMenu(MySqlConnection connection)
        {
            _productRepository = new ProductRepository(connection);
        }
        
        public void ShowMenu()
        {
            bool regresar = false;
            
            while (!regresar)
            {
                Console.Clear();
                MainMenu.ShowHeader("🛒 PRODUCTS MANAGEMENT");

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("  ╔═══════════════════════════════════════════╗");
                Console.WriteLine("  ║               📋 PRODUCTS MENU            ║");
                Console.WriteLine("  ╠═══════════════════════════════════════════╣");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("  ║      1️⃣  List Products          📋         ║");
                Console.WriteLine("  ║      2️⃣  Search Product        🔍          ║");
                Console.WriteLine("  ║      3️⃣  New Product           ➕          ║");
                Console.WriteLine("  ║      4️⃣  Update Product         ✏️          ║");
                Console.WriteLine("  ║      5️⃣  Delete Product         🗑️          ║");
                Console.WriteLine("  ║      6️⃣  Low Stock Products     ⚠️          ║");
                Console.WriteLine("  ║      0️⃣  Return to Main Menu    ↩️          ║");
                Console.WriteLine("  ╚═══════════════════════════════════════════╝");

                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                string opcion = MainMenu.ReadText("\n✨ Select an option: ");

                switch (opcion)
                {
                    case "1":
                        ListProducts().Wait();
                        break;
                    case "2":
                        BuscarProducto().Wait();
                        break;
                    case "3":
                        NuevoProducto().Wait();
                        break;
                    case "4":
                        ActualizarProducto().Wait();
                        break;
                    case "5":
                        EliminarProducto().Wait();
                        break;
                    case "6":
                        ProductosStockBajo().Wait();
                        break;
                    case "0":
                        regresar = true;
                        break;
                    default:
                        MainMenu.ShowMessage("⚠️ Invalid option. Please try again.", ConsoleColor.Red);
                        Console.ReadKey();
                        break;
                }
            }
        }
        
        private async Task ListProducts()
        {
            Console.Clear();
            MainMenu.ShowHeader("LIST PRODUCTS");
            
            try
            {
                var products = await _productRepository.GetAllAsync();
                
                if (!products.Any())
                {
                    MainMenu.ShowMessage("\nThere aren't products.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.WriteLine("\n{0,-10} {1,-30} {2,-8} {3,-15}", "ID", "Name", "Stock", "Barcode");
                    Console.WriteLine(new string('-', 70));
                    
                    foreach (var product in products)
                    {
                        Console.WriteLine("{0,-10} {1,-30} {2,-8} {3,-15}", 
                            product.Id, 
                            product.Name.Length > 27 ? product.Name.Substring(0, 27) + "..." : product.Name, 
                            product.Stock, 
                            product.BarCode);
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError listing products: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
        
        private async Task BuscarProducto()
        {
            Console.Clear();
            MainMenu.ShowHeader("SEARCH PRODUCT");
            
            string id = MainMenu.ReadText("\nEnter product ID: ");
            
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                
                if (product == null)
                {
                    MainMenu.ShowMessage("\nThe product doesn't exist.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.WriteLine("\nPRODUCT INFORMATION:");
                    Console.WriteLine($"ID: {product.Id}");
                    Console.WriteLine($"Name: {product.Name}");
                    Console.WriteLine($"Stock: {product.Stock}");
                    Console.WriteLine($"Minimum tock: {product.StockMin}");
                    Console.WriteLine($"Maximum Stock: {product.StockMax}");
                    Console.WriteLine($"Barcode: {product.BarCode}");
                    Console.WriteLine($"Create Date: {product.CreateDate:d}");
                    Console.WriteLine($"Update Date: {product.UpdateDate:d}");
                    
                    // Alerta de stock
                    if (product.Stock <= product.StockMin)
                    {
                        MainMenu.ShowMessage("\nALERT! Stock below the minimum allowed.", ConsoleColor.Red);
                    }
                    else if (product.Stock >= product.StockMax)
                    {
                        MainMenu.ShowMessage("\nALERT! Stock above the recommended maximum.", ConsoleColor.Yellow);
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError searching the product: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
        
        private async Task NuevoProducto()
        {
            Console.Clear();
            MainMenu.ShowHeader("NEW PRODUCT");
            
            try
            {
                string id = MainMenu.ReadText("\nEnter Product ID: ");
                
                // Verificar si ya existe
                var existe = await _productRepository.GetByIdAsync(id);
                if (existe != null)
                {
                    MainMenu.ShowMessage("\nError: A product with that ID already exists.", ConsoleColor.Red);
                    Console.ReadKey();
                    return;
                }
                
                string nombre = MainMenu.ReadText("Enter the product name: ");
                string codigoBarra = MainMenu.ReadText("Enter the barcode: ");
                int stock = MainMenu.ReadInteger("Enter the initial stock: ");
                int stockMin = MainMenu.ReadInteger("Enter the minimum stock: ");
                int stockMax = MainMenu.ReadInteger("Enter the maximum stock: ");
                
                var producto = new Product
                {
                    Id = id,
                    Name = nombre,
                    Stock = stock,
                    StockMin = stockMin,
                    StockMax = stockMax,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    BarCode = codigoBarra
                };
                
                bool resultado = await _productRepository.InsertAsync(producto);
                
                if (resultado)
                {
                    MainMenu.ShowMessage("\nProduct registered successfully.", ConsoleColor.Green);
                }
                else
                {
                    MainMenu.ShowMessage("\nFailed to register the product.", ConsoleColor.Red);
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError registering the product: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
        
        private async Task ActualizarProducto()
        {
            Console.Clear();
            MainMenu.ShowHeader("UPDATE PRODUCT");
            
            string id = MainMenu.ReadText("\nEnter product ID to update: ");
            
            try
            {
                var producto = await _productRepository.GetByIdAsync(id);
                
                if (producto == null)
                {
                    MainMenu.ShowMessage("\nThe product doesn't exist", ConsoleColor.Yellow);
                }
                else
                {
                    Console.WriteLine($"\nCurrent product: {producto.Name}");
                    
                    string nombre = MainMenu.ReadText($"Enter new name product ({producto.Name}): ");
                    if (!string.IsNullOrWhiteSpace(nombre))
                    {
                        producto.Name = nombre;
                    }
                    
                    string codigoBarra = MainMenu.ReadText($"Enter new bar code ({producto.BarCode}): ");
                    if (!string.IsNullOrWhiteSpace(codigoBarra))
                    {
                        producto.BarCode = codigoBarra;
                    }
                    
                    Console.Write($"Enter new stock ({producto.Stock}): ");
                    string stockStr = Console.ReadLine() ?? "";
                    if (!string.IsNullOrWhiteSpace(stockStr) && int.TryParse(stockStr, out int stock) && stock >= 0)
                    {
                        producto.Stock = stock;
                    }
                    
                    Console.Write($"Enter new minimum stock ({producto.StockMin}): ");
                    string stockMinStr = Console.ReadLine() ?? "";
                    if (!string.IsNullOrWhiteSpace(stockMinStr) && int.TryParse(stockMinStr, out int stockMin) && stockMin >= 0)
                    {
                        producto.StockMin = stockMin;
                    }
                    
                    Console.Write($"Enter maximum stock ({producto.StockMax}): ");
                    string stockMaxStr = Console.ReadLine() ?? "";
                    if (!string.IsNullOrWhiteSpace(stockMaxStr) && int.TryParse(stockMaxStr, out int stockMax) && stockMax >= 0)
                    {
                        producto.StockMax = stockMax;
                    }
                    
                    producto.UpdateDate = DateTime.Now;
                    
                    bool resultado = await _productRepository.UpdateAsync(producto);
                    
                    if (resultado)
                    {
                        MainMenu.ShowMessage("\nProduct updated successfully.", ConsoleColor.Green);
                    }
                    else
                    {
                        MainMenu.ShowMessage("\nFailed to update the product.", ConsoleColor.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError updating the product: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
        
        private async Task EliminarProducto()
        {
            Console.Clear();
            MainMenu.ShowHeader("DELETE PRODUCT");
            
            string id = MainMenu.ReadText("\nEnter the product ID to delete: ");
            
            try
            {
                var producto = await _productRepository.GetByIdAsync(id);
                
                if (producto == null)
                {
                    MainMenu.ShowMessage("\nThe product does not exist.", ConsoleColor.Yellow);
                }
                else
                {
                    Console.WriteLine($"\nProduct to delete: {producto.Name}");
                    
                    string confirmacion = MainMenu.ReadText("\nAre you sure you want to delete this product? (Y/N): ");
                    
                    if (confirmacion.ToUpper() == "Y")
                    {
                        bool resultado = await _productRepository.DeleteAsync(id);
                        
                        if (resultado)
                        {
                            MainMenu.ShowMessage("\nProduct deleted successfully.", ConsoleColor.Green);
                        }
                        else
                        {
                            MainMenu.ShowMessage("\nFailed to delete the product.", ConsoleColor.Red);
                        }
                    }
                    else
                    {
                        MainMenu.ShowMessage("\nOperation cancelled", ConsoleColor.Yellow);
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError deleting the product: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
        
        private async Task ProductosStockBajo()
        {
            Console.Clear();
            MainMenu.ShowHeader("PRODUCTS WITH LOW STOCK");
            
            try
            {
                var products = await _productRepository.GetProductosBajoStockAsync();
                
                if (!products.Any())
                {
                    MainMenu.ShowMessage("\nNo products with low stock", ConsoleColor.Green);
                }
                else
                {
                    MainMenu.ShowMessage($"\nThere are {products.Count()}  products with low or critical stock found.", ConsoleColor.Yellow);
                    
                    Console.WriteLine("\n{0,-10} {1,-30} {2,-8} {3,-8} {4,-8}", "ID", "Name", "Stock", "Minimum Stock", "Status");
                    Console.WriteLine(new string('-', 70));
                    
                    foreach (var product in products)
                    {
                        string estado = product.Stock == 0 ? "CRÍTICO" : "BAJO";
                        ConsoleColor color = product.Stock == 0 ? ConsoleColor.Red : ConsoleColor.Yellow;
                        
                        Console.ForegroundColor = color;
                        Console.WriteLine("{0,-10} {1,-30} {2,-8} {3,-8} {4,-8}", 
                            product.Id, 
                            product.Name.Length > 27 ? product.Name.Substring(0, 27) + "..." : product.Name, 
                            product.Stock, 
                            product.StockMin,
                            estado);
                        Console.ResetColor();
                    }
                }
            }
            catch (Exception ex)
            {
                MainMenu.ShowMessage($"\nError finding products with low stock: {ex.Message}", ConsoleColor.Red);
            }
            
            Console.Write("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}