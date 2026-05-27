using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventario.Domain.Models.Farma.SP
{
    public class FuncionarioTaquillaPost
    {
        public int? IdSede { get; set; }
        public string Identificacion { get; set; }
        public string NombreFuncionario { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Usuario { get; set; }
        public string Contrasena { get; set; }
        public bool? Activo { get; set; }
        public bool? Disponible { get; set; }
        public bool? Eliminar { get; set; }
        public string CorreoElectronico { get; set; }
        public int? IdCompania { get; set; }
    }
}
