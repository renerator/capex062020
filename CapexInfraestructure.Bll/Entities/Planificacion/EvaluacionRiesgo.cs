using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Planificacion
{
    public class EvaluacionRiesgo
    {
        public class GuardarEvaluacion
        {
            public string IniToken { get; set; }
            public string IniUsuario { get; set; }
            public string EriProb1 { get; set; }
            public string EriImp1 { get; set; }
            public string EriRies1 { get; set; }
            public string EriClas1 { get; set; }
            public string EriMFL1 { get; set; }
            public string EriProb2 { get; set; }
            public string EriImp2 { get; set; }
            public string EriRies2 { get; set; }
            public string EriClas2 { get; set; }
            public string EriMFL2 { get; set; }
            public int EriItemMR { get; set; }

        }
    }
}
