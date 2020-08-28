using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapexIdentity.Utilities;
using CapexInfraestructure.Bll.Entities.Mantenedor;
using CapexInfraestructure.Bll.Business.Mantenedor;
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
using CapexInfraestructure.Bll.Entities.Planificacion;

namespace Capex.Web.Controllers
{
    [AuthorizeAdminOrMember]
    [RoutePrefix("Mantenedor")]
    public class MantenedorController : Controller
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
        ///     CONTROLADOR "MantenedorController" 
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
        //public SqlConnection ORM;
        #endregion

        #region "CONSTRUCTOR"
        public MantenedorController()
        {
            ////IDENTIFICACION
            //FactoryPlanificacion = new PlanificacionFactory();
            JsonResponse = string.Empty;
            //ORM = CapexInfraestructure.Utilities.Utils.Conectar();
        }
        #endregion

        #region "METODOS INDICE DE MANTENEDORES"
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
                var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                {
                    return RedirectToAction("Logout", "Login");
                }
            }
            return View("Index");
        }
        #endregion

        #region "METODOS MANTENEDORES DE USUARIO"
        /// <summary>
        /// METODO LISTADO DE USUARIOS
        /// </summary>
        /// <returns></returns>
        [Route("Usuario")]
        public ActionResult Usuario()
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                        var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                        if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                        {
                            return RedirectToAction("Logout", "Login");
                        }
                        else
                        {
                            Session["CAPEX_SESS_FILTRO_USR"] = "";
                            ViewBag.Usuarios = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_USUARIO_LISTAR", commandType: CommandType.StoredProcedure).ToList();
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { Mensaje = ex.Message.ToString() + "-----" + ex.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
            return View("Usuario");
        }
        /// <summary>
        /// METODO BUSCAR USUARIO SUGERIDO POR BUSQUEDA PREVIA
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Usuario/Accion/BuscarSugerido/{token}")]
        public ActionResult UsuarioSugerido(string token)
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                        var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                        if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                        {
                            return RedirectToAction("Logout", "Login");
                        }
                        else
                        {
                            Session["CAPEX_SESS_FILTRO_USR"] = "ACTIVO";
                            ViewBag.Usuarios = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_USUARIO_LISTAR_SUGERIDO", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { Mensaje = ex.Message.ToString() + "-----" + ex.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
            return View("Usuario");
        }


        /// <summary>
        /// METODO DESPLEGAR VISTA NUEVO USUARIO
        /// </summary>
        /// <returns></returns>
        [Route("Usuario/Vista/Nuevo")]
        public ActionResult UsuarioNuevo()
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
            }
            return View("UsuarioNuevo");
        }
        /// <summary>
        /// METODO DE BUSQUEDA
        /// </summary>
        /// <param name="Terminos"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Usuario/Accion/Buscar")]
        public ActionResult UsuarioBuscar(string termino)
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                        var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                        if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                        {
                            return RedirectToAction("Logout", "Login");
                        }
                        else
                        {
                            var resultado = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_USUARIO_BUSCAR", new { @Termino = termino }, commandType: CommandType.StoredProcedure).ToList();
                            return Json(new { Resultado = resultado }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception err)
                    {
                        var ExceptionResult = "UsuarioBuscar, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// <summary>
        /// METODO ACCION GUARDAR
        /// </summary>
        /// <param name="DatosUsuario"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Usuario/Accion/Crear")]
        public ActionResult UsuarioGuardarDatos(Usuario.NuevoUsuario DatosUsuario)
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        var parametos = new DynamicParameters();
                        parametos.Add("UserName", DatosUsuario.UserName);
                        parametos.Add("Password", DatosUsuario.Password);
                        parametos.Add("Email", DatosUsuario.Email);
                        parametos.Add("Status", DatosUsuario.Status);

                        parametos.Add("ComToken", DatosUsuario.ComToken);
                        parametos.Add("AreaToken", DatosUsuario.AreaToken);
                        parametos.Add("IdEmpresa", DatosUsuario.IdEmpresa);
                        parametos.Add("UsuTipo", DatosUsuario.UsuTipo);
                        parametos.Add("UsuRut", DatosUsuario.UsuRut);
                        parametos.Add("UsuNombre", DatosUsuario.UsuNombre);
                        parametos.Add("UsuApellido", DatosUsuario.UsuApellido);
                        parametos.Add("UsuEmail", DatosUsuario.UsuEmail);
                        parametos.Add("UsuTelefono", DatosUsuario.UsuTelefono);
                        parametos.Add("UsuMovil", DatosUsuario.UsuMovil);
                        parametos.Add("UsuImagen", DatosUsuario.UsuImagen);

                        parametos.Add("GrvUser", DatosUsuario.GrvUser);
                        parametos.Add("GrvUserToken", DatosUsuario.GrvUserToken);
                        parametos.Add("GrvAreaRevToken", DatosUsuario.GrvAreaRevToken);
                        parametos.Add("GrvAreaRevNombre", DatosUsuario.GrvAreaRevNombre);

                        parametos.Add("UserID", DatosUsuario.UserID);
                        parametos.Add("RoleID", DatosUsuario.RoleID);

                        parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);
                        SqlMapper.Query(objConnection, "CAPEX_INS_MANTENEDOR_USUARIO", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                        if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                        {
                            return Json(new { Mensaje = parametos.Get<string>("Respuesta") }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { Mensaje = "ERROR" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception err)
                    {
                        var ExceptionResult = "UsuarioGuardarDatos, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                        return Json(new { Mensaje = "ERROR" }, JsonRequestBehavior.AllowGet);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
        }
        /// <summary>
        /// METODO DESPLEGAR VISTA MODIFICAR
        /// </summary>
        /// <returns></returns>
        [Route("Usuario/Vista/Modificar/{token}")]
        public ActionResult UsuarioModificar(string token)
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
                    using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                    {
                        try
                        {
                            objConnection.Open();
                            var list_usuario_token = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_BUSCAR_USUARIO_TOKEN", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
                            foreach (var usuarioSeleccionado in list_usuario_token)
                            {
                                ViewBag.UsuToken = usuarioSeleccionado.UsuToken;
                                ViewBag.UserName = usuarioSeleccionado.UserName;
                                ViewBag.Email = usuarioSeleccionado.Email;
                                ViewBag.Password = usuarioSeleccionado.Password;
                                ViewBag.ComToken = usuarioSeleccionado.ComToken;
                                ViewBag.AreaToken = usuarioSeleccionado.AreaToken;
                                ViewBag.UsuTipo = usuarioSeleccionado.UsuTipo;
                                ViewBag.UsuRut = usuarioSeleccionado.UsuRut;
                                ViewBag.UsuNombre = usuarioSeleccionado.UsuNombre;
                                ViewBag.UsuApellido = usuarioSeleccionado.UsuApellido;
                                ViewBag.UsuEmail = usuarioSeleccionado.UsuEmail;
                                ViewBag.UsuTelefono = usuarioSeleccionado.UsuTelefono;
                                ViewBag.UsuMovil = usuarioSeleccionado.UsuMovil;
                                ViewBag.UserRoleID = usuarioSeleccionado.UserRoleID;
                                ViewBag.RoleID = usuarioSeleccionado.RoleID;
                                ViewBag.GrvToken = usuarioSeleccionado.GrvToken;
                                ViewBag.GrvAreaRevToken = usuarioSeleccionado.GrvAreaRevToken;
                            }
                        }
                        catch (Exception ex)
                        {
                            return Json(new { Mensaje = ex.Message.ToString() + "-----" + ex.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                        }
                        finally
                        {
                            objConnection.Close();
                        }
                    }
                }
            }
            return View("UsuarioModificar");
        }
        /// <summary>
        /// METODO ACCION MODIFICAR
        /// </summary>
        /// <param name="DatosUsuario"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Usuario/Accion/Modificar")]
        public ActionResult UsuarioModificarDatos(Usuario.ModificarUsuarioCorregido DatosUsuario)
        {
            if (!@User.Identity.IsAuthenticated || Session["CAPEX_SESS_USERNAME"] == null)
            {
                return Json(new { redirectUrlLogout = "true" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        var parametos = new DynamicParameters();
                        parametos.Add("IdToken", DatosUsuario.IdToken);
                        parametos.Add("IdRolToken", DatosUsuario.IdRolToken);
                        parametos.Add("IdGrvToken", DatosUsuario.IdGrvToken);
                        parametos.Add("UserName", DatosUsuario.UserName);
                        parametos.Add("Password", DatosUsuario.Password);
                        parametos.Add("Email", DatosUsuario.Email);
                        parametos.Add("RoleID", DatosUsuario.RoleID);
                        parametos.Add("UsuRut", DatosUsuario.UsuRut);
                        parametos.Add("UsuNombre", DatosUsuario.UsuNombre);
                        parametos.Add("UsuApellido", DatosUsuario.UsuApellido);
                        parametos.Add("UsuTelefono", DatosUsuario.UsuTelefono);
                        parametos.Add("UsuMovil", DatosUsuario.UsuMovil);
                        parametos.Add("ComToken", DatosUsuario.ComToken);
                        parametos.Add("AreaToken", DatosUsuario.AreaToken);
                        parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);
                        SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_CORREGIDO_USUARIO", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                        if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                        {
                            return Json(new { Mensaje = parametos.Get<string>("Respuesta") }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception err)
                    {
                        var ExceptionResult = "UsuarioModificarDatos, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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

        /// <summary>
        /// ELIMINAR /CERRAR CUENTA DE USUARIO
        /// </summary>
        /// <param name="UsuToken"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Usuario/Accion/Eliminar")]
        public ActionResult UsuarioEliminar(string UsuToken, string Status)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    SqlMapper.Query(objConnection, "CAPEX_DEL_MANTENEDOR_USUARIO", new { UsuToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Eliminado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "UsuarioEliminar, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// NETODO ACTIVAR USUARIO
        /// </summary>
        /// <param name="UsuToken"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Usuario/Accion/Activar")]
        public ActionResult UsuarioActivar(string UsuToken, string Status)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_USUARIO_ESTADO", new { UsuToken, Status }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Activado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "UsuarioActivar, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// 
        /// </summary>
        /// <param name="UsuToken"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Usuario/Accion/Bloquear")]
        public ActionResult UsuarioBloquear(string UsuToken, string Status)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_USUARIO_ESTADO", new { UsuToken, Status }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Bloqueado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "UsuarioBloquear, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }
        #endregion


        #region "METODOS MANTENEDORES DE GERENCIA"
        /// <summary>
        ///  METODO DESPLEGAR GERENCIA
        /// </summary>
        /// <returns></returns>
        [Route("Gerencia")]
        public ActionResult Gerencia()
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                        var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                        if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                        {
                            return RedirectToAction("Logout", "Login");
                        }
                        else
                        {
                            Session["CAPEX_SESS_FILTRO_USR"] = "";
                            ViewBag.Usuarios = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_GERENCIA_LISTAR", commandType: CommandType.StoredProcedure).ToList();
                        }
                    }
                    catch (Exception err)
                    {
                        var ExceptionResult = "Gerencia, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
            return View("Gerencia");
        }

        /// <summary>
        /// METODO PARA GUARDAR GERENCIA
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Gerencia/Guardar")]
        public ActionResult GuardarGerencia(Gerencia.GuardarGerencia Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    parametos.Add("GerNombre", Datos.GerNombre);
                    parametos.Add("GerDescripcion", Datos.GerDescripcion);
                    parametos.Add("GerEstado", Datos.GerEstado);
                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);
                    SqlMapper.Query(objConnection, "CAPEX_INS_MANTENEDOR_GERENCIA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                    {
                        return Json(new { Mensaje = "Guardado" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception err)
                {
                    var ExceptionResult = "GuardarGerencia, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        [HttpGet]
        [Route("Gerencia/BuscarPorToken")]
        public ActionResult BuscarGerenciaPorToken(string GerToken)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    GerToken = GerToken.Replace(System.Environment.NewLine, "");
                    var resultado = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_BUSCAR_GERENCIA_TOKEN", new { GerToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = resultado }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "BuscarGerenciaPorToken, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// METODO ACTUALIZAR GERENCIA
        /// </summary>
        /// <param name="DatosArea"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Gerencia/Actualizar")]
        public ActionResult ActualizarGerencia(Gerencia.ActualizarGerencia Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    parametos.Add("GerToken", Datos.GerToken);
                    parametos.Add("GerNombre", Datos.GerNombre);
                    parametos.Add("GerDescripcion", Datos.GerDescripcion);
                    parametos.Add("GerEstado", Datos.GerEstado);
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_GERENCIA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Actualizado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "ActualizarArea, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// METODO DESCATIVAR GERENCIA
        /// </summary>
        /// <param name="GerToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Gerencia/Desactivar")]
        public ActionResult DesactivarGerencia(string GerToken)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_DESACTIVAR_GERENCIA", new { GerToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Descativado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "DesactivarGerencia, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        #endregion

        #region "METODOS MANTENEDORES DE SUPERINTENDENCIA"
        /// <summary>
        ///  METODO DESPLEGAR SUPERINTENDENCIA
        /// </summary>
        /// <returns></returns>
        [Route("Superintendencia")]
        public ActionResult Superintendencia()
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                        var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                        if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                        {
                            return RedirectToAction("Logout", "Login");
                        }
                        else
                        {
                            Session["CAPEX_SESS_FILTRO_USR"] = "";
                            ViewBag.Usuarios = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_GERENCIA_LISTAR", commandType: CommandType.StoredProcedure).ToList();
                        }
                    }
                    catch (Exception err)
                    {
                        var ExceptionResult = "Superintendencia, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
            return View("Superintendencia");
        }

        /// <summary>
        /// METODO PARA GUARDAR SUPERINTENDENCIA
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Superintendencia/Guardar")]
        public ActionResult GuardarSuperintendencia(Gerencia.GuardarGerencia Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    parametos.Add("GerNombre", Datos.GerNombre);
                    parametos.Add("GerDescripcion", Datos.GerDescripcion);
                    parametos.Add("GerEstado", Datos.GerEstado);
                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);
                    SqlMapper.Query(objConnection, "CAPEX_INS_MANTENEDOR_GERENCIA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                    {
                        return Json(new { Mensaje = "Guardado" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception err)
                {
                    var ExceptionResult = "GuardarGerencia, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        [HttpGet]
        [Route("Superintendencia/BuscarPorToken")]
        public ActionResult BuscarSuperintendenciaPorToken(string GerToken)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    GerToken = GerToken.Replace(System.Environment.NewLine, "");
                    var resultado = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_BUSCAR_GERENCIA_TOKEN", new { GerToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = resultado }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "BuscarSuperintendenciaPorToken, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// METODO ACTUALIZAR SUPERINTENDENCIA
        /// </summary>
        /// <param name="DatosArea"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Superintendencia/Actualizar")]
        public ActionResult ActualizarSuperintendencia(Gerencia.ActualizarGerencia Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    parametos.Add("GerToken", Datos.GerToken);
                    parametos.Add("GerNombre", Datos.GerNombre);
                    parametos.Add("GerDescripcion", Datos.GerDescripcion);
                    parametos.Add("GerEstado", Datos.GerEstado);
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_GERENCIA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Actualizado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "ActualizarSuperintendencia, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// METODO DESCATIVAR SUPERINTENDENCIA
        /// </summary>
        /// <param name="GerToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Superintendencia/Desactivar")]
        public ActionResult DesactivarSuperintendencia(string GerToken)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_DESACTIVAR_GERENCIA", new { GerToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Descativado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "DesactivarSuperintendencia, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        #endregion

        #region "METODOS MANTENEDORES DE GERENTE"
        /// <summary>
        /// METODO LISTADO DE GERENTE
        /// </summary>
        /// <returns></returns>
        [Route("Gerente")]
        public ActionResult Gerente()
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                        var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                        if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                        {
                            return RedirectToAction("Logout", "Login");
                        }
                        else
                        {
                            Session["CAPEX_SESS_FILTRO_USR"] = "";
                            ViewBag.Usuarios = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_GERENTE_LISTAR", commandType: CommandType.StoredProcedure).ToList();
                        }
                    }
                    catch (Exception err)
                    {
                        var ExceptionResult = "Gerente, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
            return View("Gerente");
        }

        /// <summary>
        /// METODO PARA GUARDAR CATEGORIA
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Gerente/Guardar")]
        public ActionResult GuardarGerente(Gerente.NuevoGerente Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    parametos.Add("GteNombre", Datos.GteNombre);
                    parametos.Add("GteDescripcion", Datos.GteDescripcion);
                    parametos.Add("GteEstado", Datos.GteEstado);
                    parametos.Add("IdGerencia", Datos.IdGerencia);
                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);
                    SqlMapper.Query(objConnection, "CAPEX_INS_MANTENEDOR_GERENTE", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                    {
                        return Json(new { Mensaje = "Guardado" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception err)
                {
                    var ExceptionResult = "GuardarGerente, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// VALIDAR IDENTIFICACION INICIATIVA
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Gerente/ValidarGerenciaGerenteActivo")]
        public ActionResult ValidarGerenciaGerenteActivo(string GteToken, int IdGerencia)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var resultProcedure = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_BUSCAR_GERENTE_GERENCIA_ACTIVO", new { @GteToken = GteToken, IdGerencia = IdGerencia }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    if (resultProcedure != null)
                    {
                        var value = "0";
                        if (resultProcedure.GA > 0)
                        {
                            value = "1";
                        }
                        else
                        {
                            value = "0";
                        }
                        return Json(new { Mensaje = value }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception err)
                {
                    var ExceptionResult = "ValidarActivo, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// VALIDAR IDENTIFICACION INICIATIVA
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Gerente/ValidarActivo")]
        public ActionResult ValidarGerenteActivo(int Gerencia)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var resultProcedure = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_BUSCAR_GERENTE_ACTIVO", new { IdGerencia = Gerencia }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    if (resultProcedure != null)
                    {
                        var value = "0";
                        if (resultProcedure.GA > 0)
                        {
                            value = "1";
                        }
                        else
                        {
                            value = "0";
                        }
                        return Json(new { Mensaje = value }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception err)
                {
                    var ExceptionResult = "ValidarActivo, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        [HttpGet]
        [Route("Gerente/BuscarPorToken")]
        public ActionResult BuscarGerentePorToken(string GteToken)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    GteToken = GteToken.Replace(System.Environment.NewLine, "");
                    var resultado = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_BUSCAR_GERENTE_TOKEN", new { GteToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = resultado }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "BuscarPorToken, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// METODO ACTUALIZAR CATEGORIA
        /// </summary>
        /// <param name="DatosCategoria"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Gerente/Actualizar")]
        public ActionResult ActualizarGerente(Gerente.ModificarGerente Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    parametos.Add("GteToken", Datos.GteToken);
                    parametos.Add("GteNombre", Datos.GteNombre);
                    parametos.Add("IdGerencia", Datos.IdGerencia);
                    parametos.Add("GteDescripcion", Datos.GteDescripcion);
                    parametos.Add("GteEstado", Datos.GteEstado);
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_GERENTE", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Actualizado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "ActualizarGerente, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// METODO DESCATIVAR CATEGORIA
        /// </summary>
        /// <param name="CatToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Gerente/Desactivar")]
        public ActionResult DesactivarGerente(string GteToken)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_DESACTIVAR_GERENTE", new { GteToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Descativado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "DesactivarGerente, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// METODO DESPLEGAR VISTA NUEVO GERENTE
        /// </summary>
        /// <returns></returns>
        [Route("Gerente/Vista/Nuevo")]
        public ActionResult GerenteNuevo()
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                        var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                        if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                        {
                            return RedirectToAction("Logout", "Login");
                        }
                        else
                        {
                            ViewBag.Gerencias = SqlMapper.Query(objConnection, "CAPEX_SEL_GERENCIAS", commandType: CommandType.StoredProcedure).ToList();
                        }
                    }
                    catch (Exception err)
                    {
                        var ExceptionResult = "GerenteNuevo, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
            return View("GerenteNuevo");
        }
        /// <summary>
        /// METODO ACCION GUARDAR
        /// </summary>
        /// <param name="DatosUsuario"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Gerente/Accion/Crear")]
        public ActionResult GerenteGuardarDatos(Gerente.NuevoGerente DatosGerente)
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        var parametos = new DynamicParameters();
                        parametos.Add("IdGerencia", DatosGerente.IdGerencia);
                        parametos.Add("GteNombre", DatosGerente.GteNombre);
                        parametos.Add("GteDescripcion", DatosGerente.GteDescripcion);
                        parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);
                        SqlMapper.Query(objConnection, "CAPEX_INS_MANTENEDOR_GERENTE", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                        if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                        {
                            return Json(new { Mensaje = parametos.Get<string>("Respuesta") }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { Mensaje = "ERROR" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception err)
                    {
                        var ExceptionResult = "GerenteGuardarDatos, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                        return Json(new { Mensaje = "ERROR" }, JsonRequestBehavior.AllowGet);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
        }
        /// <summary>
        /// METODO DESPLEGAR VISTA MODIFICAR
        /// </summary>
        /// <returns></returns>
        [Route("Gerente/Vista/Modificar/{token}")]
        public ActionResult GerenteModificar(string token)
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

                    }
                    catch (Exception ex)
                    {
                        return Json(new { Mensaje = ex.Message.ToString() + "-----" + ex.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return View("GerenteModificar");
        }
        /// <summary>
        /// METODO ACCION MODIFICAR
        /// </summary>
        /// <param name="DatosUsuario"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Gerente/Accion/Modificar")]
        public ActionResult GerenteModificarDatos(Gerente.ModificarGerente DatosGerente)
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        var parametos = new DynamicParameters();
                        parametos.Add("IdGerencia", DatosGerente.IdGerencia);
                        parametos.Add("GteToken", DatosGerente.GteToken);
                        parametos.Add("GteNombre", DatosGerente.GteNombre);
                        parametos.Add("GteDescripcion", DatosGerente.GteDescripcion);

                        parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);
                        SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_GERENTE", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                        if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                        {
                            return Json(new { Mensaje = parametos.Get<string>("Respuesta") }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception err)
                    {
                        var ExceptionResult = "GerenteModificarDatos, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// <summary>
        /// NETODO ACTIVAR GERENTE
        /// </summary>
        /// <param name="GteToken"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Gerente/Accion/Activar")]
        public ActionResult GerenteActivar(string GteToken, string Status)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_GERENTE_ESTADO", new { GteToken, Status }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Activado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "GerenteActivar, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// NETODO BLOQUEAR GERENTE
        /// </summary>
        /// <param name="GteToken"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Gerente/Accion/Bloquear")]
        public ActionResult GerenteBloquear(string GteToken, string Status)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_GERENTE_ESTADO", new { GteToken, Status }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Bloqueado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "GerenteBloquear, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }
        #endregion

        #region "METODOS MANTENEDORES DE SUPERINTENDENTE"
        /// <summary>
        /// METODO LISTADO DE SUPERINTENDENTE
        /// </summary>
        /// <returns></returns>
        [Route("Superintendente")]
        public ActionResult Superintendente()
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                        var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                        if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                        {
                            return RedirectToAction("Logout", "Login");
                        }
                        else
                        {
                            Session["CAPEX_SESS_FILTRO_USR"] = "";
                            ViewBag.Usuarios = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_SUPERINTENDENTE_LISTAR", commandType: CommandType.StoredProcedure).ToList();
                        }
                    }
                    catch (Exception err)
                    {
                        var ExceptionResult = "Superintendente, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
            return View("Superintendente");
        }
        /// <summary>
        /// METODO DESPLEGAR VISTA NUEVO SUPERINTENDENTE
        /// </summary>
        /// <returns></returns>
        [Route("Superintendente/Vista/Nuevo")]
        public ActionResult SuperintendenteNuevo()
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                        var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                        if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                        {
                            return RedirectToAction("Logout", "Login");
                        }
                        else
                        {
                            ViewBag.Superintendencias = SqlMapper.Query(objConnection, "CAPEX_SEL_SUPERINTENDENCIAS", commandType: CommandType.StoredProcedure).ToList();
                        }
                    }
                    catch (Exception err)
                    {
                        var ExceptionResult = "SuperintendenteNuevo, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
            return View("SuperintendenteNuevo");
        }
        /// <summary>
        /// METODO ACCION GUARDAR SUPERINTENDENTE
        /// </summary>
        /// <param name="DatosUsuario"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Superintendente/Accion/Crear")]
        public ActionResult SuperintendenteGuardarDatos(Superintendente.NuevoSuperintendente DatosSuperintendente)
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        var parametos = new DynamicParameters();
                        parametos.Add("IdSuper", DatosSuperintendente.IdSuper);
                        parametos.Add("IntNombre", DatosSuperintendente.IntNombre);
                        parametos.Add("IntDescripcion", DatosSuperintendente.IntDescripcion);
                        parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);
                        SqlMapper.Query(objConnection, "CAPEX_INS_MANTENEDOR_SUPERINTENDENTE", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                        if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                        {
                            return Json(new { Mensaje = parametos.Get<string>("Respuesta") }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { Mensaje = "ERROR" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception err)
                    {
                        var ExceptionResult = "SuperintendenteGuardarDatos, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                        return Json(new { Mensaje = "ERROR" }, JsonRequestBehavior.AllowGet);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
        }
        /// <summary>
        /// METODO DESPLEGAR VISTA MODIFICAR
        /// </summary>
        /// <returns></returns>
        [Route("Superintendente/Vista/Modificar/{token}")]
        public ActionResult SuperintendenteModificar(string IntToken)
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

                    }
                    catch (Exception ex)
                    {
                        return Json(new { Mensaje = ex.Message.ToString() + "-----" + ex.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return View("SuperintendenteModificar");
        }
        /// <summary>
        /// METODO ACCION MODIFICAR SUPERINTENDENTE
        /// </summary>
        /// <param name="DatosUsuario"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Superintendente/Accion/Modificar")]
        public ActionResult SuperintendenteModificarDatos(Superintendente.NuevoSuperintendente DatosSuperintendente)
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                try
                {
                    //var parametos = new DynamicParameters();
                    //parametos.Add("ID", DatosUsuario.ID);
                    //parametos.Add("UserName", DatosUsuario.UserName);
                    //parametos.Add("Password", DatosUsuario.Password);
                    //parametos.Add("Email", DatosUsuario.Email);
                    //parametos.Add("Status", DatosUsuario.Status);

                    //parametos.Add("UsuToken", DatosUsuario.UsuToken);
                    //parametos.Add("ComToken", DatosUsuario.ComToken);
                    //parametos.Add("AreaToken", DatosUsuario.AreaToken);
                    //parametos.Add("IdEmpresa", DatosUsuario.IdEmpresa);
                    //parametos.Add("UsuTipo", DatosUsuario.UsuTipo);
                    //parametos.Add("UsuRut", DatosUsuario.UsuRut);
                    //parametos.Add("UsuNombre", DatosUsuario.UsuNombre);
                    //parametos.Add("UsuApellido", DatosUsuario.UsuApellido);
                    //parametos.Add("UsuEmail", DatosUsuario.UsuEmail);
                    //parametos.Add("UsuTelefono", DatosUsuario.UsuTelefono);
                    //parametos.Add("UsuMovil", DatosUsuario.UsuMovil);
                    //parametos.Add("UsuImagen", DatosUsuario.UsuImagen);

                    //parametos.Add("GrvToken", DatosUsuario.GrvToken);
                    //parametos.Add("GrvUser", DatosUsuario.GrvUser);
                    //parametos.Add("GrvUserToken", DatosUsuario.GrvUserToken);
                    //parametos.Add("GrvAreaRevToken", DatosUsuario.GrvAreaRevToken);
                    //parametos.Add("GrvAreaRevNombre", DatosUsuario.GrvAreaRevNombre);

                    //parametos.Add("UserRolId", DatosUsuario.UserRolID);
                    //parametos.Add("UserID", DatosUsuario.UserID);
                    //parametos.Add("RoleID", DatosUsuario.RoleID);

                    //parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                    //ORM.Query("CAPEX_UPD_MANTENEDOR_USUARIO", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    //if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                    //{
                    //    return Json(new { Mensaje = parametos.Get<string>("Respuesta") }, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    //}
                    return null;
                }
                catch (Exception err)
                {
                    var ExceptionResult = "SuperintendenteModificarDatos, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                }

            }
        }
        /// <summary>
        /// NETODO ACTIVAR SUPERINTENDENTE
        /// </summary>
        /// <param name="GteToken"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Superintendente/Accion/Activar")]
        public ActionResult SuperintendenteActivar(string IntToken, string Status)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_SUPERINTENDENTE_ESTADO", new { IntToken, Status }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Activado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "SuperintendenteActivar, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// NETODO BLOQUEAR SUPERINTENDENTE
        /// </summary>
        /// <param name="GteToken"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Superintendente/Accion/Bloquear")]
        public ActionResult SuperintendenteBloquear(string IntToken, string Status)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_SUPERINTENDENTE_ESTADO", new { IntToken, Status }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Bloqueado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "SuperintendenteBloquear, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }
        #endregion

        #region "METODOS MANTENEDORES DE CATEGORIAS"
        /// <summary>
        ///  METIDO DESPLEGAR CATEGORIA
        /// </summary>
        /// <returns></returns>
        [Route("Categoria")]
        public ActionResult Categoria()
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                        var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                        if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                        {
                            return RedirectToAction("Logout", "Login");
                        }
                        else
                        {
                            ViewBag.Categorias = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_CATEGORIAS", commandType: CommandType.StoredProcedure).ToList();
                        }
                    }
                    catch (Exception err)
                    {
                        var ExceptionResult = "Categoria, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
            return View("Categoria");
        }
        /// <summary>
        /// METODO PARA GUARDAR CATEGORIA
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Categoria/Guardar")]
        public ActionResult GuardarCategoria(Categoria.GuardarCategoria Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    parametos.Add("CatNombre", Datos.CatNombre);
                    parametos.Add("CatDescripcion", Datos.CatDescripcion);
                    parametos.Add("CatEstado", Datos.CatEstado);
                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);
                    SqlMapper.Query(objConnection, "CAPEX_INS_MANTENEDOR_CATEGORIA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                    {
                        return Json(new { Mensaje = "Guardado" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception err)
                {
                    var ExceptionResult = "GuardarCategoria, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        [HttpGet]
        [Route("Categoria/BuscarPorToken")]
        public ActionResult BuscarPorToken(string CatToken)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    CatToken = CatToken.Replace(System.Environment.NewLine, "");
                    var resultado = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_BUSCAR_CATEGORIA_TOKEN", new { CatToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = resultado }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "BuscarPorToken, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// METODO ACTUALIZAR CATEGORIA
        /// </summary>
        /// <param name="DatosCategoria"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Categoria/Actualizar")]
        public ActionResult ActualizarCategoria(Categoria.ActualizarCategoria Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    parametos.Add("CatToken", Datos.CatToken);
                    parametos.Add("CatNombre", Datos.CatNombre);
                    parametos.Add("CatDescripcion", Datos.CatDescripcion);
                    parametos.Add("CatEstado", Datos.CatEstado);
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_CATEGORIA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Actualizado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "ActualizarCategoria, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// METODO DESCATIVAR CATEGORIA
        /// </summary>
        /// <param name="CatToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Categoria/Desactivar")]
        public ActionResult DesactivarCategoria(string CatToken)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var Estado = 0;
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_DESACTIVAR_CATEGORIA", new { CatToken, Estado }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Descativado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "DesactivarCategoria, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }
        #endregion

        #region "METODOS MANTENEDORES DE MATRIZ DE RIESGO"
        /// <summary>
        ///  METIDO DESPLEGAR MATRIZ RIESGO
        /// </summary>
        /// <returns></returns>
        [Route("MatrizRiesgo")]
        public ActionResult MatrizRiesgo()
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                        var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                        if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                        {
                            return RedirectToAction("Logout", "Login");
                        }
                        else
                        {
                            ViewBag.Riesgos = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_RIESGOS", commandType: CommandType.StoredProcedure).ToList();
                        }
                    }
                    catch (Exception err)
                    {
                        var ExceptionResult = "MatrizRiesgo, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
            return View("MatrizRiesgo");
        }
        /// <summary>
        /// METODO PARA GUARDAR MATRIZ RIESGO
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("MatrizRiesgo/Guardar")]
        public ActionResult GuardarMatrizRiesgo(Riesgo.GuardarRiesgo Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    parametos.Add("RiesgoNombre", Datos.RiesgoNombre);
                    parametos.Add("EvrImpacto", Datos.EvrImpacto);
                    parametos.Add("EvrProbabilidad", Datos.EvrProbabilidad);
                    parametos.Add("RiesgoEstado", Datos.RiesgoEstado);
                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);
                    SqlMapper.Query(objConnection, "CAPEX_INS_MANTENEDOR_RIESGO", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                    {
                        return Json(new { Mensaje = "Guardado" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception err)
                {
                    var ExceptionResult = "GuardarMatrizRiesgo, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        [HttpGet]
        [Route("MatrizRiesgo/BuscarPorToken")]
        public ActionResult BuscarPorTokenMatrizRiesgo(string RiesgoToken)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    RiesgoToken = RiesgoToken.Replace(System.Environment.NewLine, "");
                    var resultado = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_BUSCAR_RIESGO_TOKEN", new { RiesgoToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = resultado }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "BuscarPorTokenMatrizRiesgo, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// METODO ACTUALIZAR MATRIZ RIESGO
        /// </summary>
        /// <param name="DatosCategoria"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("MatrizRiesgo/Actualizar")]
        public ActionResult ActualizarMatrizRiesgo(Riesgo.ActualizarRiesgo Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    parametos.Add("RiesgoToken", Datos.RiesgoToken);
                    parametos.Add("RiesgoNombre", Datos.RiesgoNombre);
                    parametos.Add("EvrImpacto", Datos.EvrImpacto);
                    parametos.Add("EvrProbabilidad", Datos.EvrProbabilidad);
                    parametos.Add("RiesgoEstado", Datos.RiesgoEstado);
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_RIESGO", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Actualizado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "ActualizarMatrizRiesgo, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// METODO DESCATIVAR MATRIZ RIESGO
        /// </summary>
        /// <param name="CatToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("MatrizRiesgo/Desactivar")]
        public ActionResult DesactivarMatrizRiesgo(string RiesgoToken)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var RiesgoEstado = 0;
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_DESACTIVAR_RIESGO", new { RiesgoToken, RiesgoEstado }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Descativado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "DesactivarMatrizRiesgo, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }
        #endregion

        #region "METODOS MANTENEDORES DE TEMPLATE"
        /// <summary>
        ///  METIDO DESPLEGAR MATRIZ RIESGO
        /// </summary>
        /// <returns></returns>
        [Route("TemplateIniciativa")]
        public ActionResult TemplateIniciativa()
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        var tipoIniciativaSeleccionadoMantenedor = Request.QueryString["tipoIniciativaSeleccionado"];
                        //1 CASO BASE//2 PRESUPUESTO
                        if (string.IsNullOrEmpty(tipoIniciativaSeleccionadoMantenedor))
                        {
                            tipoIniciativaSeleccionadoMantenedor = "1";
                        }
                        DateTime now = DateTime.Today;
                        ViewBag.anioMin = Convert.ToInt32(now.ToString("yyyy"));
                        if (tipoIniciativaSeleccionadoMantenedor.Equals("2"))
                        {
                            ViewBag.anioMin += 1;
                        }
                        Session["tipoIniciativaSeleccionadoMantenedor"] = tipoIniciativaSeleccionadoMantenedor;
                        ViewBag.tipoIniciativaSeleccionadoMantenedor = tipoIniciativaSeleccionadoMantenedor;
                        var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                        var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                        if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                        {
                            return RedirectToAction("Logout", "Login");
                        }
                        else
                        {
                            ViewBag.Templates = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_TEMPLATE_PARAM_COMERCIALES", new { @tipoIniciativaSeleccionadoMantenedor = tipoIniciativaSeleccionadoMantenedor }, commandType: CommandType.StoredProcedure).ToList();
                        }
                    }
                    catch (Exception err)
                    {
                        var ExceptionResult = "TemplateIniciativa, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
            return View("TemplateIniciativa");
        }
        /// <summary>
        /// METODO PARA GUARDAR MATRIZ RIESGO
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("TemplateIniciativa/Guardar")]
        public ActionResult GuardarTemplateIniciativa(Template.GuardarTemplateCorregido Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var parametos = new DynamicParameters();
                    parametos.Add("USUARIO", usuario);
                    parametos.Add("TPEPERIODO", Datos.TPEPERIODO);
                    parametos.Add("TipoIniciativaSeleccionado", Datos.TipoIniciativaSeleccionado);
                    parametos.Add("TPEPERIODOS", Datos.TPEPERIODOS);
                    parametos.Add("VALUEMESTC", Datos.VALUEMESTC);
                    parametos.Add("VALUEMESIPC", Datos.VALUEMESIPC);
                    parametos.Add("VALUEMESCPI", Datos.VALUEMESCPI);
                    parametos.Add("VALUEANIOTC", Datos.VALUEANIOTC);
                    parametos.Add("VALUEANIOIPC", Datos.VALUEANIOIPC);
                    parametos.Add("VALUEANIOCPI", Datos.VALUEANIOCPI);
                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 100);
                    SqlMapper.Query(objConnection, "CAPEX_INS_MANTENEDOR_TEMPLATE", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    string respuesta = parametos.Get<string>("Respuesta");
                    if (respuesta != null && !string.IsNullOrEmpty(respuesta.Trim()))
                    {
                        return Json(new { Mensaje = respuesta.Trim() }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "1|Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception err)
                {
                    var ExceptionResult = "GuardarTemplateIniciativa, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "1|Error|No es posible guardar el template. Por favor, inténtelo más tarde." }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        [HttpGet]
        [Route("TemplateIniciativa/BuscarPorToken")]
        public ActionResult BuscarPorTemplateIniciativa(string TemplateToken, string TipoIniciativaSeleccionado)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    TemplateToken = TemplateToken.Replace(System.Environment.NewLine, "");
                    Template.ObtenerTemplateCorregido obtenerTemplateCorregido = new Template.ObtenerTemplateCorregido();
                    var results = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_BUSCAR_TEMPLATE_TOKEN", new { TemplateToken, TipoIniciativaSeleccionado }, commandType: CommandType.StoredProcedure).ToList();
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
                            obtenerTemplateCorregido.EJERCICIOOFICIAL = result.EJERCICIOOFICIAL;
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
        /// METODO ACTUALIZAR MATRIZ RIESGO
        /// </summary>
        /// <param name="DatosCategoria"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("TemplateIniciativa/Actualizar")]
        public ActionResult ActualizarTemplateIniciativa(Template.ActualizarTemplateCorregido Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    parametos.Add("TipoIniciativaSeleccionado", Datos.TipoIniciativaSeleccionado);
                    parametos.Add("ITPEToken", Datos.ITPEToken);
                    parametos.Add("TPEPERIODO", Datos.TPEPERIODO);
                    parametos.Add("TPEPERIODORESPALDO", Datos.TPEPERIODORESPALDO);
                    parametos.Add("PETokenTC", Datos.PETokenTC);
                    parametos.Add("PETokenIPC", Datos.PETokenIPC);
                    parametos.Add("PETokenCPI", Datos.PETokenCPI);
                    parametos.Add("IdParamEconomicoDetalleTCMES", Datos.IdParamEconomicoDetalleTCMES);
                    parametos.Add("IdParamEconomicoDetalleIPCMES", Datos.IdParamEconomicoDetalleIPCMES);
                    parametos.Add("IdParamEconomicoDetalleCPIMES", Datos.IdParamEconomicoDetalleCPIMES);
                    parametos.Add("IdParamEconomicoDetalleTCANIO", Datos.IdParamEconomicoDetalleTCANIO);
                    parametos.Add("IdParamEconomicoDetalleIPCANIO", Datos.IdParamEconomicoDetalleIPCANIO);
                    parametos.Add("IdParamEconomicoDetalleCPIANIO", Datos.IdParamEconomicoDetalleCPIANIO);
                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 120);
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_TEMPLATE", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    var respuestaSP = parametos.Get<string>("Respuesta");
                    if (!string.IsNullOrEmpty(respuestaSP) && !string.IsNullOrEmpty(respuestaSP.Trim()))
                    {
                        if (respuestaSP.Trim().Equals("Actualizado"))
                        {
                            return Json(new { Mensaje = "Actualizado" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { Mensaje = respuestaSP.Trim() }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception err)
                {
                    var ExceptionResult = "TemplateIniciativa, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error al actualizar" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        /// <summary>
        /// METODO DESCATIVAR MATRIZ RIESGO
        /// </summary>
        /// <param name="CatToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("TemplateIniciativa/Desactivar")]
        public ActionResult DesactivarTemplateIniciativa(string TemplateToken)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var TemplateEstado = 0;
                    var parametos = new DynamicParameters();
                    parametos.Add("TemplateToken", TemplateToken);
                    parametos.Add("TemplateEstado", TemplateEstado);
                    parametos.Add("Usuario", usuario);
                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 120);
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_DESACTIVAR_TEMPLATE", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    var respuestaSP = parametos.Get<string>("Respuesta");
                    if (!string.IsNullOrEmpty(respuestaSP) && !string.IsNullOrEmpty(respuestaSP.Trim()))
                    {
                        if (respuestaSP.Trim().Equals("Actualizado"))
                        {
                            return Json(new { Mensaje = "Descativado" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { Mensaje = respuestaSP.Trim() }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception err)
                {
                    var ExceptionResult = "TemplateIniciativa, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }
        #endregion

        #region "METODOS MANTENEDORES DE TEMPLATE"
        /// <summary>
        ///  METIDO DESPLEGAR MATRIZ RIESGO
        /// </summary>
        /// <returns></returns>
        [Route("Bloqueo")]
        public ActionResult Bloqueo()
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        var tipoIniciativaSeleccionadoMantenedor = Request.QueryString["tipoIniciativaSeleccionado"];
                        //1 CASO BASE//2 PRESUPUESTO
                        if (string.IsNullOrEmpty(tipoIniciativaSeleccionadoMantenedor))
                        {
                            tipoIniciativaSeleccionadoMantenedor = "1";
                        }
                        Session["tipoIniciativaSeleccionadoMantenedor"] = tipoIniciativaSeleccionadoMantenedor;
                        ViewBag.tipoIniciativaSeleccionadoMantenedor = tipoIniciativaSeleccionadoMantenedor;
                        var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                        var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                        if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                        {
                            return RedirectToAction("Logout", "Login");
                        }
                        else
                        {
                            ViewBag.Bloqueos = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_BLOQUEO", new { @tipoIniciativaSeleccionadoMantenedor = tipoIniciativaSeleccionadoMantenedor }, commandType: CommandType.StoredProcedure).ToList();
                        }
                    }
                    catch (Exception err)
                    {
                        var ExceptionResult = "Bloqueo, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
            return View("Bloqueo");
        }

        /// <summary>
        /// METODO PARA GUARDAR MATRIZ RIESGO
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Bloqueo/Guardar")]
        public ActionResult GuardarBloqueo(Bloqueo.GuardarBloqueo Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var parametos = new DynamicParameters();
                    parametos.Add("USUARIO", usuario);
                    parametos.Add("TipoIniciativaSeleccionado", Datos.TipoIniciativaSeleccionado);
                    string convertFechaDesde = ((string.IsNullOrEmpty(Datos.FechaDesde)) ? Datos.FechaDesde : Datos.FechaDesde.Replace("-", "/"));
                    string[] fechaComponentes = convertFechaDesde.Split('/');
                    parametos.Add("FechaDesde", (fechaComponentes[1] + "/" + fechaComponentes[0] + "/" + fechaComponentes[2]));
                    string convertFechaHasta = ((string.IsNullOrEmpty(Datos.FechaHasta)) ? Datos.FechaHasta : Datos.FechaHasta.Replace("-", "/"));
                    string[] fechaComponentes2 = convertFechaHasta.Split('/');
                    parametos.Add("FechaHasta", (fechaComponentes2[1] + "/" + fechaComponentes2[0] + "/" + fechaComponentes2[2]));
                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 100);
                    SqlMapper.Query(objConnection, "CAPEX_INS_MANTENEDOR_BLOQUEO", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    string respuesta = parametos.Get<string>("Respuesta");
                    if (respuesta != null && !string.IsNullOrEmpty(respuesta.Trim()))
                    {
                        return Json(new { Mensaje = respuesta.Trim() }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "1|Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception err)
                {
                    var ExceptionResult = "GuardarBloqueo, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "1|Error|No es posible guardar el bloqueo. Por favor, inténtelo más tarde." }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        [HttpGet]
        [Route("Bloqueo/BuscarPorToken")]
        public ActionResult BuscarPorBloqueo(string BloqueoToken, string TipoIniciativaSeleccionado)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    BloqueoToken = BloqueoToken.Replace(System.Environment.NewLine, "");
                    List<Bloqueo.ObtenerBloqueo> results = SqlMapper.Query<Bloqueo.ObtenerBloqueo>(objConnection, "CAPEX_SEL_MANTENEDOR_BUSCAR_BLOQUEO_TOKEN", new { BloqueoToken, TipoIniciativaSeleccionado }, commandType: CommandType.StoredProcedure).ToList();
                    return Json(new { Mensaje = "Ok", Resultados = results }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "BuscarPorToken, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// METODO ACTUALIZAR MATRIZ RIESGO
        /// </summary>
        /// <param name="DatosCategoria"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Bloqueo/Actualizar")]
        public ActionResult ActualizarBloqueo(Bloqueo.ActualizarBloqueo Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var parametos = new DynamicParameters();
                    parametos.Add("TipoIniciativaSeleccionado", Datos.TipoIniciativaSeleccionado);
                    parametos.Add("FechaBloqueoToken", Datos.FechaBloqueoToken);
                    string convertFechaDesde = ((string.IsNullOrEmpty(Datos.FechaDesde)) ? Datos.FechaDesde : Datos.FechaDesde.Replace("-", "/"));
                    string[] fechaComponentes = convertFechaDesde.Split('/');
                    parametos.Add("FechaDesde", (fechaComponentes[1] + "/" + fechaComponentes[0] + "/" + fechaComponentes[2]));
                    string convertFechaHasta = ((string.IsNullOrEmpty(Datos.FechaHasta)) ? Datos.FechaHasta : Datos.FechaHasta.Replace("-", "/"));
                    string[] fechaComponentes2 = convertFechaHasta.Split('/');
                    parametos.Add("FechaHasta", (fechaComponentes2[1] + "/" + fechaComponentes2[0] + "/" + fechaComponentes2[2]));
                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 120);
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_BLOQUEO", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    var respuestaSP = parametos.Get<string>("Respuesta");
                    if (!string.IsNullOrEmpty(respuestaSP) && !string.IsNullOrEmpty(respuestaSP.Trim()))
                    {
                        if (respuestaSP.Trim().Equals("Actualizado"))
                        {
                            return Json(new { Mensaje = "Actualizado" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { Mensaje = respuestaSP.Trim() }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception err)
                {
                    var ExceptionResult = "Bloqueo, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error al actualizar" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        /// <summary>
        /// METODO DESCATIVAR MATRIZ RIESGO
        /// </summary>
        /// <param name="CatToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Bloqueo/Desactivar")]
        public ActionResult DesactivarBloqueo(string BloqueoToken)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var BloqueoEstado = 0;
                    var parametos = new DynamicParameters();
                    parametos.Add("BloqueoToken", BloqueoToken);
                    parametos.Add("BloqueoEstado", BloqueoEstado);
                    parametos.Add("Usuario", usuario);
                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 120);
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_DESACTIVAR_BLOQUEO", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    var respuestaSP = parametos.Get<string>("Respuesta");
                    if (!string.IsNullOrEmpty(respuestaSP) && !string.IsNullOrEmpty(respuestaSP.Trim()))
                    {
                        if (respuestaSP.Trim().Equals("Actualizado"))
                        {
                            return Json(new { Mensaje = "Descativado" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { Mensaje = respuestaSP.Trim() }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception err)
                {
                    var ExceptionResult = "Desactivar, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }
        #endregion

        #region "METODOS MANTENEDORES DE AREAS"
        /// <summary>
        ///  METODO DESPLEGAR AREAS
        /// </summary>
        /// <returns></returns>
        [Route("Area")]
        public ActionResult Area()
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                        var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                        if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                        {
                            return RedirectToAction("Logout", "Login");
                        }
                        else
                        {
                            ViewBag.Areas = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEODR_AREAS", commandType: CommandType.StoredProcedure).ToList();
                        }
                    }
                    catch (Exception err)
                    {
                        var ExceptionResult = "Area, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
            return View("Area");
        }
        /// <summary>
        /// METODO PARA GUARDAR AREA
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Area/Guardar")]
        public ActionResult GuardarArea(Area.GuardarArea Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    parametos.Add("AreaNombre", Datos.AreaNombre);
                    parametos.Add("AreaAcronimo", Datos.AreaAcronimo);
                    parametos.Add("AreaDescripcion", Datos.AreaDescripcion);
                    parametos.Add("AreaEstado", Datos.AreaEstado);
                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);
                    SqlMapper.Query(objConnection, "CAPEX_INS_MANTENEDOR_AREA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                    {
                        return Json(new { Mensaje = "Guardado" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception err)
                {
                    var ExceptionResult = "GuardarArea, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        [HttpGet]
        [Route("Area/BuscarPorToken")]
        public ActionResult BuscarAreaPorToken(string AreaToken)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    AreaToken = AreaToken.Replace(System.Environment.NewLine, "");
                    var resultado = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_BUSCAR_AREA_TOKEN", new { AreaToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = resultado }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "BuscarAreaPorToken, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// METODO ACTUALIZAR AREA
        /// </summary>
        /// <param name="DatosArea"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Area/Actualizar")]
        public ActionResult ActualizarArea(Area.ActualizarArea Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    parametos.Add("AreaToken", Datos.AreaToken);
                    parametos.Add("AreaNombre", Datos.AreaNombre);
                    parametos.Add("AreaAcronimo", Datos.AreaAcronimo);
                    parametos.Add("AreaDescripcion", Datos.AreaDescripcion);
                    parametos.Add("AreaEstado", Datos.AreaEstado);
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_AREA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Actualizado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "ActualizarArea, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// METODO DESCATIVAR AREA
        /// </summary>
        /// <param name="AreaToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Area/Desactivar")]
        public ActionResult DesactivarArea(string AreaToken)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_DESACTIVAR_AREA", new { AreaToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Descativado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "DesactivarArea, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// METODO DESCATIVAR AREA
        /// </summary>
        /// <param name="AreaToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Area/Activar")]
        public ActionResult ActivarArea(string AreaToken)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_ACTIVAR_AREA", new { AreaToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Activado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "ActivarArea, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }
        #endregion

        #region "METODOS MANTENEDORES DE CLASIFICACION SSO"
        /// <summary>
        ///  METODO DESPLEGAR CLASIFICACION SSO
        /// </summary>
        /// <returns></returns>
        [Route("ClasificacionSSO")]
        public ActionResult CLASIFICACIONSSO()
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
                {
                    try
                    {
                        objConnection.Open();
                        var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                        var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                        if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                        {
                            return RedirectToAction("Logout", "Login");
                        }
                        else
                        {
                            ViewBag.Clasificaciones = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_CLASIFICACIONES_SSO", commandType: CommandType.StoredProcedure).ToList();
                        }
                    }
                    catch (Exception err)
                    {
                        var ExceptionResult = "ActivarArea, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
            return View("ClasificacionSSO");
        }
        /// <summary>
        /// METODO PARA GUARDAR CLASIFICACION SSO
        /// </summary>
        /// <param name="Datos"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ClasificacionSSO/Guardar")]
        public ActionResult GuardarClasificacionSSO(ClasificacionSSO.GuardarClasificacionSSO Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    parametos.Add("CSNombre", Datos.CSNombre);
                    parametos.Add("CSDescripcion", Datos.CSDescripcion);
                    parametos.Add("CSEstado", Datos.CSEstado);
                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);
                    SqlMapper.Query(objConnection, "CAPEX_INS_MANTENEDOR_CLASIFICACION_SSO", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                    {
                        return Json(new { Mensaje = "Guardado" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception err)
                {
                    var ExceptionResult = "GuardarClasificacionSSO, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        [HttpGet]
        [Route("ClasificacionSSO/BuscarPorToken")]
        public ActionResult BuscarClasificacionSSOPorToken(string CSToken)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    CSToken = CSToken.Replace(System.Environment.NewLine, "");
                    var resultado = SqlMapper.Query(objConnection, "CAPEX_SEL_MANTENEDOR_BUSCAR_CLASIFICACION_SSO_TOKEN", new { CSToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = resultado }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "BuscarClasificacionSSOPorToken, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// METODO ACTUALIZAR Clasificacion SSO
        /// </summary>
        /// <param name="DatosClasificacionSSO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ClasificacionSSO/Actualizar")]
        public ActionResult ActualizarClasificacionSSO(ClasificacionSSO.ActualizarClasificacionSSO Datos)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    var parametos = new DynamicParameters();
                    parametos.Add("CSToken", Datos.CSToken);
                    parametos.Add("CSNombre", Datos.CSNombre);
                    parametos.Add("CSDescripcion", Datos.CSDescripcion);
                    parametos.Add("CSEstado", Datos.CSEstado);
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_CLASIFICACION_SSO", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Actualizado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "ActualizarClasificacionSSO, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// METODO DESCATIVAR AREA
        /// </summary>
        /// <param name="AreaToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ClasificacionSSO/Desactivar")]
        public ActionResult DesactivarClasificacionSSO(string CSToken)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_DESACTIVAR_CLASIFICACION_SSO", new { CSToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Descativado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "DesactivarClasificacionSSO, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
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
        /// METODO DESCATIVAR AREA
        /// </summary>
        /// <param name="AreaToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ClasificacionSSO/Activar")]
        public ActionResult ActivarClasificacionSSO(string CSToken)
        {
            using (SqlConnection objConnection = new SqlConnection(CapexIdentity.Utilities.Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    SqlMapper.Query(objConnection, "CAPEX_UPD_MANTENEDOR_ACTIVAR_CLASIFICACION_SSO", new { CSToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    return Json(new { Mensaje = "Activado" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception err)
                {
                    var ExceptionResult = "ActivarClasificacionSSO, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }
        #endregion
    }
}
