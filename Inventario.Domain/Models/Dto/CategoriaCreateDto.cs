namespace Inventario.Domain.Models.Dto
{
    public class CategoriaCreateDto
    {
        /// <summary>
        /// Nombre de la categoría
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Descripción de la categoría (opcional)
        /// </summary>
        public string? Descripcion { get; set; }
    }
}