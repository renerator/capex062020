using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapexInfraestructure.Bll.Entities.Contacto;

namespace CapexInfraestructure.Bll.Business.Contacto
{
    public interface IContacto
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
        * RESPONABILIDAD   : PROVEER METODOS INTERFAZ DE IMPLEMENTACION
        * TIPO             : LOGICA DE NEGOCIO
        * DESARROLLADO POR : PMO360
        * FECHA            : 2018
        * VERSION          : 0.0.1
        * PROPOSITO        : SEGURIDAD 
        * 
        * 
        */

        // --------------------------- SOLICITUD ---------------------------- //
        string GuardarSolicitud(ContactoAdministrador.NuevaSolicitud DatosSolicitud);

    }
}
