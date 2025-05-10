using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Ports;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Infrastructure.Repositories
{
    public class CityRepository : IGenericRepository<City>, ICityRepository
    {
        private readonly MySqlConnection _connection;

        public CityRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<City>> GetAllAsync()
        {
            var cities = new List<City>();
            const string query = "SELECT id, nombre, region_id FROM ciudad";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                cities.Add(new City
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["nombre"].ToString() ?? string.Empty,
                    RegionId = Convert.ToInt32(reader["region_id"])
                });
            }

            return cities;
        }

        public async Task<City?> GetByIdAsync(object id)
        {
            const string query = "SELECT id, nombre, region_id FROM ciudad WHERE id = @Id";
            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new City
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["nombre"].ToString() ?? string.Empty,
                    RegionId = Convert.ToInt32(reader["region_id"])
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(City city)
        {
            if (city == null)
                throw new ArgumentNullException(nameof(city));

            const string query = "INSERT INTO ciudad (nombre, region_id) VALUES (@Name, @RegionId)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Name", city.Name);
                command.Parameters.AddWithValue("@RegionId", city.RegionId);

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

        public async Task<bool> UpdateAsync(City city)
        {
            if (city == null)
                throw new ArgumentNullException(nameof(city));

            const string query = "UPDATE ciudad SET nombre = @Name, region_id = @RegionId WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Id", city.Id);
                command.Parameters.AddWithValue("@Name", city.Name);
                command.Parameters.AddWithValue("@RegionId", city.RegionId);

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
            const string query = "DELETE FROM ciudad WHERE id = @Id";
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
