using System;

namespace InventoryManagement.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string PersonId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public DateTime? LastAccess { get; set; }
        public bool IsActive { get; set; }
        
        // Propiedad de navegación
        public Person? Person { get; set; }
        
        public override string ToString()
        {
            return $"ID: {Id}, Username: {Username}, Role: {Role}, Active: {IsActive}";
        }
    }
} 