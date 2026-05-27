namespace Inventario.Domain.Models.Dto
{
    public class InventorySummaryDto
    {
        /// <summary>
        /// Valor total de todo el inventario (suma de precio × stock de todos los productos)
        /// </summary>
        public decimal ValorTotalInventario { get; set; }

        /// <summary>
        /// Resumen de productos agrupados por categoría
        /// </summary>
        public List<CategoriaResumenDto> ProductosPorCategoria { get; set; } = new();

        /// <summary>
        /// Lista de productos con stock por debajo del mínimo requerido
        /// </summary>
        public List<ProductoStockCriticoDto> ProductosStockCritico { get; set; } = new();

        /// <summary>
        /// Porcentaje de ocupación del inventario respecto a la capacidad total
        /// </summary>
        public decimal PorcentajeOcupacion { get; set; }
    }
}