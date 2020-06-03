using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Planificacion
{
    public class Presupuesto
    {
        public class Financiero
        {
            public string IniToken { get; set; }
            public string IniUsuario { get; set; }
            public string IfDato0 { get; set; }
            public string IfDato1 { get; set; }
            public string IfDato2 { get; set; }
            public string IfDato3 { get; set; }
            public string IfDato4 { get; set; }
            public string IfDato5 { get; set; }
            public string IfDato6 { get; set; }
            public string IfDato7 { get; set; }
            public string IfDato8 { get; set; }
            public string IfDato9 { get; set; }
            public string IfDato10 { get; set; }
            public string IfDato11 { get; set; }
            public string IfDato12 { get; set; }
            public string IfDato13 { get; set; }
            public string IfDato14 { get; set; }
            public string IfDato15 { get; set; }
            public string IfDato16 { get; set; }
            public string IfDato17 { get; set; }
            public string IfDato18 { get; set; }
            public string IfDato19 { get; set; }
        }

        public class Fisico
        {
            public string IniToken { get; set; }
            public string IniUsuario { get; set; }
            public string FiDato0 { get; set; }
            public string FiDato1 { get; set; }
            public string FiDato2 { get; set; }
            public string FiDato3 { get; set; }
            public string FiDato4 { get; set; }
            public string FiDato5 { get; set; }
            public string FiDato6 { get; set; }
            public string FiDato7 { get; set; }
            public string FiDato8 { get; set; }
            public string FiDato9 { get; set; }
            public string FiDato10 { get; set; }
            public string FiDato11 { get; set; }
            public string FiDato12 { get; set; }
            public string FiDato13 { get; set; }
            public string FiDato14 { get; set; }
            public string FiDato15 { get; set; }
            public string FiDato16 { get; set; }
            public string FiDato17 { get; set; }
            public string FiDato18 { get; set; }
            public string FiDato19 { get; set; }
        }

        public class General
        {
            public string IdIg          { get; set; }
            public string IgToken       { get; set; }
            public string IniToken      { get; set; }
            public string IniUsuario    { get; set; }
            public string IgPresupuesto { get; set; }
            public string IgFechaInicio { get; set; }
            public string IgFechaTermino { get; set; }
            public string IgFechaCierre { get; set; }
            public string IgFecha       { get; set; }
            public string IgEstadoImportacion { get; set; }
            public string IgEstado      { get; set; }
        }
    }
}
