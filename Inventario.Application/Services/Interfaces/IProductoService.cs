using Inventario.Domain.Models.Dto;

namespace Inventario.Application.Services.Interfaces
{
    public interface IProductoService
    {
        Task<ResultOperation<ProductoDto>> CreateAsync(ProductoCreateDto dto);
        Task<ResultOperation<ProductoDto>> UpdateAsync(ProductoUpdateDto dto);
        Task<ResultOperation> DeleteAsync(int id);
        Task<ResultOperation<ProductoDto>> GetByIdAsync(int id);
        Task<ResultOperation<List<ProductoDto>>> GetPagedAsync(int page, int pageSize);
        Task<ResultOperation<List<ProductoDto>>> GetAllAsync();
        Task<ResultOperation<ProductoDto>> GetBySkuAsync(string sku);
    }
}
