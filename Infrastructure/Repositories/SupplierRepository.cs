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

        private async Task<bool> PersonExistsAsync(string personId)
        {
            using var command = new MySqlCommand("SELECT COUNT(*) FROM tercero WHERE id = @PersonId", _connection);
            command.Parameters.AddWithValue("@PersonId", personId);
            var count = Convert.ToInt32(await command.ExecuteScalarAsync());
            return count > 0;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            var suppliers = new List<Supplier>();

            using var command = new MySqlCommand(@"
                SELECT p.*, t.nombre, t.apellidos 
                FROM proveedor p
                INNER JOIN tercero t ON p.tercero_id = t.id", _connection);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                suppliers.Add(new Supplier
                {
                    Id = Convert.ToInt32(reader["id"]),
                    PersonId = reader["tercero_id"].ToString()!,
                    Discount = Convert.ToDecimal(reader["descuento"]),
                    PayDay = Convert.ToInt32(reader["dia_pago"]),
                    Person = new Person
                    {
                        Id = reader["tercero_id"].ToString()!,
                        Name = reader["nombre"].ToString()!,
                        LastName = reader["apellidos"].ToString()!
                    }
                });
            }

            return suppliers;
        }

        public async Task<Supplier?> GetByIdAsync(object id)
        {
            Supplier? supplier = null;

            using var command = new MySqlCommand(@"
                SELECT p.*, t.nombre, t.apellidos 
                FROM proveedor p
                INNER JOIN tercero t ON p.tercero_id = t.id
                WHERE p.id = @Id", _connection);

            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                supplier = new Supplier
                {
                    Id = Convert.ToInt32(reader["id"]),
                    PersonId = reader["tercero_id"].ToString()!,
                    Discount = Convert.ToDecimal(reader["descuento"]),
                    PayDay = Convert.ToInt32(reader["dia_pago"]),
                    Person = new Person
                    {
                        Id = reader["tercero_id"].ToString()!,
                        Name = reader["nombre"].ToString()!,
                        LastName = reader["apellidos"].ToString()!
                    }
                };
            }

            return supplier;
        }

        public async Task<bool> InsertAsync(Supplier supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            if (string.IsNullOrEmpty(supplier.PersonId))
                throw new ArgumentException("Person ID cannot be empty", nameof(supplier.PersonId));

            if (supplier.Discount < 0)
                throw new ArgumentException("Discount cannot be negative", nameof(supplier.Discount));

            if (supplier.PayDay < 1 || supplier.PayDay > 31)
                throw new ArgumentException("Pay Day must be between 1 and 31", nameof(supplier.PayDay));

            if (!await PersonExistsAsync(supplier.PersonId))
                throw new ArgumentException($"Person with ID {supplier.PersonId} does not exist", nameof(supplier.PersonId));

            using var transaction = await _connection.BeginTransactionAsync();
            try
            {
                using var command = new MySqlCommand(
                    "INSERT INTO proveedor (tercero_id, descuento, dia_pago) VALUES (@PersonId, @Discount, @PayDay)",
                    _connection,
                    transaction);

                command.Parameters.AddWithValue("@PersonId", supplier.PersonId);
                command.Parameters.AddWithValue("@Discount", supplier.Discount);
                command.Parameters.AddWithValue("@PayDay", supplier.PayDay);

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

        public async Task<bool> UpdateAsync(Supplier supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            if (string.IsNullOrEmpty(supplier.PersonId))
                throw new ArgumentException("Person ID cannot be empty", nameof(supplier.PersonId));

            if (supplier.Discount < 0)
                throw new ArgumentException("Discount cannot be negative", nameof(supplier.Discount));

            if (supplier.PayDay < 1 || supplier.PayDay > 31)
                throw new ArgumentException("Pay Day must be between 1 and 31", nameof(supplier.PayDay));

            if (!await PersonExistsAsync(supplier.PersonId))
                throw new ArgumentException($"Person with ID {supplier.PersonId} does not exist", nameof(supplier.PersonId));

            using var transaction = await _connection.BeginTransactionAsync();
            try
            {
                using var command = new MySqlCommand(
                    "UPDATE proveedor SET tercero_id = @PersonId, descuento = @Discount, dia_pago = @PayDay WHERE id = @Id",
                    _connection,
                    transaction);

                command.Parameters.AddWithValue("@Id", supplier.Id);
                command.Parameters.AddWithValue("@PersonId", supplier.PersonId);
                command.Parameters.AddWithValue("@Discount", supplier.Discount);
                command.Parameters.AddWithValue("@PayDay", supplier.PayDay);

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
                using var command = new MySqlCommand("DELETE FROM proveedor WHERE id = @Id", _connection, transaction);
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
