using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Ports;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Infrastructure.Repositories
{
    public class CustomerRepository : IGenericRepository<Customer>, ICustomerRepository
    {
        private readonly MySqlConnection _connection;

        public CustomerRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        private async Task<bool> PersonExistsAsync(string personId)
        {
            using var command = new MySqlCommand("SELECT COUNT(*) FROM tercero WHERE id = @PersonId", _connection);
            command.Parameters.AddWithValue("@PersonId", personId);
            var count = Convert.ToInt32(await command.ExecuteScalarAsync());
            return count > 0;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            var customers = new List<Customer>();
            using var command = new MySqlCommand(@"
                SELECT c.*, t.nombre, t.apellidos 
                FROM cliente c
                INNER JOIN tercero t ON c.tercero_id = t.id", _connection);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                customers.Add(new Customer
                {
                    Id = Convert.ToInt32(reader["id"]),
                    PersonId = reader["tercero_id"].ToString()!,
                    Birthdate = Convert.ToDateTime(reader["fecha_nacimiento"]),
                    PurchaseDate = reader["fecha_compra"] == DBNull.Value ? null : Convert.ToDateTime(reader["fecha_compra"]),
                    Person = new Person
                    {
                        Id = reader["tercero_id"].ToString()!,
                        Name = reader["nombre"].ToString()!,
                        LastName = reader["apellidos"].ToString()!
                    }
                });
            }

            return customers;
        }

        public async Task<Customer?> GetByIdAsync(object id)
        {
            Customer? customer = null;
            using var command = new MySqlCommand(@"
                SELECT c.*, t.nombre, t.apellidos 
                FROM cliente c
                INNER JOIN tercero t ON c.tercero_id = t.id
                WHERE c.id = @Id", _connection);

            command.Parameters.AddWithValue("@Id", id);
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                customer = new Customer
                {
                    Id = Convert.ToInt32(reader["id"]),
                    PersonId = reader["tercero_id"].ToString()!,
                    Birthdate = Convert.ToDateTime(reader["fecha_nacimiento"]),
                    PurchaseDate = reader["fecha_compra"] == DBNull.Value ? null : Convert.ToDateTime(reader["fecha_compra"]),
                    Person = new Person
                    {
                        Id = reader["tercero_id"].ToString()!,
                        Name = reader["nombre"].ToString()!,
                        LastName = reader["apellidos"].ToString()!
                    }
                };
            }

            return customer;
        }

        public async Task<bool> InsertAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (string.IsNullOrEmpty(customer.PersonId))
                throw new ArgumentException("Person ID cannot be empty", nameof(customer.PersonId));

            if (customer.Birthdate > DateTime.Now)
                throw new ArgumentException("Birthdate cannot be in the future", nameof(customer.Birthdate));

            if (customer.PurchaseDate.HasValue && customer.PurchaseDate.Value > DateTime.Now)
                throw new ArgumentException("Purchase date cannot be in the future", nameof(customer.PurchaseDate));

            if (!await PersonExistsAsync(customer.PersonId))
                throw new ArgumentException($"Person with ID {customer.PersonId} does not exist", nameof(customer.PersonId));

            using var transaction = await _connection.BeginTransactionAsync();
            try
            {
                using var command = new MySqlCommand(
                    "INSERT INTO cliente (tercero_id, fecha_nacimiento, fecha_compra) VALUES (@PersonId, @Birthdate, @PurchaseDate)",
                    _connection,
                    transaction);

                command.Parameters.AddWithValue("@PersonId", customer.PersonId);
                command.Parameters.AddWithValue("@Birthdate", customer.Birthdate);
                command.Parameters.AddWithValue("@PurchaseDate", customer.PurchaseDate ?? (object)DBNull.Value);

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

        public async Task<bool> UpdateAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (string.IsNullOrEmpty(customer.PersonId))
                throw new ArgumentException("Person ID cannot be empty", nameof(customer.PersonId));

            if (customer.Birthdate > DateTime.Now)
                throw new ArgumentException("Birthdate cannot be in the future", nameof(customer.Birthdate));

            if (customer.PurchaseDate.HasValue && customer.PurchaseDate.Value > DateTime.Now)
                throw new ArgumentException("Purchase date cannot be in the future", nameof(customer.PurchaseDate));

            if (!await PersonExistsAsync(customer.PersonId))
                throw new ArgumentException($"Person with ID {customer.PersonId} does not exist", nameof(customer.PersonId));

            using var transaction = await _connection.BeginTransactionAsync();
            try
            {
                using var command = new MySqlCommand(
                    "UPDATE cliente SET tercero_id = @PersonId, fecha_nacimiento = @Birthdate, fecha_compra = @PurchaseDate WHERE id = @Id",
                    _connection,
                    transaction);

                command.Parameters.AddWithValue("@Id", customer.Id);
                command.Parameters.AddWithValue("@PersonId", customer.PersonId);
                command.Parameters.AddWithValue("@Birthdate", customer.Birthdate);
                command.Parameters.AddWithValue("@PurchaseDate", customer.PurchaseDate ?? (object)DBNull.Value);

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
            using var transaction = await _connection.BeginTransactionAsync();
            try
            {
                using var command = new MySqlCommand("DELETE FROM cliente WHERE id = @Id", _connection, transaction);
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
