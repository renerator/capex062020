using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Mantenedor
{
    public class Superintendente
    {
        public class NuevoSuperintendente
        {
            public string IdSuper  { get; set; }
            public string IntNombre { get; set; }
            public string IntDescripcion { get; set; }
        }

        public class ModificarSuperintendente
        {
            public string IdSuper { get; set; }
            public string IntToken { get; set; }
            public string IntNombre { get; set; }
            public string IntDescripcion { get; set; }

        }
    }
}
