namespace InventoryManagement.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string PersonId { get; set; } = string.Empty;
        public DateTime Birthdate { get; set; }
        public DateTime? PurchaseDate { get; set; }
        
        // Propiedades de navegación
        public Person? Person { get; set; }
        
        public override string ToString()
        {
            return $"ID: {Id}, PersonId: {PersonId}, Last Purchase: {PurchaseDate?.ToString("d") ?? "Without Purchase"}";
        }
    }
}