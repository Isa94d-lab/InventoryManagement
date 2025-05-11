namespace InventoryManagement.Domain.Entities
{
    public class Eps
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}";
        }
    }
}
