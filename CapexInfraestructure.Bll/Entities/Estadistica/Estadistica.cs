using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Estadistica
{
    public class EstadisticaModel
    {
        public class Grafico1Data
        {
            public string IniTipoEjercicio { get; set; }
            public float Ene { get; set; }
            public float Feb { get; set; }
            public float Mar { get; set; }
            public float Abr { get; set; }
            public float May { get; set; }
            public float Jun { get; set; }
            public float Jul { get; set; }
            public float Ago { get; set; }
            public float Sep { get; set; }
            public float Oct { get; set; }
            public float Nov { get; set; }
            public float Dic { get; set; }
        }

        public class Grafico1DTO
        {
            public string month { get; set; }
            public float nuevos { get; set; }
            public float remanentes { get; set; }
        }

        public class Grafico1FinalDTO
        {
            private List<Grafico1DTO> _graficos1DTO;
            private List<string> _totalesAcumulados;
            public List<Grafico1DTO> Graficos1DTO
            {
                get { return _graficos1DTO; }
                set { _graficos1DTO = value; }
            }
            public List<string> TotalesAcumulados
            {
                get { return _totalesAcumulados; }
                set { _totalesAcumulados = value; }
            }
        }

        public class Grafico1IniNuevos
        {
            public string month { get; set; }
            public float nuevos { get; set; }
        }

        public class Grafico1IniRemanentes
        {
            public string month { get; set; }
            public float remanentes { get; set; }
        }

        public class Grafico2Data
        {
            public string IniTipo { get; set; }
            public float Ene { get; set; }
            public float Feb { get; set; }
            public float Mar { get; set; }
            public float Abr { get; set; }
            public float May { get; set; }
            public float Jun { get; set; }
            public float Jul { get; set; }
            public float Ago { get; set; }
            public float Sep { get; set; }
            public float Oct { get; set; }
            public float Nov { get; set; }
            public float Dic { get; set; }
        }

        public class Grafico2DTO
        {
            public string month { get; set; }
            public float CB { get; set; }
            public float PP { get; set; }
        }

        public class Grafico2FinalDTO
        {
            private List<Grafico2DTO> _graficos2DTO;
            private List<string> _totalesAcumCB;
            private List<string> _totalesAcumPP;
            public string Comparativa { get; set; }


            public List<Grafico2DTO> Graficos2DTO
            {
                get { return _graficos2DTO; }
                set { _graficos2DTO = value; }
            }
            public List<string> TotalesAcumCB
            {
                get { return _totalesAcumCB; }
                set { _totalesAcumCB = value; }
            }
            public List<string> TotalesAcumPP
            {
                get { return _totalesAcumPP; }
                set { _totalesAcumPP = value; }
            }
        }

        public class Grafico2IniCB
        {
            public string month { get; set; }
            public float CB { get; set; }
        }

        public class Grafico2IniPP
        {
            public string month { get; set; }
            public float PP { get; set; }
        }

        public class Grafico3Data
        {
            public string IniPeriodo { get; set; }
            public string IniTipo { get; set; }
            public string IniTipoEjercicio { get; set; }
            public string CatEstadoProyecto { get; set; }
            public double Ene { get; set; }
            public double Feb { get; set; }
            public double Mar { get; set; }
            public double Abr { get; set; }
            public double May { get; set; }
            public double Jun { get; set; }
            public double Jul { get; set; }
            public double Ago { get; set; }
            public double Sep { get; set; }
            public double Oct { get; set; }
            public double Nov { get; set; }
            public double Dic { get; set; }
        }

        public class GraficoValorEstimadoData
        {
            public string IniTipo { get; set; }
            public double Ene { get; set; }
            public double Feb { get; set; }
            public double Mar { get; set; }
            public double Abr { get; set; }
            public double May { get; set; }
            public double Jun { get; set; }
            public double Jul { get; set; }
            public double Ago { get; set; }
            public double Sep { get; set; }
            public double Oct { get; set; }
            public double Nov { get; set; }
            public double Dic { get; set; }
            public double Sgtes { get; set; }
        }

        public class Grafico3DTO
        {
            public string year { get; set; }
            public double? Nuevo { get; set; }
            public double? Rem { get; set; }
            public double? EX { get; set; }
        }

        public class Grafico3FinalDTO
        {
            private List<Grafico3DTO> _anio;
            private List<Grafico3DTO> _anioAnterior;

            public string TotalesKusAnio { get; set; }
            public string TotalesKusAnioAnterior { get; set; }
            public double TotalesKusAnioD { get; set; }
            public double TotalesKusAnioAnteriorD { get; set; }

            public string Comparativa { get; set; }


            public List<Grafico3DTO> Anio
            {
                get { return _anio; }
                set { _anio = value; }
            }

            public List<Grafico3DTO> AnioAnterior
            {
                get { return _anioAnterior; }
                set { _anioAnterior = value; }
            }

        }



        public class Grafico3TipoNuevo
        {
            public string year { get; set; }
            public double Nuevo { get; set; }
            public string IniTipoEjercicio { get; set; }
        }

        public class Grafico3TipoRem
        {
            public string year { get; set; }
            public double Rem { get; set; }
            public string IniTipoEjercicio { get; set; }
        }

        public class Grafico3TipoEX
        {
            public string year { get; set; }
            public double EX { get; set; }
        }

        public class GraficoValorEstimadoBase
        {
            public List<double> data1 { get; set; }
            public List<double> data2 { get; set; }
        }

        public class Grafico4Data
        {
            public string years { get; set; }
            public string tipo { get; set; }
            public int totales { get; set; }
        }

        public class Grafico4DTO
        {
            public string year { get; set; }
            public string tipo { get; set; }
            public int totales { get; set; }
        }

        public class Grafico4DTO_Categoria
        {
            public string Categoria { get; set; }
            public int Total { get; set; }
        }

    }

}
