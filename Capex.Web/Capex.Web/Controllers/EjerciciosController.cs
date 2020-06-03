﻿using System;
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
using static System.Net.WebRequestMethods;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Text;

namespace Capex.Web.Controllers
{
    [AuthorizeAdminOrMember]
    [RoutePrefix("Ejercicios")]
    public class EjerciciosController : Controller
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
        ///     CONTROLADOR "EjerciciosController" 
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
        public EjerciciosController()
        {
            ////IDENTIFICACION
            //FactoryPlanificacion = new PlanificacionFactory();
            JsonResponse = string.Empty;
            ORM = CapexInfraestructure.Utilities.Utils.Conectar();
        }
        #endregion

        #region "METODOS BANDEJA DE ENTRADA"
        [Route]
        public ActionResult Index()
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])))
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    ViewBag.Iniciativas = ORM.Query("CAPEX_SEL_EJERCICIOS_OFICIALES", commandType: CommandType.StoredProcedure).ToList();
                }
            }
            return View();
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
                    try
                    {
                        var categorias = ORM.Query("CAPEX_SEL_CATEGORIA_FILTRO_VIGENTES", new { @pagina = 2, @categoria = "1" }, commandType: CommandType.StoredProcedure).ToList();
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

                        var Iniciativa = ORM.Query("CAPEX_SEL_EJERCICIOS_OFICIALES_FILTROS", new { @anio = anio, @etapa = etapa, @area = area, @proceso = proceso, @clase = clase, @estadoProyecto = estadoProyecto, @estadoIniciativa = estadoIniciativa, @macroCategoria = macroCategoria, @categoria = categoria, @clasificacionSSO = clasificacionSSO, @nivelIngenieria = nivelIngenieria, @clasificacionRiesgo = clasificacionRiesgo, @gerenciaEjecutora = gerenciaEjecutora, @gerenciaInversion = gerenciaInversion, @estandarSeguridad = estandarSeguridad }, commandType: CommandType.StoredProcedure).ToList();
                        if (Iniciativa.Count > 0)
                        {
                            ViewBag.Iniciativas = Iniciativa;
                        }
                        else
                        {
                            ViewBag.Iniciativas = null;
                        }

                        var tableTrs = new StringBuilder();

                        var paginatorTable = new StringBuilder();
                        var countRows = 0;
                        var totalRows = 0;

                        if (ViewBag.Iniciativas != null && ViewBag.Iniciativas.Count > 0)
                        {


                            foreach (var ini in ViewBag.Iniciativas)
                            {
                                var selectTrs = new StringBuilder();
                                selectTrs.Append("<select id=" + Convert.ToChar(34) + "Funcionalidades" + Convert.ToChar(34) + " class=" + Convert.ToChar(34) + "form-control" + Convert.ToChar(34) + " style =" + Convert.ToChar(34) + "font-size:11px;;text-align:center" + Convert.ToChar(34) + " onchange =" + Convert.ToChar(34) + "FNEvaluarAccion(this.value, '" + ini.PidToken + "')" + Convert.ToChar(34) + ">");
                                selectTrs.Append("<option value=" + Convert.ToChar(34) + "-1" + Convert.ToChar(34) + " selected>...</option>");
                                selectTrs.Append("<option value=" + Convert.ToChar(34) + "0" + Convert.ToChar(34) + ">Ver</option>");
                                selectTrs.Append("<option value=" + Convert.ToChar(34) + "2" + Convert.ToChar(34) + ">Adjuntos</option>");
                                selectTrs.Append("<option value=" + Convert.ToChar(34) + "3" + Convert.ToChar(34) + ">Pdf</option>");
                                selectTrs.Append("</select>");
                                if (countRows < 10)
                                {
                                    tableTrs.Append("<tr style=''>");
                                }
                                else
                                {
                                    tableTrs.Append("<tr style='display: none;'>");
                                }

                                tableTrs.Append("<td width=" + Convert.ToChar(34) + "5px" + Convert.ToChar(34) + ">");
                                tableTrs.Append("<input type=" + Convert.ToChar(34) + "hidden" + Convert.ToChar(34) + " name=" + Convert.ToChar(34) + "defaultHiddenChecked_" + ini.PidId + Convert.ToChar(34) + " value=" + Convert.ToChar(34) + "defaultChecked_" + ini.PidId + "," + ini.PidToken + Convert.ToChar(34) + ">");
                                tableTrs.Append("<div class=" + Convert.ToChar(34) + "form-check" + Convert.ToChar(34) + ">");
                                tableTrs.Append("<input id =" + Convert.ToChar(34) + "defaultChecked_" + ini.PidId + Convert.ToChar(34) + " class=" + Convert.ToChar(34) + "styled" + Convert.ToChar(34) + " type=" + Convert.ToChar(34) + "checkbox" + Convert.ToChar(34) + " onclick ='seleccionarIniciativaPdf(" + Convert.ToChar(34) + "defaultChecked_" + ini.PidId + Convert.ToChar(34) + ", " + Convert.ToChar(34) + ini.PidToken + Convert.ToChar(34) + ");'>");
                                tableTrs.Append("<label class=" + Convert.ToChar(34) + "form-check-label" + Convert.ToChar(34) + " for= " + Convert.ToChar(34) + "defaultChecked_" + ini.PidId + Convert.ToChar(34) + "></label>");
                                tableTrs.Append("</div>");
                                tableTrs.Append("</td>");

                                tableTrs.Append("<td><span style =" + Convert.ToChar(34) + "font-size:11px;" + Convert.ToChar(34) + ">&nbsp;" + ini.PidCodigoIniciativa + "</span></td>");
                                tableTrs.Append("<td class = " + Convert.ToChar(34) + "text-center" + Convert.ToChar(34) + " width =" + Convert.ToChar(34) + "120px" + Convert.ToChar(34) + "><div style = " + Convert.ToChar(34) + "text-transform:uppercase; text-align:left" + Convert.ToChar(34) + ">" + ini.IniUsuario + "</div></td>");
                                tableTrs.Append("<td width = " + Convert.ToChar(34) + "100px" + Convert.ToChar(34) + "><div style=" + Convert.ToChar(34) + "text-align:center" + Convert.ToChar(34) + ">" + ini.IniPeriodo + "</div></td>");
                                tableTrs.Append("<td><div style = " + Convert.ToChar(34) + "text-align:center" + Convert.ToChar(34) + ">" + ini.IniTipo + "</div></td>");
                                tableTrs.Append("<td><div style = " + Convert.ToChar(34) + "text-transform:uppercase;" + Convert.ToChar(34) + ">" + ini.PidNombreProyecto + "</div></td>");
                                tableTrs.Append("<td class='text-center' width='200px'><div style='text - align:left'>" + ini.GerNombre + "</div></td>");
                                tableTrs.Append("<td class='text-center' width='120px'><div style='text-transform:uppercase;text-align:left'>" + ini.CatMacroCategoria + "</div></td>");
                                tableTrs.Append("<td class='text-center' width='200px'><div style='text-transform:uppercase;text-align:center' id='TotalCapex'>" + ini.IfDato19 + "</div></td>");
                                tableTrs.Append("<td class='text-center' width='100px' onclick='exitFilterPanel()'>" + selectTrs.ToString() + "</td>");
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
            string Desplegable = string.Empty;
            string Estado = string.Empty;
            try
            {
                //@pagina = 2 ejercicios oficiales, @categoria = "1" presupuestos
                var categorias = ORM.Query("CAPEX_SEL_CATEGORIA_FILTRO_VIGENTES", new { @pagina = 2, @categoria = "1" }, commandType: CommandType.StoredProcedure).ToList();
                var items = ORM.Query("CAPEX_SEL_CATEGORIA_FILTRO_ITEM_VIGENTES", new { @pagina = 2, @categoria = "1" }, commandType: CommandType.StoredProcedure).ToList();
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

        #endregion
    }
}
