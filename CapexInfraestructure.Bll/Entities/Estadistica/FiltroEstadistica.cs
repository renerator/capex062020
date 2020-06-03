namespace CapexInfraestructure.Bll.Entities.Estadistica
{
    public class FiltroEstadistica
    {
        public class Grafico1
        {
            public string AreaToken { get; set; }
            public string IniPeriodo { get; set; }
            public string NIAcronimo { get; set; }
            public string CSToken { get; set; }
            public string EssToken { get; set; }
            public string EstToken { get; set; }
            public string CatToken { get; set; }
            public string Clase { get; set; }
            public string MacroCategoria { get; set; }
            public string GerToken { get; set; }
        }

        public class Grafico2
        {
            public string AreaToken { get; set; }
            public string IniPeriodo { get; set; }
            public string NIAcronimo { get; set; }
            public string CSToken { get; set; }
            public string Clase { get; set; }
            public string EstToken { get; set; }
            public string MacroCategoria { get; set; }
            public string EssToken { get; set; }
            public string CatToken { get; set; }
            public string EstadoProyecto { get; set; }
            public string GerToken { get; set; }
        }

        public class Grafico3
        {

            public string IniPeriodo { get; set; }
            public string EstToken { get; set; }

        }

        public class Grafico4Resumen
        {

            public string IniPeriodo { get; set; }
            public string Opcion { get; set; }

        }

        public class AreaCliente
        {

            public string AreaToken { get; set; }
            public string AreaNombre { get; set; }

        }

        public class AnnEjercicio
        {
            public string IniToken { get; set; }
            public string IniPeriodo { get; set; }

        }

        public class Etapas
        {

            public string NIAcronimo { get; set; }
            public string NINombre { get; set; }

        }

        public class SSO
        {
            public string CSToken { get; set; }
            public string CSNombre { get; set; }

        }


        public class EstandarSeguridad
        {
            public string EssToken { get; set; }
            public string EssNombre { get; set; }
        }

        public class Categorias
        {
            public string CatToken { get; set; }
            public string CatNombre { get; set; }

        }


        public class EstadoIniciativa
        {
            public string EstToken { get; set; }
            public string EstNombre { get; set; }

        }


        public class AreaEjecutora
        {
            public string GerToken { get; set; }
            public string GerNombre { get; set; }

        }
    }
}
