using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Ports;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Infrastructure.Repositories
{
    public class RegionRepository : IGenericRepository<Region>, IRegionRepository
    {
        private readonly MySqlConnection _connection;

        public RegionRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Region>> GetAllAsync()
        {
            var regions = new List<Region>();
            const string query = "SELECT id, nombre, pais_id FROM region";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                regions.Add(new Region
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["nombre"].ToString() ?? string.Empty,
                    CountryId = Convert.ToInt32(reader["pais_id"])
                });
            }

            return regions;
        }

        public async Task<Region?> GetByIdAsync(object id)
        {
            const string query = "SELECT id, nombre, pais_id FROM region WHERE id = @Id";
            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Region
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["nombre"].ToString() ?? string.Empty,
                    CountryId = Convert.ToInt32(reader["pais_id"])
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(Region region)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            const string query = "INSERT INTO region (nombre, pais_id) VALUES (@Name, @CountryId)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Name", region.Name);
                command.Parameters.AddWithValue("@CountryId", region.CountryId);

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

        public async Task<bool> UpdateAsync(Region region)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            const string query = "UPDATE region SET nombre = @Name, pais_id = @CountryId WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Id", region.Id);
                command.Parameters.AddWithValue("@Name", region.Name);
                command.Parameters.AddWithValue("@CountryId", region.CountryId);

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
            const string query = "DELETE FROM region WHERE id = @Id";
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
