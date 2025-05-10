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

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            var employees = new List<Employee>();

            using var command = new MySqlCommand("SELECT * FROM empleado", _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                employees.Add(new Employee
                {
                    Id = Convert.ToInt32(reader["id"]),
                    PersonId = reader["tercero_id"].ToString()!,
                    JoinDate = Convert.ToDateTime(reader["fecha_ingreso"]),
                    BaseSalary = Convert.ToDouble(reader["salario_base"]),
                    EpsId = Convert.ToInt32(reader["eps_id"]),
                    ArlId = Convert.ToInt32(reader["arl_id"])
                });
            }

            return employees;
        }

        public async Task<Employee?> GetByIdAsync(object id)
        {
            Employee? employee = null;

            using var command = new MySqlCommand("SELECT * FROM empleado WHERE id = @Id", _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                employee = new Employee
                {
                    Id = Convert.ToInt32(reader["id"]),
                    PersonId = reader["tercero_id"].ToString()!,
                    JoinDate = Convert.ToDateTime(reader["fecha_ingreso"]),
                    BaseSalary = Convert.ToDouble(reader["salario_base"]),
                    EpsId = Convert.ToInt32(reader["eps_id"]),
                    ArlId = Convert.ToInt32(reader["arl_id"])
                };
            }

            return employee;
        }

        public async Task<bool> InsertAsync(Employee employee)
        {
            using var command = new MySqlCommand(@"
                INSERT INTO empleado (tercero_id, fecha_ingreso, salario_base, eps_id, arl_id)
                VALUES (@TerceroId, @FechaIngreso, @SalarioBase, @EpsId, @ArlId)", _connection);

            command.Parameters.AddWithValue("@TerceroId", employee.PersonId);
            command.Parameters.AddWithValue("@FechaIngreso", employee.JoinDate);
            command.Parameters.AddWithValue("@SalarioBase", employee.BaseSalary);
            command.Parameters.AddWithValue("@EpsId", employee.EpsId);
            command.Parameters.AddWithValue("@ArlId", employee.ArlId);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Employee employee)
        {
            using var command = new MySqlCommand(@"
                UPDATE empleado 
                SET tercero_id = @TerceroId, fecha_ingreso = @FechaIngreso, salario_base = @SalarioBase, 
                    eps_id = @EpsId, arl_id = @ArlId
                WHERE id = @Id", _connection);

            command.Parameters.AddWithValue("@Id", employee.Id);
            command.Parameters.AddWithValue("@TerceroId", employee.PersonId);
            command.Parameters.AddWithValue("@FechaIngreso", employee.JoinDate);
            command.Parameters.AddWithValue("@SalarioBase", employee.BaseSalary);
            command.Parameters.AddWithValue("@EpsId", employee.EpsId);
            command.Parameters.AddWithValue("@ArlId", employee.ArlId);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(object id)
        {
            using var command = new MySqlCommand("DELETE FROM empleado WHERE id = @Id", _connection);
            command.Parameters.AddWithValue("@Id", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }
    }
}
