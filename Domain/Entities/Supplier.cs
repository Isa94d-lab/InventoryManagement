namespace InventoryManagement.Domain.Entities
{
    public class Supplier
    {
        public int Id { get; set; }
        public string PersonId { get; set; } = string.Empty;
        public double Discount { get; set; }
        public int PayDay { get; set; }
        
        // Propiedades de navegación
        public Person? Person { get; set; }
        
        public override string ToString()
        {
            return $"ID: {Id}, PersonID: {PersonId}, Discount: {Discount:P}, PayDay: {PayDay}";
        }
    }
}