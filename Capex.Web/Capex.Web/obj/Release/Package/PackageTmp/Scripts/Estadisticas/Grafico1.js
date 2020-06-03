//
//CHARTS CONFIGURACION GRAFICO 1
//
am4core.useTheme(am4themes_animated);

var chart = am4core.create("chartdiv1", am4charts.XYChart);

// Create axes
var categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis());
categoryAxis.dataFields.category = "month";
categoryAxis.renderer.grid.template.location = 0;

var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
valueAxis.renderer.inside = true;
valueAxis.renderer.labels.template.disabled = true;
valueAxis.min = 0;

// Create series
function createSeries(field, name) {

    // Set up series
    var series = chart.series.push(new am4charts.ColumnSeries());
    series.name = name;
    series.dataFields.valueY = field;
    series.dataFields.categoryX = "month";
    series.sequencedInterpolation = true;

    // Make it stacked
    series.stacked = true;

    // Configure columns
    series.columns.template.width = am4core.percent(70);
    series.columns.template.tooltipText = "[bold]{name}[/]\n[font-size:14px]{categoryX}: {valueY}";

    // Add label
    var labelBullet = series.bullets.push(new am4charts.LabelBullet());
    labelBullet.label.text = "{valueY}";
    labelBullet.locationY = 0.5;

    return series;
}

createSeries("remanentes", "Remanentes");
createSeries("nuevos", "Nuevos");

// Legend
chart.legend = new am4charts.Legend();
chart.language.locale["_decimalSeparator"] = ",";
chart.language.locale["_thousandSeparator"] = ".";


//
// OBTENER DATOS GRAFICO 1
//
$("#FNObtenerDatosGrafico1").click(function () {
    $.ajax({
        type: "GET",
        url: "Estadistica/ObtenerDatosGrafico1",
        dataType: "json",
        data: {
            AreaToken: Area.val(),
            IniPeriodo: AnnEjercicio.val(),
            NIAcronimo: Etapa.val(),
            CSToken: Sso.val(),
            EssToken: EstandarSeguridad.val(),
            EstToken: Estado.val(),
            CatToken: Categoria.val(),
            GerToken: AreaEjecutora.val(),
            Clase: $('#Clase').val(),
            Macrocategoria: $('#Macrocategoria').val()

        },
        success: function (resp) {
            chart.data = resp.Graficos1DTO
            console.log(resp.Graficos1DTO);
            $("#aditionalDiv").remove();
            $("#aditionalGraficos1DivMeses").remove();
            if (resp.TotalesAcumulados) {
                var aditionalGraficos1DivMeses = '<div id="aditionalGraficos1DivMeses"><div class="row">  <div class="col-sm-1"> <span style="color:black"></span></div> <div class="col-sm-11"> <div class="row">  <div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Ene</span></div> <div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Feb</span></div> <div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Mar</span></div> <div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Abr</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">May</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Jun</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Jul</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Ago</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Sep</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Oct</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Nov</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Dic</span></div></div ></div ></div ></div >';
                $("#chartdiv1").after(aditionalGraficos1DivMeses);
                var arrayTotales = resp.TotalesAcumulados;
                console.log("arrayTotales=", arrayTotales);
                if (arrayTotales && arrayTotales.length == 12) {
                    var aditionalDiv = '<div id="aditionalDiv"><div class="row">  <div class="col-sm-1 border border-dark" style="text-align: center;"> <span style="color:black">Acum.</span></div> <div class="col-sm-11"> <div class="row">  <div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotales[0] + '</span></div> <div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotales[1] + '</span></div> <div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotales[2] + '</span></div> <div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotales[3] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotales[4] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotales[5] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotales[6] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotales[7] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotales[8] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotales[9] + '</span></div><div class="col-sm-1 border border-dark style="text-align: center;"">' + '<span style="color:black">' + arrayTotales[10] + '</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + arrayTotales[11] + '</span></div></div ></div ></div ></div >';
                    $("#aditionalGraficos1DivMeses").after(aditionalDiv);
                }
            }
        }

    });
});

