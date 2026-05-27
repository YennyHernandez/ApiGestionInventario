using Inventario.Application.Services.Interfaces;
using Inventario.Domain.Entities;
using Inventario.Domain.Models.Dto;
using Inventario.Infrastructure.Repositories._UnitOfWork;

namespace Inventario.Application.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly IUnitOfWorkInventory _unitOfWork;

        public CategoriaService(IUnitOfWorkInventory unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultOperation<CategoriaDto>> CreateAsync(CategoriaCreateDto dto)
        {
            var result = new ResultOperation<CategoriaDto>();

            try
            {
                var nameExists = await _unitOfWork.CategoriaRepository.ExistsByNameAsync(dto.Nombre);
                if (nameExists)
                {
                    result.stateOperation = false;
                    result.MessageResult = $"Ya existe una categoria con el nombre '{dto.Nombre}'.";
                    return result;
                }

                var categoria = new Categoria
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };

                await _unitOfWork.CategoriaRepository.CreateAsync(categoria);

                result.stateOperation = true;
                result.MessageResult = "Categoria creada exitosamente.";
                result.Result = MapToDto(categoria, 0);

                return result;
            }
            catch (Exception ex)
            {
                result.stateOperation = false;
                result.MessageResult = "Error al crear la categoria.";
                result.MessageExceptionTechnical = ex.Message;
                return result;
            }
        }

        public async Task<ResultOperation<CategoriaDto>> UpdateAsync(CategoriaUpdateDto dto)
        {
            var result = new ResultOperation<CategoriaDto>();

            try
            {
                var categoria = await _unitOfWork.CategoriaRepository.GetByIdAsync(dto.Id);
                if (categoria == null)
                {
                    result.stateOperation = false;
                    result.MessageResult = "Categoria no encontrada.";
                    return result;
                }

                if (categoria.Nombre != dto.Nombre)
                {
                    var nameExists = await _unitOfWork.CategoriaRepository.ExistsByNameExcludingAsync(dto.Nombre, dto.Id);
                    if (nameExists)
                    {
                        result.stateOperation = false;
                        result.MessageResult = $"Ya existe una categoria con el nombre '{dto.Nombre}'.";
                        return result;
                    }
                }

                categoria.Nombre = dto.Nombre;
                categoria.Descripcion = dto.Descripcion;
                categoria.Activo = dto.Activo;

                await _unitOfWork.CategoriaRepository.UpdateAsync(categoria);

                var productCount = categoria.Productos.Count(p => p.Activo);
                result.stateOperation = true;
                result.MessageResult = "Categoria actualizada exitosamente.";
                result.Result = MapToDto(categoria, productCount);

                return result;
            }
            catch (Exception ex)
            {
                result.stateOperation = false;
                result.MessageResult = "Error al actualizar la categoria.";
                result.MessageExceptionTechnical = ex.Message;
                return result;
            }
        }

        public async Task<ResultOperation> DeleteAsync(int id)
        {
            var result = new ResultOperation();

            try
            {
                var categoria = await _unitOfWork.CategoriaRepository.GetByIdAsync(id);
                if (categoria == null)
                {
                    result.stateOperation = false;
                    result.MessageResult = "Categoria no encontrada.";
                    return result;
                }

                var hasActiveProducts = await _unitOfWork.CategoriaRepository.HasActiveProductsAsync(id);
                if (hasActiveProducts)
                {
                    result.stateOperation = false;
                    result.MessageResult = "No se puede eliminar la categoria porque tiene productos activos asociados.";
                    return result;
                }

                await _unitOfWork.CategoriaRepository.DeleteAsync(id);

                result.stateOperation = true;
                result.MessageResult = "Categoria eliminada exitosamente.";

                return result;
            }
            catch (Exception ex)
            {
                result.stateOperation = false;
                result.MessageResult = "Error al eliminar la categoria.";
                result.MessageExceptionTechnical = ex.Message;
                return result;
            }
        }

        public async Task<ResultOperation<CategoriaDto>> GetByIdAsync(int id)
        {
            var result = new ResultOperation<CategoriaDto>();

            try
            {
                var categoria = await _unitOfWork.CategoriaRepository.GetByIdAsync(id);
                if (categoria == null)
                {
                    result.stateOperation = false;
                    result.MessageResult = "Categoria no encontrada.";
                    return result;
                }

                var productCount = categoria.Productos.Count(p => p.Activo);
                result.stateOperation = true;
                result.Result = MapToDto(categoria, productCount);

                return result;
            }
            catch (Exception ex)
            {
                result.stateOperation = false;
                result.MessageResult = "Error al obtener la categoria.";
                result.MessageExceptionTechnical = ex.Message;
                return result;
            }
        }

        public async Task<ResultOperation<List<CategoriaDto>>> GetPagedAsync(int page, int pageSize)
        {
            var result = new ResultOperation<List<CategoriaDto>>();

            try
            {
                var categorias = await _unitOfWork.CategoriaRepository.GetPagedAsync(page, pageSize);

                result.stateOperation = true;
                result.Result = categorias;

                return result;
            }
            catch (Exception ex)
            {
                result.stateOperation = false;
                result.MessageResult = "Error al obtener las categorias.";
                result.MessageExceptionTechnical = ex.Message;
                return result;
            }
        }

        public async Task<ResultOperation<List<CategoriaDto>>> GetAllActiveAsync()
        {
            var result = new ResultOperation<List<CategoriaDto>>();

            try
            {
                var categorias = await _unitOfWork.CategoriaRepository.GetAllActiveAsync();

                result.stateOperation = true;
                result.Result = categorias;

                return result;
            }
            catch (Exception ex)
            {
                result.stateOperation = false;
                result.MessageResult = "Error al obtener las categorias activas.";
                result.MessageExceptionTechnical = ex.Message;
                return result;
            }
        }

        private CategoriaDto MapToDto(Categoria categoria, int productCount)
        {
            return new CategoriaDto
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion,
                Activo = categoria.Activo,
                FechaCreacion = categoria.FechaCreacion,
                ProductoCount = productCount
            };
        }
    }
}
