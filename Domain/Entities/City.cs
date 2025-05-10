namespace InventoryManagement.Domain.Entities
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int RegionId { get; set; }
        public override string ToString()
        {
            return $"City ID: {Id}, Name: {Name}, Region ID: {RegionId}";
        }
    }
}
