using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Contacto
{
    public class ContactoAdministrador
    {
        public class NuevaSolicitud
        {
            public string SolTipo { get; set; }
            public string SolOtroTelefono { get; set; }
            public string SolComentario { get; set; }
            public string PidUsuario { get; set; }
        }
    }
}
