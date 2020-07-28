using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data.Common;
using Dapper;
using System.Text;
using System.Threading.Tasks;
using CapexInfraestructure.Bll.Entities.Login;
using CapexInfraestructure.Utilities;
using ClosedXML.Excel;

namespace CapexInfraestructure.Bll.Business.Login
{
    public class Login : ILogin
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
         * RESPONABILIDAD   : PROVEER OPERACIONES Y LOGICA DE NEGOCIO PARA EL MODULO DE EJERCICIO DE PLANIFICACION
         * TIPO             : LOGICA DE NEGOCIO
         * DESARROLLADO POR : PMO360
         * FECHA            : 2018
         * VERSION          : 0.0.1
         * PROPOSITO        : WRAPPER DE OPERACIONES A LA BASE DE DATOS /REPOSITORIO
         * 
         * 
        */
        #region "PROPIEDADES"
        public string ExceptionResult { get; set; }
        public string AppModule { get; set; }
        #endregion

        #region "GLOBALS"
        //public SqlConnection ORM;
        #endregion

        #region "CONSTRUCTOR"
        public Login()
        {
            AppModule = "Login";
            //ORM = Utils.Conectar();
        }
        #endregion

        #region "METODOS LOGIN"
        /// <summary>
        /// OBTENER DETALLE DE INFORMACION DE USUARIO
        /// </summary>
        /// <param name="NombreUsuario"></param>
        /// <returns></returns>
        public List<Usuario.InformacionUsuario> ObtenerInformacionUsuario(string NombreUsuario)
        {
            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                try
                {
                    objConnection.Open();
                    return SqlMapper.Query<Usuario.InformacionUsuario>(objConnection, "CAPEX_SEL_INFORMACION_USUARIO", new { NombreUsuario }, commandType: CommandType.StoredProcedure).ToList();
                }
                catch (Exception err)
                {
                    ExceptionResult = AppModule + "ObtenerInformacionUsuario, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    Utils.LogError(ExceptionResult);
                    return null;
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
