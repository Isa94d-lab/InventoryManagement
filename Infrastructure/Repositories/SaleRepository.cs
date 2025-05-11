using System;
using System.Data;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Ports;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Infrastructure.Repositories
{
    public class SaleRepository : IGenericRepository<Sale>, ISaleRepository
    {
        private readonly MySqlConnection _connection;
        private readonly ProductRepository _productRepository;

        public SaleRepository(MySqlConnection connection)
        {
            _connection = connection;
            _productRepository = new ProductRepository(connection); // O inyectado si prefieres
        }


        public async Task<IEnumerable<Sale>> GetAllAsync()
        {
            List<Sale> sales = new();

            using var command = new MySqlCommand(
                "SELECT v.*, " +
                "tc.nombre AS cliente_nombre, tc.apellidos AS cliente_apellidos, " +
                "te.nombre AS empleado_nombre, te.apellidos AS empleado_apellidos " +
                "FROM venta v " +
                "JOIN tercero tc ON v.terceroCliente_id = tc.id " +
                "JOIN tercero te ON v.terceroEmpleado_id = te.id " +
                "ORDER BY v.fecha DESC",
                _connection);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var sale = new Sale
                {
                    InvoiceId = Convert.ToInt32(reader["factura_id"]),
                    Date = Convert.ToDateTime(reader["fecha"]),
                    EmployeePersonId = reader["terceroEmpleado_id"].ToString()!,
                    CustomerPersonId = reader["terceroCliente_id"].ToString()!,
                    Customer = new Person
                    {
                        Id = reader["terceroCliente_id"].ToString()!,
                        Name = reader["cliente_nombre"].ToString()!,
                        LastName = reader["cliente_apellidos"].ToString()!
                    },
                    Employee = new Person
                    {
                        Id = reader["terceroEmpleado_id"].ToString()!,
                        Name = reader["empleado_nombre"].ToString()!,
                        LastName = reader["empleado_apellidos"].ToString()!
                    }
                };

                sales.Add(sale);
            }

            reader.Close(); // Asegura que puedes ejecutar nuevos comandos

            foreach (var sale in sales)
            {
                sale.Details = (await GetDetallesVentaAsync(sale.InvoiceId)).ToList();
            }

            return sales;
        }

        public async Task<Sale?> GetByIdAsync(object id)
        {
            Sale? sale = null;

            using var command = new MySqlCommand(
                "SELECT v.*, " +
                "tc.nombre AS cliente_nombre, tc.apellidos AS cliente_apellidos, " +
                "te.nombre AS empleado_nombre, te.apellidos AS empleado_apellidos " +
                "FROM venta v " +
                "JOIN tercero tc ON v.terceroCliente_id = tc.id " +
                "JOIN tercero te ON v.terceroEmpleado_id = te.id " +
                "WHERE v.factura_id = @FacturaId",
                _connection);

            command.Parameters.AddWithValue("@FacturaId", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                sale = new Sale
                {
                    InvoiceId = Convert.ToInt32(reader["factura_id"]),
                    Date = Convert.ToDateTime(reader["fecha"]),
                    EmployeePersonId = reader["terceroEmpleado_id"].ToString()!,
                    CustomerPersonId = reader["terceroCliente_id"].ToString()!,
                    Customer = new Person
                    {
                        Id = reader["terceroCliente_id"].ToString()!,
                        Name = reader["cliente_nombre"].ToString()!,
                        LastName = reader["cliente_apellidos"].ToString()!
                    },
                    Employee = new Person
                    {
                        Id = reader["terceroEmpleado_id"].ToString()!,
                        Name = reader["empleado_nombre"].ToString()!,
                        LastName = reader["empleado_apellidos"].ToString()!
                    }
                };
            }

            reader.Close();

            if (sale != null)
            {
                sale.Details = (await GetDetallesVentaAsync(sale.InvoiceId)).ToList();
            }

            return sale;
        }

        public async Task<bool> InsertAsync(Sale sale)
        {
            using var transaction = await _connection.BeginTransactionAsync();

            try
            {
                int invoiceId = await GetNextFacturaIdAsync();
                sale.InvoiceId = invoiceId;

                using var commandVenta = new MySqlCommand(
                    "INSERT INTO venta (factura_id, fecha, terceroEmpleado_id, terceroCliente_id) " +
                    "VALUES (@FacturaId, @Fecha, @TerceroEmpleadoId, @TerceroClienteId)",
                    _connection, (MySqlTransaction)transaction);

                commandVenta.Parameters.AddWithValue("@FacturaId", invoiceId);
                commandVenta.Parameters.AddWithValue("@Fecha", sale.Date);
                commandVenta.Parameters.AddWithValue("@TerceroEmpleadoId", sale.EmployeePersonId);
                commandVenta.Parameters.AddWithValue("@TerceroClienteId", sale.CustomerPersonId);

                await commandVenta.ExecuteNonQueryAsync();

                foreach (var detail in sale.Details)
                {
                    using var commandDetalle = new MySqlCommand(
                    "INSERT INTO detalle_venta (factura_id, producto_id, cantidad, valor) " +
                    "VALUES (@FacturaId, @ProductoId, @Cantidad, @Valor)",
                    _connection, (MySqlTransaction)transaction);  // Asegúrate de que la transacción es MySqlTransaction

                commandDetalle.Parameters.AddWithValue("@FacturaId", invoiceId);
                commandDetalle.Parameters.AddWithValue("@ProductoId", detail.ProductId);
                commandDetalle.Parameters.AddWithValue("@Cantidad", detail.Quantity);
                commandDetalle.Parameters.AddWithValue("@Valor", detail.Cost);

                await commandDetalle.ExecuteNonQueryAsync();

                // Convierte la transacción de IDbTransaction a MySqlTransaction
                var mysqlTransaction = (MySqlTransaction)transaction;

                // Pásala correctamente a ActualizarStockAsync
                await _productRepository.ActualizarStockAsync(detail.ProductId, -detail.Quantity, _connection);

                }

                await UpdateFacturacionAsync(invoiceId, transaction);
                await UpdateClienteFechaCompraAsync(sale.CustomerPersonId, sale.Date, transaction);

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

                public async Task<bool> UpdateAsync(Sale sale)
        {
            if (sale == null)
                throw new ArgumentNullException(nameof(sale));

            if (string.IsNullOrEmpty(sale.InvoiceId.ToString()))
                throw new ArgumentException("Invoice ID cannot be empty", nameof(sale.InvoiceId));

            if (string.IsNullOrEmpty(sale.EmployeePersonId))
                throw new ArgumentException("Employee ID cannot be empty", nameof(sale.EmployeePersonId));

            if (string.IsNullOrEmpty(sale.CustomerPersonId))
                throw new ArgumentException("Customer ID cannot be empty", nameof(sale.CustomerPersonId));

            if (sale.Date == default)
                throw new ArgumentException("Sale date cannot be empty", nameof(sale.Date));

            using var transaction = await _connection.BeginTransactionAsync();
            try
            {
                // Update the sale header
                using var commandSale = new MySqlCommand(
                    @"UPDATE venta 
                      SET fecha = @Fecha, 
                          terceroEmpleado_id = @TerceroEmpleadoId, 
                          terceroCliente_id = @TerceroClienteId
                      WHERE factura_id = @Id",
                    _connection,
                    transaction);

                commandSale.Parameters.AddWithValue("@Id", sale.InvoiceId);
                commandSale.Parameters.AddWithValue("@Fecha", sale.Date);
                commandSale.Parameters.AddWithValue("@TerceroEmpleadoId", sale.EmployeePersonId);
                commandSale.Parameters.AddWithValue("@TerceroClienteId", sale.CustomerPersonId);

                var result = await commandSale.ExecuteNonQueryAsync() > 0;

                if (!result)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                // Delete existing details
                using var commandDeleteDetails = new MySqlCommand(
                    "DELETE FROM detalle_venta WHERE factura_id = @Id",
                    _connection,
                    transaction);

                commandDeleteDetails.Parameters.AddWithValue("@Id", sale.InvoiceId);
                await commandDeleteDetails.ExecuteNonQueryAsync();

                // Insert new details if they exist
                if (sale.Details != null && sale.Details.Any())
                {
                    foreach (var detail in sale.Details)
                    {
                        using var commandDetail = new MySqlCommand(
                            @"INSERT INTO detalle_venta 
                              (factura_id, producto_id, cantidad, valor) 
                              VALUES (@FacturaId, @ProductoId, @Cantidad, @Valor)",
                            _connection,
                            transaction);

                        commandDetail.Parameters.AddWithValue("@FacturaId", sale.InvoiceId);
                        commandDetail.Parameters.AddWithValue("@ProductoId", detail.ProductId);
                        commandDetail.Parameters.AddWithValue("@Cantidad", detail.Quantity);
                        commandDetail.Parameters.AddWithValue("@Valor", detail.Cost);

                        await commandDetail.ExecuteNonQueryAsync();

                        // Update product stock
                        await _productRepository.ActualizarStockAsync(detail.ProductId, -detail.Quantity, _connection);
                    }
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<bool> DeleteAsync(object id)
        {
            using var command = new MySqlCommand(
                "DELETE FROM venta WHERE factura_id = @Id",
                _connection);

            command.Parameters.AddWithValue("@Id", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        private async Task<IEnumerable<SaleDetail>> GetDetallesVentaAsync(int invoiceId)
        {
            List<SaleDetail> details = new();

            using var command = new MySqlCommand(
                "SELECT dv.*, p.nombre AS producto_nombre " +
                "FROM detalle_venta dv " +
                "JOIN producto p ON dv.producto_id = p.id " +
                "WHERE dv.factura_id = @FacturaId",
                _connection);

            command.Parameters.AddWithValue("@FacturaId", invoiceId);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                details.Add(new SaleDetail
                {
                    Id = Convert.ToInt32(reader["id"]),
                    InvoiceId = invoiceId,
                    ProductId = reader["producto_id"].ToString()!,
                    Quantity = Convert.ToInt32(reader["cantidad"]),
                    Cost = Convert.ToDecimal(reader["valor"]),
                    Product = new Product
                    {
                        Id = reader["producto_id"].ToString()!,
                        Name = reader["producto_nombre"].ToString()!
                    }
                });
            }

            return details;
        }

        private async Task<int> GetNextFacturaIdAsync()
        {
            using var command = new MySqlCommand(
                "SELECT factura_actual, numFinal FROM facturacion " +
                "WHERE factura_actual < numFinal " +
                "ORDER BY fechaResolucion DESC LIMIT 1", 
                _connection);
            
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                int currentId = Convert.ToInt32(reader["factura_actual"]);
                int maxId = Convert.ToInt32(reader["numFinal"]);
                
                if (currentId < maxId)
                {
                    return currentId + 1;
                }
            }
            
            // Si no hay rangos disponibles o se alcanzó el máximo, crear nuevo rango
            using var createCommand = new MySqlCommand(
                "INSERT INTO facturacion (fechaResolucion, numInicio, numFinal, factura_actual) " +
                "VALUES (NOW(), @NumInicio, @NumFinal, @NumInicio)", 
                _connection);
            
            // Obtener el último número final usado
            using var lastCommand = new MySqlCommand(
                "SELECT MAX(numFinal) as last_max FROM facturacion", 
                _connection);
            var lastMax = await lastCommand.ExecuteScalarAsync();
            int nextStart = lastMax != null && lastMax != DBNull.Value ? 
                Convert.ToInt32(lastMax) + 1 : 1000;
            
            createCommand.Parameters.AddWithValue("@NumInicio", nextStart);
            createCommand.Parameters.AddWithValue("@NumFinal", nextStart + 999);
            
            await createCommand.ExecuteNonQueryAsync();
            return nextStart;
        }

        private async Task UpdateFacturacionAsync(int invoiceId, IDbTransaction transaction)
        {
            using var command = new MySqlCommand(
                "UPDATE facturacion SET factura_actual = @FacturaId",
                _connection, (MySqlTransaction)transaction);

            command.Parameters.AddWithValue("@FacturaId", invoiceId);
            await command.ExecuteNonQueryAsync();
        }

        private async Task UpdateClienteFechaCompraAsync(string personId, DateTime fechaCompra, IDbTransaction transaction)
        {
            using var findCommand = new MySqlCommand(
                "SELECT id FROM cliente WHERE tercero_id = @TerceroId",
                _connection, (MySqlTransaction)transaction);

            findCommand.Parameters.AddWithValue("@TerceroId", personId);
            var customerId = await findCommand.ExecuteScalarAsync();

            if (customerId != null && customerId != DBNull.Value)
            {
                using var updateCommand = new MySqlCommand(
                    "UPDATE cliente SET fecha_compra = @FechaCompra WHERE id = @ClienteId",
                    _connection, (MySqlTransaction)transaction);

                updateCommand.Parameters.AddWithValue("@ClienteId", customerId);
                updateCommand.Parameters.AddWithValue("@FechaCompra", fechaCompra);
                await updateCommand.ExecuteNonQueryAsync();
            }
        }
    }
}
