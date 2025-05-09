using System;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Ports;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Infrastructure.Repositories;

public class ProductRepository : IGenericRepository<Product>, IProductRepository
{
    private readonly MySqlConnection _connection;

    public ProductRepository(MySqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        var products = new List<Product>();

        using var command = new MySqlCommand("SELECT * FROM producto", _connection);
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

        return products;
    }

    public async Task<Product?> GetByIdAsync(object id)
    {
        Product? product = null;

        using var command = new MySqlCommand("SELECT * FROM producto WHERE id = @Id", _connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            product = new Product
            {
                Id = reader["id"].ToString()!,
                Name = reader["nombre"].ToString()!,
                Stock = Convert.ToInt32(reader["stock"]),
                StockMin = Convert.ToInt32(reader["stockMin"]),
                StockMax = Convert.ToInt32(reader["stockMax"]),
                CreateDate = Convert.ToDateTime(reader["fecha_creacion"]),
                UpdateDate = Convert.ToDateTime(reader["fecha_actualizacion"]),
                BarCode = reader["codigo_barra"].ToString()!
            };
        }

        return product;
    }

    public async Task<bool> InsertAsync(Product product)
    {
        using var command = new MySqlCommand(
            "INSERT INTO producto (id, nombre, stock, stockMin, stockMax, fecha_creacion, fecha_actualizacion, codigo_barra) " +
            "VALUES (@Id, @Nombre, @Stock, @StockMin, @StockMax, @FechaCreacion, @FechaActualizacion, @CodigoBarra)",
            _connection);

        command.Parameters.AddWithValue("@Id", product.Id);
        command.Parameters.AddWithValue("@Nombre", product.Name);
        command.Parameters.AddWithValue("@Stock", product.Stock);
        command.Parameters.AddWithValue("@StockMin", product.StockMin);
        command.Parameters.AddWithValue("@StockMax", product.StockMax);
        command.Parameters.AddWithValue("@FechaCreacion", product.CreateDate);
        command.Parameters.AddWithValue("@FechaActualizacion", product.UpdateDate);
        command.Parameters.AddWithValue("@CodigoBarra", product.BarCode);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        using var command = new MySqlCommand(
            "UPDATE producto SET nombre = @Nombre, stock = @Stock, stockMin = @StockMin, " +
            "stockMax = @StockMax, fecha_actualizacion = @FechaActualizacion, codigo_barra = @CodigoBarra " +
            "WHERE id = @Id",
            _connection);

        command.Parameters.AddWithValue("@Id", product.Id);
        command.Parameters.AddWithValue("@Nombre", product.Name);
        command.Parameters.AddWithValue("@Stock", product.Stock);
        command.Parameters.AddWithValue("@StockMin", product.StockMin);
        command.Parameters.AddWithValue("@StockMax", product.StockMax);
        command.Parameters.AddWithValue("@FechaActualizacion", DateTime.Now);
        command.Parameters.AddWithValue("@CodigoBarra", product.BarCode);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(object id)
    {
        using var command = new MySqlCommand("DELETE FROM producto WHERE id = @Id", _connection);
        command.Parameters.AddWithValue("@Id", id);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> ActualizarStockAsync(string productId, int quantity, MySqlConnection connection)
    {
        using var command = new MySqlCommand(
            "UPDATE producto SET stock = stock + @Cantidad, fecha_actualizacion = @FechaActualizacion WHERE id = @Id",
            connection);

        command.Parameters.AddWithValue("@Id", productId);
        command.Parameters.AddWithValue("@Cantidad", quantity);
        command.Parameters.AddWithValue("@FechaActualizacion", DateTime.Now);

        return await command.ExecuteNonQueryAsync() > 0;
    }


    public async Task<IEnumerable<Product>> GetProductosBajoStockAsync()
    {
        var products = new List<Product>();

        using var command = new MySqlCommand("SELECT * FROM producto WHERE stock <= stockMin", _connection);
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

        return products;
    }
}
