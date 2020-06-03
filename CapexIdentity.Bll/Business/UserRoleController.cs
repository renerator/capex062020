using CapexIdentity.Infraestructure;
using CapexIdentity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexIdentity.Business
{
    public static class UserRoleController
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
        /// <param name="userID"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public static int NewUserRole(string userID, string roleName)
        {
            List<ParameterInfo> parameters = new List<ParameterInfo>();
            parameters.Add(new ParameterInfo() { ParameterName = "UserID", ParameterValue = userID });
            parameters.Add(new ParameterInfo() { ParameterName = "RoleName", ParameterValue = roleName });
            int success = Operation.ExecuteQuery("CAPEX_INS_NEWUSERROLE", parameters);
            return success;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public static int DeleteUserRole(string userID, string roleName)
        {
            List<ParameterInfo> parameters = new List<ParameterInfo>();
            parameters.Add(new ParameterInfo() { ParameterName = "UserID", ParameterValue = userID });
            parameters.Add(new ParameterInfo() { ParameterName = "RoleName", ParameterValue = roleName });
            int success = Operation.ExecuteQuery("CAPEX_DEL_DELETEUSERROLE", parameters);
            return success;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static IList<string> GetUserRoles(string userID)
        {
            List<ParameterInfo> parameters = new List<ParameterInfo>();
            parameters.Add(new ParameterInfo() { ParameterName = "UserID", ParameterValue = userID });
            IList<string> roles = Operation.GetRecords<string>("CAPEX_SEL_GETUSERROLES", parameters);
            return roles;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static UserInfo GetUserByUsername(string userName)
        {
            List<ParameterInfo> parameters = new List<ParameterInfo>();
            parameters.Add(new ParameterInfo() { ParameterName = "Username", ParameterValue = userName });
            UserInfo oUser = Operation.GetRecord<UserInfo>("CAPEX_SEL_GETUSERBYUSERNAME", parameters);
            return oUser;
        }
    }
}
