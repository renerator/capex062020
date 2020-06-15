
// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : PLANIFICACION
// FECHA            : MAYO 2018
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// METODOS          : EVALUACION DE RIESGO
/*document.getElementById("EvrMFL2").onblur = function () {
    this.value = FNFormatearReglaUnoEvalRi(this.value.replace('.', ''));
}

document.getElementById("EvrMFL2").onblur = function () {
    this.value = FNFormatearReglaUnoEvalRi(this.value.replace('.', ''));
}*/

FNCheckDownKey2 = function (selector) {
    console.log("FNCheckDownKey2 inputValue=", $(('#' + selector)).val());
    if ($(('#' + selector)).val() == "") {
        $(('#' + selector)).val("0,00");
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

/*document.getElementById("EvrProbabilidad1").onblur = function () {
    console.log("EvrProbabilidad1 INICIO this.value=", this.value);
    if (this.value > 5) {
        this.value = 5;
    } else if (this.value < 1) {
        this.value = 1;
    }
    console.log("EvrProbabilidad1 FIN this.value=", this.value);
}*/

/*document.getElementById("EvrProbabilidad2").onblur = function () {
    if (this.value > 5) {
        this.value = 5;
    } else if (this.value < 1) {
        this.value = 1;
    }
}*/

/*document.getElementById("EvrImpacto1").onblur = function () {
    console.log("EvrImpacto1 INICIO this.value=", this.value);
    if (this.value > 5) {
        this.value = 5;
    } else if (this.value < 1) {
        this.value = 1;
    }
    console.log("EvrImpacto1 FIN this.value=", this.value);
}*/

/*document.getElementById("EvrImpacto2").onblur = function () {
    if (this.value > 5) {
        this.value = 5;
    } else if (this.value < 1) {
        this.value = 1;
    }
}*/

//
// GUARDAR EVALUACION DE RIESGO
//
FNGuardarEvaluacionRiesgo = function () {

    //PARAMETROS BASE
    var estado = localStorage.getItem("CAPEX_INICIATIVA_EVALUACION_RIESGO_ESTADO");
    var IniToken = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    var IniUsuario = $("#CAPEX_H_USERNAME").val();

    var EvrProbabilidad1 = $("#EvrProbabilidad1").val();
    var EvrImpacto1 = $("#EvrImpacto1").val();
    var EvrRiesgo1 = $("#EvrRiesgo1").val();
    var EvrClasificacion1 = $("#EvrClasificacion1").val();
    var EvrMFL1 = $("#EvrMFL1").val();

    var EvrProbabilidad2 = $("#EvrProbabilidad2").val();
    var EvrImpacto2 = $("#EvrImpacto2").val();
    var EvrRiesgo2 = $("#EvrRiesgo2").val();
    var EvrClasificacion2 = $("#EvrClasificacion2").val();
    var EvrMFL2 = $("#EvrMFL2").val();

    var EriItemMR = localStorage.getItem("CAPEX_MATRIZ_RIESGO_ITEM");

    if (IniToken == "") {
        swal("Error", "Debe identificar una iniciativa para poder guardar este paso.", "error");
        return false;
    }
    if (estado == "Guardado") {
        swal("Error", "Esta evaluación ya se encuentra guardada.", "error");
        return false;
    }

    if (!$('#form_eval_riesgo').valid()) {
        console.log("validacion incorrecta");
        return false;
    }
    console.log("validacion correcta");

    if (EvrProbabilidad1 == "0" || EvrImpacto1 == "0" || EvrProbabilidad2 == "0" || EvrImpacto2 == "0" || EvrProbabilidad1 == "" || EvrImpacto1 == "" || EvrMFL1 == "" || EvrProbabilidad2 == "" || EvrImpacto2 == "" || EvrMFL2 == "") {
        swal("Error", "Debe completar el formulario.", "error");
        return false;
    } else {
        var DTOValidacion = {
            'IniToken': IniToken
        };
        var responseValidacion = FNRealizarValidacionArchivoRespaldo(DTOValidacion);
        if (responseValidacion && responseValidacion != "" && responseValidacion != undefined) {
            var DTO = {
                "IniToken": IniToken,
                "IniUsuario": IniUsuario,
                "EriProb1": EvrProbabilidad1,
                "EriImp1": EvrImpacto1,
                "EriRies1": EvrRiesgo1,
                "EriClas1": EvrClasificacion1,
                "EriMFL1": EvrMFL1,
                "EriProb2": EvrProbabilidad2,
                "EriImp2": EvrImpacto2,
                "EriRies2": EvrRiesgo2,
                "EriClas2": EvrClasificacion2,
                "EriMFL2": EvrMFL2,
                "EriItemMR": EriItemMR
            }
            jQuery("#AppLoaderContainer").show();
            $.ajax({
                url: "../../Planificacion/GuardarEvaluacionRiesgo",
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
                        swal("Error", "No es posible almacenar la evaluación.", "error");
                        return false;
                    } else if (estado == "Guardado") {
                        localStorage.setItem("CAPEX_INICIATIVA_EVALUACION_RIESGO_ESTADO", estado);
                        jQuery("#AppLoaderContainer").hide();
                        swal("Exito", "Evaluación de Riesgo Guardada", "success");
                        $("#BotonGuardarEvaluacionRiesgo").prop("disabled", true);
                        $("#BotonActualizarEvaluacionRiesgo").show();
                        return false;
                    }
                }
            });
        } else {
            swal("Error", "El archivo de respaldo no ha sido cargado.", "error");
            return false;
        }
    }
}

FNRealizarValidacionArchivoRespaldo = function (DTO) {
    var response = '';
    $.ajax({
        url: "../../Planificacion/SeleccionarAdjuntoEvaluacionRiesgoVigente",
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
            console.log("FNRealizarValidacionArchivoRespaldo estructura=", estructura)
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

FNObtenerMatrizRiesgo = function (cargar) {
    var DTO = {};
    $.ajax({
        url: "../../Planificacion/ListarMatrizRiesgo",
        type: "GET",
        dataType: "json",
        data: (DTO),
        async: false,
        success: function (r) {
            if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                document.getElementById('linkToLogout').click();
                return;
            }
            var content = '';
            var contador = 1;
            $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                /*content += '<tr>';
                content += '<td><button class="btn btn-warning btn-sm" style="font-size:10px" onclick="FNSeleccionarItemMatrizRiesgo(' + contador + ',' + value.IdMatrizRiesgo + ',\'' + value.MatrizRiesgoNombre + '\',\'' + value.MatrizRiesgoImpacto + '\',\'' + value.MatrizRiesgoProbabilidad + '\')">Seleccionar</button></td>';
                content += '<td><div align="center">' + value.IdMatrizRiesgo + '</div></td>';
                content += '<td align="left">' + value.MatrizRiesgoNombre + '</td>';
                content += '<td>' + value.MatrizRiesgoImpacto + '</td>';
                content += '<td>' + value.MatrizRiesgoProbabilidad + '</td>';
                content += '</tr>';
                if (cargar && cargar != undefined && cargar != "0" && cargar == value.IdMatrizRiesgo) {
                    FNSeleccionarItemMatrizRiesgo(contador, value.IdMatrizRiesgo, value.MatrizRiesgoNombre, value.MatrizRiesgoImpacto, value.MatrizRiesgoProbabilidad);
                }
                contador++;*/
                content += '<tr>';
                content += '<td align="center" class="col-sm-2"><a href="#"> <span class="glyphicon glyphicon-ok" style="color:green" onclick="FNSeleccionarItemMatrizRiesgo(' + contador + ',' + value.IdMatrizRiesgo + ',\'' + value.MatrizRiesgoNombre + '\',\'' + value.MatrizRiesgoImpacto + '\',\'' + value.MatrizRiesgoProbabilidad + '\')"></span> </a></td>';
                if (value.MatrizRiesgoNombre && value.MatrizRiesgoNombre.length > 64) {
                    content += '<td style="cursor:pointer;" title="' + value.MatrizRiesgoNombre + '" class="col-sm-6">' + value.MatrizRiesgoNombre.substr(0, 61) + '...</td>';
                } else {
                    content += '<td class="col-sm-6">' + value.MatrizRiesgoNombre + '</td>';
                }
                content += '<td align="center" class="col-sm-2">' + value.MatrizRiesgoImpacto + '</td>';
                content += '<td align="center" class="col-sm-2">' + value.MatrizRiesgoProbabilidad + '</td>';
                content += '</tr>';
                if (cargar && cargar != undefined && cargar != "0" && cargar == value.IdMatrizRiesgo) {
                    FNSeleccionarItemMatrizRiesgo(contador, value.IdMatrizRiesgo, value.MatrizRiesgoNombre, value.MatrizRiesgoImpacto, value.MatrizRiesgoProbabilidad);
                }
                contador++;
            });
            $('#tablaMatrizRiesgo tbody').html(content);
        }
    });
}

//
// ACTUALIZAR EVALUACION DE RIESGO
//
FNActualizarEvaluacionRiesgo = function () {

    //PARAMETROS BASE
    var estado = localStorage.getItem("CAPEX_INICIATIVA_EVALUACION_RIESGO_ESTADO");
    var IniToken = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    var IniUsuario = $("#CAPEX_H_USERNAME").val();

    var EvrProbabilidad1 = $("#EvrProbabilidad1").val();
    var EvrImpacto1 = $("#EvrImpacto1").val();
    var EvrRiesgo1 = $("#EvrRiesgo1").val();
    var EvrClasificacion1 = $("#EvrClasificacion1").val();
    var EvrMFL1 = $("#EvrMFL1").val();

    var EvrProbabilidad2 = $("#EvrProbabilidad2").val();
    var EvrImpacto2 = $("#EvrImpacto2").val();
    var EvrRiesgo2 = $("#EvrRiesgo2").val();
    var EvrClasificacion2 = $("#EvrClasificacion2").val();
    var EvrMFL2 = $("#EvrMFL2").val();

    var EriItemMR = localStorage.getItem("CAPEX_MATRIZ_RIESGO_ITEM");


    if (IniToken == "") {
        swal("", "Debe identificar una iniciativa para poder guardar este paso.", "info");
        return false;
    }

    if (!$('#form_eval_riesgo').valid()) {
        console.log("validacion incorrecta");
        return false;
    }
    console.log("validacion correcta");

    if (EvrProbabilidad1 == "0" || EvrImpacto1 == "0" || EvrProbabilidad2 == "0" || EvrImpacto2 == "0" || EvrProbabilidad1 == "" || EvrImpacto1 == "" || EvrMFL1 == "" || EvrProbabilidad2 == "" || EvrImpacto2 == "" || EvrMFL2 == "") {
        swal("Error", "Debe completar el formulario.", "error");
        return false;
    } else {
        var DTOValidacion = {
            'IniToken': IniToken
        };
        var responseValidacion = FNRealizarValidacionArchivoRespaldo(DTOValidacion);
        if (responseValidacion && responseValidacion != "" && responseValidacion != undefined) {
            var DTO = {
                "IniToken": IniToken,
                "IniUsuario": IniUsuario,
                "EriProb1": EvrProbabilidad1,
                "EriImp1": EvrImpacto1,
                "EriRies1": EvrRiesgo1,
                "EriClas1": EvrClasificacion1,
                "EriMFL1": EvrMFL1,
                "EriProb2": EvrProbabilidad2,
                "EriImp2": EvrImpacto2,
                "EriRies2": EvrRiesgo2,
                "EriClas2": EvrClasificacion2,
                "EriMFL2": EvrMFL2,
                "EriItemMR": EriItemMR
            }
            jQuery("#AppLoaderContainer").show();
            $.ajax({
                url: "../../Planificacion/ActualizarEvaluacionRiesgo",
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
                        swal("Error", "No es posible actualizar esta evaluación.", "error");
                        return false;
                    } else if (estado == "Actualizado") {
                        localStorage.setItem("CAPEX_INICIATIVA_EVALUACION_RIESGO_ESTADO", estado);
                        jQuery("#AppLoaderContainer").hide();
                        swal("Exito", "Evaluación de Riesgo Actualizada", "success");
                        return false;
                    }
                }
            });
        } else {
            swal("Error", "El archivo de respaldo no ha sido cargado.", "error");
            return false;
        }
    }
}
//
// SUBIR ARCHIVO
//
/*document.getElementById('form_eval_riesgo').onsubmit = function (e) {
    e.preventDefault();
    var iniciativa_token = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    if (iniciativa_token == null || iniciativa_token == "") {
        swal("Error", "Debe identificar iniciativa antes de importar información.", "error");
        return false;
    }
    else {

        if (document.getElementById("ArchivoEvalRiesgo").files.length == 0 || document.getElementById("ArchivoEvalRiesgo").files.length == null) {
            return false;
        }
        else {
            jQuery("#AppLoaderContainer").show();
            var formdata = new FormData();
            var fileInput = document.getElementById('ArchivoEvalRiesgo');
            var nombreArchivo = fileInput.files[0].name;
            $("#NombreArchivoRiesgo").html(nombreArchivo);
            var tamano = fileInput.files[0].size;

            var extension = nombreArchivo.substr((nombreArchivo.lastIndexOf('.') + 1));
            formdata.append(fileInput.files[0].name, fileInput.files[0]);

            var ajaxRequest = $.ajax({
                type: "POST",
                url: "../../Planificacion/SubirEvaluacionRiesgo",
                contentType: false,
                processData: false,
                data: formdata
            });

            ajaxRequest.done(function (xhr, textStatus) {
                var ParUsuario = $("#CAPEX_H_USERNAME").val();
                var DTO = { "IniToken": iniciativa_token, "ParUsuario": ParUsuario, "ParNombre": nombreArchivo, "ParPaso": "Evaluacion-Riesgo", "ParCaso": "Evaluación Riesgo" };
                $.ajax({
                    url: "../../Planificacion/RegistrarArchivo",
                    type: "GET",
                    dataType: "json",
                    data: (DTO),
                    success: function (r) {
                        jQuery("#AppLoaderContainer").hide();
                        swal("Exito", "Archivo evaluacion riesgo subido correctamente", "success");
                        FNPoblarDocumentos();
                        return false;
                    },
                    error: function (xhr, error, status) {
                        jQuery("#AppLoaderContainer").hide();
                        swal("Exito", "Archivo evaluacion riesgo subido correctamente", "success");
                        return false;
                    }
                });
            });

        }
    }
}*/

//
// MOSTRAR MATRIZ DE RIESGO
//
FNVerMatrizRiesgo = function () {
    FNObtenerMatrizRiesgo();
    $("#ModalMatrizRiesgo").show();
}
//
// MOSTRAR TABLA DE RIESGO
//
FNVerModalTablaRiesgo = function () {
    $("#ModalTablaRiesgo").show();
}
//
// MOSTRAR IMPACTO PROYECTO
//
FNVerModalImpactoProyecto = function () {
    $("#ModalImpactoPoyecto").show();
}
//
// MOSTRAR NIVELES DE PROBABILIDAD
//
FNVerModalNivelesProbabilidad = function () {
    $("#ModalNivelesProbabilidad").show();
}
//
// CERRAR IMPACTO PROYECTO
//
FNCerrarModalImpactoProyecto = function () {
    $("#ModalImpactoPoyecto").hide();
}
//
// CERRAR NIVELES DE PROBABILIDAD
//
FNCerrarModalNivelesProbabilidad = function () {
    $("#ModalNivelesProbabilidad").hide();
}
//
// CERRAR MATRIZ DE RIESGO
//
FNCerrarModalMatrizRiesgo = function () {
    $("#ModalMatrizRiesgo").hide();
}
//
// TABLA DE RIESGOS
//
FNCerrarModalTablaRiesgo = function () {
    $("#ModalTablaRiesgo").hide();
}
//
// ASIGANCION MATRIZ TABLA
//
FNSeleccionarItemMatrizRiesgo = function (contador, rr, glosa, impacto, probabilidad) {
    FNCerrarModalMatrizRiesgo();
    $("#ConetenidoRiesgoClaveEnPantalla").html("<strong>RR:</strong> " + contador + " <strong>Impacto:</strong> " + impacto + " <strong>Probabilidad:</strong> " + probabilidad + " <br><span style='margin-left:30px;'><blockquote><strong>Riesgo Clave:</strong> " + glosa + "</blockquote></span>");
    $("#ContenedorRiesgoClaveEnPantalla").show();
    localStorage.setItem("CAPEX_MATRIZ_RIESGO_ITEM", rr);
}
//
// CALCULO DE RIESGO
//
//
// INICIO CONTROL DE CAMBIO, CAMBIA PONDERACION MODIFICA "MODERADO" POR "MEDIO" Y SE AGREGA "EXTREMO"
//

FNCalculoRiesgoSinProyecto = function () {
    var probabilidad = $("#EvrProbabilidad1").val();
    var impacto = $("#EvrImpacto1").val();

    if (!probabilidad || probabilidad == undefined || probabilidad.trim() == "" || !impacto || impacto == undefined || impacto.trim() == "") {
        $("#EvrRiesgo1").val("");
        $("#EvrClasificacion1").val("");
        return;
    }

    var resultado = parseInt(probabilidad) * parseInt(impacto);
    $("#EvrRiesgo1").val(resultado)
    //1
    if (probabilidad == 1) {
        if (resultado < 3) {
            $("#EvrClasificacion1").val("Bajo");
        }
        else if (resultado > 2 && resultado < 5) {
            $("#EvrClasificacion1").val("Medio");
        }
        else if (resultado == 5) {
            $("#EvrClasificacion1").val("Alto");
        }
    }
    //2
    if (probabilidad == 2) {
        if (resultado > 1 && resultado < 5) {
            $("#EvrClasificacion1").val("Bajo");
        }
        else if (resultado > 5 && resultado < 7) {
            $("#EvrClasificacion1").val("Medio");
        }
        else if (resultado > 7 && resultado < 11) {
            $("#EvrClasificacion1").val("Alto");
        }
    }
    //3
    if (probabilidad == 3) {
        if (resultado == 3) {
            $("#EvrClasificacion1").val("Bajo");
        }
        else if (resultado == 6) {
            $("#EvrClasificacion1").val("Medio");
        }
        else if (resultado > 8 && resultado < 13) {
            $("#EvrClasificacion1").val("Alto");
        }
        else if (resultado == 15) {
            $("#EvrClasificacion1").val("Extremo");
        }
    }
    //4
    if (probabilidad == 4) {
        if (resultado > 3 && resultado < 9) {
            $("#EvrClasificacion1").val("Medio");
        }
        else if (resultado == 12) {
            $("#EvrClasificacion1").val("Alto");
        }
        else if (resultado > 12 && resultado < 21) {
            $("#EvrClasificacion1").val("Extremo");
        }
    }
    //5
    if (probabilidad == 5) {
        if (resultado == 5) {
            $("#EvrClasificacion1").val("Medio");
        }
        else if (resultado == 10) {
            $("#EvrClasificacion1").val("Alto");
        }
        else if (resultado > 14 && resultado < 26) {
            $("#EvrClasificacion1").val("Extremo");
        }
    }
}
FNCalculoRiesgoConProyecto = function () {
    var probabilidad = $("#EvrProbabilidad2").val();
    var impacto = $("#EvrImpacto2").val();

    if (!probabilidad || probabilidad == undefined || probabilidad.trim() == "" || !impacto || impacto == undefined || impacto.trim() == "") {
        $("#EvrRiesgo2").val("");
        $("#EvrClasificacion2").val("");
        return;
    }

    var resultado = parseInt(probabilidad) * parseInt(impacto);
    $("#EvrRiesgo2").val(resultado)
    //1
    if (probabilidad == 1) {
        if (resultado < 3) {
            $("#EvrClasificacion2").val("Bajo");
        }
        else if (resultado > 2 && resultado < 5) {
            $("#EvrClasificacion2").val("Medio");
        }
        else if (resultado == 5) {
            $("#EvrClasificacion2").val("Alto");
        }
    }
    //2
    if (probabilidad == 2) {
        if (resultado > 1 && resultado < 5) {
            $("#EvrClasificacion2").val("Bajo");
        }
        else if (resultado > 5 && resultado < 7) {
            $("#EvrClasificacion2").val("Medio");
        }
        else if (resultado > 7 && resultado < 11) {
            $("#EvrClasificacion2").val("Alto");
        }
    }
    //3
    if (probabilidad == 3) {
        if (resultado == 3) {
            $("#EvrClasificacion2").val("Bajo");
        }
        else if (resultado == 6) {
            $("#EvrClasificacion2").val("Medio");
        }
        else if (resultado > 8 && resultado < 13) {
            $("#EvrClasificacion2").val("Alto");
        }
        else if (resultado == 15) {
            $("#EvrClasificacion2").val("Extremo");
        }
    }
    //4
    if (probabilidad == 4) {
        if (resultado > 3 && resultado < 9) {
            $("#EvrClasificacion2").val("Medio");
        }
        else if (resultado == 12) {
            $("#EvrClasificacion2").val("Alto");
        }
        else if (resultado > 12 && resultado < 21) {
            $("#EvrClasificacion2").val("Extremo");
        }
    }
    //5
    if (probabilidad == 5) {
        if (resultado == 5) {
            $("#EvrClasificacion2").val("Medio");
        }
        else if (resultado == 10) {
            $("#EvrClasificacion2").val("Alto");
        }
        else if (resultado > 14 && resultado < 26) {
            $("#EvrClasificacion2").val("Extremo");
        }
    }
    //
    // FIN CONTROL CAMBIO
    //
}
