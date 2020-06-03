// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : GESTION
// FECHA            : FEBRERO 2019
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------


FNObtenerMatrizRiesgo = function (cargar) {
    var DTO = {};
    $.ajax({
        url: "../../Planificacion/ListarMatrizRiesgo",
        type: "GET",
        dataType: "json",
        data: (DTO),
        async: false,
        success: function (r) {
            if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                document.getElementById('linkToLogout').click();
                return;
            }
            var contador = 1;
            $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                if (cargar && cargar != undefined && cargar != "0" && cargar == value.IdMatrizRiesgo) {
                    FNSeleccionarItemMatrizRiesgo(contador, value.IdMatrizRiesgo, value.MatrizRiesgoNombre, value.MatrizRiesgoImpacto, value.MatrizRiesgoProbabilidad);
                }
                contador++;
            });
        }
    });
}

FNSeleccionarItemMatrizRiesgo = function (contador, rr, glosa, impacto, probabilidad) {
    $("#ConetenidoRiesgoClaveEnPantalla").html("<strong>RR:</strong> " + contador + " <strong>Impacto:</strong> " + impacto + " <strong>Probabilidad:</strong> " + probabilidad + " <br><span style='margin-left:30px;'><blockquote><strong>Riesgo Clave:</strong> " + glosa + "</blockquote></span>");
    $("#divMatrizRiesgo").show();
}

//
// GRAFICOS DE COMPROBACION 1
//
FNMostrarModalGraficoComprobacion1 = function () {
    var estado_importacion = localStorage.getItem("CAPEX_INICIATIVA_PRESUPUESTO_ESTADO");
    if (estado_importacion == "Guardado") {
        FNGraficoComprobacionValorEstimadoBase();
        $("#ModalGraficoComprobacion1").show();
        return;
    }
    else {
        swal("", "Debe cargar template para realizar comprobación", "info");
        return false;
    }
    return
}
FNCerrarModalGraficoComprobacion1 = function () {
    $("#ModalGraficoComprobacion1").hide();
    return
}
FNGraficoComprobacionValorEstimadoBase = function () {
    var data1 = [0, 0, 1, 2, 8, 9, 16, 10, 20, 13, 13, 3, 100];
    new Chart(document.getElementById("GC1"), {
        type: 'line',
        data: {
            labels: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic", "Sgtes"],
            datasets: [{
                data: data1,
                label: "Físico",
                borderColor: "#a52e2e",
                fill: true
            }, {
                data: [0, 2, 3, 15, 14, 18, 24, 42, 58, 76, 91, 97, 100],
                label: "Financiero",
                borderColor: "#288aed",
                fill: true
            }]
        },
        options: {
            title: {
                display: true,
                text: 'Total Valor Estimado Base - Avance Físico v/s Financiero'
            }
        }
    });
}
//
// GRAFICOS DE COMPROBACION 1
//
FNMostrarModalGraficoComprobacion1 = function () {
    var estado_importacion = localStorage.getItem("CAPEX_INICIATIVA_PRESUPUESTO_ESTADO");
    if (estado_importacion != "Guardado") {
        FNGraficoComprobacionValorEstimadoBase();
        $("#ModalGraficoComprobacion1").show();
        return;
    }
    else {
        swal("", "Debe cargar template para realizar comprobación", "info");
        return false;
    }
    return
}
FNCerrarModalGraficoComprobacion1 = function () {
    $("#ModalGraficoComprobacion1").hide();
    return
}

FNGraficoComprobacionValorEstimadoBase = function () {
    $.ajax({
        type: "GET",
        url: "../../Estadistica/ObtenerDatosGraficoValorEstimadoBase",
        dataType: "json",
        data: {
            "token": localStorage.getItem("CAPEX_INICIATIVA_TOKEN")
        },
        success: function (resp) {
            console.log(resp)
            CargarDatosGraficoComprobacionValorEstimadoBase(resp.data1, resp.data2);
        }
    });
}

CargarDatosGraficoComprobacionValorEstimadoBase = function (data1, data2) {
    new Chart(document.getElementById("GC1"), {
        type: 'line',
        data: {
            labels: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic", "Sgtes"],
            datasets: [{
                data: data1,
                label: "Físico",
                borderColor: "#a52e2e",
                fill: true
            }, {
                data: data2,
                label: "Financiero",
                borderColor: "#288aed",
                fill: true
            }]
        },
        options: {
            title: {
                display: true,
                text: 'Total Valor Estimado Base - Avance Físico v/s Financiero'
            }
        }
    });
}

//
// GRAFICOS DE COMPROBACION 2
//
FNMostrarModalGraficoComprobacion2 = function () {
    FNGraficoFaseIngenieria();
    $("#ModalGraficoComprobacion2").show();
}
FNCerrarModalGraficoComprobacion2 = function () {
    $("#ModalGraficoComprobacion2").hide();
    return
}

FNGraficoFaseIngenieria = function () {
    $.ajax({
        type: "GET",
        url: "../../Estadistica/ObtenerDatosGraficoValorIngenieria",
        dataType: "json",
        data: {
            "token": localStorage.getItem("CAPEX_INICIATIVA_TOKEN")
        },
        success: function (resp) {
            console.log(resp)
            CargarDatosGraficoComprobacionValorIngenieria(resp.data1, resp.data2);
        }
    });
}

CargarDatosGraficoComprobacionValorIngenieria = function (data1, data2) {
    new Chart(document.getElementById("GC2"), {
        type: 'line',
        data: {
            labels: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic", "Sgtes"],
            datasets: [{
                data: data1,
                label: "Físico",
                borderColor: "#a52e2e",
                fill: true
            }, {
                data: data2,
                label: "Financiero",
                borderColor: "#288aed",
                fill: true
            }]
        },
        options: {
            title: {
                display: true,
                text: 'Fase Ingeniería - Avance Físico v/s Financiero'
            }
        }
    });
}

//
// GRAFICOS DE COMPROBACION 3
//

CargarDatosGraficoFaseAdquisiciones = function (data1, data2) {
    new Chart(document.getElementById("GC3"), {
        type: 'line',
        data: {
            labels: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic", "Sgtes"],
            datasets: [{
                data: data1,
                label: "Físico",
                borderColor: "#a52e2e",
                fill: true
            }, {
                data: data2,
                label: "Financiero",
                borderColor: "#288aed",
                fill: true
            }]
        },
        options: {
            title: {
                display: true,
                text: 'Fase Adquisiciones - Avance Físico v/s Financiero'
            }
        }
    });
}

FNGraficoFaseAdquisiciones = function () {
    $.ajax({
        type: "GET",
        url: "../../Estadistica/ObtenerDatosGraficoValorAdquisiciones",
        dataType: "json",
        data: {
            "token": localStorage.getItem("CAPEX_INICIATIVA_TOKEN")
        },
        success: function (resp) {
            console.log(resp)
            CargarDatosGraficoFaseAdquisiciones(resp.data1, resp.data2);
        }
    });
}

FNMostrarModalGraficoComprobacion3 = function () {
    FNGraficoFaseAdquisiciones();
    $("#ModalGraficoComprobacion3").show();
    return;
}

FNCerrarModalGraficoComprobacion3 = function () {
    $("#ModalGraficoComprobacion3").hide();
    return;
}

//
// GRAFICOS DE COMPROBACION 4
//
FNMostrarModalGraficoComprobacion4 = function () {
    FNGraficoFaseConstruccion();
    $("#ModalGraficoComprobacion4").show();
    return;
}
FNCerrarModalGraficoComprobacion4 = function () {
    $("#ModalGraficoComprobacion4").hide();
    return
}


FNGraficoFaseConstruccion = function () {
    $.ajax({
        type: "GET",
        url: "../../Estadistica/ObtenerDatosGraficoValorConstruccion",
        dataType: "json",
        data: {
            "token": localStorage.getItem("CAPEX_INICIATIVA_TOKEN")
        },
        success: function (resp) {
            console.log(resp)
            CargarDatosGraficoFaseConstruccion(resp.data1, resp.data2);
        }
    });
}

CargarDatosGraficoFaseConstruccion = function (data1, data2) {
    new Chart(document.getElementById("GC4"), {
        type: 'line',
        data: {
            labels: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic", "Sgtes"],
            datasets: [{
                data: data1,
                label: "Físico",
                borderColor: "#a52e2e",
                fill: true
            }, {
                data: data2,
                label: "Financiero",
                borderColor: "#288aed",
                fill: true
            }]
        },
        options: {
            title: {
                display: true,
                text: 'Fase Construcción - Avance Físico v/s Financiero'
            }
        }
    });
}
