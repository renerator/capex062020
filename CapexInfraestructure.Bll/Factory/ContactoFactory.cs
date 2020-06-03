using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapexInfraestructure.Bll.Business.Contacto;

namespace CapexInfraestructure.Bll.Factory
{
    public class ContactoFactory
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
            //SOLICITUD
            GuardarSolicitud
        }

        public IContacto delega(tipo T)
        {
            switch (T)
            {
                //IDENTIFICACION
                case tipo.GuardarSolicitud: return new Business.Contacto.Contacto();
                default: return new Business.Contacto.Contacto();
            }
        }
    }
}
