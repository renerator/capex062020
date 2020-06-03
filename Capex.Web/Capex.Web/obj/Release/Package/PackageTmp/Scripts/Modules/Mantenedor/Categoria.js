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
    $("#categorias tr").each(function (index, value) {
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
            case "1": FNCrearCategoria(); break;
            case "2": FNEditarCategoria(cual); break;
            case "3": FNMostrarModalConfirmarDesactivar(cual); break;
        }
    }
}
//
// CREAR
//
FNCrearCategoria = function () {
    $('#ModalCrearCategoria').show();
    return false;
}
//
// GUARDAR
//
FNGuardarCategoria = function () {
    var CatNombre = $("#CatNombre").val();
    var CatDescripcion = $("#CatDescripcion").val();
    var CatEstado = $("#CatEstado").val();
    if (!CatNombre || !CatDescripcion || !CatEstado || CatEstado == -1) {
        swal("", "Debe completar el formulario.", "error");
        return false;
    }
    else {
        var DTO = {
            'CatNombre': CatNombre,
            'CatDescripcion': CatDescripcion,
            'CatEstado': CatEstado
        }
        $.ajax({
            url: "Categoria/Guardar",
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                //PARSEAR RESPUESTA
                var estructura = JSON.parse(JSON.stringify(r));
                estructura = estructura.Mensaje;
                //PROCESAR RESPUESTA
                if (estructura == "Error") {
                    swal("Error", "No es posible crear la categoria.", "error");
                    $(".orderselect").val('-1').prop('selected', true);
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                }
                else if (estructura == "Guardado") {
                    swal("", "Categoria creada.", "success");
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
FNCerrarCrearCategoria = function () {
    $('#ModalCrearCategoria').hide()
    document.location.reload();
    return false;
}
//
// EDITAR /ACTUALIZAR
//
FNEditarCategoria = function (cual) {
    var DTO = {
        'CatToken': cual
    }
    $.ajax({
        url: "Categoria/BuscarPorToken",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            var estructura = JSON.parse(JSON.stringify(r));
            $("#CatToken_Editar").val(cual);
            $("#CatNombre_Editar").val(estructura.Mensaje[2].Value);
            $("#CatDescripcion_Editar").val(estructura.Mensaje[3].Value);
            if (estructura.Mensaje[4].Value == "1") {
                $('#CatEstado_Editar>option:eq(1)').prop('selected', true);
            }
            else if (estructura.Mensaje[4].Value == "0") {
                $('#CatEstado_Editar>option:eq(2)').prop('selected', true);
            }
        }
    });
    $('#ModalActualizarCategoria').show();
    return false;
}
//
// ACTUALIZAR CATEGORIA
//
FNActualizarCategoria = function () {
    var CatToken = $("#CatToken_Editar").val();
    var CatNombre = $("#CatNombre_Editar").val();
    var CatDescripcion = $("#CatDescripcion_Editar").val();
    var CatEstado = $("#CatEstado_Editar").val();
    if (!CatToken || !CatNombre || !CatDescripcion || !CatEstado || CatEstado == -1) {
        swal("", "Debe completar el formulario.", "error");
        return false;
    } else {
        var DTO = {
            'CatToken': CatToken,
            'CatNombre': CatNombre,
            'CatDescripcion': CatDescripcion,
            'CatEstado': CatEstado
        }
        $.ajax({
            url: "Categoria/Actualizar",
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                //PARSEAR RESPUESTA
                var estructura = JSON.parse(JSON.stringify(r));
                estructura = estructura.Mensaje;
                //PROCESAR RESPUESTA
                if (estructura == "Error") {
                    swal("Error", "No es posible actualizar la categoria seleccionada.", "error");
                    $(".orderselect").val('-1').prop('selected', true);
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                }
                else if (estructura == "Actualizado") {
                    swal("", "Categoria actualizada.", "success");
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
FNCerrarActualizarCategoria = function () {
    $('#ModalActualizarCategoria').hide()
    $(".orderselect").val('-1').prop('selected', true);
    document.location.reload();
    return false;
}
//
// CONFIRMAR
//
FNMostrarModalConfirmarDesactivar = function (cual) {
    $('#ModalConfirmarDesactivar').show();
    $("#CategoriaToken_Desactivar").val(cual);
    return true;
}

//
// DESACTIVAR
//
FNDesactivarCategoria = function (cual) {
    /*$('#ModalConfirmarDesactivar').hide();
    swal("", "Categoria desactivada.", "success");
    setTimeout(function () {
        document.location.reload();
    }, 3000);
    return false;*/

    var DTO = {
        'CatToken': $("#CategoriaToken_Desactivar").val()
    }
    $.ajax({
        url: "Categoria/Desactivar",
        type: "POST",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            //PARSEAR RESPUESTA
            var estructura = JSON.parse(JSON.stringify(r));
            estructura = estructura.Mensaje;
            //PROCESAR RESPUESTA
            if (estructura == "Error") {
                swal("Error", "No es posible desactivar la categoria seleccionada.", "error");
                $(".orderselect").val('-1').prop('selected', true);
            }
            else if (estructura == "Descativado") {
                swal("", "Categoria desactivada.", "success");
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
