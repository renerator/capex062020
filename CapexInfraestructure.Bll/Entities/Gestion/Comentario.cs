using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Gestion
{
    public class Comentario
    {
        public class Comentar
        {
            public string IniToken { get; set; }
            public string ObsRemite { get; set; }
            public string ComeTexto { get; set; }
            public string ComePrioridad { get; set; }
        }
    }
}
