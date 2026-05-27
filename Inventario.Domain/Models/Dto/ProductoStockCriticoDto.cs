namespace Inventario.Domain.Models.Dto
{
    public class ProductoStockCriticoDto
    {
        /// <summary>
        /// Código único de identificación del producto (SKU)
        /// </summary>
        public string Sku { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del producto
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Cantidad actual en inventario
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// Cantidad mínima requerida antes de generar alerta
        /// </summary>
        public int StockMinimo { get; set; }

        /// <summary>
        /// Nombre de la categoría a la que pertenece el producto
        /// </summary>
        public string? CategoriaNombre { get; set; }
    }
}