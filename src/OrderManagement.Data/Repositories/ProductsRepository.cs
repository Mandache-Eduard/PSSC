using Microsoft.EntityFrameworkCore;
using OrderManagement.Data.Models;
using OrderManagement.Domain.Models;
using OrderManagement.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderManagement.Data.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly OrderManagementContext _context;

        public ProductsRepository(OrderManagementContext context)
        {
            _context = context;
        }

        public async Task<(bool exists, string productName, decimal price)> GetProductByCodeAsync(ProductCode productCode)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == productCode.Value && p.IsActive);

            if (product == null)
            {
                return (false, string.Empty, 0m);
            }

            return (true, product.Name, product.Price);
        }

        public async Task<bool> CheckStockAsync(ProductCode productCode, Quantity quantity)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == productCode.Value && p.IsActive);

            if (product == null)
            {
                return false;
            }

            return product.Stock >= (int)quantity.Value;
        }

        public async Task<IEnumerable<(ProductCode code, string name, decimal price)>> GetProductsByCodesAsync(IEnumerable<ProductCode> productCodes)
        {
            var codes = productCodes.Select(pc => pc.Value).ToList();
            
            var products = await _context.Products
                .AsNoTracking()
                .Where(p => codes.Contains(p.Code) && p.IsActive)
                .ToListAsync();

            return products.Select(p => (
                ProductCode.TryParse(p.Code, out var code) ? code! : null!,
                p.Name,
                p.Price
            )).Where(p => p.Item1 != null);
        }
    }
}

