// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : PLANIFICACION
// FECHA            : MAYO 2018
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// SECCION          : CAPEX ADJUNTOS /DOCUMENTOS
// METODOS          :

FNDescargarAdjuntoFinal = function (token) {
    var link = document.createElement("a");
    console.info("token=", token);
    $.ajax({
        url: "/Documentacion/DescargarDocumentoAdjuntoFinal/" + token,
        method: "GET",
        data: { "token": token }
    }).done(function (r) {
        if (r && r.IsSuccess && r.ResponseData) {
            document.location.href = r.ResponseData;
        }
    }).fail(function (xhr) {
        console.log('error', xhr);
    });
    return;
}

FNModalEliminarAdjunto = function (token) {
    var token = token.trim();
    localStorage.setItem("CAPEX_TOKEN_ADJUNTO", token);
    $("#optionFileDelete").val("0");
    $("#ModalEliminarAdjunto").show();
    return;
}

FNModalEliminarAdjuntoEE = function (token) {
    var token = token.trim();
    localStorage.setItem("CAPEX_TOKEN_ADJUNTO", token);
    $("#optionFileDelete").val("1");
    $("#ModalEliminarAdjunto").show();
    return;
}

FNModalEliminarAdjuntoER = function (token) {
    var token = token.trim();
    localStorage.setItem("CAPEX_TOKEN_ADJUNTO", token);
    $("#optionFileDelete").val("2");
    $("#ModalEliminarAdjunto").show();
    return;
}

FNCerrarModalEliminarAdjunto = function () {
    $("#ModalEliminarAdjunto").hide();
    return false;
}

FNEliminarAdjuntoFinal = function () {
    var token = localStorage.getItem("CAPEX_TOKEN_ADJUNTO");
    var IniToken = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    var optionFileDelete = $("#optionFileDelete").val();
    if (optionFileDelete == "0") {
        FNEliminarAdjuntoFinalBD(token);
    } else if (optionFileDelete == "1") {
        var estadoEvaluacionEconomica = localStorage.getItem("CAPEX_INICIATIVA_EVALUACION_ECONOMICA_ESTADO");
        if (estadoEvaluacionEconomica && estadoEvaluacionEconomica != undefined && estadoEvaluacionEconomica != "" && (estadoEvaluacionEconomica == "Guardado" || estadoEvaluacionEconomica == "Actualizado")) {
            var DTOValidacion = {
                'IniToken': IniToken,
                'ParToken': token
            };
            var responseValidacion = FNRealizarValidacionOtroArchivoEERespaldo(DTOValidacion);
            if (!responseValidacion || responseValidacion == undefined || responseValidacion == "") {
                $("#ModalEliminarAdjunto").hide();
                swal({
                    title: 'Esta seguro?',
                    text: "Al eliminar este archivo, perderá lo establecido en Evaluación Económica!",
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
                        FNEliminarAdjuntoFinalConEEBD(DTOValidacion);
                    } else {
                        $("#ModalEliminarAdjunto").show();
                        return false;
                    }
                });
            } else {
                FNEliminarAdjuntoFinalBD(token);
                FNLimpiarEvaluacionEconomica();
            }
        } else {
            FNEliminarAdjuntoFinalBD(token);
            FNLimpiarEvaluacionEconomica();
        }
    } else if (optionFileDelete == "2") {
        var estadoEvaluacionRiesgo = localStorage.getItem("CAPEX_INICIATIVA_EVALUACION_RIESGO_ESTADO");
        if (estadoEvaluacionRiesgo && estadoEvaluacionRiesgo != undefined && estadoEvaluacionRiesgo != "" && (estadoEvaluacionRiesgo == "Guardado" || estadoEvaluacionRiesgo == "Actualizado")) {
            var DTOValidacion = {
                'IniToken': IniToken,
                'ParToken': token
            };
            var responseValidacion = FNRealizarValidacionOtroArchivoERRespaldo(DTOValidacion);
            if (!responseValidacion || responseValidacion == undefined || responseValidacion == "") {
                $("#ModalEliminarAdjunto").hide();
                swal({
                    title: 'Esta seguro?',
                    text: "Al eliminar este archivo, perderá lo establecido en Evaluación Riesgo!",
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
                        FNEliminarAdjuntoFinalConERBD(DTOValidacion);
                    } else {
                        $("#ModalEliminarAdjunto").show();
                        return false;
                    }
                });
            } else {
                FNEliminarAdjuntoFinalBD(token);
                FNLimpiarEvaluacionRiesgo();
            }
        } else {
            FNEliminarAdjuntoFinalBD(token);
            FNLimpiarEvaluacionRiesgo();
        }
    }
}


FNLimpiarEvaluacionEconomica = function () {
    $("#EveVan").val("0.00");
    $("#EveIvan").val("0.00");
    $("#EvePayBack").val("0.00");
    $("#EveVidaUtil").val("0.0");
    $("#EveTipoCambio").val("0.00");
    $("#EveTir").val("0%");
    $('#frame25').attr('src', $('#frame25').attr('src'));
    localStorage.setItem("CAPEX_INICIATIVA_EVALUACION_ECONOMICA_ESTADO", "");
    $("#BotonGuardarEvaluacionEconomica").prop("disabled", false);
    $("#BotonGuardarEvaluacionEconomica").show();
    $("#BotonActualizarEvaluacionEconomica").hide();
}

FNLimpiarEvaluacionRiesgo = function () {
    $("#EvrProbabilidad1").val("1");
    $("#EvrImpacto1").val("1");
    FNCalculoRiesgoSinProyecto();
    $("#EvrMFL1").val("0.00");
    $("#EvrProbabilidad2").val("1");
    $("#EvrImpacto2").val("1");
    FNCalculoRiesgoConProyecto();
    $("#EvrMFL2").val("0.00");
    $('#frame26').attr('src', $('#frame26').attr('src'));
    localStorage.setItem("CAPEX_INICIATIVA_EVALUACION_RIESGO_ESTADO", "");
    $("#BotonGuardarEvaluacionRiesgo").prop("disabled", false);
    $("#BotonGuardarEvaluacionRiesgo").show();
    $("#BotonActualizarEvaluacionRiesgo").hide();
}

FNEliminarAdjuntoFinalBD = function (token) {
    $.ajaxSetup({ cache: false });
    $.ajax({
        url: "../../Planificacion/EliminarAdjuntoVigente",
        method: "GET",
        data: { "token": token }
    }).done(function (request) {
        if (request && request.redirectUrlLogout && request.redirectUrlLogout == "true") {
            document.getElementById('linkToLogout').click();
            return;
        }
        $("#ModalEliminarAdjunto").hide();
        swal("", "Archivo eliminado.", "success");
        FNPoblarDocumentos();
    }).fail(function (xhr) {
        console.log('error', xhr);
    });
}

FNEliminarAdjuntoFinalConEEBD = function (DTO) {
    $.ajaxSetup({ cache: false });
    $.ajax({
        url: "../../Planificacion/EliminarAdjuntoVigenteConEvaluacionEconomica",
        method: "POST",
        data: (DTO)
    }).done(function (request) {
        if (request && request.redirectUrlLogout && request.redirectUrlLogout == "true") {
            document.getElementById('linkToLogout').click();
            return;
        }
        swal("", "Archivo eliminado.", "success");
        FNLimpiarEvaluacionEconomica();
        FNPoblarDocumentos();
    }).fail(function (xhr) {
        console.log('error', xhr);
    });
}

FNEliminarAdjuntoFinalConERBD = function (DTO) {
    $.ajaxSetup({ cache: false });
    $.ajax({
        url: "../../Planificacion/EliminarAdjuntoVigenteConEvaluacionRiesgo",
        method: "POST",
        data: (DTO)
    }).done(function (request) {
        if (request && request.redirectUrlLogout && request.redirectUrlLogout == "true") {
            document.getElementById('linkToLogout').click();
            return;
        }
        swal("", "Archivo eliminado.", "success");
        FNLimpiarEvaluacionRiesgo();
        FNPoblarDocumentos();
    }).fail(function (xhr) {
        console.log('error', xhr);
    });
}

FNRealizarValidacionOtroArchivoEERespaldo = function (DTO) {
    var response = '';
    $.ajax({
        url: "../../Planificacion/SeleccionarOtroAdjuntoEvaluacionEconomicaVigente",
        type: "POST",
        async: false,
        dataType: "json",
        data: (DTO),
        success: function (r) {
            if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                document.getElementById('linkToLogout').click();
                return;
            }
            var estructura = JSON.parse(JSON.stringify(r));
            console.log("FNRealizarValidacionOtroArchivoEERespaldo estructura=", estructura)
            estructura = estructura.Mensaje;
            var parte = estructura.split("|");
            var estado = parte[0];
            if (estado == "0") {
                response = parte[1];
            }
        }
    });
    return response;
}

FNRealizarValidacionOtroArchivoERRespaldo = function (DTO) {
    var response = '';
    $.ajax({
        url: "../../Planificacion/SeleccionarOtroAdjuntoEvaluacionRiesgoVigente",
        type: "POST",
        async: false,
        dataType: "json",
        data: (DTO),
        success: function (r) {
            if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                document.getElementById('linkToLogout').click();
                return;
            }
            var estructura = JSON.parse(JSON.stringify(r));
            console.log("FNRealizarValidacionOtroArchivoERRespaldo estructura=", estructura)
            estructura = estructura.Mensaje;
            var parte = estructura.split("|");
            var estado = parte[0];
            if (estado == "0") {
                response = parte[1];
            }
        }
    });
    return response;
}

//
// DESCARGA ADJUNTOS
//
FNDescargarAdjunto = function (token, archivo, paso) {
    var link = document.createElement("a");
    console.info("token=", token);
    console.info("archivo=", archivo);
    console.info("paso=", paso);

    document.location.href = window.origin + "/Documentacion/DescargarDocumentoAdjunto/" + token + "/" + archivo + "/" + paso + "/";
    return;

    /*switch (paso) {
        case "Presupuesto-Gantt":
            link.download = archivo;
            link.href = "~/Files/Iniciativas/Presupuesto/Gantt/" + token + "/";
            //link.href = "/Files/Iniciativas/Presupuesto/Gantt/" + token + "/";
            link.click();
            break;
        case "Evaluacion-Economica":
            link.download = archivo;
            link.href = "~/Files/Iniciativas/EvaluacionEconomica/" + token + "/";
            //link.href = "/Files/Iniciativas/EvaluacionEconomica/" + token + "/";
            link.click();
            break;
        case "Evaluacion-Riesgo":
            link.download = archivo;
            link.href = "~/Files/Iniciativas/EvaluacionRiesgo/" + token + "/";
            //link.href = "/Files/Iniciativas/EvaluacionRiesgo/" + token + "/";
            link.click();
            break;
        case "Categorizacion":
            link.download = archivo;
            link.href = "~/Files/Iniciativas/Categorizacion/" + token + "/";
            //link.href = "/Files/Iniciativas/Categorizacion/" + token + "/";
            link.click();
            break;
        case "Descripcion-Detallada":
            link.download = archivo;
            link.href = "~/Files/Iniciativas/Descripcion/" + token + "/";
            //link.href = "/Files/Iniciativas/Descripcion/" + token + "/";
            link.click();
            break;
    }*/
}
//
// ELIMINAR ADJUNTO
//
FNEliminarAdjunto = function (token) {
    $.ajaxSetup({ cache: false });
    $.ajax({
        url: "../Planificacion/EliminarAdjuntoVigente",
        method: "GET",
        data: { "token": token }
    }).done(function (request) {
        if (request && request.redirectUrlLogout && request.redirectUrlLogout == "true") {
            document.getElementById('linkToLogout').click();
            return;
        }
        swal("", "Archivo eliminado.", "success");
        FNPoblarDocumentos();

    }).fail(function (xhr) { console.log('error', xhr); });
}
//
// POBLAR TABLA DE DOCUMENTOS - ADJUNTOS
//
FNPoblarDocumentos = function () {
    var iniciativa_token = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    //CONSULTA
    $.ajaxSetup({ cache: false });
    $.ajax({
        url: "../Planificacion/VerAdjuntos",
        method: "GET",
        data: { "token": iniciativa_token }
    }).done(function (request) {
        $("#ContenedorDocumentos").show();
        $("#ContenedorElementosAdjuntos").html(request);

    }).fail(function (xhr) { console.log('error', xhr); });
}



