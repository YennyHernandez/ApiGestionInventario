namespace Inventario.Domain.Models.Farma.SP
{
    public class FuncionarioTaquilla
    {
        public int IdFuncionario { get; set; }
        public string Identificación { get; set; }
        public string Nombre { get; set; }
        public string Usuario { get; set; }
        public string CorreoElectronico { get; set; }
        public bool Activo { get; set; }
        public int IdEmpresa { get; set; }
        public string Empresa { get; set; }
        public int IdSede { get; set; }
        public string Sede { get; set; }
    }
}
