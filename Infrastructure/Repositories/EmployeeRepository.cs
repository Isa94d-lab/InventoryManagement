using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Ports;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Infrastructure.Repositories
{
    public class EmployeeRepository : IGenericRepository<Employee>, IEmployeeRepository
    {
        private readonly MySqlConnection _connection;

        public EmployeeRepository(MySqlConnection connection)
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

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            var employees = new List<Employee>();

            using var command = new MySqlCommand(@"
                SELECT e.*, t.nombre, t.apellidos 
                FROM empleado e
                INNER JOIN tercero t ON e.tercero_id = t.id", _connection);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                employees.Add(new Employee
                {
                    Id = Convert.ToInt32(reader["id"]),
                    PersonId = reader["tercero_id"].ToString()!,
                    BaseSalary = Convert.ToDecimal(reader["salario_base"]),
                    ArlId = Convert.ToInt32(reader["arl_id"]),
                    EpsId = Convert.ToInt32(reader["eps_id"]),
                    Person = new Person
                    {
                        Id = reader["tercero_id"].ToString()!,
                        Name = reader["nombre"].ToString()!,
                        LastName = reader["apellidos"].ToString()!
                    }
                });
            }

            return employees;
        }

        public async Task<Employee?> GetByIdAsync(object id)
        {
            Employee? employee = null;

            using var command = new MySqlCommand(@"
                SELECT e.*, t.nombre, t.apellidos 
                FROM empleado e
                INNER JOIN tercero t ON e.tercero_id = t.id
                WHERE e.id = @Id", _connection);

            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                employee = new Employee
                {
                    Id = Convert.ToInt32(reader["id"]),
                    PersonId = reader["tercero_id"].ToString()!,
                    BaseSalary = Convert.ToDecimal(reader["salario_base"]),
                    ArlId = Convert.ToInt32(reader["arl_id"]),
                    EpsId = Convert.ToInt32(reader["eps_id"]),
                    Person = new Person
                    {
                        Id = reader["tercero_id"].ToString()!,
                        Name = reader["nombre"].ToString()!,
                        LastName = reader["apellidos"].ToString()!
                    }
                };
            }

            return employee;
        }

        public async Task<bool> InsertAsync(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            if (string.IsNullOrEmpty(employee.PersonId))
                throw new ArgumentException("Person ID cannot be empty", nameof(employee.PersonId));

            if (employee.BaseSalary <= 0)
                throw new ArgumentException("Base salary must be greater than zero", nameof(employee.BaseSalary));

            if (employee.ArlId <= 0)
                throw new ArgumentException("ARL ID must be greater than zero", nameof(employee.ArlId));

            if (employee.EpsId <= 0)
                throw new ArgumentException("EPS ID must be greater than zero", nameof(employee.EpsId));

            // Verificar si el tercero existe
            if (!await PersonExistsAsync(employee.PersonId))
                throw new ArgumentException($"Person with ID {employee.PersonId} does not exist", nameof(employee.PersonId));

            using var transaction = await _connection.BeginTransactionAsync();
            try
            {
                using var command = new MySqlCommand(
                    "INSERT INTO empleado (tercero_id, salario_base, arl_id, eps_id) VALUES (@PersonId, @BaseSalary, @ArlId, @EpsId)",
                    _connection,
                    transaction);

                command.Parameters.AddWithValue("@PersonId", employee.PersonId);
                command.Parameters.AddWithValue("@BaseSalary", employee.BaseSalary);
                command.Parameters.AddWithValue("@ArlId", employee.ArlId);
                command.Parameters.AddWithValue("@EpsId", employee.EpsId);

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

        public async Task<bool> UpdateAsync(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            if (string.IsNullOrEmpty(employee.PersonId))
                throw new ArgumentException("Person ID cannot be empty", nameof(employee.PersonId));

            if (employee.BaseSalary <= 0)
                throw new ArgumentException("Base salary must be greater than zero", nameof(employee.BaseSalary));

            if (employee.ArlId <= 0)
                throw new ArgumentException("ARL ID must be greater than zero", nameof(employee.ArlId));

            if (employee.EpsId <= 0)
                throw new ArgumentException("EPS ID must be greater than zero", nameof(employee.EpsId));

            // Verificar si el tercero existe
            if (!await PersonExistsAsync(employee.PersonId))
                throw new ArgumentException($"Person with ID {employee.PersonId} does not exist", nameof(employee.PersonId));

            using var transaction = await _connection.BeginTransactionAsync();
            try
            {
                using var command = new MySqlCommand(
                    "UPDATE empleado SET tercero_id = @PersonId, salario_base = @BaseSalary, arl_id = @ArlId, eps_id = @EpsId WHERE id = @Id",
                    _connection,
                    transaction);

                command.Parameters.AddWithValue("@Id", employee.Id);
                command.Parameters.AddWithValue("@PersonId", employee.PersonId);
                command.Parameters.AddWithValue("@BaseSalary", employee.BaseSalary);
                command.Parameters.AddWithValue("@ArlId", employee.ArlId);
                command.Parameters.AddWithValue("@EpsId", employee.EpsId);

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
                using var command = new MySqlCommand("DELETE FROM empleado WHERE id = @Id", _connection, transaction);
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
