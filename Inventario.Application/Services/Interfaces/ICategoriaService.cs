using Inventario.Domain.Models.Dto;

namespace Inventario.Application.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<ResultOperation<CategoriaDto>> CreateAsync(CategoriaCreateDto dto);
        Task<ResultOperation<CategoriaDto>> UpdateAsync(CategoriaUpdateDto dto);
        Task<ResultOperation> DeleteAsync(int id);
        Task<ResultOperation<CategoriaDto>> GetByIdAsync(int id);
        Task<ResultOperation<List<CategoriaDto>>> GetPagedAsync(int page, int pageSize);
        Task<ResultOperation<List<CategoriaDto>>> GetAllActiveAsync();
    }
}
