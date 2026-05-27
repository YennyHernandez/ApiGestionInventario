using Inventario.Domain.Entities;
using Inventario.Domain.Models.Dto;

namespace Inventario.Infrastructure.Repositories.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<List<CategoriaDto>> GetPagedAsync(int page, int pageSize);
        Task<int> GetTotalCountAsync();
        Task<Categoria?> GetByIdAsync(int id);
        Task<List<CategoriaDto>> GetAllActiveAsync();
        Task<Categoria> CreateAsync(Categoria categoria);
        Task UpdateAsync(Categoria categoria);
        Task DeleteAsync(int id);
        Task<bool> ExistsByNameAsync(string nombre);
        Task<bool> ExistsByNameExcludingAsync(string nombre, int excludeId);
        Task<List<CategoriaDto>> GetWithProductCountAsync();
        Task<bool> HasActiveProductsAsync(int categoriaId);
    }
}
