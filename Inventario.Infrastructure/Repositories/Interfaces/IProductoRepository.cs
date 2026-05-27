using Inventario.Domain.Entities;
using Inventario.Domain.Models.Dto;

namespace Inventario.Infrastructure.Repositories.Interfaces
{
    public interface IProductoRepository
    {
        Task<List<ProductoDto>> GetPagedAsync(int page, int pageSize);
        Task<int> GetTotalCountAsync();
        Task<Producto?> GetByIdAsync(int id);
        Task<Producto?> GetBySkuAsync(string sku);
        Task<List<ProductoDto>> GetAllAsync();
        Task<Producto> CreateAsync(Producto producto);
        Task UpdateAsync(Producto producto);
        Task DeleteAsync(int id);
        Task<bool> ExistsBySkuAsync(string sku);
        Task<bool> ExistsBySkuExcludingAsync(string sku, int excludeId);
        Task<decimal> GetTotalInventoryValueAsync();
        Task<List<ProductoStockCriticoDto>> GetCriticalStockProductsAsync();
        Task<List<CategoriaResumenDto>> GetProductsByCategoryAsync();
        Task<int> GetTotalStockAsync();
        Task<int> GetTotalCapacityAsync();
    }
}
