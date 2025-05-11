using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Factory;
using InventoryManagement.Domain.Ports;
using InventoryManagement.Infrastructure.Repositories;
using MySql.Data.MySqlClient;

namespace InventoryManagement.Infrastructure.Mysql;

public class MySqlDbFactory : IDbFactory
{
    private readonly string _connectionString;

    public MySqlDbFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IPurchaseRepository CreatePurchaseRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new PurchaseRepository(connection);
    }

    public IProductRepository CreateProductRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new ProductRepository(connection);
    }

    public IPromotionalPlanRepository CreatePromotionalPlanRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new PlanRepository(connection);
    }

    public ICashFlowRepository CreateCashFlowRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new CashFlowRepository(connection);
    }

    public ISaleRepository CreateSaleRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new SaleRepository(connection);
    }

    public IEmployeeRepository CreateEmployeeRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new EmployeeRepository(connection);
    }

    public ISupplierRepository CreateSupplierRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new SupplierRepository(connection);
    }

    public ICustomerRepository CreateCustomerRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new CustomerRepository(connection);
    }

    public ICountryRepository CreateCountryRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new CountryRepository(connection);
    }

    public IRegionRepository CreateRegionRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new RegionRepository(connection);
    }

    public ICityRepository CreateCityRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new CityRepository(connection);
    }

    public IPersonRepository CreatePersonRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new PersonRepository(connection);
    }

    public IPersonTelephoneRepository CreatePersonTelephoneRepository()
    {
        var connection = ConexionSingleton.Instancia(_connectionString).ObtenerConexion();
        return new PersonTelephoneRepository(connection);
    }


}
