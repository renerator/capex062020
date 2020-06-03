using CapexInfraestructure.Bll.Entities.Estadistica;
using CapexInfraestructure.Utilities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using static CapexInfraestructure.Bll.Entities.Estadistica.EstadisticaModel;

namespace CapexInfraestructure.Bll.Business.Estadistica
{
    public class Estadistica : IEstadistica
    {
        /* ------------------------------------------------------------------------------------
         * 
         * PMO360
         * 
         * -----------------------------------------------------------------------------------
         * 
         * CLIENTE          : 
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
        public Estadistica()
        {
            AppModule = "Estadística";
            ORM = Utils.Conectar();
        }
        #endregion

        #region "METODOS ESTADISTICA"
        /// <summary>
        /// OBTENER DATOS GRAFICO 1
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        /// 
        public Grafico1FinalDTO ObtenerDatosGrafico1(FiltroEstadistica.Grafico1 filtro)
        {
            try
            {
                var query = ORM.Query<EstadisticaModel.Grafico1Data>("CAPEX_SEL_ESTADISTICA_GRAFICO1", filtro, commandType: CommandType.StoredProcedure).ToList();

                List<EstadisticaModel.Grafico1IniNuevos> listNuevos = new List<EstadisticaModel.Grafico1IniNuevos>();
                List<EstadisticaModel.Grafico1IniRemanentes> listRemanentes = new List<EstadisticaModel.Grafico1IniRemanentes>();
                List<string> listTotalesAcumulados = new List<string>();
                float eneroNuevo = 0, febreroNuevo = 0, marzoNuevo = 0, abrilNuevo = 0, mayoNuevo = 0, junioNuevo = 0, julioNuevo = 0, agostoNuevo = 0, septiembreNuevo = 0, octubreNuevo = 0, noviembreNuevo = 0, diciembreNuevo = 0;
                float eneroRemanente = 0, febreroRemanente = 0, marzoRemanente = 0, abrilRemanente = 0, mayoRemanente = 0, junioRemanente = 0, julioRemanente = 0, agostoRemanente = 0, septiembreRemanente = 0, octubreRemanente = 0, noviembreRemanente = 0, diciembreRemanente = 0;
                Console.WriteLine("111111111");
                foreach (var item in query)
                {
                    if (item.IniTipoEjercicio == "CREAR")
                    {
                        Console.WriteLine("222222222");
                        #region "NUEVOS"
                        var data = new EstadisticaModel.Grafico1IniNuevos() { month = "Ene", nuevos = item.Ene };
                        eneroNuevo = data.nuevos;
                        listNuevos.Add(data);

                        data = new EstadisticaModel.Grafico1IniNuevos() { month = "Feb", nuevos = item.Feb };
                        febreroNuevo = eneroNuevo + data.nuevos;
                        listNuevos.Add(data);

                        data = new EstadisticaModel.Grafico1IniNuevos() { month = "Mar", nuevos = item.Mar };
                        marzoNuevo = febreroNuevo + data.nuevos;
                        listNuevos.Add(data);

                        data = new EstadisticaModel.Grafico1IniNuevos() { month = "Abr", nuevos = item.Abr };
                        abrilNuevo = marzoNuevo + data.nuevos;
                        listNuevos.Add(data);

                        data = new EstadisticaModel.Grafico1IniNuevos() { month = "May", nuevos = item.May };
                        mayoNuevo = abrilNuevo + data.nuevos;
                        listNuevos.Add(data);

                        data = new EstadisticaModel.Grafico1IniNuevos() { month = "Jun", nuevos = item.Jun };
                        junioNuevo = mayoNuevo + data.nuevos;
                        listNuevos.Add(data);

                        data = new EstadisticaModel.Grafico1IniNuevos() { month = "Jul", nuevos = item.Jul };
                        julioNuevo = junioNuevo + data.nuevos;
                        listNuevos.Add(data);

                        data = new EstadisticaModel.Grafico1IniNuevos() { month = "Ago", nuevos = item.Ago };
                        agostoNuevo = julioNuevo + data.nuevos;
                        listNuevos.Add(data);

                        data = new EstadisticaModel.Grafico1IniNuevos() { month = "Sep", nuevos = item.Sep };
                        septiembreNuevo = agostoNuevo + data.nuevos;
                        listNuevos.Add(data);

                        data = new EstadisticaModel.Grafico1IniNuevos() { month = "Oct", nuevos = item.Oct };
                        octubreNuevo = septiembreNuevo + data.nuevos;
                        listNuevos.Add(data);

                        data = new EstadisticaModel.Grafico1IniNuevos() { month = "Nov", nuevos = item.Nov };
                        noviembreNuevo = octubreNuevo + data.nuevos;
                        listNuevos.Add(data);

                        data = new EstadisticaModel.Grafico1IniNuevos() { month = "Dic", nuevos = item.Dic };
                        diciembreNuevo = noviembreNuevo + data.nuevos;
                        listNuevos.Add(data);

                        #endregion
                    }

                    else if (item.IniTipoEjercicio == "IMPORTAR")
                    {
                        Console.WriteLine("333333333");
                        #region "REMANENTES"
                        var data = new EstadisticaModel.Grafico1IniRemanentes() { month = "Ene", remanentes = item.Ene };
                        eneroRemanente = data.remanentes;
                        listRemanentes.Add(data);

                        data = new EstadisticaModel.Grafico1IniRemanentes() { month = "Feb", remanentes = item.Feb };
                        febreroRemanente = eneroRemanente + data.remanentes;
                        listRemanentes.Add(data);

                        data = new EstadisticaModel.Grafico1IniRemanentes() { month = "Mar", remanentes = item.Mar };
                        marzoRemanente = febreroRemanente + data.remanentes;
                        listRemanentes.Add(data);

                        data = new EstadisticaModel.Grafico1IniRemanentes() { month = "Abr", remanentes = item.Abr };
                        abrilRemanente = marzoRemanente + data.remanentes;
                        listRemanentes.Add(data);

                        data = new EstadisticaModel.Grafico1IniRemanentes() { month = "May", remanentes = item.May };
                        mayoRemanente = abrilRemanente + data.remanentes;
                        listRemanentes.Add(data);

                        data = new EstadisticaModel.Grafico1IniRemanentes() { month = "Jun", remanentes = item.Jun };
                        junioRemanente = mayoRemanente + data.remanentes;
                        listRemanentes.Add(data);

                        data = new EstadisticaModel.Grafico1IniRemanentes() { month = "Jul", remanentes = item.Jul };
                        julioRemanente = junioRemanente + data.remanentes;
                        listRemanentes.Add(data);

                        data = new EstadisticaModel.Grafico1IniRemanentes() { month = "Ago", remanentes = item.Ago };
                        agostoRemanente = julioRemanente + data.remanentes;
                        listRemanentes.Add(data);

                        data = new EstadisticaModel.Grafico1IniRemanentes() { month = "Sep", remanentes = item.Sep };
                        septiembreRemanente = agostoRemanente + data.remanentes;
                        listRemanentes.Add(data);

                        data = new EstadisticaModel.Grafico1IniRemanentes() { month = "Oct", remanentes = item.Oct };
                        octubreRemanente = septiembreRemanente + data.remanentes;
                        listRemanentes.Add(data);

                        data = new EstadisticaModel.Grafico1IniRemanentes() { month = "Nov", remanentes = item.Nov };
                        noviembreRemanente = octubreRemanente + data.remanentes;
                        listRemanentes.Add(data);

                        data = new EstadisticaModel.Grafico1IniRemanentes() { month = "Dic", remanentes = item.Dic };
                        diciembreRemanente = noviembreRemanente + data.remanentes;
                        listRemanentes.Add(data);

                        #endregion
                    }
                };

                /*listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", (eneroNuevo + eneroRemanente)));
                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", (febreroNuevo + febreroRemanente)));
                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", (marzoNuevo + marzoRemanente)));
                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", (abrilNuevo + abrilRemanente)));
                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", (mayoNuevo + mayoRemanente)));
                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", (junioNuevo + junioRemanente)));
                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", (julioNuevo + julioRemanente)));
                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", (agostoNuevo + agostoRemanente)));
                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", (septiembreNuevo + septiembreRemanente)));
                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", (octubreNuevo + octubreRemanente)));
                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", (noviembreNuevo + noviembreRemanente)));
                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", (diciembreNuevo + diciembreRemanente)));*/

                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", Convert.ToDouble((eneroNuevo + eneroRemanente).ToString())).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", Convert.ToDouble((febreroNuevo + febreroRemanente).ToString())).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", Convert.ToDouble((marzoNuevo + marzoRemanente).ToString())).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", Convert.ToDouble((abrilNuevo + abrilRemanente).ToString())).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", Convert.ToDouble((mayoNuevo + mayoRemanente).ToString())).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", Convert.ToDouble((junioNuevo + junioRemanente).ToString())).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", Convert.ToDouble((julioNuevo + julioRemanente).ToString())).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", Convert.ToDouble((agostoNuevo + agostoRemanente).ToString())).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", Convert.ToDouble((septiembreNuevo + septiembreRemanente).ToString())).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", Convert.ToDouble((octubreNuevo + octubreRemanente).ToString())).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", Convert.ToDouble((noviembreNuevo + noviembreRemanente).ToString())).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesAcumulados.Add(String.Format("{0:#,##0.##}", Convert.ToDouble((diciembreNuevo + diciembreRemanente).ToString())).ToString().Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                if (listNuevos.Count() > 0 || listRemanentes.Count() > 0)
                {
                    Console.WriteLine("44444444444444");
                    if (listNuevos.Count() > 0 && listRemanentes.Count() > 0)
                    {
                        Console.WriteLine("555555555555");
                        var result = from lNuevos in listNuevos
                                     join lRemanentes in listRemanentes on lNuevos.month equals lRemanentes.month
                                     select new EstadisticaModel.Grafico1DTO
                                     {
                                         month = lNuevos.month,
                                         remanentes = lRemanentes.remanentes,
                                         nuevos = lNuevos.nuevos
                                     };
                        return new EstadisticaModel.Grafico1FinalDTO { Graficos1DTO = result.ToList(), TotalesAcumulados = listTotalesAcumulados };
                    }
                    else if (listNuevos.Count() > 0 && listRemanentes.Count() == 0)
                    {
                        Console.WriteLine("66666666666");
                        var result = from lNuevos in listNuevos
                                     select new EstadisticaModel.Grafico1DTO
                                     {
                                         month = lNuevos.month,
                                         remanentes = 0,
                                         nuevos = lNuevos.nuevos
                                     };
                        return new EstadisticaModel.Grafico1FinalDTO { Graficos1DTO = result.ToList(), TotalesAcumulados = listTotalesAcumulados };
                    }
                    else
                    {
                        Console.WriteLine("77777777777");
                        var result = from lRemanentes in listRemanentes
                                     select new EstadisticaModel.Grafico1DTO
                                     {
                                         month = lRemanentes.month,
                                         remanentes = lRemanentes.remanentes,
                                         nuevos = 0
                                     };
                        return new EstadisticaModel.Grafico1FinalDTO { Graficos1DTO = result.ToList(), TotalesAcumulados = listTotalesAcumulados };
                    }
                }
                Console.WriteLine("8888888888888");
                return new EstadisticaModel.Grafico1FinalDTO();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "Obtener Datos Grafico 1, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        /// <summary>
        /// OBTENER DATOS GRAFICO 2
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        /// 
        public EstadisticaModel.Grafico2FinalDTO ObtenerDatosGrafico2(FiltroEstadistica.Grafico2 filtro)
        {
            try
            {
                if (String.IsNullOrEmpty(filtro.EssToken))
                {
                    filtro.EssToken = "0";
                }
                if (String.IsNullOrEmpty(filtro.EstadoProyecto))
                {
                    filtro.EstadoProyecto = "0";
                }
                var query = ORM.Query<EstadisticaModel.Grafico2Data>("CAPEX_SEL_ESTADISTICA_GRAFICO2", filtro, commandType: CommandType.StoredProcedure).ToList();

                List<EstadisticaModel.Grafico2IniCB> listCB = new List<EstadisticaModel.Grafico2IniCB>();
                List<EstadisticaModel.Grafico2IniPP> listPP = new List<EstadisticaModel.Grafico2IniPP>();
                List<string> listTotalesCB = new List<string>();
                List<string> listTotalesPP = new List<string>();
                float eneroCB = 0, febreroCB = 0, marzoCB = 0, abrilCB = 0, mayoCB = 0, junioCB = 0, julioCB = 0, agostoCB = 0, septiembreCB = 0, octubreCB = 0, noviembreCB = 0, diciembreCB = 0;
                float eneroPP = 0, febreroPP = 0, marzoPP = 0, abrilPP = 0, mayoPP = 0, junioPP = 0, julioPP = 0, agostoPP = 0, septiembrePP = 0, octubrePP = 0, noviembrePP = 0, diciembrePP = 0;
                float comparativa = 0;
                foreach (var item in query)
                {
                    if (item.IniTipo == "CB")
                    {
                        #region "CB"
                        var data = new EstadisticaModel.Grafico2IniCB() { month = "Ene", CB = item.Ene };
                        eneroCB = data.CB;
                        listCB.Add(data);

                        data = new EstadisticaModel.Grafico2IniCB() { month = "Feb", CB = item.Feb };
                        febreroCB = eneroCB + data.CB;
                        listCB.Add(data);

                        data = new EstadisticaModel.Grafico2IniCB() { month = "Mar", CB = item.Mar };
                        marzoCB = febreroCB + data.CB;
                        listCB.Add(data);

                        data = new EstadisticaModel.Grafico2IniCB() { month = "Abr", CB = item.Abr };
                        abrilCB = marzoCB + data.CB;
                        listCB.Add(data);

                        data = new EstadisticaModel.Grafico2IniCB() { month = "May", CB = item.May };
                        mayoCB = abrilCB + data.CB;
                        listCB.Add(data);

                        data = new EstadisticaModel.Grafico2IniCB() { month = "Jun", CB = item.Jun };
                        junioCB = mayoCB + data.CB;
                        listCB.Add(data);

                        data = new EstadisticaModel.Grafico2IniCB() { month = "Jul", CB = item.Jul };
                        julioCB = junioCB + data.CB;
                        listCB.Add(data);

                        data = new EstadisticaModel.Grafico2IniCB() { month = "Ago", CB = item.Ago };
                        agostoCB = julioCB + data.CB;
                        listCB.Add(data);

                        data = new EstadisticaModel.Grafico2IniCB() { month = "Sep", CB = item.Sep };
                        septiembreCB = agostoCB + data.CB;
                        listCB.Add(data);

                        data = new EstadisticaModel.Grafico2IniCB() { month = "Oct", CB = item.Oct };
                        octubreCB = septiembreCB + data.CB;
                        listCB.Add(data);

                        data = new EstadisticaModel.Grafico2IniCB() { month = "Nov", CB = item.Nov };
                        noviembreCB = octubreCB + data.CB;
                        listCB.Add(data);

                        data = new EstadisticaModel.Grafico2IniCB() { month = "Dic", CB = item.Dic };
                        diciembreCB = noviembreCB + data.CB;
                        listCB.Add(data);

                        #endregion
                    }

                    else if (item.IniTipo == "PP")
                    {
                        #region "PP"
                        var data = new EstadisticaModel.Grafico2IniPP() { month = "Ene", PP = item.Ene };
                        eneroPP = data.PP;
                        listPP.Add(data);

                        data = new EstadisticaModel.Grafico2IniPP() { month = "Feb", PP = item.Feb };
                        febreroPP = eneroPP + data.PP;
                        listPP.Add(data);

                        data = new EstadisticaModel.Grafico2IniPP() { month = "Mar", PP = item.Mar };
                        marzoPP = febreroPP + data.PP;
                        listPP.Add(data);

                        data = new EstadisticaModel.Grafico2IniPP() { month = "Abr", PP = item.Abr };
                        abrilPP = marzoPP + data.PP;
                        listPP.Add(data);

                        data = new EstadisticaModel.Grafico2IniPP() { month = "May", PP = item.May };
                        mayoPP = abrilPP + data.PP;
                        listPP.Add(data);

                        data = new EstadisticaModel.Grafico2IniPP() { month = "Jun", PP = item.Jun };
                        junioPP = mayoPP + data.PP;
                        listPP.Add(data);

                        data = new EstadisticaModel.Grafico2IniPP() { month = "Jul", PP = item.Jul };
                        julioPP = junioPP + data.PP;
                        listPP.Add(data);

                        data = new EstadisticaModel.Grafico2IniPP() { month = "Ago", PP = item.Ago };
                        agostoPP = julioPP + data.PP;
                        listPP.Add(data);

                        data = new EstadisticaModel.Grafico2IniPP() { month = "Sep", PP = item.Sep };
                        septiembrePP = agostoPP + data.PP;
                        listPP.Add(data);

                        data = new EstadisticaModel.Grafico2IniPP() { month = "Oct", PP = item.Oct };
                        octubrePP = septiembrePP + data.PP;
                        listPP.Add(data);

                        data = new EstadisticaModel.Grafico2IniPP() { month = "Nov", PP = item.Nov };
                        noviembrePP = octubrePP + data.PP;
                        listPP.Add(data);

                        data = new EstadisticaModel.Grafico2IniPP() { month = "Dic", PP = item.Dic };
                        diciembrePP = noviembrePP + data.PP;
                        listPP.Add(data);

                        #endregion
                    }
                };

                if (diciembreCB != 0)
                {
                    comparativa = ((diciembrePP / diciembreCB) - 1) * 100;
                }


                /*listTotalesCB.Add(String.Format("{0:#,##0.##}", (eneroCB)));
                listTotalesCB.Add(String.Format("{0:#,##0.##}", (febreroCB)));
                listTotalesCB.Add(String.Format("{0:#,##0.##}", (marzoCB)));
                listTotalesCB.Add(String.Format("{0:#,##0.##}", (abrilCB)));
                listTotalesCB.Add(String.Format("{0:#,##0.##}", (mayoCB)));
                listTotalesCB.Add(String.Format("{0:#,##0.##}", (junioCB)));
                listTotalesCB.Add(String.Format("{0:#,##0.##}", (julioCB)));
                listTotalesCB.Add(String.Format("{0:#,##0.##}", (agostoCB)));
                listTotalesCB.Add(String.Format("{0:#,##0.##}", (septiembreCB)));
                listTotalesCB.Add(String.Format("{0:#,##0.##}", (octubreCB)));
                listTotalesCB.Add(String.Format("{0:#,##0.##}", (noviembreCB)));
                listTotalesCB.Add(String.Format("{0:#,##0.##}", (diciembreCB)));

                listTotalesPP.Add(String.Format("{0:#,##0.##}", (eneroPP)));
                listTotalesPP.Add(String.Format("{0:#,##0.##}", (febreroPP)));
                listTotalesPP.Add(String.Format("{0:#,##0.##}", (marzoPP)));
                listTotalesPP.Add(String.Format("{0:#,##0.##}", (abrilPP)));
                listTotalesPP.Add(String.Format("{0:#,##0.##}", (mayoPP)));
                listTotalesPP.Add(String.Format("{0:#,##0.##}", (junioPP)));
                listTotalesPP.Add(String.Format("{0:#,##0.##}", (julioPP)));
                listTotalesPP.Add(String.Format("{0:#,##0.##}", (agostoPP)));
                listTotalesPP.Add(String.Format("{0:#,##0.##}", (septiembrePP)));
                listTotalesPP.Add(String.Format("{0:#,##0.##}", (octubrePP)));
                listTotalesPP.Add(String.Format("{0:#,##0.##}", (noviembrePP)));
                listTotalesPP.Add(String.Format("{0:#,##0.##}", (diciembrePP)));*/

                listTotalesCB.Add(String.Format("{0:#,##0.##}", (eneroCB)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesCB.Add(String.Format("{0:#,##0.##}", (febreroCB)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesCB.Add(String.Format("{0:#,##0.##}", (marzoCB)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesCB.Add(String.Format("{0:#,##0.##}", (abrilCB)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesCB.Add(String.Format("{0:#,##0.##}", (mayoCB)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesCB.Add(String.Format("{0:#,##0.##}", (junioCB)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesCB.Add(String.Format("{0:#,##0.##}", (julioCB)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesCB.Add(String.Format("{0:#,##0.##}", (agostoCB)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesCB.Add(String.Format("{0:#,##0.##}", (septiembreCB)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesCB.Add(String.Format("{0:#,##0.##}", (octubreCB)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesCB.Add(String.Format("{0:#,##0.##}", (noviembreCB)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesCB.Add(String.Format("{0:#,##0.##}", (diciembreCB)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));

                listTotalesPP.Add(String.Format("{0:#,##0.##}", (eneroPP)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesPP.Add(String.Format("{0:#,##0.##}", (febreroPP)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesPP.Add(String.Format("{0:#,##0.##}", (marzoPP)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesPP.Add(String.Format("{0:#,##0.##}", (abrilPP)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesPP.Add(String.Format("{0:#,##0.##}", (mayoPP)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesPP.Add(String.Format("{0:#,##0.##}", (junioPP)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesPP.Add(String.Format("{0:#,##0.##}", (julioPP)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesPP.Add(String.Format("{0:#,##0.##}", (agostoPP)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesPP.Add(String.Format("{0:#,##0.##}", (septiembrePP)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesPP.Add(String.Format("{0:#,##0.##}", (octubrePP)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesPP.Add(String.Format("{0:#,##0.##}", (noviembrePP)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));
                listTotalesPP.Add(String.Format("{0:#,##0.##}", (diciembrePP)).Replace(',', ':').Replace('.', ',').Replace(':', '.'));


                if (listCB.Count() > 0 || listPP.Count() > 0)
                {
                    Console.WriteLine("44444444444444");
                    if (listCB.Count() > 0 && listPP.Count() > 0)
                    {
                        Console.WriteLine("555555555555");
                        var result = from lCB in listCB
                                     join lPP in listPP on lCB.month equals lPP.month
                                     select new EstadisticaModel.Grafico2DTO
                                     {
                                         month = lCB.month,
                                         PP = lPP.PP,
                                         CB = lCB.CB
                                     };
                        //return new EstadisticaModel.Grafico2FinalDTO { Graficos2DTO = result.ToList(), TotalesAcumCB = listTotalesCB, TotalesAcumPP = listTotalesPP, Comparativa = (String.Format("{0:#,##0.#}", comparativa) + "%") };
                        return new EstadisticaModel.Grafico2FinalDTO { Graficos2DTO = result.ToList(), TotalesAcumCB = listTotalesCB, TotalesAcumPP = listTotalesPP, Comparativa = (String.Format("{0:#,##0.#}", comparativa) + "%").Replace('.', ',') };
                    }
                    else if (listCB.Count() > 0 && listPP.Count() == 0)
                    {
                        Console.WriteLine("66666666666");
                        var result = from lCB in listCB
                                     select new EstadisticaModel.Grafico2DTO
                                     {
                                         month = lCB.month,
                                         PP = 0.0f,
                                         CB = lCB.CB
                                     };
                        //return new EstadisticaModel.Grafico2FinalDTO { Graficos2DTO = result.ToList(), TotalesAcumCB = listTotalesCB, TotalesAcumPP = listTotalesPP, Comparativa = (String.Format("{0:#,##0.#}", comparativa) + "%")};
                        return new EstadisticaModel.Grafico2FinalDTO { Graficos2DTO = result.ToList(), TotalesAcumCB = listTotalesCB, TotalesAcumPP = listTotalesPP, Comparativa = (String.Format("{0:#,##0.#}", comparativa) + "%").Replace('.', ',') };
                    }
                    else
                    {
                        Console.WriteLine("77777777777");
                        var result = from lPP in listPP
                                     select new EstadisticaModel.Grafico2DTO
                                     {
                                         month = lPP.month,
                                         PP = lPP.PP,
                                         CB = 0.0f
                                     };
                        //return new EstadisticaModel.Grafico2FinalDTO { Graficos2DTO = result.ToList(), TotalesAcumCB = listTotalesCB, TotalesAcumPP = listTotalesPP, Comparativa = (String.Format("{0:#,##0.#}", comparativa) + "%")};
                        return new EstadisticaModel.Grafico2FinalDTO { Graficos2DTO = result.ToList(), TotalesAcumCB = listTotalesCB, TotalesAcumPP = listTotalesPP, Comparativa = (String.Format("{0:#,##0.#}", comparativa) + "%").Replace('.', ',') };
                    }
                }
                Console.WriteLine("8888888888888");
                return new EstadisticaModel.Grafico2FinalDTO();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "Obtener Datos Grafico 2, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }




        /// <summary>
        /// OBTENER DATOS GRAFICO 3
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        /// 
        public List<EstadisticaModel.Grafico3DTO> ObtenerDatosGrafico3(FiltroEstadistica.Grafico3 filtro)
        {
            try
            {
                var query = ORM.Query<EstadisticaModel.Grafico3Data>("CAPEX_SEL_ESTADISTICA_GRAFICO3", filtro, commandType: CommandType.StoredProcedure).ToList();

                List<EstadisticaModel.Grafico3TipoNuevo> listNuevo = new List<EstadisticaModel.Grafico3TipoNuevo>();
                List<EstadisticaModel.Grafico3TipoRem> listRem = new List<EstadisticaModel.Grafico3TipoRem>();
                List<EstadisticaModel.Grafico3TipoEX> listEX = new List<EstadisticaModel.Grafico3TipoEX>();

                foreach (var item in query)
                {
                    if (item.IniTipo == "PP" && item.CatEstadoProyecto == "NUEVO")
                    {
                        var data = new EstadisticaModel.Grafico3TipoNuevo()
                        {
                            year = item.IniPeriodo,
                            IniTipoEjercicio = item.CatEstadoProyecto,
                            Nuevo = item.Ene + item.Feb + item.Mar + item.Abr + item.May + item.Jun + item.Jul + item.Ago + item.Sep + item.Oct + item.Nov + item.Dic
                        };
                        listNuevo.Add(data);
                    }

                    else if (item.IniTipo == "PP" && item.CatEstadoProyecto == "REMANENTE")
                    {
                        var data = new EstadisticaModel.Grafico3TipoRem()
                        {
                            year = item.IniPeriodo,
                            IniTipoEjercicio = item.CatEstadoProyecto,
                            Rem = item.Ene + item.Feb + item.Mar + item.Abr + item.May + item.Jun + item.Jul + item.Ago + item.Sep + item.Oct + item.Nov + item.Dic
                        };
                        listRem.Add(data);
                    }

                    else if (item.IniTipo == "EX" && item.CatEstadoProyecto == "EXTRAORDINARIO")
                    {
                        var data = new EstadisticaModel.Grafico3TipoEX()
                        {
                            year = item.IniPeriodo,
                            EX = item.Ene + item.Feb + item.Mar + item.Abr + item.May + item.Jun + item.Jul + item.Ago + item.Sep + item.Oct + item.Nov + item.Dic
                        };
                        listEX.Add(data);
                    }
                };

                var result = from lNuevo in listNuevo
                             join lRem in listRem on lNuevo.year equals lRem.year into leftRem
                             join lEX in listEX on lNuevo.year equals lEX.year into leftEX
                             from EXLeft in leftEX.DefaultIfEmpty()
                             from RemLeft in leftRem.DefaultIfEmpty()
                             select new EstadisticaModel.Grafico3DTO
                             {
                                 year = lNuevo.year,
                                 Nuevo = lNuevo?.Nuevo,
                                 Rem = RemLeft?.Rem,
                                 EX = EXLeft?.EX
                             };
                return result.ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "Obtener Datos Grafico 3, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        /// <summary>
        /// OBTENER DATOS GRAFICO 3
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        /// 
        public Grafico3FinalDTO ObtenerDatosGrafico3Final(FiltroEstadistica.Grafico3 filtro)
        {
            try
            {
                int anioFinal = Int32.Parse(filtro.IniPeriodo);
                int anioInicial = (anioFinal - 1);
                Grafico3FinalDTO grafico3FinalDTO = new Grafico3FinalDTO();
                for (int anioActual = anioInicial; anioActual <= anioFinal; anioActual++)
                {
                    double nuevos = 0, remanentes = 0, extraordinarios = 0;
                    filtro.IniPeriodo = anioActual.ToString();
                    var query = ORM.Query<EstadisticaModel.Grafico3Data>("CAPEX_SEL_ESTADISTICA_GRAFICO3", filtro, commandType: CommandType.StoredProcedure).ToList();

                    List<EstadisticaModel.Grafico3TipoNuevo> listNuevo = new List<EstadisticaModel.Grafico3TipoNuevo>();
                    List<EstadisticaModel.Grafico3TipoRem> listRem = new List<EstadisticaModel.Grafico3TipoRem>();
                    List<EstadisticaModel.Grafico3TipoEX> listEX = new List<EstadisticaModel.Grafico3TipoEX>();

                    foreach (var item in query)
                    {
                        if (item.IniTipo == "PP" && item.CatEstadoProyecto == "NUEVO")
                        {
                            var data = new EstadisticaModel.Grafico3TipoNuevo()
                            {
                                year = item.IniPeriodo,
                                IniTipoEjercicio = item.CatEstadoProyecto,
                                Nuevo = item.Ene + item.Feb + item.Mar + item.Abr + item.May + item.Jun + item.Jul + item.Ago + item.Sep + item.Oct + item.Nov + item.Dic
                            };
                            nuevos += data.Nuevo;
                            listNuevo.Add(data);
                        }

                        else if (item.IniTipo == "PP" && item.CatEstadoProyecto == "REMANENTE")
                        {
                            var data = new EstadisticaModel.Grafico3TipoRem()
                            {
                                year = item.IniPeriodo,
                                IniTipoEjercicio = item.CatEstadoProyecto,
                                Rem = item.Ene + item.Feb + item.Mar + item.Abr + item.May + item.Jun + item.Jul + item.Ago + item.Sep + item.Oct + item.Nov + item.Dic
                            };
                            remanentes += data.Rem;
                            listRem.Add(data);
                        }

                        else if (item.IniTipo == "EX" && item.CatEstadoProyecto == "EXTRAORDINARIO")
                        {
                            var data = new EstadisticaModel.Grafico3TipoEX()
                            {
                                year = item.IniPeriodo,
                                EX = item.Ene + item.Feb + item.Mar + item.Abr + item.May + item.Jun + item.Jul + item.Ago + item.Sep + item.Oct + item.Nov + item.Dic
                            };
                            extraordinarios += data.EX;
                            listEX.Add(data);
                        }
                    };

                    var result = from lNuevo in listNuevo
                                 join lRem in listRem on lNuevo.year equals lRem.year into leftRem
                                 join lEX in listEX on lNuevo.year equals lEX.year into leftEX
                                 from EXLeft in leftEX.DefaultIfEmpty()
                                 from RemLeft in leftRem.DefaultIfEmpty()
                                 select new EstadisticaModel.Grafico3DTO
                                 {
                                     year = lNuevo.year,
                                     Nuevo = lNuevo?.Nuevo,
                                     Rem = RemLeft?.Rem,
                                     EX = EXLeft?.EX
                                 };

                    if (anioActual == anioInicial)
                    {
                        grafico3FinalDTO.AnioAnterior = result.ToList();
                        grafico3FinalDTO.TotalesKusAnioAnteriorD = nuevos + remanentes + extraordinarios;
                    }
                    else
                    {
                        grafico3FinalDTO.Anio = result.ToList();
                        grafico3FinalDTO.TotalesKusAnioD = nuevos + remanentes + extraordinarios;
                    }
                }
                grafico3FinalDTO.TotalesKusAnioAnterior = String.Format("{0:#,##0.##}", (grafico3FinalDTO.TotalesKusAnioAnteriorD)).Replace(',', ':').Replace('.', ',').Replace(':', '.');
                grafico3FinalDTO.TotalesKusAnio = String.Format("{0:#,##0.##}", (grafico3FinalDTO.TotalesKusAnioD)).Replace(',', ':').Replace('.', ',').Replace(':', '.');
                if (grafico3FinalDTO.TotalesKusAnioAnteriorD > 0)
                {
                    grafico3FinalDTO.Comparativa = String.Format("{0:#,##0.##}", ((grafico3FinalDTO.TotalesKusAnioD - grafico3FinalDTO.TotalesKusAnioAnteriorD) / grafico3FinalDTO.TotalesKusAnioAnteriorD) * 100).Replace(',', ':').Replace('.', ',').Replace(':', '.');
                }
                else
                {
                    grafico3FinalDTO.Comparativa = "0";
                }
                return grafico3FinalDTO;
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "Obtener Datos Grafico 3, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        /// <summary>
        /// OBTENER DATOS GRAFICO VALOR ESTIMADO BASE
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        /// 
        public EstadisticaModel.GraficoValorEstimadoBase ObtenerDatosGraficoValorEstimadoBase(string token)
        {
            try
            {
                var query = ORM.Query<EstadisticaModel.GraficoValorEstimadoData>("CAPEX_SEL_ESTADISTICA_GRAFICO_VALOR_ESTIMADO_BASE", new { token }, commandType: CommandType.StoredProcedure).ToList();
                List<double> fisico = new List<double>();
                List<double> financiero = new List<double>();
                foreach (var item in query)
                {
                    if (item.IniTipo == "FISICO")
                    {
                        fisico.Add(item.Ene);
                        fisico.Add(item.Feb);
                        fisico.Add(item.Mar);
                        fisico.Add(item.Abr);
                        fisico.Add(item.May);
                        fisico.Add(item.Jun);
                        fisico.Add(item.Jul);
                        fisico.Add(item.Ago);
                        fisico.Add(item.Sep);
                        fisico.Add(item.Oct);
                        fisico.Add(item.Nov);
                        fisico.Add(item.Dic);
                        fisico.Add(item.Sgtes);
                        /*fisico.Add(0.0);
                        fisico.Add(0.0);
                        fisico.Add(1.0);
                        fisico.Add(2.0);
                        fisico.Add(8.0);
                        fisico.Add(9.0);
                        fisico.Add(16.0);
                        fisico.Add(10.0);
                        fisico.Add(20.0);
                        fisico.Add(13.0);
                        fisico.Add(13.0);
                        fisico.Add(3.0);
                        fisico.Add(200.0);*/
                        //  var data1 = [0, 0, 1, 2, 8, 9, 16, 10, 20, 13, 13, 3, 200];

                    }

                    else if (item.IniTipo == "FINANCIERO")
                    {
                        financiero.Add(item.Ene);
                        financiero.Add(item.Feb);
                        financiero.Add(item.Mar);
                        financiero.Add(item.Abr);
                        financiero.Add(item.May);
                        financiero.Add(item.Jun);
                        financiero.Add(item.Jul);
                        financiero.Add(item.Ago);
                        financiero.Add(item.Sep);
                        financiero.Add(item.Oct);
                        financiero.Add(item.Nov);
                        financiero.Add(item.Dic);
                        financiero.Add(item.Sgtes);
                        // var data2= [0, 2, 3, 15, 14, 18, 24, 42, 58, 76, 91, 97, 300];
                        /*financiero.Add(0);
                        financiero.Add(2);
                        financiero.Add(3);
                        financiero.Add(15);
                        financiero.Add(14);
                        financiero.Add(18);
                        financiero.Add(34);
                        financiero.Add(42);
                        financiero.Add(58);
                        financiero.Add(76);
                        financiero.Add(91);
                        financiero.Add(97);
                        financiero.Add(300);*/
                    }
                };
                var result = new EstadisticaModel.GraficoValorEstimadoBase()
                {
                    data1 = fisico,
                    data2 = financiero
                };
                return result;
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "Obtener Datos Grafico Valor Estimado Base, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        /// <summary>
        /// OBTENER DATOS GRAFICO VALOR ESTIMADO BASE
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        /// 
        public EstadisticaModel.GraficoValorEstimadoBase ObtenerDatosGraficoValorIngenieria(string token)
        {
            try
            {
                var query = ORM.Query<EstadisticaModel.GraficoValorEstimadoData>("CAPEX_SEL_ESTADISTICA_GRAFICO_VALOR_INGENIERIA", new { token }, commandType: CommandType.StoredProcedure).ToList();
                List<double> fisico = new List<double>();
                List<double> financiero = new List<double>();
                foreach (var item in query)
                {
                    if (item.IniTipo == "FISICO")
                    {
                        fisico.Add(item.Ene);
                        fisico.Add(item.Feb);
                        fisico.Add(item.Mar);
                        fisico.Add(item.Abr);
                        fisico.Add(item.May);
                        fisico.Add(item.Jun);
                        fisico.Add(item.Jul);
                        fisico.Add(item.Ago);
                        fisico.Add(item.Sep);
                        fisico.Add(item.Oct);
                        fisico.Add(item.Nov);
                        fisico.Add(item.Dic);
                        fisico.Add(item.Sgtes);
                    }

                    else if (item.IniTipo == "FINANCIERO")
                    {
                        financiero.Add(item.Ene);
                        financiero.Add(item.Feb);
                        financiero.Add(item.Mar);
                        financiero.Add(item.Abr);
                        financiero.Add(item.May);
                        financiero.Add(item.Jun);
                        financiero.Add(item.Jul);
                        financiero.Add(item.Ago);
                        financiero.Add(item.Sep);
                        financiero.Add(item.Oct);
                        financiero.Add(item.Nov);
                        financiero.Add(item.Dic);
                        financiero.Add(item.Sgtes);

                    }
                };
                var result = new EstadisticaModel.GraficoValorEstimadoBase()
                {
                    data1 = fisico,
                    data2 = financiero
                };
                return result;
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "Obtener Datos Grafico Valor Ingenieria, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        /// <summary>
        /// OBTENER DATOS GRAFICO VALOR ADQUICISIONES
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        /// 
        public EstadisticaModel.GraficoValorEstimadoBase ObtenerDatosGraficoValorAdquisiciones(string token)
        {
            try
            {
                var query = ORM.Query<EstadisticaModel.GraficoValorEstimadoData>("CAPEX_SEL_ESTADISTICA_GRAFICO_VALOR_ADQUISICIONES", new { token }, commandType: CommandType.StoredProcedure).ToList();
                List<double> fisico = new List<double>();
                List<double> financiero = new List<double>();
                foreach (var item in query)
                {
                    if (item.IniTipo == "FISICO")
                    {
                        fisico.Add(item.Ene);
                        fisico.Add(item.Feb);
                        fisico.Add(item.Mar);
                        fisico.Add(item.Abr);
                        fisico.Add(item.May);
                        fisico.Add(item.Jun);
                        fisico.Add(item.Jul);
                        fisico.Add(item.Ago);
                        fisico.Add(item.Sep);
                        fisico.Add(item.Oct);
                        fisico.Add(item.Nov);
                        fisico.Add(item.Dic);
                        fisico.Add(item.Sgtes);
                    }
                    else if (item.IniTipo == "FINANCIERO")
                    {
                        financiero.Add(item.Ene);
                        financiero.Add(item.Feb);
                        financiero.Add(item.Mar);
                        financiero.Add(item.Abr);
                        financiero.Add(item.May);
                        financiero.Add(item.Jun);
                        financiero.Add(item.Jul);
                        financiero.Add(item.Ago);
                        financiero.Add(item.Sep);
                        financiero.Add(item.Oct);
                        financiero.Add(item.Nov);
                        financiero.Add(item.Dic);
                        financiero.Add(item.Sgtes);

                    }
                };
                var result = new EstadisticaModel.GraficoValorEstimadoBase()
                {
                    data1 = fisico,
                    data2 = financiero
                };
                return result;
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "Obtener Datos Grafico Valor Ingenieria, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        /// <summary>
        /// OBTENER DATOS GRAFICO VALOR CONSTRUCCION
        /// </summary>
        /// <param name="filtro"></param>
        /// <returns></returns>
        /// 
        public EstadisticaModel.GraficoValorEstimadoBase ObtenerDatosGraficoValorConstruccion(string token)
        {
            try
            {
                var query = ORM.Query<EstadisticaModel.GraficoValorEstimadoData>("CAPEX_SEL_ESTADISTICA_GRAFICO_VALOR_CONSTRUCCION", new { token }, commandType: CommandType.StoredProcedure).ToList();
                List<double> fisico = new List<double>();
                List<double> financiero = new List<double>();
                foreach (var item in query)
                {
                    if (item.IniTipo == "FISICO")
                    {
                        fisico.Add(item.Ene);
                        fisico.Add(item.Feb);
                        fisico.Add(item.Mar);
                        fisico.Add(item.Abr);
                        fisico.Add(item.May);
                        fisico.Add(item.Jun);
                        fisico.Add(item.Jul);
                        fisico.Add(item.Ago);
                        fisico.Add(item.Sep);
                        fisico.Add(item.Oct);
                        fisico.Add(item.Nov);
                        fisico.Add(item.Dic);
                        fisico.Add(item.Sgtes);
                    }
                    else if (item.IniTipo == "FINANCIERO")
                    {
                        financiero.Add(item.Ene);
                        financiero.Add(item.Feb);
                        financiero.Add(item.Mar);
                        financiero.Add(item.Abr);
                        financiero.Add(item.May);
                        financiero.Add(item.Jun);
                        financiero.Add(item.Jul);
                        financiero.Add(item.Ago);
                        financiero.Add(item.Sep);
                        financiero.Add(item.Oct);
                        financiero.Add(item.Nov);
                        financiero.Add(item.Dic);
                        financiero.Add(item.Sgtes);
                    }
                };
                var result = new EstadisticaModel.GraficoValorEstimadoBase()
                {
                    data1 = fisico,
                    data2 = financiero
                };
                return result;
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "Obtener Datos Grafico Valor Construccion, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }


        /// <summary>
        /// OBTENER DATOS GRAFICO 4 - ESTADOS
        /// </summary>
        /// <param name="filtro"></param> Para evitar redundancia se reutilizan filtros del grafico 2
        /// <returns></returns>
        /// 
        //public List<EstadisticaModel.Grafico4DTO> ObtenerDatosGrafico4(FiltroEstadistica.Grafico2 filtro) 
        public List<EstadisticaModel.Grafico4DTO_Categoria> ObtenerDatosGrafico4(FiltroEstadistica.Grafico4Resumen filtro)
        {
            try
            {
                /* var query = ORM.Query<EstadisticaModel.Grafico4Data>("CAPEX_SEL_ESTADISTICA_GRAFICO4_ESTADO", filtro, commandType: CommandType.StoredProcedure).ToList();
                var result = from data in query
                             select new EstadisticaModel.Grafico4DTO
                             {
                                 year = data.years,
                                 tipo = data.tipo,
                                 totales = data.totales
                             };

                return result.ToList();*/
                var query = ORM.Query<EstadisticaModel.Grafico4DTO_Categoria>("CAPEX_SEL_ESTADISTICA_GRAFICO4_CATEGORIA", filtro, commandType: CommandType.StoredProcedure).ToList();
                var result = from data in query
                             select new EstadisticaModel.Grafico4DTO_Categoria
                             {
                                 Categoria = data.Categoria,
                                 Total = data.Total
                             };

                return result.ToList();


            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "Obtener Datos Grafico 4 - Estados, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }


        /// <summary>
        /// OBTENER DATOS GRAFICO 4 - CATEGORIAS
        /// </summary>
        /// <param name="filtro"></param> Para evitar redundancia se reutilizan filtros del grafico 2
        /// <returns></returns>
        /// 
        public List<EstadisticaModel.Grafico4DTO_Categoria> ObtenerDatosGrafico4_Categoria(FiltroEstadistica.Grafico4Resumen filtro)
        {
            try
            {
                var query = ORM.Query<EstadisticaModel.Grafico4DTO_Categoria>("CAPEX_SEL_ESTADISTICA_GRAFICO4_CATEGORIA", filtro, commandType: CommandType.StoredProcedure).ToList();

                var result = from data in query
                             select new EstadisticaModel.Grafico4DTO_Categoria
                             {
                                 Categoria = data.Categoria,
                                 Total = data.Total
                             };

                return result.ToList();

            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "Obtener Datos Grafico 4 - Categorias, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }



        /// <summary>
        /// OBTENER DATOS AREA CLIENTE
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// 
        public List<FiltroEstadistica.AreaCliente> ListarAreaCliente(string token)
        {
            try
            {
                return ORM.Query<FiltroEstadistica.AreaCliente>("CAPEX_SEL_AREAS", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "Obtener Datos Area Cliente, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }

        /// <summary>
        /// OBTENER DATOS AÑO EJERCICIO
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// 
        public List<FiltroEstadistica.AnnEjercicio> ListarAnnEjercicio(string token)
        {
            try
            {
                return ORM.Query<FiltroEstadistica.AnnEjercicio>("CAPEX_ANO_SEL_INICIATIVA", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "Obtener Datos Año Ejercicio, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }


        /// <summary>
        /// OBTENER DATOS ETAPAS
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// 
        public List<FiltroEstadistica.Etapas> ListarEtapas(string token)
        {
            try
            {
                return ORM.Query<FiltroEstadistica.Etapas>("CAPEX_SEL_ETAPAS", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "Obtener Datos Etapas, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }


        /// <summary>
        /// OBTENER ESTANDAR C SSO
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// 
        public List<FiltroEstadistica.SSO> ListarSSO(string token)
        {
            try
            {
                return ORM.Query<FiltroEstadistica.SSO>("CAPEX_SEL_CLASIFICACION_SSO", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "Obtener Datos SSO, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }





        /// <summary>
        /// OBTENER ESTANDAR SEGURIDAD
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// 
        public List<FiltroEstadistica.EstandarSeguridad> ListarEstandarSeguridad(string token)
        {
            try
            {
                return ORM.Query<FiltroEstadistica.EstandarSeguridad>("CAPEX_SEL_ESTANDAR_SEGURIDAD_V2", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "Obtener Estandar Seguridad, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }



        /// <summary>
        /// OBTENER CATEGORIAS
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// 
        public List<FiltroEstadistica.Categorias> ListarCategorias(string token)
        {
            try
            {
                return ORM.Query<FiltroEstadistica.Categorias>("CAPEX_SEL_CATEGORIAS", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "Obtener Categorias, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }




        /// <summary>
        /// OBTENER ESTADOS INICIATIVA
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// 
        public List<FiltroEstadistica.EstadoIniciativa> ListarEstadoIniciativa(string token)
        {
            try
            {
                return ORM.Query<FiltroEstadistica.EstadoIniciativa>("CAPEX_SEL_ESTADO_INICIATIVA", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "Obtener Categorias, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }


        /// <summary>
        /// OBTENER AREA EJECUTORA
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// 
        public List<FiltroEstadistica.AreaEjecutora> ListarAreaEjecutora(string token)
        {
            try
            {
                return ORM.Query<FiltroEstadistica.AreaEjecutora>("CAPEX_SEL_GERENCIAS", commandType: CommandType.StoredProcedure).ToList();
            }
            catch (Exception err)
            {
                ExceptionResult = AppModule + "Obtener Area Ejecutora, Mensaje: " + err.Message.ToString() + "-" + ", Detalle: " + err.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return null;
            }
        }



        #endregion "METODOS IDENTIFICACION"

        #region "GLOBALES"
        /// <summary>
        /// REGISTRAR  ARCHIVOS EN DB
        /// </summary>
        /// <param name="IniToken"></param>
        /// <param name="ParUsuario"></param>
        /// <param name="ParNombre"></param>
        /// <param name="ParPaso"></param>
        /// <param name="ParCaso"></param>
        /// <returns></returns>
        public string RegistrarArchivo(string IniToken, string ParUsuario, string ParNombre, string ParPaso, string ParCaso)
        {
            try
            {
                ORM.Query("CAPEX_INS_REGISTRAR_ARCHIVO", new { IniToken, ParUsuario, ParNombre, ParPaso, ParCaso }, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return "Registrado";
            }
            catch (Exception exc)
            {
                ExceptionResult = AppModule + "RegistrarArchivo, Mensaje: " + exc.Message.ToString() + "-" + ", Detalle: " + exc.StackTrace.ToString();
                Utils.LogError(ExceptionResult);
                return "Error";
            }
        }

        #endregion
    }

}
