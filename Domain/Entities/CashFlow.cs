namespace InventoryManagement.Domain.Entities
{
    public class CashFlow
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int MovementTypeId { get; set; }
        public decimal Cost { get; set; }
        public string Concept { get; set; } = string.Empty;
        public string PersonId { get; set; } = string.Empty;
        
        // Propiedades de navegación
        public string? MovementNameType { get; set; }
        public string? MovementType { get; set; } // Entrada o Salida
        public Person? Person { get; set; }
        
        public override string ToString()
        {
            return $"ID: {Id}, Date: {Date:d}, Type: {MovementNameType} ({MovementType}), Cost: {Cost:C}";
        }
    }
}