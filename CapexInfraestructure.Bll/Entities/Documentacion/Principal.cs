using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Documentacion
{
    public class Principal
    {
        public class Categorias
        {
            public string IdDocCat { get; set; }
            public string DocCatToken { get; set; }
            public string DocCatNombre { get; set; }
            public string DocCatFecha { get; set; }
            public string DocCatEstado { get; set; }
        }
    }
}
