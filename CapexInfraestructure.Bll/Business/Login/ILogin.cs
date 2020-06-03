﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapexInfraestructure.Bll.Entities.Login;

namespace CapexInfraestructure.Bll.Business.Login
{
    public interface ILogin
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

        // ---------------------------------------- LOGIN -------------------------------- //

        List<Usuario.InformacionUsuario> ObtenerInformacionUsuario(string NombreUsuario);
    }
}