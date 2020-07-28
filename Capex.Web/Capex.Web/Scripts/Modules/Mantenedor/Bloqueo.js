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
    $("#usuarios tr").each(function (index, value) {
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


FNCheckDownKey = function (key) {
    console.log("inputValue=", $('#EveTir').val());
    if ($('#EveTir').val().trim() == "" || $('#EveTir').val().trim() == "%") {
        $('#EveTir').val("0%");
    }
}

FNCheckDownKey2 = function (selector) {
    console.log("FNCheckDownKey2 inputValue=", $(('#' + selector)).val());
    if ($(('#' + selector)).val() == "") {
        $(('#' + selector)).val("0,00");
    }
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
            case "1": FNCrearBloqueo(); break;
            case "2": FNEditarTemplate(cual); break;
            case "3": FNMostrarModalConfirmarDesactivar(cual); break;
        }
    }
}
//
// CREAR
//
FNCrearBloqueo = function () {
    $('#datepickerFDesde').val('');
    $('#datepickerFHasta').val('');
    $('#ModalCrearBloqueo').show();
    return false;
}
//
// GUARDAR
//
FNGuardarBloqueo = function () {
    var fDesde = $('#datepickerFDesde').val();
    var fHasta = $('#datepickerFHasta').val();
    var radioValue = $("input[name='optradio']:checked").val();

    if (!fDesde || fDesde == undefined || fDesde == "" || !fHasta || fHasta == undefined || fHasta == "") {
        swal("", "Debe completar el formulario.", "error");
        return false;
    }

    if (compare(fDesde, fHasta) == 1) {
        swal("", "La fecha desde debe ser menor que la fecha hasta.", "error");
        return false;
    } else {
        var DTO = {
            'TipoIniciativaSeleccionado': radioValue,
            'FechaDesde': fDesde,
            'FechaHasta': fHasta
        }
        $.ajaxSetup({ cache: false });
        $.ajax({
            url: "Bloqueo/Guardar",
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                //PARSEAR RESPUESTA
                var estructura = JSON.parse(JSON.stringify(r));
                estructura = estructura.Mensaje;
                //DESCOMPONER RESPUESTA
                var parte = estructura.split("|");
                var codigo = "";
                var estado = "";
                var mensaje = "";
                if (parte && parte.length >= 2) {
                    codigo = parte[0];
                    estado = parte[1];
                    if (parte.length > 2) {
                        mensaje = parte[2];
                    }
                }
                //PROCESAR RESPUESTA
                if (estado == "Error") {
                    $(".orderselect").val('-1').prop('selected', true);
                    if (mensaje != "") {
                        swal("Error", mensaje, "error");
                    } else {
                        swal("Error", "No es posible crear el template.", "error");
                        setTimeout(function () {
                            document.location.reload();
                        }, 3000);
                    }
                } else if (estado == "Guardado") {
                    $('#ModalCrearBloqueo').hide();
                    $(".orderselect").val('-1').prop('selected', true);
                    swal("", "Bloqueo creado.", "success");
                    setTimeout(function () {
                        window.location.href = location.protocol + '//' + location.host + location.pathname + "?tipoIniciativaSeleccionado=" + radioValue;
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
FNCerrarCrearBloqueo = function () {
    $('#ModalCrearBloqueo').hide()
    document.location.reload();
    return false;
}

FNErrorTamanioArchivo = function () {
    swal("", "EL archivo no debe superar los 10MB de tamaño,.", "info");
}

FNChechFechaBloqueo = function () {
    var fDesde = $('#datepickerFDesde').val();
    var fHasta = $('#datepickerFHasta').val();
    console.log("FNChechFechaBloqueo fDesde=" + fDesde + ", fHasta=" + fHasta);
    if (!fDesde || fDesde == undefined || fDesde == "" || !fHasta || fHasta == undefined || fHasta == "") {
        setTimeout(function () { $('#guadarBloqueo').prop('disabled', true); }, 500);
        return;
    }
    setTimeout(function () { $('#guadarBloqueo').prop('disabled', false); }, 500);
    return;
}

FNInicioSubidaDocumentTemplate = function () {
    $("#AppLoaderContainer").show();
}

FNCallBackUploadDocumentTemplate = function (paramJson) {
    $("#AppLoaderContainer").hide();
    if (paramJson.Data.code == "0") {
        swal("Exito", "Archivo template subido correctamente", "success");
    } else {
        swal("Error", "Error al intentar subir template.", "error");
    }
}

FNCallBackUploadDocumentExcelTemplate = function (paramJson) {
    $("#AppLoaderContainer").hide();
    if (paramJson.Data.code == "0") {
        swal("Exito", "Archivo template subido correctamente", "success");
        setTimeout(function () {
            window.location.href = location.protocol + '//' + location.host + location.pathname + "?paginaSeleccionada=" + paginaSeleccionada;
        }, 2500);
    } else {
        swal("Error", "Error al intentar subir template.", "error");
    }
}

function compare(dateTimeA, dateTimeB) {
    var momentA = moment(dateTimeA, "DD-MM-YYYY");
    var momentB = moment(dateTimeB, "DD-MM-YYYY");
    if (momentA > momentB) return 1;
    else if (momentA < momentB) return -1;
    else return 0;
}

//
// EDITAR /ACTUALIZAR
//
FNEditarTemplate = function (cual) {
    var radioValue = $("input[name='optradio']:checked").val();
    var DTO = {
        'BloqueoToken': cual,
        'TipoIniciativaSeleccionado': radioValue
    }
    $('#FechaBloqueoToken').val('');
    $('#datepickerMFDesde').val('');
    $('#datepickerMFHasta').val('');

    $('#actualizarBloqueo').prop('disabled', true);
    $.ajaxSetup({ cache: false });
    $.ajax({
        url: "Bloqueo/BuscarPorToken",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            var estructura = JSON.parse(JSON.stringify(r));
            console.log("JSON.stringify(r)", JSON.stringify(r));
            if (estructura && estructura.Mensaje && estructura.Mensaje == "Ok" && estructura.Resultados && estructura.Resultados.length > 0) {
                
                $('#FechaBloqueoToken').val(estructura.Resultados[0].FechaBloqueoToken);
                $('#datepickerMFDesde').datepicker("setDate", new Date(estructura.Resultados[0].FechaDesde.split("/")[2], (estructura.Resultados[0].FechaDesde.split("/")[1] - 1), estructura.Resultados[0].FechaDesde.split("/")[0]));
                $('#datepickerMFHasta').datepicker("setDate", new Date(estructura.Resultados[0].FechaHasta.split("/")[2], (estructura.Resultados[0].FechaHasta.split("/")[1] - 1), estructura.Resultados[0].FechaHasta.split("/")[0]));
 
                setTimeout(function () { $('#actualizarBloqueo').prop('disabled', false); }, 500);
            }
        }
    });
    $('#ModalActualizarBloqueo').show();
    return false;
}
//
// ACTUALIZAR CATEGORIA
//
FNActualizarBloqueo = function () {
    var fechaBloqueoToken = $('#FechaBloqueoToken').val();
    var fDesde = $('#datepickerMFDesde').val();
    var fHasta = $('#datepickerMFHasta').val();
    var radioValue = $("input[name='optradio']:checked").val();

    if (!fDesde || fDesde == undefined || fDesde == "" || !fHasta || fHasta == undefined || fHasta == "") {
        swal("", "Debe completar el formulario.", "error");
        return false;
    }
    if (compare(fDesde, fHasta) == 1) {
        swal("", "La fecha desde debe ser menor que la fecha hasta.", "error");
        return false;
    } else {
        var DTO = {
            'TipoIniciativaSeleccionado': radioValue,
            'FechaBloqueoToken': fechaBloqueoToken,
            'FechaDesde': fDesde,
            'FechaHasta': fHasta
        }
        $.ajaxSetup({ cache: false });
        $.ajax({
            url: "Bloqueo/Actualizar",
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                //PARSEAR RESPUESTA
                var estructura = JSON.parse(JSON.stringify(r));
                estructura = estructura.Mensaje;
                //PROCESAR RESPUESTA
                if (estructura == "Actualizado") {
                    swal("", "Bloqueo actualizado.", "success");
                    $(".orderselect").val('-1').prop('selected', true);
                    setTimeout(function () {
                        window.location.href = location.protocol + '//' + location.host + location.pathname + "?paginaSeleccionada=" + paginaSeleccionada;
                    }, 3000);
                } else if (estructura.startsWith("Error") && estructura.length > 5) {
                    swal("Error", "No es posible actualizar el bloqueo seleccionado.", "error");
                    $(".orderselect").val('-1').prop('selected', true);
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                } else if (estructura == "Error") {
                    swal("Error", "No es posible actualizar el bloqueo seleccionado.", "error");
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
//
// CERRAR ACTUALIZAR CATEGORIA
//
FNCerrarActualizarBloqueo = function () {
    $('#ModalActualizarBloqueo').hide();
    $(".orderselect").val('-1').prop('selected', true);
    var radioValue = $("input[name='optradio']:checked").val();
    console.log("radioValue=", radioValue);
    setTimeout(function () {
        window.location.href = location.protocol + '//' + location.host + location.pathname + "?tipoIniciativaSeleccionado=" + radioValue;
    }, 1000);
    return false;
}


FNCerrarActualizarExcelTemplate = function () {
    $('#ModalActualizarExcelTemplate').hide();
    var radioValue = $("input[name='optradio']:checked").val();
    console.log("radioValue=", radioValue);
    setTimeout(function () {
        window.location.href = location.protocol + '//' + location.host + location.pathname + "?tipoIniciativaSeleccionado=" + radioValue;
    }, 1000);
    return false;
}

FNErrorTipoIniciativa = function () {
    swal("", "Seleccione un tipo iniciativa.", "info");
}

FNTipoIniciativaSeleccionada = function () {
    var radioValue = $("input[name='optradio']:checked").val();
    return radioValue;
}

FNTemplateToken = function () {
    return $('#itPEToken').val();
}

//
// CONFIRMAR
//
FNMostrarModalConfirmarDesactivar = function (cual) {
    $('#ModalConfirmarDesactivar').show();
    $("#BloqueoToken_Desactivar").val(cual);
    return true;
}

//
// DESACTIVAR
//
FNDesactivarBloqueo = function (cual) {
    var DTO = {
        'BloqueoToken': $("#BloqueoToken_Desactivar").val()
    }
    var radioValue = $("input[name='optradio']:checked").val();
    console.log("FNDesactivarBloqueo radioValue=", radioValue);
    $.ajax({
        url: "Bloqueo/Desactivar",
        type: "POST",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            //PARSEAR RESPUESTA
            var estructura = JSON.parse(JSON.stringify(r));
            estructura = estructura.Mensaje;

            //PROCESAR RESPUESTA
            if (estructura == "Error") {
                swal("Error", "No es posible eliminar el bloqueo seleccionado.", "error");
                $(".orderselect").val('-1').prop('selected', true);
                setTimeout(function () {
                    document.location.reload();
                }, 3000);
            } else if (estructura == "Descativado") {
                swal("", "Bloqueo eliminado.", "success");
                $(".orderselect").val('-1').prop('selected', true);
                setTimeout(function () {
                    window.location.href = location.protocol + '//' + location.host + location.pathname + "?tipoIniciativaSeleccionado=" + radioValue + "&paginaSeleccionada=" + paginaSeleccionada;
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
    var radioValue = $("input[name='optradio']:checked").val();
    console.log("FNCerrarModalDesactivar radioValue=", radioValue);
    window.location.href = location.protocol + '//' + location.host + location.pathname + "?tipoIniciativaSeleccionado=" + radioValue + "&paginaSeleccionada=" + paginaSeleccionada;
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
        } else if (estado_panel_filtro == "SI") {
            localStorage.setItem("CAPEX_UI_PANELFILTRO", "NO");
            $("#CardFiltro").hide();
        } else {
            localStorage.setItem("CAPEX_UI_PANELFILTRO", "NO");
            $("#CardFiltro").show();
        }
    });

});