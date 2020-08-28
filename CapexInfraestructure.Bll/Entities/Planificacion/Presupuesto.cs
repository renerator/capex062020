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
            public string IdIg { get; set; }
            public string IgToken { get; set; }
            public string IniToken { get; set; }
            public string IniUsuario { get; set; }
            public string IgPresupuesto { get; set; }
            public string IgFechaInicio { get; set; }
            public string IgFechaTermino { get; set; }
            public string IgFechaCierre { get; set; }
            public string IgFecha { get; set; }
            public string IgEstadoImportacion { get; set; }
            public string IgEstado { get; set; }
        }

        public class ParametroOrientacionComercialMes
        {
            public int IdParamEconomicoDetalleMes { get; set; }
            public string PEToken { get; set; }
            public int Anio { get; set; }
            public int Mes { get; set; }
            public decimal Value { get; set; }
            public int PEDMEstado { get; set; }
        }

        public class ParametroOrientacionComercialAnio
        {
            public int IdParamEconomicoDetalle { get; set; }
            public string PEToken { get; set; }
            public int Anio { get; set; }
            public decimal Value { get; set; }
            public int PEDEstado { get; set; }
        }

        public class ParametroOrientacionVN
        {
            public int IdParametroVN { get; set; }
            public string ParametroVNToken { get; set; }
            public string PVNUsuarioCrea { get; set; }
            public string PVNFecha { get; set; }
            public int PVNPERIODO { get; set; }
            public int PVNVERSION { get; set; }
            public int PVNVERSIONORIGEN { get; set; }
            public int PVNACTIVO { get; set; }
            public int PVNEstado { get; set; }
            public int PVNTIPO { get; set; }
        }


        public class CellValue
        {
            public string Titulo { get; set; }
            public string TituloNac { get; set; }
            public string TituloExt { get; set; }
            public decimal ValorNac { get; set; }
            public decimal ValorExt { get; set; }
            public decimal ValorTotal { get; set; }
        }

        public class FinancieroDetalleCasoBase
        {
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
            public string IfDato20 { get; set; }
            public string IfDato21 { get; set; }
            public string IfDato22 { get; set; }
            public string IfDato23 { get; set; }
            public string IfDato24 { get; set; }
            public string IfDato25 { get; set; }
            public string IfDato26 { get; set; }
            public string IfDato27 { get; set; }
            public string IfDato28 { get; set; }
            public string IfDato29 { get; set; }
            public string IfDato30 { get; set; }
            public string IfDato31 { get; set; }
            public string IfDato32 { get; set; }
            public string IfDato33 { get; set; }
            public string IfDato34 { get; set; }
            public string IfDato35 { get; set; }
            public string IfDato36 { get; set; }
            public string IfDato37 { get; set; }
            public string IfDato38 { get; set; }
            public string IfDato39 { get; set; }
            public string IfDato40 { get; set; }
            public string IfDato41 { get; set; }
            public string IfDato42 { get; set; }
            public string IfDato43 { get; set; }
            public string IfDato44 { get; set; }
            public string IfDato45 { get; set; }
            public string IfDato46 { get; set; }
            public string IfDato47 { get; set; }
            public string IfDato48 { get; set; }
            public string IfDato49 { get; set; }
            public string IfDato50 { get; set; }
            public string IfDato51 { get; set; }
            public string IfDato52 { get; set; }
            public string IfDato53 { get; set; }
            public string IfDato54 { get; set; }
            public string IfDato55 { get; set; }
            public string IfDato56 { get; set; }
            public string IfDato57 { get; set; }
            public string IfDato58 { get; set; }
            public string IfDato59 { get; set; }
            public string IfDato60 { get; set; }
            public string IfDato61 { get; set; }
            public string IfDato62 { get; set; }
            public string IfDato63 { get; set; }
            public string IfDato64 { get; set; }
            public string IfDato65 { get; set; }
            public string IfDato66 { get; set; }
            public string IfDato67 { get; set; }
            public string IfDato68 { get; set; }
            public string IfDato69 { get; set; }
            public string IfDato70 { get; set; }
            public string IfDato71 { get; set; }
            public string IfDato72 { get; set; }
            public string IfDato73 { get; set; }
            public string IfDato74 { get; set; }
            public string IfDato75 { get; set; }
            public string IfDato76 { get; set; }
            public string IfDato77 { get; set; }
            public string IfDato78 { get; set; }
            public string IfDato79 { get; set; }
            public string IfDato80 { get; set; }
            public string IfDato81 { get; set; }
            public string IfDato82 { get; set; }
            public string IfDato83 { get; set; }
            public string IfDato84 { get; set; }
            public string IfDato85 { get; set; }
            public string IfDato86 { get; set; }
            public string IfDato87 { get; set; }
            public string IfDato88 { get; set; }
            public string IfDato89 { get; set; }
            public string IfDato90 { get; set; }
            public string IfDato91 { get; set; }
            public string IfDato92 { get; set; }
            public string IfDato93 { get; set; }
            public string IfDato94 { get; set; }
            public string IfDato95 { get; set; }
            public string IfDato96 { get; set; }
        }

        public class FinancieroDetallePresupuesto
        {
            public string IfDDato0 { get; set; }
            public string IfDDato1 { get; set; }
            public string IfDDato2 { get; set; }
            public string IfDDato3 { get; set; }
            public string IfDDato4 { get; set; }
            public string IfDDato5 { get; set; }
            public string IfDDato6 { get; set; }
            public string IfDDato7 { get; set; }
            public string IfDDato8 { get; set; }
            public string IfDDato9 { get; set; }
            public string IfDDato10 { get; set; }
            public string IfDDato11 { get; set; }
            public string IfDDato12 { get; set; }
            public string IfDDato13 { get; set; }
            public string IfDDato14 { get; set; }
            public string IfDDato15 { get; set; }
            public string IfDDato16 { get; set; }
            public string IfDDato17 { get; set; }
            public string IfDDato18 { get; set; }
            public string IfDDato19 { get; set; }
        }

    }
}
