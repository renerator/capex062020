using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Planificacion
{
    public class Descripcion
    {
        public class DescripcionDetallada
        {
            public string IniUsuario { get; set; }
            public string IniToken { get; set; }
            public string PddObjetivo { get; set; }
            public string PddAlcance { get; set; }
            public string PddJustificacion { get; set; }
            public string PddDescripcion1 { get; set; }
            public string PddUnidad1 { get; set; }
            public string PddActual1 { get; set; }
            public string PddTarget1 { get; set; }
            public string PddDescripcion2 { get; set; }
            public string PddUnidad2 { get; set; }
            public string PddActual2 { get; set; }
            public string PddTarget2 { get; set; }
            public string PddDescripcion3 { get; set; }
            public string PddUnidad3 { get; set; }
            public string PddActual3 { get; set; }
            public string PddTarget3 { get; set; }
            public DateTime PddFechaPostEval { get; set; }
        }
    }
}
