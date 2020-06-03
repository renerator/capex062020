using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CapexIdentity.Utilities
{
    public class Utils
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
         * RESPONABILIDAD   : 
         * TIPO             : LOGICA DE NEGOCIO
         * DESARROLLADO POR : PMO360
         * FECHA            : 2018
         * VERSION          : 0.0.1
         * PROPOSITO        : 
         * 
         * 
         */
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static String ConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["CapexRepository"].ConnectionString;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IfUserAuthenticated()
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public static bool IfUserInRole(string roleName)
        {
            if (IfUserAuthenticated())
            {
                if (HttpContext.Current.User.IsInRole(roleName))
                {
                    return true;
                }
            }
            return false;
        }
    }
}