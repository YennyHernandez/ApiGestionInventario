using Inventario.Domain.Entities;
using Inventario.Domain.Entities.CustomEntities;
using Inventario.Domain.Models.Dto;
using Inventario.Infrastructure.Context;
using Inventario.Infrastructure.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Infrastructure.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly ContextInventory _context;

        public ProductoRepository(ContextInventory context)
        {
            _context = context;
        }

        public async Task<List<ProductoDto>> GetPagedAsync(int page, int pageSize)
        {
            var parameters = new[]
            {
                new SqlParameter("@Page", page),
                new SqlParameter("@PageSize", pageSize)
            };

            return await _context.Database
                .SqlQueryRaw<ProductoDto>("EXEC SP_Productos_GetPaged @Page, @PageSize", parameters)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            var results = await _context.Database
                .SqlQueryRaw<ScalarResultInt>("EXEC SP_Productos_GetTotalCount")
                .ToListAsync();

            return results.FirstOrDefault()?.Value ?? 0;
        }

        public async Task<Producto?> GetByIdAsync(int id)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", id)
            };

            var productos = await _context.Productos
                .FromSqlRaw("EXEC SP_Productos_GetById @Id", parameters)
                .ToListAsync();

            var producto = productos.FirstOrDefault();
            if (producto != null)
            {
                await _context.Entry(producto).Reference(p => p.Categoria).LoadAsync();
            }
            return producto;
        }

        public async Task<Producto?> GetBySkuAsync(string sku)
        {
            var parameters = new[]
            {
                new SqlParameter("@Sku", sku)
            };

            var productos = await _context.Productos
                .FromSqlRaw("EXEC SP_Productos_GetBySku @Sku", parameters)
                .ToListAsync();

            return productos.FirstOrDefault();
        }

        public async Task<List<ProductoDto>> GetAllAsync()
        {
            return await _context.Database
                .SqlQueryRaw<ProductoDto>("EXEC SP_Productos_GetAll")
                .ToListAsync();
        }

        public async Task<Producto> CreateAsync(Producto producto)
        {
            var parameters = new[]
            {
                new SqlParameter("@Sku", producto.Sku),
                new SqlParameter("@Nombre", producto.Nombre),
                new SqlParameter("@Descripcion", (object?)producto.Descripcion ?? DBNull.Value),
                new SqlParameter("@Precio", producto.Precio),
                new SqlParameter("@Stock", producto.Stock),
                new SqlParameter("@StockMinimo", producto.StockMinimo),
                new SqlParameter("@Activo", producto.Activo),
                new SqlParameter("@CategoriaId", producto.CategoriaId)
            };

            var resultados = await _context.Productos
                .FromSqlRaw("EXEC SP_Productos_Create @Sku, @Nombre, @Descripcion, @Precio, @Stock, @StockMinimo, @Activo, @CategoriaId", parameters)
                .ToListAsync();

            return resultados.First();
        }

        public async Task UpdateAsync(Producto producto)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", producto.Id),
                new SqlParameter("@Sku", producto.Sku),
                new SqlParameter("@Nombre", producto.Nombre),
                new SqlParameter("@Descripcion", (object?)producto.Descripcion ?? DBNull.Value),
                new SqlParameter("@Precio", producto.Precio),
                new SqlParameter("@Stock", producto.Stock),
                new SqlParameter("@StockMinimo", producto.StockMinimo),
                new SqlParameter("@Activo", producto.Activo),
                new SqlParameter("@CategoriaId", producto.CategoriaId)
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC SP_Productos_Update @Id, @Sku, @Nombre, @Descripcion, @Precio, @Stock, @StockMinimo, @Activo, @CategoriaId",
                parameters);
        }

        public async Task DeleteAsync(int id)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", id)
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC SP_Productos_Delete @Id", parameters);
        }

        public async Task<bool> ExistsBySkuAsync(string sku)
        {
            var parameters = new[]
            {
                new SqlParameter("@Sku", sku)
            };

            var results = await _context.Database
                .SqlQueryRaw<ScalarResultInt>("EXEC SP_Productos_ExistsBySku @Sku", parameters)
                .ToListAsync();

            return results.FirstOrDefault()?.Value == 1;
        }

        public async Task<bool> ExistsBySkuExcludingAsync(string sku, int excludeId)
        {
            var parameters = new[]
            {
                new SqlParameter("@Sku", sku),
                new SqlParameter("@ExcludeId", excludeId)
            };

            var results = await _context.Database
                .SqlQueryRaw<ScalarResultInt>("EXEC SP_Productos_ExistsBySkuExcluding @Sku, @ExcludeId", parameters)
                .ToListAsync();

            return results.FirstOrDefault()?.Value == 1;
        }

        public async Task<decimal> GetTotalInventoryValueAsync()
        {
            var results = await _context.Database
                .SqlQueryRaw<ScalarResult>("EXEC SP_Productos_GetTotalInventoryValue")
                .ToListAsync();

            return results.FirstOrDefault()?.Value ?? 0;
        }

        public async Task<List<ProductoStockCriticoDto>> GetCriticalStockProductsAsync()
        {
            return await _context.Database
                .SqlQueryRaw<ProductoStockCriticoDto>("EXEC SP_Productos_GetCriticalStock")
                .ToListAsync();
        }

        public async Task<List<CategoriaResumenDto>> GetProductsByCategoryAsync()
        {
            return await _context.Database
                .SqlQueryRaw<CategoriaResumenDto>("EXEC SP_Productos_GetByCategory")
                .ToListAsync();
        }

        public async Task<int> GetTotalStockAsync()
        {
            var results = await _context.Database
                .SqlQueryRaw<ScalarResultInt>("EXEC SP_Productos_GetTotalStock")
                .ToListAsync();

            return results.FirstOrDefault()?.Value ?? 0;
        }

        public async Task<int> GetTotalCapacityAsync()
        {
            var results = await _context.Database
                .SqlQueryRaw<ScalarResultInt>("EXEC SP_Productos_GetTotalCapacity")
                .ToListAsync();

            return results.FirstOrDefault()?.Value ?? 0;
        }
    }
}
