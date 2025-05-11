using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Ports;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Infrastructure.Repositories
{
    public class ArlRepository : IGenericRepository<Arl>, IArlRepository
    {
        private readonly MySqlConnection _connection;

        public ArlRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Arl>> GetAllAsync()
        {
            var arlList = new List<Arl>();
            const string query = "SELECT id, nombre FROM arl";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                arlList.Add(new Arl
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["nombre"].ToString() ?? string.Empty
                });
            }

            return arlList;
        }

        public async Task<Arl?> GetByIdAsync(object id)
        {
            const string query = "SELECT id, nombre FROM arl WHERE id = @Id";
            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Arl
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["nombre"].ToString() ?? string.Empty
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(Arl arl)
        {
            if (arl == null)
                throw new ArgumentNullException(nameof(arl));

            const string query = "INSERT INTO arl (nombre) VALUES (@Name)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Name", arl.Name);

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

        public async Task<bool> UpdateAsync(Arl arl)
        {
            if (arl == null)
                throw new ArgumentNullException(nameof(arl));

            const string query = "UPDATE arl SET nombre = @Name WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Id", arl.Id);
                command.Parameters.AddWithValue("@Name", arl.Name);

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
            const string query = "DELETE FROM arl WHERE id = @Id";
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
                // commit
            }
        }
    }
}
