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
    [RoutePrefix("Contacto")]
    public class ContactoController : Controller
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
        ///     CONTROLADOR "ContactoController" 
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
        //SOLICITUD
        private const ContactoFactory.tipo SO = ContactoFactory.tipo.GuardarSolicitud;
        #endregion

        #region "CAMPOS"
        //SOLICITUD
        public static ContactoFactory FactoryContacto;
        public static IContacto IContacto;
        public SqlConnection ORM;
        #endregion

        #region "CONSTRUCTOR"
        public ContactoController()
        {
            //SOLICITUD
            FactoryContacto = new ContactoFactory();
            JsonResponse = string.Empty;
            ORM = CapexInfraestructure.Utilities.Utils.Conectar();
        }
        #endregion
        #region "METODOS CONTACTO"
        public ActionResult Index()
        {
            if (!@User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                string usuario = @User.Identity.Name;
                ViewBag.Mensajes = ORM.Query("CAPEX_SEL_OBTENER_COMENTARIOS", new { @usuario = usuario }, commandType: CommandType.StoredProcedure).ToList();
            }
            return View();
        }
        /// <summary>
        /// METODO GUARDAR SOLICITUD
        /// </summary>
        /// <param name="DatosSolicitud"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GuardarSolicitud(ContactoAdministrador.NuevaSolicitud DatosSolicitud)
        {
            try
            {
                IContacto = FactoryContacto.delega(SO);
                JsonResponse = JsonConvert.SerializeObject(IContacto.GuardarSolicitud(DatosSolicitud), Formatting.None);
                return Json(
                    JsonResponse,
                    JsonRequestBehavior.AllowGet
                );
               
            }
            catch (Exception exc)
            {
                return Redirect("Login");
            }
            finally
            {
                FactoryContacto = null;
                IContacto = null;
            }
        }

        #endregion
    }
}