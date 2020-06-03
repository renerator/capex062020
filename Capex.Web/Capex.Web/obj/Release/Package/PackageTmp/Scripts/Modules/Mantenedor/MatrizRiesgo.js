// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : GESTION
// FECHA            : FEBRERO 2019
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// SECCION          : CAPEX MODULO - CATEGORIA
// METODOS          :
//

var paginaSeleccionada = 1;

getUrlParams = function (prop) {
    var params = {};
    var search = decodeURIComponent(window.location.href.slice(window.location.href.indexOf('?') + 1));
    var definitions = search.split('&');

    definitions.forEach(function (val, key) {
        var parts = val.split('=', 2);
        params[parts[0]] = parts[1];
    });

    return (prop && prop in params) ? params[prop] : params;
}

FNGotoPage = function (page, perPage) {
    var startAt = (page - 1) * perPage,
        endOn = startAt + perPage;

    startAt++;
    endOn++;
    console.log("FNGotoPage startAt=" + startAt + ", endOn=" + endOn);
    $("#riesgos tr").each(function (index, value) {
        var item = $(this);
        if (index > 0) {
            if (index >= startAt && index < endOn) {
                item.attr("style", "");
                //console.log("FNGotoPage if index=", index);
            } else {
                item.attr("style", "display: none");
                //console.log("FNGotoPage else index=", index);
            }
        }
    });
    var pager = $('.pager');
    pager.children().removeClass("active");
    pager.children().eq((page - 1)).addClass("active");
}


//
// MOSTRAR ITEM DE MENU SELECCIONADO
//

//
// EVLAUR ACCIONES DEL MANTENEDOR
//
FNEvaluarAccion = function (accion, cual) {
    if (!accion || !cual) {
        return false;
    }
    else {
        paginaSeleccionada = $(".page-number.clickable.active").text();
        console.log("paginaSeleccionada=", paginaSeleccionada);
        switch (accion) {
            case "1": FNCrearRiesgo(); break;
            case "2": FNEditarRiesgo(cual); break;
            case "3": FNMostrarModalConfirmarDesactivar(cual); break;
        }
    }
}
//
// CREAR
//
FNCrearRiesgo = function () {
    $('#RiesgoNombre').val('');
    $('#EvrImpacto').val(1);
    $('#EvrProbabilidad').val(1);
    $('#RiesgoEstado').val('-1');
    $('#ModalCrearRiesgo').show();
    return false;
}
//
// GUARDAR
//
FNGuardarRiesgo = function () {
    var RiesgoNombre = $("#RiesgoNombre").val();
    var EvrImpacto = $("#EvrImpacto").val();
    var EvrProbabilidad = $("#EvrProbabilidad").val();
    var RiesgoEstado = $("#RiesgoEstado").val();
    if (!RiesgoNombre || RiesgoNombre == undefined || RiesgoNombre == "" || !EvrImpacto || !EvrProbabilidad || !RiesgoEstado || RiesgoEstado == -1) {
        swal("", "Debe completar el formulario.", "error");
        return false;
    }
    else {
        var DTO = {
            'RiesgoNombre': RiesgoNombre,
            'EvrImpacto': EvrImpacto,
            'EvrProbabilidad': EvrProbabilidad,
            'RiesgoEstado': RiesgoEstado
        }
        $.ajax({
            url: "MatrizRiesgo/Guardar",
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                //PARSEAR RESPUESTA
                var estructura = JSON.parse(JSON.stringify(r));
                estructura = estructura.Mensaje;
                //PROCESAR RESPUESTA
                if (estructura == "Error") {
                    swal("Error", "No es posible crear el riesgo.", "error");
                    $(".orderselect").val('-1').prop('selected', true);
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                } else if (estructura == "Guardado") {
                    swal("", "Riesgo creado.", "success");
                    $(".orderselect").val('-1').prop('selected', true);
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                }
            }
        });
        return false;
    }
}

function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    return true;
}

function fnc(value, min, max) {
    var responseValue = value;
    if (parseInt(value) < min || isNaN(value))
        responseValue = min;
    else if (parseInt(value) > max)
        responseValue = max;
    else
        responseValue = value;
    return responseValue;
}

//
// CERRAR MODAL CREAR
//
FNCerrarCrearRiesgo = function () {
    $('#ModalCrearRiesgo').hide()
    document.location.reload();
    return false;
}
//
// EDITAR /ACTUALIZAR
//
FNEditarRiesgo = function (cual) {
    var DTO = {
        'RiesgoToken': cual
    }
    $.ajax({
        url: "MatrizRiesgo/BuscarPorToken",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            var estructura = JSON.parse(JSON.stringify(r));
            $("#RiesgoToken_Editar").val(cual);
            $("#RiesgoNombre_Editar").val(estructura.Mensaje[2].Value);
            $("#RiesgoEvrImpacto_Editar").val(estructura.Mensaje[3].Value);
            $("#RiesgoEvrProbabilidad_Editar").val(estructura.Mensaje[4].Value);
            if (estructura.Mensaje[5].Value == "1") {
                $('#RiesgoEstado_Editar>option:eq(1)').prop('selected', true);
            }
            else if (estructura.Mensaje[5].Value == "0") {
                $('#RiesgoEstado_Editar>option:eq(2)').prop('selected', true);
            }
        }
    });
    $('#ModalActualizarRiesgo').show();
    return false;
}
//
// ACTUALIZAR CATEGORIA
//
FNActualizarRiesgo = function () {
    var RiesgoToken = $("#RiesgoToken_Editar").val();
    var RiesgoNombre = $("#RiesgoNombre_Editar").val();
    var EvrImpacto = $("#RiesgoEvrImpacto_Editar").val();
    var EvrProbabilidad = $("#RiesgoEvrProbabilidad_Editar").val();
    var RiesgoEstado = $("#RiesgoEstado_Editar").val();
    if (!RiesgoToken || RiesgoToken == undefined || RiesgoToken == "" || !RiesgoNombre || RiesgoNombre == undefined || RiesgoNombre == "" || !EvrImpacto || !EvrProbabilidad || !RiesgoEstado || RiesgoEstado == -1) {
        swal("", "Debe completar el formulario.", "error");
        return false;
    } else {
        var DTO = {
            'RiesgoToken': RiesgoToken,
            'RiesgoNombre': RiesgoNombre,
            'EvrImpacto': EvrImpacto,
            'EvrProbabilidad': EvrProbabilidad,
            'RiesgoEstado': RiesgoEstado
        }
        $.ajax({
            url: "MatrizRiesgo/Actualizar",
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                //PARSEAR RESPUESTA
                var estructura = JSON.parse(JSON.stringify(r));
                estructura = estructura.Mensaje;
                //PROCESAR RESPUESTA
                if (estructura == "Error") {
                    swal("Error", "No es posible actualizar el riesgo seleccionado.", "error");
                    $(".orderselect").val('-1').prop('selected', true);
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                } else if (estructura == "Actualizado") {
                    swal("", "Riesgo actualizado.", "success");
                    $(".orderselect").val('-1').prop('selected', true);
                    setTimeout(function () {
                        window.location.href = location.protocol + '//' + location.host + location.pathname + "?paginaSeleccionada=" + paginaSeleccionada;
                    }, 3000);
                }
            }
        });
        return false;
    }
}
//
// CERRAR ACTUALIZAR CATEGORIA
//
FNCerrarActualizarRiesgo = function () {
    $('#ModalActualizarRiesgo').hide()
    $(".orderselect").val('-1').prop('selected', true);
    document.location.reload();
    return false;
}
//
// CONFIRMAR
//
FNMostrarModalConfirmarDesactivar = function (cual) {
    $('#ModalConfirmarDesactivar').show();
    $("#RiesgoToken_Desactivar").val(cual);
    return true;
}

//
// DESACTIVAR
//
FNDesactivarRiesgo = function (cual) {
    var DTO = {
        'RiesgoToken': $("#RiesgoToken_Desactivar").val()
    }
    $.ajax({
        url: "MatrizRiesgo/Desactivar",
        type: "POST",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            //PARSEAR RESPUESTA
            var estructura = JSON.parse(JSON.stringify(r));
            estructura = estructura.Mensaje;
            //PROCESAR RESPUESTA
            if (estructura == "Error") {
                swal("Error", "No es posible desactivar el riesgo seleccionado.", "error");
                $(".orderselect").val('-1').prop('selected', true);
            } else if (estructura == "Descativado") {
                swal("", "Riesgo desactivado.", "success");
                $(".orderselect").val('-1').prop('selected', true);
                setTimeout(function () {
                    window.location.href = location.protocol + '//' + location.host + location.pathname + "?paginaSeleccionada=" + paginaSeleccionada;
                }, 3000);
            }
        }
    });
    return false;


}
//
// CERRAR MODAL DESCATIVAR
//
FNCerrarModalDesactivar = function () {
    $('#ModalConfirmarDesactivar').hide()
    $(".orderselect").val('-1').prop('selected', true);
    window.location.href = location.protocol + '//' + location.host + location.pathname + "?paginaSeleccionada=" + paginaSeleccionada;
    return false;
}

//
// INICIALIZADOR
//
$(document).ready(function () {
    //INICIALIZAR VARIABLES
    //QUITAR LOADER
    jQuery("#AppLoaderContainer").hide();
    //CONFIGURAR UI
    $("#VisualizarFiltro").click(function () {
        var estado_panel_filtro = localStorage.getItem("CAPEX_UI_PANELFILTRO");
        if (estado_panel_filtro == "" || estado_panel_filtro == "null" || estado_panel_filtro == null || estado_panel_filtro == "NO") {
            localStorage.setItem("CAPEX_UI_PANELFILTRO", "SI");
            $("#CardFiltro").show();
        }
        else if (estado_panel_filtro == "SI") {
            localStorage.setItem("CAPEX_UI_PANELFILTRO", "NO");
            $("#CardFiltro").hide();
        }
        else {
            localStorage.setItem("CAPEX_UI_PANELFILTRO", "NO");
            $("#CardFiltro").show();
        }
    });
     
});
