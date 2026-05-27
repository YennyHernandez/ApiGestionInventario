namespace Inventario.Domain.Models.Dto
{
    public class ProductoDto
    {
        /// <summary>
        /// Identificador único del producto
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Código único de identificación del producto (SKU)
        /// </summary>
        public string Sku { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del producto
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Descripción detallada del producto (opcional)
        /// </summary>
        public string? Descripcion { get; set; }

        /// <summary>
        /// Precio unitario del producto
        /// </summary>
        public decimal Precio { get; set; }

        /// <summary>
        /// Cantidad disponible en inventario
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// Cantidad mínima de stock antes de alerta
        /// </summary>
        public int StockMinimo { get; set; }

        /// <summary>
        /// Indica si el producto está activo
        /// </summary>
        public bool Activo { get; set; }

        /// <summary>
        /// ID de la categoría a la que pertenece
        /// </summary>
        public int CategoriaId { get; set; }

        /// <summary>
        /// Nombre de la categoría a la que pertenece
        /// </summary>
        public string? CategoriaNombre { get; set; }

        /// <summary>
        /// Fecha y hora de creación del producto
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Fecha y hora de la última actualización (nulo si no se ha modificado)
        /// </summary>
        public DateTime? FechaActualizacion { get; set; }
    }
}