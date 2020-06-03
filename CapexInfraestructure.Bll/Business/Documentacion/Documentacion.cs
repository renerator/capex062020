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
using CapexInfraestructure.Bll.Entities.Documentacion;
using CapexInfraestructure.Utilities;
using ClosedXML.Excel;

namespace CapexInfraestructure.Bll.Business.Documentacion
{
    public class Documentacion : IDocumentacion
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
        public SqlConnection ORM;
        #endregion

        #region "CONSTRUCTOR"
        public Documentacion()
        {
            AppModule = "Planificación";
            ORM = Utils.Conectar();
        }
        #endregion

        #region "METODOS VISTA PRINCIPAL"
        public List<Principal.Categorias> ListarCategorias()
        {
            return null;
        }
        #endregion
    }

}
