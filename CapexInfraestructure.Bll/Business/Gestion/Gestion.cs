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
using CapexInfraestructure.Bll.Entities.Gestion;
using CapexInfraestructure.Utilities;


namespace CapexInfraestructure.Bll.Business.Gestion
{
    public class Gestion : IGestion
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
         * RESPONABILIDAD   : PROVEER OPERACIONES Y LOGICA DE NEGOCIO PARA EL MODULO DE GESTION
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
        public Gestion()
        {
            AppModule = "Gestion";
        }
        #endregion

        #region "METODOS RESUMEN INICIATIVA"

        /// <remark>
        ///     
        ///     ----------------------------------------------------------------------------
        ///     GRUPO DE METODOS "GESTION"
        ///     VERSION     0.0.1
        ///     ----------------------------------------------------------------------------
        ///     PROPOSITO
        ///     ----------------------------------------------------------------------------
        ///     IMPLEMENTAR ENTRADA Y SALIDA DE DATOS, ASI COMO OPERACIONES POR MEDIO
        ///     DE PATRON CREACIONAL FACTORY QUE ES PROVISTO POR CAPA DDD DE INFRAESTRUCTURA
        ///     
        /// </remark>

       

        #endregion


    }
}
