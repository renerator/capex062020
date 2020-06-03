using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Planificacion
{
    public class Categorizacion
    {
        public class Categoria
        {
            public string IdCat {get;set;}
            public string CatToken {get;set;}
            public string CatNombre {get;set;}
            public string CatDescripcion {get;set;}
            public string CatEstado {get;set;}
        }

        public class NivelIngenieria
        {
            public string IdNI { get; set; }
            public string NIToken { get; set; }
            public string NINombre { get; set; }
            public string NIAcronimo { get; set; }
            public string NIDescripcion { get; set; }
            public string NIEstado { get; set; }
        }

        public class ClasificacionSSO
        {
            public string IdCS { get; set; }
            public string CSToken { get; set; }
            public string CSNombre { get; set; }
            public string CSDescripcion { get; set; }
            public string CSEstado { get; set; }
        }

        public class EstandarSeguridad
        {
            public string IdEss { get; set; }
            public string EssToken { get; set; }
            public string EssComToken { get; set; }
            public string EssCSToken { get; set; }
            public string EssNombre { get; set; }
            public string EssDescripcion { get; set; }
            public string EssEstado { get; set; }
        }

        public class DatosCategorizacion
        {
            public string IniToken { get; set; }
            public string IniUsuario { get; set; }
            public string CatEstadoProyecto { get; set; }
            public string CatCategoria { get; set; }
            public string CatNivelIngenieria { get; set; }
            public string CatAgrega { get; set; }
            public string CatTipoCotizacion { get; set; }
            public string CatClasificacionSSO { get; set; }
            public string CatEstandarSeguridad { get; set; }
            public string CatClase { get; set; }
            public string CatMacroCategoria { get; set; }
            public string CatAnalisis { get; set; }
            public int CatACNota1 { get; set; }
            public int CatACNota2 { get; set; }
            public int CatACNota3 { get; set; }
            public int CatACNota4 { get; set; }
            public int CatACNota5 { get; set; }
            public int CatACNota6 { get; set; }
            public string CatACTotal { get; set; }
            public string CatACObs1 { get; set; }
            public string CatACObs2 { get; set; }
            public string CatACObs3 { get; set; }
            public string CatACObs4 { get; set; }
            public string CatACObs5 { get; set; }
            public string CatACObs6 { get; set; }
        }
    }
}
