using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Mantenedor
{
    public class Categoria
    {
        public class GuardarCategoria
        {
            public string CatNombre { get; set; }
            public string CatDescripcion { get; set; }
            public int CatEstado { get; set; }
        }
        public class ActualizarCategoria
        {
            public string CatToken { get; set; }
            public string CatNombre { get; set; }
            public string CatDescripcion { get; set; }
            public int CatEstado { get; set; }
        }
    }
}
