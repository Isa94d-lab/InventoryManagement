namespace InventoryManagement.Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string PersonId { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
        public double BaseSalary { get; set; }
        public int EpsId { get; set; }
        public int ArlId { get; set; }
        
        // Propiedades de navegación
        public Person? Person { get; set; }
        
        public override string ToString()
        {
            return $"ID: {Id}, PersonID: {PersonId}, JoinDate: {JoinDate:d}, Salary: {BaseSalary:C}";
        }
    }
}