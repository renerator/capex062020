using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CapexIdentity.Utilities;
using CapexInfraestructure.Bll.Entities.Mantenedor;
using CapexInfraestructure.Bll.Entities.Planificacion;
using Dapper;

namespace Capex.Web.Controllers
{
    [AuthorizeAdminOrMember]
    [RoutePrefix("Orientacion")]
    public class OrientacionController : Controller
    {
        #region "PROPIEDADES"
        private List<string> Listar { get; set; }
        private string JsonResponse { get; set; }
        private string ExceptionResult { get; set; }
        #endregion

        #region "CONSTRUCTOR"
        public OrientacionController()
        {
            ////IDENTIFICACION
            //FactoryPlanificacion = new PlanificacionFactory();
            JsonResponse = string.Empty;
        }
        #endregion

        // GET: Orientacion
        public ActionResult Index()
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                return RedirectToAction("Logout", "Login");
                //return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                Session["tipoIniciativaEjercicioOficial"] = "";
                Session["anioIniciativaEjercicioOficial"] = "";
                Session["tipoIniciativaSeleccionado"] = "";
                Session["anioIniciativaSeleccionado"] = "";
                string tipoIniciativaOrientacionComercialKey = "tipoIniciativaOrientacionComercial";
                string anioIniciativaOrientacionComercialKey = "anioIniciativaOrientacionComercial";
                var tipoIniciativaOrientacionComercial = Request.QueryString[tipoIniciativaOrientacionComercialKey];
                var anioIniciativaOrientacionComercial = Request.QueryString[anioIniciativaOrientacionComercialKey];
                Session[tipoIniciativaOrientacionComercialKey] = tipoIniciativaOrientacionComercial;
                Session[anioIniciativaOrientacionComercialKey] = anioIniciativaOrientacionComercial;
                if (tipoIniciativaOrientacionComercial == null || string.IsNullOrEmpty(tipoIniciativaOrientacionComercial.ToString())
                    || anioIniciativaOrientacionComercial == null || string.IsNullOrEmpty(anioIniciativaOrientacionComercial.ToString()))
                {
                    return RedirectToAction("Index", "Panel");
                }
                //using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        ViewBag.tipoIniciativaSeleccionadoMantenedor = tipoIniciativaOrientacionComercial;
                        objConnection.Open();
                        ViewBag.Iniciativas = SqlMapper.Query(objConnection, "CAPEX_SEL_PARAMETRO_VN", new { TPEPERIODO = anioIniciativaOrientacionComercial, TipoIniciativaSeleccionado = tipoIniciativaOrientacionComercial }, commandType: CommandType.StoredProcedure).ToList();
                    }
                    catch (Exception err)
                    {
                        err.ToString();
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
            return View();
        }


        [HttpGet]
        [Route("obtenerParametrosComercialesPorToken")]
        public ActionResult ObtenerParametrosComercialesPorToken(string ParametroVNToken)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    ParametroVNToken = ParametroVNToken.Replace(System.Environment.NewLine, "");
                    string TipoIniciativaSeleccionado = ((Session["tipoIniciativaOrientacionComercial"] != null) ? Convert.ToString(Session["tipoIniciativaOrientacionComercial"]) : "");
                    Template.ObtenerTemplateCorregido obtenerTemplateCorregido = new Template.ObtenerTemplateCorregido();
                    var results = SqlMapper.Query(objConnection, "CAPEX_SEL_PARAMETRO_COMERCIAL_ParametroVN_Token", new { ParametroVNToken, TipoIniciativaSeleccionado }, commandType: CommandType.StoredProcedure).ToList();
                    if (results != null && results.Count > 0)
                    {
                        foreach (var result in results)
                        {
                            obtenerTemplateCorregido.ITPEToken = result.ITPEToken;
                            obtenerTemplateCorregido.TPEPERIODO = result.TPEPERIODO;
                            obtenerTemplateCorregido.TPEPERIODORESPALDO = result.TPEPERIODORESPALDO;
                            obtenerTemplateCorregido.TPEPERIODOS = result.TPEPERIODOS;
                            obtenerTemplateCorregido.PETokenTC = result.PETokenTC;
                            obtenerTemplateCorregido.TPETokenTC = result.TPETokenTC;
                            obtenerTemplateCorregido.PETokenIPC = result.PETokenIPC;
                            obtenerTemplateCorregido.TPETokenIPC = result.TPETokenIPC;
                            obtenerTemplateCorregido.PETokenCPI = result.PETokenCPI;
                            obtenerTemplateCorregido.TPETokenCPI = result.TPETokenCPI;
                            StringBuilder paramTCMes = new StringBuilder();
                            if (result.IdParamEconomicoDetalleMesTCUNO != null)
                            {
                                paramTCMes.Append(result.IdParamEconomicoDetalleMesTCUNO).Append("|").Append(result.PARAMMESTCUNO).Append("|").Append(result.VALUEMESTCUNO).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesTCDOS != null)
                            {
                                paramTCMes.Append(result.IdParamEconomicoDetalleMesTCDOS).Append("|").Append(result.PARAMMESTCDOS).Append("|").Append(result.VALUEMESTCDOS).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesTCTRES != null)
                            {
                                paramTCMes.Append(result.IdParamEconomicoDetalleMesTCTRES).Append("|").Append(result.PARAMMESTCTRES).Append("|").Append(result.VALUEMESTCTRES).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesTCCUATRO != null)
                            {
                                paramTCMes.Append(result.IdParamEconomicoDetalleMesTCCUATRO).Append("|").Append(result.PARAMMESTCCUATRO).Append("|").Append(result.VALUEMESTCCUATRO).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesTCCINCO != null)
                            {
                                paramTCMes.Append(result.IdParamEconomicoDetalleMesTCCINCO).Append("|").Append(result.PARAMMESTCCINCO).Append("|").Append(result.VALUEMESTCCINCO).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesTCSEIS != null)
                            {
                                paramTCMes.Append(result.IdParamEconomicoDetalleMesTCSEIS).Append("|").Append(result.PARAMMESTCSEIS).Append("|").Append(result.VALUEMESTCSEIS).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesTCSIETE != null)
                            {
                                paramTCMes.Append(result.IdParamEconomicoDetalleMesTCSIETE).Append("|").Append(result.PARAMMESTCSIETE).Append("|").Append(result.VALUEMESTCSIETE).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesTCOCHO != null)
                            {
                                paramTCMes.Append(result.IdParamEconomicoDetalleMesTCOCHO).Append("|").Append(result.PARAMMESTCOCHO).Append("|").Append(result.VALUEMESTCOCHO).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesTCNUEVE != null)
                            {
                                paramTCMes.Append(result.IdParamEconomicoDetalleMesTCNUEVE).Append("|").Append(result.PARAMMESTCNUEVE).Append("|").Append(result.VALUEMESTCNUEVE).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesTCDIEZ != null)
                            {
                                paramTCMes.Append(result.IdParamEconomicoDetalleMesTCDIEZ).Append("|").Append(result.PARAMMESTCDIEZ).Append("|").Append(result.VALUEMESTCDIEZ).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesTCONCE != null)
                            {
                                paramTCMes.Append(result.IdParamEconomicoDetalleMesTCONCE).Append("|").Append(result.PARAMMESTCONCE).Append("|").Append(result.VALUEMESTCONCE).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesTCDOCE != null)
                            {
                                paramTCMes.Append(result.IdParamEconomicoDetalleMesTCDOCE).Append("|").Append(result.PARAMMESTCDOCE).Append("|").Append(result.VALUEMESTCDOCE);
                            }
                            obtenerTemplateCorregido.ParamTCMes = paramTCMes.ToString();
                            StringBuilder paramIPCMes = new StringBuilder();
                            if (result.IdParamEconomicoDetalleMesIPCUNO != null)
                            {
                                paramIPCMes.Append(result.IdParamEconomicoDetalleMesIPCUNO).Append("|").Append(result.PARAMMESIPCUNO).Append("|").Append(result.VALUEMESIPCUNO).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesIPCDOS != null)
                            {
                                paramIPCMes.Append(result.IdParamEconomicoDetalleMesIPCDOS).Append("|").Append(result.PARAMMESIPCDOS).Append("|").Append(result.VALUEMESIPCDOS).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesIPCTRES != null)
                            {
                                paramIPCMes.Append(result.IdParamEconomicoDetalleMesIPCTRES).Append("|").Append(result.PARAMMESIPCTRES).Append("|").Append(result.VALUEMESIPCTRES).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesIPCCUATRO != null)
                            {
                                paramIPCMes.Append(result.IdParamEconomicoDetalleMesIPCCUATRO).Append("|").Append(result.PARAMMESIPCCINCO).Append("|").Append(result.VALUEMESIPCCINCO).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesIPCCINCO != null)
                            {
                                paramIPCMes.Append(result.IdParamEconomicoDetalleMesIPCCINCO).Append("|").Append(result.PARAMMESIPCCINCO).Append("|").Append(result.VALUEMESIPCCINCO).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesIPCSEIS != null)
                            {
                                paramIPCMes.Append(result.IdParamEconomicoDetalleMesIPCSEIS).Append("|").Append(result.PARAMMESIPCSEIS).Append("|").Append(result.VALUEMESIPCSEIS).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesIPCSIETE != null)
                            {
                                paramIPCMes.Append(result.IdParamEconomicoDetalleMesIPCSIETE).Append("|").Append(result.PARAMMESIPCSIETE).Append("|").Append(result.VALUEMESIPCSIETE).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesIPCOCHO != null)
                            {
                                paramIPCMes.Append(result.IdParamEconomicoDetalleMesIPCOCHO).Append("|").Append(result.PARAMMESIPCOCHO).Append("|").Append(result.VALUEMESIPCOCHO).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesIPCNUEVE != null)
                            {
                                paramIPCMes.Append(result.IdParamEconomicoDetalleMesIPCNUEVE).Append("|").Append(result.PARAMMESIPCNUEVE).Append("|").Append(result.VALUEMESIPCNUEVE).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesIPCDIEZ != null)
                            {
                                paramIPCMes.Append(result.IdParamEconomicoDetalleMesIPCDIEZ).Append("|").Append(result.PARAMMESIPCDIEZ).Append("|").Append(result.VALUEMESIPCDIEZ).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesIPCONCE != null)
                            {
                                paramIPCMes.Append(result.IdParamEconomicoDetalleMesIPCONCE).Append("|").Append(result.PARAMMESIPCONCE).Append("|").Append(result.VALUEMESIPCONCE).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesIPDOCE != null)
                            {
                                paramIPCMes.Append(result.IdParamEconomicoDetalleMesIPDOCE).Append("|").Append(result.PARAMMESIPCDOCE).Append("|").Append(result.VALUEMESIPCDOCE);
                            }
                            obtenerTemplateCorregido.ParamIPCMes = paramIPCMes.ToString();
                            StringBuilder paramCPIMes = new StringBuilder();
                            if (result.IdParamEconomicoDetalleMesCPIUNO != null)
                            {
                                paramCPIMes.Append(result.IdParamEconomicoDetalleMesCPIUNO).Append("|").Append(result.PARAMMESCPIUNO).Append("|").Append(result.VALUEMESCPIUNO).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesCPIDOS != null)
                            {
                                paramCPIMes.Append(result.IdParamEconomicoDetalleMesCPIDOS).Append("|").Append(result.PARAMMESCPIDOS).Append("|").Append(result.VALUEMESCPIDOS).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesCPITRES != null)
                            {
                                paramCPIMes.Append(result.IdParamEconomicoDetalleMesCPITRES).Append("|").Append(result.PARAMMESCPITRES).Append("|").Append(result.VALUEMESCPITRES).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesCPICUATRO != null)
                            {
                                paramCPIMes.Append(result.IdParamEconomicoDetalleMesCPICUATRO).Append("|").Append(result.PARAMMESCPICUATRO).Append("|").Append(result.VALUEMESCPICUATRO).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesCPICINCO != null)
                            {
                                paramCPIMes.Append(result.IdParamEconomicoDetalleMesCPICINCO).Append("|").Append(result.PARAMMESCPICINCO).Append("|").Append(result.VALUEMESCPICINCO).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesCPISEIS != null)
                            {
                                paramCPIMes.Append(result.IdParamEconomicoDetalleMesCPISEIS).Append("|").Append(result.PARAMMESCPISEIS).Append("|").Append(result.VALUEMESCPISEIS).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesCPISIETE != null)
                            {
                                paramCPIMes.Append(result.IdParamEconomicoDetalleMesCPISIETE).Append("|").Append(result.PARAMMESCPISIETE).Append("|").Append(result.VALUEMESCPISIETE).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesCPIOCHO != null)
                            {
                                paramCPIMes.Append(result.IdParamEconomicoDetalleMesCPIOCHO).Append("|").Append(result.PARAMMESCPIOCHO).Append("|").Append(result.VALUEMESCPIOCHO).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesCPINUEVE != null)
                            {
                                paramCPIMes.Append(result.IdParamEconomicoDetalleMesCPINUEVE).Append("|").Append(result.PARAMMESCPINUEVE).Append("|").Append(result.VALUEMESCPINUEVE).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesCPIDIEZ != null)
                            {
                                paramCPIMes.Append(result.IdParamEconomicoDetalleMesCPIDIEZ).Append("|").Append(result.PARAMMESCPIDIEZ).Append("|").Append(result.VALUEMESCPIDIEZ).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesCPIONCE != null)
                            {
                                paramCPIMes.Append(result.IdParamEconomicoDetalleMesCPIONCE).Append("|").Append(result.PARAMMESCPIONCE).Append("|").Append(result.VALUEMESCPIONCE).Append(";");
                            }
                            if (result.IdParamEconomicoDetalleMesCPIDOCE != null)
                            {
                                paramCPIMes.Append(result.IdParamEconomicoDetalleMesCPIDOCE).Append("|").Append(result.PARAMMESCPIDOCE).Append("|").Append(result.VALUEMESCPIDOCE);
                            }
                            obtenerTemplateCorregido.ParamCPIMes = paramCPIMes.ToString();
                        }
                    }
                    int pasoFinal = (("2".Equals(TipoIniciativaSeleccionado)) ? 3 : 80);
                    if (!string.IsNullOrEmpty(obtenerTemplateCorregido.PETokenTC))
                    {
                        var resultsTcAnio = SqlMapper.Query(objConnection, "CAPEX_SEL_PARAM_ECONOMICO_DETALLE_TOKEN", new { PEToken = obtenerTemplateCorregido.PETokenTC, PasoFinal = pasoFinal }, commandType: CommandType.StoredProcedure).ToList();
                        if (resultsTcAnio != null && resultsTcAnio.Count > 0)
                        {
                            var cont = 1;
                            StringBuilder paramTCAnio = new StringBuilder();
                            foreach (var result in resultsTcAnio)
                            {
                                if (result.IdParamEconomicoDetalle != null)
                                {
                                    paramTCAnio.Append(result.IdParamEconomicoDetalle).Append("|").Append(result.Anio).Append("|").Append(result.Value);
                                    if (cont < resultsTcAnio.Count)
                                    {
                                        paramTCAnio.Append(";");
                                    }
                                }
                                cont++;
                            }
                            obtenerTemplateCorregido.ParamTCAnio = paramTCAnio.ToString();
                        }
                    }
                    if (!string.IsNullOrEmpty(obtenerTemplateCorregido.PETokenIPC))
                    {
                        var resultsIpcAnio = SqlMapper.Query(objConnection, "CAPEX_SEL_PARAM_ECONOMICO_DETALLE_TOKEN", new { PEToken = obtenerTemplateCorregido.PETokenIPC, PasoFinal = pasoFinal }, commandType: CommandType.StoredProcedure).ToList();
                        if (resultsIpcAnio != null && resultsIpcAnio.Count > 0)
                        {
                            var cont = 1;
                            StringBuilder paramIPCAnio = new StringBuilder();
                            foreach (var result in resultsIpcAnio)
                            {
                                if (result.IdParamEconomicoDetalle != null)
                                {
                                    paramIPCAnio.Append(result.IdParamEconomicoDetalle).Append("|").Append(result.Anio).Append("|").Append(result.Value);
                                    if (cont < resultsIpcAnio.Count)
                                    {
                                        paramIPCAnio.Append(";");
                                    }
                                }
                                cont++;
                            }
                            obtenerTemplateCorregido.ParamIPCAnio = paramIPCAnio.ToString();
                        }
                    }
                    if (!string.IsNullOrEmpty(obtenerTemplateCorregido.PETokenCPI))
                    {
                        var resultsCpiAnio = SqlMapper.Query(objConnection, "CAPEX_SEL_PARAM_ECONOMICO_DETALLE_TOKEN", new { PEToken = obtenerTemplateCorregido.PETokenCPI, PasoFinal = pasoFinal }, commandType: CommandType.StoredProcedure).ToList();
                        if (resultsCpiAnio != null && resultsCpiAnio.Count > 0)
                        {
                            var cont = 1;
                            StringBuilder paramCPIAnio = new StringBuilder();
                            foreach (var result in resultsCpiAnio)
                            {
                                if (result.IdParamEconomicoDetalle != null)
                                {
                                    paramCPIAnio.Append(result.IdParamEconomicoDetalle).Append("|").Append(result.Anio).Append("|").Append(result.Value);
                                    if (cont < resultsCpiAnio.Count)
                                    {
                                        paramCPIAnio.Append(";");
                                    }
                                }
                                cont++;
                            }
                            obtenerTemplateCorregido.ParamCPIAnio = paramCPIAnio.ToString();
                        }
                    }
                    return Json(new { Mensaje = "Ok", Resultados = obtenerTemplateCorregido }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "BuscarPorTemplateIniciativa, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
        [Route("ListarIniciativas/{token}")]
        public ActionResult ListarIniciativas(string token)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                Session["ParametroVNToken"] = token.Trim();
                if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    try
                    {
                        //using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
                        using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                        {
                            try
                            {
                                objConnection.Open();

                                var parametroVN = SqlMapper.Query(objConnection, "CAPEX_GET_PARAMETRO_VN", new { ParametroVNToken = token }, commandType: CommandType.StoredProcedure).ToList();
                                ViewBag.Version = string.Empty;
                                if (parametroVN != null && parametroVN.Count > 0)
                                {
                                    foreach (var parVN in parametroVN)
                                    {
                                        ViewBag.Version = parVN.PVNVERSION;
                                    }
                                }
                                var Iniciativa = SqlMapper.Query(objConnection, "CAPEX_SEL_INICIATIVA_PARAMETRO_VN", new { ParametroVNToken = token }, commandType: CommandType.StoredProcedure).ToList();
                                if (Iniciativa != null && Iniciativa.Count > 0)
                                {
                                    ViewBag.Iniciativas = Iniciativa;
                                }
                                else
                                {
                                    ViewBag.Iniciativas = null;
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
                        }

                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }
            }
            return View("~/Views/Orientacion/ListarIniciativas.cshtml");
        }

        private decimal stringToDecimal(string paramValue)
        {
            try
            {
                return decimal.Parse(stringNumberFormat(paramValue), NumberStyles.Number | NumberStyles.AllowExponent);
            }
            catch (Exception err)
            {
                err.ToString();
            }
            return 0;
        }

        private string stringNumberFormat(string paramValue)
        {
            if (!string.IsNullOrEmpty(paramValue))
            {
                if (paramValue.IndexOf(".") != -1 && paramValue.IndexOf(",") != -1)
                {
                    paramValue = paramValue.Replace(".", "");
                    return paramValue.Replace(",", ".");
                }
                else if (paramValue.IndexOf(".") != -1 && paramValue.IndexOf(",") == -1)
                {
                    return paramValue.Replace(".", "");
                }
                else if (paramValue.IndexOf(".") == -1 && paramValue.IndexOf(",") != -1)
                {
                    return paramValue.Replace(",", ".");
                }
                else
                {
                    return paramValue;
                }
            }
            return "0";
        }

        private bool decimalIsZero(decimal paramValue)
        {
            return (Decimal.Compare(Decimal.Zero, paramValue) == 0);
        }

        [HttpPost]
        public ActionResult GenerarVersionParametroVNToken(string ParametroVNToken)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    objConnection.Open();
                    try
                    {
                        string parametroVNToken = string.Empty;
                        if (ParametroVNToken != null && !string.IsNullOrEmpty(ParametroVNToken))
                        {
                            parametroVNToken = ParametroVNToken;
                        }
                        else
                        {
                            parametroVNToken = ((Session["ParametroVNToken"] != null) ? Convert.ToString(Session["ParametroVNToken"]) : "");
                        }
                        if (!string.IsNullOrEmpty(parametroVNToken))
                        {
                            var Iniciativas = SqlMapper.Query(objConnection, "CAPEX_SEL_INICIATIVA_PARAMETRO_VN", new { ParametroVNToken = parametroVNToken }, commandType: CommandType.StoredProcedure).ToList();
                            if (Iniciativas != null && Iniciativas.Count > 0)
                            {
                                foreach (var iniciativa in Iniciativas)
                                {
                                    string iniToken = ((iniciativa != null && iniciativa.PidToken != null && !string.IsNullOrEmpty(iniciativa.PidToken.ToString())) ? iniciativa.PidToken.ToString() : "");
                                    string tipoIniciativaOrientacionComercial = ((Session["tipoIniciativaOrientacionComercial"] != null) ? Convert.ToString(Session["tipoIniciativaOrientacionComercial"]) : "");
                                    var parametos = new DynamicParameters();
                                    parametos.Add("ParametroVNToken", parametroVNToken);
                                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 1);
                                    SqlMapper.Query(objConnection, "CAPEX_PARAMETRO_IS_V0", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                                    string respuesta = parametos.Get<string>("Respuesta");
                                    bool isV0 = false;
                                    if (respuesta != null && !string.IsNullOrEmpty(respuesta.Trim()) && "1".Equals(respuesta.Trim()))
                                    {
                                        isV0 = true;
                                    }
                                    else
                                    {
                                        var paramFinanciero = new DynamicParameters();
                                        paramFinanciero.Add("IniToken", iniToken);
                                        paramFinanciero.Add("ParametroVNToken", parametroVNToken);
                                        paramFinanciero.Add("TipoIniciativa", tipoIniciativaOrientacionComercial);
                                        paramFinanciero.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 1);
                                        SqlMapper.Query(objConnection, "CHECK_IMPORTACION_FINANCIERO", paramFinanciero, commandType: CommandType.StoredProcedure).SingleOrDefault();
                                        string existeImportacionFinanciero = paramFinanciero.Get<string>("Respuesta");
                                        if (existeImportacionFinanciero != null && !string.IsNullOrEmpty(existeImportacionFinanciero.Trim()) && "0".Equals(existeImportacionFinanciero.Trim()))
                                        {
                                            string usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                                            string parametroVNTokenOrigen = string.Empty;
                                            var selParametroVNByParametroVNToken = SqlMapper.Query(objConnection, "CAPEX_SEL_PARAMETRO_VN_BY_ParametroVNToken", new { ParametroVNToken = parametroVNToken }, commandType: CommandType.StoredProcedure).ToList();
                                            if (selParametroVNByParametroVNToken != null && selParametroVNByParametroVNToken.Count > 0)
                                            {
                                                foreach (var s in selParametroVNByParametroVNToken)
                                                {
                                                    parametroVNTokenOrigen = s.ParametroVNTokenOrigen;
                                                }
                                            }
                                            var parametosOrigen = new DynamicParameters();
                                            parametosOrigen.Add("ParametroVNToken", parametroVNTokenOrigen);
                                            parametosOrigen.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 1);
                                            SqlMapper.Query(objConnection, "CAPEX_PARAMETRO_IS_V0", parametosOrigen, commandType: CommandType.StoredProcedure).SingleOrDefault();
                                            string respuestaOrigen = parametosOrigen.Get<string>("Respuesta");

                                            IList<Presupuesto.ParametroOrientacionComercialMes> orientacionComercialTCMesOrigen = null;
                                            IList<Presupuesto.ParametroOrientacionComercialAnio> orientacionComercialTCAnioOrigen = null;
                                            IList<Presupuesto.ParametroOrientacionComercialMes> orientacionComercialIPCMesOrigen = null;
                                            IList<Presupuesto.ParametroOrientacionComercialAnio> orientacionComercialIPCAnioOrigen = null;
                                            IList<Presupuesto.ParametroOrientacionComercialMes> orientacionComercialCPIMesOrigen = null;
                                            IList<Presupuesto.ParametroOrientacionComercialAnio> orientacionComercialCPIAnioOrigen = null;
                                            string tipoTC = "1";
                                            string tipoIPC = "2";
                                            string tipoCPI = "3";
                                            if (respuestaOrigen != null && !string.IsNullOrEmpty(respuestaOrigen.Trim()) && "1".Equals(respuestaOrigen.Trim()))
                                            {
                                                //importacionFinancieroParametroVNOrigen = SqlMapper.Query(objConnection, "CAPEX_SEL_IMPORTACION_FINANCIERO_PARAMETROV0", new { IniToken = iniToken, TipoIniciativa = tipoIniciativaOrientacionComercial }, commandType: CommandType.StoredProcedure).ToList();
                                                orientacionComercialTCMesOrigen = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialMes>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_MES_V0", new { ParametroVNToken = parametroVNTokenOrigen, TipoIniciativa = tipoIniciativaOrientacionComercial, TipoParam = tipoTC }, commandType: CommandType.StoredProcedure).ToList();
                                                orientacionComercialTCAnioOrigen = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialAnio>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_ANIO_V0", new { ParametroVNToken = parametroVNTokenOrigen, TipoIniciativa = tipoIniciativaOrientacionComercial, TipoParam = tipoTC }, commandType: CommandType.StoredProcedure).ToList();
                                                orientacionComercialIPCMesOrigen = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialMes>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_MES_V0", new { ParametroVNToken = parametroVNTokenOrigen, TipoIniciativa = tipoIniciativaOrientacionComercial, TipoParam = tipoIPC }, commandType: CommandType.StoredProcedure).ToList();
                                                orientacionComercialIPCAnioOrigen = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialAnio>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_ANIO_V0", new { ParametroVNToken = parametroVNTokenOrigen, TipoIniciativa = tipoIniciativaOrientacionComercial, TipoParam = tipoIPC }, commandType: CommandType.StoredProcedure).ToList();
                                                orientacionComercialCPIMesOrigen = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialMes>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_MES_V0", new { ParametroVNToken = parametroVNTokenOrigen, TipoIniciativa = tipoIniciativaOrientacionComercial, TipoParam = tipoCPI }, commandType: CommandType.StoredProcedure).ToList();
                                                orientacionComercialCPIAnioOrigen = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialAnio>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_ANIO_V0", new { ParametroVNToken = parametroVNTokenOrigen, TipoIniciativa = tipoIniciativaOrientacionComercial, TipoParam = tipoCPI }, commandType: CommandType.StoredProcedure).ToList();
                                            }
                                            else
                                            {
                                                //importacionFinancieroParametroVNOrigen = SqlMapper.Query(objConnection, "CAPEX_SEL_IMPORTACION_FINANCIERO_PARAMETROVN", new { IniToken = iniToken, ParametroVNToken = parametroVNTokenOrigen, TipoIniciativa = tipoIniciativaOrientacionComercial }, commandType: CommandType.StoredProcedure).ToList();
                                                orientacionComercialTCMesOrigen = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialMes>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_MES", new { ParametroVNToken = parametroVNTokenOrigen, TipoIniciativa = tipoIniciativaOrientacionComercial, TipoParam = tipoTC }, commandType: CommandType.StoredProcedure).ToList();
                                                orientacionComercialTCAnioOrigen = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialAnio>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_ANIO", new { ParametroVNToken = parametroVNTokenOrigen, TipoIniciativa = tipoIniciativaOrientacionComercial, TipoParam = tipoTC }, commandType: CommandType.StoredProcedure).ToList();
                                                orientacionComercialIPCMesOrigen = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialMes>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_MES", new { ParametroVNToken = parametroVNTokenOrigen, TipoIniciativa = tipoIniciativaOrientacionComercial, TipoParam = tipoIPC }, commandType: CommandType.StoredProcedure).ToList();
                                                orientacionComercialIPCAnioOrigen = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialAnio>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_ANIO", new { ParametroVNToken = parametroVNTokenOrigen, TipoIniciativa = tipoIniciativaOrientacionComercial, TipoParam = tipoIPC }, commandType: CommandType.StoredProcedure).ToList();
                                                orientacionComercialCPIMesOrigen = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialMes>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_MES", new { ParametroVNToken = parametroVNTokenOrigen, TipoIniciativa = tipoIniciativaOrientacionComercial, TipoParam = tipoCPI }, commandType: CommandType.StoredProcedure).ToList();
                                                orientacionComercialCPIAnioOrigen = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialAnio>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_ANIO", new { ParametroVNToken = parametroVNTokenOrigen, TipoIniciativa = tipoIniciativaOrientacionComercial, TipoParam = tipoCPI }, commandType: CommandType.StoredProcedure).ToList();
                                            }
                                            var importacionFinancieroParametroVNOrigen = ((respuestaOrigen != null && !string.IsNullOrEmpty(respuestaOrigen.Trim()) && "1".Equals(respuestaOrigen.Trim())) ? SqlMapper.Query(objConnection, "CAPEX_SEL_IMPORTACION_FINANCIERO_PARAMETROV0", new { IniToken = iniToken, TipoIniciativa = tipoIniciativaOrientacionComercial }, commandType: CommandType.StoredProcedure).ToList() : SqlMapper.Query(objConnection, "CAPEX_SEL_IMPORTACION_FINANCIERO_PARAMETROVN", new { IniToken = iniToken, ParametroVNToken = parametroVNTokenOrigen, TipoIniciativa = tipoIniciativaOrientacionComercial }, commandType: CommandType.StoredProcedure).ToList());
                                            if (importacionFinancieroParametroVNOrigen != null && importacionFinancieroParametroVNOrigen.Count > 0)
                                            {
                                                IList<Presupuesto.ParametroOrientacionComercialMes> orientacionComercialTCMesDestino = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialMes>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_MES", new { ParametroVNToken = parametroVNToken, TipoIniciativa = tipoIniciativaOrientacionComercial, TipoParam = tipoTC }, commandType: CommandType.StoredProcedure).ToList();
                                                IList<Presupuesto.ParametroOrientacionComercialAnio> orientacionComercialTCAnioDestino = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialAnio>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_ANIO", new { ParametroVNToken = parametroVNToken, TipoIniciativa = tipoIniciativaOrientacionComercial, TipoParam = tipoTC }, commandType: CommandType.StoredProcedure).ToList();
                                                IList<Presupuesto.ParametroOrientacionComercialMes> orientacionComercialIPCMesDestino = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialMes>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_MES", new { ParametroVNToken = parametroVNToken, TipoIniciativa = tipoIniciativaOrientacionComercial, TipoParam = tipoIPC }, commandType: CommandType.StoredProcedure).ToList();
                                                IList<Presupuesto.ParametroOrientacionComercialAnio> orientacionComercialIPCAnioDestino = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialAnio>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_ANIO", new { ParametroVNToken = parametroVNToken, TipoIniciativa = tipoIniciativaOrientacionComercial, TipoParam = tipoIPC }, commandType: CommandType.StoredProcedure).ToList();
                                                IList<Presupuesto.ParametroOrientacionComercialMes> orientacionComercialCPIMesDestino = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialMes>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_MES", new { ParametroVNToken = parametroVNToken, TipoIniciativa = tipoIniciativaOrientacionComercial, TipoParam = tipoCPI }, commandType: CommandType.StoredProcedure).ToList();
                                                IList<Presupuesto.ParametroOrientacionComercialAnio> orientacionComercialCPIAnioDestino = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialAnio>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_ANIO", new { ParametroVNToken = parametroVNToken, TipoIniciativa = tipoIniciativaOrientacionComercial, TipoParam = tipoCPI }, commandType: CommandType.StoredProcedure).ToList();
                                                string prefixTP = "Total Parcial";
                                                string prefixTA = "Total Acumulado";
                                                string prefixIng = "Ing";
                                                string prefixAdq = "Adq";
                                                string prefixCons = "Cons";
                                                string prefixAdm = "Adm";
                                                string prefixCont = "Cont";
                                                string prefixMonNac = "Moneda Nac";
                                                string prefixMonExt = "Moneda Ext";
                                                IList<Presupuesto.CellValue> ingMes = new List<Presupuesto.CellValue>();
                                                IList<Presupuesto.CellValue> ingAnio = new List<Presupuesto.CellValue>();
                                                IList<Presupuesto.CellValue> adqMes = new List<Presupuesto.CellValue>();
                                                IList<Presupuesto.CellValue> adqAnio = new List<Presupuesto.CellValue>();
                                                IList<Presupuesto.CellValue> consMes = new List<Presupuesto.CellValue>();
                                                IList<Presupuesto.CellValue> consAnio = new List<Presupuesto.CellValue>();
                                                IList<Presupuesto.CellValue> admMes = new List<Presupuesto.CellValue>();
                                                IList<Presupuesto.CellValue> admAnio = new List<Presupuesto.CellValue>();
                                                IList<Presupuesto.CellValue> contMes = new List<Presupuesto.CellValue>();
                                                IList<Presupuesto.CellValue> contAnio = new List<Presupuesto.CellValue>();
                                                IList<Presupuesto.CellValue> totParMes = new List<Presupuesto.CellValue>();
                                                IList<Presupuesto.CellValue> totParAnio = new List<Presupuesto.CellValue>();
                                                IList<Presupuesto.CellValue> totAcumMes = new List<Presupuesto.CellValue>();
                                                IList<Presupuesto.CellValue> totAcumAnio = new List<Presupuesto.CellValue>();
                                                string agrupadorCostoDueño = "% Costo del Dueño";
                                                string keyCostoDuenoAnio = "kcda";
                                                decimal valueCostoDuenoAnio = 0;
                                                string keyCostoDuenoTotalCapex = "kcdtc";
                                                decimal valueCostoDuenoTotalCapex = 0;
                                                string agrupadorContingencia = "% Contingencia";
                                                string keyContingenciaAnio = "kca";
                                                decimal valueContingenciaAnio = 0;
                                                string keyContingenciaTotalCapex = "kctc";
                                                decimal valueContingenciaTotalCapex = 0;
                                                string agrupadorPorcentajeInversion = "Porcentaje (%) de Inversión";
                                                string keyPorcentajeInversionNacional = "kpin";
                                                decimal valuePorcentajeInversionNacional = 0;
                                                string keyPorcentajeInversionExtranjero = "kpie";
                                                decimal valuePorcentajeInversionExtranjero = 0;
                                                foreach (var impFinOrigen in importacionFinancieroParametroVNOrigen)
                                                {
                                                    string IfToken = impFinOrigen.IfToken.ToString();
                                                    string IfDato0 = impFinOrigen.IfDato0.ToString();
                                                    decimal perAntTotal = stringToDecimal(impFinOrigen.IfDato2.ToString());
                                                    if ("1".Equals(tipoIniciativaOrientacionComercial)) // caso base
                                                    {
                                                        IList<Presupuesto.FinancieroDetalleCasoBase> informacionFinancieroDetalleParametroVNOrigen = SqlMapper.Query<Presupuesto.FinancieroDetalleCasoBase>(objConnection, "CAPEX_SEL_IMPORTACION_FINANCIERO_DETALLE_PARAMETROVN", new { TipoIniciativa = tipoIniciativaOrientacionComercial, IfToken = IfToken }, commandType: CommandType.StoredProcedure).ToList();
                                                        if (IfDato0.StartsWith(prefixTA) && (informacionFinancieroDetalleParametroVNOrigen == null || informacionFinancieroDetalleParametroVNOrigen.Count == 0))
                                                        {
                                                            totAcumMes.Add(new Presupuesto.CellValue { Titulo = IfDato0 });
                                                        }
                                                        else if (informacionFinancieroDetalleParametroVNOrigen != null && informacionFinancieroDetalleParametroVNOrigen.Count == 2)
                                                        {
                                                            string IfDato0D = informacionFinancieroDetalleParametroVNOrigen[0].IfDato0.ToString();
                                                            Presupuesto.FinancieroDetalleCasoBase filaMonedaNacional = null;
                                                            Presupuesto.FinancieroDetalleCasoBase filaMonedaExtranjera = null;
                                                            if (IfDato0D.StartsWith(prefixMonNac))
                                                            {
                                                                filaMonedaNacional = informacionFinancieroDetalleParametroVNOrigen[0];
                                                                filaMonedaExtranjera = informacionFinancieroDetalleParametroVNOrigen[1];
                                                            }
                                                            else if (IfDato0D.StartsWith(prefixMonExt))
                                                            {
                                                                filaMonedaNacional = informacionFinancieroDetalleParametroVNOrigen[1];
                                                                filaMonedaExtranjera = informacionFinancieroDetalleParametroVNOrigen[0];
                                                            }
                                                            if (IfDato0.StartsWith(prefixIng))
                                                            {
                                                                decimal perAntNac = stringToDecimal(filaMonedaNacional.IfDato2.ToString());
                                                                decimal perAntExt = stringToDecimal(filaMonedaExtranjera.IfDato2.ToString());
                                                                ingMes.Add(new Presupuesto.CellValue { Titulo = IfDato0, TituloNac = filaMonedaNacional.IfDato0.ToString(), TituloExt = filaMonedaExtranjera.IfDato0.ToString() });
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = perAntNac, ValorExt = perAntExt, ValorTotal = perAntTotal });
                                                                //Enero
                                                                decimal facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[0].Value) || decimalIsZero(orientacionComercialTCMesDestino[0].Value)) ? 0 : ((orientacionComercialIPCMesDestino[0].Value / orientacionComercialIPCMesOrigen[0].Value) * (orientacionComercialTCMesOrigen[0].Value / orientacionComercialTCMesDestino[0].Value)));
                                                                decimal facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[0].Value)) ? 0 : (orientacionComercialCPIMesDestino[0].Value / orientacionComercialCPIMesOrigen[0].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato3) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato3) * facCorrMonExt, ValorTotal = 0 });
                                                                //Febrero
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[1].Value) || decimalIsZero(orientacionComercialTCMesDestino[1].Value)) ? 0 : ((orientacionComercialIPCMesDestino[1].Value / orientacionComercialIPCMesOrigen[1].Value) * (orientacionComercialTCMesOrigen[1].Value / orientacionComercialTCMesDestino[1].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[1].Value)) ? 0 : (orientacionComercialCPIMesDestino[1].Value / orientacionComercialCPIMesOrigen[1].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato4) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato4) * facCorrMonExt, ValorTotal = 0 });
                                                                //Marzo
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[2].Value) || decimalIsZero(orientacionComercialTCMesDestino[2].Value)) ? 0 : ((orientacionComercialIPCMesDestino[2].Value / orientacionComercialIPCMesOrigen[2].Value) * (orientacionComercialTCMesOrigen[2].Value / orientacionComercialTCMesDestino[2].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[2].Value)) ? 0 : (orientacionComercialCPIMesDestino[2].Value / orientacionComercialCPIMesOrigen[2].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato5) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato5) * facCorrMonExt, ValorTotal = 0 });
                                                                //Abril
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[3].Value) || decimalIsZero(orientacionComercialTCMesDestino[3].Value)) ? 0 : ((orientacionComercialIPCMesDestino[3].Value / orientacionComercialIPCMesOrigen[3].Value) * (orientacionComercialTCMesOrigen[3].Value / orientacionComercialTCMesDestino[3].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[3].Value)) ? 0 : (orientacionComercialCPIMesDestino[3].Value / orientacionComercialCPIMesOrigen[3].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato6) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato6) * facCorrMonExt, ValorTotal = 0 });
                                                                //Mayo
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[4].Value) || decimalIsZero(orientacionComercialTCMesDestino[4].Value)) ? 0 : ((orientacionComercialIPCMesDestino[4].Value / orientacionComercialIPCMesOrigen[4].Value) * (orientacionComercialTCMesOrigen[4].Value / orientacionComercialTCMesDestino[4].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[4].Value)) ? 0 : (orientacionComercialCPIMesDestino[4].Value / orientacionComercialCPIMesOrigen[4].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato7) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato7) * facCorrMonExt, ValorTotal = 0 });
                                                                //Junio
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[5].Value) || decimalIsZero(orientacionComercialTCMesDestino[5].Value)) ? 0 : ((orientacionComercialIPCMesDestino[5].Value / orientacionComercialIPCMesOrigen[5].Value) * (orientacionComercialTCMesOrigen[5].Value / orientacionComercialTCMesDestino[5].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[5].Value)) ? 0 : (orientacionComercialCPIMesDestino[5].Value / orientacionComercialCPIMesOrigen[5].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato8) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato8) * facCorrMonExt, ValorTotal = 0 });
                                                                //Julio
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[6].Value) || decimalIsZero(orientacionComercialTCMesDestino[6].Value)) ? 0 : ((orientacionComercialIPCMesDestino[6].Value / orientacionComercialIPCMesOrigen[6].Value) * (orientacionComercialTCMesOrigen[6].Value / orientacionComercialTCMesDestino[6].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[6].Value)) ? 0 : (orientacionComercialCPIMesDestino[6].Value / orientacionComercialCPIMesOrigen[6].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato9) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato9) * facCorrMonExt, ValorTotal = 0 });
                                                                //Agosto
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[7].Value) || decimalIsZero(orientacionComercialTCMesDestino[7].Value)) ? 0 : ((orientacionComercialIPCMesDestino[7].Value / orientacionComercialIPCMesOrigen[7].Value) * (orientacionComercialTCMesOrigen[7].Value / orientacionComercialTCMesDestino[7].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[7].Value)) ? 0 : (orientacionComercialCPIMesDestino[7].Value / orientacionComercialCPIMesOrigen[7].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato10) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato10) * facCorrMonExt, ValorTotal = 0 });
                                                                //Septiembre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[8].Value) || decimalIsZero(orientacionComercialTCMesDestino[8].Value)) ? 0 : ((orientacionComercialIPCMesDestino[8].Value / orientacionComercialIPCMesOrigen[8].Value) * (orientacionComercialTCMesOrigen[8].Value / orientacionComercialTCMesDestino[8].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[8].Value)) ? 0 : (orientacionComercialCPIMesDestino[8].Value / orientacionComercialCPIMesOrigen[8].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato11) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato11) * facCorrMonExt, ValorTotal = 0 });
                                                                //Octubre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[9].Value) || decimalIsZero(orientacionComercialTCMesDestino[9].Value)) ? 0 : ((orientacionComercialIPCMesDestino[9].Value / orientacionComercialIPCMesOrigen[9].Value) * (orientacionComercialTCMesOrigen[9].Value / orientacionComercialTCMesDestino[9].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[9].Value)) ? 0 : (orientacionComercialCPIMesDestino[9].Value / orientacionComercialCPIMesOrigen[9].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato12) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato12) * facCorrMonExt, ValorTotal = 0 });
                                                                //Noviembre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[10].Value) || decimalIsZero(orientacionComercialTCMesDestino[10].Value)) ? 0 : ((orientacionComercialIPCMesDestino[10].Value / orientacionComercialIPCMesOrigen[10].Value) * (orientacionComercialTCMesOrigen[10].Value / orientacionComercialTCMesDestino[10].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[10].Value)) ? 0 : (orientacionComercialCPIMesDestino[10].Value / orientacionComercialCPIMesOrigen[10].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato13) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato13) * facCorrMonExt, ValorTotal = 0 });
                                                                //Diciembre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[11].Value) || decimalIsZero(orientacionComercialTCMesDestino[11].Value)) ? 0 : ((orientacionComercialIPCMesDestino[11].Value / orientacionComercialIPCMesOrigen[11].Value) * (orientacionComercialTCMesOrigen[11].Value / orientacionComercialTCMesDestino[11].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[11].Value)) ? 0 : (orientacionComercialCPIMesDestino[11].Value / orientacionComercialCPIMesOrigen[11].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato14) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato14) * facCorrMonExt, ValorTotal = 0 });
                                                                //Calculo Totales por Mes
                                                                decimal totalValorNacMes = 0;
                                                                decimal totalValorExtMes = 0;
                                                                for (int i = 2; i < ingMes.Count; i++)
                                                                {
                                                                    totalValorNacMes += ingMes[i].ValorNac;
                                                                    totalValorExtMes += ingMes[i].ValorExt;
                                                                    ingMes[i].ValorTotal = (ingMes[i].ValorNac + ingMes[i].ValorExt);
                                                                }
                                                                //Total
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = totalValorNacMes, ValorExt = totalValorExtMes, ValorTotal = (totalValorNacMes + totalValorExtMes) });
                                                                //AñoMas1
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[0].Value) || decimalIsZero(orientacionComercialTCAnioDestino[0].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[0].Value / orientacionComercialIPCAnioOrigen[0].Value) * (orientacionComercialTCAnioOrigen[0].Value / orientacionComercialTCAnioDestino[0].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[0].Value)) ? 0 : (orientacionComercialCPIAnioDestino[0].Value / orientacionComercialCPIAnioOrigen[0].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato16) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato16) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas2
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[1].Value) || decimalIsZero(orientacionComercialTCAnioDestino[1].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[1].Value / orientacionComercialIPCAnioOrigen[1].Value) * (orientacionComercialTCAnioOrigen[1].Value / orientacionComercialTCAnioDestino[1].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[1].Value)) ? 0 : (orientacionComercialCPIAnioDestino[1].Value / orientacionComercialCPIAnioOrigen[1].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato17) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato17) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas3
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[2].Value) || decimalIsZero(orientacionComercialTCAnioDestino[2].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[2].Value / orientacionComercialIPCAnioOrigen[2].Value) * (orientacionComercialTCAnioOrigen[2].Value / orientacionComercialTCAnioDestino[2].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[2].Value)) ? 0 : (orientacionComercialCPIAnioDestino[2].Value / orientacionComercialCPIAnioOrigen[2].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato18) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato18) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas4
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[3].Value) || decimalIsZero(orientacionComercialTCAnioDestino[3].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[3].Value / orientacionComercialIPCAnioOrigen[3].Value) * (orientacionComercialTCAnioOrigen[3].Value / orientacionComercialTCAnioDestino[3].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[3].Value)) ? 0 : (orientacionComercialCPIAnioDestino[3].Value / orientacionComercialCPIAnioOrigen[3].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato19) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato19) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas5
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[4].Value) || decimalIsZero(orientacionComercialTCAnioDestino[4].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[4].Value / orientacionComercialIPCAnioOrigen[4].Value) * (orientacionComercialTCAnioOrigen[4].Value / orientacionComercialTCAnioDestino[4].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[4].Value)) ? 0 : (orientacionComercialCPIAnioDestino[4].Value / orientacionComercialCPIAnioOrigen[4].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato20) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato20) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas6
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[5].Value) || decimalIsZero(orientacionComercialTCAnioDestino[5].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[5].Value / orientacionComercialIPCAnioOrigen[5].Value) * (orientacionComercialTCAnioOrigen[5].Value / orientacionComercialTCAnioDestino[5].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[5].Value)) ? 0 : (orientacionComercialCPIAnioDestino[5].Value / orientacionComercialCPIAnioOrigen[5].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato21) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato21) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas7
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[6].Value) || decimalIsZero(orientacionComercialTCAnioDestino[6].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[6].Value / orientacionComercialIPCAnioOrigen[6].Value) * (orientacionComercialTCAnioOrigen[6].Value / orientacionComercialTCAnioDestino[6].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[6].Value)) ? 0 : (orientacionComercialCPIAnioDestino[6].Value / orientacionComercialCPIAnioOrigen[6].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato22) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato22) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas8
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[7].Value) || decimalIsZero(orientacionComercialTCAnioDestino[7].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[7].Value / orientacionComercialIPCAnioOrigen[7].Value) * (orientacionComercialTCAnioOrigen[7].Value / orientacionComercialTCAnioDestino[7].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[7].Value)) ? 0 : (orientacionComercialCPIAnioDestino[7].Value / orientacionComercialCPIAnioOrigen[7].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato23) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato23) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas9
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[8].Value) || decimalIsZero(orientacionComercialTCAnioDestino[8].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[8].Value / orientacionComercialIPCAnioOrigen[8].Value) * (orientacionComercialTCAnioOrigen[8].Value / orientacionComercialTCAnioDestino[8].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[8].Value)) ? 0 : (orientacionComercialCPIAnioDestino[8].Value / orientacionComercialCPIAnioOrigen[8].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato24) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato24) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas10
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[9].Value) || decimalIsZero(orientacionComercialTCAnioDestino[9].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[9].Value / orientacionComercialIPCAnioOrigen[9].Value) * (orientacionComercialTCAnioOrigen[9].Value / orientacionComercialTCAnioDestino[9].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[9].Value)) ? 0 : (orientacionComercialCPIAnioDestino[9].Value / orientacionComercialCPIAnioOrigen[9].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato25) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato25) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas11
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[10].Value) || decimalIsZero(orientacionComercialTCAnioDestino[10].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[10].Value / orientacionComercialIPCAnioOrigen[10].Value) * (orientacionComercialTCAnioOrigen[10].Value / orientacionComercialTCAnioDestino[10].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[10].Value)) ? 0 : (orientacionComercialCPIAnioDestino[10].Value / orientacionComercialCPIAnioOrigen[10].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato26) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato26) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas12
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[11].Value) || decimalIsZero(orientacionComercialTCAnioDestino[11].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[11].Value / orientacionComercialIPCAnioOrigen[11].Value) * (orientacionComercialTCAnioOrigen[11].Value / orientacionComercialTCAnioDestino[11].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[11].Value)) ? 0 : (orientacionComercialCPIAnioDestino[11].Value / orientacionComercialCPIAnioOrigen[11].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato27) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato27) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas13
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[12].Value) || decimalIsZero(orientacionComercialTCAnioDestino[12].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[12].Value / orientacionComercialIPCAnioOrigen[12].Value) * (orientacionComercialTCAnioOrigen[12].Value / orientacionComercialTCAnioDestino[12].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[12].Value)) ? 0 : (orientacionComercialCPIAnioDestino[12].Value / orientacionComercialCPIAnioOrigen[12].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato28) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato28) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas14
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[13].Value) || decimalIsZero(orientacionComercialTCAnioDestino[13].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[13].Value / orientacionComercialIPCAnioOrigen[13].Value) * (orientacionComercialTCAnioOrigen[13].Value / orientacionComercialTCAnioDestino[13].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[13].Value)) ? 0 : (orientacionComercialCPIAnioDestino[13].Value / orientacionComercialCPIAnioOrigen[13].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato29) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato29) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas15
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[14].Value) || decimalIsZero(orientacionComercialTCAnioDestino[14].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[14].Value / orientacionComercialIPCAnioOrigen[14].Value) * (orientacionComercialTCAnioOrigen[14].Value / orientacionComercialTCAnioDestino[14].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[14].Value)) ? 0 : (orientacionComercialCPIAnioDestino[14].Value / orientacionComercialCPIAnioOrigen[14].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato30) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato30) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas16
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[15].Value) || decimalIsZero(orientacionComercialTCAnioDestino[15].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[15].Value / orientacionComercialIPCAnioOrigen[15].Value) * (orientacionComercialTCAnioOrigen[15].Value / orientacionComercialTCAnioDestino[15].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[15].Value)) ? 0 : (orientacionComercialCPIAnioDestino[15].Value / orientacionComercialCPIAnioOrigen[15].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato31) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato31) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas17
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[16].Value) || decimalIsZero(orientacionComercialTCAnioDestino[16].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[16].Value / orientacionComercialIPCAnioOrigen[16].Value) * (orientacionComercialTCAnioOrigen[16].Value / orientacionComercialTCAnioDestino[16].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[16].Value)) ? 0 : (orientacionComercialCPIAnioDestino[16].Value / orientacionComercialCPIAnioOrigen[16].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato32) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato32) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas18
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[17].Value) || decimalIsZero(orientacionComercialTCAnioDestino[17].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[17].Value / orientacionComercialIPCAnioOrigen[17].Value) * (orientacionComercialTCAnioOrigen[17].Value / orientacionComercialTCAnioDestino[17].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[17].Value)) ? 0 : (orientacionComercialCPIAnioDestino[17].Value / orientacionComercialCPIAnioOrigen[17].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato33) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato33) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas19
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[18].Value) || decimalIsZero(orientacionComercialTCAnioDestino[18].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[18].Value / orientacionComercialIPCAnioOrigen[18].Value) * (orientacionComercialTCAnioOrigen[18].Value / orientacionComercialTCAnioDestino[18].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[18].Value)) ? 0 : (orientacionComercialCPIAnioDestino[18].Value / orientacionComercialCPIAnioOrigen[18].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato34) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato34) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas20
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[19].Value) || decimalIsZero(orientacionComercialTCAnioDestino[19].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[19].Value / orientacionComercialIPCAnioOrigen[19].Value) * (orientacionComercialTCAnioOrigen[19].Value / orientacionComercialTCAnioDestino[19].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[19].Value)) ? 0 : (orientacionComercialCPIAnioDestino[19].Value / orientacionComercialCPIAnioOrigen[19].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato35) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato35) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas21
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[20].Value) || decimalIsZero(orientacionComercialTCAnioDestino[20].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[20].Value / orientacionComercialIPCAnioOrigen[20].Value) * (orientacionComercialTCAnioOrigen[20].Value / orientacionComercialTCAnioDestino[20].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[20].Value)) ? 0 : (orientacionComercialCPIAnioDestino[20].Value / orientacionComercialCPIAnioOrigen[20].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato36) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato36) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas22
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[21].Value) || decimalIsZero(orientacionComercialTCAnioDestino[21].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[21].Value / orientacionComercialIPCAnioOrigen[21].Value) * (orientacionComercialTCAnioOrigen[21].Value / orientacionComercialTCAnioDestino[21].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[21].Value)) ? 0 : (orientacionComercialCPIAnioDestino[21].Value / orientacionComercialCPIAnioOrigen[21].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato37) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato37) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas23
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[22].Value) || decimalIsZero(orientacionComercialTCAnioDestino[22].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[22].Value / orientacionComercialIPCAnioOrigen[22].Value) * (orientacionComercialTCAnioOrigen[22].Value / orientacionComercialTCAnioDestino[22].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[22].Value)) ? 0 : (orientacionComercialCPIAnioDestino[22].Value / orientacionComercialCPIAnioOrigen[22].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato38) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato38) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas24
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[23].Value) || decimalIsZero(orientacionComercialTCAnioDestino[23].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[23].Value / orientacionComercialIPCAnioOrigen[23].Value) * (orientacionComercialTCAnioOrigen[23].Value / orientacionComercialTCAnioDestino[23].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[23].Value)) ? 0 : (orientacionComercialCPIAnioDestino[23].Value / orientacionComercialCPIAnioOrigen[23].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato39) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato39) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas25
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[24].Value) || decimalIsZero(orientacionComercialTCAnioDestino[24].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[24].Value / orientacionComercialIPCAnioOrigen[24].Value) * (orientacionComercialTCAnioOrigen[24].Value / orientacionComercialTCAnioDestino[24].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[24].Value)) ? 0 : (orientacionComercialCPIAnioDestino[24].Value / orientacionComercialCPIAnioOrigen[24].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato40) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato40) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas26
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[25].Value) || decimalIsZero(orientacionComercialTCAnioDestino[25].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[25].Value / orientacionComercialIPCAnioOrigen[25].Value) * (orientacionComercialTCAnioOrigen[25].Value / orientacionComercialTCAnioDestino[25].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[25].Value)) ? 0 : (orientacionComercialCPIAnioDestino[25].Value / orientacionComercialCPIAnioOrigen[25].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato41) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato41) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas27
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[26].Value) || decimalIsZero(orientacionComercialTCAnioDestino[26].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[26].Value / orientacionComercialIPCAnioOrigen[26].Value) * (orientacionComercialTCAnioOrigen[26].Value / orientacionComercialTCAnioDestino[26].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[26].Value)) ? 0 : (orientacionComercialCPIAnioDestino[26].Value / orientacionComercialCPIAnioOrigen[26].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato42) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato42) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas28
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[27].Value) || decimalIsZero(orientacionComercialTCAnioDestino[27].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[27].Value / orientacionComercialIPCAnioOrigen[27].Value) * (orientacionComercialTCAnioOrigen[27].Value / orientacionComercialTCAnioDestino[27].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[27].Value)) ? 0 : (orientacionComercialCPIAnioDestino[27].Value / orientacionComercialCPIAnioOrigen[27].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato43) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato43) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas29
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[28].Value) || decimalIsZero(orientacionComercialTCAnioDestino[28].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[28].Value / orientacionComercialIPCAnioOrigen[28].Value) * (orientacionComercialTCAnioOrigen[28].Value / orientacionComercialTCAnioDestino[28].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[28].Value)) ? 0 : (orientacionComercialCPIAnioDestino[28].Value / orientacionComercialCPIAnioOrigen[28].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato44) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato44) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas30
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[29].Value) || decimalIsZero(orientacionComercialTCAnioDestino[29].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[29].Value / orientacionComercialIPCAnioOrigen[29].Value) * (orientacionComercialTCAnioOrigen[29].Value / orientacionComercialTCAnioDestino[29].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[29].Value)) ? 0 : (orientacionComercialCPIAnioDestino[29].Value / orientacionComercialCPIAnioOrigen[29].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato45) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato45) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas31
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[30].Value) || decimalIsZero(orientacionComercialTCAnioDestino[30].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[30].Value / orientacionComercialIPCAnioOrigen[30].Value) * (orientacionComercialTCAnioOrigen[30].Value / orientacionComercialTCAnioDestino[30].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[30].Value)) ? 0 : (orientacionComercialCPIAnioDestino[30].Value / orientacionComercialCPIAnioOrigen[30].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato46) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato46) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas32
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[31].Value) || decimalIsZero(orientacionComercialTCAnioDestino[31].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[31].Value / orientacionComercialIPCAnioOrigen[31].Value) * (orientacionComercialTCAnioOrigen[31].Value / orientacionComercialTCAnioDestino[31].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[31].Value)) ? 0 : (orientacionComercialCPIAnioDestino[31].Value / orientacionComercialCPIAnioOrigen[31].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato47) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato47) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas33
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[32].Value) || decimalIsZero(orientacionComercialTCAnioDestino[32].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[32].Value / orientacionComercialIPCAnioOrigen[32].Value) * (orientacionComercialTCAnioOrigen[32].Value / orientacionComercialTCAnioDestino[32].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[32].Value)) ? 0 : (orientacionComercialCPIAnioDestino[32].Value / orientacionComercialCPIAnioOrigen[32].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato48) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato48) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas34
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[33].Value) || decimalIsZero(orientacionComercialTCAnioDestino[33].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[33].Value / orientacionComercialIPCAnioOrigen[33].Value) * (orientacionComercialTCAnioOrigen[33].Value / orientacionComercialTCAnioDestino[33].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[33].Value)) ? 0 : (orientacionComercialCPIAnioDestino[33].Value / orientacionComercialCPIAnioOrigen[33].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato49) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato49) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas35
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[34].Value) || decimalIsZero(orientacionComercialTCAnioDestino[34].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[34].Value / orientacionComercialIPCAnioOrigen[34].Value) * (orientacionComercialTCAnioOrigen[34].Value / orientacionComercialTCAnioDestino[34].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[34].Value)) ? 0 : (orientacionComercialCPIAnioDestino[34].Value / orientacionComercialCPIAnioOrigen[34].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato50) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato50) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas36
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[35].Value) || decimalIsZero(orientacionComercialTCAnioDestino[35].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[35].Value / orientacionComercialIPCAnioOrigen[35].Value) * (orientacionComercialTCAnioOrigen[35].Value / orientacionComercialTCAnioDestino[35].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[35].Value)) ? 0 : (orientacionComercialCPIAnioDestino[35].Value / orientacionComercialCPIAnioOrigen[35].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato51) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato51) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas37
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[36].Value) || decimalIsZero(orientacionComercialTCAnioDestino[36].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[36].Value / orientacionComercialIPCAnioOrigen[36].Value) * (orientacionComercialTCAnioOrigen[36].Value / orientacionComercialTCAnioDestino[36].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[36].Value)) ? 0 : (orientacionComercialCPIAnioDestino[36].Value / orientacionComercialCPIAnioOrigen[36].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato52) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato52) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas38
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[37].Value) || decimalIsZero(orientacionComercialTCAnioDestino[37].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[37].Value / orientacionComercialIPCAnioOrigen[37].Value) * (orientacionComercialTCAnioOrigen[37].Value / orientacionComercialTCAnioDestino[37].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[37].Value)) ? 0 : (orientacionComercialCPIAnioDestino[37].Value / orientacionComercialCPIAnioOrigen[37].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato53) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato53) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas39
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[38].Value) || decimalIsZero(orientacionComercialTCAnioDestino[38].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[38].Value / orientacionComercialIPCAnioOrigen[38].Value) * (orientacionComercialTCAnioOrigen[38].Value / orientacionComercialTCAnioDestino[38].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[38].Value)) ? 0 : (orientacionComercialCPIAnioDestino[38].Value / orientacionComercialCPIAnioOrigen[38].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato54) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato54) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas40
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[39].Value) || decimalIsZero(orientacionComercialTCAnioDestino[39].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[39].Value / orientacionComercialIPCAnioOrigen[39].Value) * (orientacionComercialTCAnioOrigen[39].Value / orientacionComercialTCAnioDestino[39].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[39].Value)) ? 0 : (orientacionComercialCPIAnioDestino[39].Value / orientacionComercialCPIAnioOrigen[39].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato55) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato55) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas41
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[40].Value) || decimalIsZero(orientacionComercialTCAnioDestino[40].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[40].Value / orientacionComercialIPCAnioOrigen[40].Value) * (orientacionComercialTCAnioOrigen[40].Value / orientacionComercialTCAnioDestino[40].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[40].Value)) ? 0 : (orientacionComercialCPIAnioDestino[40].Value / orientacionComercialCPIAnioOrigen[40].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato56) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato56) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas42
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[41].Value) || decimalIsZero(orientacionComercialTCAnioDestino[41].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[41].Value / orientacionComercialIPCAnioOrigen[41].Value) * (orientacionComercialTCAnioOrigen[41].Value / orientacionComercialTCAnioDestino[41].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[41].Value)) ? 0 : (orientacionComercialCPIAnioDestino[41].Value / orientacionComercialCPIAnioOrigen[41].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato57) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato57) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas43
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[42].Value) || decimalIsZero(orientacionComercialTCAnioDestino[42].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[42].Value / orientacionComercialIPCAnioOrigen[42].Value) * (orientacionComercialTCAnioOrigen[42].Value / orientacionComercialTCAnioDestino[42].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[42].Value)) ? 0 : (orientacionComercialCPIAnioDestino[42].Value / orientacionComercialCPIAnioOrigen[42].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato58) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato58) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas44
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[43].Value) || decimalIsZero(orientacionComercialTCAnioDestino[43].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[43].Value / orientacionComercialIPCAnioOrigen[43].Value) * (orientacionComercialTCAnioOrigen[43].Value / orientacionComercialTCAnioDestino[43].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[43].Value)) ? 0 : (orientacionComercialCPIAnioDestino[43].Value / orientacionComercialCPIAnioOrigen[43].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato59) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato59) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas45
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[44].Value) || decimalIsZero(orientacionComercialTCAnioDestino[44].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[44].Value / orientacionComercialIPCAnioOrigen[44].Value) * (orientacionComercialTCAnioOrigen[44].Value / orientacionComercialTCAnioDestino[44].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[44].Value)) ? 0 : (orientacionComercialCPIAnioDestino[44].Value / orientacionComercialCPIAnioOrigen[44].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato60) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato60) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas46
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[45].Value) || decimalIsZero(orientacionComercialTCAnioDestino[45].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[45].Value / orientacionComercialIPCAnioOrigen[45].Value) * (orientacionComercialTCAnioOrigen[45].Value / orientacionComercialTCAnioDestino[45].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[45].Value)) ? 0 : (orientacionComercialCPIAnioDestino[45].Value / orientacionComercialCPIAnioOrigen[45].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato61) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato61) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas47
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[46].Value) || decimalIsZero(orientacionComercialTCAnioDestino[46].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[46].Value / orientacionComercialIPCAnioOrigen[46].Value) * (orientacionComercialTCAnioOrigen[46].Value / orientacionComercialTCAnioDestino[46].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[46].Value)) ? 0 : (orientacionComercialCPIAnioDestino[46].Value / orientacionComercialCPIAnioOrigen[46].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato62) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato62) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas48
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[47].Value) || decimalIsZero(orientacionComercialTCAnioDestino[47].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[47].Value / orientacionComercialIPCAnioOrigen[47].Value) * (orientacionComercialTCAnioOrigen[47].Value / orientacionComercialTCAnioDestino[47].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[47].Value)) ? 0 : (orientacionComercialCPIAnioDestino[47].Value / orientacionComercialCPIAnioOrigen[47].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato63) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato63) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas49
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[48].Value) || decimalIsZero(orientacionComercialTCAnioDestino[48].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[48].Value / orientacionComercialIPCAnioOrigen[48].Value) * (orientacionComercialTCAnioOrigen[48].Value / orientacionComercialTCAnioDestino[48].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[48].Value)) ? 0 : (orientacionComercialCPIAnioDestino[48].Value / orientacionComercialCPIAnioOrigen[48].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato64) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato64) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas50
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[49].Value) || decimalIsZero(orientacionComercialTCAnioDestino[49].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[49].Value / orientacionComercialIPCAnioOrigen[49].Value) * (orientacionComercialTCAnioOrigen[49].Value / orientacionComercialTCAnioDestino[49].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[49].Value)) ? 0 : (orientacionComercialCPIAnioDestino[49].Value / orientacionComercialCPIAnioOrigen[49].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato65) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato65) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas51
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[50].Value) || decimalIsZero(orientacionComercialTCAnioDestino[50].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[50].Value / orientacionComercialIPCAnioOrigen[50].Value) * (orientacionComercialTCAnioOrigen[50].Value / orientacionComercialTCAnioDestino[50].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[50].Value)) ? 0 : (orientacionComercialCPIAnioDestino[50].Value / orientacionComercialCPIAnioOrigen[50].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato66) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato66) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas52
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[51].Value) || decimalIsZero(orientacionComercialTCAnioDestino[51].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[51].Value / orientacionComercialIPCAnioOrigen[51].Value) * (orientacionComercialTCAnioOrigen[51].Value / orientacionComercialTCAnioDestino[51].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[51].Value)) ? 0 : (orientacionComercialCPIAnioDestino[51].Value / orientacionComercialCPIAnioOrigen[51].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato67) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato67) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas53
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[52].Value) || decimalIsZero(orientacionComercialTCAnioDestino[52].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[52].Value / orientacionComercialIPCAnioOrigen[52].Value) * (orientacionComercialTCAnioOrigen[52].Value / orientacionComercialTCAnioDestino[52].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[52].Value)) ? 0 : (orientacionComercialCPIAnioDestino[52].Value / orientacionComercialCPIAnioOrigen[52].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato68) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato68) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas54
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[53].Value) || decimalIsZero(orientacionComercialTCAnioDestino[53].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[53].Value / orientacionComercialIPCAnioOrigen[53].Value) * (orientacionComercialTCAnioOrigen[53].Value / orientacionComercialTCAnioDestino[53].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[53].Value)) ? 0 : (orientacionComercialCPIAnioDestino[53].Value / orientacionComercialCPIAnioOrigen[53].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato69) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato69) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas55
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[54].Value) || decimalIsZero(orientacionComercialTCAnioDestino[54].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[54].Value / orientacionComercialIPCAnioOrigen[54].Value) * (orientacionComercialTCAnioOrigen[54].Value / orientacionComercialTCAnioDestino[54].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[54].Value)) ? 0 : (orientacionComercialCPIAnioDestino[54].Value / orientacionComercialCPIAnioOrigen[54].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato70) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato70) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas56
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[55].Value) || decimalIsZero(orientacionComercialTCAnioDestino[55].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[55].Value / orientacionComercialIPCAnioOrigen[55].Value) * (orientacionComercialTCAnioOrigen[55].Value / orientacionComercialTCAnioDestino[55].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[55].Value)) ? 0 : (orientacionComercialCPIAnioDestino[55].Value / orientacionComercialCPIAnioOrigen[55].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato71) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato71) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas57
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[56].Value) || decimalIsZero(orientacionComercialTCAnioDestino[56].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[56].Value / orientacionComercialIPCAnioOrigen[56].Value) * (orientacionComercialTCAnioOrigen[56].Value / orientacionComercialTCAnioDestino[56].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[56].Value)) ? 0 : (orientacionComercialCPIAnioDestino[56].Value / orientacionComercialCPIAnioOrigen[56].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato72) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato72) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas58
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[57].Value) || decimalIsZero(orientacionComercialTCAnioDestino[57].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[57].Value / orientacionComercialIPCAnioOrigen[57].Value) * (orientacionComercialTCAnioOrigen[57].Value / orientacionComercialTCAnioDestino[57].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[57].Value)) ? 0 : (orientacionComercialCPIAnioDestino[57].Value / orientacionComercialCPIAnioOrigen[57].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato73) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato73) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas59
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[58].Value) || decimalIsZero(orientacionComercialTCAnioDestino[58].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[58].Value / orientacionComercialIPCAnioOrigen[58].Value) * (orientacionComercialTCAnioOrigen[58].Value / orientacionComercialTCAnioDestino[58].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[58].Value)) ? 0 : (orientacionComercialCPIAnioDestino[58].Value / orientacionComercialCPIAnioOrigen[58].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato74) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato74) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas60
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[59].Value) || decimalIsZero(orientacionComercialTCAnioDestino[59].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[59].Value / orientacionComercialIPCAnioOrigen[59].Value) * (orientacionComercialTCAnioOrigen[59].Value / orientacionComercialTCAnioDestino[59].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[59].Value)) ? 0 : (orientacionComercialCPIAnioDestino[59].Value / orientacionComercialCPIAnioOrigen[59].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato75) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato75) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas61
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[60].Value) || decimalIsZero(orientacionComercialTCAnioDestino[60].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[60].Value / orientacionComercialIPCAnioOrigen[60].Value) * (orientacionComercialTCAnioOrigen[60].Value / orientacionComercialTCAnioDestino[60].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[60].Value)) ? 0 : (orientacionComercialCPIAnioDestino[60].Value / orientacionComercialCPIAnioOrigen[60].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato76) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato76) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas62
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[61].Value) || decimalIsZero(orientacionComercialTCAnioDestino[61].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[61].Value / orientacionComercialIPCAnioOrigen[61].Value) * (orientacionComercialTCAnioOrigen[61].Value / orientacionComercialTCAnioDestino[61].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[61].Value)) ? 0 : (orientacionComercialCPIAnioDestino[61].Value / orientacionComercialCPIAnioOrigen[61].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato77) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato77) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas63
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[62].Value) || decimalIsZero(orientacionComercialTCAnioDestino[62].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[62].Value / orientacionComercialIPCAnioOrigen[62].Value) * (orientacionComercialTCAnioOrigen[62].Value / orientacionComercialTCAnioDestino[62].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[62].Value)) ? 0 : (orientacionComercialCPIAnioDestino[62].Value / orientacionComercialCPIAnioOrigen[62].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato78) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato78) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas64
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[63].Value) || decimalIsZero(orientacionComercialTCAnioDestino[63].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[63].Value / orientacionComercialIPCAnioOrigen[63].Value) * (orientacionComercialTCAnioOrigen[63].Value / orientacionComercialTCAnioDestino[63].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[63].Value)) ? 0 : (orientacionComercialCPIAnioDestino[63].Value / orientacionComercialCPIAnioOrigen[63].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato79) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato79) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas65
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[64].Value) || decimalIsZero(orientacionComercialTCAnioDestino[64].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[64].Value / orientacionComercialIPCAnioOrigen[64].Value) * (orientacionComercialTCAnioOrigen[64].Value / orientacionComercialTCAnioDestino[64].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[64].Value)) ? 0 : (orientacionComercialCPIAnioDestino[64].Value / orientacionComercialCPIAnioOrigen[64].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato80) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato80) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas66
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[65].Value) || decimalIsZero(orientacionComercialTCAnioDestino[65].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[65].Value / orientacionComercialIPCAnioOrigen[65].Value) * (orientacionComercialTCAnioOrigen[65].Value / orientacionComercialTCAnioDestino[65].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[65].Value)) ? 0 : (orientacionComercialCPIAnioDestino[65].Value / orientacionComercialCPIAnioOrigen[65].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato81) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato81) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas67
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[66].Value) || decimalIsZero(orientacionComercialTCAnioDestino[66].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[66].Value / orientacionComercialIPCAnioOrigen[66].Value) * (orientacionComercialTCAnioOrigen[66].Value / orientacionComercialTCAnioDestino[66].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[66].Value)) ? 0 : (orientacionComercialCPIAnioDestino[66].Value / orientacionComercialCPIAnioOrigen[66].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato82) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato82) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas68
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[67].Value) || decimalIsZero(orientacionComercialTCAnioDestino[67].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[67].Value / orientacionComercialIPCAnioOrigen[67].Value) * (orientacionComercialTCAnioOrigen[67].Value / orientacionComercialTCAnioDestino[67].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[67].Value)) ? 0 : (orientacionComercialCPIAnioDestino[67].Value / orientacionComercialCPIAnioOrigen[67].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato83) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato83) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas69
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[68].Value) || decimalIsZero(orientacionComercialTCAnioDestino[68].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[68].Value / orientacionComercialIPCAnioOrigen[68].Value) * (orientacionComercialTCAnioOrigen[68].Value / orientacionComercialTCAnioDestino[68].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[68].Value)) ? 0 : (orientacionComercialCPIAnioDestino[68].Value / orientacionComercialCPIAnioOrigen[68].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato84) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato84) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas70
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[69].Value) || decimalIsZero(orientacionComercialTCAnioDestino[69].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[69].Value / orientacionComercialIPCAnioOrigen[69].Value) * (orientacionComercialTCAnioOrigen[69].Value / orientacionComercialTCAnioDestino[69].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[69].Value)) ? 0 : (orientacionComercialCPIAnioDestino[69].Value / orientacionComercialCPIAnioOrigen[69].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato85) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato85) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas71
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[70].Value) || decimalIsZero(orientacionComercialTCAnioDestino[70].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[70].Value / orientacionComercialIPCAnioOrigen[70].Value) * (orientacionComercialTCAnioOrigen[70].Value / orientacionComercialTCAnioDestino[70].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[70].Value)) ? 0 : (orientacionComercialCPIAnioDestino[70].Value / orientacionComercialCPIAnioOrigen[70].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato86) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato86) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas72
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[71].Value) || decimalIsZero(orientacionComercialTCAnioDestino[71].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[71].Value / orientacionComercialIPCAnioOrigen[71].Value) * (orientacionComercialTCAnioOrigen[71].Value / orientacionComercialTCAnioDestino[71].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[71].Value)) ? 0 : (orientacionComercialCPIAnioDestino[71].Value / orientacionComercialCPIAnioOrigen[71].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato87) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato87) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas73
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[72].Value) || decimalIsZero(orientacionComercialTCAnioDestino[72].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[72].Value / orientacionComercialIPCAnioOrigen[72].Value) * (orientacionComercialTCAnioOrigen[72].Value / orientacionComercialTCAnioDestino[72].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[72].Value)) ? 0 : (orientacionComercialCPIAnioDestino[72].Value / orientacionComercialCPIAnioOrigen[72].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato88) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato88) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas74
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[73].Value) || decimalIsZero(orientacionComercialTCAnioDestino[73].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[73].Value / orientacionComercialIPCAnioOrigen[73].Value) * (orientacionComercialTCAnioOrigen[73].Value / orientacionComercialTCAnioDestino[73].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[73].Value)) ? 0 : (orientacionComercialCPIAnioDestino[73].Value / orientacionComercialCPIAnioOrigen[73].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato89) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato89) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas75
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[74].Value) || decimalIsZero(orientacionComercialTCAnioDestino[74].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[74].Value / orientacionComercialIPCAnioOrigen[74].Value) * (orientacionComercialTCAnioOrigen[74].Value / orientacionComercialTCAnioDestino[74].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[74].Value)) ? 0 : (orientacionComercialCPIAnioDestino[74].Value / orientacionComercialCPIAnioOrigen[74].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato90) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato90) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas76
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[75].Value) || decimalIsZero(orientacionComercialTCAnioDestino[75].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[75].Value / orientacionComercialIPCAnioOrigen[75].Value) * (orientacionComercialTCAnioOrigen[75].Value / orientacionComercialTCAnioDestino[75].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[75].Value)) ? 0 : (orientacionComercialCPIAnioDestino[75].Value / orientacionComercialCPIAnioOrigen[75].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato91) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato91) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas77
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[76].Value) || decimalIsZero(orientacionComercialTCAnioDestino[76].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[76].Value / orientacionComercialIPCAnioOrigen[76].Value) * (orientacionComercialTCAnioOrigen[76].Value / orientacionComercialTCAnioDestino[76].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[76].Value)) ? 0 : (orientacionComercialCPIAnioDestino[76].Value / orientacionComercialCPIAnioOrigen[76].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato92) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato92) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas78
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[77].Value) || decimalIsZero(orientacionComercialTCAnioDestino[77].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[77].Value / orientacionComercialIPCAnioOrigen[77].Value) * (orientacionComercialTCAnioOrigen[77].Value / orientacionComercialTCAnioDestino[77].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[77].Value)) ? 0 : (orientacionComercialCPIAnioDestino[77].Value / orientacionComercialCPIAnioOrigen[77].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato93) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato93) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas79
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[78].Value) || decimalIsZero(orientacionComercialTCAnioDestino[78].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[78].Value / orientacionComercialIPCAnioOrigen[78].Value) * (orientacionComercialTCAnioOrigen[78].Value / orientacionComercialTCAnioDestino[78].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[78].Value)) ? 0 : (orientacionComercialCPIAnioDestino[78].Value / orientacionComercialCPIAnioOrigen[78].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato94) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato94) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas80
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[79].Value) || decimalIsZero(orientacionComercialTCAnioDestino[79].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[79].Value / orientacionComercialIPCAnioOrigen[79].Value) * (orientacionComercialTCAnioOrigen[79].Value / orientacionComercialTCAnioDestino[79].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[79].Value)) ? 0 : (orientacionComercialCPIAnioDestino[79].Value / orientacionComercialCPIAnioOrigen[79].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato95) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato95) * facCorrMonExt, ValorTotal = 0 });
                                                                //Total por año
                                                                decimal totalValorNacAnio = 0;
                                                                decimal totalValorExtAnio = 0;
                                                                for (int i = 0; i < ingAnio.Count; i++)
                                                                {
                                                                    totalValorNacAnio += ingAnio[i].ValorNac;
                                                                    totalValorExtAnio += ingAnio[i].ValorExt;
                                                                    ingAnio[i].ValorTotal = (ingAnio[i].ValorNac + ingAnio[i].ValorExt);
                                                                }
                                                                //Total Capex
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = (perAntNac + totalValorNacMes + totalValorNacAnio), ValorExt = (perAntExt + totalValorExtMes + totalValorExtAnio), ValorTotal = ((perAntNac + totalValorNacMes + totalValorNacAnio) + (perAntExt + totalValorExtMes + totalValorExtAnio)) });
                                                            }
                                                            else if (IfDato0.StartsWith(prefixAdq))
                                                            {
                                                                adqMes.Add(new Presupuesto.CellValue { Titulo = IfDato0, TituloNac = filaMonedaNacional.IfDato0, TituloExt = filaMonedaExtranjera.IfDato0 });
                                                                decimal perAntNac = stringToDecimal(filaMonedaNacional.IfDato2);
                                                                decimal perAntExt = stringToDecimal(filaMonedaExtranjera.IfDato2);
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = perAntNac, ValorExt = perAntExt, ValorTotal = perAntTotal });
                                                                //Enero
                                                                decimal facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[0].Value) || decimalIsZero(orientacionComercialTCMesDestino[0].Value)) ? 0 : ((orientacionComercialIPCMesDestino[0].Value / orientacionComercialIPCMesOrigen[0].Value) * (orientacionComercialTCMesOrigen[0].Value / orientacionComercialTCMesDestino[0].Value)));
                                                                decimal facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[0].Value)) ? 0 : (orientacionComercialCPIMesDestino[0].Value / orientacionComercialCPIMesOrigen[0].Value));
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato3) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato3) * facCorrMonExt, ValorTotal = 0 });
                                                                //Febrero
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[1].Value) || decimalIsZero(orientacionComercialTCMesDestino[1].Value)) ? 0 : ((orientacionComercialIPCMesDestino[1].Value / orientacionComercialIPCMesOrigen[1].Value) * (orientacionComercialTCMesOrigen[1].Value / orientacionComercialTCMesDestino[1].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[1].Value)) ? 0 : (orientacionComercialCPIMesDestino[1].Value / orientacionComercialCPIMesOrigen[1].Value));
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato4) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato4) * facCorrMonExt, ValorTotal = 0 });
                                                                //Marzo
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[2].Value) || decimalIsZero(orientacionComercialTCMesDestino[2].Value)) ? 0 : ((orientacionComercialIPCMesDestino[2].Value / orientacionComercialIPCMesOrigen[2].Value) * (orientacionComercialTCMesOrigen[2].Value / orientacionComercialTCMesDestino[2].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[2].Value)) ? 0 : (orientacionComercialCPIMesDestino[2].Value / orientacionComercialCPIMesOrigen[2].Value));
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato5) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato5) * facCorrMonExt, ValorTotal = 0 });
                                                                //Abril
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[3].Value) || decimalIsZero(orientacionComercialTCMesDestino[3].Value)) ? 0 : ((orientacionComercialIPCMesDestino[3].Value / orientacionComercialIPCMesOrigen[3].Value) * (orientacionComercialTCMesOrigen[3].Value / orientacionComercialTCMesDestino[3].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[3].Value)) ? 0 : (orientacionComercialCPIMesDestino[3].Value / orientacionComercialCPIMesOrigen[3].Value));
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato6) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato6) * facCorrMonExt, ValorTotal = 0 });
                                                                //Mayo
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[4].Value) || decimalIsZero(orientacionComercialTCMesDestino[4].Value)) ? 0 : ((orientacionComercialIPCMesDestino[4].Value / orientacionComercialIPCMesOrigen[4].Value) * (orientacionComercialTCMesOrigen[4].Value / orientacionComercialTCMesDestino[4].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[4].Value)) ? 0 : (orientacionComercialCPIMesDestino[4].Value / orientacionComercialCPIMesOrigen[4].Value));
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato7) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato7) * facCorrMonExt, ValorTotal = 0 });
                                                                //Junio
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[5].Value) || decimalIsZero(orientacionComercialTCMesDestino[5].Value)) ? 0 : ((orientacionComercialIPCMesDestino[5].Value / orientacionComercialIPCMesOrigen[5].Value) * (orientacionComercialTCMesOrigen[5].Value / orientacionComercialTCMesDestino[5].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[5].Value)) ? 0 : (orientacionComercialCPIMesDestino[5].Value / orientacionComercialCPIMesOrigen[5].Value));
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato8) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato8) * facCorrMonExt, ValorTotal = 0 });
                                                                //Julio
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[6].Value) || decimalIsZero(orientacionComercialTCMesDestino[6].Value)) ? 0 : ((orientacionComercialIPCMesDestino[6].Value / orientacionComercialIPCMesOrigen[6].Value) * (orientacionComercialTCMesOrigen[6].Value / orientacionComercialTCMesDestino[6].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[6].Value)) ? 0 : (orientacionComercialCPIMesDestino[6].Value / orientacionComercialCPIMesOrigen[6].Value));
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato9) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato9) * facCorrMonExt, ValorTotal = 0 });
                                                                //Agosto
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[7].Value) || decimalIsZero(orientacionComercialTCMesDestino[7].Value)) ? 0 : ((orientacionComercialIPCMesDestino[7].Value / orientacionComercialIPCMesOrigen[7].Value) * (orientacionComercialTCMesOrigen[7].Value / orientacionComercialTCMesDestino[7].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[7].Value)) ? 0 : (orientacionComercialCPIMesDestino[7].Value / orientacionComercialCPIMesOrigen[7].Value));
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato10) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato10) * facCorrMonExt, ValorTotal = 0 });
                                                                //Septiembre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[8].Value) || decimalIsZero(orientacionComercialTCMesDestino[8].Value)) ? 0 : ((orientacionComercialIPCMesDestino[8].Value / orientacionComercialIPCMesOrigen[8].Value) * (orientacionComercialTCMesOrigen[8].Value / orientacionComercialTCMesDestino[8].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[8].Value)) ? 0 : (orientacionComercialCPIMesDestino[8].Value / orientacionComercialCPIMesOrigen[8].Value));
                                                                //Octubre
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato11) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato11) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[9].Value) || decimalIsZero(orientacionComercialTCMesDestino[9].Value)) ? 0 : ((orientacionComercialIPCMesDestino[9].Value / orientacionComercialIPCMesOrigen[9].Value) * (orientacionComercialTCMesOrigen[9].Value / orientacionComercialTCMesDestino[9].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[9].Value)) ? 0 : (orientacionComercialCPIMesDestino[9].Value / orientacionComercialCPIMesOrigen[9].Value));
                                                                //Noviembre
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato12) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato12) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[10].Value) || decimalIsZero(orientacionComercialTCMesDestino[10].Value)) ? 0 : ((orientacionComercialIPCMesDestino[10].Value / orientacionComercialIPCMesOrigen[10].Value) * (orientacionComercialTCMesOrigen[10].Value / orientacionComercialTCMesDestino[10].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[10].Value)) ? 0 : (orientacionComercialCPIMesDestino[10].Value / orientacionComercialCPIMesOrigen[10].Value));
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato13) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato13) * facCorrMonExt, ValorTotal = 0 });
                                                                //Diciembre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[11].Value) || decimalIsZero(orientacionComercialTCMesDestino[11].Value)) ? 0 : ((orientacionComercialIPCMesDestino[11].Value / orientacionComercialIPCMesOrigen[11].Value) * (orientacionComercialTCMesOrigen[11].Value / orientacionComercialTCMesDestino[11].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[11].Value)) ? 0 : (orientacionComercialCPIMesDestino[11].Value / orientacionComercialCPIMesOrigen[11].Value));
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato14) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato14) * facCorrMonExt, ValorTotal = 0 });
                                                                //Calculo Totales por Mes
                                                                decimal totalValorNacMes = 0;
                                                                decimal totalValorExtMes = 0;
                                                                for (int i = 2; i < adqMes.Count; i++)
                                                                {
                                                                    totalValorNacMes += adqMes[i].ValorNac;
                                                                    totalValorExtMes += adqMes[i].ValorExt;
                                                                    adqMes[i].ValorTotal = (adqMes[i].ValorNac + adqMes[i].ValorExt);
                                                                }
                                                                //Total
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = totalValorNacMes, ValorExt = totalValorExtMes, ValorTotal = (totalValorNacMes + totalValorExtMes) });
                                                                //AñoMas1
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[0].Value) || decimalIsZero(orientacionComercialTCAnioDestino[0].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[0].Value / orientacionComercialIPCAnioOrigen[0].Value) * (orientacionComercialTCAnioOrigen[0].Value / orientacionComercialTCAnioDestino[0].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[0].Value)) ? 0 : (orientacionComercialCPIAnioDestino[0].Value / orientacionComercialCPIAnioOrigen[0].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato16) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato16) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas2
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[1].Value) || decimalIsZero(orientacionComercialTCAnioDestino[1].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[1].Value / orientacionComercialIPCAnioOrigen[1].Value) * (orientacionComercialTCAnioOrigen[1].Value / orientacionComercialTCAnioDestino[1].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[1].Value)) ? 0 : (orientacionComercialCPIAnioDestino[1].Value / orientacionComercialCPIAnioOrigen[1].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato17) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato17) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas3
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[2].Value) || decimalIsZero(orientacionComercialTCAnioDestino[2].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[2].Value / orientacionComercialIPCAnioOrigen[2].Value) * (orientacionComercialTCAnioOrigen[2].Value / orientacionComercialTCAnioDestino[2].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[2].Value)) ? 0 : (orientacionComercialCPIAnioDestino[2].Value / orientacionComercialCPIAnioOrigen[2].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato18) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato18) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas4
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[3].Value) || decimalIsZero(orientacionComercialTCAnioDestino[3].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[3].Value / orientacionComercialIPCAnioOrigen[3].Value) * (orientacionComercialTCAnioOrigen[3].Value / orientacionComercialTCAnioDestino[3].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[3].Value)) ? 0 : (orientacionComercialCPIAnioDestino[3].Value / orientacionComercialCPIAnioOrigen[3].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato19) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato19) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas5
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[4].Value) || decimalIsZero(orientacionComercialTCAnioDestino[4].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[4].Value / orientacionComercialIPCAnioOrigen[4].Value) * (orientacionComercialTCAnioOrigen[4].Value / orientacionComercialTCAnioDestino[4].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[4].Value)) ? 0 : (orientacionComercialCPIAnioDestino[4].Value / orientacionComercialCPIAnioOrigen[4].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato20) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato20) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas6
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[5].Value) || decimalIsZero(orientacionComercialTCAnioDestino[5].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[5].Value / orientacionComercialIPCAnioOrigen[5].Value) * (orientacionComercialTCAnioOrigen[5].Value / orientacionComercialTCAnioDestino[5].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[5].Value)) ? 0 : (orientacionComercialCPIAnioDestino[5].Value / orientacionComercialCPIAnioOrigen[5].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato21) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato21) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas7
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[6].Value) || decimalIsZero(orientacionComercialTCAnioDestino[6].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[6].Value / orientacionComercialIPCAnioOrigen[6].Value) * (orientacionComercialTCAnioOrigen[6].Value / orientacionComercialTCAnioDestino[6].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[6].Value)) ? 0 : (orientacionComercialCPIAnioDestino[6].Value / orientacionComercialCPIAnioOrigen[6].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato22) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato22) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas8
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[7].Value) || decimalIsZero(orientacionComercialTCAnioDestino[7].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[7].Value / orientacionComercialIPCAnioOrigen[7].Value) * (orientacionComercialTCAnioOrigen[7].Value / orientacionComercialTCAnioDestino[7].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[7].Value)) ? 0 : (orientacionComercialCPIAnioDestino[7].Value / orientacionComercialCPIAnioOrigen[7].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato23) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato23) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas9
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[8].Value) || decimalIsZero(orientacionComercialTCAnioDestino[8].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[8].Value / orientacionComercialIPCAnioOrigen[8].Value) * (orientacionComercialTCAnioOrigen[8].Value / orientacionComercialTCAnioDestino[8].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[8].Value)) ? 0 : (orientacionComercialCPIAnioDestino[8].Value / orientacionComercialCPIAnioOrigen[8].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato24) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato24) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas10
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[9].Value) || decimalIsZero(orientacionComercialTCAnioDestino[9].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[9].Value / orientacionComercialIPCAnioOrigen[9].Value) * (orientacionComercialTCAnioOrigen[9].Value / orientacionComercialTCAnioDestino[9].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[9].Value)) ? 0 : (orientacionComercialCPIAnioDestino[9].Value / orientacionComercialCPIAnioOrigen[9].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato25) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato25) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas11
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[10].Value) || decimalIsZero(orientacionComercialTCAnioDestino[10].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[10].Value / orientacionComercialIPCAnioOrigen[10].Value) * (orientacionComercialTCAnioOrigen[10].Value / orientacionComercialTCAnioDestino[10].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[10].Value)) ? 0 : (orientacionComercialCPIAnioDestino[10].Value / orientacionComercialCPIAnioOrigen[10].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato26) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato26) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas12
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[11].Value) || decimalIsZero(orientacionComercialTCAnioDestino[11].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[11].Value / orientacionComercialIPCAnioOrigen[11].Value) * (orientacionComercialTCAnioOrigen[11].Value / orientacionComercialTCAnioDestino[11].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[11].Value)) ? 0 : (orientacionComercialCPIAnioDestino[11].Value / orientacionComercialCPIAnioOrigen[11].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato27) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato27) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas13
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[12].Value) || decimalIsZero(orientacionComercialTCAnioDestino[12].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[12].Value / orientacionComercialIPCAnioOrigen[12].Value) * (orientacionComercialTCAnioOrigen[12].Value / orientacionComercialTCAnioDestino[12].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[12].Value)) ? 0 : (orientacionComercialCPIAnioDestino[12].Value / orientacionComercialCPIAnioOrigen[12].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato28) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato28) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas14
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[13].Value) || decimalIsZero(orientacionComercialTCAnioDestino[13].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[13].Value / orientacionComercialIPCAnioOrigen[13].Value) * (orientacionComercialTCAnioOrigen[13].Value / orientacionComercialTCAnioDestino[13].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[13].Value)) ? 0 : (orientacionComercialCPIAnioDestino[13].Value / orientacionComercialCPIAnioOrigen[13].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato29) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato29) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas15
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[14].Value) || decimalIsZero(orientacionComercialTCAnioDestino[14].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[14].Value / orientacionComercialIPCAnioOrigen[14].Value) * (orientacionComercialTCAnioOrigen[14].Value / orientacionComercialTCAnioDestino[14].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[14].Value)) ? 0 : (orientacionComercialCPIAnioDestino[14].Value / orientacionComercialCPIAnioOrigen[14].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato30) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato30) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas16
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[15].Value) || decimalIsZero(orientacionComercialTCAnioDestino[15].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[15].Value / orientacionComercialIPCAnioOrigen[15].Value) * (orientacionComercialTCAnioOrigen[15].Value / orientacionComercialTCAnioDestino[15].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[15].Value)) ? 0 : (orientacionComercialCPIAnioDestino[15].Value / orientacionComercialCPIAnioOrigen[15].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato31) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato31) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas17
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[16].Value) || decimalIsZero(orientacionComercialTCAnioDestino[16].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[16].Value / orientacionComercialIPCAnioOrigen[16].Value) * (orientacionComercialTCAnioOrigen[16].Value / orientacionComercialTCAnioDestino[16].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[16].Value)) ? 0 : (orientacionComercialCPIAnioDestino[16].Value / orientacionComercialCPIAnioOrigen[16].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato32) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato32) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas18
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[17].Value) || decimalIsZero(orientacionComercialTCAnioDestino[17].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[17].Value / orientacionComercialIPCAnioOrigen[17].Value) * (orientacionComercialTCAnioOrigen[17].Value / orientacionComercialTCAnioDestino[17].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[17].Value)) ? 0 : (orientacionComercialCPIAnioDestino[17].Value / orientacionComercialCPIAnioOrigen[17].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato33) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato33) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas19
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[18].Value) || decimalIsZero(orientacionComercialTCAnioDestino[18].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[18].Value / orientacionComercialIPCAnioOrigen[18].Value) * (orientacionComercialTCAnioOrigen[18].Value / orientacionComercialTCAnioDestino[18].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[18].Value)) ? 0 : (orientacionComercialCPIAnioDestino[18].Value / orientacionComercialCPIAnioOrigen[18].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato34) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato34) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas20
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[19].Value) || decimalIsZero(orientacionComercialTCAnioDestino[19].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[19].Value / orientacionComercialIPCAnioOrigen[19].Value) * (orientacionComercialTCAnioOrigen[19].Value / orientacionComercialTCAnioDestino[19].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[19].Value)) ? 0 : (orientacionComercialCPIAnioDestino[19].Value / orientacionComercialCPIAnioOrigen[19].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato35) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato35) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas21
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[20].Value) || decimalIsZero(orientacionComercialTCAnioDestino[20].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[20].Value / orientacionComercialIPCAnioOrigen[20].Value) * (orientacionComercialTCAnioOrigen[20].Value / orientacionComercialTCAnioDestino[20].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[20].Value)) ? 0 : (orientacionComercialCPIAnioDestino[20].Value / orientacionComercialCPIAnioOrigen[20].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato36) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato36) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas22
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[21].Value) || decimalIsZero(orientacionComercialTCAnioDestino[21].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[21].Value / orientacionComercialIPCAnioOrigen[21].Value) * (orientacionComercialTCAnioOrigen[21].Value / orientacionComercialTCAnioDestino[21].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[21].Value)) ? 0 : (orientacionComercialCPIAnioDestino[21].Value / orientacionComercialCPIAnioOrigen[21].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato37) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato37) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas23
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[22].Value) || decimalIsZero(orientacionComercialTCAnioDestino[22].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[22].Value / orientacionComercialIPCAnioOrigen[22].Value) * (orientacionComercialTCAnioOrigen[22].Value / orientacionComercialTCAnioDestino[22].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[22].Value)) ? 0 : (orientacionComercialCPIAnioDestino[22].Value / orientacionComercialCPIAnioOrigen[22].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato38) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato38) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas24
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[23].Value) || decimalIsZero(orientacionComercialTCAnioDestino[23].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[23].Value / orientacionComercialIPCAnioOrigen[23].Value) * (orientacionComercialTCAnioOrigen[23].Value / orientacionComercialTCAnioDestino[23].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[23].Value)) ? 0 : (orientacionComercialCPIAnioDestino[23].Value / orientacionComercialCPIAnioOrigen[23].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato39) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato39) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas25
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[24].Value) || decimalIsZero(orientacionComercialTCAnioDestino[24].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[24].Value / orientacionComercialIPCAnioOrigen[24].Value) * (orientacionComercialTCAnioOrigen[24].Value / orientacionComercialTCAnioDestino[24].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[24].Value)) ? 0 : (orientacionComercialCPIAnioDestino[24].Value / orientacionComercialCPIAnioOrigen[24].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato40) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato40) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas26
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[25].Value) || decimalIsZero(orientacionComercialTCAnioDestino[25].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[25].Value / orientacionComercialIPCAnioOrigen[25].Value) * (orientacionComercialTCAnioOrigen[25].Value / orientacionComercialTCAnioDestino[25].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[25].Value)) ? 0 : (orientacionComercialCPIAnioDestino[25].Value / orientacionComercialCPIAnioOrigen[25].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato41) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato41) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas27
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[26].Value) || decimalIsZero(orientacionComercialTCAnioDestino[26].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[26].Value / orientacionComercialIPCAnioOrigen[26].Value) * (orientacionComercialTCAnioOrigen[26].Value / orientacionComercialTCAnioDestino[26].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[26].Value)) ? 0 : (orientacionComercialCPIAnioDestino[26].Value / orientacionComercialCPIAnioOrigen[26].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato42) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato42) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas28
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[27].Value) || decimalIsZero(orientacionComercialTCAnioDestino[27].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[27].Value / orientacionComercialIPCAnioOrigen[27].Value) * (orientacionComercialTCAnioOrigen[27].Value / orientacionComercialTCAnioDestino[27].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[27].Value)) ? 0 : (orientacionComercialCPIAnioDestino[27].Value / orientacionComercialCPIAnioOrigen[27].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato43) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato43) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas29
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[28].Value) || decimalIsZero(orientacionComercialTCAnioDestino[28].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[28].Value / orientacionComercialIPCAnioOrigen[28].Value) * (orientacionComercialTCAnioOrigen[28].Value / orientacionComercialTCAnioDestino[28].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[28].Value)) ? 0 : (orientacionComercialCPIAnioDestino[28].Value / orientacionComercialCPIAnioOrigen[28].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato44) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato44) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas30
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[29].Value) || decimalIsZero(orientacionComercialTCAnioDestino[29].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[29].Value / orientacionComercialIPCAnioOrigen[29].Value) * (orientacionComercialTCAnioOrigen[29].Value / orientacionComercialTCAnioDestino[29].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[29].Value)) ? 0 : (orientacionComercialCPIAnioDestino[29].Value / orientacionComercialCPIAnioOrigen[29].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato45) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato45) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas31
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[30].Value) || decimalIsZero(orientacionComercialTCAnioDestino[30].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[30].Value / orientacionComercialIPCAnioOrigen[30].Value) * (orientacionComercialTCAnioOrigen[30].Value / orientacionComercialTCAnioDestino[30].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[30].Value)) ? 0 : (orientacionComercialCPIAnioDestino[30].Value / orientacionComercialCPIAnioOrigen[30].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato46) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato46) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas32
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[31].Value) || decimalIsZero(orientacionComercialTCAnioDestino[31].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[31].Value / orientacionComercialIPCAnioOrigen[31].Value) * (orientacionComercialTCAnioOrigen[31].Value / orientacionComercialTCAnioDestino[31].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[31].Value)) ? 0 : (orientacionComercialCPIAnioDestino[31].Value / orientacionComercialCPIAnioOrigen[31].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato47) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato47) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas33
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[32].Value) || decimalIsZero(orientacionComercialTCAnioDestino[32].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[32].Value / orientacionComercialIPCAnioOrigen[32].Value) * (orientacionComercialTCAnioOrigen[32].Value / orientacionComercialTCAnioDestino[32].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[32].Value)) ? 0 : (orientacionComercialCPIAnioDestino[32].Value / orientacionComercialCPIAnioOrigen[32].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato48) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato48) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas34
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[33].Value) || decimalIsZero(orientacionComercialTCAnioDestino[33].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[33].Value / orientacionComercialIPCAnioOrigen[33].Value) * (orientacionComercialTCAnioOrigen[33].Value / orientacionComercialTCAnioDestino[33].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[33].Value)) ? 0 : (orientacionComercialCPIAnioDestino[33].Value / orientacionComercialCPIAnioOrigen[33].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato49) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato49) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas35
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[34].Value) || decimalIsZero(orientacionComercialTCAnioDestino[34].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[34].Value / orientacionComercialIPCAnioOrigen[34].Value) * (orientacionComercialTCAnioOrigen[34].Value / orientacionComercialTCAnioDestino[34].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[34].Value)) ? 0 : (orientacionComercialCPIAnioDestino[34].Value / orientacionComercialCPIAnioOrigen[34].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato50) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato50) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas36
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[35].Value) || decimalIsZero(orientacionComercialTCAnioDestino[35].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[35].Value / orientacionComercialIPCAnioOrigen[35].Value) * (orientacionComercialTCAnioOrigen[35].Value / orientacionComercialTCAnioDestino[35].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[35].Value)) ? 0 : (orientacionComercialCPIAnioDestino[35].Value / orientacionComercialCPIAnioOrigen[35].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato51) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato51) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas37
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[36].Value) || decimalIsZero(orientacionComercialTCAnioDestino[36].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[36].Value / orientacionComercialIPCAnioOrigen[36].Value) * (orientacionComercialTCAnioOrigen[36].Value / orientacionComercialTCAnioDestino[36].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[36].Value)) ? 0 : (orientacionComercialCPIAnioDestino[36].Value / orientacionComercialCPIAnioOrigen[36].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato52) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato52) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas38
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[37].Value) || decimalIsZero(orientacionComercialTCAnioDestino[37].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[37].Value / orientacionComercialIPCAnioOrigen[37].Value) * (orientacionComercialTCAnioOrigen[37].Value / orientacionComercialTCAnioDestino[37].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[37].Value)) ? 0 : (orientacionComercialCPIAnioDestino[37].Value / orientacionComercialCPIAnioOrigen[37].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato53) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato53) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas39
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[38].Value) || decimalIsZero(orientacionComercialTCAnioDestino[38].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[38].Value / orientacionComercialIPCAnioOrigen[38].Value) * (orientacionComercialTCAnioOrigen[38].Value / orientacionComercialTCAnioDestino[38].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[38].Value)) ? 0 : (orientacionComercialCPIAnioDestino[38].Value / orientacionComercialCPIAnioOrigen[38].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato54) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato54) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas40
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[39].Value) || decimalIsZero(orientacionComercialTCAnioDestino[39].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[39].Value / orientacionComercialIPCAnioOrigen[39].Value) * (orientacionComercialTCAnioOrigen[39].Value / orientacionComercialTCAnioDestino[39].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[39].Value)) ? 0 : (orientacionComercialCPIAnioDestino[39].Value / orientacionComercialCPIAnioOrigen[39].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato55) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato55) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas41
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[40].Value) || decimalIsZero(orientacionComercialTCAnioDestino[40].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[40].Value / orientacionComercialIPCAnioOrigen[40].Value) * (orientacionComercialTCAnioOrigen[40].Value / orientacionComercialTCAnioDestino[40].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[40].Value)) ? 0 : (orientacionComercialCPIAnioDestino[40].Value / orientacionComercialCPIAnioOrigen[40].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato56) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato56) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas42
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[41].Value) || decimalIsZero(orientacionComercialTCAnioDestino[41].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[41].Value / orientacionComercialIPCAnioOrigen[41].Value) * (orientacionComercialTCAnioOrigen[41].Value / orientacionComercialTCAnioDestino[41].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[41].Value)) ? 0 : (orientacionComercialCPIAnioDestino[41].Value / orientacionComercialCPIAnioOrigen[41].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato57) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato57) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas43
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[42].Value) || decimalIsZero(orientacionComercialTCAnioDestino[42].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[42].Value / orientacionComercialIPCAnioOrigen[42].Value) * (orientacionComercialTCAnioOrigen[42].Value / orientacionComercialTCAnioDestino[42].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[42].Value)) ? 0 : (orientacionComercialCPIAnioDestino[42].Value / orientacionComercialCPIAnioOrigen[42].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato58) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato58) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas44
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[43].Value) || decimalIsZero(orientacionComercialTCAnioDestino[43].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[43].Value / orientacionComercialIPCAnioOrigen[43].Value) * (orientacionComercialTCAnioOrigen[43].Value / orientacionComercialTCAnioDestino[43].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[43].Value)) ? 0 : (orientacionComercialCPIAnioDestino[43].Value / orientacionComercialCPIAnioOrigen[43].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato59) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato59) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas45
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[44].Value) || decimalIsZero(orientacionComercialTCAnioDestino[44].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[44].Value / orientacionComercialIPCAnioOrigen[44].Value) * (orientacionComercialTCAnioOrigen[44].Value / orientacionComercialTCAnioDestino[44].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[44].Value)) ? 0 : (orientacionComercialCPIAnioDestino[44].Value / orientacionComercialCPIAnioOrigen[44].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato60) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato60) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas46
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[45].Value) || decimalIsZero(orientacionComercialTCAnioDestino[45].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[45].Value / orientacionComercialIPCAnioOrigen[45].Value) * (orientacionComercialTCAnioOrigen[45].Value / orientacionComercialTCAnioDestino[45].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[45].Value)) ? 0 : (orientacionComercialCPIAnioDestino[45].Value / orientacionComercialCPIAnioOrigen[45].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato61) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato61) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas47
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[46].Value) || decimalIsZero(orientacionComercialTCAnioDestino[46].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[46].Value / orientacionComercialIPCAnioOrigen[46].Value) * (orientacionComercialTCAnioOrigen[46].Value / orientacionComercialTCAnioDestino[46].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[46].Value)) ? 0 : (orientacionComercialCPIAnioDestino[46].Value / orientacionComercialCPIAnioOrigen[46].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato62) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato62) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas48
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[47].Value) || decimalIsZero(orientacionComercialTCAnioDestino[47].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[47].Value / orientacionComercialIPCAnioOrigen[47].Value) * (orientacionComercialTCAnioOrigen[47].Value / orientacionComercialTCAnioDestino[47].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[47].Value)) ? 0 : (orientacionComercialCPIAnioDestino[47].Value / orientacionComercialCPIAnioOrigen[47].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato63) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato63) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas49
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[48].Value) || decimalIsZero(orientacionComercialTCAnioDestino[48].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[48].Value / orientacionComercialIPCAnioOrigen[48].Value) * (orientacionComercialTCAnioOrigen[48].Value / orientacionComercialTCAnioDestino[48].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[48].Value)) ? 0 : (orientacionComercialCPIAnioDestino[48].Value / orientacionComercialCPIAnioOrigen[48].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato64) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato64) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas50
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[49].Value) || decimalIsZero(orientacionComercialTCAnioDestino[49].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[49].Value / orientacionComercialIPCAnioOrigen[49].Value) * (orientacionComercialTCAnioOrigen[49].Value / orientacionComercialTCAnioDestino[49].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[49].Value)) ? 0 : (orientacionComercialCPIAnioDestino[49].Value / orientacionComercialCPIAnioOrigen[49].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato65) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato65) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas51
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[50].Value) || decimalIsZero(orientacionComercialTCAnioDestino[50].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[50].Value / orientacionComercialIPCAnioOrigen[50].Value) * (orientacionComercialTCAnioOrigen[50].Value / orientacionComercialTCAnioDestino[50].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[50].Value)) ? 0 : (orientacionComercialCPIAnioDestino[50].Value / orientacionComercialCPIAnioOrigen[50].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato66) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato66) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas52
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[51].Value) || decimalIsZero(orientacionComercialTCAnioDestino[51].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[51].Value / orientacionComercialIPCAnioOrigen[51].Value) * (orientacionComercialTCAnioOrigen[51].Value / orientacionComercialTCAnioDestino[51].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[51].Value)) ? 0 : (orientacionComercialCPIAnioDestino[51].Value / orientacionComercialCPIAnioOrigen[51].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato67) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato67) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas53
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[52].Value) || decimalIsZero(orientacionComercialTCAnioDestino[52].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[52].Value / orientacionComercialIPCAnioOrigen[52].Value) * (orientacionComercialTCAnioOrigen[52].Value / orientacionComercialTCAnioDestino[52].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[52].Value)) ? 0 : (orientacionComercialCPIAnioDestino[52].Value / orientacionComercialCPIAnioOrigen[52].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato68) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato68) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas54
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[53].Value) || decimalIsZero(orientacionComercialTCAnioDestino[53].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[53].Value / orientacionComercialIPCAnioOrigen[53].Value) * (orientacionComercialTCAnioOrigen[53].Value / orientacionComercialTCAnioDestino[53].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[53].Value)) ? 0 : (orientacionComercialCPIAnioDestino[53].Value / orientacionComercialCPIAnioOrigen[53].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato69) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato69) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas55
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[54].Value) || decimalIsZero(orientacionComercialTCAnioDestino[54].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[54].Value / orientacionComercialIPCAnioOrigen[54].Value) * (orientacionComercialTCAnioOrigen[54].Value / orientacionComercialTCAnioDestino[54].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[54].Value)) ? 0 : (orientacionComercialCPIAnioDestino[54].Value / orientacionComercialCPIAnioOrigen[54].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato70) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato70) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas56
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[55].Value) || decimalIsZero(orientacionComercialTCAnioDestino[55].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[55].Value / orientacionComercialIPCAnioOrigen[55].Value) * (orientacionComercialTCAnioOrigen[55].Value / orientacionComercialTCAnioDestino[55].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[55].Value)) ? 0 : (orientacionComercialCPIAnioDestino[55].Value / orientacionComercialCPIAnioOrigen[55].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato71) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato71) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas57
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[56].Value) || decimalIsZero(orientacionComercialTCAnioDestino[56].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[56].Value / orientacionComercialIPCAnioOrigen[56].Value) * (orientacionComercialTCAnioOrigen[56].Value / orientacionComercialTCAnioDestino[56].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[56].Value)) ? 0 : (orientacionComercialCPIAnioDestino[56].Value / orientacionComercialCPIAnioOrigen[56].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato72) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato72) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas58
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[57].Value) || decimalIsZero(orientacionComercialTCAnioDestino[57].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[57].Value / orientacionComercialIPCAnioOrigen[57].Value) * (orientacionComercialTCAnioOrigen[57].Value / orientacionComercialTCAnioDestino[57].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[57].Value)) ? 0 : (orientacionComercialCPIAnioDestino[57].Value / orientacionComercialCPIAnioOrigen[57].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato73) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato73) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas59
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[58].Value) || decimalIsZero(orientacionComercialTCAnioDestino[58].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[58].Value / orientacionComercialIPCAnioOrigen[58].Value) * (orientacionComercialTCAnioOrigen[58].Value / orientacionComercialTCAnioDestino[58].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[58].Value)) ? 0 : (orientacionComercialCPIAnioDestino[58].Value / orientacionComercialCPIAnioOrigen[58].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato74) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato74) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas60
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[59].Value) || decimalIsZero(orientacionComercialTCAnioDestino[59].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[59].Value / orientacionComercialIPCAnioOrigen[59].Value) * (orientacionComercialTCAnioOrigen[59].Value / orientacionComercialTCAnioDestino[59].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[59].Value)) ? 0 : (orientacionComercialCPIAnioDestino[59].Value / orientacionComercialCPIAnioOrigen[59].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato75) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato75) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas61
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[60].Value) || decimalIsZero(orientacionComercialTCAnioDestino[60].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[60].Value / orientacionComercialIPCAnioOrigen[60].Value) * (orientacionComercialTCAnioOrigen[60].Value / orientacionComercialTCAnioDestino[60].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[60].Value)) ? 0 : (orientacionComercialCPIAnioDestino[60].Value / orientacionComercialCPIAnioOrigen[60].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato76) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato76) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas62
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[61].Value) || decimalIsZero(orientacionComercialTCAnioDestino[61].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[61].Value / orientacionComercialIPCAnioOrigen[61].Value) * (orientacionComercialTCAnioOrigen[61].Value / orientacionComercialTCAnioDestino[61].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[61].Value)) ? 0 : (orientacionComercialCPIAnioDestino[61].Value / orientacionComercialCPIAnioOrigen[61].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato77) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato77) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas63
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[62].Value) || decimalIsZero(orientacionComercialTCAnioDestino[62].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[62].Value / orientacionComercialIPCAnioOrigen[62].Value) * (orientacionComercialTCAnioOrigen[62].Value / orientacionComercialTCAnioDestino[62].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[62].Value)) ? 0 : (orientacionComercialCPIAnioDestino[62].Value / orientacionComercialCPIAnioOrigen[62].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato78) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato78) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas64
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[63].Value) || decimalIsZero(orientacionComercialTCAnioDestino[63].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[63].Value / orientacionComercialIPCAnioOrigen[63].Value) * (orientacionComercialTCAnioOrigen[63].Value / orientacionComercialTCAnioDestino[63].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[63].Value)) ? 0 : (orientacionComercialCPIAnioDestino[63].Value / orientacionComercialCPIAnioOrigen[63].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato79) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato79) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas65
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[64].Value) || decimalIsZero(orientacionComercialTCAnioDestino[64].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[64].Value / orientacionComercialIPCAnioOrigen[64].Value) * (orientacionComercialTCAnioOrigen[64].Value / orientacionComercialTCAnioDestino[64].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[64].Value)) ? 0 : (orientacionComercialCPIAnioDestino[64].Value / orientacionComercialCPIAnioOrigen[64].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato80) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato80) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas66
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[65].Value) || decimalIsZero(orientacionComercialTCAnioDestino[65].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[65].Value / orientacionComercialIPCAnioOrigen[65].Value) * (orientacionComercialTCAnioOrigen[65].Value / orientacionComercialTCAnioDestino[65].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[65].Value)) ? 0 : (orientacionComercialCPIAnioDestino[65].Value / orientacionComercialCPIAnioOrigen[65].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato81) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato81) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas67
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[66].Value) || decimalIsZero(orientacionComercialTCAnioDestino[66].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[66].Value / orientacionComercialIPCAnioOrigen[66].Value) * (orientacionComercialTCAnioOrigen[66].Value / orientacionComercialTCAnioDestino[66].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[66].Value)) ? 0 : (orientacionComercialCPIAnioDestino[66].Value / orientacionComercialCPIAnioOrigen[66].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato82) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato82) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas68
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[67].Value) || decimalIsZero(orientacionComercialTCAnioDestino[67].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[67].Value / orientacionComercialIPCAnioOrigen[67].Value) * (orientacionComercialTCAnioOrigen[67].Value / orientacionComercialTCAnioDestino[67].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[67].Value)) ? 0 : (orientacionComercialCPIAnioDestino[67].Value / orientacionComercialCPIAnioOrigen[67].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato83) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato83) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas69
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[68].Value) || decimalIsZero(orientacionComercialTCAnioDestino[68].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[68].Value / orientacionComercialIPCAnioOrigen[68].Value) * (orientacionComercialTCAnioOrigen[68].Value / orientacionComercialTCAnioDestino[68].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[68].Value)) ? 0 : (orientacionComercialCPIAnioDestino[68].Value / orientacionComercialCPIAnioOrigen[68].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato84) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato84) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas70
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[69].Value) || decimalIsZero(orientacionComercialTCAnioDestino[69].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[69].Value / orientacionComercialIPCAnioOrigen[69].Value) * (orientacionComercialTCAnioOrigen[69].Value / orientacionComercialTCAnioDestino[69].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[69].Value)) ? 0 : (orientacionComercialCPIAnioDestino[69].Value / orientacionComercialCPIAnioOrigen[69].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato85) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato85) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas71
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[70].Value) || decimalIsZero(orientacionComercialTCAnioDestino[70].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[70].Value / orientacionComercialIPCAnioOrigen[70].Value) * (orientacionComercialTCAnioOrigen[70].Value / orientacionComercialTCAnioDestino[70].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[70].Value)) ? 0 : (orientacionComercialCPIAnioDestino[70].Value / orientacionComercialCPIAnioOrigen[70].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato86) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato86) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas72
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[71].Value) || decimalIsZero(orientacionComercialTCAnioDestino[71].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[71].Value / orientacionComercialIPCAnioOrigen[71].Value) * (orientacionComercialTCAnioOrigen[71].Value / orientacionComercialTCAnioDestino[71].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[71].Value)) ? 0 : (orientacionComercialCPIAnioDestino[71].Value / orientacionComercialCPIAnioOrigen[71].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato87) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato87) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas73
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[72].Value) || decimalIsZero(orientacionComercialTCAnioDestino[72].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[72].Value / orientacionComercialIPCAnioOrigen[72].Value) * (orientacionComercialTCAnioOrigen[72].Value / orientacionComercialTCAnioDestino[72].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[72].Value)) ? 0 : (orientacionComercialCPIAnioDestino[72].Value / orientacionComercialCPIAnioOrigen[72].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato88) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato88) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas74
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[73].Value) || decimalIsZero(orientacionComercialTCAnioDestino[73].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[73].Value / orientacionComercialIPCAnioOrigen[73].Value) * (orientacionComercialTCAnioOrigen[73].Value / orientacionComercialTCAnioDestino[73].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[73].Value)) ? 0 : (orientacionComercialCPIAnioDestino[73].Value / orientacionComercialCPIAnioOrigen[73].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato89) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato89) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas75
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[74].Value) || decimalIsZero(orientacionComercialTCAnioDestino[74].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[74].Value / orientacionComercialIPCAnioOrigen[74].Value) * (orientacionComercialTCAnioOrigen[74].Value / orientacionComercialTCAnioDestino[74].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[74].Value)) ? 0 : (orientacionComercialCPIAnioDestino[74].Value / orientacionComercialCPIAnioOrigen[74].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato90) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato90) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas76
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[75].Value) || decimalIsZero(orientacionComercialTCAnioDestino[75].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[75].Value / orientacionComercialIPCAnioOrigen[75].Value) * (orientacionComercialTCAnioOrigen[75].Value / orientacionComercialTCAnioDestino[75].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[75].Value)) ? 0 : (orientacionComercialCPIAnioDestino[75].Value / orientacionComercialCPIAnioOrigen[75].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato91) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato91) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas77
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[76].Value) || decimalIsZero(orientacionComercialTCAnioDestino[76].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[76].Value / orientacionComercialIPCAnioOrigen[76].Value) * (orientacionComercialTCAnioOrigen[76].Value / orientacionComercialTCAnioDestino[76].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[76].Value)) ? 0 : (orientacionComercialCPIAnioDestino[76].Value / orientacionComercialCPIAnioOrigen[76].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato92) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato92) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas78
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[77].Value) || decimalIsZero(orientacionComercialTCAnioDestino[77].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[77].Value / orientacionComercialIPCAnioOrigen[77].Value) * (orientacionComercialTCAnioOrigen[77].Value / orientacionComercialTCAnioDestino[77].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[77].Value)) ? 0 : (orientacionComercialCPIAnioDestino[77].Value / orientacionComercialCPIAnioOrigen[77].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato93) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato93) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas79
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[78].Value) || decimalIsZero(orientacionComercialTCAnioDestino[78].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[78].Value / orientacionComercialIPCAnioOrigen[78].Value) * (orientacionComercialTCAnioOrigen[78].Value / orientacionComercialTCAnioDestino[78].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[78].Value)) ? 0 : (orientacionComercialCPIAnioDestino[78].Value / orientacionComercialCPIAnioOrigen[78].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato94) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato94) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas80
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[79].Value) || decimalIsZero(orientacionComercialTCAnioDestino[79].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[79].Value / orientacionComercialIPCAnioOrigen[79].Value) * (orientacionComercialTCAnioOrigen[79].Value / orientacionComercialTCAnioDestino[79].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[79].Value)) ? 0 : (orientacionComercialCPIAnioDestino[79].Value / orientacionComercialCPIAnioOrigen[79].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato95) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato95) * facCorrMonExt, ValorTotal = 0 });
                                                                //Total por año
                                                                decimal totalValorNacAnio = 0;
                                                                decimal totalValorExtAnio = 0;
                                                                for (int i = 0; i < adqAnio.Count; i++)
                                                                {
                                                                    totalValorNacAnio += adqAnio[i].ValorNac;
                                                                    totalValorExtAnio += adqAnio[i].ValorExt;
                                                                    adqAnio[i].ValorTotal = (adqAnio[i].ValorNac + adqAnio[i].ValorExt);
                                                                }
                                                                //Total Capex
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = (perAntNac + totalValorNacMes + totalValorNacAnio), ValorExt = (perAntExt + totalValorExtMes + totalValorExtAnio), ValorTotal = ((perAntNac + totalValorNacMes + totalValorNacAnio) + (perAntExt + totalValorExtMes + totalValorExtAnio)) });
                                                            }
                                                            else if (IfDato0.StartsWith(prefixCons))
                                                            {
                                                                consMes.Add(new Presupuesto.CellValue { Titulo = IfDato0, TituloNac = filaMonedaNacional.IfDato0.ToString(), TituloExt = filaMonedaExtranjera.IfDato0.ToString() });
                                                                decimal perAntNac = stringToDecimal(filaMonedaNacional.IfDato2);
                                                                decimal perAntExt = stringToDecimal(filaMonedaExtranjera.IfDato2);
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = perAntNac, ValorExt = perAntExt });
                                                                //Enero
                                                                decimal facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[0].Value) || decimalIsZero(orientacionComercialTCMesDestino[0].Value)) ? 0 : ((orientacionComercialIPCMesDestino[0].Value / orientacionComercialIPCMesOrigen[0].Value) * (orientacionComercialTCMesOrigen[0].Value / orientacionComercialTCMesDestino[0].Value)));
                                                                decimal facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[0].Value)) ? 0 : (orientacionComercialCPIMesDestino[0].Value / orientacionComercialCPIMesOrigen[0].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato3) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato3) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[1].Value) || decimalIsZero(orientacionComercialTCMesDestino[1].Value)) ? 0 : ((orientacionComercialIPCMesDestino[1].Value / orientacionComercialIPCMesOrigen[1].Value) * (orientacionComercialTCMesOrigen[1].Value / orientacionComercialTCMesDestino[1].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[1].Value)) ? 0 : (orientacionComercialCPIMesDestino[1].Value / orientacionComercialCPIMesOrigen[1].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato4) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato4) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[2].Value) || decimalIsZero(orientacionComercialTCMesDestino[2].Value)) ? 0 : ((orientacionComercialIPCMesDestino[2].Value / orientacionComercialIPCMesOrigen[2].Value) * (orientacionComercialTCMesOrigen[2].Value / orientacionComercialTCMesDestino[2].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[2].Value)) ? 0 : (orientacionComercialCPIMesDestino[2].Value / orientacionComercialCPIMesOrigen[2].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato5) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato5) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[3].Value) || decimalIsZero(orientacionComercialTCMesDestino[3].Value)) ? 0 : ((orientacionComercialIPCMesDestino[3].Value / orientacionComercialIPCMesOrigen[3].Value) * (orientacionComercialTCMesOrigen[3].Value / orientacionComercialTCMesDestino[3].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[3].Value)) ? 0 : (orientacionComercialCPIMesDestino[3].Value / orientacionComercialCPIMesOrigen[3].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato6) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato6) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[4].Value) || decimalIsZero(orientacionComercialTCMesDestino[4].Value)) ? 0 : ((orientacionComercialIPCMesDestino[4].Value / orientacionComercialIPCMesOrigen[4].Value) * (orientacionComercialTCMesOrigen[4].Value / orientacionComercialTCMesDestino[4].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[4].Value)) ? 0 : (orientacionComercialCPIMesDestino[4].Value / orientacionComercialCPIMesOrigen[4].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato7) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato7) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[5].Value) || decimalIsZero(orientacionComercialTCMesDestino[5].Value)) ? 0 : ((orientacionComercialIPCMesDestino[5].Value / orientacionComercialIPCMesOrigen[5].Value) * (orientacionComercialTCMesOrigen[5].Value / orientacionComercialTCMesDestino[5].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[5].Value)) ? 0 : (orientacionComercialCPIMesDestino[5].Value / orientacionComercialCPIMesOrigen[5].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato8) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato8) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[6].Value) || decimalIsZero(orientacionComercialTCMesDestino[6].Value)) ? 0 : ((orientacionComercialIPCMesDestino[6].Value / orientacionComercialIPCMesOrigen[6].Value) * (orientacionComercialTCMesOrigen[6].Value / orientacionComercialTCMesDestino[6].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[6].Value)) ? 0 : (orientacionComercialCPIMesDestino[6].Value / orientacionComercialCPIMesOrigen[6].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato9) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato9) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[7].Value) || decimalIsZero(orientacionComercialTCMesDestino[7].Value)) ? 0 : ((orientacionComercialIPCMesDestino[7].Value / orientacionComercialIPCMesOrigen[7].Value) * (orientacionComercialTCMesOrigen[7].Value / orientacionComercialTCMesDestino[7].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[7].Value)) ? 0 : (orientacionComercialCPIMesDestino[7].Value / orientacionComercialCPIMesOrigen[7].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato10) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato10) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[8].Value) || decimalIsZero(orientacionComercialTCMesDestino[8].Value)) ? 0 : ((orientacionComercialIPCMesDestino[8].Value / orientacionComercialIPCMesOrigen[8].Value) * (orientacionComercialTCMesOrigen[8].Value / orientacionComercialTCMesDestino[8].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[8].Value)) ? 0 : (orientacionComercialCPIMesDestino[8].Value / orientacionComercialCPIMesOrigen[8].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato11) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato11) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[9].Value) || decimalIsZero(orientacionComercialTCMesDestino[9].Value)) ? 0 : ((orientacionComercialIPCMesDestino[9].Value / orientacionComercialIPCMesOrigen[9].Value) * (orientacionComercialTCMesOrigen[9].Value / orientacionComercialTCMesDestino[9].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[9].Value)) ? 0 : (orientacionComercialCPIMesDestino[9].Value / orientacionComercialCPIMesOrigen[9].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato12) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato12) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[10].Value) || decimalIsZero(orientacionComercialTCMesDestino[10].Value)) ? 0 : ((orientacionComercialIPCMesDestino[10].Value / orientacionComercialIPCMesOrigen[10].Value) * (orientacionComercialTCMesOrigen[10].Value / orientacionComercialTCMesDestino[10].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[10].Value)) ? 0 : (orientacionComercialCPIMesDestino[10].Value / orientacionComercialCPIMesOrigen[10].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato13) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato13) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[11].Value) || decimalIsZero(orientacionComercialTCMesDestino[11].Value)) ? 0 : ((orientacionComercialIPCMesDestino[11].Value / orientacionComercialIPCMesOrigen[11].Value) * (orientacionComercialTCMesOrigen[11].Value / orientacionComercialTCMesDestino[11].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[11].Value)) ? 0 : (orientacionComercialCPIMesDestino[11].Value / orientacionComercialCPIMesOrigen[11].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato14) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato14) * facCorrMonExt, ValorTotal = 0 });
                                                                //diciembre
                                                                decimal totalValorNacMes = 0;
                                                                decimal totalValorExtMes = 0;
                                                                for (int i = 2; i < consMes.Count; i++)
                                                                {
                                                                    totalValorNacMes += consMes[i].ValorNac;
                                                                    totalValorExtMes += consMes[i].ValorExt;
                                                                    consMes[i].ValorTotal = (consMes[i].ValorNac + consMes[i].ValorExt);
                                                                }
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = totalValorNacMes, ValorExt = totalValorExtMes, ValorTotal = (totalValorNacMes + totalValorExtMes) });
                                                                //Total meses
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[0].Value) || decimalIsZero(orientacionComercialTCAnioDestino[0].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[0].Value / orientacionComercialIPCAnioOrigen[0].Value) * (orientacionComercialTCAnioOrigen[0].Value / orientacionComercialTCAnioDestino[0].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[0].Value)) ? 0 : (orientacionComercialCPIAnioDestino[0].Value / orientacionComercialCPIAnioOrigen[0].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato16) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato16) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[1].Value) || decimalIsZero(orientacionComercialTCAnioDestino[1].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[1].Value / orientacionComercialIPCAnioOrigen[1].Value) * (orientacionComercialTCAnioOrigen[1].Value / orientacionComercialTCAnioDestino[1].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[1].Value)) ? 0 : (orientacionComercialCPIAnioDestino[1].Value / orientacionComercialCPIAnioOrigen[1].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato17) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato17) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[2].Value) || decimalIsZero(orientacionComercialTCAnioDestino[2].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[2].Value / orientacionComercialIPCAnioOrigen[2].Value) * (orientacionComercialTCAnioOrigen[2].Value / orientacionComercialTCAnioDestino[2].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[2].Value)) ? 0 : (orientacionComercialCPIAnioDestino[2].Value / orientacionComercialCPIAnioOrigen[2].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato18) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato18) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[3].Value) || decimalIsZero(orientacionComercialTCAnioDestino[3].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[3].Value / orientacionComercialIPCAnioOrigen[3].Value) * (orientacionComercialTCAnioOrigen[3].Value / orientacionComercialTCAnioDestino[3].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[3].Value)) ? 0 : (orientacionComercialCPIAnioDestino[3].Value / orientacionComercialCPIAnioOrigen[3].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato19) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato19) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[4].Value) || decimalIsZero(orientacionComercialTCAnioDestino[4].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[4].Value / orientacionComercialIPCAnioOrigen[4].Value) * (orientacionComercialTCAnioOrigen[4].Value / orientacionComercialTCAnioDestino[4].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[4].Value)) ? 0 : (orientacionComercialCPIAnioDestino[4].Value / orientacionComercialCPIAnioOrigen[4].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato20) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato20) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[5].Value) || decimalIsZero(orientacionComercialTCAnioDestino[5].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[5].Value / orientacionComercialIPCAnioOrigen[5].Value) * (orientacionComercialTCAnioOrigen[5].Value / orientacionComercialTCAnioDestino[5].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[5].Value)) ? 0 : (orientacionComercialCPIAnioDestino[5].Value / orientacionComercialCPIAnioOrigen[5].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato21) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato21) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[6].Value) || decimalIsZero(orientacionComercialTCAnioDestino[6].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[6].Value / orientacionComercialIPCAnioOrigen[6].Value) * (orientacionComercialTCAnioOrigen[6].Value / orientacionComercialTCAnioDestino[6].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[6].Value)) ? 0 : (orientacionComercialCPIAnioDestino[6].Value / orientacionComercialCPIAnioOrigen[6].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato22) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato22) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[7].Value) || decimalIsZero(orientacionComercialTCAnioDestino[7].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[7].Value / orientacionComercialIPCAnioOrigen[7].Value) * (orientacionComercialTCAnioOrigen[7].Value / orientacionComercialTCAnioDestino[7].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[7].Value)) ? 0 : (orientacionComercialCPIAnioDestino[7].Value / orientacionComercialCPIAnioOrigen[7].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato23) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato23) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[8].Value) || decimalIsZero(orientacionComercialTCAnioDestino[8].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[8].Value / orientacionComercialIPCAnioOrigen[8].Value) * (orientacionComercialTCAnioOrigen[8].Value / orientacionComercialTCAnioDestino[8].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[8].Value)) ? 0 : (orientacionComercialCPIAnioDestino[8].Value / orientacionComercialCPIAnioOrigen[8].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato24) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato24) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[9].Value) || decimalIsZero(orientacionComercialTCAnioDestino[9].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[9].Value / orientacionComercialIPCAnioOrigen[9].Value) * (orientacionComercialTCAnioOrigen[9].Value / orientacionComercialTCAnioDestino[9].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[9].Value)) ? 0 : (orientacionComercialCPIAnioDestino[9].Value / orientacionComercialCPIAnioOrigen[9].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato25) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato25) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[10].Value) || decimalIsZero(orientacionComercialTCAnioDestino[10].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[10].Value / orientacionComercialIPCAnioOrigen[10].Value) * (orientacionComercialTCAnioOrigen[10].Value / orientacionComercialTCAnioDestino[10].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[10].Value)) ? 0 : (orientacionComercialCPIAnioDestino[10].Value / orientacionComercialCPIAnioOrigen[10].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato26) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato26) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[11].Value) || decimalIsZero(orientacionComercialTCAnioDestino[11].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[11].Value / orientacionComercialIPCAnioOrigen[11].Value) * (orientacionComercialTCAnioOrigen[11].Value / orientacionComercialTCAnioDestino[11].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[11].Value)) ? 0 : (orientacionComercialCPIAnioDestino[11].Value / orientacionComercialCPIAnioOrigen[11].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato27) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato27) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[12].Value) || decimalIsZero(orientacionComercialTCAnioDestino[12].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[12].Value / orientacionComercialIPCAnioOrigen[12].Value) * (orientacionComercialTCAnioOrigen[12].Value / orientacionComercialTCAnioDestino[12].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[12].Value)) ? 0 : (orientacionComercialCPIAnioDestino[12].Value / orientacionComercialCPIAnioOrigen[12].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato28) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato28) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[13].Value) || decimalIsZero(orientacionComercialTCAnioDestino[13].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[13].Value / orientacionComercialIPCAnioOrigen[13].Value) * (orientacionComercialTCAnioOrigen[13].Value / orientacionComercialTCAnioDestino[13].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[13].Value)) ? 0 : (orientacionComercialCPIAnioDestino[13].Value / orientacionComercialCPIAnioOrigen[13].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato29) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato29) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[14].Value) || decimalIsZero(orientacionComercialTCAnioDestino[14].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[14].Value / orientacionComercialIPCAnioOrigen[14].Value) * (orientacionComercialTCAnioOrigen[14].Value / orientacionComercialTCAnioDestino[14].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[14].Value)) ? 0 : (orientacionComercialCPIAnioDestino[14].Value / orientacionComercialCPIAnioOrigen[14].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato30) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato30) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[15].Value) || decimalIsZero(orientacionComercialTCAnioDestino[15].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[15].Value / orientacionComercialIPCAnioOrigen[15].Value) * (orientacionComercialTCAnioOrigen[15].Value / orientacionComercialTCAnioDestino[15].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[15].Value)) ? 0 : (orientacionComercialCPIAnioDestino[15].Value / orientacionComercialCPIAnioOrigen[15].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato31) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato31) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[16].Value) || decimalIsZero(orientacionComercialTCAnioDestino[16].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[16].Value / orientacionComercialIPCAnioOrigen[16].Value) * (orientacionComercialTCAnioOrigen[16].Value / orientacionComercialTCAnioDestino[16].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[16].Value)) ? 0 : (orientacionComercialCPIAnioDestino[16].Value / orientacionComercialCPIAnioOrigen[16].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato32) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato32) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[17].Value) || decimalIsZero(orientacionComercialTCAnioDestino[17].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[17].Value / orientacionComercialIPCAnioOrigen[17].Value) * (orientacionComercialTCAnioOrigen[17].Value / orientacionComercialTCAnioDestino[17].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[17].Value)) ? 0 : (orientacionComercialCPIAnioDestino[17].Value / orientacionComercialCPIAnioOrigen[17].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato33) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato33) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[18].Value) || decimalIsZero(orientacionComercialTCAnioDestino[18].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[18].Value / orientacionComercialIPCAnioOrigen[18].Value) * (orientacionComercialTCAnioOrigen[18].Value / orientacionComercialTCAnioDestino[18].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[18].Value)) ? 0 : (orientacionComercialCPIAnioDestino[18].Value / orientacionComercialCPIAnioOrigen[18].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato34) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato34) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[19].Value) || decimalIsZero(orientacionComercialTCAnioDestino[19].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[19].Value / orientacionComercialIPCAnioOrigen[19].Value) * (orientacionComercialTCAnioOrigen[19].Value / orientacionComercialTCAnioDestino[19].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[19].Value)) ? 0 : (orientacionComercialCPIAnioDestino[19].Value / orientacionComercialCPIAnioOrigen[19].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato35) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato35) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[20].Value) || decimalIsZero(orientacionComercialTCAnioDestino[20].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[20].Value / orientacionComercialIPCAnioOrigen[20].Value) * (orientacionComercialTCAnioOrigen[20].Value / orientacionComercialTCAnioDestino[20].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[20].Value)) ? 0 : (orientacionComercialCPIAnioDestino[20].Value / orientacionComercialCPIAnioOrigen[20].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato36) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato36) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[21].Value) || decimalIsZero(orientacionComercialTCAnioDestino[21].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[21].Value / orientacionComercialIPCAnioOrigen[21].Value) * (orientacionComercialTCAnioOrigen[21].Value / orientacionComercialTCAnioDestino[21].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[21].Value)) ? 0 : (orientacionComercialCPIAnioDestino[21].Value / orientacionComercialCPIAnioOrigen[21].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato37) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato37) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[22].Value) || decimalIsZero(orientacionComercialTCAnioDestino[22].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[22].Value / orientacionComercialIPCAnioOrigen[22].Value) * (orientacionComercialTCAnioOrigen[22].Value / orientacionComercialTCAnioDestino[22].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[22].Value)) ? 0 : (orientacionComercialCPIAnioDestino[22].Value / orientacionComercialCPIAnioOrigen[22].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato38) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato38) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[23].Value) || decimalIsZero(orientacionComercialTCAnioDestino[23].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[23].Value / orientacionComercialIPCAnioOrigen[23].Value) * (orientacionComercialTCAnioOrigen[23].Value / orientacionComercialTCAnioDestino[23].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[23].Value)) ? 0 : (orientacionComercialCPIAnioDestino[23].Value / orientacionComercialCPIAnioOrigen[23].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato39) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato39) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[24].Value) || decimalIsZero(orientacionComercialTCAnioDestino[24].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[24].Value / orientacionComercialIPCAnioOrigen[24].Value) * (orientacionComercialTCAnioOrigen[24].Value / orientacionComercialTCAnioDestino[24].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[24].Value)) ? 0 : (orientacionComercialCPIAnioDestino[24].Value / orientacionComercialCPIAnioOrigen[24].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato40) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato40) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[25].Value) || decimalIsZero(orientacionComercialTCAnioDestino[25].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[25].Value / orientacionComercialIPCAnioOrigen[25].Value) * (orientacionComercialTCAnioOrigen[25].Value / orientacionComercialTCAnioDestino[25].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[25].Value)) ? 0 : (orientacionComercialCPIAnioDestino[25].Value / orientacionComercialCPIAnioOrigen[25].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato41) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato41) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[26].Value) || decimalIsZero(orientacionComercialTCAnioDestino[26].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[26].Value / orientacionComercialIPCAnioOrigen[26].Value) * (orientacionComercialTCAnioOrigen[26].Value / orientacionComercialTCAnioDestino[26].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[26].Value)) ? 0 : (orientacionComercialCPIAnioDestino[26].Value / orientacionComercialCPIAnioOrigen[26].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato42) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato42) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[27].Value) || decimalIsZero(orientacionComercialTCAnioDestino[27].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[27].Value / orientacionComercialIPCAnioOrigen[27].Value) * (orientacionComercialTCAnioOrigen[27].Value / orientacionComercialTCAnioDestino[27].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[27].Value)) ? 0 : (orientacionComercialCPIAnioDestino[27].Value / orientacionComercialCPIAnioOrigen[27].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato43) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato43) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[28].Value) || decimalIsZero(orientacionComercialTCAnioDestino[28].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[28].Value / orientacionComercialIPCAnioOrigen[28].Value) * (orientacionComercialTCAnioOrigen[28].Value / orientacionComercialTCAnioDestino[28].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[28].Value)) ? 0 : (orientacionComercialCPIAnioDestino[28].Value / orientacionComercialCPIAnioOrigen[28].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato44) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato44) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[29].Value) || decimalIsZero(orientacionComercialTCAnioDestino[29].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[29].Value / orientacionComercialIPCAnioOrigen[29].Value) * (orientacionComercialTCAnioOrigen[29].Value / orientacionComercialTCAnioDestino[29].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[29].Value)) ? 0 : (orientacionComercialCPIAnioDestino[29].Value / orientacionComercialCPIAnioOrigen[29].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato45) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato45) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[30].Value) || decimalIsZero(orientacionComercialTCAnioDestino[30].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[30].Value / orientacionComercialIPCAnioOrigen[30].Value) * (orientacionComercialTCAnioOrigen[30].Value / orientacionComercialTCAnioDestino[30].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[30].Value)) ? 0 : (orientacionComercialCPIAnioDestino[30].Value / orientacionComercialCPIAnioOrigen[30].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato46) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato46) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[31].Value) || decimalIsZero(orientacionComercialTCAnioDestino[31].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[31].Value / orientacionComercialIPCAnioOrigen[31].Value) * (orientacionComercialTCAnioOrigen[31].Value / orientacionComercialTCAnioDestino[31].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[31].Value)) ? 0 : (orientacionComercialCPIAnioDestino[31].Value / orientacionComercialCPIAnioOrigen[31].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato47) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato47) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[32].Value) || decimalIsZero(orientacionComercialTCAnioDestino[32].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[32].Value / orientacionComercialIPCAnioOrigen[32].Value) * (orientacionComercialTCAnioOrigen[32].Value / orientacionComercialTCAnioDestino[32].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[32].Value)) ? 0 : (orientacionComercialCPIAnioDestino[32].Value / orientacionComercialCPIAnioOrigen[32].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato48) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato48) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[33].Value) || decimalIsZero(orientacionComercialTCAnioDestino[33].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[33].Value / orientacionComercialIPCAnioOrigen[33].Value) * (orientacionComercialTCAnioOrigen[33].Value / orientacionComercialTCAnioDestino[33].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[33].Value)) ? 0 : (orientacionComercialCPIAnioDestino[33].Value / orientacionComercialCPIAnioOrigen[33].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato49) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato49) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[34].Value) || decimalIsZero(orientacionComercialTCAnioDestino[34].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[34].Value / orientacionComercialIPCAnioOrigen[34].Value) * (orientacionComercialTCAnioOrigen[34].Value / orientacionComercialTCAnioDestino[34].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[34].Value)) ? 0 : (orientacionComercialCPIAnioDestino[34].Value / orientacionComercialCPIAnioOrigen[34].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato50) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato50) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[35].Value) || decimalIsZero(orientacionComercialTCAnioDestino[35].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[35].Value / orientacionComercialIPCAnioOrigen[35].Value) * (orientacionComercialTCAnioOrigen[35].Value / orientacionComercialTCAnioDestino[35].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[35].Value)) ? 0 : (orientacionComercialCPIAnioDestino[35].Value / orientacionComercialCPIAnioOrigen[35].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato51) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato51) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[36].Value) || decimalIsZero(orientacionComercialTCAnioDestino[36].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[36].Value / orientacionComercialIPCAnioOrigen[36].Value) * (orientacionComercialTCAnioOrigen[36].Value / orientacionComercialTCAnioDestino[36].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[36].Value)) ? 0 : (orientacionComercialCPIAnioDestino[36].Value / orientacionComercialCPIAnioOrigen[36].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato52) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato52) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[37].Value) || decimalIsZero(orientacionComercialTCAnioDestino[37].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[37].Value / orientacionComercialIPCAnioOrigen[37].Value) * (orientacionComercialTCAnioOrigen[37].Value / orientacionComercialTCAnioDestino[37].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[37].Value)) ? 0 : (orientacionComercialCPIAnioDestino[37].Value / orientacionComercialCPIAnioOrigen[37].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato53) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato53) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[38].Value) || decimalIsZero(orientacionComercialTCAnioDestino[38].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[38].Value / orientacionComercialIPCAnioOrigen[38].Value) * (orientacionComercialTCAnioOrigen[38].Value / orientacionComercialTCAnioDestino[38].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[38].Value)) ? 0 : (orientacionComercialCPIAnioDestino[38].Value / orientacionComercialCPIAnioOrigen[38].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato54) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato54) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[39].Value) || decimalIsZero(orientacionComercialTCAnioDestino[39].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[39].Value / orientacionComercialIPCAnioOrigen[39].Value) * (orientacionComercialTCAnioOrigen[39].Value / orientacionComercialTCAnioDestino[39].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[39].Value)) ? 0 : (orientacionComercialCPIAnioDestino[39].Value / orientacionComercialCPIAnioOrigen[39].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato55) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato55) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[40].Value) || decimalIsZero(orientacionComercialTCAnioDestino[40].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[40].Value / orientacionComercialIPCAnioOrigen[40].Value) * (orientacionComercialTCAnioOrigen[40].Value / orientacionComercialTCAnioDestino[40].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[40].Value)) ? 0 : (orientacionComercialCPIAnioDestino[40].Value / orientacionComercialCPIAnioOrigen[40].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato56) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato56) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[41].Value) || decimalIsZero(orientacionComercialTCAnioDestino[41].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[41].Value / orientacionComercialIPCAnioOrigen[41].Value) * (orientacionComercialTCAnioOrigen[41].Value / orientacionComercialTCAnioDestino[41].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[41].Value)) ? 0 : (orientacionComercialCPIAnioDestino[41].Value / orientacionComercialCPIAnioOrigen[41].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato57) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato57) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[42].Value) || decimalIsZero(orientacionComercialTCAnioDestino[42].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[42].Value / orientacionComercialIPCAnioOrigen[42].Value) * (orientacionComercialTCAnioOrigen[42].Value / orientacionComercialTCAnioDestino[42].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[42].Value)) ? 0 : (orientacionComercialCPIAnioDestino[42].Value / orientacionComercialCPIAnioOrigen[42].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato58) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato58) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[43].Value) || decimalIsZero(orientacionComercialTCAnioDestino[43].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[43].Value / orientacionComercialIPCAnioOrigen[43].Value) * (orientacionComercialTCAnioOrigen[43].Value / orientacionComercialTCAnioDestino[43].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[43].Value)) ? 0 : (orientacionComercialCPIAnioDestino[43].Value / orientacionComercialCPIAnioOrigen[43].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato59) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato59) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[44].Value) || decimalIsZero(orientacionComercialTCAnioDestino[44].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[44].Value / orientacionComercialIPCAnioOrigen[44].Value) * (orientacionComercialTCAnioOrigen[44].Value / orientacionComercialTCAnioDestino[44].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[44].Value)) ? 0 : (orientacionComercialCPIAnioDestino[44].Value / orientacionComercialCPIAnioOrigen[44].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato60) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato60) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[45].Value) || decimalIsZero(orientacionComercialTCAnioDestino[45].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[45].Value / orientacionComercialIPCAnioOrigen[45].Value) * (orientacionComercialTCAnioOrigen[45].Value / orientacionComercialTCAnioDestino[45].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[45].Value)) ? 0 : (orientacionComercialCPIAnioDestino[45].Value / orientacionComercialCPIAnioOrigen[45].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato61) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato61) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[46].Value) || decimalIsZero(orientacionComercialTCAnioDestino[46].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[46].Value / orientacionComercialIPCAnioOrigen[46].Value) * (orientacionComercialTCAnioOrigen[46].Value / orientacionComercialTCAnioDestino[46].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[46].Value)) ? 0 : (orientacionComercialCPIAnioDestino[46].Value / orientacionComercialCPIAnioOrigen[46].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato62) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato62) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[47].Value) || decimalIsZero(orientacionComercialTCAnioDestino[47].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[47].Value / orientacionComercialIPCAnioOrigen[47].Value) * (orientacionComercialTCAnioOrigen[47].Value / orientacionComercialTCAnioDestino[47].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[47].Value)) ? 0 : (orientacionComercialCPIAnioDestino[47].Value / orientacionComercialCPIAnioOrigen[47].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato63) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato63) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[48].Value) || decimalIsZero(orientacionComercialTCAnioDestino[48].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[48].Value / orientacionComercialIPCAnioOrigen[48].Value) * (orientacionComercialTCAnioOrigen[48].Value / orientacionComercialTCAnioDestino[48].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[48].Value)) ? 0 : (orientacionComercialCPIAnioDestino[48].Value / orientacionComercialCPIAnioOrigen[48].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato64) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato64) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[49].Value) || decimalIsZero(orientacionComercialTCAnioDestino[49].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[49].Value / orientacionComercialIPCAnioOrigen[49].Value) * (orientacionComercialTCAnioOrigen[49].Value / orientacionComercialTCAnioDestino[49].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[49].Value)) ? 0 : (orientacionComercialCPIAnioDestino[49].Value / orientacionComercialCPIAnioOrigen[49].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato65) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato65) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[50].Value) || decimalIsZero(orientacionComercialTCAnioDestino[50].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[50].Value / orientacionComercialIPCAnioOrigen[50].Value) * (orientacionComercialTCAnioOrigen[50].Value / orientacionComercialTCAnioDestino[50].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[50].Value)) ? 0 : (orientacionComercialCPIAnioDestino[50].Value / orientacionComercialCPIAnioOrigen[50].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato66) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato66) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[51].Value) || decimalIsZero(orientacionComercialTCAnioDestino[51].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[51].Value / orientacionComercialIPCAnioOrigen[51].Value) * (orientacionComercialTCAnioOrigen[51].Value / orientacionComercialTCAnioDestino[51].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[51].Value)) ? 0 : (orientacionComercialCPIAnioDestino[51].Value / orientacionComercialCPIAnioOrigen[51].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato67) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato67) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[52].Value) || decimalIsZero(orientacionComercialTCAnioDestino[52].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[52].Value / orientacionComercialIPCAnioOrigen[52].Value) * (orientacionComercialTCAnioOrigen[52].Value / orientacionComercialTCAnioDestino[52].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[52].Value)) ? 0 : (orientacionComercialCPIAnioDestino[52].Value / orientacionComercialCPIAnioOrigen[52].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato68) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato68) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[53].Value) || decimalIsZero(orientacionComercialTCAnioDestino[53].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[53].Value / orientacionComercialIPCAnioOrigen[53].Value) * (orientacionComercialTCAnioOrigen[53].Value / orientacionComercialTCAnioDestino[53].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[53].Value)) ? 0 : (orientacionComercialCPIAnioDestino[53].Value / orientacionComercialCPIAnioOrigen[53].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato69) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato69) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[54].Value) || decimalIsZero(orientacionComercialTCAnioDestino[54].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[54].Value / orientacionComercialIPCAnioOrigen[54].Value) * (orientacionComercialTCAnioOrigen[54].Value / orientacionComercialTCAnioDestino[54].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[54].Value)) ? 0 : (orientacionComercialCPIAnioDestino[54].Value / orientacionComercialCPIAnioOrigen[54].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato70) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato70) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[55].Value) || decimalIsZero(orientacionComercialTCAnioDestino[55].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[55].Value / orientacionComercialIPCAnioOrigen[55].Value) * (orientacionComercialTCAnioOrigen[55].Value / orientacionComercialTCAnioDestino[55].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[55].Value)) ? 0 : (orientacionComercialCPIAnioDestino[55].Value / orientacionComercialCPIAnioOrigen[55].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato71) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato71) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[56].Value) || decimalIsZero(orientacionComercialTCAnioDestino[56].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[56].Value / orientacionComercialIPCAnioOrigen[56].Value) * (orientacionComercialTCAnioOrigen[56].Value / orientacionComercialTCAnioDestino[56].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[56].Value)) ? 0 : (orientacionComercialCPIAnioDestino[56].Value / orientacionComercialCPIAnioOrigen[56].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato72) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato72) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[57].Value) || decimalIsZero(orientacionComercialTCAnioDestino[57].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[57].Value / orientacionComercialIPCAnioOrigen[57].Value) * (orientacionComercialTCAnioOrigen[57].Value / orientacionComercialTCAnioDestino[57].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[57].Value)) ? 0 : (orientacionComercialCPIAnioDestino[57].Value / orientacionComercialCPIAnioOrigen[57].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato73) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato73) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[58].Value) || decimalIsZero(orientacionComercialTCAnioDestino[58].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[58].Value / orientacionComercialIPCAnioOrigen[58].Value) * (orientacionComercialTCAnioOrigen[58].Value / orientacionComercialTCAnioDestino[58].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[58].Value)) ? 0 : (orientacionComercialCPIAnioDestino[58].Value / orientacionComercialCPIAnioOrigen[58].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato74) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato74) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[59].Value) || decimalIsZero(orientacionComercialTCAnioDestino[59].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[59].Value / orientacionComercialIPCAnioOrigen[59].Value) * (orientacionComercialTCAnioOrigen[59].Value / orientacionComercialTCAnioDestino[59].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[59].Value)) ? 0 : (orientacionComercialCPIAnioDestino[59].Value / orientacionComercialCPIAnioOrigen[59].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato75) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato75) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[60].Value) || decimalIsZero(orientacionComercialTCAnioDestino[60].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[60].Value / orientacionComercialIPCAnioOrigen[60].Value) * (orientacionComercialTCAnioOrigen[60].Value / orientacionComercialTCAnioDestino[60].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[60].Value)) ? 0 : (orientacionComercialCPIAnioDestino[60].Value / orientacionComercialCPIAnioOrigen[60].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato76) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato76) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[61].Value) || decimalIsZero(orientacionComercialTCAnioDestino[61].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[61].Value / orientacionComercialIPCAnioOrigen[61].Value) * (orientacionComercialTCAnioOrigen[61].Value / orientacionComercialTCAnioDestino[61].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[61].Value)) ? 0 : (orientacionComercialCPIAnioDestino[61].Value / orientacionComercialCPIAnioOrigen[61].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato77) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato77) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[62].Value) || decimalIsZero(orientacionComercialTCAnioDestino[62].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[62].Value / orientacionComercialIPCAnioOrigen[62].Value) * (orientacionComercialTCAnioOrigen[62].Value / orientacionComercialTCAnioDestino[62].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[62].Value)) ? 0 : (orientacionComercialCPIAnioDestino[62].Value / orientacionComercialCPIAnioOrigen[62].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato78) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato78) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[63].Value) || decimalIsZero(orientacionComercialTCAnioDestino[63].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[63].Value / orientacionComercialIPCAnioOrigen[63].Value) * (orientacionComercialTCAnioOrigen[63].Value / orientacionComercialTCAnioDestino[63].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[63].Value)) ? 0 : (orientacionComercialCPIAnioDestino[63].Value / orientacionComercialCPIAnioOrigen[63].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato79) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato79) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[64].Value) || decimalIsZero(orientacionComercialTCAnioDestino[64].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[64].Value / orientacionComercialIPCAnioOrigen[64].Value) * (orientacionComercialTCAnioOrigen[64].Value / orientacionComercialTCAnioDestino[64].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[64].Value)) ? 0 : (orientacionComercialCPIAnioDestino[64].Value / orientacionComercialCPIAnioOrigen[64].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato80) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato80) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[65].Value) || decimalIsZero(orientacionComercialTCAnioDestino[65].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[65].Value / orientacionComercialIPCAnioOrigen[65].Value) * (orientacionComercialTCAnioOrigen[65].Value / orientacionComercialTCAnioDestino[65].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[65].Value)) ? 0 : (orientacionComercialCPIAnioDestino[65].Value / orientacionComercialCPIAnioOrigen[65].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato81) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato81) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[66].Value) || decimalIsZero(orientacionComercialTCAnioDestino[66].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[66].Value / orientacionComercialIPCAnioOrigen[66].Value) * (orientacionComercialTCAnioOrigen[66].Value / orientacionComercialTCAnioDestino[66].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[66].Value)) ? 0 : (orientacionComercialCPIAnioDestino[66].Value / orientacionComercialCPIAnioOrigen[66].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato82) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato82) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[67].Value) || decimalIsZero(orientacionComercialTCAnioDestino[67].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[67].Value / orientacionComercialIPCAnioOrigen[67].Value) * (orientacionComercialTCAnioOrigen[67].Value / orientacionComercialTCAnioDestino[67].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[67].Value)) ? 0 : (orientacionComercialCPIAnioDestino[67].Value / orientacionComercialCPIAnioOrigen[67].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato83) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato83) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[68].Value) || decimalIsZero(orientacionComercialTCAnioDestino[68].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[68].Value / orientacionComercialIPCAnioOrigen[68].Value) * (orientacionComercialTCAnioOrigen[68].Value / orientacionComercialTCAnioDestino[68].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[68].Value)) ? 0 : (orientacionComercialCPIAnioDestino[68].Value / orientacionComercialCPIAnioOrigen[68].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato84) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato84) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[69].Value) || decimalIsZero(orientacionComercialTCAnioDestino[69].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[69].Value / orientacionComercialIPCAnioOrigen[69].Value) * (orientacionComercialTCAnioOrigen[69].Value / orientacionComercialTCAnioDestino[69].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[69].Value)) ? 0 : (orientacionComercialCPIAnioDestino[69].Value / orientacionComercialCPIAnioOrigen[69].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato85) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato85) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[70].Value) || decimalIsZero(orientacionComercialTCAnioDestino[70].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[70].Value / orientacionComercialIPCAnioOrigen[70].Value) * (orientacionComercialTCAnioOrigen[70].Value / orientacionComercialTCAnioDestino[70].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[70].Value)) ? 0 : (orientacionComercialCPIAnioDestino[70].Value / orientacionComercialCPIAnioOrigen[70].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato86) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato86) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[71].Value) || decimalIsZero(orientacionComercialTCAnioDestino[71].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[71].Value / orientacionComercialIPCAnioOrigen[71].Value) * (orientacionComercialTCAnioOrigen[71].Value / orientacionComercialTCAnioDestino[71].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[71].Value)) ? 0 : (orientacionComercialCPIAnioDestino[71].Value / orientacionComercialCPIAnioOrigen[71].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato87) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato87) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[72].Value) || decimalIsZero(orientacionComercialTCAnioDestino[72].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[72].Value / orientacionComercialIPCAnioOrigen[72].Value) * (orientacionComercialTCAnioOrigen[72].Value / orientacionComercialTCAnioDestino[72].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[72].Value)) ? 0 : (orientacionComercialCPIAnioDestino[72].Value / orientacionComercialCPIAnioOrigen[72].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato88) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato88) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[73].Value) || decimalIsZero(orientacionComercialTCAnioDestino[73].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[73].Value / orientacionComercialIPCAnioOrigen[73].Value) * (orientacionComercialTCAnioOrigen[73].Value / orientacionComercialTCAnioDestino[73].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[73].Value)) ? 0 : (orientacionComercialCPIAnioDestino[73].Value / orientacionComercialCPIAnioOrigen[73].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato89) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato89) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[74].Value) || decimalIsZero(orientacionComercialTCAnioDestino[74].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[74].Value / orientacionComercialIPCAnioOrigen[74].Value) * (orientacionComercialTCAnioOrigen[74].Value / orientacionComercialTCAnioDestino[74].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[74].Value)) ? 0 : (orientacionComercialCPIAnioDestino[74].Value / orientacionComercialCPIAnioOrigen[74].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato90) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato90) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[75].Value) || decimalIsZero(orientacionComercialTCAnioDestino[75].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[75].Value / orientacionComercialIPCAnioOrigen[75].Value) * (orientacionComercialTCAnioOrigen[75].Value / orientacionComercialTCAnioDestino[75].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[75].Value)) ? 0 : (orientacionComercialCPIAnioDestino[75].Value / orientacionComercialCPIAnioOrigen[75].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato91) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato91) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[76].Value) || decimalIsZero(orientacionComercialTCAnioDestino[76].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[76].Value / orientacionComercialIPCAnioOrigen[76].Value) * (orientacionComercialTCAnioOrigen[76].Value / orientacionComercialTCAnioDestino[76].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[76].Value)) ? 0 : (orientacionComercialCPIAnioDestino[76].Value / orientacionComercialCPIAnioOrigen[76].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato92) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato92) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[77].Value) || decimalIsZero(orientacionComercialTCAnioDestino[77].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[77].Value / orientacionComercialIPCAnioOrigen[77].Value) * (orientacionComercialTCAnioOrigen[77].Value / orientacionComercialTCAnioDestino[77].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[77].Value)) ? 0 : (orientacionComercialCPIAnioDestino[77].Value / orientacionComercialCPIAnioOrigen[77].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato93) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato93) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[78].Value) || decimalIsZero(orientacionComercialTCAnioDestino[78].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[78].Value / orientacionComercialIPCAnioOrigen[78].Value) * (orientacionComercialTCAnioOrigen[78].Value / orientacionComercialTCAnioDestino[78].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[78].Value)) ? 0 : (orientacionComercialCPIAnioDestino[78].Value / orientacionComercialCPIAnioOrigen[78].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato94) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato94) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[79].Value) || decimalIsZero(orientacionComercialTCAnioDestino[79].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[79].Value / orientacionComercialIPCAnioOrigen[79].Value) * (orientacionComercialTCAnioOrigen[79].Value / orientacionComercialTCAnioDestino[79].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[79].Value)) ? 0 : (orientacionComercialCPIAnioDestino[79].Value / orientacionComercialCPIAnioOrigen[79].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato95) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato95) * facCorrMonExt, ValorTotal = 0 });
                                                                decimal totalValorNacAnio = 0;
                                                                decimal totalValorExtAnio = 0;
                                                                for (int i = 0; i < consAnio.Count; i++)
                                                                {
                                                                    totalValorNacAnio += consAnio[i].ValorNac;
                                                                    totalValorExtAnio += consAnio[i].ValorExt;
                                                                    consAnio[i].ValorTotal = (consAnio[i].ValorNac + consAnio[i].ValorExt);
                                                                }
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = (perAntNac + totalValorNacMes + totalValorNacAnio), ValorExt = (perAntExt + totalValorExtMes + totalValorExtAnio), ValorTotal = ((perAntNac + totalValorNacMes + totalValorNacAnio) + (perAntExt + totalValorExtMes + totalValorExtAnio)) });
                                                            }
                                                            else if (IfDato0.StartsWith(prefixAdm))
                                                            {
                                                                admMes.Add(new Presupuesto.CellValue { Titulo = IfDato0, TituloNac = filaMonedaNacional.IfDato0.ToString(), TituloExt = filaMonedaExtranjera.IfDato0.ToString() });
                                                                decimal perAntNac = stringToDecimal(filaMonedaNacional.IfDato2);
                                                                decimal perAntExt = stringToDecimal(filaMonedaExtranjera.IfDato2);
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = perAntNac, ValorExt = perAntExt });
                                                                //Enero
                                                                decimal facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[0].Value) || decimalIsZero(orientacionComercialTCMesDestino[0].Value)) ? 0 : ((orientacionComercialIPCMesDestino[0].Value / orientacionComercialIPCMesOrigen[0].Value) * (orientacionComercialTCMesOrigen[0].Value / orientacionComercialTCMesDestino[0].Value)));
                                                                decimal facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[0].Value)) ? 0 : (orientacionComercialCPIMesDestino[0].Value / orientacionComercialCPIMesOrigen[0].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato3) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato3) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[1].Value) || decimalIsZero(orientacionComercialTCMesDestino[1].Value)) ? 0 : ((orientacionComercialIPCMesDestino[1].Value / orientacionComercialIPCMesOrigen[1].Value) * (orientacionComercialTCMesOrigen[1].Value / orientacionComercialTCMesDestino[1].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[1].Value)) ? 0 : (orientacionComercialCPIMesDestino[1].Value / orientacionComercialCPIMesOrigen[1].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato4) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato4) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[2].Value) || decimalIsZero(orientacionComercialTCMesDestino[2].Value)) ? 0 : ((orientacionComercialIPCMesDestino[2].Value / orientacionComercialIPCMesOrigen[2].Value) * (orientacionComercialTCMesOrigen[2].Value / orientacionComercialTCMesDestino[2].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[2].Value)) ? 0 : (orientacionComercialCPIMesDestino[2].Value / orientacionComercialCPIMesOrigen[2].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato5) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato5) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[3].Value) || decimalIsZero(orientacionComercialTCMesDestino[3].Value)) ? 0 : ((orientacionComercialIPCMesDestino[3].Value / orientacionComercialIPCMesOrigen[3].Value) * (orientacionComercialTCMesOrigen[3].Value / orientacionComercialTCMesDestino[3].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[3].Value)) ? 0 : (orientacionComercialCPIMesDestino[3].Value / orientacionComercialCPIMesOrigen[3].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato6) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato6) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[4].Value) || decimalIsZero(orientacionComercialTCMesDestino[4].Value)) ? 0 : ((orientacionComercialIPCMesDestino[4].Value / orientacionComercialIPCMesOrigen[4].Value) * (orientacionComercialTCMesOrigen[4].Value / orientacionComercialTCMesDestino[4].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[4].Value)) ? 0 : (orientacionComercialCPIMesDestino[4].Value / orientacionComercialCPIMesOrigen[4].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato7) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato7) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[5].Value) || decimalIsZero(orientacionComercialTCMesDestino[5].Value)) ? 0 : ((orientacionComercialIPCMesDestino[5].Value / orientacionComercialIPCMesOrigen[5].Value) * (orientacionComercialTCMesOrigen[5].Value / orientacionComercialTCMesDestino[5].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[5].Value)) ? 0 : (orientacionComercialCPIMesDestino[5].Value / orientacionComercialCPIMesOrigen[5].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato8) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato8) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[6].Value) || decimalIsZero(orientacionComercialTCMesDestino[6].Value)) ? 0 : ((orientacionComercialIPCMesDestino[6].Value / orientacionComercialIPCMesOrigen[6].Value) * (orientacionComercialTCMesOrigen[6].Value / orientacionComercialTCMesDestino[6].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[6].Value)) ? 0 : (orientacionComercialCPIMesDestino[6].Value / orientacionComercialCPIMesOrigen[6].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato9) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato9) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[7].Value) || decimalIsZero(orientacionComercialTCMesDestino[7].Value)) ? 0 : ((orientacionComercialIPCMesDestino[7].Value / orientacionComercialIPCMesOrigen[7].Value) * (orientacionComercialTCMesOrigen[7].Value / orientacionComercialTCMesDestino[7].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[7].Value)) ? 0 : (orientacionComercialCPIMesDestino[7].Value / orientacionComercialCPIMesOrigen[7].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato10) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato10) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[8].Value) || decimalIsZero(orientacionComercialTCMesDestino[8].Value)) ? 0 : ((orientacionComercialIPCMesDestino[8].Value / orientacionComercialIPCMesOrigen[8].Value) * (orientacionComercialTCMesOrigen[8].Value / orientacionComercialTCMesDestino[8].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[8].Value)) ? 0 : (orientacionComercialCPIMesDestino[8].Value / orientacionComercialCPIMesOrigen[8].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato11) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato11) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[9].Value) || decimalIsZero(orientacionComercialTCMesDestino[9].Value)) ? 0 : ((orientacionComercialIPCMesDestino[9].Value / orientacionComercialIPCMesOrigen[9].Value) * (orientacionComercialTCMesOrigen[9].Value / orientacionComercialTCMesDestino[9].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[9].Value)) ? 0 : (orientacionComercialCPIMesDestino[9].Value / orientacionComercialCPIMesOrigen[9].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato12) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato12) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[10].Value) || decimalIsZero(orientacionComercialTCMesDestino[10].Value)) ? 0 : ((orientacionComercialIPCMesDestino[10].Value / orientacionComercialIPCMesOrigen[10].Value) * (orientacionComercialTCMesOrigen[10].Value / orientacionComercialTCMesDestino[10].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[10].Value)) ? 0 : (orientacionComercialCPIMesDestino[10].Value / orientacionComercialCPIMesOrigen[10].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato13) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato13) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[11].Value) || decimalIsZero(orientacionComercialTCMesDestino[11].Value)) ? 0 : ((orientacionComercialIPCMesDestino[11].Value / orientacionComercialIPCMesOrigen[11].Value) * (orientacionComercialTCMesOrigen[11].Value / orientacionComercialTCMesDestino[11].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[11].Value)) ? 0 : (orientacionComercialCPIMesDestino[11].Value / orientacionComercialCPIMesOrigen[11].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato14) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato14) * facCorrMonExt, ValorTotal = 0 });
                                                                //diciembre
                                                                decimal totalValorNacMes = 0;
                                                                decimal totalValorExtMes = 0;
                                                                for (int i = 2; i < admMes.Count; i++)
                                                                {
                                                                    totalValorNacMes += admMes[i].ValorNac;
                                                                    totalValorExtMes += admMes[i].ValorExt;
                                                                    admMes[i].ValorTotal = (admMes[i].ValorNac + admMes[i].ValorExt);
                                                                }
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = totalValorNacMes, ValorExt = totalValorExtMes, ValorTotal = (totalValorNacMes + totalValorExtMes) });
                                                                //Total meses
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[0].Value) || decimalIsZero(orientacionComercialTCAnioDestino[0].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[0].Value / orientacionComercialIPCAnioOrigen[0].Value) * (orientacionComercialTCAnioOrigen[0].Value / orientacionComercialTCAnioDestino[0].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[0].Value)) ? 0 : (orientacionComercialCPIAnioDestino[0].Value / orientacionComercialCPIAnioOrigen[0].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato16) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato16) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[1].Value) || decimalIsZero(orientacionComercialTCAnioDestino[1].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[1].Value / orientacionComercialIPCAnioOrigen[1].Value) * (orientacionComercialTCAnioOrigen[1].Value / orientacionComercialTCAnioDestino[1].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[1].Value)) ? 0 : (orientacionComercialCPIAnioDestino[1].Value / orientacionComercialCPIAnioOrigen[1].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato17) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato17) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[2].Value) || decimalIsZero(orientacionComercialTCAnioDestino[2].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[2].Value / orientacionComercialIPCAnioOrigen[2].Value) * (orientacionComercialTCAnioOrigen[2].Value / orientacionComercialTCAnioDestino[2].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[2].Value)) ? 0 : (orientacionComercialCPIAnioDestino[2].Value / orientacionComercialCPIAnioOrigen[2].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato18) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato18) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[3].Value) || decimalIsZero(orientacionComercialTCAnioDestino[3].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[3].Value / orientacionComercialIPCAnioOrigen[3].Value) * (orientacionComercialTCAnioOrigen[3].Value / orientacionComercialTCAnioDestino[3].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[3].Value)) ? 0 : (orientacionComercialCPIAnioDestino[3].Value / orientacionComercialCPIAnioOrigen[3].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato19) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato19) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[4].Value) || decimalIsZero(orientacionComercialTCAnioDestino[4].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[4].Value / orientacionComercialIPCAnioOrigen[4].Value) * (orientacionComercialTCAnioOrigen[4].Value / orientacionComercialTCAnioDestino[4].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[4].Value)) ? 0 : (orientacionComercialCPIAnioDestino[4].Value / orientacionComercialCPIAnioOrigen[4].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato20) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato20) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[5].Value) || decimalIsZero(orientacionComercialTCAnioDestino[5].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[5].Value / orientacionComercialIPCAnioOrigen[5].Value) * (orientacionComercialTCAnioOrigen[5].Value / orientacionComercialTCAnioDestino[5].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[5].Value)) ? 0 : (orientacionComercialCPIAnioDestino[5].Value / orientacionComercialCPIAnioOrigen[5].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato21) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato21) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[6].Value) || decimalIsZero(orientacionComercialTCAnioDestino[6].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[6].Value / orientacionComercialIPCAnioOrigen[6].Value) * (orientacionComercialTCAnioOrigen[6].Value / orientacionComercialTCAnioDestino[6].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[6].Value)) ? 0 : (orientacionComercialCPIAnioDestino[6].Value / orientacionComercialCPIAnioOrigen[6].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato22) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato22) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[7].Value) || decimalIsZero(orientacionComercialTCAnioDestino[7].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[7].Value / orientacionComercialIPCAnioOrigen[7].Value) * (orientacionComercialTCAnioOrigen[7].Value / orientacionComercialTCAnioDestino[7].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[7].Value)) ? 0 : (orientacionComercialCPIAnioDestino[7].Value / orientacionComercialCPIAnioOrigen[7].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato23) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato23) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[8].Value) || decimalIsZero(orientacionComercialTCAnioDestino[8].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[8].Value / orientacionComercialIPCAnioOrigen[8].Value) * (orientacionComercialTCAnioOrigen[8].Value / orientacionComercialTCAnioDestino[8].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[8].Value)) ? 0 : (orientacionComercialCPIAnioDestino[8].Value / orientacionComercialCPIAnioOrigen[8].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato24) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato24) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[9].Value) || decimalIsZero(orientacionComercialTCAnioDestino[9].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[9].Value / orientacionComercialIPCAnioOrigen[9].Value) * (orientacionComercialTCAnioOrigen[9].Value / orientacionComercialTCAnioDestino[9].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[9].Value)) ? 0 : (orientacionComercialCPIAnioDestino[9].Value / orientacionComercialCPIAnioOrigen[9].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato25) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato25) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[10].Value) || decimalIsZero(orientacionComercialTCAnioDestino[10].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[10].Value / orientacionComercialIPCAnioOrigen[10].Value) * (orientacionComercialTCAnioOrigen[10].Value / orientacionComercialTCAnioDestino[10].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[10].Value)) ? 0 : (orientacionComercialCPIAnioDestino[10].Value / orientacionComercialCPIAnioOrigen[10].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato26) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato26) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[11].Value) || decimalIsZero(orientacionComercialTCAnioDestino[11].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[11].Value / orientacionComercialIPCAnioOrigen[11].Value) * (orientacionComercialTCAnioOrigen[11].Value / orientacionComercialTCAnioDestino[11].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[11].Value)) ? 0 : (orientacionComercialCPIAnioDestino[11].Value / orientacionComercialCPIAnioOrigen[11].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato27) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato27) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[12].Value) || decimalIsZero(orientacionComercialTCAnioDestino[12].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[12].Value / orientacionComercialIPCAnioOrigen[12].Value) * (orientacionComercialTCAnioOrigen[12].Value / orientacionComercialTCAnioDestino[12].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[12].Value)) ? 0 : (orientacionComercialCPIAnioDestino[12].Value / orientacionComercialCPIAnioOrigen[12].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato28) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato28) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[13].Value) || decimalIsZero(orientacionComercialTCAnioDestino[13].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[13].Value / orientacionComercialIPCAnioOrigen[13].Value) * (orientacionComercialTCAnioOrigen[13].Value / orientacionComercialTCAnioDestino[13].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[13].Value)) ? 0 : (orientacionComercialCPIAnioDestino[13].Value / orientacionComercialCPIAnioOrigen[13].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato29) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato29) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[14].Value) || decimalIsZero(orientacionComercialTCAnioDestino[14].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[14].Value / orientacionComercialIPCAnioOrigen[14].Value) * (orientacionComercialTCAnioOrigen[14].Value / orientacionComercialTCAnioDestino[14].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[14].Value)) ? 0 : (orientacionComercialCPIAnioDestino[14].Value / orientacionComercialCPIAnioOrigen[14].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato30) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato30) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[15].Value) || decimalIsZero(orientacionComercialTCAnioDestino[15].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[15].Value / orientacionComercialIPCAnioOrigen[15].Value) * (orientacionComercialTCAnioOrigen[15].Value / orientacionComercialTCAnioDestino[15].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[15].Value)) ? 0 : (orientacionComercialCPIAnioDestino[15].Value / orientacionComercialCPIAnioOrigen[15].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato31) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato31) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[16].Value) || decimalIsZero(orientacionComercialTCAnioDestino[16].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[16].Value / orientacionComercialIPCAnioOrigen[16].Value) * (orientacionComercialTCAnioOrigen[16].Value / orientacionComercialTCAnioDestino[16].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[16].Value)) ? 0 : (orientacionComercialCPIAnioDestino[16].Value / orientacionComercialCPIAnioOrigen[16].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato32) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato32) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[17].Value) || decimalIsZero(orientacionComercialTCAnioDestino[17].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[17].Value / orientacionComercialIPCAnioOrigen[17].Value) * (orientacionComercialTCAnioOrigen[17].Value / orientacionComercialTCAnioDestino[17].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[17].Value)) ? 0 : (orientacionComercialCPIAnioDestino[17].Value / orientacionComercialCPIAnioOrigen[17].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato33) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato33) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[18].Value) || decimalIsZero(orientacionComercialTCAnioDestino[18].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[18].Value / orientacionComercialIPCAnioOrigen[18].Value) * (orientacionComercialTCAnioOrigen[18].Value / orientacionComercialTCAnioDestino[18].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[18].Value)) ? 0 : (orientacionComercialCPIAnioDestino[18].Value / orientacionComercialCPIAnioOrigen[18].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato34) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato34) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[19].Value) || decimalIsZero(orientacionComercialTCAnioDestino[19].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[19].Value / orientacionComercialIPCAnioOrigen[19].Value) * (orientacionComercialTCAnioOrigen[19].Value / orientacionComercialTCAnioDestino[19].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[19].Value)) ? 0 : (orientacionComercialCPIAnioDestino[19].Value / orientacionComercialCPIAnioOrigen[19].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato35) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato35) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[20].Value) || decimalIsZero(orientacionComercialTCAnioDestino[20].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[20].Value / orientacionComercialIPCAnioOrigen[20].Value) * (orientacionComercialTCAnioOrigen[20].Value / orientacionComercialTCAnioDestino[20].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[20].Value)) ? 0 : (orientacionComercialCPIAnioDestino[20].Value / orientacionComercialCPIAnioOrigen[20].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato36) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato36) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[21].Value) || decimalIsZero(orientacionComercialTCAnioDestino[21].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[21].Value / orientacionComercialIPCAnioOrigen[21].Value) * (orientacionComercialTCAnioOrigen[21].Value / orientacionComercialTCAnioDestino[21].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[21].Value)) ? 0 : (orientacionComercialCPIAnioDestino[21].Value / orientacionComercialCPIAnioOrigen[21].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato37) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato37) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[22].Value) || decimalIsZero(orientacionComercialTCAnioDestino[22].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[22].Value / orientacionComercialIPCAnioOrigen[22].Value) * (orientacionComercialTCAnioOrigen[22].Value / orientacionComercialTCAnioDestino[22].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[22].Value)) ? 0 : (orientacionComercialCPIAnioDestino[22].Value / orientacionComercialCPIAnioOrigen[22].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato38) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato38) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[23].Value) || decimalIsZero(orientacionComercialTCAnioDestino[23].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[23].Value / orientacionComercialIPCAnioOrigen[23].Value) * (orientacionComercialTCAnioOrigen[23].Value / orientacionComercialTCAnioDestino[23].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[23].Value)) ? 0 : (orientacionComercialCPIAnioDestino[23].Value / orientacionComercialCPIAnioOrigen[23].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato39) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato39) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[24].Value) || decimalIsZero(orientacionComercialTCAnioDestino[24].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[24].Value / orientacionComercialIPCAnioOrigen[24].Value) * (orientacionComercialTCAnioOrigen[24].Value / orientacionComercialTCAnioDestino[24].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[24].Value)) ? 0 : (orientacionComercialCPIAnioDestino[24].Value / orientacionComercialCPIAnioOrigen[24].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato40) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato40) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[25].Value) || decimalIsZero(orientacionComercialTCAnioDestino[25].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[25].Value / orientacionComercialIPCAnioOrigen[25].Value) * (orientacionComercialTCAnioOrigen[25].Value / orientacionComercialTCAnioDestino[25].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[25].Value)) ? 0 : (orientacionComercialCPIAnioDestino[25].Value / orientacionComercialCPIAnioOrigen[25].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato41) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato41) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[26].Value) || decimalIsZero(orientacionComercialTCAnioDestino[26].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[26].Value / orientacionComercialIPCAnioOrigen[26].Value) * (orientacionComercialTCAnioOrigen[26].Value / orientacionComercialTCAnioDestino[26].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[26].Value)) ? 0 : (orientacionComercialCPIAnioDestino[26].Value / orientacionComercialCPIAnioOrigen[26].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato42) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato42) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[27].Value) || decimalIsZero(orientacionComercialTCAnioDestino[27].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[27].Value / orientacionComercialIPCAnioOrigen[27].Value) * (orientacionComercialTCAnioOrigen[27].Value / orientacionComercialTCAnioDestino[27].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[27].Value)) ? 0 : (orientacionComercialCPIAnioDestino[27].Value / orientacionComercialCPIAnioOrigen[27].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato43) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato43) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[28].Value) || decimalIsZero(orientacionComercialTCAnioDestino[28].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[28].Value / orientacionComercialIPCAnioOrigen[28].Value) * (orientacionComercialTCAnioOrigen[28].Value / orientacionComercialTCAnioDestino[28].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[28].Value)) ? 0 : (orientacionComercialCPIAnioDestino[28].Value / orientacionComercialCPIAnioOrigen[28].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato44) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato44) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[29].Value) || decimalIsZero(orientacionComercialTCAnioDestino[29].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[29].Value / orientacionComercialIPCAnioOrigen[29].Value) * (orientacionComercialTCAnioOrigen[29].Value / orientacionComercialTCAnioDestino[29].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[29].Value)) ? 0 : (orientacionComercialCPIAnioDestino[29].Value / orientacionComercialCPIAnioOrigen[29].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato45) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato45) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[30].Value) || decimalIsZero(orientacionComercialTCAnioDestino[30].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[30].Value / orientacionComercialIPCAnioOrigen[30].Value) * (orientacionComercialTCAnioOrigen[30].Value / orientacionComercialTCAnioDestino[30].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[30].Value)) ? 0 : (orientacionComercialCPIAnioDestino[30].Value / orientacionComercialCPIAnioOrigen[30].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato46) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato46) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[31].Value) || decimalIsZero(orientacionComercialTCAnioDestino[31].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[31].Value / orientacionComercialIPCAnioOrigen[31].Value) * (orientacionComercialTCAnioOrigen[31].Value / orientacionComercialTCAnioDestino[31].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[31].Value)) ? 0 : (orientacionComercialCPIAnioDestino[31].Value / orientacionComercialCPIAnioOrigen[31].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato47) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato47) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[32].Value) || decimalIsZero(orientacionComercialTCAnioDestino[32].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[32].Value / orientacionComercialIPCAnioOrigen[32].Value) * (orientacionComercialTCAnioOrigen[32].Value / orientacionComercialTCAnioDestino[32].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[32].Value)) ? 0 : (orientacionComercialCPIAnioDestino[32].Value / orientacionComercialCPIAnioOrigen[32].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato48) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato48) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[33].Value) || decimalIsZero(orientacionComercialTCAnioDestino[33].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[33].Value / orientacionComercialIPCAnioOrigen[33].Value) * (orientacionComercialTCAnioOrigen[33].Value / orientacionComercialTCAnioDestino[33].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[33].Value)) ? 0 : (orientacionComercialCPIAnioDestino[33].Value / orientacionComercialCPIAnioOrigen[33].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato49) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato49) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[34].Value) || decimalIsZero(orientacionComercialTCAnioDestino[34].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[34].Value / orientacionComercialIPCAnioOrigen[34].Value) * (orientacionComercialTCAnioOrigen[34].Value / orientacionComercialTCAnioDestino[34].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[34].Value)) ? 0 : (orientacionComercialCPIAnioDestino[34].Value / orientacionComercialCPIAnioOrigen[34].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato50) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato50) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[35].Value) || decimalIsZero(orientacionComercialTCAnioDestino[35].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[35].Value / orientacionComercialIPCAnioOrigen[35].Value) * (orientacionComercialTCAnioOrigen[35].Value / orientacionComercialTCAnioDestino[35].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[35].Value)) ? 0 : (orientacionComercialCPIAnioDestino[35].Value / orientacionComercialCPIAnioOrigen[35].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato51) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato51) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[36].Value) || decimalIsZero(orientacionComercialTCAnioDestino[36].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[36].Value / orientacionComercialIPCAnioOrigen[36].Value) * (orientacionComercialTCAnioOrigen[36].Value / orientacionComercialTCAnioDestino[36].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[36].Value)) ? 0 : (orientacionComercialCPIAnioDestino[36].Value / orientacionComercialCPIAnioOrigen[36].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato52) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato52) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[37].Value) || decimalIsZero(orientacionComercialTCAnioDestino[37].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[37].Value / orientacionComercialIPCAnioOrigen[37].Value) * (orientacionComercialTCAnioOrigen[37].Value / orientacionComercialTCAnioDestino[37].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[37].Value)) ? 0 : (orientacionComercialCPIAnioDestino[37].Value / orientacionComercialCPIAnioOrigen[37].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato53) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato53) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[38].Value) || decimalIsZero(orientacionComercialTCAnioDestino[38].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[38].Value / orientacionComercialIPCAnioOrigen[38].Value) * (orientacionComercialTCAnioOrigen[38].Value / orientacionComercialTCAnioDestino[38].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[38].Value)) ? 0 : (orientacionComercialCPIAnioDestino[38].Value / orientacionComercialCPIAnioOrigen[38].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato54) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato54) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[39].Value) || decimalIsZero(orientacionComercialTCAnioDestino[39].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[39].Value / orientacionComercialIPCAnioOrigen[39].Value) * (orientacionComercialTCAnioOrigen[39].Value / orientacionComercialTCAnioDestino[39].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[39].Value)) ? 0 : (orientacionComercialCPIAnioDestino[39].Value / orientacionComercialCPIAnioOrigen[39].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato55) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato55) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[40].Value) || decimalIsZero(orientacionComercialTCAnioDestino[40].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[40].Value / orientacionComercialIPCAnioOrigen[40].Value) * (orientacionComercialTCAnioOrigen[40].Value / orientacionComercialTCAnioDestino[40].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[40].Value)) ? 0 : (orientacionComercialCPIAnioDestino[40].Value / orientacionComercialCPIAnioOrigen[40].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato56) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato56) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[41].Value) || decimalIsZero(orientacionComercialTCAnioDestino[41].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[41].Value / orientacionComercialIPCAnioOrigen[41].Value) * (orientacionComercialTCAnioOrigen[41].Value / orientacionComercialTCAnioDestino[41].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[41].Value)) ? 0 : (orientacionComercialCPIAnioDestino[41].Value / orientacionComercialCPIAnioOrigen[41].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato57) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato57) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[42].Value) || decimalIsZero(orientacionComercialTCAnioDestino[42].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[42].Value / orientacionComercialIPCAnioOrigen[42].Value) * (orientacionComercialTCAnioOrigen[42].Value / orientacionComercialTCAnioDestino[42].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[42].Value)) ? 0 : (orientacionComercialCPIAnioDestino[42].Value / orientacionComercialCPIAnioOrigen[42].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato58) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato58) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[43].Value) || decimalIsZero(orientacionComercialTCAnioDestino[43].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[43].Value / orientacionComercialIPCAnioOrigen[43].Value) * (orientacionComercialTCAnioOrigen[43].Value / orientacionComercialTCAnioDestino[43].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[43].Value)) ? 0 : (orientacionComercialCPIAnioDestino[43].Value / orientacionComercialCPIAnioOrigen[43].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato59) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato59) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[44].Value) || decimalIsZero(orientacionComercialTCAnioDestino[44].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[44].Value / orientacionComercialIPCAnioOrigen[44].Value) * (orientacionComercialTCAnioOrigen[44].Value / orientacionComercialTCAnioDestino[44].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[44].Value)) ? 0 : (orientacionComercialCPIAnioDestino[44].Value / orientacionComercialCPIAnioOrigen[44].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato60) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato60) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[45].Value) || decimalIsZero(orientacionComercialTCAnioDestino[45].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[45].Value / orientacionComercialIPCAnioOrigen[45].Value) * (orientacionComercialTCAnioOrigen[45].Value / orientacionComercialTCAnioDestino[45].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[45].Value)) ? 0 : (orientacionComercialCPIAnioDestino[45].Value / orientacionComercialCPIAnioOrigen[45].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato61) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato61) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[46].Value) || decimalIsZero(orientacionComercialTCAnioDestino[46].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[46].Value / orientacionComercialIPCAnioOrigen[46].Value) * (orientacionComercialTCAnioOrigen[46].Value / orientacionComercialTCAnioDestino[46].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[46].Value)) ? 0 : (orientacionComercialCPIAnioDestino[46].Value / orientacionComercialCPIAnioOrigen[46].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato62) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato62) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[47].Value) || decimalIsZero(orientacionComercialTCAnioDestino[47].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[47].Value / orientacionComercialIPCAnioOrigen[47].Value) * (orientacionComercialTCAnioOrigen[47].Value / orientacionComercialTCAnioDestino[47].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[47].Value)) ? 0 : (orientacionComercialCPIAnioDestino[47].Value / orientacionComercialCPIAnioOrigen[47].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato63) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato63) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[48].Value) || decimalIsZero(orientacionComercialTCAnioDestino[48].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[48].Value / orientacionComercialIPCAnioOrigen[48].Value) * (orientacionComercialTCAnioOrigen[48].Value / orientacionComercialTCAnioDestino[48].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[48].Value)) ? 0 : (orientacionComercialCPIAnioDestino[48].Value / orientacionComercialCPIAnioOrigen[48].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato64) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato64) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[49].Value) || decimalIsZero(orientacionComercialTCAnioDestino[49].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[49].Value / orientacionComercialIPCAnioOrigen[49].Value) * (orientacionComercialTCAnioOrigen[49].Value / orientacionComercialTCAnioDestino[49].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[49].Value)) ? 0 : (orientacionComercialCPIAnioDestino[49].Value / orientacionComercialCPIAnioOrigen[49].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato65) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato65) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[50].Value) || decimalIsZero(orientacionComercialTCAnioDestino[50].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[50].Value / orientacionComercialIPCAnioOrigen[50].Value) * (orientacionComercialTCAnioOrigen[50].Value / orientacionComercialTCAnioDestino[50].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[50].Value)) ? 0 : (orientacionComercialCPIAnioDestino[50].Value / orientacionComercialCPIAnioOrigen[50].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato66) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato66) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[51].Value) || decimalIsZero(orientacionComercialTCAnioDestino[51].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[51].Value / orientacionComercialIPCAnioOrigen[51].Value) * (orientacionComercialTCAnioOrigen[51].Value / orientacionComercialTCAnioDestino[51].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[51].Value)) ? 0 : (orientacionComercialCPIAnioDestino[51].Value / orientacionComercialCPIAnioOrigen[51].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato67) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato67) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[52].Value) || decimalIsZero(orientacionComercialTCAnioDestino[52].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[52].Value / orientacionComercialIPCAnioOrigen[52].Value) * (orientacionComercialTCAnioOrigen[52].Value / orientacionComercialTCAnioDestino[52].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[52].Value)) ? 0 : (orientacionComercialCPIAnioDestino[52].Value / orientacionComercialCPIAnioOrigen[52].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato68) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato68) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[53].Value) || decimalIsZero(orientacionComercialTCAnioDestino[53].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[53].Value / orientacionComercialIPCAnioOrigen[53].Value) * (orientacionComercialTCAnioOrigen[53].Value / orientacionComercialTCAnioDestino[53].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[53].Value)) ? 0 : (orientacionComercialCPIAnioDestino[53].Value / orientacionComercialCPIAnioOrigen[53].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato69) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato69) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[54].Value) || decimalIsZero(orientacionComercialTCAnioDestino[54].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[54].Value / orientacionComercialIPCAnioOrigen[54].Value) * (orientacionComercialTCAnioOrigen[54].Value / orientacionComercialTCAnioDestino[54].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[54].Value)) ? 0 : (orientacionComercialCPIAnioDestino[54].Value / orientacionComercialCPIAnioOrigen[54].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato70) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato70) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[55].Value) || decimalIsZero(orientacionComercialTCAnioDestino[55].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[55].Value / orientacionComercialIPCAnioOrigen[55].Value) * (orientacionComercialTCAnioOrigen[55].Value / orientacionComercialTCAnioDestino[55].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[55].Value)) ? 0 : (orientacionComercialCPIAnioDestino[55].Value / orientacionComercialCPIAnioOrigen[55].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato71) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato71) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[56].Value) || decimalIsZero(orientacionComercialTCAnioDestino[56].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[56].Value / orientacionComercialIPCAnioOrigen[56].Value) * (orientacionComercialTCAnioOrigen[56].Value / orientacionComercialTCAnioDestino[56].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[56].Value)) ? 0 : (orientacionComercialCPIAnioDestino[56].Value / orientacionComercialCPIAnioOrigen[56].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato72) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato72) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[57].Value) || decimalIsZero(orientacionComercialTCAnioDestino[57].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[57].Value / orientacionComercialIPCAnioOrigen[57].Value) * (orientacionComercialTCAnioOrigen[57].Value / orientacionComercialTCAnioDestino[57].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[57].Value)) ? 0 : (orientacionComercialCPIAnioDestino[57].Value / orientacionComercialCPIAnioOrigen[57].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato73) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato73) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[58].Value) || decimalIsZero(orientacionComercialTCAnioDestino[58].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[58].Value / orientacionComercialIPCAnioOrigen[58].Value) * (orientacionComercialTCAnioOrigen[58].Value / orientacionComercialTCAnioDestino[58].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[58].Value)) ? 0 : (orientacionComercialCPIAnioDestino[58].Value / orientacionComercialCPIAnioOrigen[58].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato74) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato74) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[59].Value) || decimalIsZero(orientacionComercialTCAnioDestino[59].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[59].Value / orientacionComercialIPCAnioOrigen[59].Value) * (orientacionComercialTCAnioOrigen[59].Value / orientacionComercialTCAnioDestino[59].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[59].Value)) ? 0 : (orientacionComercialCPIAnioDestino[59].Value / orientacionComercialCPIAnioOrigen[59].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato75) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato75) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[60].Value) || decimalIsZero(orientacionComercialTCAnioDestino[60].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[60].Value / orientacionComercialIPCAnioOrigen[60].Value) * (orientacionComercialTCAnioOrigen[60].Value / orientacionComercialTCAnioDestino[60].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[60].Value)) ? 0 : (orientacionComercialCPIAnioDestino[60].Value / orientacionComercialCPIAnioOrigen[60].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato76) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato76) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[61].Value) || decimalIsZero(orientacionComercialTCAnioDestino[61].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[61].Value / orientacionComercialIPCAnioOrigen[61].Value) * (orientacionComercialTCAnioOrigen[61].Value / orientacionComercialTCAnioDestino[61].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[61].Value)) ? 0 : (orientacionComercialCPIAnioDestino[61].Value / orientacionComercialCPIAnioOrigen[61].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato77) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato77) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[62].Value) || decimalIsZero(orientacionComercialTCAnioDestino[62].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[62].Value / orientacionComercialIPCAnioOrigen[62].Value) * (orientacionComercialTCAnioOrigen[62].Value / orientacionComercialTCAnioDestino[62].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[62].Value)) ? 0 : (orientacionComercialCPIAnioDestino[62].Value / orientacionComercialCPIAnioOrigen[62].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato78) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato78) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[63].Value) || decimalIsZero(orientacionComercialTCAnioDestino[63].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[63].Value / orientacionComercialIPCAnioOrigen[63].Value) * (orientacionComercialTCAnioOrigen[63].Value / orientacionComercialTCAnioDestino[63].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[63].Value)) ? 0 : (orientacionComercialCPIAnioDestino[63].Value / orientacionComercialCPIAnioOrigen[63].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato79) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato79) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[64].Value) || decimalIsZero(orientacionComercialTCAnioDestino[64].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[64].Value / orientacionComercialIPCAnioOrigen[64].Value) * (orientacionComercialTCAnioOrigen[64].Value / orientacionComercialTCAnioDestino[64].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[64].Value)) ? 0 : (orientacionComercialCPIAnioDestino[64].Value / orientacionComercialCPIAnioOrigen[64].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato80) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato80) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[65].Value) || decimalIsZero(orientacionComercialTCAnioDestino[65].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[65].Value / orientacionComercialIPCAnioOrigen[65].Value) * (orientacionComercialTCAnioOrigen[65].Value / orientacionComercialTCAnioDestino[65].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[65].Value)) ? 0 : (orientacionComercialCPIAnioDestino[65].Value / orientacionComercialCPIAnioOrigen[65].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato81) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato81) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[66].Value) || decimalIsZero(orientacionComercialTCAnioDestino[66].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[66].Value / orientacionComercialIPCAnioOrigen[66].Value) * (orientacionComercialTCAnioOrigen[66].Value / orientacionComercialTCAnioDestino[66].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[66].Value)) ? 0 : (orientacionComercialCPIAnioDestino[66].Value / orientacionComercialCPIAnioOrigen[66].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato82) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato82) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[67].Value) || decimalIsZero(orientacionComercialTCAnioDestino[67].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[67].Value / orientacionComercialIPCAnioOrigen[67].Value) * (orientacionComercialTCAnioOrigen[67].Value / orientacionComercialTCAnioDestino[67].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[67].Value)) ? 0 : (orientacionComercialCPIAnioDestino[67].Value / orientacionComercialCPIAnioOrigen[67].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato83) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato83) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[68].Value) || decimalIsZero(orientacionComercialTCAnioDestino[68].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[68].Value / orientacionComercialIPCAnioOrigen[68].Value) * (orientacionComercialTCAnioOrigen[68].Value / orientacionComercialTCAnioDestino[68].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[68].Value)) ? 0 : (orientacionComercialCPIAnioDestino[68].Value / orientacionComercialCPIAnioOrigen[68].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato84) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato84) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[69].Value) || decimalIsZero(orientacionComercialTCAnioDestino[69].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[69].Value / orientacionComercialIPCAnioOrigen[69].Value) * (orientacionComercialTCAnioOrigen[69].Value / orientacionComercialTCAnioDestino[69].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[69].Value)) ? 0 : (orientacionComercialCPIAnioDestino[69].Value / orientacionComercialCPIAnioOrigen[69].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato85) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato85) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[70].Value) || decimalIsZero(orientacionComercialTCAnioDestino[70].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[70].Value / orientacionComercialIPCAnioOrigen[70].Value) * (orientacionComercialTCAnioOrigen[70].Value / orientacionComercialTCAnioDestino[70].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[70].Value)) ? 0 : (orientacionComercialCPIAnioDestino[70].Value / orientacionComercialCPIAnioOrigen[70].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato86) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato86) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[71].Value) || decimalIsZero(orientacionComercialTCAnioDestino[71].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[71].Value / orientacionComercialIPCAnioOrigen[71].Value) * (orientacionComercialTCAnioOrigen[71].Value / orientacionComercialTCAnioDestino[71].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[71].Value)) ? 0 : (orientacionComercialCPIAnioDestino[71].Value / orientacionComercialCPIAnioOrigen[71].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato87) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato87) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[72].Value) || decimalIsZero(orientacionComercialTCAnioDestino[72].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[72].Value / orientacionComercialIPCAnioOrigen[72].Value) * (orientacionComercialTCAnioOrigen[72].Value / orientacionComercialTCAnioDestino[72].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[72].Value)) ? 0 : (orientacionComercialCPIAnioDestino[72].Value / orientacionComercialCPIAnioOrigen[72].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato88) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato88) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[73].Value) || decimalIsZero(orientacionComercialTCAnioDestino[73].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[73].Value / orientacionComercialIPCAnioOrigen[73].Value) * (orientacionComercialTCAnioOrigen[73].Value / orientacionComercialTCAnioDestino[73].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[73].Value)) ? 0 : (orientacionComercialCPIAnioDestino[73].Value / orientacionComercialCPIAnioOrigen[73].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato89) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato89) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[74].Value) || decimalIsZero(orientacionComercialTCAnioDestino[74].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[74].Value / orientacionComercialIPCAnioOrigen[74].Value) * (orientacionComercialTCAnioOrigen[74].Value / orientacionComercialTCAnioDestino[74].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[74].Value)) ? 0 : (orientacionComercialCPIAnioDestino[74].Value / orientacionComercialCPIAnioOrigen[74].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato90) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato90) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[75].Value) || decimalIsZero(orientacionComercialTCAnioDestino[75].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[75].Value / orientacionComercialIPCAnioOrigen[75].Value) * (orientacionComercialTCAnioOrigen[75].Value / orientacionComercialTCAnioDestino[75].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[75].Value)) ? 0 : (orientacionComercialCPIAnioDestino[75].Value / orientacionComercialCPIAnioOrigen[75].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato91) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato91) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[76].Value) || decimalIsZero(orientacionComercialTCAnioDestino[76].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[76].Value / orientacionComercialIPCAnioOrigen[76].Value) * (orientacionComercialTCAnioOrigen[76].Value / orientacionComercialTCAnioDestino[76].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[76].Value)) ? 0 : (orientacionComercialCPIAnioDestino[76].Value / orientacionComercialCPIAnioOrigen[76].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato92) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato92) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[77].Value) || decimalIsZero(orientacionComercialTCAnioDestino[77].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[77].Value / orientacionComercialIPCAnioOrigen[77].Value) * (orientacionComercialTCAnioOrigen[77].Value / orientacionComercialTCAnioDestino[77].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[77].Value)) ? 0 : (orientacionComercialCPIAnioDestino[77].Value / orientacionComercialCPIAnioOrigen[77].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato93) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato93) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[78].Value) || decimalIsZero(orientacionComercialTCAnioDestino[78].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[78].Value / orientacionComercialIPCAnioOrigen[78].Value) * (orientacionComercialTCAnioOrigen[78].Value / orientacionComercialTCAnioDestino[78].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[78].Value)) ? 0 : (orientacionComercialCPIAnioDestino[78].Value / orientacionComercialCPIAnioOrigen[78].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato94) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato94) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[79].Value) || decimalIsZero(orientacionComercialTCAnioDestino[79].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[79].Value / orientacionComercialIPCAnioOrigen[79].Value) * (orientacionComercialTCAnioOrigen[79].Value / orientacionComercialTCAnioDestino[79].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[79].Value)) ? 0 : (orientacionComercialCPIAnioDestino[79].Value / orientacionComercialCPIAnioOrigen[79].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato95) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato95) * facCorrMonExt, ValorTotal = 0 });
                                                                decimal totalValorNacAnio = 0;
                                                                decimal totalValorExtAnio = 0;
                                                                for (int i = 0; i < admAnio.Count; i++)
                                                                {
                                                                    totalValorNacAnio += admAnio[i].ValorNac;
                                                                    totalValorExtAnio += admAnio[i].ValorExt;
                                                                    admAnio[i].ValorTotal = (admAnio[i].ValorNac + admAnio[i].ValorExt);
                                                                }
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = (perAntNac + totalValorNacMes + totalValorNacAnio), ValorExt = (perAntExt + totalValorExtMes + totalValorExtAnio), ValorTotal = ((perAntNac + totalValorNacMes + totalValorNacAnio) + (perAntExt + totalValorExtMes + totalValorExtAnio)) });
                                                            }
                                                            else if (IfDato0.StartsWith(prefixCont))
                                                            {
                                                                contMes.Add(new Presupuesto.CellValue { Titulo = IfDato0, TituloNac = filaMonedaNacional.IfDato0.ToString(), TituloExt = filaMonedaExtranjera.IfDato0.ToString() });
                                                                decimal perAntNac = stringToDecimal(filaMonedaNacional.IfDato2);
                                                                decimal perAntExt = stringToDecimal(filaMonedaExtranjera.IfDato2);
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = perAntNac, ValorExt = perAntExt });
                                                                //Enero
                                                                decimal facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[0].Value) || decimalIsZero(orientacionComercialTCMesDestino[0].Value)) ? 0 : ((orientacionComercialIPCMesDestino[0].Value / orientacionComercialIPCMesOrigen[0].Value) * (orientacionComercialTCMesOrigen[0].Value / orientacionComercialTCMesDestino[0].Value)));
                                                                decimal facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[0].Value)) ? 0 : (orientacionComercialCPIMesDestino[0].Value / orientacionComercialCPIMesOrigen[0].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato3) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato3) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[1].Value) || decimalIsZero(orientacionComercialTCMesDestino[1].Value)) ? 0 : ((orientacionComercialIPCMesDestino[1].Value / orientacionComercialIPCMesOrigen[1].Value) * (orientacionComercialTCMesOrigen[1].Value / orientacionComercialTCMesDestino[1].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[1].Value)) ? 0 : (orientacionComercialCPIMesDestino[1].Value / orientacionComercialCPIMesOrigen[1].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato4) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato4) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[2].Value) || decimalIsZero(orientacionComercialTCMesDestino[2].Value)) ? 0 : ((orientacionComercialIPCMesDestino[2].Value / orientacionComercialIPCMesOrigen[2].Value) * (orientacionComercialTCMesOrigen[2].Value / orientacionComercialTCMesDestino[2].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[2].Value)) ? 0 : (orientacionComercialCPIMesDestino[2].Value / orientacionComercialCPIMesOrigen[2].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato5) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato5) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[3].Value) || decimalIsZero(orientacionComercialTCMesDestino[3].Value)) ? 0 : ((orientacionComercialIPCMesDestino[3].Value / orientacionComercialIPCMesOrigen[3].Value) * (orientacionComercialTCMesOrigen[3].Value / orientacionComercialTCMesDestino[3].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[3].Value)) ? 0 : (orientacionComercialCPIMesDestino[3].Value / orientacionComercialCPIMesOrigen[3].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato6) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato6) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[4].Value) || decimalIsZero(orientacionComercialTCMesDestino[4].Value)) ? 0 : ((orientacionComercialIPCMesDestino[4].Value / orientacionComercialIPCMesOrigen[4].Value) * (orientacionComercialTCMesOrigen[4].Value / orientacionComercialTCMesDestino[4].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[4].Value)) ? 0 : (orientacionComercialCPIMesDestino[4].Value / orientacionComercialCPIMesOrigen[4].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato7) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato7) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[5].Value) || decimalIsZero(orientacionComercialTCMesDestino[5].Value)) ? 0 : ((orientacionComercialIPCMesDestino[5].Value / orientacionComercialIPCMesOrigen[5].Value) * (orientacionComercialTCMesOrigen[5].Value / orientacionComercialTCMesDestino[5].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[5].Value)) ? 0 : (orientacionComercialCPIMesDestino[5].Value / orientacionComercialCPIMesOrigen[5].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato8) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato8) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[6].Value) || decimalIsZero(orientacionComercialTCMesDestino[6].Value)) ? 0 : ((orientacionComercialIPCMesDestino[6].Value / orientacionComercialIPCMesOrigen[6].Value) * (orientacionComercialTCMesOrigen[6].Value / orientacionComercialTCMesDestino[6].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[6].Value)) ? 0 : (orientacionComercialCPIMesDestino[6].Value / orientacionComercialCPIMesOrigen[6].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato9) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato9) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[7].Value) || decimalIsZero(orientacionComercialTCMesDestino[7].Value)) ? 0 : ((orientacionComercialIPCMesDestino[7].Value / orientacionComercialIPCMesOrigen[7].Value) * (orientacionComercialTCMesOrigen[7].Value / orientacionComercialTCMesDestino[7].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[7].Value)) ? 0 : (orientacionComercialCPIMesDestino[7].Value / orientacionComercialCPIMesOrigen[7].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato10) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato10) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[8].Value) || decimalIsZero(orientacionComercialTCMesDestino[8].Value)) ? 0 : ((orientacionComercialIPCMesDestino[8].Value / orientacionComercialIPCMesOrigen[8].Value) * (orientacionComercialTCMesOrigen[8].Value / orientacionComercialTCMesDestino[8].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[8].Value)) ? 0 : (orientacionComercialCPIMesDestino[8].Value / orientacionComercialCPIMesOrigen[8].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato11) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato11) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[9].Value) || decimalIsZero(orientacionComercialTCMesDestino[9].Value)) ? 0 : ((orientacionComercialIPCMesDestino[9].Value / orientacionComercialIPCMesOrigen[9].Value) * (orientacionComercialTCMesOrigen[9].Value / orientacionComercialTCMesDestino[9].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[9].Value)) ? 0 : (orientacionComercialCPIMesDestino[9].Value / orientacionComercialCPIMesOrigen[9].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato12) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato12) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[10].Value) || decimalIsZero(orientacionComercialTCMesDestino[10].Value)) ? 0 : ((orientacionComercialIPCMesDestino[10].Value / orientacionComercialIPCMesOrigen[10].Value) * (orientacionComercialTCMesOrigen[10].Value / orientacionComercialTCMesDestino[10].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[10].Value)) ? 0 : (orientacionComercialCPIMesDestino[10].Value / orientacionComercialCPIMesOrigen[10].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato13) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato13) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[11].Value) || decimalIsZero(orientacionComercialTCMesDestino[11].Value)) ? 0 : ((orientacionComercialIPCMesDestino[11].Value / orientacionComercialIPCMesOrigen[11].Value) * (orientacionComercialTCMesOrigen[11].Value / orientacionComercialTCMesDestino[11].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[11].Value)) ? 0 : (orientacionComercialCPIMesDestino[11].Value / orientacionComercialCPIMesOrigen[11].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato14) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato14) * facCorrMonExt, ValorTotal = 0 });
                                                                //diciembre
                                                                decimal totalValorNacMes = 0;
                                                                decimal totalValorExtMes = 0;
                                                                for (int i = 2; i < contMes.Count; i++)
                                                                {
                                                                    totalValorNacMes += contMes[i].ValorNac;
                                                                    totalValorExtMes += contMes[i].ValorExt;
                                                                    contMes[i].ValorTotal = (contMes[i].ValorNac + contMes[i].ValorExt);
                                                                }
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = totalValorNacMes, ValorExt = totalValorExtMes, ValorTotal = (totalValorNacMes + totalValorExtMes) });
                                                                //Total meses
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[0].Value) || decimalIsZero(orientacionComercialTCAnioDestino[0].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[0].Value / orientacionComercialIPCAnioOrigen[0].Value) * (orientacionComercialTCAnioOrigen[0].Value / orientacionComercialTCAnioDestino[0].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[0].Value)) ? 0 : (orientacionComercialCPIAnioDestino[0].Value / orientacionComercialCPIAnioOrigen[0].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato16) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato16) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[1].Value) || decimalIsZero(orientacionComercialTCAnioDestino[1].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[1].Value / orientacionComercialIPCAnioOrigen[1].Value) * (orientacionComercialTCAnioOrigen[1].Value / orientacionComercialTCAnioDestino[1].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[1].Value)) ? 0 : (orientacionComercialCPIAnioDestino[1].Value / orientacionComercialCPIAnioOrigen[1].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato17) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato17) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[2].Value) || decimalIsZero(orientacionComercialTCAnioDestino[2].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[2].Value / orientacionComercialIPCAnioOrigen[2].Value) * (orientacionComercialTCAnioOrigen[2].Value / orientacionComercialTCAnioDestino[2].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[2].Value)) ? 0 : (orientacionComercialCPIAnioDestino[2].Value / orientacionComercialCPIAnioOrigen[2].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato18) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato18) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[3].Value) || decimalIsZero(orientacionComercialTCAnioDestino[3].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[3].Value / orientacionComercialIPCAnioOrigen[3].Value) * (orientacionComercialTCAnioOrigen[3].Value / orientacionComercialTCAnioDestino[3].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[3].Value)) ? 0 : (orientacionComercialCPIAnioDestino[3].Value / orientacionComercialCPIAnioOrigen[3].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato19) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato19) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[4].Value) || decimalIsZero(orientacionComercialTCAnioDestino[4].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[4].Value / orientacionComercialIPCAnioOrigen[4].Value) * (orientacionComercialTCAnioOrigen[4].Value / orientacionComercialTCAnioDestino[4].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[4].Value)) ? 0 : (orientacionComercialCPIAnioDestino[4].Value / orientacionComercialCPIAnioOrigen[4].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato20) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato20) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[5].Value) || decimalIsZero(orientacionComercialTCAnioDestino[5].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[5].Value / orientacionComercialIPCAnioOrigen[5].Value) * (orientacionComercialTCAnioOrigen[5].Value / orientacionComercialTCAnioDestino[5].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[5].Value)) ? 0 : (orientacionComercialCPIAnioDestino[5].Value / orientacionComercialCPIAnioOrigen[5].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato21) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato21) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[6].Value) || decimalIsZero(orientacionComercialTCAnioDestino[6].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[6].Value / orientacionComercialIPCAnioOrigen[6].Value) * (orientacionComercialTCAnioOrigen[6].Value / orientacionComercialTCAnioDestino[6].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[6].Value)) ? 0 : (orientacionComercialCPIAnioDestino[6].Value / orientacionComercialCPIAnioOrigen[6].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato22) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato22) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[7].Value) || decimalIsZero(orientacionComercialTCAnioDestino[7].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[7].Value / orientacionComercialIPCAnioOrigen[7].Value) * (orientacionComercialTCAnioOrigen[7].Value / orientacionComercialTCAnioDestino[7].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[7].Value)) ? 0 : (orientacionComercialCPIAnioDestino[7].Value / orientacionComercialCPIAnioOrigen[7].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato23) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato23) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[8].Value) || decimalIsZero(orientacionComercialTCAnioDestino[8].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[8].Value / orientacionComercialIPCAnioOrigen[8].Value) * (orientacionComercialTCAnioOrigen[8].Value / orientacionComercialTCAnioDestino[8].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[8].Value)) ? 0 : (orientacionComercialCPIAnioDestino[8].Value / orientacionComercialCPIAnioOrigen[8].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato24) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato24) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[9].Value) || decimalIsZero(orientacionComercialTCAnioDestino[9].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[9].Value / orientacionComercialIPCAnioOrigen[9].Value) * (orientacionComercialTCAnioOrigen[9].Value / orientacionComercialTCAnioDestino[9].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[9].Value)) ? 0 : (orientacionComercialCPIAnioDestino[9].Value / orientacionComercialCPIAnioOrigen[9].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato25) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato25) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[10].Value) || decimalIsZero(orientacionComercialTCAnioDestino[10].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[10].Value / orientacionComercialIPCAnioOrigen[10].Value) * (orientacionComercialTCAnioOrigen[10].Value / orientacionComercialTCAnioDestino[10].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[10].Value)) ? 0 : (orientacionComercialCPIAnioDestino[10].Value / orientacionComercialCPIAnioOrigen[10].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato26) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato26) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[11].Value) || decimalIsZero(orientacionComercialTCAnioDestino[11].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[11].Value / orientacionComercialIPCAnioOrigen[11].Value) * (orientacionComercialTCAnioOrigen[11].Value / orientacionComercialTCAnioDestino[11].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[11].Value)) ? 0 : (orientacionComercialCPIAnioDestino[11].Value / orientacionComercialCPIAnioOrigen[11].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato27) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato27) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[12].Value) || decimalIsZero(orientacionComercialTCAnioDestino[12].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[12].Value / orientacionComercialIPCAnioOrigen[12].Value) * (orientacionComercialTCAnioOrigen[12].Value / orientacionComercialTCAnioDestino[12].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[12].Value)) ? 0 : (orientacionComercialCPIAnioDestino[12].Value / orientacionComercialCPIAnioOrigen[12].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato28) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato28) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[13].Value) || decimalIsZero(orientacionComercialTCAnioDestino[13].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[13].Value / orientacionComercialIPCAnioOrigen[13].Value) * (orientacionComercialTCAnioOrigen[13].Value / orientacionComercialTCAnioDestino[13].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[13].Value)) ? 0 : (orientacionComercialCPIAnioDestino[13].Value / orientacionComercialCPIAnioOrigen[13].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato29) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato29) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[14].Value) || decimalIsZero(orientacionComercialTCAnioDestino[14].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[14].Value / orientacionComercialIPCAnioOrigen[14].Value) * (orientacionComercialTCAnioOrigen[14].Value / orientacionComercialTCAnioDestino[14].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[14].Value)) ? 0 : (orientacionComercialCPIAnioDestino[14].Value / orientacionComercialCPIAnioOrigen[14].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato30) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato30) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[15].Value) || decimalIsZero(orientacionComercialTCAnioDestino[15].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[15].Value / orientacionComercialIPCAnioOrigen[15].Value) * (orientacionComercialTCAnioOrigen[15].Value / orientacionComercialTCAnioDestino[15].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[15].Value)) ? 0 : (orientacionComercialCPIAnioDestino[15].Value / orientacionComercialCPIAnioOrigen[15].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato31) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato31) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[16].Value) || decimalIsZero(orientacionComercialTCAnioDestino[16].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[16].Value / orientacionComercialIPCAnioOrigen[16].Value) * (orientacionComercialTCAnioOrigen[16].Value / orientacionComercialTCAnioDestino[16].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[16].Value)) ? 0 : (orientacionComercialCPIAnioDestino[16].Value / orientacionComercialCPIAnioOrigen[16].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato32) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato32) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[17].Value) || decimalIsZero(orientacionComercialTCAnioDestino[17].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[17].Value / orientacionComercialIPCAnioOrigen[17].Value) * (orientacionComercialTCAnioOrigen[17].Value / orientacionComercialTCAnioDestino[17].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[17].Value)) ? 0 : (orientacionComercialCPIAnioDestino[17].Value / orientacionComercialCPIAnioOrigen[17].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato33) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato33) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[18].Value) || decimalIsZero(orientacionComercialTCAnioDestino[18].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[18].Value / orientacionComercialIPCAnioOrigen[18].Value) * (orientacionComercialTCAnioOrigen[18].Value / orientacionComercialTCAnioDestino[18].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[18].Value)) ? 0 : (orientacionComercialCPIAnioDestino[18].Value / orientacionComercialCPIAnioOrigen[18].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato34) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato34) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[19].Value) || decimalIsZero(orientacionComercialTCAnioDestino[19].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[19].Value / orientacionComercialIPCAnioOrigen[19].Value) * (orientacionComercialTCAnioOrigen[19].Value / orientacionComercialTCAnioDestino[19].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[19].Value)) ? 0 : (orientacionComercialCPIAnioDestino[19].Value / orientacionComercialCPIAnioOrigen[19].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato35) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato35) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[20].Value) || decimalIsZero(orientacionComercialTCAnioDestino[20].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[20].Value / orientacionComercialIPCAnioOrigen[20].Value) * (orientacionComercialTCAnioOrigen[20].Value / orientacionComercialTCAnioDestino[20].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[20].Value)) ? 0 : (orientacionComercialCPIAnioDestino[20].Value / orientacionComercialCPIAnioOrigen[20].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato36) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato36) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[21].Value) || decimalIsZero(orientacionComercialTCAnioDestino[21].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[21].Value / orientacionComercialIPCAnioOrigen[21].Value) * (orientacionComercialTCAnioOrigen[21].Value / orientacionComercialTCAnioDestino[21].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[21].Value)) ? 0 : (orientacionComercialCPIAnioDestino[21].Value / orientacionComercialCPIAnioOrigen[21].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato37) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato37) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[22].Value) || decimalIsZero(orientacionComercialTCAnioDestino[22].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[22].Value / orientacionComercialIPCAnioOrigen[22].Value) * (orientacionComercialTCAnioOrigen[22].Value / orientacionComercialTCAnioDestino[22].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[22].Value)) ? 0 : (orientacionComercialCPIAnioDestino[22].Value / orientacionComercialCPIAnioOrigen[22].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato38) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato38) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[23].Value) || decimalIsZero(orientacionComercialTCAnioDestino[23].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[23].Value / orientacionComercialIPCAnioOrigen[23].Value) * (orientacionComercialTCAnioOrigen[23].Value / orientacionComercialTCAnioDestino[23].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[23].Value)) ? 0 : (orientacionComercialCPIAnioDestino[23].Value / orientacionComercialCPIAnioOrigen[23].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato39) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato39) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[24].Value) || decimalIsZero(orientacionComercialTCAnioDestino[24].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[24].Value / orientacionComercialIPCAnioOrigen[24].Value) * (orientacionComercialTCAnioOrigen[24].Value / orientacionComercialTCAnioDestino[24].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[24].Value)) ? 0 : (orientacionComercialCPIAnioDestino[24].Value / orientacionComercialCPIAnioOrigen[24].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato40) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato40) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[25].Value) || decimalIsZero(orientacionComercialTCAnioDestino[25].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[25].Value / orientacionComercialIPCAnioOrigen[25].Value) * (orientacionComercialTCAnioOrigen[25].Value / orientacionComercialTCAnioDestino[25].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[25].Value)) ? 0 : (orientacionComercialCPIAnioDestino[25].Value / orientacionComercialCPIAnioOrigen[25].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato41) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato41) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[26].Value) || decimalIsZero(orientacionComercialTCAnioDestino[26].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[26].Value / orientacionComercialIPCAnioOrigen[26].Value) * (orientacionComercialTCAnioOrigen[26].Value / orientacionComercialTCAnioDestino[26].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[26].Value)) ? 0 : (orientacionComercialCPIAnioDestino[26].Value / orientacionComercialCPIAnioOrigen[26].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato42) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato42) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[27].Value) || decimalIsZero(orientacionComercialTCAnioDestino[27].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[27].Value / orientacionComercialIPCAnioOrigen[27].Value) * (orientacionComercialTCAnioOrigen[27].Value / orientacionComercialTCAnioDestino[27].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[27].Value)) ? 0 : (orientacionComercialCPIAnioDestino[27].Value / orientacionComercialCPIAnioOrigen[27].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato43) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato43) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[28].Value) || decimalIsZero(orientacionComercialTCAnioDestino[28].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[28].Value / orientacionComercialIPCAnioOrigen[28].Value) * (orientacionComercialTCAnioOrigen[28].Value / orientacionComercialTCAnioDestino[28].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[28].Value)) ? 0 : (orientacionComercialCPIAnioDestino[28].Value / orientacionComercialCPIAnioOrigen[28].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato44) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato44) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[29].Value) || decimalIsZero(orientacionComercialTCAnioDestino[29].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[29].Value / orientacionComercialIPCAnioOrigen[29].Value) * (orientacionComercialTCAnioOrigen[29].Value / orientacionComercialTCAnioDestino[29].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[29].Value)) ? 0 : (orientacionComercialCPIAnioDestino[29].Value / orientacionComercialCPIAnioOrigen[29].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato45) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato45) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[30].Value) || decimalIsZero(orientacionComercialTCAnioDestino[30].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[30].Value / orientacionComercialIPCAnioOrigen[30].Value) * (orientacionComercialTCAnioOrigen[30].Value / orientacionComercialTCAnioDestino[30].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[30].Value)) ? 0 : (orientacionComercialCPIAnioDestino[30].Value / orientacionComercialCPIAnioOrigen[30].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato46) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato46) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[31].Value) || decimalIsZero(orientacionComercialTCAnioDestino[31].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[31].Value / orientacionComercialIPCAnioOrigen[31].Value) * (orientacionComercialTCAnioOrigen[31].Value / orientacionComercialTCAnioDestino[31].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[31].Value)) ? 0 : (orientacionComercialCPIAnioDestino[31].Value / orientacionComercialCPIAnioOrigen[31].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato47) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato47) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[32].Value) || decimalIsZero(orientacionComercialTCAnioDestino[32].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[32].Value / orientacionComercialIPCAnioOrigen[32].Value) * (orientacionComercialTCAnioOrigen[32].Value / orientacionComercialTCAnioDestino[32].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[32].Value)) ? 0 : (orientacionComercialCPIAnioDestino[32].Value / orientacionComercialCPIAnioOrigen[32].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato48) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato48) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[33].Value) || decimalIsZero(orientacionComercialTCAnioDestino[33].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[33].Value / orientacionComercialIPCAnioOrigen[33].Value) * (orientacionComercialTCAnioOrigen[33].Value / orientacionComercialTCAnioDestino[33].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[33].Value)) ? 0 : (orientacionComercialCPIAnioDestino[33].Value / orientacionComercialCPIAnioOrigen[33].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato49) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato49) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[34].Value) || decimalIsZero(orientacionComercialTCAnioDestino[34].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[34].Value / orientacionComercialIPCAnioOrigen[34].Value) * (orientacionComercialTCAnioOrigen[34].Value / orientacionComercialTCAnioDestino[34].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[34].Value)) ? 0 : (orientacionComercialCPIAnioDestino[34].Value / orientacionComercialCPIAnioOrigen[34].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato50) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato50) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[35].Value) || decimalIsZero(orientacionComercialTCAnioDestino[35].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[35].Value / orientacionComercialIPCAnioOrigen[35].Value) * (orientacionComercialTCAnioOrigen[35].Value / orientacionComercialTCAnioDestino[35].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[35].Value)) ? 0 : (orientacionComercialCPIAnioDestino[35].Value / orientacionComercialCPIAnioOrigen[35].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato51) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato51) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[36].Value) || decimalIsZero(orientacionComercialTCAnioDestino[36].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[36].Value / orientacionComercialIPCAnioOrigen[36].Value) * (orientacionComercialTCAnioOrigen[36].Value / orientacionComercialTCAnioDestino[36].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[36].Value)) ? 0 : (orientacionComercialCPIAnioDestino[36].Value / orientacionComercialCPIAnioOrigen[36].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato52) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato52) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[37].Value) || decimalIsZero(orientacionComercialTCAnioDestino[37].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[37].Value / orientacionComercialIPCAnioOrigen[37].Value) * (orientacionComercialTCAnioOrigen[37].Value / orientacionComercialTCAnioDestino[37].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[37].Value)) ? 0 : (orientacionComercialCPIAnioDestino[37].Value / orientacionComercialCPIAnioOrigen[37].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato53) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato53) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[38].Value) || decimalIsZero(orientacionComercialTCAnioDestino[38].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[38].Value / orientacionComercialIPCAnioOrigen[38].Value) * (orientacionComercialTCAnioOrigen[38].Value / orientacionComercialTCAnioDestino[38].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[38].Value)) ? 0 : (orientacionComercialCPIAnioDestino[38].Value / orientacionComercialCPIAnioOrigen[38].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato54) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato54) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[39].Value) || decimalIsZero(orientacionComercialTCAnioDestino[39].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[39].Value / orientacionComercialIPCAnioOrigen[39].Value) * (orientacionComercialTCAnioOrigen[39].Value / orientacionComercialTCAnioDestino[39].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[39].Value)) ? 0 : (orientacionComercialCPIAnioDestino[39].Value / orientacionComercialCPIAnioOrigen[39].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato55) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato55) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[40].Value) || decimalIsZero(orientacionComercialTCAnioDestino[40].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[40].Value / orientacionComercialIPCAnioOrigen[40].Value) * (orientacionComercialTCAnioOrigen[40].Value / orientacionComercialTCAnioDestino[40].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[40].Value)) ? 0 : (orientacionComercialCPIAnioDestino[40].Value / orientacionComercialCPIAnioOrigen[40].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato56) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato56) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[41].Value) || decimalIsZero(orientacionComercialTCAnioDestino[41].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[41].Value / orientacionComercialIPCAnioOrigen[41].Value) * (orientacionComercialTCAnioOrigen[41].Value / orientacionComercialTCAnioDestino[41].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[41].Value)) ? 0 : (orientacionComercialCPIAnioDestino[41].Value / orientacionComercialCPIAnioOrigen[41].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato57) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato57) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[42].Value) || decimalIsZero(orientacionComercialTCAnioDestino[42].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[42].Value / orientacionComercialIPCAnioOrigen[42].Value) * (orientacionComercialTCAnioOrigen[42].Value / orientacionComercialTCAnioDestino[42].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[42].Value)) ? 0 : (orientacionComercialCPIAnioDestino[42].Value / orientacionComercialCPIAnioOrigen[42].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato58) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato58) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[43].Value) || decimalIsZero(orientacionComercialTCAnioDestino[43].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[43].Value / orientacionComercialIPCAnioOrigen[43].Value) * (orientacionComercialTCAnioOrigen[43].Value / orientacionComercialTCAnioDestino[43].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[43].Value)) ? 0 : (orientacionComercialCPIAnioDestino[43].Value / orientacionComercialCPIAnioOrigen[43].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato59) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato59) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[44].Value) || decimalIsZero(orientacionComercialTCAnioDestino[44].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[44].Value / orientacionComercialIPCAnioOrigen[44].Value) * (orientacionComercialTCAnioOrigen[44].Value / orientacionComercialTCAnioDestino[44].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[44].Value)) ? 0 : (orientacionComercialCPIAnioDestino[44].Value / orientacionComercialCPIAnioOrigen[44].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato60) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato60) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[45].Value) || decimalIsZero(orientacionComercialTCAnioDestino[45].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[45].Value / orientacionComercialIPCAnioOrigen[45].Value) * (orientacionComercialTCAnioOrigen[45].Value / orientacionComercialTCAnioDestino[45].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[45].Value)) ? 0 : (orientacionComercialCPIAnioDestino[45].Value / orientacionComercialCPIAnioOrigen[45].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato61) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato61) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[46].Value) || decimalIsZero(orientacionComercialTCAnioDestino[46].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[46].Value / orientacionComercialIPCAnioOrigen[46].Value) * (orientacionComercialTCAnioOrigen[46].Value / orientacionComercialTCAnioDestino[46].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[46].Value)) ? 0 : (orientacionComercialCPIAnioDestino[46].Value / orientacionComercialCPIAnioOrigen[46].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato62) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato62) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[47].Value) || decimalIsZero(orientacionComercialTCAnioDestino[47].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[47].Value / orientacionComercialIPCAnioOrigen[47].Value) * (orientacionComercialTCAnioOrigen[47].Value / orientacionComercialTCAnioDestino[47].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[47].Value)) ? 0 : (orientacionComercialCPIAnioDestino[47].Value / orientacionComercialCPIAnioOrigen[47].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato63) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato63) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[48].Value) || decimalIsZero(orientacionComercialTCAnioDestino[48].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[48].Value / orientacionComercialIPCAnioOrigen[48].Value) * (orientacionComercialTCAnioOrigen[48].Value / orientacionComercialTCAnioDestino[48].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[48].Value)) ? 0 : (orientacionComercialCPIAnioDestino[48].Value / orientacionComercialCPIAnioOrigen[48].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato64) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato64) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[49].Value) || decimalIsZero(orientacionComercialTCAnioDestino[49].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[49].Value / orientacionComercialIPCAnioOrigen[49].Value) * (orientacionComercialTCAnioOrigen[49].Value / orientacionComercialTCAnioDestino[49].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[49].Value)) ? 0 : (orientacionComercialCPIAnioDestino[49].Value / orientacionComercialCPIAnioOrigen[49].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato65) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato65) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[50].Value) || decimalIsZero(orientacionComercialTCAnioDestino[50].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[50].Value / orientacionComercialIPCAnioOrigen[50].Value) * (orientacionComercialTCAnioOrigen[50].Value / orientacionComercialTCAnioDestino[50].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[50].Value)) ? 0 : (orientacionComercialCPIAnioDestino[50].Value / orientacionComercialCPIAnioOrigen[50].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato66) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato66) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[51].Value) || decimalIsZero(orientacionComercialTCAnioDestino[51].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[51].Value / orientacionComercialIPCAnioOrigen[51].Value) * (orientacionComercialTCAnioOrigen[51].Value / orientacionComercialTCAnioDestino[51].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[51].Value)) ? 0 : (orientacionComercialCPIAnioDestino[51].Value / orientacionComercialCPIAnioOrigen[51].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato67) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato67) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[52].Value) || decimalIsZero(orientacionComercialTCAnioDestino[52].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[52].Value / orientacionComercialIPCAnioOrigen[52].Value) * (orientacionComercialTCAnioOrigen[52].Value / orientacionComercialTCAnioDestino[52].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[52].Value)) ? 0 : (orientacionComercialCPIAnioDestino[52].Value / orientacionComercialCPIAnioOrigen[52].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato68) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato68) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[53].Value) || decimalIsZero(orientacionComercialTCAnioDestino[53].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[53].Value / orientacionComercialIPCAnioOrigen[53].Value) * (orientacionComercialTCAnioOrigen[53].Value / orientacionComercialTCAnioDestino[53].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[53].Value)) ? 0 : (orientacionComercialCPIAnioDestino[53].Value / orientacionComercialCPIAnioOrigen[53].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato69) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato69) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[54].Value) || decimalIsZero(orientacionComercialTCAnioDestino[54].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[54].Value / orientacionComercialIPCAnioOrigen[54].Value) * (orientacionComercialTCAnioOrigen[54].Value / orientacionComercialTCAnioDestino[54].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[54].Value)) ? 0 : (orientacionComercialCPIAnioDestino[54].Value / orientacionComercialCPIAnioOrigen[54].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato70) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato70) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[55].Value) || decimalIsZero(orientacionComercialTCAnioDestino[55].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[55].Value / orientacionComercialIPCAnioOrigen[55].Value) * (orientacionComercialTCAnioOrigen[55].Value / orientacionComercialTCAnioDestino[55].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[55].Value)) ? 0 : (orientacionComercialCPIAnioDestino[55].Value / orientacionComercialCPIAnioOrigen[55].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato71) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato71) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[56].Value) || decimalIsZero(orientacionComercialTCAnioDestino[56].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[56].Value / orientacionComercialIPCAnioOrigen[56].Value) * (orientacionComercialTCAnioOrigen[56].Value / orientacionComercialTCAnioDestino[56].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[56].Value)) ? 0 : (orientacionComercialCPIAnioDestino[56].Value / orientacionComercialCPIAnioOrigen[56].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato72) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato72) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[57].Value) || decimalIsZero(orientacionComercialTCAnioDestino[57].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[57].Value / orientacionComercialIPCAnioOrigen[57].Value) * (orientacionComercialTCAnioOrigen[57].Value / orientacionComercialTCAnioDestino[57].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[57].Value)) ? 0 : (orientacionComercialCPIAnioDestino[57].Value / orientacionComercialCPIAnioOrigen[57].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato73) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato73) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[58].Value) || decimalIsZero(orientacionComercialTCAnioDestino[58].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[58].Value / orientacionComercialIPCAnioOrigen[58].Value) * (orientacionComercialTCAnioOrigen[58].Value / orientacionComercialTCAnioDestino[58].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[58].Value)) ? 0 : (orientacionComercialCPIAnioDestino[58].Value / orientacionComercialCPIAnioOrigen[58].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato74) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato74) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[59].Value) || decimalIsZero(orientacionComercialTCAnioDestino[59].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[59].Value / orientacionComercialIPCAnioOrigen[59].Value) * (orientacionComercialTCAnioOrigen[59].Value / orientacionComercialTCAnioDestino[59].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[59].Value)) ? 0 : (orientacionComercialCPIAnioDestino[59].Value / orientacionComercialCPIAnioOrigen[59].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato75) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato75) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[60].Value) || decimalIsZero(orientacionComercialTCAnioDestino[60].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[60].Value / orientacionComercialIPCAnioOrigen[60].Value) * (orientacionComercialTCAnioOrigen[60].Value / orientacionComercialTCAnioDestino[60].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[60].Value)) ? 0 : (orientacionComercialCPIAnioDestino[60].Value / orientacionComercialCPIAnioOrigen[60].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato76) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato76) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[61].Value) || decimalIsZero(orientacionComercialTCAnioDestino[61].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[61].Value / orientacionComercialIPCAnioOrigen[61].Value) * (orientacionComercialTCAnioOrigen[61].Value / orientacionComercialTCAnioDestino[61].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[61].Value)) ? 0 : (orientacionComercialCPIAnioDestino[61].Value / orientacionComercialCPIAnioOrigen[61].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato77) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato77) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[62].Value) || decimalIsZero(orientacionComercialTCAnioDestino[62].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[62].Value / orientacionComercialIPCAnioOrigen[62].Value) * (orientacionComercialTCAnioOrigen[62].Value / orientacionComercialTCAnioDestino[62].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[62].Value)) ? 0 : (orientacionComercialCPIAnioDestino[62].Value / orientacionComercialCPIAnioOrigen[62].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato78) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato78) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[63].Value) || decimalIsZero(orientacionComercialTCAnioDestino[63].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[63].Value / orientacionComercialIPCAnioOrigen[63].Value) * (orientacionComercialTCAnioOrigen[63].Value / orientacionComercialTCAnioDestino[63].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[63].Value)) ? 0 : (orientacionComercialCPIAnioDestino[63].Value / orientacionComercialCPIAnioOrigen[63].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato79) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato79) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[64].Value) || decimalIsZero(orientacionComercialTCAnioDestino[64].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[64].Value / orientacionComercialIPCAnioOrigen[64].Value) * (orientacionComercialTCAnioOrigen[64].Value / orientacionComercialTCAnioDestino[64].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[64].Value)) ? 0 : (orientacionComercialCPIAnioDestino[64].Value / orientacionComercialCPIAnioOrigen[64].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato80) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato80) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[65].Value) || decimalIsZero(orientacionComercialTCAnioDestino[65].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[65].Value / orientacionComercialIPCAnioOrigen[65].Value) * (orientacionComercialTCAnioOrigen[65].Value / orientacionComercialTCAnioDestino[65].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[65].Value)) ? 0 : (orientacionComercialCPIAnioDestino[65].Value / orientacionComercialCPIAnioOrigen[65].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato81) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato81) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[66].Value) || decimalIsZero(orientacionComercialTCAnioDestino[66].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[66].Value / orientacionComercialIPCAnioOrigen[66].Value) * (orientacionComercialTCAnioOrigen[66].Value / orientacionComercialTCAnioDestino[66].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[66].Value)) ? 0 : (orientacionComercialCPIAnioDestino[66].Value / orientacionComercialCPIAnioOrigen[66].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato82) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato82) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[67].Value) || decimalIsZero(orientacionComercialTCAnioDestino[67].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[67].Value / orientacionComercialIPCAnioOrigen[67].Value) * (orientacionComercialTCAnioOrigen[67].Value / orientacionComercialTCAnioDestino[67].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[67].Value)) ? 0 : (orientacionComercialCPIAnioDestino[67].Value / orientacionComercialCPIAnioOrigen[67].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato83) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato83) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[68].Value) || decimalIsZero(orientacionComercialTCAnioDestino[68].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[68].Value / orientacionComercialIPCAnioOrigen[68].Value) * (orientacionComercialTCAnioOrigen[68].Value / orientacionComercialTCAnioDestino[68].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[68].Value)) ? 0 : (orientacionComercialCPIAnioDestino[68].Value / orientacionComercialCPIAnioOrigen[68].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato84) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato84) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[69].Value) || decimalIsZero(orientacionComercialTCAnioDestino[69].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[69].Value / orientacionComercialIPCAnioOrigen[69].Value) * (orientacionComercialTCAnioOrigen[69].Value / orientacionComercialTCAnioDestino[69].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[69].Value)) ? 0 : (orientacionComercialCPIAnioDestino[69].Value / orientacionComercialCPIAnioOrigen[69].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato85) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato85) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[70].Value) || decimalIsZero(orientacionComercialTCAnioDestino[70].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[70].Value / orientacionComercialIPCAnioOrigen[70].Value) * (orientacionComercialTCAnioOrigen[70].Value / orientacionComercialTCAnioDestino[70].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[70].Value)) ? 0 : (orientacionComercialCPIAnioDestino[70].Value / orientacionComercialCPIAnioOrigen[70].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato86) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato86) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[71].Value) || decimalIsZero(orientacionComercialTCAnioDestino[71].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[71].Value / orientacionComercialIPCAnioOrigen[71].Value) * (orientacionComercialTCAnioOrigen[71].Value / orientacionComercialTCAnioDestino[71].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[71].Value)) ? 0 : (orientacionComercialCPIAnioDestino[71].Value / orientacionComercialCPIAnioOrigen[71].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato87) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato87) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[72].Value) || decimalIsZero(orientacionComercialTCAnioDestino[72].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[72].Value / orientacionComercialIPCAnioOrigen[72].Value) * (orientacionComercialTCAnioOrigen[72].Value / orientacionComercialTCAnioDestino[72].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[72].Value)) ? 0 : (orientacionComercialCPIAnioDestino[72].Value / orientacionComercialCPIAnioOrigen[72].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato88) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato88) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[73].Value) || decimalIsZero(orientacionComercialTCAnioDestino[73].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[73].Value / orientacionComercialIPCAnioOrigen[73].Value) * (orientacionComercialTCAnioOrigen[73].Value / orientacionComercialTCAnioDestino[73].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[73].Value)) ? 0 : (orientacionComercialCPIAnioDestino[73].Value / orientacionComercialCPIAnioOrigen[73].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato89) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato89) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[74].Value) || decimalIsZero(orientacionComercialTCAnioDestino[74].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[74].Value / orientacionComercialIPCAnioOrigen[74].Value) * (orientacionComercialTCAnioOrigen[74].Value / orientacionComercialTCAnioDestino[74].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[74].Value)) ? 0 : (orientacionComercialCPIAnioDestino[74].Value / orientacionComercialCPIAnioOrigen[74].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato90) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato90) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[75].Value) || decimalIsZero(orientacionComercialTCAnioDestino[75].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[75].Value / orientacionComercialIPCAnioOrigen[75].Value) * (orientacionComercialTCAnioOrigen[75].Value / orientacionComercialTCAnioDestino[75].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[75].Value)) ? 0 : (orientacionComercialCPIAnioDestino[75].Value / orientacionComercialCPIAnioOrigen[75].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato91) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato91) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[76].Value) || decimalIsZero(orientacionComercialTCAnioDestino[76].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[76].Value / orientacionComercialIPCAnioOrigen[76].Value) * (orientacionComercialTCAnioOrigen[76].Value / orientacionComercialTCAnioDestino[76].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[76].Value)) ? 0 : (orientacionComercialCPIAnioDestino[76].Value / orientacionComercialCPIAnioOrigen[76].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato92) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato92) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[77].Value) || decimalIsZero(orientacionComercialTCAnioDestino[77].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[77].Value / orientacionComercialIPCAnioOrigen[77].Value) * (orientacionComercialTCAnioOrigen[77].Value / orientacionComercialTCAnioDestino[77].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[77].Value)) ? 0 : (orientacionComercialCPIAnioDestino[77].Value / orientacionComercialCPIAnioOrigen[77].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato93) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato93) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[78].Value) || decimalIsZero(orientacionComercialTCAnioDestino[78].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[78].Value / orientacionComercialIPCAnioOrigen[78].Value) * (orientacionComercialTCAnioOrigen[78].Value / orientacionComercialTCAnioDestino[78].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[78].Value)) ? 0 : (orientacionComercialCPIAnioDestino[78].Value / orientacionComercialCPIAnioOrigen[78].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato94) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato94) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[79].Value) || decimalIsZero(orientacionComercialTCAnioDestino[79].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[79].Value / orientacionComercialIPCAnioOrigen[79].Value) * (orientacionComercialTCAnioOrigen[79].Value / orientacionComercialTCAnioDestino[79].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[79].Value)) ? 0 : (orientacionComercialCPIAnioDestino[79].Value / orientacionComercialCPIAnioOrigen[79].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDato95) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDato95) * facCorrMonExt, ValorTotal = 0 });
                                                                decimal totalValorNacAnio = 0;
                                                                decimal totalValorExtAnio = 0;
                                                                for (int i = 0; i < contAnio.Count; i++)
                                                                {
                                                                    totalValorNacAnio += contAnio[i].ValorNac;
                                                                    totalValorExtAnio += contAnio[i].ValorExt;
                                                                    contAnio[i].ValorTotal = (contAnio[i].ValorNac + contAnio[i].ValorExt);
                                                                }
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = (perAntNac + totalValorNacMes + totalValorNacAnio), ValorExt = (perAntExt + totalValorExtMes + totalValorExtAnio), ValorTotal = ((perAntNac + totalValorNacMes + totalValorNacAnio) + (perAntExt + totalValorExtMes + totalValorExtAnio)) });
                                                            }
                                                            else if (IfDato0.StartsWith(prefixTP))
                                                            {
                                                                totParMes.Add(new Presupuesto.CellValue { Titulo = IfDato0, TituloNac = filaMonedaNacional.IfDato0.ToString(), TituloExt = filaMonedaExtranjera.IfDato0.ToString() });
                                                            }
                                                        }
                                                    }
                                                    else // presupuesto
                                                    {

                                                        IList<Presupuesto.FinancieroDetallePresupuesto> informacionFinancieroDetalleParametroVNOrigen = SqlMapper.Query<Presupuesto.FinancieroDetallePresupuesto>(objConnection, "CAPEX_SEL_IMPORTACION_FINANCIERO_DETALLE_PARAMETROVN", new { TipoIniciativa = tipoIniciativaOrientacionComercial, IfToken = IfToken }, commandType: CommandType.StoredProcedure).ToList();
                                                        if (IfDato0.StartsWith(prefixTA) && (informacionFinancieroDetalleParametroVNOrigen == null || informacionFinancieroDetalleParametroVNOrigen.Count == 0))
                                                        {
                                                            totAcumMes.Add(new Presupuesto.CellValue { Titulo = IfDato0 });
                                                        }
                                                        else if (informacionFinancieroDetalleParametroVNOrigen != null && informacionFinancieroDetalleParametroVNOrigen.Count == 2)
                                                        {
                                                            string IfDato0D = informacionFinancieroDetalleParametroVNOrigen[0].IfDDato0.ToString();
                                                            Presupuesto.FinancieroDetallePresupuesto filaMonedaNacional = null;
                                                            Presupuesto.FinancieroDetallePresupuesto filaMonedaExtranjera = null;
                                                            if (IfDato0D.StartsWith(prefixMonNac))
                                                            {
                                                                filaMonedaNacional = informacionFinancieroDetalleParametroVNOrigen[0];
                                                                filaMonedaExtranjera = informacionFinancieroDetalleParametroVNOrigen[1];
                                                            }
                                                            else if (IfDato0D.StartsWith(prefixMonExt))
                                                            {
                                                                filaMonedaNacional = informacionFinancieroDetalleParametroVNOrigen[1];
                                                                filaMonedaExtranjera = informacionFinancieroDetalleParametroVNOrigen[0];
                                                            }
                                                            if (IfDato0.StartsWith(prefixIng))
                                                            {
                                                                decimal perAntNac = stringToDecimal(filaMonedaNacional.IfDDato2.ToString());
                                                                decimal perAntExt = stringToDecimal(filaMonedaExtranjera.IfDDato2.ToString());
                                                                ingMes.Add(new Presupuesto.CellValue { Titulo = IfDato0, TituloNac = filaMonedaNacional.IfDDato0.ToString(), TituloExt = filaMonedaExtranjera.IfDDato0.ToString() });
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = perAntNac, ValorExt = perAntExt, ValorTotal = perAntTotal });
                                                                //Enero
                                                                decimal facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[0].Value) || decimalIsZero(orientacionComercialTCMesDestino[0].Value)) ? 0 : ((orientacionComercialIPCMesDestino[0].Value / orientacionComercialIPCMesOrigen[0].Value) * (orientacionComercialTCMesOrigen[0].Value / orientacionComercialTCMesDestino[0].Value)));
                                                                decimal facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[0].Value)) ? 0 : (orientacionComercialCPIMesDestino[0].Value / orientacionComercialCPIMesOrigen[0].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato3) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato3) * facCorrMonExt, ValorTotal = 0 });
                                                                //Febrero
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[1].Value) || decimalIsZero(orientacionComercialTCMesDestino[1].Value)) ? 0 : ((orientacionComercialIPCMesDestino[1].Value / orientacionComercialIPCMesOrigen[1].Value) * (orientacionComercialTCMesOrigen[1].Value / orientacionComercialTCMesDestino[1].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[1].Value)) ? 0 : (orientacionComercialCPIMesDestino[1].Value / orientacionComercialCPIMesOrigen[1].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato4) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato4) * facCorrMonExt, ValorTotal = 0 });
                                                                //Marzo
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[2].Value) || decimalIsZero(orientacionComercialTCMesDestino[2].Value)) ? 0 : ((orientacionComercialIPCMesDestino[2].Value / orientacionComercialIPCMesOrigen[2].Value) * (orientacionComercialTCMesOrigen[2].Value / orientacionComercialTCMesDestino[2].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[2].Value)) ? 0 : (orientacionComercialCPIMesDestino[2].Value / orientacionComercialCPIMesOrigen[2].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato5) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato5) * facCorrMonExt, ValorTotal = 0 });
                                                                //Abril
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[3].Value) || decimalIsZero(orientacionComercialTCMesDestino[3].Value)) ? 0 : ((orientacionComercialIPCMesDestino[3].Value / orientacionComercialIPCMesOrigen[3].Value) * (orientacionComercialTCMesOrigen[3].Value / orientacionComercialTCMesDestino[3].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[3].Value)) ? 0 : (orientacionComercialCPIMesDestino[3].Value / orientacionComercialCPIMesOrigen[3].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato6) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato6) * facCorrMonExt, ValorTotal = 0 });
                                                                //Mayo
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[4].Value) || decimalIsZero(orientacionComercialTCMesDestino[4].Value)) ? 0 : ((orientacionComercialIPCMesDestino[4].Value / orientacionComercialIPCMesOrigen[4].Value) * (orientacionComercialTCMesOrigen[4].Value / orientacionComercialTCMesDestino[4].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[4].Value)) ? 0 : (orientacionComercialCPIMesDestino[4].Value / orientacionComercialCPIMesOrigen[4].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato7) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato7) * facCorrMonExt, ValorTotal = 0 });
                                                                //Junio
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[5].Value) || decimalIsZero(orientacionComercialTCMesDestino[5].Value)) ? 0 : ((orientacionComercialIPCMesDestino[5].Value / orientacionComercialIPCMesOrigen[5].Value) * (orientacionComercialTCMesOrigen[5].Value / orientacionComercialTCMesDestino[5].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[5].Value)) ? 0 : (orientacionComercialCPIMesDestino[5].Value / orientacionComercialCPIMesOrigen[5].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato8) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato8) * facCorrMonExt, ValorTotal = 0 });
                                                                //Julio
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[6].Value) || decimalIsZero(orientacionComercialTCMesDestino[6].Value)) ? 0 : ((orientacionComercialIPCMesDestino[6].Value / orientacionComercialIPCMesOrigen[6].Value) * (orientacionComercialTCMesOrigen[6].Value / orientacionComercialTCMesDestino[6].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[6].Value)) ? 0 : (orientacionComercialCPIMesDestino[6].Value / orientacionComercialCPIMesOrigen[6].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato9) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato9) * facCorrMonExt, ValorTotal = 0 });
                                                                //Agosto
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[7].Value) || decimalIsZero(orientacionComercialTCMesDestino[7].Value)) ? 0 : ((orientacionComercialIPCMesDestino[7].Value / orientacionComercialIPCMesOrigen[7].Value) * (orientacionComercialTCMesOrigen[7].Value / orientacionComercialTCMesDestino[7].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[7].Value)) ? 0 : (orientacionComercialCPIMesDestino[7].Value / orientacionComercialCPIMesOrigen[7].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato10) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato10) * facCorrMonExt, ValorTotal = 0 });
                                                                //Septiembre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[8].Value) || decimalIsZero(orientacionComercialTCMesDestino[8].Value)) ? 0 : ((orientacionComercialIPCMesDestino[8].Value / orientacionComercialIPCMesOrigen[8].Value) * (orientacionComercialTCMesOrigen[8].Value / orientacionComercialTCMesDestino[8].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[8].Value)) ? 0 : (orientacionComercialCPIMesDestino[8].Value / orientacionComercialCPIMesOrigen[8].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato11) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato11) * facCorrMonExt, ValorTotal = 0 });
                                                                //Octubre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[9].Value) || decimalIsZero(orientacionComercialTCMesDestino[9].Value)) ? 0 : ((orientacionComercialIPCMesDestino[9].Value / orientacionComercialIPCMesOrigen[9].Value) * (orientacionComercialTCMesOrigen[9].Value / orientacionComercialTCMesDestino[9].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[9].Value)) ? 0 : (orientacionComercialCPIMesDestino[9].Value / orientacionComercialCPIMesOrigen[9].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato12) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato12) * facCorrMonExt, ValorTotal = 0 });
                                                                //Noviembre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[10].Value) || decimalIsZero(orientacionComercialTCMesDestino[10].Value)) ? 0 : ((orientacionComercialIPCMesDestino[10].Value / orientacionComercialIPCMesOrigen[10].Value) * (orientacionComercialTCMesOrigen[10].Value / orientacionComercialTCMesDestino[10].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[10].Value)) ? 0 : (orientacionComercialCPIMesDestino[10].Value / orientacionComercialCPIMesOrigen[10].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato13) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato13) * facCorrMonExt, ValorTotal = 0 });
                                                                //Diciembre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[11].Value) || decimalIsZero(orientacionComercialTCMesDestino[11].Value)) ? 0 : ((orientacionComercialIPCMesDestino[11].Value / orientacionComercialIPCMesOrigen[11].Value) * (orientacionComercialTCMesOrigen[11].Value / orientacionComercialTCMesDestino[11].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[11].Value)) ? 0 : (orientacionComercialCPIMesDestino[11].Value / orientacionComercialCPIMesOrigen[11].Value));
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato14) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato14) * facCorrMonExt, ValorTotal = 0 });
                                                                //Calculo Totales por Mes
                                                                decimal totalValorNacMes = 0;
                                                                decimal totalValorExtMes = 0;
                                                                for (int i = 2; i < ingMes.Count; i++)
                                                                {
                                                                    totalValorNacMes += ingMes[i].ValorNac;
                                                                    totalValorExtMes += ingMes[i].ValorExt;
                                                                    ingMes[i].ValorTotal = (ingMes[i].ValorNac + ingMes[i].ValorExt);
                                                                }
                                                                //Total
                                                                ingMes.Add(new Presupuesto.CellValue { ValorNac = totalValorNacMes, ValorExt = totalValorExtMes, ValorTotal = (totalValorNacMes + totalValorExtMes) });
                                                                //AñoMas1
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[0].Value) || decimalIsZero(orientacionComercialTCAnioDestino[0].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[0].Value / orientacionComercialIPCAnioOrigen[0].Value) * (orientacionComercialTCAnioOrigen[0].Value / orientacionComercialTCAnioDestino[0].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[0].Value)) ? 0 : (orientacionComercialCPIAnioDestino[0].Value / orientacionComercialCPIAnioOrigen[0].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato16) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato16) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas2
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[1].Value) || decimalIsZero(orientacionComercialTCAnioDestino[1].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[1].Value / orientacionComercialIPCAnioOrigen[1].Value) * (orientacionComercialTCAnioOrigen[1].Value / orientacionComercialTCAnioDestino[1].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[1].Value)) ? 0 : (orientacionComercialCPIAnioDestino[1].Value / orientacionComercialCPIAnioOrigen[1].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato17) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato17) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas3
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[2].Value) || decimalIsZero(orientacionComercialTCAnioDestino[2].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[2].Value / orientacionComercialIPCAnioOrigen[2].Value) * (orientacionComercialTCAnioOrigen[2].Value / orientacionComercialTCAnioDestino[2].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[2].Value)) ? 0 : (orientacionComercialCPIAnioDestino[2].Value / orientacionComercialCPIAnioOrigen[2].Value));
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato18) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato18) * facCorrMonExt, ValorTotal = 0 });
                                                                //Total por año
                                                                decimal totalValorNacAnio = 0;
                                                                decimal totalValorExtAnio = 0;
                                                                for (int i = 0; i < ingAnio.Count; i++)
                                                                {
                                                                    totalValorNacAnio += ingAnio[i].ValorNac;
                                                                    totalValorExtAnio += ingAnio[i].ValorExt;
                                                                    ingAnio[i].ValorTotal = (ingAnio[i].ValorNac + ingAnio[i].ValorExt);
                                                                }
                                                                //Total Capex
                                                                ingAnio.Add(new Presupuesto.CellValue { ValorNac = (perAntNac + totalValorNacMes + totalValorNacAnio), ValorExt = (perAntExt + totalValorExtMes + totalValorExtAnio), ValorTotal = ((perAntNac + totalValorNacMes + totalValorNacAnio) + (perAntExt + totalValorExtMes + totalValorExtAnio)) });
                                                            }
                                                            else if (IfDato0.StartsWith(prefixAdq))
                                                            {
                                                                adqMes.Add(new Presupuesto.CellValue { Titulo = IfDato0, TituloNac = filaMonedaNacional.IfDDato0, TituloExt = filaMonedaExtranjera.IfDDato0 });
                                                                decimal perAntNac = stringToDecimal(filaMonedaNacional.IfDDato2);
                                                                decimal perAntExt = stringToDecimal(filaMonedaExtranjera.IfDDato2);
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = perAntNac, ValorExt = perAntExt, ValorTotal = perAntTotal });
                                                                //Enero
                                                                decimal facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[0].Value) || decimalIsZero(orientacionComercialTCMesDestino[0].Value)) ? 0 : ((orientacionComercialIPCMesDestino[0].Value / orientacionComercialIPCMesOrigen[0].Value) * (orientacionComercialTCMesOrigen[0].Value / orientacionComercialTCMesDestino[0].Value)));
                                                                decimal facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[0].Value)) ? 0 : (orientacionComercialCPIMesDestino[0].Value / orientacionComercialCPIMesOrigen[0].Value));
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato3) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato3) * facCorrMonExt, ValorTotal = 0 });
                                                                //Febrero
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[1].Value) || decimalIsZero(orientacionComercialTCMesDestino[1].Value)) ? 0 : ((orientacionComercialIPCMesDestino[1].Value / orientacionComercialIPCMesOrigen[1].Value) * (orientacionComercialTCMesOrigen[1].Value / orientacionComercialTCMesDestino[1].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[1].Value)) ? 0 : (orientacionComercialCPIMesDestino[1].Value / orientacionComercialCPIMesOrigen[1].Value));
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato4) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato4) * facCorrMonExt, ValorTotal = 0 });
                                                                //Marzo
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[2].Value) || decimalIsZero(orientacionComercialTCMesDestino[2].Value)) ? 0 : ((orientacionComercialIPCMesDestino[2].Value / orientacionComercialIPCMesOrigen[2].Value) * (orientacionComercialTCMesOrigen[2].Value / orientacionComercialTCMesDestino[2].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[2].Value)) ? 0 : (orientacionComercialCPIMesDestino[2].Value / orientacionComercialCPIMesOrigen[2].Value));
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato5) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato5) * facCorrMonExt, ValorTotal = 0 });
                                                                //Abril
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[3].Value) || decimalIsZero(orientacionComercialTCMesDestino[3].Value)) ? 0 : ((orientacionComercialIPCMesDestino[3].Value / orientacionComercialIPCMesOrigen[3].Value) * (orientacionComercialTCMesOrigen[3].Value / orientacionComercialTCMesDestino[3].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[3].Value)) ? 0 : (orientacionComercialCPIMesDestino[3].Value / orientacionComercialCPIMesOrigen[3].Value));
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato6) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato6) * facCorrMonExt, ValorTotal = 0 });
                                                                //Mayo
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[4].Value) || decimalIsZero(orientacionComercialTCMesDestino[4].Value)) ? 0 : ((orientacionComercialIPCMesDestino[4].Value / orientacionComercialIPCMesOrigen[4].Value) * (orientacionComercialTCMesOrigen[4].Value / orientacionComercialTCMesDestino[4].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[4].Value)) ? 0 : (orientacionComercialCPIMesDestino[4].Value / orientacionComercialCPIMesOrigen[4].Value));
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato7) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato7) * facCorrMonExt, ValorTotal = 0 });
                                                                //Junio
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[5].Value) || decimalIsZero(orientacionComercialTCMesDestino[5].Value)) ? 0 : ((orientacionComercialIPCMesDestino[5].Value / orientacionComercialIPCMesOrigen[5].Value) * (orientacionComercialTCMesOrigen[5].Value / orientacionComercialTCMesDestino[5].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[5].Value)) ? 0 : (orientacionComercialCPIMesDestino[5].Value / orientacionComercialCPIMesOrigen[5].Value));
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato8) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato8) * facCorrMonExt, ValorTotal = 0 });
                                                                //Julio
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[6].Value) || decimalIsZero(orientacionComercialTCMesDestino[6].Value)) ? 0 : ((orientacionComercialIPCMesDestino[6].Value / orientacionComercialIPCMesOrigen[6].Value) * (orientacionComercialTCMesOrigen[6].Value / orientacionComercialTCMesDestino[6].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[6].Value)) ? 0 : (orientacionComercialCPIMesDestino[6].Value / orientacionComercialCPIMesOrigen[6].Value));
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato9) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato9) * facCorrMonExt, ValorTotal = 0 });
                                                                //Agosto
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[7].Value) || decimalIsZero(orientacionComercialTCMesDestino[7].Value)) ? 0 : ((orientacionComercialIPCMesDestino[7].Value / orientacionComercialIPCMesOrigen[7].Value) * (orientacionComercialTCMesOrigen[7].Value / orientacionComercialTCMesDestino[7].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[7].Value)) ? 0 : (orientacionComercialCPIMesDestino[7].Value / orientacionComercialCPIMesOrigen[7].Value));
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato10) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato10) * facCorrMonExt, ValorTotal = 0 });
                                                                //Septiembre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[8].Value) || decimalIsZero(orientacionComercialTCMesDestino[8].Value)) ? 0 : ((orientacionComercialIPCMesDestino[8].Value / orientacionComercialIPCMesOrigen[8].Value) * (orientacionComercialTCMesOrigen[8].Value / orientacionComercialTCMesDestino[8].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[8].Value)) ? 0 : (orientacionComercialCPIMesDestino[8].Value / orientacionComercialCPIMesOrigen[8].Value));
                                                                //Octubre
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato11) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato11) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[9].Value) || decimalIsZero(orientacionComercialTCMesDestino[9].Value)) ? 0 : ((orientacionComercialIPCMesDestino[9].Value / orientacionComercialIPCMesOrigen[9].Value) * (orientacionComercialTCMesOrigen[9].Value / orientacionComercialTCMesDestino[9].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[9].Value)) ? 0 : (orientacionComercialCPIMesDestino[9].Value / orientacionComercialCPIMesOrigen[9].Value));
                                                                //Noviembre
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato12) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato12) * facCorrMonExt, ValorTotal = 0 });
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[10].Value) || decimalIsZero(orientacionComercialTCMesDestino[10].Value)) ? 0 : ((orientacionComercialIPCMesDestino[10].Value / orientacionComercialIPCMesOrigen[10].Value) * (orientacionComercialTCMesOrigen[10].Value / orientacionComercialTCMesDestino[10].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[10].Value)) ? 0 : (orientacionComercialCPIMesDestino[10].Value / orientacionComercialCPIMesOrigen[10].Value));
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato13) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato13) * facCorrMonExt, ValorTotal = 0 });
                                                                //Diciembre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[11].Value) || decimalIsZero(orientacionComercialTCMesDestino[11].Value)) ? 0 : ((orientacionComercialIPCMesDestino[11].Value / orientacionComercialIPCMesOrigen[11].Value) * (orientacionComercialTCMesOrigen[11].Value / orientacionComercialTCMesDestino[11].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[11].Value)) ? 0 : (orientacionComercialCPIMesDestino[11].Value / orientacionComercialCPIMesOrigen[11].Value));
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato14) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato14) * facCorrMonExt, ValorTotal = 0 });
                                                                //Calculo Totales por Mes
                                                                decimal totalValorNacMes = 0;
                                                                decimal totalValorExtMes = 0;
                                                                for (int i = 2; i < adqMes.Count; i++)
                                                                {
                                                                    totalValorNacMes += adqMes[i].ValorNac;
                                                                    totalValorExtMes += adqMes[i].ValorExt;
                                                                    adqMes[i].ValorTotal = (adqMes[i].ValorNac + adqMes[i].ValorExt);
                                                                }
                                                                //Total
                                                                adqMes.Add(new Presupuesto.CellValue { ValorNac = totalValorNacMes, ValorExt = totalValorExtMes, ValorTotal = (totalValorNacMes + totalValorExtMes) });
                                                                //AñoMas1
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[0].Value) || decimalIsZero(orientacionComercialTCAnioDestino[0].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[0].Value / orientacionComercialIPCAnioOrigen[0].Value) * (orientacionComercialTCAnioOrigen[0].Value / orientacionComercialTCAnioDestino[0].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[0].Value)) ? 0 : (orientacionComercialCPIAnioDestino[0].Value / orientacionComercialCPIAnioOrigen[0].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato16) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato16) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas2
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[1].Value) || decimalIsZero(orientacionComercialTCAnioDestino[1].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[1].Value / orientacionComercialIPCAnioOrigen[1].Value) * (orientacionComercialTCAnioOrigen[1].Value / orientacionComercialTCAnioDestino[1].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[1].Value)) ? 0 : (orientacionComercialCPIAnioDestino[1].Value / orientacionComercialCPIAnioOrigen[1].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato17) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato17) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas3
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[2].Value) || decimalIsZero(orientacionComercialTCAnioDestino[2].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[2].Value / orientacionComercialIPCAnioOrigen[2].Value) * (orientacionComercialTCAnioOrigen[2].Value / orientacionComercialTCAnioDestino[2].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[2].Value)) ? 0 : (orientacionComercialCPIAnioDestino[2].Value / orientacionComercialCPIAnioOrigen[2].Value));
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato18) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato18) * facCorrMonExt, ValorTotal = 0 });
                                                                //Total por año
                                                                decimal totalValorNacAnio = 0;
                                                                decimal totalValorExtAnio = 0;
                                                                for (int i = 0; i < adqAnio.Count; i++)
                                                                {
                                                                    totalValorNacAnio += adqAnio[i].ValorNac;
                                                                    totalValorExtAnio += adqAnio[i].ValorExt;
                                                                    adqAnio[i].ValorTotal = (adqAnio[i].ValorNac + adqAnio[i].ValorExt);
                                                                }
                                                                //Total Capex
                                                                adqAnio.Add(new Presupuesto.CellValue { ValorNac = (perAntNac + totalValorNacMes + totalValorNacAnio), ValorExt = (perAntExt + totalValorExtMes + totalValorExtAnio), ValorTotal = ((perAntNac + totalValorNacMes + totalValorNacAnio) + (perAntExt + totalValorExtMes + totalValorExtAnio)) });
                                                            }
                                                            else if (IfDato0.StartsWith(prefixCons))
                                                            {
                                                                consMes.Add(new Presupuesto.CellValue { Titulo = IfDato0, TituloNac = filaMonedaNacional.IfDDato0.ToString(), TituloExt = filaMonedaExtranjera.IfDDato0.ToString() });
                                                                decimal perAntNac = stringToDecimal(filaMonedaNacional.IfDDato2);
                                                                decimal perAntExt = stringToDecimal(filaMonedaExtranjera.IfDDato2);
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = perAntNac, ValorExt = perAntExt });
                                                                //Enero
                                                                decimal facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[0].Value) || decimalIsZero(orientacionComercialTCMesDestino[0].Value)) ? 0 : ((orientacionComercialIPCMesDestino[0].Value / orientacionComercialIPCMesOrigen[0].Value) * (orientacionComercialTCMesOrigen[0].Value / orientacionComercialTCMesDestino[0].Value)));
                                                                decimal facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[0].Value)) ? 0 : (orientacionComercialCPIMesDestino[0].Value / orientacionComercialCPIMesOrigen[0].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato3) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato3) * facCorrMonExt, ValorTotal = 0 });
                                                                //Febrero
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[1].Value) || decimalIsZero(orientacionComercialTCMesDestino[1].Value)) ? 0 : ((orientacionComercialIPCMesDestino[1].Value / orientacionComercialIPCMesOrigen[1].Value) * (orientacionComercialTCMesOrigen[1].Value / orientacionComercialTCMesDestino[1].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[1].Value)) ? 0 : (orientacionComercialCPIMesDestino[1].Value / orientacionComercialCPIMesOrigen[1].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato4) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato4) * facCorrMonExt, ValorTotal = 0 });
                                                                //Marzo
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[2].Value) || decimalIsZero(orientacionComercialTCMesDestino[2].Value)) ? 0 : ((orientacionComercialIPCMesDestino[2].Value / orientacionComercialIPCMesOrigen[2].Value) * (orientacionComercialTCMesOrigen[2].Value / orientacionComercialTCMesDestino[2].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[2].Value)) ? 0 : (orientacionComercialCPIMesDestino[2].Value / orientacionComercialCPIMesOrigen[2].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato5) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato5) * facCorrMonExt, ValorTotal = 0 });
                                                                //Abril
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[3].Value) || decimalIsZero(orientacionComercialTCMesDestino[3].Value)) ? 0 : ((orientacionComercialIPCMesDestino[3].Value / orientacionComercialIPCMesOrigen[3].Value) * (orientacionComercialTCMesOrigen[3].Value / orientacionComercialTCMesDestino[3].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[3].Value)) ? 0 : (orientacionComercialCPIMesDestino[3].Value / orientacionComercialCPIMesOrigen[3].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato6) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato6) * facCorrMonExt, ValorTotal = 0 });
                                                                //Mayo
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[4].Value) || decimalIsZero(orientacionComercialTCMesDestino[4].Value)) ? 0 : ((orientacionComercialIPCMesDestino[4].Value / orientacionComercialIPCMesOrigen[4].Value) * (orientacionComercialTCMesOrigen[4].Value / orientacionComercialTCMesDestino[4].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[4].Value)) ? 0 : (orientacionComercialCPIMesDestino[4].Value / orientacionComercialCPIMesOrigen[4].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato7) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato7) * facCorrMonExt, ValorTotal = 0 });
                                                                //Junio
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[5].Value) || decimalIsZero(orientacionComercialTCMesDestino[5].Value)) ? 0 : ((orientacionComercialIPCMesDestino[5].Value / orientacionComercialIPCMesOrigen[5].Value) * (orientacionComercialTCMesOrigen[5].Value / orientacionComercialTCMesDestino[5].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[5].Value)) ? 0 : (orientacionComercialCPIMesDestino[5].Value / orientacionComercialCPIMesOrigen[5].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato8) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato8) * facCorrMonExt, ValorTotal = 0 });
                                                                //Julio
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[6].Value) || decimalIsZero(orientacionComercialTCMesDestino[6].Value)) ? 0 : ((orientacionComercialIPCMesDestino[6].Value / orientacionComercialIPCMesOrigen[6].Value) * (orientacionComercialTCMesOrigen[6].Value / orientacionComercialTCMesDestino[6].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[6].Value)) ? 0 : (orientacionComercialCPIMesDestino[6].Value / orientacionComercialCPIMesOrigen[6].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato9) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato9) * facCorrMonExt, ValorTotal = 0 });
                                                                //Agosto
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[7].Value) || decimalIsZero(orientacionComercialTCMesDestino[7].Value)) ? 0 : ((orientacionComercialIPCMesDestino[7].Value / orientacionComercialIPCMesOrigen[7].Value) * (orientacionComercialTCMesOrigen[7].Value / orientacionComercialTCMesDestino[7].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[7].Value)) ? 0 : (orientacionComercialCPIMesDestino[7].Value / orientacionComercialCPIMesOrigen[7].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato10) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato10) * facCorrMonExt, ValorTotal = 0 });
                                                                //Septiembre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[8].Value) || decimalIsZero(orientacionComercialTCMesDestino[8].Value)) ? 0 : ((orientacionComercialIPCMesDestino[8].Value / orientacionComercialIPCMesOrigen[8].Value) * (orientacionComercialTCMesOrigen[8].Value / orientacionComercialTCMesDestino[8].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[8].Value)) ? 0 : (orientacionComercialCPIMesDestino[8].Value / orientacionComercialCPIMesOrigen[8].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato11) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato11) * facCorrMonExt, ValorTotal = 0 });
                                                                //Octubre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[9].Value) || decimalIsZero(orientacionComercialTCMesDestino[9].Value)) ? 0 : ((orientacionComercialIPCMesDestino[9].Value / orientacionComercialIPCMesOrigen[9].Value) * (orientacionComercialTCMesOrigen[9].Value / orientacionComercialTCMesDestino[9].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[9].Value)) ? 0 : (orientacionComercialCPIMesDestino[9].Value / orientacionComercialCPIMesOrigen[9].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato12) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato12) * facCorrMonExt, ValorTotal = 0 });
                                                                //Noviembre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[10].Value) || decimalIsZero(orientacionComercialTCMesDestino[10].Value)) ? 0 : ((orientacionComercialIPCMesDestino[10].Value / orientacionComercialIPCMesOrigen[10].Value) * (orientacionComercialTCMesOrigen[10].Value / orientacionComercialTCMesDestino[10].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[10].Value)) ? 0 : (orientacionComercialCPIMesDestino[10].Value / orientacionComercialCPIMesOrigen[10].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato13) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato13) * facCorrMonExt, ValorTotal = 0 });
                                                                //Diciembre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[11].Value) || decimalIsZero(orientacionComercialTCMesDestino[11].Value)) ? 0 : ((orientacionComercialIPCMesDestino[11].Value / orientacionComercialIPCMesOrigen[11].Value) * (orientacionComercialTCMesOrigen[11].Value / orientacionComercialTCMesDestino[11].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[11].Value)) ? 0 : (orientacionComercialCPIMesDestino[11].Value / orientacionComercialCPIMesOrigen[11].Value));
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato14) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato14) * facCorrMonExt, ValorTotal = 0 });

                                                                decimal totalValorNacMes = 0;
                                                                decimal totalValorExtMes = 0;
                                                                for (int i = 2; i < consMes.Count; i++)
                                                                {
                                                                    totalValorNacMes += consMes[i].ValorNac;
                                                                    totalValorExtMes += consMes[i].ValorExt;
                                                                    consMes[i].ValorTotal = (consMes[i].ValorNac + consMes[i].ValorExt);
                                                                }
                                                                //Total meses
                                                                consMes.Add(new Presupuesto.CellValue { ValorNac = totalValorNacMes, ValorExt = totalValorExtMes, ValorTotal = (totalValorNacMes + totalValorExtMes) });
                                                                //AñoMas1
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[0].Value) || decimalIsZero(orientacionComercialTCAnioDestino[0].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[0].Value / orientacionComercialIPCAnioOrigen[0].Value) * (orientacionComercialTCAnioOrigen[0].Value / orientacionComercialTCAnioDestino[0].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[0].Value)) ? 0 : (orientacionComercialCPIAnioDestino[0].Value / orientacionComercialCPIAnioOrigen[0].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato16) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato16) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas2
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[1].Value) || decimalIsZero(orientacionComercialTCAnioDestino[1].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[1].Value / orientacionComercialIPCAnioOrigen[1].Value) * (orientacionComercialTCAnioOrigen[1].Value / orientacionComercialTCAnioDestino[1].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[1].Value)) ? 0 : (orientacionComercialCPIAnioDestino[1].Value / orientacionComercialCPIAnioOrigen[1].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato17) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato17) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas3
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[2].Value) || decimalIsZero(orientacionComercialTCAnioDestino[2].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[2].Value / orientacionComercialIPCAnioOrigen[2].Value) * (orientacionComercialTCAnioOrigen[2].Value / orientacionComercialTCAnioDestino[2].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[2].Value)) ? 0 : (orientacionComercialCPIAnioDestino[2].Value / orientacionComercialCPIAnioOrigen[2].Value));
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato18) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato18) * facCorrMonExt, ValorTotal = 0 });
                                                                decimal totalValorNacAnio = 0;
                                                                decimal totalValorExtAnio = 0;
                                                                for (int i = 0; i < consAnio.Count; i++)
                                                                {
                                                                    totalValorNacAnio += consAnio[i].ValorNac;
                                                                    totalValorExtAnio += consAnio[i].ValorExt;
                                                                    consAnio[i].ValorTotal = (consAnio[i].ValorNac + consAnio[i].ValorExt);
                                                                }
                                                                //Total Capex
                                                                consAnio.Add(new Presupuesto.CellValue { ValorNac = (perAntNac + totalValorNacMes + totalValorNacAnio), ValorExt = (perAntExt + totalValorExtMes + totalValorExtAnio), ValorTotal = ((perAntNac + totalValorNacMes + totalValorNacAnio) + (perAntExt + totalValorExtMes + totalValorExtAnio)) });
                                                            }
                                                            else if (IfDato0.StartsWith(prefixAdm))
                                                            {
                                                                admMes.Add(new Presupuesto.CellValue { Titulo = IfDato0, TituloNac = filaMonedaNacional.IfDDato0.ToString(), TituloExt = filaMonedaExtranjera.IfDDato0.ToString() });
                                                                decimal perAntNac = stringToDecimal(filaMonedaNacional.IfDDato2);
                                                                decimal perAntExt = stringToDecimal(filaMonedaExtranjera.IfDDato2);
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = perAntNac, ValorExt = perAntExt });
                                                                //Enero
                                                                decimal facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[0].Value) || decimalIsZero(orientacionComercialTCMesDestino[0].Value)) ? 0 : ((orientacionComercialIPCMesDestino[0].Value / orientacionComercialIPCMesOrigen[0].Value) * (orientacionComercialTCMesOrigen[0].Value / orientacionComercialTCMesDestino[0].Value)));
                                                                decimal facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[0].Value)) ? 0 : (orientacionComercialCPIMesDestino[0].Value / orientacionComercialCPIMesOrigen[0].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato3) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato3) * facCorrMonExt, ValorTotal = 0 });
                                                                //Febrero
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[1].Value) || decimalIsZero(orientacionComercialTCMesDestino[1].Value)) ? 0 : ((orientacionComercialIPCMesDestino[1].Value / orientacionComercialIPCMesOrigen[1].Value) * (orientacionComercialTCMesOrigen[1].Value / orientacionComercialTCMesDestino[1].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[1].Value)) ? 0 : (orientacionComercialCPIMesDestino[1].Value / orientacionComercialCPIMesOrigen[1].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato4) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato4) * facCorrMonExt, ValorTotal = 0 });
                                                                //Marzo
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[2].Value) || decimalIsZero(orientacionComercialTCMesDestino[2].Value)) ? 0 : ((orientacionComercialIPCMesDestino[2].Value / orientacionComercialIPCMesOrigen[2].Value) * (orientacionComercialTCMesOrigen[2].Value / orientacionComercialTCMesDestino[2].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[2].Value)) ? 0 : (orientacionComercialCPIMesDestino[2].Value / orientacionComercialCPIMesOrigen[2].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato5) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato5) * facCorrMonExt, ValorTotal = 0 });
                                                                //Abril
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[3].Value) || decimalIsZero(orientacionComercialTCMesDestino[3].Value)) ? 0 : ((orientacionComercialIPCMesDestino[3].Value / orientacionComercialIPCMesOrigen[3].Value) * (orientacionComercialTCMesOrigen[3].Value / orientacionComercialTCMesDestino[3].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[3].Value)) ? 0 : (orientacionComercialCPIMesDestino[3].Value / orientacionComercialCPIMesOrigen[3].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato6) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato6) * facCorrMonExt, ValorTotal = 0 });
                                                                //Mayo
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[4].Value) || decimalIsZero(orientacionComercialTCMesDestino[4].Value)) ? 0 : ((orientacionComercialIPCMesDestino[4].Value / orientacionComercialIPCMesOrigen[4].Value) * (orientacionComercialTCMesOrigen[4].Value / orientacionComercialTCMesDestino[4].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[4].Value)) ? 0 : (orientacionComercialCPIMesDestino[4].Value / orientacionComercialCPIMesOrigen[4].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato7) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato7) * facCorrMonExt, ValorTotal = 0 });
                                                                //Junio
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[5].Value) || decimalIsZero(orientacionComercialTCMesDestino[5].Value)) ? 0 : ((orientacionComercialIPCMesDestino[5].Value / orientacionComercialIPCMesOrigen[5].Value) * (orientacionComercialTCMesOrigen[5].Value / orientacionComercialTCMesDestino[5].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[5].Value)) ? 0 : (orientacionComercialCPIMesDestino[5].Value / orientacionComercialCPIMesOrigen[5].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato8) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato8) * facCorrMonExt, ValorTotal = 0 });
                                                                //Julio
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[6].Value) || decimalIsZero(orientacionComercialTCMesDestino[6].Value)) ? 0 : ((orientacionComercialIPCMesDestino[6].Value / orientacionComercialIPCMesOrigen[6].Value) * (orientacionComercialTCMesOrigen[6].Value / orientacionComercialTCMesDestino[6].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[6].Value)) ? 0 : (orientacionComercialCPIMesDestino[6].Value / orientacionComercialCPIMesOrigen[6].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato9) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato9) * facCorrMonExt, ValorTotal = 0 });
                                                                //Agosto
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[7].Value) || decimalIsZero(orientacionComercialTCMesDestino[7].Value)) ? 0 : ((orientacionComercialIPCMesDestino[7].Value / orientacionComercialIPCMesOrigen[7].Value) * (orientacionComercialTCMesOrigen[7].Value / orientacionComercialTCMesDestino[7].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[7].Value)) ? 0 : (orientacionComercialCPIMesDestino[7].Value / orientacionComercialCPIMesOrigen[7].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato10) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato10) * facCorrMonExt, ValorTotal = 0 });
                                                                //Septiembre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[8].Value) || decimalIsZero(orientacionComercialTCMesDestino[8].Value)) ? 0 : ((orientacionComercialIPCMesDestino[8].Value / orientacionComercialIPCMesOrigen[8].Value) * (orientacionComercialTCMesOrigen[8].Value / orientacionComercialTCMesDestino[8].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[8].Value)) ? 0 : (orientacionComercialCPIMesDestino[8].Value / orientacionComercialCPIMesOrigen[8].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato11) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato11) * facCorrMonExt, ValorTotal = 0 });
                                                                //Octubre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[9].Value) || decimalIsZero(orientacionComercialTCMesDestino[9].Value)) ? 0 : ((orientacionComercialIPCMesDestino[9].Value / orientacionComercialIPCMesOrigen[9].Value) * (orientacionComercialTCMesOrigen[9].Value / orientacionComercialTCMesDestino[9].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[9].Value)) ? 0 : (orientacionComercialCPIMesDestino[9].Value / orientacionComercialCPIMesOrigen[9].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato12) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato12) * facCorrMonExt, ValorTotal = 0 });
                                                                //Noviembre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[10].Value) || decimalIsZero(orientacionComercialTCMesDestino[10].Value)) ? 0 : ((orientacionComercialIPCMesDestino[10].Value / orientacionComercialIPCMesOrigen[10].Value) * (orientacionComercialTCMesOrigen[10].Value / orientacionComercialTCMesDestino[10].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[10].Value)) ? 0 : (orientacionComercialCPIMesDestino[10].Value / orientacionComercialCPIMesOrigen[10].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato13) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato13) * facCorrMonExt, ValorTotal = 0 });
                                                                //Diciembre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[11].Value) || decimalIsZero(orientacionComercialTCMesDestino[11].Value)) ? 0 : ((orientacionComercialIPCMesDestino[11].Value / orientacionComercialIPCMesOrigen[11].Value) * (orientacionComercialTCMesOrigen[11].Value / orientacionComercialTCMesDestino[11].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[11].Value)) ? 0 : (orientacionComercialCPIMesDestino[11].Value / orientacionComercialCPIMesOrigen[11].Value));
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato14) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato14) * facCorrMonExt, ValorTotal = 0 });
                                                                decimal totalValorNacMes = 0;
                                                                decimal totalValorExtMes = 0;
                                                                for (int i = 2; i < admMes.Count; i++)
                                                                {
                                                                    totalValorNacMes += admMes[i].ValorNac;
                                                                    totalValorExtMes += admMes[i].ValorExt;
                                                                    admMes[i].ValorTotal = (admMes[i].ValorNac + admMes[i].ValorExt);
                                                                }
                                                                //Total meses
                                                                admMes.Add(new Presupuesto.CellValue { ValorNac = totalValorNacMes, ValorExt = totalValorExtMes, ValorTotal = (totalValorNacMes + totalValorExtMes) });
                                                                //AñoMas1
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[0].Value) || decimalIsZero(orientacionComercialTCAnioDestino[0].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[0].Value / orientacionComercialIPCAnioOrigen[0].Value) * (orientacionComercialTCAnioOrigen[0].Value / orientacionComercialTCAnioDestino[0].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[0].Value)) ? 0 : (orientacionComercialCPIAnioDestino[0].Value / orientacionComercialCPIAnioOrigen[0].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato16) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato16) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas2
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[1].Value) || decimalIsZero(orientacionComercialTCAnioDestino[1].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[1].Value / orientacionComercialIPCAnioOrigen[1].Value) * (orientacionComercialTCAnioOrigen[1].Value / orientacionComercialTCAnioDestino[1].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[1].Value)) ? 0 : (orientacionComercialCPIAnioDestino[1].Value / orientacionComercialCPIAnioOrigen[1].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato17) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato17) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas3
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[2].Value) || decimalIsZero(orientacionComercialTCAnioDestino[2].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[2].Value / orientacionComercialIPCAnioOrigen[2].Value) * (orientacionComercialTCAnioOrigen[2].Value / orientacionComercialTCAnioDestino[2].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[2].Value)) ? 0 : (orientacionComercialCPIAnioDestino[2].Value / orientacionComercialCPIAnioOrigen[2].Value));
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato18) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato18) * facCorrMonExt, ValorTotal = 0 });
                                                                decimal totalValorNacAnio = 0;
                                                                decimal totalValorExtAnio = 0;
                                                                for (int i = 0; i < admAnio.Count; i++)
                                                                {
                                                                    totalValorNacAnio += admAnio[i].ValorNac;
                                                                    totalValorExtAnio += admAnio[i].ValorExt;
                                                                    admAnio[i].ValorTotal = (admAnio[i].ValorNac + admAnio[i].ValorExt);
                                                                }
                                                                //Total Capex
                                                                admAnio.Add(new Presupuesto.CellValue { ValorNac = (perAntNac + totalValorNacMes + totalValorNacAnio), ValorExt = (perAntExt + totalValorExtMes + totalValorExtAnio), ValorTotal = ((perAntNac + totalValorNacMes + totalValorNacAnio) + (perAntExt + totalValorExtMes + totalValorExtAnio)) });
                                                            }
                                                            else if (IfDato0.StartsWith(prefixCont))
                                                            {
                                                                contMes.Add(new Presupuesto.CellValue { Titulo = IfDato0, TituloNac = filaMonedaNacional.IfDDato0.ToString(), TituloExt = filaMonedaExtranjera.IfDDato0.ToString() });
                                                                decimal perAntNac = stringToDecimal(filaMonedaNacional.IfDDato2);
                                                                decimal perAntExt = stringToDecimal(filaMonedaExtranjera.IfDDato2);
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = perAntNac, ValorExt = perAntExt });
                                                                //Enero
                                                                decimal facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[0].Value) || decimalIsZero(orientacionComercialTCMesDestino[0].Value)) ? 0 : ((orientacionComercialIPCMesDestino[0].Value / orientacionComercialIPCMesOrigen[0].Value) * (orientacionComercialTCMesOrigen[0].Value / orientacionComercialTCMesDestino[0].Value)));
                                                                decimal facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[0].Value)) ? 0 : (orientacionComercialCPIMesDestino[0].Value / orientacionComercialCPIMesOrigen[0].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato3) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato3) * facCorrMonExt, ValorTotal = 0 });
                                                                //Febrero
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[1].Value) || decimalIsZero(orientacionComercialTCMesDestino[1].Value)) ? 0 : ((orientacionComercialIPCMesDestino[1].Value / orientacionComercialIPCMesOrigen[1].Value) * (orientacionComercialTCMesOrigen[1].Value / orientacionComercialTCMesDestino[1].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[1].Value)) ? 0 : (orientacionComercialCPIMesDestino[1].Value / orientacionComercialCPIMesOrigen[1].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato4) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato4) * facCorrMonExt, ValorTotal = 0 });
                                                                //Marzo
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[2].Value) || decimalIsZero(orientacionComercialTCMesDestino[2].Value)) ? 0 : ((orientacionComercialIPCMesDestino[2].Value / orientacionComercialIPCMesOrigen[2].Value) * (orientacionComercialTCMesOrigen[2].Value / orientacionComercialTCMesDestino[2].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[2].Value)) ? 0 : (orientacionComercialCPIMesDestino[2].Value / orientacionComercialCPIMesOrigen[2].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato5) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato5) * facCorrMonExt, ValorTotal = 0 });
                                                                //Abril
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[3].Value) || decimalIsZero(orientacionComercialTCMesDestino[3].Value)) ? 0 : ((orientacionComercialIPCMesDestino[3].Value / orientacionComercialIPCMesOrigen[3].Value) * (orientacionComercialTCMesOrigen[3].Value / orientacionComercialTCMesDestino[3].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[3].Value)) ? 0 : (orientacionComercialCPIMesDestino[3].Value / orientacionComercialCPIMesOrigen[3].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato6) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato6) * facCorrMonExt, ValorTotal = 0 });
                                                                //Mayo
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[4].Value) || decimalIsZero(orientacionComercialTCMesDestino[4].Value)) ? 0 : ((orientacionComercialIPCMesDestino[4].Value / orientacionComercialIPCMesOrigen[4].Value) * (orientacionComercialTCMesOrigen[4].Value / orientacionComercialTCMesDestino[4].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[4].Value)) ? 0 : (orientacionComercialCPIMesDestino[4].Value / orientacionComercialCPIMesOrigen[4].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato7) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato7) * facCorrMonExt, ValorTotal = 0 });
                                                                //Junio
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[5].Value) || decimalIsZero(orientacionComercialTCMesDestino[5].Value)) ? 0 : ((orientacionComercialIPCMesDestino[5].Value / orientacionComercialIPCMesOrigen[5].Value) * (orientacionComercialTCMesOrigen[5].Value / orientacionComercialTCMesDestino[5].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[5].Value)) ? 0 : (orientacionComercialCPIMesDestino[5].Value / orientacionComercialCPIMesOrigen[5].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato8) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato8) * facCorrMonExt, ValorTotal = 0 });
                                                                //Julio
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[6].Value) || decimalIsZero(orientacionComercialTCMesDestino[6].Value)) ? 0 : ((orientacionComercialIPCMesDestino[6].Value / orientacionComercialIPCMesOrigen[6].Value) * (orientacionComercialTCMesOrigen[6].Value / orientacionComercialTCMesDestino[6].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[6].Value)) ? 0 : (orientacionComercialCPIMesDestino[6].Value / orientacionComercialCPIMesOrigen[6].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato9) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato9) * facCorrMonExt, ValorTotal = 0 });
                                                                //Agosto
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[7].Value) || decimalIsZero(orientacionComercialTCMesDestino[7].Value)) ? 0 : ((orientacionComercialIPCMesDestino[7].Value / orientacionComercialIPCMesOrigen[7].Value) * (orientacionComercialTCMesOrigen[7].Value / orientacionComercialTCMesDestino[7].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[7].Value)) ? 0 : (orientacionComercialCPIMesDestino[7].Value / orientacionComercialCPIMesOrigen[7].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato10) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato10) * facCorrMonExt, ValorTotal = 0 });
                                                                //Septiembre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[8].Value) || decimalIsZero(orientacionComercialTCMesDestino[8].Value)) ? 0 : ((orientacionComercialIPCMesDestino[8].Value / orientacionComercialIPCMesOrigen[8].Value) * (orientacionComercialTCMesOrigen[8].Value / orientacionComercialTCMesDestino[8].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[8].Value)) ? 0 : (orientacionComercialCPIMesDestino[8].Value / orientacionComercialCPIMesOrigen[8].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato11) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato11) * facCorrMonExt, ValorTotal = 0 });
                                                                //Octubre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[9].Value) || decimalIsZero(orientacionComercialTCMesDestino[9].Value)) ? 0 : ((orientacionComercialIPCMesDestino[9].Value / orientacionComercialIPCMesOrigen[9].Value) * (orientacionComercialTCMesOrigen[9].Value / orientacionComercialTCMesDestino[9].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[9].Value)) ? 0 : (orientacionComercialCPIMesDestino[9].Value / orientacionComercialCPIMesOrigen[9].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato12) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato12) * facCorrMonExt, ValorTotal = 0 });
                                                                //Noviembre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[10].Value) || decimalIsZero(orientacionComercialTCMesDestino[10].Value)) ? 0 : ((orientacionComercialIPCMesDestino[10].Value / orientacionComercialIPCMesOrigen[10].Value) * (orientacionComercialTCMesOrigen[10].Value / orientacionComercialTCMesDestino[10].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[10].Value)) ? 0 : (orientacionComercialCPIMesDestino[10].Value / orientacionComercialCPIMesOrigen[10].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato13) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato13) * facCorrMonExt, ValorTotal = 0 });
                                                                //Diciembre
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCMesOrigen[11].Value) || decimalIsZero(orientacionComercialTCMesDestino[11].Value)) ? 0 : ((orientacionComercialIPCMesDestino[11].Value / orientacionComercialIPCMesOrigen[11].Value) * (orientacionComercialTCMesOrigen[11].Value / orientacionComercialTCMesDestino[11].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIMesOrigen[11].Value)) ? 0 : (orientacionComercialCPIMesDestino[11].Value / orientacionComercialCPIMesOrigen[11].Value));
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato14) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato14) * facCorrMonExt, ValorTotal = 0 });
                                                                decimal totalValorNacMes = 0;
                                                                decimal totalValorExtMes = 0;
                                                                for (int i = 2; i < contMes.Count; i++)
                                                                {
                                                                    totalValorNacMes += contMes[i].ValorNac;
                                                                    totalValorExtMes += contMes[i].ValorExt;
                                                                    contMes[i].ValorTotal = (contMes[i].ValorNac + contMes[i].ValorExt);
                                                                }
                                                                //Total meses
                                                                contMes.Add(new Presupuesto.CellValue { ValorNac = totalValorNacMes, ValorExt = totalValorExtMes, ValorTotal = (totalValorNacMes + totalValorExtMes) });
                                                                //AñoMas1
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[0].Value) || decimalIsZero(orientacionComercialTCAnioDestino[0].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[0].Value / orientacionComercialIPCAnioOrigen[0].Value) * (orientacionComercialTCAnioOrigen[0].Value / orientacionComercialTCAnioDestino[0].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[0].Value)) ? 0 : (orientacionComercialCPIAnioDestino[0].Value / orientacionComercialCPIAnioOrigen[0].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato16) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato16) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas2
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[1].Value) || decimalIsZero(orientacionComercialTCAnioDestino[1].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[1].Value / orientacionComercialIPCAnioOrigen[1].Value) * (orientacionComercialTCAnioOrigen[1].Value / orientacionComercialTCAnioDestino[1].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[1].Value)) ? 0 : (orientacionComercialCPIAnioDestino[1].Value / orientacionComercialCPIAnioOrigen[1].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato17) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato17) * facCorrMonExt, ValorTotal = 0 });
                                                                //AñoMas3
                                                                facCorrMonNac = ((decimalIsZero(orientacionComercialIPCAnioOrigen[2].Value) || decimalIsZero(orientacionComercialTCAnioDestino[2].Value)) ? 0 : ((orientacionComercialIPCAnioDestino[2].Value / orientacionComercialIPCAnioOrigen[2].Value) * (orientacionComercialTCAnioOrigen[2].Value / orientacionComercialTCAnioDestino[2].Value)));
                                                                facCorrMonExt = ((decimalIsZero(orientacionComercialCPIAnioOrigen[2].Value)) ? 0 : (orientacionComercialCPIAnioDestino[2].Value / orientacionComercialCPIAnioOrigen[2].Value));
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = stringToDecimal(filaMonedaNacional.IfDDato18) * facCorrMonNac, ValorExt = stringToDecimal(filaMonedaExtranjera.IfDDato18) * facCorrMonExt, ValorTotal = 0 });
                                                                decimal totalValorNacAnio = 0;
                                                                decimal totalValorExtAnio = 0;
                                                                for (int i = 0; i < contAnio.Count; i++)
                                                                {
                                                                    totalValorNacAnio += contAnio[i].ValorNac;
                                                                    totalValorExtAnio += contAnio[i].ValorExt;
                                                                    contAnio[i].ValorTotal = (contAnio[i].ValorNac + contAnio[i].ValorExt);
                                                                }
                                                                //Total Capex
                                                                contAnio.Add(new Presupuesto.CellValue { ValorNac = (perAntNac + totalValorNacMes + totalValorNacAnio), ValorExt = (perAntExt + totalValorExtMes + totalValorExtAnio), ValorTotal = ((perAntNac + totalValorNacMes + totalValorNacAnio) + (perAntExt + totalValorExtMes + totalValorExtAnio)) });
                                                            }
                                                            else if (IfDato0.StartsWith(prefixTP))
                                                            {
                                                                totParMes.Add(new Presupuesto.CellValue { Titulo = IfDato0, TituloNac = filaMonedaNacional.IfDDato0.ToString(), TituloExt = filaMonedaExtranjera.IfDDato0.ToString() });
                                                            }
                                                        }
                                                    }
                                                }

                                                int totalItems = ingMes.Count;
                                                if (totalItems > 0 && totalItems == adqMes.Count && totalItems == consMes.Count && totalItems == admMes.Count && totalItems == contMes.Count)
                                                {
                                                    for (int i = 1; i < totalItems; i++)
                                                    {
                                                        Presupuesto.CellValue cellValue = new Presupuesto.CellValue { ValorNac = ingMes[i].ValorNac, ValorExt = ingMes[i].ValorExt, ValorTotal = ingMes[i].ValorTotal };
                                                        cellValue.ValorNac += adqMes[i].ValorNac;
                                                        cellValue.ValorExt += adqMes[i].ValorExt;
                                                        cellValue.ValorTotal += adqMes[i].ValorTotal;
                                                        cellValue.ValorNac += consMes[i].ValorNac;
                                                        cellValue.ValorExt += consMes[i].ValorExt;
                                                        cellValue.ValorTotal += consMes[i].ValorTotal;
                                                        cellValue.ValorNac += admMes[i].ValorNac;
                                                        cellValue.ValorExt += admMes[i].ValorExt;
                                                        cellValue.ValorTotal += admMes[i].ValorTotal;
                                                        cellValue.ValorNac += contMes[i].ValorNac;
                                                        cellValue.ValorExt += contMes[i].ValorExt;
                                                        cellValue.ValorTotal += contMes[i].ValorTotal;
                                                        totParMes.Add(cellValue);
                                                    }
                                                    valueContingenciaAnio = ((decimalIsZero(contMes[(totalItems - 1)].ValorTotal) || decimalIsZero((totParMes[(totalItems - 1)].ValorTotal - contMes[(totalItems - 1)].ValorTotal))) ? 0 : (contMes[(totalItems - 1)].ValorTotal / (totParMes[(totalItems - 1)].ValorTotal - contMes[(totalItems - 1)].ValorTotal)));
                                                    valueCostoDuenoAnio = ((decimalIsZero(admMes[(totalItems - 1)].ValorTotal) || decimalIsZero(totParMes[(totalItems - 1)].ValorTotal)) ? 0 : (admMes[(totalItems - 1)].ValorTotal / totParMes[(totalItems - 1)].ValorTotal));
                                                }
                                                totalItems = ingAnio.Count;
                                                if (totalItems > 0 && totalItems == adqAnio.Count && totalItems == consAnio.Count && totalItems == admAnio.Count && totalItems == contAnio.Count)
                                                {
                                                    for (int i = 0; i < (totalItems - 1); i++)
                                                    {
                                                        Presupuesto.CellValue cellValue = new Presupuesto.CellValue { ValorNac = ingAnio[i].ValorNac, ValorExt = ingAnio[i].ValorExt, ValorTotal = ingAnio[i].ValorTotal };
                                                        cellValue.ValorNac += adqAnio[i].ValorNac;
                                                        cellValue.ValorExt += adqAnio[i].ValorExt;
                                                        cellValue.ValorTotal += adqAnio[i].ValorTotal;
                                                        cellValue.ValorNac += consAnio[i].ValorNac;
                                                        cellValue.ValorExt += consAnio[i].ValorExt;
                                                        cellValue.ValorTotal += consAnio[i].ValorTotal;
                                                        cellValue.ValorNac += admAnio[i].ValorNac;
                                                        cellValue.ValorExt += admAnio[i].ValorExt;
                                                        cellValue.ValorTotal += admAnio[i].ValorTotal;
                                                        cellValue.ValorNac += contAnio[i].ValorNac;
                                                        cellValue.ValorExt += contAnio[i].ValorExt;
                                                        cellValue.ValorTotal += contAnio[i].ValorTotal;
                                                        totParAnio.Add(cellValue);
                                                    }
                                                    decimal totalValorNacAnio = 0;
                                                    decimal totalValorExtAnio = 0;
                                                    decimal totalValorAnio = 0;
                                                    for (int i = 0; i < totParAnio.Count; i++)
                                                    {
                                                        totalValorNacAnio += totParAnio[i].ValorNac;
                                                        totalValorExtAnio += totParAnio[i].ValorExt;
                                                        totalValorAnio += totParAnio[i].ValorTotal;
                                                    }
                                                    totParAnio.Add(new Presupuesto.CellValue { ValorNac = (totParMes[1].ValorNac + totParMes[(totParMes.Count - 1)].ValorNac + totalValorNacAnio), ValorExt = (totParMes[1].ValorExt + totParMes[(totParMes.Count - 1)].ValorExt + totalValorExtAnio), ValorTotal = ((totParMes[1].ValorNac + totParMes[(totParMes.Count - 1)].ValorNac + totalValorNacAnio) + (totParMes[1].ValorExt + totParMes[(totParMes.Count - 1)].ValorExt + totalValorExtAnio)) });
                                                    valueContingenciaTotalCapex = ((decimalIsZero(contAnio[(totalItems - 1)].ValorTotal) || decimalIsZero((totParAnio[(totalItems - 1)].ValorTotal - contAnio[(totalItems - 1)].ValorTotal))) ? 0 : (contAnio[(totalItems - 1)].ValorTotal / (totParAnio[(totalItems - 1)].ValorTotal - contAnio[(totalItems - 1)].ValorTotal)));
                                                    valueCostoDuenoTotalCapex = ((decimalIsZero(admAnio[(totalItems - 1)].ValorTotal) || decimalIsZero(totParAnio[(totalItems - 1)].ValorTotal)) ? 0 : (admAnio[(totalItems - 1)].ValorTotal / totParAnio[(totalItems - 1)].ValorTotal));
                                                }
                                                totAcumMes.Add(new Presupuesto.CellValue { ValorTotal = totParMes[1].ValorTotal });
                                                for (int i = 2; i < totParMes.Count; i++)
                                                {
                                                    if (i == (totParMes.Count - 1))
                                                    {
                                                        totAcumMes.Add(new Presupuesto.CellValue { ValorTotal = totAcumMes[(i - 1)].ValorTotal });
                                                    }
                                                    else
                                                    {
                                                        totAcumMes.Add(new Presupuesto.CellValue { ValorTotal = (totAcumMes[(i - 1)].ValorTotal + totParMes[i].ValorTotal) });
                                                    }
                                                }

                                                for (int i = 0; i < totParAnio.Count; i++)
                                                {
                                                    if (i == 0)
                                                    {
                                                        totAcumAnio.Add(new Presupuesto.CellValue { ValorTotal = (totAcumMes[totAcumMes.Count - 1].ValorTotal + totParAnio[i].ValorTotal) });
                                                    }
                                                    else if (i == (totParAnio.Count - 1))
                                                    {
                                                        totAcumAnio.Add(new Presupuesto.CellValue { ValorTotal = totParAnio[i].ValorTotal });
                                                    }
                                                    else
                                                    {
                                                        totAcumAnio.Add(new Presupuesto.CellValue { ValorTotal = (totAcumAnio[i - 1].ValorTotal + totParAnio[i].ValorTotal) });
                                                    }
                                                }
                                                List<String> Datos = new List<String>();
                                                List<String> DatosAdqMes = new List<String>();
                                                List<String> DatosConsMes = new List<String>();
                                                List<String> DatosAdmMes = new List<String>();
                                                List<String> DatosContMes = new List<String>();
                                                List<String> DatosTotParMes = new List<String>();
                                                List<String> DatosTotAcumMes = new List<String>();
                                                Datos.Add(iniToken);
                                                Datos.Add(usuario);
                                                Datos.Add(parametroVNToken);
                                                DatosAdqMes.Add(iniToken);
                                                DatosAdqMes.Add(usuario);
                                                DatosAdqMes.Add(parametroVNToken);
                                                DatosConsMes.Add(iniToken);
                                                DatosConsMes.Add(usuario);
                                                DatosConsMes.Add(parametroVNToken);
                                                DatosAdmMes.Add(iniToken);
                                                DatosAdmMes.Add(usuario);
                                                DatosAdmMes.Add(parametroVNToken);
                                                DatosContMes.Add(iniToken);
                                                DatosContMes.Add(usuario);
                                                DatosContMes.Add(parametroVNToken);
                                                DatosTotParMes.Add(iniToken);
                                                DatosTotParMes.Add(usuario);
                                                DatosTotParMes.Add(parametroVNToken);
                                                DatosTotAcumMes.Add(iniToken);
                                                DatosTotAcumMes.Add(usuario);
                                                DatosTotAcumMes.Add(parametroVNToken);
                                                decimal ponderacionParcial = 0;
                                                decimal ponderacionParcialPorcentaje = 0;
                                                for (int i = 0; i < 3; i++)
                                                {
                                                    for (int j = 0; j < ingMes.Count; j++)
                                                    {
                                                        if (i == 0)
                                                        {
                                                            if (j == 0)
                                                            {
                                                                Datos.Add(ingMes[j].Titulo);
                                                            }
                                                            else if (j == 1)
                                                            {
                                                                decimal ponderacion = 0;
                                                                if (!decimalIsZero(ingAnio[(ingAnio.Count - 1)].ValorTotal) && !decimalIsZero(totParAnio[(totParAnio.Count - 1)].ValorTotal))
                                                                {
                                                                    ponderacion = ingAnio[(ingAnio.Count - 1)].ValorTotal / totParAnio[(totParAnio.Count - 1)].ValorTotal;
                                                                    ponderacionParcial += ponderacion;
                                                                    ponderacion = ponderacion * 100;
                                                                    ponderacionParcialPorcentaje += ponderacion;
                                                                }
                                                                Datos.Add(ponderacion.ToString("0.0"));
                                                                Datos.Add(ingMes[j].ValorTotal.ToString());
                                                            }
                                                            else
                                                            {
                                                                Datos.Add(ingMes[j].ValorTotal.ToString());
                                                            }
                                                            if (j == (ingMes.Count - 1))
                                                            {
                                                                for (int k = 0; k < ingAnio.Count; k++)
                                                                {
                                                                    Datos.Add(ingAnio[k].ValorTotal.ToString());
                                                                }
                                                            }
                                                        }
                                                        else if (i == 1)
                                                        {
                                                            if (j == 0)
                                                            {
                                                                Datos.Add(ingMes[j].TituloNac);
                                                            }
                                                            else if (j == 1)
                                                            {
                                                                Datos.Add("");
                                                                Datos.Add(ingMes[j].ValorNac.ToString());
                                                            }
                                                            else
                                                            {
                                                                Datos.Add(ingMes[j].ValorNac.ToString());
                                                            }
                                                            if (j == (ingMes.Count - 1))
                                                            {
                                                                for (int k = 0; k < ingAnio.Count; k++)
                                                                {
                                                                    Datos.Add(ingAnio[k].ValorNac.ToString());
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (j == 0)
                                                            {
                                                                Datos.Add(ingMes[j].TituloExt);
                                                            }
                                                            else if (j == 1)
                                                            {
                                                                Datos.Add("");
                                                                Datos.Add(ingMes[j].ValorExt.ToString());
                                                            }
                                                            else
                                                            {
                                                                Datos.Add(ingMes[j].ValorExt.ToString());
                                                            }
                                                            if (j == (ingMes.Count - 1))
                                                            {
                                                                for (int k = 0; k < ingAnio.Count; k++)
                                                                {
                                                                    Datos.Add(ingAnio[k].ValorExt.ToString());
                                                                }
                                                            }
                                                        }
                                                    }

                                                    for (int j = 0; j < adqMes.Count; j++)
                                                    {
                                                        if (i == 0)
                                                        {
                                                            if (j == 0)
                                                            {
                                                                DatosAdqMes.Add(adqMes[j].Titulo);
                                                            }
                                                            else if (j == 1)
                                                            {
                                                                decimal ponderacion = 0;
                                                                if (!decimalIsZero(adqAnio[(adqAnio.Count - 1)].ValorTotal) && !decimalIsZero(totParAnio[(totParAnio.Count - 1)].ValorTotal))
                                                                {
                                                                    ponderacion = adqAnio[(adqAnio.Count - 1)].ValorTotal / totParAnio[(totParAnio.Count - 1)].ValorTotal;
                                                                    ponderacionParcial += ponderacion;
                                                                    ponderacion = ponderacion * 100;
                                                                    ponderacionParcialPorcentaje += ponderacion;
                                                                }
                                                                DatosAdqMes.Add(ponderacion.ToString("0.0"));
                                                                DatosAdqMes.Add(adqMes[j].ValorTotal.ToString());
                                                            }
                                                            else
                                                            {
                                                                DatosAdqMes.Add(adqMes[j].ValorTotal.ToString());
                                                            }
                                                            if (j == (adqMes.Count - 1))
                                                            {
                                                                for (int k = 0; k < adqAnio.Count; k++)
                                                                {
                                                                    DatosAdqMes.Add(adqAnio[k].ValorTotal.ToString());
                                                                }
                                                            }
                                                        }
                                                        else if (i == 1)
                                                        {
                                                            if (j == 0)
                                                            {
                                                                DatosAdqMes.Add(adqMes[j].TituloNac);
                                                            }
                                                            else if (j == 1)
                                                            {
                                                                DatosAdqMes.Add("");
                                                                DatosAdqMes.Add(adqMes[j].ValorNac.ToString());
                                                            }
                                                            else
                                                            {
                                                                DatosAdqMes.Add(adqMes[j].ValorNac.ToString());
                                                            }
                                                            if (j == (adqMes.Count - 1))
                                                            {
                                                                for (int k = 0; k < adqAnio.Count; k++)
                                                                {
                                                                    DatosAdqMes.Add(adqAnio[k].ValorNac.ToString());
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (j == 0)
                                                            {
                                                                DatosAdqMes.Add(adqMes[j].TituloExt);
                                                            }
                                                            else if (j == 1)
                                                            {
                                                                DatosAdqMes.Add("");
                                                                DatosAdqMes.Add(adqMes[j].ValorExt.ToString());
                                                            }
                                                            else
                                                            {
                                                                DatosAdqMes.Add(adqMes[j].ValorExt.ToString());
                                                            }
                                                            if (j == (adqMes.Count - 1))
                                                            {
                                                                for (int k = 0; k < adqAnio.Count; k++)
                                                                {
                                                                    DatosAdqMes.Add(adqAnio[k].ValorExt.ToString());
                                                                }
                                                            }
                                                        }
                                                    }

                                                    for (int j = 0; j < consMes.Count; j++)
                                                    {
                                                        if (i == 0)
                                                        {
                                                            if (j == 0)
                                                            {
                                                                DatosConsMes.Add(consMes[j].Titulo);
                                                            }
                                                            else if (j == 1)
                                                            {
                                                                decimal ponderacion = 0;
                                                                if (!decimalIsZero(consAnio[(consAnio.Count - 1)].ValorTotal) && !decimalIsZero(totParAnio[(totParAnio.Count - 1)].ValorTotal))
                                                                {
                                                                    ponderacion = consAnio[(consAnio.Count - 1)].ValorTotal / totParAnio[(totParAnio.Count - 1)].ValorTotal;
                                                                    ponderacionParcial += ponderacion;
                                                                    ponderacion = ponderacion * 100;
                                                                    ponderacionParcialPorcentaje += ponderacion;
                                                                }
                                                                DatosConsMes.Add(ponderacion.ToString("0.0"));
                                                                DatosConsMes.Add(consMes[j].ValorTotal.ToString());
                                                            }
                                                            else
                                                            {
                                                                DatosConsMes.Add(consMes[j].ValorTotal.ToString());
                                                            }
                                                            if (j == (consMes.Count - 1))
                                                            {
                                                                for (int k = 0; k < consAnio.Count; k++)
                                                                {
                                                                    DatosConsMes.Add(consAnio[k].ValorTotal.ToString());
                                                                }
                                                            }
                                                        }
                                                        else if (i == 1)
                                                        {
                                                            if (j == 0)
                                                            {
                                                                DatosConsMes.Add(consMes[j].TituloNac);
                                                            }
                                                            else if (j == 1)
                                                            {
                                                                DatosConsMes.Add("");
                                                                DatosConsMes.Add(consMes[j].ValorNac.ToString());
                                                            }
                                                            else
                                                            {
                                                                DatosConsMes.Add(consMes[j].ValorNac.ToString());
                                                            }
                                                            if (j == (consMes.Count - 1))
                                                            {
                                                                for (int k = 0; k < consAnio.Count; k++)
                                                                {
                                                                    DatosConsMes.Add(consAnio[k].ValorNac.ToString());
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (j == 0)
                                                            {
                                                                DatosConsMes.Add(consMes[j].TituloExt);
                                                            }
                                                            else if (j == 1)
                                                            {
                                                                DatosConsMes.Add("");
                                                                DatosConsMes.Add(consMes[j].ValorExt.ToString());
                                                            }
                                                            else
                                                            {
                                                                DatosConsMes.Add(consMes[j].ValorExt.ToString());
                                                            }
                                                            if (j == (consMes.Count - 1))
                                                            {
                                                                for (int k = 0; k < consAnio.Count; k++)
                                                                {
                                                                    DatosConsMes.Add(consAnio[k].ValorExt.ToString());
                                                                }
                                                            }
                                                        }
                                                    }

                                                    for (int j = 0; j < admMes.Count; j++)
                                                    {
                                                        if (i == 0)
                                                        {
                                                            if (j == 0)
                                                            {
                                                                DatosAdmMes.Add(admMes[j].Titulo);
                                                            }
                                                            else if (j == 1)
                                                            {
                                                                decimal ponderacion = 0;
                                                                if (!decimalIsZero(admAnio[(admAnio.Count - 1)].ValorTotal) && !decimalIsZero(totParAnio[(totParAnio.Count - 1)].ValorTotal))
                                                                {
                                                                    ponderacion = admAnio[(admAnio.Count - 1)].ValorTotal / totParAnio[(totParAnio.Count - 1)].ValorTotal;
                                                                    ponderacionParcial += ponderacion;
                                                                    ponderacion = ponderacion * 100;
                                                                    ponderacionParcialPorcentaje += ponderacion;
                                                                }
                                                                DatosAdmMes.Add(ponderacion.ToString("0.0"));
                                                                DatosAdmMes.Add(admMes[j].ValorTotal.ToString());
                                                            }
                                                            else
                                                            {
                                                                DatosAdmMes.Add(admMes[j].ValorTotal.ToString());
                                                            }
                                                            if (j == (admMes.Count - 1))
                                                            {
                                                                for (int k = 0; k < admAnio.Count; k++)
                                                                {
                                                                    DatosAdmMes.Add(admAnio[k].ValorTotal.ToString());
                                                                }
                                                            }
                                                        }
                                                        else if (i == 1)
                                                        {
                                                            if (j == 0)
                                                            {
                                                                DatosAdmMes.Add(admMes[j].TituloNac);
                                                            }
                                                            else if (j == 1)
                                                            {
                                                                DatosAdmMes.Add("");
                                                                DatosAdmMes.Add(admMes[j].ValorNac.ToString());
                                                            }
                                                            else
                                                            {
                                                                DatosAdmMes.Add(admMes[j].ValorNac.ToString());
                                                            }
                                                            if (j == (admMes.Count - 1))
                                                            {
                                                                for (int k = 0; k < admAnio.Count; k++)
                                                                {
                                                                    DatosAdmMes.Add(admAnio[k].ValorNac.ToString());
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (j == 0)
                                                            {
                                                                DatosAdmMes.Add(admMes[j].TituloExt);
                                                            }
                                                            else if (j == 1)
                                                            {
                                                                DatosAdmMes.Add("");
                                                                DatosAdmMes.Add(admMes[j].ValorExt.ToString());
                                                            }
                                                            else
                                                            {
                                                                DatosAdmMes.Add(admMes[j].ValorExt.ToString());
                                                            }
                                                            if (j == (admMes.Count - 1))
                                                            {
                                                                for (int k = 0; k < admAnio.Count; k++)
                                                                {
                                                                    DatosAdmMes.Add(admAnio[k].ValorExt.ToString());
                                                                }
                                                            }
                                                        }
                                                    }

                                                    for (int j = 0; j < contMes.Count; j++)
                                                    {
                                                        if (i == 0)
                                                        {
                                                            if (j == 0)
                                                            {
                                                                DatosContMes.Add(contMes[j].Titulo);
                                                            }
                                                            else if (j == 1)
                                                            {
                                                                decimal ponderacion = 0;
                                                                if (!decimalIsZero(contAnio[(contAnio.Count - 1)].ValorTotal) && !decimalIsZero(totParAnio[(totParAnio.Count - 1)].ValorTotal))
                                                                {
                                                                    ponderacion = contAnio[(contAnio.Count - 1)].ValorTotal / totParAnio[(totParAnio.Count - 1)].ValorTotal;
                                                                    ponderacionParcial += ponderacion;
                                                                    ponderacion = ponderacion * 100;
                                                                    ponderacionParcialPorcentaje += ponderacion;
                                                                }
                                                                DatosContMes.Add(ponderacion.ToString("0.0"));
                                                                DatosContMes.Add(contMes[j].ValorTotal.ToString());
                                                            }
                                                            else
                                                            {
                                                                DatosContMes.Add(contMes[j].ValorTotal.ToString());
                                                            }
                                                            if (j == (contMes.Count - 1))
                                                            {
                                                                for (int k = 0; k < contAnio.Count; k++)
                                                                {
                                                                    DatosContMes.Add(contAnio[k].ValorTotal.ToString());
                                                                }
                                                            }
                                                        }
                                                        else if (i == 1)
                                                        {
                                                            if (j == 0)
                                                            {
                                                                DatosContMes.Add(contMes[j].TituloNac);
                                                            }
                                                            else if (j == 1)
                                                            {
                                                                DatosContMes.Add("");
                                                                DatosContMes.Add(contMes[j].ValorNac.ToString());
                                                            }
                                                            else
                                                            {
                                                                DatosContMes.Add(contMes[j].ValorNac.ToString());
                                                            }
                                                            if (j == (contMes.Count - 1))
                                                            {
                                                                for (int k = 0; k < contAnio.Count; k++)
                                                                {
                                                                    DatosContMes.Add(contAnio[k].ValorNac.ToString());
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (j == 0)
                                                            {
                                                                DatosContMes.Add(contMes[j].TituloExt);
                                                            }
                                                            else if (j == 1)
                                                            {
                                                                DatosContMes.Add("");
                                                                DatosContMes.Add(contMes[j].ValorExt.ToString());
                                                            }
                                                            else
                                                            {
                                                                DatosContMes.Add(contMes[j].ValorExt.ToString());
                                                            }
                                                            if (j == (contMes.Count - 1))
                                                            {
                                                                for (int k = 0; k < contAnio.Count; k++)
                                                                {
                                                                    DatosContMes.Add(contAnio[k].ValorExt.ToString());
                                                                }
                                                            }
                                                        }
                                                    }

                                                    for (int j = 0; j < totParMes.Count; j++)
                                                    {
                                                        if (i == 0)
                                                        {
                                                            if (j == 0)
                                                            {
                                                                DatosTotParMes.Add(totParMes[j].Titulo);
                                                            }
                                                            else if (j == 1)
                                                            {
                                                                DatosTotParMes.Add(ponderacionParcialPorcentaje.ToString("0.0"));
                                                                DatosTotParMes.Add(totParMes[j].ValorTotal.ToString());
                                                            }
                                                            else
                                                            {
                                                                DatosTotParMes.Add(totParMes[j].ValorTotal.ToString());
                                                            }
                                                            if (j == (totParMes.Count - 1))
                                                            {
                                                                for (int k = 0; k < totParAnio.Count; k++)
                                                                {
                                                                    DatosTotParMes.Add(totParAnio[k].ValorTotal.ToString());
                                                                }
                                                            }
                                                        }
                                                        else if (i == 1)
                                                        {
                                                            if (j == 0)
                                                            {
                                                                DatosTotParMes.Add(totParMes[j].TituloNac);
                                                            }
                                                            else if (j == 1)
                                                            {
                                                                DatosTotParMes.Add("");
                                                                DatosTotParMes.Add(totParMes[j].ValorNac.ToString());
                                                            }
                                                            else
                                                            {
                                                                DatosTotParMes.Add(totParMes[j].ValorNac.ToString());
                                                            }
                                                            if (j == (totParMes.Count - 1))
                                                            {
                                                                for (int k = 0; k < totParAnio.Count; k++)
                                                                {
                                                                    DatosTotParMes.Add(totParAnio[k].ValorNac.ToString());
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (j == 0)
                                                            {
                                                                DatosTotParMes.Add(totParMes[j].TituloExt);
                                                            }
                                                            else if (j == 1)
                                                            {
                                                                DatosTotParMes.Add("");
                                                                DatosTotParMes.Add(totParMes[j].ValorExt.ToString());
                                                            }
                                                            else
                                                            {
                                                                DatosTotParMes.Add(totParMes[j].ValorExt.ToString());
                                                            }
                                                            if (j == (totParMes.Count - 1))
                                                            {
                                                                for (int k = 0; k < totParAnio.Count; k++)
                                                                {
                                                                    DatosTotParMes.Add(totParAnio[k].ValorExt.ToString());
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                for (int j = 0; j < totAcumMes.Count; j++)
                                                {
                                                    if (j == 0)
                                                    {
                                                        DatosTotAcumMes.Add(totAcumMes[j].Titulo);
                                                    }
                                                    else if (j == 1)
                                                    {
                                                        DatosTotAcumMes.Add(ponderacionParcialPorcentaje.ToString("0.0"));
                                                        DatosTotAcumMes.Add(totAcumMes[j].ValorTotal.ToString());
                                                    }
                                                    else
                                                    {
                                                        DatosTotAcumMes.Add(totAcumMes[j].ValorTotal.ToString());
                                                    }
                                                    if (j == (totAcumMes.Count - 1))
                                                    {
                                                        for (int k = 0; k < totAcumAnio.Count; k++)
                                                        {
                                                            DatosTotAcumMes.Add(totAcumAnio[k].ValorTotal.ToString());
                                                        }
                                                    }
                                                }
                                                int NumIngreso = 0;
                                                if ("1".Equals(tipoIniciativaOrientacionComercial))
                                                {
                                                    InsertarInformacionFinancieraCasoBaseParametroVN(objConnection, Datos, "", NumIngreso++);
                                                    InsertarInformacionFinancieraCasoBaseParametroVN(objConnection, DatosAdqMes, "", NumIngreso++);
                                                    InsertarInformacionFinancieraCasoBaseParametroVN(objConnection, DatosConsMes, "", NumIngreso++);
                                                    InsertarInformacionFinancieraCasoBaseParametroVN(objConnection, DatosAdmMes, "", NumIngreso++);
                                                    InsertarInformacionFinancieraCasoBaseParametroVN(objConnection, DatosContMes, "", NumIngreso++);
                                                    InsertarInformacionFinancieraCasoBaseParametroVN(objConnection, DatosTotParMes, "", NumIngreso++);
                                                    InsertarInformacionFinancieraCasoBaseParametroVN(objConnection, DatosTotAcumMes, "", NumIngreso++);
                                                }
                                                else
                                                {
                                                    InsertarInformacionFinancieraPresupuestoParametroVN(objConnection, Datos, "", NumIngreso++);
                                                    InsertarInformacionFinancieraPresupuestoParametroVN(objConnection, DatosAdqMes, "", NumIngreso++);
                                                    InsertarInformacionFinancieraPresupuestoParametroVN(objConnection, DatosConsMes, "", NumIngreso++);
                                                    InsertarInformacionFinancieraPresupuestoParametroVN(objConnection, DatosAdmMes, "", NumIngreso++);
                                                    InsertarInformacionFinancieraPresupuestoParametroVN(objConnection, DatosContMes, "", NumIngreso++);
                                                    InsertarInformacionFinancieraPresupuestoParametroVN(objConnection, DatosTotParMes, "", NumIngreso++);
                                                    InsertarInformacionFinancieraPresupuestoParametroVN(objConnection, DatosTotAcumMes, "", NumIngreso++);
                                                }
                                                if (!decimalIsZero(totParAnio[(totParAnio.Count - 1)].ValorTotal))
                                                {
                                                    valuePorcentajeInversionNacional = (totParAnio[(totParAnio.Count - 1)].ValorNac * ponderacionParcial) / totParAnio[(totParAnio.Count - 1)].ValorTotal;
                                                    valuePorcentajeInversionNacional = valuePorcentajeInversionNacional * 100;
                                                    valuePorcentajeInversionExtranjero = (totParAnio[(totParAnio.Count - 1)].ValorExt * ponderacionParcial) / totParAnio[(totParAnio.Count - 1)].ValorTotal;
                                                    valuePorcentajeInversionExtranjero = valuePorcentajeInversionExtranjero * 100;
                                                }
                                                InsertarInformacionAdicionalParametroVN(objConnection, keyCostoDuenoAnio, agrupadorCostoDueño, valueCostoDuenoAnio.ToString(), parametroVNToken, iniToken);
                                                InsertarInformacionAdicionalParametroVN(objConnection, keyCostoDuenoTotalCapex, agrupadorCostoDueño, valueCostoDuenoTotalCapex.ToString(), parametroVNToken, iniToken);
                                                InsertarInformacionAdicionalParametroVN(objConnection, keyContingenciaAnio, agrupadorContingencia, valueContingenciaAnio.ToString(), parametroVNToken, iniToken);
                                                InsertarInformacionAdicionalParametroVN(objConnection, keyContingenciaTotalCapex, agrupadorContingencia, valueContingenciaTotalCapex.ToString(), parametroVNToken, iniToken);
                                                InsertarInformacionAdicionalParametroVN(objConnection, keyPorcentajeInversionNacional, agrupadorPorcentajeInversion, valuePorcentajeInversionNacional.ToString("0.0"), parametroVNToken, iniToken);
                                                InsertarInformacionAdicionalParametroVN(objConnection, keyPorcentajeInversionExtranjero, agrupadorPorcentajeInversion, valuePorcentajeInversionExtranjero.ToString("0.0"), parametroVNToken, iniToken);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return Json(new { Mensaje = "false" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception err)
                    {
                        err.ToString();
                        return Json(new { Mensaje = "false" }, JsonRequestBehavior.AllowGet);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
        }

        private void InsertarInformacionAdicionalParametroVN(SqlConnection objConnection, string key, string groupBy, string value, string parametroVNToken, string iniToken)
        {
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("KeyInformacionAdicionalParametroVN", key);
                parametos.Add("NombreInformacionAdicionalParametroVN", groupBy);
                parametos.Add("ValueInformacionAdicionalParametroVN", value);
                parametos.Add("ParametroVNToken", parametroVNToken);
                parametos.Add("IniToken", iniToken);
                SqlMapper.Execute(objConnection, "CAPEX_INS_INFORMACION_ADICIONAL_PARAMETROVN", parametos, commandType: CommandType.StoredProcedure);
            }
            catch (Exception err)
            {
                ExceptionResult = "InsertarInformacionAdicionalParametroVN, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
            }
        }

        private void InsertarInformacionFinancieraCasoBaseParametroVN(List<String> Datos, String PorInvNacExt, int NumIngreso)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    var parametos = new DynamicParameters();
                    if (Datos.Count == 294)
                    {
                        parametos.Add("IniToken", cadenaLargoFijo(Datos[0].ToString(), 50));
                        parametos.Add("IniUsuario", cadenaLargoFijo(Datos[1].ToString(), 30));
                        parametos.Add("ParametroVNToken", cadenaLargoFijo(Datos[2].ToString(), 50));
                        parametos.Add("IfDato0", cadenaLargoFijo(Datos[3].ToString(), 50));
                        parametos.Add("IfDato1", cadenaLargoFijo(Datos[4].ToString(), 50));
                        parametos.Add("IfDato2", cadenaLargoFijo(Datos[5].ToString(), 50));

                        parametos.Add("IfDato3", cadenaLargoFijo(Datos[6].ToString(), 50));
                        parametos.Add("IfDato4", cadenaLargoFijo(Datos[7].ToString(), 50));
                        parametos.Add("IfDato5", cadenaLargoFijo(Datos[8].ToString(), 50));
                        parametos.Add("IfDato6", cadenaLargoFijo(Datos[9].ToString(), 50));
                        parametos.Add("IfDato7", cadenaLargoFijo(Datos[10].ToString(), 50));
                        parametos.Add("IfDato8", cadenaLargoFijo(Datos[11].ToString(), 50));
                        parametos.Add("IfDato9", cadenaLargoFijo(Datos[12].ToString(), 50));
                        parametos.Add("IfDato10", cadenaLargoFijo(Datos[13].ToString(), 50));
                        parametos.Add("IfDato11", cadenaLargoFijo(Datos[14].ToString(), 50));
                        parametos.Add("IfDato12", cadenaLargoFijo(Datos[15].ToString(), 50));
                        parametos.Add("IfDato13", cadenaLargoFijo(Datos[16].ToString(), 50));
                        parametos.Add("IfDato14", cadenaLargoFijo(Datos[17].ToString(), 50));

                        parametos.Add("IfDato15", cadenaLargoFijo(Datos[18].ToString(), 50));

                        parametos.Add("IfDato16", cadenaLargoFijo(Datos[19].ToString(), 50)); //2021 - 2030
                        parametos.Add("IfDato17", cadenaLargoFijo(Datos[20].ToString(), 50));
                        parametos.Add("IfDato18", cadenaLargoFijo(Datos[21].ToString(), 50));
                        parametos.Add("IfDato19", cadenaLargoFijo(Datos[22].ToString(), 50));
                        parametos.Add("IfDato20", cadenaLargoFijo(Datos[23].ToString(), 50));
                        parametos.Add("IfDato21", cadenaLargoFijo(Datos[24].ToString(), 50));
                        parametos.Add("IfDato22", cadenaLargoFijo(Datos[25].ToString(), 50));
                        parametos.Add("IfDato23", cadenaLargoFijo(Datos[26].ToString(), 50));
                        parametos.Add("IfDato24", cadenaLargoFijo(Datos[27].ToString(), 50));
                        parametos.Add("IfDato25", cadenaLargoFijo(Datos[28].ToString(), 50));

                        parametos.Add("IfDato26", cadenaLargoFijo(Datos[29].ToString(), 50)); //2031 - 2040
                        parametos.Add("IfDato27", cadenaLargoFijo(Datos[30].ToString(), 50));
                        parametos.Add("IfDato28", cadenaLargoFijo(Datos[31].ToString(), 50));
                        parametos.Add("IfDato29", cadenaLargoFijo(Datos[32].ToString(), 50));
                        parametos.Add("IfDato30", cadenaLargoFijo(Datos[33].ToString(), 50));
                        parametos.Add("IfDato31", cadenaLargoFijo(Datos[34].ToString(), 50));
                        parametos.Add("IfDato32", cadenaLargoFijo(Datos[35].ToString(), 50));
                        parametos.Add("IfDato33", cadenaLargoFijo(Datos[36].ToString(), 50));
                        parametos.Add("IfDato34", cadenaLargoFijo(Datos[37].ToString(), 50));
                        parametos.Add("IfDato35", cadenaLargoFijo(Datos[38].ToString(), 50));

                        parametos.Add("IfDato36", cadenaLargoFijo(Datos[39].ToString(), 50)); //2041 - 2050
                        parametos.Add("IfDato37", cadenaLargoFijo(Datos[40].ToString(), 50));
                        parametos.Add("IfDato38", cadenaLargoFijo(Datos[41].ToString(), 50));
                        parametos.Add("IfDato39", cadenaLargoFijo(Datos[42].ToString(), 50));
                        parametos.Add("IfDato40", cadenaLargoFijo(Datos[43].ToString(), 50));
                        parametos.Add("IfDato41", cadenaLargoFijo(Datos[44].ToString(), 50));
                        parametos.Add("IfDato42", cadenaLargoFijo(Datos[45].ToString(), 50));
                        parametos.Add("IfDato43", cadenaLargoFijo(Datos[46].ToString(), 50));
                        parametos.Add("IfDato44", cadenaLargoFijo(Datos[47].ToString(), 50));
                        parametos.Add("IfDato45", cadenaLargoFijo(Datos[48].ToString(), 50));

                        parametos.Add("IfDato46", cadenaLargoFijo(Datos[49].ToString(), 50)); //2051 - 2060
                        parametos.Add("IfDato47", cadenaLargoFijo(Datos[50].ToString(), 50));
                        parametos.Add("IfDato48", cadenaLargoFijo(Datos[51].ToString(), 50));
                        parametos.Add("IfDato49", cadenaLargoFijo(Datos[52].ToString(), 50));
                        parametos.Add("IfDato50", cadenaLargoFijo(Datos[53].ToString(), 50));
                        parametos.Add("IfDato51", cadenaLargoFijo(Datos[54].ToString(), 50));
                        parametos.Add("IfDato52", cadenaLargoFijo(Datos[55].ToString(), 50));
                        parametos.Add("IfDato53", cadenaLargoFijo(Datos[56].ToString(), 50));
                        parametos.Add("IfDato54", cadenaLargoFijo(Datos[57].ToString(), 50));
                        parametos.Add("IfDato55", cadenaLargoFijo(Datos[58].ToString(), 50));

                        parametos.Add("IfDato56", cadenaLargoFijo(Datos[59].ToString(), 50)); //2061 - 2070
                        parametos.Add("IfDato57", cadenaLargoFijo(Datos[60].ToString(), 50));
                        parametos.Add("IfDato58", cadenaLargoFijo(Datos[61].ToString(), 50));
                        parametos.Add("IfDato59", cadenaLargoFijo(Datos[62].ToString(), 50));
                        parametos.Add("IfDato60", cadenaLargoFijo(Datos[63].ToString(), 50));
                        parametos.Add("IfDato61", cadenaLargoFijo(Datos[64].ToString(), 50));
                        parametos.Add("IfDato62", cadenaLargoFijo(Datos[65].ToString(), 50));
                        parametos.Add("IfDato63", cadenaLargoFijo(Datos[66].ToString(), 50));
                        parametos.Add("IfDato64", cadenaLargoFijo(Datos[67].ToString(), 50));
                        parametos.Add("IfDato65", cadenaLargoFijo(Datos[68].ToString(), 50));

                        parametos.Add("IfDato66", cadenaLargoFijo(Datos[69].ToString(), 50)); //2071 - 2080
                        parametos.Add("IfDato67", cadenaLargoFijo(Datos[70].ToString(), 50));
                        parametos.Add("IfDato68", cadenaLargoFijo(Datos[71].ToString(), 50));
                        parametos.Add("IfDato69", cadenaLargoFijo(Datos[72].ToString(), 50));
                        parametos.Add("IfDato70", cadenaLargoFijo(Datos[73].ToString(), 50));
                        parametos.Add("IfDato71", cadenaLargoFijo(Datos[74].ToString(), 50));
                        parametos.Add("IfDato72", cadenaLargoFijo(Datos[75].ToString(), 50));
                        parametos.Add("IfDato73", cadenaLargoFijo(Datos[76].ToString(), 50));
                        parametos.Add("IfDato74", cadenaLargoFijo(Datos[77].ToString(), 50));
                        parametos.Add("IfDato75", cadenaLargoFijo(Datos[78].ToString(), 50));

                        parametos.Add("IfDato76", cadenaLargoFijo(Datos[79].ToString(), 50)); //2081 - 2090
                        parametos.Add("IfDato77", cadenaLargoFijo(Datos[80].ToString(), 50));
                        parametos.Add("IfDato78", cadenaLargoFijo(Datos[81].ToString(), 50));
                        parametos.Add("IfDato79", cadenaLargoFijo(Datos[82].ToString(), 50));
                        parametos.Add("IfDato80", cadenaLargoFijo(Datos[83].ToString(), 50));
                        parametos.Add("IfDato81", cadenaLargoFijo(Datos[84].ToString(), 50));
                        parametos.Add("IfDato82", cadenaLargoFijo(Datos[85].ToString(), 50));
                        parametos.Add("IfDato83", cadenaLargoFijo(Datos[86].ToString(), 50));
                        parametos.Add("IfDato84", cadenaLargoFijo(Datos[87].ToString(), 50));
                        parametos.Add("IfDato85", cadenaLargoFijo(Datos[88].ToString(), 50));

                        parametos.Add("IfDato86", cadenaLargoFijo(Datos[89].ToString(), 50)); //2091 - 2100
                        parametos.Add("IfDato87", cadenaLargoFijo(Datos[90].ToString(), 50));
                        parametos.Add("IfDato88", cadenaLargoFijo(Datos[91].ToString(), 50));
                        parametos.Add("IfDato89", cadenaLargoFijo(Datos[92].ToString(), 50));
                        parametos.Add("IfDato90", cadenaLargoFijo(Datos[93].ToString(), 50));
                        parametos.Add("IfDato91", cadenaLargoFijo(Datos[94].ToString(), 50));
                        parametos.Add("IfDato92", cadenaLargoFijo(Datos[95].ToString(), 50));
                        parametos.Add("IfDato93", cadenaLargoFijo(Datos[96].ToString(), 50));
                        parametos.Add("IfDato94", cadenaLargoFijo(Datos[97].ToString(), 50));
                        parametos.Add("IfDato95", cadenaLargoFijo(Datos[98].ToString(), 50));

                        parametos.Add("IfDato96", cadenaLargoFijo(Datos[99].ToString(), 50));//TOTAL

                        //fila 2
                        parametos.Add("IfD1Dato0", cadenaLargoFijo(Datos[100].ToString(), 50));
                        parametos.Add("IfD1Dato1", cadenaLargoFijo(Datos[101].ToString(), 50));
                        parametos.Add("IfD1Dato2", cadenaLargoFijo(Datos[102].ToString(), 50));
                        parametos.Add("IfD1Dato3", cadenaLargoFijo(Datos[103].ToString(), 50));
                        parametos.Add("IfD1Dato4", cadenaLargoFijo(Datos[104].ToString(), 50));
                        parametos.Add("IfD1Dato5", cadenaLargoFijo(Datos[105].ToString(), 50));
                        parametos.Add("IfD1Dato6", cadenaLargoFijo(Datos[106].ToString(), 50));
                        parametos.Add("IfD1Dato7", cadenaLargoFijo(Datos[107].ToString(), 50));
                        parametos.Add("IfD1Dato8", cadenaLargoFijo(Datos[108].ToString(), 50));
                        parametos.Add("IfD1Dato9", cadenaLargoFijo(Datos[109].ToString(), 50));
                        parametos.Add("IfD1Dato10", cadenaLargoFijo(Datos[110].ToString(), 50));
                        parametos.Add("IfD1Dato11", cadenaLargoFijo(Datos[111].ToString(), 50));
                        parametos.Add("IfD1Dato12", cadenaLargoFijo(Datos[112].ToString(), 50));
                        parametos.Add("IfD1Dato13", cadenaLargoFijo(Datos[113].ToString(), 50));
                        parametos.Add("IfD1Dato14", cadenaLargoFijo(Datos[114].ToString(), 50));

                        parametos.Add("IfD1Dato15", cadenaLargoFijo(Datos[115].ToString(), 50));

                        parametos.Add("IfD1Dato16", cadenaLargoFijo(Datos[116].ToString(), 50)); //2021 - 2030
                        parametos.Add("IfD1Dato17", cadenaLargoFijo(Datos[117].ToString(), 50));
                        parametos.Add("IfD1Dato18", cadenaLargoFijo(Datos[118].ToString(), 50));
                        parametos.Add("IfD1Dato19", cadenaLargoFijo(Datos[119].ToString(), 50));
                        parametos.Add("IfD1Dato20", cadenaLargoFijo(Datos[120].ToString(), 50));
                        parametos.Add("IfD1Dato21", cadenaLargoFijo(Datos[121].ToString(), 50));
                        parametos.Add("IfD1Dato22", cadenaLargoFijo(Datos[122].ToString(), 50));
                        parametos.Add("IfD1Dato23", cadenaLargoFijo(Datos[123].ToString(), 50));
                        parametos.Add("IfD1Dato24", cadenaLargoFijo(Datos[124].ToString(), 50));
                        parametos.Add("IfD1Dato25", cadenaLargoFijo(Datos[125].ToString(), 50));

                        parametos.Add("IfD1Dato26", cadenaLargoFijo(Datos[126].ToString(), 50)); //2031 - 2040
                        parametos.Add("IfD1Dato27", cadenaLargoFijo(Datos[127].ToString(), 50));
                        parametos.Add("IfD1Dato28", cadenaLargoFijo(Datos[128].ToString(), 50));
                        parametos.Add("IfD1Dato29", cadenaLargoFijo(Datos[129].ToString(), 50));
                        parametos.Add("IfD1Dato30", cadenaLargoFijo(Datos[130].ToString(), 50));
                        parametos.Add("IfD1Dato31", cadenaLargoFijo(Datos[131].ToString(), 50));
                        parametos.Add("IfD1Dato32", cadenaLargoFijo(Datos[132].ToString(), 50));
                        parametos.Add("IfD1Dato33", cadenaLargoFijo(Datos[133].ToString(), 50));
                        parametos.Add("IfD1Dato34", cadenaLargoFijo(Datos[134].ToString(), 50));
                        parametos.Add("IfD1Dato35", cadenaLargoFijo(Datos[135].ToString(), 50));

                        parametos.Add("IfD1Dato36", cadenaLargoFijo(Datos[136].ToString(), 50)); //2041 - 2050
                        parametos.Add("IfD1Dato37", cadenaLargoFijo(Datos[137].ToString(), 50));
                        parametos.Add("IfD1Dato38", cadenaLargoFijo(Datos[138].ToString(), 50));
                        parametos.Add("IfD1Dato39", cadenaLargoFijo(Datos[139].ToString(), 50));
                        parametos.Add("IfD1Dato40", cadenaLargoFijo(Datos[140].ToString(), 50));
                        parametos.Add("IfD1Dato41", cadenaLargoFijo(Datos[141].ToString(), 50));
                        parametos.Add("IfD1Dato42", cadenaLargoFijo(Datos[142].ToString(), 50));
                        parametos.Add("IfD1Dato43", cadenaLargoFijo(Datos[143].ToString(), 50));
                        parametos.Add("IfD1Dato44", cadenaLargoFijo(Datos[144].ToString(), 50));
                        parametos.Add("IfD1Dato45", cadenaLargoFijo(Datos[145].ToString(), 50));

                        parametos.Add("IfD1Dato46", cadenaLargoFijo(Datos[146].ToString(), 50)); //2051 - 2060
                        parametos.Add("IfD1Dato47", cadenaLargoFijo(Datos[147].ToString(), 50));
                        parametos.Add("IfD1Dato48", cadenaLargoFijo(Datos[148].ToString(), 50));
                        parametos.Add("IfD1Dato49", cadenaLargoFijo(Datos[149].ToString(), 50));
                        parametos.Add("IfD1Dato50", cadenaLargoFijo(Datos[150].ToString(), 50));
                        parametos.Add("IfD1Dato51", cadenaLargoFijo(Datos[151].ToString(), 50));
                        parametos.Add("IfD1Dato52", cadenaLargoFijo(Datos[152].ToString(), 50));
                        parametos.Add("IfD1Dato53", cadenaLargoFijo(Datos[153].ToString(), 50));
                        parametos.Add("IfD1Dato54", cadenaLargoFijo(Datos[154].ToString(), 50));
                        parametos.Add("IfD1Dato55", cadenaLargoFijo(Datos[155].ToString(), 50));

                        parametos.Add("IfD1Dato56", cadenaLargoFijo(Datos[156].ToString(), 50)); //2061 - 2070
                        parametos.Add("IfD1Dato57", cadenaLargoFijo(Datos[157].ToString(), 50));
                        parametos.Add("IfD1Dato58", cadenaLargoFijo(Datos[158].ToString(), 50));
                        parametos.Add("IfD1Dato59", cadenaLargoFijo(Datos[159].ToString(), 50));
                        parametos.Add("IfD1Dato60", cadenaLargoFijo(Datos[160].ToString(), 50));
                        parametos.Add("IfD1Dato61", cadenaLargoFijo(Datos[161].ToString(), 50));
                        parametos.Add("IfD1Dato62", cadenaLargoFijo(Datos[162].ToString(), 50));
                        parametos.Add("IfD1Dato63", cadenaLargoFijo(Datos[163].ToString(), 50));
                        parametos.Add("IfD1Dato64", cadenaLargoFijo(Datos[164].ToString(), 50));
                        parametos.Add("IfD1Dato65", cadenaLargoFijo(Datos[165].ToString(), 50));

                        parametos.Add("IfD1Dato66", cadenaLargoFijo(Datos[166].ToString(), 50)); //2071 - 2080
                        parametos.Add("IfD1Dato67", cadenaLargoFijo(Datos[167].ToString(), 50));
                        parametos.Add("IfD1Dato68", cadenaLargoFijo(Datos[168].ToString(), 50));
                        parametos.Add("IfD1Dato69", cadenaLargoFijo(Datos[169].ToString(), 50));
                        parametos.Add("IfD1Dato70", cadenaLargoFijo(Datos[170].ToString(), 50));
                        parametos.Add("IfD1Dato71", cadenaLargoFijo(Datos[171].ToString(), 50));
                        parametos.Add("IfD1Dato72", cadenaLargoFijo(Datos[172].ToString(), 50));
                        parametos.Add("IfD1Dato73", cadenaLargoFijo(Datos[173].ToString(), 50));
                        parametos.Add("IfD1Dato74", cadenaLargoFijo(Datos[174].ToString(), 50));
                        parametos.Add("IfD1Dato75", cadenaLargoFijo(Datos[175].ToString(), 50));

                        parametos.Add("IfD1Dato76", cadenaLargoFijo(Datos[176].ToString(), 50)); //2081 - 2090
                        parametos.Add("IfD1Dato77", cadenaLargoFijo(Datos[177].ToString(), 50));
                        parametos.Add("IfD1Dato78", cadenaLargoFijo(Datos[178].ToString(), 50));
                        parametos.Add("IfD1Dato79", cadenaLargoFijo(Datos[179].ToString(), 50));
                        parametos.Add("IfD1Dato80", cadenaLargoFijo(Datos[180].ToString(), 50));
                        parametos.Add("IfD1Dato81", cadenaLargoFijo(Datos[181].ToString(), 50));
                        parametos.Add("IfD1Dato82", cadenaLargoFijo(Datos[182].ToString(), 50));
                        parametos.Add("IfD1Dato83", cadenaLargoFijo(Datos[183].ToString(), 50));
                        parametos.Add("IfD1Dato84", cadenaLargoFijo(Datos[184].ToString(), 50));
                        parametos.Add("IfD1Dato85", cadenaLargoFijo(Datos[185].ToString(), 50));

                        parametos.Add("IfD1Dato86", cadenaLargoFijo(Datos[186].ToString(), 50)); //2091 - 2100
                        parametos.Add("IfD1Dato87", cadenaLargoFijo(Datos[187].ToString(), 50));
                        parametos.Add("IfD1Dato88", cadenaLargoFijo(Datos[188].ToString(), 50));
                        parametos.Add("IfD1Dato89", cadenaLargoFijo(Datos[189].ToString(), 50));
                        parametos.Add("IfD1Dato90", cadenaLargoFijo(Datos[190].ToString(), 50));
                        parametos.Add("IfD1Dato91", cadenaLargoFijo(Datos[191].ToString(), 50));
                        parametos.Add("IfD1Dato92", cadenaLargoFijo(Datos[192].ToString(), 50));
                        parametos.Add("IfD1Dato93", cadenaLargoFijo(Datos[193].ToString(), 50));
                        parametos.Add("IfD1Dato94", cadenaLargoFijo(Datos[194].ToString(), 50));
                        parametos.Add("IfD1Dato95", cadenaLargoFijo(Datos[195].ToString(), 50));

                        parametos.Add("IfD1Dato96", cadenaLargoFijo(Datos[196].ToString(), 50));//TOTAL
                        //fila 3
                        parametos.Add("IfD2Dato0", cadenaLargoFijo(Datos[197].ToString(), 50));
                        parametos.Add("IfD2Dato1", cadenaLargoFijo(Datos[198].ToString(), 50));
                        parametos.Add("IfD2Dato2", cadenaLargoFijo(Datos[199].ToString(), 50));

                        parametos.Add("IfD2Dato3", cadenaLargoFijo(Datos[200].ToString(), 50));
                        parametos.Add("IfD2Dato4", cadenaLargoFijo(Datos[201].ToString(), 50));
                        parametos.Add("IfD2Dato5", cadenaLargoFijo(Datos[202].ToString(), 50));
                        parametos.Add("IfD2Dato6", cadenaLargoFijo(Datos[203].ToString(), 50));
                        parametos.Add("IfD2Dato7", cadenaLargoFijo(Datos[204].ToString(), 50));
                        parametos.Add("IfD2Dato8", cadenaLargoFijo(Datos[205].ToString(), 50));
                        parametos.Add("IfD2Dato9", cadenaLargoFijo(Datos[206].ToString(), 50));
                        parametos.Add("IfD2Dato10", cadenaLargoFijo(Datos[207].ToString(), 50));
                        parametos.Add("IfD2Dato11", cadenaLargoFijo(Datos[208].ToString(), 50));
                        parametos.Add("IfD2Dato12", cadenaLargoFijo(Datos[209].ToString(), 50));
                        parametos.Add("IfD2Dato13", cadenaLargoFijo(Datos[210].ToString(), 50));
                        parametos.Add("IfD2Dato14", cadenaLargoFijo(Datos[211].ToString(), 50));

                        parametos.Add("IfD2Dato15", cadenaLargoFijo(Datos[212].ToString(), 50));

                        parametos.Add("IfD2Dato16", cadenaLargoFijo(Datos[213].ToString(), 50)); //2021 - 2030
                        parametos.Add("IfD2Dato17", cadenaLargoFijo(Datos[214].ToString(), 50));
                        parametos.Add("IfD2Dato18", cadenaLargoFijo(Datos[215].ToString(), 50));
                        parametos.Add("IfD2Dato19", cadenaLargoFijo(Datos[216].ToString(), 50));
                        parametos.Add("IfD2Dato20", cadenaLargoFijo(Datos[217].ToString(), 50));
                        parametos.Add("IfD2Dato21", cadenaLargoFijo(Datos[218].ToString(), 50));
                        parametos.Add("IfD2Dato22", cadenaLargoFijo(Datos[219].ToString(), 50));
                        parametos.Add("IfD2Dato23", cadenaLargoFijo(Datos[220].ToString(), 50));
                        parametos.Add("IfD2Dato24", cadenaLargoFijo(Datos[221].ToString(), 50));
                        parametos.Add("IfD2Dato25", cadenaLargoFijo(Datos[222].ToString(), 50));

                        parametos.Add("IfD2Dato26", cadenaLargoFijo(Datos[223].ToString(), 50)); //2031 - 2040
                        parametos.Add("IfD2Dato27", cadenaLargoFijo(Datos[224].ToString(), 50));
                        parametos.Add("IfD2Dato28", cadenaLargoFijo(Datos[225].ToString(), 50));
                        parametos.Add("IfD2Dato29", cadenaLargoFijo(Datos[226].ToString(), 50));
                        parametos.Add("IfD2Dato30", cadenaLargoFijo(Datos[227].ToString(), 50));
                        parametos.Add("IfD2Dato31", cadenaLargoFijo(Datos[228].ToString(), 50));
                        parametos.Add("IfD2Dato32", cadenaLargoFijo(Datos[229].ToString(), 50));
                        parametos.Add("IfD2Dato33", cadenaLargoFijo(Datos[230].ToString(), 50));
                        parametos.Add("IfD2Dato34", cadenaLargoFijo(Datos[231].ToString(), 50));
                        parametos.Add("IfD2Dato35", cadenaLargoFijo(Datos[232].ToString(), 50));

                        parametos.Add("IfD2Dato36", cadenaLargoFijo(Datos[233].ToString(), 50)); //2041 - 2050
                        parametos.Add("IfD2Dato37", cadenaLargoFijo(Datos[234].ToString(), 50));
                        parametos.Add("IfD2Dato38", cadenaLargoFijo(Datos[235].ToString(), 50));
                        parametos.Add("IfD2Dato39", cadenaLargoFijo(Datos[236].ToString(), 50));
                        parametos.Add("IfD2Dato40", cadenaLargoFijo(Datos[237].ToString(), 50));
                        parametos.Add("IfD2Dato41", cadenaLargoFijo(Datos[238].ToString(), 50));
                        parametos.Add("IfD2Dato42", cadenaLargoFijo(Datos[239].ToString(), 50));
                        parametos.Add("IfD2Dato43", cadenaLargoFijo(Datos[240].ToString(), 50));
                        parametos.Add("IfD2Dato44", cadenaLargoFijo(Datos[241].ToString(), 50));
                        parametos.Add("IfD2Dato45", cadenaLargoFijo(Datos[242].ToString(), 50));

                        parametos.Add("IfD2Dato46", cadenaLargoFijo(Datos[243].ToString(), 50)); //2051 - 2060
                        parametos.Add("IfD2Dato47", cadenaLargoFijo(Datos[244].ToString(), 50));
                        parametos.Add("IfD2Dato48", cadenaLargoFijo(Datos[245].ToString(), 50));
                        parametos.Add("IfD2Dato49", cadenaLargoFijo(Datos[246].ToString(), 50));
                        parametos.Add("IfD2Dato50", cadenaLargoFijo(Datos[247].ToString(), 50));
                        parametos.Add("IfD2Dato51", cadenaLargoFijo(Datos[248].ToString(), 50));
                        parametos.Add("IfD2Dato52", cadenaLargoFijo(Datos[249].ToString(), 50));
                        parametos.Add("IfD2Dato53", cadenaLargoFijo(Datos[250].ToString(), 50));
                        parametos.Add("IfD2Dato54", cadenaLargoFijo(Datos[251].ToString(), 50));
                        parametos.Add("IfD2Dato55", cadenaLargoFijo(Datos[252].ToString(), 50));

                        parametos.Add("IfD2Dato56", cadenaLargoFijo(Datos[253].ToString(), 50)); //2061 - 2070
                        parametos.Add("IfD2Dato57", cadenaLargoFijo(Datos[254].ToString(), 50));
                        parametos.Add("IfD2Dato58", cadenaLargoFijo(Datos[255].ToString(), 50));
                        parametos.Add("IfD2Dato59", cadenaLargoFijo(Datos[256].ToString(), 50));
                        parametos.Add("IfD2Dato60", cadenaLargoFijo(Datos[257].ToString(), 50));
                        parametos.Add("IfD2Dato61", cadenaLargoFijo(Datos[258].ToString(), 50));
                        parametos.Add("IfD2Dato62", cadenaLargoFijo(Datos[259].ToString(), 50));
                        parametos.Add("IfD2Dato63", cadenaLargoFijo(Datos[260].ToString(), 50));
                        parametos.Add("IfD2Dato64", cadenaLargoFijo(Datos[261].ToString(), 50));
                        parametos.Add("IfD2Dato65", cadenaLargoFijo(Datos[262].ToString(), 50));

                        parametos.Add("IfD2Dato66", cadenaLargoFijo(Datos[263].ToString(), 50)); //2071 - 2080
                        parametos.Add("IfD2Dato67", cadenaLargoFijo(Datos[264].ToString(), 50));
                        parametos.Add("IfD2Dato68", cadenaLargoFijo(Datos[265].ToString(), 50));
                        parametos.Add("IfD2Dato69", cadenaLargoFijo(Datos[266].ToString(), 50));
                        parametos.Add("IfD2Dato70", cadenaLargoFijo(Datos[267].ToString(), 50));
                        parametos.Add("IfD2Dato71", cadenaLargoFijo(Datos[268].ToString(), 50));
                        parametos.Add("IfD2Dato72", cadenaLargoFijo(Datos[269].ToString(), 50));
                        parametos.Add("IfD2Dato73", cadenaLargoFijo(Datos[270].ToString(), 50));
                        parametos.Add("IfD2Dato74", cadenaLargoFijo(Datos[271].ToString(), 50));
                        parametos.Add("IfD2Dato75", cadenaLargoFijo(Datos[272].ToString(), 50));

                        parametos.Add("IfD2Dato76", cadenaLargoFijo(Datos[273].ToString(), 50)); //2081 - 2090
                        parametos.Add("IfD2Dato77", cadenaLargoFijo(Datos[274].ToString(), 50));
                        parametos.Add("IfD2Dato78", cadenaLargoFijo(Datos[275].ToString(), 50));
                        parametos.Add("IfD2Dato79", cadenaLargoFijo(Datos[276].ToString(), 50));
                        parametos.Add("IfD2Dato80", cadenaLargoFijo(Datos[277].ToString(), 50));
                        parametos.Add("IfD2Dato81", cadenaLargoFijo(Datos[278].ToString(), 50));
                        parametos.Add("IfD2Dato82", cadenaLargoFijo(Datos[279].ToString(), 50));
                        parametos.Add("IfD2Dato83", cadenaLargoFijo(Datos[280].ToString(), 50));
                        parametos.Add("IfD2Dato84", cadenaLargoFijo(Datos[281].ToString(), 50));
                        parametos.Add("IfD2Dato85", cadenaLargoFijo(Datos[282].ToString(), 50));

                        parametos.Add("IfD2Dato86", cadenaLargoFijo(Datos[283].ToString(), 50)); //2091 - 2100
                        parametos.Add("IfD2Dato87", cadenaLargoFijo(Datos[284].ToString(), 50));
                        parametos.Add("IfD2Dato88", cadenaLargoFijo(Datos[285].ToString(), 50));
                        parametos.Add("IfD2Dato89", cadenaLargoFijo(Datos[286].ToString(), 50));
                        parametos.Add("IfD2Dato90", cadenaLargoFijo(Datos[287].ToString(), 50));
                        parametos.Add("IfD2Dato91", cadenaLargoFijo(Datos[288].ToString(), 50));
                        parametos.Add("IfD2Dato92", cadenaLargoFijo(Datos[289].ToString(), 50));
                        parametos.Add("IfD2Dato93", cadenaLargoFijo(Datos[290].ToString(), 50));
                        parametos.Add("IfD2Dato94", cadenaLargoFijo(Datos[291].ToString(), 50));
                        parametos.Add("IfD2Dato95", cadenaLargoFijo(Datos[292].ToString(), 50));

                        parametos.Add("IfD2Dato96", cadenaLargoFijo(Datos[293].ToString(), 50));//TOTAL
                        parametos.Add("PorInvNacExt", cadenaLargoFijo(PorInvNacExt, 10));
                        parametos.Add("Opcion", 1);
                        parametos.Add("NumIngreso", NumIngreso);
                    }
                    else
                    {
                        parametos.Add("IniToken", cadenaLargoFijo(Datos[0].ToString(), 50));
                        parametos.Add("IniUsuario", cadenaLargoFijo(Datos[1].ToString(), 30));
                        parametos.Add("ParametroVNToken", cadenaLargoFijo(Datos[2].ToString(), 50));
                        parametos.Add("IfDato0", cadenaLargoFijo(Datos[3].ToString(), 50));
                        parametos.Add("IfDato1", cadenaLargoFijo(Datos[4].ToString(), 50));
                        parametos.Add("IfDato2", cadenaLargoFijo(Datos[5].ToString(), 50));

                        parametos.Add("IfDato3", cadenaLargoFijo(Datos[6].ToString(), 50));
                        parametos.Add("IfDato4", cadenaLargoFijo(Datos[7].ToString(), 50));
                        parametos.Add("IfDato5", cadenaLargoFijo(Datos[8].ToString(), 50));
                        parametos.Add("IfDato6", cadenaLargoFijo(Datos[9].ToString(), 50));
                        parametos.Add("IfDato7", cadenaLargoFijo(Datos[10].ToString(), 50));
                        parametos.Add("IfDato8", cadenaLargoFijo(Datos[11].ToString(), 50));
                        parametos.Add("IfDato9", cadenaLargoFijo(Datos[12].ToString(), 50));
                        parametos.Add("IfDato10", cadenaLargoFijo(Datos[13].ToString(), 50));
                        parametos.Add("IfDato11", cadenaLargoFijo(Datos[14].ToString(), 50));
                        parametos.Add("IfDato12", cadenaLargoFijo(Datos[15].ToString(), 50));
                        parametos.Add("IfDato13", cadenaLargoFijo(Datos[16].ToString(), 50));
                        parametos.Add("IfDato14", cadenaLargoFijo(Datos[17].ToString(), 50));

                        parametos.Add("IfDato15", cadenaLargoFijo(Datos[18].ToString(), 50));

                        parametos.Add("IfDato16", cadenaLargoFijo(Datos[19].ToString(), 50)); //2021 - 2030
                        parametos.Add("IfDato17", cadenaLargoFijo(Datos[20].ToString(), 50));
                        parametos.Add("IfDato18", cadenaLargoFijo(Datos[21].ToString(), 50));
                        parametos.Add("IfDato19", cadenaLargoFijo(Datos[22].ToString(), 50));
                        parametos.Add("IfDato20", cadenaLargoFijo(Datos[23].ToString(), 50));
                        parametos.Add("IfDato21", cadenaLargoFijo(Datos[24].ToString(), 50));
                        parametos.Add("IfDato22", cadenaLargoFijo(Datos[25].ToString(), 50));
                        parametos.Add("IfDato23", cadenaLargoFijo(Datos[26].ToString(), 50));
                        parametos.Add("IfDato24", cadenaLargoFijo(Datos[27].ToString(), 50));
                        parametos.Add("IfDato25", cadenaLargoFijo(Datos[28].ToString(), 50));

                        parametos.Add("IfDato26", cadenaLargoFijo(Datos[29].ToString(), 50)); //2031 - 2040
                        parametos.Add("IfDato27", cadenaLargoFijo(Datos[30].ToString(), 50));
                        parametos.Add("IfDato28", cadenaLargoFijo(Datos[31].ToString(), 50));
                        parametos.Add("IfDato29", cadenaLargoFijo(Datos[32].ToString(), 50));
                        parametos.Add("IfDato30", cadenaLargoFijo(Datos[33].ToString(), 50));
                        parametos.Add("IfDato31", cadenaLargoFijo(Datos[34].ToString(), 50));
                        parametos.Add("IfDato32", cadenaLargoFijo(Datos[35].ToString(), 50));
                        parametos.Add("IfDato33", cadenaLargoFijo(Datos[36].ToString(), 50));
                        parametos.Add("IfDato34", cadenaLargoFijo(Datos[37].ToString(), 50));
                        parametos.Add("IfDato35", cadenaLargoFijo(Datos[38].ToString(), 50));

                        parametos.Add("IfDato36", cadenaLargoFijo(Datos[39].ToString(), 50)); //2041 - 2050
                        parametos.Add("IfDato37", cadenaLargoFijo(Datos[40].ToString(), 50));
                        parametos.Add("IfDato38", cadenaLargoFijo(Datos[41].ToString(), 50));
                        parametos.Add("IfDato39", cadenaLargoFijo(Datos[42].ToString(), 50));
                        parametos.Add("IfDato40", cadenaLargoFijo(Datos[43].ToString(), 50));
                        parametos.Add("IfDato41", cadenaLargoFijo(Datos[44].ToString(), 50));
                        parametos.Add("IfDato42", cadenaLargoFijo(Datos[45].ToString(), 50));
                        parametos.Add("IfDato43", cadenaLargoFijo(Datos[46].ToString(), 50));
                        parametos.Add("IfDato44", cadenaLargoFijo(Datos[47].ToString(), 50));
                        parametos.Add("IfDato45", cadenaLargoFijo(Datos[48].ToString(), 50));

                        parametos.Add("IfDato46", cadenaLargoFijo(Datos[49].ToString(), 50)); //2051 - 2060
                        parametos.Add("IfDato47", cadenaLargoFijo(Datos[50].ToString(), 50));
                        parametos.Add("IfDato48", cadenaLargoFijo(Datos[51].ToString(), 50));
                        parametos.Add("IfDato49", cadenaLargoFijo(Datos[52].ToString(), 50));
                        parametos.Add("IfDato50", cadenaLargoFijo(Datos[53].ToString(), 50));
                        parametos.Add("IfDato51", cadenaLargoFijo(Datos[54].ToString(), 50));
                        parametos.Add("IfDato52", cadenaLargoFijo(Datos[55].ToString(), 50));
                        parametos.Add("IfDato53", cadenaLargoFijo(Datos[56].ToString(), 50));
                        parametos.Add("IfDato54", cadenaLargoFijo(Datos[57].ToString(), 50));
                        parametos.Add("IfDato55", cadenaLargoFijo(Datos[58].ToString(), 50));

                        parametos.Add("IfDato56", cadenaLargoFijo(Datos[59].ToString(), 50)); //2061 - 2070
                        parametos.Add("IfDato57", cadenaLargoFijo(Datos[60].ToString(), 50));
                        parametos.Add("IfDato58", cadenaLargoFijo(Datos[61].ToString(), 50));
                        parametos.Add("IfDato59", cadenaLargoFijo(Datos[62].ToString(), 50));
                        parametos.Add("IfDato60", cadenaLargoFijo(Datos[63].ToString(), 50));
                        parametos.Add("IfDato61", cadenaLargoFijo(Datos[64].ToString(), 50));
                        parametos.Add("IfDato62", cadenaLargoFijo(Datos[65].ToString(), 50));
                        parametos.Add("IfDato63", cadenaLargoFijo(Datos[66].ToString(), 50));
                        parametos.Add("IfDato64", cadenaLargoFijo(Datos[67].ToString(), 50));
                        parametos.Add("IfDato65", cadenaLargoFijo(Datos[68].ToString(), 50));

                        parametos.Add("IfDato66", cadenaLargoFijo(Datos[69].ToString(), 50)); //2071 - 2080
                        parametos.Add("IfDato67", cadenaLargoFijo(Datos[70].ToString(), 50));
                        parametos.Add("IfDato68", cadenaLargoFijo(Datos[71].ToString(), 50));
                        parametos.Add("IfDato69", cadenaLargoFijo(Datos[72].ToString(), 50));
                        parametos.Add("IfDato70", cadenaLargoFijo(Datos[73].ToString(), 50));
                        parametos.Add("IfDato71", cadenaLargoFijo(Datos[74].ToString(), 50));
                        parametos.Add("IfDato72", cadenaLargoFijo(Datos[75].ToString(), 50));
                        parametos.Add("IfDato73", cadenaLargoFijo(Datos[76].ToString(), 50));
                        parametos.Add("IfDato74", cadenaLargoFijo(Datos[77].ToString(), 50));
                        parametos.Add("IfDato75", cadenaLargoFijo(Datos[78].ToString(), 50));

                        parametos.Add("IfDato76", cadenaLargoFijo(Datos[79].ToString(), 50)); //2081 - 2090
                        parametos.Add("IfDato77", cadenaLargoFijo(Datos[80].ToString(), 50));
                        parametos.Add("IfDato78", cadenaLargoFijo(Datos[81].ToString(), 50));
                        parametos.Add("IfDato79", cadenaLargoFijo(Datos[82].ToString(), 50));
                        parametos.Add("IfDato80", cadenaLargoFijo(Datos[83].ToString(), 50));
                        parametos.Add("IfDato81", cadenaLargoFijo(Datos[84].ToString(), 50));
                        parametos.Add("IfDato82", cadenaLargoFijo(Datos[85].ToString(), 50));
                        parametos.Add("IfDato83", cadenaLargoFijo(Datos[86].ToString(), 50));
                        parametos.Add("IfDato84", cadenaLargoFijo(Datos[87].ToString(), 50));
                        parametos.Add("IfDato85", cadenaLargoFijo(Datos[88].ToString(), 50));

                        parametos.Add("IfDato86", cadenaLargoFijo(Datos[89].ToString(), 50)); //2091 - 2100
                        parametos.Add("IfDato87", cadenaLargoFijo(Datos[90].ToString(), 50));
                        parametos.Add("IfDato88", cadenaLargoFijo(Datos[91].ToString(), 50));
                        parametos.Add("IfDato89", cadenaLargoFijo(Datos[92].ToString(), 50));
                        parametos.Add("IfDato90", cadenaLargoFijo(Datos[93].ToString(), 50));
                        parametos.Add("IfDato91", cadenaLargoFijo(Datos[94].ToString(), 50));
                        parametos.Add("IfDato92", cadenaLargoFijo(Datos[95].ToString(), 50));
                        parametos.Add("IfDato93", cadenaLargoFijo(Datos[96].ToString(), 50));
                        parametos.Add("IfDato94", cadenaLargoFijo(Datos[97].ToString(), 50));
                        parametos.Add("IfDato95", cadenaLargoFijo(Datos[98].ToString(), 50));

                        parametos.Add("IfDato96", cadenaLargoFijo(Datos[99].ToString(), 50));//TOTAL
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
                        parametos.Add("PorInvNacExt", cadenaLargoFijo(PorInvNacExt, 10));
                        parametos.Add("Opcion", 0);
                        parametos.Add("NumIngreso", NumIngreso);
                    }
                    SqlMapper.Execute(objConnection, "CAPEX_INS_INFORMACION_FINANCIERA_CASOBASE_2_PARAMETROVN", parametos, commandType: CommandType.StoredProcedure);
                }
                catch (Exception err)
                {
                    ExceptionResult = "InsertarInformacionFinancieraCasoBaseParametroVN, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        private void InsertarInformacionFinancieraCasoBaseParametroVN(SqlConnection objConnection, List<String> Datos, String PorInvNacExt, int NumIngreso)
        {
            try
            {
                if (Datos.Count == 294)
                {
                    var parametrosCompletos = new DynamicParameters();
                    parametrosCompletos.Add("IniToken", cadenaLargoFijo(Datos[0].ToString(), 50));
                    parametrosCompletos.Add("IniUsuario", cadenaLargoFijo(Datos[1].ToString(), 30));
                    parametrosCompletos.Add("ParametroVNToken", cadenaLargoFijo(Datos[2].ToString(), 50));
                    parametrosCompletos.Add("IfDato0", cadenaLargoFijo(Datos[3].ToString(), 50));
                    parametrosCompletos.Add("IfDato1", cadenaLargoFijo(Datos[4].ToString(), 50));
                    parametrosCompletos.Add("IfDato2", cadenaLargoFijo(Datos[5].ToString(), 50));
                    parametrosCompletos.Add("IfDato3", cadenaLargoFijo(Datos[6].ToString(), 50));
                    parametrosCompletos.Add("IfDato4", cadenaLargoFijo(Datos[7].ToString(), 50));
                    parametrosCompletos.Add("IfDato5", cadenaLargoFijo(Datos[8].ToString(), 50));
                    parametrosCompletos.Add("IfDato6", cadenaLargoFijo(Datos[9].ToString(), 50));
                    parametrosCompletos.Add("IfDato7", cadenaLargoFijo(Datos[10].ToString(), 50));
                    parametrosCompletos.Add("IfDato8", cadenaLargoFijo(Datos[11].ToString(), 50));
                    parametrosCompletos.Add("IfDato9", cadenaLargoFijo(Datos[12].ToString(), 50));
                    parametrosCompletos.Add("IfDato10", cadenaLargoFijo(Datos[13].ToString(), 50));
                    parametrosCompletos.Add("IfDato11", cadenaLargoFijo(Datos[14].ToString(), 50));
                    parametrosCompletos.Add("IfDato12", cadenaLargoFijo(Datos[15].ToString(), 50));
                    parametrosCompletos.Add("IfDato13", cadenaLargoFijo(Datos[16].ToString(), 50));
                    parametrosCompletos.Add("IfDato14", cadenaLargoFijo(Datos[17].ToString(), 50));
                    parametrosCompletos.Add("IfDato15", cadenaLargoFijo(Datos[18].ToString(), 50));
                    parametrosCompletos.Add("IfDato16", cadenaLargoFijo(Datos[19].ToString(), 50)); //2021 - 2030
                    parametrosCompletos.Add("IfDato17", cadenaLargoFijo(Datos[20].ToString(), 50));
                    parametrosCompletos.Add("IfDato18", cadenaLargoFijo(Datos[21].ToString(), 50));
                    parametrosCompletos.Add("IfDato19", cadenaLargoFijo(Datos[22].ToString(), 50));
                    parametrosCompletos.Add("IfDato20", cadenaLargoFijo(Datos[23].ToString(), 50));
                    parametrosCompletos.Add("IfDato21", cadenaLargoFijo(Datos[24].ToString(), 50));
                    parametrosCompletos.Add("IfDato22", cadenaLargoFijo(Datos[25].ToString(), 50));
                    parametrosCompletos.Add("IfDato23", cadenaLargoFijo(Datos[26].ToString(), 50));
                    parametrosCompletos.Add("IfDato24", cadenaLargoFijo(Datos[27].ToString(), 50));
                    parametrosCompletos.Add("IfDato25", cadenaLargoFijo(Datos[28].ToString(), 50));
                    parametrosCompletos.Add("IfDato26", cadenaLargoFijo(Datos[29].ToString(), 50)); //2031 - 2040
                    parametrosCompletos.Add("IfDato27", cadenaLargoFijo(Datos[30].ToString(), 50));
                    parametrosCompletos.Add("IfDato28", cadenaLargoFijo(Datos[31].ToString(), 50));
                    parametrosCompletos.Add("IfDato29", cadenaLargoFijo(Datos[32].ToString(), 50));
                    parametrosCompletos.Add("IfDato30", cadenaLargoFijo(Datos[33].ToString(), 50));
                    parametrosCompletos.Add("IfDato31", cadenaLargoFijo(Datos[34].ToString(), 50));
                    parametrosCompletos.Add("IfDato32", cadenaLargoFijo(Datos[35].ToString(), 50));
                    parametrosCompletos.Add("IfDato33", cadenaLargoFijo(Datos[36].ToString(), 50));
                    parametrosCompletos.Add("IfDato34", cadenaLargoFijo(Datos[37].ToString(), 50));
                    parametrosCompletos.Add("IfDato35", cadenaLargoFijo(Datos[38].ToString(), 50));
                    parametrosCompletos.Add("IfDato36", cadenaLargoFijo(Datos[39].ToString(), 50)); //2041 - 2050
                    parametrosCompletos.Add("IfDato37", cadenaLargoFijo(Datos[40].ToString(), 50));
                    parametrosCompletos.Add("IfDato38", cadenaLargoFijo(Datos[41].ToString(), 50));
                    parametrosCompletos.Add("IfDato39", cadenaLargoFijo(Datos[42].ToString(), 50));
                    parametrosCompletos.Add("IfDato40", cadenaLargoFijo(Datos[43].ToString(), 50));
                    parametrosCompletos.Add("IfDato41", cadenaLargoFijo(Datos[44].ToString(), 50));
                    parametrosCompletos.Add("IfDato42", cadenaLargoFijo(Datos[45].ToString(), 50));
                    parametrosCompletos.Add("IfDato43", cadenaLargoFijo(Datos[46].ToString(), 50));
                    parametrosCompletos.Add("IfDato44", cadenaLargoFijo(Datos[47].ToString(), 50));
                    parametrosCompletos.Add("IfDato45", cadenaLargoFijo(Datos[48].ToString(), 50));
                    parametrosCompletos.Add("IfDato46", cadenaLargoFijo(Datos[49].ToString(), 50)); //2051 - 2060
                    parametrosCompletos.Add("IfDato47", cadenaLargoFijo(Datos[50].ToString(), 50));
                    parametrosCompletos.Add("IfDato48", cadenaLargoFijo(Datos[51].ToString(), 50));
                    parametrosCompletos.Add("IfDato49", cadenaLargoFijo(Datos[52].ToString(), 50));
                    parametrosCompletos.Add("IfDato50", cadenaLargoFijo(Datos[53].ToString(), 50));
                    parametrosCompletos.Add("IfDato51", cadenaLargoFijo(Datos[54].ToString(), 50));
                    parametrosCompletos.Add("IfDato52", cadenaLargoFijo(Datos[55].ToString(), 50));
                    parametrosCompletos.Add("IfDato53", cadenaLargoFijo(Datos[56].ToString(), 50));
                    parametrosCompletos.Add("IfDato54", cadenaLargoFijo(Datos[57].ToString(), 50));
                    parametrosCompletos.Add("IfDato55", cadenaLargoFijo(Datos[58].ToString(), 50));
                    parametrosCompletos.Add("IfDato56", cadenaLargoFijo(Datos[59].ToString(), 50)); //2061 - 2070
                    parametrosCompletos.Add("IfDato57", cadenaLargoFijo(Datos[60].ToString(), 50));
                    parametrosCompletos.Add("IfDato58", cadenaLargoFijo(Datos[61].ToString(), 50));
                    parametrosCompletos.Add("IfDato59", cadenaLargoFijo(Datos[62].ToString(), 50));
                    parametrosCompletos.Add("IfDato60", cadenaLargoFijo(Datos[63].ToString(), 50));
                    parametrosCompletos.Add("IfDato61", cadenaLargoFijo(Datos[64].ToString(), 50));
                    parametrosCompletos.Add("IfDato62", cadenaLargoFijo(Datos[65].ToString(), 50));
                    parametrosCompletos.Add("IfDato63", cadenaLargoFijo(Datos[66].ToString(), 50));
                    parametrosCompletos.Add("IfDato64", cadenaLargoFijo(Datos[67].ToString(), 50));
                    parametrosCompletos.Add("IfDato65", cadenaLargoFijo(Datos[68].ToString(), 50));
                    parametrosCompletos.Add("IfDato66", cadenaLargoFijo(Datos[69].ToString(), 50)); //2071 - 2080
                    parametrosCompletos.Add("IfDato67", cadenaLargoFijo(Datos[70].ToString(), 50));
                    parametrosCompletos.Add("IfDato68", cadenaLargoFijo(Datos[71].ToString(), 50));
                    parametrosCompletos.Add("IfDato69", cadenaLargoFijo(Datos[72].ToString(), 50));
                    parametrosCompletos.Add("IfDato70", cadenaLargoFijo(Datos[73].ToString(), 50));
                    parametrosCompletos.Add("IfDato71", cadenaLargoFijo(Datos[74].ToString(), 50));
                    parametrosCompletos.Add("IfDato72", cadenaLargoFijo(Datos[75].ToString(), 50));
                    parametrosCompletos.Add("IfDato73", cadenaLargoFijo(Datos[76].ToString(), 50));
                    parametrosCompletos.Add("IfDato74", cadenaLargoFijo(Datos[77].ToString(), 50));
                    parametrosCompletos.Add("IfDato75", cadenaLargoFijo(Datos[78].ToString(), 50));
                    parametrosCompletos.Add("IfDato76", cadenaLargoFijo(Datos[79].ToString(), 50)); //2081 - 2090
                    parametrosCompletos.Add("IfDato77", cadenaLargoFijo(Datos[80].ToString(), 50));
                    parametrosCompletos.Add("IfDato78", cadenaLargoFijo(Datos[81].ToString(), 50));
                    parametrosCompletos.Add("IfDato79", cadenaLargoFijo(Datos[82].ToString(), 50));
                    parametrosCompletos.Add("IfDato80", cadenaLargoFijo(Datos[83].ToString(), 50));
                    parametrosCompletos.Add("IfDato81", cadenaLargoFijo(Datos[84].ToString(), 50));
                    parametrosCompletos.Add("IfDato82", cadenaLargoFijo(Datos[85].ToString(), 50));
                    parametrosCompletos.Add("IfDato83", cadenaLargoFijo(Datos[86].ToString(), 50));
                    parametrosCompletos.Add("IfDato84", cadenaLargoFijo(Datos[87].ToString(), 50));
                    parametrosCompletos.Add("IfDato85", cadenaLargoFijo(Datos[88].ToString(), 50));
                    parametrosCompletos.Add("IfDato86", cadenaLargoFijo(Datos[89].ToString(), 50)); //2091 - 2100
                    parametrosCompletos.Add("IfDato87", cadenaLargoFijo(Datos[90].ToString(), 50));
                    parametrosCompletos.Add("IfDato88", cadenaLargoFijo(Datos[91].ToString(), 50));
                    parametrosCompletos.Add("IfDato89", cadenaLargoFijo(Datos[92].ToString(), 50));
                    parametrosCompletos.Add("IfDato90", cadenaLargoFijo(Datos[93].ToString(), 50));
                    parametrosCompletos.Add("IfDato91", cadenaLargoFijo(Datos[94].ToString(), 50));
                    parametrosCompletos.Add("IfDato92", cadenaLargoFijo(Datos[95].ToString(), 50));
                    parametrosCompletos.Add("IfDato93", cadenaLargoFijo(Datos[96].ToString(), 50));
                    parametrosCompletos.Add("IfDato94", cadenaLargoFijo(Datos[97].ToString(), 50));
                    parametrosCompletos.Add("IfDato95", cadenaLargoFijo(Datos[98].ToString(), 50));
                    parametrosCompletos.Add("IfDato96", cadenaLargoFijo(Datos[99].ToString(), 50));//TOTAL
                    //fila 2
                    parametrosCompletos.Add("IfD1Dato0", cadenaLargoFijo(Datos[100].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato1", cadenaLargoFijo(Datos[101].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato2", cadenaLargoFijo(Datos[102].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato3", cadenaLargoFijo(Datos[103].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato4", cadenaLargoFijo(Datos[104].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato5", cadenaLargoFijo(Datos[105].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato6", cadenaLargoFijo(Datos[106].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato7", cadenaLargoFijo(Datos[107].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato8", cadenaLargoFijo(Datos[108].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato9", cadenaLargoFijo(Datos[109].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato10", cadenaLargoFijo(Datos[110].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato11", cadenaLargoFijo(Datos[111].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato12", cadenaLargoFijo(Datos[112].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato13", cadenaLargoFijo(Datos[113].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato14", cadenaLargoFijo(Datos[114].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato15", cadenaLargoFijo(Datos[115].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato16", cadenaLargoFijo(Datos[116].ToString(), 50)); //2021 - 2030
                    parametrosCompletos.Add("IfD1Dato17", cadenaLargoFijo(Datos[117].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato18", cadenaLargoFijo(Datos[118].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato19", cadenaLargoFijo(Datos[119].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato20", cadenaLargoFijo(Datos[120].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato21", cadenaLargoFijo(Datos[121].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato22", cadenaLargoFijo(Datos[122].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato23", cadenaLargoFijo(Datos[123].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato24", cadenaLargoFijo(Datos[124].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato25", cadenaLargoFijo(Datos[125].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato26", cadenaLargoFijo(Datos[126].ToString(), 50)); //2031 - 2040
                    parametrosCompletos.Add("IfD1Dato27", cadenaLargoFijo(Datos[127].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato28", cadenaLargoFijo(Datos[128].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato29", cadenaLargoFijo(Datos[129].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato30", cadenaLargoFijo(Datos[130].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato31", cadenaLargoFijo(Datos[131].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato32", cadenaLargoFijo(Datos[132].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato33", cadenaLargoFijo(Datos[133].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato34", cadenaLargoFijo(Datos[134].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato35", cadenaLargoFijo(Datos[135].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato36", cadenaLargoFijo(Datos[136].ToString(), 50)); //2041 - 2050
                    parametrosCompletos.Add("IfD1Dato37", cadenaLargoFijo(Datos[137].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato38", cadenaLargoFijo(Datos[138].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato39", cadenaLargoFijo(Datos[139].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato40", cadenaLargoFijo(Datos[140].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato41", cadenaLargoFijo(Datos[141].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato42", cadenaLargoFijo(Datos[142].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato43", cadenaLargoFijo(Datos[143].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato44", cadenaLargoFijo(Datos[144].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato45", cadenaLargoFijo(Datos[145].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato46", cadenaLargoFijo(Datos[146].ToString(), 50)); //2051 - 2060
                    parametrosCompletos.Add("IfD1Dato47", cadenaLargoFijo(Datos[147].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato48", cadenaLargoFijo(Datos[148].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato49", cadenaLargoFijo(Datos[149].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato50", cadenaLargoFijo(Datos[150].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato51", cadenaLargoFijo(Datos[151].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato52", cadenaLargoFijo(Datos[152].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato53", cadenaLargoFijo(Datos[153].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato54", cadenaLargoFijo(Datos[154].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato55", cadenaLargoFijo(Datos[155].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato56", cadenaLargoFijo(Datos[156].ToString(), 50)); //2061 - 2070
                    parametrosCompletos.Add("IfD1Dato57", cadenaLargoFijo(Datos[157].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato58", cadenaLargoFijo(Datos[158].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato59", cadenaLargoFijo(Datos[159].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato60", cadenaLargoFijo(Datos[160].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato61", cadenaLargoFijo(Datos[161].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato62", cadenaLargoFijo(Datos[162].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato63", cadenaLargoFijo(Datos[163].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato64", cadenaLargoFijo(Datos[164].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato65", cadenaLargoFijo(Datos[165].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato66", cadenaLargoFijo(Datos[166].ToString(), 50)); //2071 - 2080
                    parametrosCompletos.Add("IfD1Dato67", cadenaLargoFijo(Datos[167].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato68", cadenaLargoFijo(Datos[168].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato69", cadenaLargoFijo(Datos[169].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato70", cadenaLargoFijo(Datos[170].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato71", cadenaLargoFijo(Datos[171].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato72", cadenaLargoFijo(Datos[172].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato73", cadenaLargoFijo(Datos[173].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato74", cadenaLargoFijo(Datos[174].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato75", cadenaLargoFijo(Datos[175].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato76", cadenaLargoFijo(Datos[176].ToString(), 50)); //2081 - 2090
                    parametrosCompletos.Add("IfD1Dato77", cadenaLargoFijo(Datos[177].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato78", cadenaLargoFijo(Datos[178].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato79", cadenaLargoFijo(Datos[179].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato80", cadenaLargoFijo(Datos[180].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato81", cadenaLargoFijo(Datos[181].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato82", cadenaLargoFijo(Datos[182].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato83", cadenaLargoFijo(Datos[183].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato84", cadenaLargoFijo(Datos[184].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato85", cadenaLargoFijo(Datos[185].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato86", cadenaLargoFijo(Datos[186].ToString(), 50)); //2091 - 2100
                    parametrosCompletos.Add("IfD1Dato87", cadenaLargoFijo(Datos[187].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato88", cadenaLargoFijo(Datos[188].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato89", cadenaLargoFijo(Datos[189].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato90", cadenaLargoFijo(Datos[190].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato91", cadenaLargoFijo(Datos[191].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato92", cadenaLargoFijo(Datos[192].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato93", cadenaLargoFijo(Datos[193].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato94", cadenaLargoFijo(Datos[194].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato95", cadenaLargoFijo(Datos[195].ToString(), 50));
                    parametrosCompletos.Add("IfD1Dato96", cadenaLargoFijo(Datos[196].ToString(), 50));//TOTAL
                    //fila 3
                    parametrosCompletos.Add("IfD2Dato0", cadenaLargoFijo(Datos[197].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato1", cadenaLargoFijo(Datos[198].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato2", cadenaLargoFijo(Datos[199].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato3", cadenaLargoFijo(Datos[200].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato4", cadenaLargoFijo(Datos[201].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato5", cadenaLargoFijo(Datos[202].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato6", cadenaLargoFijo(Datos[203].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato7", cadenaLargoFijo(Datos[204].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato8", cadenaLargoFijo(Datos[205].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato9", cadenaLargoFijo(Datos[206].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato10", cadenaLargoFijo(Datos[207].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato11", cadenaLargoFijo(Datos[208].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato12", cadenaLargoFijo(Datos[209].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato13", cadenaLargoFijo(Datos[210].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato14", cadenaLargoFijo(Datos[211].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato15", cadenaLargoFijo(Datos[212].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato16", cadenaLargoFijo(Datos[213].ToString(), 50)); //2021 - 2030
                    parametrosCompletos.Add("IfD2Dato17", cadenaLargoFijo(Datos[214].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato18", cadenaLargoFijo(Datos[215].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato19", cadenaLargoFijo(Datos[216].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato20", cadenaLargoFijo(Datos[217].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato21", cadenaLargoFijo(Datos[218].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato22", cadenaLargoFijo(Datos[219].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato23", cadenaLargoFijo(Datos[220].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato24", cadenaLargoFijo(Datos[221].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato25", cadenaLargoFijo(Datos[222].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato26", cadenaLargoFijo(Datos[223].ToString(), 50)); //2031 - 2040
                    parametrosCompletos.Add("IfD2Dato27", cadenaLargoFijo(Datos[224].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato28", cadenaLargoFijo(Datos[225].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato29", cadenaLargoFijo(Datos[226].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato30", cadenaLargoFijo(Datos[227].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato31", cadenaLargoFijo(Datos[228].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato32", cadenaLargoFijo(Datos[229].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato33", cadenaLargoFijo(Datos[230].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato34", cadenaLargoFijo(Datos[231].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato35", cadenaLargoFijo(Datos[232].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato36", cadenaLargoFijo(Datos[233].ToString(), 50)); //2041 - 2050
                    parametrosCompletos.Add("IfD2Dato37", cadenaLargoFijo(Datos[234].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato38", cadenaLargoFijo(Datos[235].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato39", cadenaLargoFijo(Datos[236].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato40", cadenaLargoFijo(Datos[237].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato41", cadenaLargoFijo(Datos[238].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato42", cadenaLargoFijo(Datos[239].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato43", cadenaLargoFijo(Datos[240].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato44", cadenaLargoFijo(Datos[241].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato45", cadenaLargoFijo(Datos[242].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato46", cadenaLargoFijo(Datos[243].ToString(), 50)); //2051 - 2060
                    parametrosCompletos.Add("IfD2Dato47", cadenaLargoFijo(Datos[244].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato48", cadenaLargoFijo(Datos[245].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato49", cadenaLargoFijo(Datos[246].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato50", cadenaLargoFijo(Datos[247].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato51", cadenaLargoFijo(Datos[248].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato52", cadenaLargoFijo(Datos[249].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato53", cadenaLargoFijo(Datos[250].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato54", cadenaLargoFijo(Datos[251].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato55", cadenaLargoFijo(Datos[252].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato56", cadenaLargoFijo(Datos[253].ToString(), 50)); //2061 - 2070
                    parametrosCompletos.Add("IfD2Dato57", cadenaLargoFijo(Datos[254].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato58", cadenaLargoFijo(Datos[255].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato59", cadenaLargoFijo(Datos[256].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato60", cadenaLargoFijo(Datos[257].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato61", cadenaLargoFijo(Datos[258].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato62", cadenaLargoFijo(Datos[259].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato63", cadenaLargoFijo(Datos[260].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato64", cadenaLargoFijo(Datos[261].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato65", cadenaLargoFijo(Datos[262].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato66", cadenaLargoFijo(Datos[263].ToString(), 50)); //2071 - 2080
                    parametrosCompletos.Add("IfD2Dato67", cadenaLargoFijo(Datos[264].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato68", cadenaLargoFijo(Datos[265].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato69", cadenaLargoFijo(Datos[266].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato70", cadenaLargoFijo(Datos[267].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato71", cadenaLargoFijo(Datos[268].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato72", cadenaLargoFijo(Datos[269].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato73", cadenaLargoFijo(Datos[270].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato74", cadenaLargoFijo(Datos[271].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato75", cadenaLargoFijo(Datos[272].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato76", cadenaLargoFijo(Datos[273].ToString(), 50)); //2081 - 2090
                    parametrosCompletos.Add("IfD2Dato77", cadenaLargoFijo(Datos[274].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato78", cadenaLargoFijo(Datos[275].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato79", cadenaLargoFijo(Datos[276].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato80", cadenaLargoFijo(Datos[277].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato81", cadenaLargoFijo(Datos[278].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato82", cadenaLargoFijo(Datos[279].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato83", cadenaLargoFijo(Datos[280].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato84", cadenaLargoFijo(Datos[281].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato85", cadenaLargoFijo(Datos[282].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato86", cadenaLargoFijo(Datos[283].ToString(), 50)); //2091 - 2100
                    parametrosCompletos.Add("IfD2Dato87", cadenaLargoFijo(Datos[284].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato88", cadenaLargoFijo(Datos[285].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato89", cadenaLargoFijo(Datos[286].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato90", cadenaLargoFijo(Datos[287].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato91", cadenaLargoFijo(Datos[288].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato92", cadenaLargoFijo(Datos[289].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato93", cadenaLargoFijo(Datos[290].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato94", cadenaLargoFijo(Datos[291].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato95", cadenaLargoFijo(Datos[292].ToString(), 50));
                    parametrosCompletos.Add("IfD2Dato96", cadenaLargoFijo(Datos[293].ToString(), 50));//TOTAL
                    parametrosCompletos.Add("PorInvNacExt", cadenaLargoFijo(PorInvNacExt, 10));
                    parametrosCompletos.Add("Opcion", 1);
                    parametrosCompletos.Add("NumIngreso", NumIngreso);
                    var sql = "EXEC CAPEX_INS_INFORMACION_FINANCIERA_CASOBASE_2_PARAMETROVN";
                    objConnection.Execute(sql, parametrosCompletos, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    var parametrosOpcionales = new DynamicParameters();
                    parametrosOpcionales.Add("IniToken", cadenaLargoFijo(Datos[0].ToString(), 50));
                    parametrosOpcionales.Add("IniUsuario", cadenaLargoFijo(Datos[1].ToString(), 30));
                    parametrosOpcionales.Add("ParametroVNToken", cadenaLargoFijo(Datos[2].ToString(), 50));
                    parametrosOpcionales.Add("IfDato0", cadenaLargoFijo(Datos[3].ToString(), 50));
                    parametrosOpcionales.Add("IfDato1", cadenaLargoFijo(Datos[4].ToString(), 50));
                    parametrosOpcionales.Add("IfDato2", cadenaLargoFijo(Datos[5].ToString(), 50));
                    parametrosOpcionales.Add("IfDato3", cadenaLargoFijo(Datos[6].ToString(), 50));
                    parametrosOpcionales.Add("IfDato4", cadenaLargoFijo(Datos[7].ToString(), 50));
                    parametrosOpcionales.Add("IfDato5", cadenaLargoFijo(Datos[8].ToString(), 50));
                    parametrosOpcionales.Add("IfDato6", cadenaLargoFijo(Datos[9].ToString(), 50));
                    parametrosOpcionales.Add("IfDato7", cadenaLargoFijo(Datos[10].ToString(), 50));
                    parametrosOpcionales.Add("IfDato8", cadenaLargoFijo(Datos[11].ToString(), 50));
                    parametrosOpcionales.Add("IfDato9", cadenaLargoFijo(Datos[12].ToString(), 50));
                    parametrosOpcionales.Add("IfDato10", cadenaLargoFijo(Datos[13].ToString(), 50));
                    parametrosOpcionales.Add("IfDato11", cadenaLargoFijo(Datos[14].ToString(), 50));
                    parametrosOpcionales.Add("IfDato12", cadenaLargoFijo(Datos[15].ToString(), 50));
                    parametrosOpcionales.Add("IfDato13", cadenaLargoFijo(Datos[16].ToString(), 50));
                    parametrosOpcionales.Add("IfDato14", cadenaLargoFijo(Datos[17].ToString(), 50));

                    parametrosOpcionales.Add("IfDato15", cadenaLargoFijo(Datos[18].ToString(), 50));

                    parametrosOpcionales.Add("IfDato16", cadenaLargoFijo(Datos[19].ToString(), 50)); //2021 - 2030
                    parametrosOpcionales.Add("IfDato17", cadenaLargoFijo(Datos[20].ToString(), 50));
                    parametrosOpcionales.Add("IfDato18", cadenaLargoFijo(Datos[21].ToString(), 50));
                    parametrosOpcionales.Add("IfDato19", cadenaLargoFijo(Datos[22].ToString(), 50));
                    parametrosOpcionales.Add("IfDato20", cadenaLargoFijo(Datos[23].ToString(), 50));
                    parametrosOpcionales.Add("IfDato21", cadenaLargoFijo(Datos[24].ToString(), 50));
                    parametrosOpcionales.Add("IfDato22", cadenaLargoFijo(Datos[25].ToString(), 50));
                    parametrosOpcionales.Add("IfDato23", cadenaLargoFijo(Datos[26].ToString(), 50));
                    parametrosOpcionales.Add("IfDato24", cadenaLargoFijo(Datos[27].ToString(), 50));
                    parametrosOpcionales.Add("IfDato25", cadenaLargoFijo(Datos[28].ToString(), 50));

                    parametrosOpcionales.Add("IfDato26", cadenaLargoFijo(Datos[29].ToString(), 50)); //2031 - 2040
                    parametrosOpcionales.Add("IfDato27", cadenaLargoFijo(Datos[30].ToString(), 50));
                    parametrosOpcionales.Add("IfDato28", cadenaLargoFijo(Datos[31].ToString(), 50));
                    parametrosOpcionales.Add("IfDato29", cadenaLargoFijo(Datos[32].ToString(), 50));
                    parametrosOpcionales.Add("IfDato30", cadenaLargoFijo(Datos[33].ToString(), 50));
                    parametrosOpcionales.Add("IfDato31", cadenaLargoFijo(Datos[34].ToString(), 50));
                    parametrosOpcionales.Add("IfDato32", cadenaLargoFijo(Datos[35].ToString(), 50));
                    parametrosOpcionales.Add("IfDato33", cadenaLargoFijo(Datos[36].ToString(), 50));
                    parametrosOpcionales.Add("IfDato34", cadenaLargoFijo(Datos[37].ToString(), 50));
                    parametrosOpcionales.Add("IfDato35", cadenaLargoFijo(Datos[38].ToString(), 50));

                    parametrosOpcionales.Add("IfDato36", cadenaLargoFijo(Datos[39].ToString(), 50)); //2041 - 2050
                    parametrosOpcionales.Add("IfDato37", cadenaLargoFijo(Datos[40].ToString(), 50));
                    parametrosOpcionales.Add("IfDato38", cadenaLargoFijo(Datos[41].ToString(), 50));
                    parametrosOpcionales.Add("IfDato39", cadenaLargoFijo(Datos[42].ToString(), 50));
                    parametrosOpcionales.Add("IfDato40", cadenaLargoFijo(Datos[43].ToString(), 50));
                    parametrosOpcionales.Add("IfDato41", cadenaLargoFijo(Datos[44].ToString(), 50));
                    parametrosOpcionales.Add("IfDato42", cadenaLargoFijo(Datos[45].ToString(), 50));
                    parametrosOpcionales.Add("IfDato43", cadenaLargoFijo(Datos[46].ToString(), 50));
                    parametrosOpcionales.Add("IfDato44", cadenaLargoFijo(Datos[47].ToString(), 50));
                    parametrosOpcionales.Add("IfDato45", cadenaLargoFijo(Datos[48].ToString(), 50));

                    parametrosOpcionales.Add("IfDato46", cadenaLargoFijo(Datos[49].ToString(), 50)); //2051 - 2060
                    parametrosOpcionales.Add("IfDato47", cadenaLargoFijo(Datos[50].ToString(), 50));
                    parametrosOpcionales.Add("IfDato48", cadenaLargoFijo(Datos[51].ToString(), 50));
                    parametrosOpcionales.Add("IfDato49", cadenaLargoFijo(Datos[52].ToString(), 50));
                    parametrosOpcionales.Add("IfDato50", cadenaLargoFijo(Datos[53].ToString(), 50));
                    parametrosOpcionales.Add("IfDato51", cadenaLargoFijo(Datos[54].ToString(), 50));
                    parametrosOpcionales.Add("IfDato52", cadenaLargoFijo(Datos[55].ToString(), 50));
                    parametrosOpcionales.Add("IfDato53", cadenaLargoFijo(Datos[56].ToString(), 50));
                    parametrosOpcionales.Add("IfDato54", cadenaLargoFijo(Datos[57].ToString(), 50));
                    parametrosOpcionales.Add("IfDato55", cadenaLargoFijo(Datos[58].ToString(), 50));

                    parametrosOpcionales.Add("IfDato56", cadenaLargoFijo(Datos[59].ToString(), 50)); //2061 - 2070
                    parametrosOpcionales.Add("IfDato57", cadenaLargoFijo(Datos[60].ToString(), 50));
                    parametrosOpcionales.Add("IfDato58", cadenaLargoFijo(Datos[61].ToString(), 50));
                    parametrosOpcionales.Add("IfDato59", cadenaLargoFijo(Datos[62].ToString(), 50));
                    parametrosOpcionales.Add("IfDato60", cadenaLargoFijo(Datos[63].ToString(), 50));
                    parametrosOpcionales.Add("IfDato61", cadenaLargoFijo(Datos[64].ToString(), 50));
                    parametrosOpcionales.Add("IfDato62", cadenaLargoFijo(Datos[65].ToString(), 50));
                    parametrosOpcionales.Add("IfDato63", cadenaLargoFijo(Datos[66].ToString(), 50));
                    parametrosOpcionales.Add("IfDato64", cadenaLargoFijo(Datos[67].ToString(), 50));
                    parametrosOpcionales.Add("IfDato65", cadenaLargoFijo(Datos[68].ToString(), 50));

                    parametrosOpcionales.Add("IfDato66", cadenaLargoFijo(Datos[69].ToString(), 50)); //2071 - 2080
                    parametrosOpcionales.Add("IfDato67", cadenaLargoFijo(Datos[70].ToString(), 50));
                    parametrosOpcionales.Add("IfDato68", cadenaLargoFijo(Datos[71].ToString(), 50));
                    parametrosOpcionales.Add("IfDato69", cadenaLargoFijo(Datos[72].ToString(), 50));
                    parametrosOpcionales.Add("IfDato70", cadenaLargoFijo(Datos[73].ToString(), 50));
                    parametrosOpcionales.Add("IfDato71", cadenaLargoFijo(Datos[74].ToString(), 50));
                    parametrosOpcionales.Add("IfDato72", cadenaLargoFijo(Datos[75].ToString(), 50));
                    parametrosOpcionales.Add("IfDato73", cadenaLargoFijo(Datos[76].ToString(), 50));
                    parametrosOpcionales.Add("IfDato74", cadenaLargoFijo(Datos[77].ToString(), 50));
                    parametrosOpcionales.Add("IfDato75", cadenaLargoFijo(Datos[78].ToString(), 50));

                    parametrosOpcionales.Add("IfDato76", cadenaLargoFijo(Datos[79].ToString(), 50)); //2081 - 2090
                    parametrosOpcionales.Add("IfDato77", cadenaLargoFijo(Datos[80].ToString(), 50));
                    parametrosOpcionales.Add("IfDato78", cadenaLargoFijo(Datos[81].ToString(), 50));
                    parametrosOpcionales.Add("IfDato79", cadenaLargoFijo(Datos[82].ToString(), 50));
                    parametrosOpcionales.Add("IfDato80", cadenaLargoFijo(Datos[83].ToString(), 50));
                    parametrosOpcionales.Add("IfDato81", cadenaLargoFijo(Datos[84].ToString(), 50));
                    parametrosOpcionales.Add("IfDato82", cadenaLargoFijo(Datos[85].ToString(), 50));
                    parametrosOpcionales.Add("IfDato83", cadenaLargoFijo(Datos[86].ToString(), 50));
                    parametrosOpcionales.Add("IfDato84", cadenaLargoFijo(Datos[87].ToString(), 50));
                    parametrosOpcionales.Add("IfDato85", cadenaLargoFijo(Datos[88].ToString(), 50));

                    parametrosOpcionales.Add("IfDato86", cadenaLargoFijo(Datos[89].ToString(), 50)); //2091 - 2100
                    parametrosOpcionales.Add("IfDato87", cadenaLargoFijo(Datos[90].ToString(), 50));
                    parametrosOpcionales.Add("IfDato88", cadenaLargoFijo(Datos[91].ToString(), 50));
                    parametrosOpcionales.Add("IfDato89", cadenaLargoFijo(Datos[92].ToString(), 50));
                    parametrosOpcionales.Add("IfDato90", cadenaLargoFijo(Datos[93].ToString(), 50));
                    parametrosOpcionales.Add("IfDato91", cadenaLargoFijo(Datos[94].ToString(), 50));
                    parametrosOpcionales.Add("IfDato92", cadenaLargoFijo(Datos[95].ToString(), 50));
                    parametrosOpcionales.Add("IfDato93", cadenaLargoFijo(Datos[96].ToString(), 50));
                    parametrosOpcionales.Add("IfDato94", cadenaLargoFijo(Datos[97].ToString(), 50));
                    parametrosOpcionales.Add("IfDato95", cadenaLargoFijo(Datos[98].ToString(), 50));

                    parametrosOpcionales.Add("IfDato96", cadenaLargoFijo(Datos[99].ToString(), 50));//TOTAL
                    //fila 2
                    parametrosOpcionales.Add("IfD1Dato0", "");
                    parametrosOpcionales.Add("IfD1Dato1", "");
                    parametrosOpcionales.Add("IfD1Dato2", "");

                    parametrosOpcionales.Add("IfD1Dato3", "");
                    parametrosOpcionales.Add("IfD1Dato4", "");
                    parametrosOpcionales.Add("IfD1Dato5", "");
                    parametrosOpcionales.Add("IfD1Dato6", "");
                    parametrosOpcionales.Add("IfD1Dato7", "");
                    parametrosOpcionales.Add("IfD1Dato8", "");
                    parametrosOpcionales.Add("IfD1Dato9", "");
                    parametrosOpcionales.Add("IfD1Dato10", "");
                    parametrosOpcionales.Add("IfD1Dato11", "");
                    parametrosOpcionales.Add("IfD1Dato12", "");
                    parametrosOpcionales.Add("IfD1Dato13", "");
                    parametrosOpcionales.Add("IfD1Dato14", "");

                    parametrosOpcionales.Add("IfD1Dato15", "");

                    parametrosOpcionales.Add("IfD1Dato16", ""); //2021 - 2030
                    parametrosOpcionales.Add("IfD1Dato17", "");
                    parametrosOpcionales.Add("IfD1Dato18", "");
                    parametrosOpcionales.Add("IfD1Dato19", "");
                    parametrosOpcionales.Add("IfD1Dato20", "");
                    parametrosOpcionales.Add("IfD1Dato21", "");
                    parametrosOpcionales.Add("IfD1Dato22", "");
                    parametrosOpcionales.Add("IfD1Dato23", "");
                    parametrosOpcionales.Add("IfD1Dato24", "");
                    parametrosOpcionales.Add("IfD1Dato25", "");

                    parametrosOpcionales.Add("IfD1Dato26", ""); //2031 - 2040
                    parametrosOpcionales.Add("IfD1Dato27", "");
                    parametrosOpcionales.Add("IfD1Dato28", "");
                    parametrosOpcionales.Add("IfD1Dato29", "");
                    parametrosOpcionales.Add("IfD1Dato30", "");
                    parametrosOpcionales.Add("IfD1Dato31", "");
                    parametrosOpcionales.Add("IfD1Dato32", "");
                    parametrosOpcionales.Add("IfD1Dato33", "");
                    parametrosOpcionales.Add("IfD1Dato34", "");
                    parametrosOpcionales.Add("IfD1Dato35", "");

                    parametrosOpcionales.Add("IfD1Dato36", ""); //2041 - 2050
                    parametrosOpcionales.Add("IfD1Dato37", "");
                    parametrosOpcionales.Add("IfD1Dato38", "");
                    parametrosOpcionales.Add("IfD1Dato39", "");
                    parametrosOpcionales.Add("IfD1Dato40", "");
                    parametrosOpcionales.Add("IfD1Dato41", "");
                    parametrosOpcionales.Add("IfD1Dato42", "");
                    parametrosOpcionales.Add("IfD1Dato43", "");
                    parametrosOpcionales.Add("IfD1Dato44", "");
                    parametrosOpcionales.Add("IfD1Dato45", "");

                    parametrosOpcionales.Add("IfD1Dato46", ""); //2051 - 2060
                    parametrosOpcionales.Add("IfD1Dato47", "");
                    parametrosOpcionales.Add("IfD1Dato48", "");
                    parametrosOpcionales.Add("IfD1Dato49", "");
                    parametrosOpcionales.Add("IfD1Dato50", "");
                    parametrosOpcionales.Add("IfD1Dato51", "");
                    parametrosOpcionales.Add("IfD1Dato52", "");
                    parametrosOpcionales.Add("IfD1Dato53", "");
                    parametrosOpcionales.Add("IfD1Dato54", "");
                    parametrosOpcionales.Add("IfD1Dato55", "");

                    parametrosOpcionales.Add("IfD1Dato56", ""); //2061 - 2070
                    parametrosOpcionales.Add("IfD1Dato57", "");
                    parametrosOpcionales.Add("IfD1Dato58", "");
                    parametrosOpcionales.Add("IfD1Dato59", "");
                    parametrosOpcionales.Add("IfD1Dato60", "");
                    parametrosOpcionales.Add("IfD1Dato61", "");
                    parametrosOpcionales.Add("IfD1Dato62", "");
                    parametrosOpcionales.Add("IfD1Dato63", "");
                    parametrosOpcionales.Add("IfD1Dato64", "");
                    parametrosOpcionales.Add("IfD1Dato65", "");

                    parametrosOpcionales.Add("IfD1Dato66", ""); //2071 - 2080
                    parametrosOpcionales.Add("IfD1Dato67", "");
                    parametrosOpcionales.Add("IfD1Dato68", "");
                    parametrosOpcionales.Add("IfD1Dato69", "");
                    parametrosOpcionales.Add("IfD1Dato70", "");
                    parametrosOpcionales.Add("IfD1Dato71", "");
                    parametrosOpcionales.Add("IfD1Dato72", "");
                    parametrosOpcionales.Add("IfD1Dato73", "");
                    parametrosOpcionales.Add("IfD1Dato74", "");
                    parametrosOpcionales.Add("IfD1Dato75", "");

                    parametrosOpcionales.Add("IfD1Dato76", ""); //2081 - 2090
                    parametrosOpcionales.Add("IfD1Dato77", "");
                    parametrosOpcionales.Add("IfD1Dato78", "");
                    parametrosOpcionales.Add("IfD1Dato79", "");
                    parametrosOpcionales.Add("IfD1Dato80", "");
                    parametrosOpcionales.Add("IfD1Dato81", "");
                    parametrosOpcionales.Add("IfD1Dato82", "");
                    parametrosOpcionales.Add("IfD1Dato83", "");
                    parametrosOpcionales.Add("IfD1Dato84", "");
                    parametrosOpcionales.Add("IfD1Dato85", "");

                    parametrosOpcionales.Add("IfD1Dato86", ""); //2091 - 2100
                    parametrosOpcionales.Add("IfD1Dato87", "");
                    parametrosOpcionales.Add("IfD1Dato88", "");
                    parametrosOpcionales.Add("IfD1Dato89", "");
                    parametrosOpcionales.Add("IfD1Dato90", "");
                    parametrosOpcionales.Add("IfD1Dato91", "");
                    parametrosOpcionales.Add("IfD1Dato92", "");
                    parametrosOpcionales.Add("IfD1Dato93", "");
                    parametrosOpcionales.Add("IfD1Dato94", "");
                    parametrosOpcionales.Add("IfD1Dato95", "");
                    parametrosOpcionales.Add("IfD1Dato96", "");//TOTAL
                    //fila 3
                    parametrosOpcionales.Add("IfD2Dato0", "");
                    parametrosOpcionales.Add("IfD2Dato1", "");
                    parametrosOpcionales.Add("IfD2Dato2", "");

                    parametrosOpcionales.Add("IfD2Dato3", "");
                    parametrosOpcionales.Add("IfD2Dato4", "");
                    parametrosOpcionales.Add("IfD2Dato5", "");
                    parametrosOpcionales.Add("IfD2Dato6", "");
                    parametrosOpcionales.Add("IfD2Dato7", "");
                    parametrosOpcionales.Add("IfD2Dato8", "");
                    parametrosOpcionales.Add("IfD2Dato9", "");
                    parametrosOpcionales.Add("IfD2Dato10", "");
                    parametrosOpcionales.Add("IfD2Dato11", "");
                    parametrosOpcionales.Add("IfD2Dato12", "");
                    parametrosOpcionales.Add("IfD2Dato13", "");
                    parametrosOpcionales.Add("IfD2Dato14", "");

                    parametrosOpcionales.Add("IfD2Dato15", "");

                    parametrosOpcionales.Add("IfD2Dato16", ""); //2021 - 2030
                    parametrosOpcionales.Add("IfD2Dato17", "");
                    parametrosOpcionales.Add("IfD2Dato18", "");
                    parametrosOpcionales.Add("IfD2Dato19", "");
                    parametrosOpcionales.Add("IfD2Dato20", "");
                    parametrosOpcionales.Add("IfD2Dato21", "");
                    parametrosOpcionales.Add("IfD2Dato22", "");
                    parametrosOpcionales.Add("IfD2Dato23", "");
                    parametrosOpcionales.Add("IfD2Dato24", "");
                    parametrosOpcionales.Add("IfD2Dato25", "");

                    parametrosOpcionales.Add("IfD2Dato26", ""); //2031 - 2040
                    parametrosOpcionales.Add("IfD2Dato27", "");
                    parametrosOpcionales.Add("IfD2Dato28", "");
                    parametrosOpcionales.Add("IfD2Dato29", "");
                    parametrosOpcionales.Add("IfD2Dato30", "");
                    parametrosOpcionales.Add("IfD2Dato31", "");
                    parametrosOpcionales.Add("IfD2Dato32", "");
                    parametrosOpcionales.Add("IfD2Dato33", "");
                    parametrosOpcionales.Add("IfD2Dato34", "");
                    parametrosOpcionales.Add("IfD2Dato35", "");

                    parametrosOpcionales.Add("IfD2Dato36", ""); //2041 - 2050
                    parametrosOpcionales.Add("IfD2Dato37", "");
                    parametrosOpcionales.Add("IfD2Dato38", "");
                    parametrosOpcionales.Add("IfD2Dato39", "");
                    parametrosOpcionales.Add("IfD2Dato40", "");
                    parametrosOpcionales.Add("IfD2Dato41", "");
                    parametrosOpcionales.Add("IfD2Dato42", "");
                    parametrosOpcionales.Add("IfD2Dato43", "");
                    parametrosOpcionales.Add("IfD2Dato44", "");
                    parametrosOpcionales.Add("IfD2Dato45", "");

                    parametrosOpcionales.Add("IfD2Dato46", ""); //2051 - 2060
                    parametrosOpcionales.Add("IfD2Dato47", "");
                    parametrosOpcionales.Add("IfD2Dato48", "");
                    parametrosOpcionales.Add("IfD2Dato49", "");
                    parametrosOpcionales.Add("IfD2Dato50", "");
                    parametrosOpcionales.Add("IfD2Dato51", "");
                    parametrosOpcionales.Add("IfD2Dato52", "");
                    parametrosOpcionales.Add("IfD2Dato53", "");
                    parametrosOpcionales.Add("IfD2Dato54", "");
                    parametrosOpcionales.Add("IfD2Dato55", "");

                    parametrosOpcionales.Add("IfD2Dato56", ""); //2061 - 2070
                    parametrosOpcionales.Add("IfD2Dato57", "");
                    parametrosOpcionales.Add("IfD2Dato58", "");
                    parametrosOpcionales.Add("IfD2Dato59", "");
                    parametrosOpcionales.Add("IfD2Dato60", "");
                    parametrosOpcionales.Add("IfD2Dato61", "");
                    parametrosOpcionales.Add("IfD2Dato62", "");
                    parametrosOpcionales.Add("IfD2Dato63", "");
                    parametrosOpcionales.Add("IfD2Dato64", "");
                    parametrosOpcionales.Add("IfD2Dato65", "");

                    parametrosOpcionales.Add("IfD2Dato66", ""); //2071 - 2080
                    parametrosOpcionales.Add("IfD2Dato67", "");
                    parametrosOpcionales.Add("IfD2Dato68", "");
                    parametrosOpcionales.Add("IfD2Dato69", "");
                    parametrosOpcionales.Add("IfD2Dato70", "");
                    parametrosOpcionales.Add("IfD2Dato71", "");
                    parametrosOpcionales.Add("IfD2Dato72", "");
                    parametrosOpcionales.Add("IfD2Dato73", "");
                    parametrosOpcionales.Add("IfD2Dato74", "");
                    parametrosOpcionales.Add("IfD2Dato75", "");

                    parametrosOpcionales.Add("IfD2Dato76", ""); //2081 - 2090
                    parametrosOpcionales.Add("IfD2Dato77", "");
                    parametrosOpcionales.Add("IfD2Dato78", "");
                    parametrosOpcionales.Add("IfD2Dato79", "");
                    parametrosOpcionales.Add("IfD2Dato80", "");
                    parametrosOpcionales.Add("IfD2Dato81", "");
                    parametrosOpcionales.Add("IfD2Dato82", "");
                    parametrosOpcionales.Add("IfD2Dato83", "");
                    parametrosOpcionales.Add("IfD2Dato84", "");
                    parametrosOpcionales.Add("IfD2Dato85", "");

                    parametrosOpcionales.Add("IfD2Dato86", ""); //2091 - 2100
                    parametrosOpcionales.Add("IfD2Dato87", "");
                    parametrosOpcionales.Add("IfD2Dato88", "");
                    parametrosOpcionales.Add("IfD2Dato89", "");
                    parametrosOpcionales.Add("IfD2Dato90", "");
                    parametrosOpcionales.Add("IfD2Dato91", "");
                    parametrosOpcionales.Add("IfD2Dato92", "");
                    parametrosOpcionales.Add("IfD2Dato93", "");
                    parametrosOpcionales.Add("IfD2Dato94", "");
                    parametrosOpcionales.Add("IfD2Dato95", "");

                    parametrosOpcionales.Add("IfD2Dato96", "");//TOTAL
                    parametrosOpcionales.Add("PorInvNacExt", cadenaLargoFijo(PorInvNacExt, 10));
                    parametrosOpcionales.Add("Opcion", 0);
                    parametrosOpcionales.Add("NumIngreso", NumIngreso);
                    var sql = "EXEC CAPEX_INS_INFORMACION_FINANCIERA_CASOBASE_2_PARAMETROVN";
                    objConnection.Execute(sql, parametrosOpcionales, commandType: CommandType.StoredProcedure);
                }
                //SqlMapper.Execute(objConnection, "CAPEX_INS_INFORMACION_FINANCIERA_CASOBASE_2_PARAMETROVN", parametos, commandType: CommandType.StoredProcedure);


            }
            catch (Exception err)
            {
                ExceptionResult = "InsertarInformacionFinancieraCasoBaseParametroVN, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
            }
        }

        private void InsertarInformacionFinancieraPresupuestoParametroVN(List<String> Datos, String PorInvNacExt, int NumIngreso)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    if (Datos.Count == 63)
                    {
                        parametos.Add("IniToken", cadenaLargoFijo(Datos[0].ToString(), 50));
                        parametos.Add("IniUsuario", cadenaLargoFijo(Datos[1].ToString(), 30));
                        parametos.Add("ParametroVNToken", cadenaLargoFijo(Datos[2].ToString(), 50));
                        parametos.Add("IfDato0", cadenaLargoFijo(Datos[3].ToString(), 50));
                        parametos.Add("IfDato1", cadenaLargoFijo(Datos[4].ToString(), 50));
                        parametos.Add("IfDato2", cadenaLargoFijo(Datos[5].ToString(), 50));

                        parametos.Add("IfDato3", cadenaLargoFijo(Datos[6].ToString(), 50));
                        parametos.Add("IfDato4", cadenaLargoFijo(Datos[7].ToString(), 50));
                        parametos.Add("IfDato5", cadenaLargoFijo(Datos[8].ToString(), 50));
                        parametos.Add("IfDato6", cadenaLargoFijo(Datos[9].ToString(), 50));
                        parametos.Add("IfDato7", cadenaLargoFijo(Datos[10].ToString(), 50));
                        parametos.Add("IfDato8", cadenaLargoFijo(Datos[11].ToString(), 50));
                        parametos.Add("IfDato9", cadenaLargoFijo(Datos[12].ToString(), 50));
                        parametos.Add("IfDato10", cadenaLargoFijo(Datos[13].ToString(), 50));
                        parametos.Add("IfDato11", cadenaLargoFijo(Datos[14].ToString(), 50));
                        parametos.Add("IfDato12", cadenaLargoFijo(Datos[15].ToString(), 50));
                        parametos.Add("IfDato13", cadenaLargoFijo(Datos[16].ToString(), 50));
                        parametos.Add("IfDato14", cadenaLargoFijo(Datos[17].ToString(), 50));

                        parametos.Add("IfDato15", cadenaLargoFijo(Datos[18].ToString(), 50));
                        //AnioMas1
                        parametos.Add("IfDato16", cadenaLargoFijo(Datos[19].ToString(), 50));
                        //AnioMas2
                        parametos.Add("IfDato17", cadenaLargoFijo(Datos[20].ToString(), 50));
                        //AnioMas3
                        parametos.Add("IfDato18", cadenaLargoFijo(Datos[21].ToString(), 50));
                        //Total Capex
                        parametos.Add("IfDato19", cadenaLargoFijo(Datos[22].ToString(), 50));
                        //fila 2
                        parametos.Add("IfD1Dato0", cadenaLargoFijo(Datos[23].ToString(), 50));
                        parametos.Add("IfD1Dato1", cadenaLargoFijo(Datos[24].ToString(), 50));
                        parametos.Add("IfD1Dato2", cadenaLargoFijo(Datos[25].ToString(), 50));
                        parametos.Add("IfD1Dato3", cadenaLargoFijo(Datos[26].ToString(), 50));
                        parametos.Add("IfD1Dato4", cadenaLargoFijo(Datos[27].ToString(), 50));
                        parametos.Add("IfD1Dato5", cadenaLargoFijo(Datos[28].ToString(), 50));
                        parametos.Add("IfD1Dato6", cadenaLargoFijo(Datos[29].ToString(), 50));
                        parametos.Add("IfD1Dato7", cadenaLargoFijo(Datos[30].ToString(), 50));
                        parametos.Add("IfD1Dato8", cadenaLargoFijo(Datos[31].ToString(), 50));
                        parametos.Add("IfD1Dato9", cadenaLargoFijo(Datos[32].ToString(), 50));
                        parametos.Add("IfD1Dato10", cadenaLargoFijo(Datos[33].ToString(), 50));
                        parametos.Add("IfD1Dato11", cadenaLargoFijo(Datos[34].ToString(), 50));
                        parametos.Add("IfD1Dato12", cadenaLargoFijo(Datos[35].ToString(), 50));
                        parametos.Add("IfD1Dato13", cadenaLargoFijo(Datos[36].ToString(), 50));
                        parametos.Add("IfD1Dato14", cadenaLargoFijo(Datos[37].ToString(), 50));

                        parametos.Add("IfD1Dato15", cadenaLargoFijo(Datos[38].ToString(), 50));

                        parametos.Add("IfD1Dato16", cadenaLargoFijo(Datos[39].ToString(), 50)); //2021 - 2030
                        parametos.Add("IfD1Dato17", cadenaLargoFijo(Datos[40].ToString(), 50));
                        parametos.Add("IfD1Dato18", cadenaLargoFijo(Datos[41].ToString(), 50));
                        //Total Capex
                        parametos.Add("IfD1Dato19", cadenaLargoFijo(Datos[42].ToString(), 50));
                        //fila 3
                        parametos.Add("IfD2Dato0", cadenaLargoFijo(Datos[43].ToString(), 50));
                        parametos.Add("IfD2Dato1", cadenaLargoFijo(Datos[44].ToString(), 50));
                        parametos.Add("IfD2Dato2", cadenaLargoFijo(Datos[45].ToString(), 50));

                        parametos.Add("IfD2Dato3", cadenaLargoFijo(Datos[46].ToString(), 50));
                        parametos.Add("IfD2Dato4", cadenaLargoFijo(Datos[47].ToString(), 50));
                        parametos.Add("IfD2Dato5", cadenaLargoFijo(Datos[48].ToString(), 50));
                        parametos.Add("IfD2Dato6", cadenaLargoFijo(Datos[49].ToString(), 50));
                        parametos.Add("IfD2Dato7", cadenaLargoFijo(Datos[50].ToString(), 50));
                        parametos.Add("IfD2Dato8", cadenaLargoFijo(Datos[51].ToString(), 50));
                        parametos.Add("IfD2Dato9", cadenaLargoFijo(Datos[52].ToString(), 50));
                        parametos.Add("IfD2Dato10", cadenaLargoFijo(Datos[53].ToString(), 50));
                        parametos.Add("IfD2Dato11", cadenaLargoFijo(Datos[54].ToString(), 50));
                        parametos.Add("IfD2Dato12", cadenaLargoFijo(Datos[55].ToString(), 50));
                        parametos.Add("IfD2Dato13", cadenaLargoFijo(Datos[56].ToString(), 50));
                        parametos.Add("IfD2Dato14", cadenaLargoFijo(Datos[57].ToString(), 50));

                        parametos.Add("IfD2Dato15", cadenaLargoFijo(Datos[58].ToString(), 50));

                        parametos.Add("IfD2Dato16", cadenaLargoFijo(Datos[59].ToString(), 50)); //2021 - 2030
                        parametos.Add("IfD2Dato17", cadenaLargoFijo(Datos[60].ToString(), 50));
                        parametos.Add("IfD2Dato18", cadenaLargoFijo(Datos[61].ToString(), 50));
                        //Total Capex
                        parametos.Add("IfD2Dato19", cadenaLargoFijo(Datos[62].ToString(), 50));
                        parametos.Add("PorInvNacExt", cadenaLargoFijo(PorInvNacExt, 10));
                        parametos.Add("Opcion", 1);
                        parametos.Add("NumIngreso", NumIngreso);
                    }
                    else
                    {
                        parametos.Add("IniToken", cadenaLargoFijo(Datos[0].ToString(), 50));
                        parametos.Add("IniUsuario", cadenaLargoFijo(Datos[1].ToString(), 30));
                        parametos.Add("ParametroVNToken", cadenaLargoFijo(Datos[2].ToString(), 50));
                        parametos.Add("IfDato0", cadenaLargoFijo(Datos[3].ToString(), 50));
                        parametos.Add("IfDato1", cadenaLargoFijo(Datos[4].ToString(), 50));
                        parametos.Add("IfDato2", cadenaLargoFijo(Datos[5].ToString(), 50));
                        //Enero
                        parametos.Add("IfDato3", cadenaLargoFijo(Datos[6].ToString(), 50));
                        //Febrero
                        parametos.Add("IfDato4", cadenaLargoFijo(Datos[7].ToString(), 50));
                        //Marzo
                        parametos.Add("IfDato5", cadenaLargoFijo(Datos[8].ToString(), 50));
                        //Abril
                        parametos.Add("IfDato6", cadenaLargoFijo(Datos[9].ToString(), 50));
                        //Mayo
                        parametos.Add("IfDato7", cadenaLargoFijo(Datos[10].ToString(), 50));
                        //Junio
                        parametos.Add("IfDato8", cadenaLargoFijo(Datos[11].ToString(), 50));
                        //Julio
                        parametos.Add("IfDato9", cadenaLargoFijo(Datos[12].ToString(), 50));
                        //Agosto
                        parametos.Add("IfDato10", cadenaLargoFijo(Datos[13].ToString(), 50));
                        //Septiembre
                        parametos.Add("IfDato11", cadenaLargoFijo(Datos[14].ToString(), 50));
                        //Octubre
                        parametos.Add("IfDato12", cadenaLargoFijo(Datos[15].ToString(), 50));
                        //Noviembre
                        parametos.Add("IfDato13", cadenaLargoFijo(Datos[16].ToString(), 50));
                        //Diciembre
                        parametos.Add("IfDato14", cadenaLargoFijo(Datos[17].ToString(), 50));
                        //Total meses
                        parametos.Add("IfDato15", cadenaLargoFijo(Datos[18].ToString(), 50));
                        //AñoMas1
                        parametos.Add("IfDato16", cadenaLargoFijo(Datos[19].ToString(), 50));
                        //AñoMas2
                        parametos.Add("IfDato17", cadenaLargoFijo(Datos[20].ToString(), 50));
                        //AñoMas3
                        parametos.Add("IfDato18", cadenaLargoFijo(Datos[21].ToString(), 50));
                        //Total Capex
                        parametos.Add("IfDato19", cadenaLargoFijo(Datos[22].ToString(), 50));
                        //fila 2
                        parametos.Add("IfD1Dato0", "");
                        parametos.Add("IfD1Dato1", "");
                        parametos.Add("IfD1Dato2", "");
                        //Enero
                        parametos.Add("IfD1Dato3", "");
                        //Febrero
                        parametos.Add("IfD1Dato4", "");
                        //Marzo
                        parametos.Add("IfD1Dato5", "");
                        //Abril
                        parametos.Add("IfD1Dato6", "");
                        //Mayo
                        parametos.Add("IfD1Dato7", "");
                        //Junio
                        parametos.Add("IfD1Dato8", "");
                        //Julio
                        parametos.Add("IfD1Dato9", "");
                        //Agosto
                        parametos.Add("IfD1Dato10", "");
                        //Septiembre
                        parametos.Add("IfD1Dato11", "");
                        //Octubre
                        parametos.Add("IfD1Dato12", "");
                        //Noviembre
                        parametos.Add("IfD1Dato13", "");
                        //Diciembre
                        parametos.Add("IfD1Dato14", "");
                        //Total meses
                        parametos.Add("IfD1Dato15", "");
                        //AñoMas1
                        parametos.Add("IfD1Dato16", "");
                        //AñoMas2
                        parametos.Add("IfD1Dato17", "");
                        //AñoMas3
                        parametos.Add("IfD1Dato18", "");
                        //Total Capex
                        parametos.Add("IfD1Dato19", "");
                        //fila 3
                        parametos.Add("IfD2Dato0", "");
                        parametos.Add("IfD2Dato1", "");
                        parametos.Add("IfD2Dato2", "");
                        //Enero
                        parametos.Add("IfD2Dato3", "");
                        //Febrero
                        parametos.Add("IfD2Dato4", "");
                        //Marzo
                        parametos.Add("IfD2Dato5", "");
                        //Abril
                        parametos.Add("IfD2Dato6", "");
                        //Mayo
                        parametos.Add("IfD2Dato7", "");
                        //Junio
                        parametos.Add("IfD2Dato8", "");
                        //Julio
                        parametos.Add("IfD2Dato9", "");
                        //Agosto
                        parametos.Add("IfD2Dato10", "");
                        //Septiembre
                        parametos.Add("IfD2Dato11", "");
                        //Octubre
                        parametos.Add("IfD2Dato12", "");
                        //Noviembre
                        parametos.Add("IfD2Dato13", "");
                        //Diciembre
                        parametos.Add("IfD2Dato14", "");
                        //Total meses
                        parametos.Add("IfD2Dato15", "");
                        //AñoMas1
                        parametos.Add("IfD2Dato16", "");
                        //AñoMas2
                        parametos.Add("IfD2Dato17", "");
                        //AñoMas3
                        parametos.Add("IfD2Dato18", "");
                        //Total Capex
                        parametos.Add("IfD2Dato19", "");
                        parametos.Add("PorInvNacExt", cadenaLargoFijo(PorInvNacExt, 10));
                        parametos.Add("Opcion", 0);
                        parametos.Add("NumIngreso", NumIngreso);
                    }
                    SqlMapper.Execute(objConnection, "CAPEX_INS_INFORMACION_FINANCIERA_PRESUPUESTO_2_PARAMETROVN", parametos, commandType: CommandType.StoredProcedure);
                }
                catch (Exception err)
                {
                    ExceptionResult = "InsertarInformacionFinancieraPresupuestoParametroVN, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        private void InsertarInformacionFinancieraPresupuestoParametroVN(SqlConnection objConnection, List<String> Datos, String PorInvNacExt, int NumIngreso)
        {
            try
            {
                var parametos = new DynamicParameters();
                if (Datos.Count == 63)
                {
                    parametos.Add("IniToken", cadenaLargoFijo(Datos[0].ToString(), 50));
                    parametos.Add("IniUsuario", cadenaLargoFijo(Datos[1].ToString(), 30));
                    parametos.Add("ParametroVNToken", cadenaLargoFijo(Datos[2].ToString(), 50));
                    parametos.Add("IfDato0", cadenaLargoFijo(Datos[3].ToString(), 50));
                    parametos.Add("IfDato1", cadenaLargoFijo(Datos[4].ToString(), 50));
                    parametos.Add("IfDato2", cadenaLargoFijo(Datos[5].ToString(), 50));

                    parametos.Add("IfDato3", cadenaLargoFijo(Datos[6].ToString(), 50));
                    parametos.Add("IfDato4", cadenaLargoFijo(Datos[7].ToString(), 50));
                    parametos.Add("IfDato5", cadenaLargoFijo(Datos[8].ToString(), 50));
                    parametos.Add("IfDato6", cadenaLargoFijo(Datos[9].ToString(), 50));
                    parametos.Add("IfDato7", cadenaLargoFijo(Datos[10].ToString(), 50));
                    parametos.Add("IfDato8", cadenaLargoFijo(Datos[11].ToString(), 50));
                    parametos.Add("IfDato9", cadenaLargoFijo(Datos[12].ToString(), 50));
                    parametos.Add("IfDato10", cadenaLargoFijo(Datos[13].ToString(), 50));
                    parametos.Add("IfDato11", cadenaLargoFijo(Datos[14].ToString(), 50));
                    parametos.Add("IfDato12", cadenaLargoFijo(Datos[15].ToString(), 50));
                    parametos.Add("IfDato13", cadenaLargoFijo(Datos[16].ToString(), 50));
                    parametos.Add("IfDato14", cadenaLargoFijo(Datos[17].ToString(), 50));

                    parametos.Add("IfDato15", cadenaLargoFijo(Datos[18].ToString(), 50));
                    //AnioMas1
                    parametos.Add("IfDato16", cadenaLargoFijo(Datos[19].ToString(), 50));
                    //AnioMas2
                    parametos.Add("IfDato17", cadenaLargoFijo(Datos[20].ToString(), 50));
                    //AnioMas3
                    parametos.Add("IfDato18", cadenaLargoFijo(Datos[21].ToString(), 50));
                    //Total Capex
                    parametos.Add("IfDato19", cadenaLargoFijo(Datos[22].ToString(), 50));
                    //fila 2
                    parametos.Add("IfD1Dato0", cadenaLargoFijo(Datos[23].ToString(), 50));
                    parametos.Add("IfD1Dato1", cadenaLargoFijo(Datos[24].ToString(), 50));
                    parametos.Add("IfD1Dato2", cadenaLargoFijo(Datos[25].ToString(), 50));
                    parametos.Add("IfD1Dato3", cadenaLargoFijo(Datos[26].ToString(), 50));
                    parametos.Add("IfD1Dato4", cadenaLargoFijo(Datos[27].ToString(), 50));
                    parametos.Add("IfD1Dato5", cadenaLargoFijo(Datos[28].ToString(), 50));
                    parametos.Add("IfD1Dato6", cadenaLargoFijo(Datos[29].ToString(), 50));
                    parametos.Add("IfD1Dato7", cadenaLargoFijo(Datos[30].ToString(), 50));
                    parametos.Add("IfD1Dato8", cadenaLargoFijo(Datos[31].ToString(), 50));
                    parametos.Add("IfD1Dato9", cadenaLargoFijo(Datos[32].ToString(), 50));
                    parametos.Add("IfD1Dato10", cadenaLargoFijo(Datos[33].ToString(), 50));
                    parametos.Add("IfD1Dato11", cadenaLargoFijo(Datos[34].ToString(), 50));
                    parametos.Add("IfD1Dato12", cadenaLargoFijo(Datos[35].ToString(), 50));
                    parametos.Add("IfD1Dato13", cadenaLargoFijo(Datos[36].ToString(), 50));
                    parametos.Add("IfD1Dato14", cadenaLargoFijo(Datos[37].ToString(), 50));
                    parametos.Add("IfD1Dato15", cadenaLargoFijo(Datos[38].ToString(), 50));
                    parametos.Add("IfD1Dato16", cadenaLargoFijo(Datos[39].ToString(), 50)); //2021 - 2030
                    parametos.Add("IfD1Dato17", cadenaLargoFijo(Datos[40].ToString(), 50));
                    parametos.Add("IfD1Dato18", cadenaLargoFijo(Datos[41].ToString(), 50));
                    //Total Capex
                    parametos.Add("IfD1Dato19", cadenaLargoFijo(Datos[42].ToString(), 50));
                    //fila 3
                    parametos.Add("IfD2Dato0", cadenaLargoFijo(Datos[43].ToString(), 50));
                    parametos.Add("IfD2Dato1", cadenaLargoFijo(Datos[44].ToString(), 50));
                    parametos.Add("IfD2Dato2", cadenaLargoFijo(Datos[45].ToString(), 50));
                    parametos.Add("IfD2Dato3", cadenaLargoFijo(Datos[46].ToString(), 50));
                    parametos.Add("IfD2Dato4", cadenaLargoFijo(Datos[47].ToString(), 50));
                    parametos.Add("IfD2Dato5", cadenaLargoFijo(Datos[48].ToString(), 50));
                    parametos.Add("IfD2Dato6", cadenaLargoFijo(Datos[49].ToString(), 50));
                    parametos.Add("IfD2Dato7", cadenaLargoFijo(Datos[50].ToString(), 50));
                    parametos.Add("IfD2Dato8", cadenaLargoFijo(Datos[51].ToString(), 50));
                    parametos.Add("IfD2Dato9", cadenaLargoFijo(Datos[52].ToString(), 50));
                    parametos.Add("IfD2Dato10", cadenaLargoFijo(Datos[53].ToString(), 50));
                    parametos.Add("IfD2Dato11", cadenaLargoFijo(Datos[54].ToString(), 50));
                    parametos.Add("IfD2Dato12", cadenaLargoFijo(Datos[55].ToString(), 50));
                    parametos.Add("IfD2Dato13", cadenaLargoFijo(Datos[56].ToString(), 50));
                    parametos.Add("IfD2Dato14", cadenaLargoFijo(Datos[57].ToString(), 50));
                    parametos.Add("IfD2Dato15", cadenaLargoFijo(Datos[58].ToString(), 50));
                    parametos.Add("IfD2Dato16", cadenaLargoFijo(Datos[59].ToString(), 50)); //2021 - 2030
                    parametos.Add("IfD2Dato17", cadenaLargoFijo(Datos[60].ToString(), 50));
                    parametos.Add("IfD2Dato18", cadenaLargoFijo(Datos[61].ToString(), 50));
                    //Total Capex
                    parametos.Add("IfD2Dato19", cadenaLargoFijo(Datos[62].ToString(), 50));

                    parametos.Add("PorInvNacExt", cadenaLargoFijo(PorInvNacExt, 10));
                    parametos.Add("Opcion", 1);
                    parametos.Add("NumIngreso", NumIngreso);
                }
                else
                {
                    parametos.Add("IniToken", cadenaLargoFijo(Datos[0].ToString(), 50));
                    parametos.Add("IniUsuario", cadenaLargoFijo(Datos[1].ToString(), 30));
                    parametos.Add("ParametroVNToken", cadenaLargoFijo(Datos[2].ToString(), 50));
                    parametos.Add("IfDato0", cadenaLargoFijo(Datos[3].ToString(), 50));
                    parametos.Add("IfDato1", cadenaLargoFijo(Datos[4].ToString(), 50));
                    parametos.Add("IfDato2", cadenaLargoFijo(Datos[5].ToString(), 50));
                    //Enero
                    parametos.Add("IfDato3", cadenaLargoFijo(Datos[6].ToString(), 50));
                    //Febrero
                    parametos.Add("IfDato4", cadenaLargoFijo(Datos[7].ToString(), 50));
                    //Marzo
                    parametos.Add("IfDato5", cadenaLargoFijo(Datos[8].ToString(), 50));
                    //Abril
                    parametos.Add("IfDato6", cadenaLargoFijo(Datos[9].ToString(), 50));
                    //Mayo
                    parametos.Add("IfDato7", cadenaLargoFijo(Datos[10].ToString(), 50));
                    //Junio
                    parametos.Add("IfDato8", cadenaLargoFijo(Datos[11].ToString(), 50));
                    //Julio
                    parametos.Add("IfDato9", cadenaLargoFijo(Datos[12].ToString(), 50));
                    //Agosto
                    parametos.Add("IfDato10", cadenaLargoFijo(Datos[13].ToString(), 50));
                    //Septiembre
                    parametos.Add("IfDato11", cadenaLargoFijo(Datos[14].ToString(), 50));
                    //Octubre
                    parametos.Add("IfDato12", cadenaLargoFijo(Datos[15].ToString(), 50));
                    //Noviembre
                    parametos.Add("IfDato13", cadenaLargoFijo(Datos[16].ToString(), 50));
                    //Diciembre
                    parametos.Add("IfDato14", cadenaLargoFijo(Datos[17].ToString(), 50));
                    //Total meses
                    parametos.Add("IfDato15", cadenaLargoFijo(Datos[18].ToString(), 50));
                    //AñoMas1
                    parametos.Add("IfDato16", cadenaLargoFijo(Datos[19].ToString(), 50));
                    //AñoMas2
                    parametos.Add("IfDato17", cadenaLargoFijo(Datos[20].ToString(), 50));
                    //AñoMas3
                    parametos.Add("IfDato18", cadenaLargoFijo(Datos[21].ToString(), 50));
                    //Total Capex
                    parametos.Add("IfDato19", cadenaLargoFijo(Datos[22].ToString(), 50));
                    //fila 2
                    parametos.Add("IfD1Dato0", "");
                    parametos.Add("IfD1Dato1", "");
                    parametos.Add("IfD1Dato2", "");
                    //Enero
                    parametos.Add("IfD1Dato3", "");
                    //Febrero
                    parametos.Add("IfD1Dato4", "");
                    //Marzo
                    parametos.Add("IfD1Dato5", "");
                    //Abril
                    parametos.Add("IfD1Dato6", "");
                    //Mayo
                    parametos.Add("IfD1Dato7", "");
                    //Junio
                    parametos.Add("IfD1Dato8", "");
                    //Julio
                    parametos.Add("IfD1Dato9", "");
                    //Agosto
                    parametos.Add("IfD1Dato10", "");
                    //Septiembre
                    parametos.Add("IfD1Dato11", "");
                    //Octubre
                    parametos.Add("IfD1Dato12", "");
                    //Noviembre
                    parametos.Add("IfD1Dato13", "");
                    //Diciembre
                    parametos.Add("IfD1Dato14", "");
                    //Total meses
                    parametos.Add("IfD1Dato15", "");
                    //AñoMas1
                    parametos.Add("IfD1Dato16", "");
                    //AñoMas2
                    parametos.Add("IfD1Dato17", "");
                    //AñoMas3
                    parametos.Add("IfD1Dato18", "");
                    //Total Capex
                    parametos.Add("IfD1Dato19", "");
                    //fila 3
                    parametos.Add("IfD2Dato0", "");
                    parametos.Add("IfD2Dato1", "");
                    parametos.Add("IfD2Dato2", "");
                    //Enero
                    parametos.Add("IfD2Dato3", "");
                    //Febrero
                    parametos.Add("IfD2Dato4", "");
                    //Marzo
                    parametos.Add("IfD2Dato5", "");
                    //Abril
                    parametos.Add("IfD2Dato6", "");
                    //Mayo
                    parametos.Add("IfD2Dato7", "");
                    //Junio
                    parametos.Add("IfD2Dato8", "");
                    //Julio
                    parametos.Add("IfD2Dato9", "");
                    //Agosto
                    parametos.Add("IfD2Dato10", "");
                    //Septiembre
                    parametos.Add("IfD2Dato11", "");
                    //Octubre
                    parametos.Add("IfD2Dato12", "");
                    //Noviembre
                    parametos.Add("IfD2Dato13", "");
                    //Diciembre
                    parametos.Add("IfD2Dato14", "");
                    //Total meses
                    parametos.Add("IfD2Dato15", "");
                    //AñoMas1
                    parametos.Add("IfD2Dato16", "");
                    //AñoMas2
                    parametos.Add("IfD2Dato17", "");
                    //AñoMas3
                    parametos.Add("IfD2Dato18", "");
                    //Total Capex
                    parametos.Add("IfD2Dato19", "");

                    parametos.Add("PorInvNacExt", cadenaLargoFijo(PorInvNacExt, 10));
                    parametos.Add("Opcion", 0);
                    parametos.Add("NumIngreso", NumIngreso);
                }
                SqlMapper.Execute(objConnection, "CAPEX_INS_INFORMACION_FINANCIERA_PRESUPUESTO_2_PARAMETROVN", parametos, commandType: CommandType.StoredProcedure);
            }
            catch (Exception err)
            {
                ExceptionResult = "InsertarInformacionFinancieraPresupuestoParametroVN, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
            }
        }

        private string cadenaLargoFijo(string param, int size)
        {
            if (param != null && !string.IsNullOrEmpty(param.Trim()))
            {
                if (param.Trim().Length <= size)
                {
                    return param.Trim();
                }
                else
                {
                    param.Trim().Substring(0, size);
                }
            }
            return "";
        }


        /// <summary>
        /// VALIDAR ESTADO PARAMETRO
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FNRealizarValidacionIniciativaExiste(Template.ValidarTemplateExiste Datos)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        var Usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                        var parametos = new DynamicParameters();
                        parametos.Add("TPEPERIODO", Datos.TPEPERIODO);
                        parametos.Add("ITPEToken", Datos.ITPEToken);
                        parametos.Add("TipoIniciativaSeleccionado", Datos.TipoIniciativaSeleccionado);
                        parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 120);
                        SqlMapper.Query(objConnection, "CAPEX_VAL_EXIST_TEMPLATE", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                        string respuesta = parametos.Get<string>("Respuesta");
                        if (respuesta != null && !string.IsNullOrEmpty(respuesta.Trim()) && (Int32.Parse(respuesta.Trim()) > 0))
                        {
                            return Json(new { Mensaje = "true" }, JsonRequestBehavior.AllowGet);
                        }
                        return Json(new { Mensaje = "false" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception err)
                    {
                        err.ToString();
                        return Json(new { Mensaje = "false" }, JsonRequestBehavior.AllowGet);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// VALIDAR ESTADO PARAMETRO
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ValidarEstadoParametroV0DesdeMantenedor(Template.ValidarEstadoParametroVN Datos)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    objConnection.Open();
                    try
                    {
                        var Usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                        int response = SqlMapper.Query<int>(objConnection, "CAPEX_VAL_EXIST_PARAMETRO_VN", new { TIPOSELECCIONADO = Datos.TIPOSELECCIONADO, PERIODO = Datos.ANIOSELECCIONADO }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                        if (response > 0)
                        {
                            return Json(new { Mensaje = "true" }, JsonRequestBehavior.AllowGet);
                        }
                        return Json(new { Mensaje = "false" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception err)
                    {
                        err.ToString();
                        return Json(new { Mensaje = "false" }, JsonRequestBehavior.AllowGet);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
        }


        /// <summary>
        /// VALIDAR ESTADO PARAMETRO
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ValidarEstadoParametroV0()
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    objConnection.Open();
                    try
                    {
                        var tipoIniciativaSeleccionadoRecuperado = Convert.ToString(Session["tipoIniciativaSeleccionado"]);
                        if (string.IsNullOrEmpty(tipoIniciativaSeleccionadoRecuperado))
                        {
                            tipoIniciativaSeleccionadoRecuperado = "0";
                        }
                        string Tipo = string.Empty;
                        string Periodo = string.Empty;
                        var Usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                        DateTime now = DateTime.Today;
                        int anioActual = Convert.ToInt32(now.ToString("yyyy"));
                        if (tipoIniciativaSeleccionadoRecuperado.Equals("1"))
                        {
                            Tipo = "2";
                            Periodo = (anioActual + 1) + "";
                        }
                        else
                        {
                            Tipo = "1";
                            Periodo = anioActual + "";
                        }
                        int response = SqlMapper.Query<int>(objConnection, "CAPEX_VAL_EXIST_PARAMETRO_VN", new { TIPOSELECCIONADO = Tipo, PERIODO = Periodo }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                        if (response > 0)
                        {
                            return Json(new { Mensaje = "true" }, JsonRequestBehavior.AllowGet);
                        }
                        return Json(new { Mensaje = "false" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception err)
                    {
                        err.ToString();
                        return Json(new { Mensaje = "false" }, JsonRequestBehavior.AllowGet);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// METODO PARA GUARDAR MATRIZ RIESGO
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GuardarParametrosVN")]
        public ActionResult GuardarParametrosVN(Template.GuardarOrientacionCorregido Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                string ParametroVNToken = string.Empty;
                try
                {
                    objConnection.Open();
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var parametos = new DynamicParameters();
                    parametos.Add("USUARIO", usuario);
                    parametos.Add("TPEPERIODO", Datos.TPEPERIODO);
                    parametos.Add("TipoIniciativaSeleccionado", Datos.TipoIniciativaSeleccionado);
                    parametos.Add("ORIGENPARAMETROVN", Datos.ORIGENPARAMETROVN);
                    parametos.Add("VERSIONPARAMETROVN", Datos.VERSIONPARAMETROVN.Replace("V", "").Replace("v", ""));
                    parametos.Add("VALUEMESTC", Datos.VALUEMESTC);
                    parametos.Add("VALUEMESIPC", Datos.VALUEMESIPC);
                    parametos.Add("VALUEMESCPI", Datos.VALUEMESCPI);
                    parametos.Add("VALUEANIOTC", Datos.VALUEANIOTC);
                    parametos.Add("VALUEANIOIPC", Datos.VALUEANIOIPC);
                    parametos.Add("VALUEANIOCPI", Datos.VALUEANIOCPI);
                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 100);
                    SqlMapper.Query(objConnection, "CAPEX_INS_PARAMETRO_VN", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    string respuesta = parametos.Get<string>("Respuesta");
                    if (respuesta != null && !string.IsNullOrEmpty(respuesta.Trim()))
                    {
                        if (respuesta.Split('|') != null && respuesta.Split('|').Length >= 2)
                        {
                            ParametroVNToken = respuesta.Split('|')[2];
                        }
                        return Json(new { Mensaje = respuesta.Trim() }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "1|Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception err)
                {
                    var ExceptionResult = "GuardarParametrosVN, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "1|Error|No es posible guardar valores parámetros " + Datos.VERSIONPARAMETROVN + " . Por favor, inténtelo más tarde." }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                    if (!string.IsNullOrEmpty(ParametroVNToken))
                    {
                        GenerarVersionParametroVNToken(ParametroVNToken);
                    }
                }
            }
        }

        [HttpPost]
        [Route("GenerarEjercicionOficial")]
        public ActionResult GenerarEjercicionOficial(string ParametroVNToken)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                //return RedirectToAction("Logout", "Login");
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        string Tipo = string.Empty;
                        string Periodo = string.Empty;
                        var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                        var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                        objConnection.Open();
                        var parametos = new DynamicParameters();
                        parametos.Add("ParametroVNToken", ParametroVNToken);
                        parametos.Add("PidUsuario", usuario);
                        parametos.Add("PidRol", rol);
                        parametos.Add("ErrorCodeOutput", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 200);
                        SqlMapper.Query(objConnection, "CAPEX_CONVERTIR_ParametroVN_EJERCICIO_OFICIAL", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                        string ErrorCodeOutput = parametos.Get<string>("ErrorCodeOutput");
                        if (ErrorCodeOutput != null && !string.IsNullOrEmpty(ErrorCodeOutput.Trim()) && ErrorCodeOutput.Trim().StartsWith("0"))
                        {
                            return Json(new { Mensaje = "Guardado|" + parametos.Get<string>("ErrorCodeOutput") }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { Mensaje = "Error" + parametos.Get<string>("ErrorCodeOutput") }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception err)
                    {
                        var ExceptionResult = "GuardarComentario, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
        }

    }
}