using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Planificacion
{
    public class Identificacion
    {
        public class Gerente
        {
            public int IdGerente { get; set; }
            public int? IdGerencia { get; set; }
            public string GteToken { get; set; }
            public string GteNombre { get; set; }
            public string GteDescripcion { get; set; }
            public int? GteEstado { get; set; }
        }

        public class Gerencia
        {
            public int IdGerencia { get; set; }
            public string GerToken { get; set; }
            public string GerNombre { get; set; }
            public string GerDescripcion { get; set; }
            public int? GerEstado { get; set; }
        }

        public class InfoGerencia
        {
            public string GteNombre { get; set; }
            public string GteToken { get; set; }
            public int IdGerente { get; set; }
            public string GerNombre { get; set; }
            public string GerToken { get; set; }
            public int IdGerencia { get; set; }

        }

        public class Superintendente
        {
            public int IdInt { get; set; }
            public int? IdSuper { get; set; }
            public string IntToken { get; set; }
            public string IntNombre { get; set; }
            public string IntDescripcion { get; set; }
            public int? IntEstado { get; set; }
        }


        public class Superintendencia
        {
            public int IdSuper { get; set; }
            public int? CodigoSuper { get; set; }
            public string SuperToken { get; set; }
            public string SuperNombre { get; set; }
            public int? IdGerencia { get; set; }
            public string SuperDescripcion { get; set; }
            public int? SuperEstado { get; set; }
        }

        public class InfoSuperIntendencia
        {
            public int IdInt { get; set; }
            public string IntToken { get; set; }
            public string IntNombre { get; set; }
            public int? IdSuper { get; set; }
            public int? CodigoSuper { get; set; }
            public string SuperToken { get; set; }
            public string SuperNombre { get; set; }
            public int IdGerencia { get; set; }
        }

        public class Adjunto
        {
            public string ParToken { get; set; }
            public string IniToken { get; set; }
            public string ParPaso { get; set; }
            public string ParNombre { get; set; }
            public string ParNombreFinal { get; set; }
            public string ShareFile { get; set; }
            public string PathDirectory { get; set; }
            public string UrlAzure { get; set; }
        }

        public class DocumentoCategoria
        {

            public int IdDoc { get; set; }
            public string DocToken { get; set; }
            public string DocCatToken { get; set; }
            public string DocCatNombre { get; set; }
            public string DocTipo { get; set; }
            public int DocTam { get; set; }
            public string DocExt { get; set; }
            public string DocNombre { get; set; }
            public string DocFecha { get; set; }
            public int DocVisible { get; set; }
            public int DocEstado { get; set; }

        }

        public class InfoEncargado
        {
            public int IdEnc { get; set; }
            public string EncToken { get; set; }
            public string EncNombre { get; set; }
            public int IdGerencia { get; set; }
            public string GerNombre { get; set; }
            public string GerToken { get; set; }
        }

        public class Proceso
        {
            public int IdProc { get; set; }
            public string ProcToken { get; set; }
            public string ProcNombre { get; set; }
            public string ProcAcronimo { get; set; }
            public string ProcDescripcion { get; set; }
            public int ProcEstado { get; set; }
        }

        public class Area
        {
            public int IdArea { get; set; }
            public string AreaToken { get; set; }
            public string AreaNombre { get; set; }
            public string AreaAcronimo { get; set; }
            public string AreaDescripcion { get; set; }
            public int AreaEstado { get; set; }
        }

        public class Compania
        {
            public int IdCompania { get; set; }
            public string ComToken { get; set; }
            public string ComCodigo { get; set; }
            public string ComAcronimo { get; set; }
            public string ComNombre { get; set; }
            public string ComDescripcion { get; set; }
            public int ComEstado { get; set; }
        }

        public class Etapa
        {
            public int IdNI { get; set; }
            public string NIToken { get; set; }
            public string NINombre { get; set; }
            public string NIAcronimo { get; set; }
            public string NIDescripcion { get; set; }
            public int NIEstado { get; set; }

        }

        public class IdentificacionIniciativa
        {
            public string PidToken { get; set; }
            public string PidTipoIniciativa { get; set; }
            public string PidTipoEjercicio { get; set; }
            public int PidPeriodo { get; set; }
            public string PidUsuario { get; set; }
            public string PidRol { get; set; }
            public string PidProceso { get; set; }
            public string PidObjeto { get; set; }
            public string PidArea { get; set; }
            public string PidCompania { get; set; }
            public string PidEtapa { get; set; }
            public string PidNombreProyecto { get; set; }
            public string PidNombreProyectoAlias { get; set; }
            public string PidCodigoIniciativa { get; set; }
            public string PidCodigoProyecto { get; set; }
            public string PidGerenciaInversion { get; set; }
            public string PidGerenteInversion { get; set; }
            public string PidRequiere { get; set; }
            public string PidGerenciaEjecucion { get; set; }
            public string PidGerenteEjecucion { get; set; }
            public string PidSuperintendencia { get; set; }
            public string PidSuperintendente { get; set; }
            public string PidGerenciaControl { get; set; }
            public string PidGerenteControl { get; set; }
            public string PidEncargadoControl { get; set; }
        }

        public class IdentificacionIniciativaValidacion
        {
            public string PidEtapa { get; set; }
            public string PidRequiere { get; set; }
            public string PidGerenciaEjecucion { get; set; }
            public string PidGerenteEjecucion { get; set; }
            public string PidSuperintendencia { get; set; }
            public string PidSuperintendente { get; set; }
            public string PidEncargadoControl { get; set; }
            public string PidToken { get; set; }
        }

        public class IdentificacionIciativaResponse
        {
            public int IdPid { get; set; }
            public string Estado { get; set; }
        }

        public class AprobacionRechazo
        {
            public string PidToken { get; set; }
            public string PidUsuario { get; set; }
            public string PidRol { get; set; }
            public string Comentario { get; set; }
        }

        public class SharedData
        {
            public string UserOwner { get; set; }
            public string IdAcc { get; set; }
        }

        public class AsignarmeIniciativa
        {
            public string PidToken { get; set; }
            public string PidRol { get; set; }
            public string PidUsuario { get; set; }
            public string IdEstadoFinal { get; set; }
            public string Forzar { get; set; }
        }

        public class MatrizRiesgo
        {
            public int IdMatrizRiesgo { get; set; }
            public string MatrizRiesgoToken { get; set; }
            public string MatrizRiesgoNombre { get; set; }
            public int MatrizRiesgoImpacto { get; set; }
            public int MatrizRiesgoProbabilidad { get; set; }
            public int MatrizOrden { get; set; }
            public int MatrizRiesgoEstado { get; set; }
        }

    }
}
