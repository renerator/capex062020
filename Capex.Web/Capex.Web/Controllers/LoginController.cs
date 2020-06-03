using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Capex.Web.App_Start;
using CapexIdentity.Business;
using CapexIdentity.Infraestructure;
using CapexIdentity.Entities;
using CapexIdentity.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin;
using System.Configuration;
using System.Globalization;
using Newtonsoft.Json;
using System.Web.Caching;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using CapexInfraestructure.Bll.Entities.Login;
using CapexInfraestructure.Bll.Business.Login;
using CapexInfraestructure.Bll.Factory;
using CapexInfraestructure.Utilities;
using System.Text;
using Utils = CapexInfraestructure.Utilities.Utils;

namespace Capex.Web.Controllers
{
    public class LoginController : Controller
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

        #region Init
        private static string aadInstance   = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenant        = ConfigurationManager.AppSettings["ida:Tenant"];
        private static string clientId      = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string authority     = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
        private static int Estado { get; set; }
        //
        // To authenticate to the To Do list service, the client needs to know the service's App ID URI.
        // To contact the To Do list service we need its URL as well.
        //
        private static string todoListResourceId = ConfigurationManager.AppSettings["todo:TodoListResourceId"];

        //public static AuthenticationContext authContext = null;
        #endregion

        #region "PROPIEDADES"
        private List<string> Listar { get; set; }
        private string JsonResponse { get; set; }
        private string ExceptionResult { get; set; }
        #endregion

        #region "CONSTANTES"
        //LOGIN
        private const LoginFactory.tipo DU = LoginFactory.tipo.ObtenerInformacionUsuario;
        #endregion

        #region "CAMPOS"
        //LOGIN
        public static LoginFactory FactoryLogin;
        public static ILogin ILogin;
        public SqlConnection ORM;
        #endregion

        #region "CONSTRUCTOR"
        public LoginController()
        {
            //LOGIN
            FactoryLogin = new LoginFactory();
            JsonResponse = string.Empty;
            ORM = CapexInfraestructure.Utilities.Utils.Conectar();
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            Session.Abandon();
            SignInManager.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Login");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objLogin"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        /// 
        
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(bool chkRemember, LoginInfo objLogin, string returnUrl)
        {
            bool estadoAdd = true;
            //var Estado = ActiveDirectoryAzure("EXT_PMO_JuanAviles", "pmo360");
            if (ModelState.IsValid)
            {
                //
                // PASO 1: BUSCAR EN REPOSITORIO DE LA COMPAÑIA
                //

                if (estadoAdd)
                {
                    //
                    // PASO 2: BUSCAR EN REPOSITORIO LOCAL
                    //         - SI EL USUARIO EXISTE EN ADD, PERO NO EN CAPEX, ENTONCES CREAR
                    //         - SI EL USUARIO EXISTE EN ADD Y EN CAPEX, ENTONCES PROCESAR
                    //
                    ApplicationUser oUser = await SignInManager.UserManager.FindByNameAsync(objLogin.UserName);
                    //
                    // PASO 2.1: CREACION DE CUENTA TEMPORAL
                    //
                    if (oUser==null)
                    {
                        return RedirectToLocal(returnUrl);
                    }
                    //
                    // PASO 2.2 : PROCESAR
                    //
                    else if (oUser != null && oUser.Password == objLogin.Password)
                    {
                        switch (oUser.Status)
                        {
                            case EnumUserStatus.Pending:
                                ModelState.AddModelError("", "Error: Su cuenta no está activada.");
                                break;
                            case EnumUserStatus.Active:

                                SignInManager.SignIn(oUser, false, false);
                                IList<string> roleList = UserRoleController.GetUserRoles(oUser.Id);
                                foreach (string role in roleList)
                                {
                                    UserManager.AddToRole(oUser.Id, role);
                                }

                                if (string.IsNullOrEmpty(returnUrl))
                                {
                                    Session["CAPEX_SESS_ES_ADM_CAPEX"] = "NO";
                                    if (chkRemember)
                                    {
                                        HttpCookie cookie = new HttpCookie("CAPEX_COOKIE_USERNAME");
                                        cookie.Values.Add("UserName", objLogin.UserName);
                                        cookie.Expires = DateTime.Now.AddDays(15);
                                        Response.Cookies.Add(cookie);
                                    }
                                    ILogin = FactoryLogin.delega(DU);
                                    var atributos = ILogin.ObtenerInformacionUsuario(objLogin.UserName);
                                    foreach (var at in atributos) {
                                        Session["CAPEX_SESS_USUTOKEN"]  = at.UsuToken.ToString();
                                        Session["CAPEX_SESS_COMTOKEN"]  = at.ComToken.ToString();
                                        Session["CAPEX_SESS_AREATOKEN"] = at.AreaToken.ToString();
                                        Session["CAPEX_SESS_IDEMPRESA"] = at.IdEmpresa.ToString();
                                        Session["CAPEX_SESS_USUID"]     = at.UsuId.ToString();
                                        Session["CAPEX_SESS_ROLNOMBRE"] = at.RolNombre.ToString();
                                        Session["CAPEX_SESS_USURUT"]    = at.UsuRut.ToString();
                                        Session["CAPEX_SESS_USUNOMBRE"] = at.UsuNombre.ToString();
                                        Session["CAPEX_SESS_USUAPELLIDO"] = at.UsuApellido.ToString();
                                        Session["CAPEX_SESS_USUEMAIL"]  = at.UsuEmail.ToString();
                                        Session["CAPEX_SESS_USERNAME"] = objLogin.UserName.ToString();

                                    }
                                    //
                                    // REGISTRAR ACCESO ADM CAPEX
                                    //
                                    string CapexAdminToken = ConfigurationManager.AppSettings.Get("CAPEX_ADMIN_TOKEN");
                                    if (Convert.ToString(Session["CAPEX_SESS_USUTOKEN"]) == CapexAdminToken) {
                                        var Solicitudes = NumeroSolicitudesPendientesAdm();
                                        Session["CAPEX_SESS_ES_ADM_CAPEX"] = "SI";
                                        Session["CAPEX_SESS_NUM_SOL_PEND_ADM"] = Solicitudes;
                                    }
                                    var Comentarios= NumeroComentarios(objLogin.UserName);
                                    Session["CAPEX_SESS_NUM_COMENTARIOS"] = Comentarios;
                                    return RedirectToAction("Index", "Panel");
                                }
                                return RedirectToLocal(returnUrl);

                            case EnumUserStatus.Banned:
                                ModelState.AddModelError("", "Error: Su cuenta se encuntra desabilitada.");
                                break;
                            case EnumUserStatus.LockedOut:
                                ModelState.AddModelError("", "Error: Su cuenta se encuentra bloqueada.");
                                break;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Error: Datos de acceso incorrectos.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Error: Datos de acceso incorrectos.");
                }
                
            }
            return View(objLogin);
        }
        /// <summary>
        /// METODO PARA RECUPERAR SOLICITUDES PENDIENTES
        /// </summary>
        /// <returns></returns>
        private string NumeroSolicitudesPendientesAdm()
        {
            var Cadena = new StringBuilder();
            string CapexAdminToken = ConfigurationManager.AppSettings.Get("CAPEX_ADMIN_TOKEN");
            if (Convert.ToString(Session["CAPEX_SESS_USUTOKEN"]) == CapexAdminToken)
            {
                try
                {
                    using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
                    {
                        objConnection.Open();
                        try
                        {
                            var cantidad = SqlMapper.Query(objConnection,"CAPEX_SEL_SOLICITUDES_ADMIN_CONTADOR", commandType: CommandType.StoredProcedure).ToList();
                            foreach (var c in cantidad)
                            {
                                Cadena.Append(c.Pendientes);
                            }
                            return Cadena.ToString();
                        }
                        catch (Exception err)
                        {
                            ExceptionResult = "NumeroSolicitudesPendientesAdm, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                            Utils.LogError(ExceptionResult);
                            return "0";
                        }
                        finally
                        {
                            objConnection.Close();
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    return "0";
                }
                finally
                {
                    Cadena = null;
                }
            }
            else
            {
                return "0";
            }
        }
        /// <summary>
        /// NETODO PARA OBTENER NUMERO COMENTARIOS
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        private string NumeroComentarios(string usuario)
        {
            var Cadena = new StringBuilder();
            try
            {
                using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
                {
                    objConnection.Open();
                    try
                    {
                        var cantidad = SqlMapper.Query(objConnection,"CAPEX_SEL_OBTENER_CANTIDAD_COMENTARIOS", new { @usuario = usuario }, commandType: CommandType.StoredProcedure).ToList();
                        foreach (var c in cantidad)
                        {
                            Cadena.Append(c.Cuantos);
                        }
                        return Cadena.ToString();
                    }
                    catch (Exception err)
                    {
                        ExceptionResult = "NumeroComentarios, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        Utils.LogError(ExceptionResult);
                        return "0";
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return "0";
            }
            finally
            {
                Cadena = null;
            }
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Login");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task ActiveDirectoryAzure(string username, string password)
        {
            #region Obtain token

            AuthenticationResult result = await TryFetchTokenSilently();

            if (result == null)
            {
                UserCredential uc = TextualPrompt(username, password);
                try
                {
                    var context = new AuthenticationContext(authority);
                    result = await context.AcquireTokenAsync(todoListResourceId, clientId, uc);
                    if (result.IdToken != null && result.UserInfo != null)
                    {
                        Estado = 0;
                    }

                }
                catch (Exception ee)
                {
                    Estado = 2;
                }
            }

            #endregion

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private static UserCredential TextualPrompt(string user, string password)
        {
            return new UserPasswordCredential(user, password);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static async Task<AuthenticationResult> TryFetchTokenSilently()
        {

            AuthenticationResult tokenResult = null;
            // first, try to get a token silently
            try
            {
                var context = new AuthenticationContext(authority);
                context.TokenCache.Clear();
                tokenResult = await context.AcquireTokenSilentAsync(todoListResourceId, clientId);

            }
            catch (AdalException adalException)
            {
                // There is no token in the cache; prompt the user to sign-in.
                if (adalException.ErrorCode == AdalError.FailedToAcquireTokenSilently
                    || adalException.ErrorCode == AdalError.InteractionRequired)
                {
                    return tokenResult;
                }

                // An unexpected error occurred.
                //ShowError(adalException);
            }

            return tokenResult;
        }
        /// <summary>
        /// METODO ACTUALIZAR CLAVE
        /// </summary>
        /// <param name="Clave1"></param>
        /// <param name="Clave2"></param>
        /// <returns></returns>
        public ActionResult ActualizarClave(string Clave1, string Clave2, string Usuario)
        {
            try
            {
                using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
                {
                    objConnection.Open();
                    try
                    {
                        var parametos = new DynamicParameters();
                        parametos.Add("Clave1", Clave1);
                        parametos.Add("Clave2", Clave2);
                        parametos.Add("Usuario", Usuario);
                        parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                        SqlMapper.Query(objConnection,"CAPEX_UPD_ACTUALIZAR_CLAVE", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                        return Json(new { Mensaje = "Actualizado" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception err)
                    {
                        ExceptionResult = "ActualizarClave, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        Utils.LogError(ExceptionResult);
                        return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
            catch (Exception err)
            {
                ExceptionResult = "ActualizarClave, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                return Json(new { Mensaje = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}