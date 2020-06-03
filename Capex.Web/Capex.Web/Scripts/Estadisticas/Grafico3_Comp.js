//
//CHARTS CONFIGURACION GRAFICO 3 ""COMPARACION""
//

am4core.useTheme(am4themes_animated);


var chart4 = am4core.create("chartdiv4", am4charts.XYChart);

// Create axes
var categoryAxis = chart4.xAxes.push(new am4charts.CategoryAxis());
categoryAxis.dataFields.category = "year";
categoryAxis.renderer.grid.template.location = 0;

var valueAxis = chart4.yAxes.push(new am4charts.ValueAxis());
valueAxis.renderer.inside = true;
valueAxis.renderer.labels.template.disabled = true;
valueAxis.min = 0;

// Create series
function createSeries(field, name) {

    // Set up series
    var series = chart4.series.push(new am4charts.ColumnSeries());
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
//createSeries("EX2", "Extraordinarios2");


// Legend
chart4.legend = new am4charts.Legend();



