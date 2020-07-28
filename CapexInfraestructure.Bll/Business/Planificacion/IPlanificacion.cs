using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapexInfraestructure.Bll.Entities.Planificacion;

namespace CapexInfraestructure.Bll.Business.Planificacion
{
    public interface IPlanificacion
    {
        /* ------------------------------------------------------------------------------------
        * 
        * PMO360
        * Av. Nueva Tajamar 481 Of 1403 - Vitacura, Santiago
        * http://www.pmo360.cl
        * 
        * -----------------------------------------------------------------------------------
        * 
        * CLIENTE          : AMSA - ANTOFAGASTA MINERALS
        * PRODUCTO         : CAPEX
        * RESPONABILIDAD   : PROVEER METODOS INTERFAZ DE IMPLEMENTACION
        * TIPO             : LOGICA DE NEGOCIO
        * DESARROLLADO POR : PMO360
        * FECHA            : 2018
        * VERSION          : 0.0.1
        * PROPOSITO        : SEGURIDAD 
        * 
        * 
        */

        // --------------------------- IDENTIFICACION ---------------------------- //
        List<Identificacion.Proceso> ListarProcesos();
        List<Identificacion.Area> ListarAreas();
        List<Identificacion.Compania> ListarCompanias();
        List<Identificacion.Etapa> ListarEtapas();
        List<Identificacion.Gerencia> ListarGerencias();
        List<Identificacion.Superintendencia> ListarSuperintendencias();
        List<Identificacion.Superintendencia> ListarSuperintendenciasPorGerencia(string GerToken);
        Identificacion.InfoGerencia ObtenerGerente(string Token);
        Identificacion.InfoEncargado ObtenerEncargado(int IdGerencia, int CodigoSuper);
        Identificacion.InfoSuperIntendencia ObtenerIntendente(string Token);
        string AprobarVisacion(Identificacion.AprobacionRechazo DatosIniciativa);
        string RechazarVisacion(Identificacion.AprobacionRechazo DatosIniciativa);
        string AprobarCE(Identificacion.AprobacionRechazo DatosIniciativa);
        string RechazarCE(Identificacion.AprobacionRechazo DatosIniciativa);
        string AprobarAMSA(Identificacion.AprobacionRechazo DatosIniciativa);
        string RechazarAMSA(Identificacion.AprobacionRechazo DatosIniciativa);
        string ObtenerUltimaObservacion(string PidToken, string accion);
        string AsignarmeIniciativa(Identificacion.AsignarmeIniciativa Datos);
        string GuardarIdentificacion(Identificacion.IdentificacionIniciativa DatosIdentificacion);
        string ActualizarIdentificacion(Identificacion.IdentificacionIniciativa DatosIdentificacion);
        string ActualizarIdentificacionCategorizacion(Identificacion.IdentificacionIniciativa DatosIdentificacion);
        string ValidarIdentificacion(Identificacion.IdentificacionIniciativa DatosIdentificacion);
        string SeleccionarEstadoProyecto(string PidToken);
        string SeleccionarCodigoProyecto(string PidToken);
        // --------------------------- CATEGORIZACION ---------------------------- //
        List<Categorizacion.Categoria> ListarCategorias();
        List<Categorizacion.NivelIngenieria> ListarNivelIngenieria();
        List<Categorizacion.NivelIngenieria> ListarNivelIngenieriaNoRequiere(int IdNI);
        List<Categorizacion.ClasificacionSSO> ListarClasificacionSSO();
        List<Categorizacion.EstandarSeguridad> ListarEstandarSeguridad(string EssComToken, string EssCSToken);
        string ObtenerTokenCompania(string Tipo, string Valor);
        string ActualizarEtapa(string token, string etapa);
        string GuardarCategorizacion(Categorizacion.DatosCategorizacion Datos);
        string ActualizarCategorizacion(Categorizacion.DatosCategorizacion Datos);

        // --------------------------- PRESUPUESTO ---------------------------- //
        string ImportarTemplate(string token, string usuario, string archivo);
        string ImportarTemplateCasoBase(string token, string usuario, string archivo);
        string PoblarVistaPresupuestoFinanciero(string token);
        string PoblarVistaPresupuestoFisico(string token);
        string PoblarVistaPresupuestoFinancieroCasoBase(string token);
        string PoblarVistaPresupuestoFisicoCasoBase(string token);

        // --------------------------- DOTACION ---------------------------- //
        List<Dotacion.DetalleContratosDotacion> ListarContratosDotacion(string Token);
        List<Dotacion.DepartamentoDotacion> ListarDepartamentos();
        List<Dotacion.Turnos> ListarTurnos();
        List<Dotacion.Ubicacion> ListarUbicaciones();
        List<Dotacion.DotacionEECC> ListarTipoEECC();
        List<Dotacion.DotacionClasificacion> ListarClasificacion();
        string GuardarContratoDotacion(Dotacion.ContratoDotacion DatosContratoDotacion);
        string ActualizarContratoDotacion(Dotacion.ContratoDotacion DatosContratoDotacion);
        string GuardarPeriodosDotacion(string Token, string DatosPeriodoDotacion);
        string EliminarContratoDotacion(string Token);

        // --------------------------- DESCRIPCION DETALLADA ---------------------------- //
        string GuardarDescripcionDetallada(Descripcion.DescripcionDetallada DatosDescripcion);
        string ActualizarDescripcionDetallada(Descripcion.DescripcionDetallada DatosDescripcion);

        // --------------------------- EVALUACION ECONOMICA ---------------------------- //
        string GuardarEvaluacionEconomica(EvaluacionEconomica.GuardarEvaluacion DatosEvaluacionEconomica);
        string ActualizarEvaluacionEconomica(EvaluacionEconomica.GuardarEvaluacion DatosEvaluacionEconomica);
        // --------------------------- EVALUACION DE RIESGO ---------------------------- //
        string GuardarEvaluacionRiesgo(EvaluacionRiesgo.GuardarEvaluacion DatosEvaluacionRiesgo);
        string ActualizarEvaluacionRiesgo(EvaluacionRiesgo.GuardarEvaluacion DatosEvaluacionRiesgo);
        // --------------------------- HITOS  ---------------------------- //
        //string PoblarVistaHitos(string token);
        List<Hito.HitoResumen> PoblarVistaHitosResumen(string token);
        List<Hito.HitoDetalle> PoblarVistaHitosDetalle(string token);
        string PoblarVistaHitos(string token);
        string GuardarHito(Hito.HitoGuardar Datos);
        string ActualizarHito(Hito.HitoGuardar Datos);
        string EnviarIniciativa(string IniToken, string WrfUsuario, string WrfObservacion, string Rol);

        // --------------------------- ADJUNTOS  ---------------------------- //
        string VerAdjuntos(string token);
        string EliminarAdjunto(string token);
        string EliminarAdjuntoVigente(string token, string usuario);
        string EliminarAdjuntoVigenteConEvaluacionEconomica(string IniToken, string ParToken, string usuario);
        string EliminarAdjuntoVigenteConEvaluacionRiesgo(string IniToken, string ParToken, string usuario);
        Identificacion.Adjunto SeleccionarExcelTemplate(string token);
        Identificacion.Adjunto SeleccionarExcelTemplatePeriodo(string tipoIniciativaSeleccionado, string periodo);
        Identificacion.Adjunto SeleccionarAdjunto(string token);
        Identificacion.Adjunto SeleccionarAdjuntoPorTokenYPaso(string IniToken, string ParPaso);
        Identificacion.Adjunto SeleccionarOtroAdjuntoPorTokenYPaso(string IniToken, string ParToken, string ParPaso);
        Identificacion.DocumentoCategoria SeleccionarDocumentoBiblioteca(string token);
        // --------------------------- GLOBALES ---------------------------- //
        string RegistrarArchivo(string IniToken, string ParUsuario, string ParNombre, string ParPaso, string ParCaso);

        List<Identificacion.MatrizRiesgo> ListarMatrizRiesgo();
        string obtenerFechaBloqueo(String TipoIniciativa);
    }
}
