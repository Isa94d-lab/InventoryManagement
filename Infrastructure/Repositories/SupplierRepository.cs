using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Ports;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Infrastructure.Repositories
{
    public class SupplierRepository : IGenericRepository<Supplier>, ISupplierRepository
    {
        private readonly MySqlConnection _connection;

        public SupplierRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            var suppliers = new List<Supplier>();

            using var command = new MySqlCommand("SELECT * FROM proveedor", _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                suppliers.Add(new Supplier
                {
                    Id = Convert.ToInt32(reader["id"]),
                    PersonId = reader["tercero_id"].ToString()!,
                    Discount = Convert.ToDouble(reader["descuento"]),
                    PayDay = Convert.ToInt32(reader["dia_pago"])
                });
            }

            return suppliers;
        }

        public async Task<Supplier?> GetByIdAsync(object id)
        {
            Supplier? supplier = null;

            using var command = new MySqlCommand("SELECT * FROM proveedor WHERE id = @Id", _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                supplier = new Supplier
                {
                    Id = Convert.ToInt32(reader["id"]),
                    PersonId = reader["tercero_id"].ToString()!,
                    Discount = Convert.ToDouble(reader["descuento"]),
                    PayDay = Convert.ToInt32(reader["dia_pago"])
                };
            }

            return supplier;
        }

        public async Task<bool> InsertAsync(Supplier supplier)
        {
            using var command = new MySqlCommand(
                "INSERT INTO proveedor (tercero_id, descuento, dia_pago) VALUES (@PersonId, @Discount, @PayDay)",
                _connection);

            command.Parameters.AddWithValue("@PersonId", supplier.PersonId);
            command.Parameters.AddWithValue("@Discount", supplier.Discount);
            command.Parameters.AddWithValue("@PayDay", supplier.PayDay);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Supplier supplier)
        {
            using var command = new MySqlCommand(
                "UPDATE proveedor SET tercero_id = @PersonId, descuento = @Discount, dia_pago = @PayDay WHERE id = @Id",
                _connection);

            command.Parameters.AddWithValue("@Id", supplier.Id);
            command.Parameters.AddWithValue("@PersonId", supplier.PersonId);
            command.Parameters.AddWithValue("@Discount", supplier.Discount);
            command.Parameters.AddWithValue("@PayDay", supplier.PayDay);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(object id)
        {
            using var command = new MySqlCommand("DELETE FROM proveedor WHERE id = @Id", _connection);
            command.Parameters.AddWithValue("@Id", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }
    }
}
