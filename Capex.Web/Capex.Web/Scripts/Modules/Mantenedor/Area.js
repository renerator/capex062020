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
    $("#areas tr").each(function (index, value) {
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
            case "1": FNCrearArea(); break;
            case "2": FNEditarArea(cual); break;
            case "3": FNMostrarModalConfirmarDesactivar(cual); break;
            //case "4": FNMostrarModalConfirmarActivar(cual); break;
        }
    }
}
//
// CREAR
//
FNCrearArea = function () {
    $('#ModalCrearArea').show();
    return false;
}
//
// GUARDAR
//
FNGuardarArea = function () {
    var AreaNombre = $("#AreaNombre").val();
    var AreaAcronimo = $("#AreaAcronimo").val();
    var AreaDescripcion = $("#AreaDescripcion").val();
    var AreaEstado = $("#AreaEstado").val();
    if (!AreaNombre || !AreaAcronimo || !AreaDescripcion || !AreaEstado || AreaEstado == -1) {
        swal("", "Debe completar el formulario.", "error");
        return false;
    } else {
        var DTO = {
            'AreaNombre': AreaNombre,
            'AreaAcronimo': AreaAcronimo,
            'AreaDescripcion': AreaDescripcion,
            'AreaEstado': AreaEstado
        }
        $.ajax({
            url: "Area/Guardar",
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                //PARSEAR RESPUESTA
                var estructura = JSON.parse(JSON.stringify(r));
                estructura = estructura.Mensaje;
                //PROCESAR RESPUESTA
                if (estructura == "Error") {
                    swal("Error", "No es posible crear el area.", "error");
                    $(".orderselect").val('-1').prop('selected', true);
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                }
                else if (estructura == "Guardado") {
                    swal("", "Area creada.", "success");
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
FNCerrarCrearArea = function () {
    $('#ModalCrearArea').hide()
    document.location.reload();
    return false;
}
//
// EDITAR /ACTUALIZAR
//
FNEditarArea = function (cual) {
    var DTO = {
        'AreaToken': cual
    }
    $.ajax({
        url: "Area/BuscarPorToken",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            var estructura = JSON.parse(JSON.stringify(r));
            $("#AreaToken_Editar").val(cual);
            $("#AreaNombre_Editar").val(estructura.Mensaje[2].Value);
            $("#AreaAcronimo_Editar").val(estructura.Mensaje[3].Value);
            $("#AreaDescripcion_Editar").val(estructura.Mensaje[4].Value);
            if (estructura.Mensaje[5].Value == "1") {
                $('#AreaEstado_Editar>option:eq(1)').prop('selected', true);
            }
            else if (estructura.Mensaje[5].Value == "0") {
                $('#AreaEstado_Editar>option:eq(2)').prop('selected', true);
            }
        }
    });
    $('#ModalActualizarArea').show();
    return false;
}
//
// ACTUALIZAR Area
//
FNActualizarArea = function () {
    var AreaToken = $("#AreaToken_Editar").val();
    var AreaNombre = $("#AreaNombre_Editar").val();
    var AreaAcronimo = $("#AreaAcronimo_Editar").val();
    var AreaDescripcion = $("#AreaDescripcion_Editar").val();
    var AreaEstado = $("#AreaEstado_Editar").val();
    if (!AreaNombre || !AreaAcronimo || !AreaDescripcion || !AreaEstado || AreaEstado == -1) {
        swal("", "Debe completar el formulario.", "error");
        return false;
    } else {
        var DTO = {
            'AreaToken': AreaToken,
            'AreaNombre': AreaNombre,
            'AreaAcronimo': AreaAcronimo,
            'AreaDescripcion': AreaDescripcion,
            'AreaEstado': AreaEstado
        }
        $.ajax({
            url: "Area/Actualizar",
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                //PARSEAR RESPUESTA
                var estructura = JSON.parse(JSON.stringify(r));
                estructura = estructura.Mensaje;
                //PROCESAR RESPUESTA
                if (estructura == "Error") {
                    swal("Error", "No es posible actualizar el area seleccionada.", "error");
                    $(".orderselect").val('-1').prop('selected', true);
                }
                else if (estructura == "Actualizado") {
                    swal("", "Area actualizada.", "success");
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
// CERRAR ACTUALIZAR Area
//
FNCerrarActualizarArea = function () {
    $('#ModalActualizarArea').hide()
    $(".orderselect").val('-1').prop('selected', true);
    document.location.reload();
    return false;
}
//
// CONFIRMAR
//
FNMostrarModalConfirmarDesactivar = function (cual) {
    $('#ModalConfirmarDesactivar').show();
    $("#AreaToken_Desactivar").val(cual);
    return true;
}

FNMostrarModalConfirmarActivar = function (cual) {
    $('#ModalConfirmarActivar').show();
    $("#AreaToken_Activar").val(cual);
    return true;
}

//
// DESACTIVAR
//
FNDesactivarArea = function (cual) {
    var DTO = {
        'AreaToken': $("#AreaToken_Desactivar").val()
    }
    $.ajax({
        url: "Area/Desactivar",
        type: "POST",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            //PARSEAR RESPUESTA
            var estructura = JSON.parse(JSON.stringify(r));
            estructura = estructura.Mensaje;
            //PROCESAR RESPUESTA
            if (estructura == "Error") {
                swal("Error", "No es posible desactivar el area seleccionada.", "error");
                $(".orderselect").val('-1').prop('selected', true);
            }
            else if (estructura == "Descativado") {
                swal("", "Area desactivada.", "success");
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
// DESACTIVAR
//
FNActivarArea = function (cual) {
    var DTO = {
        'AreaToken': $("#AreaToken_Activar").val()
    }
    $.ajax({
        url: "Area/Activar",
        type: "POST",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            //PARSEAR RESPUESTA
            var estructura = JSON.parse(JSON.stringify(r));
            estructura = estructura.Mensaje;
            //PROCESAR RESPUESTA
            if (estructura == "Error") {
                swal("Error", "No es posible activar el area seleccionada.", "error");
                $(".orderselect").val('-1').prop('selected', true);
            }
            else if (estructura == "Activado") {
                swal("", "Area activada.", "success");
                $(".orderselect").val('-1').prop('selected', true);
                setTimeout(function () {
                    window.location.href = location.protocol + '//' + location.host + location.pathname + "?paginaSeleccionada=" + paginaSeleccionada;
                }, 3000);
            }
        }
    });
    return false;
}

FNCerrarModalActivar = function () {
    $('#ModalConfirmarActivar').hide()
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
