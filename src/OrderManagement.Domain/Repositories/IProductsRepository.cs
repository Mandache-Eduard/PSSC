using OrderManagement.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrderManagement.Domain.Repositories
{
    public interface IProductsRepository
    {
        Task<(bool exists, string productName, decimal price)> GetProductByCodeAsync(ProductCode productCode);
        Task<bool> CheckStockAsync(ProductCode productCode, Quantity quantity);
        Task<IEnumerable<(ProductCode code, string name, decimal price)>> GetProductsByCodesAsync(IEnumerable<ProductCode> productCodes);
    }
}

