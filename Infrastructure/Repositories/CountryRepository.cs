using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Ports;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Infrastructure.Repositories
{
    public class CountryRepository : IGenericRepository<Country>, ICountryRepository
    {
        private readonly MySqlConnection _connection;

        public CountryRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            var countries = new List<Country>();
            const string query = "SELECT id, nombre FROM pais";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                countries.Add(new Country
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["nombre"].ToString() ?? string.Empty
                });
            }

            return countries;
        }

        public async Task<Country?> GetByIdAsync(object id)
        {
            const string query = "SELECT id, nombre FROM pais WHERE id = @Id";
            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Country
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["nombre"].ToString() ?? string.Empty
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(Country country)
        {
            if (country == null)
                throw new ArgumentNullException(nameof(country));

            const string query = "INSERT INTO pais (nombre) VALUES (@Name)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Name", country.Name);

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

        public async Task<bool> UpdateAsync(Country country)
        {
            if (country == null)
                throw new ArgumentNullException(nameof(country));

            const string query = "UPDATE pais SET nombre = @Name WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Id", country.Id);
                command.Parameters.AddWithValue("@Name", country.Name);

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
            const string query = "DELETE FROM pais WHERE id = @Id";
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
