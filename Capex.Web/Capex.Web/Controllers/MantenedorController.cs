﻿using System;
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
        public SqlConnection ORM;
        #endregion

        #region "CONSTRUCTOR"
        public MantenedorController()
        {
            ////IDENTIFICACION
            //FactoryPlanificacion = new PlanificacionFactory();
            JsonResponse = string.Empty;
            ORM = CapexInfraestructure.Utilities.Utils.Conectar();
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
                var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    Session["CAPEX_SESS_FILTRO_USR"] = "";
                    ViewBag.Usuarios = ORM.Query("CAPEX_SEL_MANTENEDOR_USUARIO_LISTAR", commandType: CommandType.StoredProcedure).ToList();
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
                var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    Session["CAPEX_SESS_FILTRO_USR"] = "ACTIVO";
                    ViewBag.Usuarios = ORM.Query("CAPEX_SEL_MANTENEDOR_USUARIO_LISTAR_SUGERIDO", new { @token = token }, commandType: CommandType.StoredProcedure).ToList();
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
                try
                {
                    var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                    var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                    if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                    {
                        return RedirectToAction("Logout", "Login");
                    }
                    else
                    {
                        var resultado = ORM.Query("CAPEX_SEL_MANTENEDOR_USUARIO_BUSCAR", new { @Termino = termino }, commandType: CommandType.StoredProcedure).ToList();
                        return Json(new { Resultado = resultado }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception err)
                {
                    var ExceptionResult = "UsuarioBuscar, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
                try
                {
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

                    ORM.Query("CAPEX_INS_MANTENEDOR_USUARIO", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
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
                    try
                    {

                    }
                    catch (Exception ex)
                    {
                        return Json(new { Mensaje = ex.Message.ToString() + "-----" + ex.StackTrace.ToString() }, JsonRequestBehavior.AllowGet);
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
        public ActionResult UsuarioModificarDatos(Usuario.ModificarUsuario DatosUsuario)
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                try
                {
                    var parametos = new DynamicParameters();
                    parametos.Add("ID", DatosUsuario.ID);
                    parametos.Add("UserName", DatosUsuario.UserName);
                    parametos.Add("Password", DatosUsuario.Password);
                    parametos.Add("Email", DatosUsuario.Email);
                    parametos.Add("Status", DatosUsuario.Status);

                    parametos.Add("UsuToken", DatosUsuario.UsuToken);
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

                    parametos.Add("GrvToken", DatosUsuario.GrvToken);
                    parametos.Add("GrvUser", DatosUsuario.GrvUser);
                    parametos.Add("GrvUserToken", DatosUsuario.GrvUserToken);
                    parametos.Add("GrvAreaRevToken", DatosUsuario.GrvAreaRevToken);
                    parametos.Add("GrvAreaRevNombre", DatosUsuario.GrvAreaRevNombre);

                    parametos.Add("UserRolId", DatosUsuario.UserRolID);
                    parametos.Add("UserID", DatosUsuario.UserID);
                    parametos.Add("RoleID", DatosUsuario.RoleID);

                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                    ORM.Query("CAPEX_UPD_MANTENEDOR_USUARIO", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
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
            try
            {
                ORM.Query("CAPEX_DEL_MANTENEDOR_USUARIO", new { UsuToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Eliminado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "UsuarioEliminar, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
            try
            {
                ORM.Query("CAPEX_UPD_MANTENEDOR_USUARIO_ESTADO", new { UsuToken, Status }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Activado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "UsuarioActivar, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
            try
            {
                ORM.Query("CAPEX_UPD_MANTENEDOR_USUARIO_ESTADO", new { UsuToken, Status }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Bloqueado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "UsuarioBloquear, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
                var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    Session["CAPEX_SESS_FILTRO_USR"] = "";
                    ViewBag.Usuarios = ORM.Query("CAPEX_SEL_MANTENEDOR_GERENCIA_LISTAR", commandType: CommandType.StoredProcedure).ToList();
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
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("GerNombre", Datos.GerNombre);
                parametos.Add("GerDescripcion", Datos.GerDescripcion);
                parametos.Add("GerEstado", Datos.GerEstado);
                parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                ORM.Query("CAPEX_INS_MANTENEDOR_GERENCIA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
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
        }

        [HttpGet]
        [Route("Gerencia/BuscarPorToken")]
        public ActionResult BuscarGerenciaPorToken(string GerToken)
        {
            GerToken = GerToken.Replace(System.Environment.NewLine, "");
            try
            {
                var resultado = ORM.Query("CAPEX_SEL_MANTENEDOR_BUSCAR_GERENCIA_TOKEN", new { GerToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = resultado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "BuscarGerenciaPorToken, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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

            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("GerToken", Datos.GerToken);
                parametos.Add("GerNombre", Datos.GerNombre);
                parametos.Add("GerDescripcion", Datos.GerDescripcion);
                parametos.Add("GerEstado", Datos.GerEstado);

                ORM.Query("CAPEX_UPD_MANTENEDOR_GERENCIA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Actualizado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "ActualizarArea, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
            try
            {
                ORM.Query("CAPEX_UPD_MANTENEDOR_DESACTIVAR_GERENCIA", new { GerToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Descativado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "DesactivarGerencia, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
                var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    Session["CAPEX_SESS_FILTRO_USR"] = "";
                    ViewBag.Usuarios = ORM.Query("CAPEX_SEL_MANTENEDOR_GERENCIA_LISTAR", commandType: CommandType.StoredProcedure).ToList();
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
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("GerNombre", Datos.GerNombre);
                parametos.Add("GerDescripcion", Datos.GerDescripcion);
                parametos.Add("GerEstado", Datos.GerEstado);
                parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                ORM.Query("CAPEX_INS_MANTENEDOR_GERENCIA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
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
        }

        [HttpGet]
        [Route("Superintendencia/BuscarPorToken")]
        public ActionResult BuscarSuperintendenciaPorToken(string GerToken)
        {
            GerToken = GerToken.Replace(System.Environment.NewLine, "");
            try
            {
                var resultado = ORM.Query("CAPEX_SEL_MANTENEDOR_BUSCAR_GERENCIA_TOKEN", new { GerToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = resultado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "BuscarSuperintendenciaPorToken, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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

            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("GerToken", Datos.GerToken);
                parametos.Add("GerNombre", Datos.GerNombre);
                parametos.Add("GerDescripcion", Datos.GerDescripcion);
                parametos.Add("GerEstado", Datos.GerEstado);

                ORM.Query("CAPEX_UPD_MANTENEDOR_GERENCIA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Actualizado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "ActualizarSuperintendencia, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
            try
            {
                ORM.Query("CAPEX_UPD_MANTENEDOR_DESACTIVAR_GERENCIA", new { GerToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Descativado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "DesactivarSuperintendencia, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
                var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    Session["CAPEX_SESS_FILTRO_USR"] = "";
                    ViewBag.Usuarios = ORM.Query("CAPEX_SEL_MANTENEDOR_GERENTE_LISTAR", commandType: CommandType.StoredProcedure).ToList();
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
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("GteNombre", Datos.GteNombre);
                parametos.Add("GteDescripcion", Datos.GteDescripcion);
                parametos.Add("GteEstado", Datos.GteEstado);
                parametos.Add("IdGerencia", Datos.IdGerencia);
                parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                ORM.Query("CAPEX_INS_MANTENEDOR_GERENTE", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
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
            try
            {
                var resultProcedure = ORM.Query("CAPEX_SEL_MANTENEDOR_BUSCAR_GERENTE_GERENCIA_ACTIVO", new { @GteToken = GteToken, IdGerencia = IdGerencia }, commandType: CommandType.StoredProcedure).SingleOrDefault();
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
            try
            {
                var resultProcedure = ORM.Query("CAPEX_SEL_MANTENEDOR_BUSCAR_GERENTE_ACTIVO", new { IdGerencia = Gerencia }, commandType: CommandType.StoredProcedure).SingleOrDefault();
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
        }

        [HttpGet]
        [Route("Gerente/BuscarPorToken")]
        public ActionResult BuscarGerentePorToken(string GteToken)
        {
            GteToken = GteToken.Replace(System.Environment.NewLine, "");
            try
            {
                var resultado = ORM.Query("CAPEX_SEL_MANTENEDOR_BUSCAR_GERENTE_TOKEN", new { GteToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = resultado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "BuscarPorToken, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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

            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("GteToken", Datos.GteToken);
                parametos.Add("GteNombre", Datos.GteNombre);
                parametos.Add("IdGerencia", Datos.IdGerencia);
                parametos.Add("GteDescripcion", Datos.GteDescripcion);
                parametos.Add("GteEstado", Datos.GteEstado);

                ORM.Query("CAPEX_UPD_MANTENEDOR_GERENTE", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Actualizado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "ActualizarGerente, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
            try
            {
                ORM.Query("CAPEX_UPD_MANTENEDOR_DESACTIVAR_GERENTE", new { GteToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Descativado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "DesactivarGerente, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
                var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    ViewBag.Gerencias = ORM.Query("CAPEX_SEL_GERENCIAS", commandType: CommandType.StoredProcedure).ToList();
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
                try
                {
                    var parametos = new DynamicParameters();
                    parametos.Add("IdGerencia", DatosGerente.IdGerencia);
                    parametos.Add("GteNombre", DatosGerente.GteNombre);
                    parametos.Add("GteDescripcion", DatosGerente.GteDescripcion);

                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                    ORM.Query("CAPEX_INS_MANTENEDOR_GERENTE", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
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
                try
                {
                    var parametos = new DynamicParameters();
                    parametos.Add("IdGerencia", DatosGerente.IdGerencia);
                    parametos.Add("GteToken", DatosGerente.GteToken);
                    parametos.Add("GteNombre", DatosGerente.GteNombre);
                    parametos.Add("GteDescripcion", DatosGerente.GteDescripcion);

                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                    ORM.Query("CAPEX_UPD_MANTENEDOR_GERENTE", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
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
            try
            {
                ORM.Query("CAPEX_UPD_MANTENEDOR_GERENTE_ESTADO", new { GteToken, Status }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Activado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "GerenteActivar, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
            try
            {
                ORM.Query("CAPEX_UPD_MANTENEDOR_GERENTE_ESTADO", new { GteToken, Status }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Bloqueado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "GerenteBloquear, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
                var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    Session["CAPEX_SESS_FILTRO_USR"] = "";
                    ViewBag.Usuarios = ORM.Query("CAPEX_SEL_MANTENEDOR_SUPERINTENDENTE_LISTAR", commandType: CommandType.StoredProcedure).ToList();
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
                var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    ViewBag.Superintendencias = ORM.Query("CAPEX_SEL_SUPERINTENDENCIAS", commandType: CommandType.StoredProcedure).ToList();
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
                try
                {
                    var parametos = new DynamicParameters();
                    parametos.Add("IdSuper", DatosSuperintendente.IdSuper);
                    parametos.Add("IntNombre", DatosSuperintendente.IntNombre);
                    parametos.Add("IntDescripcion", DatosSuperintendente.IntDescripcion);

                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                    ORM.Query("CAPEX_INS_MANTENEDOR_SUPERINTENDENTE", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
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
            try
            {
                ORM.Query("CAPEX_UPD_MANTENEDOR_SUPERINTENDENTE_ESTADO", new { IntToken, Status }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Activado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "SuperintendenteActivar, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
            try
            {
                ORM.Query("CAPEX_UPD_MANTENEDOR_SUPERINTENDENTE_ESTADO", new { IntToken, Status }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Bloqueado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "SuperintendenteBloquear, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
                var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    ViewBag.Categorias = ORM.Query("CAPEX_SEL_MANTENEDOR_CATEGORIAS", commandType: CommandType.StoredProcedure).ToList();
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
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("CatNombre", Datos.CatNombre);
                parametos.Add("CatDescripcion", Datos.CatDescripcion);
                parametos.Add("CatEstado", Datos.CatEstado);
                parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                ORM.Query("CAPEX_INS_MANTENEDOR_CATEGORIA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
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
        }
        [HttpGet]
        [Route("Categoria/BuscarPorToken")]
        public ActionResult BuscarPorToken(string CatToken)
        {
            CatToken = CatToken.Replace(System.Environment.NewLine, "");
            try
            {
                var resultado = ORM.Query("CAPEX_SEL_MANTENEDOR_BUSCAR_CATEGORIA_TOKEN", new { CatToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = resultado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "BuscarPorToken, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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

            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("CatToken", Datos.CatToken);
                parametos.Add("CatNombre", Datos.CatNombre);
                parametos.Add("CatDescripcion", Datos.CatDescripcion);
                parametos.Add("CatEstado", Datos.CatEstado);

                ORM.Query("CAPEX_UPD_MANTENEDOR_CATEGORIA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Actualizado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "ActualizarCategoria, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
            try
            {
                var Estado = 0;
                ORM.Query("CAPEX_UPD_MANTENEDOR_DESACTIVAR_CATEGORIA", new { CatToken, Estado }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Descativado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "DesactivarCategoria, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
                var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    ViewBag.Riesgos = ORM.Query("CAPEX_SEL_MANTENEDOR_RIESGOS", commandType: CommandType.StoredProcedure).ToList();
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
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("RiesgoNombre", Datos.RiesgoNombre);
                parametos.Add("EvrImpacto", Datos.EvrImpacto);
                parametos.Add("EvrProbabilidad", Datos.EvrProbabilidad);
                parametos.Add("RiesgoEstado", Datos.RiesgoEstado);
                parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                ORM.Query("CAPEX_INS_MANTENEDOR_RIESGO", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
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
        }

        [HttpGet]
        [Route("MatrizRiesgo/BuscarPorToken")]
        public ActionResult BuscarPorTokenMatrizRiesgo(string RiesgoToken)
        {
            RiesgoToken = RiesgoToken.Replace(System.Environment.NewLine, "");
            try
            {
                var resultado = ORM.Query("CAPEX_SEL_MANTENEDOR_BUSCAR_RIESGO_TOKEN", new { RiesgoToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = resultado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "BuscarPorTokenMatrizRiesgo, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("RiesgoToken", Datos.RiesgoToken);
                parametos.Add("RiesgoNombre", Datos.RiesgoNombre);
                parametos.Add("EvrImpacto", Datos.EvrImpacto);
                parametos.Add("EvrProbabilidad", Datos.EvrProbabilidad);
                parametos.Add("RiesgoEstado", Datos.RiesgoEstado);
                ORM.Query("CAPEX_UPD_MANTENEDOR_RIESGO", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Actualizado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "ActualizarMatrizRiesgo, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
            try
            {
                var RiesgoEstado = 0;
                ORM.Query("CAPEX_UPD_MANTENEDOR_DESACTIVAR_RIESGO", new { RiesgoToken, RiesgoEstado }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Descativado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "DesactivarMatrizRiesgo, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
                var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    ViewBag.Areas = ORM.Query("CAPEX_SEL_MANTENEODR_AREAS", commandType: CommandType.StoredProcedure).ToList();
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
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("AreaNombre", Datos.AreaNombre);
                parametos.Add("AreaAcronimo", Datos.AreaAcronimo);
                parametos.Add("AreaDescripcion", Datos.AreaDescripcion);
                parametos.Add("AreaEstado", Datos.AreaEstado);
                parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                ORM.Query("CAPEX_INS_MANTENEDOR_AREA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
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
        }

        [HttpGet]
        [Route("Area/BuscarPorToken")]
        public ActionResult BuscarAreaPorToken(string AreaToken)
        {
            AreaToken = AreaToken.Replace(System.Environment.NewLine, "");
            try
            {
                var resultado = ORM.Query("CAPEX_SEL_MANTENEDOR_BUSCAR_AREA_TOKEN", new { AreaToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = resultado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "BuscarAreaPorToken, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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

            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("AreaToken", Datos.AreaToken);
                parametos.Add("AreaNombre", Datos.AreaNombre);
                parametos.Add("AreaAcronimo", Datos.AreaAcronimo);
                parametos.Add("AreaDescripcion", Datos.AreaDescripcion);
                parametos.Add("AreaEstado", Datos.AreaEstado);

                ORM.Query("CAPEX_UPD_MANTENEDOR_AREA", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Actualizado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "ActualizarArea, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
            try
            {
                ORM.Query("CAPEX_UPD_MANTENEDOR_DESACTIVAR_AREA", new { AreaToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Descativado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "DesactivarArea, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
            try
            {
                ORM.Query("CAPEX_UPD_MANTENEDOR_ACTIVAR_AREA", new { AreaToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Activado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "ActivarArea, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
                var usuario = Convert.ToString(Session["CAPEX_SESS_USERNAME"]);
                var rol = Convert.ToString(Session["CAPEX_SESS_ROLNOMBRE"]);
                if (string.IsNullOrEmpty(Convert.ToString(Session["CAPEX_SESS_USERNAME"])) || string.IsNullOrEmpty(rol) || string.IsNullOrEmpty(usuario))
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    ViewBag.Clasificaciones = ORM.Query("CAPEX_SEL_MANTENEDOR_CLASIFICACIONES_SSO", commandType: CommandType.StoredProcedure).ToList();
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
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("CSNombre", Datos.CSNombre);
                parametos.Add("CSDescripcion", Datos.CSDescripcion);
                parametos.Add("CSEstado", Datos.CSEstado);
                parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                ORM.Query("CAPEX_INS_MANTENEDOR_CLASIFICACION_SSO", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
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
        }

        [HttpGet]
        [Route("ClasificacionSSO/BuscarPorToken")]
        public ActionResult BuscarClasificacionSSOPorToken(string CSToken)
        {
            CSToken = CSToken.Replace(System.Environment.NewLine, "");
            try
            {
                var resultado = ORM.Query("CAPEX_SEL_MANTENEDOR_BUSCAR_CLASIFICACION_SSO_TOKEN", new { CSToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = resultado }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "BuscarClasificacionSSOPorToken, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
            try
            {
                var parametos = new DynamicParameters();
                parametos.Add("CSToken", Datos.CSToken);
                parametos.Add("CSNombre", Datos.CSNombre);
                parametos.Add("CSDescripcion", Datos.CSDescripcion);
                parametos.Add("CSEstado", Datos.CSEstado);

                ORM.Query("CAPEX_UPD_MANTENEDOR_CLASIFICACION_SSO", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Actualizado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "ActualizarClasificacionSSO, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
            try
            {
                ORM.Query("CAPEX_UPD_MANTENEDOR_DESACTIVAR_CLASIFICACION_SSO", new { CSToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Descativado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "DesactivarClasificacionSSO, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
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
            try
            {
                ORM.Query("CAPEX_UPD_MANTENEDOR_ACTIVAR_CLASIFICACION_SSO", new { CSToken }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return Json(new { Mensaje = "Activado" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                var ExceptionResult = "ActivarClasificacionSSO, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}