namespace InventoryManagement.Domain.Entities
{
    public class PromotionalPlan
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Discount { get; set; }
        
        // Lista de productos en el plan promocional
        public List<Product> Products { get; set; } = new List<Product>();

        public bool CurrentPromotionalPlan()
        {
            DateTime today = DateTime.Today;
            return today >= StartDate && today <= EndDate;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Discount: {Discount:P}, Start: {StartDate:d} - {EndDate:d}";
        }
    }
}