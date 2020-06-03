using CapexIdentity.Infraestructure;
using CapexIdentity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexIdentity.Business
{
    public static class UserController
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
         * RESPONABILIDAD   : IDENTIDAD & ROL MANAGEMENT
         * TIPO             : LOGICA DE NEGOCIO
         * DESARROLLADO POR : PMO360
         * FECHA            : 2018
         * VERSION          : 0.0.1
         * PROPOSITO        : CLASE CONTROLADORA
         * 
         * 
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objUser"></param>
        /// <returns></returns>
        public static int NewUser(ApplicationUser objUser)
        {
            List<ParameterInfo> parameters = new List<ParameterInfo>();
            parameters.Add(new ParameterInfo() { ParameterName = "Id", ParameterValue = objUser.Id });
            parameters.Add(new ParameterInfo() { ParameterName = "UserName", ParameterValue = objUser.UserName });
            parameters.Add(new ParameterInfo() { ParameterName = "Email", ParameterValue = objUser.Email });
            parameters.Add(new ParameterInfo() { ParameterName = "Password", ParameterValue = objUser.Password });
            parameters.Add(new ParameterInfo() { ParameterName = "Status", ParameterValue = objUser.Status });
            int success = Operation.ExecuteQuery("CAPEX_INS_NEWUSER", parameters);
            return success;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objUser"></param>
        /// <returns></returns>
        public static int DeleteUser(ApplicationUser objUser)
        {
            List<ParameterInfo> parameters = new List<ParameterInfo>();
            parameters.Add(new ParameterInfo() { ParameterName = "Id", ParameterValue = objUser.Id });
            int success = Operation.ExecuteQuery("CAPEX_DEL_DELETEUSER", parameters);
            return success;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ApplicationUser GetUser(string id)
        {
            List<ParameterInfo> parameters = new List<ParameterInfo>();
            parameters.Add(new ParameterInfo() { ParameterName = "Id", ParameterValue = id });
            ApplicationUser oUser = Operation.GetRecord<ApplicationUser>("CAPEX_SEL_GETUSER", parameters);
            return oUser;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static ApplicationUser GetUserByUsername(string userName)
        {
            List<ParameterInfo> parameters = new List<ParameterInfo>();
            parameters.Add(new ParameterInfo() { ParameterName = "Username", ParameterValue = userName });
            ApplicationUser oUser = Operation.GetRecord<ApplicationUser>("CAPEX_SEL_GETUSERBYUSERNAME", parameters);
            return oUser;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objUser"></param>
        /// <returns></returns>
        public static int UpdateUser(ApplicationUser objUser)
        {
            List<ParameterInfo> parameters = new List<ParameterInfo>();
            parameters.Add(new ParameterInfo() { ParameterName = "Email", ParameterValue = objUser.Email });
            int success = Operation.ExecuteQuery("CAPEX_UPD_UPDATEUSER", parameters);
            return success;
        }
    }
}
