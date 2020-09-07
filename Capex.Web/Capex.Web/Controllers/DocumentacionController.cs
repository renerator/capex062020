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
using CapexInfraestructure.Bll.Entities.Planificacion;

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

        private string DownloadFileParametroVN(string url, string token, string filename, string tipo)
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


        private decimal stringToDecimalFromDB(string paramValue)
        {
            try
            {
                return decimal.Parse(paramValue, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.GetCultureInfo("es-CL"));
            }
            catch (Exception err)
            {
                err.ToString();
            }
            return 0;
        }


        private void modificarValoresExcelPresupuesto(IXLWorksheet sheetOne, string iniciativaToken, string tipoIniciativaSeleccionada, string anioIniciativaSeleccionada, string parametroVNToken)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    string tipoTC = "1";
                    string tipoIPC = "2";
                    string tipoCPI = "3";
                    IList<Presupuesto.ParametroOrientacionComercialMes> orientacionComercialTCMes = null;
                    IList<Presupuesto.ParametroOrientacionComercialAnio> orientacionComercialTCAnio = null;
                    IList<Presupuesto.ParametroOrientacionComercialMes> orientacionComercialIPCMes = null;
                    IList<Presupuesto.ParametroOrientacionComercialAnio> orientacionComercialIPCAnio = null;
                    IList<Presupuesto.ParametroOrientacionComercialMes> orientacionComercialCPIMes = null;
                    IList<Presupuesto.ParametroOrientacionComercialAnio> orientacionComercialCPIAnio = null;
                    CultureInfo ciCL = new CultureInfo("es-CL", false);
                    var paramIniTokenRemanente = new DynamicParameters();
                    paramIniTokenRemanente.Add("IniToken", iniciativaToken);
                    paramIniTokenRemanente.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 1);
                    SqlMapper.Query(objConnection, "CHECK_INITOKEN_REMANENTE", paramIniTokenRemanente, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    string iniTokenRemanente = paramIniTokenRemanente.Get<string>("Respuesta");
                    if (iniTokenRemanente != null && !string.IsNullOrEmpty(iniTokenRemanente.Trim()) && "1".Equals(iniTokenRemanente.Trim()))
                    {
                        orientacionComercialTCMes = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialMes>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_MES_V0", new { ParametroVNToken = parametroVNToken, TipoIniciativa = tipoIniciativaSeleccionada, TipoParam = tipoTC }, commandType: CommandType.StoredProcedure).ToList();
                        orientacionComercialTCAnio = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialAnio>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_ANIO_V0", new { ParametroVNToken = parametroVNToken, TipoIniciativa = tipoIniciativaSeleccionada, TipoParam = tipoTC }, commandType: CommandType.StoredProcedure).ToList();
                        orientacionComercialIPCMes = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialMes>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_MES_V0", new { ParametroVNToken = parametroVNToken, TipoIniciativa = tipoIniciativaSeleccionada, TipoParam = tipoIPC }, commandType: CommandType.StoredProcedure).ToList();
                        orientacionComercialIPCAnio = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialAnio>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_ANIO_V0", new { ParametroVNToken = parametroVNToken, TipoIniciativa = tipoIniciativaSeleccionada, TipoParam = tipoIPC }, commandType: CommandType.StoredProcedure).ToList();
                        orientacionComercialCPIMes = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialMes>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_MES_V0", new { ParametroVNToken = parametroVNToken, TipoIniciativa = tipoIniciativaSeleccionada, TipoParam = tipoCPI }, commandType: CommandType.StoredProcedure).ToList();
                        orientacionComercialCPIAnio = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialAnio>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_ANIO_V0", new { ParametroVNToken = parametroVNToken, TipoIniciativa = tipoIniciativaSeleccionada, TipoParam = tipoCPI }, commandType: CommandType.StoredProcedure).ToList();
                    }
                    else
                    {
                        orientacionComercialTCMes = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialMes>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_MES", new { ParametroVNToken = parametroVNToken, TipoIniciativa = tipoIniciativaSeleccionada, TipoParam = tipoTC }, commandType: CommandType.StoredProcedure).ToList();
                        orientacionComercialTCAnio = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialAnio>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_ANIO", new { ParametroVNToken = parametroVNToken, TipoIniciativa = tipoIniciativaSeleccionada, TipoParam = tipoTC }, commandType: CommandType.StoredProcedure).ToList();
                        orientacionComercialIPCMes = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialMes>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_MES", new { ParametroVNToken = parametroVNToken, TipoIniciativa = tipoIniciativaSeleccionada, TipoParam = tipoIPC }, commandType: CommandType.StoredProcedure).ToList();
                        orientacionComercialIPCAnio = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialAnio>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_ANIO", new { ParametroVNToken = parametroVNToken, TipoIniciativa = tipoIniciativaSeleccionada, TipoParam = tipoIPC }, commandType: CommandType.StoredProcedure).ToList();
                        orientacionComercialCPIMes = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialMes>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_MES", new { ParametroVNToken = parametroVNToken, TipoIniciativa = tipoIniciativaSeleccionada, TipoParam = tipoCPI }, commandType: CommandType.StoredProcedure).ToList();
                        orientacionComercialCPIAnio = SqlMapper.Query<Presupuesto.ParametroOrientacionComercialAnio>(objConnection, "CAPEX_OBTENER_PARAMETRO_ORIENTACION_COMERCIAL_ANIO", new { ParametroVNToken = parametroVNToken, TipoIniciativa = tipoIniciativaSeleccionada, TipoParam = tipoCPI }, commandType: CommandType.StoredProcedure).ToList();
                    }

                    if (orientacionComercialTCMes != null && orientacionComercialIPCMes != null && orientacionComercialCPIMes != null
                           && orientacionComercialTCMes.Count == 12 && orientacionComercialIPCMes.Count == 12 && orientacionComercialCPIMes.Count == 12)
                    {
                        int filaModificacion = 27;
                        for (int i = 0; i < orientacionComercialTCMes.Count; i += 1)
                        {
                            sheetOne.Cell(filaModificacion, (4 + i)).Value = orientacionComercialTCMes[i].Value;
                            sheetOne.Cell(filaModificacion, (4 + i)).Style.NumberFormat.Format = "#,##0.00";

                            sheetOne.Cell((filaModificacion + 1), (4 + i)).Value = orientacionComercialIPCMes[i].Value;
                            sheetOne.Cell((filaModificacion + 1), (4 + i)).Style.NumberFormat.Format = "#,##0.00";

                            sheetOne.Cell((filaModificacion + 2), (4 + i)).Value = orientacionComercialCPIMes[i].Value;
                            sheetOne.Cell((filaModificacion + 2), (4 + i)).Style.NumberFormat.Format = "#,##0.00";
                        }
                    }
                    int pasoFinal = (("2".Equals(tipoIniciativaSeleccionada)) ? 3 : 80);
                    if (orientacionComercialTCAnio != null && orientacionComercialIPCAnio != null && orientacionComercialCPIAnio != null
                        && orientacionComercialTCAnio.Count == pasoFinal && orientacionComercialIPCAnio.Count == pasoFinal && orientacionComercialCPIAnio.Count == pasoFinal)
                    {
                        int filaModificacion = 27;
                        for (int i = 0; i < orientacionComercialTCAnio.Count; i += 1)
                        {
                            sheetOne.Cell(filaModificacion, (17 + i)).Value = orientacionComercialTCAnio[i].Value;
                            sheetOne.Cell(filaModificacion, (17 + i)).Style.NumberFormat.Format = "#,##0.00";

                            sheetOne.Cell((filaModificacion + 1), (17 + i)).Value = orientacionComercialIPCAnio[i].Value;
                            sheetOne.Cell((filaModificacion + 1), (17 + i)).Style.NumberFormat.Format = "#,##0.00";

                            sheetOne.Cell((filaModificacion + 2), (17 + i)).Value = orientacionComercialCPIAnio[i].Value;
                            sheetOne.Cell((filaModificacion + 2), (17 + i)).Style.NumberFormat.Format = "#,##0.00";
                        }
                    }
                    var importacionFinancieroParametroVN = SqlMapper.Query(objConnection, "CAPEX_SEL_IMPORTACION_FINANCIERO_PARAMETROVN", new { IniToken = iniciativaToken, ParametroVNToken = parametroVNToken, TipoIniciativa = tipoIniciativaSeleccionada }, commandType: CommandType.StoredProcedure).ToList();
                    foreach (var impFinPVN in importacionFinancieroParametroVN)
                    {
                        string IfTokenParam = impFinPVN.IfToken;
                        string IfDato0Param = impFinPVN.IfDato0;
                        decimal perAntTotal = stringToDecimalFromDB(impFinPVN.IfDato2);
                        string prefixIng = "Ing";
                        string prefixAdq = "Adq";
                        string prefixCons = "Cons";
                        string prefixAdm = "Adm";
                        string prefixCont = "Cont";
                        string prefixMonNac = "Moneda Nac";
                        string prefixMonExt = "Moneda Ext";
                        if ("1".Equals(tipoIniciativaSeleccionada)) // caso base
                        {
                            IList<Presupuesto.FinancieroDetalleCasoBase> informacionFinancieroDetalleParametroVNOrigen = SqlMapper.Query<Presupuesto.FinancieroDetalleCasoBase>(objConnection, "CAPEX_SEL_IMPORTACION_FINANCIERO_DETALLE_PARAMETROVN", new { TipoIniciativa = tipoIniciativaSeleccionada, IfToken = IfTokenParam }, commandType: CommandType.StoredProcedure).ToList();
                            if (informacionFinancieroDetalleParametroVNOrigen != null && informacionFinancieroDetalleParametroVNOrigen.Count == 2)
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
                                if (IfDato0Param.StartsWith(prefixIng))
                                {
                                    sheetOne.Cell(9, 3).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato2);
                                    sheetOne.Cell(9, 3).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 3).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato2);
                                    sheetOne.Cell(10, 3).Style.NumberFormat.Format = "#,##0.00";
                                    //Enero
                                    sheetOne.Cell(9, 4).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato3);
                                    sheetOne.Cell(9, 4).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 4).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato3);
                                    sheetOne.Cell(10, 4).Style.NumberFormat.Format = "#,##0.00";
                                    //Febrero
                                    sheetOne.Cell(9, 5).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato4);
                                    sheetOne.Cell(9, 5).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 5).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato4);
                                    sheetOne.Cell(10, 5).Style.NumberFormat.Format = "#,##0.00";
                                    //Marzo
                                    sheetOne.Cell(9, 6).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato5);
                                    sheetOne.Cell(9, 6).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 6).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato5);
                                    sheetOne.Cell(10, 6).Style.NumberFormat.Format = "#,##0.00";
                                    //Abril
                                    sheetOne.Cell(9, 7).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato6);
                                    sheetOne.Cell(9, 7).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 7).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato6);
                                    sheetOne.Cell(10, 7).Style.NumberFormat.Format = "#,##0.00";
                                    //Mayo
                                    sheetOne.Cell(9, 8).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato7);
                                    sheetOne.Cell(9, 8).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 8).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato7);
                                    sheetOne.Cell(10, 8).Style.NumberFormat.Format = "#,##0.00";
                                    //Junio
                                    sheetOne.Cell(9, 9).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato8);
                                    sheetOne.Cell(9, 9).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 9).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato8);
                                    sheetOne.Cell(10, 9).Style.NumberFormat.Format = "#,##0.00";
                                    //Julio
                                    sheetOne.Cell(9, 10).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato9);
                                    sheetOne.Cell(9, 10).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 10).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato9);
                                    sheetOne.Cell(10, 10).Style.NumberFormat.Format = "#,##0.00";
                                    //Agosto
                                    sheetOne.Cell(9, 11).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato10);
                                    sheetOne.Cell(9, 11).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 11).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato10);
                                    sheetOne.Cell(10, 11).Style.NumberFormat.Format = "#,##0.00";
                                    //Septiembre
                                    sheetOne.Cell(9, 12).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato11);
                                    sheetOne.Cell(9, 12).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 12).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato11);
                                    sheetOne.Cell(10, 12).Style.NumberFormat.Format = "#,##0.00";
                                    //Octubre
                                    sheetOne.Cell(9, 13).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato12);
                                    sheetOne.Cell(9, 13).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 13).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato12);
                                    sheetOne.Cell(10, 13).Style.NumberFormat.Format = "#,##0.00";
                                    //Noviembre
                                    sheetOne.Cell(9, 14).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato13);
                                    sheetOne.Cell(9, 14).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 14).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato13);
                                    sheetOne.Cell(10, 14).Style.NumberFormat.Format = "#,##0.00";
                                    //Diciembre
                                    sheetOne.Cell(9, 15).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato14);
                                    sheetOne.Cell(9, 15).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 15).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato14);
                                    sheetOne.Cell(10, 15).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas1
                                    sheetOne.Cell(9, 17).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato16);
                                    sheetOne.Cell(9, 17).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 17).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato16);
                                    sheetOne.Cell(10, 17).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas2
                                    sheetOne.Cell(9, 18).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato17);
                                    sheetOne.Cell(9, 18).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 18).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato17);
                                    sheetOne.Cell(10, 18).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas3
                                    sheetOne.Cell(9, 19).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato18);
                                    sheetOne.Cell(9, 19).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 19).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato18);
                                    sheetOne.Cell(10, 19).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas4
                                    sheetOne.Cell(9, 20).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato19);
                                    sheetOne.Cell(9, 20).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 20).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato19);
                                    sheetOne.Cell(10, 20).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas5
                                    sheetOne.Cell(9, 21).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato20);
                                    sheetOne.Cell(9, 21).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 21).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato20);
                                    sheetOne.Cell(10, 21).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas6
                                    sheetOne.Cell(9, 22).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato21);
                                    sheetOne.Cell(9, 22).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 22).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato21);
                                    sheetOne.Cell(10, 22).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas7
                                    sheetOne.Cell(9, 23).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato22);
                                    sheetOne.Cell(9, 23).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 23).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato22);
                                    sheetOne.Cell(10, 23).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas8
                                    sheetOne.Cell(9, 24).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato23);
                                    sheetOne.Cell(9, 24).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 24).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato23);
                                    sheetOne.Cell(10, 24).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas9
                                    sheetOne.Cell(9, 25).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato24);
                                    sheetOne.Cell(9, 25).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 25).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato24);
                                    sheetOne.Cell(10, 25).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas10
                                    sheetOne.Cell(9, 26).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato25);
                                    sheetOne.Cell(9, 26).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 26).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato25);
                                    sheetOne.Cell(10, 26).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas11
                                    sheetOne.Cell(9, 27).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato26);
                                    sheetOne.Cell(9, 27).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 27).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato26);
                                    sheetOne.Cell(10, 27).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas12
                                    sheetOne.Cell(9, 28).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato27);
                                    sheetOne.Cell(9, 28).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 28).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato27);
                                    sheetOne.Cell(10, 28).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas13
                                    sheetOne.Cell(9, 29).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato28);
                                    sheetOne.Cell(9, 29).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 29).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato28);
                                    sheetOne.Cell(10, 29).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas14
                                    sheetOne.Cell(9, 30).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato29);
                                    sheetOne.Cell(9, 30).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 30).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato29);
                                    sheetOne.Cell(10, 30).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas15
                                    sheetOne.Cell(9, 31).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato30);
                                    sheetOne.Cell(9, 31).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 31).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato30);
                                    sheetOne.Cell(10, 31).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas16
                                    sheetOne.Cell(9, 32).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato31);
                                    sheetOne.Cell(9, 32).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 32).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato31);
                                    sheetOne.Cell(10, 32).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas17
                                    sheetOne.Cell(9, 33).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato32);
                                    sheetOne.Cell(9, 33).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 33).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato32);
                                    sheetOne.Cell(10, 33).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas18
                                    sheetOne.Cell(9, 34).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato33);
                                    sheetOne.Cell(9, 34).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 34).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato33);
                                    sheetOne.Cell(10, 34).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas19
                                    sheetOne.Cell(9, 35).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato34);
                                    sheetOne.Cell(9, 35).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 35).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato34);
                                    sheetOne.Cell(10, 35).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas20
                                    sheetOne.Cell(9, 36).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato35);
                                    sheetOne.Cell(9, 36).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 36).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato35);
                                    sheetOne.Cell(10, 36).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas21
                                    sheetOne.Cell(9, 37).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato36);
                                    sheetOne.Cell(9, 37).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 37).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato36);
                                    sheetOne.Cell(10, 37).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas22
                                    sheetOne.Cell(9, 38).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato37);
                                    sheetOne.Cell(9, 38).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 38).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato37);
                                    sheetOne.Cell(10, 38).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas23
                                    sheetOne.Cell(9, 39).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato38);
                                    sheetOne.Cell(9, 39).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 39).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato38);
                                    sheetOne.Cell(10, 39).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas24
                                    sheetOne.Cell(9, 40).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato39);
                                    sheetOne.Cell(9, 40).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 40).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato39);
                                    sheetOne.Cell(10, 40).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas25
                                    sheetOne.Cell(9, 41).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato40);
                                    sheetOne.Cell(9, 41).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 41).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato40);
                                    sheetOne.Cell(10, 41).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas26
                                    sheetOne.Cell(9, 42).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato41);
                                    sheetOne.Cell(9, 42).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 42).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato41);
                                    sheetOne.Cell(10, 42).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas27
                                    sheetOne.Cell(9, 43).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato42);
                                    sheetOne.Cell(9, 43).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 43).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato42);
                                    sheetOne.Cell(10, 43).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas28
                                    sheetOne.Cell(9, 44).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato43);
                                    sheetOne.Cell(9, 44).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 44).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato43);
                                    sheetOne.Cell(10, 44).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas29
                                    sheetOne.Cell(9, 45).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato44);
                                    sheetOne.Cell(9, 45).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 45).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato44);
                                    sheetOne.Cell(10, 45).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas30
                                    sheetOne.Cell(9, 46).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato45);
                                    sheetOne.Cell(9, 46).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 46).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato45);
                                    sheetOne.Cell(10, 46).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas31
                                    sheetOne.Cell(9, 47).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato46);
                                    sheetOne.Cell(9, 47).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 47).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato46);
                                    sheetOne.Cell(10, 47).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas32
                                    sheetOne.Cell(9, 48).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato47);
                                    sheetOne.Cell(9, 48).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 48).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato47);
                                    sheetOne.Cell(10, 48).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas33
                                    sheetOne.Cell(9, 49).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato48);
                                    sheetOne.Cell(9, 49).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 49).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato48);
                                    sheetOne.Cell(10, 49).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas34
                                    sheetOne.Cell(9, 50).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato49);
                                    sheetOne.Cell(9, 50).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 50).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato49);
                                    sheetOne.Cell(10, 50).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas35
                                    sheetOne.Cell(9, 51).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato50);
                                    sheetOne.Cell(9, 51).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 51).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato50);
                                    sheetOne.Cell(10, 51).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas36
                                    sheetOne.Cell(9, 52).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato51);
                                    sheetOne.Cell(9, 52).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 52).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato51);
                                    sheetOne.Cell(10, 52).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas37
                                    sheetOne.Cell(9, 53).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato52);
                                    sheetOne.Cell(9, 53).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 53).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato52);
                                    sheetOne.Cell(10, 53).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas38
                                    sheetOne.Cell(9, 54).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato53);
                                    sheetOne.Cell(9, 54).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 54).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato53);
                                    sheetOne.Cell(10, 54).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas39
                                    sheetOne.Cell(9, 55).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato54);
                                    sheetOne.Cell(9, 55).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 55).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato54);
                                    sheetOne.Cell(10, 55).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas40
                                    sheetOne.Cell(9, 56).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato55);
                                    sheetOne.Cell(9, 56).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 56).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato55);
                                    sheetOne.Cell(10, 56).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas41
                                    sheetOne.Cell(9, 57).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato56);
                                    sheetOne.Cell(9, 57).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 57).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato56);
                                    sheetOne.Cell(10, 57).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas42
                                    sheetOne.Cell(9, 58).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato57);
                                    sheetOne.Cell(9, 58).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 58).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato57);
                                    sheetOne.Cell(10, 58).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas43
                                    sheetOne.Cell(9, 59).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato58);
                                    sheetOne.Cell(9, 59).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 59).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato58);
                                    sheetOne.Cell(10, 59).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas44
                                    sheetOne.Cell(9, 60).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato59);
                                    sheetOne.Cell(9, 60).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 60).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato59);
                                    sheetOne.Cell(10, 60).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas45
                                    sheetOne.Cell(9, 61).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato60);
                                    sheetOne.Cell(9, 61).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 61).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato60);
                                    sheetOne.Cell(10, 61).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas46
                                    sheetOne.Cell(9, 62).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato61);
                                    sheetOne.Cell(9, 62).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 62).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato61);
                                    sheetOne.Cell(10, 62).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas47
                                    sheetOne.Cell(9, 63).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato62);
                                    sheetOne.Cell(9, 63).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 63).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato62);
                                    sheetOne.Cell(10, 63).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas48
                                    sheetOne.Cell(9, 64).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato63);
                                    sheetOne.Cell(9, 64).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 64).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato63);
                                    sheetOne.Cell(10, 64).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas49
                                    sheetOne.Cell(9, 65).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato64);
                                    sheetOne.Cell(9, 65).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 65).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato64);
                                    sheetOne.Cell(10, 65).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas50
                                    sheetOne.Cell(9, 66).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato65);
                                    sheetOne.Cell(9, 66).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 66).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato65);
                                    sheetOne.Cell(10, 66).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas51
                                    sheetOne.Cell(9, 67).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato66);
                                    sheetOne.Cell(9, 67).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 67).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato66);
                                    sheetOne.Cell(10, 67).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas52
                                    sheetOne.Cell(9, 68).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato67);
                                    sheetOne.Cell(9, 68).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 68).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato67);
                                    sheetOne.Cell(10, 68).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas53
                                    sheetOne.Cell(9, 69).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato68);
                                    sheetOne.Cell(9, 69).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 69).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato68);
                                    sheetOne.Cell(10, 69).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas54
                                    sheetOne.Cell(9, 70).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato69);
                                    sheetOne.Cell(9, 70).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 70).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato69);
                                    sheetOne.Cell(10, 70).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas55
                                    sheetOne.Cell(9, 71).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato70);
                                    sheetOne.Cell(9, 71).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 71).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato70);
                                    sheetOne.Cell(10, 71).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas56
                                    sheetOne.Cell(9, 72).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato71);
                                    sheetOne.Cell(9, 72).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 72).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato71);
                                    sheetOne.Cell(10, 72).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas57
                                    sheetOne.Cell(9, 73).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato72);
                                    sheetOne.Cell(9, 73).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 73).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato72);
                                    sheetOne.Cell(10, 73).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas58
                                    sheetOne.Cell(9, 74).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato73);
                                    sheetOne.Cell(9, 74).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 74).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato73);
                                    sheetOne.Cell(10, 74).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas59
                                    sheetOne.Cell(9, 75).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato74);
                                    sheetOne.Cell(9, 75).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 75).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato74);
                                    sheetOne.Cell(10, 75).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas60
                                    sheetOne.Cell(9, 76).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato75);
                                    sheetOne.Cell(9, 76).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 76).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato75);
                                    sheetOne.Cell(10, 76).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas61
                                    sheetOne.Cell(9, 77).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato76);
                                    sheetOne.Cell(9, 77).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 77).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato76);
                                    sheetOne.Cell(10, 77).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas62
                                    sheetOne.Cell(9, 78).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato77);
                                    sheetOne.Cell(9, 78).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 78).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato77);
                                    sheetOne.Cell(10, 78).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas63
                                    sheetOne.Cell(9, 79).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato78);
                                    sheetOne.Cell(9, 79).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 79).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato78);
                                    sheetOne.Cell(10, 79).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas64
                                    sheetOne.Cell(9, 80).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato79);
                                    sheetOne.Cell(9, 80).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 80).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato79);
                                    sheetOne.Cell(10, 80).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas65
                                    sheetOne.Cell(9, 81).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato80);
                                    sheetOne.Cell(9, 81).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 81).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato80);
                                    sheetOne.Cell(10, 81).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas66
                                    sheetOne.Cell(9, 82).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato81);
                                    sheetOne.Cell(9, 82).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 82).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato81);
                                    sheetOne.Cell(10, 82).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas67
                                    sheetOne.Cell(9, 83).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato82);
                                    sheetOne.Cell(9, 83).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 83).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato82);
                                    sheetOne.Cell(10, 83).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas68
                                    sheetOne.Cell(9, 84).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato83);
                                    sheetOne.Cell(9, 84).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 84).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato83);
                                    sheetOne.Cell(10, 84).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas69
                                    sheetOne.Cell(9, 85).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato84);
                                    sheetOne.Cell(9, 85).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 85).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato84);
                                    sheetOne.Cell(10, 85).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas70
                                    sheetOne.Cell(9, 86).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato85);
                                    sheetOne.Cell(9, 86).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 86).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato85);
                                    sheetOne.Cell(10, 86).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas71
                                    sheetOne.Cell(9, 87).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato86);
                                    sheetOne.Cell(9, 87).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 87).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato86);
                                    sheetOne.Cell(10, 87).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas72
                                    sheetOne.Cell(9, 88).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato87);
                                    sheetOne.Cell(9, 88).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 88).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato87);
                                    sheetOne.Cell(10, 88).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas73
                                    sheetOne.Cell(9, 89).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato88);
                                    sheetOne.Cell(9, 89).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 89).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato88);
                                    sheetOne.Cell(10, 89).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas74
                                    sheetOne.Cell(9, 90).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato89);
                                    sheetOne.Cell(9, 90).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 90).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato89);
                                    sheetOne.Cell(10, 90).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas75
                                    sheetOne.Cell(9, 91).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato90);
                                    sheetOne.Cell(9, 91).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 91).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato90);
                                    sheetOne.Cell(10, 91).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas76
                                    sheetOne.Cell(9, 92).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato91);
                                    sheetOne.Cell(9, 92).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 92).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato91);
                                    sheetOne.Cell(10, 92).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas77
                                    sheetOne.Cell(9, 93).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato92);
                                    sheetOne.Cell(9, 93).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 93).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato92);
                                    sheetOne.Cell(10, 93).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas78
                                    sheetOne.Cell(9, 94).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato93);
                                    sheetOne.Cell(9, 94).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 94).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato93);
                                    sheetOne.Cell(10, 94).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas79
                                    sheetOne.Cell(9, 95).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato94);
                                    sheetOne.Cell(9, 95).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 95).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato94);
                                    sheetOne.Cell(10, 95).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas80
                                    sheetOne.Cell(9, 96).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato95);
                                    sheetOne.Cell(9, 96).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 96).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato95);
                                    sheetOne.Cell(10, 96).Style.NumberFormat.Format = "#,##0.00";
                                    //[IfDato96]Total Capex
                                }
                                else if (IfDato0Param.StartsWith(prefixAdq))
                                {
                                    sheetOne.Cell(12, 3).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato2);
                                    sheetOne.Cell(12, 3).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 3).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato2);
                                    sheetOne.Cell(13, 3).Style.NumberFormat.Format = "#,##0.00";
                                    //Enero
                                    sheetOne.Cell(12, 4).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato3);
                                    sheetOne.Cell(12, 4).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 4).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato3);
                                    sheetOne.Cell(13, 4).Style.NumberFormat.Format = "#,##0.00";
                                    //Febrero
                                    sheetOne.Cell(12, 5).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato4);
                                    sheetOne.Cell(12, 5).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 5).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato4);
                                    sheetOne.Cell(13, 5).Style.NumberFormat.Format = "#,##0.00";
                                    //Marzo
                                    sheetOne.Cell(12, 6).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato5);
                                    sheetOne.Cell(12, 6).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 6).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato5);
                                    sheetOne.Cell(13, 6).Style.NumberFormat.Format = "#,##0.00";
                                    //Abril
                                    sheetOne.Cell(12, 7).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato6);
                                    sheetOne.Cell(12, 7).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 7).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato6);
                                    sheetOne.Cell(13, 7).Style.NumberFormat.Format = "#,##0.00";
                                    //Mayo
                                    sheetOne.Cell(12, 8).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato7);
                                    sheetOne.Cell(12, 8).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 8).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato7);
                                    sheetOne.Cell(13, 8).Style.NumberFormat.Format = "#,##0.00";
                                    //Junio
                                    sheetOne.Cell(12, 9).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato8);
                                    sheetOne.Cell(12, 9).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 9).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato8);
                                    sheetOne.Cell(13, 9).Style.NumberFormat.Format = "#,##0.00";
                                    //Julio
                                    sheetOne.Cell(12, 10).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato9);
                                    sheetOne.Cell(12, 10).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 10).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato9);
                                    sheetOne.Cell(13, 10).Style.NumberFormat.Format = "#,##0.00";
                                    //Agosto
                                    sheetOne.Cell(12, 11).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato10);
                                    sheetOne.Cell(12, 11).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 11).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato10);
                                    sheetOne.Cell(13, 11).Style.NumberFormat.Format = "#,##0.00";
                                    //Septiembre
                                    sheetOne.Cell(12, 12).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato11);
                                    sheetOne.Cell(12, 12).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 12).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato11);
                                    sheetOne.Cell(13, 12).Style.NumberFormat.Format = "#,##0.00";
                                    //Octubre
                                    sheetOne.Cell(12, 13).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato12);
                                    sheetOne.Cell(12, 13).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 13).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato12);
                                    sheetOne.Cell(13, 13).Style.NumberFormat.Format = "#,##0.00";
                                    //Noviembre
                                    sheetOne.Cell(12, 14).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato13);
                                    sheetOne.Cell(12, 14).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 14).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato13);
                                    sheetOne.Cell(13, 14).Style.NumberFormat.Format = "#,##0.00";
                                    //Diciembre
                                    sheetOne.Cell(12, 15).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato14);
                                    sheetOne.Cell(12, 15).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 15).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato14);
                                    sheetOne.Cell(13, 15).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas1
                                    sheetOne.Cell(12, 17).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato16);
                                    sheetOne.Cell(12, 17).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 17).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato16);
                                    sheetOne.Cell(13, 17).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas2
                                    sheetOne.Cell(12, 18).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato17);
                                    sheetOne.Cell(12, 18).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 18).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato17);
                                    sheetOne.Cell(13, 18).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas3
                                    sheetOne.Cell(12, 19).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato18);
                                    sheetOne.Cell(12, 19).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 19).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato18);
                                    sheetOne.Cell(13, 19).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas4
                                    sheetOne.Cell(12, 20).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato19);
                                    sheetOne.Cell(12, 20).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 20).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato19);
                                    sheetOne.Cell(13, 20).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas5
                                    sheetOne.Cell(12, 21).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato20);
                                    sheetOne.Cell(12, 21).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 21).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato20);
                                    sheetOne.Cell(13, 21).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas6
                                    sheetOne.Cell(12, 22).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato21);
                                    sheetOne.Cell(12, 22).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 22).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato21);
                                    sheetOne.Cell(13, 22).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas7
                                    sheetOne.Cell(12, 23).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato22);
                                    sheetOne.Cell(12, 23).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 23).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato22);
                                    sheetOne.Cell(13, 23).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas8
                                    sheetOne.Cell(12, 24).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato23);
                                    sheetOne.Cell(12, 24).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 24).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato23);
                                    sheetOne.Cell(13, 24).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas9
                                    sheetOne.Cell(12, 25).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato24);
                                    sheetOne.Cell(12, 25).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 25).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato24);
                                    sheetOne.Cell(13, 25).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas10
                                    sheetOne.Cell(12, 26).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato25);
                                    sheetOne.Cell(12, 26).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 26).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato25);
                                    sheetOne.Cell(13, 26).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas11
                                    sheetOne.Cell(12, 27).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato26);
                                    sheetOne.Cell(12, 27).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 27).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato26);
                                    sheetOne.Cell(13, 27).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas12
                                    sheetOne.Cell(12, 28).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato27);
                                    sheetOne.Cell(12, 28).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 28).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato27);
                                    sheetOne.Cell(13, 28).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas13
                                    sheetOne.Cell(12, 29).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato28);
                                    sheetOne.Cell(12, 29).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 29).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato28);
                                    sheetOne.Cell(13, 29).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas14
                                    sheetOne.Cell(12, 30).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato29);
                                    sheetOne.Cell(12, 30).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 30).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato29);
                                    sheetOne.Cell(13, 30).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas15
                                    sheetOne.Cell(12, 31).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato30);
                                    sheetOne.Cell(12, 31).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 31).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato30);
                                    sheetOne.Cell(13, 31).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas16
                                    sheetOne.Cell(12, 32).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato31);
                                    sheetOne.Cell(12, 32).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 32).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato31);
                                    sheetOne.Cell(13, 32).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas17
                                    sheetOne.Cell(12, 33).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato32);
                                    sheetOne.Cell(12, 33).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 33).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato32);
                                    sheetOne.Cell(13, 33).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas18
                                    sheetOne.Cell(12, 34).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato33);
                                    sheetOne.Cell(12, 34).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 34).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato33);
                                    sheetOne.Cell(13, 34).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas19
                                    sheetOne.Cell(12, 35).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato34);
                                    sheetOne.Cell(12, 35).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 35).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato34);
                                    sheetOne.Cell(13, 35).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas20
                                    sheetOne.Cell(12, 36).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato35);
                                    sheetOne.Cell(12, 36).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 36).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato35);
                                    sheetOne.Cell(13, 36).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas21
                                    sheetOne.Cell(12, 37).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato36);
                                    sheetOne.Cell(12, 37).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 37).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato36);
                                    sheetOne.Cell(13, 37).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas22
                                    sheetOne.Cell(12, 38).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato37);
                                    sheetOne.Cell(12, 38).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 38).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato37);
                                    sheetOne.Cell(13, 38).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas23
                                    sheetOne.Cell(12, 39).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato38);
                                    sheetOne.Cell(12, 39).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 39).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato38);
                                    sheetOne.Cell(13, 39).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas24
                                    sheetOne.Cell(12, 40).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato39);
                                    sheetOne.Cell(12, 40).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 40).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato39);
                                    sheetOne.Cell(13, 40).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas25
                                    sheetOne.Cell(12, 41).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato40);
                                    sheetOne.Cell(12, 41).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 41).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato40);
                                    sheetOne.Cell(13, 41).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas26
                                    sheetOne.Cell(12, 42).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato41);
                                    sheetOne.Cell(12, 42).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 42).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato41);
                                    sheetOne.Cell(13, 42).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas27
                                    sheetOne.Cell(12, 43).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato42);
                                    sheetOne.Cell(12, 43).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 43).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato42);
                                    sheetOne.Cell(13, 43).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas28
                                    sheetOne.Cell(12, 44).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato43);
                                    sheetOne.Cell(12, 44).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 44).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato43);
                                    sheetOne.Cell(13, 44).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas29
                                    sheetOne.Cell(12, 45).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato44);
                                    sheetOne.Cell(12, 45).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 45).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato44);
                                    sheetOne.Cell(13, 45).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas30
                                    sheetOne.Cell(12, 46).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato45);
                                    sheetOne.Cell(12, 46).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 46).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato45);
                                    sheetOne.Cell(13, 46).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas31
                                    sheetOne.Cell(12, 47).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato46);
                                    sheetOne.Cell(12, 47).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 47).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato46);
                                    sheetOne.Cell(13, 47).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas32
                                    sheetOne.Cell(12, 48).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato47);
                                    sheetOne.Cell(12, 48).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 48).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato47);
                                    sheetOne.Cell(13, 48).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas33
                                    sheetOne.Cell(12, 49).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato48);
                                    sheetOne.Cell(12, 49).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 49).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato48);
                                    sheetOne.Cell(13, 49).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas34
                                    sheetOne.Cell(12, 50).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato49);
                                    sheetOne.Cell(12, 50).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 50).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato49);
                                    sheetOne.Cell(13, 50).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas35
                                    sheetOne.Cell(12, 51).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato50);
                                    sheetOne.Cell(12, 51).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 51).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato50);
                                    sheetOne.Cell(13, 51).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas36
                                    sheetOne.Cell(12, 52).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato51);
                                    sheetOne.Cell(12, 52).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 52).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato51);
                                    sheetOne.Cell(13, 52).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas37
                                    sheetOne.Cell(12, 53).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato52);
                                    sheetOne.Cell(12, 53).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 53).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato52);
                                    sheetOne.Cell(13, 53).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas38
                                    sheetOne.Cell(12, 54).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato53);
                                    sheetOne.Cell(12, 54).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 54).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato53);
                                    sheetOne.Cell(13, 54).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas39
                                    sheetOne.Cell(12, 55).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato54);
                                    sheetOne.Cell(12, 55).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 55).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato54);
                                    sheetOne.Cell(13, 55).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas40
                                    sheetOne.Cell(12, 56).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato55);
                                    sheetOne.Cell(12, 56).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 56).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato55);
                                    sheetOne.Cell(13, 56).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas41
                                    sheetOne.Cell(12, 57).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato56);
                                    sheetOne.Cell(12, 57).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 57).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato56);
                                    sheetOne.Cell(13, 57).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas42
                                    sheetOne.Cell(12, 58).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato57);
                                    sheetOne.Cell(12, 58).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 58).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato57);
                                    sheetOne.Cell(13, 58).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas43
                                    sheetOne.Cell(12, 59).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato58);
                                    sheetOne.Cell(12, 59).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 59).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato58);
                                    sheetOne.Cell(13, 59).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas44
                                    sheetOne.Cell(12, 60).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato59);
                                    sheetOne.Cell(12, 60).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 60).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato59);
                                    sheetOne.Cell(13, 60).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas45
                                    sheetOne.Cell(12, 61).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato60);
                                    sheetOne.Cell(12, 61).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 61).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato60);
                                    sheetOne.Cell(13, 61).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas46
                                    sheetOne.Cell(12, 62).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato61);
                                    sheetOne.Cell(12, 62).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 62).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato61);
                                    sheetOne.Cell(13, 62).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas47
                                    sheetOne.Cell(12, 63).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato62);
                                    sheetOne.Cell(12, 63).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 63).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato62);
                                    sheetOne.Cell(13, 63).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas48
                                    sheetOne.Cell(12, 64).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato63);
                                    sheetOne.Cell(12, 64).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 64).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato63);
                                    sheetOne.Cell(13, 64).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas49
                                    sheetOne.Cell(12, 65).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato64);
                                    sheetOne.Cell(12, 65).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 65).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato64);
                                    sheetOne.Cell(13, 65).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas50
                                    sheetOne.Cell(12, 66).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato65);
                                    sheetOne.Cell(12, 66).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 66).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato65);
                                    sheetOne.Cell(13, 66).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas51
                                    sheetOne.Cell(12, 67).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato66);
                                    sheetOne.Cell(12, 67).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 67).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato66);
                                    sheetOne.Cell(13, 67).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas52
                                    sheetOne.Cell(12, 68).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato67);
                                    sheetOne.Cell(12, 68).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 68).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato67);
                                    sheetOne.Cell(13, 68).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas53
                                    sheetOne.Cell(12, 69).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato68);
                                    sheetOne.Cell(12, 69).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 69).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato68);
                                    sheetOne.Cell(13, 69).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas54
                                    sheetOne.Cell(12, 70).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato69);
                                    sheetOne.Cell(12, 70).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 70).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato69);
                                    sheetOne.Cell(13, 70).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas55
                                    sheetOne.Cell(12, 71).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato70);
                                    sheetOne.Cell(12, 71).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 71).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato70);
                                    sheetOne.Cell(13, 71).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas56
                                    sheetOne.Cell(12, 72).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato71);
                                    sheetOne.Cell(12, 72).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 72).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato71);
                                    sheetOne.Cell(13, 72).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas57
                                    sheetOne.Cell(12, 73).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato72);
                                    sheetOne.Cell(12, 73).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 73).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato72);
                                    sheetOne.Cell(13, 73).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas58
                                    sheetOne.Cell(12, 74).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato73);
                                    sheetOne.Cell(12, 74).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 74).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato73);
                                    sheetOne.Cell(13, 74).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas59
                                    sheetOne.Cell(12, 75).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato74);
                                    sheetOne.Cell(12, 75).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 75).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato74);
                                    sheetOne.Cell(13, 75).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas60
                                    sheetOne.Cell(12, 76).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato75);
                                    sheetOne.Cell(12, 76).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 76).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato75);
                                    sheetOne.Cell(13, 76).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas61
                                    sheetOne.Cell(12, 77).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato76);
                                    sheetOne.Cell(12, 77).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 77).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato76);
                                    sheetOne.Cell(13, 77).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas62
                                    sheetOne.Cell(12, 78).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato77);
                                    sheetOne.Cell(12, 78).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 78).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato77);
                                    sheetOne.Cell(13, 78).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas63
                                    sheetOne.Cell(12, 79).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato78);
                                    sheetOne.Cell(12, 79).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 79).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato78);
                                    sheetOne.Cell(13, 79).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas64
                                    sheetOne.Cell(12, 80).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato79);
                                    sheetOne.Cell(12, 80).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 80).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato79);
                                    sheetOne.Cell(13, 80).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas65
                                    sheetOne.Cell(12, 81).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato80);
                                    sheetOne.Cell(12, 81).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 81).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato80);
                                    sheetOne.Cell(13, 81).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas66
                                    sheetOne.Cell(12, 82).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato81);
                                    sheetOne.Cell(12, 82).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 82).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato81);
                                    sheetOne.Cell(13, 82).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas67
                                    sheetOne.Cell(12, 83).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato82);
                                    sheetOne.Cell(12, 83).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 83).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato82);
                                    sheetOne.Cell(13, 83).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas68
                                    sheetOne.Cell(12, 84).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato83);
                                    sheetOne.Cell(12, 84).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 84).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato83);
                                    sheetOne.Cell(13, 84).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas69
                                    sheetOne.Cell(12, 85).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato84);
                                    sheetOne.Cell(12, 85).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 85).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato84);
                                    sheetOne.Cell(13, 85).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas70
                                    sheetOne.Cell(12, 86).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato85);
                                    sheetOne.Cell(12, 86).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 86).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato85);
                                    sheetOne.Cell(13, 86).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas71
                                    sheetOne.Cell(12, 87).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato86);
                                    sheetOne.Cell(12, 87).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 87).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato86);
                                    sheetOne.Cell(13, 87).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas72
                                    sheetOne.Cell(12, 88).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato87);
                                    sheetOne.Cell(12, 88).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 88).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato87);
                                    sheetOne.Cell(13, 88).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas73
                                    sheetOne.Cell(12, 89).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato88);
                                    sheetOne.Cell(12, 89).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 89).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato88);
                                    sheetOne.Cell(13, 89).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas74
                                    sheetOne.Cell(12, 90).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato89);
                                    sheetOne.Cell(12, 90).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 90).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato89);
                                    sheetOne.Cell(13, 90).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas75
                                    sheetOne.Cell(12, 91).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato90);
                                    sheetOne.Cell(12, 91).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 91).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato90);
                                    sheetOne.Cell(13, 91).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas76
                                    sheetOne.Cell(12, 92).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato91);
                                    sheetOne.Cell(12, 92).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 92).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato91);
                                    sheetOne.Cell(13, 92).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas77
                                    sheetOne.Cell(12, 93).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato92);
                                    sheetOne.Cell(12, 93).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 93).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato92);
                                    sheetOne.Cell(13, 93).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas78
                                    sheetOne.Cell(12, 94).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato93);
                                    sheetOne.Cell(12, 94).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 94).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato93);
                                    sheetOne.Cell(13, 94).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas79
                                    sheetOne.Cell(12, 95).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato94);
                                    sheetOne.Cell(12, 95).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 95).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato94);
                                    sheetOne.Cell(13, 95).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas80
                                    sheetOne.Cell(12, 96).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato95);
                                    sheetOne.Cell(12, 96).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 96).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato95);
                                    sheetOne.Cell(13, 96).Style.NumberFormat.Format = "#,##0.00";
                                    //[IfDato96]Total Capex
                                }
                                else if (IfDato0Param.StartsWith(prefixCons))
                                {
                                    sheetOne.Cell(15, 3).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato2);
                                    sheetOne.Cell(15, 3).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 3).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato2);
                                    sheetOne.Cell(16, 3).Style.NumberFormat.Format = "#,##0.00";
                                    //Enero
                                    sheetOne.Cell(15, 4).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato3);
                                    sheetOne.Cell(15, 4).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 4).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato3);
                                    sheetOne.Cell(16, 4).Style.NumberFormat.Format = "#,##0.00";
                                    //Febrero
                                    sheetOne.Cell(15, 5).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato4);
                                    sheetOne.Cell(15, 5).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 5).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato4);
                                    sheetOne.Cell(16, 5).Style.NumberFormat.Format = "#,##0.00";
                                    //Marzo
                                    sheetOne.Cell(15, 6).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato5);
                                    sheetOne.Cell(15, 6).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 6).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato5);
                                    sheetOne.Cell(16, 6).Style.NumberFormat.Format = "#,##0.00";
                                    //Abril
                                    sheetOne.Cell(15, 7).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato6);
                                    sheetOne.Cell(15, 7).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 7).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato6);
                                    sheetOne.Cell(16, 7).Style.NumberFormat.Format = "#,##0.00";
                                    //Mayo
                                    sheetOne.Cell(15, 8).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato7);
                                    sheetOne.Cell(15, 8).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 8).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato7);
                                    sheetOne.Cell(16, 8).Style.NumberFormat.Format = "#,##0.00";
                                    //Junio
                                    sheetOne.Cell(15, 9).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato8);
                                    sheetOne.Cell(15, 9).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 9).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato8);
                                    sheetOne.Cell(16, 9).Style.NumberFormat.Format = "#,##0.00";
                                    //Julio
                                    sheetOne.Cell(15, 10).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato9);
                                    sheetOne.Cell(15, 10).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 10).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato9);
                                    sheetOne.Cell(16, 10).Style.NumberFormat.Format = "#,##0.00";
                                    //Agosto
                                    sheetOne.Cell(15, 11).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato10);
                                    sheetOne.Cell(15, 11).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 11).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato10);
                                    sheetOne.Cell(16, 11).Style.NumberFormat.Format = "#,##0.00";
                                    //Septiembre
                                    sheetOne.Cell(15, 12).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato11);
                                    sheetOne.Cell(15, 12).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 12).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato11);
                                    sheetOne.Cell(16, 12).Style.NumberFormat.Format = "#,##0.00";
                                    //Octubre
                                    sheetOne.Cell(15, 13).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato12);
                                    sheetOne.Cell(15, 13).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 13).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato12);
                                    sheetOne.Cell(16, 13).Style.NumberFormat.Format = "#,##0.00";
                                    //Noviembre
                                    sheetOne.Cell(15, 14).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato13);
                                    sheetOne.Cell(15, 14).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 14).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato13);
                                    sheetOne.Cell(16, 14).Style.NumberFormat.Format = "#,##0.00";
                                    //Diciembre
                                    sheetOne.Cell(15, 15).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato14);
                                    sheetOne.Cell(15, 15).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 15).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato14);
                                    sheetOne.Cell(16, 15).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas1
                                    sheetOne.Cell(15, 17).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato16);
                                    sheetOne.Cell(15, 17).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 17).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato16);
                                    sheetOne.Cell(16, 17).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas2
                                    sheetOne.Cell(15, 18).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato17);
                                    sheetOne.Cell(15, 18).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 18).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato17);
                                    sheetOne.Cell(16, 18).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas3
                                    sheetOne.Cell(15, 19).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato18);
                                    sheetOne.Cell(15, 19).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 19).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato18);
                                    sheetOne.Cell(16, 19).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas4
                                    sheetOne.Cell(15, 20).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato19);
                                    sheetOne.Cell(15, 20).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 20).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato19);
                                    sheetOne.Cell(16, 20).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas5
                                    sheetOne.Cell(15, 21).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato20);
                                    sheetOne.Cell(15, 21).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 21).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato20);
                                    sheetOne.Cell(16, 21).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas6
                                    sheetOne.Cell(15, 22).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato21);
                                    sheetOne.Cell(15, 22).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 22).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato21);
                                    sheetOne.Cell(16, 22).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas7
                                    sheetOne.Cell(15, 23).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato22);
                                    sheetOne.Cell(15, 23).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 23).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato22);
                                    sheetOne.Cell(16, 23).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas8
                                    sheetOne.Cell(15, 24).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato23);
                                    sheetOne.Cell(15, 24).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 24).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato23);
                                    sheetOne.Cell(16, 24).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas9
                                    sheetOne.Cell(15, 25).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato24);
                                    sheetOne.Cell(15, 25).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 25).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato24);
                                    sheetOne.Cell(16, 25).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas10
                                    sheetOne.Cell(15, 26).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato25);
                                    sheetOne.Cell(15, 26).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 26).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato25);
                                    sheetOne.Cell(16, 26).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas11
                                    sheetOne.Cell(15, 27).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato26);
                                    sheetOne.Cell(15, 27).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 27).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato26);
                                    sheetOne.Cell(16, 27).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas12
                                    sheetOne.Cell(15, 28).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato27);
                                    sheetOne.Cell(15, 28).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 28).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato27);
                                    sheetOne.Cell(16, 28).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas13
                                    sheetOne.Cell(15, 29).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato28);
                                    sheetOne.Cell(15, 29).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 29).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato28);
                                    sheetOne.Cell(16, 29).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas14
                                    sheetOne.Cell(15, 30).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato29);
                                    sheetOne.Cell(15, 30).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 30).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato29);
                                    sheetOne.Cell(16, 30).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas15
                                    sheetOne.Cell(15, 31).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato30);
                                    sheetOne.Cell(15, 31).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 31).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato30);
                                    sheetOne.Cell(16, 31).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas16
                                    sheetOne.Cell(15, 32).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato31);
                                    sheetOne.Cell(15, 32).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 32).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato31);
                                    sheetOne.Cell(16, 32).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas17
                                    sheetOne.Cell(15, 33).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato32);
                                    sheetOne.Cell(15, 33).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 33).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato32);
                                    sheetOne.Cell(16, 33).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas18
                                    sheetOne.Cell(15, 34).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato33);
                                    sheetOne.Cell(15, 34).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 34).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato33);
                                    sheetOne.Cell(16, 34).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas19
                                    sheetOne.Cell(15, 35).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato34);
                                    sheetOne.Cell(15, 35).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 35).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato34);
                                    sheetOne.Cell(16, 35).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas20
                                    sheetOne.Cell(15, 36).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato35);
                                    sheetOne.Cell(15, 36).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 36).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato35);
                                    sheetOne.Cell(16, 36).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas21
                                    sheetOne.Cell(15, 37).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato36);
                                    sheetOne.Cell(15, 37).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 37).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato36);
                                    sheetOne.Cell(16, 37).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas22
                                    sheetOne.Cell(15, 38).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato37);
                                    sheetOne.Cell(15, 38).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 38).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato37);
                                    sheetOne.Cell(16, 38).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas23
                                    sheetOne.Cell(15, 39).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato38);
                                    sheetOne.Cell(15, 39).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 39).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato38);
                                    sheetOne.Cell(16, 39).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas24
                                    sheetOne.Cell(15, 40).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato39);
                                    sheetOne.Cell(15, 40).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 40).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato39);
                                    sheetOne.Cell(16, 40).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas25
                                    sheetOne.Cell(15, 41).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato40);
                                    sheetOne.Cell(15, 41).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 41).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato40);
                                    sheetOne.Cell(16, 41).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas26
                                    sheetOne.Cell(15, 42).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato41);
                                    sheetOne.Cell(15, 42).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 42).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato41);
                                    sheetOne.Cell(16, 42).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas27
                                    sheetOne.Cell(15, 43).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato42);
                                    sheetOne.Cell(15, 43).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 43).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato42);
                                    sheetOne.Cell(16, 43).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas28
                                    sheetOne.Cell(15, 44).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato43);
                                    sheetOne.Cell(15, 44).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 44).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato43);
                                    sheetOne.Cell(16, 44).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas29
                                    sheetOne.Cell(15, 45).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato44);
                                    sheetOne.Cell(15, 45).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 45).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato44);
                                    sheetOne.Cell(16, 45).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas30
                                    sheetOne.Cell(15, 46).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato45);
                                    sheetOne.Cell(15, 46).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 46).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato45);
                                    sheetOne.Cell(16, 46).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas31
                                    sheetOne.Cell(15, 47).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato46);
                                    sheetOne.Cell(15, 47).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 47).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato46);
                                    sheetOne.Cell(16, 47).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas32
                                    sheetOne.Cell(15, 48).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato47);
                                    sheetOne.Cell(15, 48).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 48).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato47);
                                    sheetOne.Cell(16, 48).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas33
                                    sheetOne.Cell(15, 49).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato48);
                                    sheetOne.Cell(15, 49).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 49).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato48);
                                    sheetOne.Cell(16, 49).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas34
                                    sheetOne.Cell(15, 50).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato49);
                                    sheetOne.Cell(15, 50).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 50).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato49);
                                    sheetOne.Cell(16, 50).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas35
                                    sheetOne.Cell(15, 51).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato50);
                                    sheetOne.Cell(15, 51).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 51).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato50);
                                    sheetOne.Cell(16, 51).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas36
                                    sheetOne.Cell(15, 52).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato51);
                                    sheetOne.Cell(15, 52).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 52).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato51);
                                    sheetOne.Cell(16, 52).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas37
                                    sheetOne.Cell(15, 53).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato52);
                                    sheetOne.Cell(15, 53).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 53).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato52);
                                    sheetOne.Cell(16, 53).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas38
                                    sheetOne.Cell(15, 54).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato53);
                                    sheetOne.Cell(15, 54).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 54).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato53);
                                    sheetOne.Cell(16, 54).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas39
                                    sheetOne.Cell(15, 55).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato54);
                                    sheetOne.Cell(15, 55).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 55).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato54);
                                    sheetOne.Cell(16, 55).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas40
                                    sheetOne.Cell(15, 56).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato55);
                                    sheetOne.Cell(15, 56).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 56).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato55);
                                    sheetOne.Cell(16, 56).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas41
                                    sheetOne.Cell(15, 57).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato56);
                                    sheetOne.Cell(15, 57).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 57).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato56);
                                    sheetOne.Cell(16, 57).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas42
                                    sheetOne.Cell(15, 58).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato57);
                                    sheetOne.Cell(15, 58).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 58).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato57);
                                    sheetOne.Cell(16, 58).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas43
                                    sheetOne.Cell(15, 59).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato58);
                                    sheetOne.Cell(15, 59).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 59).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato58);
                                    sheetOne.Cell(16, 59).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas44
                                    sheetOne.Cell(15, 60).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato59);
                                    sheetOne.Cell(15, 60).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 60).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato59);
                                    sheetOne.Cell(16, 60).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas45
                                    sheetOne.Cell(15, 61).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato60);
                                    sheetOne.Cell(15, 61).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 61).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato60);
                                    sheetOne.Cell(16, 61).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas46
                                    sheetOne.Cell(15, 62).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato61);
                                    sheetOne.Cell(15, 62).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 62).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato61);
                                    sheetOne.Cell(16, 62).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas47
                                    sheetOne.Cell(15, 63).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato62);
                                    sheetOne.Cell(15, 63).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 63).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato62);
                                    sheetOne.Cell(16, 63).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas48
                                    sheetOne.Cell(15, 64).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato63);
                                    sheetOne.Cell(15, 64).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 64).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato63);
                                    sheetOne.Cell(16, 64).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas49
                                    sheetOne.Cell(15, 65).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato64);
                                    sheetOne.Cell(15, 65).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 65).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato64);
                                    sheetOne.Cell(16, 65).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas50
                                    sheetOne.Cell(15, 66).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato65);
                                    sheetOne.Cell(15, 66).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 66).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato65);
                                    sheetOne.Cell(16, 66).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas51
                                    sheetOne.Cell(15, 67).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato66);
                                    sheetOne.Cell(15, 67).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 67).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato66);
                                    sheetOne.Cell(16, 67).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas52
                                    sheetOne.Cell(15, 68).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato67);
                                    sheetOne.Cell(15, 68).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 68).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato67);
                                    sheetOne.Cell(16, 68).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas53
                                    sheetOne.Cell(15, 69).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato68);
                                    sheetOne.Cell(15, 69).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 69).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato68);
                                    sheetOne.Cell(16, 69).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas54
                                    sheetOne.Cell(15, 70).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato69);
                                    sheetOne.Cell(15, 70).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 70).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato69);
                                    sheetOne.Cell(16, 70).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas55
                                    sheetOne.Cell(15, 71).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato70);
                                    sheetOne.Cell(15, 71).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 71).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato70);
                                    sheetOne.Cell(16, 71).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas56
                                    sheetOne.Cell(15, 72).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato71);
                                    sheetOne.Cell(15, 72).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 72).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato71);
                                    sheetOne.Cell(16, 72).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas57
                                    sheetOne.Cell(15, 73).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato72);
                                    sheetOne.Cell(15, 73).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 73).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato72);
                                    sheetOne.Cell(16, 73).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas58
                                    sheetOne.Cell(15, 74).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato73);
                                    sheetOne.Cell(15, 74).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 74).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato73);
                                    sheetOne.Cell(16, 74).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas59
                                    sheetOne.Cell(15, 75).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato74);
                                    sheetOne.Cell(15, 75).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 75).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato74);
                                    sheetOne.Cell(16, 75).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas60
                                    sheetOne.Cell(15, 76).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato75);
                                    sheetOne.Cell(15, 76).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 76).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato75);
                                    sheetOne.Cell(16, 76).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas61
                                    sheetOne.Cell(15, 77).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato76);
                                    sheetOne.Cell(15, 77).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 77).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato76);
                                    sheetOne.Cell(16, 77).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas62
                                    sheetOne.Cell(15, 78).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato77);
                                    sheetOne.Cell(15, 78).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 78).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato77);
                                    sheetOne.Cell(16, 78).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas63
                                    sheetOne.Cell(15, 79).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato78);
                                    sheetOne.Cell(15, 79).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 79).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato78);
                                    sheetOne.Cell(16, 79).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas64
                                    sheetOne.Cell(15, 80).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato79);
                                    sheetOne.Cell(15, 80).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 80).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato79);
                                    sheetOne.Cell(16, 80).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas65
                                    sheetOne.Cell(15, 81).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato80);
                                    sheetOne.Cell(15, 81).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 81).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato80);
                                    sheetOne.Cell(16, 81).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas66
                                    sheetOne.Cell(15, 82).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato81);
                                    sheetOne.Cell(15, 82).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 82).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato81);
                                    sheetOne.Cell(16, 82).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas67
                                    sheetOne.Cell(15, 83).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato82);
                                    sheetOne.Cell(15, 83).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 83).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato82);
                                    sheetOne.Cell(16, 83).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas68
                                    sheetOne.Cell(15, 84).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato83);
                                    sheetOne.Cell(15, 84).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 84).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato83);
                                    sheetOne.Cell(16, 84).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas69
                                    sheetOne.Cell(15, 85).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato84);
                                    sheetOne.Cell(15, 85).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 85).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato84);
                                    sheetOne.Cell(16, 85).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas70
                                    sheetOne.Cell(15, 86).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato85);
                                    sheetOne.Cell(15, 86).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 86).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato85);
                                    sheetOne.Cell(16, 86).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas71
                                    sheetOne.Cell(15, 87).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato86);
                                    sheetOne.Cell(15, 87).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 87).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato86);
                                    sheetOne.Cell(16, 87).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas72
                                    sheetOne.Cell(15, 88).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato87);
                                    sheetOne.Cell(15, 88).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 88).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato87);
                                    sheetOne.Cell(16, 88).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas73
                                    sheetOne.Cell(15, 89).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato88);
                                    sheetOne.Cell(15, 89).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 89).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato88);
                                    sheetOne.Cell(16, 89).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas74
                                    sheetOne.Cell(15, 90).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato89);
                                    sheetOne.Cell(15, 90).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 90).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato89);
                                    sheetOne.Cell(16, 90).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas75
                                    sheetOne.Cell(15, 91).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato90);
                                    sheetOne.Cell(15, 91).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 91).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato90);
                                    sheetOne.Cell(16, 91).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas76
                                    sheetOne.Cell(15, 92).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato91);
                                    sheetOne.Cell(15, 92).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 92).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato91);
                                    sheetOne.Cell(16, 92).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas77
                                    sheetOne.Cell(15, 93).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato92);
                                    sheetOne.Cell(15, 93).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 93).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato92);
                                    sheetOne.Cell(16, 93).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas78
                                    sheetOne.Cell(15, 94).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato93);
                                    sheetOne.Cell(15, 94).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 94).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato93);
                                    sheetOne.Cell(16, 94).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas79
                                    sheetOne.Cell(15, 95).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato94);
                                    sheetOne.Cell(15, 95).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 95).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato94);
                                    sheetOne.Cell(16, 95).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas80
                                    sheetOne.Cell(15, 96).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato95);
                                    sheetOne.Cell(15, 96).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 96).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato95);
                                    sheetOne.Cell(16, 96).Style.NumberFormat.Format = "#,##0.00";
                                    //[IfDato96]Total Capex
                                }
                                else if (IfDato0Param.StartsWith(prefixAdm))
                                {
                                    sheetOne.Cell(18, 3).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato2);
                                    sheetOne.Cell(18, 3).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 3).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato2);
                                    sheetOne.Cell(19, 3).Style.NumberFormat.Format = "#,##0.00";
                                    //Enero
                                    sheetOne.Cell(18, 4).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato3);
                                    sheetOne.Cell(18, 4).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 4).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato3);
                                    sheetOne.Cell(19, 4).Style.NumberFormat.Format = "#,##0.00";
                                    //Febrero
                                    sheetOne.Cell(18, 5).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato4);
                                    sheetOne.Cell(18, 5).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 5).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato4);
                                    sheetOne.Cell(19, 5).Style.NumberFormat.Format = "#,##0.00";
                                    //Marzo
                                    sheetOne.Cell(18, 6).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato5);
                                    sheetOne.Cell(18, 6).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 6).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato5);
                                    sheetOne.Cell(19, 6).Style.NumberFormat.Format = "#,##0.00";
                                    //Abril
                                    sheetOne.Cell(18, 7).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato6);
                                    sheetOne.Cell(18, 7).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 7).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato6);
                                    sheetOne.Cell(19, 7).Style.NumberFormat.Format = "#,##0.00";
                                    //Mayo
                                    sheetOne.Cell(18, 8).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato7);
                                    sheetOne.Cell(18, 8).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 8).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato7);
                                    sheetOne.Cell(19, 8).Style.NumberFormat.Format = "#,##0.00";
                                    //Junio
                                    sheetOne.Cell(18, 9).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato8);
                                    sheetOne.Cell(18, 9).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 9).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato8);
                                    sheetOne.Cell(19, 9).Style.NumberFormat.Format = "#,##0.00";
                                    //Julio
                                    sheetOne.Cell(18, 10).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato9);
                                    sheetOne.Cell(18, 10).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 10).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato9);
                                    sheetOne.Cell(19, 10).Style.NumberFormat.Format = "#,##0.00";
                                    //Agosto
                                    sheetOne.Cell(18, 11).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato10);
                                    sheetOne.Cell(18, 11).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 11).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato10);
                                    sheetOne.Cell(19, 11).Style.NumberFormat.Format = "#,##0.00";
                                    //Septiembre
                                    sheetOne.Cell(18, 12).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato11);
                                    sheetOne.Cell(18, 12).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 12).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato11);
                                    sheetOne.Cell(19, 12).Style.NumberFormat.Format = "#,##0.00";
                                    //Octubre
                                    sheetOne.Cell(18, 13).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato12);
                                    sheetOne.Cell(18, 13).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 13).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato12);
                                    sheetOne.Cell(19, 13).Style.NumberFormat.Format = "#,##0.00";
                                    //Noviembre
                                    sheetOne.Cell(18, 14).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato13);
                                    sheetOne.Cell(18, 14).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 14).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato13);
                                    sheetOne.Cell(19, 14).Style.NumberFormat.Format = "#,##0.00";
                                    //Diciembre
                                    sheetOne.Cell(18, 15).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato14);
                                    sheetOne.Cell(18, 15).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 15).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato14);
                                    sheetOne.Cell(19, 15).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas1
                                    sheetOne.Cell(18, 17).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato16);
                                    sheetOne.Cell(18, 17).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 17).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato16);
                                    sheetOne.Cell(19, 17).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas2
                                    sheetOne.Cell(18, 18).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato17);
                                    sheetOne.Cell(18, 18).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 18).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato17);
                                    sheetOne.Cell(19, 18).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas3
                                    sheetOne.Cell(18, 19).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato18);
                                    sheetOne.Cell(18, 19).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 19).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato18);
                                    sheetOne.Cell(19, 19).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas4
                                    sheetOne.Cell(18, 20).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato19);
                                    sheetOne.Cell(18, 20).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 20).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato19);
                                    sheetOne.Cell(19, 20).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas5
                                    sheetOne.Cell(18, 21).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato20);
                                    sheetOne.Cell(18, 21).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 21).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato20);
                                    sheetOne.Cell(19, 21).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas6
                                    sheetOne.Cell(18, 22).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato21);
                                    sheetOne.Cell(18, 22).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 22).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato21);
                                    sheetOne.Cell(19, 22).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas7
                                    sheetOne.Cell(18, 23).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato22);
                                    sheetOne.Cell(18, 23).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 23).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato22);
                                    sheetOne.Cell(19, 23).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas8
                                    sheetOne.Cell(18, 24).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato23);
                                    sheetOne.Cell(18, 24).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 24).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato23);
                                    sheetOne.Cell(19, 24).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas9
                                    sheetOne.Cell(18, 25).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato24);
                                    sheetOne.Cell(18, 25).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 25).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato24);
                                    sheetOne.Cell(19, 25).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas10
                                    sheetOne.Cell(18, 26).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato25);
                                    sheetOne.Cell(18, 26).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 26).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato25);
                                    sheetOne.Cell(19, 26).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas11
                                    sheetOne.Cell(18, 27).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato26);
                                    sheetOne.Cell(18, 27).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 27).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato26);
                                    sheetOne.Cell(19, 27).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas12
                                    sheetOne.Cell(18, 28).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato27);
                                    sheetOne.Cell(18, 28).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 28).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato27);
                                    sheetOne.Cell(19, 28).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas13
                                    sheetOne.Cell(18, 29).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato28);
                                    sheetOne.Cell(18, 29).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 29).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato28);
                                    sheetOne.Cell(19, 29).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas14
                                    sheetOne.Cell(18, 30).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato29);
                                    sheetOne.Cell(18, 30).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 30).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato29);
                                    sheetOne.Cell(19, 30).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas15
                                    sheetOne.Cell(18, 31).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato30);
                                    sheetOne.Cell(18, 31).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 31).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato30);
                                    sheetOne.Cell(19, 31).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas16
                                    sheetOne.Cell(18, 32).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato31);
                                    sheetOne.Cell(18, 32).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 32).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato31);
                                    sheetOne.Cell(19, 32).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas17
                                    sheetOne.Cell(18, 33).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato32);
                                    sheetOne.Cell(18, 33).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 33).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato32);
                                    sheetOne.Cell(19, 33).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas18
                                    sheetOne.Cell(18, 34).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato33);
                                    sheetOne.Cell(18, 34).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 34).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato33);
                                    sheetOne.Cell(19, 34).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas19
                                    sheetOne.Cell(18, 35).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato34);
                                    sheetOne.Cell(18, 35).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 35).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato34);
                                    sheetOne.Cell(19, 35).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas20
                                    sheetOne.Cell(18, 36).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato35);
                                    sheetOne.Cell(18, 36).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 36).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato35);
                                    sheetOne.Cell(19, 36).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas21
                                    sheetOne.Cell(18, 37).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato36);
                                    sheetOne.Cell(18, 37).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 37).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato36);
                                    sheetOne.Cell(19, 37).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas22
                                    sheetOne.Cell(18, 38).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato37);
                                    sheetOne.Cell(18, 38).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 38).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato37);
                                    sheetOne.Cell(19, 38).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas23
                                    sheetOne.Cell(18, 39).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato38);
                                    sheetOne.Cell(18, 39).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 39).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato38);
                                    sheetOne.Cell(19, 39).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas24
                                    sheetOne.Cell(18, 40).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato39);
                                    sheetOne.Cell(18, 40).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 40).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato39);
                                    sheetOne.Cell(19, 40).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas25
                                    sheetOne.Cell(18, 41).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato40);
                                    sheetOne.Cell(18, 41).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 41).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato40);
                                    sheetOne.Cell(19, 41).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas26
                                    sheetOne.Cell(18, 42).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato41);
                                    sheetOne.Cell(18, 42).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 42).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato41);
                                    sheetOne.Cell(19, 42).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas27
                                    sheetOne.Cell(18, 43).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato42);
                                    sheetOne.Cell(18, 43).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 43).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato42);
                                    sheetOne.Cell(19, 43).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas28
                                    sheetOne.Cell(18, 44).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato43);
                                    sheetOne.Cell(18, 44).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 44).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato43);
                                    sheetOne.Cell(19, 44).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas29
                                    sheetOne.Cell(18, 45).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato44);
                                    sheetOne.Cell(18, 45).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 45).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato44);
                                    sheetOne.Cell(19, 45).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas30
                                    sheetOne.Cell(18, 46).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato45);
                                    sheetOne.Cell(18, 46).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 46).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato45);
                                    sheetOne.Cell(19, 46).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas31
                                    sheetOne.Cell(18, 47).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato46);
                                    sheetOne.Cell(18, 47).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 47).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato46);
                                    sheetOne.Cell(19, 47).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas32
                                    sheetOne.Cell(18, 48).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato47);
                                    sheetOne.Cell(18, 48).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 48).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato47);
                                    sheetOne.Cell(19, 48).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas33
                                    sheetOne.Cell(18, 49).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato48);
                                    sheetOne.Cell(18, 49).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 49).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato48);
                                    sheetOne.Cell(19, 49).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas34
                                    sheetOne.Cell(18, 50).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato49);
                                    sheetOne.Cell(18, 50).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 50).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato49);
                                    sheetOne.Cell(19, 50).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas35
                                    sheetOne.Cell(18, 51).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato50);
                                    sheetOne.Cell(18, 51).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 51).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato50);
                                    sheetOne.Cell(19, 51).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas36
                                    sheetOne.Cell(18, 52).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato51);
                                    sheetOne.Cell(18, 52).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 52).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato51);
                                    sheetOne.Cell(19, 52).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas37
                                    sheetOne.Cell(18, 53).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato52);
                                    sheetOne.Cell(18, 53).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 53).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato52);
                                    sheetOne.Cell(19, 53).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas38
                                    sheetOne.Cell(18, 54).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato53);
                                    sheetOne.Cell(18, 54).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 54).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato53);
                                    sheetOne.Cell(19, 54).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas39
                                    sheetOne.Cell(18, 55).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato54);
                                    sheetOne.Cell(18, 55).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 55).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato54);
                                    sheetOne.Cell(19, 55).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas40
                                    sheetOne.Cell(18, 56).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato55);
                                    sheetOne.Cell(18, 56).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 56).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato55);
                                    sheetOne.Cell(19, 56).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas41
                                    sheetOne.Cell(18, 57).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato56);
                                    sheetOne.Cell(18, 57).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 57).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato56);
                                    sheetOne.Cell(19, 57).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas42
                                    sheetOne.Cell(18, 58).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato57);
                                    sheetOne.Cell(18, 58).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 58).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato57);
                                    sheetOne.Cell(19, 58).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas43
                                    sheetOne.Cell(18, 59).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato58);
                                    sheetOne.Cell(18, 59).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 59).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato58);
                                    sheetOne.Cell(19, 59).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas44
                                    sheetOne.Cell(18, 60).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato59);
                                    sheetOne.Cell(18, 60).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 60).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato59);
                                    sheetOne.Cell(19, 60).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas45
                                    sheetOne.Cell(18, 61).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato60);
                                    sheetOne.Cell(18, 61).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 61).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato60);
                                    sheetOne.Cell(19, 61).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas46
                                    sheetOne.Cell(18, 62).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato61);
                                    sheetOne.Cell(18, 62).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 62).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato61);
                                    sheetOne.Cell(19, 62).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas47
                                    sheetOne.Cell(18, 63).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato62);
                                    sheetOne.Cell(18, 63).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 63).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato62);
                                    sheetOne.Cell(19, 63).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas48
                                    sheetOne.Cell(18, 64).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato63);
                                    sheetOne.Cell(18, 64).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 64).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato63);
                                    sheetOne.Cell(19, 64).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas49
                                    sheetOne.Cell(18, 65).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato64);
                                    sheetOne.Cell(18, 65).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 65).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato64);
                                    sheetOne.Cell(19, 65).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas50
                                    sheetOne.Cell(18, 66).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato65);
                                    sheetOne.Cell(18, 66).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 66).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato65);
                                    sheetOne.Cell(19, 66).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas51
                                    sheetOne.Cell(18, 67).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato66);
                                    sheetOne.Cell(18, 67).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 67).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato66);
                                    sheetOne.Cell(19, 67).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas52
                                    sheetOne.Cell(18, 68).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato67);
                                    sheetOne.Cell(18, 68).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 68).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato67);
                                    sheetOne.Cell(19, 68).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas53
                                    sheetOne.Cell(18, 69).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato68);
                                    sheetOne.Cell(18, 69).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 69).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato68);
                                    sheetOne.Cell(19, 69).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas54
                                    sheetOne.Cell(18, 70).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato69);
                                    sheetOne.Cell(18, 70).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 70).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato69);
                                    sheetOne.Cell(19, 70).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas55
                                    sheetOne.Cell(18, 71).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato70);
                                    sheetOne.Cell(18, 71).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 71).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato70);
                                    sheetOne.Cell(19, 71).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas56
                                    sheetOne.Cell(18, 72).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato71);
                                    sheetOne.Cell(18, 72).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 72).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato71);
                                    sheetOne.Cell(19, 72).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas57
                                    sheetOne.Cell(18, 73).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato72);
                                    sheetOne.Cell(18, 73).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 73).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato72);
                                    sheetOne.Cell(19, 73).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas58
                                    sheetOne.Cell(18, 74).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato73);
                                    sheetOne.Cell(18, 74).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 74).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato73);
                                    sheetOne.Cell(19, 74).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas59
                                    sheetOne.Cell(18, 75).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato74);
                                    sheetOne.Cell(18, 75).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 75).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato74);
                                    sheetOne.Cell(19, 75).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas60
                                    sheetOne.Cell(18, 76).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato75);
                                    sheetOne.Cell(18, 76).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 76).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato75);
                                    sheetOne.Cell(19, 76).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas61
                                    sheetOne.Cell(18, 77).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato76);
                                    sheetOne.Cell(18, 77).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 77).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato76);
                                    sheetOne.Cell(19, 77).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas62
                                    sheetOne.Cell(18, 78).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato77);
                                    sheetOne.Cell(18, 78).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 78).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato77);
                                    sheetOne.Cell(19, 78).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas63
                                    sheetOne.Cell(18, 79).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato78);
                                    sheetOne.Cell(18, 79).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 79).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato78);
                                    sheetOne.Cell(19, 79).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas64
                                    sheetOne.Cell(18, 80).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato79);
                                    sheetOne.Cell(18, 80).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 80).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato79);
                                    sheetOne.Cell(19, 80).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas65
                                    sheetOne.Cell(18, 81).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato80);
                                    sheetOne.Cell(18, 81).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 81).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato80);
                                    sheetOne.Cell(19, 81).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas66
                                    sheetOne.Cell(18, 82).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato81);
                                    sheetOne.Cell(18, 82).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 82).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato81);
                                    sheetOne.Cell(19, 82).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas67
                                    sheetOne.Cell(18, 83).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato82);
                                    sheetOne.Cell(18, 83).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 83).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato82);
                                    sheetOne.Cell(19, 83).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas68
                                    sheetOne.Cell(18, 84).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato83);
                                    sheetOne.Cell(18, 84).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 84).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato83);
                                    sheetOne.Cell(19, 84).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas69
                                    sheetOne.Cell(18, 85).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato84);
                                    sheetOne.Cell(18, 85).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 85).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato84);
                                    sheetOne.Cell(19, 85).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas70
                                    sheetOne.Cell(18, 86).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato85);
                                    sheetOne.Cell(18, 86).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 86).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato85);
                                    sheetOne.Cell(19, 86).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas71
                                    sheetOne.Cell(18, 87).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato86);
                                    sheetOne.Cell(18, 87).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 87).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato86);
                                    sheetOne.Cell(19, 87).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas72
                                    sheetOne.Cell(18, 88).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato87);
                                    sheetOne.Cell(18, 88).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 88).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato87);
                                    sheetOne.Cell(19, 88).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas73
                                    sheetOne.Cell(18, 89).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato88);
                                    sheetOne.Cell(18, 89).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 89).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato88);
                                    sheetOne.Cell(19, 89).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas74
                                    sheetOne.Cell(18, 90).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato89);
                                    sheetOne.Cell(18, 90).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 90).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato89);
                                    sheetOne.Cell(19, 90).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas75
                                    sheetOne.Cell(18, 91).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato90);
                                    sheetOne.Cell(18, 91).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 91).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato90);
                                    sheetOne.Cell(19, 91).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas76
                                    sheetOne.Cell(18, 92).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato91);
                                    sheetOne.Cell(18, 92).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 92).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato91);
                                    sheetOne.Cell(19, 92).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas77
                                    sheetOne.Cell(18, 93).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato92);
                                    sheetOne.Cell(18, 93).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 93).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato92);
                                    sheetOne.Cell(19, 93).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas78
                                    sheetOne.Cell(18, 94).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato93);
                                    sheetOne.Cell(18, 94).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 94).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato93);
                                    sheetOne.Cell(19, 94).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas79
                                    sheetOne.Cell(18, 95).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato94);
                                    sheetOne.Cell(18, 95).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 95).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato94);
                                    sheetOne.Cell(19, 95).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas80
                                    sheetOne.Cell(18, 96).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato95);
                                    sheetOne.Cell(18, 96).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 96).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato95);
                                    sheetOne.Cell(19, 96).Style.NumberFormat.Format = "#,##0.00";
                                    //[IfDato96]Total Capex
                                }
                                else if (IfDato0Param.StartsWith(prefixCont))
                                {
                                    sheetOne.Cell(21, 3).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato2);
                                    sheetOne.Cell(21, 3).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 3).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato2);
                                    sheetOne.Cell(22, 3).Style.NumberFormat.Format = "#,##0.00";
                                    //Enero
                                    sheetOne.Cell(21, 4).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato3);
                                    sheetOne.Cell(21, 4).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 4).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato3);
                                    sheetOne.Cell(22, 4).Style.NumberFormat.Format = "#,##0.00";
                                    //Febrero
                                    sheetOne.Cell(21, 5).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato4);
                                    sheetOne.Cell(21, 5).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 5).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato4);
                                    sheetOne.Cell(22, 5).Style.NumberFormat.Format = "#,##0.00";
                                    //Marzo
                                    sheetOne.Cell(21, 6).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato5);
                                    sheetOne.Cell(21, 6).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 6).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato5);
                                    sheetOne.Cell(22, 6).Style.NumberFormat.Format = "#,##0.00";
                                    //Abril
                                    sheetOne.Cell(21, 7).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato6);
                                    sheetOne.Cell(21, 7).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 7).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato6);
                                    sheetOne.Cell(22, 7).Style.NumberFormat.Format = "#,##0.00";
                                    //Mayo
                                    sheetOne.Cell(21, 8).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato7);
                                    sheetOne.Cell(21, 8).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 8).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato7);
                                    sheetOne.Cell(22, 8).Style.NumberFormat.Format = "#,##0.00";
                                    //Junio
                                    sheetOne.Cell(21, 9).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato8);
                                    sheetOne.Cell(21, 9).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 9).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato8);
                                    sheetOne.Cell(22, 9).Style.NumberFormat.Format = "#,##0.00";
                                    //Julio
                                    sheetOne.Cell(21, 10).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato9);
                                    sheetOne.Cell(21, 10).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 10).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato9);
                                    sheetOne.Cell(22, 10).Style.NumberFormat.Format = "#,##0.00";
                                    //Agosto
                                    sheetOne.Cell(21, 11).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato10);
                                    sheetOne.Cell(21, 11).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 11).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato10);
                                    sheetOne.Cell(22, 11).Style.NumberFormat.Format = "#,##0.00";
                                    //Septiembre
                                    sheetOne.Cell(21, 12).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato11);
                                    sheetOne.Cell(21, 12).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 12).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato11);
                                    sheetOne.Cell(22, 12).Style.NumberFormat.Format = "#,##0.00";
                                    //Octubre
                                    sheetOne.Cell(21, 13).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato12);
                                    sheetOne.Cell(21, 13).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 13).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato12);
                                    sheetOne.Cell(22, 13).Style.NumberFormat.Format = "#,##0.00";
                                    //Noviembre
                                    sheetOne.Cell(21, 14).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato13);
                                    sheetOne.Cell(21, 14).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 14).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato13);
                                    sheetOne.Cell(22, 14).Style.NumberFormat.Format = "#,##0.00";
                                    //Diciembre
                                    sheetOne.Cell(21, 15).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato14);
                                    sheetOne.Cell(21, 15).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 15).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato14);
                                    sheetOne.Cell(22, 15).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas1
                                    sheetOne.Cell(21, 17).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato16);
                                    sheetOne.Cell(21, 17).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 17).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato16);
                                    sheetOne.Cell(22, 17).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas2
                                    sheetOne.Cell(21, 18).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato17);
                                    sheetOne.Cell(21, 18).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 18).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato17);
                                    sheetOne.Cell(22, 18).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas3
                                    sheetOne.Cell(21, 19).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato18);
                                    sheetOne.Cell(21, 19).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 19).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato18);
                                    sheetOne.Cell(22, 19).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas4
                                    sheetOne.Cell(21, 20).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato19);
                                    sheetOne.Cell(21, 20).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 20).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato19);
                                    sheetOne.Cell(22, 20).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas5
                                    sheetOne.Cell(21, 21).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato20);
                                    sheetOne.Cell(21, 21).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 21).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato20);
                                    sheetOne.Cell(22, 21).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas6
                                    sheetOne.Cell(21, 22).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato21);
                                    sheetOne.Cell(21, 22).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 22).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato21);
                                    sheetOne.Cell(22, 22).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas7
                                    sheetOne.Cell(21, 23).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato22);
                                    sheetOne.Cell(21, 23).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 23).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato22);
                                    sheetOne.Cell(22, 23).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas8
                                    sheetOne.Cell(21, 24).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato23);
                                    sheetOne.Cell(21, 24).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 24).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato23);
                                    sheetOne.Cell(22, 24).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas9
                                    sheetOne.Cell(21, 25).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato24);
                                    sheetOne.Cell(21, 25).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 25).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato24);
                                    sheetOne.Cell(22, 25).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas10
                                    sheetOne.Cell(21, 26).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato25);
                                    sheetOne.Cell(21, 26).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 26).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato25);
                                    sheetOne.Cell(22, 26).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas11
                                    sheetOne.Cell(21, 27).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato26);
                                    sheetOne.Cell(21, 27).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 27).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato26);
                                    sheetOne.Cell(22, 27).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas12
                                    sheetOne.Cell(21, 28).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato27);
                                    sheetOne.Cell(21, 28).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 28).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato27);
                                    sheetOne.Cell(22, 28).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas13
                                    sheetOne.Cell(21, 29).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato28);
                                    sheetOne.Cell(21, 29).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 29).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato28);
                                    sheetOne.Cell(22, 29).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas14
                                    sheetOne.Cell(21, 30).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato29);
                                    sheetOne.Cell(21, 30).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 30).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato29);
                                    sheetOne.Cell(22, 30).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas15
                                    sheetOne.Cell(21, 31).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato30);
                                    sheetOne.Cell(21, 31).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 31).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato30);
                                    sheetOne.Cell(22, 31).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas16
                                    sheetOne.Cell(21, 32).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato31);
                                    sheetOne.Cell(21, 32).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 32).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato31);
                                    sheetOne.Cell(22, 32).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas17
                                    sheetOne.Cell(21, 33).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato32);
                                    sheetOne.Cell(21, 33).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 33).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato32);
                                    sheetOne.Cell(22, 33).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas18
                                    sheetOne.Cell(21, 34).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato33);
                                    sheetOne.Cell(21, 34).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 34).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato33);
                                    sheetOne.Cell(22, 34).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas19
                                    sheetOne.Cell(21, 35).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato34);
                                    sheetOne.Cell(21, 35).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 35).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato34);
                                    sheetOne.Cell(22, 35).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas20
                                    sheetOne.Cell(21, 36).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato35);
                                    sheetOne.Cell(21, 36).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 36).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato35);
                                    sheetOne.Cell(22, 36).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas21
                                    sheetOne.Cell(21, 37).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato36);
                                    sheetOne.Cell(21, 37).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 37).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato36);
                                    sheetOne.Cell(22, 37).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas22
                                    sheetOne.Cell(21, 38).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato37);
                                    sheetOne.Cell(21, 38).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 38).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato37);
                                    sheetOne.Cell(22, 38).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas23
                                    sheetOne.Cell(21, 39).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato38);
                                    sheetOne.Cell(21, 39).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 39).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato38);
                                    sheetOne.Cell(22, 39).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas24
                                    sheetOne.Cell(21, 40).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato39);
                                    sheetOne.Cell(21, 40).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 40).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato39);
                                    sheetOne.Cell(22, 40).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas25
                                    sheetOne.Cell(21, 41).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato40);
                                    sheetOne.Cell(21, 41).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 41).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato40);
                                    sheetOne.Cell(22, 41).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas26
                                    sheetOne.Cell(21, 42).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato41);
                                    sheetOne.Cell(21, 42).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 42).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato41);
                                    sheetOne.Cell(22, 42).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas27
                                    sheetOne.Cell(21, 43).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato42);
                                    sheetOne.Cell(21, 43).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 43).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato42);
                                    sheetOne.Cell(22, 43).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas28
                                    sheetOne.Cell(21, 44).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato43);
                                    sheetOne.Cell(21, 44).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 44).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato43);
                                    sheetOne.Cell(22, 44).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas29
                                    sheetOne.Cell(21, 45).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato44);
                                    sheetOne.Cell(21, 45).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 45).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato44);
                                    sheetOne.Cell(22, 45).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas30
                                    sheetOne.Cell(21, 46).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato45);
                                    sheetOne.Cell(21, 46).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 46).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato45);
                                    sheetOne.Cell(22, 46).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas31
                                    sheetOne.Cell(21, 47).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato46);
                                    sheetOne.Cell(21, 47).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 47).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato46);
                                    sheetOne.Cell(22, 47).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas32
                                    sheetOne.Cell(21, 48).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato47);
                                    sheetOne.Cell(21, 48).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 48).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato47);
                                    sheetOne.Cell(22, 48).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas33
                                    sheetOne.Cell(21, 49).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato48);
                                    sheetOne.Cell(21, 49).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 49).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato48);
                                    sheetOne.Cell(22, 49).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas34
                                    sheetOne.Cell(21, 50).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato49);
                                    sheetOne.Cell(21, 50).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 50).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato49);
                                    sheetOne.Cell(22, 50).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas35
                                    sheetOne.Cell(21, 51).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato50);
                                    sheetOne.Cell(21, 51).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 51).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato50);
                                    sheetOne.Cell(22, 51).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas36
                                    sheetOne.Cell(21, 52).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato51);
                                    sheetOne.Cell(21, 52).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 52).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato51);
                                    sheetOne.Cell(22, 52).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas37
                                    sheetOne.Cell(21, 53).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato52);
                                    sheetOne.Cell(21, 53).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 53).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato52);
                                    sheetOne.Cell(22, 53).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas38
                                    sheetOne.Cell(21, 54).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato53);
                                    sheetOne.Cell(21, 54).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 54).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato53);
                                    sheetOne.Cell(22, 54).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas39
                                    sheetOne.Cell(21, 55).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato54);
                                    sheetOne.Cell(21, 55).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 55).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato54);
                                    sheetOne.Cell(22, 55).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas40
                                    sheetOne.Cell(21, 56).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato55);
                                    sheetOne.Cell(21, 56).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 56).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato55);
                                    sheetOne.Cell(22, 56).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas41
                                    sheetOne.Cell(21, 57).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato56);
                                    sheetOne.Cell(21, 57).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 57).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato56);
                                    sheetOne.Cell(22, 57).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas42
                                    sheetOne.Cell(21, 58).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato57);
                                    sheetOne.Cell(21, 58).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 58).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato57);
                                    sheetOne.Cell(22, 58).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas43
                                    sheetOne.Cell(21, 59).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato58);
                                    sheetOne.Cell(21, 59).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 59).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato58);
                                    sheetOne.Cell(22, 59).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas44
                                    sheetOne.Cell(21, 60).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato59);
                                    sheetOne.Cell(21, 60).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 60).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato59);
                                    sheetOne.Cell(22, 60).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas45
                                    sheetOne.Cell(21, 61).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato60);
                                    sheetOne.Cell(21, 61).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 61).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato60);
                                    sheetOne.Cell(22, 61).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas46
                                    sheetOne.Cell(21, 62).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato61);
                                    sheetOne.Cell(21, 62).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 62).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato61);
                                    sheetOne.Cell(22, 62).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas47
                                    sheetOne.Cell(21, 63).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato62);
                                    sheetOne.Cell(21, 63).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 63).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato62);
                                    sheetOne.Cell(22, 63).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas48
                                    sheetOne.Cell(21, 64).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato63);
                                    sheetOne.Cell(21, 64).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 64).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato63);
                                    sheetOne.Cell(22, 64).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas49
                                    sheetOne.Cell(21, 65).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato64);
                                    sheetOne.Cell(21, 65).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 65).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato64);
                                    sheetOne.Cell(22, 65).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas50
                                    sheetOne.Cell(21, 66).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato65);
                                    sheetOne.Cell(21, 66).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 66).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato65);
                                    sheetOne.Cell(22, 66).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas51
                                    sheetOne.Cell(21, 67).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato66);
                                    sheetOne.Cell(21, 67).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 67).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato66);
                                    sheetOne.Cell(22, 67).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas52
                                    sheetOne.Cell(21, 68).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato67);
                                    sheetOne.Cell(21, 68).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 68).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato67);
                                    sheetOne.Cell(22, 68).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas53
                                    sheetOne.Cell(21, 69).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato68);
                                    sheetOne.Cell(21, 69).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 69).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato68);
                                    sheetOne.Cell(22, 69).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas54
                                    sheetOne.Cell(21, 70).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato69);
                                    sheetOne.Cell(21, 70).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 70).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato69);
                                    sheetOne.Cell(22, 70).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas55
                                    sheetOne.Cell(21, 71).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato70);
                                    sheetOne.Cell(21, 71).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 71).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato70);
                                    sheetOne.Cell(22, 71).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas56
                                    sheetOne.Cell(21, 72).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato71);
                                    sheetOne.Cell(21, 72).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 72).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato71);
                                    sheetOne.Cell(22, 72).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas57
                                    sheetOne.Cell(21, 73).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato72);
                                    sheetOne.Cell(21, 73).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 73).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato72);
                                    sheetOne.Cell(22, 73).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas58
                                    sheetOne.Cell(21, 74).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato73);
                                    sheetOne.Cell(21, 74).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 74).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato73);
                                    sheetOne.Cell(22, 74).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas59
                                    sheetOne.Cell(21, 75).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato74);
                                    sheetOne.Cell(21, 75).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 75).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato74);
                                    sheetOne.Cell(22, 75).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas60
                                    sheetOne.Cell(21, 76).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato75);
                                    sheetOne.Cell(21, 76).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 76).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato75);
                                    sheetOne.Cell(22, 76).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas61
                                    sheetOne.Cell(21, 77).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato76);
                                    sheetOne.Cell(21, 77).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 77).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato76);
                                    sheetOne.Cell(22, 77).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas62
                                    sheetOne.Cell(21, 78).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato77);
                                    sheetOne.Cell(21, 78).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 78).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato77);
                                    sheetOne.Cell(22, 78).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas63
                                    sheetOne.Cell(21, 79).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato78);
                                    sheetOne.Cell(21, 79).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 79).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato78);
                                    sheetOne.Cell(22, 79).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas64
                                    sheetOne.Cell(21, 80).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato79);
                                    sheetOne.Cell(21, 80).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 80).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato79);
                                    sheetOne.Cell(22, 80).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas65
                                    sheetOne.Cell(21, 81).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato80);
                                    sheetOne.Cell(21, 81).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 81).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato80);
                                    sheetOne.Cell(22, 81).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas66
                                    sheetOne.Cell(21, 82).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato81);
                                    sheetOne.Cell(21, 82).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 82).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato81);
                                    sheetOne.Cell(22, 82).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas67
                                    sheetOne.Cell(21, 83).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato82);
                                    sheetOne.Cell(21, 83).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 83).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato82);
                                    sheetOne.Cell(22, 83).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas68
                                    sheetOne.Cell(21, 84).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato83);
                                    sheetOne.Cell(21, 84).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 84).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato83);
                                    sheetOne.Cell(22, 84).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas69
                                    sheetOne.Cell(21, 85).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato84);
                                    sheetOne.Cell(21, 85).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 85).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato84);
                                    sheetOne.Cell(22, 85).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas70
                                    sheetOne.Cell(21, 86).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato85);
                                    sheetOne.Cell(21, 86).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 86).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato85);
                                    sheetOne.Cell(22, 86).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas71
                                    sheetOne.Cell(21, 87).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato86);
                                    sheetOne.Cell(21, 87).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 87).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato86);
                                    sheetOne.Cell(22, 87).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas72
                                    sheetOne.Cell(21, 88).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato87);
                                    sheetOne.Cell(21, 88).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 88).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato87);
                                    sheetOne.Cell(22, 88).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas73
                                    sheetOne.Cell(21, 89).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato88);
                                    sheetOne.Cell(21, 89).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 89).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato88);
                                    sheetOne.Cell(22, 89).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas74
                                    sheetOne.Cell(21, 90).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato89);
                                    sheetOne.Cell(21, 90).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 90).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato89);
                                    sheetOne.Cell(22, 90).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas75
                                    sheetOne.Cell(21, 91).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato90);
                                    sheetOne.Cell(21, 91).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 91).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato90);
                                    sheetOne.Cell(22, 91).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas76
                                    sheetOne.Cell(21, 92).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato91);
                                    sheetOne.Cell(21, 92).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 92).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato91);
                                    sheetOne.Cell(22, 92).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas77
                                    sheetOne.Cell(21, 93).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato92);
                                    sheetOne.Cell(21, 93).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 93).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato92);
                                    sheetOne.Cell(22, 93).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas78
                                    sheetOne.Cell(21, 94).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato93);
                                    sheetOne.Cell(21, 94).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 94).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato93);
                                    sheetOne.Cell(22, 94).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas79
                                    sheetOne.Cell(21, 95).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato94);
                                    sheetOne.Cell(21, 95).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 95).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato94);
                                    sheetOne.Cell(22, 95).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMas80
                                    sheetOne.Cell(21, 96).Value = stringToDecimalFromDB(filaMonedaNacional.IfDato95);
                                    sheetOne.Cell(21, 96).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 96).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDato95);
                                    sheetOne.Cell(22, 96).Style.NumberFormat.Format = "#,##0.00";
                                    //[IfDato96]Total Capex
                                }
                            }
                        }
                        else
                        {
                            IList<Presupuesto.FinancieroDetallePresupuesto> informacionFinancieroDetalleParametroVNOrigen = SqlMapper.Query<Presupuesto.FinancieroDetallePresupuesto>(objConnection, "CAPEX_SEL_IMPORTACION_FINANCIERO_DETALLE_PARAMETROVN", new { TipoIniciativa = tipoIniciativaSeleccionada, IfToken = IfTokenParam }, commandType: CommandType.StoredProcedure).ToList();
                            if (informacionFinancieroDetalleParametroVNOrigen != null && informacionFinancieroDetalleParametroVNOrigen.Count == 2)
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
                                if (IfDato0Param.StartsWith(prefixIng))
                                {
                                    sheetOne.Cell(9, 3).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato2);
                                    sheetOne.Cell(9, 3).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 3).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato2);
                                    sheetOne.Cell(10, 3).Style.NumberFormat.Format = "#,##0.00";
                                    //Enero
                                    sheetOne.Cell(9, 4).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato3);
                                    sheetOne.Cell(9, 4).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 4).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato3);
                                    sheetOne.Cell(10, 4).Style.NumberFormat.Format = "#,##0.00";
                                    //Febrero
                                    sheetOne.Cell(9, 5).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato4);
                                    sheetOne.Cell(9, 5).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 5).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato4);
                                    sheetOne.Cell(10, 5).Style.NumberFormat.Format = "#,##0.00";
                                    //Marzo
                                    sheetOne.Cell(9, 6).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato5);
                                    sheetOne.Cell(9, 6).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 6).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato5);
                                    sheetOne.Cell(10, 6).Style.NumberFormat.Format = "#,##0.00";
                                    //Abril
                                    sheetOne.Cell(9, 7).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato6);
                                    sheetOne.Cell(9, 7).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 7).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato6);
                                    sheetOne.Cell(10, 7).Style.NumberFormat.Format = "#,##0.00";
                                    //Mayo
                                    sheetOne.Cell(9, 8).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato7);
                                    sheetOne.Cell(9, 8).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 8).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato7);
                                    sheetOne.Cell(10, 8).Style.NumberFormat.Format = "#,##0.00";
                                    //Junio
                                    sheetOne.Cell(9, 9).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato8);
                                    sheetOne.Cell(9, 9).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 9).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato8);
                                    sheetOne.Cell(10, 9).Style.NumberFormat.Format = "#,##0.00";
                                    //Julio
                                    sheetOne.Cell(9, 10).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato9);
                                    sheetOne.Cell(9, 10).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 10).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato9);
                                    sheetOne.Cell(10, 10).Style.NumberFormat.Format = "#,##0.00";
                                    //Agosto
                                    sheetOne.Cell(9, 11).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato10);
                                    sheetOne.Cell(9, 11).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 11).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato10);
                                    sheetOne.Cell(10, 11).Style.NumberFormat.Format = "#,##0.00";
                                    //Septiembre
                                    sheetOne.Cell(9, 12).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato11);
                                    sheetOne.Cell(9, 12).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 12).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato11);
                                    sheetOne.Cell(10, 12).Style.NumberFormat.Format = "#,##0.00";
                                    //Octubre
                                    sheetOne.Cell(9, 13).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato12);
                                    sheetOne.Cell(9, 13).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 13).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato12);
                                    sheetOne.Cell(10, 13).Style.NumberFormat.Format = "#,##0.00";
                                    //Noviembre
                                    sheetOne.Cell(9, 14).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato13);
                                    sheetOne.Cell(9, 14).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 14).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato13);
                                    sheetOne.Cell(10, 14).Style.NumberFormat.Format = "#,##0.00";
                                    //Diciembre
                                    sheetOne.Cell(9, 15).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato14);
                                    sheetOne.Cell(9, 15).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 15).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato14);
                                    sheetOne.Cell(10, 15).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMasUno
                                    sheetOne.Cell(9, 17).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato16);
                                    sheetOne.Cell(9, 17).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 17).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato16);
                                    sheetOne.Cell(10, 17).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMasDos
                                    sheetOne.Cell(9, 18).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato17);
                                    sheetOne.Cell(9, 18).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 18).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato17);
                                    sheetOne.Cell(10, 18).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMasTres
                                    sheetOne.Cell(9, 19).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato18);
                                    sheetOne.Cell(9, 19).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(10, 19).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato18);
                                    sheetOne.Cell(10, 19).Style.NumberFormat.Format = "#,##0.00";
                                }
                                else if (IfDato0Param.StartsWith(prefixAdq))
                                {
                                    sheetOne.Cell(12, 3).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato2);
                                    sheetOne.Cell(12, 3).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 3).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato2);
                                    sheetOne.Cell(13, 3).Style.NumberFormat.Format = "#,##0.00";
                                    //Enero
                                    sheetOne.Cell(12, 4).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato3);
                                    sheetOne.Cell(12, 4).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 4).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato3);
                                    sheetOne.Cell(13, 4).Style.NumberFormat.Format = "#,##0.00";
                                    //Febrero
                                    sheetOne.Cell(12, 5).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato4);
                                    sheetOne.Cell(12, 5).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 5).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato4);
                                    sheetOne.Cell(13, 5).Style.NumberFormat.Format = "#,##0.00";
                                    //Marzo
                                    sheetOne.Cell(12, 6).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato5);
                                    sheetOne.Cell(12, 6).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 6).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato5);
                                    sheetOne.Cell(13, 6).Style.NumberFormat.Format = "#,##0.00";
                                    //Abril
                                    sheetOne.Cell(12, 7).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato6);
                                    sheetOne.Cell(12, 7).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 7).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato6);
                                    sheetOne.Cell(13, 7).Style.NumberFormat.Format = "#,##0.00";
                                    //Mayo
                                    sheetOne.Cell(12, 8).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato7);
                                    sheetOne.Cell(12, 8).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 8).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato7);
                                    sheetOne.Cell(13, 8).Style.NumberFormat.Format = "#,##0.00";
                                    //Junio
                                    sheetOne.Cell(12, 9).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato8);
                                    sheetOne.Cell(12, 9).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 9).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato8);
                                    sheetOne.Cell(13, 9).Style.NumberFormat.Format = "#,##0.00";
                                    //Julio
                                    sheetOne.Cell(12, 10).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato9);
                                    sheetOne.Cell(12, 10).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 10).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato9);
                                    sheetOne.Cell(13, 10).Style.NumberFormat.Format = "#,##0.00";
                                    //Agosto
                                    sheetOne.Cell(12, 11).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato10);
                                    sheetOne.Cell(12, 11).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 11).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato10);
                                    sheetOne.Cell(13, 11).Style.NumberFormat.Format = "#,##0.00";
                                    //Septiembre
                                    sheetOne.Cell(12, 12).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato11);
                                    sheetOne.Cell(12, 12).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 12).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato11);
                                    sheetOne.Cell(13, 12).Style.NumberFormat.Format = "#,##0.00";
                                    //Octubre
                                    sheetOne.Cell(12, 13).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato12);
                                    sheetOne.Cell(12, 13).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 13).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato12);
                                    sheetOne.Cell(13, 13).Style.NumberFormat.Format = "#,##0.00";
                                    //Noviembre
                                    sheetOne.Cell(12, 14).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato13);
                                    sheetOne.Cell(12, 14).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 14).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato13);
                                    sheetOne.Cell(13, 14).Style.NumberFormat.Format = "#,##0.00";
                                    //Diciembre
                                    sheetOne.Cell(12, 15).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato14);
                                    sheetOne.Cell(12, 15).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 15).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato14);
                                    sheetOne.Cell(13, 15).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMasUno
                                    sheetOne.Cell(12, 17).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato16);
                                    sheetOne.Cell(12, 17).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 17).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato16);
                                    sheetOne.Cell(13, 17).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMasDos
                                    sheetOne.Cell(12, 18).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato17);
                                    sheetOne.Cell(12, 18).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 18).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato17);
                                    sheetOne.Cell(13, 18).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMasTres
                                    sheetOne.Cell(12, 19).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato18);
                                    sheetOne.Cell(12, 19).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(13, 19).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato18);
                                    sheetOne.Cell(13, 19).Style.NumberFormat.Format = "#,##0.00";
                                }
                                else if (IfDato0Param.StartsWith(prefixCons))
                                {
                                    sheetOne.Cell(15, 3).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato2);
                                    sheetOne.Cell(15, 3).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 3).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato2);
                                    sheetOne.Cell(16, 3).Style.NumberFormat.Format = "#,##0.00";
                                    //Enero
                                    sheetOne.Cell(15, 4).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato3);
                                    sheetOne.Cell(15, 4).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 4).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato3);
                                    sheetOne.Cell(16, 4).Style.NumberFormat.Format = "#,##0.00";
                                    //Febrero
                                    sheetOne.Cell(15, 5).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato4);
                                    sheetOne.Cell(15, 5).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 5).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato4);
                                    sheetOne.Cell(16, 5).Style.NumberFormat.Format = "#,##0.00";
                                    //Marzo
                                    sheetOne.Cell(15, 6).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato5);
                                    sheetOne.Cell(15, 6).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 6).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato5);
                                    sheetOne.Cell(16, 6).Style.NumberFormat.Format = "#,##0.00";
                                    //Abril
                                    sheetOne.Cell(15, 7).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato6);
                                    sheetOne.Cell(15, 7).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 7).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato6);
                                    sheetOne.Cell(16, 7).Style.NumberFormat.Format = "#,##0.00";
                                    //Mayo
                                    sheetOne.Cell(15, 8).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato7);
                                    sheetOne.Cell(15, 8).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 8).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato7);
                                    sheetOne.Cell(16, 8).Style.NumberFormat.Format = "#,##0.00";
                                    //Junio
                                    sheetOne.Cell(15, 9).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato8);
                                    sheetOne.Cell(15, 9).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 9).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato8);
                                    sheetOne.Cell(16, 9).Style.NumberFormat.Format = "#,##0.00";
                                    //Julio
                                    sheetOne.Cell(15, 10).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato9);
                                    sheetOne.Cell(15, 10).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 10).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato9);
                                    sheetOne.Cell(16, 10).Style.NumberFormat.Format = "#,##0.00";
                                    //Agosto
                                    sheetOne.Cell(15, 11).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato10);
                                    sheetOne.Cell(15, 11).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 11).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato10);
                                    sheetOne.Cell(16, 11).Style.NumberFormat.Format = "#,##0.00";
                                    //Septiembre
                                    sheetOne.Cell(15, 12).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato11);
                                    sheetOne.Cell(15, 12).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 12).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato11);
                                    sheetOne.Cell(16, 12).Style.NumberFormat.Format = "#,##0.00";
                                    //Octubre
                                    sheetOne.Cell(15, 13).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato12);
                                    sheetOne.Cell(15, 13).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 13).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato12);
                                    sheetOne.Cell(16, 13).Style.NumberFormat.Format = "#,##0.00";
                                    //Noviembre
                                    sheetOne.Cell(15, 14).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato13);
                                    sheetOne.Cell(15, 14).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 14).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato13);
                                    sheetOne.Cell(16, 14).Style.NumberFormat.Format = "#,##0.00";
                                    //Diciembre
                                    sheetOne.Cell(15, 15).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato14);
                                    sheetOne.Cell(15, 15).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 15).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato14);
                                    sheetOne.Cell(16, 15).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMasUno
                                    sheetOne.Cell(15, 17).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato16);
                                    sheetOne.Cell(15, 17).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 17).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato16);
                                    sheetOne.Cell(16, 17).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMasDos
                                    sheetOne.Cell(15, 18).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato17);
                                    sheetOne.Cell(15, 18).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 18).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato17);
                                    sheetOne.Cell(16, 18).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMasTres
                                    sheetOne.Cell(15, 19).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato18);
                                    sheetOne.Cell(15, 19).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(16, 19).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato18);
                                    sheetOne.Cell(16, 19).Style.NumberFormat.Format = "#,##0.00";
                                }
                                else if (IfDato0Param.StartsWith(prefixAdm))
                                {
                                    sheetOne.Cell(18, 3).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato2);
                                    sheetOne.Cell(18, 3).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 3).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato2);
                                    sheetOne.Cell(19, 3).Style.NumberFormat.Format = "#,##0.00";
                                    //Enero
                                    sheetOne.Cell(18, 4).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato3);
                                    sheetOne.Cell(18, 4).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 4).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato3);
                                    sheetOne.Cell(19, 4).Style.NumberFormat.Format = "#,##0.00";
                                    //Febrero
                                    sheetOne.Cell(18, 5).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato4);
                                    sheetOne.Cell(18, 5).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 5).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato4);
                                    sheetOne.Cell(19, 5).Style.NumberFormat.Format = "#,##0.00";
                                    //Marzo
                                    sheetOne.Cell(18, 6).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato5);
                                    sheetOne.Cell(18, 6).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 6).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato5);
                                    sheetOne.Cell(19, 6).Style.NumberFormat.Format = "#,##0.00";
                                    //Abril
                                    sheetOne.Cell(18, 7).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato6);
                                    sheetOne.Cell(18, 7).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 7).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato6);
                                    sheetOne.Cell(19, 7).Style.NumberFormat.Format = "#,##0.00";
                                    //Mayo
                                    sheetOne.Cell(18, 8).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato7);
                                    sheetOne.Cell(18, 8).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 8).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato7);
                                    sheetOne.Cell(19, 8).Style.NumberFormat.Format = "#,##0.00";
                                    //Junio
                                    sheetOne.Cell(18, 9).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato8);
                                    sheetOne.Cell(18, 9).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 9).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato8);
                                    sheetOne.Cell(19, 9).Style.NumberFormat.Format = "#,##0.00";
                                    //Julio
                                    sheetOne.Cell(18, 10).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato9);
                                    sheetOne.Cell(18, 10).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 10).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato9);
                                    sheetOne.Cell(19, 10).Style.NumberFormat.Format = "#,##0.00";
                                    //Agosto
                                    sheetOne.Cell(18, 11).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato10);
                                    sheetOne.Cell(18, 11).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 11).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato10);
                                    sheetOne.Cell(19, 11).Style.NumberFormat.Format = "#,##0.00";
                                    //Septiembre
                                    sheetOne.Cell(18, 12).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato11);
                                    sheetOne.Cell(18, 12).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 12).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato11);
                                    sheetOne.Cell(19, 12).Style.NumberFormat.Format = "#,##0.00";
                                    //Octubre
                                    sheetOne.Cell(18, 13).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato12);
                                    sheetOne.Cell(18, 13).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 13).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato12);
                                    sheetOne.Cell(19, 13).Style.NumberFormat.Format = "#,##0.00";
                                    //Noviembre
                                    sheetOne.Cell(18, 14).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato13);
                                    sheetOne.Cell(18, 14).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 14).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato13);
                                    sheetOne.Cell(19, 14).Style.NumberFormat.Format = "#,##0.00";
                                    //Diciembre
                                    sheetOne.Cell(18, 15).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato14);
                                    sheetOne.Cell(18, 15).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 15).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato14);
                                    sheetOne.Cell(19, 15).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMasUno
                                    sheetOne.Cell(18, 17).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato16);
                                    sheetOne.Cell(18, 17).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 17).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato16);
                                    sheetOne.Cell(19, 17).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMasDos
                                    sheetOne.Cell(18, 18).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato17);
                                    sheetOne.Cell(18, 18).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 18).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato17);
                                    sheetOne.Cell(19, 18).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMasTres
                                    sheetOne.Cell(18, 19).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato18);
                                    sheetOne.Cell(18, 19).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(19, 19).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato18);
                                    sheetOne.Cell(19, 19).Style.NumberFormat.Format = "#,##0.00";
                                }
                                else if (IfDato0Param.StartsWith(prefixCont))
                                {
                                    sheetOne.Cell(21, 3).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato2);
                                    sheetOne.Cell(21, 3).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 3).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato2);
                                    sheetOne.Cell(22, 3).Style.NumberFormat.Format = "#,##0.00";
                                    //Enero
                                    sheetOne.Cell(21, 4).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato3);
                                    sheetOne.Cell(21, 4).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 4).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato3);
                                    sheetOne.Cell(22, 4).Style.NumberFormat.Format = "#,##0.00";
                                    //Febrero
                                    sheetOne.Cell(21, 5).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato4);
                                    sheetOne.Cell(21, 5).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 5).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato4);
                                    sheetOne.Cell(22, 5).Style.NumberFormat.Format = "#,##0.00";
                                    //Marzo
                                    sheetOne.Cell(21, 6).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato5);
                                    sheetOne.Cell(21, 6).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 6).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato5);
                                    sheetOne.Cell(22, 6).Style.NumberFormat.Format = "#,##0.00";
                                    //Abril
                                    sheetOne.Cell(21, 7).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato6);
                                    sheetOne.Cell(21, 7).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 7).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato6);
                                    sheetOne.Cell(22, 7).Style.NumberFormat.Format = "#,##0.00";
                                    //Mayo
                                    sheetOne.Cell(21, 8).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato7);
                                    sheetOne.Cell(21, 8).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 8).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato7);
                                    sheetOne.Cell(22, 8).Style.NumberFormat.Format = "#,##0.00";
                                    //Junio
                                    sheetOne.Cell(21, 9).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato8);
                                    sheetOne.Cell(21, 9).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 9).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato8);
                                    sheetOne.Cell(22, 9).Style.NumberFormat.Format = "#,##0.00";
                                    //Julio
                                    sheetOne.Cell(21, 10).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato9);
                                    sheetOne.Cell(21, 10).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 10).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato9);
                                    sheetOne.Cell(22, 10).Style.NumberFormat.Format = "#,##0.00";
                                    //Agosto
                                    sheetOne.Cell(21, 11).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato10);
                                    sheetOne.Cell(21, 11).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 11).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato10);
                                    sheetOne.Cell(22, 11).Style.NumberFormat.Format = "#,##0.00";
                                    //Septiembre
                                    sheetOne.Cell(21, 12).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato11);
                                    sheetOne.Cell(21, 12).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 12).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato11);
                                    sheetOne.Cell(22, 12).Style.NumberFormat.Format = "#,##0.00";
                                    //Octubre
                                    sheetOne.Cell(21, 13).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato12);
                                    sheetOne.Cell(21, 13).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 13).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato12);
                                    sheetOne.Cell(22, 13).Style.NumberFormat.Format = "#,##0.00";
                                    //Noviembre
                                    sheetOne.Cell(21, 14).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato13);
                                    sheetOne.Cell(21, 14).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 14).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato13);
                                    sheetOne.Cell(22, 14).Style.NumberFormat.Format = "#,##0.00";
                                    //Diciembre
                                    sheetOne.Cell(21, 15).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato14);
                                    sheetOne.Cell(21, 15).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 15).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato14);
                                    sheetOne.Cell(22, 15).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMasUno
                                    sheetOne.Cell(21, 17).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato16);
                                    sheetOne.Cell(21, 17).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 17).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato16);
                                    sheetOne.Cell(22, 17).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMasDos
                                    sheetOne.Cell(21, 18).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato17);
                                    sheetOne.Cell(21, 18).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 18).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato17);
                                    sheetOne.Cell(22, 18).Style.NumberFormat.Format = "#,##0.00";
                                    //AnioMasTres
                                    sheetOne.Cell(21, 19).Value = stringToDecimalFromDB(filaMonedaNacional.IfDDato18);
                                    sheetOne.Cell(21, 19).Style.NumberFormat.Format = "#,##0.00";
                                    sheetOne.Cell(22, 19).Value = stringToDecimalFromDB(filaMonedaExtranjera.IfDDato18);
                                    sheetOne.Cell(22, 19).Style.NumberFormat.Format = "#,##0.00";
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    e.ToString();
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        [Route("DescargarExcelEjercicioOficialFinal2Pasos/{token}")]
        public ActionResult DescargarExcelEjercicioOficialFinal2Pasos(string token, string iniciativaToken, string filename)
        {
            string ruta = Path.Combine(Server.MapPath("~/Scripts/Import/" + token), filename);
            FileStream fstream = new FileStream(ruta, FileMode.Open);
            XLWorkbook wb = new XLWorkbook(fstream);
            try
            {
                string tipoIniciativaSeleccionada = ((Session["tipoIniciativaEjercicioOficial"] != null) ? Convert.ToString(Session["tipoIniciativaEjercicioOficial"]) : "");
                string anioIniciativaSeleccionada = ((Session["anioIniciativaEjercicioOficial"] != null) ? Convert.ToString(Session["anioIniciativaEjercicioOficial"]) : "");
                string parametroVNToken = string.Empty;
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        if (!string.IsNullOrEmpty(tipoIniciativaSeleccionada) && !string.IsNullOrEmpty(anioIniciativaSeleccionada))
                        {
                            var selParametroVNEjercicioOficial = SqlMapper.Query(objConnection, "CAPEX_GET_PARAMETRO_VN_OFICIAL", new { PVNPERIODO = anioIniciativaSeleccionada, @PVNTIPO = tipoIniciativaSeleccionada }, commandType: CommandType.StoredProcedure).ToList();
                            if (selParametroVNEjercicioOficial != null && selParametroVNEjercicioOficial.Count > 0)
                            {
                                foreach (var s in selParametroVNEjercicioOficial)
                                {
                                    parametroVNToken = s.ParametroVNToken;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        e.ToString();
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
                modificarValoresExcelPresupuesto(wb.Worksheet(2), iniciativaToken, tipoIniciativaSeleccionada, anioIniciativaSeleccionada, parametroVNToken);
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

            return null;
        }


        [Route("DescargarExcelPresupuestoFinal2Pasos/{token}")]
        public ActionResult DescargarExcelPresupuestoFinal2Pasos(string token, string iniciativaToken, string filename)
        {
            string ruta = Path.Combine(Server.MapPath("~/Scripts/Import/" + token), filename);
            FileStream fstream = new FileStream(ruta, FileMode.Open);
            XLWorkbook wb = new XLWorkbook(fstream);
            try
            {
                string tipoIniciativaSeleccionada = ((Session["tipoIniciativaOrientacionComercial"] != null) ? Convert.ToString(Session["tipoIniciativaOrientacionComercial"]) : "");
                string anioIniciativaSeleccionada = ((Session["anioIniciativaOrientacionComercial"] != null) ? Convert.ToString(Session["anioIniciativaOrientacionComercial"]) : "");
                string parametroVNToken = ((Session["ParametroVNToken"] != null) ? Convert.ToString(Session["ParametroVNToken"]) : "");
                modificarValoresExcelPresupuesto(wb.Worksheet(2), iniciativaToken, tipoIniciativaSeleccionada, anioIniciativaSeleccionada, parametroVNToken);
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

            return null;
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
        [Route("DescargarExcelEjercicioOficial2Pasos/{token}")]
        public JsonResult DescargarExcelEjercicioOficial2Pasos(string token)
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
                    if (!string.IsNullOrEmpty(urlAzure))
                    {
                        string tipoIniciativaEjercicioOficial = ((Session["tipoIniciativaEjercicioOficial"] != null) ? Convert.ToString(Session["tipoIniciativaEjercicioOficial"]) : "");
                        if (!string.IsNullOrEmpty(tipoIniciativaEjercicioOficial))
                        {
                            string filename = DownloadFileParametroVN(urlAzure, token, nameFileFinal, tipoIniciativaEjercicioOficial);
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
                else
                {
                    return Json(new { IsSuccess = false, Message = "Error" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception err)
            {
                ExceptionResult = "DescargarExcelPresupuesto2Pasos, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { IsSuccess = false, Message = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        [Route("DescargarExcelPresupuesto2Pasos/{token}")]
        public JsonResult DescargarExcelPresupuesto2Pasos(string token)
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
                    if (!string.IsNullOrEmpty(urlAzure))
                    {
                        string tipoIniciativaSeleccionado = ((Session["tipoIniciativaOrientacionComercial"] != null) ? Convert.ToString(Session["tipoIniciativaOrientacionComercial"]) : "");
                        if (!string.IsNullOrEmpty(tipoIniciativaSeleccionado))
                        {
                            string filename = DownloadFileParametroVN(urlAzure, token, nameFileFinal, tipoIniciativaSeleccionado);
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
                else
                {
                    return Json(new { IsSuccess = false, Message = "Error" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception err)
            {
                ExceptionResult = "DescargarExcelPresupuesto2Pasos, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { IsSuccess = false, Message = "Error" }, JsonRequestBehavior.AllowGet);
            }
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