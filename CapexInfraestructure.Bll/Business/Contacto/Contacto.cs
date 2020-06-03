using System;
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
using CapexInfraestructure.Bll.Entities.Contacto;
using CapexInfraestructure.Utilities;


namespace CapexInfraestructure.Bll.Business.Contacto
{
    public class Contacto : IContacto
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
         * RESPONABILIDAD   : PROVEER OPERACIONES Y LOGICA DE NEGOCIO PARA EL MODULO DE CONTACTO
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
        public SqlConnection ORM;
        #endregion

        #region "CONSTRUCTOR"
        public Contacto()
        {
            AppModule = "Contacto";
        }
        #endregion

        #region "METODOS SOLICITUD"

        /// <remark>
        ///     
        ///     ----------------------------------------------------------------------------
        ///     GRUPO DE METODOS "SOLICITUD"
        ///     VERSION     0.0.1
        ///     ----------------------------------------------------------------------------
        ///     PROPOSITO
        ///     ----------------------------------------------------------------------------
        ///     IMPLEMENTAR ENTRADA Y SALIDA DE DATOS, ASI COMO OPERACIONES POR MEDIO
        ///     DE PATRON CREACIONAL FACTORY QUE ES PROVISTO POR CAPA DDD DE INFRAESTRUCTURA
        ///     
        /// </remark>

        public string GuardarSolicitud(ContactoAdministrador.NuevaSolicitud DatosSolicitud)
        {
            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                try
                {
                    var parametos = new DynamicParameters();
                    parametos.Add("SolTipo", DatosSolicitud.SolTipo);
                    parametos.Add("SolOtroTelefono", DatosSolicitud.SolOtroTelefono);
                    parametos.Add("SolComentario", DatosSolicitud.SolComentario);
                    parametos.Add("PidUsuario", DatosSolicitud.PidUsuario);
                    parametos.Add("Respuesta", dbType: System.Data.DbType.String, direction: System.Data.ParameterDirection.Output, size: 50);

                    SqlMapper.Query(objConnection, "CAPEX_INS_SOLICITUD_ADMINISTRADOR", parametos, commandType: CommandType.StoredProcedure).SingleOrDefault();
                    if (!string.IsNullOrEmpty(parametos.Get<string>("Respuesta")))
                    {
                        return parametos.Get<string>("Respuesta");
                    }
                    else
                    {
                        return "ERROR";
                    }
                }
                catch (Exception err)
                {
                    ExceptionResult = AppModule + " GuardarSolicitud, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                    Utils.LogError(ExceptionResult);

                    return null;
                }
                finally
                {
                    objConnection.Close();
                }
            }
        }

        #endregion "METODOS SOLICITUD"


    }
}
