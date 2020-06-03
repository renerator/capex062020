using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapexInfraestructure.Bll.Business.Gestion;

namespace CapexInfraestructure.Bll.Factory
{
    public class GestionFactory
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
        * RESPONABILIDAD   : PROVEER FABRICA DE OBJETOS REUTILIZABLES 
        * TIPO             : LOGICA DE NEGOCIO
        * DESARROLLADO POR : PMO360
        * FECHA            : 2018
        * VERSION          : 0.0.1
        * PROPOSITO        : ADMINISTRAR LOGICA DE NEGOCIO,CREACION DE OBJETOS
        * 
        * 
        */
        public enum tipo
        {
            //RESUMEN INICIATIVA
            ListarResumenIniciativas,
            VerDetalleIniciativa
            
        }

        public IGestion delega(tipo T)
        {
            switch (T)
            {
                ///RESUMEN INICIATIVA
                case tipo.ListarResumenIniciativas  : return new Business.Gestion.Gestion();
                case tipo.VerDetalleIniciativa      : return new Business.Gestion.Gestion();
                default: return new Business.Gestion.Gestion();
            }
        }
    }
}
