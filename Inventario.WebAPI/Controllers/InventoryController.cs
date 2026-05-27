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
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryReportService _reportService;

        public InventoryController(IInventoryReportService reportService)
        {
            _reportService = reportService;
        }

        /// <summary>
        /// Obtiene el resumen del inventario con indicadores clave (KPIs)
        /// </summary>
        /// <example>
        /// GET /api/inventory/summary
        /// Response 200:
        /// {
        ///   "stateOperation": true,
        ///   "result": {
        ///     "valorTotalInventario": 150000.00,
        ///     "productosPorCategoria": [
        ///       { "categoriaNombre": "Electronica", "cantidadProductos": 5, "valorTotal": 75000.00 }
        ///     ],
        ///     "productosStockCritico": [
        ///       { "sku": "PROD-003", "nombre": "Producto Bajo", "stock": 2, "stockMinimo": 10, "categoriaNombre": "Electronica" }
        ///     ],
        ///     "porcentajeOcupacion": 75.50
        ///   }
        /// }
        /// </example>
        [HttpGet("summary")]
        [ProducesResponseType(typeof(ResultOperation<InventorySummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSummary()
        {
            var result = await _reportService.GetSummaryAsync();
            return Ok(result);
        }
    }
}