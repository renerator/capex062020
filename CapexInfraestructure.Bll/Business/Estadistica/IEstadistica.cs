using CapexInfraestructure.Bll.Entities.Estadistica;
using System.Collections.Generic;
using static CapexInfraestructure.Bll.Entities.Estadistica.EstadisticaModel;

namespace CapexInfraestructure.Bll.Business.Estadistica
{
    public interface IEstadistica
    {
        /* ------------------------------------------------------------------------------------
        * 
        * PMO360
        * 
        * -----------------------------------------------------------------------------------
        * 
        * CLIENTE          : 
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

        // --------------------------- ESTADISTICA ---------------------------- //
        EstadisticaModel.Grafico1FinalDTO ObtenerDatosGrafico1(FiltroEstadistica.Grafico1 filtro);

        EstadisticaModel.Grafico2FinalDTO ObtenerDatosGrafico2(FiltroEstadistica.Grafico2 filtro);

        List<EstadisticaModel.Grafico3DTO> ObtenerDatosGrafico3(FiltroEstadistica.Grafico3 filtro);

        Grafico3FinalDTO ObtenerDatosGrafico3Final(FiltroEstadistica.Grafico3 filtro);

        //List<EstadisticaModel.Grafico4DTO> ObtenerDatosGrafico4(FiltroEstadistica.Grafico2 filtro);
        List<EstadisticaModel.Grafico4DTO_Categoria> ObtenerDatosGrafico4(FiltroEstadistica.Grafico4Resumen filtro);

        List<EstadisticaModel.Grafico4DTO_Categoria> ObtenerDatosGrafico4_Categoria(FiltroEstadistica.Grafico4Resumen filtro);

        EstadisticaModel.GraficoValorEstimadoBase ObtenerDatosGraficoValorEstimadoBase(string token);

        EstadisticaModel.GraficoValorEstimadoBase ObtenerDatosGraficoValorIngenieria(string token);

        EstadisticaModel.GraficoValorEstimadoBase ObtenerDatosGraficoValorAdquisiciones(string token);

        EstadisticaModel.GraficoValorEstimadoBase ObtenerDatosGraficoValorConstruccion(string token);

        List<FiltroEstadistica.AreaCliente> ListarAreaCliente(string token);

        List<FiltroEstadistica.AnnEjercicio> ListarAnnEjercicio(string token);

        List<FiltroEstadistica.Etapas> ListarEtapas(string token);

        List<FiltroEstadistica.SSO> ListarSSO(string token);

        List<FiltroEstadistica.EstandarSeguridad> ListarEstandarSeguridad(string token);

        List<FiltroEstadistica.Categorias> ListarCategorias(string token);

        List<FiltroEstadistica.EstadoIniciativa> ListarEstadoIniciativa(string token);

        List<FiltroEstadistica.AreaEjecutora> ListarAreaEjecutora(string token);

    }
}
