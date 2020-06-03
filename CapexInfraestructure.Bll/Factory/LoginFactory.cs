using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapexInfraestructure.Bll.Business.Login;

namespace CapexInfraestructure.Bll.Factory
{
    public class LoginFactory
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
            //LOGIN
            ObtenerInformacionUsuario
        }

        public ILogin delega(tipo T)
        {
            switch (T)
            {
                //LOGIN
                case tipo.ObtenerInformacionUsuario: return new Business.Login.Login();
                default: return new Business.Login.Login();
            }
        }



    }
}
