namespace InventoryManagement.Domain.Entities
{
    public class SaleDetail
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Cost { get; set; }
        
        // Propiedades de navegación
        public Product? Product { get; set; }
        
        public decimal Subtotal => Cost * Quantity;
        
        public override string ToString()
        {
            return $"ID: {Id}, Product: {ProductId}, Quantity: {Quantity}, Cost: {Cost:C}, Subtotal: {Subtotal:C}";
        }
    }
}