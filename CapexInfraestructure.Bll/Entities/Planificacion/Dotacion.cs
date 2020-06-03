using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Planificacion
{
    public class Dotacion
    {
        public class DepartamentoDotacion
        {
            public string IdDep { get; set; }
            public string DepToken { get; set; }
            public string ComToken { get; set; }
            public string DepNombre { get; set; }
            public string DepCom { get; set; }
            public string DepDescripcion { get; set; }
            public string DepEstado { get; set; }
        }

        public class Turnos
        {
            public string IdTur { get; set; }
            public string TurToken { get; set; }
            public string ComToken { get; set; }
            public string TurNombre { get; set; }
            public string TurDescipcion { get; set; }
            public string TurCom { get; set; }
            public string TurEstado { get; set; }
        }

        public class DotacionEECC
        {
            public string IdEecc { get; set; }
            public string EeccToken { get; set; }
            public string ComToken { get; set; }
            public string EeccNombre { get; set; }
            public string EeccCom { get; set; }
            public string EeccDescripcion { get; set; }
            public string EeccEstado { get; set; }
        }

        public class DotacionClasificacion
        {
            public string IdCdot { get; set; }
            public string CdotToken { get; set; }
            public string ComToken { get; set; }
            public string CdotNombre { get; set; }
            public string CdotCom { get; set; }
            public string CdotDescripcion { get; set; }
            public string CdotEstado { get; set; }
        }
        public class Ubicacion
        {
            public string IdUbi { get; set; }
            public string UbiToken { get; set; }
            public string ComToken { get; set; }
            public string UbiNombre { get; set; }
            public string UbiDescion { get; set; }
            public string UbiCom { get; set; }
            public string UbiEstado { get; set; }
        }

        public class ContratoDotacion
        {
            public string IniToken { get; set; }
            public string PidArea { get; set; }
            public string PidCodigoIniciativa { get; set; }
            public string PidNombreProyecto { get; set; }
            public string DotAnn { get; set; }
            public string DotSitProyecto { get; set; }
            public string DotSitFaena { get; set; }
            public string DotDepto { get; set; }
            public string DotNumContrato { get; set; }
            public string DotNombEECC { get; set; }
            public string DotServicio { get; set; }
            public string DotSubContrato { get; set; }
            public string DotCodCentro { get; set; }
            public string DotTurno { get; set; }
            public string DotHoteleria { get; set; }
            public string DotAlimentacion { get; set; }
            public string DotUbicacion { get; set; }
            public string DotClasificacion { get; set; }
            public string DotTipoEECC { get; set; }
            public string DotTotalDotacion { get; set; }
            public string DotEne { get; set; }
            public string DotFeb { get; set; }
            public string DotMar { get; set; }
            public string DotAbr { get; set; }
            public string DotMay { get; set; }
            public string DotJun { get; set; }
            public string DotJul { get; set; }
            public string DotAgo { get; set; }
            public string DotSep { get; set; }
            public string DotOct { get; set; }
            public string DotNov { get; set; }
            public string DotDic { get; set; }
        }

        public class ModificarDotacion
        {
            public string IdDot { get; set; }
            public string DotToken { get; set; }
            public string IniToken { get; set; }
            public string PidArea { get; set; }
            public string PidCodigoIniciativa { get; set; }
            public string PidNombreProyecto { get; set; }
            public string DotAnn { get; set; }
            public string DotSitProyecto { get; set; }
            public string DotSitFaena { get; set; }
            public string DotDepto { get; set; }
            public string DotNumContrato { get; set; }
            public string DotNombEECC { get; set; }
            public string DotServicio { get; set; }
            public string DotSubContrato { get; set; }
            public string DotCodCentro { get; set; }
            public string DotTurno { get; set; }
            public string DotHoteleria { get; set; }
            public string DotAlimentacion { get; set; }
            public string DotUbicacion { get; set; }
            public string DotClasificacion { get; set; }
            public string DotTipoEECC { get; set; }
            public string DotTotalDotacion { get; set; }
            public string DotEne { get; set; }
            public string DotFeb { get; set; }
            public string DotMar { get; set; }
            public string DotAbr { get; set; }
            public string DotMay { get; set; }
            public string DotJun { get; set; }
            public string DotJul { get; set; }
            public string DotAgo { get; set; }
            public string DotSep { get; set; }
            public string DotOct { get; set; }
            public string DotNov { get; set; }
            public string DotDic { get; set; }
        }

        public class DetalleContratosDotacion
        {
            public string Opcions { get; set; }
            public string IdDot { get; set; }
            public string DotToken { get; set; }
            public string IniToken { get; set; }
            public string PidArea { get; set; }
            public string PidCodigoIniciativa { get; set; }
            public string PidNombreProyecto { get; set; }
            public string DotSitProyecto { get; set; }
            public string DotSitFaena { get; set; }
            public string DotDepto { get; set; }
            public string DotNumContrato { get; set; }
            public string DotNombEECC { get; set; }
            public string DotServicio { get; set; }
            public string DotSubContrato { get; set; }
            public string DotCodCentro { get; set; }
            public string DotTurno { get; set; }
            public string DotHoteleria { get; set; }
            public string DotAlimentacion { get; set; }
            public string DotUbicacion { get; set; }
            public string DotClasificacion { get; set; }
            public string DotTipoEECC { get; set; }
            public string DotFecha { get; set; }
            public string DotEstado { get; set; }
        }
    }
}
