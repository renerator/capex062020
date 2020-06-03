
// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : PLANIFICACION
// FECHA            : MAYO 2018
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// SECCION          : EVALUACION ECONOMICA
// METODOS          :


var EPSILON = 0.000001;

FNFPLessThan = function (A, B, Epsilon) {
    Epsilon = Epsilon || EPSILON;
    return ((A - B) < Epsilon) && (Math.abs(A - B) > Epsilon);
};

FNFPGreaterThan = function (A, B, Epsilon) {
    Epsilon = Epsilon || EPSILON;
    return ((A - B) > Epsilon) && (Math.abs(A - B) > Epsilon);
}

FNFPEqualsThan = function (A, B, Epsilon) {
    Epsilon = Epsilon || EPSILON;
    return (Math.abs(A - B) < Epsilon);
}


FNFormatearReglaUnoEvalEco = function (nStr) {
    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + '.' + '$2');
    }
    return x1 + x2;
}
FNFormatearReglaDosEvalEco = function (nStr) {
    nStr = nStr + "0";
    var tir = nStr.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,')
    tir = tir.replace("0", "")
    return tir + "%";
}

FNFormatearReglaTresEvalEco = function (nStr) {
    nStr = nStr + "0";
    var ivan = nStr.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,')
    ivan = ivan.replace("0", "")
    return ivan;
}

/*document.getElementById("EveVan").onblur = function () {
    this.value = FNFormatearReglaUnoEvalEco(this.value.replace('.', ''));
}
document.getElementById("EveTir").onblur = function () {
    this.value = FNFormatearReglaDosEvalEco(this.value.replace('.', ''));
}
document.getElementById("EveIvan").onblur = function () {
    this.value = FNFormatearReglaTresEvalEco(this.value.replace(',', ''));
}
document.getElementById("EvePayBack").onblur = function () {
    this.value = FNFormatearReglaUnoEvalEco(this.value.replace('.', ''));
}
document.getElementById("EveTipoCambio").onblur = function () {
    this.value = FNFormatearReglaUnoEvalEco(this.value.replace('.', ''));
}*/

//
// GUARDAR EVALUACION ECONOMICA
//

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

FNGuardarEvaluacionEconomica = function () {

    //PARAMETROS BASE
    var estado = localStorage.getItem("CAPEX_INICIATIVA_EVALUACION_ECONOMICA_ESTADO");
    var IniToken = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    var IniUsuario = $("#CAPEX_H_USERNAME").val();

    var EveVan = $("#EveVan").val();
    var EveIvan = $("#EveIvan").val();
    var EvePayBack = $("#EvePayBack").val();
    var EveVidaUtil = $("#EveVidaUtil").val();
    var EveTipoCambio = $("#EveTipoCambio").val();
    var EveTir = $("#EveTir").val();

    if (EveTir && EveTir != null && EveTir != undefined && EveTir != "") {
        EveTir = EveTir.replace("%", "");
    }

    if (IniToken == "") {
        swal("", "Debe identificar una iniciativa para poder guardar este paso.", "info");
        return false;
    }
    if (estado == "Guardado") {
        swal("", "Esta evaluación ya se encuentra guardada.", "info");
        return false;
    }
    if (!$('#form_eval_eco').valid()) {
        console.log("validacion incorrecta");
        return false;
    }
    console.log("validacion correcta");
    if (EveVan == "" || EveIvan == "" || EvePayBack == "" || EveVidaUtil == "" || EveTipoCambio == "" || EveTir == "") {
        swal("", "Debe completar el formulario.", "info");
        return false;
    } else {
        var validarArchivo = false;
        var valueA = parseFloat(EveVan);
        var valueB = parseFloat("0");
        if (!FNFPEqualsThan(valueA, valueB)) {
            validarArchivo = true;
        }
        valueA = parseFloat(EveIvan);
        valueB = parseFloat("0");
        if (!validarArchivo && !FNFPEqualsThan(valueA, valueB)) {
            validarArchivo = true;
        }
        valueA = parseFloat(EvePayBack);
        valueB = parseFloat("0");
        if (!validarArchivo && !FNFPEqualsThan(valueA, valueB)) {
            validarArchivo = true;
        }
        valueA = parseFloat(EveVidaUtil);
        valueB = parseFloat("0");
        if (!validarArchivo && !FNFPEqualsThan(valueA, valueB)) {
            validarArchivo = true;
        }
        valueA = parseFloat(EveTipoCambio);
        valueB = parseFloat("0");
        if (!validarArchivo && !FNFPEqualsThan(valueA, valueB)) {
            validarArchivo = true;
        }
        valueA = parseFloat(EveTir);
        valueB = parseFloat("0");
        if (!validarArchivo && !FNFPEqualsThan(valueA, valueB)) {
            validarArchivo = true;
        }

        if (validarArchivo) {
            var DTOValidacion = {
                'IniToken': IniToken
            };
            var responseValidacion = FNRealizarValidacionArchivoEERespaldo(DTOValidacion);
            if (!responseValidacion || responseValidacion == undefined || responseValidacion == "") {
                swal("Error", "El archivo de respaldo no ha sido cargado.", "error");
                return false;
            }
        }

        var DTO = {
            "IniToken": IniToken,
            "IniUsuario": IniUsuario,
            "EveVan": EveVan,
            "EveIvan": EveIvan,
            "EvePayBack": EvePayBack,
            "EveVidaUtil": EveVidaUtil,
            "EveTipoCambio": EveTipoCambio
        }
        jQuery("#AppLoaderContainer").show();
        $.ajax({
            url: "../../Planificacion/GuardarEvaluacionEconomica",
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
                    jQuery("#AppLoaderContainer").hide();
                    swal("Error", "No es posible almacenar esta evaluación.", "error");
                    return false;
                }
                else if (estado == "Guardado") {

                    localStorage.setItem("CAPEX_INICIATIVA_EVALUACION_ECONOMICA_ESTADO", estado);
                    jQuery("#AppLoaderContainer").hide();

                    swal("Exito", "Evaluación Económica Guardada", "success");

                    $("#BotonGuardarEvaluacionEconomica").prop("disabled", true);
                    $("#BotonActualizarEvaluacionEconomica").show();
                    return false;
                }
            }
        });
    }
}
//
// ACTUALIZAR EVALUACION ECONOMICA
//
FNActualizarEvaluacionEconomica = function () {

    //PARAMETROS BASE
    var estado = localStorage.getItem("CAPEX_INICIATIVA_EVALUACION_ECONOMICA_ESTADO");
    var IniToken = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    var IniUsuario = $("#CAPEX_H_USERNAME").val();

    var EveVan = $("#EveVan").val();
    var EveIvan = $("#EveIvan").val();
    var EvePayBack = $("#EvePayBack").val();
    var EveVidaUtil = $("#EveVidaUtil").val();
    var EveTipoCambio = $("#EveTipoCambio").val();
    var EveTir = $("#EveTir").val();

    if (EveTir && EveTir != null && EveTir != undefined && EveTir != "") {
        EveTir = EveTir.replace("%", "");
    }

    if (IniToken == "") {
        swal("", "Debe identificar una iniciativa para poder guardar este paso.", "info");
        return false;
    }
    if (!$('#form_eval_eco').valid()) {
        console.log("validacion incorrecta");
        return false;
    }
    console.log("validacion correcta");
    if (EveVan == "" || EveIvan == "" || EvePayBack == "" || EveVidaUtil == "" || EveTipoCambio == "" || EveTir == "") {
        swal("", "Debe completar el formulario.", "info");
        return false;
    } else {

        var validarArchivo = false;
        var valueA = parseFloat(EveVan);
        var valueB = parseFloat("0");
        if (!FNFPEqualsThan(valueA, valueB)) {
            validarArchivo = true;
        }
        valueA = parseFloat(EveIvan);
        valueB = parseFloat("0");
        if (!validarArchivo && !FNFPEqualsThan(valueA, valueB)) {
            validarArchivo = true;
        }
        valueA = parseFloat(EvePayBack);
        valueB = parseFloat("0");
        if (!validarArchivo && !FNFPEqualsThan(valueA, valueB)) {
            validarArchivo = true;
        }
        valueA = parseFloat(EveVidaUtil);
        valueB = parseFloat("0");
        if (!validarArchivo && !FNFPEqualsThan(valueA, valueB)) {
            validarArchivo = true;
        }
        valueA = parseFloat(EveTipoCambio);
        valueB = parseFloat("0");
        if (!validarArchivo && !FNFPEqualsThan(valueA, valueB)) {
            validarArchivo = true;
        }
        valueA = parseFloat(EveTir);
        valueB = parseFloat("0");
        if (!validarArchivo && !FNFPEqualsThan(valueA, valueB)) {
            validarArchivo = true;
        }

        if (validarArchivo) {
            var DTOValidacion = {
                'IniToken': IniToken
            };
            var responseValidacion = FNRealizarValidacionArchivoEERespaldo(DTOValidacion);
            if (!responseValidacion || responseValidacion == undefined || responseValidacion == "") {
                swal("Error", "El archivo de respaldo no ha sido cargado.", "error");
                return false;
            }
        }

        var DTO = {
            "IniToken": IniToken,
            "IniUsuario": IniUsuario,
            "EveVan": EveVan,
            "EveIvan": EveIvan,
            "EvePayBack": EvePayBack,
            "EveVidaUtil": EveVidaUtil,
            "EveTipoCambio": EveTipoCambio,
            "EveTir": EveTir
        }
        jQuery("#AppLoaderContainer").show();
        $.ajax({
            url: "../../Planificacion/ActualizarEvaluacionEconomica",
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
                    jQuery("#AppLoaderContainer").hide();
                    swal("Error", "No es posible actualizar la evaluación económica.", "error");
                    return false;
                }
                else if (estado == "Actualizado") {
                    jQuery("#AppLoaderContainer").hide();
                    swal("Exito", "Evaluación Económica Actualizada.", "success");
                    localStorage.setItem("CAPEX_INICIATIVA_EVALUACION_ECONOMICA_ESTADO", estado);
                    return false;
                }

            }
        });
    }
}

FNRealizarValidacionArchivoEERespaldo = function (DTO) {
    var response = '';
    $.ajax({
        url: "../../Planificacion/SeleccionarAdjuntoEvaluacionEconomicaVigente",
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
            console.log("FNRealizarValidacionArchivoEERespaldo estructura=", estructura)
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
// GUARDAR EVALUACION ECONOMICA
//
/*document.getElementById('form_eval_eco').onsubmit = function (e) {
    e.preventDefault();
    var iniciativa_token = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    if (iniciativa_token == null || iniciativa_token == "") {
        swal("Error", "Debe identificar iniciativa antes de importar información.", "error");
        return false;
    }
    else {

        if (document.getElementById("ArchivoEvalEco").files.length == 0 || document.getElementById("ArchivoEvalEco").files.length == null) {
            return false;
        }
        else {
            jQuery("#AppLoaderContainer").show();
            var formdata = new FormData();
            var fileInput = document.getElementById('ArchivoEvalEco');
            var nombreArchivo = fileInput.files[0].name;
            $("#NombreArchivoEvalEco").html(nombreArchivo);
            var tamano = fileInput.files[0].size;

            var extension = nombreArchivo.substr((nombreArchivo.lastIndexOf('.') + 1));
            formdata.append(fileInput.files[0].name, fileInput.files[0]);
            var ajaxRequest = $.ajax({
                type: "POST",
                url: "../../Planificacion/SubirEvaluacionEconomica",
                contentType: false,
                processData: false,
                data: formdata
            });

            ajaxRequest.done(function (xhr, textStatus) {
                var ParUsuario = $("#CAPEX_H_USERNAME").val();
                var DTO = { "IniToken": iniciativa_token, "ParUsuario": ParUsuario, "ParNombre": nombreArchivo, "ParPaso": "Evaluacion-Economica", "ParCaso": "Evaluacion Económica" };
                $.ajax({
                    url: "../../Planificacion/RegistrarArchivo",
                    type: "GET",
                    dataType: "json",
                    data: (DTO),
                    success: function (r) {
                        jQuery("#AppLoaderContainer").hide();
                        swal("Exito", "Archivo evaluacion económica subido correctamente", "success");
                        FNPoblarDocumentos();
                        return false;
                    },
                    error: function (xhr, error, status) {
                        jQuery("#AppLoaderContainer").hide();
                        swal("Error", "Error al intentar subir archivo de Evaluacion Económica.", "error");
                        return false
                    }
                });
            });

        }
    }
}
*/