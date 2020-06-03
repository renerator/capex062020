using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.Common;
using Dapper;
using System.Text;
using System.Threading.Tasks;
using CapexInfraestructure.Bll.Entities.Planificacion;
using CapexInfraestructure.Utilities;
using ClosedXML.Excel;
using System.Globalization;

namespace CapexInfraestructure.Bll.Business.Planificacion
{
    public class Planificacion : IPlanificacion
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
         * RESPONABILIDAD   : PROVEER OPERACIONES Y LOGICA DE NEGOCIO PARA EL MODULO DE EJERCICIO DE PLANIFICACION
         * TIPO             : LOGICA DE NEGOCIO
         * DESARROLLADO POR : PMO360
         * FECHA            : 2018
         * VERSION          : 0.0.1
         * PROPOSITO        : WRAPPER DE OPERACIONES A LA BASE DE DATOS /REPOSITORIO
         * 
         * 
         */
        #region "PROPIEDADES"
        public string ExceptionResult { get; set; }
        public string AppModule { get; set; }
        #endregion

        #region "GLOBALS"
        public SqlConnection ORM;
        #endregion

        #region "CONSTRUCTOR"
        public Planificacion()
        {
            AppModule = "Planificación";
            ORM = Utils.Conectar();
        }
        #endregion

        #region "METODOS IDENTIFICACION"

        /// <remark>
        ///     
        ///     ----------------------------------------------------------------------------
        ///     GRUPO DE METODOS "IDENTIFICACION"
        ///     VERSION     0.0.1
        ///     ----------------------------------------------------------------------------
        ///     PROPOSITO
        ///     ----------------------------------------------------------------------------
        ///     IMPLEMENTAR ENTRADA Y SALIDA DE DATOS, ASI COMO OPERACIONES POR MEDIO
        ///     DE PATRON CREACIONAL FACTORY QUE ES PROVISTO POR CAPA DDD DE INFRAESTRUCTURA
        ///     
        /// </remark>

        /// <summary>
        /// METODO BO "ListarProcesos"
        /// </summary>
        /// <returns></returns>
        public List<Identificacion.Proceso> ListarProcesos()
        {
            try
            {
                return ORM.Query<Identificacion.Proceso>("CAPEX_SEL_PROCESOS", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ListarProcesos, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Identificacion.Area> ListarAreas()
        {
            try
            {
                return ORM.Query<Identificacion.Area>("CAPEX_SEL_AREAS", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ListarAreas, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Identificacion.Compania> ListarCompanias()
        {
            try
            {
                return ORM.Query<Identificacion.Compania>("CAPEX_SEL_COMPANIAS", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ListarCompania, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Identificacion.Etapa> ListarEtapas()
        {
            try
            {
                return ORM.Query<Identificacion.Etapa>("CAPEX_SEL_ETAPAS", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ListarEtapas, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Identificacion.Gerencia> ListarGerencias()
        {
            try
            {
                return ORM.Query<Identificacion.Gerencia>("CAPEX_SEL_GERENCIAS", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ListarGerencias, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Identificacion.Superintendencia> ListarSuperintendencias()
        {
            try
            {
                return ORM.Query<Identificacion.Superintendencia>("CAPEX_SEL_SUPERINTENDENCIAS", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ListarSuperintendencias, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Identificacion.Superintendencia> ListarSuperintendenciasPorGerencia(string GerToken)
        {
            try
            {
                return ORM.Query<Identificacion.Superintendencia>("CAPEX_SEL_SUPERINTENDENCIAS_POR_GERENCIA", new { GerToken }, commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ListarSuperintendenciasPorGerencia, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        public Identificacion.InfoGerencia ObtenerGerente(string Token)
        {
            try
            {
                return ORM.Query<Identificacion.InfoGerencia>("CAPEX_SEL_INFOGERENCIA", new { Token }, commandType: CommandType.StoredProcedure).SingleOrDefault();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ObtenerGerente, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        public Identificacion.InfoEncargado ObtenerEncargado(int IdGerencia, int CodigoSuper)
        {
            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    return SqlMapper.Query<Identificacion.InfoEncargado>(objConnection, "CAPEX_SEL_INFOENCARGADO", new { IdGerencia, CodigoSuper }, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                catch (Exception err)
                {
                    ExceptionResult = AppModule + "ObtenerEncargado, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    Utils.LogError(ExceptionResult);

                    return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        public Identificacion.InfoSuperIntendencia ObtenerIntendente(string Token)
        {
            try
            {
                return ORM.Query<Identificacion.InfoSuperIntendencia>("CAPEX_SEL_INFOSUPERINTEN", new { Token }, commandType: CommandType.StoredProcedure).SingleOrDefault();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ObtenerIntendente, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        public string AprobarVisacion(Identificacion.AprobacionRechazo DatosIniciativa)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", DatosIniciativa.PidToken);
                parametos.Add("PidUsuario", DatosIniciativa.PidUsuario);
                parametos.Add("PidRol", DatosIniciativa.PidRol);
                parametos.Add("ErrorCodeOutput", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 200);
                ORM.Query("CAPEX_APROBAR_VISACION", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                var ErrorCodeSP = parametos.Get<string>("ErrorCodeOutput");
                if (!string.IsNullOrEmpty(ErrorCodeSP) && !string.IsNullOrEmpty(ErrorCodeSP.Trim()) && ErrorCodeSP.StartsWith("0"))
                {
                    return "Aprobado|" + ErrorCodeSP.Trim();
                }
                else
                {
                    return "Error|" + ((!string.IsNullOrEmpty(ErrorCodeSP) && !string.IsNullOrEmpty(ErrorCodeSP.Trim()) ? ErrorCodeSP.Trim() : "1|ERROR AL APROBAR VISACION"));
                }
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "AprobarVisacion, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        public string RechazarVisacion(Identificacion.AprobacionRechazo DatosIniciativa)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", DatosIniciativa.PidToken);
                parametos.Add("PidUsuario", DatosIniciativa.PidUsuario);
                parametos.Add("PidRol", DatosIniciativa.PidRol);
                parametos.Add("Comentario", (!string.IsNullOrEmpty(DatosIniciativa.Comentario) ? ((DatosIniciativa.Comentario.Trim().Length > 400) ? DatosIniciativa.Comentario.Substring(0, 400) : DatosIniciativa.Comentario.Trim()) : ""));
                parametos.Add("ErrorCodeOutput", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 200);
                ORM.Query("CAPEX_RECHAZAR_VISACION", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                var ErrorCodeSP = parametos.Get<string>("ErrorCodeOutput");
                if (!string.IsNullOrEmpty(ErrorCodeSP) && !string.IsNullOrEmpty(ErrorCodeSP.Trim()) && ErrorCodeSP.StartsWith("0"))
                {
                    return "Rechazado|" + ErrorCodeSP.Trim();
                }
                else
                {
                    return "Error|" + ((!string.IsNullOrEmpty(ErrorCodeSP) && !string.IsNullOrEmpty(ErrorCodeSP.Trim()) ? ErrorCodeSP.Trim() : "1|ERROR AL RECHAZAR VISACION"));
                }
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "RechazarVisacion, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        public string AprobarCE(Identificacion.AprobacionRechazo DatosIniciativa)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", DatosIniciativa.PidToken);
                parametos.Add("PidUsuario", DatosIniciativa.PidUsuario);
                parametos.Add("PidRol", DatosIniciativa.PidRol);
                parametos.Add("ErrorCodeOutput", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 200);
                ORM.Query("CAPEX_APROBAR_CE", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                var ErrorCodeSP = parametos.Get<string>("ErrorCodeOutput");
                if (!string.IsNullOrEmpty(ErrorCodeSP) && !string.IsNullOrEmpty(ErrorCodeSP.Trim()) && ErrorCodeSP.StartsWith("0"))
                {
                    return "Aprobado|" + ErrorCodeSP.Trim();
                }
                else
                {
                    return "Error|" + ((!string.IsNullOrEmpty(ErrorCodeSP) && !string.IsNullOrEmpty(ErrorCodeSP.Trim()) ? ErrorCodeSP.Trim() : "1|ERROR AL APROBAR COMITE EJECUTIVO"));
                }
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "AprobarCE, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        public string RechazarCE(Identificacion.AprobacionRechazo DatosIniciativa)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", DatosIniciativa.PidToken);
                parametos.Add("PidUsuario", DatosIniciativa.PidUsuario);
                parametos.Add("PidRol", DatosIniciativa.PidRol);
                parametos.Add("Comentario", (!string.IsNullOrEmpty(DatosIniciativa.Comentario) ? ((DatosIniciativa.Comentario.Trim().Length > 400) ? DatosIniciativa.Comentario.Substring(0, 400) : DatosIniciativa.Comentario.Trim()) : ""));
                parametos.Add("ErrorCodeOutput", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 200);
                ORM.Query("CAPEX_RECHAZAR_CE", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                var ErrorCodeSP = parametos.Get<string>("ErrorCodeOutput");
                if (!string.IsNullOrEmpty(ErrorCodeSP) && !string.IsNullOrEmpty(ErrorCodeSP.Trim()) && ErrorCodeSP.StartsWith("0"))
                {
                    return "Rechazado|" + ErrorCodeSP.Trim();
                }
                else
                {
                    return "Error|" + ((!string.IsNullOrEmpty(ErrorCodeSP) && !string.IsNullOrEmpty(ErrorCodeSP.Trim()) ? ErrorCodeSP.Trim() : "1|ERROR AL RECHAZAR COMITE EJECUTIVO"));
                }
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "RechazarCE, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        public string AprobarAMSA(Identificacion.AprobacionRechazo DatosIniciativa)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", DatosIniciativa.PidToken);
                parametos.Add("PidUsuario", DatosIniciativa.PidUsuario);
                parametos.Add("PidRol", DatosIniciativa.PidRol);
                parametos.Add("ErrorCodeOutput", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 200);
                ORM.Query("CAPEX_APROBAR_AMSA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                var ErrorCodeSP = parametos.Get<string>("ErrorCodeOutput");
                if (!string.IsNullOrEmpty(ErrorCodeSP) && !string.IsNullOrEmpty(ErrorCodeSP.Trim()) && ErrorCodeSP.StartsWith("0"))
                {
                    return "Aprobado|" + ErrorCodeSP.Trim();
                }
                else
                {
                    return "Error|" + ((!string.IsNullOrEmpty(ErrorCodeSP) && !string.IsNullOrEmpty(ErrorCodeSP.Trim()) ? ErrorCodeSP.Trim() : "1|ERROR AL APROBAR COMITE EJECUTIVO"));
                }
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "AprobarCE, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        public string RechazarAMSA(Identificacion.AprobacionRechazo DatosIniciativa)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", DatosIniciativa.PidToken);
                parametos.Add("PidUsuario", DatosIniciativa.PidUsuario);
                parametos.Add("PidRol", DatosIniciativa.PidRol);
                parametos.Add("Comentario", (!string.IsNullOrEmpty(DatosIniciativa.Comentario) ? ((DatosIniciativa.Comentario.Trim().Length > 400) ? DatosIniciativa.Comentario.Substring(0, 400) : DatosIniciativa.Comentario.Trim()) : ""));
                parametos.Add("ErrorCodeOutput", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 200);
                ORM.Query("CAPEX_RECHAZAR_AMSA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                var ErrorCodeSP = parametos.Get<string>("ErrorCodeOutput");
                if (!string.IsNullOrEmpty(ErrorCodeSP) && !string.IsNullOrEmpty(ErrorCodeSP.Trim()) && ErrorCodeSP.StartsWith("0"))
                {
                    return "Rechazado|" + ErrorCodeSP.Trim();
                }
                else
                {
                    return "Error|" + ((!string.IsNullOrEmpty(ErrorCodeSP) && !string.IsNullOrEmpty(ErrorCodeSP.Trim()) ? ErrorCodeSP.Trim() : "1|ERROR AL RECHAZAR COMITE EJECUTIVO"));
                }
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "RechazarCE, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        public string ObtenerUltimaObservacion(string PidToken, string accion)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", PidToken);
                parametos.Add("IdAcc", (string.IsNullOrEmpty(accion) ? "0" : accion));
                parametos.Add("Comentario", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 450);
                ORM.Query("CAPEX_SEL_ULTIMO_COMENTARIO", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                var ComentarioSP = parametos.Get<string>("Comentario");
                if (!string.IsNullOrEmpty(ComentarioSP) && !string.IsNullOrEmpty(ComentarioSP.Trim()) && ComentarioSP.StartsWith("0"))
                {
                    return "Obtenido|" + ComentarioSP.Trim();
                }
                else
                {
                    return "Error|" + ((!string.IsNullOrEmpty(ComentarioSP) && !string.IsNullOrEmpty(ComentarioSP.Trim()) ? ComentarioSP.Trim() : "1|ERROR AL OBTENER COMENTARIO PARA LA INICIATIVA"));
                }
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "RechazarVisacion, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        public string AsignarmeIniciativa(Identificacion.AsignarmeIniciativa Datos)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", Datos.PidToken);
                parametos.Add("Usuario", Datos.PidUsuario);
                parametos.Add("Rol", Datos.PidRol);
                parametos.Add("Forzar", (!string.IsNullOrEmpty(Datos.Forzar) ? Datos.Forzar : "0"));
                parametos.Add("IdEstadoFinal", Datos.IdEstadoFinal);
                parametos.Add("ErrorCodeOutput", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 200);
                ORM.Query("CAPEX_UPD_ASIGNAR_INICIATIVA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                var RepuestaSP = parametos.Get<string>("ErrorCodeOutput");
                if (!string.IsNullOrEmpty(RepuestaSP) && !string.IsNullOrEmpty(RepuestaSP.Trim()) && RepuestaSP.StartsWith("0"))
                {
                    return "Asignado|" + RepuestaSP.Trim();
                }
                else
                {
                    return "Error|" + ((!string.IsNullOrEmpty(RepuestaSP) && !string.IsNullOrEmpty(RepuestaSP.Trim()) ? RepuestaSP.Trim() : "1|ERROR AL ASIGNAR LA INICIATIVA"));
                }
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "AsignarmeIniciativa, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DatosIdentificacion"></param>
        /// <returns></returns>
        public string GuardarIdentificacion(Identificacion.IdentificacionIniciativa DatosIdentificacion)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("PidTipoIniciativa", DatosIdentificacion.PidTipoIniciativa);
                parametos.Add("PidTipoEjercicio", DatosIdentificacion.PidTipoEjercicio);
                parametos.Add("PidPeriodo", DatosIdentificacion.PidPeriodo);
                parametos.Add("PidUsuario", DatosIdentificacion.PidUsuario);
                parametos.Add("PidRol", DatosIdentificacion.PidRol);
                parametos.Add("PidProceso", DatosIdentificacion.PidProceso);
                parametos.Add("PidObjeto", DatosIdentificacion.PidObjeto);
                parametos.Add("PidArea", DatosIdentificacion.PidArea);
                parametos.Add("PidCompania", DatosIdentificacion.PidCompania);
                parametos.Add("PidEtapa", DatosIdentificacion.PidEtapa);
                parametos.Add("PidNombreProyecto", DatosIdentificacion.PidNombreProyecto);
                parametos.Add("PidNombreProyectoAlias", DatosIdentificacion.PidNombreProyectoAlias);
                parametos.Add("PidCodigoIniciativa", DatosIdentificacion.PidCodigoIniciativa);
                parametos.Add("PidCodigoProyecto", DatosIdentificacion.PidCodigoProyecto);
                parametos.Add("PidGerenciaInversion", DatosIdentificacion.PidGerenciaInversion);
                parametos.Add("PidGerenteInversion", DatosIdentificacion.PidGerenteInversion);
                parametos.Add("PidRequiere", DatosIdentificacion.PidRequiere);
                parametos.Add("PidGerenciaEjecucion", DatosIdentificacion.PidGerenciaEjecucion);
                parametos.Add("PidGerenteEjecucion", DatosIdentificacion.PidGerenteEjecucion);
                parametos.Add("PidSuperintendencia", DatosIdentificacion.PidSuperintendencia);
                parametos.Add("PidSuperintendente", DatosIdentificacion.PidSuperintendente);
                parametos.Add("PidGerenciaControl", DatosIdentificacion.PidGerenciaControl);
                parametos.Add("PidGerenteControl", DatosIdentificacion.PidGerenteControl);
                parametos.Add("PidEncargadoControl", DatosIdentificacion.PidEncargadoControl);
                parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 4000);

                ORM.Query("CAPEX_INS_IDENTIFICACION_INICIATIVA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                var respuestaSP = parametos.Get<string>("Respuesta");
                if (!string.IsNullOrEmpty(respuestaSP) && !string.IsNullOrEmpty(respuestaSP.Trim()))
                {
                    return respuestaSP.Trim();
                }
                else
                {
                    return "Error|0|0|0";
                }

            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "GuardarIdentificacion, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DatosIdentificacion"></param>
        /// <returns></returns>
        public string ActualizarIdentificacion(Identificacion.IdentificacionIniciativa DatosIdentificacion)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("PidToken", DatosIdentificacion.PidToken);
                parametos.Add("PidTipoIniciativa", DatosIdentificacion.PidTipoIniciativa);
                parametos.Add("PidTipoEjercicio", DatosIdentificacion.PidTipoEjercicio);
                parametos.Add("PidPeriodo", DatosIdentificacion.PidPeriodo);
                parametos.Add("PidUsuario", DatosIdentificacion.PidUsuario);
                parametos.Add("PidProceso", DatosIdentificacion.PidProceso);
                parametos.Add("PidObjeto", DatosIdentificacion.PidObjeto);
                parametos.Add("PidArea", DatosIdentificacion.PidArea);
                parametos.Add("PidCompania", DatosIdentificacion.PidCompania);
                parametos.Add("PidEtapa", DatosIdentificacion.PidEtapa);
                parametos.Add("PidNombreProyecto", DatosIdentificacion.PidNombreProyecto);
                parametos.Add("PidNombreProyectoAlias", DatosIdentificacion.PidNombreProyectoAlias);
                parametos.Add("PidCodigoIniciativa", DatosIdentificacion.PidCodigoIniciativa);
                parametos.Add("PidCodigoProyecto", DatosIdentificacion.PidCodigoProyecto);
                parametos.Add("PidGerenciaInversion", DatosIdentificacion.PidGerenciaInversion);
                parametos.Add("PidGerenteInversion", DatosIdentificacion.PidGerenteInversion);
                parametos.Add("PidRequiere", DatosIdentificacion.PidRequiere);
                parametos.Add("PidGerenciaEjecucion", DatosIdentificacion.PidGerenciaEjecucion);
                parametos.Add("PidGerenteEjecucion", DatosIdentificacion.PidGerenteEjecucion);
                parametos.Add("PidSuperintendencia", DatosIdentificacion.PidSuperintendencia);
                parametos.Add("PidSuperintendente", DatosIdentificacion.PidSuperintendente);
                parametos.Add("PidGerenciaControl", DatosIdentificacion.PidGerenciaControl);
                parametos.Add("PidGerenteControl", DatosIdentificacion.PidGerenteControl);
                parametos.Add("PidEncargadoControl", DatosIdentificacion.PidEncargadoControl);
                parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 4000);

                ORM.Query("CAPEX_UPD_IDENTIFICACION_INICIATIVA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                var respuestaSP = parametos.Get<string>("Respuesta");
                if (!string.IsNullOrEmpty(respuestaSP) && !string.IsNullOrEmpty(respuestaSP.Trim()))
                {
                    return respuestaSP.Trim();
                }
                else
                {
                    return "Error|0|0";
                }

            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ActualizarIdentificacion, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        public string ActualizarIdentificacionCategorizacion(Identificacion.IdentificacionIniciativa DatosIdentificacion)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("PidToken", DatosIdentificacion.PidToken);
                parametos.Add("PidTipoIniciativa", DatosIdentificacion.PidTipoIniciativa);
                parametos.Add("PidTipoEjercicio", DatosIdentificacion.PidTipoEjercicio);
                parametos.Add("PidPeriodo", DatosIdentificacion.PidPeriodo);
                parametos.Add("PidUsuario", DatosIdentificacion.PidUsuario);
                parametos.Add("PidProceso", DatosIdentificacion.PidProceso);
                parametos.Add("PidObjeto", DatosIdentificacion.PidObjeto);
                parametos.Add("PidArea", DatosIdentificacion.PidArea);
                parametos.Add("PidCompania", DatosIdentificacion.PidCompania);
                parametos.Add("PidEtapa", DatosIdentificacion.PidEtapa);
                parametos.Add("PidNombreProyecto", DatosIdentificacion.PidNombreProyecto);
                parametos.Add("PidNombreProyectoAlias", DatosIdentificacion.PidNombreProyectoAlias);
                parametos.Add("PidCodigoIniciativa", DatosIdentificacion.PidCodigoIniciativa);
                parametos.Add("PidCodigoProyecto", DatosIdentificacion.PidCodigoProyecto);
                parametos.Add("PidGerenciaInversion", DatosIdentificacion.PidGerenciaInversion);
                parametos.Add("PidGerenteInversion", DatosIdentificacion.PidGerenteInversion);
                parametos.Add("PidRequiere", DatosIdentificacion.PidRequiere);
                parametos.Add("PidGerenciaEjecucion", DatosIdentificacion.PidGerenciaEjecucion);
                parametos.Add("PidGerenteEjecucion", DatosIdentificacion.PidGerenteEjecucion);
                parametos.Add("PidSuperintendencia", DatosIdentificacion.PidSuperintendencia);
                parametos.Add("PidSuperintendente", DatosIdentificacion.PidSuperintendente);
                parametos.Add("PidGerenciaControl", DatosIdentificacion.PidGerenciaControl);
                parametos.Add("PidGerenteControl", DatosIdentificacion.PidGerenteControl);
                parametos.Add("PidEncargadoControl", DatosIdentificacion.PidEncargadoControl);
                parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 4000);

                ORM.Query("CAPEX_UPD_IDENTIFICACION_INICIATIVA_CATEGORIZACION", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                var respuestaSP = parametos.Get<string>("Respuesta");
                if (!string.IsNullOrEmpty(respuestaSP) && !string.IsNullOrEmpty(respuestaSP.Trim()))
                {
                    return respuestaSP.Trim();
                }
                else
                {
                    return "Error|0|0";
                }
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ActualizarIdentificacionCategorizacion, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        public string ValidarIdentificacion(Identificacion.IdentificacionIniciativa DatosIdentificacion)
        {
            try
            {
                var resultProcedure = ORM.Query<Identificacion.IdentificacionIniciativaValidacion>("CAPEX_SEL_VALIDACION_IDENTIFICACION_INI", new { token = DatosIdentificacion.PidToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                if (resultProcedure != null)
                {
                    if (EvaluarCamposValidarIdentificacion(resultProcedure.PidEtapa, DatosIdentificacion.PidEtapa) && EvaluarCamposValidarIdentificacion(resultProcedure.PidRequiere, DatosIdentificacion.PidRequiere)
                        && EvaluarCamposValidarIdentificacion(resultProcedure.PidGerenciaEjecucion, DatosIdentificacion.PidGerenciaEjecucion) && EvaluarCamposValidarIdentificacion(resultProcedure.PidGerenteEjecucion, DatosIdentificacion.PidGerenteEjecucion)
                        && EvaluarCamposValidarIdentificacion(resultProcedure.PidSuperintendencia, DatosIdentificacion.PidSuperintendencia) && EvaluarCamposValidarIdentificacion(resultProcedure.PidSuperintendente, DatosIdentificacion.PidSuperintendente)
                        && EvaluarCamposValidarIdentificacion(resultProcedure.PidEncargadoControl, DatosIdentificacion.PidEncargadoControl))
                    {
                        return "0";
                    }
                    else
                    {
                        return "1";
                    }
                }
                return null;
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ValidarIdentificacion, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        public string SeleccionarEstadoProyecto(string PidToken)
        {
            try
            {
                var resultProcedure = ORM.Query<string>("CAPEX_SEL_CATEGORIZACION_ESTADO_PROYECTO", new { token = PidToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                if (!string.IsNullOrEmpty(resultProcedure))
                {
                    return resultProcedure;
                }
                return "";
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "SeleccionarEstadoProyecto, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return "";
            }
        }

        public string SeleccionarCodigoProyecto(string PidToken)
        {
            try
            {
                var resultProcedure = ORM.Query<string>("CAPEX_SEL_IDENTIFICACION_CODIGO_PROYECTO", new { token = PidToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                if (!string.IsNullOrEmpty(resultProcedure))
                {
                    return resultProcedure;
                }
                return "";
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "SeleccionarCodigoProyecto, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return "";
            }
        }

        private bool EvaluarCamposValidarIdentificacion(string fuente, string destino)
        {
            if (fuente == null && destino != null && destino.Trim().Length > 0)
            {
                return false;
            }
            if (destino == null && fuente != null && fuente.Trim().Length > 0)
            {
                return false;
            }
            if (fuente == null || destino == null)
            {
                if (fuente == null)
                {
                    if (destino != null && destino.Trim().Length > 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                if (destino == null)
                {
                    if (fuente != null && fuente.Trim().Length > 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            if (!fuente.ToUpper().Equals(destino.ToUpper()))
            {
                return false;
            }
            return true;
        }

        #endregion "METODOS IDENTIFICACION"

        #region "METODOS CATEGORIZACION"
        /// <remark>
        ///     
        ///     ----------------------------------------------------------------------------
        ///     GRUPO DE METODOS "CATEGORIZACION"
        ///     VERSION     0.0.1
        ///     ----------------------------------------------------------------------------
        ///     PROPOSITO
        ///     ----------------------------------------------------------------------------
        ///     IMPLEMENTAR ENTRADA Y SALIDA DE DATOS, ASI COMO OPERACIONES POR MEDIO
        ///     DE PATRON CREACIONAL FACTORY QUE ES PROVISTO POR CAPA DDD DE INFRAESTRUCTURA
        ///     
        /// </remark>


        public List<Categorizacion.Categoria> ListarCategorias()
        {
            try
            {
                return ORM.Query<Categorizacion.Categoria>("CAPEX_SEL_CATEGORIAS", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ListarCategorias, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        /// <summary>
        /// METODO PARA PROVEER DATOS DE NIVEL DE INGENIERIA
        /// </summary>
        /// <returns></returns>
        public List<Categorizacion.NivelIngenieria> ListarNivelIngenieria()
        {
            try
            {
                return ORM.Query<Categorizacion.NivelIngenieria>("CAPEX_SEL_NIVEL_INGENIERIA", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ListarNivelIngenieria, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        /// <summary>
        /// METODO PARA PROVEER DATOS DE NIVEL DE INGENIERIA NO REQUIERE
        /// </summary>
        /// <returns></returns>
        public List<Categorizacion.NivelIngenieria> ListarNivelIngenieriaNoRequiere(int IdNI)
        {
            try
            {
                return ORM.Query<Categorizacion.NivelIngenieria>("CAPEX_SEL_NIVEL_INGENIERIA_BY_ID", new { IdNI }, commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ListarNivelIngenieriaNoRequiere, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        /// <summary>
        ///  METODO PARA PROVEER DATOS PARA CLASIFICACION SSO
        /// </summary>
        /// <returns></returns>
        public List<Categorizacion.ClasificacionSSO> ListarClasificacionSSO()
        {

            try
            {
                return ORM.Query<Categorizacion.ClasificacionSSO>("CAPEX_SEL_CLASIFICACION_SSO", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ListarClasificacionSSO, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// METODO PARA RECUPERAR TOKENS
        /// </summary>
        /// <param name="Tipo"></param>
        /// <param name="Valor"></param>
        /// <returns></returns>
        public string ObtenerTokenCompania(string Tipo, string Valor)
        {
            try
            {
                var parametros = new DynamicParameters();
                parametros.Add("Tipo", "Compania");
                parametros.Add("Valor", Valor);
                parametros.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                ORM.Query("CAPEX_SEL_OBTENER_TOKEN", parametros, commandType: CommandType.StoredProcedure).SingleOrDefault();
                if (!string.IsNullOrEmpty(parametros.Get<string>("Respuesta")))
                {
                    return parametros.Get<string>("Respuesta");
                }
                else
                {
                    return "Error";
                }
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ObtenerTokenCompania, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// METODO PARA PROVEER DATOS DE ESTANDARES DE SEGURIDAD
        /// </summary>
        /// <returns></returns>
        public List<Categorizacion.EstandarSeguridad> ListarEstandarSeguridad(string EssComToken, string EssCSToken)
        {
            try
            {
                var result = ORM.Query<Categorizacion.EstandarSeguridad>("CAPEX_SEL_ESTANDAR_SEGURIDAD", new { EssComToken, EssCSToken }, commandType: CommandType.StoredProcedure).ToList();
                return result;
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ListarEstandarSeguridad, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// METODO ACTUALIZAR ETAPA POST-ALMACENAMIENTO
        /// </summary>
        /// <param name="token"></param>
        /// <param name="etapa"></param>
        /// <returns></returns>
        public string ActualizarEtapa(string token, string etapa)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("PidToken", token);
                parametos.Add("PidEtapa", etapa);
                parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                ORM.Query("CAPEX_UPD_IDENTIFICACION_ETAPA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                {
                    return parametos.Get<string>("Respuesta");
                }
                else
                {
                    return "Error";
                }
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ActualizarEtapa, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// METODO QUE PERMITE GUARDAR CATEGORIZACION
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        public string GuardarCategorizacion(Categorizacion.DatosCategorizacion Datos)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", Datos.IniToken);
                parametos.Add("IniUsuario", Datos.IniUsuario);
                parametos.Add("CatEstadoProyecto", Datos.CatEstadoProyecto);
                parametos.Add("CatCategoria", Datos.CatCategoria);
                parametos.Add("CatNivelIngenieria", Datos.CatNivelIngenieria);
                parametos.Add("CatAgrega", Datos.CatAgrega);
                parametos.Add("CatTipoCotizacion", Datos.CatTipoCotizacion);
                parametos.Add("CatClasificacionSSO", Datos.CatClasificacionSSO);
                parametos.Add("CatEstandarSeguridad", Datos.CatEstandarSeguridad);
                parametos.Add("CatClase", Datos.CatClase);
                parametos.Add("CatMacroCategoria", Datos.CatMacroCategoria);
                parametos.Add("CatAnalisis", Datos.CatAnalisis);
                parametos.Add("CatACNota1", Datos.CatACNota1);
                parametos.Add("CatACNota2", Datos.CatACNota2);
                parametos.Add("CatACNota3", Datos.CatACNota3);
                parametos.Add("CatACNota4", Datos.CatACNota4);
                parametos.Add("CatACNota5", Datos.CatACNota5);
                parametos.Add("CatACNota6", Datos.CatACNota6);
                parametos.Add("CatACTotal", Datos.CatACTotal);
                parametos.Add("CatACObs1", Datos.CatACObs1);
                parametos.Add("CatACObs2", Datos.CatACObs2);
                parametos.Add("CatACObs3", Datos.CatACObs3);
                parametos.Add("CatACObs4", Datos.CatACObs4);
                parametos.Add("CatACObs5", Datos.CatACObs5);
                parametos.Add("CatACObs6", Datos.CatACObs6);
                parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                ORM.Query("CAPEX_INS_IDENTIFICACION_CATEGORIZACION", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                {
                    return parametos.Get<string>("Respuesta");
                }
                else
                {
                    return "Error";
                }

            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "GuardarCategorizacion, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        public string ActualizarCategorizacion(Categorizacion.DatosCategorizacion Datos)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", Datos.IniToken);
                parametos.Add("IniUsuario", Datos.IniUsuario);
                parametos.Add("CatEstadoProyecto", Datos.CatEstadoProyecto);
                parametos.Add("CatCategoria", Datos.CatCategoria);
                parametos.Add("CatNivelIngenieria", Datos.CatNivelIngenieria);
                parametos.Add("CatAgrega", Datos.CatAgrega);
                parametos.Add("CatTipoCotizacion", Datos.CatTipoCotizacion);
                parametos.Add("CatClasificacionSSO", Datos.CatClasificacionSSO);
                parametos.Add("CatEstandarSeguridad", Datos.CatEstandarSeguridad);
                parametos.Add("CatClase", Datos.CatClase);
                parametos.Add("CatMacroCategoria", Datos.CatMacroCategoria);
                parametos.Add("CatAnalisis", Datos.CatAnalisis);
                parametos.Add("CatACNota1", Datos.CatACNota1);
                parametos.Add("CatACNota2", Datos.CatACNota2);
                parametos.Add("CatACNota3", Datos.CatACNota3);
                parametos.Add("CatACNota4", Datos.CatACNota4);
                parametos.Add("CatACNota5", Datos.CatACNota5);
                parametos.Add("CatACNota6", Datos.CatACNota6);
                parametos.Add("CatACTotal", Datos.CatACTotal);
                parametos.Add("CatACObs1", Datos.CatACObs1);
                parametos.Add("CatACObs2", Datos.CatACObs2);
                parametos.Add("CatACObs3", Datos.CatACObs3);
                parametos.Add("CatACObs4", Datos.CatACObs4);
                parametos.Add("CatACObs5", Datos.CatACObs5);
                parametos.Add("CatACObs6", Datos.CatACObs6);
                parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                ORM.Query("CAPEX_UPD_IDENTIFICACION_CATEGORIZACION", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                {
                    return parametos.Get<string>("Respuesta");
                }
                else
                {
                    return "Error";
                }

            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ActualizarCategorizacion, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        #endregion

        #region "METODOS PRESUPUESTO"
        /// <remark>
        ///     
        ///     ----------------------------------------------------------------------------
        ///     GRUPO DE METODOS "PRESUPUESTO"
        ///     VERSION     0.0.1
        ///     ----------------------------------------------------------------------------
        ///     PROPOSITO
        ///     ----------------------------------------------------------------------------
        ///     IMPLEMENTAR ENTRADA Y SALIDA DE DATOS, ASI COMO OPERACIONES POR MEDIO
        ///     DE PATRON CREACIONAL FACTORY QUE ES PROVISTO POR CAPA DDD DE INFRAESTRUCTURA
        ///     
        /// </remark>
        /// 


        public string ImportarTemplateCasoBase(string token, string usuario, string archivo)
        {

            /*-------------------------- CONFIGURAR --------------------------------*/
            List<String> registro = new List<String>();

            string path = ConfigurationManager.AppSettings.Get("CAPEX_IMPOR_PATH");
            var workbook = new XLWorkbook(path + token + "\\" + archivo);
            /*-------------------------- FINANCIERO --------------------------------*/
            var ws = workbook.Worksheet(2);
            /*-------------------------- ESTRUCTURAR --------------------------------*/

            for (int i = 5; i < 12; i++)
            {
                registro.Add(token);
                registro.Add(usuario);
                registro.Add(ws.Cell(i, 1).Value.ToString());

                if (!string.IsNullOrEmpty(ws.Cell(i, 2).Value.ToString()))
                {
                    decimal d01 = decimal.Parse(ws.Cell(i, 2).Value.ToString()) * 100;
                    string sd01 = d01.ToString("0.0");
                    registro.Add(sd01);
                }
                else
                {
                    registro.Add("0");
                }
                registro.Add(ws.Cell(i, 3).Value.ToString());
                registro.Add(ws.Cell(i, 4).Value.ToString());//ENE
                registro.Add(ws.Cell(i, 5).Value.ToString());
                registro.Add(ws.Cell(i, 6).Value.ToString());
                registro.Add(ws.Cell(i, 7).Value.ToString());
                registro.Add(ws.Cell(i, 8).Value.ToString());
                registro.Add(ws.Cell(i, 9).Value.ToString());
                registro.Add(ws.Cell(i, 10).Value.ToString());
                registro.Add(ws.Cell(i, 11).Value.ToString());
                registro.Add(ws.Cell(i, 12).Value.ToString());
                registro.Add(ws.Cell(i, 13).Value.ToString());
                registro.Add(ws.Cell(i, 14).Value.ToString());
                registro.Add(ws.Cell(i, 15).Value.ToString());//DIC

                registro.Add(ws.Cell(i, 16).Value.ToString());//TOTAL
                registro.Add(ws.Cell(i, 17).Value.ToString());//2021
                registro.Add(ws.Cell(i, 18).Value.ToString());//2022
                registro.Add(ws.Cell(i, 19).Value.ToString());//2023
                registro.Add(ws.Cell(i, 20).Value.ToString());//2024

                //
                // CASO BASE
                //
                //2025 - 2034

                registro.Add(ws.Cell(i, 21).Value.ToString());
                registro.Add(ws.Cell(i, 22).Value.ToString());
                registro.Add(ws.Cell(i, 23).Value.ToString());
                registro.Add(ws.Cell(i, 24).Value.ToString());
                registro.Add(ws.Cell(i, 25).Value.ToString());
                registro.Add(ws.Cell(i, 26).Value.ToString());
                registro.Add(ws.Cell(i, 27).Value.ToString());
                registro.Add(ws.Cell(i, 28).Value.ToString());
                registro.Add(ws.Cell(i, 29).Value.ToString());
                registro.Add(ws.Cell(i, 30).Value.ToString());

                //2035 - 2044
                registro.Add(ws.Cell(i, 31).Value.ToString());
                registro.Add(ws.Cell(i, 32).Value.ToString());
                registro.Add(ws.Cell(i, 33).Value.ToString());
                registro.Add(ws.Cell(i, 34).Value.ToString());
                registro.Add(ws.Cell(i, 35).Value.ToString());
                registro.Add(ws.Cell(i, 36).Value.ToString());
                registro.Add(ws.Cell(i, 37).Value.ToString());
                registro.Add(ws.Cell(i, 38).Value.ToString());
                registro.Add(ws.Cell(i, 39).Value.ToString());
                registro.Add(ws.Cell(i, 40).Value.ToString());
                //2045 - 2054
                registro.Add(ws.Cell(i, 41).Value.ToString());
                registro.Add(ws.Cell(i, 42).Value.ToString());
                registro.Add(ws.Cell(i, 43).Value.ToString());
                registro.Add(ws.Cell(i, 44).Value.ToString());
                registro.Add(ws.Cell(i, 45).Value.ToString());
                registro.Add(ws.Cell(i, 46).Value.ToString());
                registro.Add(ws.Cell(i, 47).Value.ToString());
                registro.Add(ws.Cell(i, 48).Value.ToString());
                registro.Add(ws.Cell(i, 49).Value.ToString());
                registro.Add(ws.Cell(i, 50).Value.ToString());

                //2055 - 2064
                registro.Add(ws.Cell(i, 51).Value.ToString());
                registro.Add(ws.Cell(i, 52).Value.ToString());
                registro.Add(ws.Cell(i, 53).Value.ToString());
                registro.Add(ws.Cell(i, 54).Value.ToString());
                registro.Add(ws.Cell(i, 55).Value.ToString());
                registro.Add(ws.Cell(i, 56).Value.ToString());
                registro.Add(ws.Cell(i, 57).Value.ToString());
                registro.Add(ws.Cell(i, 58).Value.ToString());
                registro.Add(ws.Cell(i, 59).Value.ToString());
                registro.Add(ws.Cell(i, 60).Value.ToString());

                //2065 - 2074
                registro.Add(ws.Cell(i, 61).Value.ToString());
                registro.Add(ws.Cell(i, 62).Value.ToString());
                registro.Add(ws.Cell(i, 63).Value.ToString());
                registro.Add(ws.Cell(i, 64).Value.ToString());
                registro.Add(ws.Cell(i, 65).Value.ToString());
                registro.Add(ws.Cell(i, 66).Value.ToString());
                registro.Add(ws.Cell(i, 67).Value.ToString());
                registro.Add(ws.Cell(i, 68).Value.ToString());
                registro.Add(ws.Cell(i, 69).Value.ToString());
                registro.Add(ws.Cell(i, 70).Value.ToString());

                //2075 - 2084
                registro.Add(ws.Cell(i, 71).Value.ToString());
                registro.Add(ws.Cell(i, 72).Value.ToString());
                registro.Add(ws.Cell(i, 73).Value.ToString());
                registro.Add(ws.Cell(i, 74).Value.ToString());
                registro.Add(ws.Cell(i, 75).Value.ToString());
                registro.Add(ws.Cell(i, 76).Value.ToString());
                registro.Add(ws.Cell(i, 77).Value.ToString());
                registro.Add(ws.Cell(i, 78).Value.ToString());
                registro.Add(ws.Cell(i, 79).Value.ToString());
                registro.Add(ws.Cell(i, 80).Value.ToString());

                //2085 - 2094
                registro.Add(ws.Cell(i, 81).Value.ToString());
                registro.Add(ws.Cell(i, 82).Value.ToString());
                registro.Add(ws.Cell(i, 83).Value.ToString());
                registro.Add(ws.Cell(i, 84).Value.ToString());
                registro.Add(ws.Cell(i, 85).Value.ToString());
                registro.Add(ws.Cell(i, 96).Value.ToString());
                registro.Add(ws.Cell(i, 87).Value.ToString());
                registro.Add(ws.Cell(i, 88).Value.ToString());
                registro.Add(ws.Cell(i, 89).Value.ToString());
                registro.Add(ws.Cell(i, 90).Value.ToString());

                //2095 - 2100
                registro.Add(ws.Cell(i, 91).Value.ToString());
                registro.Add(ws.Cell(i, 92).Value.ToString());
                registro.Add(ws.Cell(i, 93).Value.ToString());
                registro.Add(ws.Cell(i, 94).Value.ToString());
                registro.Add(ws.Cell(i, 95).Value.ToString());
                registro.Add(ws.Cell(i, 96).Value.ToString());
                registro.Add(ws.Cell(i, 97).Value.ToString());
                registro.Add(ws.Cell(i, 98).Value.ToString());

                InsertarInformacionFinancieraCasoBase(registro);
                registro.Clear();

            }
            /*-------------------------- FINANCIERO RESUMEN--------------------------------*/
            for (int X = 15; X < 17; X++)
            {
                registro.Add(token);
                registro.Add(usuario);
                registro.Add(ws.Cell(X, 1).Value.ToString());
                if (!string.IsNullOrEmpty(ws.Cell(X, 2).Value.ToString()))
                {
                    decimal p02 = decimal.Parse(ws.Cell(X, 2).Value.ToString()) * 100;
                    string sp02 = p02.ToString("0.0");
                    registro.Add(sp02);
                }
                else
                {
                    registro.Add("0");
                }
                if (!string.IsNullOrEmpty(ws.Cell(X, 3).Value.ToString()))
                {
                    decimal p03 = decimal.Parse(ws.Cell(X, 3).Value.ToString()) * 100;
                    string sp03 = p03.ToString("0.0");
                    registro.Add(sp03);
                }
                else
                {
                    registro.Add("0");
                }
                InsertarInformacionFinancieraResumidaCasoBase(registro);
                registro.Clear();
            }
            /*-------------------------- FISICO --------------------------------*/
            var ws1 = workbook.Worksheet(3);
            /*-------------------------- ESTRUCTURAR --------------------------------*/

            for (int e = 5; e < 10; e++)
            {
                registro.Add(token);
                registro.Add(usuario);
                registro.Add(ws1.Cell(e, 1).Value.ToString());
                if (!string.IsNullOrEmpty(ws1.Cell(e, 2).Value.ToString()))
                {
                    decimal t01 = decimal.Parse(ws1.Cell(e, 2).Value.ToString()) * 100;
                    string st01 = t01.ToString("0.0");
                    registro.Add(st01);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 3).Value.ToString()))
                {
                    decimal t03 = decimal.Parse(ws1.Cell(e, 3).Value.ToString()) * 100;
                    string st03 = t03.ToString("0.0");
                    registro.Add(st03);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 4).Value.ToString()))
                {
                    decimal t04 = decimal.Parse(ws1.Cell(e, 4).Value.ToString()) * 100;
                    string st04 = t04.ToString("0.0");
                    registro.Add(st04);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 5).Value.ToString()))
                {
                    decimal t05 = decimal.Parse(ws1.Cell(e, 5).Value.ToString()) * 100;
                    string st05 = t05.ToString("0.0");
                    registro.Add(st05);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 6).Value.ToString()))
                {
                    decimal t06 = decimal.Parse(ws1.Cell(e, 6).Value.ToString()) * 100;
                    string st06 = t06.ToString("0.0");
                    registro.Add(st06);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 7).Value.ToString()))
                {
                    decimal t07 = decimal.Parse(ws1.Cell(e, 7).Value.ToString()) * 100;
                    string st07 = t07.ToString("0.0");
                    registro.Add(st07);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 8).Value.ToString()))
                {
                    decimal t08 = decimal.Parse(ws1.Cell(e, 8).Value.ToString()) * 100;
                    string st08 = t08.ToString("0.0");
                    registro.Add(st08);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 9).Value.ToString()))
                {
                    decimal t09 = decimal.Parse(ws1.Cell(e, 9).Value.ToString()) * 100;
                    string st09 = t09.ToString("0.0");
                    registro.Add(st09);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 10).Value.ToString()))
                {
                    decimal t10 = decimal.Parse(ws1.Cell(e, 10).Value.ToString()) * 100;
                    string st10 = t10.ToString("0.0");
                    registro.Add(st10);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 11).Value.ToString()))
                {
                    decimal t11 = decimal.Parse(ws1.Cell(e, 11).Value.ToString()) * 100;
                    string st11 = t11.ToString("0.0");
                    registro.Add(st11);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 12).Value.ToString()))
                {
                    decimal t12 = decimal.Parse(ws1.Cell(e, 12).Value.ToString()) * 100;
                    string st12 = t12.ToString("0.0");
                    registro.Add(st12);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 13).Value.ToString()))
                {
                    decimal t13 = decimal.Parse(ws1.Cell(e, 13).Value.ToString()) * 100;
                    string st13 = t13.ToString("0.0");
                    registro.Add(st13);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 14).Value.ToString()))
                {
                    decimal t14 = decimal.Parse(ws1.Cell(e, 14).Value.ToString()) * 100;
                    string st14 = t14.ToString("0.0");
                    registro.Add(st14);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 15).Value.ToString()))
                {
                    decimal t15 = decimal.Parse(ws1.Cell(e, 15).Value.ToString()) * 100;
                    string st15 = t15.ToString("0.0");
                    registro.Add(st15);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 16).Value.ToString()))
                {
                    decimal t16 = decimal.Parse(ws1.Cell(e, 16).Value.ToString()) * 100;
                    string st16 = t16.ToString("0.0");
                    registro.Add(st16);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 17).Value.ToString()))
                {
                    decimal t17 = decimal.Parse(ws1.Cell(e, 17).Value.ToString()) * 100;
                    string st17 = t17.ToString("0.0");
                    registro.Add(st17);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 18).Value.ToString()))
                {
                    decimal t18 = decimal.Parse(ws1.Cell(e, 18).Value.ToString()) * 100;
                    string st18 = t18.ToString("0.0");
                    registro.Add(st18);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 19).Value.ToString()))
                {
                    decimal t19 = decimal.Parse(ws1.Cell(e, 19).Value.ToString()) * 100;
                    string st19 = t19.ToString("0.0");
                    registro.Add(st19);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 20).Value.ToString()))
                {
                    decimal t20 = decimal.Parse(ws1.Cell(e, 20).Value.ToString()) * 100;
                    string st20 = t20.ToString("0.0");
                    registro.Add(st20);
                }
                else
                {
                    registro.Add("0");
                }
                ///
                /// 21-30
                ///
                if (!string.IsNullOrEmpty(ws1.Cell(e, 21).Value.ToString()))
                {
                    decimal t21 = decimal.Parse(ws1.Cell(e, 21).Value.ToString()) * 100;
                    string st21 = t21.ToString("0.0");
                    registro.Add(st21);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 22).Value.ToString()))
                {
                    decimal t22 = decimal.Parse(ws1.Cell(e, 22).Value.ToString()) * 100;
                    string st22 = t22.ToString("0.0");
                    registro.Add(st22);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 23).Value.ToString()))
                {
                    decimal t23 = decimal.Parse(ws1.Cell(e, 23).Value.ToString()) * 100;
                    string st23 = t23.ToString("0.0");
                    registro.Add(st23);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 24).Value.ToString()))
                {
                    decimal t24 = decimal.Parse(ws1.Cell(e, 24).Value.ToString()) * 100;
                    string st24 = t24.ToString("0.0");
                    registro.Add(st24);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 25).Value.ToString()))
                {
                    decimal t25 = decimal.Parse(ws1.Cell(e, 25).Value.ToString()) * 100;
                    string st25 = t25.ToString("0.0");
                    registro.Add(st25);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 26).Value.ToString()))
                {
                    decimal t26 = decimal.Parse(ws1.Cell(e, 26).Value.ToString()) * 100;
                    string st26 = t26.ToString("0.0");
                    registro.Add(st26);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 27).Value.ToString()))
                {
                    decimal t27 = decimal.Parse(ws1.Cell(e, 27).Value.ToString()) * 100;
                    string st27 = t27.ToString("0.0");
                    registro.Add(st27);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 28).Value.ToString()))
                {
                    decimal t28 = decimal.Parse(ws1.Cell(e, 28).Value.ToString()) * 100;
                    string st28 = t28.ToString("0.0");
                    registro.Add(st28);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 29).Value.ToString()))
                {
                    decimal t29 = decimal.Parse(ws1.Cell(e, 29).Value.ToString()) * 100;
                    string st29 = t29.ToString("0.0");
                    registro.Add(st29);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 30).Value.ToString()))
                {
                    decimal t30 = decimal.Parse(ws1.Cell(e, 30).Value.ToString()) * 100;
                    string st30 = t30.ToString("0.0");
                    registro.Add(st30);
                }
                else
                {
                    registro.Add("0");
                }
                ///
                /// 31-40
                ///
                if (!string.IsNullOrEmpty(ws1.Cell(e, 31).Value.ToString()))
                {
                    decimal t31 = decimal.Parse(ws1.Cell(e, 31).Value.ToString()) * 100;
                    string st31 = t31.ToString("0.0");
                    registro.Add(st31);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 32).Value.ToString()))
                {
                    decimal t32 = decimal.Parse(ws1.Cell(e, 32).Value.ToString()) * 100;
                    string st32 = t32.ToString("0.0");
                    registro.Add(st32);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 33).Value.ToString()))
                {
                    decimal t33 = decimal.Parse(ws1.Cell(e, 33).Value.ToString()) * 100;
                    string st33 = t33.ToString("0.0");
                    registro.Add(st33);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 34).Value.ToString()))
                {
                    decimal t34 = decimal.Parse(ws1.Cell(e, 34).Value.ToString()) * 100;
                    string st34 = t34.ToString("0.0");
                    registro.Add(st34);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 35).Value.ToString()))
                {
                    decimal t35 = decimal.Parse(ws1.Cell(e, 35).Value.ToString()) * 100;
                    string st35 = t35.ToString("0.0");
                    registro.Add(st35);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 36).Value.ToString()))
                {
                    decimal t36 = decimal.Parse(ws1.Cell(e, 36).Value.ToString()) * 100;
                    string st36 = t36.ToString("0.0");
                    registro.Add(st36);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 37).Value.ToString()))
                {
                    decimal t37 = decimal.Parse(ws1.Cell(e, 37).Value.ToString()) * 100;
                    string st37 = t37.ToString("0.0");
                    registro.Add(st37);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 38).Value.ToString()))
                {
                    decimal t38 = decimal.Parse(ws1.Cell(e, 38).Value.ToString()) * 100;
                    string st38 = t38.ToString("0.0");
                    registro.Add(st38);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 39).Value.ToString()))
                {
                    decimal t39 = decimal.Parse(ws1.Cell(e, 39).Value.ToString()) * 100;
                    string st39 = t39.ToString("0.0");
                    registro.Add(st39);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 40).Value.ToString()))
                {

                    decimal t40 = decimal.Parse(ws1.Cell(e, 40).Value.ToString()) * 100;
                    string st40 = t40.ToString("0.0");
                    registro.Add(st40);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 41).Value.ToString()))
                {
                    decimal t41 = decimal.Parse(ws1.Cell(e, 41).Value.ToString()) * 100;
                    string st41 = t41.ToString("0.0");
                    registro.Add(st41);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 42).Value.ToString()))
                {
                    decimal t42 = decimal.Parse(ws1.Cell(e, 42).Value.ToString()) * 100;
                    string st42 = t42.ToString("0.0");
                    registro.Add(st42);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 43).Value.ToString()))
                {
                    decimal t43 = decimal.Parse(ws1.Cell(e, 43).Value.ToString()) * 100;
                    string st43 = t43.ToString("0.0");
                    registro.Add(st43);
                }
                else
                {
                    registro.Add("0");
                }


                if (!string.IsNullOrEmpty(ws1.Cell(e, 44).Value.ToString()))
                {
                    decimal t44 = decimal.Parse(ws1.Cell(e, 44).Value.ToString()) * 100;
                    string st44 = t44.ToString("0.0");
                    registro.Add(st44);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 45).Value.ToString()))
                {
                    decimal t45 = decimal.Parse(ws1.Cell(e, 45).Value.ToString()) * 100;
                    string st45 = t45.ToString("0.0");
                    registro.Add(st45);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 46).Value.ToString()))
                {
                    decimal t46 = decimal.Parse(ws1.Cell(e, 46).Value.ToString()) * 100;
                    string st46 = t46.ToString("0.0");
                    registro.Add(st46);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 47).Value.ToString()))
                {
                    decimal t47 = decimal.Parse(ws1.Cell(e, 47).Value.ToString()) * 100;
                    string st47 = t47.ToString("0.0");
                    registro.Add(st47);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 48).Value.ToString()))
                {
                    decimal t48 = decimal.Parse(ws1.Cell(e, 48).Value.ToString()) * 100;
                    string st48 = t48.ToString("0.0");
                    registro.Add(st48);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 49).Value.ToString()))
                {
                    decimal t49 = decimal.Parse(ws1.Cell(e, 49).Value.ToString()) * 100;
                    string st49 = t49.ToString("0.0");
                    registro.Add(st49);
                }
                else
                {
                    registro.Add("0");
                }


                if (!string.IsNullOrEmpty(ws1.Cell(e, 50).Value.ToString()))
                {
                    decimal t50 = decimal.Parse(ws1.Cell(e, 50).Value.ToString()) * 100;
                    string st50 = t50.ToString("0.0");
                    registro.Add(st50);
                }
                else
                {
                    registro.Add("0");
                }
                ///
                /// 51-60
                ///
                if (!string.IsNullOrEmpty(ws1.Cell(e, 51).Value.ToString()))
                {
                    decimal t51 = decimal.Parse(ws1.Cell(e, 51).Value.ToString()) * 100;
                    string st51 = t51.ToString("0.0");
                    registro.Add(st51);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 52).Value.ToString()))
                {
                    decimal t52 = decimal.Parse(ws1.Cell(e, 52).Value.ToString()) * 100;
                    string st52 = t52.ToString("0.0");
                    registro.Add(st52);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 53).Value.ToString()))
                {
                    decimal t53 = decimal.Parse(ws1.Cell(e, 53).Value.ToString()) * 100;
                    string st53 = t53.ToString("0.0");
                    registro.Add(st53);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 54).Value.ToString()))
                {
                    decimal t54 = decimal.Parse(ws1.Cell(e, 54).Value.ToString()) * 100;
                    string st54 = t54.ToString("0.0");
                    registro.Add(st54);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 55).Value.ToString()))
                {
                    decimal t55 = decimal.Parse(ws1.Cell(e, 55).Value.ToString()) * 100;
                    string st55 = t55.ToString("0.0");
                    registro.Add(st55);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 56).Value.ToString()))
                {
                    decimal t56 = decimal.Parse(ws1.Cell(e, 56).Value.ToString()) * 100;
                    string st56 = t56.ToString("0.0");
                    registro.Add(st56);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 57).Value.ToString()))
                {
                    decimal t57 = decimal.Parse(ws1.Cell(e, 57).Value.ToString()) * 100;
                    string st57 = t57.ToString("0.0");
                    registro.Add(st57);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 58).Value.ToString()))
                {
                    decimal t58 = decimal.Parse(ws1.Cell(e, 58).Value.ToString()) * 100;
                    string st58 = t58.ToString("0.0");
                    registro.Add(st58);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 59).Value.ToString()))
                {
                    decimal t59 = decimal.Parse(ws1.Cell(e, 59).Value.ToString()) * 100;
                    string st59 = t59.ToString("0.0");
                    registro.Add(st59);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 60).Value.ToString()))
                {
                    decimal t60 = decimal.Parse(ws1.Cell(e, 60).Value.ToString()) * 100;
                    string st60 = t60.ToString("0.0");
                    registro.Add(st60);
                }
                else
                {
                    registro.Add("0");
                }

                ///
                /// 61-70
                ///
                if (!string.IsNullOrEmpty(ws1.Cell(e, 61).Value.ToString()))
                {
                    decimal t61 = decimal.Parse(ws1.Cell(e, 61).Value.ToString()) * 100;
                    string st61 = t61.ToString("0.0");
                    registro.Add(st61);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 62).Value.ToString()))
                {
                    decimal t62 = decimal.Parse(ws1.Cell(e, 62).Value.ToString()) * 100;
                    string st62 = t62.ToString("0.0");
                    registro.Add(st62);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 63).Value.ToString()))
                {
                    decimal t63 = decimal.Parse(ws1.Cell(e, 63).Value.ToString()) * 100;
                    string st63 = t63.ToString("0.0");
                    registro.Add(st63);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 64).Value.ToString()))
                {
                    decimal t64 = decimal.Parse(ws1.Cell(e, 64).Value.ToString()) * 100;
                    string st64 = t64.ToString("0.0");
                    registro.Add(st64);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 65).Value.ToString()))
                {
                    decimal t65 = decimal.Parse(ws1.Cell(e, 65).Value.ToString()) * 100;
                    string st65 = t65.ToString("0.0");
                    registro.Add(st65);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 66).Value.ToString()))
                {
                    decimal t66 = decimal.Parse(ws1.Cell(e, 66).Value.ToString()) * 100;
                    string st66 = t66.ToString("0.0");
                    registro.Add(st66);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 67).Value.ToString()))
                {
                    decimal t67 = decimal.Parse(ws1.Cell(e, 67).Value.ToString()) * 100;
                    string st67 = t67.ToString("0.0");
                    registro.Add(st67);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 68).Value.ToString()))
                {
                    decimal t68 = decimal.Parse(ws1.Cell(e, 68).Value.ToString()) * 100;
                    string st68 = t68.ToString("0.0");
                    registro.Add(st68);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 69).Value.ToString()))
                {
                    decimal t69 = decimal.Parse(ws1.Cell(e, 69).Value.ToString()) * 100;
                    string st69 = t69.ToString("0.0");
                    registro.Add(st69);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 70).Value.ToString()))
                {
                    decimal t70 = decimal.Parse(ws1.Cell(e, 70).Value.ToString()) * 100;
                    string st70 = t70.ToString("0.0");
                    registro.Add(st70);
                }
                else
                {
                    registro.Add("0");
                }

                ///
                /// 71-80
                ///
                if (!string.IsNullOrEmpty(ws1.Cell(e, 71).Value.ToString()))
                {
                    decimal t71 = decimal.Parse(ws1.Cell(e, 71).Value.ToString()) * 100;
                    string st71 = t71.ToString("0.0");
                    registro.Add(st71);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 72).Value.ToString()))
                {
                    decimal t72 = decimal.Parse(ws1.Cell(e, 72).Value.ToString()) * 100;
                    string st72 = t72.ToString("0.0");
                    registro.Add(st72);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 73).Value.ToString()))
                {
                    decimal t73 = decimal.Parse(ws1.Cell(e, 73).Value.ToString()) * 100;
                    string st73 = t73.ToString("0.0");
                    registro.Add(st73);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 74).Value.ToString()))
                {
                    decimal t74 = decimal.Parse(ws1.Cell(e, 74).Value.ToString()) * 100;
                    string st74 = t74.ToString("0.0");
                    registro.Add(st74);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 75).Value.ToString()))
                {
                    decimal t75 = decimal.Parse(ws1.Cell(e, 75).Value.ToString()) * 100;
                    string st75 = t75.ToString("0.0");
                    registro.Add(st75);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 76).Value.ToString()))
                {
                    decimal t76 = decimal.Parse(ws1.Cell(e, 76).Value.ToString()) * 100;
                    string st76 = t76.ToString("0.0");
                    registro.Add(st76);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 77).Value.ToString()))
                {
                    decimal t77 = decimal.Parse(ws1.Cell(e, 77).Value.ToString()) * 100;
                    string st77 = t77.ToString("0.0");
                    registro.Add(st77);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 78).Value.ToString()))
                {
                    decimal t78 = decimal.Parse(ws1.Cell(e, 78).Value.ToString()) * 100;
                    string st78 = t78.ToString("0.0");
                    registro.Add(st78);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 79).Value.ToString()))
                {
                    decimal t79 = decimal.Parse(ws1.Cell(e, 79).Value.ToString()) * 100;
                    string st79 = t79.ToString("0.0");
                    registro.Add(st79);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 80).Value.ToString()))
                {
                    decimal t80 = decimal.Parse(ws1.Cell(e, 80).Value.ToString()) * 100;
                    string st80 = t80.ToString("0.0");
                    registro.Add(st80);
                }
                else
                {
                    registro.Add("0");
                }


                ///
                /// 81-90
                ///
                if (!string.IsNullOrEmpty(ws1.Cell(e, 81).Value.ToString()))
                {
                    decimal t81 = decimal.Parse(ws1.Cell(e, 81).Value.ToString()) * 100;
                    string st81 = t81.ToString("0.0");
                    registro.Add(st81);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 82).Value.ToString()))
                {
                    decimal t82 = decimal.Parse(ws1.Cell(e, 82).Value.ToString()) * 100;
                    string st82 = t82.ToString("0.0");
                    registro.Add(st82);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 83).Value.ToString()))
                {
                    decimal t83 = decimal.Parse(ws1.Cell(e, 83).Value.ToString()) * 100;
                    string st83 = t83.ToString("0.0");
                    registro.Add(st83);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 84).Value.ToString()))
                {
                    decimal t84 = decimal.Parse(ws1.Cell(e, 84).Value.ToString()) * 100;
                    string st84 = t84.ToString("0.0");
                    registro.Add(st84);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 85).Value.ToString()))
                {
                    decimal t85 = decimal.Parse(ws1.Cell(e, 85).Value.ToString()) * 100;
                    string st85 = t85.ToString("0.0");
                    registro.Add(st85);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 86).Value.ToString()))
                {
                    decimal t86 = decimal.Parse(ws1.Cell(e, 86).Value.ToString()) * 100;
                    string st86 = t86.ToString("0.0");
                    registro.Add(st86);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 87).Value.ToString()))
                {
                    decimal t87 = decimal.Parse(ws1.Cell(e, 87).Value.ToString()) * 100;
                    string st87 = t87.ToString("0.0");
                    registro.Add(st87);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 88).Value.ToString()))
                {
                    decimal t88 = decimal.Parse(ws1.Cell(e, 88).Value.ToString()) * 100;
                    string st88 = t88.ToString("0.0");
                    registro.Add(st88);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 89).Value.ToString()))
                {
                    decimal t89 = decimal.Parse(ws1.Cell(e, 89).Value.ToString()) * 100;
                    string st89 = t89.ToString("0.0");
                    registro.Add(st89);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 90).Value.ToString()))
                {
                    decimal t90 = decimal.Parse(ws1.Cell(e, 90).Value.ToString()) * 100;
                    string st90 = t90.ToString("0.0");
                    registro.Add(st90);
                }
                else
                {
                    registro.Add("0");
                }

                ///
                /// 91-100
                ///
                if (!string.IsNullOrEmpty(ws1.Cell(e, 91).Value.ToString()))
                {
                    decimal t91 = decimal.Parse(ws1.Cell(e, 91).Value.ToString()) * 100;
                    string st91 = t91.ToString("0.0");
                    registro.Add(st91);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 92).Value.ToString()))
                {
                    decimal t92 = decimal.Parse(ws1.Cell(e, 92).Value.ToString()) * 100;
                    string st92 = t92.ToString("0.0");
                    registro.Add(st92);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 93).Value.ToString()))
                {
                    decimal t93 = decimal.Parse(ws1.Cell(e, 93).Value.ToString()) * 100;
                    string st93 = t93.ToString("0.0");
                    registro.Add(st93);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 94).Value.ToString()))
                {
                    decimal t94 = decimal.Parse(ws1.Cell(e, 94).Value.ToString()) * 100;
                    string st94 = t94.ToString("0.0");
                    registro.Add(st94);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 95).Value.ToString()))
                {
                    decimal t95 = decimal.Parse(ws1.Cell(e, 95).Value.ToString()) * 100;
                    string st95 = t95.ToString("0.0");
                    registro.Add(st95);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 96).Value.ToString()))
                {
                    decimal t96 = decimal.Parse(ws1.Cell(e, 96).Value.ToString()) * 100;
                    string st96 = t96.ToString("0.0");
                    registro.Add(st96);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 97).Value.ToString()))
                {
                    decimal t97 = decimal.Parse(ws1.Cell(e, 97).Value.ToString()) * 100;
                    string st97 = t97.ToString("0.0");
                    registro.Add(st97);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 98).Value.ToString()))
                {
                    registro.Add(ws1.Cell(e, 98).Value.ToString());
                }
                else
                {
                    registro.Add("0");
                }

                InsertarInformacionFisicoCasoBase(registro);
                registro.Clear();
            }
            /*-------------------------- GENERAL --------------------------------*/
            var ws2 = workbook.Worksheet(1);
            /*-------------------------- ESTRUCTURAR --------------------------------*/

            registro.Add(token);
            registro.Add(usuario);
            registro.Add(ws2.Cell("C4").Value.ToString());
            registro.Add(ws2.Cell("C5").Value.ToString());
            registro.Add(ws2.Cell("C6").Value.ToString());
            registro.Add(ws2.Cell("C7").Value.ToString());
            InsertarInformacionGeneralCasoBase(registro);
            registro.Clear();

            return "OK";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Datos"></param>
        public void InsertarInformacionFinancieraCasoBase(List<String> Datos)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", Datos[0].ToString());
                parametos.Add("IniUsuario", Datos[1].ToString());
                parametos.Add("IfDato0", Datos[2].ToString());
                parametos.Add("IfDato1", Datos[3].ToString());
                parametos.Add("IfDato2", Datos[4].ToString());

                parametos.Add("IfDato3", Datos[5].ToString());
                parametos.Add("IfDato4", Datos[6].ToString());
                parametos.Add("IfDato5", Datos[7].ToString());
                parametos.Add("IfDato6", Datos[8].ToString());
                parametos.Add("IfDato7", Datos[9].ToString());
                parametos.Add("IfDato8", Datos[10].ToString());
                parametos.Add("IfDato9", Datos[11].ToString());
                parametos.Add("IfDato10", Datos[12].ToString());
                parametos.Add("IfDato11", Datos[13].ToString());
                parametos.Add("IfDato12", Datos[14].ToString());
                parametos.Add("IfDato13", Datos[15].ToString());
                parametos.Add("IfDato14", Datos[16].ToString());

                parametos.Add("IfDato15", Datos[17].ToString());

                parametos.Add("IfDato16", Datos[18].ToString()); //2021 - 2030
                parametos.Add("IfDato17", Datos[19].ToString());
                parametos.Add("IfDato18", Datos[20].ToString());
                parametos.Add("IfDato19", Datos[21].ToString());
                parametos.Add("IfDato20", Datos[22].ToString());
                parametos.Add("IfDato21", Datos[23].ToString());
                parametos.Add("IfDato22", Datos[24].ToString());
                parametos.Add("IfDato23", Datos[25].ToString());
                parametos.Add("IfDato24", Datos[26].ToString());
                parametos.Add("IfDato25", Datos[27].ToString());

                parametos.Add("IfDato26", Datos[28].ToString()); //2031 - 2040
                parametos.Add("IfDato27", Datos[29].ToString());
                parametos.Add("IfDato28", Datos[30].ToString());
                parametos.Add("IfDato29", Datos[31].ToString());
                parametos.Add("IfDato30", Datos[32].ToString());
                parametos.Add("IfDato31", Datos[33].ToString());
                parametos.Add("IfDato32", Datos[34].ToString());
                parametos.Add("IfDato33", Datos[35].ToString());
                parametos.Add("IfDato34", Datos[36].ToString());
                parametos.Add("IfDato35", Datos[37].ToString());

                parametos.Add("IfDato36", Datos[38].ToString()); //2041 - 2050
                parametos.Add("IfDato37", Datos[39].ToString());
                parametos.Add("IfDato38", Datos[40].ToString());
                parametos.Add("IfDato39", Datos[41].ToString());
                parametos.Add("IfDato40", Datos[42].ToString());
                parametos.Add("IfDato41", Datos[43].ToString());
                parametos.Add("IfDato42", Datos[44].ToString());
                parametos.Add("IfDato43", Datos[45].ToString());
                parametos.Add("IfDato44", Datos[46].ToString());
                parametos.Add("IfDato45", Datos[47].ToString());

                parametos.Add("IfDato46", Datos[48].ToString()); //2051 - 2060
                parametos.Add("IfDato47", Datos[49].ToString());
                parametos.Add("IfDato48", Datos[50].ToString());
                parametos.Add("IfDato49", Datos[51].ToString());
                parametos.Add("IfDato50", Datos[52].ToString());
                parametos.Add("IfDato51", Datos[53].ToString());
                parametos.Add("IfDato52", Datos[54].ToString());
                parametos.Add("IfDato53", Datos[55].ToString());
                parametos.Add("IfDato54", Datos[56].ToString());
                parametos.Add("IfDato55", Datos[57].ToString());

                parametos.Add("IfDato56", Datos[58].ToString()); //2061 - 2070
                parametos.Add("IfDato57", Datos[59].ToString());
                parametos.Add("IfDato58", Datos[60].ToString());
                parametos.Add("IfDato59", Datos[61].ToString());
                parametos.Add("IfDato60", Datos[62].ToString());
                parametos.Add("IfDato61", Datos[63].ToString());
                parametos.Add("IfDato62", Datos[64].ToString());
                parametos.Add("IfDato63", Datos[65].ToString());
                parametos.Add("IfDato64", Datos[66].ToString());
                parametos.Add("IfDato65", Datos[67].ToString());

                parametos.Add("IfDato66", Datos[68].ToString()); //2071 - 2080
                parametos.Add("IfDato67", Datos[69].ToString());
                parametos.Add("IfDato68", Datos[70].ToString());
                parametos.Add("IfDato69", Datos[71].ToString());
                parametos.Add("IfDato70", Datos[72].ToString());
                parametos.Add("IfDato71", Datos[73].ToString());
                parametos.Add("IfDato72", Datos[74].ToString());
                parametos.Add("IfDato73", Datos[75].ToString());
                parametos.Add("IfDato74", Datos[76].ToString());
                parametos.Add("IfDato75", Datos[77].ToString());

                parametos.Add("IfDato76", Datos[78].ToString()); //2081 - 2090
                parametos.Add("IfDato77", Datos[79].ToString());
                parametos.Add("IfDato78", Datos[80].ToString());
                parametos.Add("IfDato79", Datos[81].ToString());
                parametos.Add("IfDato80", Datos[82].ToString());
                parametos.Add("IfDato81", Datos[83].ToString());
                parametos.Add("IfDato82", Datos[84].ToString());
                parametos.Add("IfDato83", Datos[85].ToString());
                parametos.Add("IfDato84", Datos[86].ToString());
                parametos.Add("IfDato85", Datos[87].ToString());

                parametos.Add("IfDato86", Datos[88].ToString()); //2091 - 2100
                parametos.Add("IfDato87", Datos[89].ToString());
                parametos.Add("IfDato88", Datos[90].ToString());
                parametos.Add("IfDato89", Datos[91].ToString());
                parametos.Add("IfDato90", Datos[92].ToString());
                parametos.Add("IfDato91", Datos[93].ToString());
                parametos.Add("IfDato92", Datos[94].ToString());
                parametos.Add("IfDato93", Datos[95].ToString());
                parametos.Add("IfDato94", Datos[96].ToString());
                parametos.Add("IfDato95", Datos[97].ToString());

                parametos.Add("IfDato96", Datos[98].ToString());//TOTAL



                ORM.Execute("CAPEX_INS_INFORMACION_FINANCIERA_CASOBASE", parametos, commandType: CommandType.StoredProcedure);
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "InsertarInformacionFinancieraCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Datos"></param>
        public void InsertarInformacionFisicoCasoBase(List<String> Datos)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", Datos[0].ToString());
                parametos.Add("IniUsuario", Datos[1].ToString());
                parametos.Add("IfDato0", Datos[2].ToString());
                parametos.Add("IfDato1", Datos[3].ToString());
                parametos.Add("IfDato2", Datos[4].ToString());

                parametos.Add("IfDato3", Datos[5].ToString());
                parametos.Add("IfDato4", Datos[6].ToString());
                parametos.Add("IfDato5", Datos[7].ToString());
                parametos.Add("IfDato6", Datos[8].ToString());
                parametos.Add("IfDato7", Datos[9].ToString());
                parametos.Add("IfDato8", Datos[10].ToString());
                parametos.Add("IfDato9", Datos[11].ToString());
                parametos.Add("IfDato10", Datos[12].ToString());
                parametos.Add("IfDato11", Datos[13].ToString());
                parametos.Add("IfDato12", Datos[14].ToString());
                parametos.Add("IfDato13", Datos[15].ToString());
                parametos.Add("IfDato14", Datos[16].ToString());

                parametos.Add("IfDato15", Datos[17].ToString());

                parametos.Add("IfDato16", Datos[18].ToString()); //2021 - 2030
                parametos.Add("IfDato17", Datos[19].ToString());
                parametos.Add("IfDato18", Datos[20].ToString());
                parametos.Add("IfDato19", Datos[21].ToString());
                parametos.Add("IfDato20", Datos[22].ToString());
                parametos.Add("IfDato21", Datos[23].ToString());
                parametos.Add("IfDato22", Datos[24].ToString());
                parametos.Add("IfDato23", Datos[25].ToString());
                parametos.Add("IfDato24", Datos[26].ToString());
                parametos.Add("IfDato25", Datos[27].ToString());

                parametos.Add("IfDato26", Datos[28].ToString()); //2031 - 2040
                parametos.Add("IfDato27", Datos[29].ToString());
                parametos.Add("IfDato28", Datos[30].ToString());
                parametos.Add("IfDato29", Datos[31].ToString());
                parametos.Add("IfDato30", Datos[32].ToString());
                parametos.Add("IfDato31", Datos[33].ToString());
                parametos.Add("IfDato32", Datos[34].ToString());
                parametos.Add("IfDato33", Datos[35].ToString());
                parametos.Add("IfDato34", Datos[36].ToString());
                parametos.Add("IfDato35", Datos[37].ToString());

                parametos.Add("IfDato36", Datos[38].ToString()); //2041 - 2050
                parametos.Add("IfDato37", Datos[39].ToString());
                parametos.Add("IfDato38", Datos[40].ToString());
                parametos.Add("IfDato39", Datos[41].ToString());
                parametos.Add("IfDato40", Datos[42].ToString());
                parametos.Add("IfDato41", Datos[43].ToString());
                parametos.Add("IfDato42", Datos[44].ToString());
                parametos.Add("IfDato43", Datos[45].ToString());
                parametos.Add("IfDato44", Datos[46].ToString());
                parametos.Add("IfDato45", Datos[47].ToString());

                parametos.Add("IfDato46", Datos[48].ToString()); //2051 - 2060
                parametos.Add("IfDato47", Datos[49].ToString());
                parametos.Add("IfDato48", Datos[50].ToString());
                parametos.Add("IfDato49", Datos[51].ToString());
                parametos.Add("IfDato50", Datos[52].ToString());
                parametos.Add("IfDato51", Datos[53].ToString());
                parametos.Add("IfDato52", Datos[54].ToString());
                parametos.Add("IfDato53", Datos[55].ToString());
                parametos.Add("IfDato54", Datos[56].ToString());
                parametos.Add("IfDato55", Datos[57].ToString());

                parametos.Add("IfDato56", Datos[58].ToString()); //2061 - 2070
                parametos.Add("IfDato57", Datos[59].ToString());
                parametos.Add("IfDato58", Datos[60].ToString());
                parametos.Add("IfDato59", Datos[61].ToString());
                parametos.Add("IfDato60", Datos[62].ToString());
                parametos.Add("IfDato61", Datos[63].ToString());
                parametos.Add("IfDato62", Datos[64].ToString());
                parametos.Add("IfDato63", Datos[65].ToString());
                parametos.Add("IfDato64", Datos[66].ToString());
                parametos.Add("IfDato65", Datos[67].ToString());

                parametos.Add("IfDato66", Datos[68].ToString()); //2071 - 2080
                parametos.Add("IfDato67", Datos[69].ToString());
                parametos.Add("IfDato68", Datos[70].ToString());
                parametos.Add("IfDato69", Datos[71].ToString());
                parametos.Add("IfDato70", Datos[72].ToString());
                parametos.Add("IfDato71", Datos[73].ToString());
                parametos.Add("IfDato72", Datos[74].ToString());
                parametos.Add("IfDato73", Datos[75].ToString());
                parametos.Add("IfDato74", Datos[76].ToString());
                parametos.Add("IfDato75", Datos[77].ToString());

                parametos.Add("IfDato76", Datos[78].ToString()); //2081 - 2090
                parametos.Add("IfDato77", Datos[79].ToString());
                parametos.Add("IfDato78", Datos[80].ToString());
                parametos.Add("IfDato79", Datos[81].ToString());
                parametos.Add("IfDato80", Datos[82].ToString());
                parametos.Add("IfDato81", Datos[83].ToString());
                parametos.Add("IfDato82", Datos[84].ToString());
                parametos.Add("IfDato83", Datos[85].ToString());
                parametos.Add("IfDato84", Datos[86].ToString());
                parametos.Add("IfDato85", Datos[87].ToString());

                parametos.Add("IfDato86", Datos[88].ToString()); //2091 - 2100

                parametos.Add("IfDato87", Datos[89].ToString());
                parametos.Add("IfDato88", Datos[90].ToString());
                parametos.Add("IfDato89", Datos[91].ToString());
                parametos.Add("IfDato90", Datos[92].ToString());
                parametos.Add("IfDato91", Datos[93].ToString());
                parametos.Add("IfDato92", Datos[94].ToString());
                parametos.Add("IfDato93", Datos[95].ToString());
                parametos.Add("IfDato94", Datos[96].ToString());
                parametos.Add("IfDato95", Datos[97].ToString());
                parametos.Add("IfDato96", Datos[98].ToString());

                ORM.Execute("CAPEX_INS_INFORMACION_FISICO_CASOBASE", parametos, commandType: CommandType.StoredProcedure);
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "InsertarInformacionFisicoCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Datos"></param>
        private void InsertarInformacionFinancieraResumidaCasoBase(List<String> Datos)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", Datos[0].ToString());
                parametos.Add("IniUsuario", Datos[1].ToString());
                parametos.Add("IrDato0", Datos[2].ToString());
                parametos.Add("IrDato1", Datos[3].ToString());
                parametos.Add("IrDato2", Datos[4].ToString());

                ORM.Execute("CAPEX_INS_INFORMACION_FINANCIERA_RESUMIDA", parametos, commandType: CommandType.StoredProcedure);
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "InsertarInformacionFinancieraResumida, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Datos"></param>
        private void InsertarInformacionGeneralCasoBase(List<String> Datos)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", Datos[0].ToString());
                parametos.Add("IniUsuario", Datos[1].ToString());
                parametos.Add("IgPresupuesto", Datos[2].ToString());
                parametos.Add("IgFechaInicio", Datos[3].ToString());
                parametos.Add("IgTermino", Datos[4].ToString());
                parametos.Add("IgCierre", Datos[5].ToString());

                ORM.Execute("CAPEX_INS_INFORMACION_GENERAL", parametos, commandType: CommandType.StoredProcedure);
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "InsertarInformacionGeneralCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string PoblarVistaPresupuestoFinancieroCasoBase(string token)
        {
            string Desplegable = String.Empty;
            try
            {
                var resultado = ORM.Query("CAPEX_SEL_INFORMACION_FINANCIERA_CASOBASE", new { token }, commandType: CommandType.StoredProcedure).ToList();
                var table = new StringBuilder();
                if (resultado.Count > 0)
                {
                    foreach (var result in resultado)
                    {
                        table.Append("<tr>");
                        table.Append("<td style='text-align:left;background-color:#5c808d;'>" + result.IfDato0 + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato2)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato3)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato4)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato5)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato6)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato7)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato8)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato9)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato10)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato11)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato12)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato13)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato14)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato15)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato16)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato17)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato18)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato19)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");

                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato20)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato21)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato22)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato23)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato24)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato25)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato26)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato27)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato28)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato29)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");

                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato30)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato31)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato32)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato33)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato34)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato35)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato36)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato37)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato38)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato39)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");

                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato40)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato41)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato42)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato43)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato44)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato45)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato46)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato47)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato48)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato49)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");

                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato50)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato51)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato52)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato53)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato54)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato55)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato56)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato57)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato58)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato59)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");

                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato60)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato61)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato62)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato63)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato64)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato65)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato66)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato67)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato68)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato69)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");


                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato70)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato71)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato72)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato73)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato74)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato75)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato76)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato77)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato78)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato79)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");

                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato80)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato81)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato82)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato83)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato84)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato85)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato86)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato87)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato88)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato89)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");

                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato90)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato91)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato92)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato93)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato94)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato95)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato96)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");

                        table.Append("</tr>");
                    }
                    Desplegable = table.ToString();
                }
                else
                {
                    Desplegable = "";
                }

            }
            catch (Exception exc)
            {
                //Desplegable = null;
                Desplegable = exc.Message.ToString() + "-----" + exc.StackTrace.ToString();
            }
            return Desplegable.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string PoblarVistaPresupuestoFisicoCasoBase(string token)
        {
            string Desplegable = String.Empty;
            try
            {
                var resultado = ORM.Query("CAPEX_SEL_INFORMACION_FISICO_CASOBASE", new { token }, commandType: CommandType.StoredProcedure).ToList();
                var table = new StringBuilder();
                if (resultado.Count > 0)
                {
                    foreach (var result in resultado)
                    {
                        CultureInfo ciCL = new CultureInfo("es-CL", false);
                        table.Append("<tr>");
                        table.Append("<td style='text-align:left;background-color:#5c808d;'>" + result.IfDato0 + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato2.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato3.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato4.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato5.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato6.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato7.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato8.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato9.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato10.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato11.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato12.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato13.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato14.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato15.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato16.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato17.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato18.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato19.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");

                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato20.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato21.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato22.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato23.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato24.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato25.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato26.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato27.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato28.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato29.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");

                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato30.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato31.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato32.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato33.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato34.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato35.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato36.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato37.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato38.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato39.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");

                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato40.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato41.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato42.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato43.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato44.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato45.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato46.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato47.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato48.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato49.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");

                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato50.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato51.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato52.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato53.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato54.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato55.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato56.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato57.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato58.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato59.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");

                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato60.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato61.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato62.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato63.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato64.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato65.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato66.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato67.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato68.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato69.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");


                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato70.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato71.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato72.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato73.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato74.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato75.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato76.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato77.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato78.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato79.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");

                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato80.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato81.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato82.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato83.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato84.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato85.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato86.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato87.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato88.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato89.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");

                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato90.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato91.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato92.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato93.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato94.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato95.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato96.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");

                        /*CultureInfo ciCL = new CultureInfo("es-CL", false);
                        NumberFormatInfo nfiCL = new CultureInfo("es-CL", true).NumberFormat; 
                        table.Append("<td style='text-align:left;background-color:#5c808d;'>" + result.IfDato0 + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato2, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato3, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato4, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato5, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato6, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato7, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato8, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato9, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato10, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato11, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato12, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato13, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato14, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato15, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato16, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato17, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato18, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato19, ciCL).ToString("n", nfiCL) + "</td>");

                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato20, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato21, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato22, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato23, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato24, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato25, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato26, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato27, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato28, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato29, ciCL).ToString("n", nfiCL) + "</td>");

                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato30, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato31, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato32, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato33, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato34, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato35, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato36, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato37, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato38, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato39, ciCL).ToString("n", nfiCL) + "</td>");

                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato40, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato41, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato42, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato43, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato44, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato45, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato46, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato47, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato48, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato49, ciCL).ToString("n", nfiCL) + "</td>");

                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato50, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato51, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato52, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato53, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato54, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato55, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato56, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato57, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato58, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato59, ciCL).ToString("n", nfiCL) + "</td>");

                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato60, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato61, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato62, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato63, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato64, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato65, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato66, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato67, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato68, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato69, ciCL).ToString("n", nfiCL) + "</td>");


                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato70, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato71, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato72, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato73, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato74, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato75, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato76, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato77, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato78, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato79, ciCL).ToString("n", nfiCL) + "</td>");

                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato80, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato81, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato82, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato83, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato84, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato85, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato86, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato87, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato88, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato89, ciCL).ToString("n", nfiCL) + "</td>");

                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato90, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato91, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato92, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato93, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato94, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato95, ciCL).ToString("n", nfiCL) + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.IfDato96, ciCL).ToString("n", nfiCL) + "</td>");*/

                        table.Append("</tr>");
                    }
                    Desplegable = table.ToString();
                }
                else
                {
                    Desplegable = "";
                }

            }
            catch (Exception exc)
            {
                Desplegable = null;
                //Desplegable = exc.Message.ToString() + "-----" + exc.StackTrace.ToString();
            }
            return Desplegable.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="usuario"></param>
        /// <param name="archivo"></param>
        /// <returns></returns>

        public string ImportarTemplate(string token, string usuario, string archivo)
        {

            /*-------------------------- CONFIGURAR --------------------------------*/
            List<String> registro = new List<String>();
            string path = ConfigurationManager.AppSettings.Get("CAPEX_IMPOR_PATH");
            var workbook = new XLWorkbook(path + token + "\\" + archivo);
            /*-------------------------- FINANCIERO --------------------------------*/
            var ws = workbook.Worksheet(2);
            /*-------------------------- ESTRUCTURAR --------------------------------*/
            for (int i = 5; i < 12; i++)
            {
                registro.Add(token);
                registro.Add(usuario);
                registro.Add(ws.Cell(i, 1).Value.ToString());

                if (!string.IsNullOrEmpty(ws.Cell(i, 2).Value.ToString()))
                {
                    decimal d01 = decimal.Parse(ws.Cell(i, 2).Value.ToString()) * 100;
                    string sd01 = d01.ToString("0.0");
                    registro.Add(sd01);
                }
                else
                {
                    registro.Add("0");
                }
                registro.Add(ws.Cell(i, 3).Value.ToString());

                registro.Add(ws.Cell(i, 4).Value.ToString());
                registro.Add(ws.Cell(i, 5).Value.ToString());
                registro.Add(ws.Cell(i, 6).Value.ToString());
                registro.Add(ws.Cell(i, 7).Value.ToString());
                registro.Add(ws.Cell(i, 8).Value.ToString());
                registro.Add(ws.Cell(i, 9).Value.ToString());
                registro.Add(ws.Cell(i, 10).Value.ToString());
                registro.Add(ws.Cell(i, 11).Value.ToString());
                registro.Add(ws.Cell(i, 12).Value.ToString());
                registro.Add(ws.Cell(i, 13).Value.ToString());
                registro.Add(ws.Cell(i, 14).Value.ToString());
                registro.Add(ws.Cell(i, 15).Value.ToString());

                registro.Add(ws.Cell(i, 16).Value.ToString());

                registro.Add(ws.Cell(i, 17).Value.ToString());
                registro.Add(ws.Cell(i, 18).Value.ToString());
                registro.Add(ws.Cell(i, 19).Value.ToString());

                registro.Add(ws.Cell(i, 20).Value.ToString());

                registro.Add(ws.Cell(i, 21).Value.ToString());

                InsertarInformacionFinanciera(registro);
                registro.Clear();

            }
            /*-------------------------- FINANCIERO RESUMEN--------------------------------*/
            for (int X = 15; X < 17; X++)
            {
                registro.Add(token);
                registro.Add(usuario);
                registro.Add(ws.Cell(X, 1).Value.ToString());
                if (!string.IsNullOrEmpty(ws.Cell(X, 2).Value.ToString()))
                {
                    decimal p02 = decimal.Parse(ws.Cell(X, 2).Value.ToString()) * 100;
                    string sp02 = p02.ToString("0.0");
                    registro.Add(sp02);
                }
                else
                {
                    registro.Add("0");
                }
                if (!string.IsNullOrEmpty(ws.Cell(X, 3).Value.ToString()))
                {
                    decimal p03 = decimal.Parse(ws.Cell(X, 3).Value.ToString()) * 100;
                    string sp03 = p03.ToString("0.0");
                    registro.Add(sp03);
                }
                else
                {
                    registro.Add("0");
                }
                InsertarInformacionFinancieraResumida(registro);
                registro.Clear();
            }

            /*-------------------------- FISICO --------------------------------*/
            var ws1 = workbook.Worksheet(3);
            /*-------------------------- ESTRUCTURAR --------------------------------*/

            for (int e = 5; e < 10; e++)
            {
                registro.Add(token);
                registro.Add(usuario);
                registro.Add(ws1.Cell(e, 1).Value.ToString());
                if (!string.IsNullOrEmpty(ws1.Cell(e, 2).Value.ToString()))
                {
                    decimal t01 = decimal.Parse(ws1.Cell(e, 2).Value.ToString()) * 100;
                    string st01 = t01.ToString("0.0");
                    registro.Add(st01);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 3).Value.ToString()))
                {
                    decimal t03 = decimal.Parse(ws1.Cell(e, 3).Value.ToString()) * 100;
                    string st03 = t03.ToString("0.0");
                    registro.Add(st03);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 4).Value.ToString()))
                {
                    decimal t04 = decimal.Parse(ws1.Cell(e, 4).Value.ToString()) * 100;
                    string st04 = t04.ToString("0.0");
                    registro.Add(st04);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 5).Value.ToString()))
                {
                    decimal t05 = decimal.Parse(ws1.Cell(e, 5).Value.ToString()) * 100;
                    string st05 = t05.ToString("0.0");
                    registro.Add(st05);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 6).Value.ToString()))
                {
                    decimal t06 = decimal.Parse(ws1.Cell(e, 6).Value.ToString()) * 100;
                    string st06 = t06.ToString("0.0");
                    registro.Add(st06);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 7).Value.ToString()))
                {
                    decimal t07 = decimal.Parse(ws1.Cell(e, 7).Value.ToString()) * 100;
                    string st07 = t07.ToString("0.0");
                    registro.Add(st07);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 8).Value.ToString()))
                {
                    decimal t08 = decimal.Parse(ws1.Cell(e, 8).Value.ToString()) * 100;
                    string st08 = t08.ToString("0.0");
                    registro.Add(st08);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 9).Value.ToString()))
                {
                    decimal t09 = decimal.Parse(ws1.Cell(e, 9).Value.ToString()) * 100;
                    string st09 = t09.ToString("0.0");
                    registro.Add(st09);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 10).Value.ToString()))
                {
                    decimal t10 = decimal.Parse(ws1.Cell(e, 10).Value.ToString()) * 100;
                    string st10 = t10.ToString("0.0");
                    registro.Add(st10);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 11).Value.ToString()))
                {
                    decimal t11 = decimal.Parse(ws1.Cell(e, 11).Value.ToString()) * 100;
                    string st11 = t11.ToString("0.0");
                    registro.Add(st11);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 12).Value.ToString()))
                {
                    decimal t12 = decimal.Parse(ws1.Cell(e, 12).Value.ToString()) * 100;
                    string st12 = t12.ToString("0.0");
                    registro.Add(st12);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 13).Value.ToString()))
                {
                    decimal t13 = decimal.Parse(ws1.Cell(e, 13).Value.ToString()) * 100;
                    string st13 = t13.ToString("0.0");
                    registro.Add(st13);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 14).Value.ToString()))
                {
                    decimal t14 = decimal.Parse(ws1.Cell(e, 14).Value.ToString()) * 100;
                    string st14 = t14.ToString("0.0");
                    registro.Add(st14);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 15).Value.ToString()))
                {
                    decimal t15 = decimal.Parse(ws1.Cell(e, 15).Value.ToString()) * 100;
                    string st15 = t15.ToString("0.0");
                    registro.Add(st15);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 16).Value.ToString()))
                {
                    decimal t16 = decimal.Parse(ws1.Cell(e, 16).Value.ToString()) * 100;
                    string st16 = t16.ToString("0.0");
                    registro.Add(st16);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 17).Value.ToString()))
                {
                    decimal t17 = decimal.Parse(ws1.Cell(e, 17).Value.ToString()) * 100;
                    string st17 = t17.ToString("0.0");
                    registro.Add(st17);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 18).Value.ToString()))
                {
                    decimal t18 = decimal.Parse(ws1.Cell(e, 18).Value.ToString()) * 100;
                    string st18 = t18.ToString("0.0");
                    registro.Add(st18);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 19).Value.ToString()))
                {
                    decimal t19 = decimal.Parse(ws1.Cell(e, 19).Value.ToString()) * 100;
                    string st19 = t19.ToString("0.0");
                    registro.Add(st19);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 20).Value.ToString()))
                {
                    decimal t20 = decimal.Parse(ws1.Cell(e, 20).Value.ToString()) * 100;
                    string st20 = t20.ToString("0.0");
                    registro.Add(st20);
                }
                else
                {
                    registro.Add("0");
                }


                InsertarInformacionFisico(registro);
                registro.Clear();
            }
            /*-------------------------- GENERAL --------------------------------*/
            var ws2 = workbook.Worksheet(1);
            /*-------------------------- ESTRUCTURAR --------------------------------*/

            registro.Add(token);
            registro.Add(usuario);
            registro.Add(ws2.Cell("C4").Value.ToString());
            registro.Add(ws2.Cell("C5").Value.ToString());
            registro.Add(ws2.Cell("C6").Value.ToString());
            registro.Add(ws2.Cell("C7").Value.ToString());
            InsertarInformacionGeneral(registro);
            registro.Clear();

            return "OK";
        }
        /// <summary>
        /// INSERTAR DATOS
        /// </summary>
        /// <param name="Datos"></param>
        private void InsertarInformacionFinanciera(List<String> Datos)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", Datos[0].ToString());
                parametos.Add("IniUsuario", Datos[1].ToString());
                parametos.Add("IfDato0", Datos[2].ToString());
                parametos.Add("IfDato1", Datos[3].ToString());
                parametos.Add("IfDato2", Datos[4].ToString());
                parametos.Add("IfDato3", Datos[5].ToString());
                parametos.Add("IfDato4", Datos[6].ToString());
                parametos.Add("IfDato5", Datos[7].ToString());
                parametos.Add("IfDato6", Datos[8].ToString());
                parametos.Add("IfDato7", Datos[9].ToString());
                parametos.Add("IfDato8", Datos[10].ToString());
                parametos.Add("IfDato9", Datos[11].ToString());
                parametos.Add("IfDato10", Datos[12].ToString());
                parametos.Add("IfDato11", Datos[13].ToString());
                parametos.Add("IfDato12", Datos[14].ToString());
                parametos.Add("IfDato13", Datos[15].ToString());
                parametos.Add("IfDato14", Datos[16].ToString());
                parametos.Add("IfDato15", Datos[17].ToString());
                parametos.Add("IfDato16", Datos[18].ToString());
                parametos.Add("IfDato17", Datos[19].ToString());
                parametos.Add("IfDato18", Datos[20].ToString());
                parametos.Add("IfDato19", Datos[21].ToString());
                ORM.Execute("CAPEX_INS_INFORMACION_FINANCIERA", parametos, commandType: CommandType.StoredProcedure);
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "InsertarInformacionFinanciera, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
            }
        }
        /// <summary>
        /// INSERTAR DATOS DE FINANCIERO RESUMIDA
        /// </summary>
        /// <param name="Datos"></param>
        private void InsertarInformacionFinancieraResumida(List<String> Datos)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", Datos[0].ToString());
                parametos.Add("IniUsuario", Datos[1].ToString());
                parametos.Add("IrDato0", Datos[2].ToString());
                parametos.Add("IrDato1", Datos[3].ToString());
                parametos.Add("IrDato2", Datos[4].ToString());

                ORM.Execute("CAPEX_INS_INFORMACION_FINANCIERA_RESUMIDA", parametos, commandType: CommandType.StoredProcedure);
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "InsertarInformacionFinancieraResumida, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
            }
        }
        /// <summary>
        /// INSERTAR DATOS DE FISICO
        /// </summary>
        /// <param name="Datos"></param>
        private void InsertarInformacionFisico(List<String> Datos)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", Datos[0].ToString());
                parametos.Add("IniUsuario", Datos[1].ToString());
                parametos.Add("FiDato0", Datos[2].ToString());
                parametos.Add("FiDato1", Datos[3].ToString());
                parametos.Add("FiDato2", Datos[4].ToString());
                parametos.Add("FiDato3", Datos[5].ToString());
                parametos.Add("FiDato4", Datos[6].ToString());
                parametos.Add("FiDato5", Datos[7].ToString());
                parametos.Add("FiDato6", Datos[8].ToString());
                parametos.Add("FiDato7", Datos[9].ToString());
                parametos.Add("FiDato8", Datos[10].ToString());
                parametos.Add("FiDato9", Datos[11].ToString());
                parametos.Add("FiDato10", Datos[12].ToString());
                parametos.Add("FiDato11", Datos[13].ToString());
                parametos.Add("FiDato12", Datos[14].ToString());
                parametos.Add("FiDato13", Datos[15].ToString());
                parametos.Add("FiDato14", Datos[16].ToString());
                parametos.Add("FiDato15", Datos[17].ToString());
                parametos.Add("FiDato16", Datos[18].ToString());
                parametos.Add("FiDato17", Datos[19].ToString());
                parametos.Add("FiDato18", Datos[20].ToString());
                parametos.Add("FiDato19", Datos[21].ToString());
                ORM.Execute("CAPEX_INS_INFORMACION_FISICO", parametos, commandType: CommandType.StoredProcedure);
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "InsertarInformacionFisico, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
            }
        }
        /// <summary>
        /// INSERTAR INFORMACION GENERAL DE IMPORTACION
        /// </summary>
        /// <param name="Datos"></param>
        private void InsertarInformacionGeneral(List<String> Datos)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", Datos[0].ToString());
                parametos.Add("IniUsuario", Datos[1].ToString());
                parametos.Add("IgPresupuesto", Datos[2].ToString());
                parametos.Add("IgFechaInicio", Datos[3].ToString());
                parametos.Add("IgTermino", Datos[4].ToString());
                parametos.Add("IgCierre", Datos[5].ToString());

                ORM.Execute("CAPEX_INS_INFORMACION_GENERAL", parametos, commandType: CommandType.StoredProcedure);
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "InsertarInformacionGeneral, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
            }
        }
        /// <summary>
        /// DESPLEGAR DATOS IMPORTACION /INFORMACION FINANCIERA
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>

        public string PoblarVistaPresupuestoFinanciero(string token)
        {
            string Desplegable = String.Empty;
            try
            {
                var resultado = ORM.Query("CAPEX_SEL_INFORMACION_FINANCIERA", new { token }, commandType: CommandType.StoredProcedure).ToList();
                var table = new StringBuilder();
                if (resultado.Count > 0)
                {
                    foreach (var result in resultado)
                    {
                        table.Append("<tr>");
                        table.Append("<td style='text-align:left;background-color:#5c808d;'>" + result.IfDato0 + "</td>");
                        table.Append("<td style='text-align:center;'>" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato2)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;' >" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato3)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;' >" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato4)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;' >" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato5)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;' >" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato6)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;' >" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato7)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;' >" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato8)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;' >" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato9)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;' >" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato10)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;' >" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato11)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;' >" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato12)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;' >" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato13)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;' >" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato14)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;' >" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato15)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;' >" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato16)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;' >" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato17)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;' >" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato18)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("<td style='text-align:center;' >" + String.Format("{0:#,##0.##}", Convert.ToDouble(result.IfDato19)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                        table.Append("</tr>");
                    }
                    Desplegable = table.ToString();
                }
                else
                {
                    Desplegable = "";
                }

            }
            catch (Exception exc)
            {
                Desplegable = null;
                //Desplegable = exc.Message.ToString() + "-----" + exc.StackTrace.ToString();
            }
            return Desplegable.ToString();
        }
        /// <summary>
        ///  DESPLEGAR DATOS IMPORTACION /INFORMACION FISICO
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string PoblarVistaPresupuestoFisico(string token)
        {
            string Desplegable = String.Empty;
            try
            {
                var resultado = ORM.Query("CAPEX_SEL_INFORMACION_FISICO", new { token }, commandType: CommandType.StoredProcedure).ToList();
                var table = new StringBuilder();
                if (resultado.Count > 0)
                {
                    foreach (var result in resultado)
                    {

                        CultureInfo ciCL = new CultureInfo("es-CL", false);

                        table.Append("<tr>");
                        table.Append("<td style='text-align:left;background-color:#5c808d;'>" + result.FiDato0 + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.FiDato2.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.FiDato3.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.FiDato4.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.FiDato5.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.FiDato6.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.FiDato7.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.FiDato8.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.FiDato9.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.FiDato10.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.FiDato11.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.FiDato12.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.FiDato13.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.FiDato14.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.FiDato15.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.FiDato16.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.FiDato17.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.FiDato18.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("<td style='text-align:center;'>" + double.Parse(result.FiDato19.Replace('.', ','), ciCL).ToString().Replace('.', ',') + "</td>");
                        table.Append("</tr>");
                    }
                    Desplegable = table.ToString();
                }
                else
                {
                    Desplegable = "";
                }

            }
            catch (Exception exc)
            {
                Desplegable = null;
                //Desplegable = exc.Message.ToString() + "-----" + exc.StackTrace.ToString();
            }
            return Desplegable.ToString();
        }
        #endregion

        #region "METODOS DOTACION"
        /// <summary>
        /// METODO PARA LISTAR CONTRATOS DE DOTACION
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        public List<Dotacion.DetalleContratosDotacion> ListarContratosDotacion(string Token)
        {
            try
            {
                return ORM.Query<Dotacion.DetalleContratosDotacion>("CAPEX_SEL_DOTACION_CONTRATOS_LISTAR", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ListarContratosDotacion, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Dotacion.DepartamentoDotacion> ListarDepartamentos()
        {
            try
            {
                return ORM.Query<Dotacion.DepartamentoDotacion>("CAPEX_SEL_DEP_GLOSA", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ListarDepartamentos, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Dotacion.Turnos> ListarTurnos()
        {
            try
            {
                return ORM.Query<Dotacion.Turnos>("CAPEX_SEL_TURNOS", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ListarTurnos, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Dotacion.Ubicacion> ListarUbicaciones()
        {
            try
            {
                return ORM.Query<Dotacion.Ubicacion>("CAPEX_SEL_DOTACION_UBICACION", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ListarUbicaciones, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Dotacion.DotacionEECC> ListarTipoEECC()
        {
            try
            {
                return ORM.Query<Dotacion.DotacionEECC>("CAPEX_SEL_DOTACION_EECC", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ListarTipoEECC, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Dotacion.DotacionClasificacion> ListarClasificacion()
        {
            try
            {
                return ORM.Query<Dotacion.DotacionClasificacion>("CAPEX_SEL_DOTACION_CLASIFICACION", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ListarClasificacion, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DatosContratoDotacion"></param>
        /// <returns></returns>
        public string GuardarContratoDotacion(Dotacion.ContratoDotacion DatosContratoDotacion)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", DatosContratoDotacion.IniToken);
                parametos.Add("PidArea", DatosContratoDotacion.PidArea);
                parametos.Add("PidCodigoIniciativa", DatosContratoDotacion.PidCodigoIniciativa);
                parametos.Add("PidNombreProyecto", DatosContratoDotacion.PidNombreProyecto);
                parametos.Add("DotAnn", DatosContratoDotacion.DotAnn);
                parametos.Add("DotSitProyecto", DatosContratoDotacion.DotSitProyecto);
                parametos.Add("DotSitFaena", DatosContratoDotacion.DotSitFaena);
                parametos.Add("DotDepto", DatosContratoDotacion.DotDepto);
                parametos.Add("DotNumContrato", DatosContratoDotacion.DotNumContrato);
                parametos.Add("DotNombEECC", DatosContratoDotacion.DotNombEECC);
                parametos.Add("DotServicio", DatosContratoDotacion.DotServicio);
                parametos.Add("DotSubContrato", DatosContratoDotacion.DotSubContrato);
                parametos.Add("DotCodCentro", DatosContratoDotacion.DotCodCentro);
                parametos.Add("DotTurno", DatosContratoDotacion.DotTurno);
                parametos.Add("DotHoteleria", DatosContratoDotacion.DotHoteleria);
                parametos.Add("DotAlimentacion", DatosContratoDotacion.DotAlimentacion);
                parametos.Add("DotUbicacion", DatosContratoDotacion.DotUbicacion);
                parametos.Add("DotClasificacion", DatosContratoDotacion.DotClasificacion);
                parametos.Add("DotTipoEECC", DatosContratoDotacion.DotTipoEECC);
                parametos.Add("DotTotalDotacion", DatosContratoDotacion.DotTotalDotacion);
                parametos.Add("DotEne", DatosContratoDotacion.DotEne);
                parametos.Add("DotFeb", DatosContratoDotacion.DotFeb);
                parametos.Add("DotMar", DatosContratoDotacion.DotMar);
                parametos.Add("DotAbr", DatosContratoDotacion.DotAbr);
                parametos.Add("DotMay", DatosContratoDotacion.DotMay);
                parametos.Add("DotJun", DatosContratoDotacion.DotJun);
                parametos.Add("DotJul", DatosContratoDotacion.DotJul);
                parametos.Add("DotAgo", DatosContratoDotacion.DotAgo);
                parametos.Add("DotSep", DatosContratoDotacion.DotSep);
                parametos.Add("DotOct", DatosContratoDotacion.DotOct);
                parametos.Add("DotNov", DatosContratoDotacion.DotNov);
                parametos.Add("DotDic", DatosContratoDotacion.DotDic);
                parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                ORM.Query("CAPEX_INS_IDENTIFICACION_DOTACION", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                {
                    return parametos.Get<string>("Respuesta");
                }
                else
                {
                    return "ERROR";
                }
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "GuardarContratoDotacion, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="DatosContratoDotacion"></param>
        /// <returns></returns>
        public string ActualizarContratoDotacion(Dotacion.ContratoDotacion DatosContratoDotacion)
        {
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="DatosPeriodoDotacion"></param>
        /// <returns></returns>
        public string GuardarPeriodosDotacion(string Token, string DatosPeriodoDotacion)
        {
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        public string EliminarContratoDotacion(string Token)
        {
            try
            {
                ORM.Query("CAPEX_DEL_CONTRATO_DOTACION", new { Token }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return "Eliminado";
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "EliminarContratoDotacion, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        #endregion

        #region "METODOS DESCRIPCION DETALLADA"

        /// <remark>
        ///     
        ///     ----------------------------------------------------------------------------
        ///     GRUPO DE METODOS "DESCRIPCION DETALLADA"
        ///     VERSION     0.0.1
        ///     ----------------------------------------------------------------------------
        ///     PROPOSITO
        ///     ----------------------------------------------------------------------------
        ///     IMPLEMENTAR ENTRADA Y SALIDA DE DATOS, ASI COMO OPERACIONES POR MEDIO
        ///     DE PATRON CREACIONAL FACTORY QUE ES PROVISTO POR CAPA DDD DE INFRAESTRUCTURA
        ///     
        /// </remark>


        /// <summary>
        /// METODO PARA GUARDAR DESCRIPCION DETALLADA
        /// </summary>
        /// <param name="DatosDescripcion"></param>
        /// <returns></returns>
        public string GuardarDescripcionDetallada(Descripcion.DescripcionDetallada DatosDescripcion)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniUsuario", DatosDescripcion.IniUsuario);
                parametos.Add("IniToken", DatosDescripcion.IniToken);
                parametos.Add("PddObjetivo", DatosDescripcion.PddObjetivo);
                parametos.Add("PddAlcance", DatosDescripcion.PddAlcance);
                parametos.Add("PddJustificacion", DatosDescripcion.PddJustificacion);
                parametos.Add("PddDescripcion1", DatosDescripcion.PddDescripcion1);
                parametos.Add("PddUnidad1", DatosDescripcion.PddUnidad1);
                parametos.Add("PddActual1", DatosDescripcion.PddActual1);
                parametos.Add("PddTarget1", DatosDescripcion.PddTarget1);
                parametos.Add("PddDescripcion2", DatosDescripcion.PddDescripcion2);
                parametos.Add("PddUnidad2", DatosDescripcion.PddUnidad2);
                parametos.Add("PddActual2", DatosDescripcion.PddActual2);
                parametos.Add("PddTarget2", DatosDescripcion.PddTarget2);
                parametos.Add("PddDescripcion3", DatosDescripcion.PddDescripcion3);
                parametos.Add("PddUnidad3", DatosDescripcion.PddUnidad3);
                parametos.Add("PddActual3", DatosDescripcion.PddActual3);
                parametos.Add("PddTarget3", DatosDescripcion.PddTarget3);
                parametos.Add("PddFechaPostEval", DatosDescripcion.PddFechaPostEval);
                parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                ORM.Query("CAPEX_INS_IDENTIFICACION_DESCRIPCION", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                {
                    return parametos.Get<string>("Respuesta");
                }
                else
                {
                    return "Error";
                }

            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "GuardarDescripcionDetallada, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DatosDescripcion"></param>
        /// <returns></returns>
        public string ActualizarDescripcionDetallada(Descripcion.DescripcionDetallada DatosDescripcion)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniUsuario", DatosDescripcion.IniUsuario);
                parametos.Add("IniToken", DatosDescripcion.IniToken);
                parametos.Add("PddObjetivo", DatosDescripcion.PddObjetivo);
                parametos.Add("PddAlcance", DatosDescripcion.PddAlcance);
                parametos.Add("PddJustificacion", DatosDescripcion.PddJustificacion);
                parametos.Add("PddDescripcion1", DatosDescripcion.PddDescripcion1);
                parametos.Add("PddUnidad1", DatosDescripcion.PddUnidad1);
                parametos.Add("PddActual1", DatosDescripcion.PddActual1);
                parametos.Add("PddTarget1", DatosDescripcion.PddTarget1);
                parametos.Add("PddDescripcion2", DatosDescripcion.PddDescripcion2);
                parametos.Add("PddUnidad2", DatosDescripcion.PddUnidad2);
                parametos.Add("PddActual2", DatosDescripcion.PddActual2);
                parametos.Add("PddTarget2", DatosDescripcion.PddTarget2);
                parametos.Add("PddDescripcion3", DatosDescripcion.PddDescripcion3);
                parametos.Add("PddUnidad3", DatosDescripcion.PddUnidad3);
                parametos.Add("PddActual3", DatosDescripcion.PddActual3);
                parametos.Add("PddTarget3", DatosDescripcion.PddTarget3);
                parametos.Add("PddFechaPostEval", DatosDescripcion.PddFechaPostEval);
                parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                ORM.Query("CAPEX_UPD_IDENTIFICACION_DESCRIPCION", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                {
                    return parametos.Get<string>("Respuesta");
                }
                else
                {
                    return "Error";
                }

            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ActualizarDescripcionDetallada, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }

        }
        #endregion

        #region "METODOS EVALUACION ECONOMICA"

        /// <remark>
        ///     
        ///     ----------------------------------------------------------------------------
        ///     GRUPO DE METODOS "EVALUACION ECONOMICA"
        ///     VERSION     0.0.1
        ///     ----------------------------------------------------------------------------
        ///     PROPOSITO
        ///     ----------------------------------------------------------------------------
        ///     IMPLEMENTAR ENTRADA Y SALIDA DE DATOS, ASI COMO OPERACIONES POR MEDIO
        ///     DE PATRON CREACIONAL FACTORY QUE ES PROVISTO POR CAPA DDD DE INFRAESTRUCTURA
        ///     
        /// </remark>


        /// <summary>
        /// METODO PARA GUARDAR EVALUACION ECONOMICA
        /// </summary>
        /// <param name="DatosDescripcion"></param>
        /// <returns></returns>
        public string GuardarEvaluacionEconomica(EvaluacionEconomica.GuardarEvaluacion DatosEvaluacion)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniUsuario", DatosEvaluacion.IniUsuario);
                parametos.Add("IniToken", DatosEvaluacion.IniToken);
                parametos.Add("EveVan", DatosEvaluacion.EveVan);
                parametos.Add("EveIvan", DatosEvaluacion.EveIvan);
                parametos.Add("EvePayBack", DatosEvaluacion.EvePayBack);
                parametos.Add("EveVidaUtil", DatosEvaluacion.EveVidaUtil);
                parametos.Add("EveTipoCambio", DatosEvaluacion.EveTipoCambio);
                string EveTir = "0";
                if (DatosEvaluacion.EveTir != null && !string.IsNullOrEmpty(DatosEvaluacion.EveTir))
                {
                    EveTir = DatosEvaluacion.EveTir.Trim();
                    EveTir = EveTir.Replace("%", "");
                    if (EveTir.IndexOf(",") > 0)
                    {
                        EveTir = EveTir.Replace(",", ".");
                    }
                }
                parametos.Add("EveTir", EveTir);
                parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                ORM.Query("CAPEX_INS_IDENTIFICACION_EVAL_ECONOMICA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                {
                    return parametos.Get<string>("Respuesta");
                }
                else
                {
                    return "Error";
                }

            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "GuardarEvaluacionEconomica, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DatosEvaluacion"></param>
        /// <returns></returns>
        public string ActualizarEvaluacionEconomica(EvaluacionEconomica.GuardarEvaluacion DatosEvaluacion)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniUsuario", DatosEvaluacion.IniUsuario);
                parametos.Add("IniToken", DatosEvaluacion.IniToken);
                parametos.Add("EveVan", DatosEvaluacion.EveVan);
                parametos.Add("EveIvan", DatosEvaluacion.EveIvan);
                parametos.Add("EvePayBack", DatosEvaluacion.EvePayBack);
                parametos.Add("EveVidaUtil", DatosEvaluacion.EveVidaUtil);
                parametos.Add("EveTipoCambio", DatosEvaluacion.EveTipoCambio);
                string EveTir = "0";
                if (DatosEvaluacion.EveTir != null && !string.IsNullOrEmpty(DatosEvaluacion.EveTir))
                {
                    EveTir = DatosEvaluacion.EveTir.Trim();
                    EveTir = EveTir.Replace("%", "");
                    if (EveTir.IndexOf(",") > 0)
                    {
                        EveTir = EveTir.Replace(",", ".");
                    }
                }
                parametos.Add("EveTir", EveTir);
                parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                ORM.Query("CAPEX_UPD_IDENTIFICACION_EVAL_ECONOMICA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                {
                    return parametos.Get<string>("Respuesta");
                }
                else
                {
                    return "Error";
                }

            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ActualizarEvaluacionEconomica, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }

        }
        #endregion

        #region "METODOS EVALUACION RIESGO"

        /// <remark>
        ///     
        ///     ----------------------------------------------------------------------------
        ///     GRUPO DE METODOS "EVALUACION RIESGO"
        ///     VERSION     0.0.1
        ///     ----------------------------------------------------------------------------
        ///     PROPOSITO
        ///     ----------------------------------------------------------------------------
        ///     IMPLEMENTAR ENTRADA Y SALIDA DE DATOS, ASI COMO OPERACIONES POR MEDIO
        ///     DE PATRON CREACIONAL FACTORY QUE ES PROVISTO POR CAPA DDD DE INFRAESTRUCTURA
        ///     
        /// </remark>


        /// <summary>
        /// METODO PARA GUARDAR EVALUACION RIESGO
        /// </summary>
        /// <param name="DatosDescripcion"></param>
        /// <returns></returns>
        public string GuardarEvaluacionRiesgo(EvaluacionRiesgo.GuardarEvaluacion DatosEvaluacion)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniUsuario", DatosEvaluacion.IniUsuario);
                parametos.Add("IniToken", DatosEvaluacion.IniToken);
                parametos.Add("EriProb1", DatosEvaluacion.EriProb1);
                parametos.Add("EriImp1", DatosEvaluacion.EriImp1);
                parametos.Add("EriRies1", DatosEvaluacion.EriRies1);
                parametos.Add("EriClas1", DatosEvaluacion.EriClas1);
                parametos.Add("EriMFL1", DatosEvaluacion.EriMFL1);

                parametos.Add("EriProb2", DatosEvaluacion.EriProb2);
                parametos.Add("EriImp2", DatosEvaluacion.EriImp2);
                parametos.Add("EriRies2", DatosEvaluacion.EriRies2);
                parametos.Add("EriClas2", DatosEvaluacion.EriClas2);
                parametos.Add("EriMFL2", DatosEvaluacion.EriMFL2);
                parametos.Add("EriItemMR", DatosEvaluacion.EriItemMR);

                parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                ORM.Query("CAPEX_INS_IDENTIFICACION_EVAL_RIESGO", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                {
                    return parametos.Get<string>("Respuesta");
                }
                else
                {
                    return "Error";
                }

            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "GuardarEvaluacionRiesgo, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DatosEvaluacion"></param>
        /// <returns></returns>
        public string ActualizarEvaluacionRiesgo(EvaluacionRiesgo.GuardarEvaluacion DatosEvaluacion)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniUsuario", DatosEvaluacion.IniUsuario);
                parametos.Add("IniToken", DatosEvaluacion.IniToken);
                parametos.Add("EriProb1", DatosEvaluacion.EriProb1);
                parametos.Add("EriImp1", DatosEvaluacion.EriImp1);
                parametos.Add("EriRies1", DatosEvaluacion.EriRies1);
                parametos.Add("EriClas1", DatosEvaluacion.EriClas1);
                parametos.Add("EriMFL1", DatosEvaluacion.EriMFL1);

                parametos.Add("EriProb2", DatosEvaluacion.EriProb2);
                parametos.Add("EriImp2", DatosEvaluacion.EriImp2);
                parametos.Add("EriRies2", DatosEvaluacion.EriRies2);
                parametos.Add("EriClas2", DatosEvaluacion.EriClas2);
                parametos.Add("EriMFL2", DatosEvaluacion.EriMFL2);
                parametos.Add("EriItemMR", DatosEvaluacion.EriItemMR);

                parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                ORM.Query("CAPEX_UPD_IDENTIFICACION_EVAL_RIESGO", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                {
                    return parametos.Get<string>("Respuesta");
                }
                else
                {
                    return "Error";
                }

            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ActualizarEvaluacionRiesgo, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }

        }
        #endregion

        #region "CAPEX HITO"
        /// <summary>
        /// POBLAR VISTA PRINCIPAL DE HITOS
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string PoblarVistaHitos(string token)
        {
            string Desplegable = String.Empty;
            try
            {
                var resultado = ORM.Query("CAPEX_SEL_RESUMEN_FINANCIERO", new { token }, commandType: CommandType.StoredProcedure).ToList();
                var table = new StringBuilder();
                var contador = 1;
                var fondocelda = "transparent";

                if (resultado.Count > 0)
                {
                    foreach (var result in resultado)
                    {
                        if (contador < 7)
                        {
                            table.Append("<tr>");
                            if (contador == 4)
                            {
                                table.Append("<td style='height:20px;text-align:left;font-weight:normal;background-color:#5c808d;'> Administración o Costos Dueños  (<span id='HitosCostos'></span>%)</td>");
                            }
                            else if (contador == 5)
                            {
                                table.Append("<td style='height:20px;text-align:left;font-weight:normal;background-color:#5c808d;'> Contingencia (<span id='HitosContingencia'></span>%)</td>");
                            }
                            else if (contador == 6)
                            {
                                table.Append("<td style='height:20px;text-align:left;font-weight:normal;background-color:#5c808d;'> Total Presupuesto</td>");
                            }
                            else
                            {
                                table.Append("<td style='height:20px;text-align:left;font-weight:normal;background-color:#5c808d;'> " + result.Fase + "</td>");
                            }

                            if (contador < 6)
                            {
                                table.Append("<td style='height:20px;font-weight:normal;color:#f0f0f0; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.AnterioresConvert).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                                table.Append("<td style='height:20px;font-weight:normal;color:#f0f0f0; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.ActualConvert).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                                table.Append("<td style='height:20px;font-weight:normal;color:#f0f0f0; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.Posteriores).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                                table.Append("<td style='height:20px;font-weight:bold;color:#fff; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.TotalCapexConvert).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "<input type='hidden' id='HitosTotalCapex" + contador + "' value='" + String.Format("{0:#,##0.##}", result.TotalCapexConvert).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "'></td>");
                            }
                            else
                            {
                                table.Append("<td style='height:20px;font-weight:bold;color:#fff; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.AnterioresConvert).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                                table.Append("<td style='height:20px;font-weight:bold;color:#fff; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.ActualConvert).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                                table.Append("<td style='height:20px;font-weight:bold;color:#fff; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.Posteriores).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                                table.Append("<td style='height:20px;font-weight:bold;color:#fff; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.TotalCapexConvert).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "<input type='hidden' id='HitosTotalCapex" + contador + "' value='" + String.Format("{0:#,##0.##}", result.TotalCapexConvert).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "'></td>");
                            }
                            table.Append("</tr>");
                        }
                        contador++;
                    }
                    Desplegable = table.ToString();
                    table = null;
                }
                else
                {
                    Desplegable = "";
                    table = null;
                }

            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "PoblarVistaHitos, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                Desplegable = "ERROR";
            }
            return Desplegable.ToString();
        }

        /// <summary>
        /// PROVEER INFORMACION DE RESUMEN FINANCIERO
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public List<Hito.HitoResumen> PoblarVistaHitosResumen(string token)
        {
            try
            {
                return ORM.Query<Hito.HitoResumen>("CAPEX_SEL_IMPORTACION_RESUMEN", new { token }, commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "PoblarVistaHitosResumen, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// PROVEER INFORMACION DE DETALLE FINANCIERO
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public List<Hito.HitoDetalle> PoblarVistaHitosDetalle(string token)
        {
            try
            {
                return ORM.Query<Hito.HitoDetalle>("CAPEX_SEL_IMPORTACION_GENERAL", new { token }, commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "PoblarVistaHitosDetalle, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// GUARDAR CAPEX HITOS
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        public string GuardarHito(Hito.HitoGuardar Datos)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", Datos.IniToken);
                parametos.Add("IniUsuario", Datos.IniUsuario);
                parametos.Add("HitNacExt", Datos.HitNacExt);
                parametos.Add("HitSAP", Datos.HitSAP);
                parametos.Add("HitCI", Datos.HitCI);
                parametos.Add("HitCA", Datos.HitCA);
                parametos.Add("HitOPR", Datos.HitOPR);
                parametos.Add("HitPE", Datos.HitPE);
                parametos.Add("HitDIRCEN", Datos.HitDIRCEN);
                parametos.Add("HitDirPLC", Datos.HitDirPLC);

                parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                ORM.Query("CAPEX_INS_IDENTIFICACION_HITO", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                {
                    return parametos.Get<string>("Respuesta");
                }
                else
                {
                    return "Error";
                }

            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "GuardarHito, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        public string ActualizarHito(Hito.HitoGuardar Datos)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", Datos.IniToken);
                parametos.Add("IniUsuario", Datos.IniUsuario);
                parametos.Add("HitNacExt", Datos.HitNacExt);
                parametos.Add("HitSAP", Datos.HitSAP);
                parametos.Add("HitCI", Datos.HitCI);
                parametos.Add("HitCA", Datos.HitCA);
                parametos.Add("HitOPR", Datos.HitOPR);
                parametos.Add("HitPE", Datos.HitPE);
                parametos.Add("HitDIRCEN", Datos.HitDIRCEN);
                parametos.Add("HitDirPLC", Datos.HitDirPLC);

                parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                ORM.Query("CAPEX_UPD_IDENTIFICACION_HITO", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                {
                    return parametos.Get<string>("Respuesta");
                }
                else
                {
                    return "Error";
                }

            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ActualizarHito, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }
        /// <summary>
        /// ENVIA INICIATIVA A WORKFLOW DEL SISTEMA
        /// </summary>
        /// <param name="IniToken"></param>
        /// <param name="WrfUsuario"></param>
        /// <param name="WrfObservacion"></param>
        /// <returns></returns>
        public string EnviarIniciativa(string IniToken, string WrfUsuario, string WrfObservacion, string Rol)
        {
            try
            {
                ORM.Execute("CAPEX_INS_IDENTIFICACION_ENVIAR", new { IniToken, WrfUsuario, WrfObservacion, Rol }, commandType: CommandType.StoredProcedure);
                return "Enviado";
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "EnviarIniciativa, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return "Error";
            }
        }

        #endregion

        #region "ADJUNTOS"
        /// <summary>
        /// MOSTRAR ADJUNTOS RECOLECTADOS A LO LARGO DE LA CREACIOND DE LA INICIATIVA
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string VerAdjuntos(string token)
        {
            string Desplegable = string.Empty;
            string Estado = string.Empty;
            try
            {
                var categorias = ORM.Query("CAPEX_SEL_ADJUNTOS_VIGENTES", new { token }, commandType: CommandType.StoredProcedure).ToList();
                var items = ORM.Query("CAPEX_SEL_ADJUNTOS_ITEMS_VIGENTES", new { token }, commandType: CommandType.StoredProcedure).ToList();
                var arbol = new StringBuilder();
                var contador = 1;

                if (categorias.Count > 0)
                {
                    foreach (var categoria in categorias)
                    {
                        arbol.Append("<li>");
                        arbol.Append("<i class='fa fa-folder fa-2x'></i> <a href='#'>" + categoria.ParPaso + "</a>");
                        foreach (var item in items)
                        {
                            if (categoria.ParPaso == item.ParPaso)
                            {
                                if ("Evaluacion-Economica".Equals(item.ParPaso, StringComparison.OrdinalIgnoreCase))
                                {
                                    arbol.Append("<ul><li><a style='cursor:default;' href='#link" + contador + "'><i class='fa fa-file fa-2x' style='color:#79a557; margin-left:10px;'></i> " + ((!string.IsNullOrEmpty(item.ParNombreFinal)) ? item.ParNombreFinal : item.ParNombre) + "</a> | <span onclick='FNDescargarAdjuntoFinal(" + Convert.ToChar(34) + item.ParToken + Convert.ToChar(34) + ")' style='cursor: pointer'> Descargar </span> | <span onclick='FNModalEliminarAdjuntoEE(" + Convert.ToChar(34) + item.ParToken + Convert.ToChar(34) + ")' style='cursor: pointer'> Eliminar </span></li></ul>");
                                }
                                else if ("Evaluacion-Riesgo".Equals(item.ParPaso, StringComparison.OrdinalIgnoreCase))
                                {
                                    arbol.Append("<ul><li><a style='cursor:default;' href='#link" + contador + "'><i class='fa fa-file fa-2x' style='color:#79a557; margin-left:10px;'></i> " + ((!string.IsNullOrEmpty(item.ParNombreFinal)) ? item.ParNombreFinal : item.ParNombre) + "</a> | <span onclick='FNDescargarAdjuntoFinal(" + Convert.ToChar(34) + item.ParToken + Convert.ToChar(34) + ")' style='cursor: pointer'> Descargar </span> | <span onclick='FNModalEliminarAdjuntoER(" + Convert.ToChar(34) + item.ParToken + Convert.ToChar(34) + ")' style='cursor: pointer'> Eliminar </span></li></ul>");
                                }
                                else if ("Presupuesto".Equals(item.ParPaso, StringComparison.OrdinalIgnoreCase))
                                {
                                    arbol.Append("<ul><li><a style='cursor:default;' href='#link" + contador + "'><i class='fa fa-file fa-2x' style='color:#79a557; margin-left:10px;'></i> " + ((!string.IsNullOrEmpty(item.ParNombreFinal)) ? item.ParNombreFinal : item.ParNombre) + "</a> | <span onclick='FNDescargarAdjuntoFinal(" + Convert.ToChar(34) + item.ParToken + Convert.ToChar(34) + ")' style='cursor: pointer'> Descargar </span></li></ul>");
                                }
                                else
                                {
                                    arbol.Append("<ul><li><a style='cursor:default;' href='#link" + contador + "'><i class='fa fa-file fa-2x' style='color:#79a557; margin-left:10px;'></i> " + ((!string.IsNullOrEmpty(item.ParNombreFinal)) ? item.ParNombreFinal : item.ParNombre) + "</a> | <span onclick='FNDescargarAdjuntoFinal(" + Convert.ToChar(34) + item.ParToken + Convert.ToChar(34) + ")' style='cursor: pointer'> Descargar </span> | <span onclick='FNModalEliminarAdjunto(" + Convert.ToChar(34) + item.ParToken + Convert.ToChar(34) + ")' style='cursor: pointer'> Eliminar </span></li></ul>");
                                }
                            }
                        }
                        arbol.Append("</li>");
                        contador++;
                    }

                    Desplegable = arbol.ToString();
                    arbol = null;
                }
                else
                {
                    Desplegable = "";
                }

            }
            catch (Exception exc)
            {
                ExceptionResult = AppModule + "VerAdjuntos, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return "Error";
            }
            return Desplegable.ToString();
        }


        /// <summary>
        /// SELECCIONAR REGISTRO TEMPORAL DE ARCHIVO ADJUNTO
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Identificacion.Adjunto SeleccionarAdjunto(string token)
        {
            try
            {
                return ORM.Query<Identificacion.Adjunto>("CAPEX_SEL_ADJUNTO_ITEM", new { @Token = token }, commandType: CommandType.StoredProcedure).SingleOrDefault();
            }
            catch (Exception exc)
            {
                ExceptionResult = AppModule + "SeleccionarAdjunto, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        public Identificacion.DocumentoCategoria SeleccionarDocumentoBiblioteca(string token)
        {
            try
            {
                return ORM.Query<Identificacion.DocumentoCategoria>("CAPEX_SEL_DOCUMENTACION_BY_TOKEN", new { @DocToken = token }, commandType: CommandType.StoredProcedure).SingleOrDefault();
            }
            catch (Exception exc)
            {
                ExceptionResult = AppModule + "SeleccionarDocumentoBiblioteca, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        /// <summary>
        /// ELIMINAR REGISTRO TEMPORAL DE ARCHIVO ADJUNTO
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string EliminarAdjuntoVigente(string token, string usuario)
        {
            try
            {
                ORM.Execute("CAPEX_DEL_ADJUNTO_VIGENTE", new { @Token = token, @Usuario = usuario }, commandType: CommandType.StoredProcedure);
                return "Eliminado";
            }
            catch (Exception exc)
            {
                ExceptionResult = AppModule + "VerAdjuntos, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return "Error";
            }
        }


        public string EliminarAdjuntoVigenteConEvaluacionEconomica(string IniToken, string ParToken, string usuario)
        {
            try
            {
                ORM.Execute("CAPEX_DEL_ADJUNTO_VIGENTE_EVALUACION_ECONOMICA", new { @IniToken = IniToken, @ParToken = ParToken, @Usuario = usuario }, commandType: CommandType.StoredProcedure);
                return "Eliminado";
            }
            catch (Exception exc)
            {
                ExceptionResult = AppModule + "VerAdjuntos, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return "Error";
            }
        }

        public string EliminarAdjuntoVigenteConEvaluacionRiesgo(string IniToken, string ParToken, string usuario)
        {
            try
            {
                ORM.Execute("CAPEX_DEL_ADJUNTO_VIGENTE_EVALUACION_RIESGO", new { @IniToken = IniToken, @ParToken = ParToken, @Usuario = usuario }, commandType: CommandType.StoredProcedure);
                return "Eliminado";
            }
            catch (Exception exc)
            {
                ExceptionResult = AppModule + "VerAdjuntos, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return "Error";
            }
        }

        /// <summary>
        /// ELIMINAR REGISTRO TEMPORAL DE ARCHIVO ADJUNTO
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string EliminarAdjunto(string token)
        {
            try
            {
                ORM.Execute("[CAPEX_DEL_ADJUNTO]", new { @Token = token }, commandType: CommandType.StoredProcedure);
                return "Eliminado";
            }
            catch (Exception exc)
            {
                ExceptionResult = AppModule + "VerAdjuntos, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return "Error";
            }
        }

        /// <summary>
        /// SELECCIONAR REGISTRO TEMPORAL DE ARCHIVO ADJUNTO
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Identificacion.Adjunto SeleccionarAdjuntoPorTokenYPaso(string IniToken, string ParPaso)
        {
            try
            {
                return ORM.Query<Identificacion.Adjunto>("CAPEX_SEL_ADJUNTO_ITEM_POR_TOKEN_PARPASO", new { @IniToken = IniToken, @ParPaso = ParPaso }, commandType: CommandType.StoredProcedure).SingleOrDefault();
            }
            catch (Exception exc)
            {
                ExceptionResult = AppModule + "SeleccionarAdjuntoPorTokenYPaso, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        /// <summary>
        /// SELECCIONAR REGISTRO TEMPORAL DE ARCHIVO ADJUNTO
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Identificacion.Adjunto SeleccionarOtroAdjuntoPorTokenYPaso(string IniToken, string ParToken, string ParPaso)
        {
            try
            {
                return ORM.Query<Identificacion.Adjunto>("CAPEX_SEL_ADJUNTO_ITEM_POR_TOKEN_PARPASO", new { @IniToken = IniToken, ParToken = ParToken, @ParPaso = ParPaso }, commandType: CommandType.StoredProcedure).SingleOrDefault();
            }
            catch (Exception exc)
            {
                ExceptionResult = AppModule + "SeleccionarOtroAdjuntoPorTokenYPaso, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }



        #endregion

        #region "GLOBALES"
        /// <summary>
        /// REGISTRAR  ARCHIVOS EN DB
        /// </summary>
        /// <param name="IniToken"></param>
        /// <param name="ParUsuario"></param>
        /// <param name="ParNombre"></param>
        /// <param name="ParPaso"></param>
        /// <param name="ParCaso"></param>
        /// <returns></returns>
        public string RegistrarArchivo(string IniToken, string ParUsuario, string ParNombre, string ParPaso, string ParCaso)
        {
            try
            {
                ORM.Query("CAPEX_INS_REGISTRAR_ARCHIVO", new { IniToken, ParUsuario, ParNombre, ParPaso, ParCaso }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return "Registrado";
            }
            catch (Exception exc)
            {
                ExceptionResult = AppModule + "RegistrarArchivo, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return "Error";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Identificacion.MatrizRiesgo> ListarMatrizRiesgo()
        {
            try
            {
                return ORM.Query<Identificacion.MatrizRiesgo>("CAPEX_SEL_MATRIZ_RIESGO", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "ListarMatrizRiesgo, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        #endregion
    }

}
