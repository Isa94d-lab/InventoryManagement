using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Ports;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Infrastructure.Repositories
{
    public class EpsRepository : IGenericRepository<Eps>, IEpsRepository
    {
        private readonly MySqlConnection _connection;

        public EpsRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Eps>> GetAllAsync()
        {
            var epsList = new List<Eps>();
            const string query = "SELECT id, nombre FROM eps";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                epsList.Add(new Eps
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["nombre"].ToString() ?? string.Empty
                });
            }

            return epsList;
        }

        public async Task<Eps?> GetByIdAsync(object id)
        {
            const string query = "SELECT id, nombre FROM eps WHERE id = @Id";
            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Eps
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["nombre"].ToString() ?? string.Empty
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(Eps eps)
        {
            if (eps == null)
                throw new ArgumentNullException(nameof(eps));

            const string query = "INSERT INTO eps (nombre) VALUES (@Name)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Name", eps.Name);

                var result = await command.ExecuteNonQueryAsync() > 0;
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Eps eps)
        {
            if (eps == null)
                throw new ArgumentNullException(nameof(eps));

            const string query = "UPDATE eps SET nombre = @Name WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Id", eps.Id);
                command.Parameters.AddWithValue("@Name", eps.Name);

                var result = await command.ExecuteNonQueryAsync() > 0;
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(object id)
        {
            const string query = "DELETE FROM eps WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Id", id);

                var result = await command.ExecuteNonQueryAsync() > 0;
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
