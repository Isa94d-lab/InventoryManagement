namespace InventoryManagement.Domain.Entities
{
    public class Product
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Stock { get; set; }
        public int StockMin { get; set; }
        public int StockMax { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string BarCode { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Stock: {Stock}, Code: {BarCode}";
        }
    }
}