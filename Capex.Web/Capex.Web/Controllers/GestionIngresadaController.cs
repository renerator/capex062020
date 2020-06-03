﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapexIdentity.Utilities;
using CapexInfraestructure.Bll.Entities.Gestion;
using CapexInfraestructure.Bll.Business.Gestion;
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
using System.Globalization;

namespace Capex.Web.Controllers
{
    [AuthorizeAdminOrMember]
    [RoutePrefix("GestionIngresada")]
    public class GestionIngresadaController : Controller
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
        ///     CONTROLADOR "GestionIngresadaController" 
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
        #endregion

        #region "CONSTANTES"
        //IDENTIFICACION
        //private const PlanificacionFactory.tipo LP = PlanificacionFactory.tipo.ListarProcesos;
        #endregion

        #region "CAMPOS"
        //IDENTIFICACION
        //public static PlanificacionFactory FactoryPlanificacion;
        //public static IPlanificacion IPlanificacion;
        public SqlConnection ORM;
        #endregion

        #region "CONSTRUCTOR"
        public GestionIngresadaController()
        {
            ////IDENTIFICACION
            //FactoryPlanificacion = new PlanificacionFactory();
            JsonResponse = string.Empty;
            ORM = CapexInfraestructure.Utilities.Utils.Conectar();
        }
        #endregion

        #region "METODOS BANDEJA DE ENTRADA"
        /// <summary>
        /// METODO VISTA INICIAL
        /// </summary>
        /// <returns></returns>
        [Route]
        public class IniciativaManualMap
        {
            public string PidToken { get; set; }
            public string PidEstadoFlujo { get; set; }
            public string PidNombreProyecto { get; set; }
            public string IniTipo { get; set; }
            public string PidEtapa { get; set; }
            public string CatClasificacionSSO { get; set; }
            public string IfDato19 { get; set; }
            public string PidCodigoIniciativa { get; set; }
        }
        public ActionResult Index()
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                //
                Session["CAPEX_SESS_VISTA_CONTENEDORA_PADRE"] = "GestionIngresada";
                //
                var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])))
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    try
                    {
                        var tipoIniciativaSeleccionado = Convert.ToString(Session["tipoIniciativaSeleccionado"]);
                        if (string.IsNullOrEmpty(tipoIniciativaSeleccionado))
                        {
                            tipoIniciativaSeleccionado = "0";
                        }
                        var usuarioAux = "";
                        if (!rol.Contains("Administrador1") && !rol.Contains("Administrador2") && !rol.Contains("Administrador3"))
                        {
                            usuarioAux = usuario;
                        }
                        if (tipoIniciativaSeleccionado.Equals("0"))
                        {
                            var Iniciativa = ORM.Query("CAPEX_SEL_GESTION_INGRESADA", new { @usuario = usuarioAux }, commandType: CommandType.StoredProcedure).ToList();
                            if (Iniciativa != null && Iniciativa.Count > 0)
                            {
                                ViewBag.Iniciativas = Iniciativa;
                            }
                            else
                            {
                                ViewBag.Iniciativas = null;
                            }
                        }
                        else
                        {
                            var Iniciativa = ORM.Query("CAPEX_SEL_GESTION_INGRESADA_2", new { @usuario = usuarioAux, @tipoIniciativa = tipoIniciativaSeleccionado }, commandType: CommandType.StoredProcedure).ToList();
                            if (Iniciativa != null && Iniciativa.Count > 0)
                            {
                                ViewBag.Iniciativas = Iniciativa;
                            }
                            else
                            {
                                ViewBag.Iniciativas = null;
                            }
                        }
                        ViewBag.Revisores = null;
                        //ViewBag.Revisores = ORM.Query("CAPEX_SEL_GESTION_REVISORES", commandType: CommandType.StoredProcedure).ToList();
                    }
                    catch (Exception ex)
                    {
                        return Json(new { Mensaje = ex.Message.ToString() + "-----" + ex.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return View("Index");
        }

        [HttpPost]
        public ActionResult getData(string filtroGetData)
        {
            Dictionary<string, int[]> jsonFilter = JsonConvert.DeserializeObject<Dictionary<string, int[]>>(filtroGetData);
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                //
                // SETEAR VALORES CLAVE
                //
                var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                var anio = "0";
                var etapa = "0";
                var area = "0";
                var proceso = "0";
                var clase = "0";
                var estadoProyecto = "0";
                var estadoIniciativa = "0";
                var macroCategoria = "0";
                var categoria = "0";
                var clasificacionSSO = "0";
                var nivelIngenieria = "0";
                var clasificacionRiesgo = "0";
                var gerenciaEjecutora = "0";
                var gerenciaInversion = "0";
                var estandarSeguridad = "0";

                if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])))
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    var tipoIniciativaSeleccionado = Convert.ToString(Session["tipoIniciativaSeleccionado"]);
                    if (string.IsNullOrEmpty(tipoIniciativaSeleccionado))
                    {
                        tipoIniciativaSeleccionado = "0";
                    }
                    try
                    {
                        var categorias = ORM.Query("CAPEX_SEL_CATEGORIA_FILTRO_VIGENTES", new { @pagina = 6, @categoria = tipoIniciativaSeleccionado }, commandType: CommandType.StoredProcedure).ToList();
                        if (categorias != null && categorias.Count > 0)
                        {
                            foreach (var cats in categorias)
                            {
                                if (jsonFilter.ContainsKey(cats.Sigla))
                                {
                                    switch (Convert.ToInt32(cats.Orden))
                                    {
                                        case 1:
                                            anio = ((jsonFilter[cats.Sigla] != null && jsonFilter[cats.Sigla].Length > 0) ? string.Join(",", jsonFilter[cats.Sigla]) : "0");
                                            break;
                                        case 2:
                                            etapa = ((jsonFilter[cats.Sigla] != null && jsonFilter[cats.Sigla].Length > 0) ? string.Join(",", jsonFilter[cats.Sigla]) : "0");
                                            break;
                                        case 3:
                                            area = ((jsonFilter[cats.Sigla] != null && jsonFilter[cats.Sigla].Length > 0) ? string.Join(",", jsonFilter[cats.Sigla]) : "0");
                                            break;
                                        case 4:
                                            proceso = ((jsonFilter[cats.Sigla] != null && jsonFilter[cats.Sigla].Length > 0) ? string.Join(",", jsonFilter[cats.Sigla]) : "0");
                                            break;
                                        case 5:
                                            clase = ((jsonFilter[cats.Sigla] != null && jsonFilter[cats.Sigla].Length > 0) ? string.Join(",", jsonFilter[cats.Sigla]) : "0");
                                            break;
                                        case 6:
                                            estadoProyecto = ((jsonFilter[cats.Sigla] != null && jsonFilter[cats.Sigla].Length > 0) ? string.Join(",", jsonFilter[cats.Sigla]) : "0");
                                            break;
                                        case 7:
                                            estadoIniciativa = ((jsonFilter[cats.Sigla] != null && jsonFilter[cats.Sigla].Length > 0) ? string.Join(",", jsonFilter[cats.Sigla]) : "0");
                                            break;
                                        case 8:
                                            macroCategoria = ((jsonFilter[cats.Sigla] != null && jsonFilter[cats.Sigla].Length > 0) ? string.Join(",", jsonFilter[cats.Sigla]) : "0");
                                            break;
                                        case 9:
                                            categoria = ((jsonFilter[cats.Sigla] != null && jsonFilter[cats.Sigla].Length > 0) ? string.Join(",", jsonFilter[cats.Sigla]) : "0");
                                            break;
                                        case 10:
                                            clasificacionSSO = ((jsonFilter[cats.Sigla] != null && jsonFilter[cats.Sigla].Length > 0) ? string.Join(",", jsonFilter[cats.Sigla]) : "0");
                                            break;
                                        case 11:
                                            nivelIngenieria = ((jsonFilter[cats.Sigla] != null && jsonFilter[cats.Sigla].Length > 0) ? string.Join(",", jsonFilter[cats.Sigla]) : "0");
                                            break;
                                        case 12:
                                            clasificacionRiesgo = ((jsonFilter[cats.Sigla] != null && jsonFilter[cats.Sigla].Length > 0) ? string.Join(",", jsonFilter[cats.Sigla]) : "0");
                                            break;
                                        case 13:
                                            gerenciaEjecutora = ((jsonFilter[cats.Sigla] != null && jsonFilter[cats.Sigla].Length > 0) ? string.Join(",", jsonFilter[cats.Sigla]) : "0");
                                            break;
                                        case 14:
                                            gerenciaInversion = ((jsonFilter[cats.Sigla] != null && jsonFilter[cats.Sigla].Length > 0) ? string.Join(",", jsonFilter[cats.Sigla]) : "0");
                                            break;
                                        case 15:
                                            estandarSeguridad = ((jsonFilter[cats.Sigla] != null && jsonFilter[cats.Sigla].Length > 0) ? string.Join(",", jsonFilter[cats.Sigla]) : "0");
                                            break;
                                        case 16:
                                            clasificacionRiesgo = ((jsonFilter[cats.Sigla] != null && jsonFilter[cats.Sigla].Length > 0) ? string.Join(",", jsonFilter[cats.Sigla]) : "0");
                                            break;
                                    }
                                }
                            }
                        }
                        if (!rol.Contains("Administrador1") && !rol.Contains("Administrador2") && !rol.Contains("Administrador3"))
                        {
                            if (tipoIniciativaSeleccionado.Equals("0"))
                            {
                                var Iniciativa = ORM.Query("CAPEX_SEL_GESTION_INGRESADA_FILTROS", new { @usuario = usuario, @tipoIniciativa = tipoIniciativaSeleccionado, @anio = anio, @etapa = etapa, @area = area, @proceso = proceso, @clase = clase, @estadoProyecto = estadoProyecto, @estadoIniciativa = estadoIniciativa, @macroCategoria = macroCategoria, @categoria = categoria, @clasificacionSSO = clasificacionSSO, @nivelIngenieria = nivelIngenieria, @clasificacionRiesgo = clasificacionRiesgo, @gerenciaEjecutora = gerenciaEjecutora, @gerenciaInversion = gerenciaInversion, @estandarSeguridad = estandarSeguridad }, commandType: CommandType.StoredProcedure).ToList();
                                if (Iniciativa != null && Iniciativa.Count > 0)
                                {
                                    ViewBag.Iniciativas = Iniciativa;
                                }
                                else
                                {
                                    ViewBag.Iniciativas = null;
                                }
                            }
                            else
                            {
                                var Iniciativa = ORM.Query("CAPEX_SEL_GESTION_INGRESADA_2_FILTROS", new { @usuario = usuario, @tipoIniciativa = tipoIniciativaSeleccionado, @anio = anio, @etapa = etapa, @area = area, @proceso = proceso, @clase = clase, @estadoProyecto = estadoProyecto, @estadoIniciativa = estadoIniciativa, @macroCategoria = macroCategoria, @categoria = categoria, @clasificacionSSO = clasificacionSSO, @nivelIngenieria = nivelIngenieria, @clasificacionRiesgo = clasificacionRiesgo, @gerenciaEjecutora = gerenciaEjecutora, @gerenciaInversion = gerenciaInversion, @estandarSeguridad = estandarSeguridad }, commandType: CommandType.StoredProcedure).ToList();
                                if (Iniciativa != null && Iniciativa.Count > 0)
                                {
                                    ViewBag.Iniciativas = Iniciativa;
                                }
                                else
                                {
                                    ViewBag.Iniciativas = null;
                                }
                            }
                        }
                        else
                        {
                            if (tipoIniciativaSeleccionado.Equals("0"))
                            {
                                var Iniciativa = ORM.Query("CAPEX_SEL_GESTION_INGRESADA_FILTROS", new { @usuario = "", @tipoIniciativa = tipoIniciativaSeleccionado, @anio = anio, @etapa = etapa, @area = area, @proceso = proceso, @clase = clase, @estadoProyecto = estadoProyecto, @estadoIniciativa = estadoIniciativa, @macroCategoria = macroCategoria, @categoria = categoria, @clasificacionSSO = clasificacionSSO, @nivelIngenieria = nivelIngenieria, @clasificacionRiesgo = clasificacionRiesgo, @gerenciaEjecutora = gerenciaEjecutora, @gerenciaInversion = gerenciaInversion, @estandarSeguridad = estandarSeguridad }, commandType: CommandType.StoredProcedure).ToList();
                                if (Iniciativa != null && Iniciativa.Count > 0)
                                {
                                    ViewBag.Iniciativas = Iniciativa;
                                }
                                else
                                {
                                    ViewBag.Iniciativas = null;
                                }
                            }
                            else
                            {
                                var Iniciativa = ORM.Query("CAPEX_SEL_GESTION_INGRESADA_2_FILTROS", new { @usuario = "", @tipoIniciativa = tipoIniciativaSeleccionado, @anio = anio, @etapa = etapa, @area = area, @proceso = proceso, @clase = clase, @estadoProyecto = estadoProyecto, @estadoIniciativa = estadoIniciativa, @macroCategoria = macroCategoria, @categoria = categoria, @clasificacionSSO = clasificacionSSO, @nivelIngenieria = nivelIngenieria, @clasificacionRiesgo = clasificacionRiesgo, @gerenciaEjecutora = gerenciaEjecutora, @gerenciaInversion = gerenciaInversion, @estandarSeguridad = estandarSeguridad }, commandType: CommandType.StoredProcedure).ToList();
                                if (Iniciativa != null && Iniciativa.Count > 0)
                                {
                                    ViewBag.Iniciativas = Iniciativa;
                                }
                                else
                                {
                                    ViewBag.Iniciativas = null;
                                }
                            }
                        }
                        var tableTrs = new StringBuilder();
                        var selectTrs = new StringBuilder();
                        var paginatorTable = new StringBuilder();
                        var countRows = 0;
                        var totalRows = 0;

                        if (ViewBag.Iniciativas != null && ViewBag.Iniciativas.Count > 0)
                        {
                            if (Convert.ToString(@Session["CAPEX_SESS_ROLNOMBRE"]) == "Gestor")
                            {
                                selectTrs.Append("<select id=" + Convert.ToChar(34) + "Funcionalidades" + Convert.ToChar(34) + " class=" + Convert.ToChar(34) + "form-control" + Convert.ToChar(34) + " style =" + Convert.ToChar(34) + "font-size:11px;" + Convert.ToChar(34) + " onchange =" + Convert.ToChar(34) + "FNEvaluarAccion(this.value)" + Convert.ToChar(34) + ">");
                                selectTrs.Append("<option value=" + Convert.ToChar(34) + "-1" + Convert.ToChar(34) + " selected>...</option>");
                                selectTrs.Append("<option value=" + Convert.ToChar(34) + "0" + Convert.ToChar(34) + ">Ver</option>");
                                selectTrs.Append("<option value=" + Convert.ToChar(34) + "2" + Convert.ToChar(34) + ">Adjuntos</option>");
                                selectTrs.Append("<option value=" + Convert.ToChar(34) + "3" + Convert.ToChar(34) + ">Pdf</option>");
                                selectTrs.Append("</select>");
                            }
                            else if (Convert.ToString(@Session["CAPEX_SESS_ROLNOMBRE"]) == "Administrador1")
                            {
                                selectTrs.Append("<select id=" + Convert.ToChar(34) + "Funcionalidades" + Convert.ToChar(34) + " class=" + Convert.ToChar(34) + "form-control" + Convert.ToChar(34) + " style =" + Convert.ToChar(34) + "font-size:11px;" + Convert.ToChar(34) + " onchange =" + Convert.ToChar(34) + "FNEvaluarAccion(this.value)" + Convert.ToChar(34) + ">");
                                selectTrs.Append("<option value=" + Convert.ToChar(34) + "-1" + Convert.ToChar(34) + " selected>...</option>");
                                selectTrs.Append("<option value=" + Convert.ToChar(34) + "0" + Convert.ToChar(34) + ">Ver</option>");
                                selectTrs.Append("<option value=" + Convert.ToChar(34) + "2" + Convert.ToChar(34) + ">Adjuntos</option>");
                                selectTrs.Append("<option value=" + Convert.ToChar(34) + "3" + Convert.ToChar(34) + ">Pdf</option>");
                                selectTrs.Append("</select>");
                            }
                            else if (Convert.ToString(@Session["CAPEX_SESS_ROLNOMBRE"]) == "Administrador2")
                            {
                                selectTrs.Append("<select id=" + Convert.ToChar(34) + "Funcionalidades" + Convert.ToChar(34) + " class=" + Convert.ToChar(34) + "form-control" + Convert.ToChar(34) + " style =" + Convert.ToChar(34) + "font-size:11px;" + Convert.ToChar(34) + " onchange =" + Convert.ToChar(34) + "FNEvaluarAccion(this.value)" + Convert.ToChar(34) + ">");
                                selectTrs.Append("<option value=" + Convert.ToChar(34) + "-1" + Convert.ToChar(34) + " selected>...</option>");
                                selectTrs.Append("<option value=" + Convert.ToChar(34) + "0" + Convert.ToChar(34) + ">Ver</option>");
                                selectTrs.Append("<option value=" + Convert.ToChar(34) + "1" + Convert.ToChar(34) + ">Evaluar</option>");
                                selectTrs.Append("<option value=" + Convert.ToChar(34) + "2" + Convert.ToChar(34) + ">Adjuntos</option>");
                                selectTrs.Append("<option value=" + Convert.ToChar(34) + "3" + Convert.ToChar(34) + ">Pdf</option>");
                                selectTrs.Append("<option value=" + Convert.ToChar(34) + "18" + Convert.ToChar(34) + ">No Aprobar</option>");
                                selectTrs.Append("</select>");
                            }
                            else if (Convert.ToString(@Session["CAPEX_SESS_ROLNOMBRE"]) == "Administrador3")
                            {
                                selectTrs.Append("<select id=" + Convert.ToChar(34) + "Funcionalidades" + Convert.ToChar(34) + " class=" + Convert.ToChar(34) + "form-control" + Convert.ToChar(34) + " style =" + Convert.ToChar(34) + "font-size:11px;" + Convert.ToChar(34) + " onchange =" + Convert.ToChar(34) + "FNEvaluarAccion(this.value)" + Convert.ToChar(34) + ">");
                                selectTrs.Append("<option value=" + Convert.ToChar(34) + "-1" + Convert.ToChar(34) + " selected>...</option>");
                                selectTrs.Append("<option value=" + Convert.ToChar(34) + "0" + Convert.ToChar(34) + ">Ver</option>");
                                selectTrs.Append("<option value=" + Convert.ToChar(34) + "2" + Convert.ToChar(34) + ">Adjuntos</option>");
                                selectTrs.Append("<option value=" + Convert.ToChar(34) + "3" + Convert.ToChar(34) + ">Pdf</option>");
                                selectTrs.Append("</select>");
                            }
                            foreach (var result in ViewBag.Iniciativas)
                            {
                                if (countRows < 10)
                                {
                                    tableTrs.Append("<tr style=''>");
                                }
                                else
                                {
                                    tableTrs.Append("<tr style='display: none;'>");
                                }

                                tableTrs.Append("<td width=" + Convert.ToChar(34) + "5px" + Convert.ToChar(34) + ">");
                                tableTrs.Append("<input type=" + Convert.ToChar(34) + "hidden" + Convert.ToChar(34) + " name=" + Convert.ToChar(34) + "defaultHiddenChecked_" + result.PidId + Convert.ToChar(34) + " value=" + Convert.ToChar(34) + "defaultChecked_" + result.PidId + "," + result.PidToken + Convert.ToChar(34) + ">");
                                tableTrs.Append("<div class=" + Convert.ToChar(34) + "form-check" + Convert.ToChar(34) + ">");
                                tableTrs.Append("<input id =" + Convert.ToChar(34) + "defaultChecked_" + result.PidId + Convert.ToChar(34) + " class=" + Convert.ToChar(34) + "styled" + Convert.ToChar(34) + " type=" + Convert.ToChar(34) + "checkbox" + Convert.ToChar(34) + " onclick ='seleccionarIniciativaPdf(" + Convert.ToChar(34) + "defaultChecked_" + result.PidId + Convert.ToChar(34) + ", " + Convert.ToChar(34) + result.PidToken + Convert.ToChar(34) + ");'>");
                                tableTrs.Append("<label class=" + Convert.ToChar(34) + "form-check-label" + Convert.ToChar(34) + " for= " + Convert.ToChar(34) + "defaultChecked_" + result.PidId + Convert.ToChar(34) + "></label>");
                                tableTrs.Append("</div>");
                                tableTrs.Append("</td>");
                                tableTrs.Append("<td>" + result.PidCodigoIniciativa + "</td>");
                                tableTrs.Append("<td>" + result.PidNombreProyecto + "</td>");
                                tableTrs.Append("<td>" + result.IniTipo + "</td>");
                                tableTrs.Append("<td>" + result.PidEtapa + "</td>");
                                tableTrs.Append("<td class='text-center' width='100px'>" + result.CatClasificacionSSO + "</td>");
                                tableTrs.Append("<td class='text-center' width='140px'>" + ((string.IsNullOrEmpty(result.TotalCapex) || result.TotalCapex.Equals("0")) ? "0" : String.Format("{0:0,0}", double.Parse(result.TotalCapex, CultureInfo.InvariantCulture))) + "</td>");
                                tableTrs.Append("<td class='text-center' width='100px' onclick='exitFilterPanel();FNRegistrarIniciativa(" + Convert.ToChar(34) + result.PidToken + Convert.ToChar(34) + ")'>" + selectTrs.ToString() + "</td>");
                                tableTrs.Append("</tr>");
                                countRows++;
                                totalRows++;
                            }
                        }
                        else
                        {
                            tableTrs.Append("<tr>");
                            tableTrs.Append("<td></td>");
                            tableTrs.Append("<td></td>");
                            tableTrs.Append("<td></td>");
                            tableTrs.Append("<td></td>");
                            tableTrs.Append("<td></td>");
                            tableTrs.Append("<td></td>");
                            tableTrs.Append("<td></td>");
                            tableTrs.Append("<td></td>");
                            tableTrs.Append("</tr>");
                            countRows++;
                        }
                        var totalPage = (int)Math.Ceiling((double)countRows / 10);
                        for (int i = 0; i < totalPage; i++)
                        {
                            paginatorTable.Append("<span onclick = 'FNGotoPage(" + (i + 1) + ", " + 10 + ");' class=" + Convert.ToChar(34) + "page-number clickable" + ((i == 0) ? " active" : "") + Convert.ToChar(34) + ">" + (i + 1) + "</span>");
                        }
                        return Json(new { success = true, message = "", tableTrs = tableTrs.ToString(), countBadge = totalRows.ToString(), paginator = paginatorTable.ToString() });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = ex.Message.ToString() + "-----" + ex.StackTrace.ToString() });
                    }
                }
            }
        }



        /// <summary>
        /// METODO OBTENER ARBOL FILTRO
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult getTreeFilter()
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                string Desplegable = string.Empty;
                string Estado = string.Empty;
                try
                {
                    var tipoIniciativaSeleccionado = Convert.ToString(Session["tipoIniciativaSeleccionado"]);
                    var categorias = ORM.Query("CAPEX_SEL_CATEGORIA_FILTRO_VIGENTES", new { @pagina = 6, @categoria = tipoIniciativaSeleccionado }, commandType: CommandType.StoredProcedure).ToList();
                    var items = ORM.Query("[CAPEX_SEL_CATEGORIA_FILTRO_ITEM_VIGENTES]", new { @pagina = 6, @categoria = tipoIniciativaSeleccionado }, commandType: CommandType.StoredProcedure).ToList();
                    var arbol = new StringBuilder();
                    var contador = 1;

                    if (categorias != null && categorias.Count > 0)
                    {
                        foreach (var categoria in categorias)
                        {
                            arbol.Append("<div class=" + Convert.ToChar(34) + "card" + Convert.ToChar(34) + " > ");
                            arbol.Append("<div class=" + Convert.ToChar(34) + "card-header" + Convert.ToChar(34) + " id =" + Convert.ToChar(34) + "heading_" + contador + Convert.ToChar(34) + ">");
                            arbol.Append("<div class=" + Convert.ToChar(34) + "kkkkk" + Convert.ToChar(34) + " data-toggle=" + Convert.ToChar(34) + "collapse" + Convert.ToChar(34) + " data-target=" + Convert.ToChar(34) + "#collapse_" + contador + Convert.ToChar(34) + " aria-expanded=" + Convert.ToChar(34) + "true" + Convert.ToChar(34) + " aria-controls=" + Convert.ToChar(34) + "collapse_" + contador + Convert.ToChar(34) + " onclick=FNChangeIcon('class_span_'," + contador + "," + categorias.Count + ");> ");
                            arbol.Append("<div class=" + Convert.ToChar(34) + "row" + Convert.ToChar(34) + ">");
                            arbol.Append("<div class=" + Convert.ToChar(34) + "col-10" + Convert.ToChar(34) + "><h4 class=" + Convert.ToChar(34) + "toggle" + Convert.ToChar(34) + ">" + categoria.Nombre + "</h4></div>");
                            arbol.Append("<div class=" + Convert.ToChar(34) + "col-2" + Convert.ToChar(34) + "> <span id=" + Convert.ToChar(34) + "class_span_" + contador + Convert.ToChar(34) + " class=" + Convert.ToChar(34) + "glyphicon glyphicon-plus" + Convert.ToChar(34) + " style =" + Convert.ToChar(34) + "color:#367ab3" + Convert.ToChar(34) + "></span></div>");
                            arbol.Append("</div>");
                            arbol.Append("</div>");
                            arbol.Append("</div>");
                            arbol.Append("<div id = " + Convert.ToChar(34) + "collapse_" + contador + Convert.ToChar(34) + " class=" + Convert.ToChar(34) + "collapse" + Convert.ToChar(34) + " aria-labelledby=" + Convert.ToChar(34) + "heading_" + contador + Convert.ToChar(34) + " data-parent=" + Convert.ToChar(34) + "#accordion" + Convert.ToChar(34) + ">");
                            arbol.Append("<div class=" + Convert.ToChar(34) + "card-body" + Convert.ToChar(34) + ">");
                            if (items != null && items.Count > 0)
                            {
                                var match = false;
                                foreach (var item in items)
                                {
                                    if (categoria.Id == item.IdCategoriaFiltro)
                                    {
                                        match = true;
                                        arbol.Append("<section class=" + Convert.ToChar(34) + "section-preview" + Convert.ToChar(34) + ">");
                                        arbol.Append("<div class=" + Convert.ToChar(34) + "form-check" + Convert.ToChar(34) + ">");
                                        if (categoria.Tipo_Control == 0)
                                        {
                                            arbol.Append("<input type=" + Convert.ToChar(34) + "checkbox" + Convert.ToChar(34) + " onclick = " + Convert.ToChar(34) + "FNGetData('" + categoria.Sigla + "_" + categoria.Id + "_" + item.Id + "','" + categoria.Sigla + "'," + categoria.Id + "," + item.Id + ",'" + item.ValueParam + "');" + Convert.ToChar(34) + " class=" + Convert.ToChar(34) + "form-check-input" + Convert.ToChar(34) + " id=" + Convert.ToChar(34) + categoria.Sigla + "_" + categoria.Id + "_" + item.Id + Convert.ToChar(34) + ">");
                                            arbol.Append("<label class=" + Convert.ToChar(34) + "form-check-label" + Convert.ToChar(34) + " for=" + Convert.ToChar(34) + categoria.Sigla + "_" + categoria.Id + "_" + item.Id + Convert.ToChar(34) + ">" + item.ValueParam + "</label>");
                                        }
                                        else
                                        {
                                            arbol.Append("<input type=" + Convert.ToChar(34) + "radio" + Convert.ToChar(34) + " value=" + Convert.ToChar(34) + item.ValueParam + Convert.ToChar(34) + " class=" + Convert.ToChar(34) + "form-check-input" + Convert.ToChar(34) + " onclick = " + Convert.ToChar(34) + "FNGetDataRadio('" + categoria.Sigla + "_" + categoria.Id + "_" + item.Id + "','" + categoria.Sigla + "'," + categoria.Id + "," + item.Id + ",'" + item.ValueParam + "');" + Convert.ToChar(34) + " name=" + Convert.ToChar(34) + categoria.Sigla + Convert.ToChar(34) + " id=" + Convert.ToChar(34) + categoria.Sigla + "_" + categoria.Id + "_" + item.Id + Convert.ToChar(34) + ">");
                                            arbol.Append("<label class=" + Convert.ToChar(34) + "form-check-label" + Convert.ToChar(34) + " for=" + Convert.ToChar(34) + categoria.Sigla + "_" + categoria.Id + "_" + item.Id + Convert.ToChar(34) + ">" + item.ValueParam + "</label>");
                                        }
                                        arbol.Append("</div>");
                                        arbol.Append("<div class=" + Convert.ToChar(34) + "my-2" + Convert.ToChar(34) + "></div>");
                                        arbol.Append("</section>");
                                    }
                                }
                                if (match)
                                {

                                }
                            }
                            arbol.Append("</div>");
                            arbol.Append("</div>");
                            arbol.Append("</div>");
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
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message.ToString() + "-----" + ex.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, message = Desplegable.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        ///  METODO PARA OBTENER REVISORES
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ObtenerListadoRevisores()
        {
            try
            {
                var resultado = ViewBag.Revisores = ORM.Query("CAPEX_SEL_GESTION_REVISORES", commandType: CommandType.StoredProcedure).ToList();
                return resultado;
            }
            catch (Exception ex)
            {
                var ExceptionResult = "ObtenerListadoRevisores, Mensaje: " + ex.Message.ToString() + "-" + ", Detalle: " + ex.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = ex.Message.ToString() + "-----" + ex.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
            }

        }
        //
        // METODO ASIGNAR REVISOR
        //
        [HttpPost]
        public ActionResult Asignar(string IniToken, string Usuario, string Observacion, string Estado)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", IniToken);
                parametos.Add("Usuario", Usuario);
                parametos.Add("Observacion", Observacion);
                parametos.Add("Estado", Estado);

                ORM.Query("CAPEX_INS_GESTION_REVISOR", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Asignado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "Asignar, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// METODO APROBACION INICIATIVA
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AprobarIniciativa(string IniToken, string Usuario)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", IniToken);
                parametos.Add("Usuario", Usuario);

                ORM.Query("CAPEX_INS_GESTION_APROBACION_ADMIN1", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Aprobada" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "AprobarIniciativa, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// METODO RECHAZO DE INICIATIVA
        /// </summary>
        /// <param name="IniToken"></param>
        /// <param name="Usuario"></param>
        /// <param name="Observacion"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RechazarIniciativa(string IniToken, string Usuario, string Observacion)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("IniToken", IniToken);
                parametos.Add("Usuario", Usuario);
                parametos.Add("Observacion", Observacion);
                ORM.Query("CAPEX_INS_GESTION_RECHAZO_ADMIN1", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Rechazada" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "RechazarIniciativa, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// METODO RECHAZO DE INICIATIVA
        /// </summary>
        /// <param name="IniToken"></param>
        /// <param name="Usuario"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult NoAprobarIniciativaCE(string IniToken)
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    var parametos = new DynamicParameters();
                    parametos.Add("IniToken", IniToken);
                    parametos.Add("PidUsuario", usuario);
                    parametos.Add("PidRol", rol);
                    parametos.Add("ErrorCodeOutput", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 200);
                    ORM.Query("CAPEX_NO_APROBAR_CE", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    var ErrorCodeSP = parametos.Get<string>("ErrorCodeOutput");
                    string resultado = string.Empty;
                    if (!string.IsNullOrEmpty(ErrorCodeSP) && !string.IsNullOrEmpty(ErrorCodeSP.Trim()) && ErrorCodeSP.StartsWith("0"))
                    {
                        resultado = "NoAprobada|" + ErrorCodeSP.Trim();
                    }
                    else
                    {
                        resultado = "Error|" + ((!string.IsNullOrEmpty(ErrorCodeSP) && !string.IsNullOrEmpty(ErrorCodeSP.Trim()) ? ErrorCodeSP.Trim() : "1|ERROR AL NO APROBAR VISACION"));
                    }
                    string[] datos = resultado.Split('|');
                    if (datos[0] == "NoAprobada")
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
                catch (Exception err)
                {
                    var ExceptionResult = "NoAprobarIniciativaCE, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        #endregion
    }
}