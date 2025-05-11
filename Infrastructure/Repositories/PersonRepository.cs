using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Ports;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Infrastructure.Repositories
{
    public class PersonRepository : IGenericRepository<Person>, IPersonRepository
    {
        private readonly MySqlConnection _connection;

        public PersonRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            var persons = new List<Person>();

            using var command = new MySqlCommand("SELECT * FROM tercero", _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                persons.Add(new Person
                {
                    Id = reader["id"].ToString()!,
                    Name = reader["nombre"].ToString()!,
                    LastName = reader["apellidos"].ToString()!,
                    Email = reader["email"].ToString()!,
                    DocumentTypeId = Convert.ToInt32(reader["tipo_documento_id"]),
                    PersonTypeId = Convert.ToInt32(reader["tipo_tercero_id"]),
                    CityId = Convert.ToInt32(reader["ciudad_id"])
                });
            }

            return persons;
        }

        public async Task<Person?> GetByIdAsync(object id)
        {
            var stringId = id.ToString(); // Convertir el parámetro object a string

            using var command = new MySqlCommand("SELECT * FROM tercero WHERE id = @Id", _connection);
            command.Parameters.AddWithValue("@Id", stringId);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Person
                {
                    Id = reader["id"].ToString()!,
                    Name = reader["nombre"].ToString()!,
                    LastName = reader["apellidos"].ToString()!,
                    Email = reader["email"].ToString()!,
                    DocumentTypeId = Convert.ToInt32(reader["tipo_documento_id"]),
                    PersonTypeId = Convert.ToInt32(reader["tipo_tercero_id"]),
                    CityId = Convert.ToInt32(reader["ciudad_id"])
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(Person person)
        {
            using var transaction = await _connection.BeginTransactionAsync();
            try
            {
                using var command = new MySqlCommand(@"
                    INSERT INTO tercero 
                    (id, nombre, apellidos, email, tipo_documento_id, tipo_tercero_id, ciudad_id)
                    VALUES (@Id, @Name, @LastName, @Email, @DocTypeId, @PersonTypeId, @CityId)", _connection, transaction);

                command.Parameters.AddWithValue("@Id", person.Id);
                command.Parameters.AddWithValue("@Name", person.Name);
                command.Parameters.AddWithValue("@LastName", person.LastName);
                command.Parameters.AddWithValue("@Email", person.Email);
                command.Parameters.AddWithValue("@DocTypeId", person.DocumentTypeId);
                command.Parameters.AddWithValue("@PersonTypeId", person.PersonTypeId);
                command.Parameters.AddWithValue("@CityId", person.CityId);

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

        public async Task<bool> UpdateAsync(Person person)
        {
            using var transaction = await _connection.BeginTransactionAsync();
            try
            {
                using var command = new MySqlCommand(@"
                    UPDATE tercero SET
                        nombre = @Name,
                        apellidos = @LastName,
                        email = @Email,
                        tipo_documento_id = @DocTypeId,
                        tipo_tercero_id = @PersonTypeId,
                        ciudad_id = @CityId
                    WHERE id = @Id", _connection, transaction);

                command.Parameters.AddWithValue("@Id", person.Id);
                command.Parameters.AddWithValue("@Name", person.Name);
                command.Parameters.AddWithValue("@LastName", person.LastName);
                command.Parameters.AddWithValue("@Email", person.Email);
                command.Parameters.AddWithValue("@DocTypeId", person.DocumentTypeId);
                command.Parameters.AddWithValue("@PersonTypeId", person.PersonTypeId);
                command.Parameters.AddWithValue("@CityId", person.CityId);

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
            var stringId = id.ToString(); // Convertir el parámetro object a string

            using var transaction = await _connection.BeginTransactionAsync();
            try
            {
                using var command = new MySqlCommand("DELETE FROM tercero WHERE id = @Id", _connection, transaction);
                command.Parameters.AddWithValue("@Id", stringId);

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
