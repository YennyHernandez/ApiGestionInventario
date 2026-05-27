namespace Inventario.Domain.Models.Dto
{
    public class CategoriaUpdateDto
    {
        /// <summary>
        /// Identificador único de la categoría
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre de la categoría
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Descripción de la categoría (opcional)
        /// </summary>
        public string? Descripcion { get; set; }

        /// <summary>
        /// Indica si la categoría está activa
        /// </summary>
        public bool Activo { get; set; }
    }
}