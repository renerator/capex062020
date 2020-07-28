using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Mantenedor
{
    public class Template
    {
        public class GuardarTemplate
        {
            public int TPEPERIODO { get; set; }
            public int TipoIniciativaSeleccionado { get; set; }
            public string TPEPERIODOS { get; set; }
            public int PARAMANIOMASUNO { get; set; }
            public int PARAMANIOMASDOS { get; set; }
            public int PARAMANIOMASTRES { get; set; }
            public string VALUEANIOTC { get; set; }
            public string VALUEANIOTCMASUNO { get; set; }
            public string VALUEANIOTCMASDOS { get; set; }
            public string VALUEANIOTCMASTRES { get; set; }
            public string VALUEANIOIPC { get; set; }
            public string VALUEANIOIPCMASUNO { get; set; }
            public string VALUEANIOIPCMASDOS { get; set; }
            public string VALUEANIOIPCMASTRES { get; set; }
            public string VALUEANIOCPI { get; set; }
            public string VALUEANIOCPIMASUNO { get; set; }
            public string VALUEANIOCPIMASDOS { get; set; }
            public string VALUEANIOCPIMASTRES { get; set; }
        }

        public class GuardarTemplateCorregido
        {
            public int TPEPERIODO { get; set; }
            public int TipoIniciativaSeleccionado { get; set; }
            public string TPEPERIODOS { get; set; }
            public string VALUEMESTC { get; set; }
            public string VALUEMESIPC { get; set; }
            public string VALUEMESCPI { get; set; }
            public string VALUEANIOTC { get; set; }
            public string VALUEANIOIPC { get; set; }
            public string VALUEANIOCPI { get; set; }
        }

        public class ObtenerTemplate
        {
            public string ITPEToken { get; set; }
            public int TPEPERIODO { get; set; }
            public int TPEPERIODORESPALDO { get; set; }
            public string TPEPERIODOS { get; set; }
            public string PEToken { get; set; }
            public string TPEToken { get; set; }
            public string TPENombre { get; set; }
            public int IdParamEconomicoDetalleANIO { get; set; }
            public int PARAMANIO { get; set; }
            public double VALUEANIO { get; set; }
            public int IdParamEconomicoDetalleANIOMASUNO { get; set; }
            public int PARAMANIOMASUNO { get; set; }
            public double VALUEANIOMASUNO { get; set; }
            public int IdParamEconomicoDetalleANIOMASDOS { get; set; }
            public int PARAMANIOMASDOS { get; set; }
            public double VALUEANIOMASDOS { get; set; }
            public int IdParamEconomicoDetalleANIOMASTRES { get; set; }
            public int PARAMANIOMASTRES { get; set; }
            public double VALUEANIOMASTRES { get; set; }
        }

        public class ObtenerTemplateCorregido
        {
            public string ITPEToken { get; set; }
            public int TPEPERIODO { get; set; }
            public int TPEPERIODORESPALDO { get; set; }
            public string TPEPERIODOS { get; set; }
            public string PETokenTC { get; set; }
            public string TPETokenTC { get; set; }
            public string PETokenIPC { get; set; }
            public string TPETokenIPC { get; set; }
            public string PETokenCPI { get; set; }
            public string TPETokenCPI { get; set; }

            public string ParamTCMes { get; set; }
            public string ParamIPCMes { get; set; }
            public string ParamCPIMes { get; set; }

            public string ParamTCAnio { get; set; }
            public string ParamIPCAnio { get; set; }
            public string ParamCPIAnio { get; set; }

        }

        public class ActualizarTemplate
        {
            public int TipoIniciativaSeleccionado { get; set; }
            public string ITPEToken { get; set; }
            public int TPEPERIODO { get; set; }
            public int TPEPERIODORESPALDO { get; set; }
            public int PARAMANIOMASUNO { get; set; }
            public int PARAMANIOMASDOS { get; set; }
            public int PARAMANIOMASTRES { get; set; }
            public string PETokenTC { get; set; }
            public int IdParamEconomicoDetalleTCANIO { get; set; }
            public string VALUEANIOTC { get; set; }
            public int IdParamEconomicoDetalleTCANIOMASUNO { get; set; }
            public string VALUEANIOTCMASUNO { get; set; }
            public int IdParamEconomicoDetalleTCANIOMASDOS { get; set; }
            public string VALUEANIOTCMASDOS { get; set; }
            public int IdParamEconomicoDetalleTCANIOMASTRES { get; set; }
            public string VALUEANIOTCMASTRES { get; set; }
            public string PETokenIPC { get; set; }
            public int IdParamEconomicoDetalleIPCANIO { get; set; }
            public string VALUEANIOIPC { get; set; }
            public int IdParamEconomicoDetalleIPCANIOMASUNO { get; set; }
            public string VALUEANIOIPCMASUNO { get; set; }
            public int IdParamEconomicoDetalleIPCANIOMASDOS { get; set; }
            public string VALUEANIOIPCMASDOS { get; set; }
            public int IdParamEconomicoDetalleIPCANIOMASTRES { get; set; }
            public string VALUEANIOIPCMASTRES { get; set; }
            public string PETokenCPI { get; set; }
            public int IdParamEconomicoDetalleCPIANIO { get; set; }
            public string VALUEANIOCPI { get; set; }
            public int IdParamEconomicoDetalleCPIANIOMASUNO { get; set; }
            public string VALUEANIOCPIMASUNO { get; set; }
            public int IdParamEconomicoDetalleCPIANIOMASDOS { get; set; }
            public string VALUEANIOCPIMASDOS { get; set; }
            public int IdParamEconomicoDetalleCPIANIOMASTRES { get; set; }
            public string VALUEANIOCPIMASTRES { get; set; }
        }

        public class ActualizarTemplateCorregido
        {
            public int TipoIniciativaSeleccionado { get; set; }
            public string ITPEToken { get; set; }
            public int TPEPERIODO { get; set; }
            public int TPEPERIODORESPALDO { get; set; }
            public string PETokenTC { get; set; }
            public string PETokenIPC { get; set; }
            public string PETokenCPI { get; set; }
            public string IdParamEconomicoDetalleTCMES { get; set; }
            public string IdParamEconomicoDetalleIPCMES { get; set; }
            public string IdParamEconomicoDetalleCPIMES { get; set; }
            public string IdParamEconomicoDetalleTCANIO { get; set; }
            public string IdParamEconomicoDetalleIPCANIO { get; set; }
            public string IdParamEconomicoDetalleCPIANIO { get; set; }
        }

    }
}
