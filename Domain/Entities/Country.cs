namespace InventoryManagement.Domain.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        
        public override string ToString()
        {
            return $"Country ID: {Id}, Name: {Name}";
        }
    }
}
