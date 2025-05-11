namespace InventoryManagement.Domain.Entities
{
    public class PersonTelephone
    {
        public int Id { get; set; } 
        public string Number { get; set; } = string.Empty; 
        public string PersonId { get; set; } = string.Empty; 
        public string PhoneType { get; set; } = string.Empty; // Tipo de teléfono (Fijo, Móvil)

        // Propiedad de navegación
        public Person? Person { get; set; }

        public override string ToString()
        {
            return $"ID: {Id}, Number: {Number}, Type: {PhoneType}, Person ID: {PersonId}";
        }
    }
}
