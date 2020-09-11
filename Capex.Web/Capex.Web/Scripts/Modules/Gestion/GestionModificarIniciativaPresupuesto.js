
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
        url: "../../Planificacion/ListarDepartamentos",
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
                    DepartamentoDot.append(new Option(value.DepNombre, value.DepToken, false, false));
                    cuantos++;
                });
                if (cuantos == 1) {
                    DepartamentoDot.prop('selectedIndex', 0);
                }
                else if (cuantos > 1) {
                    DepartamentoDot.append('<option selected="true" value="-1">Seleccionar..</option>')
                }
            }, 200);
        }
    });
}

FNObtenerDepartamentoDotacionMod = function () {
    //PREPARAR
    var DepartamentoDot = $('#DepartamentoDotMod');
    DepartamentoDot.empty();
    DepartamentoDot.append('<option selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../../Planificacion/ListarDepartamentos",
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
                    DepartamentoDot.append(new Option(value.DepNombre, value.DepToken, false, false));
                    cuantos++;
                });
                if (cuantos == 1) {
                    DepartamentoDot.prop('selectedIndex', 0);
                }
                else if (cuantos > 1) {
                    DepartamentoDot.append('<option selected="true" value="-1">Seleccionar..</option>')
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
        url: "../../Planificacion/ListarTurnos",
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
                    Turno.append('<option selected="true" value="-1">Seleccionar..</option>')
                }
            }, 200);
        }
    });
}

FNObtenerTurnosDotacionMod = function () {
    //PREPARAR
    var Turno = $('#TurnoMod');
    Turno.empty();
    Turno.append('<option selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../../Planificacion/ListarTurnos",
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
                    Turno.append('<option selected="true" value="-1">Seleccionar..</option>')
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
        url: "../../Planificacion/ListarUbicaciones",
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
                    Ubicacion.append('<option selected="true" value="-1">Seleccionar..</option>')
                }
            }, 200);
        }
    });
}

FNObtenerUbicacionesMod = function () {
    //PREPARAR
    var Ubicacion = $('#UbicacionMod');
    Ubicacion.empty();
    Ubicacion.append('<option selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../../Planificacion/ListarUbicaciones",
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
                    Ubicacion.append('<option selected="true" value="-1">Seleccionar..</option>')
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
        url: "../../Planificacion/ListarTipoEECC",
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
                    TipoEECC.append('<option selected="true" value="-1">Seleccionar..</option>')
                }
            }, 200);
        }
    });
}

FNObtenerTipoEECCMod = function () {
    //PREPARAR
    var TipoEECC = $('#TipoEECCMod');
    TipoEECC.empty();
    TipoEECC.append('<option selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../../Planificacion/ListarTipoEECC",
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
                    TipoEECC.append('<option selected="true" value="-1">Seleccionar..</option>')
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
        url: "../../Planificacion/ListaClasificaciones",
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
                    ClasificacionDot.append('<option selected="true" value="-1">Seleccionar..</option>')
                }
            }, 200);
        }
    });
}

FNObtenerClasificacionesMod = function () {
    //PREPARAR
    var ClasificacionDot = $('#ClasificacionDotMod');
    ClasificacionDot.empty();
    ClasificacionDot.append('<option selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../../Planificacion/ListaClasificaciones",
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
                    ClasificacionDot.append('<option selected="true" value="-1">Seleccionar..</option>')
                }
            }, 200);
        }
    });
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
// GUARDAR CONTRATO DOTACION
//
FNFuardarContratoDotacion = function () {
    var IniToken = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    var PidArea = localStorage.getItem("CAPEX_PLAN_AREA");
    var PidCodigoIniciativa = $("#CodigoIniciativa").val();
    var PidNombreProyecto = $("#NombreProyecto").val();
    var PidUsuario = $("#CAPEX_H_USERNAME").val();

    /***************************************** CAMPOS *********************************************/
    var PeriodoAnn = $("#PeriodoAnn").val();
    var DepartamentoDot = $("#DepartamentoDot").val();
    var CentroCosto = $("#CentroCosto").val();
    var ClasificacionDot = $("#ClasificacionDot").val();
    var TipoEECC = $("#TipoEECC").val();
    var SituacionProyecto = $("#SituacionProyecto").val();
    var Ubicacion = $("#Ubicacion").val();
    var NumContrato = $("#NumContrato").val();
    var NombreEECC = $("#NombreEECC").val();
    var SubContrato = $("#SubContrato").val();
    var DescripcionServicio = $("#DescripcionServicio").val();
    var SituacionFaena = $("#SituacionFaena").val();
    var Turno = $("#Turno").val();
    var Alimentacion = $("#Alimentacion").val();
    var Hoteleria = $("#Hoteleria").val();

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

    if (!PeriodoAnn || PeriodoAnn == undefined || PeriodoAnn == "-1") {
        swal("", "El Año es un campo requerido.", "info");
        return false;
    }

    if (!DepartamentoDot || DepartamentoDot == undefined || DepartamentoDot == "-1") {
        swal("", "El Departamento es un campo requerido.", "info");
        return false;
    }

    if (!CentroCosto || CentroCosto == undefined || CentroCosto.trim() == "") {
        swal("", "El Centro de Costo es un campo requerido.", "info");
        return false;
    }

    if (!ClasificacionDot || ClasificacionDot == undefined || ClasificacionDot == "-1") {
        swal("", "La Clasificación es un campo requerido.", "info");
        return false;
    }

    if (!TipoEECC || TipoEECC == undefined || TipoEECC == "-1") {
        swal("", "El Tipo EECC es un campo requerido.", "info");
        return false;
    }

    if (!SituacionProyecto || SituacionProyecto == undefined || SituacionProyecto == "-1") {
        swal("", "La Situación del Proyecto es un campo requerido.", "info");
        return false;
    }

    if (!Ubicacion || Ubicacion == undefined || Ubicacion == "-1") {
        swal("", "La Ubicación es un campo requerido.", "info");
        return false;
    }

    if (!NumContrato || NumContrato == undefined || NumContrato.trim() == "") {
        swal("", "El N° de Contrato es un campo requerido.", "info");
        return false;
    }

    if (!NombreEECC || NombreEECC == undefined || NombreEECC.trim() == "") {
        swal("", "El Nombre EECC es un campo requerido.", "info");
        return false;
    }

    if (!SubContrato || SubContrato == undefined || SubContrato.trim() == "") {
        swal("", "El Sub Contrato es un campo requerido.", "info");
        return false;
    }

    if (!DescripcionServicio || DescripcionServicio == undefined || DescripcionServicio.trim() == "") {
        swal("", "La Descripción del Servicio es un campo requerido.", "info");
        return false;
    }

    if (!SituacionFaena || SituacionFaena == undefined || SituacionFaena == "-1") {
        swal("", "La Situación de la Faena es un campo requerido.", "info");
        return false;
    }

    if (!Turno || Turno == undefined || Turno == "-1") {
        swal("", "El Turno es un campo requerido.", "info");
        return false;
    }

    if (!Alimentacion || Alimentacion == undefined || Alimentacion == "-1") {
        swal("", "La Alimentación es un campo requerido.", "info");
        return false;
    }

    if (!Hoteleria || Hoteleria == undefined || Hoteleria == "-1") {
        swal("", "La Hoteleria es un campo requerido.", "info");
        return false;
    }

    var dotTot = parseInt(parseInt(dotEne) + parseInt(dotFeb) + parseInt(dotMar) + parseInt(dotAbr) + parseInt(dotJun) + parseInt(dotJul) + parseInt(dotAgo) + parseInt(dotAgo) + parseInt(dotSep) + parseInt(dotOct) + parseInt(dotNov) + parseInt(dotDic));
    /*****************************************VALIDACIONES ***********************************/

    if (IniToken == "" || IniToken == null) {
        swal("", "Debe identificar y guardar la iniciativa en el paso de 'Identificación'.", "info");
        return false;
    }
    if (!PeriodoAnn || PeriodoAnn == undefined || PeriodoAnn == "-1" || !DepartamentoDot || DepartamentoDot == undefined || DepartamentoDot == "-1"
        || !CentroCosto || CentroCosto == undefined || CentroCosto.trim() == "" || !ClasificacionDot || ClasificacionDot == undefined || ClasificacionDot == "-1"
        || !TipoEECC || TipoEECC == undefined || TipoEECC == "-1" || !SituacionProyecto || SituacionProyecto == undefined || SituacionProyecto == "-1"
        || !Ubicacion || Ubicacion == undefined || Ubicacion == "-1" || !NumContrato || NumContrato == undefined || NumContrato.trim() == ""
        || !NombreEECC || NombreEECC == undefined || NombreEECC.trim() == "" || !SubContrato || SubContrato == undefined || SubContrato.trim() == ""
        || !DescripcionServicio || DescripcionServicio == undefined || DescripcionServicio.trim() == "" || !SituacionFaena || SituacionFaena == undefined || SituacionFaena == "-1"
        || !Turno || Turno == undefined || Turno == "-1" || !Alimentacion || Alimentacion == undefined || Alimentacion == "-1"
        || !Hoteleria || Hoteleria == undefined || Hoteleria == "-1") {
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
            url: '../../Planificacion/GuardarContratoDotacion',
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
                    FNLimpiarFormContratoDotacion();
                    $("#ContenedorPresupuesto").show();
                    $("#ContenedorContratosDotacion").show();
                    $("#ContenedorDotacion").hide()
                }, 3000);
            }
            else {
                swal("", "Error al intentar guardar dotación, por favor intente nuevamente.", "error")
                setTimeout(function () {
                    swal.close();
                    FNLimpiarFormContratoDotacion();
                    $("#ContenedorPresupuesto").show();
                    $("#ContenedorDotacion").hide();
                }, 3000);
            }
        }).fail(function (xhr) {
            console.log('error', xhr);
        });

    }

}
FNLimpiarFormContratoDotacion = function () {
    $("#PeriodoAnn").val("-1");
    $("#DepartamentoDot").val("-1");
    $("#CentroCosto").val("");
    $("#ClasificacionDot").val("-1");
    $("#TipoEECC").val("-1");
    $("#SituacionProyecto").val("-1");
    $("#Ubicacion").val("-1");
    $("#NumContrato").val("");
    $("#NombreEECC").val("");
    $("#SubContrato").val("");
    $("#DescripcionServicio").val("");
    $("#SituacionFaena").val("-1");
    $("#Turno").val("-1");
    $("#Alimentacion").val("-1");
    $("#Hoteleria").val("-1");
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

FNLimpiarFormContratoDotacionMod = function () {
    $("#DotToken").val("");
    $("#PeriodoAnnMod").val("-1");
    $("#DepartamentoDotMod").val("-1");
    $("#CentroCostoMod").val("");
    $("#ClasificacionDotMod").val("-1");
    $("#TipoEECCMod").val("-1");
    $("#SituacionProyectoMod").val("-1");
    $("#UbicacionMod").val("-1");
    $("#NumContratoMod").val("");
    $("#NombreEECCMod").val("");
    $("#SubContratoMod").val("");
    $("#DescripcionServicioMod").val("");
    $("#SituacionFaenaMod").val("-1");
    $("#TurnoMod").val("-1");
    $("#AlimentacionMod").val("-1");
    $("#HoteleriaMod").val("-1");
    $("#dotEneMod").val("0");
    $("#dotFebMod").val("0");
    $("#dotMarMod").val("0");
    $("#dotAbrMod").val("0");
    $("#dotMayMod").val("0");
    $("#dotJunMod").val("0");
    $("#dotJulMod").val("0");
    $("#dotAgoMod").val("0");
    $("#dotSepMod").val("0");
    $("#dotOctMod").val("0");
    $("#dotNovMod").val("0");
    $("#dotDicMod").val("0");
    return true;
}

FNActualizarContratoDotacion = function () {
    var IniToken = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    var PidArea = localStorage.getItem("CAPEX_PLAN_AREA");
    var PidCodigoIniciativa = $("#CodigoIniciativa").val();
    var PidNombreProyecto = $("#NombreProyecto").val();
    var PidUsuario = $("#CAPEX_H_USERNAME").val();
    var DotToken = $("#DotToken").val();
    /***************************************** CAMPOS *********************************************/
    var PeriodoAnn = $("#PeriodoAnnMod").val();
    var DepartamentoDot = $("#DepartamentoDotMod").val();
    var CentroCosto = $("#CentroCostoMod").val();
    var ClasificacionDot = $("#ClasificacionDotMod").val();
    var TipoEECC = $("#TipoEECCMod").val();
    var SituacionProyecto = $("#SituacionProyectoMod").val();
    var Ubicacion = $("#UbicacionMod").val();
    var NumContrato = $("#NumContratoMod").val();
    var NombreEECC = $("#NombreEECCMod").val();
    var SubContrato = $("#SubContratoMod").val();
    var DescripcionServicio = $("#DescripcionServicioMod").val();
    var SituacionFaena = $("#SituacionFaenaMod").val();
    var Turno = $("#TurnoMod").val();
    var Alimentacion = $("#AlimentacionMod").val();
    var Hoteleria = $("#HoteleriaMod").val();

    /***************************************** TABLA DOTACION ***********************************/

    var dotEne = $("#dotEneMod").val();
    var dotFeb = $("#dotFebMod").val();
    var dotMar = $("#dotMarMod").val();
    var dotAbr = $("#dotAbrMod").val();
    var dotMay = $("#dotMayMod").val();
    var dotJun = $("#dotJunMod").val();
    var dotJul = $("#dotJulMod").val();
    var dotAgo = $("#dotAgoMod").val();
    var dotSep = $("#dotSepMod").val();
    var dotOct = $("#dotOctMod").val();
    var dotNov = $("#dotNovMod").val();
    var dotDic = $("#dotDicMod").val();

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

    if (!PeriodoAnn || PeriodoAnn == undefined || PeriodoAnn == "-1") {
        swal("", "El Año es un campo requerido.", "info");
        return false;
    }

    if (!DepartamentoDot || DepartamentoDot == undefined || DepartamentoDot == "-1") {
        swal("", "El Departamento es un campo requerido.", "info");
        return false;
    }

    if (!CentroCosto || CentroCosto == undefined || CentroCosto.trim() == "") {
        swal("", "El Centro de Costo es un campo requerido.", "info");
        return false;
    }

    if (!ClasificacionDot || ClasificacionDot == undefined || ClasificacionDot == "-1") {
        swal("", "La Clasificación es un campo requerido.", "info");
        return false;
    }

    if (!TipoEECC || TipoEECC == undefined || TipoEECC == "-1") {
        swal("", "El Tipo EECC es un campo requerido.", "info");
        return false;
    }

    if (!SituacionProyecto || SituacionProyecto == undefined || SituacionProyecto == "-1") {
        swal("", "La Situación del Proyecto es un campo requerido.", "info");
        return false;
    }

    if (!Ubicacion || Ubicacion == undefined || Ubicacion == "-1") {
        swal("", "La Ubicación es un campo requerido.", "info");
        return false;
    }

    if (!NumContrato || NumContrato == undefined || NumContrato.trim() == "") {
        swal("", "El N° de Contrato es un campo requerido.", "info");
        return false;
    }

    if (!NombreEECC || NombreEECC == undefined || NombreEECC.trim() == "") {
        swal("", "El Nombre EECC es un campo requerido.", "info");
        return false;
    }

    if (!SubContrato || SubContrato == undefined || SubContrato.trim() == "") {
        swal("", "El Sub Contrato es un campo requerido.", "info");
        return false;
    }

    if (!DescripcionServicio || DescripcionServicio == undefined || DescripcionServicio.trim() == "") {
        swal("", "La Descripción del Servicio es un campo requerido.", "info");
        return false;
    }

    if (!SituacionFaena || SituacionFaena == undefined || SituacionFaena == "-1") {
        swal("", "La Situación de la Faena es un campo requerido.", "info");
        return false;
    }

    if (!Turno || Turno == undefined || Turno == "-1") {
        swal("", "El Turno es un campo requerido.", "info");
        return false;
    }

    if (!Alimentacion || Alimentacion == undefined || Alimentacion == "-1") {
        swal("", "La Alimentación es un campo requerido.", "info");
        return false;
    }

    if (!Hoteleria || Hoteleria == undefined || Hoteleria == "-1") {
        swal("", "La Hoteleria es un campo requerido.", "info");
        return false;
    }

    var dotTot = parseInt(parseInt(dotEne) + parseInt(dotFeb) + parseInt(dotMar) + parseInt(dotAbr) + parseInt(dotJun) + parseInt(dotJul) + parseInt(dotAgo) + parseInt(dotAgo) + parseInt(dotSep) + parseInt(dotOct) + parseInt(dotNov) + parseInt(dotDic));
    /*****************************************VALIDACIONES ***********************************/

    if (IniToken == "" || IniToken == null) {
        swal("", "Debe identificar y guardar la iniciativa en el paso de 'Identificación'.", "info");
        return false;
    }
    if (!PeriodoAnn || PeriodoAnn == undefined || PeriodoAnn == "-1" || !DepartamentoDot || DepartamentoDot == undefined || DepartamentoDot == "-1"
        || !CentroCosto || CentroCosto == undefined || CentroCosto.trim() == "" || !ClasificacionDot || ClasificacionDot == undefined || ClasificacionDot == "-1"
        || !TipoEECC || TipoEECC == undefined || TipoEECC == "-1" || !SituacionProyecto || SituacionProyecto == undefined || SituacionProyecto == "-1"
        || !Ubicacion || Ubicacion == undefined || Ubicacion == "-1" || !NumContrato || NumContrato == undefined || NumContrato.trim() == ""
        || !NombreEECC || NombreEECC == undefined || NombreEECC.trim() == "" || !SubContrato || SubContrato == undefined || SubContrato.trim() == ""
        || !DescripcionServicio || DescripcionServicio == undefined || DescripcionServicio.trim() == "" || !SituacionFaena || SituacionFaena == undefined || SituacionFaena == "-1"
        || !Turno || Turno == undefined || Turno == "-1" || !Alimentacion || Alimentacion == undefined || Alimentacion == "-1"
        || !Hoteleria || Hoteleria == undefined || Hoteleria == "-1") {
        swal("", "Debe completar el formulario, todos los campos son requeridos.", "info");
        return false;
    } else {
        var DTO = {
            "IniToken": IniToken,
            "DotToken": DotToken,
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
            url: '../../Planificacion/ActualizarContratoDotacion',
            method: "POST",
            data: (DTO)
        }).done(function (r) {
            if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                document.getElementById('linkToLogout').click();
                return;
            }
            var response = JSON.parse(JSON.stringify(r));
            var contenido = response.Mensaje;
            if (contenido == "Actualizado") {
                swal("Exito", "Dotación actualizada correctamente.", "success")
                setTimeout(function () {
                    swal.close();
                    FNPoblarVistaDotacionResumen();
                    FNLimpiarFormContratoDotacionMod();
                    $("#ContenedorPresupuesto").show();
                    $("#ContenedorContratosDotacion").show();
                    $("#ContenedorDotacionMod").hide()
                }, 3000);
            }
            else {
                swal("", "Error al intentar actualizar dotación, por favor intente nuevamente.", "error")
                setTimeout(function () {
                    swal.close();
                    FNLimpiarFormContratoDotacionMod();
                    $("#ContenedorPresupuesto").show();
                    $("#ContenedorDotacionMod").hide();
                }, 3000);
            }
        }).fail(function (xhr) {
            console.log('error', xhr);
        });
    }
}

FNObtenerDotacion = function (DotToken) {
    $('#AppLoaderContainer').show();
    FNLimpiarFormContratoDotacionMod();
    $.ajaxSetup({ cache: false });
    $.ajax({
        url: "../../Planificacion/ObtenerContratoDotacionByToken",
        method: "GET",
        data: { "DotToken": DotToken }
    }).done(function (response) {
        console.log("response=", response);
        $('#AppLoaderContainer').hide();
        if (response && response.redirectUrlLogout && response.redirectUrlLogout == "true") {
            document.getElementById('linkToLogout').click();
            return;
        }
        if (response && response.Dotacion) {
            $("#DotToken").val(response.Dotacion.DotToken);
            if (response.Dotacion.DotAnn != undefined && response.Dotacion.DotAnn != "") {
                $("#PeriodoAnnMod").val(response.Dotacion.DotAnn);
            }
            if (response.Dotacion.DotDepto != undefined && response.Dotacion.DotDepto != "") {
                $("#DepartamentoDotMod").val(response.Dotacion.DotDepto);
            }

            $("#CentroCostoMod").val(response.Dotacion.DotCodCentro);

            if (response.Dotacion.DotClasificacion != undefined && response.Dotacion.DotClasificacion != "") {
                $("#ClasificacionDotMod").val(response.Dotacion.DotClasificacion);
            }
            if (response.Dotacion.DotTipoEECC != undefined && response.Dotacion.DotTipoEECC != "") {
                $("#TipoEECCMod").val(response.Dotacion.DotTipoEECC);
            }
            if (response.Dotacion.DotSitProyecto != undefined && response.Dotacion.DotSitProyecto != "") {
                $("#SituacionProyectoMod").val(response.Dotacion.DotSitProyecto);
            }
            if (response.Dotacion.DotUbicacion != undefined && response.Dotacion.DotUbicacion != "") {
                $("#UbicacionMod").val(response.Dotacion.DotUbicacion);
            }

            $("#NumContratoMod").val(response.Dotacion.DotNumContrato);
            $("#NombreEECCMod").val(response.Dotacion.DotNombEECC);
            $("#SubContratoMod").val(response.Dotacion.DotSubContrato);
            $("#DescripcionServicioMod").val(response.Dotacion.DotServicio);

            if (response.Dotacion.DotSitFaena != undefined && response.Dotacion.DotSitFaena != "") {
                $("#SituacionFaenaMod").val(response.Dotacion.DotSitFaena);
            }
            if (response.Dotacion.DotTurno != undefined && response.Dotacion.DotTurno != "") {
                $("#TurnoMod").val(response.Dotacion.DotTurno);
            }
            if (response.Dotacion.DotAlimentacion != undefined && response.Dotacion.DotAlimentacion != "") {
                $("#AlimentacionMod").val(response.Dotacion.DotAlimentacion);
            }
            if (response.Dotacion.DotHoteleria != undefined && response.Dotacion.DotHoteleria != "") {
                $("#HoteleriaMod").val(response.Dotacion.DotHoteleria);
            }
            $("#dotEneMod").val(response.Dotacion.DotEne);
            $("#dotFebMod").val(response.Dotacion.DotFeb);
            $("#dotMarMod").val(response.Dotacion.DotMar);
            $("#dotAbrMod").val(response.Dotacion.DotAbr);
            $("#dotMayMod").val(response.Dotacion.DotMay);
            $("#dotJunMod").val(response.Dotacion.DotJun);
            $("#dotJulMod").val(response.Dotacion.DotJul);
            $("#dotAgoMod").val(response.Dotacion.DotAgo);
            $("#dotSepMod").val(response.Dotacion.DotSep);
            $("#dotOctMod").val(response.Dotacion.DotOct);
            $("#dotNovMod").val(response.Dotacion.DotNov);
            $("#dotDicMod").val(response.Dotacion.DotDic);
        }
        $("#ContenedorPresupuesto").hide();
        $("#ContenedorDotacionMod").show()
    }).fail(function (xhr) {
        $('#AppLoaderContainer').hide();
        console.log('error', xhr);
    });
}
//
// ELIMINAR DOTACION
//
FNEliminarContratoDotacion = function (token) {
    swal({
        title: 'Esta seguro?',
        text: "Está seguro que desea eliminar la Dotación?",
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
            $('#AppLoaderContainer').show();
            $.ajaxSetup({ cache: false });
            $.ajax({
                url: "../../Planificacion/EliminarContratoDotacion",
                method: "GET",
                data: { "token": token }
            }).done(function (request) {
                $('#AppLoaderContainer').hide();
                if (request && request.redirectUrlLogout && request.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                swal("", "Dotación eliminada.", "success");
                $("#ContenedorDotacionesResumen").html("");
                $("#ContenedorDotacion").hide()
                $("#ContenedorContratosDotacion").hide();
                FNPoblarVistaDotacionResumen();
            }).fail(function (xhr) {
                $('#AppLoaderContainer').hide();
                console.log('error', xhr);
            });
        } else {
            return false;
        }
    });
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
document.getElementById('BotonVolverPresupuestoMod').onclick = function (e) {
    FNPoblarVistaDotacionResumen();
    $("#ContenedorPresupuesto").show();
    $("#ContenedorContratosDotacion").show();
    $("#ContenedorDotacionMod").hide()
}
document.getElementById('BotonAgregarDotacion').onclick = function (e) {
    $("#ContenedorPresupuesto").hide();
    $("#ContenedorDotacion").show();
    FNLimpiarFormContratoDotacion();
}
//
// VER DOTACION AGREGADA
//
FNPoblarVistaDotacionResumen = function () {
    //PREPARAR
    $("#ContenedorDotacionesResumen").html('');
    $(".pageNumbers").html("");
    $(".pager").hide();
    $.ajaxSetup({ cache: false });
    //FINANCIERO
    $.ajax({
        url: '../../Planificacion/ListarContratosDotacionResumen',
        method: "GET",
        data: { "IniToken": localStorage.getItem("CAPEX_INICIATIVA_TOKEN") }
    }).done(function (r) {
        if (r && r.Error && r.Error == "false") {
            if (r.TotalItems > 0) {
                $("#ContenedorContratosDotacion").show();
                $("#ContenedorDotacionesResumen").html(r.ContenedorDotacionesResumen);
                if (r.TotalItems > 1) {
                    $(".pager").show();
                    $(".pageNumbers").html(r.Pager);
                    ChangePage(1);
                }
            } else {
                $("#ContenedorContratosDotacion").hide();
            }
        } else {
            $("#ContenedorContratosDotacion").hide();
        }
        /*$(".paginate").paginga({
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
        });*/
    }).fail(function (xhr) {
        $("#ContenedorContratosDotacion").hide();
        console.log('error', xhr);
    });
}


ChangePage = function (page) {
    console.log("ChangePage page=" + page);
    console.log("$('.pageNumbers a').length=" + $('.pageNumbers a').length);
    var totalPages = $('.pageNumbers a').length;
    var selectorSelectedPage = 'pageSelected_' + page;
    var selectorSelectedDotacion = 'datosDotacionIniciativa_' + page;
    $(('#' + selectorSelectedPage)).addClass('active');
    $(('#' + selectorSelectedDotacion)).show();
    var contador;
    for (contador = 1; contador <= totalPages; contador++) {
        if (contador != page) {
            $(('#pageSelected_' + contador)).removeClass('active');
            $(('#datosDotacionIniciativa_' + contador)).hide();
        }
    }
    console.log("final ChangePage");
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
                    url: "../../Planificacion/SubirTemplatePresupuesto",
                    contentType: false,
                    processData: false,
                    data: formdata
                });
                ajaxRequest.done(function (xhr, textStatus) {
                    var usuario = $("#CAPEX_H_USERNAME").val();
                    var tipo = localStorage.getItem("CAPEX_TIPO_INICIATIVA");
                    var DTO = { "token": iniciativa_token, "usuario": usuario, "archivo": nombreArchivo, "tipo": tipo };
                    $.ajax({
                        url: "../../Planificacion/ProcesarTemplatePresupuesto",
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
                            swal("Error", "No es posible subir archivo o importar datos.", "error")
                        }
                    });
                });
            }
            else {
                swal("Error", "Solo se permite la subida de archivos Excel (.Xlsx)", "error")
            }
        }
    }
}*/

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
    else {
        if (document.getElementById("ArchivoGantt").files.length == 0 || document.getElementById("ArchivoGantt").files.length == null) {
            return false;
        }
        else {
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
                url: "../../Planificacion/SubirCartaGanttPresupuesto",
                contentType: false,
                processData: false,
                data: formdata
            });

            ajaxRequest.done(function (xhr, textStatus) {
                var parusuario = $("#CAPEX_H_USERNAME").val();
                var DTO = { "IniToken": iniciativa_token, "ParUsuario": parusuario, "ParNombre": nombreArchivo, "ParPaso": "Presupuesto-Gantt", "ParCaso": "Carta Gantt" };
                $.ajax({
                    url: "../../Planificacion/RegistrarArchivo",
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
                        swal("Exito", "Archivo carta gantt subido correctamente", "success");
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
        url: '../../Planificacion/PoblarVistaPresupuestoTabla1',
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
        url: '../../Planificacion/PoblarVistaPresupuestoTabla2',
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
        url: '../../Planificacion/PoblarVistaPresupuestoCasoBaseTabla1',
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
        url: '../../Planificacion/PoblarVistaPresupuestoCasoBaseTabla2',
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
    console.info('PoblarVistaHitos localStorage.getItem("CAPEX_INICIATIVA_TOKEN")=', localStorage.getItem("CAPEX_INICIATIVA_TOKEN"));
    $.ajaxSetup({ cache: false });
    //FINANCIERO
    $.ajax({
        url: '../../Planificacion/PoblarVistaHitos',
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
            swal("Error", "No se puede calcular 'total capex' debido a fallo desde el servidor. Favor, volver a cargar template.)", "error");
            return false;
        }
        else {
            HitosTotalCapex = HitosTotalCapex.replace(".", "");
            HitosTotalCapex = parseInt(HitosTotalCapex);
        }

        $("#BotonAgregarSAP").hide();
        $("#BotonAgregarCI").hide();
        $("#BotonAgregarCA").hide();
        $("#BotonAgregarOPR").hide()
        $("#BotonAgregarPE").hide()
        $("#BotonAgregarDIRCEN").hide();
        $("#BotonAgregarDIRPLC").hide();

        $("#HitoSinInformacion").hide();
        $("#SuperContenedorHitosIngreso").show();

        if (HitosTotalCapex < 500) {
            $("#BotonAgregarSAP").show();
            $('#HitSAP').css('display', 'block');
        }
        else if (HitosTotalCapex > 500 && HitosTotalCapex <= 10000) {
            $("#BotonAgregarSAP").show();
            $("#BotonAgregarCI").show();
            $('#HitSAP').css('display', 'block');
            $('#HitCI').css('display', 'block');
        }
        else if (HitosTotalCapex >= 10000 && HitosTotalCapex < 15000) {
            $("#BotonAgregarSAP").show();
            $("#BotonAgregarCI").show();
            $("#BotonAgregarCA").show();
            $('#HitSAP').css('display', 'block');
            $('#HitCI').css('display', 'block');
            $('#HitCA').css('display', 'block');
        }
        /*else if (HitosTotalCapex >= 10000 && HitosTotalCapex < 15000) {
            $("#BotonAgregarSAP").show();
            $("#BotonAgregarCI").show();
            $("#BotonAgregarCA").show();
            $("#BotonAgregarOPR").show()
            $("#BotonAgregarDIRCEN").show();
            $('#HitSAP').css('display', 'block');
            $('#HitCI').css('display', 'block');
            $('#HitCA').css('display', 'block');
            $('#HitOPR').css('display', 'block');
        }*/
        else if (HitosTotalCapex >= 15000 && HitosTotalCapex < 50000) {
            $("#BotonAgregarSAP").show();
            $("#BotonAgregarCI").show();
            $("#BotonAgregarCA").show();
            $("#BotonAgregarOPR").show()
            $("#BotonAgregarPE").show()
            $("#BotonAgregarDIRCEN").show();
            $('#HitSAP').css('display', 'block');
            $('#HitCI').css('display', 'block');
            $('#HitCA').css('display', 'block');
            $('#HitOPR').css('display', 'block');
            $('#HitPE').css('display', 'block');
            $('#HitDIRCEN').css('display', 'block');
        }
        else if (HitosTotalCapex >= 50000) {
            $("#BotonAgregarSAP").show();
            $("#BotonAgregarCI").show();
            $("#BotonAgregarCA").show();
            $("#BotonAgregarOPR").show()
            $("#BotonAgregarPE").show()
            $("#BotonAgregarDIRCEN").show();
            $("#BotonAgregarDIRPLC").show();
            $('#HitSAP').css('display', 'block');
            $('#HitCI').css('display', 'block');
            $('#HitCA').css('display', 'block');
            $('#HitOPR').css('display', 'block');
            $('#HitPE').css('display', 'block');
            $('#HitDIRCEN').css('display', 'block');
            $('#HitDirPLC').css('display', 'block');
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

FNDescargarExcelTemplateFinal = function (token) {
    var link = document.createElement("a");
    console.info("token=", token);
    $.ajax({
        url: "/Documentacion/DescargarExcelTemplate/" + token,
        method: "GET",
        data: { "token": token },
        async: true
    }).done(function (r) {
        console.log("r=", r);
        if (r && r.IsSuccess && r.ResponseData) {
            console.log("r.ResponseData=", r.ResponseData);
            document.location.href = r.ResponseData;
        }
    }).fail(function (xhr) {
        console.log('fail error', xhr);
    });
    return;
}

FNDescargarExcelTemplateFinal2Pasos = function (token) {
    var link = document.createElement("a");
    var tipo = localStorage.getItem("CAPEX_TIPO_INICIATIVA");
    var tipoIniciativaSeleccionado = "";
    if (tipo == "CB" || tipo == "CD") {
        tipoIniciativaSeleccionado = "1";
    } else {
        tipoIniciativaSeleccionado = "2";
    }
    var iniciativa_token = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    console.info("token=", token);
    $.ajax({
        url: "/Documentacion/DescargarExcelTemplate2Pasos/" + token,
        method: "GET",
        data: { "token": token, "iniciativaToken": iniciativa_token, "tipo": tipoIniciativaSeleccionado },
        async: true
    }).done(function (r) {
        console.log("r=", r);
        if (r && r.IsSuccess && r.ResponseData) {
            console.log("r.ResponseData=", r.ResponseData);
            var urlFinal = '/Documentacion/DescargarExcelTemplateFinal2Pasos/' + r.ParToken + '/' + iniciativa_token + '/' + r.ResponseData + '/' + tipoIniciativaSeleccionado;
            document.location.href = encodeURI(urlFinal);
        }
    }).fail(function (xhr) {
        console.log('fail error', xhr);
    });
    return;
}

FNObtenerExcelTemplateFinal = function () {
    var tipo_iniciativa = localStorage.getItem("CAPEX_TIPO_INICIATIVA");
    var periodo = localStorage.getItem("CAPEX_PERIODO_EP");
    console.info("FNObtenerExcelTemplateFinal tipo_iniciativa=" + tipo_iniciativa + ", periodo=" + periodo);
    var tipoIniciativaSeleccionado = 1;
    if (tipo_iniciativa == "CB" || tipo_iniciativa == "CD") {
        tipoIniciativaSeleccionado = 1;
    } else {
        tipoIniciativaSeleccionado = 2;
    }
    $.ajax({
        url: "/Documentacion/SeleccionarExcelTemplatePeriodo/" + tipoIniciativaSeleccionado + "/" + periodo,
        method: "GET",
        async: true
    }).done(function (r) {
        console.log("FNObtenerExcelTemplateFinal r=", r);
        if (r && r.IsSuccess && r.ResponseData) {
            var contentSpan = "<img src='../../Content/icons/excel-48x48-1.png' height='24'/><a href='#' onclick=FNDescargarExcelTemplateFinal2Pasos(\'" + r.ResponseData.ParToken + "\'); style='color:white;'> Descargar Template</a>";
            if (tipoIniciativaSeleccionado == 1) {
                $("#ContenedorDescargaTemplateCB").html(contentSpan);
            } else {
                $("#ContenedorDescargaTemplate").html(contentSpan);
            }
        } else {
            var contentSpan = "<strong>No hay un template disponible.</strong>";
            if (tipoIniciativaSeleccionado == 1) {
                $('#ContenedorDescargaTemplateCB').css('color', 'red');
                $("#ContenedorDescargaTemplateCB").html(contentSpan);
            } else {
                $('#ContenedorDescargaTemplate').css('color', 'red');
                $("#ContenedorDescargaTemplate").html(contentSpan);
            }
        }
    }).fail(function (xhr) {
        console.log('fail error', xhr);
        var contentSpan = "<strong>No hay un template disponible.</strong>";
        if (tipoIniciativaSeleccionado == 1) {
            $('#ContenedorDescargaTemplateCB').css('color', 'red');
            $("#ContenedorDescargaTemplateCB").html(contentSpan);
        } else {
            $('#ContenedorDescargaTemplate').css('color', 'red');
            $("#ContenedorDescargaTemplate").html(contentSpan);
        }
    });
    return;
}