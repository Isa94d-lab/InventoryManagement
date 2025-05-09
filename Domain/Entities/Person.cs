namespace InventoryManagement.Domain.Entities
{
    public class Person
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int DocumentTypeId { get; set; }
        public int PersonTypeId { get; set; }
        public int CityId { get; set; }
        
        // Propiedades de navegación
        public string FullName => $"{Name} {LastName}";
        
        public override string ToString()
        {
            return $"ID: {Id}, Name: {FullName}, Email: {Email}";
        }
    }
}