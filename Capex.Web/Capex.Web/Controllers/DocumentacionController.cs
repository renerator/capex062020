using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapexIdentity.Utilities;
using CapexInfraestructure.Bll.Entities.Documentacion;
using CapexInfraestructure.Bll.Business.Documentacion;
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
using SHARED.AzureStorage;
using CapexInfraestructure.Bll.Business.Planificacion;
using System.Configuration;
using System.Net;
using System.Net.Mime;
using ClosedXML.Excel;
using System.Globalization;

namespace Capex.Web.Controllers
{

    static class CurrentMillis
    {
        private static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        /// <summary>Get extra long current timestamp</summary>
        public static long Millis { get { return (long)((DateTime.UtcNow - Jan1St1970).TotalMilliseconds); } }
    }

    [AuthorizeAdminOrMember]
    [RoutePrefix("Documentacion")]
    public class DocumentacionController : Controller
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
        ///     CONTROLADOR "DocumentacionController" 
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
        //DOCUMENTACION
        //private const DocumentacionFactory.tipo LD = DocumentacionFactory.tipo.ListarDocumentos;

        private const PlanificacionFactory.tipo VA = PlanificacionFactory.tipo.VerAdjunto;
        #endregion

        #region "CAMPOS"
        //DOCUMENTACION
        //public static DocumentacionFactory FactoryDocumentacion;
        //public static IDocumentacion IDocumentacion;
        //public SqlConnection ORM;
        #endregion

        //IDENTIFICACION
        public static PlanificacionFactory FactoryPlanificacion;
        public static IPlanificacion IPlanificacion;

        #region "CONSTRUCTOR"
        public DocumentacionController()
        {
            //DOCUMENTACION
            //FactoryDocumentacion = new DocumentacionFactory();
            //IDENTIFICACION
            FactoryPlanificacion = new PlanificacionFactory();
            JsonResponse = string.Empty;
            //ORM = CapexInfraestructure.Utilities.Utils.Conectar();
        }
        #endregion

        #region "METODOS DCUMENTACION - VISTA INICIAL"
        public class ModeloCategorias
        {
            public string DocCatToken { get; set; }
            public string DocCatNombre { get; set; }
        }
        /// <summary>
        /// METODO INICIAL - CONTRUCCION DE LAYOUT
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        //[Route("Index")]
        [Route]
        public ActionResult Index()
        {
            var ComToken = "F1DBFB20-6DCA-4537-A66E-53E3B3C3B830";
            var Categoria = string.Empty;
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    ViewBag.Categoria = SqlMapper.Query(objConnection, "CAPEX_SEL_DOCUMENTACION_CATEGORIA", new { @ComToken = ComToken }, commandType: CommandType.StoredProcedure).ToList();
                    ViewBag.Documentos = SqlMapper.Query(objConnection, "CAPEX_SEL_DOCUMENTACION_LISTAR", new { @Categoria = Categoria }, commandType: CommandType.StoredProcedure).ToList();
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
            return View("Index");
        }
        #endregion
        #region "FILTRADO DE VISTA PRINCIPAL"
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Categoria"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{Categoria}")]
        public ActionResult Index(string Categoria)
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                var ComToken = "F1DBFB20-6DCA-4537-A66E-53E3B3C3B830";
                if (string.IsNullOrEmpty(Categoria))
                {
                    Categoria = string.Empty;
                }
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        ViewBag.Categoria = SqlMapper.Query(objConnection, "CAPEX_SEL_DOCUMENTACION_CATEGORIA", new { @ComToken = ComToken }, commandType: CommandType.StoredProcedure).ToList();
                        ViewBag.Documentos = SqlMapper.Query(objConnection, "CAPEX_SEL_DOCUMENTACION_LISTAR", new { @Categoria = Categoria }, commandType: CommandType.StoredProcedure).ToList();
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
                return View("Index");
            }
        }
        #endregion

        #region "ACCIONES DE DOCUMENTACION"
        /// <summary>
        /// METODO PARA CREAR UNA CATEGORIA DOCUMENTAL
        /// </summary>
        /// <param name="CatNombre"></param>
        /// <param name="ComToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Accion/CrearCategoria")]
        public ActionResult CrearCategoria(string CatNombre, string ComToken)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    SqlMapper.Query(objConnection, "CAPEX_INS_DOCUMENTACION_CATEGORIA", new { CatNombre, ComToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Creado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    ExceptionResult = "CrearCategoria, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
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
        /// METODO LISTAR CTAGORIA
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Accion/ListarCategoria")]
        public ActionResult ListarCategoria()
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var ComToken = "F1DBFB20-6DCA-4537-A66E-53E3B3C3B830";
                    var Categorias = SqlMapper.Query<Principal.Categorias>(objConnection, "CAPEX_SEL_DOCUMENTACION_CATEGORIA", new { @ComToken = ComToken }, commandType: CommandType.StoredProcedure).ToList();
                    return Json(new { Resultado = Categorias }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    ExceptionResult = "ListarCategoria, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Resultado = "ERROR" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }
        /// <summary>
        /// METODO PARA SUBIR ARCHIVO DE ANALISIS DE BAJA COMPLEJIDAD
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Accion/SubirDocumento")]
        public JsonResult SubirDocumento()
        {
            string resultado = string.Empty;
            try
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    int fileSize = file.ContentLength;
                    string fileName = file.FileName;
                    string mimeType = file.ContentType;
                    System.IO.Stream fileContent = file.InputStream;

                    string path = Server.MapPath("~/Files/DocumentosGenerales");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    var donde = Path.Combine(Server.MapPath("~/Files/DocumentosGenerales"), fileName);
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
        /// <summary>
        /// METODO DESCARGAR DOCUMENTO
        /// </summary>
        /// <returns></returns>
        [Route("DescargarDocumento/{Documento}")]
        public ActionResult DescargarDocumento(string Documento)
        {
            string nueva_ruta = string.Empty;
            nueva_ruta = "Files/DocumentosGenerales/" + Documento;
            string path = AppDomain.CurrentDomain.BaseDirectory + nueva_ruta;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            string fileName = Documento;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpGet]
        [Route("DescargarDocumentoAdjuntoFinal/{token}")]
        public JsonResult DescargarDocumentoAdjunto(string token)
        {
            try
            {
                IPlanificacion = FactoryPlanificacion.delega(VA);
                string mensaje = String.Empty;
                var adjunto = IPlanificacion.SeleccionarAdjunto(token);
                if (adjunto != null && !string.IsNullOrEmpty(adjunto.ShareFile) && !string.IsNullOrEmpty(adjunto.ParNombreFinal))
                {
                    string shareFile = adjunto.ShareFile;
                    string pathDirectory = adjunto.PathDirectory;
                    string nameFileFinal = adjunto.ParNombreFinal;
                    UploadDownload uploadDownload = new UploadDownload();
                    string urlAzure = UploadDownload.DownloadFile(shareFile, pathDirectory, nameFileFinal);
                    return Json(new { IsSuccess = true, Message = "Ok", ResponseData = urlAzure }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false, Message = "Error" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception err)
            {
                ExceptionResult = "DescargarDocumentoAdjunto, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { IsSuccess = false, Message = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Route("DescargarDocumentoBiblitecaFinal/{token}")]
        public JsonResult DescargarDocumentoBiblitecaFinal(string token)
        {
            try
            {
                IPlanificacion = FactoryPlanificacion.delega(VA);
                string mensaje = String.Empty;
                var documentoBiblioteca = IPlanificacion.SeleccionarDocumentoBiblioteca(token);
                if (documentoBiblioteca != null)
                {
                    string shareFile = ConfigurationManager.AppSettings.Get("Shared");
                    string pathDirectory = ConfigurationManager.AppSettings.Get("PathDirectory") + "\\Biblioteca\\" + documentoBiblioteca.DocCatNombre;
                    string nameFileFinal = documentoBiblioteca.DocNombre;
                    UploadDownload uploadDownload = new UploadDownload();
                    string urlAzure = UploadDownload.DownloadFile(shareFile, pathDirectory, nameFileFinal);
                    return Json(new { IsSuccess = true, Message = "Ok", ResponseData = urlAzure }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false, Message = "Error" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception err)
            {
                ExceptionResult = "DescargarDocumentoBiblitecaFinal, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { IsSuccess = false, Message = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        [Route("SeleccionarExcelTemplatePeriodo/{tipoIniciativaSeleccionado}/{periodo}")]
        public JsonResult SeleccionarExcelTemplatePeriodo(string tipoIniciativaSeleccionado, string periodo)
        {
            try
            {
                IPlanificacion = FactoryPlanificacion.delega(VA);
                string mensaje = String.Empty;
                var adjunto = IPlanificacion.SeleccionarExcelTemplatePeriodo(tipoIniciativaSeleccionado, periodo);
                if (adjunto != null && !string.IsNullOrEmpty(adjunto.ShareFile) && !string.IsNullOrEmpty(adjunto.ParNombreFinal))
                {
                    return Json(new { IsSuccess = true, Message = "Ok", ResponseData = adjunto }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false, Message = "Error" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception err)
            {
                ExceptionResult = "SeleccionarExcelTemplatePeriodo, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { IsSuccess = false, Message = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        [Route("DescargarExcelTemplate/{token}")]
        public JsonResult DescargarExcelTemplate(string token)
        {
            try
            {
                IPlanificacion = FactoryPlanificacion.delega(VA);
                string mensaje = String.Empty;
                var adjunto = IPlanificacion.SeleccionarExcelTemplate(token);
                if (adjunto != null && !string.IsNullOrEmpty(adjunto.ShareFile) && !string.IsNullOrEmpty(adjunto.ParNombreFinal))
                {
                    string shareFile = adjunto.ShareFile;
                    string pathDirectory = adjunto.PathDirectory;
                    string nameFileFinal = adjunto.ParNombreFinal;
                    UploadDownload uploadDownload = new UploadDownload();
                    string urlAzure = UploadDownload.DownloadFile(shareFile, pathDirectory, nameFileFinal);
                    return Json(new { IsSuccess = true, Message = "Ok", ResponseData = urlAzure }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false, Message = "Error" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception err)
            {
                ExceptionResult = "DescargarDocumentoAdjunto, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { IsSuccess = false, Message = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        private string DownloadFile(string url, string token, string filename, string tipo)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.ContentType = "application/x-www-form-urlencoded";
            string destinationpath = Server.MapPath("~/Scripts/Import/" + token);
            string filenameFinal = (("2".Equals(tipo) ? "PRESUPUESTO_" : "CASO_BASE_") + CurrentMillis.Millis + Path.GetExtension(filename));
            string rutaFinal = Path.Combine(destinationpath, filenameFinal);
            if (!Directory.Exists(destinationpath))
            {
                Directory.CreateDirectory(destinationpath);
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().Result)
            {
                var responseStream = response.GetResponseStream();
                using (var fileStream = new FileStream(rutaFinal, FileMode.Append))
                {
                    try
                    {
                        responseStream.CopyTo(fileStream);
                    }
                    catch (Exception err)
                    {
                        err.ToString();
                    }
                    finally
                    {
                        if (responseStream != null)
                        {
                            responseStream.Close();
                        }
                        if (fileStream != null)
                        {
                            fileStream.Close();
                        }
                    }
                }
            }
            return filenameFinal;
        }

        [Route("DescargarExcelTemplateFinal2Pasos/{token}/{iniciativaToken}/{filename}/{tipo}")]
        public ActionResult DescargarExcelTemplateFinal2Pasos(string token, string iniciativaToken, string filename, string tipo)
        {
            string ruta = Path.Combine(Server.MapPath("~/Scripts/Import/" + token), filename);
            FileStream fstream = new FileStream(ruta, FileMode.Open);
            XLWorkbook wb = new XLWorkbook(fstream);
            var sheetOne = wb.Worksheet(2);
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    CultureInfo ciCL = new CultureInfo("es-CL", false);
                    objConnection.Open();
                    var resultado = SqlMapper.Query(objConnection, "CAPEX_SEL_TEMPLATE_BY_TOKEN_INICIATIVA", new { Token = iniciativaToken, TipoIniciativaSeleccionado = tipo }, commandType: CommandType.StoredProcedure).ToList();
                    if (resultado != null && resultado.Count > 0)
                    {
                        foreach (var result in resultado)
                        {
                            sheetOne.Cell("D27").Value = result.VALUEMESTCUNO;
                            sheetOne.Cell("D27").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("E27").Value = result.VALUEMESTCDOS;
                            sheetOne.Cell("E27").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("F27").Value = result.VALUEMESTCTRES;
                            sheetOne.Cell("F27").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("G27").Value = result.VALUEMESTCCUATRO;
                            sheetOne.Cell("G27").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("H27").Value = result.VALUEMESTCCINCO;
                            sheetOne.Cell("H27").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("I27").Value = result.VALUEMESTCSEIS;
                            sheetOne.Cell("I27").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("J27").Value = result.VALUEMESTCSIETE;
                            sheetOne.Cell("J27").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("K27").Value = result.VALUEMESTCOCHO;
                            sheetOne.Cell("K27").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("L27").Value = result.VALUEMESTCNUEVE;
                            sheetOne.Cell("L27").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("M27").Value = result.VALUEMESTCDIEZ;
                            sheetOne.Cell("M27").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("N27").Value = result.VALUEMESTCONCE;
                            sheetOne.Cell("N27").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("O27").Value = result.VALUEMESTCDOCE;
                            sheetOne.Cell("O27").Style.NumberFormat.Format = "#,##0.00";

                            sheetOne.Cell("D28").Value = result.VALUEMESIPCUNO;
                            sheetOne.Cell("D28").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("E28").Value = result.VALUEMESIPCDOS;
                            sheetOne.Cell("E28").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("F28").Value = result.VALUEMESIPCTRES;
                            sheetOne.Cell("F28").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("G28").Value = result.VALUEMESIPCCUATRO;
                            sheetOne.Cell("G28").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("H28").Value = result.VALUEMESIPCCINCO;
                            sheetOne.Cell("H28").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("I28").Value = result.VALUEMESIPCSEIS;
                            sheetOne.Cell("I28").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("J28").Value = result.VALUEMESIPCSIETE;
                            sheetOne.Cell("J28").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("K28").Value = result.VALUEMESIPCOCHO;
                            sheetOne.Cell("K28").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("L28").Value = result.VALUEMESIPCNUEVE;
                            sheetOne.Cell("L28").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("M28").Value = result.VALUEMESIPCDIEZ;
                            sheetOne.Cell("M28").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("N28").Value = result.VALUEMESIPCONCE;
                            sheetOne.Cell("N28").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("O28").Value = result.VALUEMESIPCDOCE;
                            sheetOne.Cell("O28").Style.NumberFormat.Format = "#,##0.00";

                            sheetOne.Cell("D29").Value = result.VALUEMESCPIUNO;
                            sheetOne.Cell("D29").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("E29").Value = result.VALUEMESCPIDOS;
                            sheetOne.Cell("E29").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("F29").Value = result.VALUEMESCPITRES;
                            sheetOne.Cell("F29").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("G29").Value = result.VALUEMESCPICUATRO;
                            sheetOne.Cell("G29").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("H29").Value = result.VALUEMESCPICINCO;
                            sheetOne.Cell("H29").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("I29").Value = result.VALUEMESCPISEIS;
                            sheetOne.Cell("I29").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("J29").Value = result.VALUEMESCPISIETE;
                            sheetOne.Cell("J29").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("K29").Value = result.VALUEMESCPIOCHO;
                            sheetOne.Cell("K29").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("L29").Value = result.VALUEMESCPINUEVE;
                            sheetOne.Cell("L29").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("M29").Value = result.VALUEMESCPIDIEZ;
                            sheetOne.Cell("M29").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("N29").Value = result.VALUEMESCPIONCE;
                            sheetOne.Cell("N29").Style.NumberFormat.Format = "#,##0.00";
                            sheetOne.Cell("O29").Value = result.VALUEMESCPIDOCE;
                            sheetOne.Cell("O29").Style.NumberFormat.Format = "#,##0.00";

                            int pasoFinal = (("2".Equals(tipo)) ? 3 : 80);
                            if (!string.IsNullOrEmpty(result.PETokenTC))
                            {
                                var resultsTcAnio = SqlMapper.Query(objConnection, "CAPEX_SEL_PARAM_ECONOMICO_DETALLE_TOKEN", new { PEToken = result.PETokenTC, PasoFinal = pasoFinal }, commandType: CommandType.StoredProcedure).ToList();
                                if (resultsTcAnio != null && resultsTcAnio.Count > 0)
                                {
                                    var columna = 17;
                                    foreach (var result2 in resultsTcAnio)
                                    {
                                        if (result2.IdParamEconomicoDetalle != null)
                                        {
                                            sheetOne.Cell(27, columna).Value = result2.Value;
                                            sheetOne.Cell(27, columna).Style.NumberFormat.Format = "#,##0.00";
                                        }
                                        columna++;
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(result.PETokenIPC))
                            {
                                var resultsIpcAnio = SqlMapper.Query(objConnection, "CAPEX_SEL_PARAM_ECONOMICO_DETALLE_TOKEN", new { PEToken = result.PETokenIPC, PasoFinal = pasoFinal }, commandType: CommandType.StoredProcedure).ToList();
                                if (resultsIpcAnio != null && resultsIpcAnio.Count > 0)
                                {
                                    var columna = 17;
                                    foreach (var result2 in resultsIpcAnio)
                                    {
                                        if (result2.IdParamEconomicoDetalle != null)
                                        {
                                            sheetOne.Cell(28, columna).Value = result2.Value;
                                            sheetOne.Cell(28, columna).Style.NumberFormat.Format = "#,##0.00";
                                        }
                                        columna++;
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(result.PETokenCPI))
                            {
                                var resultsCpiAnio = SqlMapper.Query(objConnection, "CAPEX_SEL_PARAM_ECONOMICO_DETALLE_TOKEN", new { PEToken = result.PETokenCPI, PasoFinal = pasoFinal }, commandType: CommandType.StoredProcedure).ToList();
                                if (resultsCpiAnio != null && resultsCpiAnio.Count > 0)
                                {
                                    var columna = 17;
                                    foreach (var result2 in resultsCpiAnio)
                                    {
                                        if (result2.IdParamEconomicoDetalle != null)
                                        {
                                            sheetOne.Cell(29, columna).Value = result2.Value;
                                            sheetOne.Cell(29, columna).Style.NumberFormat.Format = "#,##0.00";
                                        }
                                        columna++;
                                    }
                                }
                            }
                        }
                    }
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                    }
                }
                catch (Exception e)
                {
                    e.ToString();
                }
                finally
                {
                    objConnection.Close();
                    fstream.Close();
                    if (!string.IsNullOrEmpty(ruta))
                    {
                        try
                        {
                            if (System.IO.File.Exists(ruta))
                            {
                                System.IO.File.Delete(ruta);
                            }
                        }
                        catch (Exception Exp)
                        {
                            Exp.ToString();
                        }
                    }
                }
            }
            return null;
        }


        [HttpGet]
        [Route("DescargarExcelTemplate2Pasos/{token}/")]
        public JsonResult DescargarExcelTemplate2Pasos(string token, string tipo)
        {
            try
            {
                IPlanificacion = FactoryPlanificacion.delega(VA);
                string mensaje = String.Empty;
                var adjunto = IPlanificacion.SeleccionarExcelTemplate(token);
                if (adjunto != null && !string.IsNullOrEmpty(adjunto.ShareFile) && !string.IsNullOrEmpty(adjunto.ParNombreFinal))
                {
                    string shareFile = adjunto.ShareFile;
                    string pathDirectory = adjunto.PathDirectory;
                    string nameFileFinal = adjunto.ParNombreFinal;
                    UploadDownload uploadDownload = new UploadDownload();
                    string urlAzure = UploadDownload.DownloadFile(shareFile, pathDirectory, nameFileFinal);
                    if (!string.IsNullOrEmpty(urlAzure))
                    {
                        string filename = DownloadFile(urlAzure, token, nameFileFinal, tipo);
                        if (!string.IsNullOrEmpty(filename))
                        {
                            return Json(new { IsSuccess = true, Message = "Ok", ResponseData = filename, ParToken = token }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { IsSuccess = false, Message = "Error" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { IsSuccess = false, Message = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { IsSuccess = false, Message = "Error" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception err)
            {
                ExceptionResult = "DescargarExcelTemplate2Pasos, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { IsSuccess = false, Message = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [Route("DescargarDocumentoAdjunto/{token}/{archivo}/{paso}")]
        public ActionResult DescargarDocumentoAdjunto(string token, string archivo, string paso)
        {
            string nueva_ruta = "Files/Iniciativas/";
            // D43191F3 - 522E-4455 - 8E71 - 176E30D4E4D6 / Formato % 20presentacion_Evaluación % 20de % 20Riesgos_capex2020.pptx / Evaluacion - Riesgo /
            switch (paso.Trim())
            {
                case "Presupuesto-Gantt":
                    nueva_ruta += "Presupuesto/Gantt/" + token.Trim() + "/";
                    break;
                case "Evaluacion-Economica":
                    nueva_ruta += "EvaluacionEconomica/" + token.Trim() + "/";
                    break;
                case "Evaluacion-Riesgo":
                    nueva_ruta += "EvaluacionRiesgo/" + token.Trim() + "/";
                    break;
                case "Categorizacion":
                    nueva_ruta += "Categorizacion/" + token.Trim() + "/";
                    break;
                case "Descripcion-Detallada":
                    nueva_ruta += "Descripcion/" + token.Trim() + "/";
                    break;
            }
            nueva_ruta += archivo.Trim();
            string path = AppDomain.CurrentDomain.BaseDirectory + nueva_ruta;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            string fileName = archivo.Trim();
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        /// <summary>
        /// METODO REGISTRO DOCUMENTO
        /// </summary>
        /// <param name="IniToken"></param>
        /// <param name="ParUsuario"></param>
        /// <param name="ParNombre"></param>
        /// <param name="ParPaso"></param>
        /// <param name="ParCaso"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Accion/RegistrarDocumento")]
        public ActionResult RegistrarDocumento(string Documento, int Tamano, string Extension, string Tipo, string Categoria)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    SqlMapper.Query(objConnection, "CAPEX_INS_DOCUMENTACION", new { Documento, Tamano, Extension, Tipo, Categoria }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Registrado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exc)
                {
                    ExceptionResult = "RegistrarDocumento, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
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
        /// METODO ELIMINAR DOCUMENTO
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Accion/EliminarDocumento")]
        public ActionResult EliminarDocumento(string Token)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    IPlanificacion = FactoryPlanificacion.delega(VA);
                    string mensaje = String.Empty;
                    var documentoBiblioteca = IPlanificacion.SeleccionarDocumentoBiblioteca(Token);
                    if (documentoBiblioteca != null)
                    {
                        string shareFile = ConfigurationManager.AppSettings.Get("Shared");
                        string pathDirectory = ConfigurationManager.AppSettings.Get("PathDirectory") + "\\Biblioteca\\" + documentoBiblioteca.DocCatNombre;
                        string nameFileFinal = documentoBiblioteca.DocNombre;
                        UploadDownload uploadDownload = new UploadDownload();
                        if (UploadDownload.DeleteFile(shareFile, pathDirectory, nameFileFinal))
                        {
                            SqlMapper.Query(objConnection, "CAPEX_DEL_ELIMINAR_DOCUMENTO", new { @Token = Token }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                            return Json(new { Mensaje = "Eliminado" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception exc)
                {
                    ExceptionResult = "EliminarDocumento, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
            #endregion

        }
    }
}