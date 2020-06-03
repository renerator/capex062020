using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapexInfraestructure.Bll.Business.Estadistica;

namespace CapexInfraestructure.Bll.Factory.EstadisticaFactory
{
    public class EstadisticaFactory
    {
        /* ------------------------------------------------------------------------------------
        * 
        * PMO360
        * 
        * -----------------------------------------------------------------------------------
        * 
        * CLIENTE          : 
        * PRODUCTO         : CAPEX
        * RESPONABILIDAD   : PROVEER FABRICA DE OBJETOS REUTILIZABLES 
        * TIPO             : LOGICA DE NEGOCIO
        * DESARROLLADO POR : PMO360
        * FECHA            : 2018
        * VERSION          : 0.0.1
        * PROPOSITO        :CREACION DE OBJETOS
        * 
        * 
        */
        public enum tipo
        {
            //ESTADISTICA
            ObtenerDatosGrafico1,
            ObtenerDatosGrafico2,
            Otros

        }

        public IEstadistica delega(tipo T)
        {
            switch (T)
            {
                //IDENTIFICACION
                case tipo.ObtenerDatosGrafico1: return new Business.Estadistica.Estadistica();
                default: return new Business.Estadistica.Estadistica();
            }
        }
    }
}
