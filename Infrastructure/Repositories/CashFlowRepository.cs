using System;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Ports;
using InventoryManagement.Infrastructure.Mysql;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Infrastructure.Repositories
{
    public class CashFlowRepository : IGenericRepository<CashFlow>, ICashFlowRepository
    {
        private readonly MySqlConnection _connection;

        public CashFlowRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<CashFlow>> GetAllAsync()
        {
            List<CashFlow> movements = new List<CashFlow>();

            using var command = new MySqlCommand(
                "SELECT m.*, tm.nombre as tipo_nombre, tm.tipoMovimiento, " +
                "t.nombre as tercero_nombre, t.apellidos as tercero_apellidos " +
                "FROM movimientoCaja m " +
                "JOIN tipoMovCaja tm ON m.tipoMovimiento_id = tm.id " +
                "JOIN tercero t ON m.tercero_id = t.id " +
                "ORDER BY m.fecha DESC",
                _connection);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                movements .Add(new CashFlow
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Date = Convert.ToDateTime(reader["fecha"]),
                    MovementTypeId = Convert.ToInt32(reader["tipoMovimiento_id"]),
                    Cost = Convert.ToDecimal(reader["valor"]),
                    Concept = reader["concepto"].ToString()!,
                    PersonId = reader["tercero_id"].ToString()!,
                    MovementNameType = reader["tipo_nombre"].ToString(),
                    MovementType = reader["tipoMovimiento"].ToString(),
                    Person = new Person
                    {
                        Id = reader["tercero_id"].ToString()!,
                        Name = reader["tercero_nombre"].ToString()!,
                        LastName = reader["tercero_apellidos"].ToString()!
                    }
                });
            }

            return movements;
        }

        public async Task<CashFlow?> GetByIdAsync(object id)
        {
            CashFlow? movement = null;

            using var command = new MySqlCommand(
                "SELECT m.*, tm.nombre as tipo_nombre, tm.tipoMovimiento, " +
                "t.nombre as tercero_nombre, t.apellidos as tercero_apellidos " +
                "FROM movimientoCaja m " +
                "JOIN tipoMovCaja tm ON m.tipoMovimiento_id = tm.id " +
                "JOIN tercero t ON m.tercero_id = t.id " +
                "WHERE m.id = @Id",
                _connection);

            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                movement = new CashFlow
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Date = Convert.ToDateTime(reader["fecha"]),
                    MovementTypeId = Convert.ToInt32(reader["tipoMovimiento_id"]),
                    Cost = Convert.ToDecimal(reader["valor"]),
                    Concept = reader["concepto"].ToString()!,
                    PersonId = reader["tercero_id"].ToString()!,
                    MovementNameType = reader["tipo_nombre"].ToString(),
                    MovementType = reader["tipoMovimiento"].ToString(),
                    Person = new Person
                    {
                        Id = reader["tercero_id"].ToString()!,
                        Name = reader["tercero_nombre"].ToString()!,
                        LastName = reader["tercero_apellidos"].ToString()!
                    }
                };
            }

            return movement;
        }

        public async Task<bool> InsertAsync(CashFlow movement)
        {
            using var command = new MySqlCommand(
                "INSERT INTO movimientoCaja (fecha, tipoMovimiento_id, valor, concepto, tercero_id) " +
                "VALUES (@Fecha, @TipoMovimientoId, @Valor, @Concepto, @TerceroId)",
                _connection);

            command.Parameters.AddWithValue("@Fecha", movement.Date);
            command.Parameters.AddWithValue("@TipoMovimientoId", movement.MovementTypeId);
            command.Parameters.AddWithValue("@Valor", movement.Cost);
            command.Parameters.AddWithValue("@Concepto", movement.Concept);
            command.Parameters.AddWithValue("@TerceroId", movement.PersonId);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> UpdateAsync(CashFlow movement)
        {
            using var command = new MySqlCommand(
                "UPDATE movimientoCaja SET fecha = @Fecha, tipoMovimiento_id = @TipoMovimientoId, " +
                "valor = @Valor, concepto = @Concepto, tercero_id = @TerceroId " +
                "WHERE id = @Id",
                _connection);

            command.Parameters.AddWithValue("@Id", movement.Id);
            command.Parameters.AddWithValue("@Fecha", movement.Date);
            command.Parameters.AddWithValue("@TipoMovimientoId", movement.MovementTypeId);
            command.Parameters.AddWithValue("@Valor", movement.Cost);
            command.Parameters.AddWithValue("@Concepto", movement.Concept);
            command.Parameters.AddWithValue("@TerceroId", movement.PersonId);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(object id)
        {
            using var command = new MySqlCommand(
                "DELETE FROM movimientoCaja WHERE id = @Id",
                _connection);

            command.Parameters.AddWithValue("@Id", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<decimal> GetSaldoCajaAsync(DateTime date)
        {
            using var command = new MySqlCommand(
                "SELECT SUM(CASE WHEN tm.tipoMovimiento = 'Entrada' THEN m.valor ELSE -m.valor END) as saldo " +
                "FROM movimientoCaja m " +
                "JOIN tipoMovCaja tm ON m.tipoMovimiento_id = tm.id " +
                "WHERE DATE(m.fecha) = DATE(@Fecha)",
                _connection);

            command.Parameters.AddWithValue("@Fecha", date);

            var result = await command.ExecuteScalarAsync();

            if (result != null && result != DBNull.Value)
            {
                return Convert.ToDecimal(result);
            }

            return 0;
        }

        public async Task<IEnumerable<CashFlow>> GetMovimientosByFechaAsync(DateTime date)
        {
            List<CashFlow> movements = new List<CashFlow>();

            using var command = new MySqlCommand(
                "SELECT m.*, tm.nombre as tipo_nombre, tm.tipoMovimiento, " +
                "t.nombre as tercero_nombre, t.apellidos as tercero_apellidos " +
                "FROM movimientoCaja m " +
                "JOIN tipoMovCaja tm ON m.tipoMovimiento_id = tm.id " +
                "JOIN tercero t ON m.tercero_id = t.id " +
                "WHERE DATE(m.fecha) = DATE(@Fecha) " +
                "ORDER BY m.fecha DESC",
                _connection);

            command.Parameters.AddWithValue("@Fecha", date);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                movements.Add(new CashFlow
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Date = Convert.ToDateTime(reader["fecha"]),
                    MovementTypeId = Convert.ToInt32(reader["tipoMovimiento_id"]),
                    Cost = Convert.ToDecimal(reader["valor"]),
                    Concept = reader["concepto"].ToString()!,
                    PersonId = reader["tercero_id"].ToString()!,
                    MovementNameType = reader["tipo_nombre"].ToString(),
                    MovementType = reader["tipoMovimiento"].ToString(),
                    Person = new Person
                    {
                        Id = reader["tercero_id"].ToString()!,
                        Name = reader["tercero_nombre"].ToString()!,
                        LastName = reader["tercero_apellidos"].ToString()!
                    }
                });
            }

            return movements;
        }
    }
}
