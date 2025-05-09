namespace InventoryManagement.Domain.Entities;
    public class Purchase
    {
        public int Id { get; set; }
        public string SupplierPersonId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string EmployeePersonId { get; set; } = string.Empty;
        public string PurOrder { get; set; } = string.Empty;
        
        // Propiedades de navegación
        public Person? Supplier { get; set; }
        public Person? Employee { get; set; }
        public List<PurchaseDetail> Details { get; set; } = new List<PurchaseDetail>();
        
        public decimal Total => Details.Sum(d => d.Cost * d.Quantity);
        
        public override string ToString()
        {
            return $"ID: {Id}, Date: {Date:d}, Purchase Order: {PurOrder}, Total: {Total:C}";
        }
    }
