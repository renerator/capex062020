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
using static System.Net.WebRequestMethods;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Text;
using Utils = CapexInfraestructure.Utilities.Utils;

namespace Capex.Web.Controllers
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
    ///     CONTROLADOR "PanelController" 
    ///     VERSION     0.0.1
    ///     ----------------------------------------------------------------------------
    ///     PROPOSITO
    ///     ----------------------------------------------------------------------------
    ///     IMPLEMENTAR ENTRADA Y SALIDA DE DATOS, ASI COMO OPERACIONES POR MEDIO
    ///     DE PATRON MVC
    ///     
    /// </remark>

    public class AuthorizeAdminOrMember : AuthorizeAttribute
    {
        /// <summary>
        /// METODO CONTROLADR DE LISTADO DE ROLES
        /// </summary>
        public AuthorizeAdminOrMember()
        {
            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var Cadena = new StringBuilder();
                    var ListadoRoles = SqlMapper.Query(objConnection,"CAPEX_SEL_ROLE", commandType: CommandType.StoredProcedure).ToList();
                    foreach (var Rol in ListadoRoles)
                    {
                        Cadena.Append(Rol.Name + ",");
                    }
                    Roles = Cadena.ToString();
                    Cadena = null;
                }
                catch (Exception err)
                {
                    Utils.LogError("AuthorizeAdminOrMember, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString());
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }
    }
    /// <summary>
    /// METODO DE SALIDA /FIN DE SESSION
    /// </summary>
    [AuthorizeAdminOrMember]
    [RoutePrefix("Panel")]
    public class PanelController : Controller
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
            */

        /// <summary>
        /// METODO CONTROL DE ROLES PERMITIDOS
        /// </summary>
        /// 

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
        public PanelController()
        {
            ////IDENTIFICACION
            JsonResponse = string.Empty;
            ORM = CapexInfraestructure.Utilities.Utils.Conectar();
        }
        #endregion

        #region "METODOS PANEL"
        public ActionResult Index()
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            return View();
        }
        #endregion
    }
}