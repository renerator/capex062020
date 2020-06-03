using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexIdentity.Utilities
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
     * PROPOSITO        : 
     * 
     * 
     */
    public enum EnumUserStatus
    {
        Pending = 0,
        Active,
        LockedOut,
        Closed,
        Banned
    }
}
