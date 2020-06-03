using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CapexIdentity.Utilities;
using CapexInfraestructure.Bll.Entities.Estadistica;
using CapexInfraestructure.Bll.Business.Estadistica;
using CapexInfraestructure.Bll.Factory.EstadisticaFactory;
using CapexInfraestructure.Utilities;
using Newtonsoft.Json;
using System.Web.Caching;
using System.IO;
using static System.Net.WebRequestMethods;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Text;
using static CapexInfraestructure.Bll.Entities.Estadistica.EstadisticaModel;

namespace Capex.Web.Controllers
{
    [AuthorizeAdminOrMember]
    [RoutePrefix("Estadistica")]
    public class EstadisticaController : Controller
    {
        /* ------------------------------------------------------------------------------------
         * 
         * PMO360
         * 
         * -----------------------------------------------------------------------------------
         * 
         * CLIENTE          :
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
        ///     CONTROLADOR "EstadisticaController" 
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
        public string Excepcion { get; set; }
        #endregion

        #region "CONSTANTES"
        //ESTADISTICA
        private const EstadisticaFactory.tipo E = EstadisticaFactory.tipo.ObtenerDatosGrafico1;

        #endregion

        #region "CAMPOS"
        //ESTADISTICA
        public static EstadisticaFactory FactoryEstadistica;
        public static IEstadistica IEstadistica;
        public SqlConnection ORM;
        #endregion

        #region "CONSTRUCTOR"
        public EstadisticaController()
        {
            //ESTADISTICA
            FactoryEstadistica = new EstadisticaFactory();
            JsonResponse = string.Empty;
            ORM = CapexInfraestructure.Utilities.Utils.Conectar();
        }
        #endregion

        #region "METODOS ESTADISTICA"
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
                else
                {
                    ViewBag.Mensajes = ORM.Query("CAPEX_SEL_OBTENER_COMENTARIOS", new { @usuario = usuario }, commandType: CommandType.StoredProcedure).ToList();
                }
            }
            return View();
        }

        /// <remark>
        ///     
        ///     ----------------------------------------------------------------------------
        ///     GRUPO DE METODOS "Estadistica"
        ///     VERSION     0.0.1
        ///     ----------------------------------------------------------------------------
        ///     PROPOSITO
        ///     ----------------------------------------------------------------------------
        ///     IMPLEMENTAR ENTRADA Y SALIDA DE DATOS, ASI COMO OPERACIONES POR MEDIO
        ///     DE PATRON CREACIONAL FACTORY QUE ES PROVISTO POR CAPA DDD DE INFRAESTRUCTURA
        ///     
        /// </remark>
        /// 

        /// <summary>
        /// OBTENER DATOS GRAFICO 1
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public JsonResult ObtenerDatosGrafico1(FiltroEstadistica.Grafico1 filtro)
        {
            try
            {
                IEstadistica = FactoryEstadistica.delega(E);
                var json = IEstadistica.ObtenerDatosGrafico1(filtro);
                return Json(
                    json,
                    JsonRequestBehavior.AllowGet
                    );

            }
            catch (Exception exc)
            {
                Excepcion = "Módulo Estadística - Obtener Datos Grafico 1 , Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                return null;
            }
            finally
            {
                FactoryEstadistica = null;
                IEstadistica = null;
            }
        }


        /// <summary>
        /// OBTENER DATOS GRAFICO 2
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public JsonResult ObtenerDatosGrafico2(FiltroEstadistica.Grafico2 filtro)
        {
            try
            {
                IEstadistica = FactoryEstadistica.delega(E);
                var json = IEstadistica.ObtenerDatosGrafico2(filtro);
                return Json(
                    json,
                    JsonRequestBehavior.AllowGet
                    );

            }
            catch (Exception exc)
            {
                Excepcion = "Módulo Estadística - Obtener Datos Grafico 2 , Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                return null;
            }
            finally
            {
                FactoryEstadistica = null;
                IEstadistica = null;
            }
        }


        /// <summary>
        /// OBTENER DATOS GRAFICO 3
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public JsonResult ObtenerDatosGrafico3(FiltroEstadistica.Grafico3 filtro)
        {
            try
            {
                IEstadistica = FactoryEstadistica.delega(E);
                var json = IEstadistica.ObtenerDatosGrafico3(filtro);
                return Json(
                    json,
                    JsonRequestBehavior.AllowGet
                    );

            }
            catch (Exception exc)
            {
                Excepcion = "Módulo Estadística - Obtener Datos Grafico 3 , Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                return null;
            }
            finally
            {
                FactoryEstadistica = null;
                IEstadistica = null;
            }
        }

        /// <summary>
        /// OBTENER DATOS GRAFICO 3
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public JsonResult ObtenerDatosGrafico3Final(FiltroEstadistica.Grafico3 filtro)
        {
            try
            {
                IEstadistica = FactoryEstadistica.delega(E);
                var json = IEstadistica.ObtenerDatosGrafico3Final(filtro);
                return Json(
                    json,
                    JsonRequestBehavior.AllowGet
                    );

            }
            catch (Exception exc)
            {
                Excepcion = "Módulo Estadística - Obtener Datos Grafico 3 Final, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                return null;
            }
            finally
            {
                FactoryEstadistica = null;
                IEstadistica = null;
            }
        }


        /// <summary>
        /// OBTENER DATOS GRAFICO 4 - ESTADOS
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public JsonResult ObtenerDatosGrafico4(FiltroEstadistica.Grafico4Resumen filtro)
        {
            try
            {
                IEstadistica = FactoryEstadistica.delega(E);
                var json = IEstadistica.ObtenerDatosGrafico4(filtro);
                return Json(
                    json,
                    JsonRequestBehavior.AllowGet
                    );

            }
            catch (Exception exc)
            {
                Excepcion = "Módulo Estadística - Obtener Datos Grafico 4 - Estado, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                return null;
            }
            finally
            {
                FactoryEstadistica = null;
                IEstadistica = null;
            }
        }


        /// <summary>
        /// OBTENER DATOS GRAFICO 4 - CATEGORIAS
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public JsonResult ObtenerDatosGrafico4_Categoria(FiltroEstadistica.Grafico4Resumen filtro)
        {
            try
            {
                IEstadistica = FactoryEstadistica.delega(E);
                var json = IEstadistica.ObtenerDatosGrafico4_Categoria(filtro);
                return Json(
                    json,
                    JsonRequestBehavior.AllowGet
                    );

            }
            catch (Exception exc)
            {
                Excepcion = "Módulo Estadística - Obtener Datos Grafico 4 - Categoria , Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                return null;
            }
            finally
            {
                FactoryEstadistica = null;
                IEstadistica = null;
            }
        }

        /// <summary>
        /// OBTENER DATOS GRAFICO 4 - CATEGORIAS
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public JsonResult ObtenerDatosGraficoValorEstimadoBase(string token)
        {
            try
            {
                IEstadistica = FactoryEstadistica.delega(E);
                var json = IEstadistica.ObtenerDatosGraficoValorEstimadoBase(token);
                return Json(
                    json,
                    JsonRequestBehavior.AllowGet
                    );

            }
            catch (Exception exc)
            {
                Excepcion = "Módulo Estadística - Obtener Datos Grafico Valor Estimado Base , Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                return null;
            }
            finally
            {
                FactoryEstadistica = null;
                IEstadistica = null;
            }
        }

        /// <summary>
        /// OBTENER DATOS GRAFICO VALOR INGENIERIA
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public JsonResult ObtenerDatosGraficoValorIngenieria(string token)
        {
            try
            {
                IEstadistica = FactoryEstadistica.delega(E);
                var json = IEstadistica.ObtenerDatosGraficoValorIngenieria(token);
                return Json(
                    json,
                    JsonRequestBehavior.AllowGet
                    );

            }
            catch (Exception exc)
            {
                Excepcion = "Módulo Estadística - Obtener Datos Grafico Valor Ingenieria , Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                return null;
            }
            finally
            {
                FactoryEstadistica = null;
                IEstadistica = null;
            }
        }

        /// <summary>
        /// OBTENER DATOS GRAFICO VALOR ADQUISICIONES
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public JsonResult ObtenerDatosGraficoValorAdquisiciones(string token)
        {
            try
            {
                IEstadistica = FactoryEstadistica.delega(E);
                var json = IEstadistica.ObtenerDatosGraficoValorAdquisiciones(token);
                return Json(
                    json,
                    JsonRequestBehavior.AllowGet
                    );

            }
            catch (Exception exc)
            {
                Excepcion = "Módulo Estadística - Obtener Datos Grafico Valor Adquisiciones , Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                return null;
            }
            finally
            {
                FactoryEstadistica = null;
                IEstadistica = null;
            }
        }

        /// <summary>
        /// OBTENER DATOS GRAFICO VALOR ADQUISICIONES
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public JsonResult ObtenerDatosGraficoValorConstruccion(string token)
        {
            try
            {
                IEstadistica = FactoryEstadistica.delega(E);
                var json = IEstadistica.ObtenerDatosGraficoValorConstruccion(token);
                return Json(
                    json,
                    JsonRequestBehavior.AllowGet
                    );

            }
            catch (Exception exc)
            {
                Excepcion = "Módulo Estadística - Obtener Datos Grafico Valor Construccion , Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                return null;
            }
            finally
            {
                FactoryEstadistica = null;
                IEstadistica = null;
            }
        }

        /// <summary>
        /// OBTENER DATOS AREA CLIENTE
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public JsonResult ListarAreaCliente(string token)
        {
            try
            {
                IEstadistica = FactoryEstadistica.delega(E);
                string json = JsonConvert.SerializeObject(IEstadistica.ListarAreaCliente(token), Formatting.None);
                return Json(
                    json,
                    JsonRequestBehavior.AllowGet
                    );


            }
            catch (Exception err)
            {
                Excepcion = "Módulo Estadística - Listar Area Cliente, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                return null;
            }
        }

        /// <summary>
        /// OBTENER DATOS AÑO EJERCICIO
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public JsonResult ListarAnnEjercicio(string token)
        {
            try
            {
                IEstadistica = FactoryEstadistica.delega(E);
                string json = JsonConvert.SerializeObject(IEstadistica.ListarAnnEjercicio(token), Formatting.None);
                return Json(
                    json,
                    JsonRequestBehavior.AllowGet
                    );


            }
            catch (Exception err)
            {
                Excepcion = "Módulo Estadística - Listar Año Ejercicio, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                return null;
            }
        }


        /// <summary>
        /// OBTENER DATOS ETAPAS
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public JsonResult ListarEtapas(string token)
        {
            try
            {
                IEstadistica = FactoryEstadistica.delega(E);
                string json = JsonConvert.SerializeObject(IEstadistica.ListarEtapas(token), Formatting.None);
                return Json(
                    json,
                    JsonRequestBehavior.AllowGet
                    );


            }
            catch (Exception err)
            {
                Excepcion = "Módulo Estadística - Listar Etapas, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                return null;
            }
        }
        #endregion


        /// <summary>
        /// OBTENER CLASIFICACION SSO
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public JsonResult ListarSSO(string token)
        {
            try
            {
                IEstadistica = FactoryEstadistica.delega(E);
                string json = JsonConvert.SerializeObject(IEstadistica.ListarSSO(token), Formatting.None);
                return Json(
                    json,
                    JsonRequestBehavior.AllowGet
                    );


            }
            catch (Exception err)
            {
                Excepcion = "Módulo Estadística - Listar SSO, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                return null;
            }
        }




        /// <summary>
        /// OBTENER ESTANDAR SEGURIDAD
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public JsonResult ListarEstandarSeguridad(string token)
        {
            try
            {
                IEstadistica = FactoryEstadistica.delega(E);
                string json = JsonConvert.SerializeObject(IEstadistica.ListarEstandarSeguridad(token), Formatting.None);
                return Json(
                    json,
                    JsonRequestBehavior.AllowGet
                    );


            }
            catch (Exception err)
            {
                Excepcion = "Módulo Estadística - Listar Estandar Seguridad, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                return null;
            }
        }



        /// <summary>
        /// OBTENER CATEGORIAS
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public JsonResult ListarCategorias(string token)
        {
            try
            {
                IEstadistica = FactoryEstadistica.delega(E);
                string json = JsonConvert.SerializeObject(IEstadistica.ListarCategorias(token), Formatting.None);
                return Json(
                    json,
                    JsonRequestBehavior.AllowGet
                    );


            }
            catch (Exception err)
            {
                Excepcion = "Módulo Estadística - Listar Categorias, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                return null;
            }
        }



        /// <summary>
        /// OBTENER ESTADO INICIATIVA
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public JsonResult ListarEstadoIniciativa(string token)
        {
            try
            {
                IEstadistica = FactoryEstadistica.delega(E);
                string json = JsonConvert.SerializeObject(IEstadistica.ListarEstadoIniciativa(token), Formatting.None);
                return Json(
                    json,
                    JsonRequestBehavior.AllowGet
                    );


            }
            catch (Exception err)
            {
                Excepcion = "Módulo Estadística - Listar Estados Iniciativa, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                return null;
            }
        }



        /// <summary>
        /// OBTENER AREA EJECUTORA
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public JsonResult ListarAreaEjecutora(string token)
        {
            try
            {
                IEstadistica = FactoryEstadistica.delega(E);
                string json = JsonConvert.SerializeObject(IEstadistica.ListarAreaEjecutora(token), Formatting.None);
                return Json(
                    json,
                    JsonRequestBehavior.AllowGet
                    );


            }
            catch (Exception err)
            {
                Excepcion = "Módulo Estadística - Listar Area Ejecutora, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                return null;
            }
        }

    }
}
