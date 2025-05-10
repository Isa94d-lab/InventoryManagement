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

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            var customers = new List<Customer>();
            using var command = new MySqlCommand("SELECT * FROM cliente", _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                customers.Add(new Customer
                {
                    Id = Convert.ToInt32(reader["id"]),
                    PersonId = reader["tercero_id"].ToString()!,
                    Birthdate = Convert.ToDateTime(reader["fecha_nacimiento"]),
                    PurchaseDate = reader["fecha_compra"] == DBNull.Value ? null : Convert.ToDateTime(reader["fecha_compra"])
                });
            }

            return customers;
        }

        public async Task<Customer?> GetByIdAsync(object id)
        {
            Customer? customer = null;
            using var command = new MySqlCommand("SELECT * FROM cliente WHERE id = @Id", _connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                customer = new Customer
                {
                    Id = Convert.ToInt32(reader["id"]),
                    PersonId = reader["tercero_id"].ToString()!,
                    Birthdate = Convert.ToDateTime(reader["fecha_nacimiento"]),
                    PurchaseDate = reader["fecha_compra"] == DBNull.Value ? null : Convert.ToDateTime(reader["fecha_compra"])
                };
            }

            return customer;
        }

        public async Task<bool> InsertAsync(Customer customer)
        {
            using var command = new MySqlCommand(
                "INSERT INTO cliente (tercero_id, fecha_nacimiento, fecha_compra) VALUES (@PersonId, @Birthdate, @PurchaseDate)",
                _connection);

            command.Parameters.AddWithValue("@PersonId", customer.PersonId);
            command.Parameters.AddWithValue("@Birthdate", customer.Birthdate);
            command.Parameters.AddWithValue("@PurchaseDate", customer.PurchaseDate ?? (object)DBNull.Value);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Customer customer)
        {
            using var command = new MySqlCommand(
                "UPDATE cliente SET tercero_id = @PersonId, fecha_nacimiento = @Birthdate, fecha_compra = @PurchaseDate WHERE id = @Id",
                _connection);

            command.Parameters.AddWithValue("@Id", customer.Id);
            command.Parameters.AddWithValue("@PersonId", customer.PersonId);
            command.Parameters.AddWithValue("@Birthdate", customer.Birthdate);
            command.Parameters.AddWithValue("@PurchaseDate", customer.PurchaseDate ?? (object)DBNull.Value);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(object id)
        {
            using var command = new MySqlCommand("DELETE FROM cliente WHERE id = @Id", _connection);
            command.Parameters.AddWithValue("@Id", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }
    }
}
