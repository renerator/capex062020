using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexInfraestructure.Bll.Entities.Planificacion
{
    public class Hito
    {
        public class HitoResumen
        {
            public string IdIr { get; set; }
            public string IrToken { get; set; }
            public string IniToken { get; set; }
            public string IniUsuario { get; set; }
            public string IrDato0 { get; set; }
            public string IrDato1 { get; set; }
            public string IrDato2 { get; set; }
            public string IrFecha { get; set; }
            public string IrEstadoImportacion { get; set; }
            public string IrEstado { get; set; }
        }
        public class HitoDetalle
        {
            public string IdIg { get; set; }
            public string IgToken { get; set; }
            public string IniToken { get; set; }
            public string IniUsuario { get; set; }
            public string IgPresupuesto { get; set; }
            public string IgFechaInicio { get; set; }
            public string IgFechaTermino { get; set; }
            public string IgFechaCierre { get; set; }
            public string IgFecha { get; set; }
            public string IgEstadoImportacion { get; set; }
            public string IgEstado { get; set; }
            public string PorInvNacExt { get; set; }
        }
        public class HitoGuardar
        {
            public string IniToken { get; set; }
            public string IniUsuario { get; set; }
            public string HitNacExt { get; set; }
            public string HitSAP { get; set; }
            public string HitCI { get; set; }
            public string HitCA { get; set; }
            public string HitOPR { get; set; }
            public string HitPE { get; set; }
            public string HitDIRCEN { get; set; }
            public string HitDirPLC { get; set; }

        }

        public class Pdf
        {
            public string IniPeriodo { get; set; }
            public string IrFecha { get; set; }
            public string IfDato19 { get; set; }
            public string FechaMeses { get; set; }
            public string PidCodigoProyecto { get; set; }
            public string PidNombreProyecto { get; set; }
            public string PidEtapa { get; set; }
            public string PidGerenciaInversion { get; set; }
            public string PidGErenciaEjecucion { get; set; }
            public string PidSuperintendencia { get; set; }
            public string CatEstadoProyecto { get; set; }
            public string CatMacroCategoria { get; set; }
            public string CatCategoria { get; set; }
            public string CatClasificacionSSO { get; set; }
            public string CatClase { get; set; }
            public string CatNivelIngenieria { get; set; }
            public string CatEstandarSeguridad { get; set; }
            public string CatTipoCotizacion { get; set; }
            public string PddObjetivo { get; set; }
            public string PddJustificacion { get; set; }
            public string PddAlcance { get; set; }
            public string HitCI { get; set; }
            public string HitCA { get; set; }
            public string HitOPR { get; set; }
            public string VFP { get; set; }
            public string HitPE { get; set; }
            public string HitDirPLC { get; set; }
            public string HitDIRCEN { get; set; }
            public string HitInicio { get; set; }
            public string HitTermino { get; set; }
            public string HitCierreSAP { get; set; }
            public string PddFechaPostEval { get; set; }
            public string Ingenieria { get; set; }
            public string Adquisicion { get; set; }
            public string Construccion { get; set; }
            public string Administracion { get; set; }
            public string Contingencia { get; set; }
            public string PContingencia { get; set; }
            public string Ccd { get; set; }
            public string TotalAcumuladoA { get; set; }
            public string EneroA { get; set; }
            public string FebreroA { get; set; }
            public string MarzoA { get; set; }
            public string AbrilA { get; set; }
            public string MayoA { get; set; }
            public string JunioA { get; set; }
            public string JulioA { get; set; }
            public string AgostoA { get; set; }
            public string SeptiembreA { get; set; }
            public string OctubreA { get; set; }
            public string NoviembreA { get; set; }
            public string DiciembreA { get; set; }
            public string TotalPacialAA { get; set; }
            public string TotalPacialAB { get; set; }
            public string TotalPacialAC { get; set; }
            public string TotalPacialAD { get; set; }
            public string TotalAcumuladoB { get; set; }
            public string EneroB { get; set; }
            public string FebreroB { get; set; }
            public string MarzoB { get; set; }
            public string AbrilB { get; set; }
            public string MayoB { get; set; }
            public string JunioB { get; set; }
            public string JulioB { get; set; }
            public string AgostoB { get; set; }
            public string SeptiembreB { get; set; }
            public string OctubreB { get; set; }
            public string NoviembreB { get; set; }
            public string DiciembreB { get; set; }
            public string TotalPacialBA { get; set; }
            public string TotalPacialBB { get; set; }
            public string TotalPacialBC { get; set; }
            public string TotalPacialBD { get; set; }
            public string TotalAcumuladoC { get; set; }
            public string EneroC { get; set; }
            public string FebreroC { get; set; }
            public string MarzoC { get; set; }
            public string AbrilC { get; set; }
            public string MayoC { get; set; }
            public string JunioC { get; set; }
            public string JulioC { get; set; }
            public string AgostoC { get; set; }
            public string SeptiembreC { get; set; }
            public string OctubreC { get; set; }
            public string NoviembreC { get; set; }
            public string DiciembreC { get; set; }
            public string TotalPacialCA { get; set; }
            public string TotalPacialCB { get; set; }
            public string TotalPacialCC { get; set; }
            public string TotalPacialCD { get; set; }
            public string TotalAcumuladoD { get; set; }
            public string EneroD { get; set; }
            public string FebreroD { get; set; }
            public string MarzoD { get; set; }
            public string AbrilD { get; set; }
            public string MayoD { get; set; }
            public string JunioD { get; set; }
            public string JulioD { get; set; }
            public string AgostoD { get; set; }
            public string SeptiembreD { get; set; }
            public string OctubreD { get; set; }
            public string NoviembreD { get; set; }
            public string DiciembreD { get; set; }
            public string TotalPacialDA { get; set; }
            public string TotalPacialDB { get; set; }
            public string TotalPacialDC { get; set; }
            public string TotalPacialDD { get; set; }
            public string TotalAcumuladoE { get; set; }
            public string EneroE { get; set; }
            public string FebreroE { get; set; }
            public string MarzoE { get; set; }
            public string AbrilE { get; set; }
            public string MayoE { get; set; }
            public string JunioE { get; set; }
            public string JulioE { get; set; }
            public string AgostoE { get; set; }
            public string SeptiembreE { get; set; }
            public string OctubreE { get; set; }
            public string NoviembreE { get; set; }
            public string DiciembreE { get; set; }
            public string TotalPacialEA { get; set; }
            public string TotalPacialEB { get; set; }
            public string TotalPacialEC { get; set; }
            public string TotalPacialED { get; set; }
            public string TotalAcumuladoF { get; set; }
            public string EneroF { get; set; }
            public string FebreroF { get; set; }
            public string MarzoF { get; set; }
            public string AbrilF { get; set; }
            public string MayoF { get; set; }
            public string JunioF { get; set; }
            public string JulioF { get; set; }
            public string AgostoF { get; set; }
            public string SeptiembreF { get; set; }
            public string OctubreF { get; set; }
            public string NoviembreF { get; set; }
            public string DiciembreF { get; set; }
            public string TotalPacialFA { get; set; }
            public string TotalPacialFB { get; set; }
            public string TotalPacialFC { get; set; }
            public string TotalPacialFD { get; set; }
            public string TotalAcumuladoG { get; set; }
            public string EneroG { get; set; }
            public string FebreroG { get; set; }
            public string MarzoG { get; set; }
            public string AbrilG { get; set; }
            public string MayoG { get; set; }
            public string JunioG { get; set; }
            public string JulioG { get; set; }
            public string AgostoG { get; set; }
            public string SeptiembreG { get; set; }
            public string OctubreG { get; set; }
            public string NoviembreG { get; set; }
            public string DiciembreG { get; set; }
            public string TotalPacialGA { get; set; }
            public string TotalPacialGB { get; set; }
            public string TotalPacialGC { get; set; }
            public string TotalPacialGD { get; set; }
            public string EriProb1 { get; set; }
            public string EriImp1 { get; set; }
            public string EriRies1 { get; set; }
            public string EriClas1 { get; set; }
            public string EriProb2 { get; set; }
            public string EriImp2 { get; set; }
            public string EriRies2 { get; set; }
            public string EriClas2 { get; set; }
            public string EriItemMR { get; set; }
            public string EveVan { get; set; }
            public string Tir { get; set; }
            public string EveIvan { get; set; }
            public string EvePayBack { get; set; }
            public string EveVidaUtil { get; set; }
            public string PddDescripcion1 { get; set; }
            public string PddUnidad1 { get; set; }
            public string PddActual1 { get; set; }
            public string PddTarget1 { get; set; }
            public string PddDescripcion2 { get; set; }
            public string PddUnidad2 { get; set; }
            public string PddActual2 { get; set; }
            public string PddTarget2 { get; set; }
            public string PddDescripcion3 { get; set; }
            public string PddUnidad3 { get; set; }
            public string PddActual3 { get; set; }
            public string PddTarget3 { get; set; }
        }
    }
}
