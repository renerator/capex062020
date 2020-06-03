using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Mantenedor
{
    public class Gerencia
    {
        public class GuardarGerencia
        {
            public string GerNombre { get; set; }
            public string GerDescripcion { get; set; }
            public int GerEstado { get; set; }
        }
        public class ActualizarGerencia
        {
            public string GerToken { get; set; }
            public string GerNombre { get; set; }
            public string GerDescripcion { get; set; }
            public int GerEstado { get; set; }
        }
    }
}
