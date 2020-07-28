using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapexInfraestructure.Bll.Entities.Contacto;
using CapexInfraestructure.Bll.Business.Contacto;
using CapexInfraestructure.Bll.Factory;
using Newtonsoft.Json;
using System.Web.Caching;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Configuration;

namespace Capex.Web.Controllers
{
    [AuthorizeAdminOrMember]
    [RoutePrefix("Solicitud")]
    public class SolicitudController : Controller
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
        ///     CONTROLADOR "SolicitudesController" 
        ///     VERSION     0.0.1
        ///     ----------------------------------------------------------------------------
        ///     PROPOSITO
        ///     ----------------------------------------------------------------------------
        ///     IMPLEMENTAR ENTRADA Y SALIDA DE DATOS, ASI COMO OPERACIONES POR MEDIO
        ///     DE PATRON MVC Y NO DDD
        ///     
        /// </remark>
        /// 


        #region "PROPIEDADES"
        private List<string> Listar { get; set; }
        private string JsonResponse { get; set; }
        #endregion

        #region "CAMPOS"
        //public SqlConnection ORM;
        #endregion

        #region "CONSTRUCTOR"
        public SolicitudController()
        {
            JsonResponse = string.Empty;
            //ORM = CapexInfraestructure.Utilities.Utils.Conectar();
        }
        #endregion

        #region "CONSTRUCTOR"
        [Route]
        public ActionResult Index()
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
                        ViewBag.Solicitudes = SqlMapper.Query(objConnection, "CAPEX_SEL_SOLICITUDES_ADMIN_LISTAR", commandType: CommandType.StoredProcedure).ToList();
                    }
                    catch (Exception err)
                    {
                        var ExceptionResult = "SolicitudController Index, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                        CapexInfraestructure.Utilities.Utils.LogError(ExceptionResult);
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }
            }
            return View();
        }
        #endregion
    }
}