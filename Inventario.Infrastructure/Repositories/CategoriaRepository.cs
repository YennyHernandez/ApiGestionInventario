using Inventario.Domain.Entities;
using Inventario.Domain.Entities.CustomEntities;
using Inventario.Domain.Models.Dto;
using Inventario.Infrastructure.Context;
using Inventario.Infrastructure.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Infrastructure.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly ContextInventory _context;

        public CategoriaRepository(ContextInventory context)
        {
            _context = context;
        }

        public async Task<List<CategoriaDto>> GetPagedAsync(int page, int pageSize)
        {
            var parameters = new[]
            {
                new SqlParameter("@Page", page),
                new SqlParameter("@PageSize", pageSize)
            };

            return await _context.Database
                .SqlQueryRaw<CategoriaDto>("EXEC SP_Categorias_GetPaged @Page, @PageSize", parameters)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            var results = await _context.Database
                .SqlQueryRaw<ScalarResultInt>("EXEC SP_Categorias_GetTotalCount")
                .ToListAsync();

            return results.FirstOrDefault()?.Value ?? 0;
        }

        public async Task<Categoria?> GetByIdAsync(int id)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", id)
            };

            var categorias = await _context.Categorias
                .FromSqlRaw("EXEC SP_Categorias_GetById @Id", parameters)
                .ToListAsync();

            var categoria = categorias.FirstOrDefault();
            if (categoria != null)
            {
                await _context.Entry(categoria).Collection(c => c.Productos).LoadAsync();
            }
            return categoria;
        }

        public async Task<List<CategoriaDto>> GetAllActiveAsync()
        {
            return await _context.Database
                .SqlQueryRaw<CategoriaDto>("EXEC SP_Categorias_GetAllActive")
                .ToListAsync();
        }

        public async Task<Categoria> CreateAsync(Categoria categoria)
        {
            var parameters = new[]
            {
                new SqlParameter("@Nombre", categoria.Nombre),
                new SqlParameter("@Descripcion", (object?)categoria.Descripcion ?? DBNull.Value),
                new SqlParameter("@Activo", categoria.Activo)
            };

            var resultados = await _context.Categorias
                .FromSqlRaw("EXEC SP_Categorias_Create @Nombre, @Descripcion, @Activo", parameters)
                .ToListAsync();

            return resultados.First();
        }

        public async Task UpdateAsync(Categoria categoria)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", categoria.Id),
                new SqlParameter("@Nombre", categoria.Nombre),
                new SqlParameter("@Descripcion", (object?)categoria.Descripcion ?? DBNull.Value),
                new SqlParameter("@Activo", categoria.Activo)
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC SP_Categorias_Update @Id, @Nombre, @Descripcion, @Activo",
                parameters);
        }

        public async Task DeleteAsync(int id)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", id)
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC SP_Categorias_Delete @Id", parameters);
        }

        public async Task<bool> ExistsByNameAsync(string nombre)
        {
            var parameters = new[]
            {
                new SqlParameter("@Nombre", nombre)
            };

            var results = await _context.Database
                .SqlQueryRaw<ScalarResultInt>("EXEC SP_Categorias_ExistsByName @Nombre", parameters)
                .ToListAsync();

            return results.FirstOrDefault()?.Value == 1;
        }

        public async Task<bool> ExistsByNameExcludingAsync(string nombre, int excludeId)
        {
            var parameters = new[]
            {
                new SqlParameter("@Nombre", nombre),
                new SqlParameter("@ExcludeId", excludeId)
            };

            var results = await _context.Database
                .SqlQueryRaw<ScalarResultInt>("EXEC SP_Categorias_ExistsByNameExcluding @Nombre, @ExcludeId", parameters)
                .ToListAsync();

            return results.FirstOrDefault()?.Value == 1;
        }

        public async Task<List<CategoriaDto>> GetWithProductCountAsync()
        {
            return await _context.Database
                .SqlQueryRaw<CategoriaDto>("EXEC SP_Categorias_GetWithProductCount")
                .ToListAsync();
        }

        public async Task<bool> HasActiveProductsAsync(int categoriaId)
        {
            var parameters = new[]
            {
                new SqlParameter("@CategoriaId", categoriaId)
            };

            var results = await _context.Database
                .SqlQueryRaw<ScalarResultInt>("EXEC SP_Categorias_HasActiveProducts @CategoriaId", parameters)
                .ToListAsync();

            return results.FirstOrDefault()?.Value == 1;
        }
    }
}
