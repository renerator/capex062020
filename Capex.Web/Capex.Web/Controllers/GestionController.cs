using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapexIdentity.Utilities;
using CapexInfraestructure.Bll.Entities.Gestion;
using CapexInfraestructure.Bll.Business.Gestion;
using CapexInfraestructure.Bll.Entities.Planificacion;
using CapexInfraestructure.Bll.Business.Planificacion;
using CapexInfraestructure.Bll.Factory;
using CapexInfraestructure.Utilities;
using Newtonsoft.Json;
using System.Web.Caching;
using System.IO;
using static System.Net.WebRequestMethods;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Utils = CapexInfraestructure.Utilities.Utils;
using System.Globalization;

namespace Capex.Web.Controllers
{
    [AuthorizeAdminOrMember]
    [RoutePrefix("Gestion")]
    public class GestionController : Controller
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
         * RESPONABILIDAD   : IMPLEMENTACION
         * TIPO             : CONTROLLER MVC
         * DESARROLLADO POR : PMO360
         * FECHA            : 2018
         * VERSION          : 0.0.1
         * 
         * 
         */

        /// <remark>
        ///     
        ///     ----------------------------------------------------------------------------
        ///     CONTROLADOR "GestionController" 
        ///     VERSION     0.0.1
        ///     ----------------------------------------------------------------------------
        ///     PROPOSITO
        ///     ----------------------------------------------------------------------------
        ///     IMPLEMENTAR ENTRADA Y SALIDA DE DATOS, ASI COMO OPERACIONES POR MEDIO
        ///     DE PATRON CREACIONAL FACTORY QUE ES PROVISTO POR CAPA DDD DE INFRAESTRUCTURA
        ///     
        /// </remark>



        #region "PROPIEDADES"
        private List<string> Listar { get; set; }
        private string JsonResponse { get; set; }
        private string ExceptionResult { get; set; }
        #endregion

        #region "CONSTANTES"
        #endregion

        #region "CAMPOS"
        //IDENTIFICACION
        //public static PlanificacionFactory FactoryPlanificacion;
        //public static IPlanificacion IPlanificacion;
        //public SqlConnection ORM;
        #endregion

        #region "CONSTRUCTOR"
        public GestionController()
        {
            ////IDENTIFICACION
            //FactoryPlanificacion = new PlanificacionFactory();
            JsonResponse = string.Empty;
            //ORM = CapexInfraestructure.Utilities.Utils.Conectar();
        }
        #endregion

        #region "METODOS COMUNES"
        /// <summary>
        /// METODO VISTA INICIAL
        /// </summary>
        /// <returns></returns>
        [Route]
        public ActionResult Index()
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                var tipoIniciativaSeleccionado = Request.QueryString["tipoIniciativaSeleccionado"];

                if (string.IsNullOrEmpty(tipoIniciativaSeleccionado))
                {
                    tipoIniciativaSeleccionado = "0";
                }
                Session["tipoIniciativaSeleccionado"] = tipoIniciativaSeleccionado;
                var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    //
                    // REDIRECCIONAR A LA VISTA Y CONTROLADOR INICIAL PARA CADA ROL
                    //
                    if (rol.Contains("Gestor"))
                    {
                        return RedirectToAction("Index", "GestionResumen");
                    }
                    else if (rol.Contains("Sponsor"))
                    {
                        return RedirectToAction("Index", "GestionVisacion");
                    }
                    else if (rol.Contains("Administrador1"))
                    {
                        //return RedirectToAction("Index", "GestionIngresada");
                        return RedirectToAction("Index", "GestionVisacion");
                    }
                    else if (rol.Contains("Revisor"))
                    {
                        return RedirectToAction("Index", "GestionEnRevision");
                    }
                    else if (rol.Contains("Administrador2"))
                    {
                        //return RedirectToAction("Index", "GestionListaAprobacion");
                        return RedirectToAction("Index", "GestionIngresada");
                    }
                    else if (rol.Contains("Administrador3"))
                    {
                        //return RedirectToAction("Index", "GestionAprobacionAmsa");
                        return RedirectToAction("Index", "GestionEnRevision");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Panel");
                    }

                }
            }
        }
        /// <summary>
        /// METODO CONTADORES
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="cual"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ObtenerContador(string usuario, string cual)
        {
            string sql = string.Empty;
            string rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
            var tipoIniciativaSeleccionado = Convert.ToString(Session["tipoIniciativaSeleccionado"]);
            if (string.IsNullOrEmpty(tipoIniciativaSeleccionado))
            {
                tipoIniciativaSeleccionado = "0";
            }

            switch (cual)
            {
                case "RESUMEN":
                    if (!rol.Contains("Administrador1") && !rol.Contains("Administrador2") && !rol.Contains("Administrador3"))
                    {
                        if (tipoIniciativaSeleccionado.Equals("0"))
                        {
                            sql = "SELECT COUNT(DISTINCT([IdIni])) AS NumIniciativas FROM [dbo].[CAPEX_INICIATIVA] INI WHERE INI.[IniUsuarioOwner]='" + usuario + "' AND NOT EXISTS (SELECT * FROM [dbo].[CAPEX_FLUJO] FLU2 WHERE INI.[IniToken] = FLU2.[IniToken] AND FLU2.[WrfEstadoActual] = 11)";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("1")) //PRESUPUESTO AND [IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(2,4)
                        {
                            sql = "SELECT COUNT(DISTINCT([IdIni])) AS NumIniciativas FROM [dbo].[CAPEX_INICIATIVA] INI WHERE INI.[IniUsuarioOwner]='" + usuario + "' AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(2,4)) AND NOT EXISTS (SELECT * FROM [dbo].[CAPEX_FLUJO] FLU2 WHERE INI.[IniToken] = FLU2.[IniToken] AND FLU2.[WrfEstadoActual] = 11)";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("2")) //CASO BASE AND [IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(1,3)
                        {
                            sql = "SELECT COUNT(DISTINCT([IdIni])) AS NumIniciativas FROM [dbo].[CAPEX_INICIATIVA] INI WHERE INI.[IniUsuarioOwner]='" + usuario + "' AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(1,3)) AND NOT EXISTS (SELECT * FROM [dbo].[CAPEX_FLUJO] FLU2 WHERE INI.[IniToken] = FLU2.[IniToken] AND FLU2.[WrfEstadoActual] = 11)";
                        }
                    }
                    else
                    {
                        if (tipoIniciativaSeleccionado.Equals("0"))
                        {
                            sql = "SELECT COUNT(DISTINCT([IdIni])) AS NumIniciativas FROM [dbo].[CAPEX_INICIATIVA] INI WHERE NOT EXISTS (SELECT * FROM [dbo].[CAPEX_FLUJO] FLU2 WHERE INI.[IniToken] = FLU2.[IniToken] AND FLU2.[WrfEstadoActual] = 11)";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("1"))
                        {
                            sql = "SELECT COUNT(DISTINCT([IdIni])) AS NumIniciativas FROM [dbo].[CAPEX_INICIATIVA] INI WHERE INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(2,4)) AND NOT EXISTS (SELECT * FROM [dbo].[CAPEX_FLUJO] FLU2 WHERE INI.[IniToken] = FLU2.[IniToken] AND FLU2.[WrfEstadoActual] = 11)";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("2"))
                        {
                            sql = "SELECT COUNT(DISTINCT([IdIni])) AS NumIniciativas FROM [dbo].[CAPEX_INICIATIVA] INI WHERE INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(1,3)) AND NOT EXISTS (SELECT * FROM [dbo].[CAPEX_FLUJO] FLU2 WHERE INI.[IniToken] = FLU2.[IniToken] AND FLU2.[WrfEstadoActual] = 11)";
                        }
                    }
                    break;
                case "DESARROLLO":
                    if (!rol.Contains("Administrador1") && !rol.Contains("Administrador2") && !rol.Contains("Administrador3"))
                    {
                        if (tipoIniciativaSeleccionado.Equals("0"))
                        {
                            sql = "SELECT COUNT(DISTINCT([IdPid])) AS NumIniciativas FROM [dbo].[CAPEX_INICIATIVA] INI, [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN WHERE INI.IniToken = IDEN.PidToken AND IDEN.[PidEstadoFlujo] = 0 AND IDEN.[PidEstado] = 1 AND INI.[IniUsuarioOwner]='" + usuario + "'";
                            //sql = "SELECT COUNT([IdIni]) AS NumIniciativas FROM [dbo].[CAPEX_INICIATIVA] WHERE [IniEstadoFlujo]=0 AND [IniEstado]=1 AND [IniUsuario]='" + usuario + "'";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("1"))
                        {
                            sql = "SELECT COUNT(DISTINCT([IdPid])) AS NumIniciativas FROM [dbo].[CAPEX_INICIATIVA] INI, [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN WHERE INI.IniToken = IDEN.PidToken AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(2,4)) AND IDEN.[PidEstadoFlujo] = 0 AND IDEN.[PidEstado] = 1 AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("2"))
                        {
                            sql = "SELECT COUNT(DISTINCT([IdPid])) AS NumIniciativas FROM [dbo].[CAPEX_INICIATIVA] INI, [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN WHERE INI.IniToken = IDEN.PidToken AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(1,3)) AND IDEN.[PidEstadoFlujo] = 0 AND IDEN.[PidEstado] = 1 AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                    }
                    else
                    {
                        if (tipoIniciativaSeleccionado.Equals("0"))
                        {
                            sql = "SELECT COUNT(DISTINCT([IdPid])) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN WHERE IDEN.[PidEstadoFlujo] = 0 AND IDEN.[PidEstado] = 1";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("1"))
                        {
                            sql = "SELECT COUNT(DISTINCT([IdPid])) AS NumIniciativas FROM [dbo].[CAPEX_INICIATIVA] INI, [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN WHERE INI.IniToken = IDEN.PidToken AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(2,4)) AND IDEN.[PidEstadoFlujo] = 0 AND IDEN.[PidEstado] = 1";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("2"))
                        {
                            sql = "SELECT COUNT(DISTINCT([IdPid])) AS NumIniciativas FROM [dbo].[CAPEX_INICIATIVA] INI, [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN WHERE INI.IniToken = IDEN.PidToken AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(1,3)) AND IDEN.[PidEstadoFlujo] = 0 AND IDEN.[PidEstado] = 1";
                        }
                    }
                    break;

                case "VISACION":
                    if (!rol.Contains("Administrador1") && !rol.Contains("Administrador2") && !rol.Contains("Administrador3"))
                    {
                        if (tipoIniciativaSeleccionado.Equals("0"))
                        {
                            sql = "SELECT COUNT(DISTINCT([IdPid])) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 1 AND FLU.[WrfEstado] = 1 AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("1"))
                        {
                            sql = "SELECT COUNT(DISTINCT([IdPid])) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 1 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(2,4)) AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("2"))
                        {
                            sql = "SELECT COUNT(DISTINCT([IdPid])) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 1 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(1,3)) AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                    }
                    else
                    {
                        if (tipoIniciativaSeleccionado.Equals("0")) //AMBAS
                        {
                            sql = "SELECT COUNT(DISTINCT([IdPid])) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.WrfEstadoActual = 1 AND FLU.WrfEstado = 1";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("1")) //PRESUPUESTO
                        {
                            sql = "SELECT COUNT(DISTINCT([IdPid])) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.WrfEstadoActual = 1 AND FLU.WrfEstado = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(2,4))";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("2")) //CASO BASE
                        {
                            sql = "SELECT COUNT(DISTINCT([IdPid])) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.WrfEstadoActual = 1 AND FLU.WrfEstado = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(1,3))";
                        }
                    }
                    break;

                //sql = "SELECT COUNT([IdIni]) AS NumIniciativas FROM [dbo].[CAPEX_INICIATIVA] WHERE [IniEstadoFlujo]=1 AND [IniEstado]=1  AND [IniUsuario]='" + usuario + "'";
                //break;

                case "INGRESO":
                    if (!rol.Contains("Administrador1") && !rol.Contains("Administrador2") && !rol.Contains("Administrador3"))
                    {
                        if (tipoIniciativaSeleccionado.Equals("0"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 3 AND FLU.[WrfEstado] = 1 AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("1"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 3 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(2,4)) AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("2"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 3 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(1,3)) AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                    }
                    else
                    {
                        if (tipoIniciativaSeleccionado.Equals("0"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 3 AND FLU.[WrfEstado] = 1";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("1"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 3 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(2,4))";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("2"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 3 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(1,3))";
                        }
                    }
                    break;
                case "OBSERVADAS":
                    if (!rol.Contains("Administrador1") && !rol.Contains("Administrador2") && !rol.Contains("Administrador3"))
                    {
                        if (tipoIniciativaSeleccionado.Equals("0"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 2 AND FLU.[WrfEstado] = 1 AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("1"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 2 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(2,4)) AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("2"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 2 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(1,3)) AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                    }
                    else
                    {
                        if (tipoIniciativaSeleccionado.Equals("0"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 2 AND FLU.[WrfEstado] = 1";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("1"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 2 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(2,4))";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("2"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 2 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(1,3))";
                        }
                    }
                    break;
                case "REVISION AMSA":
                    if (!rol.Contains("Administrador1") && !rol.Contains("Administrador2") && !rol.Contains("Administrador3"))
                    {
                        if (tipoIniciativaSeleccionado.Equals("0"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 4 AND FLU.[WrfEstado] = 1 AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("1"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 4 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(2,4)) AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("2"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 4 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(1,3)) AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                    }
                    else
                    {
                        if (tipoIniciativaSeleccionado.Equals("0"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 4 AND FLU.[WrfEstado] = 1";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("1"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 4 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(2,4))";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("2"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 4 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(1,3))";
                        }
                    }
                    break;
                case "APROBADA AMSA":
                    if (!rol.Contains("Administrador1") && !rol.Contains("Administrador2") && !rol.Contains("Administrador3"))
                    {
                        if (tipoIniciativaSeleccionado.Equals("0"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 12 AND FLU.[WrfEstado] = 1 AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("1"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 12 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(2,4)) AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("2"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 12 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(1,3)) AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                    }
                    else
                    {
                        if (tipoIniciativaSeleccionado.Equals("0"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 12 AND FLU.[WrfEstado] = 1";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("1"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 12 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(2,4))";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("2"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 12 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(1,3))";
                        }
                    }
                    break;
                case "NO APROBADA GAF":
                    if (!rol.Contains("Administrador1") && !rol.Contains("Administrador2") && !rol.Contains("Administrador3"))
                    {
                        if (tipoIniciativaSeleccionado.Equals("0"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 8 AND FLU.[WrfEstado] = 1 AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("1"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 8 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(2,4)) AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("2"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 8 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(1,3)) AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                    }
                    else
                    {
                        if (tipoIniciativaSeleccionado.Equals("0"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 8 AND FLU.[WrfEstado] = 1";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("1"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 8 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(2,4))";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("2"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 8 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(1,3))";
                        }
                    }
                    break;
                case "NO APROBADA CE":
                    if (!rol.Contains("Administrador1") && !rol.Contains("Administrador2") && !rol.Contains("Administrador3"))
                    {
                        if (tipoIniciativaSeleccionado.Equals("0"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 9 AND FLU.[WrfEstado] = 1 AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("1"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 9 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(2,4)) AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("2"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 9 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(1,3)) AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                    }
                    else
                    {
                        if (tipoIniciativaSeleccionado.Equals("0"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 9 AND FLU.[WrfEstado] = 1";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("1"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 9 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(2,4))";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("2"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 9 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(1,3))";
                        }
                    }
                    break;
                case "NO APROBADA AMSA":
                    if (!rol.Contains("Administrador1") && !rol.Contains("Administrador2") && !rol.Contains("Administrador3"))
                    {
                        if (tipoIniciativaSeleccionado.Equals("0"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 10 AND FLU.[WrfEstado] = 1 AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("1"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 10 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(2,4)) AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("2"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 10 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(1,3)) AND INI.[IniUsuarioOwner]='" + usuario + "'";
                        }
                    }
                    else
                    {
                        if (tipoIniciativaSeleccionado.Equals("0"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 10 AND FLU.[WrfEstado] = 1";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("1"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 10 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(2,4))";
                        }
                        else if (tipoIniciativaSeleccionado.Equals("2"))
                        {
                            sql = "SELECT COUNT(FLU.[IdWrf]) AS NumIniciativas FROM [dbo].[CAPEX_PLANIFICACION_IDENTIFICACION] IDEN RIGHT JOIN [dbo].[CAPEX_INICIATIVA] INI ON INI.IniToken = IDEN.PidToken LEFT JOIN [dbo].[CAPEX_FLUJO] FLU ON FLU.IniToken = IDEN.PidToken WHERE IDEN.[PidEstadoFlujo] = 1 AND IDEN.[PidEstado] = 1 AND FLU.[WrfEstadoActual] = 10 AND FLU.[WrfEstado] = 1 AND INI.[IniTipo] in (SELECT [TipAcronimo] FROM [dbo].[CAPEX_TIPO_INICIATIVA] WHERE [IdTipini] in(1,3))";
                        }
                    }
                    break;
            }
            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var resultado = SqlMapper.Query(objConnection, sql).FirstOrDefault();
                    return Json(new { Resultado = resultado }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    Utils.LogError("ObtenerContador, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString());
                    return Json(new { Resultado = "0" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }
        /// <summary>
        /// METODO VER INICIATIVA
        /// </summary>
        /// <returns></returns>
        [Route("VerIniciativa/{token}")]
        public ActionResult VerIniciativa(string token)
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    try
                    {
                        using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
                        {
                            try
                            {
                                objConnection.Open();
                                ViewBag.Identificacion = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_IDENTIFICACION_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                                ViewBag.Categorizacion = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_CATEGORIZACION_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                                ViewBag.DescripcionDetallada = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_DESCRIPCIONDETALLADA_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                                ViewBag.EvaluacionEconomica = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_EVALUACIONECONOMICA_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                                ViewBag.EvaluacionRiesgo = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_EVALUACIONRIESGO_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                                ViewBag.Hito = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_HITO_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                                ViewBag.HitoDetalle = PoblarVistaHitos(token);
                            }
                            catch (Exception err)
                            {
                                Utils.LogError("ObtenerContador, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString());
                                return Json(new { Resultado = "0" }, JsonRequestBehavior.AllowGet);
                            }
                            finally
                            {
                                objConnection.Close();
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        return Json(new { Mensaje = ex.Message.ToString() + "-----" + ex.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return View("~/Views/Gestion/VerIniciativa.cshtml");
        }
        /// <summary>
        /// METODO MODIFICAR INICIATIVA
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [Route("ModificarIniciativa/{token}")]
        public ActionResult ModificarIniciativa(string token)
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
                    {
                        try
                        {
                            objConnection.Open();
                            var Situacion = SqlMapper.Query(objConnection, "CAPEX_SEL_SITUACION_INICIATIVA", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                            foreach (var s in Situacion)
                            {
                                ViewBag.IniToken = s.IniToken;
                                HttpContext.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"] = s.IniToken;
                                HttpContext.Session["_SESS_CAPEX_INCIATIVA_ID_"] = s.IdPid;
                                ViewBag.IniUsuario = s.IniUsuario;
                                ViewBag.IniTipo = s.IniTipo;
                                ViewBag.IniEjercicio = s.IniEjercicio;
                                ViewBag.IniFlujoGestion = s.IniFlujoGestion;
                                ViewBag.IniDotacion = s.IniDotacion;
                                ViewBag.IniPaso1 = s.IniPaso1;
                                ViewBag.IniPaso2 = s.IniPaso2;
                                ViewBag.IniPaso3 = s.IniPaso3;
                                ViewBag.IniPaso4 = s.IniPaso4;
                                ViewBag.IniPaso5 = s.IniPaso5;
                                ViewBag.IniPaso6 = s.IniPaso6;
                                ViewBag.IniPaso7 = s.IniPaso7;
                                ViewBag.IniEstado = s.IniEstado;
                                ViewBag.EstadoActualFlujo = s.EstadoActualFlujo;
                                ViewBag.IniFecha = s.IniFecha;
                                ViewBag.IniBloqueo = s.IniBloqueo;
                            }

                            var Base = SqlMapper.Query(objConnection, "CAPEX_SEL_PLANIFICACION_INICIATIVA", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                            string fechaIniciativa = string.Empty;
                            foreach (var b in Base)
                            {
                                ViewBag.BaseId = b.IdIni;
                                ViewBag.BaseIniToken = b.IniToken;
                                ViewBag.BaseIniUsuario = b.IniUsuario;
                                ViewBag.BaseIniTipo = b.IniTipo;
                                ViewBag.BaseIniTipoEjercicio = b.IniTipoEjercicio;
                                ViewBag.BaseIniPeriodo = b.IniPeriodo;
                                ViewBag.BaseIniFecha = b.IniFecha;
                                fechaIniciativa = b.IniFecha;
                            }
                            var Identificacion = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_IDENTIFICACION_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                            foreach (var i in Identificacion)
                            {
                                ViewBag.IdenProceso = i.PidProceso;
                                ViewBag.IdenProcesoReal = i.PidProcesoReal;
                                ViewBag.IdenObjeto = i.PidObjeto;
                                ViewBag.IdenArea = i.PidArea;
                                ViewBag.IdenAreaReal = i.PidAreaReal;
                                ViewBag.PidPeriodo = i.PidPeriodo;
                                ViewBag.IdenCompania = i.PidCompania;
                                ViewBag.IdenCompaniaReal = i.PidCompaniaReal;
                                ViewBag.IdenEtapa = i.PidEtapa;
                                ViewBag.IdenEtapaAcronimo = i.PidEtapaAcronimo;
                                ViewBag.IdenIniTipo = i.IniTipo;
                                ViewBag.IdenNombre = i.PidNombreProyecto;
                                ViewBag.IdenNombreAlias = i.PidNombreProyectoAlias;
                                ViewBag.IdenCodigo = i.PidCodigoIniciativa;
                                ViewBag.IdenProyecto = codigoProyectoFormateado(i.PidCodigoProyecto);
                                ViewBag.IdenGerInver = i.PidGerenciaInversion;//NOMBRE GERENCIA INVERSION
                                ViewBag.PidGerenciaInversionReal = i.PidGerenciaInversionReal;//PidGerenciaInversion es id de la tabla gerencia
                                ViewBag.PidGerenteInversionToken = i.PidGerenteInversionToken;//[GteToken] de la tabla gerente correspondiente al gerente inversion
                                ViewBag.IdenGereInve = i.PidGerenteInversion;//[GteNombre] de la tabla gerente correspondiente al gerente inversion

                                ViewBag.IdenRequiere = i.PidRequiere;

                                ViewBag.IdenIdGeren = i.IdGerencia;//PidGerenciaEjecucion de la tabla CAPEX_PLANIFICACION_IDENTIFICACION es el token de la tabla gerencia
                                ViewBag.IdenIdGerenInv = i.IdGerenciaInversion;
                                ViewBag.IdenGerEjec = i.PidGerenciaEjecucion;
                                ViewBag.PidGerenteEjecucionToken = i.PidGerenteEjecucionToken;//[GteToken] de la tabla gerente correspondiente al gerente inversion
                                ViewBag.PidGerenciaEjecucionReal = i.PidGerenciaEjecucionReal;
                                ViewBag.IdGerenciaEjecucionReal = i.idGerenciaEjecucionReal;
                                ViewBag.IdenGereEje = i.PidGerenteEjecucion;
                                ViewBag.IdenGereEjeReal = i.PidGerenteEjecucionReal;
                                ViewBag.PidGerenciaEjecucionToken = i.PidGerenciaEjecucionToken;

                                ViewBag.IdenIdSuper = i.IdSuperintendencia;
                                ViewBag.IdenSuperInt = i.PidSuperintendencia;
                                ViewBag.IdenSupInten = i.PidSuperintendente;
                                ViewBag.PidSuperintendenteToken = i.PidSuperintendenteToken;
                                ViewBag.IdenEncargad = i.PidEncargadoControl;
                                ViewBag.IdenFecha = i.PidFecha;
                                ViewBag.IdenToken = token;
                            }
                            var Categorizacion = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_CATEGORIZACION_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                            foreach (var c in Categorizacion)
                            {
                                ViewBag.CatEstadoProyecto = c.CatEstadoProyecto;
                                ViewBag.CatEstadoProyectoReal = c.CatEstadoProyectoReal;
                                ViewBag.CatToken = c.CatToken;
                                ViewBag.CatTokenCat = c.CatTokenCat;
                                ViewBag.CatCategoriaNombre = c.CatCategoriaNombre;
                                ViewBag.CatCategoriaToken = c.CatCategoriaToken;
                                ViewBag.CatNivelIngenieria = c.CatNivelIngenieria;
                                ViewBag.CatNivelIngenieriaToken = c.CatNivelIngenieriaToken;
                                ViewBag.CatNivelIngenieriaAcronimo = c.CatNivelIngenieriaAcronimo;
                                ViewBag.CatAgrega = c.CatAgrega;
                                ViewBag.CatTipoCotizacion = c.CatTipoCotizacion;
                                ViewBag.CatClasificacionSSO = c.CatClasificacionSSO;
                                ViewBag.CatClasificacionSSOToken = c.CatClasificacionSSOToken;
                                ViewBag.CatEstandarSeguridad = c.CatEstandarSeguridad;
                                ViewBag.CatEstandarSeguridadToken = c.CatEstandarSeguridadToken;
                                ViewBag.CatClase = c.CatClase;
                                ViewBag.CatMacroCategoria = c.CatMacroCategoria;
                                ViewBag.CatAnalisis = c.CatAnalisis;

                                ViewBag.CatACNota1 = c.CatACNota1;
                                ViewBag.CatACNota2 = c.CatACNota2;
                                ViewBag.CatACNota3 = c.CatACNota3;
                                ViewBag.CatACNota4 = c.CatACNota4;
                                ViewBag.CatACNota5 = c.CatACNota5;
                                ViewBag.CatACNota6 = c.CatACNota6;
                                ViewBag.CatACTotal = c.CatACTotal;

                                ViewBag.CatACObs1 = c.CatACObs1;
                                ViewBag.CatACObs2 = c.CatACObs2;
                                ViewBag.CatACObs3 = c.CatACObs3;
                                ViewBag.CatACObs4 = c.CatACObs4;
                                ViewBag.CatACObs5 = c.CatACObs5;
                                ViewBag.CatACObs6 = c.CatACObs6;

                            }

                            var DescripcionDetallada = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_DESCRIPCIONDETALLADA_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                            foreach (var d in DescripcionDetallada)
                            {
                                ViewBag.IdPdd = d.IdPdd;
                                ViewBag.PddToken = d.PddToken;
                                ViewBag.IniUsuario = d.IniUsuario;
                                ViewBag.IniToken = d.IniToken;
                                ViewBag.PddObjetivo = d.PddObjetivo;
                                ViewBag.PddAlcance = d.PddAlcance;
                                ViewBag.PddJustificacion = d.PddJustificacion;
                                ViewBag.PddDescripcion1 = d.PddDescripcion1;
                                ViewBag.PddUnidad1 = d.PddUnidad1;
                                ViewBag.PddActual1 = d.PddActual1;
                                ViewBag.PddTarget1 = d.PddTarget1;
                                ViewBag.PddDescripcion2 = d.PddDescripcion2;
                                ViewBag.PddUnidad2 = d.PddUnidad2;
                                ViewBag.PddActual2 = d.PddActual2;
                                ViewBag.PddTarget2 = d.PddTarget2;
                                ViewBag.PddDescripcion3 = d.PddDescripcion3;
                                ViewBag.PddUnidad3 = d.PddUnidad3;
                                ViewBag.PddActual3 = d.PddActual3;
                                ViewBag.PddTarget3 = d.PddTarget3;
                                DateTime currentDateTime = DateTime.Now;
                                if (!string.IsNullOrEmpty(fechaIniciativa) && fechaIniciativa.Trim().Length == 10)
                                {
                                    CultureInfo ciCL = new CultureInfo("es-CL", false);
                                    fechaIniciativa = fechaIniciativa.Replace("/", "-");
                                    currentDateTime = DateTime.ParseExact(fechaIniciativa, "dd-MM-yyyy", ciCL);
                                }
                                currentDateTime = currentDateTime.AddMonths(6);
                                string PddFechaPostEval = ((string.IsNullOrEmpty(d.PddFechaPostEval)) ? currentDateTime.ToString("dd-MM-yyyy") : d.PddFechaPostEval.Replace("/", "-"));
                                ViewBag.PddFechaPostEval = PddFechaPostEval;
                                ViewBag.PddFecha = d.PddFecha;
                                ViewBag.PddEstadoFlujo = d.PddEstadoFlujo;
                                ViewBag.PddEstado = d.PddEstado;
                            }

                            var EvaluacionEconomica = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_EVALUACIONECONOMICA_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                            ViewBag.EveVan = "0";
                            ViewBag.EveIvan = "0";
                            ViewBag.EvePayBack = "0";
                            ViewBag.EveVidaUtil = "0";
                            ViewBag.EveTipoCambio = "0";
                            ViewBag.EveTir = "0";
                            if (EvaluacionEconomica != null && EvaluacionEconomica.Count > 0)
                            {
                                foreach (var ee in EvaluacionEconomica)
                                {
                                    ViewBag.IdEve = ee.IdEve;
                                    ViewBag.EveToken = ee.EveToken;
                                    ViewBag.IniUsuario = ee.IniUsuario;
                                    ViewBag.IniToken = ee.IniToken;
                                    ViewBag.EveVan = varEvaluacionEconomica(ee.EveVan);
                                    ViewBag.EveIvan = varEvaluacionEconomica(ee.EveIvan);
                                    ViewBag.EvePayBack = varEvaluacionEconomica(ee.EvePayBack);
                                    ViewBag.EveVidaUtil = varEvaluacionEconomica(ee.EveVidaUtil);
                                    ViewBag.EveTipoCambio = varEvaluacionEconomica(ee.EveTipoCambio);
                                    ViewBag.EveTir = varEvaluacionEconomica(ee.EveTir);
                                    ViewBag.EveFecha = ee.EveFecha;
                                    ViewBag.EveEstadoFlujo = ee.EveEstadoFlujo;
                                    ViewBag.EveEstado = ee.EveEstado;
                                }
                            }

                            var EvaluacionRiesgo = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_EVALUACIONRIESGO_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                            ViewBag.EriProb1 = "1";
                            ViewBag.EriImp1 = "1";
                            ViewBag.EriRies1 = "1";
                            ViewBag.EriClas1 = "Bajo";
                            ViewBag.EriMFL1 = "0";
                            ViewBag.EriProb2 = "1";
                            ViewBag.EriImp2 = "1";
                            ViewBag.EriRies2 = "1";
                            ViewBag.EriClas2 = "Bajo";
                            ViewBag.EriMFL2 = "0";
                            ViewBag.EriItemMR = 0;
                            if (EvaluacionRiesgo != null && EvaluacionRiesgo.Count > 0)
                            {
                                foreach (var er in EvaluacionRiesgo)
                                {
                                    ViewBag.IniToken = er.IniToken;
                                    ViewBag.IniUsuario = er.IniUsuario;
                                    ViewBag.EriProb1 = er.EriProb1;
                                    ViewBag.EriImp1 = er.EriImp1;
                                    ViewBag.EriRies1 = er.EriRies1;
                                    ViewBag.EriClas1 = er.EriClas1;
                                    ViewBag.EriMFL1 = varEvaluacionRiesgo(er.EriMFL1);
                                    ViewBag.EriProb2 = er.EriProb2;
                                    ViewBag.EriImp2 = er.EriImp2;
                                    ViewBag.EriRies2 = er.EriRies2;
                                    ViewBag.EriClas2 = er.EriClas2;
                                    ViewBag.EriMFL2 = varEvaluacionRiesgo(er.EriMFL2);
                                    ViewBag.EriItemMR = er.EriItemMR;
                                }

                            }

                            var Hito = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_HITO_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                            foreach (var hi in Hito)
                            {
                                ViewBag.HitNacExt = hi.HitNacExt;
                                ViewBag.HitSAP = hi.HitSAP;
                                ViewBag.HitCI = hi.HitCI;
                                ViewBag.HitCA = hi.HitCA;
                                ViewBag.HitOPR = hi.HitOPR;
                                ViewBag.HitPE = hi.HitPE;
                                ViewBag.HitDIRCEN = hi.HitDIRCEN;
                                ViewBag.HitDirPLC = hi.HitDirPLC;
                                /* ViewBag.HitDirPLC = hi.HitDirPLC;
                                   ViewBag.HitDirPLC = hi.HitDirPLC;
                                   ViewBag.HitDirPLC = hi.HitDirPLC;*/
                            }
                            ViewBag.HitoDetalle = PoblarVistaHitos(token);
                        }
                        catch (Exception err)
                        {
                            Utils.LogError("ModificarIniciativa, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString());
                            return null;
                        }
                        finally
                        {
                            objConnection.Close();
                        }
                    }
                }
            }
            return View("~/Views/Gestion/ModificarIniciativa.cshtml");
        }

        private string codigoProyectoFormateado(string codigoProyecto)
        {
            var codigoProyectoFormateado = new StringBuilder();
            if (codigoProyecto != null && !string.IsNullOrEmpty(codigoProyecto.Trim()))
            {
                for (int i = 0; i < codigoProyecto.Length; i++)
                {
                    if (codigoProyectoFormateado.Length == 12)
                    {
                        break;
                    }
                    if (codigoProyectoFormateado.Length == 4)
                    {
                        codigoProyectoFormateado.Append('-');
                        if (codigoProyecto[i] != '-')
                        {
                            codigoProyectoFormateado.Append(codigoProyecto[i]);
                        }
                    }
                    else
                    {
                        codigoProyectoFormateado.Append(codigoProyecto[i]);
                    }
                }
            }
            return codigoProyectoFormateado.ToString();
        }

        private string varEvaluacionEconomica(string param)
        {
            string value = "0";
            if (param != null && !string.IsNullOrEmpty(param.Trim()))
            {
                if (param.IndexOf(".") > 0 && param.IndexOf(",") > 0)
                {
                    value = ((param.Replace(".", ";")).Replace(",", "")).Replace(";", "");
                }
                else if (param.IndexOf(".") > 0 && param.IndexOf(",") < 0)
                {
                    value = param.Replace(".", ",");
                }
                else if (param.IndexOf(".") < 0 && param.IndexOf(",") > 0)
                {
                    value = param.Replace(",", "");
                }
                else
                {
                    value = param;
                }
            }
            return value;
        }

        private string varEvaluacionRiesgo(string param)
        {
            string value = "1";
            if (param != null && !string.IsNullOrEmpty(param.Trim()))
            {
                if (param.IndexOf(".") > 0 && param.IndexOf(",") > 0)
                {
                    value = ((param.Replace(".", ";")).Replace(",", "")).Replace(";", "");
                }
                else if (param.IndexOf(".") > 0 && param.IndexOf(",") < 0)
                {
                    value = param.Replace(".", ",");
                }
                else if (param.IndexOf(".") < 0 && param.IndexOf(",") > 0)
                {
                    value = param.Replace(",", "");
                }
                else
                {
                    value = param;
                }
            }
            return value;
        }

        /// <summary>
        /// METODO POBLAR CUADRO RESULTADO HITOS
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private string PoblarVistaHitos(string token)
        {
            string Desplegable = String.Empty;
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var resultado = SqlMapper.Query(objConnection, "CAPEX_SEL_RESUMEN_FINANCIERO", new { token }, commandType: CommandType.StoredProcedure).ToList();
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
                                CultureInfo ciCL = new CultureInfo("es-CL", false);
                                if (contador < 6)
                                {
                                    /*table.Append("<td style='height:20px;font-weight:normal;color:#f0f0f0; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.AnterioresConvert).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                                    table.Append("<td style='height:20px;font-weight:normal;color:#f0f0f0; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.ActualConvert).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                                    table.Append("<td style='height:20px;font-weight:normal;color:#f0f0f0; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.Posteriores).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                                    table.Append("<td style='height:20px;font-weight:bold;color:#fff; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.TotalCapexConvert).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "<input type='hidden' id='HitosTotalCapex" + contador + "' value='" + String.Format("{0:#,##0.##}", result.TotalCapexConvert).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "'></td>");*/
                                    table.Append("<td style='height:20px;font-weight:normal;color:#f0f0f0; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.AnterioresConvert).ToString() + "</td>");
                                    table.Append("<td style='height:20px;font-weight:normal;color:#f0f0f0; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.ActualConvert).ToString() + "</td>");
                                    table.Append("<td style='height:20px;font-weight:normal;color:#f0f0f0; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.Posteriores).ToString() + "</td>");
                                    table.Append("<td style='height:20px;font-weight:bold;color:#fff; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.TotalCapexConvert).ToString() + "<input type='hidden' id='HitosTotalCapex" + contador + "' value='" + String.Format("{0:#,##0.##}", result.TotalCapexConvert).ToString() + "'></td>");
                                }
                                else
                                {
                                    /*table.Append("<td style='height:20px;font-weight:bold;color:#fff; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.AnterioresConvert).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                                    table.Append("<td style='height:20px;font-weight:bold;color:#fff; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.ActualConvert).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                                    table.Append("<td style='height:20px;font-weight:bold;color:#fff; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.Posteriores).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "</td>");
                                    table.Append("<td style='height:20px;font-weight:bold;color:#fff; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.TotalCapexConvert).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "<input type='hidden' id='HitosTotalCapex" + contador + "' value='" + String.Format("{0:#,##0.##}", result.TotalCapexConvert).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.') + "'></td>");*/
                                    table.Append("<td style='height:20px;font-weight:bold;color:#fff; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.AnterioresConvert).ToString() + "</td>");
                                    table.Append("<td style='height:20px;font-weight:bold;color:#fff; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.ActualConvert).ToString() + "</td>");
                                    table.Append("<td style='height:20px;font-weight:bold;color:#fff; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.Posteriores).ToString() + "</td>");
                                    table.Append("<td style='height:20px;font-weight:bold;color:#fff; text-align:center;background-color:" + fondocelda + ";'> " + String.Format("{0:#,##0.##}", result.TotalCapexConvert).ToString() + "<input type='hidden' id='HitosTotalCapex" + contador + "' value='" + String.Format("{0:#,##0.##}", result.TotalCapexConvert).ToString() + "'></td>");
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
                    ExceptionResult = "PoblarVistaHitos, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    Desplegable = "ERROR";
                }
                finally
                {
                    objConnection.Close();
                }
            }
            return Desplegable.ToString();
        }
        /// <summary>
        /// VER ADJUNTOS
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public string VerAdjuntos(string token)
        {
            string Desplegable = string.Empty;
            string Estado = string.Empty;
            try
            {
                using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
                {
                    objConnection.Open();
                    try
                    {
                        var categorias = SqlMapper.Query(objConnection, "CAPEX_SEL_ADJUNTOS", new { token }, commandType: CommandType.StoredProcedure).ToList();
                        var items = SqlMapper.Query(objConnection, "CAPEX_SEL_ADJUNTOS_ITEMS", new { token }, commandType: CommandType.StoredProcedure).ToList();
                        var arbol = new StringBuilder();
                        var contador = 1;

                        if (categorias.Count > 0)
                        {
                            foreach (var categoria in categorias)
                            {
                                arbol.Append("<li>");
                                arbol.Append("<i class='fa fa-folder fa-1x' style='color:#3476ad;'></i> <span style='color:#3476ad;'>" + categoria.ParPaso + "</span>");
                                string ruta = String.Empty;
                                string paso = String.Empty;
                                switch (categoria.ParPaso)
                                {
                                    case "Presupuesto-Gantt":
                                        ruta = "Presupuesto";
                                        paso = "Gantt";
                                        break;
                                    case "Evaluacion-Economica":
                                        ruta = "EvaluacionEconomica";
                                        paso = "";
                                        break;
                                    case "Evaluacion-Riesgo":
                                        ruta = "EvaluacionRiesgo";
                                        paso = "";
                                        break;
                                    case "Categorizacion":
                                        ruta = "Categorizacion";
                                        paso = "";
                                        break;
                                    case "Descripcion-Detallada":
                                        ruta = "Descripcion";
                                        paso = "";
                                        break;
                                }
                                foreach (var item in items)
                                {
                                    if (categoria.ParPaso == item.ParPaso)
                                    {
                                        //string descarga = "<a href=\"" + Url.Action("Descargar", "Gestion", new { ruta = ruta, paso = paso, token = token, archivo = item.ParNombre }) + "\" style='color:#0094ff;' >Descargar</a>";
                                        string descarga = "<span onclick='FNDescargarAdjuntoFinal(" + Convert.ToChar(34) + item.ParToken + Convert.ToChar(34) + ")' style='color:#0094ff;cursor: pointer'> Descargar </span>";
                                        arbol.Append("<ul><li><span><i class='fa fa-file fa-1x' style='color:#0094ff; margin-left:10px;'></i> " + item.ParNombre + "</span> | <span style='color:#0094ff;' style='cursor:hand;'>" + descarga + "</span></li></ul>");
                                    }
                                }
                                arbol.Append("</li>");
                                contador++;
                            }

                            Desplegable = arbol.ToString();
                            arbol = null;

                        }
                    }
                    catch (Exception err)
                    {
                        Utils.LogError("VerAdjuntos, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString());
                        Desplegable = "";
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
            catch (Exception exc)
            {
                ExceptionResult = "VerAdjuntos, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                Desplegable = "";
            }
            return Desplegable.ToString();
        }
        /// <summary>
        /// DESCARGA DE ADJUNTOS
        /// </summary>
        /// <returns></returns>
        public ActionResult Descargar(string ruta, string paso, string token, string archivo)
        {
            try
            {
                string nueva_ruta = string.Empty;
                if (string.IsNullOrEmpty(paso))
                {
                    nueva_ruta = "Files/Iniciativas/" + ruta + "/" + token + "/" + archivo;
                    //nueva_ruta = "Scripts/Files/Iniciativas/" + ruta + "/" + token + "/" + archivo;
                }
                else
                {
                    nueva_ruta = "Files/Iniciativas/" + ruta + "/" + paso + "/" + token + "/" + archivo;
                    //nueva_ruta = "Scripts/Files/Iniciativas/" + ruta + "/" + paso + "/" + token + "/" + archivo;
                }
                string path = AppDomain.CurrentDomain.BaseDirectory + nueva_ruta;
                // string path = Server.MapPath(nueva_ruta);
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                string fileName = archivo;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
                /*string nueva_ruta = string.Empty;
                if (string.IsNullOrEmpty(paso))
                {
                    nueva_ruta = Server.MapPath("~/Files/Iniciativas/" + ruta + "/" + token + "/" + archivo);
                }
                else
                {
                    nueva_ruta = Server.MapPath("~/Files/Iniciativas/" + ruta + "/" + paso + "/" + token + "/" + archivo);
                }
                byte[] fileBytes = System.IO.File.ReadAllBytes(nueva_ruta);
                string fileName = archivo;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);*/
            }
            catch (Exception exc)
            {
                ExceptionResult = "Descargar, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return View("~/Views/Ejercicios/NoEncontrado.cshtml");
            }
        }
        #endregion
    }
}
