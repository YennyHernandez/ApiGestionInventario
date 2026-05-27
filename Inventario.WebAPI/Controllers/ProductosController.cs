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
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        /// <summary>
        /// Crea un nuevo producto
        /// </summary>
        /// <example>
        /// POST /api/productos
        /// {
        ///   "sku": "PROD-001",
        ///   "nombre": "Producto Ejemplo",
        ///   "descripcion": "Descripcion del producto",
        ///   "precio": 99.99,
        ///   "stock": 100,
        ///   "stockMinimo": 10,
        ///   "categoriaId": 1
        /// }
        /// Response 200: { "stateOperation": true, "messageResult": "Producto creado exitosamente.", "result": { "id": 1, "sku": "PROD-001", ... } }
        /// </example>
        [HttpPost]
        [ProducesResponseType(typeof(ResultOperation<ProductoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultOperation<ProductoDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ProductoCreateDto dto)
        {
            var result = await _productoService.CreateAsync(dto);
            if (!result.stateOperation)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene productos paginados
        /// </summary>
        /// <example>
        /// GET /api/productos?page=1&amp;pageSize=10
        /// Response 200: { "stateOperation": true, "results": [{ "id": 1, "sku": "PROD-001", "nombre": "Producto Ejemplo", ... }] }
        /// </example>
        [HttpGet]
        [ProducesResponseType(typeof(ResultOperation<List<ProductoDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _productoService.GetPagedAsync(page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene un producto por su ID
        /// </summary>
        /// <example>
        /// GET /api/productos/1
        /// Response 200: { "stateOperation": true, "result": { "id": 1, "sku": "PROD-001", ... } }
        /// Response 404: { "stateOperation": false, "messageResult": "Producto no encontrado." }
        /// </example>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResultOperation<ProductoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultOperation<ProductoDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productoService.GetByIdAsync(id);
            if (!result.stateOperation)
                return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene un producto por su código SKU
        /// </summary>
        /// <example>
        /// GET /api/productos/sku/PROD-001
        /// Response 200: { "stateOperation": true, "result": { "id": 1, "sku": "PROD-001", ... } }
        /// Response 404: { "stateOperation": false, "messageResult": "Producto no encontrado." }
        /// </example>
        [HttpGet("sku/{sku}")]
        [ProducesResponseType(typeof(ResultOperation<ProductoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultOperation<ProductoDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBySku(string sku)
        {
            var result = await _productoService.GetBySkuAsync(sku);
            if (!result.stateOperation)
                return NotFound(result);
            return Ok(result);
        }

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        /// <example>
        /// PUT /api/productos/1
        /// {
        ///   "id": 1,
        ///   "sku": "PROD-001",
        ///   "nombre": "Producto Actualizado",
        ///   "descripcion": "Nueva descripcion",
        ///   "precio": 149.99,
        ///   "stock": 50,
        ///   "stockMinimo": 5,
        ///   "activo": true,
        ///   "categoriaId": 1
        /// }
        /// Response 200: { "stateOperation": true, "messageResult": "Producto actualizado exitosamente.", "result": { ... } }
        /// </example>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResultOperation<ProductoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultOperation<ProductoDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultOperation<ProductoDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] ProductoUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new ResultOperation<ProductoDto> { stateOperation = false, MessageResult = "El ID de la URL no coincide con el del cuerpo." });

            var result = await _productoService.UpdateAsync(dto);
            if (!result.stateOperation)
                return result.MessageResult == "Producto no encontrado." ? NotFound(result) : BadRequest(result);
            return Ok(result);
        }

        /// <summary>
        /// Elimina un producto de forma lógica (soft delete)
        /// </summary>
        /// <example>
        /// DELETE /api/productos/1
        /// Response 200: { "stateOperation": true, "messageResult": "Producto eliminado exitosamente." }
        /// Response 404: { "stateOperation": false, "messageResult": "Producto no encontrado." }
        /// </example>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResultOperation), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultOperation), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productoService.DeleteAsync(id);
            if (!result.stateOperation)
                return NotFound(result);
            return Ok(result);
        }
    }
}