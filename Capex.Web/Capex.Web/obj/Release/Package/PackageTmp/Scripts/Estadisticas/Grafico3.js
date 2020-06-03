//
//CHARTS CONFIGURACION GRAFICO 3
//

am4core.useTheme(am4themes_frozen);
am4core.useTheme(am4themes_animated);

var chart3 = am4core.create("chartdiv3", am4charts.XYChart);

// Create axes
var categoryAxis = chart3.xAxes.push(new am4charts.CategoryAxis());
categoryAxis.dataFields.category = "year";
categoryAxis.renderer.grid.template.location = 0;

var valueAxis = chart3.yAxes.push(new am4charts.ValueAxis());
valueAxis.renderer.inside = true;
valueAxis.renderer.labels.template.disabled = true;
valueAxis.min = 0;

// Create series
function createSeries(field, name) {

    // Set up series
    var series = chart3.series.push(new am4charts.ColumnSeries());
    series.name = name;
    series.dataFields.valueY = field;
    series.dataFields.categoryX = "year";
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

createSeries("Rem", "Remanentes");
createSeries("Nuevo", "Nuevos");
createSeries("EX", "Extraordinarios");

// Legend
chart3.legend = new am4charts.Legend();





//
// OBTENER DATOS GRAFICO 3
//
/*$("#FNObtenerDatosGrafico3").click(function () {
    $.ajax({
        type: "GET",
        url: "Estadistica/ObtenerDatosGrafico3",
        dataType: "json",
        data: {
            IniPeriodo: AnnEjercicio3.val(),           
            EstToken: $('#Estado3').val(),         
        },
        success: function (resp) {
            chart3.data = resp
            console.log(resp)           
        },
        complete: function () {
            $.ajax({
                type: "GET",
                url: "Estadistica/ObtenerDatosGrafico3",
                dataType: "json",
                data: {
                    IniPeriodo: AnnEjercicio3.val() - 1,
                    EstToken: $('#Estado3').val(),
                },
                success: function (resp) {
                    chart4.data = resp
                    console.log(resp)
                }
            });
        }
    });
});*/

$("#FNObtenerDatosGrafico3").click(function () {
    $.ajax({
        type: "GET",
        url: "Estadistica/ObtenerDatosGrafico3Final",
        dataType: "json",
        data: {
            IniPeriodo: AnnEjercicio3.val(),
            EstToken: $('#Estado3').val(),
        },
        success: function (resp) {
            console.log(resp);
            $("#aditionalGraficos3DivComparativa").remove();
            $("#aditionalGraficos3DivKus").remove();
            $("#aditionalGraficos3DivKus2").remove();
            if (resp && resp.Anio && resp.AnioAnterior) {
                if (resp.Comparativa && resp.Comparativa > 0) {
                    var aditionalDivComparativa = '<div id="aditionalGraficos3DivComparativa"><div class="row"><div class="col-sm-11"></div> <div class="col-sm-1">' + '<span style="color:yellow">' + resp.Comparativa + '</span></div></div></div>';
                    $("#chartdiv3").before(aditionalDivComparativa);
                }
                chart3.data = resp.Anio;
                chart4.data = resp.AnioAnterior;

                //var aditionalGraficos3DivKus = '<div id="aditionalGraficos3DivKus"><div class="row">  <div class="col-sm-1"> <span style="color:black"></span></div> <div class="col-sm-11"> <div class="row">  <div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div> <div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div> <div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div> <div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div><div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div><div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div><div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div><div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div><div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div><div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div><div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div><div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div></div ></div ></div ></div >';
                var aditionalGraficos3DivKus = '<div id="aditionalGraficos3DivKus"><div class="row">  <div class="col-sm-1"> <span style="color:black"></span></div> <div class="col-sm-11"> <div class="row">  <div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div> <div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div> <div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div> <div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div><div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div><div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div><div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div><div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div><div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div><div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div><div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div></div ></div ></div ></div >';
                $("#chartdiv4").after(aditionalGraficos3DivKus);
                var aditionalGraficos3DivKus2 = '<div id="aditionalGraficos3DivKus2"><div class="row">  <div class="col-sm-1"> <span style="color:black">&nbsp;&nbsp;&nbsp;</span></div> <div class="col-sm-11"> <div class="row"> <div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div> <div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Total (KUS$)</span></div> <div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + ((resp.TotalesKusAnio != "0") ? resp.TotalesKusAnio : "&nbsp;&nbsp;&nbsp;") + '</span></div> <div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div><div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div><div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div><div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">Total (KUS$)</span></div><div class="col-sm-1 border border-dark" style="text-align: center;">' + '<span style="color:black">' + ((resp.TotalesKusAnioAnterior && resp.TotalesKusAnioAnterior != "") ? resp.TotalesKusAnioAnterior : "&nbsp;&nbsp;&nbsp;") + '</span></div><div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div><div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div><div class="col-sm-1">' + '<span style="color:black">&nbsp;&nbsp;&nbsp;</span></div></div ></div ></div ></div >';
                $("#aditionalGraficos3DivKus").after(aditionalGraficos3DivKus2);
            }
        },
        complete: function () {

        }
    });
});



