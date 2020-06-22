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

namespace Capex.Web.Controllers
{

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