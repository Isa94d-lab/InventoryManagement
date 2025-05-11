namespace InventoryManagement.Domain.Entities
{
    public class Arl
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}";
        }
    }
}
