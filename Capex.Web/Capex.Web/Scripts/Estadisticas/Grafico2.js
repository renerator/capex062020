//
//CHARTS CONFIGURACION GRAFICO 2
//


am4core.useTheme(am4themes_animated);

var chart2 = am4core.create("chartdiv2", am4charts.XYChart);

// Create axes
var categoryAxis = chart2.xAxes.push(new am4charts.CategoryAxis());
categoryAxis.dataFields.category = "month";
categoryAxis.renderer.grid.template.location = 0;

var valueAxis = chart2.yAxes.push(new am4charts.ValueAxis());
valueAxis.renderer.inside = true;
valueAxis.renderer.labels.template.disabled = true;
valueAxis.min = 0;

// Create series
function createSeries(field, name) {

    // Set up series
    var series = chart2.series.push(new am4charts.ColumnSeries());
    series.name = name;
    series.dataFields.valueY = field;
    series.dataFields.categoryX = "month";
    series.sequencedInterpolation = true;

    // Make it stacked
    series.stacked = false;

    // Configure columns
    series.columns.template.width = am4core.percent(70);
    series.columns.template.tooltipText = "[bold]{name}[/]\n[font-size:14px]{categoryX}: {valueY}";

    // Add label
    var labelBullet = series.bullets.push(new am4charts.LabelBullet());
    labelBullet.label.text = "{valueY}";
    labelBullet.locationY = 0.5;

    return series;
}

createSeries("CB", "CB - CD (Parcial)");
createSeries("PP", "Presupuesto (Parcial)");

// Legend
chart2.legend = new am4charts.Legend();

//
// OBTENER DATOS GRAFICO 2
//
$("#FNObtenerDatosGrafico2").click(function () {
    $.ajax({
        type: "GET",
        url: "Estadistica/ObtenerDatosGrafico2",
        dataType: "json",
        data: {
            AreaToken: Area2.val(),
            IniPeriodo: AnnEjercicio2.val(),
            NIAcronimo: Etapa2.val(),
            CSToken: Sso2.val(),
            EssToken: EstandarSeguridad2.val(),
            EstToken: Estado2.val(),
            CatToken: Categoria2.val(),
            GerToken: AreaEjecutora2.val(),
            EstadoProyecto: $('#EstadoProyecto2').val(),
            Clase: $('#Clase2').val(),
            Macrocategoria: $('#Macrocategoria2').val()

        },
        success: function (resp) {
            chart2.data = resp.Graficos2DTO;
            console.log(resp.Graficos2DTO);
            $("#aditionalGraficos2DivComparativa").remove();
            $("#aditionalGraficos2DivMeses").remove();
            $("#aditionalGraficos2DivCB").remove();
            $("#aditionalGraficos2DivPP").remove();

            if (resp.Comparativa) {
                console.log("resp.Comparativa=", resp.Comparativa);
                var aditionalDivComparativa = '<div id="aditionalGraficos2DivComparativa"><div class="row">  <div class="col-sm-11"></div> <div class="col-sm-1">' + '<span style="color:yellow">' + resp.Comparativa + '</span></div></div></div>';
                $("#chartdiv2").before(aditionalDivComparativa);
            }

            var aditionalGraficos2DivMeses = '<div id="aditionalGraficos2DivMeses"><div class="row">  <div class="col-sm-1"> <span style="color:black"></span></div> <div class="col-sm-11"> <div class="row">  <div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Ene</span></div> <div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Feb</span></div> <div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Mar</span></div> <div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Abr</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">May</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Jun</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Jul</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Ago</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Sep</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Oct</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Nov</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Dic</span></div></div ></div ></div ></div >';
            $("#chartdiv2").after(aditionalGraficos2DivMeses);

            if (resp.TotalesAcumCB) {
                var arrayTotalesCB = resp.TotalesAcumCB;
                console.log("arrayTotalesCB=", arrayTotalesCB);
                if (arrayTotalesCB && arrayTotalesCB.length == 12) {
                    var aditionalDivCB = '<div id="aditionalGraficos2DivCB"><div class="row">  <div class="col-sm-1 border border-dark" style="text-align: center;"> <span style="color:black">Acum CB</span></div> <div class="col-sm-11"> <div class="row">  <div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesCB[0] + '</span></div> <div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesCB[1] + '</span></div> <div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesCB[2] + '</span></div> <div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesCB[3] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesCB[4] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesCB[5] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesCB[6] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesCB[7] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesCB[8] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesCB[9] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesCB[10] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesCB[11] + '</span></div></div ></div ></div ></div >';
                    $("#aditionalGraficos2DivMeses").after(aditionalDivCB);
                }
            }

            if (resp.TotalesAcumPP) {
                var arrayTotalesPP = resp.TotalesAcumPP;
                console.log("arrayTotalesPP=", arrayTotalesPP);
                if (arrayTotalesPP && arrayTotalesPP.length == 12) {
                    var aditionalDivPP = '<div id="aditionalGraficos2DivPP"><div class="row">  <div class="col-sm-1 border border-dark" style="text-align: center;"> <span style="color:black">Acum PP</span></div> <div class="col-sm-11"> <div class="row">  <div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesPP[0] + '</span></div> <div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesPP[1] + '</span></div> <div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesPP[2] + '</span></div> <div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesPP[3] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesPP[4] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesPP[5] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesPP[6] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesPP[7] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesPP[8] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesCB[9] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesPP[10] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotalesPP[11] + '</span></div></div ></div ></div ></div >';
                    $("#aditionalGraficos2DivCB").after(aditionalDivPP);
                }
            }
        }

    });
});


