using System;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Ports;
using InventoryManagement.Infrastructure.Mysql;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Infrastructure.Repositories;

public class PurchaseRepository : IGenericRepository<Purchase>, IPurchaseRepository
{
    private readonly MySqlConnection _connection;

    public PurchaseRepository(MySqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Purchase>> GetAllAsync()
    {
        var purchases = new List<Purchase>();

        using var command = new MySqlCommand(
            "SELECT c.*, " +
            "tp.nombre as proveedor_nombre, tp.apellidos as proveedor_apellidos, " +
            "te.nombre as empleado_nombre, te.apellidos as empleado_apellidos " +
            "FROM compra c " +
            "JOIN tercero tp ON c.terceroProveedor_id = tp.id " +
            "JOIN tercero te ON c.terceroEmpleado_id = te.id " +
            "ORDER BY c.fecha DESC",
            _connection);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var purchase = new Purchase
            {
                Id = Convert.ToInt32(reader["id"]),
                SupplierPersonId = reader["terceroProveedor_id"].ToString()!,
                Date = Convert.ToDateTime(reader["fecha"]),
                EmployeePersonId = reader["terceroEmpleado_id"].ToString()!,
                PurOrder = reader["DocCompra"].ToString()!,
                Supplier = new Person
                {
                    Id = reader["terceroProveedor_id"].ToString()!,
                    Name = reader["proveedor_nombre"].ToString()!,
                    LastName = reader["proveedor_apellidos"].ToString()!
                },
                Employee = new Person
                {
                    Id = reader["terceroEmpleado_id"].ToString()!,
                    Name = reader["empleado_nombre"].ToString()!,
                    LastName = reader["empleado_apellidos"].ToString()!
                }
            };

            purchases.Add(purchase);
        }

        reader.Close();

        foreach (var purchase in purchases)
        {
            purchase.Details = (await GetDetallesCompraAsync(purchase.Id)).ToList();
        }

        return purchases;
    }

    public async Task<Purchase?> GetByIdAsync(object id)
    {
        Purchase? purchase = null;

        using var command = new MySqlCommand(
            "SELECT c.*, " +
            "tp.nombre as proveedor_nombre, tp.apellidos as proveedor_apellidos, " +
            "te.nombre as empleado_nombre, te.apellidos as empleado_apellidos " +
            "FROM compra c " +
            "JOIN tercero tp ON c.terceroProveedor_id = tp.id " +
            "JOIN tercero te ON c.terceroEmpleado_id = te.id " +
            "WHERE c.id = @Id",
            _connection);

        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            purchase = new Purchase
            {
                Id = Convert.ToInt32(reader["id"]),
                SupplierPersonId = reader["terceroProveedor_id"].ToString()!,
                Date = Convert.ToDateTime(reader["fecha"]),
                EmployeePersonId = reader["terceroEmpleado_id"].ToString()!,
                PurOrder = reader["DocCompra"].ToString()!,
                Supplier = new Person
                {
                    Id = reader["terceroProveedor_id"].ToString()!,
                    Name = reader["proveedor_nombre"].ToString()!,
                    LastName = reader["proveedor_apellidos"].ToString()!
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

        if (purchase != null)
        {
            purchase.Details = (await GetDetallesCompraAsync(purchase.Id)).ToList();
        }

        return purchase;
    }

    public async Task<bool> InsertAsync(Purchase purchase)
    {
        using var transaction = await _connection.BeginTransactionAsync();

        try
        {
            using var commandPurchase = new MySqlCommand(
                "INSERT INTO compra (terceroProveedor_id, fecha, terceroEmpleado_id, DocCompra) " +
                "VALUES (@TerceroProveedorId, @Fecha, @TerceroEmpleadoId, @DocCompra); " +
                "SELECT LAST_INSERT_ID();",
                _connection, (MySqlTransaction)transaction);

            commandPurchase.Parameters.AddWithValue("@TerceroProveedorId", purchase.SupplierPersonId);
            commandPurchase.Parameters.AddWithValue("@Fecha", purchase.Date);
            commandPurchase.Parameters.AddWithValue("@TerceroEmpleadoId", purchase.EmployeePersonId);
            commandPurchase.Parameters.AddWithValue("@DocCompra", purchase.PurOrder);

            var purchaseId = Convert.ToInt32(await commandPurchase.ExecuteScalarAsync());
            purchase.Id = purchaseId;

            foreach (var detail in purchase.Details)
            {
                using var commandDetail = new MySqlCommand(
                    "INSERT INTO detalle_compra (fecha, producto_id, cantidad, valor, compra_id) " +
                    "VALUES (@Fecha, @ProductoId, @Cantidad, @Valor, @CompraId)",
                    _connection, (MySqlTransaction)transaction);

                commandDetail.Parameters.AddWithValue("@Fecha", purchase.Date);
                commandDetail.Parameters.AddWithValue("@ProductoId", detail.ProductId);
                commandDetail.Parameters.AddWithValue("@Cantidad", detail.Quantity);
                commandDetail.Parameters.AddWithValue("@Valor", detail.Cost);
                commandDetail.Parameters.AddWithValue("@CompraId", purchaseId);

                await commandDetail.ExecuteNonQueryAsync();

                // Aquí deberías tener implementado un método para actualizar el stock
                // await ActualizarStockAsync(detail.ProductId, detail.Quantity, _connection);
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

    public Task<bool> UpdateAsync(Purchase purchase)
    {
        return Task.FromResult(false);
    }

    public Task<bool> DeleteAsync(object id)
    {
        return Task.FromResult(false);
    }

    public async Task<IEnumerable<PurchaseDetail>> GetDetallesCompraAsync(int purchaseId)
    {
        var details = new List<PurchaseDetail>();

        using var command = new MySqlCommand(
            "SELECT dc.*, p.nombre as producto_nombre " +
            "FROM detalle_compra dc " +
            "JOIN producto p ON dc.producto_id = p.id " +
            "WHERE dc.compra_id = @CompraId",
            _connection);

        command.Parameters.AddWithValue("@CompraId", purchaseId);

        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            details.Add(new PurchaseDetail
            {
                Id = Convert.ToInt32(reader["id"]),
                Date = Convert.ToDateTime(reader["fecha"]),
                ProductId = reader["producto_id"].ToString()!,
                Quantity = Convert.ToInt32(reader["cantidad"]),
                Cost = Convert.ToDecimal(reader["valor"]),
                PurchaseId = purchaseId,
                Product = new Product
                {
                    Id = reader["producto_id"].ToString()!,
                    Name = reader["producto_nombre"].ToString()!
                }
            });
        }

        return details;
    }
}
