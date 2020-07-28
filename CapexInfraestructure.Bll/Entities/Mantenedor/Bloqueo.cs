using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Mantenedor
{
    public class Bloqueo
    {
        public class GuardarBloqueo
        {
            public int TipoIniciativaSeleccionado { get; set; }
            public string FechaDesde { get; set; }
            public string FechaHasta { get; set; }
        }

        public class ObtenerBloqueo
        {
            public int IdFechaBloqueo { get; set; }
            public string FechaBloqueoToken { get; set; }
            public int TipoIniciativaSeleccionado { get; set; }
            public string FechaDesde { get; set; }
            public string FechaHasta { get; set; }
            public string Usuario { get; set; }
            public int Estado { get; set; }
        }

        public class ActualizarBloqueo
        {
            public string FechaBloqueoToken { get; set; }
            public int TipoIniciativaSeleccionado { get; set; }
            public string FechaDesde { get; set; }
            public string FechaHasta { get; set; }
        }

    }
}
