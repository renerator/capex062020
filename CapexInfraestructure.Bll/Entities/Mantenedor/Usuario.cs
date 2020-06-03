using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Mantenedor
{
    public class Usuario
    {
        public class NuevoUsuario
        {
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public int    Status { get; set; }

            public string ComToken { get; set; }
            public string AreaToken { get; set; }
            public string IdEmpresa { get; set; }
            public string UsuTipo { get; set; }
            public string UsuRut { get; set; }
            public string UsuNombre { get; set; }
            public string UsuApellido { get; set; }
            public string UsuEmail { get; set; }
            public string UsuTelefono { get; set; }
            public string UsuMovil { get; set; }
            public string UsuImagen { get; set; }

            public string GrvUser { get; set; }
            public string GrvUserToken { get; set; }
            public string GrvAreaRevToken { get; set; }
            public string GrvAreaRevNombre { get; set; }

            public string UserID { get; set; }
            public string RoleID { get; set; }

        }
        public class ModificarUsuario
        {
            public string ID { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public int Status { get; set; }

            public string UsuToken { get; set; }
            public string Token { get; set; }
            public string ComToken { get; set; }
            public string AreaToken { get; set; }
            public string IdEmpresa { get; set; }
            public string UsuTipo { get; set; }
            public string UsuRut { get; set; }
            public string UsuNombre { get; set; }
            public string UsuApellido { get; set; }
            public string UsuEmail { get; set; }
            public string UsuTelefono { get; set; }
            public string UsuMovil { get; set; }
            public string UsuImagen { get; set; }

            public string GrvToken { get; set; }
            public string GrvUser { get; set; }
            public string GrvUserToken { get; set; }
            public string GrvAreaRevToken { get; set; }
            public string GrvAreaRevNombre { get; set; }

            public string UserRolID { get; set; }
            public string UserID { get; set; }
            public string RoleID { get; set; }

        }
    }
}
