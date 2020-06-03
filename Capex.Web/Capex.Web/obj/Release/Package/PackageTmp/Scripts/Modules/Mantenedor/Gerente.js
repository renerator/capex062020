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

//
// LISTADO DE GERENCIAS
//
FNObtenerListadoGerencia = function (selector) {
    console.log("FNObtenerListadoGerencia selector=", selector);
    //LISTO
    var DTO = {};
    $.ajax({
        url: "/Planificacion/ListarGerencias",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            setTimeout(function () {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                console.log("FNObtenerListadoGerencia");
                console.log("FNObtenerListadoGerencia r=" + r);
                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    console.log("FNObtenerListadoGerencia value.GerNombre=" + value.GerNombre + ", value.IdGerencia=" + value.IdGerencia);
                    $('#' + selector).append(new Option(value.GerNombre, value.IdGerencia, false, false));
                });
                $('#' + selector).prop('selectedIndex', 0);
            }, 500);
        }
    });
}


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
    $("#gerentes tr").each(function (index, value) {
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
            case "1": FNCrearGerente(); break;
            case "2": FNEditarGerente(cual); break;
            case "3": FNMostrarModalConfirmarDesactivar(cual); break;
            //case "4": FNMostrarModalConfirmarActivar(cual); break;
        }
    }
}
//
// CREAR
//
FNCrearGerente = function () {
    $('#ModalCrearGerente').show();
    return false;
}

FNRealizarValidacionGerenciaGerente = function (Gerencia, GerenteToken) {
    var response = '';
    var DTO = {
        'GteToken': GerenteToken,
        'IdGerencia': Gerencia
    }
    $.ajax({
        url: "Gerente/ValidarGerenciaGerenteActivo",
        type: "POST",
        async: false,
        dataType: "json",
        data: (DTO),
        success: function (r) {
            if (r && r.Mensaje) {
                response = r.Mensaje;
            }
        }
    });
    return response;
}

FNRealizarValidacionGerencia = function (Gerencia) {
    var response = '';
    $.ajax({
        url: "Gerente/ValidarActivo",
        type: "POST",
        async: false,
        dataType: "json",
        data: ({ Gerencia: Gerencia }),
        success: function (r) {
            if (r && r.Mensaje) {
                response = r.Mensaje;
            }
        }
    });
    return response;
}
//
// GUARDAR
//
FNGuardarGerente = function () {
    var GerenteNombre = $("#GerenteNombre").val();
    var GerenteGerencia = $("#GerenteGerencia").val();
    var GerenteDescripcion = $("#GerenteDescripcion").val();
    var GerenteEstado = $("#GerenteEstado").val();
    if (!GerenteNombre || !GerenteGerencia || GerenteGerencia == -1 || !GerenteDescripcion || !GerenteEstado || GerenteEstado == -1) {
        swal("", "Debe completar el formulario.", "error");
        return false;
    }
    else {
        var DTO = {
            'GteNombre': GerenteNombre,
            'IdGerencia': GerenteGerencia,
            'GteDescripcion': GerenteDescripcion,
            'GteEstado': GerenteEstado
        }
        if (GerenteEstado != 0) {
            var responseValidacion = FNRealizarValidacionGerencia(GerenteGerencia);
            if (responseValidacion && responseValidacion != "" && responseValidacion != "Error" && responseValidacion != undefined) {
                if (responseValidacion == "1") {
                    $('#ModalCrearGerente').hide();
                    swal({
                        title: 'Esta seguro?',
                        text: "Esta seguro de reemplazar gerente actual?",
                        type: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: 'Si, continuar!',
                        cancelButtonText: 'No!',
                        confirmButtonClass: 'btn btn-primary',
                        cancelButtonClass: 'btn btn-danger'
                    }).then(function (isConfirm) {
                        if (isConfirm && isConfirm.value) {
                            $.ajax({
                                url: "Gerente/Guardar",
                                type: "POST",
                                dataType: "json",
                                data: (DTO),
                                success: function (r) {
                                    //PARSEAR RESPUESTA
                                    var estructura = JSON.parse(JSON.stringify(r));
                                    estructura = estructura.Mensaje;
                                    //PROCESAR RESPUESTA
                                    if (estructura == "Error") {
                                        swal("Error", "No es posible crear al gerente.", "error");
                                        $(".orderselect").val('-1').prop('selected', true);
                                        setTimeout(function () {
                                            document.location.reload();
                                        }, 3000);
                                    }
                                    else if (estructura == "Guardado") {
                                        swal("", "Gerente creado.", "success");
                                        $(".orderselect").val('-1').prop('selected', true);
                                        setTimeout(function () {
                                            document.location.reload();
                                        }, 3000);
                                    }
                                }
                            });
                        } else {
                            $('#ModalCrearGerente').show();
                            return false;
                        }
                    });
                } else {
                    $.ajax({
                        url: "Gerente/Guardar",
                        type: "POST",
                        dataType: "json",
                        data: (DTO),
                        success: function (r) {
                            //PARSEAR RESPUESTA
                            var estructura = JSON.parse(JSON.stringify(r));
                            estructura = estructura.Mensaje;
                            //PROCESAR RESPUESTA
                            if (estructura == "Error") {
                                swal("Error", "No es posible crear al gerente.", "error");
                                $(".orderselect").val('-1').prop('selected', true);
                                setTimeout(function () {
                                    document.location.reload();
                                }, 3000);
                            }
                            else if (estructura == "Guardado") {
                                swal("", "Gerente creado.", "success");
                                $(".orderselect").val('-1').prop('selected', true);
                                setTimeout(function () {
                                    document.location.reload();
                                }, 3000);
                            }
                        }
                    });
                }
            } else {
                swal("Error", "Error al crear gerente. Por favor inténtalo de nuevo más tarde!", "error");
            }
        } else {
            $.ajax({
                url: "Gerente/Guardar",
                type: "POST",
                dataType: "json",
                data: (DTO),
                success: function (r) {
                    //PARSEAR RESPUESTA
                    var estructura = JSON.parse(JSON.stringify(r));
                    estructura = estructura.Mensaje;
                    //PROCESAR RESPUESTA
                    if (estructura == "Error") {
                        swal("Error", "No es posible crear al gerente.", "error");
                        $(".orderselect").val('-1').prop('selected', true);
                        setTimeout(function () {
                            document.location.reload();
                        }, 3000);
                    }
                    else if (estructura == "Guardado") {
                        swal("", "Gerente creado.", "success");
                        $(".orderselect").val('-1').prop('selected', true);
                        setTimeout(function () {
                            document.location.reload();
                        }, 3000);
                    }
                }
            });
        }
        return false;
    }
}
//
// CERRAR MODAL CREAR
//
FNCerrarCrearGerente = function () {
    $('#ModalCrearGerente').hide()
    document.location.reload();
    return false;
}
//
// EDITAR /ACTUALIZAR
//
FNEditarGerente = function (cual) {
    var DTO = {
        'GteToken': cual
    }
    $.ajax({
        url: "Gerente/BuscarPorToken",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            var estructura = JSON.parse(JSON.stringify(r));
            $("#GerenteToken_Editar").val(cual);
            $("#GerenteNombre_Editar").val(estructura.Mensaje[1].Value);
            $("#GerenteGerencia_Editar").val(estructura.Mensaje[2].Value);
            $("#GerenteDescripcion_Editar").val(estructura.Mensaje[4].Value);
            if (estructura.Mensaje[5].Value == "1") {
                $('#GerenteEstado_Editar>option:eq(1)').prop('selected', true);
            }
            else if (estructura.Mensaje[5].Value == "0") {
                $('#GerenteEstado_Editar>option:eq(2)').prop('selected', true);
            }
        }
    });
    $('#ModalActualizarGerente').show();
    return false;
}
//
// ACTUALIZAR Gerente
//
FNActualizarGerente = function () {
    var GerenteToken = $("#GerenteToken_Editar").val();
    var GerenteNombre = $("#GerenteNombre_Editar").val();
    var GerenteGerencia = $("#GerenteGerencia_Editar").val();
    var GerenteDescripcion = $("#GerenteDescripcion_Editar").val();
    var GerenteEstado = $("#GerenteEstado_Editar").val();
    if (!GerenteNombre || !GerenteGerencia || GerenteGerencia == -1 || !GerenteDescripcion || !GerenteEstado || GerenteEstado == -1) {
        swal("", "Debe completar el formulario.", "error");
        return false;
    } else {
        var DTO = {
            'GteToken': GerenteToken,
            'GteNombre': GerenteNombre,
            'IdGerencia': GerenteGerencia,
            'GteDescripcion': GerenteDescripcion,
            'GteEstado': GerenteEstado
        }

        if (GerenteEstado != 0) {
            var responseValidacion = FNRealizarValidacionGerenciaGerente(GerenteGerencia, GerenteToken);
            if (responseValidacion && responseValidacion != "" && responseValidacion != "Error" && responseValidacion != undefined) {
                if (responseValidacion == "1") {
                    $('#ModalActualizarGerente').hide();
                    swal({
                        title: 'Esta seguro?',
                        text: "Esta seguro de reemplazar gerente actual?",
                        type: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: 'Si, continuar!',
                        cancelButtonText: 'No!',
                        confirmButtonClass: 'btn btn-primary',
                        cancelButtonClass: 'btn btn-danger'
                    }).then(function (isConfirm) {
                        if (isConfirm && isConfirm.value) {
                            $.ajax({
                                url: "Gerente/Actualizar",
                                type: "POST",
                                dataType: "json",
                                data: (DTO),
                                success: function (r) {
                                    //PARSEAR RESPUESTA
                                    var estructura = JSON.parse(JSON.stringify(r));
                                    estructura = estructura.Mensaje;
                                    //PROCESAR RESPUESTA
                                    if (estructura == "Error") {
                                        swal("Error", "No es posible actualizar al gerente seleccionado.", "error");
                                        $(".orderselect").val('-1').prop('selected', true);
                                    }
                                    else if (estructura == "Actualizado") {
                                        swal("", "Gerente actualizado.", "success");
                                        $(".orderselect").val('-1').prop('selected', true);
                                        setTimeout(function () {
                                            window.location.href = location.protocol + '//' + location.host + location.pathname + "?paginaSeleccionada=" + paginaSeleccionada;
                                        }, 3000);
                                    }
                                }
                            });
                        } else {
                            $('#ModalActualizarGerente').show();
                            return false;
                        }
                    });
                } else {
                    $.ajax({
                        url: "Gerente/Actualizar",
                        type: "POST",
                        dataType: "json",
                        data: (DTO),
                        success: function (r) {
                            //PARSEAR RESPUESTA
                            var estructura = JSON.parse(JSON.stringify(r));
                            estructura = estructura.Mensaje;
                            //PROCESAR RESPUESTA
                            if (estructura == "Error") {
                                swal("Error", "No es posible actualizar al gerente seleccionado.", "error");
                                $(".orderselect").val('-1').prop('selected', true);
                            }
                            else if (estructura == "Actualizado") {
                                swal("", "Gerente actualizado.", "success");
                                $(".orderselect").val('-1').prop('selected', true);
                                setTimeout(function () {
                                    window.location.href = location.protocol + '//' + location.host + location.pathname + "?paginaSeleccionada=" + paginaSeleccionada;
                                }, 3000);
                            }
                        }
                    });
                }
            } else {
                swal("Error", "Error al actualizar gerente. Por favor inténtalo de nuevo más tarde!", "error");
            }
        } else {
            $.ajax({
                url: "Gerente/Actualizar",
                type: "POST",
                dataType: "json",
                data: (DTO),
                success: function (r) {
                    //PARSEAR RESPUESTA
                    var estructura = JSON.parse(JSON.stringify(r));
                    estructura = estructura.Mensaje;
                    //PROCESAR RESPUESTA
                    if (estructura == "Error") {
                        swal("Error", "No es posible actualizar al gerente seleccionado.", "error");
                        $(".orderselect").val('-1').prop('selected', true);
                    }
                    else if (estructura == "Actualizado") {
                        swal("", "Gerente actualizado.", "success");
                        $(".orderselect").val('-1').prop('selected', true);
                        setTimeout(function () {
                            window.location.href = location.protocol + '//' + location.host + location.pathname + "?paginaSeleccionada=" + paginaSeleccionada;
                        }, 3000);
                    }
                }
            });
        }
        return false;
    }
}
//
// CERRAR ACTUALIZAR Gerente
//
FNCerrarActualizarGerente = function () {
    $('#ModalActualizarGerente').hide()
    $(".orderselect").val('-1').prop('selected', true);
    document.location.reload();
    return false;
}
//
// CONFIRMAR
//
FNMostrarModalConfirmarDesactivar = function (cual) {
    $('#ModalConfirmarDesactivar').show();
    $("#GerenteToken_Desactivar").val(cual);
    return true;
}

FNMostrarModalConfirmarActivar = function (cual) {
    $('#ModalConfirmarActivar').show();
    $("#GerenteToken_Activar").val(cual);
    return true;
}

//
// DESACTIVAR
//
FNDesactivarGerente = function (cual) {
    var DTO = {
        'GteToken': $("#GerenteToken_Desactivar").val()
    }
    $.ajax({
        url: "Gerente/Desactivar",
        type: "POST",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            //PARSEAR RESPUESTA
            var estructura = JSON.parse(JSON.stringify(r));
            estructura = estructura.Mensaje;
            //PROCESAR RESPUESTA
            if (estructura == "Error") {
                swal("Error", "No es posible desactivar el gerente seleccionado.", "error");
                $(".orderselect").val('-1').prop('selected', true);
            }
            else if (estructura == "Descativado") {
                swal("", "Gerente desactivado.", "success");
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
