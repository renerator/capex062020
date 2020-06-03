using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Mantenedor
{
    public class Area
    {
        public class GuardarArea
        {
            public string AreaNombre { get; set; }
            public string AreaAcronimo { get; set; }
            public string AreaDescripcion { get; set; }
            public int AreaEstado { get; set; }
        }
        public class ActualizarArea
        {
            public string AreaToken { get; set; }
            public string AreaNombre { get; set; }
            public string AreaAcronimo { get; set; }
            public string AreaDescripcion { get; set; }
            public int AreaEstado { get; set; }
        }
    }
}
