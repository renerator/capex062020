function previosUpload() {
    var iniciativa_token = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    if (iniciativa_token == null || iniciativa_token == "") {
        swal("", "Debe identificar iniciativa.", "info");
        return false;
    }
    if ($("#File1").val()) {
        console.log("Archivo File1 seleccionado");
        parent.FNPreviousCallBackCategorizacion();
        return true;
    } else {
        console.log("Archivo File1 no seleccionado");
        return false;
    }
}

function previosUploadImport() {
    if ($("#File2").val()) {
        console.log("Archivo File2 seleccionado");
        if (!parent.FNPreviousCallBackImportCheckToken()) {
            return false;
        }
        var fileInput = document.getElementById('File2');
        var nombreArchivo = fileInput.files[0].name;
        var tamano = fileInput.files[0].size;
        var extension = nombreArchivo.substr((nombreArchivo.lastIndexOf('.') + 1));
        if (extension != "xlsx") {
            parent.FNPreviousCallBackImportExtensionError();
            return false;
        }
        parent.FNPreviousCallBackImport();
        return true;
    } else {
        console.log("Archivo File2 no seleccionado");
        return false;
    }
}

function previosUploadGantt() {
    if ($("#File3").val()) {
        console.log("Archivo File3 seleccionado");
        parent.FNPreviousCallBackGantt();
        return true;
    } else {
        console.log("Archivo File3 no seleccionado");
        return false;
    }
}

function previosUploadDescripcion() {
    if ($("#File4").val()) {
        console.log("Archivo File4 seleccionado");
        parent.FNPreviousCallDescripcion();
        return true;
    } else {
        console.log("Archivo File4 no seleccionado");
        return false;
    }
}

function previosUploadEvaluacionEconomica() {
    if ($("#File5").val()) {
        console.log("Archivo File5 seleccionado");
        parent.FNPreviousCallEvaluacionEconomica();
        return true;
    } else {
        console.log("Archivo File5 no seleccionado");
        return false;
    }
}

function previosUploadEvaluacionRiesgo() {
    if ($("#File6").val()) {
        console.log("Archivo File6 seleccionado");
        parent.FNPreviousCallEvaluacionRiesgo();
        return true;
    } else {
        console.log("Archivo File6 no seleccionado");
        return false;
    }
}

function previosUploadCategorizacion() {
    if ($("#File7").val()) {
        console.log("Archivo File7 seleccionado");
        if (!parent.FNPreviousCallBackIngresoCategorizacionCheckToken()) {
            return false;
        }
        parent.FNPreviousCallBackIngresoCategorizacion();
        return true;
    } else {
        console.log("Archivo File7 no seleccionado");
        return false;
    }
}

function previosIngresoUploadGantt() {
    if ($("#File9").val()) {
        console.log("Archivo File9 seleccionado");
        if (!parent.FNPreviousCallBackIngresoUploadGanttCheckToken()) {
            return false;
        }
        parent.FNPreviousCallBackIngresoUploadGantt();
        return true;
    } else {
        console.log("Archivo File9 no seleccionado");
        return false;
    }
}


function previosIngresoUploadDescripcion() {
    if ($("#File10").val()) {
        console.log("Archivo File10 seleccionado");
        if (!parent.FNPreviousCallBackIngresoDescripcionCheckToken()) {
            return false;
        }
        parent.FNPreviousCallBackIngresoDescripcion();
        return true;
    } else {
        console.log("Archivo File10 no seleccionado");
        return false;
    }
}

function previosIngresoUploadEvaluacionEconomica() {
    if ($("#File11").val()) {
        console.log("Archivo File11 seleccionado");
        if (!parent.FNPreviousCallBackIngresoEvaluacionEconomicaCheckToken()) {
            return false;
        }
        parent.FNPreviousCallBackIngresoEvaluacionEconomica();
        return true;
    } else {
        console.log("Archivo File11 no seleccionado");
        return false;
    }
}

function previosIngresoUploadEvaluacionRiesgo() {
    if ($("#File12").val()) {
        console.log("Archivo File12 seleccionado");
        if (!parent.FNPreviousCallBackIngresoEvaluacionRiesgoCheckToken()) {
            return false;
        }
        parent.FNPreviousCallBackIngresoEvaluacionRiesgo();
        return true;
    } else {
        console.log("Archivo File12 no seleccionado");
        return false;
    }
}

function previosIngresoUploadImport() {
    if ($("#File13").val()) {
        console.log("Archivo File13 seleccionado");
        if (!parent.FNPreviousCallBackIngresoImportCheckToken()) {
            return false;
        }
        var fileInput = document.getElementById('File13');
        var nombreArchivo = fileInput.files[0].name;
        var tamano = fileInput.files[0].size;
        var extension = nombreArchivo.substr((nombreArchivo.lastIndexOf('.') + 1));
        if (extension != "xlsx") {
            parent.FNPreviousCallBackIngresoImportExtensionError();
            return false;
        }
        parent.FNPreviousCallBackIngresoImport();
        return true;
    } else {
        console.log("Archivo File13 no seleccionado");
        return false;
    }
}

function previosUploadDocument() {
    $("#hdnfldCategoria").val(parent.FNObtenerCategoriaSeleccionada());
    if ($("#hdnfldCategoria").val() === undefined || $("#hdnfldCategoria").val() == null || $("#hdnfldCategoria").val().length <= 0) {
        parent.FNErrorCategoria();
        return false;
    }
    if ($("#File100").val()) {
        console.log("Archivo File100 seleccionado");
        var fileInput = document.getElementById('File100');
        var tamano = fileInput.files[0].size;
        tamano = Math.round(parseInt(tamano));
        tamano = (tamano ^ 0);
        if (tamano > 10485760) {
            parent.FNErrorTamanioArchivo();
            return false;
        }
        parent.FNInicioSubidaDocumento();
        return true;
    } else {
        console.log("Archivo File3 no seleccionado");
        return false;
    }
}

function laterUploadLogout() {
    parent.document.getElementById('linkToLogout').click();
}

function laterUpload(paramJson) {
    if (paramJson && paramJson.Data && paramJson.Data.type && paramJson.Data.type != "") {
        switch (paramJson.Data.type) {
            case "1":
                parent.FNCallBackCategorizacion(paramJson);
                break;
            case "2":
                parent.FNCallBackImport(paramJson);
                break;
            case "3":
                parent.FNCallBackGantt(paramJson);
                break;
            case "4":
                parent.FNCallBackDescripcion(paramJson);
                break;
            case "5":
                parent.FNCallBackEvaluacionEconomica(paramJson);
                break;
            case "6":
                parent.FNCallBackEvaluacionRiesgo(paramJson);
                break;
            case "7":
                break;
            case "21":
                parent.FNCallBackIngresoCategorizacion(paramJson);
                break;
            case "23":
                parent.FNCallBackIngresoUploadGantt(paramJson);
                break;
            case "24":
                parent.FNCallBackIngresoDescripcion(paramJson);
                break;
            case "25":
                parent.FNCallBackIngresoEvaluacionEconomica(paramJson);
                break;
            case "26":
                parent.FNCallBackIngresoEvaluacionRiesgo(paramJson);
                break;
            case "27":
                parent.FNCallBackIngresoImport(paramJson);;
                break;
            case "100":
                parent.FNCallBackSubidaDocumento(paramJson);
                //parent.location.href = parent.location.href;
                break;
            default:
        }
    }
}