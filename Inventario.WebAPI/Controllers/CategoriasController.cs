using Inventario.Application.Services.Interfaces;
using Inventario.Domain.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventario.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        /// <summary>
        /// Crea una nueva categoría
        /// </summary>
        /// <example>
        /// POST /api/categorias
        /// {
        ///   "nombre": "Electronica",
        ///   "descripcion": "Productos electronicos"
        /// }
        /// Response 200: { "stateOperation": true, "messageResult": "Categoria creada exitosamente.", "result": { "id": 1, "nombre": "Electronica", ... } }
        /// </example>
        [HttpPost]
        [ProducesResponseType(typeof(ResultOperation<CategoriaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultOperation<CategoriaDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CategoriaCreateDto dto)
        {
            var result = await _categoriaService.CreateAsync(dto);
            if (!result.stateOperation)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene categorías paginadas
        /// </summary>
        /// <example>
        /// GET /api/categorias?page=1&amp;pageSize=10
        /// Response 200: { "stateOperation": true, "results": [{ "id": 1, "nombre": "Electronica", "productoCount": 5, ... }] }
        /// </example>
        [HttpGet]
        [ProducesResponseType(typeof(ResultOperation<List<CategoriaDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _categoriaService.GetPagedAsync(page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene una categoría por su ID
        /// </summary>
        /// <example>
        /// GET /api/categorias/1
        /// Response 200: { "stateOperation": true, "result": { "id": 1, "nombre": "Electronica", ... } }
        /// Response 404: { "stateOperation": false, "messageResult": "Categoria no encontrada." }
        /// </example>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultOperation<CategoriaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultOperation<CategoriaDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _categoriaService.GetByIdAsync(id);
            if (!result.stateOperation)
                return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene todas las categorías activas
        /// </summary>
        /// <example>
        /// GET /api/categorias/activas
        /// Response 200: { "stateOperation": true, "results": [{ "id": 1, "nombre": "Electronica", ... }] }
        /// </example>
        [HttpGet("activas")]
        [ProducesResponseType(typeof(ResultOperation<List<CategoriaDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllActive()
        {
            var result = await _categoriaService.GetAllActiveAsync();
            return Ok(result);
        }

        /// <summary>
        /// Actualiza una categoría existente
        /// </summary>
        /// <example>
        /// PUT /api/categorias/1
        /// {
        ///   "id": 1,
        ///   "nombre": "Electronica Actualizada",
        ///   "descripcion": "Nueva descripcion",
        ///   "activo": true
        /// }
        /// Response 200: { "stateOperation": true, "messageResult": "Categoria actualizada exitosamente.", "result": { ... } }
        /// </example>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultOperation<CategoriaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultOperation<CategoriaDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultOperation<CategoriaDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] CategoriaUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new ResultOperation<CategoriaDto> { stateOperation = false, MessageResult = "El ID de la URL no coincide con el del cuerpo." });

            var result = await _categoriaService.UpdateAsync(dto);
            if (!result.stateOperation)
                return result.MessageResult == "Categoria no encontrada." ? NotFound(result) : BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Elimina una categoría de forma lógica (solo si no tiene productos activos)
        /// </summary>
        /// <example>
        /// DELETE /api/categorias/1
        /// Response 200: { "stateOperation": true, "messageResult": "Categoria eliminada exitosamente." }
        /// Response 400: { "stateOperation": false, "messageResult": "No se puede eliminar la categoria porque tiene productos activos asociados." }
        /// Response 404: { "stateOperation": false, "messageResult": "Categoria no encontrada." }
        /// </example>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultOperation), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultOperation), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultOperation), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoriaService.DeleteAsync(id);
            if (!result.stateOperation)
            {
                if (result.MessageResult.Contains("no encontrada"))
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}