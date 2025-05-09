using System;
using InventoryManagement.Domain.Ports;

namespace InventoryManagement.Domain.Factory;

public interface IDbFactory
{
    IPurchaseRepository CreatePurchaseRepository();
    IProductRepository CreateProductRepository();
    IPromotionalPlanRepository CreatePromotionalPlanRepository();
    ICashFlowRepository CreateCashFlowRepository();
    ISaleRepository CreateSaleRepository();

}
