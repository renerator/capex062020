using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapexInfraestructure.Bll.Business.Planificacion;

namespace CapexInfraestructure.Bll.Factory
{
    public class PlanificacionFactory
    {   /* ------------------------------------------------------------------------------------
         * 
         * PMO360
         * Av. Nueva Tajamar 481 Of 1403 - Vitacura, Santiago
         * http://www.pmo360.cl
         * 
         * -----------------------------------------------------------------------------------
         * 
         * CLIENTE          : AMSA - ANTOFAGASTA MINERALS
         * PRODUCTO         : CAPEX
         * RESPONABILIDAD   : PROVEER FABRICA DE OBJETOS REUTILIZABLES 
         * TIPO             : LOGICA DE NEGOCIO
         * DESARROLLADO POR : PMO360
         * FECHA            : 2018
         * VERSION          : 0.0.1
         * PROPOSITO        : ADMINISTRAR LOGICA DE NEGOCIO,CREACION DE OBJETOS
         * 
         * 
         */
        public enum tipo
        {
            //IDENTIFICACION
            ListarProcesos,
            ListarAreas,
            ListarCompanias,
            ListarEtapas,
            ListarGerencias,
            ListarSuperintendencias,
            ObtenerGerente,
            ObtenerIntendente,
            ObtenerEncargado,
            GuardarIdentificacion,
            ActualizarIdentificacion,
            //CATEGORIZACION
            ListarCategorias,
            ListarNivelIngenieria,
            ListarClasificacionSSO,
            ListarEstandarSeguridad,
             ObtenerTokenCompania,
            ActualizarEtapa,
            GuardarCategorizacion,
            ActualizarCategorizacion,
            //PRESUPUESTO
            Importar,
            Poblar,
            //DOTACION
            ListarContratosDotacion,
            ListarDepartamentos,
            ListarTurnos,
            ListarUbicaciones,
            ListarTipoEECC,
            ListarClasificacion,
            GuardarContratoDotacion,
            GuardarPeriodosDotacion,
            EliminarContratoDotacion,
            ActualizarContratoDotacion,
            //DESCRIPCION
            GuardarDescripcionDetallada,
            ActualizarDescripcionDetallada,
            //EVALUACION ECONOMICA
            GuardarEvaluacionEconomica,
            ActualizarEvaluacionEconomica,
            //EVALUACION RIESGO
            GuardarEvaluacionRiesgo,
            ActualizarEvaluacionRiesgo,
            //HITOS
            Detallar,
            Resumir,
            PoblarVistaHitos,
            GuardarHito,
            ActualizarHito,
            EnviarIniciativa,
            //ADJUNTOS
            VerAdjunto,
            EliminarAdjunto,
            //REGISTRAR ARCHIVOS
            RegistrarArchivo

        }

        public IPlanificacion delega(tipo T)
        {
            switch (T)
            {
                //IDENTIFICACION
                case tipo.ListarProcesos            : return new Business.Planificacion.Planificacion();
                case tipo.ListarAreas               : return new Business.Planificacion.Planificacion();
                case tipo.ListarCompanias           : return new Business.Planificacion.Planificacion();
                case tipo.ListarEtapas              : return new Business.Planificacion.Planificacion();
                case tipo.ListarGerencias           : return new Business.Planificacion.Planificacion();
                case tipo.ListarSuperintendencias   : return new Business.Planificacion.Planificacion();
                case tipo.ObtenerGerente            : return new Business.Planificacion.Planificacion();
                case tipo.ObtenerIntendente         : return new Business.Planificacion.Planificacion();
                case tipo.ObtenerEncargado          : return new Business.Planificacion.Planificacion();
                case tipo.GuardarIdentificacion     : return new Business.Planificacion.Planificacion();
                case tipo.ActualizarIdentificacion: return new Business.Planificacion.Planificacion();
                //CATEGORIZACION
                case tipo.ListarCategorias          : return new Business.Planificacion.Planificacion();
                case tipo.ListarNivelIngenieria     : return new Business.Planificacion.Planificacion();
                case tipo.ListarClasificacionSSO    : return new Business.Planificacion.Planificacion();
                case tipo.ActualizarEtapa           : return new Business.Planificacion.Planificacion();
                case tipo.GuardarCategorizacion     : return new Business.Planificacion.Planificacion();
                case tipo.ActualizarCategorizacion: return new Business.Planificacion.Planificacion();
                //IMPORTAR
                case tipo.Importar                  : return new Business.Planificacion.Planificacion();
                case tipo.Poblar                    : return new Business.Planificacion.Planificacion();
                case tipo.Detallar                  : return new Business.Planificacion.Planificacion();
                case tipo.Resumir                   : return new Business.Planificacion.Planificacion();
                //DOTACION
                case tipo.ListarContratosDotacion   : return new Business.Planificacion.Planificacion();
                case tipo.ListarDepartamentos       : return new Business.Planificacion.Planificacion();
                case tipo.ListarTurnos              : return new Business.Planificacion.Planificacion();
                case tipo.ListarUbicaciones         : return new Business.Planificacion.Planificacion();
                case tipo.ListarTipoEECC            : return new Business.Planificacion.Planificacion();
                case tipo.ListarClasificacion       : return new Business.Planificacion.Planificacion();
                case tipo.GuardarContratoDotacion   : return new Business.Planificacion.Planificacion();
                case tipo.GuardarPeriodosDotacion   : return new Business.Planificacion.Planificacion();
                case tipo.EliminarContratoDotacion  : return new Business.Planificacion.Planificacion();
                case tipo.ActualizarContratoDotacion: return new Business.Planificacion.Planificacion();
                //DESCRIPCION DETALLADA
                case tipo.GuardarDescripcionDetallada: return new Business.Planificacion.Planificacion();
                case tipo.ActualizarDescripcionDetallada: return new Business.Planificacion.Planificacion();
                //EVALUACION ECONOMICA
                case tipo.GuardarEvaluacionEconomica: return new Business.Planificacion.Planificacion();
                case tipo.ActualizarEvaluacionEconomica: return new Business.Planificacion.Planificacion();
                //EVALUACION RIESGO
                case tipo.GuardarEvaluacionRiesgo   : return new Business.Planificacion.Planificacion();
                case tipo.ActualizarEvaluacionRiesgo: return new Business.Planificacion.Planificacion();
                //HITO
                case tipo.GuardarHito               : return new Business.Planificacion.Planificacion();
                case tipo.ActualizarHito            : return new Business.Planificacion.Planificacion();
                case tipo.EnviarIniciativa          : return new Business.Planificacion.Planificacion(); 
                case tipo.PoblarVistaHitos          : return new Business.Planificacion.Planificacion();
                //ADJUNTO
                case tipo.VerAdjunto                : return new Business.Planificacion.Planificacion();
                case tipo.EliminarAdjunto           :return new Business.Planificacion.Planificacion();
                    
                //REGISTRAR ARCHIVOS
                case tipo.RegistrarArchivo          : return new Business.Planificacion.Planificacion();
                default: return new Business.Planificacion.Planificacion();
            }
        }
    }
}
