using Inventario.Application.Services.Interfaces;
using Inventario.Domain.Entities;
using Inventario.Domain.Models.Dto;
using Inventario.Infrastructure.Repositories._UnitOfWork;

namespace Inventario.Application.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IUnitOfWorkInventory _unitOfWork;

        public ProductoService(IUnitOfWorkInventory unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultOperation<ProductoDto>> CreateAsync(ProductoCreateDto dto)
        {
            var result = new ResultOperation<ProductoDto>();

            try
            {
                var skuExists = await _unitOfWork.ProductoRepository.ExistsBySkuAsync(dto.Sku);
                if (skuExists)
                {
                    result.stateOperation = false;
                    result.MessageResult = $"Ya existe un producto con el SKU '{dto.Sku}'.";
                    return result;
                }

                var categoriaExists = await _unitOfWork.CategoriaRepository.GetByIdAsync(dto.CategoriaId);
                if (categoriaExists == null || !categoriaExists.Activo)
                {
                    result.stateOperation = false;
                    result.MessageResult = "La categoria seleccionada no existe o esta inactiva.";
                    return result;
                }

                if (dto.Stock < 0)
                {
                    result.stateOperation = false;
                    result.MessageResult = "El stock no puede ser negativo.";
                    return result;
                }

                var producto = new Producto
                {
                    Sku = dto.Sku,
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    Precio = dto.Precio,
                    Stock = dto.Stock,
                    StockMinimo = dto.StockMinimo,
                    CategoriaId = dto.CategoriaId,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };

                await _unitOfWork.ProductoRepository.CreateAsync(producto);

                result.stateOperation = true;
                result.MessageResult = "Producto creado exitosamente.";
                result.Result = MapToDto(producto, categoriaExists.Nombre);

                return result;
            }
            catch (Exception ex)
            {
                result.stateOperation = false;
                result.MessageResult = "Error al crear el producto.";
                result.MessageExceptionTechnical = ex.Message;
                return result;
            }
        }

        public async Task<ResultOperation<ProductoDto>> UpdateAsync(ProductoUpdateDto dto)
        {
            var result = new ResultOperation<ProductoDto>();

            try
            {
                var producto = await _unitOfWork.ProductoRepository.GetByIdAsync(dto.Id);
                if (producto == null)
                {
                    result.stateOperation = false;
                    result.MessageResult = "Producto no encontrado.";
                    return result;
                }

                if (producto.Sku != dto.Sku)
                {
                    var skuExists = await _unitOfWork.ProductoRepository.ExistsBySkuExcludingAsync(dto.Sku, dto.Id);
                    if (skuExists)
                    {
                        result.stateOperation = false;
                        result.MessageResult = $"Ya existe un producto con el SKU '{dto.Sku}'.";
                        return result;
                    }
                }

                var categoriaExists = await _unitOfWork.CategoriaRepository.GetByIdAsync(dto.CategoriaId);
                if (categoriaExists == null || !categoriaExists.Activo)
                {
                    result.stateOperation = false;
                    result.MessageResult = "La categoria seleccionada no existe o esta inactiva.";
                    return result;
                }

                if (dto.Stock < 0)
                {
                    result.stateOperation = false;
                    result.MessageResult = "El stock no puede ser negativo.";
                    return result;
                }

                producto.Sku = dto.Sku;
                producto.Nombre = dto.Nombre;
                producto.Descripcion = dto.Descripcion;
                producto.Precio = dto.Precio;
                producto.Stock = dto.Stock;
                producto.StockMinimo = dto.StockMinimo;
                producto.CategoriaId = dto.CategoriaId;
                producto.Activo = dto.Activo;

                await _unitOfWork.ProductoRepository.UpdateAsync(producto);

                result.stateOperation = true;
                result.MessageResult = "Producto actualizado exitosamente.";
                result.Result = MapToDto(producto, categoriaExists.Nombre);

                return result;
            }
            catch (Exception ex)
            {
                result.stateOperation = false;
                result.MessageResult = "Error al actualizar el producto.";
                result.MessageExceptionTechnical = ex.Message;
                return result;
            }
        }

        public async Task<ResultOperation> DeleteAsync(int id)
        {
            var result = new ResultOperation();

            try
            {
                var producto = await _unitOfWork.ProductoRepository.GetByIdAsync(id);
                if (producto == null)
                {
                    result.stateOperation = false;
                    result.MessageResult = "Producto no encontrado.";
                    return result;
                }

                await _unitOfWork.ProductoRepository.DeleteAsync(id);

                result.stateOperation = true;
                result.MessageResult = "Producto eliminado exitosamente.";

                return result;
            }
            catch (Exception ex)
            {
                result.stateOperation = false;
                result.MessageResult = "Error al eliminar el producto.";
                result.MessageExceptionTechnical = ex.Message;
                return result;
            }
        }

        public async Task<ResultOperation<ProductoDto>> GetByIdAsync(int id)
        {
            var result = new ResultOperation<ProductoDto>();

            try
            {
                var producto = await _unitOfWork.ProductoRepository.GetByIdAsync(id);
                if (producto == null)
                {
                    result.stateOperation = false;
                    result.MessageResult = "Producto no encontrado.";
                    return result;
                }

                result.stateOperation = true;
                result.Result = MapToDto(producto, producto.Categoria?.Nombre);

                return result;
            }
            catch (Exception ex)
            {
                result.stateOperation = false;
                result.MessageResult = "Error al obtener el producto.";
                result.MessageExceptionTechnical = ex.Message;
                return result;
            }
        }

        public async Task<ResultOperation<List<ProductoDto>>> GetPagedAsync(int page, int pageSize)
        {
            var result = new ResultOperation<List<ProductoDto>>();

            try
            {
                var productos = await _unitOfWork.ProductoRepository.GetPagedAsync(page, pageSize);

                result.stateOperation = true;
                result.Result = productos;

                return result;
            }
            catch (Exception ex)
            {
                result.stateOperation = false;
                result.MessageResult = "Error al obtener los productos.";
                result.MessageExceptionTechnical = ex.Message;
                return result;
            }
        }

        public async Task<ResultOperation<List<ProductoDto>>> GetAllAsync()
        {
            var result = new ResultOperation<List<ProductoDto>>();

            try
            {
                var productos = await _unitOfWork.ProductoRepository.GetAllAsync();

                result.stateOperation = true;
                result.Result = productos;

                return result;
            }
            catch (Exception ex)
            {
                result.stateOperation = false;
                result.MessageResult = "Error al obtener los productos.";
                result.MessageExceptionTechnical = ex.Message;
                return result;
            }
        }

        public async Task<ResultOperation<ProductoDto>> GetBySkuAsync(string sku)
        {
            var result = new ResultOperation<ProductoDto>();

            try
            {
                var producto = await _unitOfWork.ProductoRepository.GetBySkuAsync(sku);
                if (producto == null)
                {
                    result.stateOperation = false;
                    result.MessageResult = "Producto no encontrado.";
                    return result;
                }

                result.stateOperation = true;
                result.Result = MapToDto(producto, producto.Categoria?.Nombre);

                return result;
            }
            catch (Exception ex)
            {
                result.stateOperation = false;
                result.MessageResult = "Error al obtener el producto.";
                result.MessageExceptionTechnical = ex.Message;
                return result;
            }
        }

        private ProductoDto MapToDto(Producto producto, string? categoriaNombre)
        {
            return new ProductoDto
            {
                Id = producto.Id,
                Sku = producto.Sku,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Stock = producto.Stock,
                StockMinimo = producto.StockMinimo,
                Activo = producto.Activo,
                CategoriaId = producto.CategoriaId,
                CategoriaNombre = categoriaNombre,
                FechaCreacion = producto.FechaCreacion,
                FechaActualizacion = producto.FechaActualizacion
            };
        }
    }
}
