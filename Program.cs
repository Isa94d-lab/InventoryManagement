using InventoryManagement.Application.UI;
using InventoryManagement.Infrastructure.Configuration;
using InventoryManagement.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace InventoryManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Start the applicatioon
            try
            {
                // Configure database connection
                var connection = DatabaseConfig.GetConnection();
                
                // Create repositories
                var productRepository = new ProductRepository(connection);
                var purchaseRepository = new PurchaseRepository(connection);
                var saleRepository = new SaleRepository(connection);
                var cashFlowRepository = new CashFlowRepository(connection);
                var planRepository = new PlanRepository(connection);

                // Create and run main menu
                var mainMenu = new MainMenu();
                mainMenu.ShowMenu();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
            }
            finally
            {
                DatabaseConfig.CloseConnection();
            }
        }
    }
}
