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
    console.log("FNCheckDownKey inputValue=", $('#EveTir').val());
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

FNCheckDownKey3 = function (selector) {
    console.log("FNCheckDownKey3 inputValue=", $(('#' + selector)).val());
    if ($(('#' + selector)).val() == "") {
        $(('#' + selector)).val("0");
    } else {
        var anioSeleccionado = $('#TemplateAnio').val();
        var anioDesde = $(('#' + selector)).val();
        anioDesde = anioDesde.replace(".", "");
        if (anioDesde.length == 4) {
            console.log("anioSeleccionado=" + anioSeleccionado + ", anioDesde=" + anioDesde);
            if (parseInt(anioDesde) < (parseInt(anioSeleccionado) + 1)) {
                $(('#' + selector)).val((parseInt(anioSeleccionado) + 1) + "");
            }
        }
    }
}

FNCheckDownKey3Actualizar = function (selector) {
    console.log("FNCheckDownKey3Actualizar inputValue=", $(('#' + selector)).val());
    if ($(('#' + selector)).val() == "") {
        $(('#' + selector)).val("0");
    } else {
        var anioSeleccionado = $('#ActualizarTemplateAnio').val();
        var anioDesde = $(('#' + selector)).val();
        anioDesde = anioDesde.replace(".", "");
        if (anioDesde.length == 4) {
            console.log("anioSeleccionado=" + anioSeleccionado + ", anioDesde=" + anioDesde);
            if (parseInt(anioDesde) < (parseInt(anioSeleccionado) + 1)) {
                $(('#' + selector)).val((parseInt(anioSeleccionado) + 1) + "");
            }
        }
    }
}

FNCheckDownKey4 = function (selector) {
    console.log("FNCheckDownKey3 inputValue=", $(('#' + selector)).val());
    if ($(('#' + selector)).val() == "") {
        $(('#' + selector)).val("0");
    } else {
        var ultimoAnio = FNLastAnio();
        var anioHasta = $(('#' + selector)).val();
        anioHasta = anioHasta.replace(".", "");
        if (anioHasta.length == 4) {
            console.log("ultimoAnio=" + ultimoAnio + ", anioHasta=" + anioHasta);
            if (parseInt(anioHasta) > parseInt(ultimoAnio)) {
                $(('#' + selector)).val(parseInt(ultimoAnio) + "");
            }
        }
    }
}

FNCheckDownKey4Actualizar = function (selector) {
    console.log("FNCheckDownKey4Actualizar inputValue=", $(('#' + selector)).val());
    if ($(('#' + selector)).val() == "") {
        $(('#' + selector)).val("0");
    } else {
        var ultimoAnio = FNActualizarLastAnio();
        var anioHasta = $(('#' + selector)).val();
        anioHasta = anioHasta.replace(".", "");
        if (anioHasta.length == 4) {
            console.log("ultimoAnio=" + ultimoAnio + ", anioHasta=" + anioHasta);
            if (parseInt(anioHasta) > parseInt(ultimoAnio)) {
                $(('#' + selector)).val(parseInt(ultimoAnio) + "");
            }
        }
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
    } else {
        paginaSeleccionada = $(".page-number.clickable.active").text();
        console.log("paginaSeleccionada=", paginaSeleccionada);
        switch (accion) {
            case "1": FNCrearTemplate(); break;
            case "2": FNEditarTemplate(cual); break;
            case "3": FNMostrarModalConfirmarDesactivar(cual); break;
            case "4": FNDescargarExcelTemplateFinal(cual); break;
        }
    }
}

FNDescargarExcelTemplateFinal = function (token) {
    var link = document.createElement("a");
    console.info("token=", token);
    $.ajax({
        url: "/Documentacion/DescargarExcelTemplate/" + token,
        method: "GET",
        data: { "token": token },
        async: false
    }).done(function (r) {
        $(".orderselect").val('-1').prop('selected', true);
        if (r && r.IsSuccess && r.ResponseData) {
            console.log("r.ResponseData=", r.ResponseData);
            document.location.href = r.ResponseData;
        }
    }).fail(function (xhr) {
        console.log('fail error', xhr);
    });
    return;
}

FNAplicarLongTime = function () {
    var anioDesde = $('#anioDesde').val();
    var anioHasta = $('#anioHasta').val();
    if (!anioDesde || anioDesde == undefined || anioDesde == "" || anioDesde == "0") {
        swal("", "El campo desde es requerido.", "error");
        return;
    }
    if (!anioHasta || anioHasta == undefined || anioHasta == "" || anioHasta == "0") {
        swal("", "El campo hasta es requerido.", "error");
        return;
    }
    if (parseInt(anioHasta.replace(".", "")) < parseInt(anioDesde.replace(".", ""))) {
        swal("", "El campo hasta no puede ser menor que el campo desde.", "error");
        return;
    }
    var anioMin = (parseInt($('#TemplateAnio').val()) + 1);
    var anioMax = FNLastAnio();
    if (parseInt(anioDesde.replace(".", "")) < anioMin) {
        swal("", "El campo desde no puede ser menor que " + anioMin + ".", "error");
        return;
    }
    if (parseInt(anioDesde.replace(".", "")) > parseInt(anioMax)) {
        swal("", "El campo desde no puede ser mayor que " + anioMax + ".", "error");
        return;
    }
    if (parseInt(anioHasta.replace(".", "")) < anioMin) {
        swal("", "El campo hasta no puede ser menor que " + anioMin + ".", "error");
        return;
    }
    if (parseInt(anioHasta.replace(".", "")) > parseInt(anioMax)) {
        swal("", "El campo hasta no puede ser mayor que " + anioMax + ".", "error");
        return;
    }

    var paso;
    for (paso = (parseInt(anioDesde.replace(".", "")) - parseInt(anioMin) + 1); paso <= (parseInt(anioHasta.replace(".", "")) - parseInt(anioMin) + 1); paso++) {
        console.log('FNAplicarLongTime paso=', paso);
        var tcSelector = '#tcMas' + paso;
        var ipcSelector = '#ipcMas' + paso;
        var cpiSelector = '#cpiMas' + paso;
        $(tcSelector).val($('#tcLT').val());
        $(ipcSelector).val($('#ipcLT').val());
        $(cpiSelector).val($('#cpiLT').val());
    }
}

FNActualizarAplicarLongTime = function () {
    var anioDesde = $('#ActualizarAnioDesde').val();
    var anioHasta = $('#ActualizarAnioHasta').val();
    if (!anioDesde || anioDesde == undefined || anioDesde == "" || anioDesde == "0") {
        swal("", "El campo desde es requerido.", "error");
        return;
    }
    if (!anioHasta || anioHasta == undefined || anioHasta == "" || anioHasta == "0") {
        swal("", "El campo hasta es requerido.", "error");
        return;
    }
    if (parseInt(anioHasta.replace(".", "")) < parseInt(anioDesde.replace(".", ""))) {
        swal("", "El campo hasta no puede ser menor que el campo desde.", "error");
        return;
    }
    var anioMin = (parseInt($('#ActualizarTemplateAnio').val()) + 1);
    var anioMax = FNActualizarLastAnio();
    if (parseInt(anioDesde.replace(".", "")) < anioMin) {
        swal("", "El campo desde no puede ser menor que " + anioMin + ".", "error");
        return;
    }
    if (parseInt(anioDesde.replace(".", "")) > parseInt(anioMax)) {
        swal("", "El campo desde no puede ser mayor que " + anioMax + ".", "error");
        return;
    }
    if (parseInt(anioHasta.replace(".", "")) < anioMin) {
        swal("", "El campo hasta no puede ser menor que " + anioMin + ".", "error");
        return;
    }
    if (parseInt(anioHasta.replace(".", "")) > parseInt(anioMax)) {
        swal("", "El campo hasta no puede ser mayor que " + anioMax + ".", "error");
        return;
    }

    var paso;
    for (paso = (parseInt(anioDesde.replace(".", "")) - parseInt(anioMin) + 1); paso <= (parseInt(anioHasta.replace(".", "")) - parseInt(anioMin) + 1); paso++) {
        console.log('FNAplicarLongTime paso=', paso);

        var tcSelector = '#tcActualizarMas' + paso;
        var ipcSelector = '#ipcActualizarMas' + paso;
        var cpiSelector = '#cpiActualizarMas' + paso;
        $(tcSelector).val($('#ActualizarTcLT').val());
        $(ipcSelector).val($('#ActualizarIpcLT').val());
        $(cpiSelector).val($('#ActualizarCpiLT').val());
    }
}


FNLastAnio = function () {
    var aniosSpan = $(".anioSpan");
    if (aniosSpan && aniosSpan != undefined && aniosSpan.length > 0) {
        var valueSpan = aniosSpan.eq((parseInt(aniosSpan.length) - 1)).attr("id");
        valueSpan = '#' + valueSpan;
        var ultimoAnioFinal = $(valueSpan).html();
        console.log("ultimoAnioFinal=", ultimoAnioFinal);
        return ultimoAnioFinal.replace("<strong>", "").replace("</strong>", "");
    }
    return "";
}

FNActualizarLastAnio = function () {
    var aniosSpan = $(".ActualizarAnioSpan");
    if (aniosSpan && aniosSpan != undefined && aniosSpan.length > 0) {
        var valueSpan = aniosSpan.eq((parseInt(aniosSpan.length) - 1)).attr("id");
        valueSpan = '#' + valueSpan;
        var ultimoAnioFinal = $(valueSpan).html();
        console.log("ultimoAnioFinal=", ultimoAnioFinal);
        return ultimoAnioFinal.replace("<strong>", "").replace("</strong>", "");
    }
    return "";
}

FNActualizarAnioChange = function (anioSeleccionado) {
    var pasoFinal = parseInt($('#pasoFinal').val());
    console.log("FNActualizarAnioChange anioSeleccionado=" + anioSeleccionado + ", pasoFinal=" + pasoFinal);
    if (anioSeleccionado == "-1") {
        $('#actualizarTemplate').prop('disabled', true);
        $('#actualizarAplicarLongTime').prop('disabled', true);
        $('#ActualizarAnioDesde').val("0");
        $('#ActualizarAnioHasta').val("0");
        $('#ActualizarAnioDesde').prop('disabled', true);
        $('#ActualizarAnioHasta').prop('disabled', true);
        var paso;
        for (paso = 1; paso <= pasoFinal; paso++) {
            var selectorAnio = '#ActualizarAnioMas' + paso;
            $(selectorAnio).html('&nbsp;&nbsp;...&nbsp;');
        };
    } else {
        var paso;
        for (paso = 1; paso <= pasoFinal; paso++) {
            var anio = '<strong>' + (parseInt(anioSeleccionado) + paso) + '</strong>';
            var selectorAnio = '#ActualizarAnioMas' + paso;
            $(selectorAnio).html(anio);
        };
        $('#actualizarTemplate').prop('disabled', false);
        $('#actualizarAplicarLongTime').prop('disabled', false);
        $('#ActualizarAnioDesde').prop('disabled', false);
        $('#ActualizarAnioHasta').prop('disabled', false);
    }
}

//
// CREAR
//
FNCrearTemplate = function () {
    var anioMin = $('#anioMin').val();
    $("#divContenedorTables").animate({ scrollTop: 0 }, "fast");
    $('#guadarTemplate').prop('disabled', true);
    $('#aplicarLongTime').prop('disabled', true);

    $('#TemplateAnio').children('option:not(:first)').remove();
    $('#TemplateAnio')
        .append($("<option></option>")
            .attr("value", anioMin)
            .text(anioMin));
    $('#TemplateAnio')
        .append($("<option></option>")
            .attr("value", (parseInt(anioMin) + 1))
            .text((parseInt(anioMin) + 1)));
    $('#TemplateAnio').val('-1');
    var paso;
    for (paso = 1; paso <= 12; paso++) {
        var selectorTCMes = '#tcMes' + paso;
        $(selectorTCMes).val('0');
        var selectorIPCMes = '#ipcMes' + paso;
        $(selectorIPCMes).val('0');
        var selectorCPIMes = '#cpiMes' + paso;
        $(selectorCPIMes).val('0');
    }
    var pasoFinal = parseInt($('#pasoFinal').val());
    for (paso = 1; paso <= pasoFinal; paso++) {
        var selectorTC = '#tcMas' + paso;
        $(selectorTC).val('0');
        var selectorIPC = '#ipcMas' + paso;
        $(selectorIPC).val('0');
        var selectorCPI = '#cpiMas' + paso;
        $(selectorCPI).val('0');
    }
    $('#anioDesde').val('0');
    $('#anioHasta').val('0');
    $('#anioDesde').prop('disabled', true);
    $('#anioHasta').prop('disabled', true);

    $('#tcLT').val('0');
    $('#ipcLT').val('0');
    $('#cpiLT').val('0');
    $('#ModalCrearTemplate').show();
    return false;
}
//
// GUARDAR
//
FNGuardarTemplate = function () {
    var anioSeleccionado = $('#TemplateAnio').val();
    var radioValue = $("input[name='optradio']:checked").val();

    var periodos = "";
    $("#TemplateAnio option").each(function (i) {
        if ("-1" != $(this).val()) {
            periodos += $(this).val() + ";";
        }
    });

    if (periodos != "" && periodos.length > 0) {
        periodos = periodos.substring(0, (periodos.length - 1));
    }

    var tcMes = "";
    var ipcMes = "";
    var cpiMes = "";
    var mes;
    for (mes = 1; mes <= 12; mes++) {
        tcMes += $(('#tcMes' + mes)).val() + ";";
        ipcMes += $(('#ipcMes' + mes)).val() + ";";
        cpiMes += $(('#cpiMes' + mes)).val() + ";";
    }
    if (tcMes != "" && tcMes.length > 0) {
        tcMes = tcMes.substring(0, (tcMes.length - 1));
    }
    if (ipcMes != "" && ipcMes.length > 0) {
        ipcMes = ipcMes.substring(0, (ipcMes.length - 1));
    }
    if (cpiMes != "" && cpiMes.length > 0) {
        cpiMes = cpiMes.substring(0, (cpiMes.length - 1));
    }
    console.log("tcMes=", tcMes);
    console.log("ipcMes=", ipcMes);
    console.log("cpiMes=", cpiMes);

    var anioMin = (parseInt($('#TemplateAnio').val()) + 1);
    var anioMax = parseInt(FNLastAnio());

    var pasoFinal = anioMax - anioMin + 1;
    var tcAnio = "";
    var ipcAnio = "";
    var cpiAnio = "";
    var anioPaso;
    for (anioPaso = 1; anioPaso <= pasoFinal; anioPaso++) {
        tcAnio += $(('#tcMas' + anioPaso)).val() + ";";
        ipcAnio += $(('#ipcMas' + anioPaso)).val() + ";";
        cpiAnio += $(('#cpiMas' + anioPaso)).val() + ";";
    }

    if (tcAnio != "" && tcAnio.length > 0) {
        tcAnio = tcAnio.substring(0, (tcAnio.length - 1));
    }
    if (ipcAnio != "" && ipcAnio.length > 0) {
        ipcAnio = ipcAnio.substring(0, (ipcAnio.length - 1));
    }
    if (cpiAnio != "" && cpiAnio.length > 0) {
        cpiAnio = cpiAnio.substring(0, (cpiAnio.length - 1));
    }
    console.log("tcAnio=", tcAnio);
    console.log("ipcAnio=", ipcAnio);
    console.log("cpiAnio=", cpiAnio);

    if (!anioSeleccionado || anioSeleccionado == undefined || !radioValue || radioValue == undefined || !tcMes || tcMes == undefined || tcMes == "" || !ipcMes || ipcMes == undefined || ipcMes == "" || !cpiMes || cpiMes == undefined || cpiMes == ""
        || !tcAnio || tcAnio == undefined || tcAnio == "" || !ipcAnio || ipcAnio == undefined || ipcAnio == "" || !cpiAnio || cpiAnio == undefined || cpiAnio == "") {
        swal("", "Debe completar el formulario.", "error");
        return false;
    } else {
        var DTO = {
            'TipoIniciativaSeleccionado': radioValue,
            'TPEPERIODO': anioSeleccionado,
            'TPEPERIODOS': periodos,
            'VALUEMESTC': tcMes,
            'VALUEMESIPC': ipcMes,
            'VALUEMESCPI': cpiMes,
            'VALUEANIOTC': tcAnio,
            'VALUEANIOIPC': ipcAnio,
            'VALUEANIOCPI': cpiAnio
        }
        $.ajaxSetup({ cache: false });
        $.ajax({
            url: "TemplateIniciativa/Guardar",
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                //PARSEAR RESPUESTA
                var estructura = JSON.parse(JSON.stringify(r));
                console.log("estructura=", estructura);
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
                        }, 2500);
                    }
                } else if (estado == "Guardado") {
                    $('#ModalCrearTemplate').hide();
                    $('#itPEToken').val('');
                    $(".orderselect").val('-1').prop('selected', true);
                    swal("", "Template creado.", "success");
                    setTimeout(function () {
                        if (mensaje != "") {
                            $('#itPEToken').val(mensaje);
                        }
                        $('#ModalActualizarExcelTemplate').show();
                    }, 500);
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
FNCerrarCrearTemplate = function () {
    $('#ModalCrearTemplate').hide()
    document.location.reload();
    return false;
}

FNErrorTamanioArchivo = function () {
    swal("", "EL archivo no debe superar los 10MB de tamaño,.", "info");
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
    var radioValue = $("input[name='optradio']:checked").val();
    if (paramJson.Data.code == "0") {
        swal("Exito", "Archivo template subido correctamente", "success");
        setTimeout(function () {
            window.location.href = location.protocol + '//' + location.host + location.pathname + "?tipoIniciativaSeleccionado=" + radioValue + "&paginaSeleccionada=" + paginaSeleccionada;
        }, 2500);
    } else {
        swal("Error", "Error al intentar subir template.", "error");
    }
}

//
// EDITAR /ACTUALIZAR
//
FNEditarTemplate = function (cual) {
    var radioValue = $("input[name='optradio']:checked").val();
    var DTO = {
        'TemplateToken': cual,
        'TipoIniciativaSeleccionado': radioValue
    }
    $('#ActualizarTemplateAnio').val('-1');
    $('#ActualizarTemplateAnio').children('option:not(:first)').remove();
    $('#itPEToken').val('');
    $('#tpPeriodoRespaldo').val('');
    $('#AnioActualizar').val('');
    $('#PETokenTC').val('');
    $('#PETokenIPC').val('');
    $('#PETokenCPI').val('');
    $('#tcIdParamEconomicoDetalleMes').val('');
    $('#ipcIdParamEconomicoDetalleMes').val('');
    $('#cpiIdParamEconomicoDetalleMes').val('');
    $('#tcIdParamEconomicoDetalleAnio').val('');
    $('#ipcIdParamEconomicoDetalleAnio').val('');
    $('#cpiIdParamEconomicoDetalleAnio').val('');

    $('#actualizarTemplate').prop('disabled', true);
    jQuery("#AppLoaderContainer").show();
    $.ajaxSetup({ cache: false });
    $.ajax({
        url: "TemplateIniciativa/BuscarPorToken",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            jQuery("#AppLoaderContainer").hide();
            var estructura = JSON.parse(JSON.stringify(r));
            console.log("JSON.stringify(r)", JSON.stringify(r));
            if (estructura && estructura.Mensaje && estructura.Mensaje == "Ok" && estructura.Resultados) {
                var TPEPERIODOS = estructura.Resultados.TPEPERIODOS.split(";");
                var TPEPERIODORESPALDO = estructura.Resultados.TPEPERIODORESPALDO;
                $('#tpPeriodoRespaldo').val(TPEPERIODORESPALDO);
                if (TPEPERIODOS && TPEPERIODOS.length == 2) {
                    $('#ActualizarTemplateAnio')
                        .append($("<option></option>")
                            .attr("value", TPEPERIODOS[0])
                            .text(TPEPERIODOS[0]));
                    $('#ActualizarTemplateAnio')
                        .append($("<option></option>")
                            .attr("value", TPEPERIODOS[1])
                            .text(TPEPERIODOS[1]));
                    $('#ActualizarTemplateAnio').val(estructura.Resultados.TPEPERIODO);
                    FNActualizarAnioChange(estructura.Resultados.TPEPERIODO);
                } else {
                    $('#ActualizarTemplateAnio').val('-1');
                    FNActualizarAnioChange("-1");
                }
                if (estructura.Resultados.ITPEToken && estructura.Resultados.ITPEToken != undefined) {
                    $('#itPEToken').val(estructura.Resultados.ITPEToken);
                }
                if (estructura.Resultados.TPEPERIODO && estructura.Resultados.TPEPERIODO != undefined) {
                    $('#AnioActualizar').val(estructura.Resultados.TPEPERIODO);
                }
                if (estructura.Resultados.PETokenTC && estructura.Resultados.PETokenTC != undefined) {
                    $('#PETokenTC').val(estructura.Resultados.PETokenTC);
                }
                if (estructura.Resultados.PETokenIPC && estructura.Resultados.PETokenIPC != undefined) {
                    $('#PETokenIPC').val(estructura.Resultados.PETokenIPC);
                }
                if (estructura.Resultados.PETokenCPI && estructura.Resultados.PETokenCPI != undefined) {
                    $('#PETokenCPI').val(estructura.Resultados.PETokenCPI);
                }
                if (estructura.Resultados.ParamTCMes && estructura.Resultados.ParamTCMes != undefined) {
                    var tcIdParamEconomicoDetalleMesSplit = estructura.Resultados.ParamTCMes.split(";");
                    if (tcIdParamEconomicoDetalleMesSplit.length > 1) {
                        var cont;
                        var tcIdParamEconomicoDetalleMes = "";
                        for (cont = 0; cont < tcIdParamEconomicoDetalleMesSplit.length; cont++) {
                            console.log("tcIdParamEconomicoDetalleMesSplit=", tcIdParamEconomicoDetalleMesSplit);
                            var splitFinal = tcIdParamEconomicoDetalleMesSplit[cont].split("|");
                            tcIdParamEconomicoDetalleMes += splitFinal[0] + ";";
                            var selectorMes = '#tcActualizarMes' + (cont + 1);
                            $(selectorMes).val(splitFinal[2]);
                        }
                        if (tcIdParamEconomicoDetalleMes != "" && tcIdParamEconomicoDetalleMes.length > 0) {
                            tcIdParamEconomicoDetalleMes = tcIdParamEconomicoDetalleMes.substring(0, (tcIdParamEconomicoDetalleMes.length - 1));
                        }
                        $('#tcIdParamEconomicoDetalleMes').val(tcIdParamEconomicoDetalleMes);
                    } else {
                        $('#tcIdParamEconomicoDetalleMes').val(tcIdParamEconomicoDetalleMesSplit);
                    }
                }
                if (estructura.Resultados.ParamIPCMes && estructura.Resultados.ParamIPCMes != undefined) {
                    var ipcIdParamEconomicoDetalleMesSplit = estructura.Resultados.ParamIPCMes.split(";");
                    if (ipcIdParamEconomicoDetalleMesSplit.length > 1) {
                        var cont;
                        var ipcIdParamEconomicoDetalleMes = "";
                        for (cont = 0; cont < ipcIdParamEconomicoDetalleMesSplit.length; cont++) {
                            console.log("ipcIdParamEconomicoDetalleMesSplit=", ipcIdParamEconomicoDetalleMesSplit);
                            var splitFinal = ipcIdParamEconomicoDetalleMesSplit[cont].split("|");
                            ipcIdParamEconomicoDetalleMes += splitFinal[0] + ";";
                            var selectorMes = '#ipcActualizarMes' + (cont + 1);
                            $(selectorMes).val(splitFinal[2]);
                        }
                        if (ipcIdParamEconomicoDetalleMes != "" && ipcIdParamEconomicoDetalleMes.length > 0) {
                            ipcIdParamEconomicoDetalleMes = ipcIdParamEconomicoDetalleMes.substring(0, (ipcIdParamEconomicoDetalleMes.length - 1));
                        }
                        $('#ipcIdParamEconomicoDetalleMes').val(ipcIdParamEconomicoDetalleMes);
                    } else {
                        $('#ipcIdParamEconomicoDetalleMes').val(ipcIdParamEconomicoDetalleMesSplit);
                    }
                }
                if (estructura.Resultados.ParamCPIMes && estructura.Resultados.ParamCPIMes != undefined) {
                    var cpiIdParamEconomicoDetalleMesSplit = estructura.Resultados.ParamCPIMes.split(";");
                    if (cpiIdParamEconomicoDetalleMesSplit.length > 1) {
                        var cont;
                        var cpiIdParamEconomicoDetalleMes = "";
                        for (cont = 0; cont < cpiIdParamEconomicoDetalleMesSplit.length; cont++) {
                            console.log("cpiIdParamEconomicoDetalleMesSplit=", cpiIdParamEconomicoDetalleMesSplit);
                            var splitFinal = cpiIdParamEconomicoDetalleMesSplit[cont].split("|");
                            cpiIdParamEconomicoDetalleMes += splitFinal[0] + ";";
                            var selectorMes = '#cpiActualizarMes' + (cont + 1);
                            $(selectorMes).val(splitFinal[2]);
                        }
                        if (cpiIdParamEconomicoDetalleMes != "" && cpiIdParamEconomicoDetalleMes.length > 0) {
                            cpiIdParamEconomicoDetalleMes = cpiIdParamEconomicoDetalleMes.substring(0, (cpiIdParamEconomicoDetalleMes.length - 1));
                        }
                        $('#cpiIdParamEconomicoDetalleMes').val(cpiIdParamEconomicoDetalleMes);
                    } else {
                        $('#cpiIdParamEconomicoDetalleMes').val(cpiIdParamEconomicoDetalleMesSplit);
                    }
                }

                if (estructura.Resultados.ParamTCAnio && estructura.Resultados.ParamTCAnio != undefined) {
                    var tcIdParamEconomicoDetalleAnioSplit = estructura.Resultados.ParamTCAnio.split(";");
                    if (tcIdParamEconomicoDetalleAnioSplit.length > 1) {
                        var cont;
                        var tcIdParamEconomicoDetalleAnio = "";
                        for (cont = 0; cont < tcIdParamEconomicoDetalleAnioSplit.length; cont++) {
                            console.log("tcIdParamEconomicoDetalleAnioSplit=", tcIdParamEconomicoDetalleAnioSplit);
                            var splitFinal = tcIdParamEconomicoDetalleAnioSplit[cont].split("|");
                            tcIdParamEconomicoDetalleAnio += splitFinal[0] + ";";
                            var selectorMes = '#tcActualizarMas' + (cont + 1);
                            $(selectorMes).val(splitFinal[2]);
                        }
                        if (tcIdParamEconomicoDetalleAnio != "" && tcIdParamEconomicoDetalleAnio.length > 0) {
                            tcIdParamEconomicoDetalleAnio = tcIdParamEconomicoDetalleAnio.substring(0, (tcIdParamEconomicoDetalleAnio.length - 1));
                        }
                        $('#tcIdParamEconomicoDetalleAnio').val(tcIdParamEconomicoDetalleAnio);
                    } else {
                        $('#tcIdParamEconomicoDetalleAnio').val(tcIdParamEconomicoDetalleAnioSplit);
                    }
                }

                if (estructura.Resultados.ParamIPCAnio && estructura.Resultados.ParamIPCAnio != undefined) {
                    var ipcIdParamEconomicoDetalleAnioSplit = estructura.Resultados.ParamIPCAnio.split(";");
                    if (ipcIdParamEconomicoDetalleAnioSplit.length > 1) {
                        var cont;
                        var ipcIdParamEconomicoDetalleAnio = "";
                        for (cont = 0; cont < ipcIdParamEconomicoDetalleAnioSplit.length; cont++) {
                            console.log("ipcIdParamEconomicoDetalleAnioSplit=", ipcIdParamEconomicoDetalleAnioSplit);
                            var splitFinal = ipcIdParamEconomicoDetalleAnioSplit[cont].split("|");
                            ipcIdParamEconomicoDetalleAnio += splitFinal[0] + ";";
                            var selectorMes = '#ipcActualizarMas' + (cont + 1);
                            $(selectorMes).val(splitFinal[2]);
                        }
                        if (ipcIdParamEconomicoDetalleAnio != "" && ipcIdParamEconomicoDetalleAnio.length > 0) {
                            ipcIdParamEconomicoDetalleAnio = ipcIdParamEconomicoDetalleAnio.substring(0, (ipcIdParamEconomicoDetalleAnio.length - 1));
                        }
                        $('#ipcIdParamEconomicoDetalleAnio').val(ipcIdParamEconomicoDetalleAnio);
                    } else {
                        $('#ipcIdParamEconomicoDetalleAnio').val(ipcIdParamEconomicoDetalleAnioSplit);
                    }
                }

                if (estructura.Resultados.ParamCPIAnio && estructura.Resultados.ParamCPIAnio != undefined) {
                    var cpiIdParamEconomicoDetalleAnioSplit = estructura.Resultados.ParamCPIAnio.split(";");
                    if (cpiIdParamEconomicoDetalleAnioSplit.length > 1) {
                        var cont;
                        var cpiIdParamEconomicoDetalleAnio = "";
                        for (cont = 0; cont < cpiIdParamEconomicoDetalleAnioSplit.length; cont++) {
                            console.log("cpiIdParamEconomicoDetalleAnioSplit=", cpiIdParamEconomicoDetalleAnioSplit);
                            var splitFinal = cpiIdParamEconomicoDetalleAnioSplit[cont].split("|");
                            cpiIdParamEconomicoDetalleAnio += splitFinal[0] + ";";
                            var selectorMes = '#cpiActualizarMas' + (cont + 1);
                            $(selectorMes).val(splitFinal[2]);
                        }
                        if (cpiIdParamEconomicoDetalleAnio != "" && cpiIdParamEconomicoDetalleAnio.length > 0) {
                            cpiIdParamEconomicoDetalleAnio = cpiIdParamEconomicoDetalleAnio.substring(0, (cpiIdParamEconomicoDetalleAnio.length - 1));
                        }
                        $('#cpiIdParamEconomicoDetalleAnio').val(cpiIdParamEconomicoDetalleAnio);
                    } else {
                        $('#cpiIdParamEconomicoDetalleAnio').val(cpiIdParamEconomicoDetalleAnioSplit);
                    }
                }
                setTimeout(function () { $('#actualizarTemplate').prop('disabled', false); }, 500);
            }
        },
        error: function (error) {
            jQuery("#AppLoaderContainer").hide();
        }
    });
    $('#ModalActualizarTemplate').show();
    return false;
}
//
// ACTUALIZAR CATEGORIA
//
FNActualizarTemplate = function () {
    var anioSeleccionado = $('#ActualizarTemplateAnio').val();
    var radioValue = $("input[name='optradio']:checked").val();

    var PETokenTC = $('#PETokenTC').val();
    var PETokenIPC = $('#PETokenIPC').val();
    var PETokenCPI = $('#PETokenCPI').val();

    var tcIdParamEconomicoDetalleMes = $('#tcIdParamEconomicoDetalleMes').val();
    var ipcIdParamEconomicoDetalleMes = $('#ipcIdParamEconomicoDetalleMes').val();
    var cpiIdParamEconomicoDetalleMes = $('#cpiIdParamEconomicoDetalleMes').val();

    var tcIdParamEconomicoDetalleMesFinal = "";
    if (tcIdParamEconomicoDetalleMes && tcIdParamEconomicoDetalleMes.split(";").length == 12) {
        var tcIdParamEconomicoDetalleMesSplit = tcIdParamEconomicoDetalleMes.split(";");
        var paso;
        for (paso = 0; paso < tcIdParamEconomicoDetalleMesSplit.length; paso++) {
            var keyvalue = tcIdParamEconomicoDetalleMesSplit[paso] + "|" + $(('#tcActualizarMes' + (paso + 1))).val() + ";";
            tcIdParamEconomicoDetalleMesFinal += keyvalue;
        }
        if (tcIdParamEconomicoDetalleMesFinal != "" && tcIdParamEconomicoDetalleMesFinal.length > 0) {
            tcIdParamEconomicoDetalleMesFinal = tcIdParamEconomicoDetalleMesFinal.substring(0, (tcIdParamEconomicoDetalleMesFinal.length - 1));
        }
    } else {
        swal("Error", "No es posible actualizar el template seleccionado.", "error");
        return;
    }

    var ipcIdParamEconomicoDetalleMesFinal = "";
    if (ipcIdParamEconomicoDetalleMes && ipcIdParamEconomicoDetalleMes.split(";").length == 12) {
        var ipcIdParamEconomicoDetalleMesSplit = ipcIdParamEconomicoDetalleMes.split(";");
        var paso;
        for (paso = 0; paso < ipcIdParamEconomicoDetalleMesSplit.length; paso++) {
            var keyvalue = ipcIdParamEconomicoDetalleMesSplit[paso] + "|" + $(('#ipcActualizarMes' + (paso + 1))).val() + ";";
            ipcIdParamEconomicoDetalleMesFinal += keyvalue;
        }
        if (ipcIdParamEconomicoDetalleMesFinal != "" && ipcIdParamEconomicoDetalleMesFinal.length > 0) {
            ipcIdParamEconomicoDetalleMesFinal = ipcIdParamEconomicoDetalleMesFinal.substring(0, (ipcIdParamEconomicoDetalleMesFinal.length - 1));
        }
    } else {
        swal("Error", "No es posible actualizar el template seleccionado.", "error");
        return;
    }

    var cpiIdParamEconomicoDetalleMesFinal = "";
    if (cpiIdParamEconomicoDetalleMes && cpiIdParamEconomicoDetalleMes.split(";").length == 12) {
        var cpiIdParamEconomicoDetalleMesSplit = cpiIdParamEconomicoDetalleMes.split(";");
        var paso;
        for (paso = 0; paso < cpiIdParamEconomicoDetalleMesSplit.length; paso++) {
            var keyvalue = cpiIdParamEconomicoDetalleMesSplit[paso] + "|" + $(('#cpiActualizarMes' + (paso + 1))).val() + ";";
            cpiIdParamEconomicoDetalleMesFinal += keyvalue;
        }
        if (cpiIdParamEconomicoDetalleMesFinal != "" && cpiIdParamEconomicoDetalleMesFinal.length > 0) {
            cpiIdParamEconomicoDetalleMesFinal = cpiIdParamEconomicoDetalleMesFinal.substring(0, (cpiIdParamEconomicoDetalleMesFinal.length - 1));
        }
    } else {
        swal("Error", "No es posible actualizar el template seleccionado.", "error");
        return;
    }

    var pasoFinal = parseInt($('#pasoFinal').val());
    var tcIdParamEconomicoDetalleAnio = $('#tcIdParamEconomicoDetalleAnio').val();
    var ipcIdParamEconomicoDetalleAnio = $('#ipcIdParamEconomicoDetalleAnio').val();
    var cpiIdParamEconomicoDetalleAnio = $('#cpiIdParamEconomicoDetalleAnio').val();

    var tcIdParamEconomicoDetalleAnioFinal = "";
    if (tcIdParamEconomicoDetalleAnio && tcIdParamEconomicoDetalleAnio.split(";").length == pasoFinal) {
        var tcIdParamEconomicoDetalleAnioSplit = tcIdParamEconomicoDetalleAnio.split(";");
        var paso;
        for (paso = 0; paso < tcIdParamEconomicoDetalleAnioSplit.length; paso++) {
            var keyvalue = tcIdParamEconomicoDetalleAnioSplit[paso] + "|" + $(('#tcActualizarMas' + (paso + 1))).val() + ";";
            tcIdParamEconomicoDetalleAnioFinal += keyvalue;
        }
        if (tcIdParamEconomicoDetalleAnioFinal != "" && tcIdParamEconomicoDetalleAnioFinal.length > 0) {
            tcIdParamEconomicoDetalleAnioFinal = tcIdParamEconomicoDetalleAnioFinal.substring(0, (tcIdParamEconomicoDetalleAnioFinal.length - 1));
        }
    } else {
        swal("Error", "No es posible actualizar el template seleccionado.", "error");
        return;
    }

    var ipcIdParamEconomicoDetalleAnioFinal = "";
    if (ipcIdParamEconomicoDetalleAnio && ipcIdParamEconomicoDetalleAnio.split(";").length == pasoFinal) {
        var ipcIdParamEconomicoDetalleAnioSplit = ipcIdParamEconomicoDetalleAnio.split(";");
        var paso;
        for (paso = 0; paso < ipcIdParamEconomicoDetalleAnioSplit.length; paso++) {
            var keyvalue = ipcIdParamEconomicoDetalleAnioSplit[paso] + "|" + $(('#ipcActualizarMas' + (paso + 1))).val() + ";";
            ipcIdParamEconomicoDetalleAnioFinal += keyvalue;
        }
        if (ipcIdParamEconomicoDetalleAnioFinal != "" && ipcIdParamEconomicoDetalleAnioFinal.length > 0) {
            ipcIdParamEconomicoDetalleAnioFinal = ipcIdParamEconomicoDetalleAnioFinal.substring(0, (ipcIdParamEconomicoDetalleAnioFinal.length - 1));
        }
    } else {
        swal("Error", "No es posible actualizar el template seleccionado.", "error");
        return;
    }

    var cpiIdParamEconomicoDetalleAnioFinal = "";
    if (cpiIdParamEconomicoDetalleAnio && cpiIdParamEconomicoDetalleAnio.split(";").length == pasoFinal) {
        var cpiIdParamEconomicoDetalleAnioSplit = cpiIdParamEconomicoDetalleAnio.split(";");
        var paso;
        for (paso = 0; paso < cpiIdParamEconomicoDetalleAnioSplit.length; paso++) {
            var keyvalue = cpiIdParamEconomicoDetalleAnioSplit[paso] + "|" + $(('#cpiActualizarMas' + (paso + 1))).val() + ";";
            cpiIdParamEconomicoDetalleAnioFinal += keyvalue;
        }
        if (cpiIdParamEconomicoDetalleAnioFinal != "" && cpiIdParamEconomicoDetalleAnioFinal.length > 0) {
            cpiIdParamEconomicoDetalleAnioFinal = cpiIdParamEconomicoDetalleAnioFinal.substring(0, (cpiIdParamEconomicoDetalleAnioFinal.length - 1));
        }
    } else {
        swal("Error", "No es posible actualizar el template seleccionado.", "error");
        return;
    }

    if (!anioSeleccionado || anioSeleccionado == undefined || anioSeleccionado == "-1" || !PETokenTC || PETokenTC == undefined || PETokenTC == ""
        || !PETokenIPC || PETokenIPC == undefined || PETokenIPC == "" || !PETokenCPI || PETokenCPI == undefined || PETokenCPI == "") {
        swal("", "Debe completar el formulario.", "error");
        return false;
    } else {
        var DTO = {
            'TipoIniciativaSeleccionado': radioValue,
            'ITPEToken': $('#itPEToken').val(),
            'TPEPERIODO': anioSeleccionado,
            'TPEPERIODORESPALDO': $('#tpPeriodoRespaldo').val(),
            'PETokenTC': $('#PETokenTC').val(),
            'PETokenIPC': $('#PETokenIPC').val(),
            'PETokenCPI': $('#PETokenCPI').val(),
            'IdParamEconomicoDetalleTCMES': tcIdParamEconomicoDetalleMesFinal,
            'IdParamEconomicoDetalleIPCMES': ipcIdParamEconomicoDetalleMesFinal,
            'IdParamEconomicoDetalleCPIMES': cpiIdParamEconomicoDetalleMesFinal,
            'IdParamEconomicoDetalleTCANIO': tcIdParamEconomicoDetalleAnioFinal,
            'IdParamEconomicoDetalleIPCANIO': ipcIdParamEconomicoDetalleAnioFinal,
            'IdParamEconomicoDetalleCPIANIO': cpiIdParamEconomicoDetalleAnioFinal
        }
        $.ajax({
            url: "TemplateIniciativa/Actualizar",
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                //PARSEAR RESPUESTA
                var estructura = JSON.parse(JSON.stringify(r));
                estructura = estructura.Mensaje;
                //PROCESAR RESPUESTA
                if (estructura == "Actualizado") {
                    $('#ModalActualizarTemplate').hide();
                    swal("", "Template actualizado.", "success");
                    $(".orderselect").val('-1').prop('selected', true);
                    setTimeout(function () {
                        $('#ModalActualizarExcelTemplate').show();
                    }, 500);
                } else if (estructura.startsWith("Error") && estructura.length > 5) {
                    swal("Error", "No es posible actualizar el template seleccionado.", "error");
                    $(".orderselect").val('-1').prop('selected', true);
                    setTimeout(function () {
                        document.location.reload();
                    }, 2500);
                } else if (estructura == "Error") {
                    swal("Error", "No es posible actualizar el template seleccionado.", "error");
                    $(".orderselect").val('-1').prop('selected', true);
                    setTimeout(function () {
                        document.location.reload();
                    }, 2500);
                }
            }
        });
        return false;
    }
}
//
// CERRAR ACTUALIZAR CATEGORIA
//
FNCerrarActualizarTemplate = function () {
    $('#ModalActualizarTemplate').hide();
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
    $("#TemplateToken_Desactivar").val(cual);
    return true;
}

//
// DESACTIVAR
//
FNDesactivarTemplate = function (cual) {
    var DTO = {
        'TemplateToken': $("#TemplateToken_Desactivar").val()
    }
    var radioValue = $("input[name='optradio']:checked").val();
    console.log("FNDesactivarTemplate radioValue=", radioValue);
    $.ajax({
        url: "TemplateIniciativa/Desactivar",
        type: "POST",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            //PARSEAR RESPUESTA
            var estructura = JSON.parse(JSON.stringify(r));
            estructura = estructura.Mensaje;

            //PROCESAR RESPUESTA
            if (estructura == "Error") {
                swal("Error", "No es posible eliminar el template seleccionado.", "error");
                $(".orderselect").val('-1').prop('selected', true);
                setTimeout(function () {
                    document.location.reload();
                }, 2500);
            } else if (estructura == "Descativado") {
                swal("", "Template eliminado.", "success");
                $(".orderselect").val('-1').prop('selected', true);
                setTimeout(function () {
                    window.location.href = location.protocol + '//' + location.host + location.pathname + "?tipoIniciativaSeleccionado=" + radioValue + "&paginaSeleccionada=" + paginaSeleccionada;
                }, 2500);
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
