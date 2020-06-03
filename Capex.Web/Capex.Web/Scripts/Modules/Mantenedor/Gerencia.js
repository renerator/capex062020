// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : GESTION
// FECHA            : FEBRERO 2019
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// SECCION          : CAPEX MODULO - AREA
// METODOS          :
//

//
// MOSTRAR ITEM DE MENU SELECCIONADO
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
    $("#gerencias tr").each(function (index, value) {
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
            case "1": FNCrearGerencia(); break;
            case "2": FNEditarGerencia(cual); break;
            case "3": FNMostrarModalConfirmarDesactivar(cual); break;
            //case "4": FNMostrarModalConfirmarActivar(cual); break;
        }
    }
}
//
// CREAR
//
FNCrearGerencia = function () {
    $('#ModalCrearGerencia').show();
    return false;
}
//
// GUARDAR
//
FNGuardarGerencia = function () {
    var GerenciaNombre = $("#GerenciaNombre").val();
    var GerenciaDescripcion = $("#GerenciaDescripcion").val();
    var GerenciaEstado = $("#GerenciaEstado").val();
    if (!GerenciaNombre || !GerenciaDescripcion || !GerenciaEstado || GerenciaEstado == -1) {
        swal("", "Debe completar el formulario.", "error");
        return false;
    }
    else {
        var DTO = {
            'GerNombre': GerenciaNombre,
            'GerDescripcion': GerenciaDescripcion,
            'GerEstado': GerenciaEstado
        }
        $.ajax({
            url: "Gerencia/Guardar",
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                //PARSEAR RESPUESTA
                var estructura = JSON.parse(JSON.stringify(r));
                estructura = estructura.Mensaje;
                //PROCESAR RESPUESTA
                if (estructura == "Error") {
                    swal("Error", "No es posible crear al gerencia.", "error");
                    $(".orderselect").val('-1').prop('selected', true);
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                }
                else if (estructura == "Guardado") {
                    swal("", "Gerencia creado.", "success");
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
// CERRAR MODAL CREAR
//
FNCerrarCrearGerencia = function () {
    $('#ModalCrearGerencia').hide()
    document.location.reload();
    return false;
}
//
// EDITAR /ACTUALIZAR
//
FNEditarGerencia = function (cual) {
    var DTO = {
        'GerToken': cual
    }
    $.ajax({
        url: "Gerencia/BuscarPorToken",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            var estructura = JSON.parse(JSON.stringify(r));
            $("#GerenciaToken_Editar").val(cual);
            $("#GerenciaNombre_Editar").val(estructura.Mensaje[2].Value);
            $("#GerenciaDescripcion_Editar").val(estructura.Mensaje[3].Value);
            if (estructura.Mensaje[4].Value == "1") {
                $('#GerenciaEstado_Editar>option:eq(1)').prop('selected', true);
            }
            else if (estructura.Mensaje[4].Value == "0") {
                $('#GerenciaEstado_Editar>option:eq(2)').prop('selected', true);
            }
        }
    });
    $('#ModalActualizarGerencia').show();
    return false;
}
//
// ACTUALIZAR Gerencia
//
FNActualizarGerencia = function () {
    var GerenciaToken = $("#GerenciaToken_Editar").val();
    var GerenciaNombre = $("#GerenciaNombre_Editar").val();
    var GerenciaDescripcion = $("#GerenciaDescripcion_Editar").val();
    var GerenciaEstado = $("#GerenciaEstado_Editar").val();
    if (!GerenciaNombre || !GerenciaDescripcion || !GerenciaEstado || GerenciaEstado == -1) {
        swal("", "Debe completar el formulario.", "error");
        return false;
    } else {
        var DTO = {
            'GerToken': GerenciaToken,
            'GerNombre': GerenciaNombre,
            'GerDescripcion': GerenciaDescripcion,
            'GerEstado': GerenciaEstado
        }
        $.ajax({
            url: "Gerencia/Actualizar",
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                //PARSEAR RESPUESTA
                var estructura = JSON.parse(JSON.stringify(r));
                estructura = estructura.Mensaje;
                //PROCESAR RESPUESTA
                if (estructura == "Error") {
                    swal("Error", "No es posible actualizar al gerencia seleccionado.", "error");
                    $(".orderselect").val('-1').prop('selected', true);
                }
                else if (estructura == "Actualizado") {
                    swal("", "Gerencia actualizado.", "success");
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
// CERRAR ACTUALIZAR Gerencia
//
FNCerrarActualizarGerencia = function () {
    $('#ModalActualizarGerencia').hide()
    $(".orderselect").val('-1').prop('selected', true);
    document.location.reload();
    return false;
}
//
// CONFIRMAR
//
FNMostrarModalConfirmarDesactivar = function (cual) {
    $('#ModalConfirmarDesactivar').show();
    $("#GerenciaToken_Desactivar").val(cual);
    return true;
}

FNMostrarModalConfirmarActivar = function (cual) {
    $('#ModalConfirmarActivar').show();
    $("#GerenciaToken_Activar").val(cual);
    return true;
}

//
// DESACTIVAR
//
FNDesactivarGerencia = function (cual) {
    var DTO = {
        'GerToken': $("#GerenciaToken_Desactivar").val()
    }
    $.ajax({
        url: "Gerencia/Desactivar",
        type: "POST",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            //PARSEAR RESPUESTA
            var estructura = JSON.parse(JSON.stringify(r));
            estructura = estructura.Mensaje;
            //PROCESAR RESPUESTA
            if (estructura == "Error") {
                swal("Error", "No es posible desactivar el gerencia seleccionado.", "error");
                $(".orderselect").val('-1').prop('selected', true);
            }
            else if (estructura == "Descativado") {
                swal("", "Gerencia desactivada.", "success");
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
