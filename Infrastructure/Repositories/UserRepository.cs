using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Ports;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MySqlConnection _connection;

        public UserRepository(MySqlConnection connection)
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

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = new List<User>();

            using var command = new MySqlCommand(@"
                SELECT u.*, t.nombre, t.apellidos 
                FROM usuario u
                INNER JOIN tercero t ON u.tercero_id = t.id", _connection);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new User
                {
                    Id = Convert.ToInt32(reader["id"]),
                    PersonId = reader["tercero_id"].ToString()!,
                    Username = reader["username"].ToString()!,
                    Password = reader["password"].ToString()!,
                    Role = reader["rol"].ToString()!,
                    CreationDate = Convert.ToDateTime(reader["fecha_creacion"]),
                    LastAccess = reader["ultimo_acceso"] == DBNull.Value ? null : Convert.ToDateTime(reader["ultimo_acceso"]),
                    IsActive = Convert.ToBoolean(reader["activo"]),
                    Person = new Person
                    {
                        Id = reader["tercero_id"].ToString()!,
                        Name = reader["nombre"].ToString()!,
                        LastName = reader["apellidos"].ToString()!
                    }
                });
            }

            return users;
        }

        public async Task<User?> GetByIdAsync(object id)
        {
            using var command = new MySqlCommand(@"
                SELECT u.*, t.nombre, t.apellidos 
                FROM usuario u
                INNER JOIN tercero t ON u.tercero_id = t.id
                WHERE u.id = @Id", _connection);

            command.Parameters.AddWithValue("@Id", id);
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = Convert.ToInt32(reader["id"]),
                    PersonId = reader["tercero_id"].ToString()!,
                    Username = reader["username"].ToString()!,
                    Password = reader["password"].ToString()!,
                    Role = reader["rol"].ToString()!,
                    CreationDate = Convert.ToDateTime(reader["fecha_creacion"]),
                    LastAccess = reader["ultimo_acceso"] == DBNull.Value ? null : Convert.ToDateTime(reader["ultimo_acceso"]),
                    IsActive = Convert.ToBoolean(reader["activo"]),
                    Person = new Person
                    {
                        Id = reader["tercero_id"].ToString()!,
                        Name = reader["nombre"].ToString()!,
                        LastName = reader["apellidos"].ToString()!
                    }
                };
            }

            return null;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            using var command = new MySqlCommand(@"
                SELECT u.*, t.nombre, t.apellidos 
                FROM usuario u
                INNER JOIN tercero t ON u.tercero_id = t.id
                WHERE u.username = @Username", _connection);

            command.Parameters.AddWithValue("@Username", username);
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = Convert.ToInt32(reader["id"]),
                    PersonId = reader["tercero_id"].ToString()!,
                    Username = reader["username"].ToString()!,
                    Password = reader["password"].ToString()!,
                    Role = reader["rol"].ToString()!,
                    CreationDate = Convert.ToDateTime(reader["fecha_creacion"]),
                    LastAccess = reader["ultimo_acceso"] == DBNull.Value ? null : Convert.ToDateTime(reader["ultimo_acceso"]),
                    IsActive = Convert.ToBoolean(reader["activo"]),
                    Person = new Person
                    {
                        Id = reader["tercero_id"].ToString()!,
                        Name = reader["nombre"].ToString()!,
                        LastName = reader["apellidos"].ToString()!
                    }
                };
            }

            return null;
        }

        public async Task<bool> InsertAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(user.PersonId))
                throw new ArgumentException("Person ID cannot be empty", nameof(user.PersonId));

            if (string.IsNullOrEmpty(user.Username))
                throw new ArgumentException("Username cannot be empty", nameof(user.Username));

            if (string.IsNullOrEmpty(user.Password))
                throw new ArgumentException("Password cannot be empty", nameof(user.Password));

            if (string.IsNullOrEmpty(user.Role))
                throw new ArgumentException("Role cannot be empty", nameof(user.Role));

            if (!await PersonExistsAsync(user.PersonId))
                throw new ArgumentException($"Person with ID {user.PersonId} does not exist", nameof(user.PersonId));

            using var transaction = await _connection.BeginTransactionAsync();
            try
            {
                using var command = new MySqlCommand(@"
                    INSERT INTO usuario 
                    (tercero_id, username, password, rol, fecha_creacion, activo)
                    VALUES (@PersonId, @Username, @Password, @Role, @CreationDate, @IsActive)",
                    _connection, transaction);

                command.Parameters.AddWithValue("@PersonId", user.PersonId);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@Role", user.Role);
                command.Parameters.AddWithValue("@CreationDate", DateTime.Now);
                command.Parameters.AddWithValue("@IsActive", true);

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

        public async Task<bool> UpdateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(user.PersonId))
                throw new ArgumentException("Person ID cannot be empty", nameof(user.PersonId));

            if (string.IsNullOrEmpty(user.Username))
                throw new ArgumentException("Username cannot be empty", nameof(user.Username));

            if (string.IsNullOrEmpty(user.Role))
                throw new ArgumentException("Role cannot be empty", nameof(user.Role));

            if (!await PersonExistsAsync(user.PersonId))
                throw new ArgumentException($"Person with ID {user.PersonId} does not exist", nameof(user.PersonId));

            using var transaction = await _connection.BeginTransactionAsync();
            try
            {
                using var command = new MySqlCommand(@"
                    UPDATE usuario SET
                        tercero_id = @PersonId,
                        username = @Username,
                        rol = @Role,
                        activo = @IsActive
                    WHERE id = @Id",
                    _connection, transaction);

                command.Parameters.AddWithValue("@Id", user.Id);
                command.Parameters.AddWithValue("@PersonId", user.PersonId);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Role", user.Role);
                command.Parameters.AddWithValue("@IsActive", user.IsActive);

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
                using var command = new MySqlCommand("DELETE FROM usuario WHERE id = @Id", _connection, transaction);
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

        public async Task<bool> UpdateLastAccessAsync(int userId)
        {
            using var command = new MySqlCommand(@"
                UPDATE usuario 
                SET ultimo_acceso = @LastAccess
                WHERE id = @Id", _connection);

            command.Parameters.AddWithValue("@Id", userId);
            command.Parameters.AddWithValue("@LastAccess", DateTime.Now);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdatePasswordAsync(int userId, string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
                throw new ArgumentException("New password cannot be empty", nameof(newPassword));

            using var command = new MySqlCommand(@"
                UPDATE usuario 
                SET password = @Password
                WHERE id = @Id", _connection);

            command.Parameters.AddWithValue("@Id", userId);
            command.Parameters.AddWithValue("@Password", newPassword);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateStatusAsync(int userId, bool isActive)
        {
            using var command = new MySqlCommand(@"
                UPDATE usuario 
                SET activo = @IsActive
                WHERE id = @Id", _connection);

            command.Parameters.AddWithValue("@Id", userId);
            command.Parameters.AddWithValue("@IsActive", isActive);

            return await command.ExecuteNonQueryAsync() > 0;
        }
    }
} 