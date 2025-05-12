using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Domain.Ports
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> UpdateLastAccessAsync(int userId);
        Task<bool> UpdatePasswordAsync(int userId, string newPassword);
        Task<bool> UpdateStatusAsync(int userId, bool isActive);
    }
} 