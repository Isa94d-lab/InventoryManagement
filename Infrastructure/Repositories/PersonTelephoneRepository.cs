using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Ports;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Infrastructure.Repositories
{
    public class PersonTelephoneRepository : IGenericRepository<PersonTelephone>, IPersonTelephoneRepository
    {
        private readonly MySqlConnection _connection;

        public PersonTelephoneRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        // Obtener todos los teléfonos de todas las personas
        public async Task<IEnumerable<PersonTelephone>> GetAllAsync()
        {
            var phones = new List<PersonTelephone>();
            const string query = "SELECT id, numero, tercero_id, tipo_telefono FROM tercero_telefono";

            using var command = new MySqlCommand(query, _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                phones.Add(new PersonTelephone
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Number = reader["numero"].ToString() ?? string.Empty,
                    PersonId = reader["tercero_id"].ToString() ?? string.Empty,
                    PhoneType = reader["tipo_telefono"].ToString() ?? string.Empty
                });
            }

            return phones;
        }

        // Obtener un telefono por su ID
        public async Task<PersonTelephone?> GetByIdAsync(object id)
        {
            const string query = "SELECT id, numero, tercero_id, tipo_telefono FROM tercero_telefono WHERE id = @Id";
            using var command = new MySqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new PersonTelephone
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Number = reader["numero"].ToString() ?? string.Empty,
                    PersonId = reader["tercero_id"].ToString() ?? string.Empty,
                    PhoneType = reader["tipo_telefono"].ToString() ?? string.Empty
                };
            }

            return null;
        }

        // Insertar un nuevo teléfono para una persona
        public async Task<bool> InsertAsync(PersonTelephone phone)
        {
            if (phone == null)
                throw new ArgumentNullException(nameof(phone));

            const string query = "INSERT INTO tercero_telefono (numero, tercero_id, tipo_telefono) VALUES (@Number, @PersonId, @PhoneType)";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Number", phone.Number);
                command.Parameters.AddWithValue("@PersonId", phone.PersonId);
                command.Parameters.AddWithValue("@PhoneType", phone.PhoneType);

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

        // Actualizar los detalles de un teléfono
        public async Task<bool> UpdateAsync(PersonTelephone phone)
        {
            if (phone == null)
                throw new ArgumentNullException(nameof(phone));

            const string query = "UPDATE tercero_telefono SET numero = @Number, tipo_telefono = @PhoneType WHERE id = @Id";
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var command = new MySqlCommand(query, _connection, transaction);
                command.Parameters.AddWithValue("@Id", phone.Id);
                command.Parameters.AddWithValue("@Number", phone.Number);
                command.Parameters.AddWithValue("@PhoneType", phone.PhoneType);

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

        // Eliminar un teléfono por ID
        public async Task<bool> DeleteAsync(object id)
        {
            const string query = "DELETE FROM tercero_telefono WHERE id = @Id";
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
