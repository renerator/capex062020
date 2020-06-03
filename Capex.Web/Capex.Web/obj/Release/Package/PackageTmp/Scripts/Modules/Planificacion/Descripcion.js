// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : PLANIFICACION
// FECHA            : MAYO 2018
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// SECCION          : DESCRIPCION DETALLADA
// METODOS          :
//
// AUX CONTROL DE CARACTERES PARA ENTRADA DE DESCRIPCIONES
//
FNCuentaLosCaracteres = function (control, val) {
    var len = val.value.length;
    if (len >= 400) {
        val.value = val.value.substring(0, 400);
    } else {
        switch (control) {
            case "OBJETIVO_INVERSION":
                $('#ContenedorContador1').text(400 - len);
                break;
            case "ALCANCE_INVERSION":
                $('#ContenedorContador2').text(400 - len);
                break;
            case "JUSTIFICACON_INVERSION":
                $('#ContenedorContador3').text(400 - len);
                break;
        }
    }
};
//
// GUARDAR DESCRIPCION DETALLADA
//
FNGuardarDescripcion = function () {

    //PARAMETROS BASE
    var estado = localStorage.getItem("CAPEX_INICIATIVA_DESCRIPCION_ESTADO");
    var IniToken = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    var IniUsuario = $("#CAPEX_H_USERNAME").val();

    var PddObjetivo = $("#PddObjetivo").val();
    var PddAlcance = $("#PddAlcance").val();
    var PddJustificacion = $("#PddJustificacion").val();
    var PddDescripcion1 = $("#PddDescripcion1").val();
    var PddUnidad1 = $("#PddUnidad1").val();
    var PddActual1 = $("#PddActual1").val();
    var PddTarget1 = $("#PddTarget1").val();
    var PddDescripcion2 = $("#PddDescripcion2").val();
    var PddUnidad2 = $("#PddUnidad2").val();
    var PddActual2 = $("#PddActual2").val();
    var PddTarget2 = $("#PddTarget2").val();
    var PddDescripcion3 = $("#PddDescripcion3").val();
    var PddUnidad3 = $("#PddUnidad3").val();
    var PddActual3 = $("#PddActual3").val();
    var PddTarget3 = $("#PddTarget3").val();
    var CualFecha = localStorage.getItem("CAPEX_FECHA_POSTEVAL_EN_USO");

    /*if (CualFecha == "DP" && CualFecha != "") {
        var PddFechaPostEval = $("#PddFechaPostEvalDp").val();
    } else if (CualFecha == "PD" || CualFecha == "") {
        var PddFechaPostEval = $("#PddFechaPostEval").val();
    }*/
    var PddFechaPostEval = $("#datepickerFPE").val();
    if (IniToken == "") {
        swal("", "Debe identificar una iniciativa para poder guardar este paso.", "info");
        return false;
    }
    if (!$('#form_descripcion').valid()) {
        console.log("validacion incorrecta");
        return false;
    }
    console.log("validacion correcta");
    if (!PddObjetivo || PddObjetivo == undefined || PddObjetivo == "") {
        swal("", "Debe ingresar Objetivo de la Inversión", "info");
        return false;
    }
    if (!PddAlcance || PddAlcance == undefined || PddAlcance == "") {
        swal("", "Debe ingresar Alcance/Descripción de la Inversión", "info");
        return false;
    }
    if (!PddJustificacion || PddJustificacion == undefined || PddJustificacion == "") {
        swal("", "Debe ingresar Justificación de la Inversión", "info");
        return false;
    }
    if (estado == "Guardado") {
        swal("", "Esta descripción ya se encuentra guardada.", "info");
        return false;
    } else {
        var DTO = {
            "IniToken": IniToken,
            "IniUsuario": IniUsuario,
            "PddObjetivo": PddObjetivo,
            "PddAlcance": PddAlcance,
            "PddJustificacion": PddJustificacion,
            "PddDescripcion1": PddDescripcion1,
            "PddUnidad1": PddUnidad1,
            "PddActual1": PddActual1,
            "PddTarget1": PddTarget1,
            "PddDescripcion2": PddDescripcion2,
            "PddUnidad2": PddUnidad2,
            "PddActual2": PddActual2,
            "PddTarget2": PddTarget2,
            "PddDescripcion3": PddDescripcion3,
            "PddUnidad3": PddUnidad3,
            "PddActual3": PddActual3,
            "PddTarget3": PddTarget3,
            "PddFechaPostEval": PddFechaPostEval.replace("-", "/")
        }

        $.ajax({
            url: "../Planificacion/GuardarDescripcionDetallada",
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
                    swal("Error", "No es posible almacenar descripción detallada.", "error");
                }
                else if (estado == "Guardado") {

                    localStorage.setItem("CAPEX_INICIATIVA_DESCRIPCION_ESTADO", estado);
                    swal("Exito", "Descripción Detallada Guardada", "success");

                    $("#BotonGuardarDescripcion").prop("disabled", "true");
                    $("#BotonActualizarDescripcion").show();


                    return false;
                }

            }
        });
    }
}
//
// ACTUALIZAR DESCRIPCION DETALLADA
//
FNActualizarDescripcion = function () {

    //PARAMETROS BASE
    var estado = localStorage.getItem("CAPEX_INICIATIVA_DESCRIPCION_ESTADO");
    var IniToken = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    var IniUsuario = $("#CAPEX_H_USERNAME").val();

    var PddObjetivo = $("#PddObjetivo").val();
    var PddAlcance = $("#PddAlcance").val();
    var PddJustificacion = $("#PddJustificacion").val();
    var PddDescripcion1 = $("#PddDescripcion1").val();
    var PddUnidad1 = $("#PddUnidad1").val();
    var PddActual1 = $("#PddActual1").val();
    var PddTarget1 = $("#PddTarget1").val();
    var PddDescripcion2 = $("#PddDescripcion2").val();
    var PddUnidad2 = $("#PddUnidad2").val();
    var PddActual2 = $("#PddActual2").val();
    var PddTarget2 = $("#PddTarget2").val();
    var PddDescripcion3 = $("#PddDescripcion3").val();
    var PddUnidad3 = $("#PddUnidad3").val();
    var PddActual3 = $("#PddActual3").val();
    var PddTarget3 = $("#PddTarget3").val();
    var CualFecha = localStorage.getItem("CAPEX_FECHA_POSTEVAL_EN_USO");

    /*if (CualFecha == "DP" && CualFecha != "") {
        var PddFechaPostEval = $("#PddFechaPostEvalDp").val();
    } else if (CualFecha == "PD" || CualFecha == "") {
        var PddFechaPostEval = $("#PddFechaPostEval").val();
    }*/
    var PddFechaPostEval = $("#datepickerFPE").val();
    if (IniToken == "") {
        swal("", "Debe identificar una iniciativa para poder actualizar este paso.", "info");
        return false;
    }
    if (!$('#form_descripcion').valid()) {
        console.log("validacion incorrecta");
        return false;
    }
    console.log("validacion correcta");
    if (!PddObjetivo || PddObjetivo == undefined || PddObjetivo == "") {
        swal("", "Debe ingresar Objetivo de la Inversión", "info");
        return false;
    }
    if (!PddAlcance || PddAlcance == undefined || PddAlcance == "") {
        swal("", "Debe ingresar Alcance/Descripción de la Inversión", "info");
        return false;
    }
    if (!PddJustificacion || PddJustificacion == undefined || PddJustificacion == "") {
        swal("", "Debe ingresar Justificación de la Inversión", "info");
        return false;
    }
    else {
        var DTO = {
            "IniToken": IniToken,
            "IniUsuario": IniUsuario,
            "PddObjetivo": PddObjetivo,
            "PddAlcance": PddAlcance,
            "PddJustificacion": PddJustificacion,
            "PddDescripcion1": PddDescripcion1,
            "PddUnidad1": PddUnidad1,
            "PddActual1": PddActual1,
            "PddTarget1": PddTarget1,
            "PddDescripcion2": PddDescripcion2,
            "PddUnidad2": PddUnidad2,
            "PddActual2": PddActual2,
            "PddTarget2": PddTarget2,
            "PddDescripcion3": PddDescripcion3,
            "PddUnidad3": PddUnidad3,
            "PddActual3": PddActual3,
            "PddTarget3": PddTarget3,
            "PddFechaPostEval": PddFechaPostEval.replace("-", "/")
        }

        $.ajax({
            url: "../Planificacion/ActualizarDescripcionDetallada",
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
                    swal("Error", "No es posible actualizar esta descripción detallada.", "error");
                    return false;
                }
                else if (estado == "Actualizado") {
                    swal("Exito", "Descripción Detallada Actualizada.", "success");
                    localStorage.setItem("CAPEX_INICIATIVA_DESCRIPCION_ESTADO", estado);
                    return false;
                }

            }
        });
    }
}
//
// CONTROL DE DATE PICKER
//
FNUsarDatePickerPostEval = function () {
    $("#BotonCambiarFechaPostEval").hide();
    $("#PddFechaPostEval").hide();
    $("#PddFechaPostEvalDp").show();
    $("#BotonCambiarDatePickerPostEval").show();
    localStorage.setItem("CAPEX_FECHA_POSTEVAL_EN_USO", "DP");
}

FNUsarFechaPropuestaPostEval = function () {
    $("#BotonCambiarFechaPostEval").show();
    $("#PddFechaPostEval").show();
    $("#PddFechaPostEvalDp").hide();
    $("#BotonCambiarDatePickerPostEval").hide();
    localStorage.setItem("CAPEX_FECHA_POSTEVAL_EN_USO", "PD");
}

FNPreviousCallBackIngresoDescripcionCheckToken = function () {
    var iniciativa_token = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    if (iniciativa_token == null || iniciativa_token == "") {
        swal("", "Debe identificar iniciativa antes de importar información.", "info");
        return false;
    }
    return true;
}

FNPreviousCallBackIngresoDescripcion = function () {
    console.log("FNPreviousCallBackIngresoDescripcion");
    jQuery("#AppLoaderContainer").show();
}

FNCallBackIngresoDescripcion = function (paramJson) {
    jQuery("#AppLoaderContainer").hide();
    if (paramJson.Data.code == "0") {
        swal("Exito", "Archivo descripcion detallada subido correctamente", "success");
        FNPoblarDocumentos();
    } else {
        swal("Error", "Error al intentar subir archivo de Descripcion Detallada.", "error");
    }
}
//
// GUARDAR ARCHIVO 
//
/*
document.getElementById('form_descripcion').onsubmit = function (e) {
    e.preventDefault();
    var iniciativa_token = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    if (iniciativa_token == null || iniciativa_token == "") {
        swal("Error", "Debe identificar iniciativa antes de importar información.", "error");
        return false;
    }
    else {

        if (document.getElementById("ArchivoDesc").files.length == 0 || document.getElementById("ArchivoDesc").files.length == null) {
            return false;
        }
        else {
            var formdata = new FormData();
            var fileInput = document.getElementById('ArchivoDesc');
            var nombreArchivo = fileInput.files[0].name;
            $("#NombreArchivoDesc").html(nombreArchivo);
            var tamano = fileInput.files[0].size;

            var extension = nombreArchivo.substr((nombreArchivo.lastIndexOf('.') + 1));
            formdata.append(fileInput.files[0].name, fileInput.files[0]);
            jQuery("#AppLoaderContainer").show();
            var ajaxRequest = $.ajax({
                type: "POST",
                url: "../Planificacion/SubirDescripcionDetallada",
                contentType: false,
                processData: false,
                data: formdata
            });

            ajaxRequest.done(function (xhr, textStatus) {
                var ParUsuario = $("#CAPEX_H_USERNAME").val();
                var DTO = { "IniToken": iniciativa_token, "ParUsuario": ParUsuario, "ParNombre": nombreArchivo, "ParPaso": "Descripcion-Detallada", "ParCaso": "Descipción Detallada" };
                $.ajax({
                    url: "../Planificacion/RegistrarArchivo",
                    type: "GET",
                    dataType: "json",
                    data: (DTO),
                    success: function (r) {
                        jQuery("#AppLoaderContainer").hide();
                        swal("Exito", "Archivo descripcion detallada subido correctamente", "success");
                        FNPoblarDocumentos();
                        return false;
                    },
                    error: function (xhr, error, status) {
                        jQuery("#AppLoaderContainer").hide();
                        swal("Exito", "Archivo descripcion detallada subido correctamente", "success");
                        return false;
                    }
                });
            });

        }
    }
}
*/
