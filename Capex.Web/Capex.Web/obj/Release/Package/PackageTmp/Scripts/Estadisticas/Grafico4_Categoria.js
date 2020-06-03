//
//CHARTS CONFIGURACION GRAFICO 4 - CATEGORIA
//


am4core.useTheme(am4themes_kelly);
am4core.useTheme(am4themes_animated);

var chart6 = am4core.create("chartdiv6", am4charts.PieChart);

var pieSeries = chart6.series.push(new am4charts.PieSeries());
pieSeries.dataFields.value = "Total";
pieSeries.dataFields.category = "Categoria";
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
    $("#submittername").html("");
    $.ajax({
        type: "GET",
        url: "Estadistica/ObtenerDatosGrafico4_Categoria",
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
            chart6.data = resp
            switch (radioValue) {
                case "1":
                    $("#submittername").html("Etapa (KUS$)");
                    break;
                case "2":
                    $("#submittername").html("Clasificación SSO (KUS$)");
                    break;
                case "3":
                    $("#submittername").html("Area (KUS$)");
                    break;
                case "4":
                    $("#submittername").html("Clase (KUS$)");
                    break;
                case "5":
                    $("#submittername").html("Categoria (KUS$)");
                    break;
                case "6":
                    $("#submittername").html("Estado Iniciativa (KUS$)");
                    break;
                case "7":
                    $("#submittername").html("Macrocategoria (KUS$)");
                    break;
                case "8":
                    $("#submittername").html("Gerencia Ejecutora (KUS$)");
                    break;
                default:
                    $("#submittername").html("");
            } 
        }

    });
});




