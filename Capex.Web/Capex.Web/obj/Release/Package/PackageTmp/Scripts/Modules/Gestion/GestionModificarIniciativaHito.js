// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : GESTION - MODIFICAR INICIATIVA
// FECHA            : MAYO 2018
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// METODOS          : HITOS
//
//

//
// POBLAR VISTA CAPEX HITOS - GENERAL
//
PoblarVistaHitosGeneral = function (token) {
    //PREPARAR
    $.ajaxSetup({ cache: false });
    //FINANCIERO
    $.ajax({
        url: '../../Planificacion/PoblarVistaHitosGeneral',
        method: "GET",
        data: { "token": localStorage.getItem("CAPEX_INICIATIVA_TOKEN") }
    }).done(function (r) {
        if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
            document.getElementById('linkToLogout').click();
            return;
        }
        $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
            localStorage.setItem("CAPEX_HITO_FECHA_INICIO", value.IgFechaInicio);
            $("#HitoInicio").html(value.IgFechaInicio);
            $("#HitoTermino").html(value.IgFechaTermino);
            $("#HitoCierre").html(value.IgFechaCierre);
            $("#PorInvNacExt").text(value.PorInvNacExt);
        });
    }).fail(function (xhr) {
        console.log('error', xhr);
    });
}
//
// POBLAR VISTA CAPEX HITOS - RESUMEN
//
PoblarVistaHitosResumen = function (token) {
    //PREPARAR
    $.ajaxSetup({ cache: false });
    //FINANCIERO
    $.ajax({
        url: '../../Planificacion/PoblarVistaHitosResumen',
        method: "GET",
        data: { "token": localStorage.getItem("CAPEX_INICIATIVA_TOKEN") }
    }).done(function (r) {
        if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
            document.getElementById('linkToLogout').click();
            return;
        }
        var contador = 1;
        $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
            if (contador == 1) {
                $("#HitosCostos").html(value.IrDato2);
            }
            else if (contador == 2) {
                $("#HitosContingencia").html(value.IrDato2);
            }
            contador++;
        });
    }).fail(function (xhr) {
        console.log('error', xhr);
    });
}
//
// PORCENTAJE NACIONAL
//
FNDeterminaPorNac = function (valor) {
    if (valor) {
        switch (valor) {
            case ("10/90"): $('#PorcentajeNacional>option:eq(1)').prop('selected', true);; break;
            case ("20/80"): $('#PorcentajeNacional>option:eq(2)').prop('selected', true);; break;
            case ("30/70"): $('#PorcentajeNacional>option:eq(3)').prop('selected', true);; break;
            case ("40/60"): $('#PorcentajeNacional>option:eq(4)').prop('selected', true);; break;
            case ("50/50"): $('#PorcentajeNacional>option:eq(5)').prop('selected', true);; break;
            case ("60/40"): $('#PorcentajeNacional>option:eq(6)').prop('selected', true);; break;
            case ("70/30"): $('#PorcentajeNacional>option:eq(7)').prop('selected', true);; break;
            case ("80/30"): $('#PorcentajeNacional>option:eq(8)').prop('selected', true);; break;
            case ("90/10"): $('#PorcentajeNacional>option:eq(9)').prop('selected', true);; break;
        }
        return true;
    }
    else {
        return false;
    }

}
//
// GUARDAR CAPEX HITOS
//
FNGuardarHitos = function (origen, asynCall) {
    var HitSAP = $("#HitSAP").val();

    if (HitSAP == "-") {
        HitSAP = "";
    }
    var HitCI = $("#HitCI").val();
    if (HitCI == "-") {
        HitCI = "";
    }
    var HitCA = $("#HitCA").val();
    if (HitCA == "-") {
        HitCA = "";
    }
    var HitOPR = $("#HitOPR").val();
    if (HitOPR == "-") {
        HitOPR = "";
    }
    var HitPE = $("#HitPE").val();
    if (HitPE == "-") {
        HitPE = "";
    }
    var HitDIRCEN = $("#HitDIRCEN").val();
    if (HitDIRCEN == "-") {
        HitDIRCEN = "";
    }
    var HitDirPLC = $("#HitDirPLC").val();
    if (HitDirPLC == "-") {
        HitDirPLC = "";
    }
    var PorcentajeNacional = $("#PorInvNacExt").text();
    var tipo = localStorage.getItem("CAPEX_TIPO_INICIATIVA");
    if (asynCall == undefined || asynCall == null) {
        asynCall = true;
    }
    var iniciativa_token = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    if (iniciativa_token == null || iniciativa_token == "") {
        swal("", "Debe identificar iniciativa antes de realizar esta acción.", "info");
        return false;
    }
    if ($("#BotonAgregarSAP").is(":visible") && HitSAP == "") {
        if (tipo != "CB" && tipo != "CD") {
            swal("", "Debe ingresar fecha de ruta de aprobación (Ficha SAP)", "info");
            return false;
        }
    }
    if ($("#BotonAgregarCI").is(":visible") && HitCI == "") {
        if (tipo != "CB" && tipo != "CD") {
            swal("", "Debe ingresar fecha de ruta de aprobación (CI)", "info");
            return false;
        }
    }
    if ($("#BotonAgregarCA").is(":visible") && HitCA == "") {
        if (tipo != "CB" && tipo != "CD") {
            swal("", "Debe ingresar fecha de ruta de aprobación (CA)", "info");
            return false;
        }
    }
    if ($("#BotonAgregarOPR").is(":visible") && HitOPR == "") {
        if (tipo != "CB" && tipo != "CD") {
            swal("", "Debe ingresar fecha de ruta de aprobación (OPR)", "info");
            return false;
        }
    }
    if ($("#BotonAgregarPE").is(":visible") && HitPE == "") {
        if (tipo != "CB" && tipo != "CD") {
            swal("", "Debe ingresar fecha de ruta de aprobación (PE)", "info");
            return false;
        }
    }
    if ($("#BotonAgregarDIRCEN").is(":visible") && HitDIRCEN == "") {
        if (tipo != "CB" && tipo != "CD") {
            swal("", "Debe ingresar fecha de ruta de aprobación (DIRCEN)", "info");
            return false;
        }
    }
    if ($("#BotonAgregarDIRPLC").is(":visible") && HitDirPLC == "") {
        if (tipo != "CB" && tipo != "CD") {
            swal("", "Debe ingresar fecha de ruta de aprobación (DIRPLC)", "info");
            return false;
        }
    }
    var valueResponse = false;
    if (PorcentajeNacional == "-1" || PorcentajeNacional == null || PorcentajeNacional == "") {
        swal("", "Debe ingresar porcentaje nacional", "info");
        return false;
    } else {
        var ParUsuario = $("#CAPEX_H_USERNAME").val();
        var DTO = {
            'IniToken': iniciativa_token,
            'IniUsuario': ParUsuario,
            'HitNacExt': PorcentajeNacional,
            'HitSAP': HitSAP,
            'HitCI': HitCI,
            'HitCA': HitCA,
            'HitOPR': HitOPR,
            'HitPE': HitPE,
            'HitDIRCEN': HitDIRCEN,
            'HitDirPLC': HitDirPLC
        };

        $.ajax({
            url: "../../Planificacion/GuardarHito",
            type: "POST",
            dataType: "json",
            data: (DTO),
            async: asynCall,
            success: function (r) {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                //PARSEAR RESPUESTA
                var estado = JSON.parse(JSON.stringify(r));
                estado = estado.Mensaje;
                //PROCESAR RESPUESTA
                if (estado == "Error") {
                    swal("Error", "No es posible almacenar Capex/Hitos", "error");
                    return false;
                } else if (estado == "Guardado") {
                    if (origen == "GuardarEnviar") {
                        localStorage.setItem("CAPEX_INICIATIVA_HITO_ESTADO", estado);
                    } else {
                        localStorage.setItem("CAPEX_INICIATIVA_HITO_ESTADO", estado);
                        swal("Exito", "Información guardada correctamente.", "success");
                        $("#BotonGuardarHito").prop("disabled", "true");
                        $("#BotonActualizarHito").show();
                        $("#BotonGuardarEnviarIniciativa").show();
                    }
                    origen = null;
                    valueResponse = true;
                    return false;
                } else {
                    swal("Error", "No es posible guardar.", "error");
                    return false;
                }
            }
        });
    }
    return valueResponse;
}
//
// ACTUALIZAR CAPEX HITOS
//
FNActualizarHitos = function (origen) {
    var HitSAP = $("#HitSAP").val();
    if (HitSAP == "-") {
        HitSAP = "";
    }
    var HitCI = $("#HitCI").val();
    if (HitCI == "-") {
        HitCI = "";
    }
    var HitCA = $("#HitCA").val();
    if (HitCA == "-") {
        HitCA = "";
    }
    var HitOPR = $("#HitOPR").val();
    if (HitOPR == "-") {
        HitOPR = "";
    }
    var HitPE = $("#HitPE").val();
    if (HitPE == "-") {
        HitPE = "";
    }
    var HitDIRCEN = $("#HitDIRCEN").val();
    if (HitDIRCEN == "-") {
        HitDIRCEN = "";
    }
    var HitDirPLC = $("#HitDirPLC").val();
    if (HitDirPLC == "-") {
        HitDirPLC = "";
    }
    var PorcentajeNacional = $("#PorInvNacExt").text();
    var tipo = localStorage.getItem("CAPEX_TIPO_INICIATIVA");
    var iniciativa_token = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    if (iniciativa_token == null || iniciativa_token == "") {
        swal("", "Debe identificar iniciativa antes de realizar esta acción.", "info");
        return false;
    }
    if ($("#BotonAgregarSAP").is(":visible") && HitSAP == "") {
        if (tipo != "CB" && tipo != "CD") {
            swal("", "Debe ingresar fecha de ruta de aprobación (Ficha SAP)", "info");
            return false;
        }
    }
    if ($("#BotonAgregarCI").is(":visible") && HitCI == "") {
        if (tipo != "CB" && tipo != "CD") {
            swal("", "Debe ingresar fecha de ruta de aprobación (CI)", "info");
            return false;
        }
    }
    if ($("#BotonAgregarCA").is(":visible") && HitCA == "") {
        if (tipo != "CB" && tipo != "CD") {
            swal("", "Debe ingresar fecha de ruta de aprobación (CA)", "info");
            return false;
        }
    }
    if ($("#BotonAgregarOPR").is(":visible") && HitOPR == "") {
        if (tipo != "CB" && tipo != "CD") {
            swal("", "Debe ingresar fecha de ruta de aprobación (OPR)", "info");
            return false;
        }
    }
    if ($("#BotonAgregarPE").is(":visible") && HitPE == "") {
        if (tipo != "CB" && tipo != "CD") {
            swal("", "Debe ingresar fecha de ruta de aprobación (PE)", "info");
            return false;
        }
    }
    if ($("#BotonAgregarDIRCEN").is(":visible") && HitDIRCEN == "") {
        if (tipo != "CB" && tipo != "CD") {
            swal("", "Debe ingresar fecha de ruta de aprobación (DIRCEN)", "info");
            return false;
        }
    }
    if ($("#BotonAgregarDIRPLC").is(":visible") && HitDirPLC == "") {
        if (tipo != "CB" && tipo != "CD") {
            swal("", "Debe ingresar fecha de ruta de aprobación (DIRPLC)", "info");
            return false;
        }
    }
    if (PorcentajeNacional == "-1" || PorcentajeNacional == null || PorcentajeNacional == "") {
        swal("", "Debe ingresar porcentaje nacional", "info");
        return false;
    } else {
        var ParUsuario = $("#CAPEX_H_USERNAME").val();
        var DTO = {
            'IniToken': iniciativa_token,
            'IniUsuario': ParUsuario,
            'HitNacExt': PorcentajeNacional,
            'HitSAP': HitSAP,
            'HitCI': HitCI,
            'HitCA': HitCA,
            'HitOPR': HitOPR,
            'HitPE': HitPE,
            'HitDIRCEN': HitDIRCEN,
            'HitDirPLC': HitDirPLC
        };

        $.ajax({
            url: "../../Planificacion/ActualizarHito",
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                //PARSEAR RESPUESTA
                var estado = JSON.parse(JSON.stringify(r));
                estado = estado.Mensaje;
                //PROCESAR RESPUESTA
                if (estado == "Error") {
                    swal("", "No es posible actualizar Capex/Hitos", "info");
                    return false;
                }
                else if (estado == "Guardado") {
                    if (origen == "GuardarEnviar") {
                        localStorage.setItem("CAPEX_INICIATIVA_HITO_ESTADO", estado);
                    } else {
                        swal("Exito", "Información actualizada correctamente.", "success");
                        localStorage.setItem("CAPEX_INICIATIVA_HITO_ESTADO", estado);
                    }
                    origen = null;
                }
                else if (estado == "Actualizado") {
                    swal("Exito", "Información actualizada correctamente.", "success");
                    localStorage.setItem("CAPEX_INICIATIVA_HITO_ESTADO", estado);
                }
                else {
                    swal("Error", "No es posible actualizar.", "error");
                    return false;
                }
            }
        });
    }
}
//
// INICIO DE GUARDAR Y ENVIAR CAPEX HITOS
//
FNGuardarEnviarIniciativa = function () {
    var codigoIniciativa = $("#CodigoIniciativa").val();
    if (!codigoIniciativa || codigoIniciativa == undefined) {
        codigoIniciativa = "";
    }
    swal({
        title: 'Esta seguro?',
        text: "Esta seguro de guardar y enviar iniciativa N° " + codigoIniciativa + "!",
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
            var tipo = localStorage.getItem("CAPEX_TIPO_INICIATIVA");
            if (tipo == "CB" || tipo == "CD") {
                FNGuarEnviarSinValidacionQuarter();
            } else {
                FNGuarEnviarConValidacionQuarter();
            }
        } else {
            return false;
        }
    });
}
//
// VALIDACION DE QUARTER
//
FNGuarEnviarConValidacionQuarter = function () {
    var estado = false;
    var inicio = localStorage.getItem("CAPEX_HITO_FECHA_INICIO");
    var inicioSplit = ((inicio) ? inicio.split("-") : inicio);
    var inicioAux = ((inicioSplit && (inicioSplit.length == 3)) ? (inicioSplit[1] + "-" + inicioSplit[0] + "-" + inicioSplit[2]) : inicioSplit);
    var total = localStorage.getItem("CAPEX_HITO_TOTAL");
    total = total.replace(".", "");
    total = parseInt(total);
    var etapa = localStorage.getItem("CAPEX_PLAN_ETAPA");
    if (!etapa || etapa == undefined || etapa == null || etapa == "") {
        swal("", "Debe seleccionar 'Etapa' en el paso de Identificación.", "info");
        return false;
    }
    if (!total || total == undefined || total == null || total == "") {
        swal("", "Debe cargar Template con datos de iniciativa.", "info");
        return false;
    } else {
        estado = true;
        /*var quarter = moment(inicioAux).quarter();
        switch (quarter) {
            case 1:
                if (total < 1000 && etapa == "FACT") {
                    estado = true;
                }
                else if ((total >= 1000 && total < 3001) && etapa == "FACT") {
                    estado = true;
                }
                else if ((total >= 3000 && total < 10001) && etapa == "FACT") {
                    estado = true;
                }
                else if ((total > 10000) && etapa == "FACT") {
                    estado = true;
                }
                break;
            case 2:
                if (total < 1000 && etapa == "PREF") {
                    estado = true;
                }
                else if ((total >= 1000 && total < 3001) && etapa == "FACT") {
                    estado = true;
                }
                else if ((total >= 3000 && total < 10001) && etapa == "FACT") {
                    estado = true;
                }
                else if ((total > 10000) && etapa == "FACT") {
                    estado = true;
                }
                break;
            case 3:
                if (total < 1000 && etapa == "PERF") {
                    estado = true;
                }
                else if ((total >= 1000 && total < 3001) && etapa == "PERF") {
                    estado = true;
                }
                else if ((total >= 3000 && total < 10001) && (etapa == "PREF" || etapa == "EJEC")) {
                    estado = true;
                }
                else if ((total > 10000) && (etapa == "PREF" || etapa == "EJEC")) {
                    estado = true;
                }
                break;
            case 4:
                if (total < 1000 && etapa == "PERF") {
                    estado = true;
                }
                else if ((total >= 1000 && total < 3001) && etapa == "PREF") {
                    estado = true;
                }
                else if ((total >= 3000 && total < 10001) && (etapa == "PREF" || etapa == "EJEC")) {
                    estado = true;
                }
                else if ((total > 10000) && (etapa == "PREF" || etapa == "EJEC")) {
                    estado = true;
                }
                break;
        }*/

        //
        // ESTADO CUMPLIMIENTO QUARTER
        //
        if (!estado) {
            Swal.fire({
                title: '',
                width: 900,
                html: '<img src="/Content/quarter/CuadroQuarter.PNG" />' + '<br><hr> Su iniciativa no cumple con requisito asociado a Q. <br> Favor revisar para volver a enviar.',
                showCloseButton: false,
                showCancelButton: false,
                focusConfirm: false,
                confirmButtonText: 'Aceptar',
            });
            return false;
        }
        else {
            var iniciativa_token = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
            if (iniciativa_token == null || iniciativa_token == "") {
                swal("", "Debe identificar iniciativa antes de realizar esta acción.", "info");
                return false;
            } else {
                var estado = localStorage.getItem("CAPEX_INICIATIVA_HITO_ESTADO");
                if (estado == "Guardado" || estado == "Actualizado") {
                    var resultReponse = FNGuardarHitos("GuardarEnviar", false);
                    if (!resultReponse) {
                        return false;
                    }
                }
                setTimeout(function () {
                    var ParUsuario = $("#CAPEX_H_USERNAME").val();
                    var DTO = {
                        "IniToken": iniciativa_token,
                        "WrfUsuario": ParUsuario,
                        "WrfObservacion": ''
                    }
                    $.ajax({
                        url: "../../Planificacion/EnviarIniciativa",
                        type: "POST",
                        dataType: "json",
                        data: (DTO),
                        success: function (r) {
                            if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                                document.getElementById('linkToLogout').click();
                                return;
                            }
                            //PARSEAR RESPUESTA
                            var estado = JSON.parse(JSON.stringify(r));
                            estado = estado.Mensaje;
                            if (estado == "Error") {
                                swal("Error", "No es posible enviar en estos momentos.", "error");
                                return false;
                            }
                            else if (estado == "Enviado") {
                                swal("", "Iniciativa guardada y enviada.", "success");
                                console.log("Iniciativa guardada y enviada.");
                                localStorage.setItem("CAPEX_INICIATIVA_HITO_ESTADO", estado);
                                var tipo = localStorage.getItem("CAPEX_TIPO_INICIATIVA");
                                if (tipo == "CB" || tipo == "CD") {
                                    FNGenerarPDF(iniciativa_token);
                                } else {
                                    FNGenerarPDF(iniciativa_token);
                                }
                                console.log("luego de generado el pdf.");
                                setTimeout(function () {
                                    window.location.href = location.protocol + '//' + location.host + "/GestionVisacion";
                                }, 4000);
                            }
                        }
                    });
                }, 3000);
            }
        }

    }
}
//
// VALIDACION DE QUARTER
//
FNGuarEnviarSinValidacionQuarter = function () {
    var inicio = localStorage.getItem("CAPEX_HITO_FECHA_INICIO");
    var total = localStorage.getItem("CAPEX_HITO_TOTAL");
    total = total.replace(".", "");
    total = parseInt(total);
    var etapa = localStorage.getItem("CAPEX_PLAN_ETAPA");
    if (!etapa || etapa == undefined || etapa == null || etapa == "") {
        swal("", "Debe seleccionar 'Etapa' en el paso de Identificación.", "info");
        return false;
    }
    if (!total || total == undefined || total == null || total == "") {
        swal("", "Debe cargar Template con datos de iniciativa.", "info");
        return false;
    } else {
        var iniciativa_token = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
        if (iniciativa_token == null || iniciativa_token == "") {
            swal("", "Debe identificar iniciativa antes de realizar esta acción.", "info");
            return false;
        } else {
            var estado = localStorage.getItem("CAPEX_INICIATIVA_HITO_ESTADO");
            if (estado == "Guardado" || estado == "Actualizado") {
                var resultReponse = FNGuardarHitos("GuardarEnviar", false);
                if (!resultReponse) {
                    return false;
                }
            }
            setTimeout(function () {
                var ParUsuario = $("#CAPEX_H_USERNAME").val();
                var DTO = {
                    "IniToken": iniciativa_token,
                    "WrfUsuario": ParUsuario,
                    "WrfObservacion": ''
                }
                $.ajax({
                    url: "../../Planificacion/EnviarIniciativa",
                    type: "POST",
                    dataType: "json",
                    data: (DTO),
                    success: function (r) {
                        if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                            document.getElementById('linkToLogout').click();
                            return;
                        }
                        //PARSEAR RESPUESTA
                        var estado = JSON.parse(JSON.stringify(r));
                        estado = estado.Mensaje;
                        if (estado == "Error") {
                            swal("Error", "No es posible enviar en estos momentos.", "error");
                            return false;
                        } else if (estado == "Enviado") {
                            swal("", "Inciativa guardada y enviada.", "success");
                            localStorage.setItem("CAPEX_INICIATIVA_HITO_ESTADO", estado);
                            FNGenerarPDF(iniciativa_token);
                            setTimeout(function () {
                                window.location.href = location.protocol + '//' + location.host + "/GestionVisacion";
                            }, 4000);
                        }
                    }
                });
            }, 3000);
        }
    }
}

//
//GENERAR PDF
//
FNGenerarPDF = function (token) {
    var url = "../../Planificacion/descargaPdfPresupuesto?token=" + token;
    var win = window.open(url, '_blank');
    win.focus();
}
//
//VER RUTA DE APROBACION GRAFICA
//
FNVerGraficaRutaAprobacion = function () {

    var HitosTotalCapex = localStorage.getItem("CAPEX_HITO_TOTAL");
    HitosTotalCapex = HitosTotalCapex.replace(".", "");
    HitosTotalCapex = parseInt(HitosTotalCapex);
    if (HitosTotalCapex == "" || HitosTotalCapex == null) {
        swal("", "Debe cargar Template con datos de iniciativa.", "info");
        return false;
    }
    else {
        if (HitosTotalCapex < 500) {
            $("#ContenedorImagenRutaAprobacion").html('<img src="/Content/rutas/ruta1.png" height="200" />');
            $("#ModalRutaAprobacionGrafica").show();
        }
        else if (HitosTotalCapex > 500 && HitosTotalCapex < 10000) {
            $("#ContenedorImagenRutaAprobacion").html('<img src="/Content/rutas/ruta2.png"  height="200" />');
            $("#ModalRutaAprobacionGrafica").show();
        }
        else if (HitosTotalCapex >= 10000 && HitosTotalCapex < 15000) {
            $("#ContenedorImagenRutaAprobacion").html('<img src="/Content/rutas/ruta3.png"  height="200" />');
            $("#ModalRutaAprobacionGrafica").show();
        }
        //else if (HitosTotalCapex >= 10000 && HitosTotalCapex < 15000) {
        //    $("#ContenedorImagenRutaAprobacion").html('<img src="/Content/rutas/ruta4.png"  height="200" />');
        //    $("#ModalRutaAprobacionGrafica").show();
        //}
        else if (HitosTotalCapex >= 15000 && HitosTotalCapex < 50000) {
            $("#ContenedorImagenRutaAprobacion").html('<img src="/Content/rutas/ruta5.png" height="200" />');
            $("#ModalRutaAprobacionGrafica").show();
        }
        else if (HitosTotalCapex >= 50000) {
            $("#ContenedorImagenRutaAprobacion").html('<img src="/Content/rutas/ruta6.png"  height="200" />');
            $("#ModalRutaAprobacionGrafica").show();
        }
    }
}
//
// CERRAR MODAL RUTA APROBACION GRAFICA
//
FNCerrarModalRutaAprobacionGrafica = function () {
    $("#ModalRutaAprobacionGrafica").hide();
}
//
// MOSTRAR INGRESO DE FECHAS PARA RUTA DE APROBACION
//
FNMostrarIngresoFechaIndividual = function (cual) {
    $("#TituloTipoFecha").html("");
    switch (cual) {
        case "SAP": $("#TituloTipoFecha").html("Fecha SAP"); localStorage.setItem("CAPEX_HITO_FECHA_TIPO_ASIGNADA", cual); break;
        case "CI": $("#TituloTipoFecha").html("Fecha CI"); localStorage.setItem("CAPEX_HITO_FECHA_TIPO_ASIGNADA", cual); break;
        case "CA": $("#TituloTipoFecha").html("Fecha CA"); localStorage.setItem("CAPEX_HITO_FECHA_TIPO_ASIGNADA", cual); break;
        case "OPR": $("#TituloTipoFecha").html("Fecha OPR"); localStorage.setItem("CAPEX_HITO_FECHA_TIPO_ASIGNADA", cual); break;
        case "PE": $("#TituloTipoFecha").html("Fecha PE"); localStorage.setItem("CAPEX_HITO_FECHA_TIPO_ASIGNADA", cual); break;
        case "DIRCEN": $("#TituloTipoFecha").html("Fecha DIRCEN"); localStorage.setItem("CAPEX_HITO_FECHA_TIPO_ASIGNADA", cual); break;
        case "DIRPLC": $("#TituloTipoFecha").html("Fecha DIRPLC"); localStorage.setItem("CAPEX_HITO_FECHA_TIPO_ASIGNADA", cual); break;
    }
    $("#ModalIngresoFechaIndividual").show();
}
//
// ASIGNAR FECHA APROBACION
//
FNAsignarFechaAprobacion = function () {
    var fecha = $("#ValorFechaAprobacion").val();
    var cual = localStorage.getItem("CAPEX_HITO_FECHA_TIPO_ASIGNADA");
    switch (cual) {
        case "SAP":
            $("#BotonAgregarSAP").show();
            $("#HitSAP").val(moment(fecha).format("DD-MM-YYYY"));
            $("#HitSAP").show();
            break;
        case "CI":
            $("#BotonAgregarCI").show();
            $("#HitCI").val(moment(fecha).format("DD-MM-YYYY"));
            $("#HitCI").show();
            break;
        case "CA":
            $("#BotonAgregarCA").show();
            $("#HitCA").val(moment(fecha).format("DD-MM-YYYY"));
            $("#HitCA").show();
            break;
        case "OPR":
            $("#BotonAgregarOPR").show();
            $("#HitOPR").val(moment(fecha).format("DD-MM-YYYY"));
            $("#HitOPR").show();
            break;
        case "PE":
            $("#BotonAgregarPE").show();
            $("#HitPE").val(moment(fecha).format("DD-MM-YYYY"));
            $("#HitPE").show();
            break;
        case "DIRCEN":
            $("#BotonAgregarDIRCEN").show();
            $("#HitDIRCEN").val(moment(fecha).format("DD-MM-YYYY"));
            $("#HitDIRCEN").show();
            break;
        case "DIRPLC":
            $("#BotonAgregarDIRPLC").show();
            $("#HitDirPLC").val(moment(fecha).format("DD-MM-YYYY"));
            $("#HitDirPLC").show();
            break;
    }
    $("#ModalIngresoFechaIndividual").hide();
}
//
// CERRAR MODAL FECHA INDIVIDUAL
//
FNCerrarFechaAprobacion = function () {
    $("#ModalIngresoFechaIndividual").hide();
}
//
// CERRAR MODAL QUARTER
//
FNCerrarModalQuarter = function () {
    $("#ModalQuarter").hide();
}
