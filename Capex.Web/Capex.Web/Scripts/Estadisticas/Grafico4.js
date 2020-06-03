//
//CHARTS CONFIGURACION GRAFICO 4
//


am4core.useTheme(am4themes_animated);

var chart5 = am4core.create("chartdiv5", am4charts.PieChart);

var pieSeries = chart5.series.push(new am4charts.PieSeries());
pieSeries.dataFields.value = "totales";
pieSeries.dataFields.category = "tipo";
pieSeries.slices.template.stroke = am4core.color("#fff");
pieSeries.slices.template.strokeWidth = 2;
pieSeries.slices.template.strokeOpacity = 1;

// This creates initial animation
pieSeries.hiddenState.properties.opacity = 1;
pieSeries.hiddenState.properties.endAngle = -90;
pieSeries.hiddenState.properties.startAngle = -90;





//
// OBTENER DATOS GRAFICO 4
//

$("#FNObtenerDatosGrafico4").click(function () {
    var radioValue = $("input[name='groupOfDefaultRadios']:checked").val();
    console.info('radioValue=', radioValue);
    if (!radioValue) {
        console.info('return 1');
        return;
    }
    var anio = AnnEjercicio4.val();
    console.info('anio=', anio);
    if (!anio || anio == '0') {
        console.info('return 2');
        return;
    }
    $.ajax({
        type: "GET",
        url: "Estadistica/ObtenerDatosGrafico4",
        dataType: "json",
        data: {
            IniPeriodo: AnnEjercicio4.val(),
            Opcion: radioValue
            /*AreaToken: Area4.val(),
            IniPeriodo: AnnEjercicio4.val(),
            NIAcronimo: Etapa4.val(),
            CSToken: Sso4.val(),
            EssToken: EstandarSeguridad4.val(),
            EstToken: Estado4.val(),
            CatToken: Categoria4.val(),
            EstadoProyecto: $('#EstadoProyecto4').val(),
            Clase: $('#Clase4').val(),
            Macrocategoria: $('#Macrocategoria4').val()*/

        },
        success: function (resp) {
            console.log(resp)
            chart5.data = resp

        }

    });
});




