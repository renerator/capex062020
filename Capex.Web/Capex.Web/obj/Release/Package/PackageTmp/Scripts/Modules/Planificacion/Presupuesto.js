// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : PLANIFICACION
// FECHA            : MAYO 2018
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// SECCION          : PRESUPUESTO
// METODOS          :

//
//LISTAR DEPARTAMENTOS
//
FNObtenerDepartamentoDotacion = function () {
    //PREPARAR
    var DepartamentoDot = $('#DepartamentoDot');
    DepartamentoDot.empty();
    DepartamentoDot.append('<option selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../Planificacion/ListarDepartamentos",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            DepartamentoDot.empty();
            setTimeout(function () {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    DepartamentoDot.append(new Option(value.DepNombre, value.DepToken, false, false))
                    cuantos++;
                });
                if (cuantos == 1) {
                    DepartamentoDot.prop('selectedIndex', 0);
                }
                else if (cuantos > 1) {
                    DepartamentoDot.append('<option selected="true">Departamento..</option>')
                }
            }, 200);
        }
    });
}
//
// LISTAR TURNOS
//
FNObtenerTurnosDotacion = function () {
    //PREPARAR
    var Turno = $('#Turno');
    Turno.empty();
    Turno.append('<option selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../Planificacion/ListarTurnos",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            Turno.empty();
            setTimeout(function () {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    Turno.append(new Option(value.TurNombre, value.TurToken, false, false))
                    cuantos++;
                });
                if (cuantos == 1) {
                    Turno.prop('selectedIndex', 0);
                }
                else if (cuantos > 1) {
                    Turno.append('<option selected="true">Turnos..</option>')
                }
            }, 200);
        }
    });
}
//
// LISTAR UBICACIONES
//
FNObtenerUbicaciones = function () {
    //PREPARAR
    var Ubicacion = $('#Ubicacion');
    Ubicacion.empty();
    Ubicacion.append('<option selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../Planificacion/ListarUbicaciones",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            Ubicacion.empty();
            setTimeout(function () {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    Ubicacion.append(new Option(value.UbiNombre, value.UbiToken, false, false))
                    cuantos++;
                });
                if (cuantos == 1) {
                    Ubicacion.prop('selectedIndex', 0);
                }
                else if (cuantos > 1) {
                    Ubicacion.append('<option selected="true">Ubicacion..</option>')
                }
            }, 200);
        }
    });
}
//
// LISTAR TIPO EECC
//
FNObtenerTipoEECC = function () {
    //PREPARAR
    var TipoEECC = $('#TipoEECC');
    TipoEECC.empty();
    TipoEECC.append('<option selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../Planificacion/ListarTipoEECC",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            TipoEECC.empty();
            setTimeout(function () {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    TipoEECC.append(new Option(value.EeccNombre, value.EeccToken, false, false))
                    cuantos++;
                });
                if (cuantos == 1) {
                    TipoEECC.prop('selectedIndex', 0);
                }
                else if (cuantos > 1) {
                    TipoEECC.append('<option selected="true">Tipo EECC..</option>')
                }
            }, 200);
        }
    });
}
//
// LISTAR CLASIFICACION
//
FNObtenerClasificaciones = function () {
    //PREPARAR
    var ClasificacionDot = $('#ClasificacionDot');
    ClasificacionDot.empty();
    ClasificacionDot.append('<option selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../Planificacion/ListaClasificaciones",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            ClasificacionDot.empty();
            setTimeout(function () {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    ClasificacionDot.append(new Option(value.CdotNombre, value.CdotToken, false, false))
                    cuantos++;
                });
                if (cuantos == 1) {
                    ClasificacionDot.prop('selectedIndex', 0);
                }
                else if (cuantos > 1) {
                    ClasificacionDot.append('<option selected="true">Clasificacion..</option>')
                }
            }, 200);
        }
    });
}

//
// GUARDAR CONTRATO DOTACION
//
FNFuardarContratoDotacion = function () {
    var IniToken = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    var PidArea = localStorage.getItem("CAPEX_PLAN_AREA");
    var PidCodigoIniciativa = $("#CodigoIniciativa").val();
    var PidNombreProyecto = $("#NombreProyecto").val();
    var PidUsuario = $("#CAPEX_H_USERNAME").val();

    /***************************************** CAMPOS *********************************************/
    var NumContrato = $("#NumContrato").val();
    var NombreEECC = $("#NombreEECC").val();
    var CentroCosto = $("#CentroCosto").val();
    var SubContrato = $("#SubContrato").val();
    var DescripcionServicio = $("#DescripcionServicio").val();
    var SituacionFaena = $("#SituacionFaena").val();
    var Turno = $("#Turno").val();
    var Alimentacion = $("#Alimentacion").val();
    var Hoteleria = $("#Hoteleria").val();
    var PeriodoAnn = $("#PeriodoAnn").val();
    var DepartamentoDot = $("#DepartamentoDot").val();
    var CentroCosto = $("#CentroCosto").val();
    var ClasificacionDot = $("#ClasificacionDot").val();
    var TipoEECC = $("#TipoEECC").val();
    var SituacionProyecto = $("#SituacionProyecto").val();
    var Ubicacion = $("#Ubicacion").val();
    /***************************************** TABLA DOTACION ***********************************/

    var dotEne = $("#dotEne").val();
    var dotFeb = $("#dotFeb").val();
    var dotMar = $("#dotMar").val();
    var dotAbr = $("#dotAbr").val();
    var dotMay = $("#dotMay").val();
    var dotJun = $("#dotJun").val();
    var dotJul = $("#dotJul").val();
    var dotAgo = $("#dotAgo").val();
    var dotSep = $("#dotSep").val();
    var dotOct = $("#dotOct").val();
    var dotNov = $("#dotNov").val();
    var dotDic = $("#dotDic").val();

    if (dotEne == "") { dotEne = "0"; }
    if (dotFeb == "") { dotFeb = "0"; }
    if (dotMar == "") { dotMar = "0"; }
    if (dotAbr == "") { dotAbr = "0"; }
    if (dotMay == "") { dotMay = "0"; }
    if (dotJun == "") { dotJun = "0"; }
    if (dotJul == "") { dotJul = "0"; }
    if (dotAgo == "") { dotAgo = "0"; }
    if (dotSep == "") { dotSep = "0"; }
    if (dotOct == "") { dotOct = "0"; }
    if (dotNov == "") { dotNov = "0"; }
    if (dotDic == "") { dotDic = "0"; }

    var dotTot = parseInt(parseInt(dotEne) + parseInt(dotFeb) + parseInt(dotMar) + parseInt(dotAbr) + parseInt(dotJun) + parseInt(dotJul) + parseInt(dotAgo) + parseInt(dotAgo) + parseInt(dotSep) + parseInt(dotOct) + parseInt(dotNov) + parseInt(dotDic));
    /*****************************************VALIDACIONES ***********************************/

    if (IniToken == "" || IniToken == null) {
        swal("", "Debe identificar y guardar la iniciativa en el paso de 'Identificación'.", "info");
        return false;
    }
    else if (PeriodoAnn == "-1" || PeriodoAnn == "" || PeriodoAnn == null || NumContrato == "" || CentroCosto == "" || SituacionFaena == "-1" || Turno == "-1" || Alimentacion == "-1" || Hoteleria == "-1" || Ubicacion == "-1") {
        swal("", "Debe completar el formulario, todos los campos son requeridos.", "info");
        return false;
    } else {
        var DTO = {
            "IniToken": IniToken,
            "PidArea": PidArea,
            "PidCodigoIniciativa": PidCodigoIniciativa,
            "PidNombreProyecto": PidNombreProyecto,
            "DotAnn": PeriodoAnn,
            "DotSitProyecto": SituacionProyecto,
            "DotSitFaena": SituacionFaena,
            "DotDepto": DepartamentoDot,
            "DotNumContrato": NumContrato,
            "DotNombEECC": NombreEECC,
            "DotServicio": DescripcionServicio,
            "DotSubContrato": SubContrato,
            "DotCodCentro": CentroCosto,
            "DotTurno": Turno,
            "DotHoteleria": Hoteleria,
            "DotAlimentacion": Alimentacion,
            "DotUbicacion": Ubicacion,
            "DotClasificacion": ClasificacionDot,
            "DotTipoEECC": TipoEECC,
            "DotTotalDotacion": dotTot,
            "DotEne": dotEne,
            "DotFeb": dotFeb,
            "DotMar": dotMar,
            "DotAbr": dotAbr,
            "DotMay": dotMay,
            "DotJun": dotJun,
            "DotJul": dotJul,
            "DotAgo": dotAgo,
            "DotSep": dotSep,
            "DotOct": dotOct,
            "DotNov": dotNov,
            "DotDic": dotDic
        }
        $.ajax({
            url: '../Planificacion/GuardarContratoDotacion',
            method: "POST",
            data: (DTO)
        }).done(function (r) {
            if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                document.getElementById('linkToLogout').click();
                return;
            }
            var response = JSON.parse(JSON.stringify(r));
            var contenido = response.Mensaje;
            if (contenido == "Guardado") {
                swal("Exito", "Dotación agregada correctamente.", "success")
                setTimeout(function () {
                    swal.close();
                    FNPoblarVistaDotacionResumen();
                    $("#ContenedorPresupuesto").show();
                    $("#ContenedorContratosDotacion").show();
                    $("#ContenedorDotacion").hide()
                    FNLimpiarFormContratoDotacion();
                }, 3000);
            }
            else {
                swal("", "Error al intentar guardar dotación, por favor intente nuevamente.", "error")
                setTimeout(function () {
                    swal.close();
                    $("#ContenedorPresupuesto").show();
                    $("#ContenedorDotacion").hide();
                    FNLimpiarFormContratoDotacion();
                }, 3000);
            }
        }).fail(function (xhr) {
            console.log('error', xhr);
        });

    }

}
FNLimpiarFormContratoDotacion = function () {
    $("#NumContrato").val("");
    $("#SubContrato").val("");
    $("#DescripcionServicio").val("");
    $("#SituacionFaena").val("");
    $("#Turno").val("");
    $("#Alimentacion").val("");
    $("#Hoteleria").val("");
    $("#PeriodoAnn").val("");
    $("#DepartamentoDot").val("");
    $("#CentroCosto").val("");
    $("#ClasificacionDot").val("");
    $("#TipoEECC").val("");
    $("#SituacionProyecto").val("");
    $("#Ubicacion").val("");
    $("#dotEne").val("0");
    $("#dotFeb").val("0");
    $("#dotMar").val("0");
    $("#dotAbr").val("0");
    $("#dotMay").val("0");
    $("#dotJun").val("0");
    $("#dotJul").val("0");
    $("#dotAgo").val("0");
    $("#dotSep").val("0");
    $("#dotOct").val("0");
    $("#dotNov").val("0");
    $("#dotDic").val("0");
    return true;
}
FNModificarContratoDotacion = function (token) {
    swal("", "Esta funcionalidad se encuentra en desarrollo, ETA 23-03-2019.)", "info");
    return false;
}
//
// ELIMINAR DOTACION
//
FNEliminarContratoDotacion = function (token) {
    $.ajaxSetup({ cache: false });
    $.ajax({
        url: "../Planificacion/EliminarContratoDotacion",
        method: "GET",
        data: { "token": token }
    }).done(function (request) {
        if (request && request.redirectUrlLogout && request.redirectUrlLogout == "true") {
            document.getElementById('linkToLogout').click();
            return;
        }
        swal("", "Dotación eliminada.", "success");
        $("#ContenedorDotacionesResumen").html("");
        $("#ContenedorDotacion").hide()
        $("#ContenedorContratosDotacion").hide();
        FNLimpiarFormContratoDotacion();
    }).fail(function (xhr) { console.log('error', xhr); });
    return false;
}
//
// ACCIONES DOTACION
//
document.getElementById('BotonVolverPresupuesto').onclick = function (e) {
    FNPoblarVistaDotacionResumen();
    $("#ContenedorPresupuesto").show();
    $("#ContenedorContratosDotacion").show();
    $("#ContenedorDotacion").hide()
}
document.getElementById('BotonAgregarDotacion').onclick = function (e) {
    $("#ContenedorPresupuesto").hide();
    $("#ContenedorDotacion").show()
}
//
// VER DOTACION AGREGADA
//
FNPoblarVistaDotacionResumen = function () {
    //PREPARAR
    $.ajaxSetup({ cache: false });
    //FINANCIERO
    $.ajax({
        url: '../Planificacion/ListarContratosDotacionResumen',
        method: "GET",
        data: { "IniToken": localStorage.getItem("CAPEX_INICIATIVA_TOKEN") }
    }).done(function (r) {
        $("#ContenedorDotacionesResumen").html(r);
        $(".paginate").paginga({
            itemsPerPage: 1,
            itemsContainer: ".items",
            item: "> div",
            page: 1,
            nextPage: ".nextPage",
            previousPage: ".previousPage",
            firstPage: ".firstPage",
            lastPage: ".lastPage",
            pageNumbers: ".pageNumbers",
            currentPageClass: "active",
            pager: ".pager",
            maxPageNumbers: false,
            autoHidePager: true,
            scrollToTop: {
                offset: 15,
                speed: 100,
            },
            history: false,
            historyHashPrefix: "page-"

        });
    }).fail(function (xhr) {
        console.log('error', xhr);
    });
}

FNPreviousCallBackIngresoImportCheckToken = function () {
    var iniciativa_token = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    if (iniciativa_token == null || iniciativa_token == "") {
        swal("", "Complete y guarde 'Identificación' de iniciativa antes de importar información.", "info");
        return false;
    }
    return true;
}

FNPreviousCallBackIngresoImportExtensionError = function () {
    swal("Error", "Solo se permite la subida de archivos Excel (.xlsx)", "error");
}

FNPreviousCallBackIngresoImport = function () {
    console.log("FNPreviousCallBackImport");
    jQuery("#AppLoaderContainer").show();
}

FNCallBackIngresoImport = function (paramJson) {
    var tipo = localStorage.getItem("CAPEX_TIPO_INICIATIVA");
    var usuario = $("#CAPEX_H_USERNAME").val();
    var iniciativa_token = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    if (paramJson.Data.code == "0") {
        var DTO = { "token": iniciativa_token, "usuario": usuario, "archivo": paramJson.Data.nameFile, "tipo": tipo };

        console.log("DTO=", JSON.stringify(DTO));
        $.ajax({
            url: "/Planificacion/ProcesarTemplatePresupuestoFinal",
            type: "GET",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                jQuery("#AppLoaderContainer").hide();
                FNPoblarDocumentos();
                if (tipo == "CB" || tipo == "CD") {
                    setTimeout(function () {
                        PoblarVistaPresupuestoCasoBase();
                        setTimeout(function () {
                            PoblarVistaHitos();
                        }, 1000);
                    }, 2000);
                    setTimeout(function () {
                        PoblarVistaHitosGeneral();
                        setTimeout(function () {
                            PoblarVistaHitosResumen();
                        }, 3000);
                    }, 1000);
                }
                else {
                    setTimeout(function () {
                        PoblarVistaPresupuesto();
                        setTimeout(function () {
                            PoblarVistaHitos();
                        }, 1000);
                    }, 2000);
                    setTimeout(function () {
                        PoblarVistaHitosGeneral();
                        setTimeout(function () {
                            PoblarVistaHitosResumen();
                        }, 3000);
                    }, 1000);
                }
                swal("Exito", "Archivo template importado correctamente.", "success");
                localStorage.setItem("CAPEX_INICIATIVA_PRESUPUESTO_ESTADO", "Guardado");
                //COMPORTAMIENTO VISUAL DE HITOS
                $("#BotonGuardarEnviarIniciativa").prop('disabled', false);
                $("#ContenedorSinDatosFechasClavesInversion").hide();
                $("#ContenedorFechasClavesInversion").show();
            },
            error: function (xhr, error, status) {
                jQuery("#AppLoaderContainer").hide();
                swal("Error", "No es posible subir archivo o importar datos1.", "error")
            }
        });
    } else {
        jQuery("#AppLoaderContainer").hide();
        swal("Error", "No es posible subir archivo o importar datos2.", "error")
    }
}


//
// IMPORTAR TEMPLATE
//
/*document.getElementById('form_importar_template').onsubmit = function (e) {
    e.preventDefault();
    var iniciativa_token = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    if (iniciativa_token == null || iniciativa_token == "") {
        swal("", "Complete y guarde 'Identificación' de iniciativa antes de importar información.", "info");
        return false;
    }
    else {
        if (document.getElementById("ImportarTemplate").files.length == 0) {
            return false;
        }
        else {
            var formdata = new FormData();
            var fileInput = document.getElementById('ImportarTemplate');
            var nombreArchivo = fileInput.files[0].name;
            var tamano = fileInput.files[0].size;

            var extension = nombreArchivo.substr((nombreArchivo.lastIndexOf('.') + 1));
            formdata.append(fileInput.files[0].name, fileInput.files[0]);

            if (extension == "xlsx") {
                jQuery("#AppLoaderContainer").show();
                var ajaxRequest = $.ajax({
                    type: "POST",
                    url: "../Planificacion/SubirTemplatePresupuesto",
                    contentType: false,
                    processData: false,
                    data: formdata
                });
                ajaxRequest.done(function (xhr, textStatus) {
                    //
                    // PROCESAR ARCHIVO
                    //
                    var usuario = $("#CAPEX_H_USERNAME").val();
                    var tipo    = localStorage.getItem("CAPEX_TIPO_INICIATIVA");
                    var DTO = { "token": iniciativa_token, "usuario": usuario, "archivo": nombreArchivo, "tipo": tipo};
                    $.ajax({
                        url: "../Planificacion/ProcesarTemplatePresupuesto",
                        type: "GET",
                        dataType: "json",
                        data: (DTO),
                        success: function (r) {
                            if (tipo == "CB" || tipo == "CD") {
                                setTimeout(function () {
                                    PoblarVistaPresupuestoCasoBase();
                                    setTimeout(function () {
                                        PoblarVistaHitos();
                                    }, 1000);
                                }, 2000);
                                setTimeout(function () {
                                    PoblarVistaHitosGeneral();
                                    setTimeout(function () {
                                        PoblarVistaHitosResumen();
                                    }, 2500);
                                }, 1000);
                            }
                            else {
                                setTimeout(function () {
                                    PoblarVistaPresupuesto();
                                    setTimeout(function () {
                                        PoblarVistaHitos();
                                    }, 1000);
                                }, 2000);
                                setTimeout(function () {
                                    PoblarVistaHitosGeneral();
                                    setTimeout(function () {
                                        PoblarVistaHitosResumen();
                                    }, 2500);
                                }, 1000);
                            }

                            jQuery("#AppLoaderContainer").remove();
                            swal("Exito", "Archivo template importado correctamente.", "success");
                            localStorage.setItem("CAPEX_INICIATIVA_PRESUPUESTO_ESTADO", "Guardado");

                            //COMPORTAMIENTO VISUAL DE HITOS
                            $("#BotonGuardarEnviarIniciativa").prop('disabled', false);
                            $("#ContenedorSinDatosFechasClavesInversion").hide();
                            $("#ContenedorFechasClavesInversion").show();
                        },
                        error: function (xhr, error, status) {
                            swal("Error", "No es posible subir archivo o importar datos.", "error");
                            return false;
                        }
                    });
                    //
                    // REGISTRAR ARCHIVO
                    //
                    var DTO = { "IniToken": iniciativa_token, "ParUsuario": usuario, "ParNombre": nombreArchivo, "ParPaso": "Presupuesto", "ParCaso": "Template" };
                    $.ajax({
                        url: "../Planificacion/RegistrarArchivo",
                        type: "GET",
                        dataType: "json",
                        data: (DTO),
                        success: function (r) {
                           FNPoblarDocumentos();
                        },
                        error: function (xhr, error, status) {
                            return false;
                        }
                    });

                });
            }
            else {
                swal("Error", "Solo se permite la subida de archivos Excel (.Xlsx)", "error");
                return false;
            }
        }
    }
}
*/

FNPreviousCallBackIngresoUploadGanttCheckToken = function () {
    var iniciativa_token = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    if (iniciativa_token == null || iniciativa_token == "") {
        swal("", "Complete y guarde 'Identificación' de iniciativa antes de importar información.", "info");
        return false;
    }
    return true;
}

FNPreviousCallBackIngresoUploadGantt = function () {
    console.log("FNPreviousCallBackIngresoUploadGantt");
    jQuery("#AppLoaderContainer").show();
}

FNCallBackIngresoUploadGantt = function (paramJson) {
    console.log("FNCallBackIngresoUploadGantt");
    jQuery("#AppLoaderContainer").hide();
    if (paramJson.Data.code == "0") {
        swal("Exito", "Archivo carta gantt subido correctamente", "success");
        FNPoblarDocumentos();
    } else {
        swal("Error", "No se puede subir archivo de Carta Gantt", "error");
    }
}

//
// IMPORTAR GANTT
//
/*document.getElementById('form_importar_gantt').onsubmit = function (e) {
    e.preventDefault();
    var iniciativa_token = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    if (iniciativa_token == null || iniciativa_token == "") {
        swal("", "Complete y guarde 'Identificación' de iniciativa antes de importar información.", "info");
        return false;
    }
    else
    {
        if (document.getElementById("ArchivoGantt").files.length == 0 || document.getElementById("ArchivoGantt").files.length == null) {
            return false;
        }
        else
        {
            jQuery("#AppLoaderContainer").show();
            var formdata = new FormData();
            var fileInput = document.getElementById('ArchivoGantt');
            var nombreArchivo = fileInput.files[0].name;
            $("#NombreArchivoGantt").html(nombreArchivo);
            var tamano = fileInput.files[0].size;

            var extension = nombreArchivo.substr((nombreArchivo.lastIndexOf('.') + 1));
            formdata.append(fileInput.files[0].name, fileInput.files[0]);
            var ajaxRequest = $.ajax({
                type: "POST",
                url: "../Planificacion/SubirCartaGanttPresupuesto",
                contentType: false,
                processData: false,
                data: formdata
            });

            ajaxRequest.done(function (xhr, textStatus) {
                var parusuario = $("#CAPEX_H_USERNAME").val();
                var DTO = { "IniToken": iniciativa_token, "ParUsuario": parusuario, "ParNombre": nombreArchivo, "ParPaso": "Presupuesto-Gantt", "ParCaso": "Carta Gantt" };
                $.ajax({
                    url: "../Planificacion/RegistrarArchivo",
                    type: "GET",
                    dataType: "json",
                    data: (DTO),
                    success: function (r) {
                        jQuery("#AppLoaderContainer").hide();
                        swal("Exito", "Archivo carta gantt subido correctamente", "success");
                        FNPoblarDocumentos();
                    },
                    error: function (xhr, error, status) {
                        jQuery("#AppLoaderContainer").hide();
                        swal("Error", "No se puede subir archivo de Carta Gantt", "error");
                        return false;
                    }
                });
            });
        }
    }
}*/
//
// POBLAR VISTA DE PRESUPUESTO
//
PoblarVistaPresupuesto = function (token) {
    //PREPARAR
    $.ajaxSetup({ cache: false });
    //FINANCIERO
    $.ajax({
        url: '../Planificacion/PoblarVistaPresupuestoTabla1',
        method: "GET",
        data: { "token": localStorage.getItem("CAPEX_INICIATIVA_TOKEN") }
    }).done(function (resp) {
        $("#tabla1 > tbody").empty();
        $('#tabla1').append(resp);
    }).fail(function (xhr) {
        console.log('error', xhr);
    });
    //FISICO
    $.ajax({
        url: '../Planificacion/PoblarVistaPresupuestoTabla2',
        method: "GET",
        data: { "token": localStorage.getItem("CAPEX_INICIATIVA_TOKEN") }
    }).done(function (resp) {
        $("#tabla2 > tbody").empty();
        $('#tabla2').append(resp);
    }).fail(function (xhr) {
        console.log('error', xhr);
    });
}
//
// POBLAR VISTA DE PRESUPUESTO CASO BASE
//
PoblarVistaPresupuestoCasoBase = function (token) {
    //PREPARAR
    $.ajaxSetup({ cache: false });
    //FINANCIERO
    $.ajax({
        url: '../Planificacion/PoblarVistaPresupuestoCasoBaseTabla1',
        method: "GET",
        data: { "token": localStorage.getItem("CAPEX_INICIATIVA_TOKEN") }
    }).done(function (resp) {
        $("#tablacasobase1 > tbody").empty();
        $('#tablacasobase1').append(resp);
    }).fail(function (xhr) {
        console.log('error', xhr);
    });
    //FISICO
    $.ajax({
        url: '../Planificacion/PoblarVistaPresupuestoCasoBaseTabla2',
        method: "GET",
        data: { "token": localStorage.getItem("CAPEX_INICIATIVA_TOKEN") }
    }).done(function (resp) {
        $("#tablacasobase2 > tbody").empty();
        $('#tablacasobase2').append(resp);
    }).fail(function (xhr) {
        console.log('error', xhr);
    });
}
//
// POBLAR VISTA CAPEX HITOS
//
PoblarVistaHitos = function (token) {
    //PREPARAR
    console.info("PoblarVistaHitos localStorage.getItem(\"CAPEX_INICIATIVA_TOKEN\")=", localStorage.getItem("CAPEX_INICIATIVA_TOKEN"));
    $.ajaxSetup({ cache: false });
    //FINANCIERO
    $.ajax({
        url: '../Planificacion/PoblarVistaHitos',
        method: "GET",
        data: { "token": localStorage.getItem("CAPEX_INICIATIVA_TOKEN") }
    }).done(function (resp) {
        $("#tabla10 > tbody").empty();
        $("#ContenedorSinDatosResumenFinanciero").hide();
        $("#ContenedorResumenFinanciero").show();
        $('#tabla10').append(resp);
        //HitosTotalCapex

        var HitosTotalCapex = $("#HitosTotalCapex6").val();
        localStorage.setItem("CAPEX_HITO_TOTAL", HitosTotalCapex);
        if (HitosTotalCapex == null || HitosTotalCapex == "") {
            swal("Error", "No se puede calcular 'total capex, error interno. Favor, volver a cargar template.", "error");
            return false;
        }
        else {
            HitosTotalCapex = HitosTotalCapex.replace(".", "");
            HitosTotalCapex = parseInt(HitosTotalCapex);
        }


        if (HitosTotalCapex < 500) {
            $("#BotonAgregarSAP").show();
        }
        else if (HitosTotalCapex > 500 && HitosTotalCapex < 10000) {
            $("#BotonAgregarSAP").show();
            $("#BotonAgregarCI").show();
        }
        else if (HitosTotalCapex >= 10000 && HitosTotalCapex < 15000) {
            $("#BotonAgregarSAP").show();
            $("#BotonAgregarCI").show();
            $("#BotonAgregarCA").show();
        }
        /*else if (HitosTotalCapex >= 10000 && HitosTotalCapex < 15000) {
            $("#BotonAgregarSAP").show();
            $("#BotonAgregarCI").show();
            $("#BotonAgregarCA").show();
            $("#BotonAgregarOPR").show()
            $("#BotonAgregarDIRCEN").show();
        }*/
        else if (HitosTotalCapex >= 15000 && HitosTotalCapex < 50000) {
            $("#BotonAgregarSAP").show();
            $("#BotonAgregarCI").show();
            $("#BotonAgregarCA").show();
            $("#BotonAgregarOPR").show()
            $("#BotonAgregarPE").show()
            $("#BotonAgregarDIRCEN").show();
        }
        else if (HitosTotalCapex >= 50000) {
            $("#BotonAgregarSAP").show();
            $("#BotonAgregarCI").show();
            $("#BotonAgregarCA").show();
            $("#BotonAgregarOPR").show()
            $("#BotonAgregarPE").show()
            $("#BotonAgregarDIRCEN").show();
            $("#BotonAgregarDIRPLC").show();
        }

    }).fail(function (xhr) {
        console.log('error', xhr);
    });
}
//
// POBLAR VISTA CAPEX HITOS - GENERAL
//
PoblarVistaHitosGeneral = function (token) {
    //PREPARAR
    $.ajaxSetup({ cache: false });
    //FINANCIERO
    $.ajax({
        url: '../Planificacion/PoblarVistaHitosGeneral',
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
        url: '../Planificacion/PoblarVistaHitosResumen',
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
// GRAFICOS DE COMPROBACION 2
//
FNMostrarModalGraficoComprobacion2 = function () {
    var estado_importacion = localStorage.getItem("CAPEX_INICIATIVA_PRESUPUESTO_ESTADO");
    if (estado_importacion == "Guardado") {
        FNGraficoFaseIngenieria();
        $("#ModalGraficoComprobacion2").show();
        return;
    }
    else {
        swal("", "Debe cargar template para realizar comprobación", "info");
        return false;
    }
    return
}
FNCerrarModalGraficoComprobacion2 = function () {
    $("#ModalGraficoComprobacion2").hide();
    return
}
FNGraficoFaseIngenieria = function () {
    new Chart(document.getElementById("GC2"), {
        type: 'line',
        data: {
            labels: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic", "Sgtes"],
            datasets: [{
                data: [0, 0, 1, 2, 8, 9, 16, 10, 20, 13, 13, 3, 100],
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
                text: 'Fase Ingeniería - Avance Físico v/s Financiero'
            }
        }
    });
}
//
// GRAFICOS DE COMPROBACION 3
//
FNMostrarModalGraficoComprobacion3 = function () {
    var estado_importacion = localStorage.getItem("CAPEX_INICIATIVA_PRESUPUESTO_ESTADO");
    if (estado_importacion == "Guardado") {
        FNGraficoFaseAdquisiciones();
        $("#ModalGraficoComprobacion3").show();
        return;
    }
    else {
        swal("", "Debe cargar template para realizar comprobación", "info");
        return false;
    }
    return
}
FNCerrarModalGraficoComprobacion3 = function () {
    $("#ModalGraficoComprobacion3").hide();
    return
}
FNGraficoFaseAdquisiciones = function () {
    new Chart(document.getElementById("GC3"), {
        type: 'line',
        data: {
            labels: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic", "Sgtes"],
            datasets: [{
                data: [0, 0, 1, 2, 8, 9, 16, 10, 20, 13, 13, 3, 100],
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
                text: 'Fase Adquisiciones - Avance Físico v/s Financiero'
            }
        }
    });
}
//
// GRAFICOS DE COMPROBACION 4
//
FNMostrarModalGraficoComprobacion4 = function () {
    var estado_importacion = localStorage.getItem("CAPEX_INICIATIVA_PRESUPUESTO_ESTADO");
    if (estado_importacion == "Guardado") {
        FNGraficoFaseConstruccion();
        $("#ModalGraficoComprobacion4").show();
        return;
    }
    else {
        swal("", "Debe cargar template para realizar comprobación", "info");
        return false;
    }
    return
}
FNCerrarModalGraficoComprobacion4 = function () {
    $("#ModalGraficoComprobacion4").hide();
    return
}
FNGraficoFaseConstruccion = function () {
    new Chart(document.getElementById("GC4"), {
        type: 'line',
        data: {
            labels: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic", "Sgtes"],
            datasets: [{
                data: [0, 0, 1, 2, 8, 9, 16, 10, 20, 13, 13, 3, 100],
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
                text: 'Fase Construcción - Avance Físico v/s Financiero'
            }
        }
    });
}
