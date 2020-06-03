using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Login
{
    public class Usuario
    {
        public class InformacionUsuario
        {
            public string UsuToken { get; set; }
            public string ComToken { get; set; }
            public string AreaToken { get; set; }
            public string IdEmpresa { get; set; }
            public string UsuId { get; set; }
            public string UsuCodigo { get; set; }
            public string RolNombre { get; set; }
            public string UsuTipo { get; set; }
            public string UsuRut { get; set; }
            public string UsuNombre { get; set; }
            public string UsuApellido { get; set; }
            public string UsuEmail { get; set; }
            public string UsuTelefono { get; set; }
            public string UsuMovil { get; set; }
            public string UsuImagen { get; set; }
            public string UsuCreacion { get; set; }
            public string UsuEstadoRegistro { get; set; }
            public string UsuEstado { get; set; }
        }
    }
}
