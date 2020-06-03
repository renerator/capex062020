using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Mantenedor
{
    public class ClasificacionSSO
    {
        public class GuardarClasificacionSSO
        {
            public string CSNombre { get; set; }
            public string CSDescripcion { get; set; }
            public int CSEstado { get; set; }
        }
        public class ActualizarClasificacionSSO
        {
            public string CSToken { get; set; }
            public string CSNombre { get; set; }
            public string CSDescripcion { get; set; }
            public int CSEstado { get; set; }
        }
    }
}
