using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Planificacion
{
    public class EvaluacionEconomica
    {
        public class GuardarEvaluacion
        {
            public string IniUsuario { get; set; }
            public string IniToken { get; set; }
            public string EveVan { get; set; }
            public string EveIvan { get; set; }
            public string EvePayBack { get; set; }
            public string EveVidaUtil { get; set; }
            public string EveTipoCambio { get; set; }
            public string EveTir { get; set; }
        }
    }
}
