using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Mantenedor
{
    public class Riesgo
    {
        public class GuardarRiesgo
        {
            public string RiesgoNombre { get; set; }
            public int EvrImpacto { get; set; }
            public int EvrProbabilidad { get; set; }
            public int RiesgoEstado { get; set; }
        }
        public class ActualizarRiesgo
        {
            public string RiesgoToken { get; set; }
            public string RiesgoNombre { get; set; }
            public int EvrImpacto { get; set; }
            public int EvrProbabilidad { get; set; }
            public int RiesgoEstado { get; set; }
        }
    }
}
