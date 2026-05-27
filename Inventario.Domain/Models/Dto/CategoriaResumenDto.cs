namespace Inventario.Domain.Models.Dto
{
    public class CategoriaResumenDto
    {
        /// <summary>
        /// Nombre de la categoría
        /// </summary>
        public string CategoriaNombre { get; set; } = string.Empty;

        /// <summary>
        /// Cantidad total de productos en esta categoría
        /// </summary>
        public int CantidadProductos { get; set; }

        /// <summary>
        /// Valor total del inventario de esta categoría (precio × stock)
        /// </summary>
        public decimal ValorTotal { get; set; }
    }
}