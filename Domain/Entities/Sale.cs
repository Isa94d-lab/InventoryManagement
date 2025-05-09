namespace InventoryManagement.Domain.Entities
{
    public class Sale
    {
        public int InvoiceId { get; set; }
        public DateTime Date { get; set; }
        public string EmployeePersonId { get; set; } = string.Empty;
        public string CustomerPersonId { get; set; } = string.Empty;
        
        // Propiedades de navegación
        public Person? Employee { get; set; }
        public Person? Customer { get; set; }
        public List<SaleDetail> Details { get; set; } = new List<SaleDetail>();
        
        public decimal Total => Details.Sum(d => d.Cost * d.Quantity);
        
        public override string ToString()
        {
            return $"Invoice: {InvoiceId}, Date: {Date:d}, Total: {Total:C}";
        }
    }
}