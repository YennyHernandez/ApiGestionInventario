using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventario.Domain.Models.Farma.SP
{
    public class UsuarioAdministrativoPost
    {
        public int? IdSede { get; set; }
        public string NombreUsuario { get; set; }
        public string Contrasena { get; set; }
        public string UsuarioRed { get; set; }
        public string CorreoElectronico { get; set; }
        public bool? Activo { get; set; }
    }
}
