using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Mantenedor
{
    public class Gerente
    {
        public class NuevoGerente
        {
            public int IdGerencia { get; set; }
            public int GteEstado { get; set; }
            public string GteNombre { get; set; }
            public string GteDescripcion { get; set; }
        }

        public class ModificarGerente
        {
            public int IdGerencia { get; set; }
            public int GteEstado { get; set; }
            public string GteToken { get; set; }
            public string GteNombre { get; set; }
            public string GteDescripcion { get; set; }
        }
    }
}
