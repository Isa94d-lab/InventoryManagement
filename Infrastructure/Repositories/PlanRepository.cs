using System;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Ports;
using InventoryManagement.Infrastructure.Mysql;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Infrastructure.Repositories
{
    public class PlanRepository : IGenericRepository<PromotionalPlan>, IPromotionalPlanRepository
    {
        private readonly MySqlConnection _connection;

        public PlanRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<PromotionalPlan>> GetAllAsync()
        {
            List<PromotionalPlan> plans = new List<PromotionalPlan>();

            using var command = new MySqlCommand("SELECT * FROM plan", _connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var plan = new PromotionalPlan
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["nombre"].ToString()!,
                    StartDate = Convert.ToDateTime(reader["fecha_inicio"]),
                    EndDate = Convert.ToDateTime(reader["fecha_fin"]),
                    Discount = Convert.ToDecimal(reader["descuento"])
                };

                plans.Add(plan);
            }

            reader.Close();

            foreach (var plan in plans)
            {
                plan.Products = (await GetProductosByPlanAsync(plan.Id)).ToList();
            }

            return plans;
        }

        public async Task<PromotionalPlan?> GetByIdAsync(object id)
        {
            PromotionalPlan? plan = null;

            using var command = new MySqlCommand("SELECT * FROM plan WHERE id = @Id", _connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                plan = new PromotionalPlan
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["nombre"].ToString()!,
                    StartDate = Convert.ToDateTime(reader["fecha_inicio"]),
                    EndDate = Convert.ToDateTime(reader["fecha_fin"]),
                    Discount = Convert.ToDecimal(reader["descuento"])
                };
            }

            reader.Close();

            if (plan != null)
            {
                plan.Products = (await GetProductosByPlanAsync(plan.Id)).ToList();
            }

            return plan;
        }

        public async Task<bool> InsertAsync(PromotionalPlan plan)
        {
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var commandPlan = new MySqlCommand(
                    "INSERT INTO plan (nombre, fecha_inicio, fecha_fin, descuento) " +
                    "VALUES (@Nombre, @FechaInicio, @FechaFin, @Descuento); " +
                    "SELECT LAST_INSERT_ID();",
                    _connection);
                commandPlan.Transaction = transaction;

                commandPlan.Parameters.AddWithValue("@Nombre", plan.Name);
                commandPlan.Parameters.AddWithValue("@FechaInicio", plan.StartDate);
                commandPlan.Parameters.AddWithValue("@FechaFin", plan.EndDate);
                commandPlan.Parameters.AddWithValue("@Descuento", plan.Discount);

                var planId = Convert.ToInt32(await commandPlan.ExecuteScalarAsync());
                plan.Id = planId;

                foreach (var product in plan.Products)
                {
                    using var commandRelacion = new MySqlCommand(
                        "INSERT INTO plan_producto (plan_id, producto_id) VALUES (@PlanId, @ProductoId)",
                        _connection);
                    commandRelacion.Transaction = transaction;

                    commandRelacion.Parameters.AddWithValue("@PlanId", planId);
                    commandRelacion.Parameters.AddWithValue("@ProductoId", product.Id);

                    await commandRelacion.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(PromotionalPlan plan)
        {
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                using var commandPlan = new MySqlCommand(
                    "UPDATE plan SET nombre = @Nombre, fecha_inicio = @FechaInicio, fecha_fin = @FechaFin, descuento = @Descuento WHERE id = @Id",
                    _connection);
                commandPlan.Transaction = transaction;

                commandPlan.Parameters.AddWithValue("@Id", plan.Id);
                commandPlan.Parameters.AddWithValue("@Nombre", plan.Name);
                commandPlan.Parameters.AddWithValue("@FechaInicio", plan.StartDate);
                commandPlan.Parameters.AddWithValue("@FechaFin", plan.EndDate);
                commandPlan.Parameters.AddWithValue("@Descuento", plan.Discount);

                await commandPlan.ExecuteNonQueryAsync();

                using var commandEliminar = new MySqlCommand("DELETE FROM plan_producto WHERE plan_id = @PlanId", _connection);
                commandEliminar.Transaction = transaction;
                commandEliminar.Parameters.AddWithValue("@PlanId", plan.Id);

                await commandEliminar.ExecuteNonQueryAsync();

                foreach (var product in plan.Products)
                {
                    using var commandRelacion = new MySqlCommand("INSERT INTO plan_producto (plan_id, producto_id) VALUES (@PlanId, @ProductoId)", _connection);
                    commandRelacion.Transaction = transaction;

                    commandRelacion.Parameters.AddWithValue("@PlanId", plan.Id);
                    commandRelacion.Parameters.AddWithValue("@ProductoId", product.Id);

                    await commandRelacion.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return true;
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
                using var commandRelaciones = new MySqlCommand("DELETE FROM plan_producto WHERE plan_id = @Id", _connection);
                commandRelaciones.Transaction = transaction;
                commandRelaciones.Parameters.AddWithValue("@Id", id);
                await commandRelaciones.ExecuteNonQueryAsync();

                using var commandPlan = new MySqlCommand("DELETE FROM plan WHERE id = @Id", _connection);
                commandPlan.Transaction = transaction;
                commandPlan.Parameters.AddWithValue("@Id", id);
                bool result = await commandPlan.ExecuteNonQueryAsync() > 0;

                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetProductosByPlanAsync(int planId)
        {
            List<Product> products = new List<Product>();

            using var command = new MySqlCommand(
                "SELECT p.* FROM producto p JOIN plan_producto pp ON p.id = pp.producto_id WHERE pp.plan_id = @PlanId",
                _connection);
            command.Parameters.AddWithValue("@PlanId", planId);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(new Product
                {
                    Id = reader["id"].ToString()!,
                    Name = reader["nombre"].ToString()!,
                    Stock = Convert.ToInt32(reader["stock"]),
                    StockMin = Convert.ToInt32(reader["stockMin"]),
                    StockMax = Convert.ToInt32(reader["stockMax"]),
                    CreateDate = Convert.ToDateTime(reader["fecha_creacion"]),
                    UpdateDate = Convert.ToDateTime(reader["fecha_actualizacion"]),
                    BarCode = reader["codigo_barra"].ToString()!
                });
            }

            reader.Close();
            return products;
        }

        public async Task<IEnumerable<PromotionalPlan>> GetPlanesVigentesAsync()
        {
            List<PromotionalPlan> planes = new List<PromotionalPlan>();
            DateTime hoy = DateTime.Today;

            using var command = new MySqlCommand("SELECT * FROM plan WHERE @Hoy BETWEEN fecha_inicio AND fecha_fin", _connection);
            command.Parameters.AddWithValue("@Hoy", hoy);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var plan = new PromotionalPlan
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Name = reader["nombre"].ToString()!,
                    StartDate = Convert.ToDateTime(reader["fecha_inicio"]),
                    EndDate = Convert.ToDateTime(reader["fecha_fin"]),
                    Discount = Convert.ToDecimal(reader["descuento"])
                };

                planes.Add(plan);
            }

            reader.Close();

            foreach (var plan in planes)
            {
                plan.Products = (await GetProductosByPlanAsync(plan.Id)).ToList();
            }

            return planes;
        }
    }
}
