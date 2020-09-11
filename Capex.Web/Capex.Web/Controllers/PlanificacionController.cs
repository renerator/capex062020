using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapexIdentity.Utilities;
using CapexInfraestructure.Bll.Entities.Planificacion;
using CapexInfraestructure.Bll.Business.Planificacion;
using CapexInfraestructure.Bll.Factory;
using CapexInfraestructure.Utilities;
using Newtonsoft.Json;
using System.Web.Caching;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Text;
using static System.Net.WebRequestMethods;
using ClosedXML.Excel;
using Rotativa.Options;
using Rotativa;
using System.Net.Mime;
using Utils = CapexInfraestructure.Utilities.Utils;
using System.Data.SqlTypes;
using System.Collections;
using SHARED.AzureStorage;
using System.Configuration;
using System.Globalization;
using System.Threading;
using Capex.Web.Content.exception;
using System.Net;

namespace Capex.Web.Controllers
{
    [AuthorizeAdminOrMember]
    [RoutePrefix("Planificacion")]
    public class PlanificacionController : Controller
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
        ///     CONTROLADOR "PlanificacionController" 
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
        public string ExceptionResult { get; set; }
        public string AppModule { get; set; }
        #endregion

        #region "CONSTANTES"
        //IDENTIFICACION
        private const PlanificacionFactory.tipo LP = PlanificacionFactory.tipo.ListarProcesos;
        private const PlanificacionFactory.tipo LA = PlanificacionFactory.tipo.ListarAreas;
        private const PlanificacionFactory.tipo LC = PlanificacionFactory.tipo.ListarCompanias;
        private const PlanificacionFactory.tipo LE = PlanificacionFactory.tipo.ListarEtapas;
        private const PlanificacionFactory.tipo LG = PlanificacionFactory.tipo.ListarGerencias;
        private const PlanificacionFactory.tipo LI = PlanificacionFactory.tipo.ListarSuperintendencias;
        private const PlanificacionFactory.tipo OG = PlanificacionFactory.tipo.ObtenerGerente;
        private const PlanificacionFactory.tipo OE = PlanificacionFactory.tipo.ObtenerEncargado;
        private const PlanificacionFactory.tipo OI = PlanificacionFactory.tipo.ObtenerIntendente;
        private const PlanificacionFactory.tipo GI = PlanificacionFactory.tipo.GuardarIdentificacion;

        private const PlanificacionFactory.tipo LCAT = PlanificacionFactory.tipo.ListarCategorias;
        private const PlanificacionFactory.tipo LNII = PlanificacionFactory.tipo.ListarNivelIngenieria;
        private const PlanificacionFactory.tipo LSSO = PlanificacionFactory.tipo.ListarClasificacionSSO;
        private const PlanificacionFactory.tipo LESS = PlanificacionFactory.tipo.ListarEstandarSeguridad;
        private const PlanificacionFactory.tipo OTC = PlanificacionFactory.tipo.ObtenerTokenCompania;
        private const PlanificacionFactory.tipo AEA = PlanificacionFactory.tipo.ActualizarEtapa;
        private const PlanificacionFactory.tipo GCAT = PlanificacionFactory.tipo.GuardarCategorizacion;

        private const PlanificacionFactory.tipo IT = PlanificacionFactory.tipo.Importar;
        private const PlanificacionFactory.tipo PO = PlanificacionFactory.tipo.Poblar;
        private const PlanificacionFactory.tipo ECD = PlanificacionFactory.tipo.EliminarContratoDotacion;

        private const PlanificacionFactory.tipo LDEP = PlanificacionFactory.tipo.ListarDepartamentos;
        private const PlanificacionFactory.tipo LTUR = PlanificacionFactory.tipo.ListarTurnos;
        private const PlanificacionFactory.tipo LUBI = PlanificacionFactory.tipo.ListarUbicaciones;
        private const PlanificacionFactory.tipo LEEC = PlanificacionFactory.tipo.ListarTipoEECC;
        private const PlanificacionFactory.tipo LCLA = PlanificacionFactory.tipo.ListarClasificacion;

        private const PlanificacionFactory.tipo GD = PlanificacionFactory.tipo.GuardarDescripcionDetallada;

        private const PlanificacionFactory.tipo GE = PlanificacionFactory.tipo.GuardarEvaluacionEconomica;
        private const PlanificacionFactory.tipo GR = PlanificacionFactory.tipo.GuardarEvaluacionRiesgo;

        private const PlanificacionFactory.tipo PH = PlanificacionFactory.tipo.PoblarVistaHitos;
        private const PlanificacionFactory.tipo PVHG = PlanificacionFactory.tipo.Detallar;
        private const PlanificacionFactory.tipo PVHR = PlanificacionFactory.tipo.Resumir;
        private const PlanificacionFactory.tipo GH = PlanificacionFactory.tipo.GuardarHito;
        private const PlanificacionFactory.tipo EI = PlanificacionFactory.tipo.EnviarIniciativa;

        private const PlanificacionFactory.tipo VA = PlanificacionFactory.tipo.VerAdjunto;

        private const PlanificacionFactory.tipo RA = PlanificacionFactory.tipo.RegistrarArchivo;
        #endregion

        #region "CAMPOS"
        //IDENTIFICACION
        public static PlanificacionFactory FactoryPlanificacion;
        public static IPlanificacion IPlanificacion;
        //public SqlConnection ORM;
        #endregion

        #region "CONSTRUCTOR"
        public PlanificacionController()
        {
            //IDENTIFICACION
            FactoryPlanificacion = new PlanificacionFactory();
            JsonResponse = string.Empty;
            //ORM = CapexInfraestructure.Utilities.Utils.Conectar();
        }
        #endregion

        #region "METODOS IDENTIFICACION"
        [Route("Ingreso", Name = "Planificacion")]
        public ActionResult Index()
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                HttpContext.Session["CAPEX_SESS_USERNAME"] = @User.Identity.Name;
            }
            Session["tipoIniciativaOrientacionComercial"] = "";
            Session["anioIniciativaOrientacionComercial"] = "";
            Session["tipoIniciativaEjercicioOficial"] = "";
            Session["anioIniciativaEjercicioOficial"] = "";
            Session["tipoIniciativaSeleccionado"] = "";
            Session["anioIniciativaSeleccionado"] = "";
            Session["ParametroVNToken"] = "";
            return View("Index");
        }
        /// <remark>
        ///     
        ///     ----------------------------------------------------------------------------
        ///     GRUPO DE METODOS "Identificación"
        ///     VERSION     0.0.1
        ///     ----------------------------------------------------------------------------
        ///     PROPOSITO
        ///     ----------------------------------------------------------------------------
        ///     IMPLEMENTAR ENTRADA Y SALIDA DE DATOS, ASI COMO OPERACIONES POR MEDIO
        ///     DE PATRON CREACIONAL FACTORY QUE ES PROVISTO POR CAPA DDD DE INFRAESTRUCTURA
        ///     
        /// </remark>
        /// 


        [HttpGet]
        public ActionResult ListarProcesos()
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(LP);
                    JsonResponse = JsonConvert.SerializeObject(IPlanificacion.ListarProcesos(), Formatting.None);
                    return Json(
                        JsonResponse,
                        JsonRequestBehavior.AllowGet
                        );
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        [HttpGet]
        public ActionResult ListarAreas()
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(LA);
                    JsonResponse = JsonConvert.SerializeObject(IPlanificacion.ListarAreas(), Formatting.None);
                    return Json(
                        JsonResponse,
                        JsonRequestBehavior.AllowGet
                    );
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// OBTENER LISTADO DE COMPAÑIAS
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListarCompanias()
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(LC);
                    JsonResponse = JsonConvert.SerializeObject(IPlanificacion.ListarCompanias(), Formatting.None);
                    return Json(
                        JsonResponse,
                        JsonRequestBehavior.AllowGet
                        );
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// OBTENER LISTADO DE ETAPAS
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListarEtapas()
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(LE);
                    JsonResponse = JsonConvert.SerializeObject(IPlanificacion.ListarEtapas(), Formatting.None);
                    return Json(
                        JsonResponse,
                        JsonRequestBehavior.AllowGet
                    );
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// OBTENER LISTADO DE GERENCIAS
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListarGerencias()
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(LG);
                    string JsonResponse = JsonConvert.SerializeObject(IPlanificacion.ListarGerencias(), Formatting.None);
                    return Json(
                        JsonResponse,
                        JsonRequestBehavior.AllowGet
                        );
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// OBTENER GERENTES
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ObtenerGerente(string Token)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(OG);
                    string JsonResponse = JsonConvert.SerializeObject(IPlanificacion.ObtenerGerente(Token), Formatting.None);
                    return Json(
                    JsonResponse,
                    JsonRequestBehavior.AllowGet
                    );

                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// OBTENER ENCARGADOS
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ObtenerEncargado(int IdGerencia, int CodigoSuper)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(OE);
                    string JsonResponse = JsonConvert.SerializeObject(IPlanificacion.ObtenerEncargado(IdGerencia, CodigoSuper), Formatting.None);
                    return Json(
                        JsonResponse,
                        JsonRequestBehavior.AllowGet
                    );
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        [HttpGet]
        public ActionResult ListarSuperintendenciasPorGerencia(string GerToken)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(LI);
                    string JsonResponse = JsonConvert.SerializeObject(IPlanificacion.ListarSuperintendenciasPorGerencia(GerToken), Formatting.None);
                    return Json(
                        JsonResponse,
                        JsonRequestBehavior.AllowGet
                    );
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        [HttpGet]
        public ActionResult ListarSuperintendencias()
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(LI);
                    string JsonResponse = JsonConvert.SerializeObject(IPlanificacion.ListarSuperintendencias(), Formatting.None);
                    return Json(
                        JsonResponse,
                        JsonRequestBehavior.AllowGet
                    );
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// OBTENER INTENDENTE
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ObtenerIntendente(string Token)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(OI);
                    string JsonResponse = JsonConvert.SerializeObject(IPlanificacion.ObtenerIntendente(Token), Formatting.None);
                    return Json(
                        JsonResponse,
                        JsonRequestBehavior.AllowGet
                    );
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        /// <summary>
        /// ENVIAR COMITE LA INICIATIVA
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AprobarVisacion(Identificacion.AprobacionRechazo Datos)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    Datos.PidRol = rol;
                    IPlanificacion = FactoryPlanificacion.delega(GI);
                    var resultado = IPlanificacion.AprobarVisacion(Datos);
                    string[] datos = resultado.Split('|');
                    if (datos[0] == "Aprobado")
                    {
                        string mensaje = string.Format("{0}|{1}|{2}|{3}", datos[0], datos[1], datos[2], datos[3]);
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    else if (datos[0] == "Error")
                    {
                        string mensaje = string.Format("{0}|{1}|{2}", datos[0], datos[1], datos[2]);
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    return null;
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        /// <summary>
        /// ENVIAR COMITE LA INICIATIVA
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RechazarVisacion(Identificacion.AprobacionRechazo Datos)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    Datos.PidRol = rol;
                    IPlanificacion = FactoryPlanificacion.delega(GI);
                    var resultado = IPlanificacion.RechazarVisacion(Datos);
                    string[] datos = resultado.Split('|');
                    if (datos[0] == "Rechazado")
                    {
                        string mensaje = string.Format("{0}|{1}|{2}|{3}", datos[0], datos[1], datos[2], datos[3]);
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    else if (datos[0] == "Error")
                    {
                        string mensaje = string.Format("{0}|{1}|{2}", datos[0], datos[1], datos[2]);
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    return null;
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        /// <summary>
        /// ENVIAR COMITE LA INICIATIVA
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AprobarCE(Identificacion.AprobacionRechazo Datos)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    Datos.PidRol = rol;
                    IPlanificacion = FactoryPlanificacion.delega(GI);
                    var resultado = IPlanificacion.AprobarCE(Datos);
                    string[] datos = resultado.Split('|');
                    if (datos[0] == "Aprobado")
                    {
                        string mensaje = string.Format("{0}|{1}|{2}|{3}", datos[0], datos[1], datos[2], datos[3]);
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    else if (datos[0] == "Error")
                    {
                        string mensaje = string.Format("{0}|{1}|{2}", datos[0], datos[1], datos[2]);
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    return null;
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        /// <summary>
        /// ENVIAR COMITE LA INICIATIVA
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RechazarCE(Identificacion.AprobacionRechazo Datos)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    Datos.PidRol = rol;
                    IPlanificacion = FactoryPlanificacion.delega(GI);
                    var resultado = IPlanificacion.RechazarCE(Datos);
                    string[] datos = resultado.Split('|');
                    if (datos[0] == "Rechazado")
                    {
                        string mensaje = string.Format("{0}|{1}|{2}|{3}", datos[0], datos[1], datos[2], datos[3]);
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    else if (datos[0] == "Error")
                    {
                        string mensaje = string.Format("{0}|{1}|{2}", datos[0], datos[1], datos[2]);
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    return null;
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        /// <summary>
        /// ENVIAR COMITE LA INICIATIVA
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AprobarAMSA(Identificacion.AprobacionRechazo Datos)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    Datos.PidRol = rol;
                    IPlanificacion = FactoryPlanificacion.delega(GI);
                    var resultado = IPlanificacion.AprobarAMSA(Datos);
                    string[] datos = resultado.Split('|');
                    if (datos[0] == "Aprobado")
                    {
                        string mensaje = string.Format("{0}|{1}|{2}|{3}", datos[0], datos[1], datos[2], datos[3]);
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    else if (datos[0] == "Error")
                    {
                        string mensaje = string.Format("{0}|{1}|{2}", datos[0], datos[1], datos[2]);
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    return null;
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        /// <summary>
        /// ENVIAR COMITE LA INICIATIVA
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RechazarAMSA(Identificacion.AprobacionRechazo Datos)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    Datos.PidRol = rol;
                    IPlanificacion = FactoryPlanificacion.delega(GI);
                    var resultado = IPlanificacion.RechazarAMSA(Datos);
                    string[] datos = resultado.Split('|');
                    if (datos[0] == "Rechazado")
                    {
                        string mensaje = string.Format("{0}|{1}|{2}|{3}", datos[0], datos[1], datos[2], datos[3]);
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    else if (datos[0] == "Error")
                    {
                        string mensaje = string.Format("{0}|{1}|{2}", datos[0], datos[1], datos[2]);
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    return null;
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        /// <summary>
        /// OBTENER ULTIMA OBSERVACION
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ObtenerUltimaObservacionVisacion(string PidToken)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    IPlanificacion = FactoryPlanificacion.delega(GI);
                    var resultado = IPlanificacion.ObtenerUltimaObservacion(PidToken, "14");
                    string[] datos = resultado.Split('|');
                    if (datos[0] == "Obtenido")
                    {
                        string mensaje = string.Format("{0}|{1}|{2}|{3}", datos[0], datos[1], datos[2], datos[3]);
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    else if (datos[0] == "Error")
                    {
                        string mensaje = string.Format("{0}|{1}|{2}", datos[0], datos[1], datos[2]);
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    return null;
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        /// <summary>
        /// OBTENER ULTIMA OBSERVACION
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ObtenerUltimaObservacionComiteCE(string PidToken)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    IPlanificacion = FactoryPlanificacion.delega(GI);
                    var resultado = IPlanificacion.ObtenerUltimaObservacion(PidToken, "20");
                    string[] datos = resultado.Split('|');
                    if (datos[0] == "Obtenido")
                    {
                        string mensaje = string.Format("{0}|{1}|{2}|{3}", datos[0], datos[1], datos[2], datos[3]);
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    else if (datos[0] == "Error")
                    {
                        string mensaje = string.Format("{0}|{1}|{2}", datos[0], datos[1], datos[2]);
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    return null;
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        /// <summary>
        /// OBTENER ULTIMA OBSERVACION
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ObtenerUltimaObservacionComiteAMSA(string PidToken)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    IPlanificacion = FactoryPlanificacion.delega(GI);
                    var resultado = IPlanificacion.ObtenerUltimaObservacion(PidToken, "22");
                    string[] datos = resultado.Split('|');
                    if (datos[0] == "Obtenido")
                    {
                        string mensaje = string.Format("{0}|{1}|{2}|{3}", datos[0], datos[1], datos[2], datos[3]);
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    else if (datos[0] == "Error")
                    {
                        string mensaje = string.Format("{0}|{1}|{2}", datos[0], datos[1], datos[2]);
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    return null;
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        /// <summary>
        /// OBTENER ULTIMA OBSERVACION
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AsignarmeIniciativaDesdeObservadas(Identificacion.AsignarmeIniciativa Datos)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    Datos.PidRol = rol;
                    Datos.PidUsuario = usuario;
                    Datos.IdEstadoFinal = "1";//ESTADO FINAL DESARROLLO
                    Datos.Forzar = "1";
                    IPlanificacion = FactoryPlanificacion.delega(GI);
                    var resultado = IPlanificacion.AsignarmeIniciativa(Datos);
                    string[] datos = resultado.Split('|');
                    if (datos[0] == "Asignado")
                    {
                        string mensaje = string.Format("{0}|{1}|{2}|{3}", datos[0], datos[1], datos[2], datos[3]);
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    else if (datos[0] == "Error")
                    {
                        string mensaje = string.Format("{0}|{1}|{2}", datos[0], datos[1], datos[2]);
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    return null;
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        /// <summary>
        /// GUARDAR IDENTIFICACION INICIATIVA
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GuardarIdentificacion(Identificacion.IdentificacionIniciativa Datos)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string mensaje = String.Empty;
                try
                {
                    Datos.PidRol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    IPlanificacion = FactoryPlanificacion.delega(GI);
                    var resultado = IPlanificacion.GuardarIdentificacion(Datos);
                    string[] datos = resultado.Split('|');

                    if (datos[0] == "Guardado")
                    {
                        mensaje = string.Format("{0}|{1}|{2}|{3}", datos[0], datos[1], datos[2], datos[3]);
                        HttpContext.Session["_SESS_CAPEX_INCIATIVA_ID_"] = datos[1];
                        HttpContext.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"] = datos[2];
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// METODO PARA ACTUALIZACION DE INICIATIVA
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ActualizarIdentificacion(Identificacion.IdentificacionIniciativa Datos)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string mensaje = String.Empty;
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(GI);
                    var resultado = IPlanificacion.ActualizarIdentificacion(Datos);
                    string[] datos = resultado.Split('|');

                    if (datos[0] == "Actualizado")
                    {
                        mensaje = string.Format("{0}|{1}|{2}", datos[0], datos[1], datos[2]);
                        HttpContext.Session["_SESS_CAPEX_INCIATIVA_ID_"] = datos[1];
                        HttpContext.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"] = datos[2];
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        [HttpPost]
        public ActionResult ActualizarIdentificacionCategorizacion(Identificacion.IdentificacionIniciativa Datos)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string mensaje = String.Empty;
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(GI);
                    var resultado = IPlanificacion.ActualizarIdentificacionCategorizacion(Datos);
                    string[] datos = resultado.Split('|');

                    if (datos[0] == "Actualizado")
                    {
                        mensaje = string.Format("{0}|{1}|{2}", datos[0], datos[1], datos[2]);
                        HttpContext.Session["_SESS_CAPEX_INCIATIVA_ID_"] = datos[1];
                        HttpContext.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"] = datos[2];
                        return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        /// <summary>
        /// VALIDAR IDENTIFICACION INICIATIVA
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ValidarIdentificacion(Identificacion.IdentificacionIniciativa Datos)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(GI);
                    string response = IPlanificacion.ValidarIdentificacion(Datos);
                    return Json(new { Mensaje = response }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    string ExceptionResult = "ValidarIdentificacion, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "ERROR" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }


        /// <summary>
        /// VALIDAR IDENTIFICACION INICIATIVA
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ValidarEstadoProyectoRemanenteIdentificacion(string PidToken)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(GI);
                    string response = IPlanificacion.SeleccionarEstadoProyecto(PidToken);
                    if (!string.IsNullOrEmpty(response) && "REMANENTE".Equals(response.Trim(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        return Json(new { Mensaje = "true" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { Mensaje = "false" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    string ExceptionResult = "ValidarEstadoProyectoRemanenteIdentificacion, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "false" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        /// <summary>
        /// VALIDAR IDENTIFICACION INICIATIVA
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ValidarCodigoProyectoRemanenteCategorizacion(string PidToken)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(GI);
                    string response = IPlanificacion.SeleccionarCodigoProyecto(PidToken);
                    if (!string.IsNullOrEmpty(response) && response.Trim().Length > 0 && !"NO DISPONIBLE".Equals(response.Trim(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        return Json(new { Mensaje = "true" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { Mensaje = "false" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    string ExceptionResult = "ValidarCodigoProyectoRemanenteCategorizacion, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "false" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        #endregion

        #region "METODOS CATEGORIZACION"
        /// <remark>
        ///     
        ///     ----------------------------------------------------------------------------
        ///     GRUPO DE METODOS "Categorizacion"
        ///     VERSION     0.0.1
        ///     ----------------------------------------------------------------------------
        ///     PROPOSITO
        ///     ----------------------------------------------------------------------------
        ///     IMPLEMENTAR ENTRADA Y SALIDA DE DATOS, ASI COMO OPERACIONES POR MEDIO
        ///     DE PATRON CREACIONAL FACTORY QUE ES PROVISTO POR CAPA DDD DE INFRAESTRUCTURA
        ///     
        /// </remark>
        /// 

        /// <summary>
        /// LISTAR CATEGORIAS
        /// </summary>
        [HttpGet]
        public ActionResult ListarCategorias()
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(LCAT);
                    string JsonResponse = JsonConvert.SerializeObject(IPlanificacion.ListarCategorias(), Formatting.None);
                    return Json(
                        JsonResponse,
                        JsonRequestBehavior.AllowGet
                        );
                }
                catch (Exception exc)
                {
                    return Redirect("Login");
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        /// <summary>
        /// LISTAR NIVELES DE INGENIERIA
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListarNivelIngenieria()
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(LNII);
                    string JsonResponse = JsonConvert.SerializeObject(IPlanificacion.ListarNivelIngenieria(), Formatting.None);
                    return Json(
                        JsonResponse,
                        JsonRequestBehavior.AllowGet
                        );
                }
                catch (Exception exc)
                {
                    string ExceptionResult = "ListarNivelIngenieria, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Redirect("Login");
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        /// <summary>
        /// LISTAR NIVELES DE INGENIERIA
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListarNivelIngenieriaNoRequiere()
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(LNII);
                    string idNivelIngenieriaNoRequiere = ConfigurationManager.AppSettings.Get("ID_NIVEL_INGENIERIA_NO_REQUIERE");
                    string JsonResponse = JsonConvert.SerializeObject(IPlanificacion.ListarNivelIngenieriaNoRequiere(Int32.Parse(idNivelIngenieriaNoRequiere)), Formatting.None);
                    return Json(
                        JsonResponse,
                        JsonRequestBehavior.AllowGet
                        );
                }
                catch (Exception exc)
                {
                    string ExceptionResult = "ListarNivelIngenieriaNoRequiere, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Redirect("Login");
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        /// <summary>
        /// LISTAR SSO
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListarClasificacionSSO()
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(LSSO);
                    string JsonResponse = JsonConvert.SerializeObject(IPlanificacion.ListarClasificacionSSO(), Formatting.None);
                    return Json(
                        JsonResponse,
                        JsonRequestBehavior.AllowGet
                        );
                }
                catch (Exception exc)
                {
                    string ExceptionResult = "ListarClasificacionSSO, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Redirect("Login");
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// LISTAR ESTANDARES DE SEGURIDAD
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListarEstandarSeguridad(string EssComToken, string EssCSToken)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                if (string.IsNullOrEmpty(EssComToken) || string.IsNullOrEmpty(EssCSToken))
                {
                    return Redirect("Login");
                }
                else
                {
                    try
                    {
                        IPlanificacion = FactoryPlanificacion.delega(LESS);
                        string JsonResponse = JsonConvert.SerializeObject(IPlanificacion.ListarEstandarSeguridad(EssComToken.ToString().Trim(), EssCSToken.ToString().Trim()), Formatting.None);
                        return Json(
                            JsonResponse,
                            JsonRequestBehavior.AllowGet
                            );
                    }
                    catch (Exception exc)
                    {
                        string ExceptionResult = "ListarEstandarSeguridad, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                        CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                        return Redirect("Login");
                    }
                    finally
                    {
                        FactoryPlanificacion = null;
                        IPlanificacion = null;
                    }
                }
            }
        }
        /// <summary>
        /// LISTAR ESTANDARES DE SEGURIDAD
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ObtenerTokenCompania(string Tipo, string Valor)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(OTC);
                    string resultado = IPlanificacion.ObtenerTokenCompania(Tipo, Valor);
                    if (resultado != "Error")
                    {
                        return Json(new { Mensaje = resultado.ToString().Trim() }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = exc.Message.ToString() + "-----" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        ///  METODO PARA SUBIR ARCHIVO DE RESPALDO PARA DESARROLLO
        /// </summary>
        /// <returns></returns>
        public JsonResult SubirArchivoDesarrollo()
        {
            string resultado = string.Empty;
            if (string.IsNullOrEmpty(Convert.ToString(HttpContext.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"])))
            {
                resultado = "ERROR";
                return Json(new { Resultado = resultado }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    string token = HttpContext.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"].ToString();
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase file = Request.Files[i];
                        int fileSize = file.ContentLength;
                        string fileName = file.FileName;
                        string mimeType = file.ContentType;
                        System.IO.Stream fileContent = file.InputStream;

                        string path = Server.MapPath("Scripts/Files/Iniciativas/Categorizacion/" + token);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        var donde = Path.Combine(Server.MapPath("Scripts/Files/Iniciativas/Categorizacion/" + token), fileName);
                        file.SaveAs(donde);
                        resultado = "OK";
                    }
                    return Json(new { Resultado = resultado }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        /// <summary>
        /// METODO PARA SUBIR ARCHIVO DE ANALISIS DE BAJA COMPLEJIDAD
        /// </summary>
        /// <returns></returns>
        public JsonResult SubirArchivoBajaComplejidad()
        {
            string resultado = string.Empty;
            if (string.IsNullOrEmpty(Convert.ToString(HttpContext.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"])))
            {
                resultado = "ERROR";
                return Json(new { Resultado = resultado }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    string token = HttpContext.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"].ToString();
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase file = Request.Files[i];
                        int fileSize = file.ContentLength;
                        string fileName = file.FileName;
                        string mimeType = file.ContentType;
                        System.IO.Stream fileContent = file.InputStream;

                        string path = Server.MapPath("Scripts/Files/Iniciativas/Categorizacion/" + token);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        var donde = Path.Combine(Server.MapPath("Scripts/Files/Iniciativas/Categorizacion/" + token), fileName);
                        file.SaveAs(donde);
                        resultado = "OK";
                    }
                    return Json(new { Resultado = resultado }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = exc.Message.ToString() + "-----" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        /// <summary>
        /// METODO GUARDAR CATEGORIZACION
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GuardarCategorizacion(Categorizacion.DatosCategorizacion Datos)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(GCAT);
                    string resultado = IPlanificacion.GuardarCategorizacion(Datos);
                    if (resultado == "Guardado")
                    {
                        return Json(new { Mensaje = "Guardado" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = exc.Message.ToString() + "-----" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// METODO ACTUALIZAR CATEGORIZACION
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ActualizarCategorizacion(Categorizacion.DatosCategorizacion Datos)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(GCAT);
                    string resultado = IPlanificacion.ActualizarCategorizacion(Datos);
                    if (resultado == "Actualizado")
                    {
                        return Json(new { Mensaje = "Actualizado" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = exc.Message.ToString() + "-----" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        ///  REGISTRO DE ARCHIVOS EN DB
        /// </summary>
        /// <param name="IniToken"></param>
        /// <param name="ParUsuario"></param>
        /// <param name="ParNombre"></param>
        /// <param name="ParPaso"></param>
        /// <param name="ParCaso"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RegistrarArchivo(string IniToken, string ParUsuario, string ParNombre, string ParPaso, string ParCaso)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(RA);
                    string JsonResponse = JsonConvert.SerializeObject(IPlanificacion.RegistrarArchivo(IniToken, ParUsuario, ParNombre, ParPaso, ParCaso), Formatting.None);
                    return Json(
                            JsonResponse,
                            JsonRequestBehavior.AllowGet
                            );

                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = exc.Message.ToString() + "-----" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// ACTUALIZA ETAPA EN INICIATIVA GUARDADA
        /// </summary>
        /// <param name="token"></param>
        /// <param name="etapa"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ActualizarEtapa(string token, string etapa)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(AEA);
                    string JsonResponse = JsonConvert.SerializeObject(IPlanificacion.ActualizarEtapa(token, etapa), Formatting.None);
                    return Json(
                            JsonResponse,
                            JsonRequestBehavior.AllowGet
                            );

                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = exc.Message.ToString() + "-----" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        #endregion

        #region "METODOS PRESUPUESTO"
        /// <remark>
        ///     
        ///     ----------------------------------------------------------------------------
        ///     GRUPO DE METODOS "Presupuesto"
        ///     VERSION     0.0.1
        ///     ----------------------------------------------------------------------------
        ///     PROPOSITO
        ///     ----------------------------------------------------------------------------
        ///     IMPLEMENTAR ENTRADA Y SALIDA DE DATOS, ASI COMO OPERACIONES POR MEDIO
        ///     DE PATRON CREACIONAL FACTORY QUE ES PROVISTO POR CAPA DDD DE INFRAESTRUCTURA
        ///     
        /// </remark>
        /// 
        public JsonResult SubirTemplatePresupuesto()
        {
            string resultado = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(HttpContext.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"])))
                {
                    resultado = "ERROR";
                    return Json(new { Resultado = resultado });
                }
                else
                {
                    string token = HttpContext.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"].ToString();
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        UploadDownload uploadDownload = new UploadDownload();
                        //Parámetros:
                        //✓ string shareFile: recurso compartido de azure
                        //✓ string pathdirectory: directorio del archivo
                        //✓ string namefile: Nombre del archivo
                        //✓ HtmlInputFile pathFile: contenido del objeto tipo HtmlInputFile

                        HttpPostedFileBase file = Request.Files[i];
                        int fileSize = file.ContentLength;
                        string fileName = file.FileName;
                        string mimeType = file.ContentType;

                        //UploadDownload.UploadFile()
                        System.IO.Stream fileContent = file.InputStream;

                        string path = Server.MapPath("Scripts/Import/" + token);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        var donde = Path.Combine(Server.MapPath("Scripts/Import/" + token), fileName);
                        file.SaveAs(donde);
                        resultado = "OK";
                    }
                    return Json(new { Resultado = resultado }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception exc)
            {
                return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// ProcesarTemplatePresupuestoFinal
        /// NOTA: TRASPASO DE BUSINESS A CONTROLLER POR CAMBIO SOLICITADO CLIENTE PARA ADOPCION DE AZURE STORAGE
        ///       ANULACION DE INTERFAZ "IPlanificacion"
        /// </summary>
        /// <param name="token"></param>
        /// <param name="usuario"></param>
        /// <param name="archivo"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ProcesarTemplatePresupuestoFinal(string token, string usuario, string archivo, string tipo, string parFile)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Boolean isOk = false;
                Boolean eliminarTemplate = false;
                IPlanificacion = FactoryPlanificacion.delega(IT);
                try
                {
                    if (tipo == "CB" || tipo == "CD")
                    {
                        string json = JsonConvert.SerializeObject(ImportarTemplateCasoBaseFinal(token, usuario, archivo), Formatting.None);
                        isOk = true;
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        string json = JsonConvert.SerializeObject(ImportarTemplateFinal(token, usuario, archivo), Formatting.None);
                        isOk = true;
                        return Json(json, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (InvalidParameterExcelException iexc)
                {
                    eliminarTemplate = true;
                    return Json(new { errorFormatoTemplate = "true", mensajeError = iexc.Message.ToString() }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    if (eliminarTemplate && !string.IsNullOrEmpty(parFile))
                    {
                        var adjunto = IPlanificacion.SeleccionarAdjunto(parFile);
                        if (adjunto != null)
                        {
                            string shareFile = (!string.IsNullOrEmpty(adjunto.ShareFile) ? adjunto.ShareFile : String.Empty);
                            string pathDirectory = (!string.IsNullOrEmpty(adjunto.PathDirectory) ? adjunto.PathDirectory : String.Empty);
                            string nameFile = (!string.IsNullOrEmpty(adjunto.ParNombreFinal) ? adjunto.ParNombreFinal : ((!string.IsNullOrEmpty(adjunto.ParNombre) ? adjunto.ParNombre : String.Empty)));
                            if (!string.IsNullOrEmpty(nameFile) && !string.IsNullOrEmpty(shareFile) && !string.IsNullOrEmpty(pathDirectory))
                            {
                                //AZURE
                                UploadDownload uploadDownload = new UploadDownload();
                                if (UploadDownload.DeleteFile(shareFile, pathDirectory, nameFile))
                                {
                                    IPlanificacion.EliminarAdjuntoVigente(parFile, usuario);
                                }
                            }
                            else
                            {
                                IPlanificacion.EliminarAdjuntoVigente(token, usuario);
                            }
                        }
                    }
                    if (isOk || eliminarTemplate)
                    {
                        //delete file
                        string ruta = Path.Combine(Server.MapPath("~/Scripts/Import/" + token), archivo);
                        try
                        {
                            // Check if file exists with its full path    
                            if (System.IO.File.Exists(ruta))
                            {
                                System.IO.File.Delete(ruta);
                            }
                        }
                        catch (IOException ioExp)
                        {
                            Console.WriteLine(ioExp.Message);
                        }
                    }
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        /// <summary>
        /// PROCESO TEMPLATE
        /// NOTA: TRASPASO DE BUSINESS A CONTROLLER POR CAMBIO SOLICITADO CLIENTE PARA ADOPCION DE AZURE STORAGE
        ///       ANULACION DE INTERFAZ "IPlanificacion"
        /// </summary>
        /// <param name="token"></param>
        /// <param name="usuario"></param>
        /// <param name="archivo"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ProcesarTemplatePresupuesto(string token, string usuario, string archivo, string tipo)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    if (tipo == "CB" || tipo == "CD")
                    {
                        string json = JsonConvert.SerializeObject(ImportarTemplateCasoBase(token, usuario, archivo), Formatting.None);
                        return Json(
                            json,
                            JsonRequestBehavior.AllowGet
                            );
                    }
                    else
                    {
                        IPlanificacion = FactoryPlanificacion.delega(IT);
                        string json = JsonConvert.SerializeObject(ImportarTemplate(token, usuario, archivo), Formatting.None);
                        return Json(
                            json,
                            JsonRequestBehavior.AllowGet
                            );
                    }
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        private bool validarParametroComercialMes(string token, int tipoIniciativaSeleccionado, string tipoParam, int mes, string valueParam)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    parametos.Add("Token", token);
                    parametos.Add("TipoIniciativaSeleccionado", tipoIniciativaSeleccionado);
                    parametos.Add("TipoParam", tipoParam);
                    parametos.Add("Mes", mes);
                    parametos.Add("ValueParam", valueParam);
                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 1);
                    SqlMapper.Query(objConnection, "CAPEX_VALIDAR_PARAMETRO_ORIENTACION_COMERCIAL_MES", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    string respuesta = parametos.Get<string>("Respuesta");
                    if (respuesta != null && !string.IsNullOrEmpty(respuesta.Trim()))
                    {
                        return "0".Equals(respuesta.Trim());
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception err)
                {
                    err.ToString();
                    return false;
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        private bool validarParametroComercialAnio(string token, int tipoIniciativaSeleccionado, string tipoParam, int anio, string valueParam)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    parametos.Add("Token", token);
                    parametos.Add("TipoIniciativaSeleccionado", tipoIniciativaSeleccionado);
                    parametos.Add("TipoParam", tipoParam);
                    parametos.Add("Anio", anio);
                    parametos.Add("ValueParam", valueParam);
                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 1);
                    SqlMapper.Query(objConnection, "CAPEX_VALIDAR_PARAMETRO_ORIENTACION_COMERCIAL_ANIO", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    string respuesta = parametos.Get<string>("Respuesta");
                    if (respuesta != null && !string.IsNullOrEmpty(respuesta.Trim()))
                    {
                        return "0".Equals(respuesta.Trim());
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception err)
                {
                    err.ToString();
                    return false;
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        private string obtenerMes(int mes)
        {
            string stringMes = string.Empty;
            switch (mes)
            {
                case 1:
                    stringMes = "Enero";
                    break;
                case 2:
                    stringMes = "Febrero";
                    break;
                case 3:
                    stringMes = "Marzo";
                    break;
                case 4:
                    stringMes = "Abril";
                    break;
                case 5:
                    stringMes = "Mayo";
                    break;
                case 6:
                    stringMes = "Junio";
                    break;
                case 7:
                    stringMes = "Julio";
                    break;
                case 8:
                    stringMes = "Agosto";
                    break;
                case 9:
                    stringMes = "Septiembre";
                    break;
                case 10:
                    stringMes = "Octubre";
                    break;
                case 11:
                    stringMes = "Noviembre";
                    break;
                case 12:
                    stringMes = "Diciembre";
                    break;
            }
            return stringMes;
        }

        private string checkNumberFormat(string paramValue)
        {
            if (!string.IsNullOrEmpty(paramValue))
            {
                if (paramValue.IndexOf(".") != -1 && paramValue.IndexOf(",") != -1)
                {
                    int posicionPunto = paramValue.IndexOf(".");
                    int posicionComa = paramValue.IndexOf(",");
                    if (posicionComa > posicionPunto)
                    {
                        //separador decimal es la coma, separador de miles es el punto
                        paramValue = paramValue.Replace(".", "");
                        return paramValue.Replace(",", ".");
                    }
                    else
                    {
                        paramValue = paramValue.Replace(",", ";");
                        paramValue = paramValue.Replace(".", ",");
                        return paramValue.Replace(";", "");
                    }
                }
                else if (paramValue.IndexOf(".") != -1 && paramValue.IndexOf(",") == -1)//tiene puntos pero no tiene comas
                {
                    string[] splitParamValue = paramValue.Split('.');
                    if (splitParamValue.Length == 2)
                    {
                        if (splitParamValue[1].Length < 3)
                        {
                            //el punto se ocupo para separador decimal
                            return paramValue;
                        }
                        else if (splitParamValue[0].Length >= 1 && splitParamValue[0].Length <= 3 && splitParamValue[1].Length == 3)
                        {
                            return paramValue.Replace(".", "");
                        }
                        else
                        {
                            return paramValue;
                        }
                    }
                    else
                    {
                        paramValue = paramValue.Replace(".", "");
                    }
                }
                else if (paramValue.IndexOf(".") == -1 && paramValue.IndexOf(",") != -1)
                {
                    string[] splitParamValue = paramValue.Split(',');
                    if (splitParamValue.Length == 2)
                    {
                        if (splitParamValue[1].Length < 3)
                        {
                            //la coma se ocupo para separador decimal
                            return paramValue.Replace(",", ".");
                        }
                        else if (splitParamValue[0].Length >= 1 && splitParamValue[0].Length <= 3 && splitParamValue[1].Length == 3)
                        {
                            return paramValue.Replace(",", "");
                        }
                        else
                        {
                            return paramValue;
                        }
                    }
                    else
                    {
                        paramValue = paramValue.Replace(",", "");
                    }
                }
                else
                {
                    return paramValue;
                }
            }
            return "";
        }

        private bool isNumericValue(string paramValue)
        {
            try
            {
                decimal.Parse(paramValue, NumberStyles.Number | NumberStyles.AllowExponent);
                return true;
            }
            catch (Exception err)
            {
                err.ToString();
            }
            return false;
        }

        private string iniPeriodoIniciativa(string token)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var Base = SqlMapper.Query(objConnection, "CAPEX_SEL_PLANIFICACION_INICIATIVA", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                    foreach (var b in Base)
                    {
                        return ((b.IniPeriodo != null) ? b.IniPeriodo.ToString() : "");
                    }
                }
                catch (Exception err)
                {
                    err.ToString();
                    return null;
                }
                finally
                {
                    objConnection.Close();
                }
                return null;
            }
        }
        /// <summary>
        /// IMPORTACION - CASO BASE 
        /// NOTA: TRASPASO DE BUSINESS A CONTROLLER POR CAMBIO SOLICITADO CLIENTE PARA ADOPCION DE AZURE STORAGE
        /// </summary>
        /// <param name="token"></param>
        /// <param name="usuario"></param>
        /// <param name="archivo"></param>
        /// <returns></returns>

        public string ImportarTemplateCasoBaseFinal(string token, string usuario, string archivo)
        {

            /*-------------------------- CONFIGURAR --------------------------------*/
            List<String> registro = new List<String>();

            //string path = ConfigurationManager.AppSettings.Get("CAPEX_IMPOR_PATH");
            //var workbook = new XLWorkbook(path + token + "\\" + archivo);
            string ruta = Path.Combine(Server.MapPath("~/Scripts/Import/" + token), archivo);
            var workbook = new XLWorkbook(ruta);

            /*-------------------------- FINANCIERO --------------------------------*/
            var ws = workbook.Worksheet(2);
            /*-------------------------- ESTRUCTURAR --------------------------------*/
            string tipoTC = "1";
            string tipoIPC = "2";
            string tipoCPI = "3";
            for (int T = 27; T < 30; T++)
            {
                //Proceso por MESES
                for (int M = 4; M <= 15; M++)
                {
                    string originalCellValue = ((ws.Cell(T, M) != null && ws.Cell(T, M).Value != null) ? ws.Cell(T, M).Value.ToString() : "");
                    InsertarTraceLog(token, "T=" + T + ",M=" + M + ", Mes originalCellValue=" + originalCellValue, usuario);
                    string cellValue = checkNumberFormat(originalCellValue);
                    InsertarTraceLog(token, "T=" + T + ",M=" + M + ", Mes cellValue=" + cellValue, usuario);
                    int mes = (M - 3);
                    if (T == 27) //TC
                    {
                        if (string.IsNullOrEmpty(cellValue) || !isNumericValue(cellValue) || !validarParametroComercialMes(token, 1, tipoTC, mes, cellValue))
                        {
                            string mesString = obtenerMes(mes);
                            InsertarTraceLog(token, "Error en el parámetro tc para el mes de " + mesString + ".", usuario);
                            throw new InvalidParameterExcelException("Error en el parámetro tc para el mes de " + mesString + ".");
                        }
                    }
                    if (T == 28) //IPC
                    {
                        if (string.IsNullOrEmpty(cellValue) || !isNumericValue(cellValue) || !validarParametroComercialMes(token, 1, tipoIPC, mes, cellValue))
                        {
                            string mesString = obtenerMes(mes);
                            InsertarTraceLog(token, "Error en el parámetro ipc para el mes de " + mesString + ".", usuario);
                            throw new InvalidParameterExcelException("Error en el parámetro ipc para el mes de " + mesString + ".");
                        }
                    }
                    if (T == 29) //CPI
                    {
                        if (string.IsNullOrEmpty(cellValue) || !isNumericValue(cellValue) || !validarParametroComercialMes(token, 1, tipoCPI, mes, cellValue))
                        {
                            string mesString = obtenerMes(mes);
                            InsertarTraceLog(token, "Error en el parámetro cpi para el mes de " + mesString + ".", usuario);
                            throw new InvalidParameterExcelException("Error en el parámetro cpi para el mes de " + mesString + ".");
                        }
                    }
                }

                string iniPeriodo = iniPeriodoIniciativa(token);
                if (string.IsNullOrEmpty(iniPeriodo))
                {
                    throw new InvalidParameterExcelException("Error la iniciativa es incorrecta.");
                }
                int iniciativaIniPeriodo = 0;
                try
                {
                    iniciativaIniPeriodo = Int32.Parse(iniPeriodo);
                }
                catch (Exception e)
                {
                    e.ToString();
                    throw new InvalidParameterExcelException("Error la iniciativa es incorrecta.");
                }
                //Proceso por AÑOS
                int offsetAnio = 0;
                for (int M = 17; M <= 96; M++)
                {
                    iniciativaIniPeriodo++;
                    offsetAnio++;
                    string originalCellValue = ((ws.Cell(T, M) != null && ws.Cell(T, M).Value != null) ? ws.Cell(T, M).Value.ToString() : "");
                    InsertarTraceLog(token, "T=" + T + ",M=" + M + ", Anio originalCellValue=" + originalCellValue, usuario);
                    string cellValue = checkNumberFormat(originalCellValue);
                    InsertarTraceLog(token, "T=" + T + ",M=" + M + ", Anio cellValue=" + cellValue, usuario);
                    if (T == 27) //TC
                    {
                        if (string.IsNullOrEmpty(cellValue) || !isNumericValue(cellValue) || !validarParametroComercialAnio(token, 1, tipoTC, offsetAnio, cellValue))
                        {
                            InsertarTraceLog(token, "Error en el parámetro tc para el año " + iniciativaIniPeriodo + ".", usuario);
                            throw new InvalidParameterExcelException("Error en el parámetro tc para el año " + iniciativaIniPeriodo + ".");
                        }
                    }
                    if (T == 28) //IPC
                    {
                        if (string.IsNullOrEmpty(cellValue) || !isNumericValue(cellValue) || !validarParametroComercialAnio(token, 1, tipoIPC, offsetAnio, cellValue))
                        {
                            InsertarTraceLog(token, "Error en el parámetro ipc para el año " + iniciativaIniPeriodo + ".", usuario);
                            throw new InvalidParameterExcelException("Error en el parámetro ipc para el año " + iniciativaIniPeriodo + ".");
                        }
                    }
                    if (T == 29) //CPI
                    {
                        if (string.IsNullOrEmpty(cellValue) || !isNumericValue(cellValue) || !validarParametroComercialAnio(token, 1, tipoCPI, offsetAnio, cellValue))
                        {
                            InsertarTraceLog(token, "Error en el parámetro cpi para el año " + iniciativaIniPeriodo + ".", usuario);
                            throw new InvalidParameterExcelException("Error en el parámetro cpi para el año " + iniciativaIniPeriodo + ".");
                        }
                    }
                }
            }

            int numIngreso = 0;
            for (int i = 8; i < 27; i += 3)
            {
                registro.Add(token);
                registro.Add(usuario);
                registro.Add(ws.Cell(i, 1).Value.ToString());

                if (!string.IsNullOrEmpty(ws.Cell(i, 2).Value.ToString()) && !(ws.Cell(i, 2).Value.ToString().Equals("NaN")))
                {
                    decimal d01 = decimal.Parse(ws.Cell(i, 2).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
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
                registro.Add(ws.Cell(i, 86).Value.ToString());
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

                if (i <= 23)
                {
                    registro.Add(ws.Cell((i + 1), 1).Value.ToString());
                    registro.Add("0");
                    registro.Add(ws.Cell((i + 1), 3).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 4).Value.ToString());//ENE
                    registro.Add(ws.Cell((i + 1), 5).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 6).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 7).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 8).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 9).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 10).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 11).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 12).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 13).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 14).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 15).Value.ToString());//DIC

                    registro.Add(ws.Cell((i + 1), 16).Value.ToString());//TOTAL
                    registro.Add(ws.Cell((i + 1), 17).Value.ToString());//2021
                    registro.Add(ws.Cell((i + 1), 18).Value.ToString());//2022
                    registro.Add(ws.Cell((i + 1), 19).Value.ToString());//2023
                    registro.Add(ws.Cell((i + 1), 20).Value.ToString());//2024

                    //
                    // CASO BASE
                    //
                    //2025 - 2034
                    registro.Add(ws.Cell((i + 1), 21).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 22).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 23).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 24).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 25).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 26).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 27).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 28).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 29).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 30).Value.ToString());

                    //2035 - 2044
                    registro.Add(ws.Cell((i + 1), 31).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 32).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 33).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 34).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 35).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 36).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 37).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 38).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 39).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 40).Value.ToString());

                    //2045 - 2054
                    registro.Add(ws.Cell((i + 1), 41).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 42).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 43).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 44).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 45).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 46).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 47).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 48).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 49).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 50).Value.ToString());

                    //2055 - 2064
                    registro.Add(ws.Cell((i + 1), 51).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 52).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 53).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 54).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 55).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 56).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 57).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 58).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 59).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 60).Value.ToString());

                    //2065 - 2074
                    registro.Add(ws.Cell((i + 1), 61).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 62).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 63).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 64).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 65).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 66).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 67).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 68).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 69).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 70).Value.ToString());

                    //2075 - 2084
                    registro.Add(ws.Cell((i + 1), 71).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 72).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 73).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 74).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 75).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 76).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 77).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 78).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 79).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 80).Value.ToString());

                    //2085 - 2094
                    registro.Add(ws.Cell((i + 1), 81).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 82).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 83).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 84).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 85).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 86).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 87).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 88).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 89).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 90).Value.ToString());

                    //2095 - 2100
                    registro.Add(ws.Cell((i + 1), 91).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 92).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 93).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 94).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 95).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 96).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 97).Value.ToString());

                    //tercera fila
                    registro.Add(ws.Cell((i + 2), 1).Value.ToString());
                    registro.Add("0");
                    registro.Add(ws.Cell((i + 2), 3).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 4).Value.ToString());//ENE
                    registro.Add(ws.Cell((i + 2), 5).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 6).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 7).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 8).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 9).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 10).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 11).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 12).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 13).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 14).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 15).Value.ToString());//DIC

                    registro.Add(ws.Cell((i + 2), 16).Value.ToString());//TOTAL
                    registro.Add(ws.Cell((i + 2), 17).Value.ToString());//2021
                    registro.Add(ws.Cell((i + 2), 18).Value.ToString());//2022
                    registro.Add(ws.Cell((i + 2), 19).Value.ToString());//2023
                    registro.Add(ws.Cell((i + 2), 20).Value.ToString());//2024

                    //
                    // CASO BASE
                    //
                    //2025 - 2034

                    registro.Add(ws.Cell((i + 2), 21).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 22).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 23).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 24).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 25).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 26).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 27).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 28).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 29).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 30).Value.ToString());

                    //2035 - 2044
                    registro.Add(ws.Cell((i + 2), 31).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 32).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 33).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 34).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 35).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 36).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 37).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 38).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 39).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 40).Value.ToString());
                    //2045 - 2054
                    registro.Add(ws.Cell((i + 2), 41).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 42).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 43).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 44).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 45).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 46).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 47).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 48).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 49).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 50).Value.ToString());

                    //2055 - 2064
                    registro.Add(ws.Cell((i + 2), 51).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 52).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 53).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 54).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 55).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 56).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 57).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 58).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 59).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 60).Value.ToString());

                    //2065 - 2074
                    registro.Add(ws.Cell((i + 2), 61).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 62).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 63).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 64).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 65).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 66).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 67).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 68).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 69).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 70).Value.ToString());

                    //2075 - 2084
                    registro.Add(ws.Cell((i + 2), 71).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 72).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 73).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 74).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 75).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 76).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 77).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 78).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 79).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 80).Value.ToString());

                    //2085 - 2094
                    registro.Add(ws.Cell((i + 2), 81).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 82).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 83).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 84).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 85).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 96).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 87).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 88).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 89).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 90).Value.ToString());

                    //2095 - 2100
                    registro.Add(ws.Cell((i + 2), 91).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 92).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 93).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 94).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 95).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 96).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 97).Value.ToString());

                }

                string PorInvNacExt = string.Empty;
                if (!string.IsNullOrEmpty(ws.Cell(33, 5).Value.ToString()) && !(ws.Cell(33, 5).Value.ToString().Equals("NaN")))
                {
                    PorInvNacExt = Math.Round((ConvertToDouble(ws.Cell(33, 5).Value.ToString()) * 100)).ToString();
                }
                else
                {
                    PorInvNacExt = "0";
                }

                if (!string.IsNullOrEmpty(ws.Cell(33, 7).Value.ToString()) && !(ws.Cell(33, 7).Value.ToString().Equals("NaN")))
                {
                    if (!string.IsNullOrEmpty(PorInvNacExt))
                    {
                        PorInvNacExt = PorInvNacExt + "/" + Math.Round((ConvertToDouble(ws.Cell(33, 7).Value.ToString()) * 100)).ToString();
                    }
                    else
                    {
                        PorInvNacExt = Math.Round((ConvertToDouble(ws.Cell(33, 7).Value.ToString()) * 100)).ToString();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(PorInvNacExt))
                    {
                        PorInvNacExt = PorInvNacExt + "/" + "0";
                    }
                    else
                    {
                        PorInvNacExt = "0/0";
                    }
                }
                InsertarInformacionFinancieraCasoBase(registro, PorInvNacExt, numIngreso);
                registro.Clear();
                numIngreso++;
            }

            /*-------------------------- FINANCIERO RESUMEN--------------------------------*/
            for (int X = 33; X < 35; X++)
            {
                registro.Add(token);
                registro.Add(usuario);
                registro.Add(ws.Cell(X, 1).Value.ToString());
                if (!string.IsNullOrEmpty(ws.Cell(X, 2).Value.ToString()) && !(ws.Cell(X, 2).Value.ToString().Equals("NaN")))
                {
                    decimal p02 = decimal.Parse(ws.Cell(X, 2).Value.ToString()) * 100;
                    string sp02 = p02.ToString("0.0");
                    registro.Add(sp02);
                }
                else
                {
                    registro.Add("0");
                }
                if (!string.IsNullOrEmpty(ws.Cell(X, 3).Value.ToString()) && !(ws.Cell(X, 3).Value.ToString().Equals("NaN")))
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
                if (!string.IsNullOrEmpty(ws1.Cell(e, 2).Value.ToString()) && !(ws1.Cell(e, 2).Value.ToString().Equals("NaN")))
                {
                    decimal t01 = decimal.Parse(ws1.Cell(e, 2).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st01 = t01.ToString("0.0");
                    registro.Add(st01);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 3).Value.ToString()) && !(ws1.Cell(e, 3).Value.ToString().Equals("NaN")))
                {
                    decimal t03 = decimal.Parse(ws1.Cell(e, 3).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st03 = t03.ToString("0.0");
                    registro.Add(st03);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 4).Value.ToString()) && !(ws1.Cell(e, 4).Value.ToString().Equals("NaN")))
                {
                    decimal t04 = decimal.Parse(ws1.Cell(e, 4).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st04 = t04.ToString("0.0");
                    registro.Add(st04);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 5).Value.ToString()) && !(ws1.Cell(e, 5).Value.ToString().Equals("NaN")))
                {
                    decimal t05 = decimal.Parse(ws1.Cell(e, 5).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st05 = t05.ToString("0.0");
                    registro.Add(st05);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 6).Value.ToString()) && !(ws1.Cell(e, 6).Value.ToString().Equals("NaN")))
                {
                    decimal t06 = decimal.Parse(ws1.Cell(e, 6).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st06 = t06.ToString("0.0");
                    registro.Add(st06);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 7).Value.ToString()) && !(ws1.Cell(e, 7).Value.ToString().Equals("NaN")))
                {
                    //decimal t07 = decimal.Parse(ws1.Cell(e, 7).Value.ToString()) * 100;
                    decimal t07 = decimal.Parse(ws1.Cell(e, 7).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st07 = t07.ToString("0.0");
                    registro.Add(st07);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 8).Value.ToString()) && !(ws1.Cell(e, 8).Value.ToString().Equals("NaN")))
                {
                    decimal t08 = decimal.Parse(ws1.Cell(e, 8).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st08 = t08.ToString("0.0");
                    registro.Add(st08);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 9).Value.ToString()) && !(ws1.Cell(e, 9).Value.ToString().Equals("NaN")))
                {
                    decimal t09 = decimal.Parse(ws1.Cell(e, 9).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st09 = t09.ToString("0.0");
                    registro.Add(st09);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 10).Value.ToString()) && !(ws1.Cell(e, 10).Value.ToString().Equals("NaN")))
                {
                    decimal t10 = decimal.Parse(ws1.Cell(e, 10).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st10 = t10.ToString("0.0");
                    registro.Add(st10);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 11).Value.ToString()) && !(ws1.Cell(e, 11).Value.ToString().Equals("NaN")))
                {
                    decimal t11 = decimal.Parse(ws1.Cell(e, 11).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st11 = t11.ToString("0.0");
                    registro.Add(st11);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 12).Value.ToString()) && !(ws1.Cell(e, 12).Value.ToString().Equals("NaN")))
                {
                    decimal t12 = decimal.Parse(ws1.Cell(e, 12).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st12 = t12.ToString("0.0");
                    registro.Add(st12);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 13).Value.ToString()) && !(ws1.Cell(e, 13).Value.ToString().Equals("NaN")))
                {
                    decimal t13 = decimal.Parse(ws1.Cell(e, 13).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st13 = t13.ToString("0.0");
                    registro.Add(st13);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 14).Value.ToString()) && !(ws1.Cell(e, 14).Value.ToString().Equals("NaN")))
                {
                    decimal t14 = decimal.Parse(ws1.Cell(e, 14).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st14 = t14.ToString("0.0");
                    registro.Add(st14);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 15).Value.ToString()) && !(ws1.Cell(e, 15).Value.ToString().Equals("NaN")))
                {
                    decimal t15 = decimal.Parse(ws1.Cell(e, 15).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st15 = t15.ToString("0.0");
                    registro.Add(st15);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 16).Value.ToString()) && !(ws1.Cell(e, 16).Value.ToString().Equals("NaN")))
                {
                    decimal t16 = decimal.Parse(ws1.Cell(e, 16).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st16 = t16.ToString("0.0");
                    registro.Add(st16);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 17).Value.ToString()) && !(ws1.Cell(e, 17).Value.ToString().Equals("NaN")))
                {
                    decimal t17 = decimal.Parse(ws1.Cell(e, 17).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st17 = t17.ToString("0.0");
                    registro.Add(st17);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 18).Value.ToString()) && !(ws1.Cell(e, 18).Value.ToString().Equals("NaN")))
                {
                    decimal t18 = decimal.Parse(ws1.Cell(e, 18).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st18 = t18.ToString("0.0");
                    registro.Add(st18);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 19).Value.ToString()) && !(ws1.Cell(e, 19).Value.ToString().Equals("NaN")))
                {
                    decimal t19 = decimal.Parse(ws1.Cell(e, 19).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st19 = t19.ToString("0.0");
                    registro.Add(st19);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 20).Value.ToString()) && !(ws1.Cell(e, 20).Value.ToString().Equals("NaN")))
                {
                    decimal t20 = decimal.Parse(ws1.Cell(e, 20).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
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
                if (!string.IsNullOrEmpty(ws1.Cell(e, 21).Value.ToString()) && !(ws1.Cell(e, 21).Value.ToString().Equals("NaN")))
                {
                    decimal t21 = decimal.Parse(ws1.Cell(e, 21).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st21 = t21.ToString("0.0");
                    registro.Add(st21);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 22).Value.ToString()) && !(ws1.Cell(e, 22).Value.ToString().Equals("NaN")))
                {
                    decimal t22 = decimal.Parse(ws1.Cell(e, 22).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st22 = t22.ToString("0.0");
                    registro.Add(st22);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 23).Value.ToString()) && !(ws1.Cell(e, 23).Value.ToString().Equals("NaN")))
                {
                    decimal t23 = decimal.Parse(ws1.Cell(e, 23).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st23 = t23.ToString("0.0");
                    registro.Add(st23);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 24).Value.ToString()) && !(ws1.Cell(e, 24).Value.ToString().Equals("NaN")))
                {
                    decimal t24 = decimal.Parse(ws1.Cell(e, 24).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st24 = t24.ToString("0.0");
                    registro.Add(st24);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 25).Value.ToString()) && !(ws1.Cell(e, 25).Value.ToString().Equals("NaN")))
                {
                    decimal t25 = decimal.Parse(ws1.Cell(e, 25).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st25 = t25.ToString("0.0");
                    registro.Add(st25);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 26).Value.ToString()) && !(ws1.Cell(e, 26).Value.ToString().Equals("NaN")))
                {
                    decimal t26 = decimal.Parse(ws1.Cell(e, 26).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st26 = t26.ToString("0.0");
                    registro.Add(st26);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 27).Value.ToString()) && !(ws1.Cell(e, 27).Value.ToString().Equals("NaN")))
                {
                    decimal t27 = decimal.Parse(ws1.Cell(e, 27).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st27 = t27.ToString("0.0");
                    registro.Add(st27);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 28).Value.ToString()) && !(ws1.Cell(e, 28).Value.ToString().Equals("NaN")))
                {
                    decimal t28 = decimal.Parse(ws1.Cell(e, 28).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st28 = t28.ToString("0.0");
                    registro.Add(st28);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 29).Value.ToString()) && !(ws1.Cell(e, 29).Value.ToString().Equals("NaN")))
                {
                    decimal t29 = decimal.Parse(ws1.Cell(e, 29).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st29 = t29.ToString("0.0");
                    registro.Add(st29);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 30).Value.ToString()) && !(ws1.Cell(e, 30).Value.ToString().Equals("NaN")))
                {
                    decimal t30 = decimal.Parse(ws1.Cell(e, 30).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
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
                if (!string.IsNullOrEmpty(ws1.Cell(e, 31).Value.ToString()) && !(ws1.Cell(e, 31).Value.ToString().Equals("NaN")))
                {
                    decimal t31 = decimal.Parse(ws1.Cell(e, 31).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st31 = t31.ToString("0.0");
                    registro.Add(st31);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 32).Value.ToString()) && !(ws1.Cell(e, 32).Value.ToString().Equals("NaN")))
                {
                    decimal t32 = decimal.Parse(ws1.Cell(e, 32).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st32 = t32.ToString("0.0");
                    registro.Add(st32);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 33).Value.ToString()) && !(ws1.Cell(e, 33).Value.ToString().Equals("NaN")))
                {
                    decimal t33 = decimal.Parse(ws1.Cell(e, 33).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st33 = t33.ToString("0.0");
                    registro.Add(st33);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 34).Value.ToString()) && !(ws1.Cell(e, 34).Value.ToString().Equals("NaN")))
                {
                    decimal t34 = decimal.Parse(ws1.Cell(e, 34).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st34 = t34.ToString("0.0");
                    registro.Add(st34);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 35).Value.ToString()) && !(ws1.Cell(e, 35).Value.ToString().Equals("NaN")))
                {
                    decimal t35 = decimal.Parse(ws1.Cell(e, 35).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st35 = t35.ToString("0.0");
                    registro.Add(st35);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 36).Value.ToString()) && !(ws1.Cell(e, 36).Value.ToString().Equals("NaN")))
                {
                    decimal t36 = decimal.Parse(ws1.Cell(e, 36).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st36 = t36.ToString("0.0");
                    registro.Add(st36);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 37).Value.ToString()) && !(ws1.Cell(e, 37).Value.ToString().Equals("NaN")))
                {
                    decimal t37 = decimal.Parse(ws1.Cell(e, 37).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st37 = t37.ToString("0.0");
                    registro.Add(st37);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 38).Value.ToString()) && !(ws1.Cell(e, 38).Value.ToString().Equals("NaN")))
                {
                    decimal t38 = decimal.Parse(ws1.Cell(e, 38).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st38 = t38.ToString("0.0");
                    registro.Add(st38);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 39).Value.ToString()) && !(ws1.Cell(e, 39).Value.ToString().Equals("NaN")))
                {
                    decimal t39 = decimal.Parse(ws1.Cell(e, 39).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st39 = t39.ToString("0.0");
                    registro.Add(st39);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 40).Value.ToString()) && !(ws1.Cell(e, 40).Value.ToString().Equals("NaN")))
                {

                    decimal t40 = decimal.Parse(ws1.Cell(e, 40).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st40 = t40.ToString("0.0");
                    registro.Add(st40);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 41).Value.ToString()) && !(ws1.Cell(e, 41).Value.ToString().Equals("NaN")))
                {
                    decimal t41 = decimal.Parse(ws1.Cell(e, 41).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st41 = t41.ToString("0.0");
                    registro.Add(st41);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 42).Value.ToString()) && !(ws1.Cell(e, 42).Value.ToString().Equals("NaN")))
                {
                    decimal t42 = decimal.Parse(ws1.Cell(e, 42).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st42 = t42.ToString("0.0");
                    registro.Add(st42);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 43).Value.ToString()) && !(ws1.Cell(e, 43).Value.ToString().Equals("NaN")))
                {
                    decimal t43 = decimal.Parse(ws1.Cell(e, 43).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st43 = t43.ToString("0.0");
                    registro.Add(st43);
                }
                else
                {
                    registro.Add("0");
                }


                if (!string.IsNullOrEmpty(ws1.Cell(e, 44).Value.ToString()) && !(ws1.Cell(e, 44).Value.ToString().Equals("NaN")))
                {
                    decimal t44 = decimal.Parse(ws1.Cell(e, 44).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st44 = t44.ToString("0.0");
                    registro.Add(st44);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 45).Value.ToString()) && !(ws1.Cell(e, 45).Value.ToString().Equals("NaN")))
                {
                    decimal t45 = decimal.Parse(ws1.Cell(e, 45).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st45 = t45.ToString("0.0");
                    registro.Add(st45);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 46).Value.ToString()) && !(ws1.Cell(e, 46).Value.ToString().Equals("NaN")))
                {
                    decimal t46 = decimal.Parse(ws1.Cell(e, 46).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st46 = t46.ToString("0.0");
                    registro.Add(st46);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 47).Value.ToString()) && !(ws1.Cell(e, 47).Value.ToString().Equals("NaN")))
                {
                    decimal t47 = decimal.Parse(ws1.Cell(e, 47).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st47 = t47.ToString("0.0");
                    registro.Add(st47);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 48).Value.ToString()) && !(ws1.Cell(e, 48).Value.ToString().Equals("NaN")))
                {
                    decimal t48 = decimal.Parse(ws1.Cell(e, 48).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st48 = t48.ToString("0.0");
                    registro.Add(st48);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 49).Value.ToString()) && !(ws1.Cell(e, 49).Value.ToString().Equals("NaN")))
                {
                    decimal t49 = decimal.Parse(ws1.Cell(e, 49).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st49 = t49.ToString("0.0");
                    registro.Add(st49);
                }
                else
                {
                    registro.Add("0");
                }


                if (!string.IsNullOrEmpty(ws1.Cell(e, 50).Value.ToString()) && !(ws1.Cell(e, 50).Value.ToString().Equals("NaN")))
                {
                    decimal t50 = decimal.Parse(ws1.Cell(e, 50).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
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
                if (!string.IsNullOrEmpty(ws1.Cell(e, 51).Value.ToString()) && !(ws1.Cell(e, 51).Value.ToString().Equals("NaN")))
                {
                    decimal t51 = decimal.Parse(ws1.Cell(e, 51).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st51 = t51.ToString("0.0");
                    registro.Add(st51);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 52).Value.ToString()) && !(ws1.Cell(e, 52).Value.ToString().Equals("NaN")))
                {
                    decimal t52 = decimal.Parse(ws1.Cell(e, 52).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st52 = t52.ToString("0.0");
                    registro.Add(st52);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 53).Value.ToString()) && !(ws1.Cell(e, 53).Value.ToString().Equals("NaN")))
                {
                    decimal t53 = decimal.Parse(ws1.Cell(e, 53).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st53 = t53.ToString("0.0");
                    registro.Add(st53);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 54).Value.ToString()) && !(ws1.Cell(e, 54).Value.ToString().Equals("NaN")))
                {
                    decimal t54 = decimal.Parse(ws1.Cell(e, 54).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st54 = t54.ToString("0.0");
                    registro.Add(st54);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 55).Value.ToString()) && !(ws1.Cell(e, 55).Value.ToString().Equals("NaN")))
                {
                    decimal t55 = decimal.Parse(ws1.Cell(e, 55).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st55 = t55.ToString("0.0");
                    registro.Add(st55);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 56).Value.ToString()) && !(ws1.Cell(e, 56).Value.ToString().Equals("NaN")))
                {
                    decimal t56 = decimal.Parse(ws1.Cell(e, 56).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st56 = t56.ToString("0.0");
                    registro.Add(st56);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 57).Value.ToString()) && !(ws1.Cell(e, 57).Value.ToString().Equals("NaN")))
                {
                    decimal t57 = decimal.Parse(ws1.Cell(e, 57).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st57 = t57.ToString("0.0");
                    registro.Add(st57);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 58).Value.ToString()) && !(ws1.Cell(e, 58).Value.ToString().Equals("NaN")))
                {
                    decimal t58 = decimal.Parse(ws1.Cell(e, 58).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st58 = t58.ToString("0.0");
                    registro.Add(st58);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 59).Value.ToString()) && !(ws1.Cell(e, 59).Value.ToString().Equals("NaN")))
                {
                    decimal t59 = decimal.Parse(ws1.Cell(e, 59).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st59 = t59.ToString("0.0");
                    registro.Add(st59);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 60).Value.ToString()) && !(ws1.Cell(e, 60).Value.ToString().Equals("NaN")))
                {
                    decimal t60 = decimal.Parse(ws1.Cell(e, 60).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
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
                if (!string.IsNullOrEmpty(ws1.Cell(e, 61).Value.ToString()) && !(ws1.Cell(e, 61).Value.ToString().Equals("NaN")))
                {
                    decimal t61 = decimal.Parse(ws1.Cell(e, 61).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st61 = t61.ToString("0.0");
                    registro.Add(st61);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 62).Value.ToString()) && !(ws1.Cell(e, 62).Value.ToString().Equals("NaN")))
                {
                    decimal t62 = decimal.Parse(ws1.Cell(e, 62).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st62 = t62.ToString("0.0");
                    registro.Add(st62);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 63).Value.ToString()) && !(ws1.Cell(e, 63).Value.ToString().Equals("NaN")))
                {
                    decimal t63 = decimal.Parse(ws1.Cell(e, 63).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st63 = t63.ToString("0.0");
                    registro.Add(st63);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 64).Value.ToString()) && !(ws1.Cell(e, 64).Value.ToString().Equals("NaN")))
                {
                    decimal t64 = decimal.Parse(ws1.Cell(e, 64).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st64 = t64.ToString("0.0");
                    registro.Add(st64);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 65).Value.ToString()) && !(ws1.Cell(e, 65).Value.ToString().Equals("NaN")))
                {
                    decimal t65 = decimal.Parse(ws1.Cell(e, 65).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st65 = t65.ToString("0.0");
                    registro.Add(st65);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 66).Value.ToString()) && !(ws1.Cell(e, 66).Value.ToString().Equals("NaN")))
                {
                    decimal t66 = decimal.Parse(ws1.Cell(e, 66).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st66 = t66.ToString("0.0");
                    registro.Add(st66);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 67).Value.ToString()) && !(ws1.Cell(e, 67).Value.ToString().Equals("NaN")))
                {
                    decimal t67 = decimal.Parse(ws1.Cell(e, 67).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st67 = t67.ToString("0.0");
                    registro.Add(st67);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 68).Value.ToString()) && !(ws1.Cell(e, 68).Value.ToString().Equals("NaN")))
                {
                    decimal t68 = decimal.Parse(ws1.Cell(e, 68).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st68 = t68.ToString("0.0");
                    registro.Add(st68);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 69).Value.ToString()) && !(ws1.Cell(e, 69).Value.ToString().Equals("NaN")))
                {
                    decimal t69 = decimal.Parse(ws1.Cell(e, 69).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st69 = t69.ToString("0.0");
                    registro.Add(st69);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 70).Value.ToString()) && !(ws1.Cell(e, 70).Value.ToString().Equals("NaN")))
                {
                    decimal t70 = decimal.Parse(ws1.Cell(e, 70).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
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
                if (!string.IsNullOrEmpty(ws1.Cell(e, 71).Value.ToString()) && !(ws1.Cell(e, 71).Value.ToString().Equals("NaN")))
                {
                    decimal t71 = decimal.Parse(ws1.Cell(e, 71).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st71 = t71.ToString("0.0");
                    registro.Add(st71);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 72).Value.ToString()) && !(ws1.Cell(e, 72).Value.ToString().Equals("NaN")))
                {
                    decimal t72 = decimal.Parse(ws1.Cell(e, 72).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st72 = t72.ToString("0.0");
                    registro.Add(st72);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 73).Value.ToString()) && !(ws1.Cell(e, 73).Value.ToString().Equals("NaN")))
                {
                    decimal t73 = decimal.Parse(ws1.Cell(e, 73).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st73 = t73.ToString("0.0");
                    registro.Add(st73);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 74).Value.ToString()) && !(ws1.Cell(e, 74).Value.ToString().Equals("NaN")))
                {
                    decimal t74 = decimal.Parse(ws1.Cell(e, 74).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st74 = t74.ToString("0.0");
                    registro.Add(st74);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 75).Value.ToString()) && !(ws1.Cell(e, 75).Value.ToString().Equals("NaN")))
                {
                    decimal t75 = decimal.Parse(ws1.Cell(e, 75).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st75 = t75.ToString("0.0");
                    registro.Add(st75);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 76).Value.ToString()) && !(ws1.Cell(e, 76).Value.ToString().Equals("NaN")))
                {
                    decimal t76 = decimal.Parse(ws1.Cell(e, 76).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st76 = t76.ToString("0.0");
                    registro.Add(st76);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 77).Value.ToString()) && !(ws1.Cell(e, 77).Value.ToString().Equals("NaN")))
                {
                    decimal t77 = decimal.Parse(ws1.Cell(e, 77).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st77 = t77.ToString("0.0");
                    registro.Add(st77);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 78).Value.ToString()) && !(ws1.Cell(e, 78).Value.ToString().Equals("NaN")))
                {
                    decimal t78 = decimal.Parse(ws1.Cell(e, 78).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st78 = t78.ToString("0.0");
                    registro.Add(st78);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 79).Value.ToString()) && !(ws1.Cell(e, 79).Value.ToString().Equals("NaN")))
                {
                    decimal t79 = decimal.Parse(ws1.Cell(e, 79).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st79 = t79.ToString("0.0");
                    registro.Add(st79);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 80).Value.ToString()) && !(ws1.Cell(e, 80).Value.ToString().Equals("NaN")))
                {
                    decimal t80 = decimal.Parse(ws1.Cell(e, 80).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
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
                if (!string.IsNullOrEmpty(ws1.Cell(e, 81).Value.ToString()) && !(ws1.Cell(e, 81).Value.ToString().Equals("NaN")))
                {
                    decimal t81 = decimal.Parse(ws1.Cell(e, 81).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st81 = t81.ToString("0.0");
                    registro.Add(st81);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 82).Value.ToString()) && !(ws1.Cell(e, 82).Value.ToString().Equals("NaN")))
                {
                    decimal t82 = decimal.Parse(ws1.Cell(e, 82).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st82 = t82.ToString("0.0");
                    registro.Add(st82);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 83).Value.ToString()) && !(ws1.Cell(e, 83).Value.ToString().Equals("NaN")))
                {
                    decimal t83 = decimal.Parse(ws1.Cell(e, 83).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st83 = t83.ToString("0.0");
                    registro.Add(st83);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 84).Value.ToString()) && !(ws1.Cell(e, 84).Value.ToString().Equals("NaN")))
                {
                    decimal t84 = decimal.Parse(ws1.Cell(e, 84).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st84 = t84.ToString("0.0");
                    registro.Add(st84);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 85).Value.ToString()) && !(ws1.Cell(e, 85).Value.ToString().Equals("NaN")))
                {
                    decimal t85 = decimal.Parse(ws1.Cell(e, 85).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st85 = t85.ToString("0.0");
                    registro.Add(st85);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 86).Value.ToString()) && !(ws1.Cell(e, 86).Value.ToString().Equals("NaN")))
                {
                    decimal t86 = decimal.Parse(ws1.Cell(e, 86).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st86 = t86.ToString("0.0");
                    registro.Add(st86);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 87).Value.ToString()) && !(ws1.Cell(e, 87).Value.ToString().Equals("NaN")))
                {
                    decimal t87 = decimal.Parse(ws1.Cell(e, 87).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st87 = t87.ToString("0.0");
                    registro.Add(st87);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 88).Value.ToString()) && !(ws1.Cell(e, 88).Value.ToString().Equals("NaN")))
                {
                    decimal t88 = decimal.Parse(ws1.Cell(e, 88).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st88 = t88.ToString("0.0");
                    registro.Add(st88);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 89).Value.ToString()) && !(ws1.Cell(e, 89).Value.ToString().Equals("NaN")))
                {
                    decimal t89 = decimal.Parse(ws1.Cell(e, 89).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st89 = t89.ToString("0.0");
                    registro.Add(st89);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 90).Value.ToString()) && !(ws1.Cell(e, 90).Value.ToString().Equals("NaN")))
                {
                    decimal t90 = decimal.Parse(ws1.Cell(e, 90).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
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
                if (!string.IsNullOrEmpty(ws1.Cell(e, 91).Value.ToString()) && !(ws1.Cell(e, 91).Value.ToString().Equals("NaN")))
                {
                    decimal t91 = decimal.Parse(ws1.Cell(e, 91).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st91 = t91.ToString("0.0");
                    registro.Add(st91);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 92).Value.ToString()) && !(ws1.Cell(e, 92).Value.ToString().Equals("NaN")))
                {
                    decimal t92 = decimal.Parse(ws1.Cell(e, 92).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st92 = t92.ToString("0.0");
                    registro.Add(st92);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 93).Value.ToString()) && !(ws1.Cell(e, 93).Value.ToString().Equals("NaN")))
                {
                    decimal t93 = decimal.Parse(ws1.Cell(e, 93).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st93 = t93.ToString("0.0");
                    registro.Add(st93);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 94).Value.ToString()) && !(ws1.Cell(e, 94).Value.ToString().Equals("NaN")))
                {
                    decimal t94 = decimal.Parse(ws1.Cell(e, 94).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st94 = t94.ToString("0.0");
                    registro.Add(st94);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 95).Value.ToString()) && !(ws1.Cell(e, 95).Value.ToString().Equals("NaN")))
                {
                    decimal t95 = decimal.Parse(ws1.Cell(e, 95).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st95 = t95.ToString("0.0");
                    registro.Add(st95);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 96).Value.ToString()) && !(ws1.Cell(e, 96).Value.ToString().Equals("NaN")))
                {
                    decimal t96 = decimal.Parse(ws1.Cell(e, 96).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st96 = t96.ToString("0.0");
                    registro.Add(st96);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 97).Value.ToString()) && !(ws1.Cell(e, 97).Value.ToString().Equals("NaN")))
                {
                    decimal t97 = decimal.Parse(ws1.Cell(e, 97).Value.ToString(), NumberStyles.Number | NumberStyles.AllowExponent) * 100;
                    string st97 = t97.ToString("0.0");
                    registro.Add(st97);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 98).Value.ToString()) && !(ws1.Cell(e, 98).Value.ToString().Equals("NaN")))
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

        private bool IsExponentialFormat(string str)
        {
            double dummy;
            return (str.Contains("E") || str.Contains("e")) && double.TryParse(str, out dummy);
        }

        private string ObtenerParametroSistema(string paramKey)
        {
            string paramValue = string.Empty;
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    parametos.Add("ParamKey", paramKey);
                    var resultado = SqlMapper.Query(objConnection, "CAPEX_SEL_PARAMETRO_SISTEMA", parametos, commandType: CommandType.StoredProcedure).ToList();
                    if (resultado.Count > 0)
                    {
                        foreach (var result in resultado)
                        {
                            paramValue = ((result.ParamValue != null && !string.IsNullOrEmpty(result.ParamValue.ToString())) ? result.ParamValue.ToString() : "");
                        }
                    }
                }
                catch (Exception err)
                {
                    paramValue = string.Empty;
                    err.ToString();
                }
                finally
                {
                    objConnection.Close();
                }
            }
            return paramValue;
        }

        /// <summary>
        /// IMPORTACION - CASO BASE 
        /// NOTA: TRASPASO DE BUSINESS A CONTROLLER POR CAMBIO SOLICITADO CLIENTE PARA ADOPCION DE AZURE STORAGE
        /// </summary>
        /// <param name="token"></param>
        /// <param name="usuario"></param>
        /// <param name="archivo"></param>
        /// <returns></returns>
        public string ImportarTemplateCasoBase(string token, string usuario, string archivo)
        {

            /*-------------------------- CONFIGURAR --------------------------------*/
            List<String> registro = new List<String>();

            //string path = ConfigurationManager.AppSettings.Get("CAPEX_IMPOR_PATH");
            //var workbook = new XLWorkbook(path + token + "\\" + archivo);
            string ruta = Path.Combine(Server.MapPath("Scripts/Import/" + token), archivo);
            var workbook = new XLWorkbook(ruta);

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

                InsertarInformacionFinancieraCasoBase(registro, "", 1);
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
        public void InsertarInformacionFinancieraCasoBase(List<String> Datos, string PorInvNacExt, int numIngreso)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    if (Datos.Count == 293)
                    {
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

                        //fila 2
                        parametos.Add("IfD1Dato0", Datos[99].ToString());
                        parametos.Add("IfD1Dato1", Datos[100].ToString());
                        parametos.Add("IfD1Dato2", Datos[101].ToString());
                        parametos.Add("IfD1Dato3", Datos[102].ToString());
                        parametos.Add("IfD1Dato4", Datos[103].ToString());
                        parametos.Add("IfD1Dato5", Datos[104].ToString());
                        parametos.Add("IfD1Dato6", Datos[105].ToString());
                        parametos.Add("IfD1Dato7", Datos[106].ToString());
                        parametos.Add("IfD1Dato8", Datos[107].ToString());
                        parametos.Add("IfD1Dato9", Datos[108].ToString());
                        parametos.Add("IfD1Dato10", Datos[109].ToString());
                        parametos.Add("IfD1Dato11", Datos[110].ToString());
                        parametos.Add("IfD1Dato12", Datos[111].ToString());
                        parametos.Add("IfD1Dato13", Datos[112].ToString());
                        parametos.Add("IfD1Dato14", Datos[113].ToString());

                        parametos.Add("IfD1Dato15", Datos[114].ToString());

                        parametos.Add("IfD1Dato16", Datos[115].ToString()); //2021 - 2030
                        parametos.Add("IfD1Dato17", Datos[116].ToString());
                        parametos.Add("IfD1Dato18", Datos[117].ToString());
                        parametos.Add("IfD1Dato19", Datos[118].ToString());
                        parametos.Add("IfD1Dato20", Datos[119].ToString());
                        parametos.Add("IfD1Dato21", Datos[120].ToString());
                        parametos.Add("IfD1Dato22", Datos[121].ToString());
                        parametos.Add("IfD1Dato23", Datos[122].ToString());
                        parametos.Add("IfD1Dato24", Datos[123].ToString());
                        parametos.Add("IfD1Dato25", Datos[124].ToString());

                        parametos.Add("IfD1Dato26", Datos[125].ToString()); //2031 - 2040
                        parametos.Add("IfD1Dato27", Datos[126].ToString());
                        parametos.Add("IfD1Dato28", Datos[127].ToString());
                        parametos.Add("IfD1Dato29", Datos[128].ToString());
                        parametos.Add("IfD1Dato30", Datos[129].ToString());
                        parametos.Add("IfD1Dato31", Datos[130].ToString());
                        parametos.Add("IfD1Dato32", Datos[131].ToString());
                        parametos.Add("IfD1Dato33", Datos[132].ToString());
                        parametos.Add("IfD1Dato34", Datos[133].ToString());
                        parametos.Add("IfD1Dato35", Datos[134].ToString());

                        parametos.Add("IfD1Dato36", Datos[135].ToString()); //2041 - 2050
                        parametos.Add("IfD1Dato37", Datos[136].ToString());
                        parametos.Add("IfD1Dato38", Datos[137].ToString());
                        parametos.Add("IfD1Dato39", Datos[138].ToString());
                        parametos.Add("IfD1Dato40", Datos[139].ToString());
                        parametos.Add("IfD1Dato41", Datos[140].ToString());
                        parametos.Add("IfD1Dato42", Datos[141].ToString());
                        parametos.Add("IfD1Dato43", Datos[142].ToString());
                        parametos.Add("IfD1Dato44", Datos[143].ToString());
                        parametos.Add("IfD1Dato45", Datos[144].ToString());

                        parametos.Add("IfD1Dato46", Datos[145].ToString()); //2051 - 2060
                        parametos.Add("IfD1Dato47", Datos[146].ToString());
                        parametos.Add("IfD1Dato48", Datos[147].ToString());
                        parametos.Add("IfD1Dato49", Datos[148].ToString());
                        parametos.Add("IfD1Dato50", Datos[149].ToString());
                        parametos.Add("IfD1Dato51", Datos[150].ToString());
                        parametos.Add("IfD1Dato52", Datos[151].ToString());
                        parametos.Add("IfD1Dato53", Datos[152].ToString());
                        parametos.Add("IfD1Dato54", Datos[153].ToString());
                        parametos.Add("IfD1Dato55", Datos[154].ToString());

                        parametos.Add("IfD1Dato56", Datos[155].ToString()); //2061 - 2070
                        parametos.Add("IfD1Dato57", Datos[156].ToString());
                        parametos.Add("IfD1Dato58", Datos[157].ToString());
                        parametos.Add("IfD1Dato59", Datos[158].ToString());
                        parametos.Add("IfD1Dato60", Datos[159].ToString());
                        parametos.Add("IfD1Dato61", Datos[160].ToString());
                        parametos.Add("IfD1Dato62", Datos[161].ToString());
                        parametos.Add("IfD1Dato63", Datos[162].ToString());
                        parametos.Add("IfD1Dato64", Datos[163].ToString());
                        parametos.Add("IfD1Dato65", Datos[164].ToString());

                        parametos.Add("IfD1Dato66", Datos[165].ToString()); //2071 - 2080
                        parametos.Add("IfD1Dato67", Datos[166].ToString());
                        parametos.Add("IfD1Dato68", Datos[167].ToString());
                        parametos.Add("IfD1Dato69", Datos[168].ToString());
                        parametos.Add("IfD1Dato70", Datos[169].ToString());
                        parametos.Add("IfD1Dato71", Datos[170].ToString());
                        parametos.Add("IfD1Dato72", Datos[171].ToString());
                        parametos.Add("IfD1Dato73", Datos[172].ToString());
                        parametos.Add("IfD1Dato74", Datos[173].ToString());
                        parametos.Add("IfD1Dato75", Datos[174].ToString());

                        parametos.Add("IfD1Dato76", Datos[175].ToString()); //2081 - 2090
                        parametos.Add("IfD1Dato77", Datos[176].ToString());
                        parametos.Add("IfD1Dato78", Datos[177].ToString());
                        parametos.Add("IfD1Dato79", Datos[178].ToString());
                        parametos.Add("IfD1Dato80", Datos[179].ToString());
                        parametos.Add("IfD1Dato81", Datos[180].ToString());
                        parametos.Add("IfD1Dato82", Datos[181].ToString());
                        parametos.Add("IfD1Dato83", Datos[182].ToString());
                        parametos.Add("IfD1Dato84", Datos[183].ToString());
                        parametos.Add("IfD1Dato85", Datos[184].ToString());

                        parametos.Add("IfD1Dato86", Datos[185].ToString()); //2091 - 2100
                        parametos.Add("IfD1Dato87", Datos[186].ToString());
                        parametos.Add("IfD1Dato88", Datos[187].ToString());
                        parametos.Add("IfD1Dato89", Datos[188].ToString());
                        parametos.Add("IfD1Dato90", Datos[189].ToString());
                        parametos.Add("IfD1Dato91", Datos[190].ToString());
                        parametos.Add("IfD1Dato92", Datos[191].ToString());
                        parametos.Add("IfD1Dato93", Datos[192].ToString());
                        parametos.Add("IfD1Dato94", Datos[193].ToString());
                        parametos.Add("IfD1Dato95", Datos[194].ToString());

                        parametos.Add("IfD1Dato96", Datos[195].ToString());//TOTAL
                        //fila 3
                        parametos.Add("IfD2Dato0", Datos[196].ToString());
                        parametos.Add("IfD2Dato1", Datos[197].ToString());
                        parametos.Add("IfD2Dato2", Datos[198].ToString());

                        parametos.Add("IfD2Dato3", Datos[199].ToString());
                        parametos.Add("IfD2Dato4", Datos[200].ToString());
                        parametos.Add("IfD2Dato5", Datos[201].ToString());
                        parametos.Add("IfD2Dato6", Datos[202].ToString());
                        parametos.Add("IfD2Dato7", Datos[203].ToString());
                        parametos.Add("IfD2Dato8", Datos[204].ToString());
                        parametos.Add("IfD2Dato9", Datos[205].ToString());
                        parametos.Add("IfD2Dato10", Datos[206].ToString());
                        parametos.Add("IfD2Dato11", Datos[207].ToString());
                        parametos.Add("IfD2Dato12", Datos[208].ToString());
                        parametos.Add("IfD2Dato13", Datos[209].ToString());
                        parametos.Add("IfD2Dato14", Datos[210].ToString());

                        parametos.Add("IfD2Dato15", Datos[211].ToString());

                        parametos.Add("IfD2Dato16", Datos[212].ToString()); //2021 - 2030
                        parametos.Add("IfD2Dato17", Datos[213].ToString());
                        parametos.Add("IfD2Dato18", Datos[214].ToString());
                        parametos.Add("IfD2Dato19", Datos[215].ToString());
                        parametos.Add("IfD2Dato20", Datos[216].ToString());
                        parametos.Add("IfD2Dato21", Datos[217].ToString());
                        parametos.Add("IfD2Dato22", Datos[218].ToString());
                        parametos.Add("IfD2Dato23", Datos[219].ToString());
                        parametos.Add("IfD2Dato24", Datos[220].ToString());
                        parametos.Add("IfD2Dato25", Datos[221].ToString());

                        parametos.Add("IfD2Dato26", Datos[222].ToString()); //2031 - 2040
                        parametos.Add("IfD2Dato27", Datos[223].ToString());
                        parametos.Add("IfD2Dato28", Datos[224].ToString());
                        parametos.Add("IfD2Dato29", Datos[225].ToString());
                        parametos.Add("IfD2Dato30", Datos[226].ToString());
                        parametos.Add("IfD2Dato31", Datos[227].ToString());
                        parametos.Add("IfD2Dato32", Datos[228].ToString());
                        parametos.Add("IfD2Dato33", Datos[229].ToString());
                        parametos.Add("IfD2Dato34", Datos[230].ToString());
                        parametos.Add("IfD2Dato35", Datos[231].ToString());

                        parametos.Add("IfD2Dato36", Datos[232].ToString()); //2041 - 2050
                        parametos.Add("IfD2Dato37", Datos[233].ToString());
                        parametos.Add("IfD2Dato38", Datos[234].ToString());
                        parametos.Add("IfD2Dato39", Datos[235].ToString());
                        parametos.Add("IfD2Dato40", Datos[236].ToString());
                        parametos.Add("IfD2Dato41", Datos[237].ToString());
                        parametos.Add("IfD2Dato42", Datos[238].ToString());
                        parametos.Add("IfD2Dato43", Datos[239].ToString());
                        parametos.Add("IfD2Dato44", Datos[240].ToString());
                        parametos.Add("IfD2Dato45", Datos[241].ToString());

                        parametos.Add("IfD2Dato46", Datos[242].ToString()); //2051 - 2060
                        parametos.Add("IfD2Dato47", Datos[243].ToString());
                        parametos.Add("IfD2Dato48", Datos[244].ToString());
                        parametos.Add("IfD2Dato49", Datos[245].ToString());
                        parametos.Add("IfD2Dato50", Datos[246].ToString());
                        parametos.Add("IfD2Dato51", Datos[247].ToString());
                        parametos.Add("IfD2Dato52", Datos[248].ToString());
                        parametos.Add("IfD2Dato53", Datos[249].ToString());
                        parametos.Add("IfD2Dato54", Datos[250].ToString());
                        parametos.Add("IfD2Dato55", Datos[251].ToString());

                        parametos.Add("IfD2Dato56", Datos[252].ToString()); //2061 - 2070
                        parametos.Add("IfD2Dato57", Datos[253].ToString());
                        parametos.Add("IfD2Dato58", Datos[254].ToString());
                        parametos.Add("IfD2Dato59", Datos[255].ToString());
                        parametos.Add("IfD2Dato60", Datos[256].ToString());
                        parametos.Add("IfD2Dato61", Datos[257].ToString());
                        parametos.Add("IfD2Dato62", Datos[258].ToString());
                        parametos.Add("IfD2Dato63", Datos[259].ToString());
                        parametos.Add("IfD2Dato64", Datos[260].ToString());
                        parametos.Add("IfD2Dato65", Datos[261].ToString());

                        parametos.Add("IfD2Dato66", Datos[262].ToString()); //2071 - 2080
                        parametos.Add("IfD2Dato67", Datos[263].ToString());
                        parametos.Add("IfD2Dato68", Datos[264].ToString());
                        parametos.Add("IfD2Dato69", Datos[265].ToString());
                        parametos.Add("IfD2Dato70", Datos[266].ToString());
                        parametos.Add("IfD2Dato71", Datos[267].ToString());
                        parametos.Add("IfD2Dato72", Datos[268].ToString());
                        parametos.Add("IfD2Dato73", Datos[269].ToString());
                        parametos.Add("IfD2Dato74", Datos[270].ToString());
                        parametos.Add("IfD2Dato75", Datos[271].ToString());

                        parametos.Add("IfD2Dato76", Datos[272].ToString()); //2081 - 2090
                        parametos.Add("IfD2Dato77", Datos[273].ToString());
                        parametos.Add("IfD2Dato78", Datos[274].ToString());
                        parametos.Add("IfD2Dato79", Datos[275].ToString());
                        parametos.Add("IfD2Dato80", Datos[276].ToString());
                        parametos.Add("IfD2Dato81", Datos[277].ToString());
                        parametos.Add("IfD2Dato82", Datos[278].ToString());
                        parametos.Add("IfD2Dato83", Datos[279].ToString());
                        parametos.Add("IfD2Dato84", Datos[280].ToString());
                        parametos.Add("IfD2Dato85", Datos[281].ToString());

                        parametos.Add("IfD2Dato86", Datos[282].ToString()); //2091 - 2100
                        parametos.Add("IfD2Dato87", Datos[283].ToString());
                        parametos.Add("IfD2Dato88", Datos[284].ToString());
                        parametos.Add("IfD2Dato89", Datos[285].ToString());
                        parametos.Add("IfD2Dato90", Datos[286].ToString());
                        parametos.Add("IfD2Dato91", Datos[287].ToString());
                        parametos.Add("IfD2Dato92", Datos[288].ToString());
                        parametos.Add("IfD2Dato93", Datos[289].ToString());
                        parametos.Add("IfD2Dato94", Datos[290].ToString());
                        parametos.Add("IfD2Dato95", Datos[291].ToString());

                        parametos.Add("IfD2Dato96", Datos[292].ToString());//TOTAL
                        parametos.Add("PorInvNacExt", PorInvNacExt);
                        parametos.Add("Opcion", 1);
                        parametos.Add("NumIngreso", numIngreso);
                    }
                    else
                    {
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
                        //fila 2
                        parametos.Add("IfD1Dato0", "");
                        parametos.Add("IfD1Dato1", "");
                        parametos.Add("IfD1Dato2", "");

                        parametos.Add("IfD1Dato3", "");
                        parametos.Add("IfD1Dato4", "");
                        parametos.Add("IfD1Dato5", "");
                        parametos.Add("IfD1Dato6", "");
                        parametos.Add("IfD1Dato7", "");
                        parametos.Add("IfD1Dato8", "");
                        parametos.Add("IfD1Dato9", "");
                        parametos.Add("IfD1Dato10", "");
                        parametos.Add("IfD1Dato11", "");
                        parametos.Add("IfD1Dato12", "");
                        parametos.Add("IfD1Dato13", "");
                        parametos.Add("IfD1Dato14", "");

                        parametos.Add("IfD1Dato15", "");

                        parametos.Add("IfD1Dato16", ""); //2021 - 2030
                        parametos.Add("IfD1Dato17", "");
                        parametos.Add("IfD1Dato18", "");
                        parametos.Add("IfD1Dato19", "");
                        parametos.Add("IfD1Dato20", "");
                        parametos.Add("IfD1Dato21", "");
                        parametos.Add("IfD1Dato22", "");
                        parametos.Add("IfD1Dato23", "");
                        parametos.Add("IfD1Dato24", "");
                        parametos.Add("IfD1Dato25", "");

                        parametos.Add("IfD1Dato26", ""); //2031 - 2040
                        parametos.Add("IfD1Dato27", "");
                        parametos.Add("IfD1Dato28", "");
                        parametos.Add("IfD1Dato29", "");
                        parametos.Add("IfD1Dato30", "");
                        parametos.Add("IfD1Dato31", "");
                        parametos.Add("IfD1Dato32", "");
                        parametos.Add("IfD1Dato33", "");
                        parametos.Add("IfD1Dato34", "");
                        parametos.Add("IfD1Dato35", "");

                        parametos.Add("IfD1Dato36", ""); //2041 - 2050
                        parametos.Add("IfD1Dato37", "");
                        parametos.Add("IfD1Dato38", "");
                        parametos.Add("IfD1Dato39", "");
                        parametos.Add("IfD1Dato40", "");
                        parametos.Add("IfD1Dato41", "");
                        parametos.Add("IfD1Dato42", "");
                        parametos.Add("IfD1Dato43", "");
                        parametos.Add("IfD1Dato44", "");
                        parametos.Add("IfD1Dato45", "");

                        parametos.Add("IfD1Dato46", ""); //2051 - 2060
                        parametos.Add("IfD1Dato47", "");
                        parametos.Add("IfD1Dato48", "");
                        parametos.Add("IfD1Dato49", "");
                        parametos.Add("IfD1Dato50", "");
                        parametos.Add("IfD1Dato51", "");
                        parametos.Add("IfD1Dato52", "");
                        parametos.Add("IfD1Dato53", "");
                        parametos.Add("IfD1Dato54", "");
                        parametos.Add("IfD1Dato55", "");

                        parametos.Add("IfD1Dato56", ""); //2061 - 2070
                        parametos.Add("IfD1Dato57", "");
                        parametos.Add("IfD1Dato58", "");
                        parametos.Add("IfD1Dato59", "");
                        parametos.Add("IfD1Dato60", "");
                        parametos.Add("IfD1Dato61", "");
                        parametos.Add("IfD1Dato62", "");
                        parametos.Add("IfD1Dato63", "");
                        parametos.Add("IfD1Dato64", "");
                        parametos.Add("IfD1Dato65", "");

                        parametos.Add("IfD1Dato66", ""); //2071 - 2080
                        parametos.Add("IfD1Dato67", "");
                        parametos.Add("IfD1Dato68", "");
                        parametos.Add("IfD1Dato69", "");
                        parametos.Add("IfD1Dato70", "");
                        parametos.Add("IfD1Dato71", "");
                        parametos.Add("IfD1Dato72", "");
                        parametos.Add("IfD1Dato73", "");
                        parametos.Add("IfD1Dato74", "");
                        parametos.Add("IfD1Dato75", "");

                        parametos.Add("IfD1Dato76", ""); //2081 - 2090
                        parametos.Add("IfD1Dato77", "");
                        parametos.Add("IfD1Dato78", "");
                        parametos.Add("IfD1Dato79", "");
                        parametos.Add("IfD1Dato80", "");
                        parametos.Add("IfD1Dato81", "");
                        parametos.Add("IfD1Dato82", "");
                        parametos.Add("IfD1Dato83", "");
                        parametos.Add("IfD1Dato84", "");
                        parametos.Add("IfD1Dato85", "");

                        parametos.Add("IfD1Dato86", ""); //2091 - 2100
                        parametos.Add("IfD1Dato87", "");
                        parametos.Add("IfD1Dato88", "");
                        parametos.Add("IfD1Dato89", "");
                        parametos.Add("IfD1Dato90", "");
                        parametos.Add("IfD1Dato91", "");
                        parametos.Add("IfD1Dato92", "");
                        parametos.Add("IfD1Dato93", "");
                        parametos.Add("IfD1Dato94", "");
                        parametos.Add("IfD1Dato95", "");
                        parametos.Add("IfD1Dato96", "");//TOTAL
                        //fila 3
                        parametos.Add("IfD2Dato0", "");
                        parametos.Add("IfD2Dato1", "");
                        parametos.Add("IfD2Dato2", "");

                        parametos.Add("IfD2Dato3", "");
                        parametos.Add("IfD2Dato4", "");
                        parametos.Add("IfD2Dato5", "");
                        parametos.Add("IfD2Dato6", "");
                        parametos.Add("IfD2Dato7", "");
                        parametos.Add("IfD2Dato8", "");
                        parametos.Add("IfD2Dato9", "");
                        parametos.Add("IfD2Dato10", "");
                        parametos.Add("IfD2Dato11", "");
                        parametos.Add("IfD2Dato12", "");
                        parametos.Add("IfD2Dato13", "");
                        parametos.Add("IfD2Dato14", "");

                        parametos.Add("IfD2Dato15", "");

                        parametos.Add("IfD2Dato16", ""); //2021 - 2030
                        parametos.Add("IfD2Dato17", "");
                        parametos.Add("IfD2Dato18", "");
                        parametos.Add("IfD2Dato19", "");
                        parametos.Add("IfD2Dato20", "");
                        parametos.Add("IfD2Dato21", "");
                        parametos.Add("IfD2Dato22", "");
                        parametos.Add("IfD2Dato23", "");
                        parametos.Add("IfD2Dato24", "");
                        parametos.Add("IfD2Dato25", "");

                        parametos.Add("IfD2Dato26", ""); //2031 - 2040
                        parametos.Add("IfD2Dato27", "");
                        parametos.Add("IfD2Dato28", "");
                        parametos.Add("IfD2Dato29", "");
                        parametos.Add("IfD2Dato30", "");
                        parametos.Add("IfD2Dato31", "");
                        parametos.Add("IfD2Dato32", "");
                        parametos.Add("IfD2Dato33", "");
                        parametos.Add("IfD2Dato34", "");
                        parametos.Add("IfD2Dato35", "");

                        parametos.Add("IfD2Dato36", ""); //2041 - 2050
                        parametos.Add("IfD2Dato37", "");
                        parametos.Add("IfD2Dato38", "");
                        parametos.Add("IfD2Dato39", "");
                        parametos.Add("IfD2Dato40", "");
                        parametos.Add("IfD2Dato41", "");
                        parametos.Add("IfD2Dato42", "");
                        parametos.Add("IfD2Dato43", "");
                        parametos.Add("IfD2Dato44", "");
                        parametos.Add("IfD2Dato45", "");

                        parametos.Add("IfD2Dato46", ""); //2051 - 2060
                        parametos.Add("IfD2Dato47", "");
                        parametos.Add("IfD2Dato48", "");
                        parametos.Add("IfD2Dato49", "");
                        parametos.Add("IfD2Dato50", "");
                        parametos.Add("IfD2Dato51", "");
                        parametos.Add("IfD2Dato52", "");
                        parametos.Add("IfD2Dato53", "");
                        parametos.Add("IfD2Dato54", "");
                        parametos.Add("IfD2Dato55", "");

                        parametos.Add("IfD2Dato56", ""); //2061 - 2070
                        parametos.Add("IfD2Dato57", "");
                        parametos.Add("IfD2Dato58", "");
                        parametos.Add("IfD2Dato59", "");
                        parametos.Add("IfD2Dato60", "");
                        parametos.Add("IfD2Dato61", "");
                        parametos.Add("IfD2Dato62", "");
                        parametos.Add("IfD2Dato63", "");
                        parametos.Add("IfD2Dato64", "");
                        parametos.Add("IfD2Dato65", "");

                        parametos.Add("IfD2Dato66", ""); //2071 - 2080
                        parametos.Add("IfD2Dato67", "");
                        parametos.Add("IfD2Dato68", "");
                        parametos.Add("IfD2Dato69", "");
                        parametos.Add("IfD2Dato70", "");
                        parametos.Add("IfD2Dato71", "");
                        parametos.Add("IfD2Dato72", "");
                        parametos.Add("IfD2Dato73", "");
                        parametos.Add("IfD2Dato74", "");
                        parametos.Add("IfD2Dato75", "");

                        parametos.Add("IfD2Dato76", ""); //2081 - 2090
                        parametos.Add("IfD2Dato77", "");
                        parametos.Add("IfD2Dato78", "");
                        parametos.Add("IfD2Dato79", "");
                        parametos.Add("IfD2Dato80", "");
                        parametos.Add("IfD2Dato81", "");
                        parametos.Add("IfD2Dato82", "");
                        parametos.Add("IfD2Dato83", "");
                        parametos.Add("IfD2Dato84", "");
                        parametos.Add("IfD2Dato85", "");

                        parametos.Add("IfD2Dato86", ""); //2091 - 2100
                        parametos.Add("IfD2Dato87", "");
                        parametos.Add("IfD2Dato88", "");
                        parametos.Add("IfD2Dato89", "");
                        parametos.Add("IfD2Dato90", "");
                        parametos.Add("IfD2Dato91", "");
                        parametos.Add("IfD2Dato92", "");
                        parametos.Add("IfD2Dato93", "");
                        parametos.Add("IfD2Dato94", "");
                        parametos.Add("IfD2Dato95", "");

                        parametos.Add("IfD2Dato96", "");//TOTAL
                        parametos.Add("PorInvNacExt", PorInvNacExt);
                        parametos.Add("Opcion", 0);
                        parametos.Add("NumIngreso", numIngreso);
                    }
                    SqlMapper.Execute(objConnection, "CAPEX_INS_INFORMACION_FINANCIERA_CASOBASE_2", parametos, commandType: CommandType.StoredProcedure);
                }
                catch (Exception err)
                {
                    ExceptionResult = AppModule + "InsertarInformacionFinancieraCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
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
        /// <param name="Datos"></param>
        public void InsertarInformacionFisicoCasoBase(List<String> Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
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
                    SqlMapper.Execute(objConnection, "CAPEX_INS_INFORMACION_FISICO_CASOBASE", parametos, commandType: CommandType.StoredProcedure);
                }
                catch (Exception err)
                {
                    ExceptionResult = AppModule + "InsertarInformacionFisicoCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
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
        /// <param name="Datos"></param>
        private void InsertarInformacionFinancieraResumidaCasoBase(List<String> Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    parametos.Add("IniToken", Datos[0].ToString());
                    parametos.Add("IniUsuario", Datos[1].ToString());
                    parametos.Add("IrDato0", Datos[2].ToString());
                    parametos.Add("IrDato1", Datos[3].ToString());
                    parametos.Add("IrDato2", Datos[4].ToString());
                    SqlMapper.Execute(objConnection, "CAPEX_INS_INFORMACION_FINANCIERA_RESUMIDA", parametos, commandType: CommandType.StoredProcedure);
                }
                catch (Exception err)
                {
                    ExceptionResult = AppModule + "InsertarInformacionFinancieraResumida, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
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
        /// <param name="Datos"></param>
        private void InsertarInformacionGeneralCasoBase(List<String> Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    parametos.Add("IniToken", Datos[0].ToString());
                    parametos.Add("IniUsuario", Datos[1].ToString());
                    parametos.Add("IgPresupuesto", Datos[2].ToString());
                    parametos.Add("IgFechaInicio", Datos[3].ToString());
                    parametos.Add("IgTermino", Datos[4].ToString());
                    parametos.Add("IgCierre", Datos[5].ToString());
                    SqlMapper.Execute(objConnection, "CAPEX_INS_INFORMACION_GENERAL", parametos, commandType: CommandType.StoredProcedure);
                }
                catch (Exception err)
                {
                    ExceptionResult = AppModule + "InsertarInformacionGeneralCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }


        private double ConvertToDouble(string s)
        {
            char systemSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator[0];
            double result = 0;
            try
            {
                if (!string.IsNullOrEmpty(s))
                {
                    if (!s.Contains(","))
                        result = double.Parse(s, CultureInfo.InvariantCulture);
                    else
                        result = Convert.ToDouble(s.Replace(".", systemSeparator.ToString()).Replace(",", systemSeparator.ToString()));
                }
            }
            catch (Exception e)
            {
                try
                {
                    result = Convert.ToDouble(s);
                }
                catch
                {
                    try
                    {
                        result = Convert.ToDouble(s.Replace(",", ";").Replace(".", ",").Replace(";", "."));
                    }
                    catch
                    {
                        throw new Exception("Wrong string-to-double format");
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// IMPORATCION PP EX
        /// NOTA: TRASPASO DE BUSINESS A CONTROLLER POR CAMBIO SOLICITADO CLIENTE PARA ADOPCION DE AZURE STORAGE
        /// </summary>
        /// <param name="token"></param>
        /// <param name="usuario"></param>
        /// <param name="archivo"></param>
        /// <returns></returns>

        public string ImportarTemplateFinal(string token, string usuario, string archivo)
        {
            /*-------------------------- CONFIGURAR --------------------------------*/
            List<String> registro = new List<String>();
            //string path = ConfigurationManager.AppSettings.Get("CAPEX_IMPOR_PATH");
            //var workbook = new XLWorkbook(path + token + "\\" + archivo);

            string ruta = Path.Combine(Server.MapPath("~/Scripts/Import/" + token), archivo);
            var workbook = new XLWorkbook(ruta);

            /*-------------------------- FINANCIERO --------------------------------*/
            var ws = workbook.Worksheet(2);

            string tipoTC = "1";
            string tipoIPC = "2";
            string tipoCPI = "3";
            for (int T = 27; T < 30; T++)
            {
                //Proceso por MESES
                for (int M = 4; M <= 15; M++)
                {
                    string originalCellValue = ((ws.Cell(T, M) != null && ws.Cell(T, M).Value != null) ? ws.Cell(T, M).Value.ToString() : "");
                    InsertarTraceLog(token, "T=" + T + ",M=" + M + ", Mes originalCellValue=" + originalCellValue, usuario);
                    string cellValue = checkNumberFormat(originalCellValue);
                    InsertarTraceLog(token, "T=" + T + ",M=" + M + ", Mes cellValue=" + cellValue, usuario);
                    int mes = (M - 3);
                    if (T == 27) //TC
                    {
                        if (string.IsNullOrEmpty(cellValue) || !isNumericValue(cellValue) || !validarParametroComercialMes(token, 2, tipoTC, mes, cellValue))
                        {
                            string mesString = obtenerMes(mes);
                            InsertarTraceLog(token, "Error en el parámetro tc para el mes de " + mesString + ".", usuario);
                            throw new InvalidParameterExcelException("Error en el parámetro tc para el mes de " + mesString + ".");
                        }
                    }
                    if (T == 28) //IPC
                    {
                        if (string.IsNullOrEmpty(cellValue) || !isNumericValue(cellValue) || !validarParametroComercialMes(token, 2, tipoIPC, mes, cellValue))
                        {
                            string mesString = obtenerMes(mes);
                            InsertarTraceLog(token, "Error en el parámetro ipc para el mes de " + mesString + ".", usuario);
                            throw new InvalidParameterExcelException("Error en el parámetro ipc para el mes de " + mesString + ".");
                        }
                    }
                    if (T == 29) //CPI
                    {
                        if (string.IsNullOrEmpty(cellValue) || !isNumericValue(cellValue) || !validarParametroComercialMes(token, 2, tipoCPI, mes, cellValue))
                        {
                            string mesString = obtenerMes(mes);
                            InsertarTraceLog(token, "Error en el parámetro cpi para el mes de " + mesString + ".", usuario);
                            throw new InvalidParameterExcelException("Error en el parámetro cpi para el mes de " + mesString + ".");
                        }
                    }
                }

                string iniPeriodo = iniPeriodoIniciativa(token);
                if (string.IsNullOrEmpty(iniPeriodo))
                {
                    throw new InvalidParameterExcelException("Error la iniciativa es incorrecta.");
                }
                int iniciativaIniPeriodo = 0;
                try
                {
                    iniciativaIniPeriodo = Int32.Parse(iniPeriodo);
                }
                catch (Exception e)
                {
                    e.ToString();
                    throw new InvalidParameterExcelException("Error la iniciativa es incorrecta.");
                }
                //Proceso por AÑOS
                int offsetAnio = 0;
                for (int M = 17; M <= 19; M++)
                {
                    iniciativaIniPeriodo++;
                    offsetAnio++;
                    string originalCellValue = ((ws.Cell(T, M) != null && ws.Cell(T, M).Value != null) ? ws.Cell(T, M).Value.ToString() : "");
                    InsertarTraceLog(token, "T=" + T + ",M=" + M + ", Anio originalCellValue=" + originalCellValue, usuario);
                    string cellValue = checkNumberFormat(originalCellValue);
                    InsertarTraceLog(token, "T=" + T + ",M=" + M + ", Anio cellValue=" + cellValue, usuario);
                    if (T == 27) //TC
                    {
                        if (string.IsNullOrEmpty(cellValue) || !isNumericValue(cellValue) || !validarParametroComercialAnio(token, 2, tipoTC, offsetAnio, cellValue))
                        {
                            InsertarTraceLog(token, "Error en el parámetro tc para el año " + iniciativaIniPeriodo + ".", usuario);
                            throw new InvalidParameterExcelException("Error en el parámetro tc para el año " + iniciativaIniPeriodo + ".");
                        }
                    }
                    if (T == 28) //IPC
                    {
                        if (string.IsNullOrEmpty(cellValue) || !isNumericValue(cellValue) || !validarParametroComercialAnio(token, 2, tipoIPC, offsetAnio, cellValue))
                        {
                            InsertarTraceLog(token, "Error en el parámetro ipc para el año " + iniciativaIniPeriodo + ".", usuario);
                            throw new InvalidParameterExcelException("Error en el parámetro ipc para el año " + iniciativaIniPeriodo + ".");
                        }
                    }
                    if (T == 29) //CPI
                    {
                        if (string.IsNullOrEmpty(cellValue) || !isNumericValue(cellValue) || !validarParametroComercialAnio(token, 2, tipoCPI, offsetAnio, cellValue))
                        {
                            InsertarTraceLog(token, "Error en el parámetro cpi para el año " + iniciativaIniPeriodo + ".", usuario);
                            throw new InvalidParameterExcelException("Error en el parámetro cpi para el año " + iniciativaIniPeriodo + ".");
                        }
                    }
                }
            }

            /*-------------------------- ESTRUCTURAR --------------------------------*/
            int numIngreso = 0;
            for (int i = 8; i < 27; i += 3)
            {
                registro.Add(token);
                registro.Add(usuario);
                registro.Add(ws.Cell(i, 1).Value.ToString());
                if (!string.IsNullOrEmpty(ws.Cell(i, 2).Value.ToString()) && !(ws.Cell(i, 2).Value.ToString().Equals("NaN")))
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
                if (i <= 23)
                {
                    registro.Add(ws.Cell((i + 1), 1).Value.ToString());
                    registro.Add("0");
                    registro.Add(ws.Cell((i + 1), 3).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 4).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 5).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 6).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 7).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 8).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 9).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 10).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 11).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 12).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 13).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 14).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 15).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 16).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 17).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 18).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 19).Value.ToString());
                    registro.Add(ws.Cell((i + 1), 20).Value.ToString());

                    registro.Add(ws.Cell((i + 2), 1).Value.ToString());
                    registro.Add("0");
                    registro.Add(ws.Cell((i + 2), 3).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 4).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 5).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 6).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 7).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 8).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 9).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 10).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 11).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 12).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 13).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 14).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 15).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 16).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 17).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 18).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 19).Value.ToString());
                    registro.Add(ws.Cell((i + 2), 20).Value.ToString());
                }
                string PorInvNacExt = string.Empty;
                if (!string.IsNullOrEmpty(ws.Cell(33, 5).Value.ToString()) && !(ws.Cell(33, 5).Value.ToString().Equals("NaN")))
                {
                    PorInvNacExt = Math.Round((ConvertToDouble(ws.Cell(33, 5).Value.ToString()) * 100)).ToString();
                }
                else
                {
                    PorInvNacExt = "0";
                }

                if (!string.IsNullOrEmpty(ws.Cell(33, 7).Value.ToString()) && !(ws.Cell(33, 7).Value.ToString().Equals("NaN")))
                {
                    if (!string.IsNullOrEmpty(PorInvNacExt))
                    {
                        PorInvNacExt = PorInvNacExt + "/" + Math.Round((ConvertToDouble(ws.Cell(33, 7).Value.ToString()) * 100)).ToString();
                    }
                    else
                    {
                        PorInvNacExt = Math.Round((ConvertToDouble(ws.Cell(33, 7).Value.ToString()) * 100)).ToString();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(PorInvNacExt))
                    {
                        PorInvNacExt = PorInvNacExt + "/" + "0";
                    }
                    else
                    {
                        PorInvNacExt = "0/0";
                    }
                }
                InsertarInformacionFinanciera(registro, PorInvNacExt, numIngreso);
                registro.Clear();
                numIngreso++;
            }
            /*-------------------------- FINANCIERO RESUMEN--------------------------------*/
            for (int X = 33; X < 35; X++)
            {
                registro.Add(token);
                registro.Add(usuario);
                registro.Add(ws.Cell(X, 1).Value.ToString());
                if (!string.IsNullOrEmpty(ws.Cell(X, 2).Value.ToString()) && !(ws.Cell(X, 2).Value.ToString().Equals("NaN")))
                {
                    decimal p02 = decimal.Parse(ws.Cell(X, 2).Value.ToString()) * 100;
                    string sp02 = p02.ToString("0.0");
                    registro.Add(sp02);
                }
                else
                {
                    registro.Add("0");
                }
                if (!string.IsNullOrEmpty(ws.Cell(X, 3).Value.ToString()) && !(ws.Cell(X, 3).Value.ToString().Equals("NaN")))
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
                if (!string.IsNullOrEmpty(ws1.Cell(e, 2).Value.ToString()) && !(ws1.Cell(e, 2).Value.ToString().Equals("NaN")))
                {
                    decimal t01 = decimal.Parse(ws1.Cell(e, 2).Value.ToString()) * 100;
                    string st01 = t01.ToString("0.0");
                    registro.Add(st01);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 3).Value.ToString()) && !(ws1.Cell(e, 3).Value.ToString().Equals("NaN")))
                {
                    decimal t03 = decimal.Parse(ws1.Cell(e, 3).Value.ToString()) * 100;
                    string st03 = t03.ToString("0.0");
                    registro.Add(st03);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 4).Value.ToString()) && !(ws1.Cell(e, 4).Value.ToString().Equals("NaN")))
                {
                    decimal t04 = decimal.Parse(ws1.Cell(e, 4).Value.ToString()) * 100;
                    string st04 = t04.ToString("0.0");
                    registro.Add(st04);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 5).Value.ToString()) && !(ws1.Cell(e, 5).Value.ToString().Equals("NaN")))
                {
                    decimal t05 = decimal.Parse(ws1.Cell(e, 5).Value.ToString()) * 100;
                    string st05 = t05.ToString("0.0");
                    registro.Add(st05);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 6).Value.ToString()) && !(ws1.Cell(e, 6).Value.ToString().Equals("NaN")))
                {
                    decimal t06 = decimal.Parse(ws1.Cell(e, 6).Value.ToString()) * 100;
                    string st06 = t06.ToString("0.0");
                    registro.Add(st06);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 7).Value.ToString()) && !(ws1.Cell(e, 7).Value.ToString().Equals("NaN")))
                {
                    decimal t07 = decimal.Parse(ws1.Cell(e, 7).Value.ToString()) * 100;
                    string st07 = t07.ToString("0.0");
                    registro.Add(st07);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 8).Value.ToString()) && !(ws1.Cell(e, 8).Value.ToString().Equals("NaN")))
                {
                    decimal t08 = decimal.Parse(ws1.Cell(e, 8).Value.ToString()) * 100;
                    string st08 = t08.ToString("0.0");
                    registro.Add(st08);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 9).Value.ToString()) && !(ws1.Cell(e, 9).Value.ToString().Equals("NaN")))
                {
                    decimal t09 = decimal.Parse(ws1.Cell(e, 9).Value.ToString()) * 100;
                    string st09 = t09.ToString("0.0");
                    registro.Add(st09);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 10).Value.ToString()) && !(ws1.Cell(e, 10).Value.ToString().Equals("NaN")))
                {
                    decimal t10 = decimal.Parse(ws1.Cell(e, 10).Value.ToString()) * 100;
                    string st10 = t10.ToString("0.0");
                    registro.Add(st10);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 11).Value.ToString()) && !(ws1.Cell(e, 11).Value.ToString().Equals("NaN")))
                {
                    decimal t11 = decimal.Parse(ws1.Cell(e, 11).Value.ToString()) * 100;
                    string st11 = t11.ToString("0.0");
                    registro.Add(st11);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 12).Value.ToString()) && !(ws1.Cell(e, 12).Value.ToString().Equals("NaN")))
                {
                    decimal t12 = decimal.Parse(ws1.Cell(e, 12).Value.ToString()) * 100;
                    string st12 = t12.ToString("0.0");
                    registro.Add(st12);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 13).Value.ToString()) && !(ws1.Cell(e, 13).Value.ToString().Equals("NaN")))
                {
                    decimal t13 = decimal.Parse(ws1.Cell(e, 13).Value.ToString()) * 100;
                    string st13 = t13.ToString("0.0");
                    registro.Add(st13);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 14).Value.ToString()) && !(ws1.Cell(e, 14).Value.ToString().Equals("NaN")))
                {
                    decimal t14 = decimal.Parse(ws1.Cell(e, 14).Value.ToString()) * 100;
                    string st14 = t14.ToString("0.0");
                    registro.Add(st14);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 15).Value.ToString()) && !(ws1.Cell(e, 15).Value.ToString().Equals("NaN")))
                {
                    decimal t15 = decimal.Parse(ws1.Cell(e, 15).Value.ToString()) * 100;
                    string st15 = t15.ToString("0.0");
                    registro.Add(st15);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 16).Value.ToString()) && !(ws1.Cell(e, 16).Value.ToString().Equals("NaN")))
                {
                    decimal t16 = decimal.Parse(ws1.Cell(e, 16).Value.ToString()) * 100;
                    string st16 = t16.ToString("0.0");
                    registro.Add(st16);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 17).Value.ToString()) && !(ws1.Cell(e, 17).Value.ToString().Equals("NaN")))
                {
                    decimal t17 = decimal.Parse(ws1.Cell(e, 17).Value.ToString()) * 100;
                    string st17 = t17.ToString("0.0");
                    registro.Add(st17);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 18).Value.ToString()) && !(ws1.Cell(e, 18).Value.ToString().Equals("NaN")))
                {
                    decimal t18 = decimal.Parse(ws1.Cell(e, 18).Value.ToString()) * 100;
                    string st18 = t18.ToString("0.0");
                    registro.Add(st18);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 19).Value.ToString()) && !(ws1.Cell(e, 19).Value.ToString().Equals("NaN")))
                {
                    decimal t19 = decimal.Parse(ws1.Cell(e, 19).Value.ToString()) * 100;
                    string st19 = t19.ToString("0.0");
                    registro.Add(st19);
                }
                else
                {
                    registro.Add("0");
                }

                if (!string.IsNullOrEmpty(ws1.Cell(e, 20).Value.ToString()) && !(ws1.Cell(e, 20).Value.ToString().Equals("NaN")))
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
        /// IMPORATCION PP EX
        /// NOTA: TRASPASO DE BUSINESS A CONTROLLER POR CAMBIO SOLICITADO CLIENTE PARA ADOPCION DE AZURE STORAGE
        /// </summary>
        /// <param name="token"></param>
        /// <param name="usuario"></param>
        /// <param name="archivo"></param>
        /// <returns></returns>

        public string ImportarTemplate(string token, string usuario, string archivo)
        {
            /*-------------------------- CONFIGURAR --------------------------------*/
            List<String> registro = new List<String>();
            //string path = ConfigurationManager.AppSettings.Get("CAPEX_IMPOR_PATH");
            //var workbook = new XLWorkbook(path + token + "\\" + archivo);

            string ruta = Path.Combine(Server.MapPath("Scripts/Import/" + token), archivo);
            var workbook = new XLWorkbook(ruta);

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

                InsertarInformacionFinanciera(registro, "", 1);
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
        private void InsertarInformacionFinanciera(List<String> Datos, String PorInvNacExt, int numIngreso)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    if (Datos.Count == 62)
                    {
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

                        parametos.Add("IfD1Dato0", Datos[22].ToString());
                        parametos.Add("IfD1Dato1", Datos[23].ToString());
                        parametos.Add("IfD1Dato2", Datos[24].ToString());
                        parametos.Add("IfD1Dato3", Datos[25].ToString());
                        parametos.Add("IfD1Dato4", Datos[26].ToString());
                        parametos.Add("IfD1Dato5", Datos[27].ToString());
                        parametos.Add("IfD1Dato6", Datos[28].ToString());
                        parametos.Add("IfD1Dato7", Datos[29].ToString());
                        parametos.Add("IfD1Dato8", Datos[30].ToString());
                        parametos.Add("IfD1Dato9", Datos[31].ToString());
                        parametos.Add("IfD1Dato10", Datos[32].ToString());
                        parametos.Add("IfD1Dato11", Datos[33].ToString());
                        parametos.Add("IfD1Dato12", Datos[34].ToString());
                        parametos.Add("IfD1Dato13", Datos[35].ToString());
                        parametos.Add("IfD1Dato14", Datos[36].ToString());
                        parametos.Add("IfD1Dato15", Datos[37].ToString());
                        parametos.Add("IfD1Dato16", Datos[38].ToString());
                        parametos.Add("IfD1Dato17", Datos[39].ToString());
                        parametos.Add("IfD1Dato18", Datos[40].ToString());
                        parametos.Add("IfD1Dato19", Datos[41].ToString());

                        parametos.Add("IfD2Dato0", Datos[42].ToString());
                        parametos.Add("IfD2Dato1", Datos[43].ToString());
                        parametos.Add("IfD2Dato2", Datos[44].ToString());
                        parametos.Add("IfD2Dato3", Datos[45].ToString());
                        parametos.Add("IfD2Dato4", Datos[46].ToString());
                        parametos.Add("IfD2Dato5", Datos[47].ToString());
                        parametos.Add("IfD2Dato6", Datos[48].ToString());
                        parametos.Add("IfD2Dato7", Datos[49].ToString());
                        parametos.Add("IfD2Dato8", Datos[50].ToString());
                        parametos.Add("IfD2Dato9", Datos[51].ToString());
                        parametos.Add("IfD2Dato10", Datos[52].ToString());
                        parametos.Add("IfD2Dato11", Datos[53].ToString());
                        parametos.Add("IfD2Dato12", Datos[54].ToString());
                        parametos.Add("IfD2Dato13", Datos[55].ToString());
                        parametos.Add("IfD2Dato14", Datos[56].ToString());
                        parametos.Add("IfD2Dato15", Datos[57].ToString());
                        parametos.Add("IfD2Dato16", Datos[58].ToString());
                        parametos.Add("IfD2Dato17", Datos[59].ToString());
                        parametos.Add("IfD2Dato18", Datos[60].ToString());
                        parametos.Add("IfD2Dato19", Datos[61].ToString());
                        parametos.Add("PorInvNacExt", PorInvNacExt);
                        parametos.Add("Opcion", 1);
                        parametos.Add("NumIngreso", numIngreso);
                    }
                    else
                    {
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

                        parametos.Add("IfD1Dato0", "");
                        parametos.Add("IfD1Dato1", "");
                        parametos.Add("IfD1Dato2", "");
                        parametos.Add("IfD1Dato3", "");
                        parametos.Add("IfD1Dato4", "");
                        parametos.Add("IfD1Dato5", "");
                        parametos.Add("IfD1Dato6", "");
                        parametos.Add("IfD1Dato7", "");
                        parametos.Add("IfD1Dato8", "");
                        parametos.Add("IfD1Dato9", "");
                        parametos.Add("IfD1Dato10", "");
                        parametos.Add("IfD1Dato11", "");
                        parametos.Add("IfD1Dato12", "");
                        parametos.Add("IfD1Dato13", "");
                        parametos.Add("IfD1Dato14", "");
                        parametos.Add("IfD1Dato15", "");
                        parametos.Add("IfD1Dato16", "");
                        parametos.Add("IfD1Dato17", "");
                        parametos.Add("IfD1Dato18", "");
                        parametos.Add("IfD1Dato19", "");

                        parametos.Add("IfD2Dato0", "");
                        parametos.Add("IfD2Dato1", "");
                        parametos.Add("IfD2Dato2", "");
                        parametos.Add("IfD2Dato3", "");
                        parametos.Add("IfD2Dato4", "");
                        parametos.Add("IfD2Dato5", "");
                        parametos.Add("IfD2Dato6", "");
                        parametos.Add("IfD2Dato7", "");
                        parametos.Add("IfD2Dato8", "");
                        parametos.Add("IfD2Dato9", "");
                        parametos.Add("IfD2Dato10", "");
                        parametos.Add("IfD2Dato11", "");
                        parametos.Add("IfD2Dato12", "");
                        parametos.Add("IfD2Dato13", "");
                        parametos.Add("IfD2Dato14", "");
                        parametos.Add("IfD2Dato15", "");
                        parametos.Add("IfD2Dato16", "");
                        parametos.Add("IfD2Dato17", "");
                        parametos.Add("IfD2Dato18", "");
                        parametos.Add("IfD2Dato19", "");
                        parametos.Add("PorInvNacExt", PorInvNacExt);
                        parametos.Add("Opcion", 0);
                        parametos.Add("NumIngreso", numIngreso);
                    }
                    SqlMapper.Execute(objConnection, "CAPEX_INS_INFORMACION_FINANCIERA_2", parametos, commandType: CommandType.StoredProcedure);
                }
                catch (Exception err)
                {
                    ExceptionResult = AppModule + "InsertarInformacionFinanciera, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        private void InsertarTraceLog(string IniToken, string Log, string usuario)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    string finalLog = string.Empty;
                    if (!string.IsNullOrEmpty(Log))
                    {
                        if (Log.Trim().Length <= 2000)
                        {
                            finalLog = Log;
                        }
                        else
                        {
                            finalLog = Log.Trim().Substring(0, 2000);
                        }
                    }
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    parametos.Add("IniToken", IniToken);
                    parametos.Add("TraceLog", Log);
                    parametos.Add("TraceUsuario", usuario);
                    SqlMapper.Execute(objConnection, "CAPEX_INS_TRACE_LOG", parametos, commandType: CommandType.StoredProcedure);
                }
                catch (Exception err)
                {
                    ExceptionResult = AppModule + "CAPEX_INS_TRACE_LOG, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }


        /// <summary>
        /// INSERTAR DATOS DE FINANCIERO RESUMIDA
        /// </summary>
        /// <param name="Datos"></param>
        private void InsertarInformacionFinancieraResumida(List<String> Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    parametos.Add("IniToken", Datos[0].ToString());
                    parametos.Add("IniUsuario", Datos[1].ToString());
                    parametos.Add("IrDato0", Datos[2].ToString());
                    parametos.Add("IrDato1", Datos[3].ToString());
                    parametos.Add("IrDato2", Datos[4].ToString());
                    SqlMapper.Execute(objConnection, "CAPEX_INS_INFORMACION_FINANCIERA_RESUMIDA", parametos, commandType: CommandType.StoredProcedure);
                }
                catch (Exception err)
                {
                    ExceptionResult = AppModule + "InsertarInformacionFinancieraResumida, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }
        /// <summary>
        /// INSERTAR DATOS DE FISICO
        /// </summary>
        /// <param name="Datos"></param>
        private void InsertarInformacionFisico(List<String> Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
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
                    SqlMapper.Execute(objConnection, "CAPEX_INS_INFORMACION_FISICO", parametos, commandType: CommandType.StoredProcedure);
                }
                catch (Exception err)
                {
                    ExceptionResult = AppModule + "InsertarInformacionFisico, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }
        /// <summary>
        /// INSERTAR INFORMACION GENERAL DE IMPORTACION
        /// </summary>
        /// <param name="Datos"></param>
        private void InsertarInformacionGeneral(List<String> Datos)
        {
            var FechaInicio = string.Empty;
            var FechaTermino = string.Empty;
            var FechaCierre = string.Empty;
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    try
                    {
                        /*FechaInicio = String.Format("{0:dd/MM/yyyy}", Datos[3]);
                        FechaTermino = String.Format("{0:dd/MM/yyyy}", Datos[4]);
                        FechaCierre = String.Format("{0:dd/MM/yyyy}", Datos[5]);*/
                        FechaInicio = String.Format("{0}", Datos[3]);
                        FechaTermino = String.Format("{0}", Datos[4]);
                        FechaCierre = String.Format("{0}", Datos[5]);
                    }
                    catch (Exception y)
                    {
                        ExceptionResult = AppModule + "InsertarInformacionGeneral, Mensaje: FORMATO FECHA TEMPLATE " + y.Message.ToString() + "-" + ", Detalle: " + y.StackTrace.ToString();
                        CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    }
                    var parametos = new DynamicParameters();
                    parametos.Add("IniToken", Datos[0].ToString());
                    parametos.Add("IniUsuario", Datos[1].ToString());
                    parametos.Add("IgPresupuesto", Datos[2].ToString());
                    parametos.Add("IgFechaInicio", FechaInicio);
                    parametos.Add("IgTermino", FechaTermino);
                    parametos.Add("IgCierre", FechaCierre);
                    SqlMapper.Execute(objConnection, "CAPEX_INS_INFORMACION_GENERAL", parametos, commandType: CommandType.StoredProcedure);
                }
                catch (Exception err)
                {
                    ExceptionResult = AppModule + "InsertarInformacionGeneral, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        /// <summary>
        /// DESPLIEGUE DE DATOS FINANCIERO
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public string PoblarVistaPresupuestoTabla1(string token)
        {
            try
            {
                IPlanificacion = FactoryPlanificacion.delega(IT);
                return IPlanificacion.PoblarVistaPresupuestoFinanciero(token);
            }
            catch (Exception exc)
            {
                return exc.Message.ToString() + "-----" + exc.StackTrace.ToString();
                //eturn null;
            }
            finally
            {
                FactoryPlanificacion = null;
                IPlanificacion = null;
            }
        }
        /// <summary>
        /// DESPLIEGUE DE DATOS FISICO 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public string PoblarVistaPresupuestoTabla2(string token)
        {
            try
            {
                IPlanificacion = FactoryPlanificacion.delega(PO);
                return IPlanificacion.PoblarVistaPresupuestoFisico(token);
            }
            catch (Exception exc)
            {
                //return null;
                return exc.Message.ToString() + "-----" + exc.StackTrace.ToString();
            }
            finally
            {
                FactoryPlanificacion = null;
                IPlanificacion = null;
            }
        }
        /// <summary>
        /// DESPLIEGUE DE DATOS FINANCIERO -  CASO BASE 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public string PoblarVistaPresupuestoCasoBaseTabla1(string token)
        {
            try
            {
                IPlanificacion = FactoryPlanificacion.delega(IT);
                return IPlanificacion.PoblarVistaPresupuestoFinancieroCasoBase(token);
            }
            catch (Exception exc)
            {
                return null;
                //return exc.Message.ToString() + "-----" + exc.StackTrace.ToString();
            }
            finally
            {
                FactoryPlanificacion = null;
                IPlanificacion = null;
            }
        }
        /// <summary>
        /// DESPLIEGUE DE DATOS FISICO - CASO BASE
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public string PoblarVistaPresupuestoCasoBaseTabla2(string token)
        {
            try
            {
                IPlanificacion = FactoryPlanificacion.delega(IT);
                return IPlanificacion.PoblarVistaPresupuestoFisicoCasoBase(token);
            }
            catch (Exception exc)
            {
                return null;
                //return exc.Message.ToString() + "-----" + exc.StackTrace.ToString();
            }
            finally
            {
                FactoryPlanificacion = null;
                IPlanificacion = null;
            }
        }
        /// <summary>
        /// METODO PARA SUBIR ARCHIVO CARTA GANTT
        /// </summary>
        /// <returns></returns>
        public JsonResult SubirCartaGanttPresupuesto()
        {
            string resultado = string.Empty;
            if (string.IsNullOrEmpty(Convert.ToString(HttpContext.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"])))
            {
                resultado = "ERROR";
                return Json(new { Resultado = resultado });
            }
            else
            {
                try
                {
                    string token = HttpContext.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"].ToString();
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase file = Request.Files[i];
                        int fileSize = file.ContentLength;
                        string fileName = file.FileName;
                        string mimeType = file.ContentType;

                        System.IO.Stream fileContent = file.InputStream;

                        string path = Server.MapPath("Scripts/Files/Iniciativas/Presupuesto/Gantt/" + token);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        var donde = Path.Combine(Server.MapPath("Scripts/Files/Iniciativas/Presupuesto/Gantt/" + token), fileName);
                        file.SaveAs(donde);
                        resultado = "OK";
                    }
                    return Json(new { Resultado = resultado }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        #endregion

        #region "METODOS DOTACION"
        /// <remark>
        ///     
        ///     ----------------------------------------------------------------------------
        ///     GRUPO DE METODOS "Dotacion"
        ///     VERSION     0.0.1
        ///     ----------------------------------------------------------------------------
        ///     PROPOSITO
        ///     ----------------------------------------------------------------------------
        ///     IMPLEMENTAR ENTRADA Y SALIDA DE DATOS, ASI COMO OPERACIONES POR MEDIO
        ///     DE PATRON CREACIONAL FACTORY QUE ES PROVISTO POR CAPA DDD DE INFRAESTRUCTURA
        ///     
        /// </remark>
        /// 

        /// <summary>
        /// METODO LISTAR DEPARTAMENTOS DOTACION
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListarDepartamentos()
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(LDEP);
                    string JsonResponse = JsonConvert.SerializeObject(IPlanificacion.ListarDepartamentos(), Formatting.None);
                    return Json(
                        JsonResponse,
                        JsonRequestBehavior.AllowGet
                        );
                }
                catch (Exception exc)
                {
                    string ExceptionResult = "ListarDepartamentos, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Redirect("Login");
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// METODO LISTAR TURNOS DOTACION
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListarTurnos()
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(LTUR);
                    string JsonResponse = JsonConvert.SerializeObject(IPlanificacion.ListarTurnos(), Formatting.None);
                    return Json(
                        JsonResponse,
                        JsonRequestBehavior.AllowGet
                        );
                }
                catch (Exception exc)
                {
                    string ExceptionResult = "ListarTurnos, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Redirect("Login");
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// METODO LISTAR UBICACIONWS DOTACION
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListarUbicaciones()
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(LUBI);
                    string JsonResponse = JsonConvert.SerializeObject(IPlanificacion.ListarUbicaciones(), Formatting.None);
                    return Json(
                        JsonResponse,
                        JsonRequestBehavior.AllowGet
                        );
                }
                catch (Exception exc)
                {
                    string ExceptionResult = "ListarUbicaciones, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Redirect("Login");
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// METODO LISTAR CLASIFICACION DOTACION
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListaClasificaciones()
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(LCLA);
                    string JsonResponse = JsonConvert.SerializeObject(IPlanificacion.ListarClasificacion(), Formatting.None);
                    return Json(
                        JsonResponse,
                        JsonRequestBehavior.AllowGet
                        );
                }
                catch (Exception exc)
                {
                    string ExceptionResult = "ListaClasificaciones, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Redirect("Login");
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// METODO LISTAR TIPO EECC
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListarTipoEECC()
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(LTUR);
                    string JsonResponse = JsonConvert.SerializeObject(IPlanificacion.ListarTipoEECC(), Formatting.None);
                    return Json(
                        JsonResponse,
                        JsonRequestBehavior.AllowGet
                        );
                }
                catch (Exception exc)
                {
                    string ExceptionResult = "ListarTipoEECC, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Redirect("Login");
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// METODO LISTAR CONTRATOS DE DOTACION
        /// </summary>
        /// <param name="DotToken"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ListarContratosDotacionResumen(string IniToken)
        {

            string Desplegable = String.Empty;
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var resultado = SqlMapper.Query(objConnection, "CAPEX_SEL_CONTRATO_DOTACION_RESUMIDO", new { IniToken }, commandType: CommandType.StoredProcedure).ToList();
                    var contenedor = new StringBuilder();
                    var contador = 1;
                    int totalItems = 0;
                    var Pager = new StringBuilder();
                    if (resultado != null && resultado.Count > 0)
                    {
                        totalItems = resultado.Count;
                        //Pager.Append("<div class='previousPage disabled'>‹</div>");
                        //Pager.Append("<div class='pageNumbers'>");
                        foreach (var result in resultado)
                        {
                            Pager.Append("<a onclick=ChangePage(" + contador + "); href='javascript:void(0);' id='pageSelected_" + contador + "' data-page='" + contador + "' class='" + ((contador == 1) ? "active" : "") + "'>" + contador + "</a>");
                            // UI
                            //INICIO CONTENEDOR DE DATOS PRINCIPAL
                            contenedor.Append("<div id='datosDotacionIniciativa_" + contador + "' class='pb-0'" + ((contador == 1) ? "" : " style='display: none;'") + ">");
                            contenedor.Append("<table>");
                            contenedor.Append("<tr>");
                            //INICIO PRIMER ESPACIO CONTENEDOR PARA DATOS DEL CONTRATO  Y ETIQUETA COD. CONTRATO
                            contenedor.Append("<th valign='top'><label for='TablaDotacionDatos" + contador + "'> Contrato de Dotación - " + result.DotNumContrato + "</label>");
                            //TABLA DATOS DE CONTRATO
                            contenedor.Append("<table id='TablaDotacionDatos" + contador + "' style='width:600px; text-align:center;font-size:11px;' class='table table-bordered'>");
                            contenedor.Append("<tr>");
                            contenedor.Append("<th>Año</th>");
                            contenedor.Append("<th>Contrato</th>");
                            contenedor.Append("<th>Nombre EECC</th>");
                            contenedor.Append("<th>Turno</th>");
                            contenedor.Append("<th>Ubicación</th>");
                            contenedor.Append("<th>Promedio Año</th>");
                            contenedor.Append("</tr>");
                            contenedor.Append("<tr>");
                            contenedor.Append("<td>" + result.DotAnn + "</td>");
                            contenedor.Append("<td>" + result.DotNumContrato + "</td>");
                            contenedor.Append("<td>" + result.DotNombEECC + "</td>");
                            contenedor.Append("<td>" + result.TurNombre + "</td>");
                            contenedor.Append("<td>" + result.UbiNombre + "</td>");
                            contenedor.Append("<td>" + result.DotacionPromedio + "</td>");
                            contenedor.Append("</tr>");
                            contenedor.Append("</table>");
                            contenedor.Append("</th>");
                            //FIN PRIMER ESPACIO CONTENEDOR PARA DATOS DEL CONTRATO
                            //INICIO SEGUNDO ESPACIO CONTENEDOR PARA DATOS DEL CONTRATO
                            contenedor.Append("<th>");
                            contenedor.Append("&nbsp;&nbsp;<input type='button' class='btn btn-primary btn-sm' onclick='FNObtenerDotacion(" + Convert.ToChar(34) + result.DotToken + Convert.ToChar(34) + ")' value='Editar' />");
                            contenedor.Append("&nbsp;&nbsp;<input type='button' class='btn btn-warning btn-sm' onclick='FNEliminarContratoDotacion(" + Convert.ToChar(34) + result.DotToken + Convert.ToChar(34) + ")' value='Eliminar' />");
                            contenedor.Append("</th>");
                            contenedor.Append("</tr>");
                            contenedor.Append("</table>");
                            contenedor.Append(" </div>");
                            //FIN CONTENEDOR DE DATOS PRINCIPAL
                            contador++;
                        }
                        Pager.Append("</div>");
                        //Pager.Append("<div class='nextPage'>›</div>");
                        Desplegable = contenedor.ToString();
                        contenedor = null;
                    }
                    else
                    {
                        //Pager.Append("<div class='previousPage'>></div>");
                        Pager.Append("<div class='pageNumbers'></div>");
                        //Pager.Append("<div class='nextPage'><</div>");
                        contenedor = null;
                        Desplegable = "";
                    }
                    return Json(new { Error = "false", ContenedorDotacionesResumen = Desplegable.ToString(), Pager = Pager.ToString(), TotalItems = totalItems }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    Desplegable = exc.Message.ToString() + "-----" + exc.StackTrace.ToString();
                    return Json(new { Error = "true", Message = Desplegable.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }
        /// <summary>
        /// METODO GUARDAR CONTRATO DOTACION
        /// </summary>
        /// <param name="DatosContrato"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GuardarContratoDotacion(Dotacion.ContratoDotacion DatosContrato)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string mensaje = String.Empty;
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(GI);
                    var resultado = IPlanificacion.GuardarContratoDotacion(DatosContrato);
                    return Json(new { Mensaje = resultado }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        /// <summary>
        /// METODO GUARDAR CONTRATO DOTACION
        /// </summary>
        /// <param name="DatosContrato"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ActualizarContratoDotacion(Dotacion.ModificarDotacion DatosContrato)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string mensaje = String.Empty;
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(GI);
                    var resultado = IPlanificacion.ActualizarContratoDotacion(DatosContrato);
                    return Json(new { Mensaje = resultado }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        /// <summary>
        /// METODO GUARDAR CONTRATO DOTACION
        /// </summary>
        /// <param name="DatosContrato"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ObtenerContratoDotacionByToken(string DotToken)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
                {
                    objConnection.Open();
                    try
                    {
                        var dotacion = SqlMapper.Query<Dotacion.ModificarDotacion>(objConnection, "CAPEX_SEL_DOTACION_BY_TOKEN", new { DotToken = DotToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                        return Json(new { Error = "false", Dotacion = dotacion }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception exc)
                    {
                        return Json(new { Error = "true", Message = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// METODO ELIMINAR DOTACION
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EliminarContratoDotacion(string token)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(ECD);
                    var resultado = IPlanificacion.EliminarContratoDotacion(token);
                    return Json(new { Mensaje = resultado }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        #endregion

        #region "METODOS DESCRIPCION DETALLADA"

        /// <remark>
        ///     
        ///     ----------------------------------------------------------------------------
        ///     GRUPO DE METODOS "Descripción Detallada"
        ///     VERSION     0.0.1
        ///     ----------------------------------------------------------------------------
        ///     PROPOSITO
        ///     ----------------------------------------------------------------------------
        ///     IMPLEMENTAR ENTRADA Y SALIDA DE DATOS, ASI COMO OPERACIONES POR MEDIO
        ///     DE PATRON CREACIONAL FACTORY QUE ES PROVISTO POR CAPA DDD DE INFRAESTRUCTURA
        ///     
        /// </remark>
        /// 

        /// <summary>
        /// METODO SUBIR ARCHIVO DESCRIPCION DETALLADA
        /// </summary>
        /// <returns></returns>
        public JsonResult SubirDescripcionDetallada()
        {
            string resultado = string.Empty;
            if (string.IsNullOrEmpty(Convert.ToString(HttpContext.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"])))
            {
                resultado = "ERROR";
                return Json(new { Resultado = resultado }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    string token = HttpContext.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"].ToString();
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase file = Request.Files[i];
                        int fileSize = file.ContentLength;
                        string fileName = file.FileName;
                        string mimeType = file.ContentType;

                        System.IO.Stream fileContent = file.InputStream;

                        string path = Server.MapPath("Scripts/Files/Iniciativas/Descripcion/" + token);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        var donde = Path.Combine(Server.MapPath("Scripts/Files/Iniciativas/Descripcion/" + token), fileName);
                        file.SaveAs(donde);
                        resultado = "OK";
                    }
                    return Json(new { Resultado = resultado }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        /// <summary>
        /// METDODO GUARDAR DATOS DE DESCRIPCION DETALLADA
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GuardarDescripcionDetallada(Descripcion.DescripcionDetallada Datos)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(GD);
                    var resultado = IPlanificacion.GuardarDescripcionDetallada(Datos);
                    if (resultado == "Guardado")
                    {
                        return Json(new { Mensaje = "Guardado" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// METODO DE ACTUALIZACION PARA DESCRIPCION DETALLADA
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ActualizarDescripcionDetallada(Descripcion.DescripcionDetallada Datos)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(GD);
                    var resultado = IPlanificacion.ActualizarDescripcionDetallada(Datos);
                    if (resultado == "Actualizado")
                    {
                        return Json(new { Mensaje = "Actualizado" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        #endregion

        #region "METODOS EVALUACION ECONOMICA"
        /// <remark>
        ///     
        ///     ----------------------------------------------------------------------------
        ///     GRUPO DE METODOS "Evaluación Económica"
        ///     VERSION     0.0.1
        ///     ----------------------------------------------------------------------------
        ///     PROPOSITO
        ///     ----------------------------------------------------------------------------
        ///     IMPLEMENTAR ENTRADA Y SALIDA DE DATOS, ASI COMO OPERACIONES POR MEDIO
        ///     DE PATRON CREACIONAL FACTORY QUE ES PROVISTO POR CAPA DDD DE INFRAESTRUCTURA
        ///     
        /// </remark>
        /// 

        public JsonResult SubirEvaluacionEconomica()
        {
            string resultado = string.Empty;
            if (string.IsNullOrEmpty(Convert.ToString(HttpContext.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"])))
            {
                resultado = "ERROR";
                return Json(new { Resultado = resultado }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    string token = HttpContext.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"].ToString();
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase file = Request.Files[i];
                        int fileSize = file.ContentLength;
                        string fileName = file.FileName;
                        string mimeType = file.ContentType;

                        System.IO.Stream fileContent = file.InputStream;

                        string path = Server.MapPath("Scripts/Files/Iniciativas/EvaluacionEconomica/" + token);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        var donde = Path.Combine(Server.MapPath("Scripts/Files/Iniciativas/EvaluacionEconomica/" + token), fileName);
                        file.SaveAs(donde);
                        resultado = "OK";
                    }
                    return Json(new { Resultado = resultado }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        /// <summary>
        /// METODO GUARDAR EVALUACION ECONOMICA
        /// </summary>
        /// <param name="DatosEvaluacion"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GuardarEvaluacionEconomica(EvaluacionEconomica.GuardarEvaluacion DatosEvaluacion)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(GE);
                    string resultado = IPlanificacion.GuardarEvaluacionEconomica(DatosEvaluacion);
                    if (resultado == "Guardado")
                    {
                        return Json(new { Mensaje = "Guardado" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }

                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = exc.Message.ToString() + "-----" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// METODO DE ACTUALIZACION DE DESCRIPCION DETALLADA
        /// </summary>
        /// <param name="DatosEvaluacion"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ActualizarEvaluacionEconomica(EvaluacionEconomica.GuardarEvaluacion DatosEvaluacion)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(GE);
                    string resultado = IPlanificacion.ActualizarEvaluacionEconomica(DatosEvaluacion);
                    if (resultado == "Actualizado")
                    {
                        return Json(new { Mensaje = "Actualizado" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }

                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = exc.Message.ToString() + "-----" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        #endregion

        #region "METODOS EVALUACION RIESGO"
        /// <remark>
        ///     
        ///     ----------------------------------------------------------------------------
        ///     GRUPO DE METODOS "Evaluación Riesgo"
        ///     VERSION     0.0.1
        ///     ----------------------------------------------------------------------------
        ///     PROPOSITO
        ///     ----------------------------------------------------------------------------
        ///     IMPLEMENTAR ENTRADA Y SALIDA DE DATOS, ASI COMO OPERACIONES POR MEDIO
        ///     DE PATRON CREACIONAL FACTORY QUE ES PROVISTO POR CAPA DDD DE INFRAESTRUCTURA
        ///     
        /// </remark>
        /// 
        public JsonResult SubirEvaluacionRiesgo()
        {
            string resultado = string.Empty;
            if (string.IsNullOrEmpty(Convert.ToString(HttpContext.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"])))
            {
                resultado = "ERROR";
                return Json(new { Resultado = resultado }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    string token = HttpContext.Session["_SESS_CAPEX_INCIATIVA_TOKEN_"].ToString();
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase file = Request.Files[i];
                        int fileSize = file.ContentLength;
                        string fileName = file.FileName;
                        string mimeType = file.ContentType;

                        System.IO.Stream fileContent = file.InputStream;

                        //string path = Server.MapPath("Scripts/Files/Iniciativas/EvaluacionRiesgo/" + token);
                        string path = Server.MapPath("../Files/Iniciativas/EvaluacionRiesgo/" + token);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        //var donde = Path.Combine(Server.MapPath("Scripts/Files/Iniciativas/EvaluacionRiesgo/" + token), fileName);
                        var donde = Path.Combine(Server.MapPath("../Files/Iniciativas/EvaluacionRiesgo/" + token), fileName);
                        file.SaveAs(donde);
                        resultado = "OK";
                    }
                    return Json(new { Resultado = resultado }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = exc.Message.ToString() + "-----" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        /// <summary>
        /// METODO GUARDAR DATOS DE EVALUACION DE RIESGO
        /// </summary>
        /// <param name="DatosEvaluacion"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GuardarEvaluacionRiesgo(EvaluacionRiesgo.GuardarEvaluacion DatosEvaluacion)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(GE);
                    string resultado = IPlanificacion.GuardarEvaluacionRiesgo(DatosEvaluacion);
                    if (resultado == "Guardado")
                    {
                        return Json(new { Mensaje = "Guardado" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = exc.Message.ToString() + "-----" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// METODO DE ACTUALIZACION DE EVALUACION DE RIESGO
        /// </summary>
        /// <param name="DatosEvaluacion"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ActualizarEvaluacionRiesgo(EvaluacionRiesgo.GuardarEvaluacion DatosEvaluacion)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(GE);
                    string resultado = IPlanificacion.ActualizarEvaluacionRiesgo(DatosEvaluacion);
                    if (resultado == "Actualizado")
                    {
                        return Json(new { Mensaje = "Actualizado" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = exc.Message.ToString() + "-----" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        #endregion

        #region "METODOS HITOS"
        /// <remark>
        ///     
        ///     ----------------------------------------------------------------------------
        ///     GRUPO DE METODOS "Hitos - Capex"
        ///     VERSION     0.0.1
        ///     ----------------------------------------------------------------------------
        ///     PROPOSITO
        ///     ----------------------------------------------------------------------------
        ///     IMPLEMENTAR ENTRADA Y SALIDA DE DATOS, ASI COMO OPERACIONES POR MEDIO
        ///     DE PATRON CREACIONAL FACTORY QUE ES PROVISTO POR CAPA DDD DE INFRAESTRUCTURA
        ///     
        /// </remark>
        ///

        /// <summary>
        /// METODO GUARDAR CAPEX HITOS
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GuardarHito(Hito.HitoGuardar Datos)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(GH);
                    string resultado = IPlanificacion.GuardarHito(Datos);
                    if (resultado == "Guardado")
                    {
                        return Json(new { Mensaje = "Guardado" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }

                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = exc.Message.ToString() + "-----" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// METODO DE ACTUALIZACION DE HITO
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ActualizarHito(Hito.HitoGuardar Datos)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                // return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(GH);
                    string resultado = IPlanificacion.ActualizarHito(Datos);
                    if (resultado == "Actualizado")
                    {
                        return Json(new { Mensaje = "Actualizado" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }

                }
                catch (Exception exc)
                {
                    return null;
                    //return Json(new { Resultado = exc.Message.ToString() + "-----" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// METODO POBLAR HITOS
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public string PoblarVistaHitos(string token)
        {
            try
            {
                IPlanificacion = FactoryPlanificacion.delega(PH);
                return IPlanificacion.PoblarVistaHitos(token);
            }
            catch (Exception exc)
            {
                return null;
                //return exc.Message.ToString() + "-----" + exc.StackTrace.ToString();
            }
            finally
            {
                FactoryPlanificacion = null;
                IPlanificacion = null;
            }
        }
        /// <summary>
        /// METODO PARA POBLAR VISTA CAPEX HITOS - COMPONENTE GENERAL
        /// </summary>
        /// <returns></returns>                  
        [HttpGet]
        public ActionResult PoblarVistaHitosGeneral(string token)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(PVHG);
                    string JsonResponse = JsonConvert.SerializeObject(IPlanificacion.PoblarVistaHitosDetalle(token), Formatting.None);
                    return Json(
                    JsonResponse,
                    JsonRequestBehavior.AllowGet
                    );
                }
                catch (Exception exc)
                {
                    return null;
                    //return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// METODO PARA POBLAR VISTA CAPEX HITOS - COMPONENTE RESUMEN
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PoblarVistaHitosResumen(string token)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(PVHR);
                    string JsonResponse = JsonConvert.SerializeObject(IPlanificacion.PoblarVistaHitosResumen(token), Formatting.None);
                    return Json(
                    JsonResponse,
                    JsonRequestBehavior.AllowGet
                    );
                }
                catch (Exception exc)
                {
                    return null;
                    //return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EnviarIniciativa(string IniToken, string WrfUsuario, string WrfObservacion, string tipoIniciativaSeleccionado)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    string iniPeriodo = iniPeriodoIniciativa(IniToken);
                    if (!string.IsNullOrEmpty(tipoIniciativaSeleccionado))
                    {
                        Session["tipoIniciativaSeleccionado"] = tipoIniciativaSeleccionado;
                        if (!string.IsNullOrEmpty(iniPeriodo))
                        {
                            Session["anioIniciativaSeleccionado"] = iniPeriodo;
                        }
                    }
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    IPlanificacion = FactoryPlanificacion.delega(EI);
                    var mensaje = IPlanificacion.EnviarIniciativa(IniToken, WrfUsuario, WrfObservacion, rol);
                    return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    return Json(new { Mensaje = exc.Message.ToString() + " " + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }
        /// <summary>
        /// METODO GENERADOR PDF
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ActionResult PdfCasoBase(string token)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
                {
                    objConnection.Open();
                    try
                    {
                        var Cuerpo = SqlMapper.Query(objConnection, "CAPEX_SEL_PDF_CASO_BASE", new { @IniToken = token }, commandType: CommandType.StoredProcedure).ToList();
                        foreach (var c in Cuerpo)
                        {
                            ViewBag.IniPeriodo = c.IniPeriodo;
                            ViewBag.IrFecha = c.IrFecha;
                        }

                        ViewBag.Identificacion = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_IDENTIFICACION_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                        ViewBag.Categorizacion = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_CATEGORIZACION_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                        ViewBag.DescripcionDetallada = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_DESCRIPCIONDETALLADA_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                        ViewBag.EvaluacionEconomica = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_EVALUACIONECONOMICA_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                        ViewBag.EvaluacionRiesgo = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_EVALUACIONRIESGO_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                        ViewBag.Hito = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_HITO_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                        ViewBag.ResumenFinanciero = SqlMapper.Query(objConnection, "CAPEX_SEL_RESUMEN_FINANCIERO", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();

                    }
                    catch (Exception err)
                    {
                        ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        Utils.LogError(ExceptionResult);
                        return null;
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
                return new ViewAsPdf("PdfCasoBase")
                {
                    FileName = "PdfCasoBase.pdf",
                    PageSize = Size.A4,
                    PageOrientation = Orientation.Portrait,
                    PageMargins = { Left = 0, Right = 0 }
                };
            }
        }
        /// <summary>
        /// METODO GENERADOR PDF
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ActionResult PdfPresupuesto(string token)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
                {
                    objConnection.Open();
                    try
                    {
                        var Cuerpo = SqlMapper.Query(objConnection, "CAPEX_SEL_PDF_CASO_BASE", new { @IniToken = token }, commandType: CommandType.StoredProcedure).ToList();
                        foreach (var c in Cuerpo)
                        {
                            ViewBag.IniPeriodo = c.IniPeriodo;
                            ViewBag.IrFecha = c.IrFecha;
                        }

                        ViewBag.Identificacion = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_IDENTIFICACION_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                        ViewBag.Categorizacion = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_CATEGORIZACION_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                        ViewBag.DescripcionDetallada = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_DESCRIPCIONDETALLADA_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                        ViewBag.EvaluacionEconomica = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_EVALUACIONECONOMICA_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                        ViewBag.EvaluacionRiesgo = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_EVALUACIONRIESGO_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                        ViewBag.Hito = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_HITO_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                        ViewBag.ResumenFinanciero = SqlMapper.Query(objConnection, "CAPEX_SEL_RESUMEN_FINANCIERO", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();

                    }
                    catch (Exception err)
                    {
                        ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        Utils.LogError(ExceptionResult);
                        return null;
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
                return new ViewAsPdf("PdfPresupuesto")
                {
                    FileName = "PdfCasoBase.pdf",
                    PageSize = Size.A4,
                    PageOrientation = Orientation.Portrait,
                    PageMargins = { Left = 0, Right = 0 }
                };
            }
        }

        /// <summary>
        /// METODO GENERADOR PDF
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ActionResult descargaPdfPresupuesto(string token)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var nombreArchivo = "DESCONOCIDO";
                using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
                {
                    objConnection.Open();
                    try
                    {
                        DataTable dt = new DataTable();
                        DateTime todaysDate = DateTime.Now.Date;
                        int year = todaysDate.Year;

                        string tipoIniciativaEjercicioOficial = ((Session["tipoIniciativaEjercicioOficial"] != null) ? Convert.ToString(Session["tipoIniciativaEjercicioOficial"]) : "");
                        string anioIniciativaEjercicioOficial = ((Session["anioIniciativaEjercicioOficial"] != null) ? Convert.ToString(Session["anioIniciativaEjercicioOficial"]) : "");
                        bool esEjercicioOficial = ((!string.IsNullOrEmpty(tipoIniciativaEjercicioOficial) && !string.IsNullOrEmpty(anioIniciativaEjercicioOficial)) ? true : false);

                        string tipoIniciativaOrientacionComercial = ((Session["tipoIniciativaOrientacionComercial"] != null) ? Convert.ToString(Session["tipoIniciativaOrientacionComercial"]) : "");
                        string anioIniciativaOrientacionComercial = ((Session["anioIniciativaOrientacionComercial"] != null) ? Convert.ToString(Session["anioIniciativaOrientacionComercial"]) : "");
                        string parametroVNToken = ((Session["ParametroVNToken"] != null) ? Convert.ToString(Session["ParametroVNToken"]) : "");
                        bool esParametroVNToken = ((!string.IsNullOrEmpty(tipoIniciativaOrientacionComercial) && !string.IsNullOrEmpty(anioIniciativaOrientacionComercial) && !string.IsNullOrEmpty(parametroVNToken)) ? true : false);

                        string procedimientoAmacenado = "CAPEX_SEL_REPORTE_INICIATIVA_PDF_PRESUPUESTO";
                        var parametros = new DynamicParameters();
                        parametros.Add("IniToken", token);
                        if (esEjercicioOficial)
                        {
                            parametros.Add("TipoIniciativa", tipoIniciativaEjercicioOficial);
                            parametros.Add("Periodo", anioIniciativaEjercicioOficial);
                            procedimientoAmacenado = "CAPEX_SEL_REPORTE_INICIATIVA_PDF_PRESUPUESTO_EJERCICIO_OFICIAL";
                        }
                        else if (esParametroVNToken)
                        {
                            var parametosOrigen = new DynamicParameters();
                            parametosOrigen.Add("ParametroVNToken", parametroVNToken);
                            parametosOrigen.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 1);
                            SqlMapper.Query(objConnection, "CAPEX_PARAMETRO_IS_V0", parametosOrigen, commandType: CommandType.StoredProcedure).SingleOrDefault();
                            string respuestaOrigen = parametosOrigen.Get<string>("Respuesta");
                            if (respuestaOrigen == null || string.IsNullOrEmpty(respuestaOrigen.Trim()) || "0".Equals(respuestaOrigen.Trim()))
                            {
                                parametros.Add("TipoIniciativa", tipoIniciativaOrientacionComercial);
                                parametros.Add("Periodo", anioIniciativaOrientacionComercial);
                                parametros.Add("ParametroVNToken", parametroVNToken);
                                procedimientoAmacenado = "CAPEX_SEL_REPORTE_INICIATIVA_PDF_PRESUPUESTO_PARAMETROVN";
                            }
                        }
                        var IniciativaPdfPresupuesto = SqlMapper.Query(objConnection, procedimientoAmacenado, parametros, commandType: CommandType.StoredProcedure).ToList();
                        string ENUS = ObtenerParametroSistema("en-US");
                        foreach (var ini in IniciativaPdfPresupuesto)
                        {
                            ViewBag.Anio = ini.IniPeriodo;
                            ViewBag.IniTipo = ini.IniTipo;
                            ViewBag.anioIni = ViewBag.Anio;
                            if (ViewBag.IniTipo != null && !string.IsNullOrEmpty(ViewBag.IniTipo.ToString()) && ("CB".Equals(ViewBag.IniTipo.ToString()) || "CD".Equals(ViewBag.IniTipo.ToString())))
                            {
                                ViewBag.anioIni += 1;
                            }
                            ViewBag.AnioUno = ViewBag.anioIni + 1;
                            ViewBag.AnioDos = ViewBag.anioIni + 2;
                            ViewBag.NombreProyecto = ini.PidNombreProyecto;
                            ViewBag.NombreProyectoAlias = ini.PidNombreProyectoAlias;
                            ViewBag.CodigoIniciativa = ini.PidCodigoIniciativa;
                            ViewBag.TipoEjercicio = ini.IniTipoEjercicio;
                            ViewBag.Estado = ini.CatEstadoProyecto;
                            ViewBag.MacroCategoria = ini.CatMacroCategoria;
                            ViewBag.Categoria = ini.CatCategoria;
                            ViewBag.GerenciaInversion = ini.PidGerenciaInversion;
                            ViewBag.GerenteInversion = ini.PidGerenteInversion;
                            ViewBag.GerenciaEjecucion = ini.PidGerenciaEjecucion;
                            ViewBag.GerenteEjecucion = ini.PidGerenteEjecucion;
                            ViewBag.Superintendencia = ini.PidSuperintendencia;
                            ViewBag.EncargadoControl = ini.PidEncargadoControl;
                            ViewBag.NivelIngenieria = ini.CatNivelIngenieria;
                            ViewBag.ClasificacionSSO = ini.CatClasificacionSSO;
                            ViewBag.EstandarSeguridad = ini.CatEstandarSeguridad;
                            ViewBag.Clase = ini.CatClase;
                            ViewBag.PddObjetivo = ini.PddObjetivo;
                            ViewBag.PddJustificacion = ini.PddJustificacion;
                            ViewBag.PddAlcance = ini.PddAlcance;

                            ViewBag.KP1Descripcion1 = ini.KP1Descripcion1;
                            ViewBag.KP1Unidad1 = ini.KP1Unidad1;
                            ViewBag.KP1Actual1 = ini.KP1Actual1;
                            ViewBag.KP1Target1 = ini.KP1Target1;
                            ViewBag.KP1Descripcion2 = ini.KP1Descripcion2;
                            ViewBag.KP1Unidad2 = ini.KP1Unidad2;
                            ViewBag.KP1Actual2 = ini.KP1Actual2;
                            ViewBag.KP1Target2 = ini.KP1Target2;
                            ViewBag.KP1Descripcion3 = ini.KP1Descripcion3;
                            ViewBag.KP1Unidad3 = ini.KP1Unidad3;
                            ViewBag.KP1Actual3 = ini.KP1Actual3;
                            ViewBag.KP1Target3 = ini.KP1Target3;

                            ViewBag.HCI = ini.HCI;
                            ViewBag.HCA = ini.HCA;
                            ViewBag.HOPR = ini.HOPR;
                            ViewBag.HVPF = ini.HVPF;
                            ViewBag.HPE = ini.HPE;
                            ViewBag.HDIRPLC = ini.HDIRPLC;
                            ViewBag.HDIRMCEN = ini.HDIRMCEN;
                            ViewBag.HINICIO = ini.HINICIO;
                            ViewBag.HTERMINO = ini.HTERMINO;
                            ViewBag.HCIERRE = ini.HCIERRE;
                            ViewBag.HPOSTEVAL = ini.HPOSTEVAL;

                            ViewBag.EriMFL1 = ((string.IsNullOrEmpty(ini.EriMFL1)) ? "0" : formatNumberPdf(ini.EriMFL1));
                            ViewBag.EriMFL2 = ((string.IsNullOrEmpty(ini.EriMFL2)) ? "0" : formatNumberPdf(ini.EriMFL2));

                            ViewBag.RiesgoNombre = (string.IsNullOrEmpty(ini.RiesgoNombre) ? "" : (ini.RiesgoNombre.Length > 42 ? (ini.RiesgoNombre.ToString().Substring(0, 39) + "...") : ini.RiesgoNombre));
                            ViewBag.RiesgoImpacto = ((ini.RiesgoImpacto != null && ini.RiesgoImpacto != 0) ? ini.RiesgoImpacto.ToString() : "");
                            ViewBag.RiesgoProbabilidad = ((ini.RiesgoProbabilidad != null && ini.RiesgoProbabilidad != 0) ? ini.RiesgoProbabilidad.ToString() : "");

                            ViewBag.Van = ((string.IsNullOrEmpty(ini.EveVan)) ? "0" : formatNumberPdf(ini.EveVan));
                            ViewBag.Ivan = ((string.IsNullOrEmpty(ini.EveIvan)) ? "0" : formatNumberPdf(ini.EveIvan));
                            ViewBag.PayBack = ((string.IsNullOrEmpty(ini.EvePayBack)) ? "0" : formatNumberPdf(ini.EvePayBack));
                            ViewBag.VidaUtil = ((string.IsNullOrEmpty(ini.EveVidaUtil)) ? "0" : formatNumberPdf(ini.EveVidaUtil));
                            ViewBag.Tir = ((string.IsNullOrEmpty(ini.EveTir)) ? "0" : double.Parse(varEvaluacionEconomicaPdf(ini.EveTir), CultureInfo.InvariantCulture).ToString());

                            if (ENUS != null && !string.IsNullOrEmpty(ENUS.ToString()))
                            {
                                ViewBag.FaseIngenieria = ((ini.TotalCapexIng == null || string.IsNullOrEmpty(ini.TotalCapexIng)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalCapexIng)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.FaseAdquisicion = ((ini.TotalCapexAdq == null || string.IsNullOrEmpty(ini.TotalCapexAdq)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalCapexAdq)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.FaseConstruccion = ((ini.TotalCapexConst == null || string.IsNullOrEmpty(ini.TotalCapexConst)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalCapexConst)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.FaseAdministracion = ((ini.TotalCapexAdm == null || string.IsNullOrEmpty(ini.TotalCapexAdm)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalCapexAdm)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.TotalContingencia = ((ini.TotalCapexCont == null || string.IsNullOrEmpty(ini.TotalCapexCont)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalCapexCont)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.CostoDueno = ((ini.PorCostoDueno == null || string.IsNullOrEmpty(ini.PorCostoDueno)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.PorCostoDueno)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.Contingencia = ((ini.PorContingencia == null || string.IsNullOrEmpty(ini.PorContingencia)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.PorContingencia)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));

                                ViewBag.TotalCapexFisico = ((ini.TotalCapexFisico == null || string.IsNullOrEmpty(ini.TotalCapexFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalCapexFisico)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.EneroFisico = ((ini.EneroFisico == null || string.IsNullOrEmpty(ini.EneroFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.EneroFisico)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.FebreroFisico = ((ini.FebreroFisico == null || string.IsNullOrEmpty(ini.FebreroFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.FebreroFisico)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.MarzoFisico = ((ini.MarzoFisico == null || string.IsNullOrEmpty(ini.MarzoFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.MarzoFisico)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.AbrilFisico = ((ini.AbrilFisico == null || string.IsNullOrEmpty(ini.AbrilFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.AbrilFisico)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.MayoFisico = ((ini.MayoFisico == null || string.IsNullOrEmpty(ini.MayoFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.MayoFisico)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.JunioFisico = ((ini.JunioFisico == null || string.IsNullOrEmpty(ini.JunioFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.JunioFisico)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.JulioFisico = ((ini.JulioFisico == null || string.IsNullOrEmpty(ini.JulioFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.JulioFisico)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.AgostoFisico = ((ini.AgostoFisico == null || string.IsNullOrEmpty(ini.AgostoFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.AgostoFisico)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.SeptiembreFisico = ((ini.SeptiembreFisico == null || string.IsNullOrEmpty(ini.SeptiembreFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.SeptiembreFisico)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.OctubreFisico = ((ini.OctubreFisico == null || string.IsNullOrEmpty(ini.OctubreFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.OctubreFisico)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.NoviembreFisico = ((ini.NoviembreFisico == null || string.IsNullOrEmpty(ini.NoviembreFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.NoviembreFisico)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.DiciembreFisico = ((ini.DiciembreFisico == null || string.IsNullOrEmpty(ini.DiciembreFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.DiciembreFisico)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.TotalFisico = ((ini.TotalFisico == null || string.IsNullOrEmpty(ini.TotalFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalFisico)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.PresAnioMasUnoFisico = ((ini.PresAnioMasUnoFisico == null || string.IsNullOrEmpty(ini.PresAnioMasUnoFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.PresAnioMasUnoFisico)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.PresAnioMasDosFisico = ((ini.PresAnioMasDosFisico == null || string.IsNullOrEmpty(ini.PresAnioMasDosFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.PresAnioMasDosFisico)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.PresAnioMasTresFisico = ((ini.PresAnioMasTresFisico == null || string.IsNullOrEmpty(ini.PresAnioMasTresFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.PresAnioMasTresFisico)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.TotalAcumFisico = ((ini.TotalAcumFisico == null || string.IsNullOrEmpty(ini.TotalAcumFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalAcumFisico)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));

                                ViewBag.TotalCapexTotPar = ((ini.TotalCapexTotPar == null || string.IsNullOrEmpty(ini.TotalCapexTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalCapexTotPar)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.EneroTotPar = ((ini.EneroTotPar == null || string.IsNullOrEmpty(ini.EneroTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.EneroTotPar)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.FebreroTotPar = ((ini.FebreroTotPar == null || string.IsNullOrEmpty(ini.FebreroTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.FebreroTotPar)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.MarzoTotPar = ((ini.MarzoTotPar == null || string.IsNullOrEmpty(ini.MarzoTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.MarzoTotPar)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.AbrilTotPar = ((ini.AbrilTotPar == null || string.IsNullOrEmpty(ini.AbrilTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.AbrilTotPar)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.MayoTotPar = ((ini.MayoTotPar == null || string.IsNullOrEmpty(ini.MayoTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.MayoTotPar)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.JunioTotPar = ((ini.JunioTotPar == null || string.IsNullOrEmpty(ini.JunioTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.JunioTotPar)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.JulioTotPar = ((ini.JulioTotPar == null || string.IsNullOrEmpty(ini.JulioTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.JulioTotPar)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.AgostoTotPar = ((ini.AgostoTotPar == null || string.IsNullOrEmpty(ini.AgostoTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.AgostoTotPar)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.SeptiembreTotPar = ((ini.SeptiembreTotPar == null || string.IsNullOrEmpty(ini.SeptiembreTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.SeptiembreTotPar)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.OctubreTotPar = ((ini.OctubreTotPar == null || string.IsNullOrEmpty(ini.OctubreTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.OctubreTotPar)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.NoviembreTotPar = ((ini.NoviembreTotPar == null || string.IsNullOrEmpty(ini.NoviembreTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.NoviembreTotPar)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.DiciembreTotPar = ((ini.DiciembreTotPar == null || string.IsNullOrEmpty(ini.DiciembreTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.DiciembreTotPar)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.TotalTotPar = ((ini.TotalTotPar == null || string.IsNullOrEmpty(ini.TotalTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalTotPar)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.PresAnioMasUnoTotPar = ((ini.PresAnioMasUnoTotPar == null || string.IsNullOrEmpty(ini.PresAnioMasUnoTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.PresAnioMasUnoTotPar)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.PresAnioMasDosTotPar = ((ini.PresAnioMasDosTotPar == null || string.IsNullOrEmpty(ini.PresAnioMasDosTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.PresAnioMasDosTotPar)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.PresAnioMasTresTotPar = ((ini.PresAnioMasTresTotPar == null || string.IsNullOrEmpty(ini.PresAnioMasTresTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.PresAnioMasTresTotPar)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.TotalAcumTotPar = ((ini.TotalAcumTOTPAR == null || string.IsNullOrEmpty(ini.TotalAcumTOTPAR)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalAcumTOTPAR)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                            }
                            else
                            {
                                ViewBag.FaseIngenieria = ((ini.TotalCapexIng == null || string.IsNullOrEmpty(ini.TotalCapexIng)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalCapexIng)).ToString());
                                ViewBag.FaseAdquisicion = ((ini.TotalCapexAdq == null || string.IsNullOrEmpty(ini.TotalCapexAdq)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalCapexAdq)).ToString());
                                ViewBag.FaseConstruccion = ((ini.TotalCapexConst == null || string.IsNullOrEmpty(ini.TotalCapexConst)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalCapexConst)).ToString());
                                ViewBag.FaseAdministracion = ((ini.TotalCapexAdm == null || string.IsNullOrEmpty(ini.TotalCapexAdm)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalCapexAdm)).ToString());
                                ViewBag.TotalContingencia = ((ini.TotalCapexCont == null || string.IsNullOrEmpty(ini.TotalCapexCont)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalCapexCont)).ToString());
                                ViewBag.CostoDueno = ((ini.PorCostoDueno == null || string.IsNullOrEmpty(ini.PorCostoDueno)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.PorCostoDueno)).ToString());
                                ViewBag.Contingencia = ((ini.PorContingencia == null || string.IsNullOrEmpty(ini.PorContingencia)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.PorContingencia)).ToString());

                                ViewBag.TotalCapexFisico = ((ini.TotalCapexFisico == null || string.IsNullOrEmpty(ini.TotalCapexFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalCapexFisico)).ToString());
                                ViewBag.EneroFisico = ((ini.EneroFisico == null || string.IsNullOrEmpty(ini.EneroFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.EneroFisico)).ToString());
                                ViewBag.FebreroFisico = ((ini.FebreroFisico == null || string.IsNullOrEmpty(ini.FebreroFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.FebreroFisico)).ToString());
                                ViewBag.MarzoFisico = ((ini.MarzoFisico == null || string.IsNullOrEmpty(ini.MarzoFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.MarzoFisico)).ToString());
                                ViewBag.AbrilFisico = ((ini.AbrilFisico == null || string.IsNullOrEmpty(ini.AbrilFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.AbrilFisico)).ToString());
                                ViewBag.MayoFisico = ((ini.MayoFisico == null || string.IsNullOrEmpty(ini.MayoFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.MayoFisico)).ToString());
                                ViewBag.JunioFisico = ((ini.JunioFisico == null || string.IsNullOrEmpty(ini.JunioFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.JunioFisico)).ToString());
                                ViewBag.JulioFisico = ((ini.JulioFisico == null || string.IsNullOrEmpty(ini.JulioFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.JulioFisico)).ToString());
                                ViewBag.AgostoFisico = ((ini.AgostoFisico == null || string.IsNullOrEmpty(ini.AgostoFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.AgostoFisico)).ToString());
                                ViewBag.SeptiembreFisico = ((ini.SeptiembreFisico == null || string.IsNullOrEmpty(ini.SeptiembreFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.SeptiembreFisico)).ToString());
                                ViewBag.OctubreFisico = ((ini.OctubreFisico == null || string.IsNullOrEmpty(ini.OctubreFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.OctubreFisico)).ToString());
                                ViewBag.NoviembreFisico = ((ini.NoviembreFisico == null || string.IsNullOrEmpty(ini.NoviembreFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.NoviembreFisico)).ToString());
                                ViewBag.DiciembreFisico = ((ini.DiciembreFisico == null || string.IsNullOrEmpty(ini.DiciembreFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.DiciembreFisico)).ToString());
                                ViewBag.TotalFisico = ((ini.TotalFisico == null || string.IsNullOrEmpty(ini.TotalFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalFisico)).ToString());
                                ViewBag.PresAnioMasUnoFisico = ((ini.PresAnioMasUnoFisico == null || string.IsNullOrEmpty(ini.PresAnioMasUnoFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.PresAnioMasUnoFisico)).ToString());
                                ViewBag.PresAnioMasDosFisico = ((ini.PresAnioMasDosFisico == null || string.IsNullOrEmpty(ini.PresAnioMasDosFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.PresAnioMasDosFisico)).ToString());
                                ViewBag.PresAnioMasTresFisico = ((ini.PresAnioMasTresFisico == null || string.IsNullOrEmpty(ini.PresAnioMasTresFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.PresAnioMasTresFisico)).ToString());
                                ViewBag.TotalAcumFisico = ((ini.TotalAcumFisico == null || string.IsNullOrEmpty(ini.TotalAcumFisico)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalAcumFisico)).ToString());

                                ViewBag.TotalCapexTotPar = ((ini.TotalCapexTotPar == null || string.IsNullOrEmpty(ini.TotalCapexTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalCapexTotPar)).ToString());
                                ViewBag.EneroTotPar = ((ini.EneroTotPar == null || string.IsNullOrEmpty(ini.EneroTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.EneroTotPar)).ToString());
                                ViewBag.FebreroTotPar = ((ini.FebreroTotPar == null || string.IsNullOrEmpty(ini.FebreroTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.FebreroTotPar)).ToString());
                                ViewBag.MarzoTotPar = ((ini.MarzoTotPar == null || string.IsNullOrEmpty(ini.MarzoTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.MarzoTotPar)).ToString());
                                ViewBag.AbrilTotPar = ((ini.AbrilTotPar == null || string.IsNullOrEmpty(ini.AbrilTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.AbrilTotPar)).ToString());
                                ViewBag.MayoTotPar = ((ini.MayoTotPar == null || string.IsNullOrEmpty(ini.MayoTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.MayoTotPar)).ToString());
                                ViewBag.JunioTotPar = ((ini.JunioTotPar == null || string.IsNullOrEmpty(ini.JunioTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.JunioTotPar)).ToString());
                                ViewBag.JulioTotPar = ((ini.JulioTotPar == null || string.IsNullOrEmpty(ini.JulioTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.JulioTotPar)).ToString());
                                ViewBag.AgostoTotPar = ((ini.AgostoTotPar == null || string.IsNullOrEmpty(ini.AgostoTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.AgostoTotPar)).ToString());
                                ViewBag.SeptiembreTotPar = ((ini.SeptiembreTotPar == null || string.IsNullOrEmpty(ini.SeptiembreTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.SeptiembreTotPar)).ToString());
                                ViewBag.OctubreTotPar = ((ini.OctubreTotPar == null || string.IsNullOrEmpty(ini.OctubreTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.OctubreTotPar)).ToString());
                                ViewBag.NoviembreTotPar = ((ini.NoviembreTotPar == null || string.IsNullOrEmpty(ini.NoviembreTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.NoviembreTotPar)).ToString());
                                ViewBag.DiciembreTotPar = ((ini.DiciembreTotPar == null || string.IsNullOrEmpty(ini.DiciembreTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.DiciembreTotPar)).ToString());
                                ViewBag.TotalTotPar = ((ini.TotalTotPar == null || string.IsNullOrEmpty(ini.TotalTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalTotPar)).ToString());
                                ViewBag.PresAnioMasUnoTotPar = ((ini.PresAnioMasUnoTotPar == null || string.IsNullOrEmpty(ini.PresAnioMasUnoTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.PresAnioMasUnoTotPar)).ToString());
                                ViewBag.PresAnioMasDosTotPar = ((ini.PresAnioMasDosTotPar == null || string.IsNullOrEmpty(ini.PresAnioMasDosTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.PresAnioMasDosTotPar)).ToString());
                                ViewBag.PresAnioMasTresTotPar = ((ini.PresAnioMasTresTotPar == null || string.IsNullOrEmpty(ini.PresAnioMasTresTotPar)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.PresAnioMasTresTotPar)).ToString());
                                ViewBag.TotalAcumTotPar = ((ini.TotalAcumTOTPAR == null || string.IsNullOrEmpty(ini.TotalAcumTOTPAR)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalAcumTOTPAR)).ToString());
                            }

                            ViewBag.TotalCapexTOTACUM = ini.TotalCapexTOTACUM;
                            ViewBag.TotalAcumTOTACUM = ini.TotalAcumTOTACUM;
                            ViewBag.EneroTotAcum = ini.EneroTotAcum;
                            ViewBag.FebreroTotAcum = ini.FebreroTotAcum;
                            ViewBag.MarzoTotAcum = ini.MarzoTotAcum;
                            ViewBag.AbrilTotAcum = ini.AbrilTotAcum;
                            ViewBag.MayoTotAcum = ini.MayoTotAcum;
                            ViewBag.JunioTotAcum = ini.JunioTotAcum;
                            ViewBag.JulioTotAcum = ini.JulioTotAcum;
                            ViewBag.AgostoTotAcum = ini.AgostoTotAcum;
                            ViewBag.SeptiembreTotAcum = ini.SeptiembreTotAcum;
                            ViewBag.OctubreTotAcum = ini.OctubreTotAcum;
                            ViewBag.NoviembreTotAcum = ini.NoviembreTotAcum;
                            ViewBag.DiciembreTotAcum = ini.DiciembreTotAcum;
                            ViewBag.TotalTotAcum = ini.TotalTotAcum;
                            ViewBag.PresAnioMasUnoTotAcum = ini.PresAnioMasUnoTotAcum;
                            ViewBag.PresAnioMasDosTotAcum = ini.PresAnioMasDosTotAcum;
                            ViewBag.PresAnioMasTresTotAcum = ini.PresAnioMasTresTotAcum;

                            ViewBag.TotalCapexFinanciero = ini.TotalCapexFinanciero;
                            ViewBag.EneroFinanciero = ini.EneroFinanciero;
                            ViewBag.FebreroFinanciero = ini.FebreroFinanciero;
                            ViewBag.MarzoFinanciero = ini.MarzoFinanciero;
                            ViewBag.AbrilFinanciero = ini.AbrilFinanciero;
                            ViewBag.MayoFinanciero = ini.MayoFinanciero;
                            ViewBag.JunioFinanciero = ini.JunioFinanciero;
                            ViewBag.JulioFinanciero = ini.JulioFinanciero;
                            ViewBag.AgostoFinanciero = ini.AgostoFinanciero;
                            ViewBag.SeptiembreFinanciero = ini.SeptiembreFinanciero;
                            ViewBag.OctubreFinanciero = ini.OctubreFinanciero;
                            ViewBag.NoviembreFinanciero = ini.NoviembreFinanciero;
                            ViewBag.DiciembreFinanciero = ini.DiciembreFinanciero;
                            ViewBag.TotalFinanciero = ini.TotalFinanciero;
                            ViewBag.PresAnioMasUnoFinanciero = ini.PresAnioMasUnoFinanciero;
                            ViewBag.PresAnioMasDosFinanciero = ini.PresAnioMasDosFinanciero;
                            ViewBag.PresAnioMasTresFinanciero = ini.PresAnioMasTresFinanciero;
                            ViewBag.TotalAcumFinanciero = ini.TotalAcumFinanciero;

                            if (ENUS != null && !string.IsNullOrEmpty(ENUS.ToString()))
                            {
                                ViewBag.TotalCapexDotacion = ((ini.TotalCapexDotacion == null || string.IsNullOrEmpty(ini.TotalCapexDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalCapexDotacion)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.EneroDotacion = ((ini.EneroDotacion == null || string.IsNullOrEmpty(ini.EneroDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.EneroDotacion)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.FebreroDotacion = ((ini.FebreroDotacion == null || string.IsNullOrEmpty(ini.FebreroDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.FebreroDotacion)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.MarzoDotacion = ((ini.MarzoDotacion == null || string.IsNullOrEmpty(ini.MarzoDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.MarzoDotacion)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.AbrilDotacion = ((ini.AbrilDotacion == null || string.IsNullOrEmpty(ini.AbrilDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.AbrilDotacion)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.MayoDotacion = ((ini.MayoDotacion == null || string.IsNullOrEmpty(ini.MayoDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.MayoDotacion)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.JunioDotacion = ((ini.JunioDotacion == null || string.IsNullOrEmpty(ini.JunioDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.JunioDotacion)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.JulioDotacion = ((ini.JulioDotacion == null || string.IsNullOrEmpty(ini.JulioDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.JulioDotacion)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.AgostoDotacion = ((ini.AgostoDotacion == null || string.IsNullOrEmpty(ini.AgostoDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.AgostoDotacion)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.SeptiembreDotacion = ((ini.SeptiembreDotacion == null || string.IsNullOrEmpty(ini.SeptiembreDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.SeptiembreDotacion)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.OctubreDotacion = ((ini.OctubreDotacion == null || string.IsNullOrEmpty(ini.OctubreDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.OctubreDotacion)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.NoviembreDotacion = ((ini.NoviembreDotacion == null || string.IsNullOrEmpty(ini.NoviembreDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.NoviembreDotacion)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.DiciembreDotacion = ((ini.DiciembreDotacion == null || string.IsNullOrEmpty(ini.DiciembreDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.DiciembreDotacion)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.TotalDotacion = ((ini.TotalDotacion == null || string.IsNullOrEmpty(ini.TotalDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalDotacion)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.PresAnioMasUnoDotacion = ((ini.PresAnioMasUnoDotacion == null || string.IsNullOrEmpty(ini.PresAnioMasUnoDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.PresAnioMasUnoDotacion)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.PresAnioMasDosDotacion = ((ini.PresAnioMasDosDotacion == null || string.IsNullOrEmpty(ini.PresAnioMasDosDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.PresAnioMasDosDotacion)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                                ViewBag.PresAnioMasTresDotacion = ((ini.PresAnioMasTresDotacion == null || string.IsNullOrEmpty(ini.PresAnioMasTresDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.PresAnioMasTresDotacion)).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                            }
                            else
                            {
                                ViewBag.TotalCapexDotacion = ((ini.TotalCapexDotacion == null || string.IsNullOrEmpty(ini.TotalCapexDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalCapexDotacion)).ToString());
                                ViewBag.EneroDotacion = ((ini.EneroDotacion == null || string.IsNullOrEmpty(ini.EneroDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.EneroDotacion)).ToString());
                                ViewBag.FebreroDotacion = ((ini.FebreroDotacion == null || string.IsNullOrEmpty(ini.FebreroDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.FebreroDotacion)).ToString());
                                ViewBag.MarzoDotacion = ((ini.MarzoDotacion == null || string.IsNullOrEmpty(ini.MarzoDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.MarzoDotacion)).ToString());
                                ViewBag.AbrilDotacion = ((ini.AbrilDotacion == null || string.IsNullOrEmpty(ini.AbrilDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.AbrilDotacion)).ToString());
                                ViewBag.MayoDotacion = ((ini.MayoDotacion == null || string.IsNullOrEmpty(ini.MayoDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.MayoDotacion)).ToString());
                                ViewBag.JunioDotacion = ((ini.JunioDotacion == null || string.IsNullOrEmpty(ini.JunioDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.JunioDotacion)).ToString());
                                ViewBag.JulioDotacion = ((ini.JulioDotacion == null || string.IsNullOrEmpty(ini.JulioDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.JulioDotacion)).ToString());
                                ViewBag.AgostoDotacion = ((ini.AgostoDotacion == null || string.IsNullOrEmpty(ini.AgostoDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.AgostoDotacion)).ToString());
                                ViewBag.SeptiembreDotacion = ((ini.SeptiembreDotacion == null || string.IsNullOrEmpty(ini.SeptiembreDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.SeptiembreDotacion)).ToString());
                                ViewBag.OctubreDotacion = ((ini.OctubreDotacion == null || string.IsNullOrEmpty(ini.OctubreDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.OctubreDotacion)).ToString());
                                ViewBag.NoviembreDotacion = ((ini.NoviembreDotacion == null || string.IsNullOrEmpty(ini.NoviembreDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.NoviembreDotacion)).ToString());
                                ViewBag.DiciembreDotacion = ((ini.DiciembreDotacion == null || string.IsNullOrEmpty(ini.DiciembreDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.DiciembreDotacion)).ToString());
                                ViewBag.TotalDotacion = ((ini.TotalDotacion == null || string.IsNullOrEmpty(ini.TotalDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.TotalDotacion)).ToString());
                                ViewBag.PresAnioMasUnoDotacion = ((ini.PresAnioMasUnoDotacion == null || string.IsNullOrEmpty(ini.PresAnioMasUnoDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.PresAnioMasUnoDotacion)).ToString());
                                ViewBag.PresAnioMasDosDotacion = ((ini.PresAnioMasDosDotacion == null || string.IsNullOrEmpty(ini.PresAnioMasDosDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.PresAnioMasDosDotacion)).ToString());
                                ViewBag.PresAnioMasTresDotacion = ((ini.PresAnioMasTresDotacion == null || string.IsNullOrEmpty(ini.PresAnioMasTresDotacion)) ? "0" : String.Format("{0:#,##0.##}", Convert.ToDouble(ini.PresAnioMasTresDotacion)).ToString());
                            }

                            ViewBag.Prob1 = ini.Prob1;
                            ViewBag.Impacto1 = ini.Impacto1;
                            ViewBag.NivelRiesgo1 = ini.NivelRiesgo1;
                            ViewBag.Clasif1 = ini.Clasif1;
                            ViewBag.Prob2 = ini.Prob2;
                            ViewBag.Impacto2 = ini.Impacto2;
                            ViewBag.NivelRiesgo2 = ini.NivelRiesgo2;
                            ViewBag.Clasif2 = ini.Clasif2;

                            ViewBag.IniPeriodo = ini.IniPeriodo;
                            ViewBag.IrFecha = DateTime.Today.ToString("dd/MM/yyyy");

                            ViewBag.DESCRIPCION = ini.DESCRIPCION;
                            nombreArchivo = ini.DESCRIPCION + "_" + ini.IdPid;
                        }

                        ViewBag.Identificacion = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_IDENTIFICACION_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                        ViewBag.Categorizacion = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_CATEGORIZACION_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                        ViewBag.DescripcionDetallada = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_DESCRIPCIONDETALLADA_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                        ViewBag.EvaluacionEconomica = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_EVALUACIONECONOMICA_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                        ViewBag.EvaluacionRiesgo = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_EVALUACIONRIESGO_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                        ViewBag.Hito = SqlMapper.Query(objConnection, "CAPEX_SEL_VER_HITO_INI", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                        ViewBag.ResumenFinanciero = SqlMapper.Query(objConnection, "CAPEX_SEL_RESUMEN_FINANCIERO", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                    }
                    catch (Exception err)
                    {
                        ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        Utils.LogError(ExceptionResult);
                        return null;
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }

                return new ViewAsPdf("PdfPresupuesto")
                {
                    FileName = "Pdf_Iniciativa_" + nombreArchivo + ".pdf",
                    //  string fileName = "PresupuestoCapex_" + (year + 1) + "_" + DateTime.Now.Millisecond + ".pdf",
                    PageSize = Size.A4,
                    PageOrientation = Orientation.Portrait,
                    PageMargins = { Left = 0, Right = 0 }
                };
            }
        }


        /// <summary>
        /// METODO GENERADOR PDF
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ActionResult descargaExcelPresupuesto(string token)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var fileName = "DESCONOCIDO";
                string rutaPlantilla = Server.MapPath("~/Files/Downloads/");
                FileStream fstream = new FileStream(rutaPlantilla + "TemplateExportExcelIniciativa.xlsx", FileMode.Open);
                XLWorkbook wb = new XLWorkbook(fstream);
                // DataTable dataTable = new DataTable();
                var sheetOne = wb.Worksheets.Where(x => x.Name == "Hoja1").First();
                using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
                {
                    objConnection.Open();
                    try
                    {
                        string titulo = "FICHA INICIATIVA ";
                        DateTime todaysDate = DateTime.Now.Date;
                        int year = todaysDate.Year;
                        CultureInfo ciCL = new CultureInfo("es-CL", false);

                        string tipoIniciativaEjercicioOficial = ((Session["tipoIniciativaEjercicioOficial"] != null) ? Convert.ToString(Session["tipoIniciativaEjercicioOficial"]) : "");
                        string anioIniciativaEjercicioOficial = ((Session["anioIniciativaEjercicioOficial"] != null) ? Convert.ToString(Session["anioIniciativaEjercicioOficial"]) : "");
                        bool esEjercicioOficial = ((!string.IsNullOrEmpty(tipoIniciativaEjercicioOficial) && !string.IsNullOrEmpty(anioIniciativaEjercicioOficial)) ? true : false);

                        string tipoIniciativaOrientacionComercial = ((Session["tipoIniciativaOrientacionComercial"] != null) ? Convert.ToString(Session["tipoIniciativaOrientacionComercial"]) : "");
                        string anioIniciativaOrientacionComercial = ((Session["anioIniciativaOrientacionComercial"] != null) ? Convert.ToString(Session["anioIniciativaOrientacionComercial"]) : "");
                        string parametroVNToken = ((Session["ParametroVNToken"] != null) ? Convert.ToString(Session["ParametroVNToken"]) : "");
                        bool esParametroVNToken = ((!string.IsNullOrEmpty(tipoIniciativaOrientacionComercial) && !string.IsNullOrEmpty(anioIniciativaOrientacionComercial) && !string.IsNullOrEmpty(parametroVNToken)) ? true : false);

                        string procedimientoAmacenado = "CAPEX_SEL_REPORTE_INICIATIVA_PDF_PRESUPUESTO";
                        var parametros = new DynamicParameters();
                        parametros.Add("IniToken", token);
                        if (esEjercicioOficial)
                        {
                            parametros.Add("TipoIniciativa", tipoIniciativaEjercicioOficial);
                            parametros.Add("Periodo", anioIniciativaEjercicioOficial);
                            procedimientoAmacenado = "CAPEX_SEL_REPORTE_INICIATIVA_PDF_PRESUPUESTO_EJERCICIO_OFICIAL";
                        }
                        else if (esParametroVNToken)
                        {
                            var parametosOrigen = new DynamicParameters();
                            parametosOrigen.Add("ParametroVNToken", parametroVNToken);
                            parametosOrigen.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 1);
                            SqlMapper.Query(objConnection, "CAPEX_PARAMETRO_IS_V0", parametosOrigen, commandType: CommandType.StoredProcedure).SingleOrDefault();
                            string respuestaOrigen = parametosOrigen.Get<string>("Respuesta");
                            if (respuestaOrigen == null || string.IsNullOrEmpty(respuestaOrigen.Trim()) || "0".Equals(respuestaOrigen.Trim()))
                            {
                                parametros.Add("TipoIniciativa", tipoIniciativaOrientacionComercial);
                                parametros.Add("Periodo", anioIniciativaOrientacionComercial);
                                parametros.Add("ParametroVNToken", parametroVNToken);
                                procedimientoAmacenado = "CAPEX_SEL_REPORTE_INICIATIVA_PDF_PRESUPUESTO_PARAMETROVN";
                            }
                        }
                        var IniciativaPdfPresupuesto = SqlMapper.Query(objConnection, procedimientoAmacenado, parametros, commandType: CommandType.StoredProcedure).ToList();
                        foreach (var ini in IniciativaPdfPresupuesto)
                        {
                            sheetOne.Cell("E3").Value = titulo + ini.DESCRIPCION;
                            sheetOne.Cell("N3").Value = titulo + ini.DESCRIPCION;

                            sheetOne.Cell("B10").Value = ini.DESCRIPCION;
                            sheetOne.Cell("K10").Value = ini.DESCRIPCION;

                            ViewBag.Anio = ini.IniPeriodo;

                            string iniTipo = ini.IniTipo;
                            int anioIni = ViewBag.Anio;
                            if (iniTipo != null && !string.IsNullOrEmpty(iniTipo.ToString()) && ("CB".Equals(iniTipo.ToString()) || "CD".Equals(iniTipo.ToString())))
                            {
                                anioIni += 1;
                            }
                            sheetOne.Cell("L43").Value = anioIni;
                            ViewBag.AnioUno = anioIni + 1;
                            sheetOne.Cell("M18").Value = ViewBag.AnioUno;
                            sheetOne.Cell("M43").Value = ViewBag.AnioUno;
                            ViewBag.AnioDos = anioIni + 2;
                            sheetOne.Cell("O18").Value = ViewBag.AnioDos;
                            sheetOne.Cell("N43").Value = ViewBag.AnioDos;
                            ViewBag.AnioTres = anioIni + 3;
                            sheetOne.Cell("Q18").Value = ViewBag.AnioTres;
                            sheetOne.Cell("O43").Value = ViewBag.AnioTres;

                            ViewBag.NombreProyecto = ini.PidNombreProyecto;
                            sheetOne.Cell("E8").Value = ini.PidNombreProyecto;
                            sheetOne.Cell("O8").Value = ini.PidNombreProyecto;
                            ViewBag.NombreProyectoAlias = ini.PidNombreProyectoAlias;
                            sheetOne.Cell("E7").Value = ini.PidNombreProyectoAlias;
                            sheetOne.Cell("O7").Value = ini.PidNombreProyectoAlias;
                            ViewBag.CodigoIniciativa = ini.PidCodigoIniciativa;
                            sheetOne.Cell("E9").Value = ini.PidCodigoIniciativa;
                            sheetOne.Cell("O9").Value = ini.PidCodigoIniciativa;
                            ViewBag.TipoEjercicio = ini.IniTipoEjercicio;
                            ViewBag.Estado = ini.CatEstadoProyecto;
                            sheetOne.Cell("B17").Value = ini.CatEstadoProyecto;
                            ViewBag.MacroCategoria = ini.CatMacroCategoria;
                            sheetOne.Cell("C17").Value = ini.CatMacroCategoria;
                            ViewBag.Categoria = ini.CatCategoria;
                            sheetOne.Cell("E17").Value = ini.CatCategoria;
                            ViewBag.GerenciaInversion = ini.PidGerenciaInversion;
                            sheetOne.Cell("B14").Value = ini.PidGerenciaInversion;
                            ViewBag.GerenteInversion = ini.PidGerenteInversion;
                            ViewBag.GerenciaEjecucion = ini.PidGerenciaEjecucion;
                            sheetOne.Cell("D14").Value = ini.PidGerenciaEjecucion;
                            ViewBag.GerenteEjecucion = ini.PidGerenteEjecucion;
                            ViewBag.Superintendencia = ini.PidSuperintendencia;
                            sheetOne.Cell("F14").Value = ini.PidSuperintendencia;
                            ViewBag.EncargadoControl = ini.PidEncargadoControl;
                            sheetOne.Cell("F20").Value = ini.PidEncargadoControl;
                            ViewBag.NivelIngenieria = ini.CatNivelIngenieria;
                            sheetOne.Cell("B20").Value = ini.CatNivelIngenieria;
                            ViewBag.ClasificacionSSO = ini.CatClasificacionSSO;
                            sheetOne.Cell("F17").Value = ini.CatClasificacionSSO;
                            ViewBag.EstandarSeguridad = ini.CatEstandarSeguridad;
                            sheetOne.Cell("D20").Value = ini.CatEstandarSeguridad;
                            ViewBag.Clase = ini.CatClase;
                            sheetOne.Cell("H17").Value = ini.CatClase;
                            ViewBag.PddObjetivo = ini.PddObjetivo;
                            sheetOne.Cell("B23").Value = ini.PddObjetivo;
                            ViewBag.PddJustificacion = ini.PddJustificacion;
                            sheetOne.Cell("B28").Value = ini.PddJustificacion;
                            ViewBag.PddAlcance = ini.PddAlcance;
                            sheetOne.Cell("B33").Value = ini.PddAlcance;

                            ViewBag.KP1Descripcion1 = ini.KP1Descripcion1;
                            sheetOne.Cell("L38").Value = ini.KP1Descripcion1;
                            ViewBag.KP1Unidad1 = ini.KP1Unidad1;
                            sheetOne.Cell("Q38").Value = ini.KP1Unidad1;
                            ViewBag.KP1Actual1 = ini.KP1Actual1;
                            sheetOne.Cell("S38").Value = ini.KP1Actual1;
                            ViewBag.KP1Target1 = ini.KP1Target1;
                            sheetOne.Cell("V38").Value = ini.KP1Target1;
                            ViewBag.KP1Descripcion2 = ini.KP1Descripcion2;
                            sheetOne.Cell("L39").Value = ini.KP1Descripcion2;
                            ViewBag.KP1Unidad2 = ini.KP1Unidad2;
                            sheetOne.Cell("Q39").Value = ini.KP1Unidad2;
                            ViewBag.KP1Actual2 = ini.KP1Actual2;
                            sheetOne.Cell("S39").Value = ini.KP1Actual2;
                            ViewBag.KP1Target2 = ini.KP1Target2;
                            sheetOne.Cell("V39").Value = ini.KP1Target2;
                            ViewBag.KP1Descripcion3 = ini.KP1Descripcion3;
                            sheetOne.Cell("L40").Value = ini.KP1Descripcion3;
                            ViewBag.KP1Unidad3 = ini.KP1Unidad3;
                            sheetOne.Cell("Q40").Value = ini.KP1Unidad3;
                            ViewBag.KP1Actual3 = ini.KP1Actual3;
                            sheetOne.Cell("S40").Value = ini.KP1Actual3;
                            ViewBag.KP1Target3 = ini.KP1Target3;
                            sheetOne.Cell("V40").Value = ini.KP1Target3;

                            ViewBag.HCI = ini.HCI;
                            sheetOne.Cell("B38").Value = ViewBag.HCI;
                            ViewBag.HCA = ini.HCA;
                            sheetOne.Cell("C38").Value = ViewBag.HCA;
                            ViewBag.HOPR = ini.HOPR;
                            sheetOne.Cell("D38").Value = ViewBag.HOPR;
                            ViewBag.HVPF = ini.HVPF;
                            sheetOne.Cell("E38").Value = ViewBag.HVPF;
                            ViewBag.HPE = ini.HPE;
                            sheetOne.Cell("F38").Value = ViewBag.HPE;
                            ViewBag.HDIRPLC = ini.HDIRPLC;
                            sheetOne.Cell("G38").Value = ViewBag.HDIRPLC;
                            ViewBag.HDIRMCEN = ini.HDIRMCEN;
                            sheetOne.Cell("H38").Value = ViewBag.HDIRMCEN;
                            ViewBag.HINICIO = ini.HINICIO;
                            sheetOne.Cell("B40").Value = ViewBag.HINICIO;
                            ViewBag.HTERMINO = ini.HTERMINO;
                            sheetOne.Cell("C40").Value = ViewBag.HTERMINO;
                            ViewBag.HCIERRE = ini.HCIERRE;
                            sheetOne.Cell("D40").Value = ViewBag.HCIERRE;
                            ViewBag.HPOSTEVAL = ini.HPOSTEVAL;
                            sheetOne.Cell("E40").Value = ViewBag.HPOSTEVAL;
                            ViewBag.EriMFL1 = ((string.IsNullOrEmpty(ini.EriMFL1)) ? double.Parse("0", ciCL) : double.Parse(ini.EriMFL1.Replace('.', ','), ciCL));
                            sheetOne.Cell("P24").Value = ViewBag.EriMFL1;
                            sheetOne.Cell("P24").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.EriMFL2 = ((string.IsNullOrEmpty(ini.EriMFL2)) ? double.Parse("0", ciCL) : double.Parse(ini.EriMFL2.Replace('.', ','), ciCL));
                            sheetOne.Cell("P27").Value = ViewBag.EriMFL2;
                            sheetOne.Cell("P27").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.RiesgoNombre = (string.IsNullOrEmpty(ini.RiesgoNombre) ? "" : (ini.RiesgoNombre.Length > 42 ? (ini.RiesgoNombre.ToString().Substring(0, 39) + "...") : ini.RiesgoNombre));
                            sheetOne.Cell("R25").Value = ViewBag.RiesgoNombre;
                            ViewBag.RiesgoImpacto = ((ini.RiesgoImpacto != null && ini.RiesgoImpacto != 0) ? ini.RiesgoImpacto.ToString() : "");
                            sheetOne.Cell("S29").Value = ViewBag.RiesgoImpacto;
                            ViewBag.RiesgoProbabilidad = ((ini.RiesgoProbabilidad != null && ini.RiesgoProbabilidad != 0) ? ini.RiesgoProbabilidad.ToString() : "");
                            sheetOne.Cell("U29").Value = ViewBag.RiesgoProbabilidad;

                            ViewBag.Van = ((string.IsNullOrEmpty(ini.EveVan)) ? double.Parse("0", ciCL) : double.Parse(ini.EveVan.Replace('.', ','), ciCL));
                            sheetOne.Cell("K33").Value = ViewBag.Van;
                            sheetOne.Cell("K33").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.Tir = ((string.IsNullOrEmpty(ini.EveTir)) ? double.Parse("0", ciCL) : double.Parse(ini.EveTir.Replace('.', ','), ciCL));
                            sheetOne.Cell("L33").Value = ViewBag.Tir;
                            sheetOne.Cell("L33").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.Ivan = ((string.IsNullOrEmpty(ini.EveIvan)) ? double.Parse("0", ciCL) : double.Parse(ini.EveIvan.Replace('.', ','), ciCL));
                            sheetOne.Cell("N33").Value = ViewBag.Ivan;
                            sheetOne.Cell("N33").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.PayBack = ((string.IsNullOrEmpty(ini.EvePayBack)) ? double.Parse("0", ciCL) : double.Parse(ini.EvePayBack.Replace('.', ','), ciCL));
                            sheetOne.Cell("P33").Value = ViewBag.PayBack;
                            sheetOne.Cell("P33").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.VidaUtil = ((string.IsNullOrEmpty(ini.EveVidaUtil)) ? double.Parse("0", ciCL) : double.Parse(ini.EveVidaUtil.Replace('.', ','), ciCL));
                            sheetOne.Cell("R33").Value = ViewBag.VidaUtil;
                            sheetOne.Cell("R33").Style.NumberFormat.Format = "#,##0.00";


                            //ViewBag.EveTipoCambio = ((string.IsNullOrEmpty(ini.EveTipoCambio)) ? double.Parse("0", ciCL) : double.Parse(ini.EveTipoCambio.Replace('.', ','), ciCL));
                            //sheetOne.Cell("T33").Value = ViewBag.EveTipoCambio;
                            //sheetOne.Cell("T33").Style.NumberFormat.Format = "#,##0.00";

                            ViewBag.FaseIngenieria = ((string.IsNullOrEmpty(ini.TotalCapexIng)) ? double.Parse("0", ciCL) : double.Parse(ini.TotalCapexIng.Replace('.', ','), ciCL));
                            sheetOne.Cell("B43").Value = ViewBag.FaseIngenieria;
                            sheetOne.Cell("B43").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.FaseAdquisicion = ((string.IsNullOrEmpty(ini.TotalCapexAdq)) ? double.Parse("0", ciCL) : double.Parse(ini.TotalCapexAdq.Replace('.', ','), ciCL));
                            sheetOne.Cell("D43").Value = ViewBag.FaseAdquisicion;
                            sheetOne.Cell("D43").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.FaseConstruccion = ((string.IsNullOrEmpty(ini.TotalCapexConst)) ? double.Parse("0", ciCL) : double.Parse(ini.TotalCapexConst.Replace('.', ','), ciCL));
                            sheetOne.Cell("F43").Value = ViewBag.FaseConstruccion;
                            sheetOne.Cell("F43").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.FaseAdministracion = ((string.IsNullOrEmpty(ini.TotalCapexAdm)) ? double.Parse("0", ciCL) : double.Parse(ini.TotalCapexAdm.Replace('.', ','), ciCL));
                            sheetOne.Cell("H43").Value = ViewBag.FaseAdministracion;
                            sheetOne.Cell("H43").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.TotalContingencia = ((string.IsNullOrEmpty(ini.TotalCapexCont)) ? double.Parse("0", ciCL) : double.Parse(ini.TotalCapexCont.Replace('.', ','), ciCL));
                            sheetOne.Cell("H45").Value = ViewBag.TotalContingencia;
                            sheetOne.Cell("H45").Style.NumberFormat.Format = "#,##0.00";

                            ViewBag.Contingencia = ((string.IsNullOrEmpty(ini.PorContingencia)) ? double.Parse("0", ciCL) : double.Parse(ini.PorContingencia.Replace('.', ','), ciCL));
                            sheetOne.Cell("B45").Value = ViewBag.Contingencia;
                            sheetOne.Cell("B45").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.CostoDueno = ((string.IsNullOrEmpty(ini.PorCostoDueno)) ? double.Parse("0", ciCL) : double.Parse(ini.PorCostoDueno.Replace('.', ','), ciCL));
                            sheetOne.Cell("D45").Value = ViewBag.CostoDueno;
                            sheetOne.Cell("D45").Style.NumberFormat.Format = "#,##0.00";


                            ViewBag.TotalCapexFisico = ((string.IsNullOrEmpty(ini.TotalCapexFisico)) ? double.Parse("0", ciCL) : double.Parse(ini.TotalCapexFisico.Replace('.', ','), ciCL));
                            sheetOne.Cell("L16").Value = ViewBag.TotalCapexFisico;
                            sheetOne.Cell("L16").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.EneroFisico = ((string.IsNullOrEmpty(ini.EneroFisico)) ? double.Parse("0", ciCL) : double.Parse(ini.EneroFisico.Replace('.', ','), ciCL));
                            sheetOne.Cell("M16").Value = ViewBag.EneroFisico;
                            sheetOne.Cell("M16").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.FebreroFisico = ((string.IsNullOrEmpty(ini.FebreroFisico)) ? double.Parse("0", ciCL) : double.Parse(ini.FebreroFisico.Replace('.', ','), ciCL));
                            sheetOne.Cell("N16").Value = ViewBag.FebreroFisico;
                            sheetOne.Cell("N16").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.MarzoFisico = ((string.IsNullOrEmpty(ini.MarzoFisico)) ? double.Parse("0", ciCL) : double.Parse(ini.MarzoFisico.Replace('.', ','), ciCL));
                            sheetOne.Cell("O16").Value = ViewBag.MarzoFisico;
                            sheetOne.Cell("O16").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.AbrilFisico = ((string.IsNullOrEmpty(ini.AbrilFisico)) ? double.Parse("0", ciCL) : double.Parse(ini.AbrilFisico.Replace('.', ','), ciCL));
                            sheetOne.Cell("P16").Value = ViewBag.AbrilFisico;
                            sheetOne.Cell("P16").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.MayoFisico = ((string.IsNullOrEmpty(ini.MayoFisico)) ? double.Parse("0", ciCL) : double.Parse(ini.MayoFisico.Replace('.', ','), ciCL));
                            sheetOne.Cell("Q16").Value = ViewBag.MayoFisico;
                            sheetOne.Cell("Q16").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.JunioFisico = ((string.IsNullOrEmpty(ini.JunioFisico)) ? double.Parse("0", ciCL) : double.Parse(ini.JunioFisico.Replace('.', ','), ciCL));
                            sheetOne.Cell("R16").Value = ViewBag.JunioFisico;
                            sheetOne.Cell("R16").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.JulioFisico = ((string.IsNullOrEmpty(ini.JulioFisico)) ? double.Parse("0", ciCL) : double.Parse(ini.JulioFisico.Replace('.', ','), ciCL));
                            sheetOne.Cell("S16").Value = ViewBag.JulioFisico;
                            sheetOne.Cell("S16").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.AgostoFisico = ((string.IsNullOrEmpty(ini.AgostoFisico)) ? double.Parse("0", ciCL) : double.Parse(ini.AgostoFisico.Replace('.', ','), ciCL));
                            sheetOne.Cell("T16").Value = ViewBag.AgostoFisico;
                            sheetOne.Cell("T16").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.SeptiembreFisico = ((string.IsNullOrEmpty(ini.SeptiembreFisico)) ? double.Parse("0", ciCL) : double.Parse(ini.SeptiembreFisico.Replace('.', ','), ciCL));
                            sheetOne.Cell("U16").Value = ViewBag.SeptiembreFisico;
                            sheetOne.Cell("U16").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.OctubreFisico = ((string.IsNullOrEmpty(ini.OctubreFisico)) ? double.Parse("0", ciCL) : double.Parse(ini.OctubreFisico.Replace('.', ','), ciCL));
                            sheetOne.Cell("V16").Value = ViewBag.OctubreFisico;
                            sheetOne.Cell("V16").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.NoviembreFisico = ((string.IsNullOrEmpty(ini.NoviembreFisico)) ? double.Parse("0", ciCL) : double.Parse(ini.NoviembreFisico.Replace('.', ','), ciCL));
                            sheetOne.Cell("W16").Value = ViewBag.NoviembreFisico;
                            sheetOne.Cell("W16").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.DiciembreFisico = ((string.IsNullOrEmpty(ini.DiciembreFisico)) ? double.Parse("0", ciCL) : double.Parse(ini.DiciembreFisico.Replace('.', ','), ciCL));
                            sheetOne.Cell("X16").Value = ViewBag.DiciembreFisico;
                            sheetOne.Cell("X16").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.TotalFisico = ((string.IsNullOrEmpty(ini.TotalFisico)) ? double.Parse("0", ciCL) : double.Parse(ini.TotalFisico.Replace('.', ','), ciCL));
                            sheetOne.Cell("Y16").Value = ViewBag.TotalFisico;
                            sheetOne.Cell("Y16").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.PresAnioMasUnoFisico = ((string.IsNullOrEmpty(ini.PresAnioMasUnoFisico)) ? double.Parse("0", ciCL) : double.Parse(ini.PresAnioMasUnoFisico.Replace('.', ','), ciCL));
                            sheetOne.Cell("L20").Value = ViewBag.PresAnioMasUnoFisico;
                            sheetOne.Cell("L20").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.PresAnioMasDosFisico = ((string.IsNullOrEmpty(ini.PresAnioMasDosFisico)) ? double.Parse("0", ciCL) : double.Parse(ini.PresAnioMasDosFisico.Replace('.', ','), ciCL));
                            sheetOne.Cell("N20").Value = ViewBag.PresAnioMasDosFisico;
                            sheetOne.Cell("N20").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.PresAnioMasTresFisico = ((string.IsNullOrEmpty(ini.PresAnioMasTresFisico)) ? double.Parse("0", ciCL) : double.Parse(ini.PresAnioMasTresFisico.Replace('.', ','), ciCL));
                            sheetOne.Cell("P20").Value = ViewBag.PresAnioMasTresFisico;
                            sheetOne.Cell("P20").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.TotalAcumFisico = ((string.IsNullOrEmpty(ini.TotalAcumFisico)) ? double.Parse("0", ciCL) : double.Parse(ini.TotalAcumFisico.Replace('.', ','), ciCL));
                            sheetOne.Cell("R20").Value = ViewBag.TotalAcumFisico;
                            sheetOne.Cell("R20").Style.NumberFormat.Format = "#,##0.00";

                            ViewBag.TotalCapexTotPar = ((string.IsNullOrEmpty(ini.TotalCapexTotPar)) ? double.Parse("0", ciCL) : double.Parse(ini.TotalCapexTotPar.Replace('.', ','), ciCL));
                            sheetOne.Cell("L15").Value = ViewBag.TotalCapexTotPar;
                            sheetOne.Cell("L15").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.EneroTotPar = ((string.IsNullOrEmpty(ini.EneroTotPar)) ? double.Parse("0", ciCL) : double.Parse(ini.EneroTotPar.Replace('.', ','), ciCL));
                            sheetOne.Cell("M15").Value = ViewBag.EneroTotPar;
                            sheetOne.Cell("M15").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.FebreroTotPar = ((string.IsNullOrEmpty(ini.FebreroTotPar)) ? "0" : formatNumberPdf(ini.FebreroTotPar));
                            sheetOne.Cell("N15").Value = ViewBag.FebreroTotPar;
                            sheetOne.Cell("N15").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.MarzoTotPar = ((string.IsNullOrEmpty(ini.MarzoTotPar)) ? double.Parse("0", ciCL) : double.Parse(ini.MarzoTotPar.Replace('.', ','), ciCL));
                            sheetOne.Cell("O15").Value = ViewBag.MarzoTotPar;
                            sheetOne.Cell("O15").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.AbrilTotPar = ((string.IsNullOrEmpty(ini.AbrilTotPar)) ? double.Parse("0", ciCL) : double.Parse(ini.AbrilTotPar.Replace('.', ','), ciCL));
                            sheetOne.Cell("P15").Value = ViewBag.AbrilTotPar;
                            sheetOne.Cell("P15").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.MayoTotPar = ((string.IsNullOrEmpty(ini.MayoTotPar)) ? double.Parse("0", ciCL) : double.Parse(ini.MayoTotPar.Replace('.', ','), ciCL));
                            sheetOne.Cell("Q15").Value = ViewBag.MayoTotPar;
                            sheetOne.Cell("Q15").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.JunioTotPar = ((string.IsNullOrEmpty(ini.JunioTotPar)) ? double.Parse("0", ciCL) : double.Parse(ini.JunioTotPar.Replace('.', ','), ciCL));
                            sheetOne.Cell("R15").Value = ViewBag.JunioTotPar;
                            sheetOne.Cell("R15").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.JulioTotPar = ((string.IsNullOrEmpty(ini.JulioTotPar)) ? double.Parse("0", ciCL) : double.Parse(ini.JulioTotPar.Replace('.', ','), ciCL));
                            sheetOne.Cell("S15").Value = ViewBag.JulioTotPar;
                            sheetOne.Cell("S15").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.AgostoTotPar = ((string.IsNullOrEmpty(ini.AgostoTotPar)) ? double.Parse("0", ciCL) : double.Parse(ini.AgostoTotPar.Replace('.', ','), ciCL));
                            sheetOne.Cell("T15").Value = ViewBag.AgostoTotPar;
                            sheetOne.Cell("T15").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.SeptiembreTotPar = ((string.IsNullOrEmpty(ini.SeptiembreTotPar)) ? double.Parse("0", ciCL) : double.Parse(ini.SeptiembreTotPar.Replace('.', ','), ciCL));
                            sheetOne.Cell("U15").Value = ViewBag.SeptiembreTotPar;
                            sheetOne.Cell("U15").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.OctubreTotPar = ((string.IsNullOrEmpty(ini.OctubreTotPar)) ? double.Parse("0", ciCL) : double.Parse(ini.OctubreTotPar.Replace('.', ','), ciCL));
                            sheetOne.Cell("V15").Value = ViewBag.OctubreTotPar;
                            sheetOne.Cell("V15").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.NoviembreTotPar = ((string.IsNullOrEmpty(ini.NoviembreTotPar)) ? double.Parse("0", ciCL) : double.Parse(ini.NoviembreTotPar.Replace('.', ','), ciCL));
                            sheetOne.Cell("W15").Value = ViewBag.NoviembreTotPar;
                            sheetOne.Cell("W15").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.DiciembreTotPar = ((string.IsNullOrEmpty(ini.DiciembreTotPar)) ? double.Parse("0", ciCL) : double.Parse(ini.DiciembreTotPar.Replace('.', ','), ciCL));
                            sheetOne.Cell("X15").Value = ViewBag.DiciembreTotPar;
                            sheetOne.Cell("X15").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.TotalTotPar = ((string.IsNullOrEmpty(ini.TotalTotPar)) ? double.Parse("0", ciCL) : double.Parse(ini.TotalTotPar.Replace('.', ','), ciCL));
                            sheetOne.Cell("Y15").Value = ViewBag.TotalTotPar;
                            sheetOne.Cell("Y15").Style.NumberFormat.Format = "#,##0.00";

                            ViewBag.PresAnioMasUnoTotPar = ((string.IsNullOrEmpty(ini.PresAnioMasUnoTotPar)) ? double.Parse("0", ciCL) : double.Parse(ini.PresAnioMasUnoTotPar.Replace('.', ','), ciCL));
                            sheetOne.Cell("L19").Value = ViewBag.PresAnioMasUnoTotPar;
                            sheetOne.Cell("L19").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.PresAnioMasDosTotPar = ((string.IsNullOrEmpty(ini.PresAnioMasDosTotPar)) ? double.Parse("0", ciCL) : double.Parse(ini.PresAnioMasDosTotPar.Replace('.', ','), ciCL));
                            sheetOne.Cell("N19").Value = ViewBag.PresAnioMasDosTotPar;
                            sheetOne.Cell("N19").Style.NumberFormat.Format = "#,##0.00";
                            ViewBag.PresAnioMasTresTotPar = ((string.IsNullOrEmpty(ini.PresAnioMasTresTotPar)) ? double.Parse("0", ciCL) : double.Parse(ini.PresAnioMasTresTotPar.Replace('.', ','), ciCL));
                            sheetOne.Cell("P19").Value = ViewBag.PresAnioMasTresTotPar;
                            sheetOne.Cell("P19").Style.NumberFormat.Format = "#,##0.00";


                            ViewBag.TotalAcumTotPar = ((string.IsNullOrEmpty(ini.TotalAcumTOTPAR)) ? double.Parse("0", ciCL) : double.Parse(ini.TotalAcumTOTPAR.Replace('.', ','), ciCL));
                            sheetOne.Cell("R19").Value = ViewBag.TotalAcumTotPar;
                            sheetOne.Cell("R19").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("H11").Value = ViewBag.TotalAcumTotPar;
                            sheetOne.Cell("H11").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("C41").Value = ViewBag.TotalAcumTotPar;
                            sheetOne.Cell("C41").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("V11").Value = ViewBag.TotalAcumTotPar;
                            sheetOne.Cell("V11").Style.NumberFormat.Format = "#,##0.00";

                            sheetOne.Cell("L17").Value = "";
                            sheetOne.Cell("L17").Style.NumberFormat.Format = "#,##0";
                            ViewBag.EneroDotacion = ((string.IsNullOrEmpty(ini.EneroDotacion)) ? double.Parse("0", ciCL) : double.Parse(ini.EneroDotacion.Replace('.', ','), ciCL));
                            sheetOne.Cell("M17").Value = ViewBag.EneroDotacion;
                            sheetOne.Cell("M17").Style.NumberFormat.Format = "#,##0";
                            ViewBag.FebreroDotacion = ((string.IsNullOrEmpty(ini.FebreroDotacion)) ? double.Parse("0", ciCL) : double.Parse(ini.FebreroDotacion.Replace('.', ','), ciCL));
                            sheetOne.Cell("N17").Value = ViewBag.FebreroDotacion;
                            sheetOne.Cell("N17").Style.NumberFormat.Format = "#,##0";
                            ViewBag.MarzoDotacion = ((string.IsNullOrEmpty(ini.MarzoDotacion)) ? double.Parse("0", ciCL) : double.Parse(ini.MarzoDotacion.Replace('.', ','), ciCL));
                            sheetOne.Cell("O17").Value = ViewBag.MarzoDotacion;
                            sheetOne.Cell("O17").Style.NumberFormat.Format = "#,##0";
                            ViewBag.AbrilDotacion = ((string.IsNullOrEmpty(ini.AbrilDotacion)) ? double.Parse("0", ciCL) : double.Parse(ini.AbrilDotacion.Replace('.', ','), ciCL));
                            sheetOne.Cell("P17").Value = ViewBag.AbrilDotacion;
                            sheetOne.Cell("P17").Style.NumberFormat.Format = "#,##0";
                            ViewBag.MayoDotacion = ((string.IsNullOrEmpty(ini.MayoDotacion)) ? double.Parse("0", ciCL) : double.Parse(ini.MayoDotacion.Replace('.', ','), ciCL));
                            sheetOne.Cell("Q17").Value = ViewBag.MayoDotacion;
                            sheetOne.Cell("Q17").Style.NumberFormat.Format = "#,##0";
                            ViewBag.JunioDotacion = ((string.IsNullOrEmpty(ini.JunioDotacion)) ? double.Parse("0", ciCL) : double.Parse(ini.JunioDotacion.Replace('.', ','), ciCL));
                            sheetOne.Cell("R17").Value = ViewBag.JunioDotacion;
                            sheetOne.Cell("R17").Style.NumberFormat.Format = "#,##0";
                            ViewBag.JulioDotacion = ((string.IsNullOrEmpty(ini.JulioDotacion)) ? double.Parse("0", ciCL) : double.Parse(ini.JulioDotacion.Replace('.', ','), ciCL));
                            sheetOne.Cell("S17").Value = ViewBag.JulioDotacion;
                            sheetOne.Cell("S17").Style.NumberFormat.Format = "#,##0";
                            ViewBag.AgostoDotacion = ((string.IsNullOrEmpty(ini.AgostoDotacion)) ? double.Parse("0", ciCL) : double.Parse(ini.AgostoDotacion.Replace('.', ','), ciCL));
                            sheetOne.Cell("T17").Value = ViewBag.AgostoDotacion;
                            sheetOne.Cell("T17").Style.NumberFormat.Format = "#,##0";
                            ViewBag.SeptiembreDotacion = ((string.IsNullOrEmpty(ini.SeptiembreDotacion)) ? double.Parse("0", ciCL) : double.Parse(ini.SeptiembreDotacion.Replace('.', ','), ciCL));
                            sheetOne.Cell("U17").Value = ViewBag.SeptiembreDotacion;
                            sheetOne.Cell("U17").Style.NumberFormat.Format = "#,##0";
                            ViewBag.OctubreDotacion = ((string.IsNullOrEmpty(ini.OctubreDotacion)) ? double.Parse("0", ciCL) : double.Parse(ini.OctubreDotacion.Replace('.', ','), ciCL));
                            sheetOne.Cell("V17").Value = ViewBag.OctubreDotacion;
                            sheetOne.Cell("V17").Style.NumberFormat.Format = "#,##0";
                            ViewBag.NoviembreDotacion = ((string.IsNullOrEmpty(ini.NoviembreDotacion)) ? double.Parse("0", ciCL) : double.Parse(ini.NoviembreDotacion.Replace('.', ','), ciCL));
                            sheetOne.Cell("W17").Value = ViewBag.NoviembreDotacion;
                            sheetOne.Cell("W17").Style.NumberFormat.Format = "#,##0";
                            ViewBag.DiciembreDotacion = ((string.IsNullOrEmpty(ini.DiciembreDotacion)) ? double.Parse("0", ciCL) : double.Parse(ini.DiciembreDotacion.Replace('.', ','), ciCL));
                            sheetOne.Cell("X17").Value = ViewBag.DiciembreDotacion;
                            sheetOne.Cell("X17").Style.NumberFormat.Format = "#,##0";
                            ViewBag.TotalCapexDotacion = ((string.IsNullOrEmpty(ini.TotalCapexDotacion)) ? double.Parse("0", ciCL) : double.Parse(ini.TotalCapexDotacion.Replace('.', ','), ciCL));
                            sheetOne.Cell("Y17").Value = ViewBag.TotalCapexDotacion;
                            sheetOne.Cell("Y17").Style.NumberFormat.Format = "#,##0";

                            //ViewBag.TotalDotacion = String.Format("{0:#,##0.##}", ((string.IsNullOrEmpty(ini.TotalDotacion)) ? double.Parse("0", ciCL) : double.Parse(ini.TotalDotacion.Replace('.', ','), ciCL)));
                            ViewBag.PresAnioMasUnoDotacion = ((string.IsNullOrEmpty(ini.PresAnioMasUnoDotacion)) ? double.Parse("0", ciCL) : double.Parse(ini.PresAnioMasUnoDotacion.Replace('.', ','), ciCL));
                            sheetOne.Cell("L21").Value = ViewBag.PresAnioMasUnoDotacion;
                            sheetOne.Cell("L21").Style.NumberFormat.Format = "#,##0";
                            ViewBag.PresAnioMasDosDotacion = ((string.IsNullOrEmpty(ini.PresAnioMasDosDotacion)) ? double.Parse("0", ciCL) : double.Parse(ini.PresAnioMasDosDotacion.Replace('.', ','), ciCL));
                            sheetOne.Cell("N21").Value = ViewBag.PresAnioMasDosDotacion;
                            sheetOne.Cell("N21").Style.NumberFormat.Format = "#,##0";
                            ViewBag.PresAnioMasTresDotacion = ((string.IsNullOrEmpty(ini.PresAnioMasTresDotacion)) ? double.Parse("0", ciCL) : double.Parse(ini.PresAnioMasTresDotacion.Replace('.', ','), ciCL));
                            sheetOne.Cell("P21").Value = ViewBag.PresAnioMasTresDotacion;
                            sheetOne.Cell("P21").Style.NumberFormat.Format = "#,##0";
                            sheetOne.Cell("R21").Value = ViewBag.TotalCapexDotacion;
                            sheetOne.Cell("R21").Style.NumberFormat.Format = "#,##0";

                            sheetOne.Cell("F45").Value = (string.IsNullOrEmpty(ini.HitNacExt) ? string.Empty : ini.HitNacExt);


                            ViewBag.Prob1 = ini.Prob1;
                            sheetOne.Cell("K26").Value = ViewBag.Prob1;
                            ViewBag.Impacto1 = ini.Impacto1;
                            sheetOne.Cell("L26").Value = ViewBag.Impacto1;
                            ViewBag.NivelRiesgo1 = ini.NivelRiesgo1;
                            sheetOne.Cell("N26").Value = ViewBag.NivelRiesgo1;
                            ViewBag.Clasif1 = ini.Clasif1;
                            sheetOne.Cell("P26").Value = ViewBag.Clasif1;
                            ViewBag.Prob2 = ini.Prob2;
                            sheetOne.Cell("K29").Value = ViewBag.Prob2;
                            ViewBag.Impacto2 = ini.Impacto2;
                            sheetOne.Cell("L29").Value = ViewBag.Impacto2;
                            ViewBag.NivelRiesgo2 = ini.NivelRiesgo2;
                            sheetOne.Cell("N29").Value = ViewBag.NivelRiesgo2;
                            ViewBag.Clasif2 = ini.Clasif2;
                            sheetOne.Cell("P29").Value = ViewBag.Clasif2;

                            ViewBag.IniPeriodo = ini.IniPeriodo;
                            sheetOne.Cell("E10").Value = anioIni;
                            sheetOne.Cell("O10").Value = anioIni;
                            ViewBag.IrFecha = DateTime.Today.ToString("dd-MM-yyyy");
                            sheetOne.Cell("H10").Value = ViewBag.IrFecha;
                            sheetOne.Cell("V10").Value = ViewBag.IrFecha;

                            ViewBag.TCAnio = ((ini.TCAnio == null || string.IsNullOrEmpty(ini.TCAnio.ToString())) ? null : double.Parse(ini.TCAnio.ToString().Replace('.', ','), ciCL));
                            if (ViewBag.TCAnio != null)
                            {
                                sheetOne.Cell("L44").Value = ViewBag.TCAnio;
                            }
                            sheetOne.Cell("L44").Style.NumberFormat.Format = "#,##0.00";

                            ViewBag.TCAnioMasUno = ((ini.TCAnioMasUno == null || string.IsNullOrEmpty(ini.TCAnioMasUno.ToString())) ? null : double.Parse(ini.TCAnioMasUno.ToString().Replace('.', ','), ciCL));
                            if (ViewBag.TCAnioMasUno != null)
                            {
                                sheetOne.Cell("M44").Value = ViewBag.TCAnioMasUno;
                            }
                            sheetOne.Cell("M44").Style.NumberFormat.Format = "#,##0.00";

                            ViewBag.TCAnioMasDos = ((ini.TCAnioMasDos == null || string.IsNullOrEmpty(ini.TCAnioMasDos.ToString())) ? null : double.Parse(ini.TCAnioMasDos.ToString().Replace('.', ','), ciCL));
                            if (ViewBag.TCAnioMasDos != null)
                            {
                                sheetOne.Cell("N44").Value = ViewBag.TCAnioMasUno;
                            }
                            sheetOne.Cell("N44").Style.NumberFormat.Format = "#,##0.00";

                            ViewBag.TCAnioMasTres = ((ini.TCAnioMasTres == null || string.IsNullOrEmpty(ini.TCAnioMasTres.ToString())) ? null : double.Parse(ini.TCAnioMasTres.ToString().Replace('.', ','), ciCL));
                            if (ViewBag.TCAnioMasTres != null)
                            {
                                sheetOne.Cell("O44").Value = ViewBag.TCAnioMasTres;
                            }
                            sheetOne.Cell("O44").Style.NumberFormat.Format = "#,##0.00";

                            ViewBag.IPCAnio = ((ini.IPCAnio == null || string.IsNullOrEmpty(ini.IPCAnio.ToString())) ? null : double.Parse(ini.IPCAnio.ToString().Replace('.', ','), ciCL));
                            if (ViewBag.IPCAnio != null)
                            {
                                sheetOne.Cell("L45").Value = ViewBag.IPCAnio;
                            }
                            sheetOne.Cell("L45").Style.NumberFormat.Format = "#,##0.00";

                            ViewBag.IPCAnioMasUno = ((ini.IPCAnioMasUno == null || string.IsNullOrEmpty(ini.IPCAnioMasUno.ToString())) ? null : double.Parse(ini.IPCAnioMasUno.ToString().Replace('.', ','), ciCL));
                            if (ViewBag.IPCAnioMasUno != null)
                            {
                                sheetOne.Cell("M45").Value = ViewBag.IPCAnioMasUno;
                            }
                            sheetOne.Cell("M45").Style.NumberFormat.Format = "#,##0.00";

                            ViewBag.IPCAnioMasDos = ((ini.IPCAnioMasDos == null || string.IsNullOrEmpty(ini.IPCAnioMasDos.ToString())) ? null : double.Parse(ini.IPCAnioMasDos.ToString().Replace('.', ','), ciCL));
                            if (ViewBag.IPCAnioMasDos != null)
                            {
                                sheetOne.Cell("N45").Value = ViewBag.IPCAnioMasDos;
                            }
                            sheetOne.Cell("N45").Style.NumberFormat.Format = "#,##0.00";

                            ViewBag.IPCAnioMasTres = ((ini.IPCAnioMasTres == null || string.IsNullOrEmpty(ini.IPCAnioMasTres.ToString())) ? null : double.Parse(ini.IPCAnioMasTres.ToString().Replace('.', ','), ciCL));
                            if (ViewBag.IPCAnioMasTres != null)
                            {
                                sheetOne.Cell("O45").Value = ViewBag.IPCAnioMasTres;
                            }
                            sheetOne.Cell("O45").Style.NumberFormat.Format = "#,##0.00";

                            ViewBag.CPIAnio = ((ini.CPIAnio == null || string.IsNullOrEmpty(ini.CPIAnio.ToString())) ? null : double.Parse(ini.CPIAnio.ToString().Replace('.', ','), ciCL));
                            if (ViewBag.CPIAnio != null)
                            {
                                sheetOne.Cell("L46").Value = ViewBag.CPIAnio;
                            }
                            sheetOne.Cell("L46").Style.NumberFormat.Format = "#,##0.00";

                            ViewBag.CPIAnioMasUno = ((ini.CPIAnioMasUno == null || string.IsNullOrEmpty(ini.CPIAnioMasUno.ToString())) ? null : double.Parse(ini.CPIAnioMasUno.ToString().Replace('.', ','), ciCL));
                            if (ViewBag.CPIAnioMasUno != null)
                            {
                                sheetOne.Cell("M46").Value = ViewBag.CPIAnioMasUno;
                            }
                            sheetOne.Cell("M46").Style.NumberFormat.Format = "#,##0.00";

                            ViewBag.CPIAnioMasDos = ((ini.CPIAnioMasDos == null || string.IsNullOrEmpty(ini.CPIAnioMasDos.ToString())) ? null : double.Parse(ini.CPIAnioMasDos.ToString().Replace('.', ','), ciCL));
                            if (ViewBag.CPIAnioMasDos != null)
                            {
                                sheetOne.Cell("N46").Value = ViewBag.CPIAnioMasDos;
                            }
                            sheetOne.Cell("N46").Style.NumberFormat.Format = "#,##0.00";

                            ViewBag.CPIAnioMasTres = ((ini.CPIAnioMasTres == null || string.IsNullOrEmpty(ini.CPIAnioMasTres.ToString())) ? null : double.Parse(ini.CPIAnioMasTres.ToString().Replace('.', ','), ciCL));
                            if (ViewBag.CPIAnioMasTres != null)
                            {
                                sheetOne.Cell("O46").Value = ViewBag.CPIAnioMasTres;
                            }
                            sheetOne.Cell("O46").Style.NumberFormat.Format = "#,##0.00";

                            ViewBag.DESCRIPCION = ini.DESCRIPCION;
                            fileName = ini.DESCRIPCION + "-" + ini.IdPid + ".xlsx";
                        }
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                        }
                    }
                    catch (Exception err)
                    {
                        ExceptionResult = AppModule + "descargaExcelPresupuesto, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        Utils.LogError(ExceptionResult);
                        return null;
                    }
                    finally
                    {
                        objConnection.Close();
                        fstream.Close();
                    }
                }

            }
        }

        private string formatNumberPdf(String valueParam)
        {
            string responseValueParam = valueParam;
            double doubleValueParam = double.Parse(varEvaluacionEconomicaPdf(valueParam), CultureInfo.InvariantCulture);
            double thousand = 1000.00;
            if (compareDoubles(doubleValueParam, thousand) == 1 || compareDoubles(doubleValueParam, thousand) == 0)
            {
                //responseValueParam = String.Format("{0:0,000.00}", doubleValueParam).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.');
                responseValueParam = String.Format("{0:0,000.00}", doubleValueParam).ToString();
            }
            else
            {
                //responseValueParam = String.Format("{0:0.00}", doubleValueParam).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.');
                responseValueParam = String.Format("{0:0.00}", doubleValueParam).ToString();
            }
            return responseValueParam;
        }

        private int compareDoubles(double d1, double d2)
        {
            double epsilon = 1E-15;
            if (d1 - d2 > epsilon) return 1;
            if (d1 - d2 < epsilon) return -1;
            return 0;
        }

        private string varEvaluacionEconomicaPdf(string param)
        {
            string value = "0";
            if (param != null && !string.IsNullOrEmpty(param.Trim()))
            {
                if (param.IndexOf(".") > 0 && param.IndexOf(",") > 0)
                {
                    value = param.Replace(",", "");
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

        private DataTable getDataExcel(string token)
        {
            //Creating DataTable  

            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = todaysDate.Year;
            //Setiing Table Name  
            dt.TableName = "Resumen";
            //Add Columns  
            dt.Columns.Add("Id Iniciativa", typeof(int));
            dt.Columns.Add("Usuario", typeof(string));
            dt.Columns.Add("Estado Flujo", typeof(string));
            dt.Columns.Add("Codigo Iniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Tipo Ejercicio", typeof(string));
            dt.Columns.Add("Periodo", typeof(int));
            dt.Columns.Add("Fecha Inicio", typeof(string));
            dt.Columns.Add("Fecha Termino", typeof(string));
            dt.Columns.Add("Fecha Cierre", typeof(string));
            dt.Columns.Add("Proceso", typeof(string));
            dt.Columns.Add("Objeto", typeof(string));
            dt.Columns.Add("Area", typeof(string));
            dt.Columns.Add("Compania", typeof(string));
            dt.Columns.Add("Etapa", typeof(string));
            dt.Columns.Add("Codigo Proyecto", typeof(string));
            dt.Columns.Add("Gerencia Inversion (Cliente)", typeof(string));
            dt.Columns.Add("Gerente Inversion (Cliente)", typeof(string));
            dt.Columns.Add("Gerencia Ejecucion", typeof(string));
            dt.Columns.Add("Gerente Ejecucion", typeof(string));
            dt.Columns.Add("Superintendencia", typeof(string));
            dt.Columns.Add("Superintendente", typeof(string));
            dt.Columns.Add("Encargado Control SAP", typeof(string));
            dt.Columns.Add("Estado Proyecto", typeof(string));
            dt.Columns.Add("Tipo Cotizacion", typeof(string));
            dt.Columns.Add("Categoria", typeof(string));
            dt.Columns.Add("Nivel Ingenieria", typeof(string));
            dt.Columns.Add("Clasificacion SSO", typeof(string));
            dt.Columns.Add("Estandar Seguridad", typeof(string));
            dt.Columns.Add("Clase", typeof(string));
            dt.Columns.Add("MacroCategoria", typeof(string));
            dt.Columns.Add("Clas. Riesgo sin Proy.", typeof(string));
            dt.Columns.Add("MFL (KUS$)", typeof(double));
            dt.Columns.Add("Clas. Riesgo con Proy.", typeof(string));
            dt.Columns.Add("MPE (KUS$)", typeof(double));
            dt.Columns.Add("VAN (KUS$)", typeof(double));
            dt.Columns.Add("IVAN", typeof(double));
            dt.Columns.Add("PayBack", typeof(double));
            dt.Columns.Add("Vida Util (Años)", typeof(double));
            dt.Columns.Add("Costo Dueño", typeof(double));
            dt.Columns.Add("Contingencia", typeof(double));
            dt.Columns.Add("Per. Ant.", typeof(double));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Año" + (year + 1), typeof(double));
            dt.Columns.Add("Año " + (year + 2), typeof(double));
            dt.Columns.Add("Año " + (year + 3), typeof(double));
            dt.Columns.Add("Año " + (year + 4), typeof(double));
            dt.Columns.Add("Total Capex (Parcial)", typeof(double));


            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }

                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA", new { @usuario = usuario, @opcion = token }, commandType: CommandType.StoredProcedure).ToList();
                    foreach (var fila in contenido)
                    {
                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidEstadoFlujo, fila.PidCodigoIniciativa, fila.PidNombreProyecto,
                             fila.IniTipo, fila.IniPeriodo, fila.IgFechaInicio, fila.IgFechaTermino, fila.IgFechaCierre,
                             fila.PidProceso, fila.PidObjeto, fila.PidArea, fila.PidCompania, fila.PidEtapa, fila.PidCodigoProyecto, fila.PidGerenciaInversion,
                             fila.PidGerenteInversion, fila.PidGerenciaEjecucion, fila.PidGerenteEjecucion, fila.PidSuperintendencia, fila.PidSuperintendente,
                             fila.PidEncargadoControl, fila.CatEstadoProyecto, fila.CatTipoCotizacion, fila.CatCategoria, fila.CatNivelIngenieria,
                             fila.CatClasificacionSSO, fila.CatEstandarSeguridad, fila.CatClase, fila.CatMacroCategoria,
                             fila.EriClas1, fila.EriMFL1, fila.EriClas2, fila.EriMFL2, fila.EveVan, fila.EveIvan, fila.EvePayBack, fila.EveVidaUtil, fila.PorCostoDueno, fila.PorContingencia,
                             fila.TotalPeriodoAnterior, fila.EneroTotAcum, fila.FebreroTotAcum, fila.MarzoTotAcum, fila.AbrilTotAcum, fila.MayoTotAcum, fila.JunioTotAcum, fila.JulioTotAcum,
                             fila.AgostoTotAcum, fila.SeptiembreTotAcum, fila.OctubreTotAcum, fila.NoviembreTotAcum, fila.DiciembreTotAcum, fila.TotalTotAcum,
                             fila.PresAnioMasUnoTotAcum, fila.PresAnioMasDosTotAcum, fila.PresAnioMasTresTotAcum, 3000/*fila.TotalCapexTotAcum*/);

                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    //ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    //Utils.LogError(ExceptionResult);
                    //return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
            return dt;
        }

        private Presupuesto.ParametroOrientacionVN getParametroVN(string parametroVNToken)
        {
            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametroVN = SqlMapper.Query<Presupuesto.ParametroOrientacionVN>(objConnection, "CAPEX_GET_PARAMETRO_VN", new { ParametroVNToken = parametroVNToken }, commandType: CommandType.StoredProcedure).ToList();
                    ViewBag.Version = string.Empty;
                    if (parametroVN != null && parametroVN.Count > 0)
                    {
                        foreach (var parVN in parametroVN)
                        {
                            return parVN;
                        }
                    }
                }
                catch (Exception err)
                {
                    err.ToString();
                }
                finally
                {
                    objConnection.Close();
                }
                return null;
            }

        }

        private Hashtable getDataExcelEjerciciosOficialesFormat(string token)
        {
            //Creating DataTable  
            Hashtable hashtable = new Hashtable();
            List<string> numerosSeparadorMiles = new List<string>();
            List<string> numerosSeparadorMilesDecimales = new List<string>();
            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = todaysDate.Year;
            //Setiing Table Name  
            dt.TableName = "Resumen PP-EX";
            //Add Columns  
            dt.Columns.Add("Id Iniciativa", typeof(int));
            dt.Columns.Add("Usuario", typeof(string));
            dt.Columns.Add("Estado Flujo", typeof(string));
            dt.Columns.Add("Codigo Iniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Nombre Proyecto(Alias)", typeof(string));
            dt.Columns.Add("Tipo Ejercicio", typeof(string));
            dt.Columns.Add("Periodo", typeof(int));
            dt.Columns.Add("Fecha Inicio", typeof(string));
            dt.Columns.Add("Fecha Termino", typeof(string));
            dt.Columns.Add("Fecha Cierre", typeof(string));
            dt.Columns.Add("Proceso", typeof(string));
            dt.Columns.Add("Objeto", typeof(string));
            dt.Columns.Add("Area", typeof(string));
            dt.Columns.Add("Compania", typeof(string));
            dt.Columns.Add("Etapa", typeof(string));
            dt.Columns.Add("Codigo Proyecto", typeof(string));
            dt.Columns.Add("Gerencia Inversion (Cliente)", typeof(string));
            dt.Columns.Add("Gerente Inversion (Cliente)", typeof(string));
            dt.Columns.Add("Gerencia Ejecucion", typeof(string));
            dt.Columns.Add("Gerente Ejecucion", typeof(string));
            dt.Columns.Add("Superintendencia", typeof(string));
            dt.Columns.Add("Superintendente", typeof(string));
            dt.Columns.Add("Encargado Control SAP", typeof(string));
            dt.Columns.Add("Estado Proyecto", typeof(string));
            dt.Columns.Add("Tipo Cotizacion", typeof(string));
            dt.Columns.Add("Categoria", typeof(string));
            dt.Columns.Add("Nivel Ingenieria", typeof(string));
            dt.Columns.Add("Clasificacion SSO", typeof(string));
            dt.Columns.Add("Estandar Seguridad", typeof(string));
            dt.Columns.Add("Clase", typeof(string));
            dt.Columns.Add("MacroCategoria", typeof(string));
            dt.Columns.Add("Clas. Riesgo sin Proy.", typeof(string));
            dt.Columns.Add("MFL (KUS$)", typeof(double));
            dt.Columns.Add("Clas. Riesgo con Proy.", typeof(string));
            dt.Columns.Add("MPE (KUS$)", typeof(double));
            dt.Columns.Add("VAN (KUS$)", typeof(double));
            dt.Columns.Add("TIR", typeof(double));
            dt.Columns.Add("IVAN", typeof(double));
            dt.Columns.Add("PayBack", typeof(double));
            dt.Columns.Add("Vida Util (Años)", typeof(double));
            dt.Columns.Add("Costo Dueño", typeof(double));
            dt.Columns.Add("Contingencia", typeof(double));
            dt.Columns.Add("Per. Ant.", typeof(double));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Año" + (year + 1), typeof(double));
            dt.Columns.Add("Año " + (year + 2), typeof(double));
            dt.Columns.Add("Año " + (year + 3), typeof(double));
            dt.Columns.Add("Año " + (year + 4), typeof(double));
            dt.Columns.Add("Total Capex (Parcial)", typeof(double));
            dt.Columns.Add("Objetivo de la Inversión", typeof(string));
            dt.Columns.Add("Alcance /Descripción de la Inversión", typeof(string));


            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    /*if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }*/
                    usuario = "";
                    string periodo = ((Session["anioIniciativaEjercicioOficial"] != null && !string.IsNullOrEmpty(Convert.ToString(Session["anioIniciativaEjercicioOficial"]))) ? Convert.ToString(Session["anioIniciativaEjercicioOficial"]) : "0");
                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA_EJERCICIOS_OFICIALES", new { @usuario = usuario, @opcion = token, @periodo = periodo }, commandType: CommandType.StoredProcedure).ToList();

                    var filaActual = 2;
                    foreach (var fila in contenido)
                    {
                        if (!esCero(fila.TotalCapexTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 61;
                            if (esNumeroEntero(fila.TotalCapexTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.PresAnioMasTresTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 60;
                            if (esNumeroEntero(fila.PresAnioMasTresTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.PresAnioMasDosTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 59;
                            if (esNumeroEntero(fila.PresAnioMasDosTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.PresAnioMasUnoTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 58;
                            if (esNumeroEntero(fila.PresAnioMasUnoTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.TotalTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 57;
                            if (esNumeroEntero(fila.TotalTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.DiciembreTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 56;
                            if (esNumeroEntero(fila.DiciembreTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.NoviembreTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 55;
                            if (esNumeroEntero(fila.NoviembreTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.OctubreTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 54;
                            if (esNumeroEntero(fila.OctubreTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.SeptiembreTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 53;
                            if (esNumeroEntero(fila.SeptiembreTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.AgostoTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 52;
                            if (esNumeroEntero(fila.AgostoTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.JulioTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 51;
                            if (esNumeroEntero(fila.JulioTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.JunioTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 50;
                            if (esNumeroEntero(fila.JunioTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.MayoTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 49;
                            if (esNumeroEntero(fila.MayoTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.AbrilTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 48;
                            if (esNumeroEntero(fila.AbrilTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.MarzoTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 47;
                            if (esNumeroEntero(fila.MarzoTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.FebreroTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 46;
                            if (esNumeroEntero(fila.FebreroTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.EneroTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 45;
                            if (esNumeroEntero(fila.EneroTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.TotalPeriodoAnterior.ToString()))
                        {
                            string filaColumna = filaActual + "," + 44;
                            if (esNumeroEntero(fila.TotalPeriodoAnterior.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidEstadoFlujo, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.PidNombreProyectoAlias,
                             fila.IniTipo, fila.IniPeriodo, fila.IgFechaInicio, fila.IgFechaTermino, fila.IgFechaCierre,
                             fila.PidProceso, fila.PidObjeto, fila.PidArea, fila.PidCompania, fila.PidEtapa, fila.PidCodigoProyecto, fila.PidGerenciaInversion,
                             fila.PidGerenteInversion, fila.PidGerenciaEjecucion, fila.PidGerenteEjecucion, fila.PidSuperintendencia, fila.PidSuperintendente,
                             fila.PidEncargadoControl, fila.CatEstadoProyecto, fila.CatTipoCotizacion, fila.CatCategoria, fila.CatNivelIngenieria,
                             fila.CatClasificacionSSO, fila.CatEstandarSeguridad, fila.CatClase, fila.CatMacroCategoria,
                             fila.EriClas1, fila.EriMFL1, fila.EriClas2, fila.EriMFL2, fila.EveVan, fila.EveTir, fila.EveIvan, fila.EvePayBack, fila.EveVidaUtil, fila.PorCostoDueno, fila.PorContingencia,
                             fila.TotalPeriodoAnterior, fila.EneroTotAcum, fila.FebreroTotAcum, fila.MarzoTotAcum, fila.AbrilTotAcum, fila.MayoTotAcum, fila.JunioTotAcum, fila.JulioTotAcum,
                             fila.AgostoTotAcum, fila.SeptiembreTotAcum, fila.OctubreTotAcum, fila.NoviembreTotAcum, fila.DiciembreTotAcum, fila.TotalTotAcum,
                             fila.PresAnioMasUnoTotAcum, fila.PresAnioMasDosTotAcum, fila.PresAnioMasTresTotAcum, fila.TotalCapexTotAcum, fila.ObjetivoInversion, fila.AlcanceInversion);
                        filaActual++;
                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    //ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    //Utils.LogError(ExceptionResult);
                    //return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
            hashtable.Add(1, dt);
            hashtable.Add(2, numerosSeparadorMiles);
            hashtable.Add(3, numerosSeparadorMilesDecimales);
            return hashtable;
        }

        private Hashtable getDataExcelFormatParametroVN(string tipoIniciativaSeleccionado, string anioIniciativaSeleccionado, string parametroVN)
        {
            //Creating DataTable  
            Hashtable hashtable = new Hashtable();
            List<string> numerosSeparadorMiles = new List<string>();
            List<string> numerosSeparadorMilesDecimales = new List<string>();
            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = todaysDate.Year;
            //Setiing Table Name  
            dt.TableName = "Resumen PP-EX";
            //Add Columns  
            dt.Columns.Add("Id Iniciativa", typeof(int));
            dt.Columns.Add("Usuario", typeof(string));
            dt.Columns.Add("Estado Flujo", typeof(string));
            dt.Columns.Add("Codigo Iniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Nombre Proyecto(Alias)", typeof(string));
            dt.Columns.Add("Tipo Ejercicio", typeof(string));
            dt.Columns.Add("Periodo", typeof(int));
            dt.Columns.Add("Fecha Inicio", typeof(string));
            dt.Columns.Add("Fecha Termino", typeof(string));
            dt.Columns.Add("Fecha Cierre", typeof(string));
            dt.Columns.Add("Proceso", typeof(string));
            dt.Columns.Add("Objeto", typeof(string));
            dt.Columns.Add("Area", typeof(string));
            dt.Columns.Add("Compania", typeof(string));
            dt.Columns.Add("Etapa", typeof(string));
            dt.Columns.Add("Codigo Proyecto", typeof(string));
            dt.Columns.Add("Gerencia Inversion (Cliente)", typeof(string));
            dt.Columns.Add("Gerente Inversion (Cliente)", typeof(string));
            dt.Columns.Add("Gerencia Ejecucion", typeof(string));
            dt.Columns.Add("Gerente Ejecucion", typeof(string));
            dt.Columns.Add("Superintendencia", typeof(string));
            dt.Columns.Add("Superintendente", typeof(string));
            dt.Columns.Add("Encargado Control SAP", typeof(string));
            dt.Columns.Add("Estado Proyecto", typeof(string));
            dt.Columns.Add("Tipo Cotizacion", typeof(string));
            dt.Columns.Add("Categoria", typeof(string));
            dt.Columns.Add("Nivel Ingenieria", typeof(string));
            dt.Columns.Add("Clasificacion SSO", typeof(string));
            dt.Columns.Add("Estandar Seguridad", typeof(string));
            dt.Columns.Add("Clase", typeof(string));
            dt.Columns.Add("MacroCategoria", typeof(string));
            dt.Columns.Add("Clas. Riesgo sin Proy.", typeof(string));
            dt.Columns.Add("MFL (KUS$)", typeof(double));
            dt.Columns.Add("Clas. Riesgo con Proy.", typeof(string));
            dt.Columns.Add("MPE (KUS$)", typeof(double));
            dt.Columns.Add("VAN (KUS$)", typeof(double));
            dt.Columns.Add("TIR", typeof(double));
            dt.Columns.Add("IVAN", typeof(double));
            dt.Columns.Add("PayBack", typeof(double));
            dt.Columns.Add("Vida Util (Años)", typeof(double));
            dt.Columns.Add("Costo Dueño", typeof(double));
            dt.Columns.Add("Contingencia", typeof(double));
            dt.Columns.Add("Per. Ant.", typeof(double));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Año" + (year + 1), typeof(double));
            dt.Columns.Add("Año " + (year + 2), typeof(double));
            dt.Columns.Add("Año " + (year + 3), typeof(double));
            dt.Columns.Add("Año " + (year + 4), typeof(double));
            dt.Columns.Add("Total Capex (Parcial)", typeof(double));

            dt.Columns.Add("% Inversion Nacional", typeof(double));
            dt.Columns.Add("% Inversion Extranjera", typeof(double));
            dt.Columns.Add("Riesgo Clave", typeof(string));

            dt.Columns.Add("Objetivo de la Inversión", typeof(string));
            dt.Columns.Add("Alcance /Descripción de la Inversión", typeof(string));

            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }
                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA_PARAMETROVN", new { @periodo = anioIniciativaSeleccionado, @parametroVN = parametroVN }, commandType: CommandType.StoredProcedure).ToList();
                    var filaActual = 2;
                    var ultimoIdPid = 0;
                    foreach (var fila in contenido)
                    {
                        if (ultimoIdPid == fila.IdPid)
                        {
                            continue;
                        }
                        if (!esCero(fila.TotalCapexTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 61;
                            if (esNumeroEntero(fila.TotalCapexTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.PresAnioMasTresTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 60;
                            if (esNumeroEntero(fila.PresAnioMasTresTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.PresAnioMasDosTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 59;
                            if (esNumeroEntero(fila.PresAnioMasDosTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.PresAnioMasUnoTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 58;
                            if (esNumeroEntero(fila.PresAnioMasUnoTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.TotalTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 57;
                            if (esNumeroEntero(fila.TotalTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.DiciembreTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 56;
                            if (esNumeroEntero(fila.DiciembreTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.NoviembreTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 55;
                            if (esNumeroEntero(fila.NoviembreTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.OctubreTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 54;
                            if (esNumeroEntero(fila.OctubreTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.SeptiembreTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 53;
                            if (esNumeroEntero(fila.SeptiembreTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.AgostoTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 52;
                            if (esNumeroEntero(fila.AgostoTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.JulioTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 51;
                            if (esNumeroEntero(fila.JulioTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.JunioTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 50;
                            if (esNumeroEntero(fila.JunioTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.MayoTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 49;
                            if (esNumeroEntero(fila.MayoTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.AbrilTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 48;
                            if (esNumeroEntero(fila.AbrilTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.MarzoTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 47;
                            if (esNumeroEntero(fila.MarzoTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.FebreroTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 46;
                            if (esNumeroEntero(fila.FebreroTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.EneroTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 45;
                            if (esNumeroEntero(fila.EneroTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.TotalPeriodoAnterior.ToString()))
                        {
                            string filaColumna = filaActual + "," + 44;
                            if (esNumeroEntero(fila.TotalPeriodoAnterior.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        ultimoIdPid = fila.IdPid;
                        double inversionNacional = 0.0;
                        double inversionExtranjera = 0.0;
                        if (fila.HitNacExt != null && !string.IsNullOrEmpty(fila.HitNacExt.ToString()))
                        {
                            string[] hitNacExt = fila.HitNacExt.ToString().Split('/');
                            if (hitNacExt.Length == 2)
                            {
                                CultureInfo ciCL = new CultureInfo("es-CL", false);
                                inversionNacional = double.Parse(hitNacExt[0], ciCL);
                                inversionExtranjera = double.Parse(hitNacExt[1], ciCL);
                            }
                        }
                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidEstadoFlujo, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.PidNombreProyectoAlias,
                             fila.IniTipo, fila.IniPeriodo, fila.IgFechaInicio, fila.IgFechaTermino, fila.IgFechaCierre,
                             fila.PidProceso, fila.PidObjeto, fila.PidArea, fila.PidCompania, fila.PidEtapa, fila.PidCodigoProyecto, fila.PidGerenciaInversion,
                             fila.PidGerenteInversion, fila.PidGerenciaEjecucion, fila.PidGerenteEjecucion, fila.PidSuperintendencia, fila.PidSuperintendente,
                             fila.PidEncargadoControl, fila.CatEstadoProyecto, fila.CatTipoCotizacion, fila.CatCategoria, fila.CatNivelIngenieria,
                             fila.CatClasificacionSSO, fila.CatEstandarSeguridad, fila.CatClase, fila.CatMacroCategoria,
                             fila.EriClas1, fila.EriMFL1, fila.EriClas2, fila.EriMFL2, fila.EveVan, fila.EveTir, fila.EveIvan, fila.EvePayBack, fila.EveVidaUtil, fila.PorCostoDueno, fila.PorContingencia,
                             fila.TotalPeriodoAnterior, fila.EneroTotAcum, fila.FebreroTotAcum, fila.MarzoTotAcum, fila.AbrilTotAcum, fila.MayoTotAcum, fila.JunioTotAcum, fila.JulioTotAcum,
                             fila.AgostoTotAcum, fila.SeptiembreTotAcum, fila.OctubreTotAcum, fila.NoviembreTotAcum, fila.DiciembreTotAcum, fila.TotalTotAcum,
                             fila.PresAnioMasUnoTotAcum, fila.PresAnioMasDosTotAcum, fila.PresAnioMasTresTotAcum, fila.TotalCapexTotAcum, inversionNacional, inversionExtranjera, ((fila.MatrizRiesgoNombre != null) ? fila.MatrizRiesgoNombre.ToString().Trim() : ""), fila.ObjetivoInversion, fila.AlcanceInversion);
                        filaActual++;
                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    err.ToString();
                    //ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    //Utils.LogError(ExceptionResult);
                    //return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
            hashtable.Add(1, dt);
            hashtable.Add(2, numerosSeparadorMiles);
            hashtable.Add(3, numerosSeparadorMilesDecimales);
            return hashtable;
        }

        private Hashtable getDataExcelFormat(string token)
        {
            //Creating DataTable  
            Hashtable hashtable = new Hashtable();
            List<string> numerosSeparadorMiles = new List<string>();
            List<string> numerosSeparadorMilesDecimales = new List<string>();
            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = todaysDate.Year;
            //Setiing Table Name  
            dt.TableName = "Resumen PP-EX";
            //Add Columns  
            dt.Columns.Add("Id Iniciativa", typeof(int));
            dt.Columns.Add("Usuario", typeof(string));
            dt.Columns.Add("Estado Flujo", typeof(string));
            dt.Columns.Add("Codigo Iniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Nombre Proyecto(Alias)", typeof(string));
            dt.Columns.Add("Tipo Ejercicio", typeof(string));
            dt.Columns.Add("Periodo", typeof(int));
            dt.Columns.Add("Fecha Inicio", typeof(string));
            dt.Columns.Add("Fecha Termino", typeof(string));
            dt.Columns.Add("Fecha Cierre", typeof(string));
            dt.Columns.Add("Proceso", typeof(string));
            dt.Columns.Add("Objeto", typeof(string));
            dt.Columns.Add("Area", typeof(string));
            dt.Columns.Add("Compania", typeof(string));
            dt.Columns.Add("Etapa", typeof(string));
            dt.Columns.Add("Codigo Proyecto", typeof(string));
            dt.Columns.Add("Gerencia Inversion (Cliente)", typeof(string));
            dt.Columns.Add("Gerente Inversion (Cliente)", typeof(string));
            dt.Columns.Add("Gerencia Ejecucion", typeof(string));
            dt.Columns.Add("Gerente Ejecucion", typeof(string));
            dt.Columns.Add("Superintendencia", typeof(string));
            dt.Columns.Add("Superintendente", typeof(string));
            dt.Columns.Add("Encargado Control SAP", typeof(string));
            dt.Columns.Add("Estado Proyecto", typeof(string));
            dt.Columns.Add("Tipo Cotizacion", typeof(string));
            dt.Columns.Add("Categoria", typeof(string));
            dt.Columns.Add("Nivel Ingenieria", typeof(string));
            dt.Columns.Add("Clasificacion SSO", typeof(string));
            dt.Columns.Add("Estandar Seguridad", typeof(string));
            dt.Columns.Add("Clase", typeof(string));
            dt.Columns.Add("MacroCategoria", typeof(string));
            dt.Columns.Add("Clas. Riesgo sin Proy.", typeof(string));
            dt.Columns.Add("MFL (KUS$)", typeof(double));
            dt.Columns.Add("Clas. Riesgo con Proy.", typeof(string));
            dt.Columns.Add("MPE (KUS$)", typeof(double));
            dt.Columns.Add("VAN (KUS$)", typeof(double));
            dt.Columns.Add("TIR", typeof(double));
            dt.Columns.Add("IVAN", typeof(double));
            dt.Columns.Add("PayBack", typeof(double));
            dt.Columns.Add("Vida Util (Años)", typeof(double));
            dt.Columns.Add("Costo Dueño", typeof(double));
            dt.Columns.Add("Contingencia", typeof(double));
            dt.Columns.Add("Per. Ant.", typeof(double));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Año" + (year + 1), typeof(double));
            dt.Columns.Add("Año " + (year + 2), typeof(double));
            dt.Columns.Add("Año " + (year + 3), typeof(double));
            dt.Columns.Add("Año " + (year + 4), typeof(double));
            dt.Columns.Add("Total Capex (Parcial)", typeof(double));

            dt.Columns.Add("% Inversion Nacional", typeof(double));
            dt.Columns.Add("% Inversion Extranjera", typeof(double));
            dt.Columns.Add("Riesgo Clave", typeof(string));

            dt.Columns.Add("Objetivo de la Inversión", typeof(string));
            dt.Columns.Add("Alcance /Descripción de la Inversión", typeof(string));

            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }
                    var tipoIniciativaSeleccionado = Convert.ToString(Session["tipoIniciativaSeleccionado"]);
                    if (string.IsNullOrEmpty(tipoIniciativaSeleccionado))
                    {
                        tipoIniciativaSeleccionado = "0";
                    }
                    var anioIniciativaSeleccionado = Convert.ToString(Session["anioIniciativaSeleccionado"]);
                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA", new { @usuario = usuario, @opcion = token, @periodo = anioIniciativaSeleccionado }, commandType: CommandType.StoredProcedure).ToList();
                    var filaActual = 2;
                    var ultimoIdPid = 0;
                    foreach (var fila in contenido)
                    {
                        if (ultimoIdPid == fila.IdPid)
                        {
                            continue;
                        }
                        if (!esCero(fila.TotalCapexTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 61;
                            if (esNumeroEntero(fila.TotalCapexTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.PresAnioMasTresTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 60;
                            if (esNumeroEntero(fila.PresAnioMasTresTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.PresAnioMasDosTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 59;
                            if (esNumeroEntero(fila.PresAnioMasDosTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.PresAnioMasUnoTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 58;
                            if (esNumeroEntero(fila.PresAnioMasUnoTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.TotalTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 57;
                            if (esNumeroEntero(fila.TotalTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.DiciembreTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 56;
                            if (esNumeroEntero(fila.DiciembreTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.NoviembreTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 55;
                            if (esNumeroEntero(fila.NoviembreTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.OctubreTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 54;
                            if (esNumeroEntero(fila.OctubreTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.SeptiembreTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 53;
                            if (esNumeroEntero(fila.SeptiembreTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.AgostoTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 52;
                            if (esNumeroEntero(fila.AgostoTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.JulioTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 51;
                            if (esNumeroEntero(fila.JulioTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.JunioTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 50;
                            if (esNumeroEntero(fila.JunioTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.MayoTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 49;
                            if (esNumeroEntero(fila.MayoTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.AbrilTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 48;
                            if (esNumeroEntero(fila.AbrilTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.MarzoTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 47;
                            if (esNumeroEntero(fila.MarzoTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.FebreroTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 46;
                            if (esNumeroEntero(fila.FebreroTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.EneroTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 45;
                            if (esNumeroEntero(fila.EneroTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.TotalPeriodoAnterior.ToString()))
                        {
                            string filaColumna = filaActual + "," + 44;
                            if (esNumeroEntero(fila.TotalPeriodoAnterior.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        ultimoIdPid = fila.IdPid;
                        double inversionNacional = 0.0;
                        double inversionExtranjera = 0.0;
                        if (fila.HitNacExt != null && !string.IsNullOrEmpty(fila.HitNacExt.ToString()))
                        {
                            string[] hitNacExt = fila.HitNacExt.ToString().Split('/');
                            if (hitNacExt.Length == 2)
                            {
                                CultureInfo ciCL = new CultureInfo("es-CL", false);
                                inversionNacional = double.Parse(hitNacExt[0], ciCL);
                                inversionExtranjera = double.Parse(hitNacExt[1], ciCL);
                            }
                        }
                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidEstadoFlujo, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.PidNombreProyectoAlias,
                             fila.IniTipo, fila.IniPeriodo, fila.IgFechaInicio, fila.IgFechaTermino, fila.IgFechaCierre,
                             fila.PidProceso, fila.PidObjeto, fila.PidArea, fila.PidCompania, fila.PidEtapa, fila.PidCodigoProyecto, fila.PidGerenciaInversion,
                             fila.PidGerenteInversion, fila.PidGerenciaEjecucion, fila.PidGerenteEjecucion, fila.PidSuperintendencia, fila.PidSuperintendente,
                             fila.PidEncargadoControl, fila.CatEstadoProyecto, fila.CatTipoCotizacion, fila.CatCategoria, fila.CatNivelIngenieria,
                             fila.CatClasificacionSSO, fila.CatEstandarSeguridad, fila.CatClase, fila.CatMacroCategoria,
                             fila.EriClas1, fila.EriMFL1, fila.EriClas2, fila.EriMFL2, fila.EveVan, fila.EveTir, fila.EveIvan, fila.EvePayBack, fila.EveVidaUtil, fila.PorCostoDueno, fila.PorContingencia,
                             fila.TotalPeriodoAnterior, fila.EneroTotAcum, fila.FebreroTotAcum, fila.MarzoTotAcum, fila.AbrilTotAcum, fila.MayoTotAcum, fila.JunioTotAcum, fila.JulioTotAcum,
                             fila.AgostoTotAcum, fila.SeptiembreTotAcum, fila.OctubreTotAcum, fila.NoviembreTotAcum, fila.DiciembreTotAcum, fila.TotalTotAcum,
                             fila.PresAnioMasUnoTotAcum, fila.PresAnioMasDosTotAcum, fila.PresAnioMasTresTotAcum, fila.TotalCapexTotAcum, inversionNacional, inversionExtranjera, ((fila.MatrizRiesgoNombre != null) ? fila.MatrizRiesgoNombre.ToString().Trim() : ""), fila.ObjetivoInversion, fila.AlcanceInversion);
                        filaActual++;
                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    //ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    //Utils.LogError(ExceptionResult);
                    //return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
            hashtable.Add(1, dt);
            hashtable.Add(2, numerosSeparadorMiles);
            hashtable.Add(3, numerosSeparadorMilesDecimales);
            return hashtable;
        }


        private Hashtable getDataExcelCasoBaseFormatParametroVN(string tipoIniciativaSeleccionado, string anioIniciativaSeleccionado, string parametroVN)
        {
            //Creating DataTable  
            Hashtable hashtable = new Hashtable();
            List<string> numerosSeparadorMiles = new List<string>();
            List<string> numerosSeparadorMilesDecimales = new List<string>();
            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = (todaysDate.Year + 1);
            //Setiing Table Name  
            dt.TableName = "Resumen CB-CD";
            //Add Columns  
            dt.Columns.Add("Id Iniciativa", typeof(int));
            dt.Columns.Add("Usuario", typeof(string));
            dt.Columns.Add("Estado Flujo", typeof(string));
            dt.Columns.Add("Codigo Iniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Nombre Proyecto(Alias)", typeof(string));
            dt.Columns.Add("Tipo Ejercicio", typeof(string));
            dt.Columns.Add("Periodo", typeof(int));
            dt.Columns.Add("Fecha Inicio", typeof(string));
            dt.Columns.Add("Fecha Termino", typeof(string));
            dt.Columns.Add("Fecha Cierre", typeof(string));
            dt.Columns.Add("Proceso", typeof(string));
            dt.Columns.Add("Objeto", typeof(string));
            dt.Columns.Add("Area", typeof(string));
            dt.Columns.Add("Compania", typeof(string));
            dt.Columns.Add("Etapa", typeof(string));
            dt.Columns.Add("Codigo Proyecto", typeof(string));
            dt.Columns.Add("Gerencia Inversion (Cliente)", typeof(string));
            dt.Columns.Add("Gerente Inversion (Cliente)", typeof(string));
            dt.Columns.Add("Gerencia Ejecucion", typeof(string));
            dt.Columns.Add("Gerente Ejecucion", typeof(string));
            dt.Columns.Add("Superintendencia", typeof(string));
            dt.Columns.Add("Superintendente", typeof(string));
            dt.Columns.Add("Encargado Control SAP", typeof(string));
            dt.Columns.Add("Estado Proyecto", typeof(string));
            dt.Columns.Add("Tipo Cotizacion", typeof(string));
            dt.Columns.Add("Categoria", typeof(string));
            dt.Columns.Add("Nivel Ingenieria", typeof(string));
            dt.Columns.Add("Clasificacion SSO", typeof(string));
            dt.Columns.Add("Estandar Seguridad", typeof(string));
            dt.Columns.Add("Clase", typeof(string));
            dt.Columns.Add("MacroCategoria", typeof(string));
            dt.Columns.Add("Clas. Riesgo sin Proy.", typeof(string));
            dt.Columns.Add("MFL (KUS$)", typeof(double));
            dt.Columns.Add("Clas. Riesgo con Proy.", typeof(string));
            dt.Columns.Add("MPE (KUS$)", typeof(double));
            dt.Columns.Add("VAN (KUS$)", typeof(double));
            dt.Columns.Add("TIR", typeof(double));
            dt.Columns.Add("IVAN", typeof(double));
            dt.Columns.Add("PayBack", typeof(double));
            dt.Columns.Add("Vida Util (Años)", typeof(double));
            dt.Columns.Add("Costo Dueño", typeof(double));
            dt.Columns.Add("Contingencia", typeof(double));
            dt.Columns.Add("Per. Ant.", typeof(double));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Año" + (year), typeof(double));
            dt.Columns.Add("Año " + (year + 1), typeof(double));
            dt.Columns.Add("Año " + (year + 2), typeof(double));
            dt.Columns.Add("Año " + (year + 3), typeof(double));
            dt.Columns.Add("Año " + (year + 4), typeof(double));
            dt.Columns.Add("Año " + (year + 5), typeof(double));
            dt.Columns.Add("Año " + (year + 6), typeof(double));
            dt.Columns.Add("Año " + (year + 7), typeof(double));
            dt.Columns.Add("Año " + (year + 8), typeof(double));
            dt.Columns.Add("Año " + (year + 9), typeof(double));
            dt.Columns.Add("Año " + (year + 10), typeof(double));
            dt.Columns.Add("Año " + (year + 11), typeof(double));
            dt.Columns.Add("Año " + (year + 12), typeof(double));
            dt.Columns.Add("Año " + (year + 13), typeof(double));
            dt.Columns.Add("Año " + (year + 14), typeof(double));
            dt.Columns.Add("Año " + (year + 15), typeof(double));
            dt.Columns.Add("Año " + (year + 16), typeof(double));
            dt.Columns.Add("Año " + (year + 17), typeof(double));
            dt.Columns.Add("Año " + (year + 18), typeof(double));
            dt.Columns.Add("Año " + (year + 19), typeof(double));
            dt.Columns.Add("Año " + (year + 20), typeof(double));
            dt.Columns.Add("Año " + (year + 21), typeof(double));
            dt.Columns.Add("Año " + (year + 22), typeof(double));
            dt.Columns.Add("Año " + (year + 23), typeof(double));
            dt.Columns.Add("Año " + (year + 24), typeof(double));
            dt.Columns.Add("Año " + (year + 25), typeof(double));
            dt.Columns.Add("Año " + (year + 26), typeof(double));
            dt.Columns.Add("Año " + (year + 27), typeof(double));
            dt.Columns.Add("Año " + (year + 28), typeof(double));
            dt.Columns.Add("Año " + (year + 29), typeof(double));
            dt.Columns.Add("Año " + (year + 30), typeof(double));
            dt.Columns.Add("Año " + (year + 31), typeof(double));
            dt.Columns.Add("Año " + (year + 32), typeof(double));
            dt.Columns.Add("Año " + (year + 33), typeof(double));
            dt.Columns.Add("Año " + (year + 34), typeof(double));
            dt.Columns.Add("Año " + (year + 35), typeof(double));
            dt.Columns.Add("Año " + (year + 36), typeof(double));
            dt.Columns.Add("Año " + (year + 37), typeof(double));
            dt.Columns.Add("Año " + (year + 38), typeof(double));
            dt.Columns.Add("Año " + (year + 39), typeof(double));
            dt.Columns.Add("Año " + (year + 40), typeof(double));
            dt.Columns.Add("Año " + (year + 41), typeof(double));
            dt.Columns.Add("Año " + (year + 42), typeof(double));
            dt.Columns.Add("Año " + (year + 43), typeof(double));
            dt.Columns.Add("Año " + (year + 44), typeof(double));
            dt.Columns.Add("Año " + (year + 45), typeof(double));
            dt.Columns.Add("Año " + (year + 46), typeof(double));
            dt.Columns.Add("Año " + (year + 47), typeof(double));
            dt.Columns.Add("Año " + (year + 48), typeof(double));
            dt.Columns.Add("Año " + (year + 49), typeof(double));
            dt.Columns.Add("Año " + (year + 50), typeof(double));
            dt.Columns.Add("Año " + (year + 51), typeof(double));
            dt.Columns.Add("Año " + (year + 52), typeof(double));
            dt.Columns.Add("Año " + (year + 53), typeof(double));
            dt.Columns.Add("Año " + (year + 54), typeof(double));
            dt.Columns.Add("Año " + (year + 55), typeof(double));
            dt.Columns.Add("Año " + (year + 56), typeof(double));
            dt.Columns.Add("Año " + (year + 57), typeof(double));
            dt.Columns.Add("Año " + (year + 58), typeof(double));
            dt.Columns.Add("Año " + (year + 59), typeof(double));
            dt.Columns.Add("Año " + (year + 60), typeof(double));
            dt.Columns.Add("Año " + (year + 61), typeof(double));
            dt.Columns.Add("Año " + (year + 62), typeof(double));
            dt.Columns.Add("Año " + (year + 63), typeof(double));
            dt.Columns.Add("Año " + (year + 64), typeof(double));
            dt.Columns.Add("Año " + (year + 65), typeof(double));
            dt.Columns.Add("Año " + (year + 66), typeof(double));
            dt.Columns.Add("Año " + (year + 67), typeof(double));
            dt.Columns.Add("Año " + (year + 68), typeof(double));
            dt.Columns.Add("Año " + (year + 69), typeof(double));
            dt.Columns.Add("Año " + (year + 70), typeof(double));
            dt.Columns.Add("Año " + (year + 71), typeof(double));
            dt.Columns.Add("Año " + (year + 72), typeof(double));
            dt.Columns.Add("Año " + (year + 73), typeof(double));
            dt.Columns.Add("Año " + (year + 74), typeof(double));
            dt.Columns.Add("Año " + (year + 75), typeof(double));
            dt.Columns.Add("Año " + (year + 76), typeof(double));
            dt.Columns.Add("Año " + (year + 77), typeof(double));
            dt.Columns.Add("Año " + (year + 78), typeof(double));
            dt.Columns.Add("Año " + (year + 79), typeof(double));
            dt.Columns.Add("Año " + (year + 80), typeof(double));
            dt.Columns.Add("Total Capex (Parcial)", typeof(double));

            dt.Columns.Add("% Inversion Nacional", typeof(double));
            dt.Columns.Add("% Inversion Extranjera", typeof(double));
            dt.Columns.Add("Riesgo Clave", typeof(string));

            dt.Columns.Add("Objetivo de la Inversión", typeof(string));
            dt.Columns.Add("Alcance /Descripción de la Inversión", typeof(string));

            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }

                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA_CASO_BASE_PARAMETROVN", new { @periodo = anioIniciativaSeleccionado, @parametroVN = @parametroVN }, commandType: CommandType.StoredProcedure).ToList();

                    var filaActual = 2;
                    var ultimoIdPid = 0;
                    foreach (var fila in contenido)
                    {

                        if (ultimoIdPid == fila.IdPid)
                        {
                            continue;
                        }
                        if (!esCero(fila.TotalPeriodoAnterior.ToString()))
                        {
                            string filaColumna = filaActual + "," + 44;
                            if (esNumeroEntero(fila.TotalPeriodoAnterior.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.EneroTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 45;
                            if (esNumeroEntero(fila.EneroTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.FebreroTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 46;
                            if (esNumeroEntero(fila.FebreroTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.MarzoTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 47;
                            if (esNumeroEntero(fila.MarzoTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.AbrilTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 48;
                            if (esNumeroEntero(fila.AbrilTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.MayoTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 49;
                            if (esNumeroEntero(fila.MayoTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.JunioTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 50;
                            if (esNumeroEntero(fila.JunioTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.JulioTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 51;
                            if (esNumeroEntero(fila.JulioTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.AgostoTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 52;
                            if (esNumeroEntero(fila.AgostoTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.SeptiembreTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 53;
                            if (esNumeroEntero(fila.SeptiembreTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.OctubreTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 54;
                            if (esNumeroEntero(fila.OctubreTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.NoviembreTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 55;
                            if (esNumeroEntero(fila.NoviembreTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.DiciembreTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 56;
                            if (esNumeroEntero(fila.DiciembreTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.TotalTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 57;
                            if (esNumeroEntero(fila.TotalTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.PresAnioMasUnoTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 58;
                            if (esNumeroEntero(fila.PresAnioMasUnoTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.PresAnioMasDosTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 59;
                            if (esNumeroEntero(fila.PresAnioMasDosTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.PresAnioMasTresTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 60;
                            if (esNumeroEntero(fila.PresAnioMasTresTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }

                        if (!esCero(fila.Ano4.ToString()))
                        {
                            string filaColumna = filaActual + "," + 61;
                            if (esNumeroEntero(fila.Ano4.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano5.ToString()))
                        {
                            string filaColumna = filaActual + "," + 62;
                            if (esNumeroEntero(fila.Ano5.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano6.ToString()))
                        {
                            string filaColumna = filaActual + "," + 63;
                            if (esNumeroEntero(fila.Ano6.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano7.ToString()))
                        {
                            string filaColumna = filaActual + "," + 64;
                            if (esNumeroEntero(fila.Ano7.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano8.ToString()))
                        {
                            string filaColumna = filaActual + "," + 65;
                            if (esNumeroEntero(fila.Ano8.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano9.ToString()))
                        {
                            string filaColumna = filaActual + "," + 66;
                            if (esNumeroEntero(fila.Ano9.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano10.ToString()))
                        {
                            string filaColumna = filaActual + "," + 67;
                            if (esNumeroEntero(fila.Ano10.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano11.ToString()))
                        {
                            string filaColumna = filaActual + "," + 68;
                            if (esNumeroEntero(fila.Ano11.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano12.ToString()))
                        {
                            string filaColumna = filaActual + "," + 69;
                            if (esNumeroEntero(fila.Ano12.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano13.ToString()))
                        {
                            string filaColumna = filaActual + "," + 70;
                            if (esNumeroEntero(fila.Ano13.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano14.ToString()))
                        {
                            string filaColumna = filaActual + "," + 71;
                            if (esNumeroEntero(fila.Ano14.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano15.ToString()))
                        {
                            string filaColumna = filaActual + "," + 72;
                            if (esNumeroEntero(fila.Ano15.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano16.ToString()))
                        {
                            string filaColumna = filaActual + "," + 73;
                            if (esNumeroEntero(fila.Ano16.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano17.ToString()))
                        {
                            string filaColumna = filaActual + "," + 74;
                            if (esNumeroEntero(fila.Ano17.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano18.ToString()))
                        {
                            string filaColumna = filaActual + "," + 75;
                            if (esNumeroEntero(fila.Ano18.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano19.ToString()))
                        {
                            string filaColumna = filaActual + "," + 76;
                            if (esNumeroEntero(fila.Ano19.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano20.ToString()))
                        {
                            string filaColumna = filaActual + "," + 77;
                            if (esNumeroEntero(fila.Ano20.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }


                        if (!esCero(fila.Ano21.ToString()))
                        {
                            string filaColumna = filaActual + "," + 78;
                            if (esNumeroEntero(fila.Ano21.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano22.ToString()))
                        {
                            string filaColumna = filaActual + "," + 79;
                            if (esNumeroEntero(fila.Ano22.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano23.ToString()))
                        {
                            string filaColumna = filaActual + "," + 80;
                            if (esNumeroEntero(fila.Ano23.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano24.ToString()))
                        {
                            string filaColumna = filaActual + "," + 81;
                            if (esNumeroEntero(fila.Ano24.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano25.ToString()))
                        {
                            string filaColumna = filaActual + "," + 82;
                            if (esNumeroEntero(fila.Ano25.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano26.ToString()))
                        {
                            string filaColumna = filaActual + "," + 83;
                            if (esNumeroEntero(fila.Ano26.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano27.ToString()))
                        {
                            string filaColumna = filaActual + "," + 84;
                            if (esNumeroEntero(fila.Ano27.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano28.ToString()))
                        {
                            string filaColumna = filaActual + "," + 85;
                            if (esNumeroEntero(fila.Ano28.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano29.ToString()))
                        {
                            string filaColumna = filaActual + "," + 86;
                            if (esNumeroEntero(fila.Ano29.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano30.ToString()))
                        {
                            string filaColumna = filaActual + "," + 87;
                            if (esNumeroEntero(fila.Ano30.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano31.ToString()))
                        {
                            string filaColumna = filaActual + "," + 88;
                            if (esNumeroEntero(fila.Ano31.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano32.ToString()))
                        {
                            string filaColumna = filaActual + "," + 89;
                            if (esNumeroEntero(fila.Ano32.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano33.ToString()))
                        {
                            string filaColumna = filaActual + "," + 90;
                            if (esNumeroEntero(fila.Ano33.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano34.ToString()))
                        {
                            string filaColumna = filaActual + "," + 91;
                            if (esNumeroEntero(fila.Ano34.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano35.ToString()))
                        {
                            string filaColumna = filaActual + "," + 92;
                            if (esNumeroEntero(fila.Ano35.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano36.ToString()))
                        {
                            string filaColumna = filaActual + "," + 93;
                            if (esNumeroEntero(fila.Ano36.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano37.ToString()))
                        {
                            string filaColumna = filaActual + "," + 94;
                            if (esNumeroEntero(fila.Ano37.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano38.ToString()))
                        {
                            string filaColumna = filaActual + "," + 95;
                            if (esNumeroEntero(fila.Ano38.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano39.ToString()))
                        {
                            string filaColumna = filaActual + "," + 96;
                            if (esNumeroEntero(fila.Ano39.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano40.ToString()))
                        {
                            string filaColumna = filaActual + "," + 97;
                            if (esNumeroEntero(fila.Ano40.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano41.ToString()))
                        {
                            string filaColumna = filaActual + "," + 98;
                            if (esNumeroEntero(fila.Ano41.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano42.ToString()))
                        {
                            string filaColumna = filaActual + "," + 99;
                            if (esNumeroEntero(fila.Ano42.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano43.ToString()))
                        {
                            string filaColumna = filaActual + "," + 100;
                            if (esNumeroEntero(fila.Ano43.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano44.ToString()))
                        {
                            string filaColumna = filaActual + "," + 101;
                            if (esNumeroEntero(fila.Ano44.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano45.ToString()))
                        {
                            string filaColumna = filaActual + "," + 102;
                            if (esNumeroEntero(fila.Ano45.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano46.ToString()))
                        {
                            string filaColumna = filaActual + "," + 103;
                            if (esNumeroEntero(fila.Ano46.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano47.ToString()))
                        {
                            string filaColumna = filaActual + "," + 104;
                            if (esNumeroEntero(fila.Ano47.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano48.ToString()))
                        {
                            string filaColumna = filaActual + "," + 105;
                            if (esNumeroEntero(fila.Ano48.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano49.ToString()))
                        {
                            string filaColumna = filaActual + "," + 106;
                            if (esNumeroEntero(fila.Ano49.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano50.ToString()))
                        {
                            string filaColumna = filaActual + "," + 107;
                            if (esNumeroEntero(fila.Ano50.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano51.ToString()))
                        {
                            string filaColumna = filaActual + "," + 108;
                            if (esNumeroEntero(fila.Ano51.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano52.ToString()))
                        {
                            string filaColumna = filaActual + "," + 109;
                            if (esNumeroEntero(fila.Ano52.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano53.ToString()))
                        {
                            string filaColumna = filaActual + "," + 110;
                            if (esNumeroEntero(fila.Ano53.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano54.ToString()))
                        {
                            string filaColumna = filaActual + "," + 111;
                            if (esNumeroEntero(fila.Ano54.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano55.ToString()))
                        {
                            string filaColumna = filaActual + "," + 112;
                            if (esNumeroEntero(fila.Ano55.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano56.ToString()))
                        {
                            string filaColumna = filaActual + "," + 113;
                            if (esNumeroEntero(fila.Ano56.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano57.ToString()))
                        {
                            string filaColumna = filaActual + "," + 114;
                            if (esNumeroEntero(fila.Ano57.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano58.ToString()))
                        {
                            string filaColumna = filaActual + "," + 115;
                            if (esNumeroEntero(fila.Ano58.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano59.ToString()))
                        {
                            string filaColumna = filaActual + "," + 116;
                            if (esNumeroEntero(fila.Ano59.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano60.ToString()))
                        {
                            string filaColumna = filaActual + "," + 117;
                            if (esNumeroEntero(fila.Ano60.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }

                        if (!esCero(fila.Ano61.ToString()))
                        {
                            string filaColumna = filaActual + "," + 118;
                            if (esNumeroEntero(fila.Ano61.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano62.ToString()))
                        {
                            string filaColumna = filaActual + "," + 119;
                            if (esNumeroEntero(fila.Ano62.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano63.ToString()))
                        {
                            string filaColumna = filaActual + "," + 120;
                            if (esNumeroEntero(fila.Ano63.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano64.ToString()))
                        {
                            string filaColumna = filaActual + "," + 121;
                            if (esNumeroEntero(fila.Ano64.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano65.ToString()))
                        {
                            string filaColumna = filaActual + "," + 122;
                            if (esNumeroEntero(fila.Ano65.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano66.ToString()))
                        {
                            string filaColumna = filaActual + "," + 123;
                            if (esNumeroEntero(fila.Ano66.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano67.ToString()))
                        {
                            string filaColumna = filaActual + "," + 124;
                            if (esNumeroEntero(fila.Ano67.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano68.ToString()))
                        {
                            string filaColumna = filaActual + "," + 125;
                            if (esNumeroEntero(fila.Ano68.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano69.ToString()))
                        {
                            string filaColumna = filaActual + "," + 126;
                            if (esNumeroEntero(fila.Ano69.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano70.ToString()))
                        {
                            string filaColumna = filaActual + "," + 127;
                            if (esNumeroEntero(fila.Ano70.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }

                        if (!esCero(fila.Ano71.ToString()))
                        {
                            string filaColumna = filaActual + "," + 128;
                            if (esNumeroEntero(fila.Ano71.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano72.ToString()))
                        {
                            string filaColumna = filaActual + "," + 129;
                            if (esNumeroEntero(fila.Ano72.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano73.ToString()))
                        {
                            string filaColumna = filaActual + "," + 130;
                            if (esNumeroEntero(fila.Ano73.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano74.ToString()))
                        {
                            string filaColumna = filaActual + "," + 131;
                            if (esNumeroEntero(fila.Ano74.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano75.ToString()))
                        {
                            string filaColumna = filaActual + "," + 132;
                            if (esNumeroEntero(fila.Ano75.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano76.ToString()))
                        {
                            string filaColumna = filaActual + "," + 133;
                            if (esNumeroEntero(fila.Ano76.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano77.ToString()))
                        {
                            string filaColumna = filaActual + "," + 134;
                            if (esNumeroEntero(fila.Ano77.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano78.ToString()))
                        {
                            string filaColumna = filaActual + "," + 135;
                            if (esNumeroEntero(fila.Ano78.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano79.ToString()))
                        {
                            string filaColumna = filaActual + "," + 136;
                            if (esNumeroEntero(fila.Ano79.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano80.ToString()))
                        {
                            string filaColumna = filaActual + "," + 137;
                            if (esNumeroEntero(fila.Ano80.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.TotalCapexTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 138;
                            if (esNumeroEntero(fila.TotalCapexTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        double inversionNacional = 0.0;
                        double inversionExtranjera = 0.0;
                        if (fila.HitNacExt != null && !string.IsNullOrEmpty(fila.HitNacExt.ToString()))
                        {
                            string[] hitNacExt = fila.HitNacExt.ToString().Split('/');
                            if (hitNacExt.Length == 2)
                            {
                                CultureInfo ciCL = new CultureInfo("es-CL", false);
                                inversionNacional = double.Parse(hitNacExt[0], ciCL);
                                inversionExtranjera = double.Parse(hitNacExt[1], ciCL);
                            }
                        }
                        ultimoIdPid = fila.IdPid;
                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidEstadoFlujo, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.PidNombreProyectoAlias,
                             fila.IniTipo, (fila.IniPeriodo + 1), fila.IgFechaInicio, fila.IgFechaTermino, fila.IgFechaCierre,
                             fila.PidProceso, fila.PidObjeto, fila.PidArea, fila.PidCompania, fila.PidEtapa, fila.PidCodigoProyecto, fila.PidGerenciaInversion,
                             fila.PidGerenteInversion, fila.PidGerenciaEjecucion, fila.PidGerenteEjecucion, fila.PidSuperintendencia, fila.PidSuperintendente,
                             fila.PidEncargadoControl, fila.CatEstadoProyecto, fila.CatTipoCotizacion, fila.CatCategoria, fila.CatNivelIngenieria,
                             fila.CatClasificacionSSO, fila.CatEstandarSeguridad, fila.CatClase, fila.CatMacroCategoria,
                             fila.EriClas1, fila.EriMFL1, fila.EriClas2, fila.EriMFL2, fila.EveVan, fila.EveTir, fila.EveIvan, fila.EvePayBack, fila.EveVidaUtil, fila.PorCostoDueno, fila.PorContingencia,
                             fila.TotalPeriodoAnterior, fila.EneroTotAcum, fila.FebreroTotAcum, fila.MarzoTotAcum, fila.AbrilTotAcum, fila.MayoTotAcum, fila.JunioTotAcum, fila.JulioTotAcum,
                             fila.AgostoTotAcum, fila.SeptiembreTotAcum, fila.OctubreTotAcum, fila.NoviembreTotAcum, fila.DiciembreTotAcum, fila.TotalTotAcum,
                             fila.PresAnioMasUnoTotAcum, fila.PresAnioMasDosTotAcum, fila.PresAnioMasTresTotAcum

                             , fila.Ano4, fila.Ano5
                             , fila.Ano6, fila.Ano7, fila.Ano8, fila.Ano9, fila.Ano10, fila.Ano11, fila.Ano12, fila.Ano13, fila.Ano14, fila.Ano15, fila.Ano16, fila.Ano17, fila.Ano18, fila.Ano19, fila.Ano20, fila.Ano21
                             , fila.Ano22, fila.Ano23, fila.Ano24, fila.Ano25, fila.Ano26, fila.Ano27, fila.Ano28, fila.Ano29, fila.Ano30, fila.Ano31, fila.Ano32, fila.Ano33, fila.Ano34, fila.Ano35, fila.Ano36, fila.Ano37
                             , fila.Ano38, fila.Ano39, fila.Ano40, fila.Ano41, fila.Ano42, fila.Ano43, fila.Ano44, fila.Ano45, fila.Ano46, fila.Ano47, fila.Ano48, fila.Ano49, fila.Ano50, fila.Ano51, fila.Ano52, fila.Ano53
                             , fila.Ano54, fila.Ano55, fila.Ano56, fila.Ano57, fila.Ano58, fila.Ano59, fila.Ano60, fila.Ano61, fila.Ano62, fila.Ano63, fila.Ano64, fila.Ano65, fila.Ano66, fila.Ano67, fila.Ano68, fila.Ano69, fila.Ano70
                             , fila.Ano71, fila.Ano72, fila.Ano73, fila.Ano74, fila.Ano75, fila.Ano76, fila.Ano77, fila.Ano78, fila.Ano79, fila.Ano80,


                             fila.TotalCapexTotAcum, inversionNacional, inversionExtranjera, ((fila.MatrizRiesgoNombre != null) ? fila.MatrizRiesgoNombre.ToString().Trim() : ""), fila.ObjetivoInversion, fila.AlcanceInversion);


                        filaActual++;
                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    err.ToString();
                    ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                }
                finally
                {
                    objConnection.Close();
                }
            }
            hashtable.Add(1, dt);
            hashtable.Add(2, numerosSeparadorMiles);
            hashtable.Add(3, numerosSeparadorMilesDecimales);
            return hashtable;
        }




        private Hashtable getDataExcelCasoBaseFormat(string token)
        {
            //Creating DataTable  
            Hashtable hashtable = new Hashtable();
            List<string> numerosSeparadorMiles = new List<string>();
            List<string> numerosSeparadorMilesDecimales = new List<string>();
            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = (todaysDate.Year + 1);
            //Setiing Table Name  
            dt.TableName = "Resumen CB-CD";
            //Add Columns  
            dt.Columns.Add("Id Iniciativa", typeof(int));
            dt.Columns.Add("Usuario", typeof(string));
            dt.Columns.Add("Estado Flujo", typeof(string));
            dt.Columns.Add("Codigo Iniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Nombre Proyecto(Alias)", typeof(string));
            dt.Columns.Add("Tipo Ejercicio", typeof(string));
            dt.Columns.Add("Periodo", typeof(int));
            dt.Columns.Add("Fecha Inicio", typeof(string));
            dt.Columns.Add("Fecha Termino", typeof(string));
            dt.Columns.Add("Fecha Cierre", typeof(string));
            dt.Columns.Add("Proceso", typeof(string));
            dt.Columns.Add("Objeto", typeof(string));
            dt.Columns.Add("Area", typeof(string));
            dt.Columns.Add("Compania", typeof(string));
            dt.Columns.Add("Etapa", typeof(string));
            dt.Columns.Add("Codigo Proyecto", typeof(string));
            dt.Columns.Add("Gerencia Inversion (Cliente)", typeof(string));
            dt.Columns.Add("Gerente Inversion (Cliente)", typeof(string));
            dt.Columns.Add("Gerencia Ejecucion", typeof(string));
            dt.Columns.Add("Gerente Ejecucion", typeof(string));
            dt.Columns.Add("Superintendencia", typeof(string));
            dt.Columns.Add("Superintendente", typeof(string));
            dt.Columns.Add("Encargado Control SAP", typeof(string));
            dt.Columns.Add("Estado Proyecto", typeof(string));
            dt.Columns.Add("Tipo Cotizacion", typeof(string));
            dt.Columns.Add("Categoria", typeof(string));
            dt.Columns.Add("Nivel Ingenieria", typeof(string));
            dt.Columns.Add("Clasificacion SSO", typeof(string));
            dt.Columns.Add("Estandar Seguridad", typeof(string));
            dt.Columns.Add("Clase", typeof(string));
            dt.Columns.Add("MacroCategoria", typeof(string));
            dt.Columns.Add("Clas. Riesgo sin Proy.", typeof(string));
            dt.Columns.Add("MFL (KUS$)", typeof(double));
            dt.Columns.Add("Clas. Riesgo con Proy.", typeof(string));
            dt.Columns.Add("MPE (KUS$)", typeof(double));
            dt.Columns.Add("VAN (KUS$)", typeof(double));
            dt.Columns.Add("TIR", typeof(double));
            dt.Columns.Add("IVAN", typeof(double));
            dt.Columns.Add("PayBack", typeof(double));
            dt.Columns.Add("Vida Util (Años)", typeof(double));
            dt.Columns.Add("Costo Dueño", typeof(double));
            dt.Columns.Add("Contingencia", typeof(double));
            dt.Columns.Add("Per. Ant.", typeof(double));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Año" + (year), typeof(double));
            dt.Columns.Add("Año " + (year + 1), typeof(double));
            dt.Columns.Add("Año " + (year + 2), typeof(double));
            dt.Columns.Add("Año " + (year + 3), typeof(double));
            dt.Columns.Add("Año " + (year + 4), typeof(double));
            dt.Columns.Add("Año " + (year + 5), typeof(double));
            dt.Columns.Add("Año " + (year + 6), typeof(double));
            dt.Columns.Add("Año " + (year + 7), typeof(double));
            dt.Columns.Add("Año " + (year + 8), typeof(double));
            dt.Columns.Add("Año " + (year + 9), typeof(double));
            dt.Columns.Add("Año " + (year + 10), typeof(double));
            dt.Columns.Add("Año " + (year + 11), typeof(double));
            dt.Columns.Add("Año " + (year + 12), typeof(double));
            dt.Columns.Add("Año " + (year + 13), typeof(double));
            dt.Columns.Add("Año " + (year + 14), typeof(double));
            dt.Columns.Add("Año " + (year + 15), typeof(double));
            dt.Columns.Add("Año " + (year + 16), typeof(double));
            dt.Columns.Add("Año " + (year + 17), typeof(double));
            dt.Columns.Add("Año " + (year + 18), typeof(double));
            dt.Columns.Add("Año " + (year + 19), typeof(double));
            dt.Columns.Add("Año " + (year + 20), typeof(double));
            dt.Columns.Add("Año " + (year + 21), typeof(double));
            dt.Columns.Add("Año " + (year + 22), typeof(double));
            dt.Columns.Add("Año " + (year + 23), typeof(double));
            dt.Columns.Add("Año " + (year + 24), typeof(double));
            dt.Columns.Add("Año " + (year + 25), typeof(double));
            dt.Columns.Add("Año " + (year + 26), typeof(double));
            dt.Columns.Add("Año " + (year + 27), typeof(double));
            dt.Columns.Add("Año " + (year + 28), typeof(double));
            dt.Columns.Add("Año " + (year + 29), typeof(double));
            dt.Columns.Add("Año " + (year + 30), typeof(double));
            dt.Columns.Add("Año " + (year + 31), typeof(double));
            dt.Columns.Add("Año " + (year + 32), typeof(double));
            dt.Columns.Add("Año " + (year + 33), typeof(double));
            dt.Columns.Add("Año " + (year + 34), typeof(double));
            dt.Columns.Add("Año " + (year + 35), typeof(double));
            dt.Columns.Add("Año " + (year + 36), typeof(double));
            dt.Columns.Add("Año " + (year + 37), typeof(double));
            dt.Columns.Add("Año " + (year + 38), typeof(double));
            dt.Columns.Add("Año " + (year + 39), typeof(double));
            dt.Columns.Add("Año " + (year + 40), typeof(double));
            dt.Columns.Add("Año " + (year + 41), typeof(double));
            dt.Columns.Add("Año " + (year + 42), typeof(double));
            dt.Columns.Add("Año " + (year + 43), typeof(double));
            dt.Columns.Add("Año " + (year + 44), typeof(double));
            dt.Columns.Add("Año " + (year + 45), typeof(double));
            dt.Columns.Add("Año " + (year + 46), typeof(double));
            dt.Columns.Add("Año " + (year + 47), typeof(double));
            dt.Columns.Add("Año " + (year + 48), typeof(double));
            dt.Columns.Add("Año " + (year + 49), typeof(double));
            dt.Columns.Add("Año " + (year + 50), typeof(double));
            dt.Columns.Add("Año " + (year + 51), typeof(double));
            dt.Columns.Add("Año " + (year + 52), typeof(double));
            dt.Columns.Add("Año " + (year + 53), typeof(double));
            dt.Columns.Add("Año " + (year + 54), typeof(double));
            dt.Columns.Add("Año " + (year + 55), typeof(double));
            dt.Columns.Add("Año " + (year + 56), typeof(double));
            dt.Columns.Add("Año " + (year + 57), typeof(double));
            dt.Columns.Add("Año " + (year + 58), typeof(double));
            dt.Columns.Add("Año " + (year + 59), typeof(double));
            dt.Columns.Add("Año " + (year + 60), typeof(double));
            dt.Columns.Add("Año " + (year + 61), typeof(double));
            dt.Columns.Add("Año " + (year + 62), typeof(double));
            dt.Columns.Add("Año " + (year + 63), typeof(double));
            dt.Columns.Add("Año " + (year + 64), typeof(double));
            dt.Columns.Add("Año " + (year + 65), typeof(double));
            dt.Columns.Add("Año " + (year + 66), typeof(double));
            dt.Columns.Add("Año " + (year + 67), typeof(double));
            dt.Columns.Add("Año " + (year + 68), typeof(double));
            dt.Columns.Add("Año " + (year + 69), typeof(double));
            dt.Columns.Add("Año " + (year + 70), typeof(double));
            dt.Columns.Add("Año " + (year + 71), typeof(double));
            dt.Columns.Add("Año " + (year + 72), typeof(double));
            dt.Columns.Add("Año " + (year + 73), typeof(double));
            dt.Columns.Add("Año " + (year + 74), typeof(double));
            dt.Columns.Add("Año " + (year + 75), typeof(double));
            dt.Columns.Add("Año " + (year + 76), typeof(double));
            dt.Columns.Add("Año " + (year + 77), typeof(double));
            dt.Columns.Add("Año " + (year + 78), typeof(double));
            dt.Columns.Add("Año " + (year + 79), typeof(double));
            dt.Columns.Add("Año " + (year + 80), typeof(double));
            dt.Columns.Add("Total Capex (Parcial)", typeof(double));

            dt.Columns.Add("% Inversion Nacional", typeof(double));
            dt.Columns.Add("% Inversion Extranjera", typeof(double));
            dt.Columns.Add("Riesgo Clave", typeof(string));

            dt.Columns.Add("Objetivo de la Inversión", typeof(string));
            dt.Columns.Add("Alcance /Descripción de la Inversión", typeof(string));

            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }

                    var tipoIniciativaSeleccionado = Convert.ToString(Session["tipoIniciativaSeleccionado"]);
                    if (string.IsNullOrEmpty(tipoIniciativaSeleccionado)) //AMBAS
                    {
                        tipoIniciativaSeleccionado = "0";
                    }
                    var anioIniciativaSeleccionado = Convert.ToString(Session["anioIniciativaSeleccionado"]);
                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA_CASO_BASE", new { @usuario = usuario, @opcion = token, @periodo = anioIniciativaSeleccionado }, commandType: CommandType.StoredProcedure).ToList();

                    var filaActual = 2;
                    var ultimoIdPid = 0;
                    foreach (var fila in contenido)
                    {

                        if (ultimoIdPid == fila.IdPid)
                        {
                            continue;
                        }
                        if (!esCero(fila.TotalPeriodoAnterior.ToString()))
                        {
                            string filaColumna = filaActual + "," + 44;
                            if (esNumeroEntero(fila.TotalPeriodoAnterior.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.EneroTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 45;
                            if (esNumeroEntero(fila.EneroTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.FebreroTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 46;
                            if (esNumeroEntero(fila.FebreroTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.MarzoTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 47;
                            if (esNumeroEntero(fila.MarzoTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.AbrilTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 48;
                            if (esNumeroEntero(fila.AbrilTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.MayoTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 49;
                            if (esNumeroEntero(fila.MayoTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.JunioTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 50;
                            if (esNumeroEntero(fila.JunioTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.JulioTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 51;
                            if (esNumeroEntero(fila.JulioTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.AgostoTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 52;
                            if (esNumeroEntero(fila.AgostoTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.SeptiembreTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 53;
                            if (esNumeroEntero(fila.SeptiembreTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.OctubreTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 54;
                            if (esNumeroEntero(fila.OctubreTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.NoviembreTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 55;
                            if (esNumeroEntero(fila.NoviembreTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.DiciembreTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 56;
                            if (esNumeroEntero(fila.DiciembreTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.TotalTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 57;
                            if (esNumeroEntero(fila.TotalTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.PresAnioMasUnoTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 58;
                            if (esNumeroEntero(fila.PresAnioMasUnoTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.PresAnioMasDosTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 59;
                            if (esNumeroEntero(fila.PresAnioMasDosTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.PresAnioMasTresTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 60;
                            if (esNumeroEntero(fila.PresAnioMasTresTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }

                        if (!esCero(fila.Ano4.ToString()))
                        {
                            string filaColumna = filaActual + "," + 61;
                            if (esNumeroEntero(fila.Ano4.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano5.ToString()))
                        {
                            string filaColumna = filaActual + "," + 62;
                            if (esNumeroEntero(fila.Ano5.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano6.ToString()))
                        {
                            string filaColumna = filaActual + "," + 63;
                            if (esNumeroEntero(fila.Ano6.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano7.ToString()))
                        {
                            string filaColumna = filaActual + "," + 64;
                            if (esNumeroEntero(fila.Ano7.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano8.ToString()))
                        {
                            string filaColumna = filaActual + "," + 65;
                            if (esNumeroEntero(fila.Ano8.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano9.ToString()))
                        {
                            string filaColumna = filaActual + "," + 66;
                            if (esNumeroEntero(fila.Ano9.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano10.ToString()))
                        {
                            string filaColumna = filaActual + "," + 67;
                            if (esNumeroEntero(fila.Ano10.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano11.ToString()))
                        {
                            string filaColumna = filaActual + "," + 68;
                            if (esNumeroEntero(fila.Ano11.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano12.ToString()))
                        {
                            string filaColumna = filaActual + "," + 69;
                            if (esNumeroEntero(fila.Ano12.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano13.ToString()))
                        {
                            string filaColumna = filaActual + "," + 70;
                            if (esNumeroEntero(fila.Ano13.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano14.ToString()))
                        {
                            string filaColumna = filaActual + "," + 71;
                            if (esNumeroEntero(fila.Ano14.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano15.ToString()))
                        {
                            string filaColumna = filaActual + "," + 72;
                            if (esNumeroEntero(fila.Ano15.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano16.ToString()))
                        {
                            string filaColumna = filaActual + "," + 73;
                            if (esNumeroEntero(fila.Ano16.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano17.ToString()))
                        {
                            string filaColumna = filaActual + "," + 74;
                            if (esNumeroEntero(fila.Ano17.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano18.ToString()))
                        {
                            string filaColumna = filaActual + "," + 75;
                            if (esNumeroEntero(fila.Ano18.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano19.ToString()))
                        {
                            string filaColumna = filaActual + "," + 76;
                            if (esNumeroEntero(fila.Ano19.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano20.ToString()))
                        {
                            string filaColumna = filaActual + "," + 77;
                            if (esNumeroEntero(fila.Ano20.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }


                        if (!esCero(fila.Ano21.ToString()))
                        {
                            string filaColumna = filaActual + "," + 78;
                            if (esNumeroEntero(fila.Ano21.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano22.ToString()))
                        {
                            string filaColumna = filaActual + "," + 79;
                            if (esNumeroEntero(fila.Ano22.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano23.ToString()))
                        {
                            string filaColumna = filaActual + "," + 80;
                            if (esNumeroEntero(fila.Ano23.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano24.ToString()))
                        {
                            string filaColumna = filaActual + "," + 81;
                            if (esNumeroEntero(fila.Ano24.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano25.ToString()))
                        {
                            string filaColumna = filaActual + "," + 82;
                            if (esNumeroEntero(fila.Ano25.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano26.ToString()))
                        {
                            string filaColumna = filaActual + "," + 83;
                            if (esNumeroEntero(fila.Ano26.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano27.ToString()))
                        {
                            string filaColumna = filaActual + "," + 84;
                            if (esNumeroEntero(fila.Ano27.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano28.ToString()))
                        {
                            string filaColumna = filaActual + "," + 85;
                            if (esNumeroEntero(fila.Ano28.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano29.ToString()))
                        {
                            string filaColumna = filaActual + "," + 86;
                            if (esNumeroEntero(fila.Ano29.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano30.ToString()))
                        {
                            string filaColumna = filaActual + "," + 87;
                            if (esNumeroEntero(fila.Ano30.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano31.ToString()))
                        {
                            string filaColumna = filaActual + "," + 88;
                            if (esNumeroEntero(fila.Ano31.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano32.ToString()))
                        {
                            string filaColumna = filaActual + "," + 89;
                            if (esNumeroEntero(fila.Ano32.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano33.ToString()))
                        {
                            string filaColumna = filaActual + "," + 90;
                            if (esNumeroEntero(fila.Ano33.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano34.ToString()))
                        {
                            string filaColumna = filaActual + "," + 91;
                            if (esNumeroEntero(fila.Ano34.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano35.ToString()))
                        {
                            string filaColumna = filaActual + "," + 92;
                            if (esNumeroEntero(fila.Ano35.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano36.ToString()))
                        {
                            string filaColumna = filaActual + "," + 93;
                            if (esNumeroEntero(fila.Ano36.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano37.ToString()))
                        {
                            string filaColumna = filaActual + "," + 94;
                            if (esNumeroEntero(fila.Ano37.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano38.ToString()))
                        {
                            string filaColumna = filaActual + "," + 95;
                            if (esNumeroEntero(fila.Ano38.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano39.ToString()))
                        {
                            string filaColumna = filaActual + "," + 96;
                            if (esNumeroEntero(fila.Ano39.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano40.ToString()))
                        {
                            string filaColumna = filaActual + "," + 97;
                            if (esNumeroEntero(fila.Ano40.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano41.ToString()))
                        {
                            string filaColumna = filaActual + "," + 98;
                            if (esNumeroEntero(fila.Ano41.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano42.ToString()))
                        {
                            string filaColumna = filaActual + "," + 99;
                            if (esNumeroEntero(fila.Ano42.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano43.ToString()))
                        {
                            string filaColumna = filaActual + "," + 100;
                            if (esNumeroEntero(fila.Ano43.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano44.ToString()))
                        {
                            string filaColumna = filaActual + "," + 101;
                            if (esNumeroEntero(fila.Ano44.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano45.ToString()))
                        {
                            string filaColumna = filaActual + "," + 102;
                            if (esNumeroEntero(fila.Ano45.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano46.ToString()))
                        {
                            string filaColumna = filaActual + "," + 103;
                            if (esNumeroEntero(fila.Ano46.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano47.ToString()))
                        {
                            string filaColumna = filaActual + "," + 104;
                            if (esNumeroEntero(fila.Ano47.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano48.ToString()))
                        {
                            string filaColumna = filaActual + "," + 105;
                            if (esNumeroEntero(fila.Ano48.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano49.ToString()))
                        {
                            string filaColumna = filaActual + "," + 106;
                            if (esNumeroEntero(fila.Ano49.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano50.ToString()))
                        {
                            string filaColumna = filaActual + "," + 107;
                            if (esNumeroEntero(fila.Ano50.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano51.ToString()))
                        {
                            string filaColumna = filaActual + "," + 108;
                            if (esNumeroEntero(fila.Ano51.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano52.ToString()))
                        {
                            string filaColumna = filaActual + "," + 109;
                            if (esNumeroEntero(fila.Ano52.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano53.ToString()))
                        {
                            string filaColumna = filaActual + "," + 110;
                            if (esNumeroEntero(fila.Ano53.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano54.ToString()))
                        {
                            string filaColumna = filaActual + "," + 111;
                            if (esNumeroEntero(fila.Ano54.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano55.ToString()))
                        {
                            string filaColumna = filaActual + "," + 112;
                            if (esNumeroEntero(fila.Ano55.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano56.ToString()))
                        {
                            string filaColumna = filaActual + "," + 113;
                            if (esNumeroEntero(fila.Ano56.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano57.ToString()))
                        {
                            string filaColumna = filaActual + "," + 114;
                            if (esNumeroEntero(fila.Ano57.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano58.ToString()))
                        {
                            string filaColumna = filaActual + "," + 115;
                            if (esNumeroEntero(fila.Ano58.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano59.ToString()))
                        {
                            string filaColumna = filaActual + "," + 116;
                            if (esNumeroEntero(fila.Ano59.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano60.ToString()))
                        {
                            string filaColumna = filaActual + "," + 117;
                            if (esNumeroEntero(fila.Ano60.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }

                        if (!esCero(fila.Ano61.ToString()))
                        {
                            string filaColumna = filaActual + "," + 118;
                            if (esNumeroEntero(fila.Ano61.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano62.ToString()))
                        {
                            string filaColumna = filaActual + "," + 119;
                            if (esNumeroEntero(fila.Ano62.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano63.ToString()))
                        {
                            string filaColumna = filaActual + "," + 120;
                            if (esNumeroEntero(fila.Ano63.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano64.ToString()))
                        {
                            string filaColumna = filaActual + "," + 121;
                            if (esNumeroEntero(fila.Ano64.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano65.ToString()))
                        {
                            string filaColumna = filaActual + "," + 122;
                            if (esNumeroEntero(fila.Ano65.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano66.ToString()))
                        {
                            string filaColumna = filaActual + "," + 123;
                            if (esNumeroEntero(fila.Ano66.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano67.ToString()))
                        {
                            string filaColumna = filaActual + "," + 124;
                            if (esNumeroEntero(fila.Ano67.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano68.ToString()))
                        {
                            string filaColumna = filaActual + "," + 125;
                            if (esNumeroEntero(fila.Ano68.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano69.ToString()))
                        {
                            string filaColumna = filaActual + "," + 126;
                            if (esNumeroEntero(fila.Ano69.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano70.ToString()))
                        {
                            string filaColumna = filaActual + "," + 127;
                            if (esNumeroEntero(fila.Ano70.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }

                        if (!esCero(fila.Ano71.ToString()))
                        {
                            string filaColumna = filaActual + "," + 128;
                            if (esNumeroEntero(fila.Ano71.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano72.ToString()))
                        {
                            string filaColumna = filaActual + "," + 129;
                            if (esNumeroEntero(fila.Ano72.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano73.ToString()))
                        {
                            string filaColumna = filaActual + "," + 130;
                            if (esNumeroEntero(fila.Ano73.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano74.ToString()))
                        {
                            string filaColumna = filaActual + "," + 131;
                            if (esNumeroEntero(fila.Ano74.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano75.ToString()))
                        {
                            string filaColumna = filaActual + "," + 132;
                            if (esNumeroEntero(fila.Ano75.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano76.ToString()))
                        {
                            string filaColumna = filaActual + "," + 133;
                            if (esNumeroEntero(fila.Ano76.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano77.ToString()))
                        {
                            string filaColumna = filaActual + "," + 134;
                            if (esNumeroEntero(fila.Ano77.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano78.ToString()))
                        {
                            string filaColumna = filaActual + "," + 135;
                            if (esNumeroEntero(fila.Ano78.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano79.ToString()))
                        {
                            string filaColumna = filaActual + "," + 136;
                            if (esNumeroEntero(fila.Ano79.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano80.ToString()))
                        {
                            string filaColumna = filaActual + "," + 137;
                            if (esNumeroEntero(fila.Ano80.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.TotalCapexTotAcum.ToString()))
                        {
                            string filaColumna = filaActual + "," + 138;
                            if (esNumeroEntero(fila.TotalCapexTotAcum.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        double inversionNacional = 0.0;
                        double inversionExtranjera = 0.0;
                        if (fila.HitNacExt != null && !string.IsNullOrEmpty(fila.HitNacExt.ToString()))
                        {
                            string[] hitNacExt = fila.HitNacExt.ToString().Split('/');
                            if (hitNacExt.Length == 2)
                            {
                                CultureInfo ciCL = new CultureInfo("es-CL", false);
                                inversionNacional = double.Parse(hitNacExt[0], ciCL);
                                inversionExtranjera = double.Parse(hitNacExt[1], ciCL);
                            }
                        }
                        ultimoIdPid = fila.IdPid;
                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidEstadoFlujo, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.PidNombreProyectoAlias,
                             fila.IniTipo, (fila.IniPeriodo + 1), fila.IgFechaInicio, fila.IgFechaTermino, fila.IgFechaCierre,
                             fila.PidProceso, fila.PidObjeto, fila.PidArea, fila.PidCompania, fila.PidEtapa, fila.PidCodigoProyecto, fila.PidGerenciaInversion,
                             fila.PidGerenteInversion, fila.PidGerenciaEjecucion, fila.PidGerenteEjecucion, fila.PidSuperintendencia, fila.PidSuperintendente,
                             fila.PidEncargadoControl, fila.CatEstadoProyecto, fila.CatTipoCotizacion, fila.CatCategoria, fila.CatNivelIngenieria,
                             fila.CatClasificacionSSO, fila.CatEstandarSeguridad, fila.CatClase, fila.CatMacroCategoria,
                             fila.EriClas1, fila.EriMFL1, fila.EriClas2, fila.EriMFL2, fila.EveVan, fila.EveTir, fila.EveIvan, fila.EvePayBack, fila.EveVidaUtil, fila.PorCostoDueno, fila.PorContingencia,
                             fila.TotalPeriodoAnterior, fila.EneroTotAcum, fila.FebreroTotAcum, fila.MarzoTotAcum, fila.AbrilTotAcum, fila.MayoTotAcum, fila.JunioTotAcum, fila.JulioTotAcum,
                             fila.AgostoTotAcum, fila.SeptiembreTotAcum, fila.OctubreTotAcum, fila.NoviembreTotAcum, fila.DiciembreTotAcum, fila.TotalTotAcum,
                             fila.PresAnioMasUnoTotAcum, fila.PresAnioMasDosTotAcum, fila.PresAnioMasTresTotAcum

                             , fila.Ano4, fila.Ano5
                             , fila.Ano6, fila.Ano7, fila.Ano8, fila.Ano9, fila.Ano10, fila.Ano11, fila.Ano12, fila.Ano13, fila.Ano14, fila.Ano15, fila.Ano16, fila.Ano17, fila.Ano18, fila.Ano19, fila.Ano20, fila.Ano21
                             , fila.Ano22, fila.Ano23, fila.Ano24, fila.Ano25, fila.Ano26, fila.Ano27, fila.Ano28, fila.Ano29, fila.Ano30, fila.Ano31, fila.Ano32, fila.Ano33, fila.Ano34, fila.Ano35, fila.Ano36, fila.Ano37
                             , fila.Ano38, fila.Ano39, fila.Ano40, fila.Ano41, fila.Ano42, fila.Ano43, fila.Ano44, fila.Ano45, fila.Ano46, fila.Ano47, fila.Ano48, fila.Ano49, fila.Ano50, fila.Ano51, fila.Ano52, fila.Ano53
                             , fila.Ano54, fila.Ano55, fila.Ano56, fila.Ano57, fila.Ano58, fila.Ano59, fila.Ano60, fila.Ano61, fila.Ano62, fila.Ano63, fila.Ano64, fila.Ano65, fila.Ano66, fila.Ano67, fila.Ano68, fila.Ano69, fila.Ano70
                             , fila.Ano71, fila.Ano72, fila.Ano73, fila.Ano74, fila.Ano75, fila.Ano76, fila.Ano77, fila.Ano78, fila.Ano79, fila.Ano80,


                             fila.TotalCapexTotAcum, inversionNacional, inversionExtranjera, ((fila.MatrizRiesgoNombre != null) ? fila.MatrizRiesgoNombre.ToString().Trim() : ""), fila.ObjetivoInversion, fila.AlcanceInversion);

                        /*dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.Fase, fila.Ponderacion_Financiera, fila.Periodos_Anteriores, fila.Enero, fila.Febrero, fila.Marzo,
                        fila.Abril, fila.Mayo, fila.Junio, fila.Julio, fila.Agosto, fila.Septiembre, fila.Octubre, fila.Noviembre, fila.Diciembre, fila.Total, fila.Ano1, fila.Ano2, fila.Ano3, fila.Ano4, fila.Ano5
                        , fila.Ano6, fila.Ano7, fila.Ano8, fila.Ano9, fila.Ano10, fila.Ano11, fila.Ano12, fila.Ano13, fila.Ano14, fila.Ano15, fila.Ano16, fila.Ano17, fila.Ano18, fila.Ano19, fila.Ano20, fila.Ano21
                        , fila.Ano22, fila.Ano23, fila.Ano24, fila.Ano25, fila.Ano26, fila.Ano27, fila.Ano28, fila.Ano29, fila.Ano30, fila.Ano31, fila.Ano32, fila.Ano33, fila.Ano34, fila.Ano35, fila.Ano36, fila.Ano37
                        , fila.Ano38, fila.Ano39, fila.Ano40, fila.Ano41, fila.Ano42, fila.Ano43, fila.Ano44, fila.Ano45, fila.Ano46, fila.Ano47, fila.Ano48, fila.Ano49, fila.Ano50, fila.Ano51, fila.Ano52, fila.Ano53
                        , fila.Ano54, fila.Ano55, fila.Ano56, fila.Ano57, fila.Ano58, fila.Ano59, fila.Ano60, fila.Ano61, fila.Ano62, fila.Ano63, fila.Ano64, fila.Ano65, fila.Ano66, fila.Ano67, fila.Ano68, fila.Ano69, fila.Ano70
                        , fila.Ano71, fila.Ano72, fila.Ano73, fila.Ano74, fila.Ano75, fila.Ano76, fila.Ano77, fila.Ano78, fila.Ano79, fila.Ano80, fila.Total_Capex);*/
                        filaActual++;
                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    //Utils.LogError(ExceptionResult);
                    //return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
            hashtable.Add(1, dt);
            hashtable.Add(2, numerosSeparadorMiles);
            hashtable.Add(3, numerosSeparadorMilesDecimales);
            return hashtable;
        }

        private bool esCero(string numero)
        {
            return "0".Equals(numero) || "0,0".Equals(numero) || "0,00".Equals(numero) || "0,000000000000".Equals(numero);
        }

        private bool esNumeroEntero(string numero)
        {
            int indexChar = numero.IndexOf(',');
            if (indexChar != -1)
            {
                string parteDecimal = numero.Substring(indexChar + 1);
                for (int i = 0; i < parteDecimal.Length; i++)
                {
                    if (parteDecimal[i] != '0')
                    {
                        return false;
                    }
                }
                return true;
            }
            return true;
        }


        private DataTable getDataFinancieroExcel(string token)
        {
            //Creating DataTable  
            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = todaysDate.Year;
            //Setiing Table Name  
            dt.TableName = "Financiero PP-EX";
            //Add Columns  
            dt.Columns.Add("Id Iniciativa", typeof(int));
            dt.Columns.Add("Usuario", typeof(string));
            dt.Columns.Add("Codigo Iniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Fase", typeof(string));
            dt.Columns.Add("Pond_Fina", typeof(double));
            dt.Columns.Add("Per_Ant", typeof(double));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Año" + (year + 1), typeof(double));
            dt.Columns.Add("Año " + (year + 2), typeof(double));
            dt.Columns.Add("Año " + (year + 3), typeof(double));
            dt.Columns.Add("Año " + (year + 4), typeof(double));
            dt.Columns.Add("Total_Capex", typeof(double));

            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }

                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA_FINANCIERO", new { @usuario = usuario, @opcion = token }, commandType: CommandType.StoredProcedure).ToList();
                    var contadorFila = 0;
                    var ultimoPid = 0;
                    foreach (var fila in contenido)
                    {
                        //Add Rows in DataTable
                        if (contadorFila > 0 && ultimoPid != fila.IdPid)
                        {
                            dt.Rows.Add();
                        }
                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.Fase, fila.Ponderacion_Financiera, fila.Periodos_Anteriores, fila.Enero, fila.Febrero, fila.Marzo,
                        fila.Abril, fila.Mayo, fila.Junio, fila.Julio, fila.Agosto, fila.Septiembre, fila.Octubre, fila.Noviembre, fila.Diciembre, fila.Total, fila.Ano1, fila.Ano2,
                        fila.Ano3, fila.Total_Capex);
                        ultimoPid = fila.IdPid;
                        // contadorFila++;
                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    //ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    //Utils.LogError(ExceptionResult);
                    //return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
            return dt;
        }

        private Hashtable getDataFinancieroExcelEjerciciosOficialesFormat(string token)
        {
            Hashtable hashtable = new Hashtable();
            List<string> numerosSeparadorMiles = new List<string>();
            List<string> numerosSeparadorMilesDecimales = new List<string>();
            //Creating DataTable  
            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = todaysDate.Year;
            //Setiing Table Name  
            dt.TableName = "Financiero PP-EX";
            //Add Columns  
            dt.Columns.Add("Id Iniciativa", typeof(int));
            dt.Columns.Add("Usuario", typeof(string));
            dt.Columns.Add("Codigo Iniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Nombre Proyecto(Alias)", typeof(string));
            dt.Columns.Add("Fase", typeof(string));
            dt.Columns.Add("Pond_Fina", typeof(double));
            dt.Columns.Add("Per_Ant", typeof(double));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Año" + (year + 1), typeof(double));
            dt.Columns.Add("Año " + (year + 2), typeof(double));
            dt.Columns.Add("Año " + (year + 3), typeof(double));
            dt.Columns.Add("Año " + (year + 4), typeof(double));
            dt.Columns.Add("Total_Capex", typeof(double));

            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }
                    string periodo = ((Session["anioIniciativaEjercicioOficial"] != null && !string.IsNullOrEmpty(Convert.ToString(Session["anioIniciativaEjercicioOficial"]))) ? Convert.ToString(Session["anioIniciativaEjercicioOficial"]) : "0");
                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA_FINANCIERO_EJERCICIOS_OFICIALES", new { @usuario = usuario, @opcion = token, @periodo = periodo }, commandType: CommandType.StoredProcedure).ToList();
                    var contadorFila = 0;
                    var ultimoPid = 0;
                    var filaActual = 2;
                    foreach (var fila in contenido)
                    {
                        //Add Rows in DataTable
                        if (contadorFila > 0 && ultimoPid != fila.IdPid)
                        {
                            /*dt.Rows.Add();
                            contadorFila++;
                            filaActual++;*/
                        }

                        if (!esCero(fila.Ponderacion_Financiera.ToString()))
                        {
                            string filaColumna = filaActual + "," + 7;
                            if (esNumeroEntero(fila.Ponderacion_Financiera.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Periodos_Anteriores.ToString()))
                        {
                            string filaColumna = filaActual + "," + 8;
                            if (esNumeroEntero(fila.Periodos_Anteriores.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Enero.ToString()))
                        {
                            string filaColumna = filaActual + "," + 9;
                            if (esNumeroEntero(fila.Enero.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Febrero.ToString()))
                        {
                            string filaColumna = filaActual + "," + 10;
                            if (esNumeroEntero(fila.Febrero.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Marzo.ToString()))
                        {
                            string filaColumna = filaActual + "," + 11;
                            if (esNumeroEntero(fila.Marzo.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Abril.ToString()))
                        {
                            string filaColumna = filaActual + "," + 12;
                            if (esNumeroEntero(fila.Abril.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Mayo.ToString()))
                        {
                            string filaColumna = filaActual + "," + 13;
                            if (esNumeroEntero(fila.Mayo.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Junio.ToString()))
                        {
                            string filaColumna = filaActual + "," + 14;
                            if (esNumeroEntero(fila.Junio.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Julio.ToString()))
                        {
                            string filaColumna = filaActual + "," + 15;
                            if (esNumeroEntero(fila.Julio.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Agosto.ToString()))
                        {
                            string filaColumna = filaActual + "," + 16;
                            if (esNumeroEntero(fila.Agosto.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Septiembre.ToString()))
                        {
                            string filaColumna = filaActual + "," + 17;
                            if (esNumeroEntero(fila.Septiembre.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Octubre.ToString()))
                        {
                            string filaColumna = filaActual + "," + 18;
                            if (esNumeroEntero(fila.Octubre.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Noviembre.ToString()))
                        {
                            string filaColumna = filaActual + "," + 19;
                            if (esNumeroEntero(fila.Noviembre.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Diciembre.ToString()))
                        {
                            string filaColumna = filaActual + "," + 20;
                            if (esNumeroEntero(fila.Diciembre.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Total.ToString()))
                        {
                            string filaColumna = filaActual + "," + 21;
                            if (esNumeroEntero(fila.Total.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano1.ToString()))
                        {
                            string filaColumna = filaActual + "," + 22;
                            if (esNumeroEntero(fila.Ano1.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano2.ToString()))
                        {
                            string filaColumna = filaActual + "," + 23;
                            if (esNumeroEntero(fila.Ano2.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano3.ToString()))
                        {
                            string filaColumna = filaActual + "," + 24;
                            if (esNumeroEntero(fila.Ano3.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Total_Capex.ToString()))
                        {
                            string filaColumna = filaActual + "," + 25;
                            if (esNumeroEntero(fila.Total_Capex.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.PidNombreProyectoAlias, fila.Fase, fila.Ponderacion_Financiera, fila.Periodos_Anteriores, fila.Enero, fila.Febrero, fila.Marzo,
                        fila.Abril, fila.Mayo, fila.Junio, fila.Julio, fila.Agosto, fila.Septiembre, fila.Octubre, fila.Noviembre, fila.Diciembre, fila.Total, fila.Ano1, fila.Ano2, fila.Ano3, fila.Total_Capex);
                        ultimoPid = fila.IdPid;
                        contadorFila++;
                        filaActual++;
                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    //ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    //Utils.LogError(ExceptionResult);
                    //return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
            hashtable.Add(1, dt);
            hashtable.Add(2, numerosSeparadorMiles);
            hashtable.Add(3, numerosSeparadorMilesDecimales);
            return hashtable;
        }

        private Hashtable getDataFinancieroExcelFormatParametroVN(string tipoIniciativaSeleccionado, string anioIniciativaSeleccionado, string parametroVN)
        {
            Hashtable hashtable = new Hashtable();
            List<string> numerosSeparadorMiles = new List<string>();
            List<string> numerosSeparadorMilesDecimales = new List<string>();
            //Creating DataTable  
            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = todaysDate.Year;
            //Setiing Table Name  
            dt.TableName = "Financiero PP-EX";
            //Add Columns  
            dt.Columns.Add("Id Iniciativa", typeof(int));
            dt.Columns.Add("Usuario", typeof(string));
            dt.Columns.Add("Codigo Iniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Nombre Proyecto(Alias)", typeof(string));
            dt.Columns.Add("Fase", typeof(string));
            dt.Columns.Add("Pond_Fina", typeof(double));
            dt.Columns.Add("Per_Ant", typeof(double));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Año" + (year + 1), typeof(double));
            dt.Columns.Add("Año " + (year + 2), typeof(double));
            dt.Columns.Add("Año " + (year + 3), typeof(double));
            dt.Columns.Add("Año " + (year + 4), typeof(double));
            dt.Columns.Add("Total_Capex", typeof(double));

            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }
                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA_FINANCIERO_PARAMETROVN", new { @periodo = anioIniciativaSeleccionado, @parametroVN = parametroVN }, commandType: CommandType.StoredProcedure).ToList();
                    var contadorFila = 0;
                    var ultimoPid = 0;
                    var filaActual = 2;
                    foreach (var fila in contenido)
                    {
                        //Add Rows in DataTable
                        if (contadorFila > 0 && ultimoPid != fila.IdPid)
                        {
                            /*dt.Rows.Add();
                            contadorFila++;
                            filaActual++;*/
                        }

                        if (!esCero(fila.Ponderacion_Financiera.ToString()))
                        {
                            string filaColumna = filaActual + "," + 7;
                            if (esNumeroEntero(fila.Ponderacion_Financiera.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }

                        if (!esCero(fila.Periodos_Anteriores.ToString()))
                        {
                            string filaColumna = filaActual + "," + 8;
                            if (esNumeroEntero(fila.Periodos_Anteriores.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Enero.ToString()))
                        {
                            string filaColumna = filaActual + "," + 9;
                            if (esNumeroEntero(fila.Enero.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Febrero.ToString()))
                        {
                            string filaColumna = filaActual + "," + 10;
                            if (esNumeroEntero(fila.Febrero.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Marzo.ToString()))
                        {
                            string filaColumna = filaActual + "," + 11;
                            if (esNumeroEntero(fila.Marzo.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Abril.ToString()))
                        {
                            string filaColumna = filaActual + "," + 12;
                            if (esNumeroEntero(fila.Abril.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Mayo.ToString()))
                        {
                            string filaColumna = filaActual + "," + 13;
                            if (esNumeroEntero(fila.Mayo.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Junio.ToString()))
                        {
                            string filaColumna = filaActual + "," + 14;
                            if (esNumeroEntero(fila.Junio.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Julio.ToString()))
                        {
                            string filaColumna = filaActual + "," + 15;
                            if (esNumeroEntero(fila.Julio.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Agosto.ToString()))
                        {
                            string filaColumna = filaActual + "," + 16;
                            if (esNumeroEntero(fila.Agosto.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Septiembre.ToString()))
                        {
                            string filaColumna = filaActual + "," + 17;
                            if (esNumeroEntero(fila.Septiembre.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Octubre.ToString()))
                        {
                            string filaColumna = filaActual + "," + 18;
                            if (esNumeroEntero(fila.Octubre.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Noviembre.ToString()))
                        {
                            string filaColumna = filaActual + "," + 19;
                            if (esNumeroEntero(fila.Noviembre.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Diciembre.ToString()))
                        {
                            string filaColumna = filaActual + "," + 20;
                            if (esNumeroEntero(fila.Diciembre.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Total.ToString()))
                        {
                            string filaColumna = filaActual + "," + 21;
                            if (esNumeroEntero(fila.Total.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano1.ToString()))
                        {
                            string filaColumna = filaActual + "," + 22;
                            if (esNumeroEntero(fila.Ano1.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano2.ToString()))
                        {
                            string filaColumna = filaActual + "," + 23;
                            if (esNumeroEntero(fila.Ano2.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano3.ToString()))
                        {
                            string filaColumna = filaActual + "," + 24;
                            if (esNumeroEntero(fila.Ano3.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Total_Capex.ToString()))
                        {
                            string filaColumna = filaActual + "," + 25;
                            if (esNumeroEntero(fila.Total_Capex.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.PidNombreProyectoAlias, fila.Fase, fila.Ponderacion_Financiera, fila.Periodos_Anteriores, fila.Enero, fila.Febrero, fila.Marzo,
                        fila.Abril, fila.Mayo, fila.Junio, fila.Julio, fila.Agosto, fila.Septiembre, fila.Octubre, fila.Noviembre, fila.Diciembre, fila.Total, fila.Ano1, fila.Ano2, fila.Ano3, fila.Total_Capex);
                        ultimoPid = fila.IdPid;
                        contadorFila++;
                        filaActual++;
                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    err.ToString();
                    //ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    //Utils.LogError(ExceptionResult);
                    //return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
            hashtable.Add(1, dt);
            hashtable.Add(2, numerosSeparadorMiles);
            hashtable.Add(3, numerosSeparadorMilesDecimales);
            return hashtable;
        }

        private Hashtable getDataFinancieroExcelFormat(string token)
        {
            Hashtable hashtable = new Hashtable();
            List<string> numerosSeparadorMiles = new List<string>();
            List<string> numerosSeparadorMilesDecimales = new List<string>();
            //Creating DataTable  
            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = todaysDate.Year;
            //Setiing Table Name  
            dt.TableName = "Financiero PP-EX";
            //Add Columns  
            dt.Columns.Add("Id Iniciativa", typeof(int));
            dt.Columns.Add("Usuario", typeof(string));
            dt.Columns.Add("Codigo Iniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Nombre Proyecto(Alias)", typeof(string));
            dt.Columns.Add("Fase", typeof(string));
            dt.Columns.Add("Pond_Fina", typeof(double));
            dt.Columns.Add("Per_Ant", typeof(double));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Año" + (year + 1), typeof(double));
            dt.Columns.Add("Año " + (year + 2), typeof(double));
            dt.Columns.Add("Año " + (year + 3), typeof(double));
            dt.Columns.Add("Año " + (year + 4), typeof(double));
            dt.Columns.Add("Total_Capex", typeof(double));

            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }

                    int anio = todaysDate.Year;
                    int anioMasUno = anio + 1;
                    string anioUno = string.Empty;
                    string anioDos = string.Empty;
                    var tipoIniciativaSeleccionado = Convert.ToString(Session["tipoIniciativaSeleccionado"]);
                    if (string.IsNullOrEmpty(tipoIniciativaSeleccionado))
                    {
                        tipoIniciativaSeleccionado = "0";
                    }
                    var anioIniciativaSeleccionado = Convert.ToString(Session["anioIniciativaSeleccionado"]);
                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA_FINANCIERO", new { @usuario = usuario, @opcion = token, @periodo = anioIniciativaSeleccionado }, commandType: CommandType.StoredProcedure).ToList();
                    var contadorFila = 0;
                    var ultimoPid = 0;
                    var filaActual = 2;
                    foreach (var fila in contenido)
                    {
                        //Add Rows in DataTable
                        if (contadorFila > 0 && ultimoPid != fila.IdPid)
                        {
                            /*dt.Rows.Add();
                            contadorFila++;
                            filaActual++;*/
                        }

                        if (!esCero(fila.Ponderacion_Financiera.ToString()))
                        {
                            string filaColumna = filaActual + "," + 7;
                            if (esNumeroEntero(fila.Ponderacion_Financiera.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }

                        if (!esCero(fila.Periodos_Anteriores.ToString()))
                        {
                            string filaColumna = filaActual + "," + 8;
                            if (esNumeroEntero(fila.Periodos_Anteriores.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Enero.ToString()))
                        {
                            string filaColumna = filaActual + "," + 9;
                            if (esNumeroEntero(fila.Enero.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Febrero.ToString()))
                        {
                            string filaColumna = filaActual + "," + 10;
                            if (esNumeroEntero(fila.Febrero.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Marzo.ToString()))
                        {
                            string filaColumna = filaActual + "," + 11;
                            if (esNumeroEntero(fila.Marzo.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Abril.ToString()))
                        {
                            string filaColumna = filaActual + "," + 12;
                            if (esNumeroEntero(fila.Abril.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Mayo.ToString()))
                        {
                            string filaColumna = filaActual + "," + 13;
                            if (esNumeroEntero(fila.Mayo.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Junio.ToString()))
                        {
                            string filaColumna = filaActual + "," + 14;
                            if (esNumeroEntero(fila.Junio.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Julio.ToString()))
                        {
                            string filaColumna = filaActual + "," + 15;
                            if (esNumeroEntero(fila.Julio.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Agosto.ToString()))
                        {
                            string filaColumna = filaActual + "," + 16;
                            if (esNumeroEntero(fila.Agosto.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Septiembre.ToString()))
                        {
                            string filaColumna = filaActual + "," + 17;
                            if (esNumeroEntero(fila.Septiembre.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Octubre.ToString()))
                        {
                            string filaColumna = filaActual + "," + 18;
                            if (esNumeroEntero(fila.Octubre.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Noviembre.ToString()))
                        {
                            string filaColumna = filaActual + "," + 19;
                            if (esNumeroEntero(fila.Noviembre.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Diciembre.ToString()))
                        {
                            string filaColumna = filaActual + "," + 20;
                            if (esNumeroEntero(fila.Diciembre.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Total.ToString()))
                        {
                            string filaColumna = filaActual + "," + 21;
                            if (esNumeroEntero(fila.Total.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano1.ToString()))
                        {
                            string filaColumna = filaActual + "," + 22;
                            if (esNumeroEntero(fila.Ano1.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano2.ToString()))
                        {
                            string filaColumna = filaActual + "," + 23;
                            if (esNumeroEntero(fila.Ano2.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano3.ToString()))
                        {
                            string filaColumna = filaActual + "," + 24;
                            if (esNumeroEntero(fila.Ano3.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Total_Capex.ToString()))
                        {
                            string filaColumna = filaActual + "," + 25;
                            if (esNumeroEntero(fila.Total_Capex.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.PidNombreProyectoAlias, fila.Fase, fila.Ponderacion_Financiera, fila.Periodos_Anteriores, fila.Enero, fila.Febrero, fila.Marzo,
                        fila.Abril, fila.Mayo, fila.Junio, fila.Julio, fila.Agosto, fila.Septiembre, fila.Octubre, fila.Noviembre, fila.Diciembre, fila.Total, fila.Ano1, fila.Ano2, fila.Ano3, fila.Total_Capex);
                        ultimoPid = fila.IdPid;
                        contadorFila++;
                        filaActual++;
                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    //ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    //Utils.LogError(ExceptionResult);
                    //return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
            hashtable.Add(1, dt);
            hashtable.Add(2, numerosSeparadorMiles);
            hashtable.Add(3, numerosSeparadorMilesDecimales);
            return hashtable;
        }

        private Hashtable getDataFinancieroCasoBaseExcelFormatParametroVN(string tipoIniciativaSeleccionado, string anioIniciativaSeleccionado, string parametroVN)
        {
            Hashtable hashtable = new Hashtable();
            List<string> numerosSeparadorMiles = new List<string>();
            List<string> numerosSeparadorMilesDecimales = new List<string>();
            //Creating DataTable  
            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = (todaysDate.Year + 1);
            //Setiing Table Name  
            dt.TableName = "Financiero CB-CD";
            //Add Columns  
            dt.Columns.Add("Id Iniciativa", typeof(int));
            dt.Columns.Add("Usuario", typeof(string));
            dt.Columns.Add("Codigo Iniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Nombre Proyecto(Alias)", typeof(string));
            dt.Columns.Add("Fase", typeof(string));
            dt.Columns.Add("Pond_Fina", typeof(double));
            dt.Columns.Add("Per_Ant", typeof(double));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Año" + (year), typeof(double));
            dt.Columns.Add("Año " + (year + 1), typeof(double));
            dt.Columns.Add("Año " + (year + 2), typeof(double));
            dt.Columns.Add("Año " + (year + 3), typeof(double));
            dt.Columns.Add("Año " + (year + 4), typeof(double));
            dt.Columns.Add("Año " + (year + 5), typeof(double));
            dt.Columns.Add("Año " + (year + 6), typeof(double));
            dt.Columns.Add("Año " + (year + 7), typeof(double));
            dt.Columns.Add("Año " + (year + 8), typeof(double));
            dt.Columns.Add("Año " + (year + 9), typeof(double));
            dt.Columns.Add("Año " + (year + 10), typeof(double));
            dt.Columns.Add("Año " + (year + 11), typeof(double));
            dt.Columns.Add("Año " + (year + 12), typeof(double));
            dt.Columns.Add("Año " + (year + 13), typeof(double));
            dt.Columns.Add("Año " + (year + 14), typeof(double));
            dt.Columns.Add("Año " + (year + 15), typeof(double));
            dt.Columns.Add("Año " + (year + 16), typeof(double));
            dt.Columns.Add("Año " + (year + 17), typeof(double));
            dt.Columns.Add("Año " + (year + 18), typeof(double));
            dt.Columns.Add("Año " + (year + 19), typeof(double));
            dt.Columns.Add("Año " + (year + 20), typeof(double));
            dt.Columns.Add("Año " + (year + 21), typeof(double));
            dt.Columns.Add("Año " + (year + 22), typeof(double));
            dt.Columns.Add("Año " + (year + 23), typeof(double));
            dt.Columns.Add("Año " + (year + 24), typeof(double));
            dt.Columns.Add("Año " + (year + 25), typeof(double));
            dt.Columns.Add("Año " + (year + 26), typeof(double));
            dt.Columns.Add("Año " + (year + 27), typeof(double));
            dt.Columns.Add("Año " + (year + 28), typeof(double));
            dt.Columns.Add("Año " + (year + 29), typeof(double));
            dt.Columns.Add("Año " + (year + 30), typeof(double));
            dt.Columns.Add("Año " + (year + 31), typeof(double));
            dt.Columns.Add("Año " + (year + 32), typeof(double));
            dt.Columns.Add("Año " + (year + 33), typeof(double));
            dt.Columns.Add("Año " + (year + 34), typeof(double));
            dt.Columns.Add("Año " + (year + 35), typeof(double));
            dt.Columns.Add("Año " + (year + 36), typeof(double));
            dt.Columns.Add("Año " + (year + 37), typeof(double));
            dt.Columns.Add("Año " + (year + 38), typeof(double));
            dt.Columns.Add("Año " + (year + 39), typeof(double));
            dt.Columns.Add("Año " + (year + 40), typeof(double));
            dt.Columns.Add("Año " + (year + 41), typeof(double));
            dt.Columns.Add("Año " + (year + 42), typeof(double));
            dt.Columns.Add("Año " + (year + 43), typeof(double));
            dt.Columns.Add("Año " + (year + 44), typeof(double));
            dt.Columns.Add("Año " + (year + 45), typeof(double));
            dt.Columns.Add("Año " + (year + 46), typeof(double));
            dt.Columns.Add("Año " + (year + 47), typeof(double));
            dt.Columns.Add("Año " + (year + 48), typeof(double));
            dt.Columns.Add("Año " + (year + 49), typeof(double));
            dt.Columns.Add("Año " + (year + 50), typeof(double));
            dt.Columns.Add("Año " + (year + 51), typeof(double));
            dt.Columns.Add("Año " + (year + 52), typeof(double));
            dt.Columns.Add("Año " + (year + 53), typeof(double));
            dt.Columns.Add("Año " + (year + 54), typeof(double));
            dt.Columns.Add("Año " + (year + 55), typeof(double));
            dt.Columns.Add("Año " + (year + 56), typeof(double));
            dt.Columns.Add("Año " + (year + 57), typeof(double));
            dt.Columns.Add("Año " + (year + 58), typeof(double));
            dt.Columns.Add("Año " + (year + 59), typeof(double));
            dt.Columns.Add("Año " + (year + 60), typeof(double));
            dt.Columns.Add("Año " + (year + 61), typeof(double));
            dt.Columns.Add("Año " + (year + 62), typeof(double));
            dt.Columns.Add("Año " + (year + 63), typeof(double));
            dt.Columns.Add("Año " + (year + 64), typeof(double));
            dt.Columns.Add("Año " + (year + 65), typeof(double));
            dt.Columns.Add("Año " + (year + 66), typeof(double));
            dt.Columns.Add("Año " + (year + 67), typeof(double));
            dt.Columns.Add("Año " + (year + 68), typeof(double));
            dt.Columns.Add("Año " + (year + 69), typeof(double));
            dt.Columns.Add("Año " + (year + 70), typeof(double));
            dt.Columns.Add("Año " + (year + 71), typeof(double));
            dt.Columns.Add("Año " + (year + 72), typeof(double));
            dt.Columns.Add("Año " + (year + 73), typeof(double));
            dt.Columns.Add("Año " + (year + 74), typeof(double));
            dt.Columns.Add("Año " + (year + 75), typeof(double));
            dt.Columns.Add("Año " + (year + 76), typeof(double));
            dt.Columns.Add("Año " + (year + 77), typeof(double));
            dt.Columns.Add("Año " + (year + 78), typeof(double));
            dt.Columns.Add("Año " + (year + 79), typeof(double));
            dt.Columns.Add("Año " + (year + 80), typeof(double));
            dt.Columns.Add("Total_Capex", typeof(double));

            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }

                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA_FINANCIERO_CASO_BASE_PARAMETROVN", new { @periodo = anioIniciativaSeleccionado, @parametroVN = parametroVN }, commandType: CommandType.StoredProcedure).ToList();
                    var contadorFila = 0;
                    var ultimoPid = 0;
                    var filaActual = 2;
                    foreach (var fila in contenido)
                    {
                        //Add Rows in DataTable
                        if (contadorFila > 0 && ultimoPid != fila.IdPid)
                        {
                            /*dt.Rows.Add();
                            contadorFila++;
                            filaActual++;*/
                        }


                        if (!esCero(fila.Ponderacion_Financiera.ToString()))
                        {
                            string filaColumna = filaActual + "," + 7;
                            if (esNumeroEntero(fila.Ponderacion_Financiera.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Periodos_Anteriores.ToString()))
                        {
                            string filaColumna = filaActual + "," + 8;
                            if (esNumeroEntero(fila.Periodos_Anteriores.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Enero.ToString()))
                        {
                            string filaColumna = filaActual + "," + 9;
                            if (esNumeroEntero(fila.Enero.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Febrero.ToString()))
                        {
                            string filaColumna = filaActual + "," + 10;
                            if (esNumeroEntero(fila.Febrero.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Marzo.ToString()))
                        {
                            string filaColumna = filaActual + "," + 11;
                            if (esNumeroEntero(fila.Marzo.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Abril.ToString()))
                        {
                            string filaColumna = filaActual + "," + 12;
                            if (esNumeroEntero(fila.Abril.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Mayo.ToString()))
                        {
                            string filaColumna = filaActual + "," + 13;
                            if (esNumeroEntero(fila.Mayo.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Junio.ToString()))
                        {
                            string filaColumna = filaActual + "," + 14;
                            if (esNumeroEntero(fila.Junio.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Julio.ToString()))
                        {
                            string filaColumna = filaActual + "," + 15;
                            if (esNumeroEntero(fila.Julio.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Agosto.ToString()))
                        {
                            string filaColumna = filaActual + "," + 16;
                            if (esNumeroEntero(fila.Agosto.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Septiembre.ToString()))
                        {
                            string filaColumna = filaActual + "," + 17;
                            if (esNumeroEntero(fila.Septiembre.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Octubre.ToString()))
                        {
                            string filaColumna = filaActual + "," + 18;
                            if (esNumeroEntero(fila.Octubre.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Noviembre.ToString()))
                        {
                            string filaColumna = filaActual + "," + 19;
                            if (esNumeroEntero(fila.Noviembre.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Diciembre.ToString()))
                        {
                            string filaColumna = filaActual + "," + 20;
                            if (esNumeroEntero(fila.Diciembre.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Total.ToString()))
                        {
                            string filaColumna = filaActual + "," + 21;
                            if (esNumeroEntero(fila.Total.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano1.ToString()))
                        {
                            string filaColumna = filaActual + "," + 22;
                            if (esNumeroEntero(fila.Ano1.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano2.ToString()))
                        {
                            string filaColumna = filaActual + "," + 23;
                            if (esNumeroEntero(fila.Ano2.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano3.ToString()))
                        {
                            string filaColumna = filaActual + "," + 24;
                            if (esNumeroEntero(fila.Ano3.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }

                        if (!esCero(fila.Ano4.ToString()))
                        {
                            string filaColumna = filaActual + "," + 25;
                            if (esNumeroEntero(fila.Ano4.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano5.ToString()))
                        {
                            string filaColumna = filaActual + "," + 26;
                            if (esNumeroEntero(fila.Ano5.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano6.ToString()))
                        {
                            string filaColumna = filaActual + "," + 27;
                            if (esNumeroEntero(fila.Ano6.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano7.ToString()))
                        {
                            string filaColumna = filaActual + "," + 28;
                            if (esNumeroEntero(fila.Ano4.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano8.ToString()))
                        {
                            string filaColumna = filaActual + "," + 29;
                            if (esNumeroEntero(fila.Ano8.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano9.ToString()))
                        {
                            string filaColumna = filaActual + "," + 30;
                            if (esNumeroEntero(fila.Ano9.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano10.ToString()))
                        {
                            string filaColumna = filaActual + "," + 31;
                            if (esNumeroEntero(fila.Ano10.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano11.ToString()))
                        {
                            string filaColumna = filaActual + "," + 32;
                            if (esNumeroEntero(fila.Ano11.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano12.ToString()))
                        {
                            string filaColumna = filaActual + "," + 33;
                            if (esNumeroEntero(fila.Ano12.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano13.ToString()))
                        {
                            string filaColumna = filaActual + "," + 34;
                            if (esNumeroEntero(fila.Ano13.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano14.ToString()))
                        {
                            string filaColumna = filaActual + "," + 35;
                            if (esNumeroEntero(fila.Ano14.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano15.ToString()))
                        {
                            string filaColumna = filaActual + "," + 36;
                            if (esNumeroEntero(fila.Ano15.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano16.ToString()))
                        {
                            string filaColumna = filaActual + "," + 37;
                            if (esNumeroEntero(fila.Ano16.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano17.ToString()))
                        {
                            string filaColumna = filaActual + "," + 38;
                            if (esNumeroEntero(fila.Ano17.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano18.ToString()))
                        {
                            string filaColumna = filaActual + "," + 39;
                            if (esNumeroEntero(fila.Ano18.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano19.ToString()))
                        {
                            string filaColumna = filaActual + "," + 40;
                            if (esNumeroEntero(fila.Ano19.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano20.ToString()))
                        {
                            string filaColumna = filaActual + "," + 41;
                            if (esNumeroEntero(fila.Ano20.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }


                        if (!esCero(fila.Ano21.ToString()))
                        {
                            string filaColumna = filaActual + "," + 42;
                            if (esNumeroEntero(fila.Ano21.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano22.ToString()))
                        {
                            string filaColumna = filaActual + "," + 43;
                            if (esNumeroEntero(fila.Ano22.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano23.ToString()))
                        {
                            string filaColumna = filaActual + "," + 44;
                            if (esNumeroEntero(fila.Ano23.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano24.ToString()))
                        {
                            string filaColumna = filaActual + "," + 45;
                            if (esNumeroEntero(fila.Ano24.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano25.ToString()))
                        {
                            string filaColumna = filaActual + "," + 46;
                            if (esNumeroEntero(fila.Ano25.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano26.ToString()))
                        {
                            string filaColumna = filaActual + "," + 47;
                            if (esNumeroEntero(fila.Ano26.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano27.ToString()))
                        {
                            string filaColumna = filaActual + "," + 48;
                            if (esNumeroEntero(fila.Ano27.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano28.ToString()))
                        {
                            string filaColumna = filaActual + "," + 49;
                            if (esNumeroEntero(fila.Ano28.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano29.ToString()))
                        {
                            string filaColumna = filaActual + "," + 50;
                            if (esNumeroEntero(fila.Ano29.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano30.ToString()))
                        {
                            string filaColumna = filaActual + "," + 51;
                            if (esNumeroEntero(fila.Ano30.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano31.ToString()))
                        {
                            string filaColumna = filaActual + "," + 52;
                            if (esNumeroEntero(fila.Ano31.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano32.ToString()))
                        {
                            string filaColumna = filaActual + "," + 53;
                            if (esNumeroEntero(fila.Ano32.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano33.ToString()))
                        {
                            string filaColumna = filaActual + "," + 54;
                            if (esNumeroEntero(fila.Ano33.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano34.ToString()))
                        {
                            string filaColumna = filaActual + "," + 55;
                            if (esNumeroEntero(fila.Ano34.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano35.ToString()))
                        {
                            string filaColumna = filaActual + "," + 56;
                            if (esNumeroEntero(fila.Ano35.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano36.ToString()))
                        {
                            string filaColumna = filaActual + "," + 57;
                            if (esNumeroEntero(fila.Ano36.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano37.ToString()))
                        {
                            string filaColumna = filaActual + "," + 58;
                            if (esNumeroEntero(fila.Ano37.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano38.ToString()))
                        {
                            string filaColumna = filaActual + "," + 59;
                            if (esNumeroEntero(fila.Ano38.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano39.ToString()))
                        {
                            string filaColumna = filaActual + "," + 60;
                            if (esNumeroEntero(fila.Ano39.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano40.ToString()))
                        {
                            string filaColumna = filaActual + "," + 61;
                            if (esNumeroEntero(fila.Ano40.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano41.ToString()))
                        {
                            string filaColumna = filaActual + "," + 62;
                            if (esNumeroEntero(fila.Ano41.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano42.ToString()))
                        {
                            string filaColumna = filaActual + "," + 63;
                            if (esNumeroEntero(fila.Ano42.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano43.ToString()))
                        {
                            string filaColumna = filaActual + "," + 64;
                            if (esNumeroEntero(fila.Ano43.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano44.ToString()))
                        {
                            string filaColumna = filaActual + "," + 65;
                            if (esNumeroEntero(fila.Ano44.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano45.ToString()))
                        {
                            string filaColumna = filaActual + "," + 66;
                            if (esNumeroEntero(fila.Ano45.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano46.ToString()))
                        {
                            string filaColumna = filaActual + "," + 67;
                            if (esNumeroEntero(fila.Ano46.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano47.ToString()))
                        {
                            string filaColumna = filaActual + "," + 68;
                            if (esNumeroEntero(fila.Ano47.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano48.ToString()))
                        {
                            string filaColumna = filaActual + "," + 69;
                            if (esNumeroEntero(fila.Ano48.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano49.ToString()))
                        {
                            string filaColumna = filaActual + "," + 70;
                            if (esNumeroEntero(fila.Ano49.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano50.ToString()))
                        {
                            string filaColumna = filaActual + "," + 71;
                            if (esNumeroEntero(fila.Ano50.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano51.ToString()))
                        {
                            string filaColumna = filaActual + "," + 72;
                            if (esNumeroEntero(fila.Ano51.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano52.ToString()))
                        {
                            string filaColumna = filaActual + "," + 73;
                            if (esNumeroEntero(fila.Ano52.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano53.ToString()))
                        {
                            string filaColumna = filaActual + "," + 74;
                            if (esNumeroEntero(fila.Ano53.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano54.ToString()))
                        {
                            string filaColumna = filaActual + "," + 75;
                            if (esNumeroEntero(fila.Ano54.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano55.ToString()))
                        {
                            string filaColumna = filaActual + "," + 76;
                            if (esNumeroEntero(fila.Ano55.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano56.ToString()))
                        {
                            string filaColumna = filaActual + "," + 77;
                            if (esNumeroEntero(fila.Ano56.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano57.ToString()))
                        {
                            string filaColumna = filaActual + "," + 78;
                            if (esNumeroEntero(fila.Ano57.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano58.ToString()))
                        {
                            string filaColumna = filaActual + "," + 79;
                            if (esNumeroEntero(fila.Ano58.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano59.ToString()))
                        {
                            string filaColumna = filaActual + "," + 80;
                            if (esNumeroEntero(fila.Ano59.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano60.ToString()))
                        {
                            string filaColumna = filaActual + "," + 81;
                            if (esNumeroEntero(fila.Ano60.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }

                        if (!esCero(fila.Ano61.ToString()))
                        {
                            string filaColumna = filaActual + "," + 82;
                            if (esNumeroEntero(fila.Ano61.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano62.ToString()))
                        {
                            string filaColumna = filaActual + "," + 83;
                            if (esNumeroEntero(fila.Ano62.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano63.ToString()))
                        {
                            string filaColumna = filaActual + "," + 84;
                            if (esNumeroEntero(fila.Ano63.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano64.ToString()))
                        {
                            string filaColumna = filaActual + "," + 85;
                            if (esNumeroEntero(fila.Ano64.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano65.ToString()))
                        {
                            string filaColumna = filaActual + "," + 86;
                            if (esNumeroEntero(fila.Ano65.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano66.ToString()))
                        {
                            string filaColumna = filaActual + "," + 87;
                            if (esNumeroEntero(fila.Ano66.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano67.ToString()))
                        {
                            string filaColumna = filaActual + "," + 88;
                            if (esNumeroEntero(fila.Ano67.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano68.ToString()))
                        {
                            string filaColumna = filaActual + "," + 89;
                            if (esNumeroEntero(fila.Ano68.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano69.ToString()))
                        {
                            string filaColumna = filaActual + "," + 90;
                            if (esNumeroEntero(fila.Ano69.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano70.ToString()))
                        {
                            string filaColumna = filaActual + "," + 91;
                            if (esNumeroEntero(fila.Ano70.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }

                        if (!esCero(fila.Ano71.ToString()))
                        {
                            string filaColumna = filaActual + "," + 92;
                            if (esNumeroEntero(fila.Ano71.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano72.ToString()))
                        {
                            string filaColumna = filaActual + "," + 93;
                            if (esNumeroEntero(fila.Ano72.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano73.ToString()))
                        {
                            string filaColumna = filaActual + "," + 94;
                            if (esNumeroEntero(fila.Ano73.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano74.ToString()))
                        {
                            string filaColumna = filaActual + "," + 95;
                            if (esNumeroEntero(fila.Ano74.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano75.ToString()))
                        {
                            string filaColumna = filaActual + "," + 96;
                            if (esNumeroEntero(fila.Ano75.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano76.ToString()))
                        {
                            string filaColumna = filaActual + "," + 97;
                            if (esNumeroEntero(fila.Ano76.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano77.ToString()))
                        {
                            string filaColumna = filaActual + "," + 98;
                            if (esNumeroEntero(fila.Ano77.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano78.ToString()))
                        {
                            string filaColumna = filaActual + "," + 99;
                            if (esNumeroEntero(fila.Ano78.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano79.ToString()))
                        {
                            string filaColumna = filaActual + "," + 100;
                            if (esNumeroEntero(fila.Ano79.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano80.ToString()))
                        {
                            string filaColumna = filaActual + "," + 101;
                            if (esNumeroEntero(fila.Ano80.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }


                        if (!esCero(fila.Total_Capex.ToString()))
                        {
                            string filaColumna = filaActual + "," + 102;
                            if (esNumeroEntero(fila.Total_Capex.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }

                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.PidNombreProyectoAlias, fila.Fase, fila.Ponderacion_Financiera, fila.Periodos_Anteriores, fila.Enero, fila.Febrero, fila.Marzo,
                        fila.Abril, fila.Mayo, fila.Junio, fila.Julio, fila.Agosto, fila.Septiembre, fila.Octubre, fila.Noviembre, fila.Diciembre, fila.Total, fila.Ano1, fila.Ano2, fila.Ano3, fila.Ano4, fila.Ano5
                        , fila.Ano6, fila.Ano7, fila.Ano8, fila.Ano9, fila.Ano10, fila.Ano11, fila.Ano12, fila.Ano13, fila.Ano14, fila.Ano15, fila.Ano16, fila.Ano17, fila.Ano18, fila.Ano19, fila.Ano20, fila.Ano21
                        , fila.Ano22, fila.Ano23, fila.Ano24, fila.Ano25, fila.Ano26, fila.Ano27, fila.Ano28, fila.Ano29, fila.Ano30, fila.Ano31, fila.Ano32, fila.Ano33, fila.Ano34, fila.Ano35, fila.Ano36, fila.Ano37
                        , fila.Ano38, fila.Ano39, fila.Ano40, fila.Ano41, fila.Ano42, fila.Ano43, fila.Ano44, fila.Ano45, fila.Ano46, fila.Ano47, fila.Ano48, fila.Ano49, fila.Ano50, fila.Ano51, fila.Ano52, fila.Ano53
                        , fila.Ano54, fila.Ano55, fila.Ano56, fila.Ano57, fila.Ano58, fila.Ano59, fila.Ano60, fila.Ano61, fila.Ano62, fila.Ano63, fila.Ano64, fila.Ano65, fila.Ano66, fila.Ano67, fila.Ano68, fila.Ano69, fila.Ano70
                        , fila.Ano71, fila.Ano72, fila.Ano73, fila.Ano74, fila.Ano75, fila.Ano76, fila.Ano77, fila.Ano78, fila.Ano79, fila.Ano80, fila.Total_Capex);
                        ultimoPid = fila.IdPid;
                        contadorFila++;
                        filaActual++;
                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    err.ToString();
                    //ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    //Utils.LogError(ExceptionResult);
                    //return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
            hashtable.Add(1, dt);
            hashtable.Add(2, numerosSeparadorMiles);
            hashtable.Add(3, numerosSeparadorMilesDecimales);
            return hashtable;
        }

        private Hashtable getDataFinancieroCasoBaseExcelFormat(string token)
        {
            Hashtable hashtable = new Hashtable();
            List<string> numerosSeparadorMiles = new List<string>();
            List<string> numerosSeparadorMilesDecimales = new List<string>();
            //Creating DataTable  
            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = (todaysDate.Year + 1);
            //Setiing Table Name  
            dt.TableName = "Financiero CB-CD";
            //Add Columns  
            dt.Columns.Add("Id Iniciativa", typeof(int));
            dt.Columns.Add("Usuario", typeof(string));
            dt.Columns.Add("Codigo Iniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Nombre Proyecto(Alias)", typeof(string));
            dt.Columns.Add("Fase", typeof(string));
            dt.Columns.Add("Pond_Fina", typeof(double));
            dt.Columns.Add("Per_Ant", typeof(double));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Año" + (year), typeof(double));
            dt.Columns.Add("Año " + (year + 1), typeof(double));
            dt.Columns.Add("Año " + (year + 2), typeof(double));
            dt.Columns.Add("Año " + (year + 3), typeof(double));
            dt.Columns.Add("Año " + (year + 4), typeof(double));
            dt.Columns.Add("Año " + (year + 5), typeof(double));
            dt.Columns.Add("Año " + (year + 6), typeof(double));
            dt.Columns.Add("Año " + (year + 7), typeof(double));
            dt.Columns.Add("Año " + (year + 8), typeof(double));
            dt.Columns.Add("Año " + (year + 9), typeof(double));
            dt.Columns.Add("Año " + (year + 10), typeof(double));
            dt.Columns.Add("Año " + (year + 11), typeof(double));
            dt.Columns.Add("Año " + (year + 12), typeof(double));
            dt.Columns.Add("Año " + (year + 13), typeof(double));
            dt.Columns.Add("Año " + (year + 14), typeof(double));
            dt.Columns.Add("Año " + (year + 15), typeof(double));
            dt.Columns.Add("Año " + (year + 16), typeof(double));
            dt.Columns.Add("Año " + (year + 17), typeof(double));
            dt.Columns.Add("Año " + (year + 18), typeof(double));
            dt.Columns.Add("Año " + (year + 19), typeof(double));
            dt.Columns.Add("Año " + (year + 20), typeof(double));
            dt.Columns.Add("Año " + (year + 21), typeof(double));
            dt.Columns.Add("Año " + (year + 22), typeof(double));
            dt.Columns.Add("Año " + (year + 23), typeof(double));
            dt.Columns.Add("Año " + (year + 24), typeof(double));
            dt.Columns.Add("Año " + (year + 25), typeof(double));
            dt.Columns.Add("Año " + (year + 26), typeof(double));
            dt.Columns.Add("Año " + (year + 27), typeof(double));
            dt.Columns.Add("Año " + (year + 28), typeof(double));
            dt.Columns.Add("Año " + (year + 29), typeof(double));
            dt.Columns.Add("Año " + (year + 30), typeof(double));
            dt.Columns.Add("Año " + (year + 31), typeof(double));
            dt.Columns.Add("Año " + (year + 32), typeof(double));
            dt.Columns.Add("Año " + (year + 33), typeof(double));
            dt.Columns.Add("Año " + (year + 34), typeof(double));
            dt.Columns.Add("Año " + (year + 35), typeof(double));
            dt.Columns.Add("Año " + (year + 36), typeof(double));
            dt.Columns.Add("Año " + (year + 37), typeof(double));
            dt.Columns.Add("Año " + (year + 38), typeof(double));
            dt.Columns.Add("Año " + (year + 39), typeof(double));
            dt.Columns.Add("Año " + (year + 40), typeof(double));
            dt.Columns.Add("Año " + (year + 41), typeof(double));
            dt.Columns.Add("Año " + (year + 42), typeof(double));
            dt.Columns.Add("Año " + (year + 43), typeof(double));
            dt.Columns.Add("Año " + (year + 44), typeof(double));
            dt.Columns.Add("Año " + (year + 45), typeof(double));
            dt.Columns.Add("Año " + (year + 46), typeof(double));
            dt.Columns.Add("Año " + (year + 47), typeof(double));
            dt.Columns.Add("Año " + (year + 48), typeof(double));
            dt.Columns.Add("Año " + (year + 49), typeof(double));
            dt.Columns.Add("Año " + (year + 50), typeof(double));
            dt.Columns.Add("Año " + (year + 51), typeof(double));
            dt.Columns.Add("Año " + (year + 52), typeof(double));
            dt.Columns.Add("Año " + (year + 53), typeof(double));
            dt.Columns.Add("Año " + (year + 54), typeof(double));
            dt.Columns.Add("Año " + (year + 55), typeof(double));
            dt.Columns.Add("Año " + (year + 56), typeof(double));
            dt.Columns.Add("Año " + (year + 57), typeof(double));
            dt.Columns.Add("Año " + (year + 58), typeof(double));
            dt.Columns.Add("Año " + (year + 59), typeof(double));
            dt.Columns.Add("Año " + (year + 60), typeof(double));
            dt.Columns.Add("Año " + (year + 61), typeof(double));
            dt.Columns.Add("Año " + (year + 62), typeof(double));
            dt.Columns.Add("Año " + (year + 63), typeof(double));
            dt.Columns.Add("Año " + (year + 64), typeof(double));
            dt.Columns.Add("Año " + (year + 65), typeof(double));
            dt.Columns.Add("Año " + (year + 66), typeof(double));
            dt.Columns.Add("Año " + (year + 67), typeof(double));
            dt.Columns.Add("Año " + (year + 68), typeof(double));
            dt.Columns.Add("Año " + (year + 69), typeof(double));
            dt.Columns.Add("Año " + (year + 70), typeof(double));
            dt.Columns.Add("Año " + (year + 71), typeof(double));
            dt.Columns.Add("Año " + (year + 72), typeof(double));
            dt.Columns.Add("Año " + (year + 73), typeof(double));
            dt.Columns.Add("Año " + (year + 74), typeof(double));
            dt.Columns.Add("Año " + (year + 75), typeof(double));
            dt.Columns.Add("Año " + (year + 76), typeof(double));
            dt.Columns.Add("Año " + (year + 77), typeof(double));
            dt.Columns.Add("Año " + (year + 78), typeof(double));
            dt.Columns.Add("Año " + (year + 79), typeof(double));
            dt.Columns.Add("Año " + (year + 80), typeof(double));
            dt.Columns.Add("Total_Capex", typeof(double));

            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }

                    int anio = todaysDate.Year;
                    int anioMasUno = anio + 1;
                    string anioUno = string.Empty;
                    string anioDos = string.Empty;
                    var tipoIniciativaSeleccionado = Convert.ToString(Session["tipoIniciativaSeleccionado"]);
                    if (string.IsNullOrEmpty(tipoIniciativaSeleccionado))
                    {
                        tipoIniciativaSeleccionado = "0";
                    }
                    var anioIniciativaSeleccionado = Convert.ToString(Session["anioIniciativaSeleccionado"]);
                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA_FINANCIERO_CASO_BASE", new { @usuario = usuario, @opcion = token, @periodo = anioIniciativaSeleccionado }, commandType: CommandType.StoredProcedure).ToList();
                    var contadorFila = 0;
                    var ultimoPid = 0;
                    var filaActual = 2;
                    foreach (var fila in contenido)
                    {
                        //Add Rows in DataTable
                        if (contadorFila > 0 && ultimoPid != fila.IdPid)
                        {
                            /*dt.Rows.Add();
                            contadorFila++;
                            filaActual++;*/
                        }


                        if (!esCero(fila.Ponderacion_Financiera.ToString()))
                        {
                            string filaColumna = filaActual + "," + 7;
                            if (esNumeroEntero(fila.Ponderacion_Financiera.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Periodos_Anteriores.ToString()))
                        {
                            string filaColumna = filaActual + "," + 8;
                            if (esNumeroEntero(fila.Periodos_Anteriores.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Enero.ToString()))
                        {
                            string filaColumna = filaActual + "," + 9;
                            if (esNumeroEntero(fila.Enero.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Febrero.ToString()))
                        {
                            string filaColumna = filaActual + "," + 10;
                            if (esNumeroEntero(fila.Febrero.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Marzo.ToString()))
                        {
                            string filaColumna = filaActual + "," + 11;
                            if (esNumeroEntero(fila.Marzo.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Abril.ToString()))
                        {
                            string filaColumna = filaActual + "," + 12;
                            if (esNumeroEntero(fila.Abril.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Mayo.ToString()))
                        {
                            string filaColumna = filaActual + "," + 13;
                            if (esNumeroEntero(fila.Mayo.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Junio.ToString()))
                        {
                            string filaColumna = filaActual + "," + 14;
                            if (esNumeroEntero(fila.Junio.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Julio.ToString()))
                        {
                            string filaColumna = filaActual + "," + 15;
                            if (esNumeroEntero(fila.Julio.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Agosto.ToString()))
                        {
                            string filaColumna = filaActual + "," + 16;
                            if (esNumeroEntero(fila.Agosto.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Septiembre.ToString()))
                        {
                            string filaColumna = filaActual + "," + 17;
                            if (esNumeroEntero(fila.Septiembre.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Octubre.ToString()))
                        {
                            string filaColumna = filaActual + "," + 18;
                            if (esNumeroEntero(fila.Octubre.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Noviembre.ToString()))
                        {
                            string filaColumna = filaActual + "," + 19;
                            if (esNumeroEntero(fila.Noviembre.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Diciembre.ToString()))
                        {
                            string filaColumna = filaActual + "," + 20;
                            if (esNumeroEntero(fila.Diciembre.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Total.ToString()))
                        {
                            string filaColumna = filaActual + "," + 21;
                            if (esNumeroEntero(fila.Total.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano1.ToString()))
                        {
                            string filaColumna = filaActual + "," + 22;
                            if (esNumeroEntero(fila.Ano1.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano2.ToString()))
                        {
                            string filaColumna = filaActual + "," + 23;
                            if (esNumeroEntero(fila.Ano2.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano3.ToString()))
                        {
                            string filaColumna = filaActual + "," + 24;
                            if (esNumeroEntero(fila.Ano3.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }

                        if (!esCero(fila.Ano4.ToString()))
                        {
                            string filaColumna = filaActual + "," + 25;
                            if (esNumeroEntero(fila.Ano4.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano5.ToString()))
                        {
                            string filaColumna = filaActual + "," + 26;
                            if (esNumeroEntero(fila.Ano5.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano6.ToString()))
                        {
                            string filaColumna = filaActual + "," + 27;
                            if (esNumeroEntero(fila.Ano6.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano7.ToString()))
                        {
                            string filaColumna = filaActual + "," + 28;
                            if (esNumeroEntero(fila.Ano4.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano8.ToString()))
                        {
                            string filaColumna = filaActual + "," + 29;
                            if (esNumeroEntero(fila.Ano8.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano9.ToString()))
                        {
                            string filaColumna = filaActual + "," + 30;
                            if (esNumeroEntero(fila.Ano9.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano10.ToString()))
                        {
                            string filaColumna = filaActual + "," + 31;
                            if (esNumeroEntero(fila.Ano10.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano11.ToString()))
                        {
                            string filaColumna = filaActual + "," + 32;
                            if (esNumeroEntero(fila.Ano11.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano12.ToString()))
                        {
                            string filaColumna = filaActual + "," + 33;
                            if (esNumeroEntero(fila.Ano12.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano13.ToString()))
                        {
                            string filaColumna = filaActual + "," + 34;
                            if (esNumeroEntero(fila.Ano13.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano14.ToString()))
                        {
                            string filaColumna = filaActual + "," + 35;
                            if (esNumeroEntero(fila.Ano14.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano15.ToString()))
                        {
                            string filaColumna = filaActual + "," + 36;
                            if (esNumeroEntero(fila.Ano15.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano16.ToString()))
                        {
                            string filaColumna = filaActual + "," + 37;
                            if (esNumeroEntero(fila.Ano16.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano17.ToString()))
                        {
                            string filaColumna = filaActual + "," + 38;
                            if (esNumeroEntero(fila.Ano17.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano18.ToString()))
                        {
                            string filaColumna = filaActual + "," + 39;
                            if (esNumeroEntero(fila.Ano18.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano19.ToString()))
                        {
                            string filaColumna = filaActual + "," + 40;
                            if (esNumeroEntero(fila.Ano19.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano20.ToString()))
                        {
                            string filaColumna = filaActual + "," + 41;
                            if (esNumeroEntero(fila.Ano20.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }


                        if (!esCero(fila.Ano21.ToString()))
                        {
                            string filaColumna = filaActual + "," + 42;
                            if (esNumeroEntero(fila.Ano21.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano22.ToString()))
                        {
                            string filaColumna = filaActual + "," + 43;
                            if (esNumeroEntero(fila.Ano22.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano23.ToString()))
                        {
                            string filaColumna = filaActual + "," + 44;
                            if (esNumeroEntero(fila.Ano23.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano24.ToString()))
                        {
                            string filaColumna = filaActual + "," + 45;
                            if (esNumeroEntero(fila.Ano24.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano25.ToString()))
                        {
                            string filaColumna = filaActual + "," + 46;
                            if (esNumeroEntero(fila.Ano25.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano26.ToString()))
                        {
                            string filaColumna = filaActual + "," + 47;
                            if (esNumeroEntero(fila.Ano26.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano27.ToString()))
                        {
                            string filaColumna = filaActual + "," + 48;
                            if (esNumeroEntero(fila.Ano27.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano28.ToString()))
                        {
                            string filaColumna = filaActual + "," + 49;
                            if (esNumeroEntero(fila.Ano28.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano29.ToString()))
                        {
                            string filaColumna = filaActual + "," + 50;
                            if (esNumeroEntero(fila.Ano29.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano30.ToString()))
                        {
                            string filaColumna = filaActual + "," + 51;
                            if (esNumeroEntero(fila.Ano30.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano31.ToString()))
                        {
                            string filaColumna = filaActual + "," + 52;
                            if (esNumeroEntero(fila.Ano31.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano32.ToString()))
                        {
                            string filaColumna = filaActual + "," + 53;
                            if (esNumeroEntero(fila.Ano32.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano33.ToString()))
                        {
                            string filaColumna = filaActual + "," + 54;
                            if (esNumeroEntero(fila.Ano33.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano34.ToString()))
                        {
                            string filaColumna = filaActual + "," + 55;
                            if (esNumeroEntero(fila.Ano34.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano35.ToString()))
                        {
                            string filaColumna = filaActual + "," + 56;
                            if (esNumeroEntero(fila.Ano35.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano36.ToString()))
                        {
                            string filaColumna = filaActual + "," + 57;
                            if (esNumeroEntero(fila.Ano36.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano37.ToString()))
                        {
                            string filaColumna = filaActual + "," + 58;
                            if (esNumeroEntero(fila.Ano37.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano38.ToString()))
                        {
                            string filaColumna = filaActual + "," + 59;
                            if (esNumeroEntero(fila.Ano38.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano39.ToString()))
                        {
                            string filaColumna = filaActual + "," + 60;
                            if (esNumeroEntero(fila.Ano39.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano40.ToString()))
                        {
                            string filaColumna = filaActual + "," + 61;
                            if (esNumeroEntero(fila.Ano40.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano41.ToString()))
                        {
                            string filaColumna = filaActual + "," + 62;
                            if (esNumeroEntero(fila.Ano41.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano42.ToString()))
                        {
                            string filaColumna = filaActual + "," + 63;
                            if (esNumeroEntero(fila.Ano42.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano43.ToString()))
                        {
                            string filaColumna = filaActual + "," + 64;
                            if (esNumeroEntero(fila.Ano43.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano44.ToString()))
                        {
                            string filaColumna = filaActual + "," + 65;
                            if (esNumeroEntero(fila.Ano44.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano45.ToString()))
                        {
                            string filaColumna = filaActual + "," + 66;
                            if (esNumeroEntero(fila.Ano45.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano46.ToString()))
                        {
                            string filaColumna = filaActual + "," + 67;
                            if (esNumeroEntero(fila.Ano46.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano47.ToString()))
                        {
                            string filaColumna = filaActual + "," + 68;
                            if (esNumeroEntero(fila.Ano47.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano48.ToString()))
                        {
                            string filaColumna = filaActual + "," + 69;
                            if (esNumeroEntero(fila.Ano48.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano49.ToString()))
                        {
                            string filaColumna = filaActual + "," + 70;
                            if (esNumeroEntero(fila.Ano49.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano50.ToString()))
                        {
                            string filaColumna = filaActual + "," + 71;
                            if (esNumeroEntero(fila.Ano50.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano51.ToString()))
                        {
                            string filaColumna = filaActual + "," + 72;
                            if (esNumeroEntero(fila.Ano51.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano52.ToString()))
                        {
                            string filaColumna = filaActual + "," + 73;
                            if (esNumeroEntero(fila.Ano52.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano53.ToString()))
                        {
                            string filaColumna = filaActual + "," + 74;
                            if (esNumeroEntero(fila.Ano53.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano54.ToString()))
                        {
                            string filaColumna = filaActual + "," + 75;
                            if (esNumeroEntero(fila.Ano54.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano55.ToString()))
                        {
                            string filaColumna = filaActual + "," + 76;
                            if (esNumeroEntero(fila.Ano55.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano56.ToString()))
                        {
                            string filaColumna = filaActual + "," + 77;
                            if (esNumeroEntero(fila.Ano56.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano57.ToString()))
                        {
                            string filaColumna = filaActual + "," + 78;
                            if (esNumeroEntero(fila.Ano57.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano58.ToString()))
                        {
                            string filaColumna = filaActual + "," + 79;
                            if (esNumeroEntero(fila.Ano58.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano59.ToString()))
                        {
                            string filaColumna = filaActual + "," + 80;
                            if (esNumeroEntero(fila.Ano59.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano60.ToString()))
                        {
                            string filaColumna = filaActual + "," + 81;
                            if (esNumeroEntero(fila.Ano60.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }

                        if (!esCero(fila.Ano61.ToString()))
                        {
                            string filaColumna = filaActual + "," + 82;
                            if (esNumeroEntero(fila.Ano61.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano62.ToString()))
                        {
                            string filaColumna = filaActual + "," + 83;
                            if (esNumeroEntero(fila.Ano62.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano63.ToString()))
                        {
                            string filaColumna = filaActual + "," + 84;
                            if (esNumeroEntero(fila.Ano63.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano64.ToString()))
                        {
                            string filaColumna = filaActual + "," + 85;
                            if (esNumeroEntero(fila.Ano64.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano65.ToString()))
                        {
                            string filaColumna = filaActual + "," + 86;
                            if (esNumeroEntero(fila.Ano65.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano66.ToString()))
                        {
                            string filaColumna = filaActual + "," + 87;
                            if (esNumeroEntero(fila.Ano66.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano67.ToString()))
                        {
                            string filaColumna = filaActual + "," + 88;
                            if (esNumeroEntero(fila.Ano67.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano68.ToString()))
                        {
                            string filaColumna = filaActual + "," + 89;
                            if (esNumeroEntero(fila.Ano68.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano69.ToString()))
                        {
                            string filaColumna = filaActual + "," + 90;
                            if (esNumeroEntero(fila.Ano69.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano70.ToString()))
                        {
                            string filaColumna = filaActual + "," + 91;
                            if (esNumeroEntero(fila.Ano70.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }

                        if (!esCero(fila.Ano71.ToString()))
                        {
                            string filaColumna = filaActual + "," + 92;
                            if (esNumeroEntero(fila.Ano71.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano72.ToString()))
                        {
                            string filaColumna = filaActual + "," + 93;
                            if (esNumeroEntero(fila.Ano72.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano73.ToString()))
                        {
                            string filaColumna = filaActual + "," + 94;
                            if (esNumeroEntero(fila.Ano73.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano74.ToString()))
                        {
                            string filaColumna = filaActual + "," + 95;
                            if (esNumeroEntero(fila.Ano74.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano75.ToString()))
                        {
                            string filaColumna = filaActual + "," + 96;
                            if (esNumeroEntero(fila.Ano75.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano76.ToString()))
                        {
                            string filaColumna = filaActual + "," + 97;
                            if (esNumeroEntero(fila.Ano76.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano77.ToString()))
                        {
                            string filaColumna = filaActual + "," + 98;
                            if (esNumeroEntero(fila.Ano77.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano78.ToString()))
                        {
                            string filaColumna = filaActual + "," + 99;
                            if (esNumeroEntero(fila.Ano78.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano79.ToString()))
                        {
                            string filaColumna = filaActual + "," + 100;
                            if (esNumeroEntero(fila.Ano79.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }
                        if (!esCero(fila.Ano80.ToString()))
                        {
                            string filaColumna = filaActual + "," + 101;
                            if (esNumeroEntero(fila.Ano80.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }


                        if (!esCero(fila.Total_Capex.ToString()))
                        {
                            string filaColumna = filaActual + "," + 102;
                            if (esNumeroEntero(fila.Total_Capex.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }

                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.PidNombreProyectoAlias, fila.Fase, fila.Ponderacion_Financiera, fila.Periodos_Anteriores, fila.Enero, fila.Febrero, fila.Marzo,
                        fila.Abril, fila.Mayo, fila.Junio, fila.Julio, fila.Agosto, fila.Septiembre, fila.Octubre, fila.Noviembre, fila.Diciembre, fila.Total, fila.Ano1, fila.Ano2, fila.Ano3, fila.Ano4, fila.Ano5
                        , fila.Ano6, fila.Ano7, fila.Ano8, fila.Ano9, fila.Ano10, fila.Ano11, fila.Ano12, fila.Ano13, fila.Ano14, fila.Ano15, fila.Ano16, fila.Ano17, fila.Ano18, fila.Ano19, fila.Ano20, fila.Ano21
                        , fila.Ano22, fila.Ano23, fila.Ano24, fila.Ano25, fila.Ano26, fila.Ano27, fila.Ano28, fila.Ano29, fila.Ano30, fila.Ano31, fila.Ano32, fila.Ano33, fila.Ano34, fila.Ano35, fila.Ano36, fila.Ano37
                        , fila.Ano38, fila.Ano39, fila.Ano40, fila.Ano41, fila.Ano42, fila.Ano43, fila.Ano44, fila.Ano45, fila.Ano46, fila.Ano47, fila.Ano48, fila.Ano49, fila.Ano50, fila.Ano51, fila.Ano52, fila.Ano53
                        , fila.Ano54, fila.Ano55, fila.Ano56, fila.Ano57, fila.Ano58, fila.Ano59, fila.Ano60, fila.Ano61, fila.Ano62, fila.Ano63, fila.Ano64, fila.Ano65, fila.Ano66, fila.Ano67, fila.Ano68, fila.Ano69, fila.Ano70
                        , fila.Ano71, fila.Ano72, fila.Ano73, fila.Ano74, fila.Ano75, fila.Ano76, fila.Ano77, fila.Ano78, fila.Ano79, fila.Ano80, fila.Total_Capex);
                        ultimoPid = fila.IdPid;
                        contadorFila++;
                        filaActual++;
                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    //ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    //Utils.LogError(ExceptionResult);
                    //return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
            hashtable.Add(1, dt);
            hashtable.Add(2, numerosSeparadorMiles);
            hashtable.Add(3, numerosSeparadorMilesDecimales);
            return hashtable;
        }

        private DataTable getDataFisicoExcelEjerciciosOficiales(string token)
        {
            //Creating DataTable  
            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = todaysDate.Year;
            //Setiing Table Name  
            dt.TableName = "Fisico PP-EX";
            //Add Columns  
            dt.Columns.Add("IdPid", typeof(int));
            dt.Columns.Add("PidUsuario", typeof(string));
            dt.Columns.Add("PidCodigoIniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Nombre Proyecto(Alias)", typeof(string));
            dt.Columns.Add("Fase", typeof(string));
            dt.Columns.Add("Pond_Fina", typeof(double));
            dt.Columns.Add("Per_Ant", typeof(double));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Año" + (year + 1), typeof(double));
            dt.Columns.Add("Año " + (year + 2), typeof(double));
            dt.Columns.Add("Año " + (year + 3), typeof(double));
            dt.Columns.Add("Año " + (year + 4), typeof(double));
            dt.Columns.Add("Total_Capex", typeof(double));

            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }
                    string periodo = ((Session["anioIniciativaEjercicioOficial"] != null && !string.IsNullOrEmpty(Convert.ToString(Session["anioIniciativaEjercicioOficial"]))) ? Convert.ToString(Session["anioIniciativaEjercicioOficial"]) : "0");
                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA_FISICO_EJERCICIOS_OFICIALES", new { @usuario = usuario, @opcion = token, @periodo = periodo }, commandType: CommandType.StoredProcedure).ToList();
                    var contadorFila = 0;
                    var ultimoPid = 0;
                    foreach (var fila in contenido)
                    {
                        //Add Rows in DataTable
                        if (contadorFila > 0 && ultimoPid != fila.IdPid)
                        {
                            //dt.Rows.Add();
                        }
                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.PidNombreProyectoAlias, fila.Fase, fila.Ponderacion_Financiera, fila.Periodos_Anteriores, fila.Enero,
                        fila.Febrero, fila.Marzo, fila.Abril, fila.Mayo, fila.Junio, fila.Julio, fila.Agosto, fila.Septiembre, fila.Octubre, fila.Noviembre, fila.Diciembre, fila.Total, fila.Ano1,
                        fila.Ano2, fila.Ano3, fila.Total_Capex);
                        ultimoPid = fila.IdPid;
                        contadorFila++;
                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    //ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    //Utils.LogError(ExceptionResult);
                    //return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
            return dt;
        }

        private DataTable getDataFisicoExcelParametroVN(string tipoIniciativaSeleccionado, string anioIniciativaSeleccionado, string parametroVN)
        {
            //Creating DataTable  
            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = todaysDate.Year;
            //Setiing Table Name  
            dt.TableName = "Fisico PP-EX";
            //Add Columns  
            dt.Columns.Add("IdPid", typeof(int));
            dt.Columns.Add("PidUsuario", typeof(string));
            dt.Columns.Add("PidCodigoIniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Nombre Proyecto(Alias)", typeof(string));
            dt.Columns.Add("Fase", typeof(string));
            dt.Columns.Add("Pond_Fina", typeof(double));
            dt.Columns.Add("Per_Ant", typeof(double));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Año" + (year + 1), typeof(double));
            dt.Columns.Add("Año " + (year + 2), typeof(double));
            dt.Columns.Add("Año " + (year + 3), typeof(double));
            dt.Columns.Add("Año " + (year + 4), typeof(double));
            dt.Columns.Add("Total_Capex", typeof(double));

            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }

                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA_FISICO_PARAMETROVN", new { @periodo = anioIniciativaSeleccionado, @parametroVN = parametroVN }, commandType: CommandType.StoredProcedure).ToList();
                    var contadorFila = 0;
                    var ultimoPid = 0;
                    foreach (var fila in contenido)
                    {
                        //Add Rows in DataTable
                        if (contadorFila > 0 && ultimoPid != fila.IdPid)
                        {
                            //dt.Rows.Add();
                        }
                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.PidNombreProyectoAlias, fila.Fase, fila.Ponderacion_Financiera, fila.Periodos_Anteriores, fila.Enero,
                        fila.Febrero, fila.Marzo, fila.Abril, fila.Mayo, fila.Junio, fila.Julio, fila.Agosto, fila.Septiembre, fila.Octubre, fila.Noviembre, fila.Diciembre, fila.Total, fila.Ano1,
                        fila.Ano2, fila.Ano3, fila.Total_Capex);
                        ultimoPid = fila.IdPid;
                        contadorFila++;
                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    err.ToString();
                    //ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    //Utils.LogError(ExceptionResult);
                    //return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
            return dt;
        }

        private DataTable getDataFisicoExcel(string token)
        {
            //Creating DataTable  
            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = todaysDate.Year;
            //Setiing Table Name  
            dt.TableName = "Fisico PP-EX";
            //Add Columns  
            dt.Columns.Add("IdPid", typeof(int));
            dt.Columns.Add("PidUsuario", typeof(string));
            dt.Columns.Add("PidCodigoIniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Nombre Proyecto(Alias)", typeof(string));
            dt.Columns.Add("Fase", typeof(string));
            dt.Columns.Add("Pond_Fina", typeof(double));
            dt.Columns.Add("Per_Ant", typeof(double));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Año" + (year + 1), typeof(double));
            dt.Columns.Add("Año " + (year + 2), typeof(double));
            dt.Columns.Add("Año " + (year + 3), typeof(double));
            dt.Columns.Add("Año " + (year + 4), typeof(double));
            dt.Columns.Add("Total_Capex", typeof(double));

            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }

                    int anio = todaysDate.Year;
                    int anioMasUno = anio + 1;
                    string anioUno = string.Empty;
                    string anioDos = string.Empty;
                    var tipoIniciativaSeleccionado = Convert.ToString(Session["tipoIniciativaSeleccionado"]);
                    if (string.IsNullOrEmpty(tipoIniciativaSeleccionado))
                    {
                        tipoIniciativaSeleccionado = "0";
                    }
                    var anioIniciativaSeleccionado = Convert.ToString(Session["anioIniciativaSeleccionado"]);
                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA_FISICO", new { @usuario = usuario, @opcion = token, @periodo = anioIniciativaSeleccionado }, commandType: CommandType.StoredProcedure).ToList();
                    var contadorFila = 0;
                    var ultimoPid = 0;
                    foreach (var fila in contenido)
                    {
                        //Add Rows in DataTable
                        if (contadorFila > 0 && ultimoPid != fila.IdPid)
                        {
                            //dt.Rows.Add();
                        }
                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.PidNombreProyectoAlias, fila.Fase, fila.Ponderacion_Financiera, fila.Periodos_Anteriores, fila.Enero,
                        fila.Febrero, fila.Marzo, fila.Abril, fila.Mayo, fila.Junio, fila.Julio, fila.Agosto, fila.Septiembre, fila.Octubre, fila.Noviembre, fila.Diciembre, fila.Total, fila.Ano1,
                        fila.Ano2, fila.Ano3, fila.Total_Capex);
                        ultimoPid = fila.IdPid;
                        contadorFila++;
                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    //ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    //Utils.LogError(ExceptionResult);
                    //return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
            return dt;
        }

        private DataTable getDataFisicoCasoBaseExcelParametroVN(string tipoIniciativaSeleccionado, string anioIniciativaSeleccionado, string parametroVN)
        {
            //Creating DataTable  
            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = (todaysDate.Year + 1);
            //Setiing Table Name  
            dt.TableName = "Fisico CB-CD";
            //Add Columns  
            dt.Columns.Add("IdPid", typeof(int));
            dt.Columns.Add("PidUsuario", typeof(string));
            dt.Columns.Add("PidCodigoIniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Nombre Proyecto(Alias)", typeof(string));
            dt.Columns.Add("Fase", typeof(string));
            dt.Columns.Add("Pond_Fina", typeof(double));
            dt.Columns.Add("Per_Ant", typeof(double));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Año" + (year), typeof(double));
            dt.Columns.Add("Año " + (year + 1), typeof(double));
            dt.Columns.Add("Año " + (year + 2), typeof(double));
            dt.Columns.Add("Año " + (year + 3), typeof(double));
            dt.Columns.Add("Año " + (year + 4), typeof(double));
            dt.Columns.Add("Año " + (year + 5), typeof(double));
            dt.Columns.Add("Año " + (year + 6), typeof(double));
            dt.Columns.Add("Año " + (year + 7), typeof(double));
            dt.Columns.Add("Año " + (year + 8), typeof(double));
            dt.Columns.Add("Año " + (year + 9), typeof(double));
            dt.Columns.Add("Año " + (year + 10), typeof(double));
            dt.Columns.Add("Año " + (year + 11), typeof(double));
            dt.Columns.Add("Año " + (year + 12), typeof(double));
            dt.Columns.Add("Año " + (year + 13), typeof(double));
            dt.Columns.Add("Año " + (year + 14), typeof(double));
            dt.Columns.Add("Año " + (year + 15), typeof(double));
            dt.Columns.Add("Año " + (year + 16), typeof(double));
            dt.Columns.Add("Año " + (year + 17), typeof(double));
            dt.Columns.Add("Año " + (year + 18), typeof(double));
            dt.Columns.Add("Año " + (year + 19), typeof(double));
            dt.Columns.Add("Año " + (year + 20), typeof(double));
            dt.Columns.Add("Año " + (year + 21), typeof(double));
            dt.Columns.Add("Año " + (year + 22), typeof(double));
            dt.Columns.Add("Año " + (year + 23), typeof(double));
            dt.Columns.Add("Año " + (year + 24), typeof(double));
            dt.Columns.Add("Año " + (year + 25), typeof(double));
            dt.Columns.Add("Año " + (year + 26), typeof(double));
            dt.Columns.Add("Año " + (year + 27), typeof(double));
            dt.Columns.Add("Año " + (year + 28), typeof(double));
            dt.Columns.Add("Año " + (year + 29), typeof(double));
            dt.Columns.Add("Año " + (year + 30), typeof(double));
            dt.Columns.Add("Año " + (year + 31), typeof(double));
            dt.Columns.Add("Año " + (year + 32), typeof(double));
            dt.Columns.Add("Año " + (year + 33), typeof(double));
            dt.Columns.Add("Año " + (year + 34), typeof(double));
            dt.Columns.Add("Año " + (year + 35), typeof(double));
            dt.Columns.Add("Año " + (year + 36), typeof(double));
            dt.Columns.Add("Año " + (year + 37), typeof(double));
            dt.Columns.Add("Año " + (year + 38), typeof(double));
            dt.Columns.Add("Año " + (year + 39), typeof(double));
            dt.Columns.Add("Año " + (year + 40), typeof(double));
            dt.Columns.Add("Año " + (year + 41), typeof(double));
            dt.Columns.Add("Año " + (year + 42), typeof(double));
            dt.Columns.Add("Año " + (year + 43), typeof(double));
            dt.Columns.Add("Año " + (year + 44), typeof(double));
            dt.Columns.Add("Año " + (year + 45), typeof(double));
            dt.Columns.Add("Año " + (year + 46), typeof(double));
            dt.Columns.Add("Año " + (year + 47), typeof(double));
            dt.Columns.Add("Año " + (year + 48), typeof(double));
            dt.Columns.Add("Año " + (year + 49), typeof(double));
            dt.Columns.Add("Año " + (year + 50), typeof(double));
            dt.Columns.Add("Año " + (year + 51), typeof(double));
            dt.Columns.Add("Año " + (year + 52), typeof(double));
            dt.Columns.Add("Año " + (year + 53), typeof(double));
            dt.Columns.Add("Año " + (year + 54), typeof(double));
            dt.Columns.Add("Año " + (year + 55), typeof(double));
            dt.Columns.Add("Año " + (year + 56), typeof(double));
            dt.Columns.Add("Año " + (year + 57), typeof(double));
            dt.Columns.Add("Año " + (year + 58), typeof(double));
            dt.Columns.Add("Año " + (year + 59), typeof(double));
            dt.Columns.Add("Año " + (year + 60), typeof(double));
            dt.Columns.Add("Año " + (year + 61), typeof(double));
            dt.Columns.Add("Año " + (year + 62), typeof(double));
            dt.Columns.Add("Año " + (year + 63), typeof(double));
            dt.Columns.Add("Año " + (year + 64), typeof(double));
            dt.Columns.Add("Año " + (year + 65), typeof(double));
            dt.Columns.Add("Año " + (year + 66), typeof(double));
            dt.Columns.Add("Año " + (year + 67), typeof(double));
            dt.Columns.Add("Año " + (year + 68), typeof(double));
            dt.Columns.Add("Año " + (year + 69), typeof(double));
            dt.Columns.Add("Año " + (year + 70), typeof(double));
            dt.Columns.Add("Año " + (year + 71), typeof(double));
            dt.Columns.Add("Año " + (year + 72), typeof(double));
            dt.Columns.Add("Año " + (year + 73), typeof(double));
            dt.Columns.Add("Año " + (year + 74), typeof(double));
            dt.Columns.Add("Año " + (year + 75), typeof(double));
            dt.Columns.Add("Año " + (year + 76), typeof(double));
            dt.Columns.Add("Año " + (year + 77), typeof(double));
            dt.Columns.Add("Año " + (year + 78), typeof(double));
            dt.Columns.Add("Año " + (year + 79), typeof(double));
            dt.Columns.Add("Año " + (year + 80), typeof(double));
            dt.Columns.Add("Total_Capex", typeof(double));

            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }

                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA_FISICO_CASO_BASE_PARAMETROVN", new { @periodo = anioIniciativaSeleccionado, @parametroVN = parametroVN }, commandType: CommandType.StoredProcedure).ToList();
                    var contadorFila = 0;
                    var ultimoPid = 0;
                    foreach (var fila in contenido)
                    {
                        //Add Rows in DataTable
                        if (contadorFila > 0 && ultimoPid != fila.IdPid)
                        {
                            //dt.Rows.Add();
                        }
                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.PidNombreProyectoAlias, fila.Fase, fila.Ponderacion_Financiera, fila.Periodos_Anteriores, fila.Enero, fila.Febrero, fila.Marzo,
                        fila.Abril, fila.Mayo, fila.Junio, fila.Julio, fila.Agosto, fila.Septiembre, fila.Octubre, fila.Noviembre, fila.Diciembre, fila.Total, fila.Ano1, fila.Ano2,
                        fila.Ano3, fila.Ano4, fila.Ano5, fila.Ano6, fila.Ano7, fila.Ano8, fila.Ano9, fila.Ano10, fila.Ano11, fila.Ano12, fila.Ano13, fila.Ano14, fila.Ano15, fila.Ano16, fila.Ano17, fila.Ano18, fila.Ano19,
                        fila.Ano20, fila.Ano21, fila.Ano22, fila.Ano23, fila.Ano24, fila.Ano25, fila.Ano26, fila.Ano27, fila.Ano28, fila.Ano29, fila.Ano30, fila.Ano31, fila.Ano32, fila.Ano33, fila.Ano34, fila.Ano35, fila.Ano36,
                        fila.Ano37, fila.Ano38, fila.Ano39, fila.Ano40, fila.Ano41, fila.Ano42, fila.Ano43, fila.Ano44, fila.Ano45, fila.Ano46, fila.Ano47, fila.Ano48, fila.Ano49, fila.Ano50, fila.Ano51, fila.Ano52, fila.Ano53,
                        fila.Ano54, fila.Ano55, fila.Ano56, fila.Ano57, fila.Ano58, fila.Ano59, fila.Ano60, fila.Ano61, fila.Ano62, fila.Ano63, fila.Ano64, fila.Ano65, fila.Ano66, fila.Ano67, fila.Ano68, fila.Ano69, fila.Ano70,
                        fila.Ano71, fila.Ano72, fila.Ano73, fila.Ano74, fila.Ano75, fila.Ano76, fila.Ano77, fila.Ano78, fila.Ano79, fila.Ano80, fila.Total_Capex);
                        ultimoPid = fila.IdPid;
                        contadorFila++;
                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    err.ToString();
                    //ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    //Utils.LogError(ExceptionResult);
                    //return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
            return dt;
        }

        private DataTable getDataFisicoCasoBaseExcel(string token)
        {
            //Creating DataTable  
            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = (todaysDate.Year + 1);
            //Setiing Table Name  
            dt.TableName = "Fisico CB-CD";
            //Add Columns  
            dt.Columns.Add("IdPid", typeof(int));
            dt.Columns.Add("PidUsuario", typeof(string));
            dt.Columns.Add("PidCodigoIniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Nombre Proyecto(Alias)", typeof(string));
            dt.Columns.Add("Fase", typeof(string));
            dt.Columns.Add("Pond_Fina", typeof(double));
            dt.Columns.Add("Per_Ant", typeof(double));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Año" + (year), typeof(double));
            dt.Columns.Add("Año " + (year + 1), typeof(double));
            dt.Columns.Add("Año " + (year + 2), typeof(double));
            dt.Columns.Add("Año " + (year + 3), typeof(double));
            dt.Columns.Add("Año " + (year + 4), typeof(double));
            dt.Columns.Add("Año " + (year + 5), typeof(double));
            dt.Columns.Add("Año " + (year + 6), typeof(double));
            dt.Columns.Add("Año " + (year + 7), typeof(double));
            dt.Columns.Add("Año " + (year + 8), typeof(double));
            dt.Columns.Add("Año " + (year + 9), typeof(double));
            dt.Columns.Add("Año " + (year + 10), typeof(double));
            dt.Columns.Add("Año " + (year + 11), typeof(double));
            dt.Columns.Add("Año " + (year + 12), typeof(double));
            dt.Columns.Add("Año " + (year + 13), typeof(double));
            dt.Columns.Add("Año " + (year + 14), typeof(double));
            dt.Columns.Add("Año " + (year + 15), typeof(double));
            dt.Columns.Add("Año " + (year + 16), typeof(double));
            dt.Columns.Add("Año " + (year + 17), typeof(double));
            dt.Columns.Add("Año " + (year + 18), typeof(double));
            dt.Columns.Add("Año " + (year + 19), typeof(double));
            dt.Columns.Add("Año " + (year + 20), typeof(double));
            dt.Columns.Add("Año " + (year + 21), typeof(double));
            dt.Columns.Add("Año " + (year + 22), typeof(double));
            dt.Columns.Add("Año " + (year + 23), typeof(double));
            dt.Columns.Add("Año " + (year + 24), typeof(double));
            dt.Columns.Add("Año " + (year + 25), typeof(double));
            dt.Columns.Add("Año " + (year + 26), typeof(double));
            dt.Columns.Add("Año " + (year + 27), typeof(double));
            dt.Columns.Add("Año " + (year + 28), typeof(double));
            dt.Columns.Add("Año " + (year + 29), typeof(double));
            dt.Columns.Add("Año " + (year + 30), typeof(double));
            dt.Columns.Add("Año " + (year + 31), typeof(double));
            dt.Columns.Add("Año " + (year + 32), typeof(double));
            dt.Columns.Add("Año " + (year + 33), typeof(double));
            dt.Columns.Add("Año " + (year + 34), typeof(double));
            dt.Columns.Add("Año " + (year + 35), typeof(double));
            dt.Columns.Add("Año " + (year + 36), typeof(double));
            dt.Columns.Add("Año " + (year + 37), typeof(double));
            dt.Columns.Add("Año " + (year + 38), typeof(double));
            dt.Columns.Add("Año " + (year + 39), typeof(double));
            dt.Columns.Add("Año " + (year + 40), typeof(double));
            dt.Columns.Add("Año " + (year + 41), typeof(double));
            dt.Columns.Add("Año " + (year + 42), typeof(double));
            dt.Columns.Add("Año " + (year + 43), typeof(double));
            dt.Columns.Add("Año " + (year + 44), typeof(double));
            dt.Columns.Add("Año " + (year + 45), typeof(double));
            dt.Columns.Add("Año " + (year + 46), typeof(double));
            dt.Columns.Add("Año " + (year + 47), typeof(double));
            dt.Columns.Add("Año " + (year + 48), typeof(double));
            dt.Columns.Add("Año " + (year + 49), typeof(double));
            dt.Columns.Add("Año " + (year + 50), typeof(double));
            dt.Columns.Add("Año " + (year + 51), typeof(double));
            dt.Columns.Add("Año " + (year + 52), typeof(double));
            dt.Columns.Add("Año " + (year + 53), typeof(double));
            dt.Columns.Add("Año " + (year + 54), typeof(double));
            dt.Columns.Add("Año " + (year + 55), typeof(double));
            dt.Columns.Add("Año " + (year + 56), typeof(double));
            dt.Columns.Add("Año " + (year + 57), typeof(double));
            dt.Columns.Add("Año " + (year + 58), typeof(double));
            dt.Columns.Add("Año " + (year + 59), typeof(double));
            dt.Columns.Add("Año " + (year + 60), typeof(double));
            dt.Columns.Add("Año " + (year + 61), typeof(double));
            dt.Columns.Add("Año " + (year + 62), typeof(double));
            dt.Columns.Add("Año " + (year + 63), typeof(double));
            dt.Columns.Add("Año " + (year + 64), typeof(double));
            dt.Columns.Add("Año " + (year + 65), typeof(double));
            dt.Columns.Add("Año " + (year + 66), typeof(double));
            dt.Columns.Add("Año " + (year + 67), typeof(double));
            dt.Columns.Add("Año " + (year + 68), typeof(double));
            dt.Columns.Add("Año " + (year + 69), typeof(double));
            dt.Columns.Add("Año " + (year + 70), typeof(double));
            dt.Columns.Add("Año " + (year + 71), typeof(double));
            dt.Columns.Add("Año " + (year + 72), typeof(double));
            dt.Columns.Add("Año " + (year + 73), typeof(double));
            dt.Columns.Add("Año " + (year + 74), typeof(double));
            dt.Columns.Add("Año " + (year + 75), typeof(double));
            dt.Columns.Add("Año " + (year + 76), typeof(double));
            dt.Columns.Add("Año " + (year + 77), typeof(double));
            dt.Columns.Add("Año " + (year + 78), typeof(double));
            dt.Columns.Add("Año " + (year + 79), typeof(double));
            dt.Columns.Add("Año " + (year + 80), typeof(double));
            dt.Columns.Add("Total_Capex", typeof(double));

            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }

                    int anio = todaysDate.Year;
                    int anioMasUno = anio + 1;
                    string anioUno = string.Empty;
                    string anioDos = string.Empty;
                    var tipoIniciativaSeleccionado = Convert.ToString(Session["tipoIniciativaSeleccionado"]);
                    if (string.IsNullOrEmpty(tipoIniciativaSeleccionado))
                    {
                        tipoIniciativaSeleccionado = "0";
                    }
                    var anioIniciativaSeleccionado = Convert.ToString(Session["anioIniciativaSeleccionado"]);
                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA_FISICO_CASO_BASE", new { @usuario = usuario, @opcion = token, @periodo = anioIniciativaSeleccionado }, commandType: CommandType.StoredProcedure).ToList();
                    var contadorFila = 0;
                    var ultimoPid = 0;
                    foreach (var fila in contenido)
                    {
                        //Add Rows in DataTable
                        if (contadorFila > 0 && ultimoPid != fila.IdPid)
                        {
                            //dt.Rows.Add();
                        }
                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.PidNombreProyectoAlias, fila.Fase, fila.Ponderacion_Financiera, fila.Periodos_Anteriores, fila.Enero, fila.Febrero, fila.Marzo,
                        fila.Abril, fila.Mayo, fila.Junio, fila.Julio, fila.Agosto, fila.Septiembre, fila.Octubre, fila.Noviembre, fila.Diciembre, fila.Total, fila.Ano1, fila.Ano2,
                        fila.Ano3, fila.Ano4, fila.Ano5, fila.Ano6, fila.Ano7, fila.Ano8, fila.Ano9, fila.Ano10, fila.Ano11, fila.Ano12, fila.Ano13, fila.Ano14, fila.Ano15, fila.Ano16, fila.Ano17, fila.Ano18, fila.Ano19,
                        fila.Ano20, fila.Ano21, fila.Ano22, fila.Ano23, fila.Ano24, fila.Ano25, fila.Ano26, fila.Ano27, fila.Ano28, fila.Ano29, fila.Ano30, fila.Ano31, fila.Ano32, fila.Ano33, fila.Ano34, fila.Ano35, fila.Ano36,
                        fila.Ano37, fila.Ano38, fila.Ano39, fila.Ano40, fila.Ano41, fila.Ano42, fila.Ano43, fila.Ano44, fila.Ano45, fila.Ano46, fila.Ano47, fila.Ano48, fila.Ano49, fila.Ano50, fila.Ano51, fila.Ano52, fila.Ano53,
                        fila.Ano54, fila.Ano55, fila.Ano56, fila.Ano57, fila.Ano58, fila.Ano59, fila.Ano60, fila.Ano61, fila.Ano62, fila.Ano63, fila.Ano64, fila.Ano65, fila.Ano66, fila.Ano67, fila.Ano68, fila.Ano69, fila.Ano70,
                        fila.Ano71, fila.Ano72, fila.Ano73, fila.Ano74, fila.Ano75, fila.Ano76, fila.Ano77, fila.Ano78, fila.Ano79, fila.Ano80, fila.Total_Capex);
                        ultimoPid = fila.IdPid;
                        contadorFila++;
                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    //ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    //Utils.LogError(ExceptionResult);
                    //return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
            return dt;
        }

        private Hashtable getDataFisicoExcelFormat(string token)
        {
            Hashtable hashtable = new Hashtable();
            List<string> numerosSeparadorMiles = new List<string>();
            List<string> numerosSeparadorMilesDecimales = new List<string>();

            //Creating DataTable  
            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = todaysDate.Year;
            //Setiing Table Name  
            dt.TableName = "Fisico PP-EX";
            //Add Columns  
            dt.Columns.Add("IdPid", typeof(int));
            dt.Columns.Add("PidUsuario", typeof(string));
            dt.Columns.Add("PidCodigoIniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Fase", typeof(string));
            dt.Columns.Add("Pond_Fina", typeof(double));
            dt.Columns.Add("Per_Ant", typeof(double));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Año" + (year + 1), typeof(double));
            dt.Columns.Add("Año " + (year + 2), typeof(double));
            dt.Columns.Add("Año " + (year + 3), typeof(double));
            dt.Columns.Add("Año " + (year + 4), typeof(double));
            dt.Columns.Add("Total_Capex", typeof(double));

            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }

                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA_FISICO", new { @usuario = usuario, @opcion = token }, commandType: CommandType.StoredProcedure).ToList();
                    var contadorFila = 0;
                    var ultimoPid = 0;
                    var filaActual = 2;
                    foreach (var fila in contenido)
                    {
                        //Add Rows in DataTable
                        if (contadorFila > 0 && ultimoPid != fila.IdPid)
                        {
                            //dt.Rows.Add();
                        }

                        if (!esCero(fila.Periodos_Anteriores.ToString()))
                        {
                            string filaColumna = filaActual + "," + 7;
                            if (esNumeroEntero(fila.Periodos_Anteriores.ToString()))
                            {
                                numerosSeparadorMiles.Add(filaColumna);
                            }
                            else
                            {
                                numerosSeparadorMilesDecimales.Add(filaColumna);
                            }
                        }

                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.Fase, fila.Ponderacion_Financiera, fila.Periodos_Anteriores, fila.Enero, fila.Febrero, fila.Marzo,
                        fila.Abril, fila.Mayo, fila.Junio, fila.Julio, fila.Agosto, fila.Septiembre, fila.Octubre, fila.Noviembre, fila.Diciembre, fila.Total, fila.Ano1, fila.Ano2,
                        fila.Ano3, fila.Total_Capex);
                        ultimoPid = fila.IdPid;
                        //  contadorFila++;
                        filaActual++;
                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    //ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    //Utils.LogError(ExceptionResult);
                    //return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
            hashtable.Add(1, dt);
            hashtable.Add(2, numerosSeparadorMiles);
            hashtable.Add(3, numerosSeparadorMilesDecimales);
            return hashtable;
        }

        private DataTable getDataDotacionesExcelEjerciciosOficiales(string token)
        {
            //Creating DataTable  
            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = todaysDate.Year;
            //Setiing Table Name  
            dt.TableName = "Dotaciones PP-EX";
            //Add Columns  
            dt.Columns.Add("IdPid", typeof(int));
            dt.Columns.Add("PidUsuario", typeof(string));
            dt.Columns.Add("PidCodigoIniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Nombre Proyecto(Alias)", typeof(string));
            dt.Columns.Add("Año Dotacion", typeof(string));
            dt.Columns.Add("Sit. Proyecto", typeof(string));
            dt.Columns.Add("Sit. Faena", typeof(string));
            dt.Columns.Add("Depto.", typeof(string));
            dt.Columns.Add("Num. Contrato", typeof(string));
            dt.Columns.Add("Nombre EE.CC", typeof(string));
            dt.Columns.Add("Servicio", typeof(string));
            dt.Columns.Add("Num. Subcontrato", typeof(string));
            dt.Columns.Add("Centro Costo", typeof(string));
            dt.Columns.Add("Hoteleria", typeof(string));
            dt.Columns.Add("Alimentacion", typeof(string));
            dt.Columns.Add("Ubicacion", typeof(string));
            dt.Columns.Add("Clasificacion", typeof(string));
            dt.Columns.Add("Tipo EE.CC", typeof(string));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Dotacion", typeof(double));

            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }
                    string periodo = ((Session["anioIniciativaEjercicioOficial"] != null && !string.IsNullOrEmpty(Convert.ToString(Session["anioIniciativaEjercicioOficial"]))) ? Convert.ToString(Session["anioIniciativaEjercicioOficial"]) : "0");
                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA_DOTACION_EJERCICIOS_OFICIALES", new { @usuario = usuario, @opcion = token, @periodo = periodo }, commandType: CommandType.StoredProcedure).ToList();
                    var contadorFila = 0;
                    var ultimoPid = 0;
                    foreach (var fila in contenido)
                    {
                        //Add Rows in DataTable
                        if (contadorFila > 0 && ultimoPid != fila.IdPid)
                        {
                            //dt.Rows.Add();
                        }
                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.PidNombreProyectoAlias, fila.AnoDotacion, fila.SitProyecto, fila.SitFaena, fila.Depto,
                        fila.NumContrato, fila.NombreEECC, fila.Servicio, fila.NumSubContrato, fila.CentroCosto, fila.Hoteleria, fila.Alimentacion, fila.Ubicacion,
                        fila.Clasificacion, fila.TipoEECC, fila.Enero, fila.Febrero, fila.Marzo, fila.Abril, fila.Mayo, fila.Junio, fila.Julio, fila.Agosto,
                        fila.Septiembre, fila.Octubre, fila.Noviembre, fila.Diciembre, fila.TotalDotacionCalculado);
                        ultimoPid = fila.IdPid;
                        //  contadorFila++;
                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    //ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    //Utils.LogError(ExceptionResult);
                    //return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
            return dt;
        }

        private DataTable getDataDotacionesExcelParametroVN(string tipoIniciativaSeleccionado, string anioIniciativaSeleccionado, string parametroVN)
        {
            //Creating DataTable  
            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = todaysDate.Year;
            //Setiing Table Name  
            dt.TableName = "Dotaciones PP-EX";
            //Add Columns  
            dt.Columns.Add("IdPid", typeof(int));
            dt.Columns.Add("PidUsuario", typeof(string));
            dt.Columns.Add("PidCodigoIniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Nombre Proyecto(Alias)", typeof(string));
            dt.Columns.Add("Año Dotacion", typeof(string));
            dt.Columns.Add("Sit. Proyecto", typeof(string));
            dt.Columns.Add("Sit. Faena", typeof(string));
            dt.Columns.Add("Depto.", typeof(string));
            dt.Columns.Add("Num. Contrato", typeof(string));
            dt.Columns.Add("Nombre EE.CC", typeof(string));
            dt.Columns.Add("Servicio", typeof(string));
            dt.Columns.Add("Num. Subcontrato", typeof(string));
            dt.Columns.Add("Centro Costo", typeof(string));
            dt.Columns.Add("Hoteleria", typeof(string));
            dt.Columns.Add("Alimentacion", typeof(string));
            dt.Columns.Add("Ubicacion", typeof(string));
            dt.Columns.Add("Clasificacion", typeof(string));
            dt.Columns.Add("Tipo EE.CC", typeof(string));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Dotacion", typeof(double));

            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }
                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA_DOTACION_PARAMETROVN", new { @periodo = anioIniciativaSeleccionado, @parametroVN = parametroVN }, commandType: CommandType.StoredProcedure).ToList();
                    var contadorFila = 0;
                    var ultimoPid = 0;
                    foreach (var fila in contenido)
                    {
                        //Add Rows in DataTable
                        if (contadorFila > 0 && ultimoPid != fila.IdPid)
                        {
                            //dt.Rows.Add();
                        }
                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.PidNombreProyectoAlias, fila.AnoDotacion, fila.SitProyecto, fila.SitFaena, fila.Depto,
                        fila.NumContrato, fila.NombreEECC, fila.Servicio, fila.NumSubContrato, fila.CentroCosto, fila.Hoteleria, fila.Alimentacion, fila.Ubicacion,
                        fila.Clasificacion, fila.TipoEECC, fila.Enero, fila.Febrero, fila.Marzo, fila.Abril, fila.Mayo, fila.Junio, fila.Julio, fila.Agosto,
                        fila.Septiembre, fila.Octubre, fila.Noviembre, fila.Diciembre, fila.TotalDotacionCalculado);
                        ultimoPid = fila.IdPid;
                        //  contadorFila++;
                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    err.ToString();
                    //ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    //Utils.LogError(ExceptionResult);
                    //return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
            return dt;
        }

        private DataTable getDataDotacionesExcel(string token)
        {
            //Creating DataTable  
            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = todaysDate.Year;
            //Setiing Table Name  
            dt.TableName = "Dotaciones PP-EX";
            //Add Columns  
            dt.Columns.Add("IdPid", typeof(int));
            dt.Columns.Add("PidUsuario", typeof(string));
            dt.Columns.Add("PidCodigoIniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Nombre Proyecto(Alias)", typeof(string));
            dt.Columns.Add("Año Dotacion", typeof(string));
            dt.Columns.Add("Sit. Proyecto", typeof(string));
            dt.Columns.Add("Sit. Faena", typeof(string));
            dt.Columns.Add("Depto.", typeof(string));
            dt.Columns.Add("Num. Contrato", typeof(string));
            dt.Columns.Add("Nombre EE.CC", typeof(string));
            dt.Columns.Add("Servicio", typeof(string));
            dt.Columns.Add("Num. Subcontrato", typeof(string));
            dt.Columns.Add("Centro Costo", typeof(string));
            dt.Columns.Add("Hoteleria", typeof(string));
            dt.Columns.Add("Alimentacion", typeof(string));
            dt.Columns.Add("Ubicacion", typeof(string));
            dt.Columns.Add("Clasificacion", typeof(string));
            dt.Columns.Add("Tipo EE.CC", typeof(string));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Dotacion", typeof(double));

            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }
                    var tipoIniciativaSeleccionado = Convert.ToString(Session["tipoIniciativaSeleccionado"]);
                    if (string.IsNullOrEmpty(tipoIniciativaSeleccionado))
                    {
                        tipoIniciativaSeleccionado = "0";
                    }
                    var anioIniciativaSeleccionado = Convert.ToString(Session["anioIniciativaSeleccionado"]);
                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA_DOTACION", new { @usuario = usuario, @opcion = token, @periodo = anioIniciativaSeleccionado }, commandType: CommandType.StoredProcedure).ToList();
                    var contadorFila = 0;
                    var ultimoPid = 0;
                    foreach (var fila in contenido)
                    {
                        //Add Rows in DataTable
                        if (contadorFila > 0 && ultimoPid != fila.IdPid)
                        {
                            //dt.Rows.Add();
                        }
                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.PidNombreProyectoAlias, fila.AnoDotacion, fila.SitProyecto, fila.SitFaena, fila.Depto,
                        fila.NumContrato, fila.NombreEECC, fila.Servicio, fila.NumSubContrato, fila.CentroCosto, fila.Hoteleria, fila.Alimentacion, fila.Ubicacion,
                        fila.Clasificacion, fila.TipoEECC, fila.Enero, fila.Febrero, fila.Marzo, fila.Abril, fila.Mayo, fila.Junio, fila.Julio, fila.Agosto,
                        fila.Septiembre, fila.Octubre, fila.Noviembre, fila.Diciembre, fila.TotalDotacionCalculado);
                        ultimoPid = fila.IdPid;
                        //  contadorFila++;
                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    //ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    //Utils.LogError(ExceptionResult);
                    //return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
            return dt;
        }

        private DataTable getDataDotacionesCasoBaseExcelParametroVN(string tipoIniciativaSeleccionado, string anioIniciativaSeleccionado, string parametroVN)
        {
            //Creating DataTable  
            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = todaysDate.Year;
            //Setiing Table Name  
            dt.TableName = "Dotaciones CB-CD";
            //Add Columns  
            dt.Columns.Add("IdPid", typeof(int));
            dt.Columns.Add("PidUsuario", typeof(string));
            dt.Columns.Add("PidCodigoIniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Nombre Proyecto(Alias)", typeof(string));
            dt.Columns.Add("Año Dotacion", typeof(string));
            dt.Columns.Add("Sit. Proyecto", typeof(string));
            dt.Columns.Add("Sit. Faena", typeof(string));
            dt.Columns.Add("Depto.", typeof(string));
            dt.Columns.Add("Num. Contrato", typeof(string));
            dt.Columns.Add("Nombre EE.CC", typeof(string));
            dt.Columns.Add("Servicio", typeof(string));
            dt.Columns.Add("Num. Subcontrato", typeof(string));
            dt.Columns.Add("Centro Costo", typeof(string));
            dt.Columns.Add("Hoteleria", typeof(string));
            dt.Columns.Add("Alimentacion", typeof(string));
            dt.Columns.Add("Ubicacion", typeof(string));
            dt.Columns.Add("Clasificacion", typeof(string));
            dt.Columns.Add("Tipo EE.CC", typeof(string));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Dotacion", typeof(double));

            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }
                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA_DOTACION_CASO_BASE_PARAMETROVN", new { @periodo = anioIniciativaSeleccionado, @parametroVN = parametroVN }, commandType: CommandType.StoredProcedure).ToList();
                    var contadorFila = 0;
                    var ultimoPid = 0;
                    foreach (var fila in contenido)
                    {
                        //Add Rows in DataTable
                        if (contadorFila > 0 && ultimoPid != fila.IdPid)
                        {
                            //dt.Rows.Add();
                        }
                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.PidNombreProyectoAlias, fila.AnoDotacion, fila.SitProyecto, fila.SitFaena, fila.Depto,
                        fila.NumContrato, fila.NombreEECC, fila.Servicio, fila.NumSubContrato, fila.CentroCosto, fila.Hoteleria, fila.Alimentacion, fila.Ubicacion,
                        fila.Clasificacion, fila.TipoEECC, fila.Enero, fila.Febrero, fila.Marzo, fila.Abril, fila.Mayo, fila.Junio, fila.Julio, fila.Agosto,
                        fila.Septiembre, fila.Octubre, fila.Noviembre, fila.Diciembre, fila.TotalDotacionCalculado);
                        ultimoPid = fila.IdPid;
                        //  contadorFila++;
                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    err.ToString();
                    //ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    //Utils.LogError(ExceptionResult);
                    //return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
            return dt;
        }

        private DataTable getDataDotacionesCasoBaseExcel(string token)
        {
            //Creating DataTable  
            DataTable dt = new DataTable();
            DateTime todaysDate = DateTime.Now.Date;
            int year = todaysDate.Year;
            //Setiing Table Name  
            dt.TableName = "Dotaciones CB-CD";
            //Add Columns  
            dt.Columns.Add("IdPid", typeof(int));
            dt.Columns.Add("PidUsuario", typeof(string));
            dt.Columns.Add("PidCodigoIniciativa", typeof(string));
            dt.Columns.Add("Nombre Proyecto", typeof(string));
            dt.Columns.Add("Nombre Proyecto(Alias)", typeof(string));
            dt.Columns.Add("Año Dotacion", typeof(string));
            dt.Columns.Add("Sit. Proyecto", typeof(string));
            dt.Columns.Add("Sit. Faena", typeof(string));
            dt.Columns.Add("Depto.", typeof(string));
            dt.Columns.Add("Num. Contrato", typeof(string));
            dt.Columns.Add("Nombre EE.CC", typeof(string));
            dt.Columns.Add("Servicio", typeof(string));
            dt.Columns.Add("Num. Subcontrato", typeof(string));
            dt.Columns.Add("Centro Costo", typeof(string));
            dt.Columns.Add("Hoteleria", typeof(string));
            dt.Columns.Add("Alimentacion", typeof(string));
            dt.Columns.Add("Ubicacion", typeof(string));
            dt.Columns.Add("Clasificacion", typeof(string));
            dt.Columns.Add("Tipo EE.CC", typeof(string));
            dt.Columns.Add("Ene", typeof(double));
            dt.Columns.Add("Feb", typeof(double));
            dt.Columns.Add("Mar", typeof(double));
            dt.Columns.Add("Abr", typeof(double));
            dt.Columns.Add("May", typeof(double));
            dt.Columns.Add("Jun", typeof(double));
            dt.Columns.Add("Jul", typeof(double));
            dt.Columns.Add("Ago", typeof(double));
            dt.Columns.Add("Sep", typeof(double));
            dt.Columns.Add("Oct", typeof(double));
            dt.Columns.Add("Nov", typeof(double));
            dt.Columns.Add("Dic", typeof(double));
            dt.Columns.Add("Total Dotacion", typeof(double));

            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (rol.Contains("Administrador1") || rol.Contains("Administrador2") || rol.Contains("Administrador3"))
                    {
                        usuario = "";
                    }

                    int anio = todaysDate.Year;
                    int anioMasUno = anio + 1;
                    string anioUno = string.Empty;
                    string anioDos = string.Empty;
                    var tipoIniciativaSeleccionado = Convert.ToString(Session["tipoIniciativaSeleccionado"]);
                    if (string.IsNullOrEmpty(tipoIniciativaSeleccionado))
                    {
                        tipoIniciativaSeleccionado = "0";
                    }
                    var anioIniciativaSeleccionado = Convert.ToString(Session["anioIniciativaSeleccionado"]);
                    var contenido = SqlMapper.Query(objConnection, "CAPEX_SEL_REPORTE_INICIATIVA_DOTACION_CASO_BASE", new { @usuario = usuario, @opcion = token, @periodo = anioIniciativaSeleccionado }, commandType: CommandType.StoredProcedure).ToList();
                    var contadorFila = 0;
                    var ultimoPid = 0;
                    foreach (var fila in contenido)
                    {
                        //Add Rows in DataTable
                        if (contadorFila > 0 && ultimoPid != fila.IdPid)
                        {
                            //dt.Rows.Add();
                        }
                        dt.Rows.Add(fila.IdPid, fila.PidUsuario, fila.PidCodigoIniciativa, fila.PidNombreProyecto, fila.PidNombreProyectoAlias, fila.AnoDotacion, fila.SitProyecto, fila.SitFaena, fila.Depto,
                        fila.NumContrato, fila.NombreEECC, fila.Servicio, fila.NumSubContrato, fila.CentroCosto, fila.Hoteleria, fila.Alimentacion, fila.Ubicacion,
                        fila.Clasificacion, fila.TipoEECC, fila.Enero, fila.Febrero, fila.Marzo, fila.Abril, fila.Mayo, fila.Junio, fila.Julio, fila.Agosto,
                        fila.Septiembre, fila.Octubre, fila.Noviembre, fila.Diciembre, fila.TotalDotacionCalculado);
                        ultimoPid = fila.IdPid;
                        //  contadorFila++;
                    }
                    dt.AcceptChanges();
                }
                catch (Exception err)
                {
                    //ExceptionResult = AppModule + "PdfCasoBase, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    //Utils.LogError(ExceptionResult);
                    //return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
            return dt;
        }

        /// <summary>
        /// METODO GENERADOR PDF
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ActionResult ExcelResumenIniciativaParametroVN()
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string tipoIniciativaOrientacionComercial = ((Session["tipoIniciativaOrientacionComercial"] != null) ? Convert.ToString(Session["tipoIniciativaOrientacionComercial"]) : "");
                string anioIniciativaOrientacionComercial = ((Session["anioIniciativaOrientacionComercial"] != null) ? Convert.ToString(Session["anioIniciativaOrientacionComercial"]) : "");
                string parametroVNToken = ((Session["ParametroVNToken"] != null) ? Convert.ToString(Session["ParametroVNToken"]) : "");
                bool esParametroVNToken = ((!string.IsNullOrEmpty(tipoIniciativaOrientacionComercial) && !string.IsNullOrEmpty(anioIniciativaOrientacionComercial) && !string.IsNullOrEmpty(parametroVNToken)) ? true : false);
                int hojaResumen = 0;
                int hojaResumenCasoBase = 0;
                int hojaPresupuesto = 0;
                int hojaPresupuestoCasoBase = 0;
                int hojaFisico = 0;
                int hojaFisicoCasoBase = 0;
                int hojaDotacion = 0;
                int hojaDotacionCasoBase = 0;
                int numeroHoja = 1;

                //DataTable dt = getDataExcel(token);
                DataTable dtResumen = null;
                IList<string> dtNumerosSeparadorMilesResumen = null;
                IList<string> dtNumerosSeparadorMilesDecimalesResumen = null;

                Presupuesto.ParametroOrientacionVN parametroVN = getParametroVN(parametroVNToken);
                string fileName = String.Empty;
                if (tipoIniciativaOrientacionComercial.Equals("1"))//CASO BASE
                {
                    fileName = "V" + parametroVN.PVNVERSION + "_ParamComerciales_CB_" + parametroVN.PVNPERIODO;
                }
                else if (tipoIniciativaOrientacionComercial.Equals("2"))//PRESUPUESTO
                {
                    fileName = "V" + parametroVN.PVNVERSION + "_ParamComerciales_PP_" + parametroVN.PVNPERIODO;
                }

                fileName += "_" + CurrentMillis.Millis + ".xlsx";
                if (tipoIniciativaOrientacionComercial.Equals("2"))
                {
                    hojaResumen = numeroHoja++;
                    Hashtable htFormat = getDataExcelFormatParametroVN(tipoIniciativaOrientacionComercial, anioIniciativaOrientacionComercial, parametroVNToken);
                    dtResumen = (DataTable)htFormat[1];
                    dtNumerosSeparadorMilesResumen = (System.Collections.Generic.List<string>)htFormat[2];
                    dtNumerosSeparadorMilesDecimalesResumen = (System.Collections.Generic.List<string>)htFormat[3];
                }
                DataTable dtResumenCasoBase = null;
                IList<string> dtNumerosSeparadorMilesResumenCasoBase = null;
                IList<string> dtNumerosSeparadorMilesDecimalesResumenCasoBase = null;
                if (tipoIniciativaOrientacionComercial.Equals("1"))
                {
                    hojaResumenCasoBase = numeroHoja++;
                    Hashtable htFormat = getDataExcelCasoBaseFormatParametroVN(tipoIniciativaOrientacionComercial, anioIniciativaOrientacionComercial, parametroVNToken);
                    dtResumenCasoBase = (DataTable)htFormat[1];
                    dtNumerosSeparadorMilesResumenCasoBase = (System.Collections.Generic.List<string>)htFormat[2];
                    dtNumerosSeparadorMilesDecimalesResumenCasoBase = (System.Collections.Generic.List<string>)htFormat[3];
                }


                //2 PRESUPUESTO|| 1 CASO BASE
                DataTable dtFinanciero = null;
                IList<string> dtNumerosSeparadorMilesFinanciero = null;
                IList<string> dtNumerosSeparadorMilesDecimalesFinanciero = null;
                if (tipoIniciativaOrientacionComercial.Equals("2"))
                {
                    hojaPresupuesto = numeroHoja++;
                    Hashtable htFormatFinanciero = getDataFinancieroExcelFormatParametroVN(tipoIniciativaOrientacionComercial, anioIniciativaOrientacionComercial, parametroVNToken);
                    //DataTable dtFinanciero = getDataFinancieroExcel(token);
                    dtFinanciero = (DataTable)htFormatFinanciero[1];
                    dtNumerosSeparadorMilesFinanciero = (System.Collections.Generic.List<string>)htFormatFinanciero[2];
                    dtNumerosSeparadorMilesDecimalesFinanciero = (System.Collections.Generic.List<string>)htFormatFinanciero[3];
                }

                DataTable dtFinancieroCasoBase = null;
                IList<string> dtNumerosSeparadorMilesFinancieroCasoBase = null;
                IList<string> dtNumerosSeparadorMilesDecimalesFinancieroCasoBase = null;
                if (tipoIniciativaOrientacionComercial.Equals("1"))
                {
                    hojaPresupuestoCasoBase = numeroHoja++;
                    Hashtable htFormatFinancieroCasoBase = getDataFinancieroCasoBaseExcelFormatParametroVN(tipoIniciativaOrientacionComercial, anioIniciativaOrientacionComercial, parametroVNToken);
                    dtFinancieroCasoBase = (DataTable)htFormatFinancieroCasoBase[1];
                    dtNumerosSeparadorMilesFinancieroCasoBase = (System.Collections.Generic.List<string>)htFormatFinancieroCasoBase[2];
                    dtNumerosSeparadorMilesDecimalesFinancieroCasoBase = (System.Collections.Generic.List<string>)htFormatFinancieroCasoBase[3];
                }

                DataTable dtFisico = null;
                if (tipoIniciativaOrientacionComercial.Equals("2"))
                {
                    hojaFisico = numeroHoja++;
                    dtFisico = getDataFisicoExcelParametroVN(tipoIniciativaOrientacionComercial, anioIniciativaOrientacionComercial, parametroVNToken);
                }

                DataTable dtFisicoCasoBase = null;
                if (tipoIniciativaOrientacionComercial.Equals("1"))
                {
                    hojaFisicoCasoBase = numeroHoja++;
                    dtFisicoCasoBase = getDataFisicoCasoBaseExcelParametroVN(tipoIniciativaOrientacionComercial, anioIniciativaOrientacionComercial, parametroVNToken);
                }

                DataTable dtDotaciones = null;
                if (tipoIniciativaOrientacionComercial.Equals("2"))
                {
                    hojaDotacion = numeroHoja++;
                    dtDotaciones = getDataDotacionesExcelParametroVN(tipoIniciativaOrientacionComercial, anioIniciativaOrientacionComercial, parametroVNToken);
                }

                DataTable dtDotacionesCasoBase = null;
                if (tipoIniciativaOrientacionComercial.Equals("1"))
                {
                    hojaDotacionCasoBase = numeroHoja++;
                    dtDotacionesCasoBase = getDataDotacionesCasoBaseExcelParametroVN(tipoIniciativaOrientacionComercial, anioIniciativaOrientacionComercial, parametroVNToken);
                }

                using (XLWorkbook wb = new XLWorkbook())
                {
                    if (dtResumen != null)
                    {
                        //Add DataTable in worksheet 
                        wb.Worksheets.Add(dtResumen);
                        ClosedXML.Excel.IXLWorksheet xlWorksheet = wb.Worksheet(hojaResumen);
                        foreach (string value in dtNumerosSeparadorMilesResumen)
                        {
                            string[] posiciones = value.Split(',');
                            try
                            {
                                var posx = System.Convert.ToInt32(posiciones[0]);
                                var posy = System.Convert.ToInt32(posiciones[1]);
                                ClosedXML.Excel.IXLCell cell = xlWorksheet.Cell(posx, posy);
                                //cell.Style.NumberFormat.Format = "#,##0";
                                cell.Style.NumberFormat.Format = "#,##0";
                            }
                            catch (FormatException)
                            {

                            }
                        }
                        foreach (string value in dtNumerosSeparadorMilesDecimalesResumen)
                        {
                            string[] posiciones = value.Split(',');
                            try
                            {
                                var posx = System.Convert.ToInt32(posiciones[0]);
                                var posy = System.Convert.ToInt32(posiciones[1]);
                                ClosedXML.Excel.IXLCell cell = xlWorksheet.Cell(posx, posy);
                                //cell.Style.NumberFormat.Format = "#,##0.##";// #,##0.00 //#.##0,#0
                                //cell.Style.NumberFormat.Format = "$ #,##0.00";
                                cell.Style.NumberFormat.Format = "#,##0.00";
                            }
                            catch (FormatException)
                            {

                            }
                        }
                    }
                    if (dtResumenCasoBase != null)
                    {
                        //Add DataTable in worksheet 
                        wb.Worksheets.Add(dtResumenCasoBase);
                        ClosedXML.Excel.IXLWorksheet xlWorksheet = wb.Worksheet(hojaResumenCasoBase);
                        foreach (string value in dtNumerosSeparadorMilesResumenCasoBase)
                        {
                            string[] posiciones = value.Split(',');
                            try
                            {
                                var posx = System.Convert.ToInt32(posiciones[0]);
                                var posy = System.Convert.ToInt32(posiciones[1]);
                                ClosedXML.Excel.IXLCell cell = xlWorksheet.Cell(posx, posy);
                                //cell.Style.NumberFormat.Format = "#,##0";//#,##0
                                cell.Style.NumberFormat.Format = "#,##0";
                            }
                            catch (FormatException)
                            {

                            }
                        }
                        foreach (string value in dtNumerosSeparadorMilesDecimalesResumenCasoBase)
                        {
                            string[] posiciones = value.Split(',');
                            try
                            {
                                var posx = System.Convert.ToInt32(posiciones[0]);
                                var posy = System.Convert.ToInt32(posiciones[1]);
                                ClosedXML.Excel.IXLCell cell = xlWorksheet.Cell(posx, posy);
                                cell.Style.NumberFormat.Format = "#,##0.##";// #,##0.00 //#.##0,#0
                                //cell.Style.NumberFormat.Format = "$ #,##0.00";
                            }
                            catch (FormatException)
                            {

                            }
                        }
                    }
                    if (dtFinanciero != null)
                    {
                        wb.Worksheets.Add(dtFinanciero);
                        ClosedXML.Excel.IXLWorksheet xlWorksheetFinanciero = wb.Worksheet(hojaPresupuesto);
                        foreach (string value in dtNumerosSeparadorMilesFinanciero)
                        {
                            string[] posiciones = value.Split(',');
                            try
                            {
                                var posx = System.Convert.ToInt32(posiciones[0]);
                                var posy = System.Convert.ToInt32(posiciones[1]);
                                ClosedXML.Excel.IXLCell cell = xlWorksheetFinanciero.Cell(posx, posy);
                                cell.Style.NumberFormat.Format = "#,##0";//#,##0
                            }
                            catch (FormatException)
                            {

                            }
                        }
                        foreach (string value in dtNumerosSeparadorMilesDecimalesFinanciero)
                        {
                            string[] posiciones = value.Split(',');
                            try
                            {
                                var posx = System.Convert.ToInt32(posiciones[0]);
                                var posy = System.Convert.ToInt32(posiciones[1]);
                                ClosedXML.Excel.IXLCell cell = xlWorksheetFinanciero.Cell(posx, posy);
                                cell.Style.NumberFormat.Format = "#,##0.##";// #,##0.00 //#.##0,#0
                                //cell.Style.NumberFormat.Format = "$ #,##0.00";
                            }
                            catch (FormatException)
                            {

                            }
                        }
                    }
                    if (dtFinancieroCasoBase != null)
                    {
                        wb.Worksheets.Add(dtFinancieroCasoBase);
                        ClosedXML.Excel.IXLWorksheet xlWorksheetFinancieroCasoBase = wb.Worksheet(hojaPresupuestoCasoBase);
                        foreach (string value in dtNumerosSeparadorMilesFinancieroCasoBase)
                        {
                            string[] posiciones = value.Split(',');
                            try
                            {
                                var posx = System.Convert.ToInt32(posiciones[0]);
                                var posy = System.Convert.ToInt32(posiciones[1]);
                                ClosedXML.Excel.IXLCell cell = xlWorksheetFinancieroCasoBase.Cell(posx, posy);
                                cell.Style.NumberFormat.Format = "#,##0";//#,##0
                            }
                            catch (FormatException)
                            {

                            }
                        }
                        foreach (string value in dtNumerosSeparadorMilesDecimalesFinancieroCasoBase)
                        {
                            string[] posiciones = value.Split(',');
                            try
                            {
                                var posx = System.Convert.ToInt32(posiciones[0]);
                                var posy = System.Convert.ToInt32(posiciones[1]);
                                ClosedXML.Excel.IXLCell cell = xlWorksheetFinancieroCasoBase.Cell(posx, posy);
                                cell.Style.NumberFormat.Format = "#,##0.##";// #,##0.00 //#.##0,#0
                                //cell.Style.NumberFormat.Format = "$ #,##0.00";
                            }
                            catch (FormatException)
                            {

                            }
                        }
                    }
                    if (dtFisico != null)
                    {
                        wb.Worksheets.Add(dtFisico);
                    }
                    if (dtFisicoCasoBase != null)
                    {
                        wb.Worksheets.Add(dtFisicoCasoBase);
                    }
                    if (dtDotaciones != null)
                    {
                        wb.Worksheets.Add(dtDotaciones);
                    }
                    if (dtDotacionesCasoBase != null)
                    {
                        wb.Worksheets.Add(dtDotacionesCasoBase);
                    }
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
        }


        /// <summary>
        /// METODO GENERADOR PDF
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ActionResult ExcelResumenIniciativa(string token)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var tipoIniciativaSeleccionado = Convert.ToString(Session["tipoIniciativaSeleccionado"]);
                if (string.IsNullOrEmpty(tipoIniciativaSeleccionado))
                {
                    tipoIniciativaSeleccionado = "0";
                }
                int hojaResumen = 0;
                int hojaResumenCasoBase = 0;
                int hojaPresupuesto = 0;
                int hojaPresupuestoCasoBase = 0;
                int hojaFisico = 0;
                int hojaFisicoCasoBase = 0;
                int hojaDotacion = 0;
                int hojaDotacionCasoBase = 0;
                int numeroHoja = 1;

                if ("GestionNoAprobada".Equals(token, StringComparison.OrdinalIgnoreCase))
                {
                    if ("1".Equals(Convert.ToString(Session["CAPEX_SESS_OPCION_ENVIADA"])))
                    {
                        token = "GestionNoAprobadaGAF";
                    }
                    else if ("2".Equals(Convert.ToString(Session["CAPEX_SESS_OPCION_ENVIADA"])))
                    {
                        token = "GestionNoAprobadaCE";
                    }
                    else if ("3".Equals(Convert.ToString(Session["CAPEX_SESS_OPCION_ENVIADA"])))
                    {
                        token = "GestionNoAprobadaAMSA";
                    }
                }

                //DataTable dt = getDataExcel(token);
                DataTable dtResumen = null;
                IList<string> dtNumerosSeparadorMilesResumen = null;
                IList<string> dtNumerosSeparadorMilesDecimalesResumen = null;

                string fileName = String.Empty;
                if (tipoIniciativaSeleccionado.Equals("0"))//AMBAS
                {
                    fileName = "CB_PP_";
                }
                else if (tipoIniciativaSeleccionado.Equals("1"))//PP
                {
                    fileName = "PP_";
                }
                else
                {
                    fileName = "CB_";
                }
                string middle = string.Empty;
                if (!string.IsNullOrEmpty(token))
                {
                    if ("GestionIngresada".Equals(token))
                    {
                        middle = "Revision C.E.";
                    }
                    else if ("GestionEnRevision".Equals(token))
                    {
                        middle = "Revision AMSA";
                    }
                    else if ("GestionAprobadaAmsa".Equals(token))
                    {
                        middle = "Revision Aprobada Amsa";
                    }
                    else if ("GestionNoAprobadaGAF".Equals(token))
                    {
                        middle = "No Aprobada GAF";
                    }
                    else if ("GestionNoAprobadaCE".Equals(token))
                    {
                        middle = "No Aprobada C.E.";
                    }
                    else if ("GestionNoAprobadaAMSA".Equals(token))
                    {
                        middle = "No Aprobada AMSA";
                    }
                    else
                    {
                        middle = token.Replace("Gestion", "");
                    }
                }
                else
                {
                    middle = "Resumen";
                }
                fileName += middle + "_" + CurrentMillis.Millis + ".xlsx";
                if (tipoIniciativaSeleccionado.Equals("0") || tipoIniciativaSeleccionado.Equals("1"))
                {
                    hojaResumen = numeroHoja++;
                    Hashtable htFormat = getDataExcelFormat(token);
                    dtResumen = (DataTable)htFormat[1];
                    dtNumerosSeparadorMilesResumen = (System.Collections.Generic.List<string>)htFormat[2];
                    dtNumerosSeparadorMilesDecimalesResumen = (System.Collections.Generic.List<string>)htFormat[3];
                }
                DataTable dtResumenCasoBase = null;
                IList<string> dtNumerosSeparadorMilesResumenCasoBase = null;
                IList<string> dtNumerosSeparadorMilesDecimalesResumenCasoBase = null;
                if (tipoIniciativaSeleccionado.Equals("0") || tipoIniciativaSeleccionado.Equals("2"))
                {
                    hojaResumenCasoBase = numeroHoja++;
                    Hashtable htFormat = getDataExcelCasoBaseFormat(token);
                    dtResumenCasoBase = (DataTable)htFormat[1];
                    dtNumerosSeparadorMilesResumenCasoBase = (System.Collections.Generic.List<string>)htFormat[2];
                    dtNumerosSeparadorMilesDecimalesResumenCasoBase = (System.Collections.Generic.List<string>)htFormat[3];
                }


                //0 TODOS||1 PRESUPUESTO|| 2 CASO BASE
                DataTable dtFinanciero = null;
                IList<string> dtNumerosSeparadorMilesFinanciero = null;
                IList<string> dtNumerosSeparadorMilesDecimalesFinanciero = null;
                if (tipoIniciativaSeleccionado.Equals("0") || tipoIniciativaSeleccionado.Equals("1"))
                {
                    hojaPresupuesto = numeroHoja++;
                    Hashtable htFormatFinanciero = getDataFinancieroExcelFormat(token);
                    //DataTable dtFinanciero = getDataFinancieroExcel(token);
                    dtFinanciero = (DataTable)htFormatFinanciero[1];
                    dtNumerosSeparadorMilesFinanciero = (System.Collections.Generic.List<string>)htFormatFinanciero[2];
                    dtNumerosSeparadorMilesDecimalesFinanciero = (System.Collections.Generic.List<string>)htFormatFinanciero[3];
                }

                DataTable dtFinancieroCasoBase = null;
                IList<string> dtNumerosSeparadorMilesFinancieroCasoBase = null;
                IList<string> dtNumerosSeparadorMilesDecimalesFinancieroCasoBase = null;
                if (tipoIniciativaSeleccionado.Equals("0") || tipoIniciativaSeleccionado.Equals("2"))
                {
                    hojaPresupuestoCasoBase = numeroHoja++;
                    Hashtable htFormatFinancieroCasoBase = getDataFinancieroCasoBaseExcelFormat(token);
                    dtFinancieroCasoBase = (DataTable)htFormatFinancieroCasoBase[1];
                    dtNumerosSeparadorMilesFinancieroCasoBase = (System.Collections.Generic.List<string>)htFormatFinancieroCasoBase[2];
                    dtNumerosSeparadorMilesDecimalesFinancieroCasoBase = (System.Collections.Generic.List<string>)htFormatFinancieroCasoBase[3];
                }

                DataTable dtFisico = null;
                if (tipoIniciativaSeleccionado.Equals("0") || tipoIniciativaSeleccionado.Equals("1"))
                {
                    hojaFisico = numeroHoja++;
                    dtFisico = getDataFisicoExcel(token);
                }

                DataTable dtFisicoCasoBase = null;
                if (tipoIniciativaSeleccionado.Equals("0") || tipoIniciativaSeleccionado.Equals("2"))
                {
                    hojaFisicoCasoBase = numeroHoja++;
                    dtFisicoCasoBase = getDataFisicoCasoBaseExcel(token);
                }

                DataTable dtDotaciones = null;
                if (tipoIniciativaSeleccionado.Equals("0") || tipoIniciativaSeleccionado.Equals("1"))
                {
                    hojaDotacion = numeroHoja++;
                    dtDotaciones = getDataDotacionesExcel(token);
                }

                DataTable dtDotacionesCasoBase = null;
                if (tipoIniciativaSeleccionado.Equals("0") || tipoIniciativaSeleccionado.Equals("2"))
                {
                    hojaDotacionCasoBase = numeroHoja++;
                    dtDotacionesCasoBase = getDataDotacionesCasoBaseExcel(token);
                }



                using (XLWorkbook wb = new XLWorkbook())
                {
                    if (dtResumen != null)
                    {
                        //Add DataTable in worksheet 
                        wb.Worksheets.Add(dtResumen);
                        ClosedXML.Excel.IXLWorksheet xlWorksheet = wb.Worksheet(hojaResumen);
                        foreach (string value in dtNumerosSeparadorMilesResumen)
                        {
                            string[] posiciones = value.Split(',');
                            try
                            {
                                var posx = System.Convert.ToInt32(posiciones[0]);
                                var posy = System.Convert.ToInt32(posiciones[1]);
                                ClosedXML.Excel.IXLCell cell = xlWorksheet.Cell(posx, posy);
                                //cell.Style.NumberFormat.Format = "#,##0";
                                cell.Style.NumberFormat.Format = "#,##0";
                            }
                            catch (FormatException)
                            {

                            }
                        }
                        foreach (string value in dtNumerosSeparadorMilesDecimalesResumen)
                        {
                            string[] posiciones = value.Split(',');
                            try
                            {
                                var posx = System.Convert.ToInt32(posiciones[0]);
                                var posy = System.Convert.ToInt32(posiciones[1]);
                                ClosedXML.Excel.IXLCell cell = xlWorksheet.Cell(posx, posy);
                                //cell.Style.NumberFormat.Format = "#,##0.##";// #,##0.00 //#.##0,#0
                                //cell.Style.NumberFormat.Format = "$ #,##0.00";
                                cell.Style.NumberFormat.Format = "#,##0.00";
                            }
                            catch (FormatException)
                            {

                            }
                        }
                    }
                    if (dtResumenCasoBase != null)
                    {
                        //Add DataTable in worksheet 
                        wb.Worksheets.Add(dtResumenCasoBase);
                        ClosedXML.Excel.IXLWorksheet xlWorksheet = wb.Worksheet(hojaResumenCasoBase);
                        foreach (string value in dtNumerosSeparadorMilesResumenCasoBase)
                        {
                            string[] posiciones = value.Split(',');
                            try
                            {
                                var posx = System.Convert.ToInt32(posiciones[0]);
                                var posy = System.Convert.ToInt32(posiciones[1]);
                                ClosedXML.Excel.IXLCell cell = xlWorksheet.Cell(posx, posy);
                                //cell.Style.NumberFormat.Format = "#,##0";//#,##0
                                cell.Style.NumberFormat.Format = "#,##0";
                            }
                            catch (FormatException)
                            {

                            }
                        }
                        foreach (string value in dtNumerosSeparadorMilesDecimalesResumenCasoBase)
                        {
                            string[] posiciones = value.Split(',');
                            try
                            {
                                var posx = System.Convert.ToInt32(posiciones[0]);
                                var posy = System.Convert.ToInt32(posiciones[1]);
                                ClosedXML.Excel.IXLCell cell = xlWorksheet.Cell(posx, posy);
                                cell.Style.NumberFormat.Format = "#,##0.##";// #,##0.00 //#.##0,#0
                                //cell.Style.NumberFormat.Format = "$ #,##0.00";
                            }
                            catch (FormatException)
                            {

                            }
                        }
                    }
                    if (dtFinanciero != null)
                    {
                        wb.Worksheets.Add(dtFinanciero);
                        ClosedXML.Excel.IXLWorksheet xlWorksheetFinanciero = wb.Worksheet(hojaPresupuesto);
                        foreach (string value in dtNumerosSeparadorMilesFinanciero)
                        {
                            string[] posiciones = value.Split(',');
                            try
                            {
                                var posx = System.Convert.ToInt32(posiciones[0]);
                                var posy = System.Convert.ToInt32(posiciones[1]);
                                ClosedXML.Excel.IXLCell cell = xlWorksheetFinanciero.Cell(posx, posy);
                                cell.Style.NumberFormat.Format = "#,##0";//#,##0
                            }
                            catch (FormatException)
                            {

                            }
                        }
                        foreach (string value in dtNumerosSeparadorMilesDecimalesFinanciero)
                        {
                            string[] posiciones = value.Split(',');
                            try
                            {
                                var posx = System.Convert.ToInt32(posiciones[0]);
                                var posy = System.Convert.ToInt32(posiciones[1]);
                                ClosedXML.Excel.IXLCell cell = xlWorksheetFinanciero.Cell(posx, posy);
                                cell.Style.NumberFormat.Format = "#,##0.##";// #,##0.00 //#.##0,#0
                                //cell.Style.NumberFormat.Format = "$ #,##0.00";
                            }
                            catch (FormatException)
                            {

                            }
                        }
                    }
                    if (dtFinancieroCasoBase != null)
                    {
                        wb.Worksheets.Add(dtFinancieroCasoBase);
                        ClosedXML.Excel.IXLWorksheet xlWorksheetFinancieroCasoBase = wb.Worksheet(hojaPresupuestoCasoBase);
                        foreach (string value in dtNumerosSeparadorMilesFinancieroCasoBase)
                        {
                            string[] posiciones = value.Split(',');
                            try
                            {
                                var posx = System.Convert.ToInt32(posiciones[0]);
                                var posy = System.Convert.ToInt32(posiciones[1]);
                                ClosedXML.Excel.IXLCell cell = xlWorksheetFinancieroCasoBase.Cell(posx, posy);
                                cell.Style.NumberFormat.Format = "#,##0";//#,##0
                            }
                            catch (FormatException)
                            {

                            }
                        }
                        foreach (string value in dtNumerosSeparadorMilesDecimalesFinancieroCasoBase)
                        {
                            string[] posiciones = value.Split(',');
                            try
                            {
                                var posx = System.Convert.ToInt32(posiciones[0]);
                                var posy = System.Convert.ToInt32(posiciones[1]);
                                ClosedXML.Excel.IXLCell cell = xlWorksheetFinancieroCasoBase.Cell(posx, posy);
                                cell.Style.NumberFormat.Format = "#,##0.##";// #,##0.00 //#.##0,#0
                                //cell.Style.NumberFormat.Format = "$ #,##0.00";
                            }
                            catch (FormatException)
                            {

                            }
                        }
                    }
                    if (dtFisico != null)
                    {
                        wb.Worksheets.Add(dtFisico);
                    }
                    if (dtFisicoCasoBase != null)
                    {
                        wb.Worksheets.Add(dtFisicoCasoBase);
                    }
                    if (dtDotaciones != null)
                    {
                        wb.Worksheets.Add(dtDotaciones);
                    }
                    if (dtDotacionesCasoBase != null)
                    {
                        wb.Worksheets.Add(dtDotacionesCasoBase);
                    }
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
        }

        /// <summary>
        /// METODO GENERADOR PDF
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ActionResult ExcelResumenIniciativaEjerciciosOficiales(string token)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                int hojaResumen = 0;
                int hojaPresupuesto = 0;
                int hojaFisico = 0;
                int hojaDotacion = 0;
                int numeroHoja = 1;
                //DataTable dt = getDataExcel(token);
                DataTable dtResumen = null;
                IList<string> dtNumerosSeparadorMilesResumen = null;
                IList<string> dtNumerosSeparadorMilesDecimalesResumen = null;

                hojaResumen = numeroHoja++;
                Hashtable htFormat = getDataExcelEjerciciosOficialesFormat(token);
                dtResumen = (DataTable)htFormat[1];
                dtNumerosSeparadorMilesResumen = (System.Collections.Generic.List<string>)htFormat[2];
                dtNumerosSeparadorMilesDecimalesResumen = (System.Collections.Generic.List<string>)htFormat[3];


                //0 TODOS||1 PRESUPUESTO|| 2 CASO BASE
                DataTable dtFinanciero = null;
                IList<string> dtNumerosSeparadorMilesFinanciero = null;
                IList<string> dtNumerosSeparadorMilesDecimalesFinanciero = null;

                hojaPresupuesto = numeroHoja++;
                Hashtable htFormatFinanciero = getDataFinancieroExcelEjerciciosOficialesFormat(token);
                //DataTable dtFinanciero = getDataFinancieroExcel(token);
                dtFinanciero = (DataTable)htFormatFinanciero[1];
                dtNumerosSeparadorMilesFinanciero = (System.Collections.Generic.List<string>)htFormatFinanciero[2];
                dtNumerosSeparadorMilesDecimalesFinanciero = (System.Collections.Generic.List<string>)htFormatFinanciero[3];


                hojaFisico = numeroHoja++;
                DataTable dtFisico = getDataFisicoExcelEjerciciosOficiales(token);

                hojaDotacion = numeroHoja++;
                DataTable dtDotaciones = getDataDotacionesExcelEjerciciosOficiales(token);


                string periodo = ((Session["anioIniciativaEjercicioOficial"] != null && !string.IsNullOrEmpty(Convert.ToString(Session["anioIniciativaEjercicioOficial"]))) ? Convert.ToString(Session["anioIniciativaEjercicioOficial"]) : "0");
                //Name of File  
                string fileName = "EjercicioOficialCapex_" + periodo + "_" + CurrentMillis.Millis + ".xlsx";
                using (XLWorkbook wb = new XLWorkbook())
                {
                    if (dtResumen != null)
                    {
                        //Add DataTable in worksheet 
                        wb.Worksheets.Add(dtResumen);
                        ClosedXML.Excel.IXLWorksheet xlWorksheet = wb.Worksheet(hojaResumen);
                        foreach (string value in dtNumerosSeparadorMilesResumen)
                        {
                            string[] posiciones = value.Split(',');
                            try
                            {
                                var posx = System.Convert.ToInt32(posiciones[0]);
                                var posy = System.Convert.ToInt32(posiciones[1]);
                                ClosedXML.Excel.IXLCell cell = xlWorksheet.Cell(posx, posy);
                                cell.Style.NumberFormat.Format = "#,##0";//#,##0
                            }
                            catch (FormatException)
                            {

                            }
                        }
                        foreach (string value in dtNumerosSeparadorMilesDecimalesResumen)
                        {
                            string[] posiciones = value.Split(',');
                            try
                            {
                                var posx = System.Convert.ToInt32(posiciones[0]);
                                var posy = System.Convert.ToInt32(posiciones[1]);
                                ClosedXML.Excel.IXLCell cell = xlWorksheet.Cell(posx, posy);
                                cell.Style.NumberFormat.Format = "#,##0.##";// #,##0.00 //#.##0,#0
                                //cell.Style.NumberFormat.Format = "$ #,##0.00";
                            }
                            catch (FormatException)
                            {

                            }
                        }
                    }

                    if (dtFinanciero != null)
                    {
                        wb.Worksheets.Add(dtFinanciero);
                        ClosedXML.Excel.IXLWorksheet xlWorksheetFinanciero = wb.Worksheet(hojaPresupuesto);
                        foreach (string value in dtNumerosSeparadorMilesFinanciero)
                        {
                            string[] posiciones = value.Split(',');
                            try
                            {
                                var posx = System.Convert.ToInt32(posiciones[0]);
                                var posy = System.Convert.ToInt32(posiciones[1]);
                                ClosedXML.Excel.IXLCell cell = xlWorksheetFinanciero.Cell(posx, posy);
                                cell.Style.NumberFormat.Format = "#,##0";//#,##0
                            }
                            catch (FormatException)
                            {

                            }
                        }
                        foreach (string value in dtNumerosSeparadorMilesDecimalesFinanciero)
                        {
                            string[] posiciones = value.Split(',');
                            try
                            {
                                var posx = System.Convert.ToInt32(posiciones[0]);
                                var posy = System.Convert.ToInt32(posiciones[1]);
                                ClosedXML.Excel.IXLCell cell = xlWorksheetFinanciero.Cell(posx, posy);
                                cell.Style.NumberFormat.Format = "#,##0.##";// #,##0.00 //#.##0,#0
                                //cell.Style.NumberFormat.Format = "$ #,##0.00";
                            }
                            catch (FormatException)
                            {

                            }
                        }
                    }

                    if (dtFisico != null)
                    {
                        wb.Worksheets.Add(dtFisico);
                    }

                    if (dtDotaciones != null)
                    {
                        wb.Worksheets.Add(dtDotaciones);
                    }

                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
        }



        #endregion

        #region "METODOS ADJUNTOS-DOCUMENTOS"
        /// <summary>
        /// METODO PARA ELIMINAR ADJUNTOS TEMPORALES
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>

        [HttpGet]
        public ActionResult EliminarAdjunto(string token)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(VA);
                    var mensaje = IPlanificacion.EliminarAdjunto(token);
                    return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    return Json(new { Mensaje = exc.Message.ToString() + " " + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        [HttpGet]
        public ActionResult EliminarAdjuntoVigente(string token)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    IPlanificacion = FactoryPlanificacion.delega(VA);
                    string mensaje = String.Empty;
                    var adjunto = IPlanificacion.SeleccionarAdjunto(token);
                    if (adjunto != null)
                    {
                        string shareFile = (!string.IsNullOrEmpty(adjunto.ShareFile) ? adjunto.ShareFile : String.Empty);
                        string pathDirectory = (!string.IsNullOrEmpty(adjunto.PathDirectory) ? adjunto.PathDirectory : String.Empty);
                        string nameFile = (!string.IsNullOrEmpty(adjunto.ParNombreFinal) ? adjunto.ParNombreFinal : ((!string.IsNullOrEmpty(adjunto.ParNombre) ? adjunto.ParNombre : String.Empty)));
                        if (!string.IsNullOrEmpty(nameFile) && !string.IsNullOrEmpty(shareFile) && !string.IsNullOrEmpty(pathDirectory))
                        {
                            //AZURE
                            UploadDownload uploadDownload = new UploadDownload();
                            if (UploadDownload.DeleteFile(shareFile, pathDirectory, nameFile))
                            {
                                mensaje = IPlanificacion.EliminarAdjuntoVigente(token, usuario);
                            }
                            else
                            {
                                //mensaje = "Ocurrió Un error inesperado al intentar eliminar el archivo. Por favor, inténtelo de nuevo más tarde";
                                mensaje = "Error";
                            }
                        }
                        else
                        {
                            mensaje = IPlanificacion.EliminarAdjuntoVigente(token, usuario);
                        }
                    }
                    else
                    {
                        //mensaje = "Ocurrió Un error el archivo no existe!";
                        mensaje = "Error";
                    }
                    return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    return Json(new { Mensaje = exc.Message.ToString() + " " + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        [HttpPost]
        public ActionResult EliminarAdjuntoVigenteConEvaluacionEconomica(string IniToken, string ParToken)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    IPlanificacion = FactoryPlanificacion.delega(VA);
                    string mensaje = String.Empty;
                    var adjunto = IPlanificacion.SeleccionarAdjunto(ParToken);
                    if (adjunto != null)
                    {
                        string shareFile = (!string.IsNullOrEmpty(adjunto.ShareFile) ? adjunto.ShareFile : String.Empty);
                        string pathDirectory = (!string.IsNullOrEmpty(adjunto.PathDirectory) ? adjunto.PathDirectory : String.Empty);
                        string nameFile = (!string.IsNullOrEmpty(adjunto.ParNombreFinal) ? adjunto.ParNombreFinal : ((!string.IsNullOrEmpty(adjunto.ParNombre) ? adjunto.ParNombre : String.Empty)));
                        if (!string.IsNullOrEmpty(nameFile) && !string.IsNullOrEmpty(shareFile) && !string.IsNullOrEmpty(pathDirectory))
                        {
                            //AZURE
                            UploadDownload uploadDownload = new UploadDownload();
                            if (UploadDownload.DeleteFile(shareFile, pathDirectory, nameFile))
                            {
                                mensaje = IPlanificacion.EliminarAdjuntoVigenteConEvaluacionEconomica(IniToken, ParToken, usuario);
                            }
                            else
                            {
                                //mensaje = "Ocurrió Un error inesperado al intentar eliminar el archivo. Por favor, inténtelo de nuevo más tarde";
                                mensaje = "Error";
                            }
                        }
                        else
                        {
                            mensaje = IPlanificacion.EliminarAdjuntoVigenteConEvaluacionEconomica(IniToken, ParToken, usuario);
                        }
                    }
                    else
                    {
                        //mensaje = "Ocurrió Un error el archivo no existe!";
                        mensaje = "Error";
                    }
                    return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    return Json(new { Mensaje = exc.Message.ToString() + " " + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        [HttpPost]
        public ActionResult EliminarAdjuntoVigenteConEvaluacionRiesgo(string IniToken, string ParToken)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    IPlanificacion = FactoryPlanificacion.delega(VA);
                    string mensaje = String.Empty;
                    var adjunto = IPlanificacion.SeleccionarAdjunto(ParToken);
                    if (adjunto != null)
                    {
                        string shareFile = (!string.IsNullOrEmpty(adjunto.ShareFile) ? adjunto.ShareFile : String.Empty);
                        string pathDirectory = (!string.IsNullOrEmpty(adjunto.PathDirectory) ? adjunto.PathDirectory : String.Empty);
                        string nameFile = (!string.IsNullOrEmpty(adjunto.ParNombreFinal) ? adjunto.ParNombreFinal : ((!string.IsNullOrEmpty(adjunto.ParNombre) ? adjunto.ParNombre : String.Empty)));
                        if (!string.IsNullOrEmpty(nameFile) && !string.IsNullOrEmpty(shareFile) && !string.IsNullOrEmpty(pathDirectory))
                        {
                            //AZURE
                            UploadDownload uploadDownload = new UploadDownload();
                            if (UploadDownload.DeleteFile(shareFile, pathDirectory, nameFile))
                            {
                                mensaje = IPlanificacion.EliminarAdjuntoVigenteConEvaluacionRiesgo(IniToken, ParToken, usuario);
                            }
                            else
                            {
                                mensaje = "Error";
                            }
                        }
                        else
                        {
                            mensaje = IPlanificacion.EliminarAdjuntoVigenteConEvaluacionRiesgo(IniToken, ParToken, usuario);
                        }
                    }
                    else
                    {
                        mensaje = "Error";
                    }
                    return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    return Json(new { Mensaje = exc.Message.ToString() + " " + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        [HttpPost]
        public ActionResult SeleccionarAdjuntoEvaluacionRiesgoVigente(string IniToken)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var ParPaso = "Evaluacion-Riesgo";
                    IPlanificacion = FactoryPlanificacion.delega(VA);
                    string mensaje = String.Empty;
                    var adjunto = IPlanificacion.SeleccionarAdjuntoPorTokenYPaso(IniToken, ParPaso);
                    if (adjunto != null)
                    {
                        mensaje = "0|" + adjunto.ParToken + "|" + adjunto.ParNombreFinal;
                    }
                    else
                    {
                        mensaje = "1|Error no existe el archivo";
                    }
                    return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    return Json(new { Mensaje = "-1|Error al recuperar el archivo|" + exc.Message.ToString() + " " + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        [HttpPost]
        public ActionResult SeleccionarAdjuntoEvaluacionEconomicaVigente(string IniToken)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var ParPaso = "Evaluacion-Economica";
                    IPlanificacion = FactoryPlanificacion.delega(VA);
                    string mensaje = String.Empty;
                    var adjunto = IPlanificacion.SeleccionarAdjuntoPorTokenYPaso(IniToken, ParPaso);
                    if (adjunto != null)
                    {
                        mensaje = "0|" + adjunto.ParToken + "|" + adjunto.ParNombreFinal;
                    }
                    else
                    {
                        mensaje = "1|Error no existe el archivo";
                    }
                    return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    return Json(new { Mensaje = "-1|Error al recuperar el archivo|" + exc.Message.ToString() + " " + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        [HttpPost]
        public ActionResult SeleccionarOtroAdjuntoEvaluacionEconomicaVigente(string IniToken, string ParToken)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                // return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var ParPaso = "Evaluacion-Economica";
                    IPlanificacion = FactoryPlanificacion.delega(VA);
                    string mensaje = String.Empty;
                    var adjunto = IPlanificacion.SeleccionarOtroAdjuntoPorTokenYPaso(IniToken, ParToken, ParPaso);
                    if (adjunto != null)
                    {
                        mensaje = "0|" + adjunto.ParToken + "|" + adjunto.ParNombreFinal;
                    }
                    else
                    {
                        mensaje = "1|Error no existe el archivo";
                    }
                    return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    return Json(new { Mensaje = "-1|Error al recuperar el archivo|" + exc.Message.ToString() + " " + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }


        [HttpPost]
        public ActionResult SeleccionarOtroAdjuntoEvaluacionRiesgoVigente(string IniToken, string ParToken)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var ParPaso = "Evaluacion-Riesgo";
                    IPlanificacion = FactoryPlanificacion.delega(VA);
                    string mensaje = String.Empty;
                    var adjunto = IPlanificacion.SeleccionarOtroAdjuntoPorTokenYPaso(IniToken, ParToken, ParPaso);
                    if (adjunto != null)
                    {
                        mensaje = "0|" + adjunto.ParToken + "|" + adjunto.ParNombreFinal;
                    }
                    else
                    {
                        mensaje = "1|Error no existe el archivo";
                    }
                    return Json(new { Mensaje = mensaje }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    return Json(new { Mensaje = "-1|Error al recuperar el archivo|" + exc.Message.ToString() + " " + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        /// <summary>
        /// METODO PARA VISUALIZACION DE ADJUNTOS
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public string VerAdjuntos(string token)
        {
            try
            {
                IPlanificacion = FactoryPlanificacion.delega(VA);
                return IPlanificacion.VerAdjuntos(token);
            }
            catch (Exception exc)
            {
                return exc.Message.ToString() + "-----" + exc.StackTrace.ToString();
            }
            finally
            {
                FactoryPlanificacion = null;
                IPlanificacion = null;
            }
        }
        /// <summary>
        /// DESCARGAR ARCHIVO O ADJUNTO
        /// </summary>
        /// <param name="token"></param>
        /// <param name="paso"></param>
        /// <param name="archivo"></param>
        /// <returns></returns>
        [HttpGet]
        public FileResult DescargarArchivo(string token, string paso, string archivo)
        {
            var ruta = string.Empty;
            switch (paso)
            {
                case "Presupuesto-Gantt":
                    ruta = Server.MapPath("~/Files/Iniciativas/Presupuesto/Gantt/" + token + "/" + archivo);
                    break;
                case "Evaluacion-Economica":
                    ruta = Server.MapPath("~/Files/Iniciativas/EvaluacionEconomica/" + token + "/" + archivo);
                    break;
                case "Evaluacion-Riesgo":
                    ruta = Server.MapPath("~/Files/Iniciativas/EvaluacionRiesgo/" + token + "/" + archivo);
                    break;
                case "Categorizacion":
                    ruta = Server.MapPath("~/Files/Iniciativas/Categorizacion/" + token + "/" + archivo);
                    break;
                case "Descripcion-Detallada":
                    ruta = Server.MapPath("~/Files/Iniciativas/Descripcion/" + token + "/" + archivo);
                    break;
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(ruta);
            string fileName = archivo;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpGet]
        public ActionResult ListarMatrizRiesgo()
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(LA);
                    JsonResponse = JsonConvert.SerializeObject(IPlanificacion.ListarMatrizRiesgo(), Formatting.None);
                    return Json(JsonResponse, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }

        [HttpGet]
        public ActionResult obtenerFechaBloqueo(String TipoIniciativa)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    IPlanificacion = FactoryPlanificacion.delega(LA);
                    JsonResponse = IPlanificacion.obtenerFechaBloqueo(TipoIniciativa);
                    return Json(JsonResponse, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    return Json(new { Resultado = "ERROR|" + exc.Message.ToString() + "|" + exc.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    FactoryPlanificacion = null;
                    IPlanificacion = null;
                }
            }
        }



        #endregion


    }
}
