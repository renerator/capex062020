using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapexInfraestructure.Bll.Business.Documentacion;


namespace CapexInfraestructure.Bll.Factory
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
    public class DocumentacionFactory
    {
        public enum tipo
        {
            //VISTA PRINCIPAL
            ListarCategorias,
            ListarDocumentos,
            AgregarCategoria,
            AgregarDocumento,
            ModificarDocumento,
            EliminarDocumento,
            ModificarCategoria
        }

        public IDocumentacion delega(tipo T)
        {
            switch (T)
            {
                //IDENTIFICACION
                case tipo.ListarCategorias: return new Business.Documentacion.Documentacion();
                case tipo.ListarDocumentos: return new Business.Documentacion.Documentacion();
                case tipo.AgregarCategoria: return new Business.Documentacion.Documentacion();
                case tipo.AgregarDocumento: return new Business.Documentacion.Documentacion();
                case tipo.ModificarDocumento: return new Business.Documentacion.Documentacion();
                case tipo.EliminarDocumento: return new Business.Documentacion.Documentacion();
                case tipo.ModificarCategoria: return new Business.Documentacion.Documentacion();

                default: return new Business.Documentacion.Documentacion();
            }
        }
    }
}
