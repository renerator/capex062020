
FNEvaluarCategoria = function () {
    if (FNEvaluarRequerimientoNivelIngeneria()) {
        FNObtenerListadoNivelIngenieraNoRequiereCorregido(true);
    } else {
        FNObtenerListadoNivelIngenieraNoRequiereCorregido(false);
    }
    var Categoria = $("#Categoria").val();
    if (Categoria.trim() == "9005BD87-6800-49F1-9D68-9A11EFBBDBAF") {
        console.log("FNEvaluarCategoria if Categoria=", Categoria);
        FNObtenerListadoClasificacionSSOHSEC();
    } else {
        console.log("FNEvaluarCategoria else Categoria=", Categoria);
        if (Categoria != "" && Categoria == "-1") {
            FNObtenerListadoClasificacionSSO("");
        } else {
            FNObtenerListadoClasificacionSSO("029FA2E7-58FA-4E86-B93D-8ED5A9DA28ED");
        }
    }
}

// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : PLANIFICACION
// FECHA            : MAYO 2018
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// SECCION          : CATEGORIZACION
// METODOS          :

//
// LISTAR OPCIONES DE CATEGORIA
//
FNObtenerListadoCategorias = function (Macrocategoria) {
    //PREPARAR
    var Categoria = $('#Categoria');
    Categoria.empty();
    Categoria.append('<option value="-1" selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../Planificacion/ListarCategorias",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            Categoria.empty();
            setTimeout(function () {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    //INICIO  - CONTROL DE CAMBIO, 24/07/2019
                    if (Macrocategoria == "DEVELOPMENT") {
                        if (value.CatToken.trim() == "BDC8CC53-16CB-4F82-8B83-79B02C323EDE" || value.CatToken.trim() == "0D58C515-9D3C-4A02-B904-A5D04310EB61" || value.CatToken.trim() == "1D823A68-B601-4763-B91D-4F28571DF372" || value.CatToken.trim() == "30A15475-6851-4DCB-861C-43106284E241") {
                            Categoria.append(new Option(value.CatNombre, value.CatToken, false, false));
                        }
                    }
                    else if (Macrocategoria == "SUSTAINING") {
                        if (value.CatToken.trim() !== "BDC8CC53-16CB-4F82-8B83-79B02C323EDE" && value.CatToken.trim() !== "0D58C515-9D3C-4A02-B904-A5D04310EB61") {
                            Categoria.append(new Option(value.CatNombre, value.CatToken, false, false));
                        }
                    }
                    //FIN  - CONTROL DE CAMBIO, 24/07/2019
                    cuantos++;
                });
                if (cuantos == 1) {
                    Categoria.prop('selectedIndex', 0);
                }
                else if (cuantos > 1) {
                    Categoria.append('<option value="-1" selected="true">Seleccionar..</option>')
                }
            }, 200);
        }
    });
}
//
// LISTAR OPCIONES NIVEL DE INGENIERIA
//
FNObtenerListadoNivelIngeniera = function () {
    //PREPARAR
    var NivelIngenieria = $('#NivelIngenieria');
    NivelIngenieria.empty();
    NivelIngenieria.append('<option value="-1" selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../Planificacion/ListarNivelIngenieria",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            NivelIngenieria.empty();
            setTimeout(function () {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    NivelIngenieria.append(new Option(value.NINombre, value.NIAcronimo, false, false))
                    cuantos++;
                });
                if (cuantos == 1) {
                    NivelIngenieria.prop('selectedIndex', 0);
                }
                else if (cuantos > 1) {
                    NivelIngenieria.append('<option value="-1" selected="true">Seleccionar..</option>')
                }
            }, 200);
        }
    });
}

//
// LISTAR OPCIONES NIVEL DE INGENIERIA NO REQUIERE
//
FNObtenerListadoNivelIngenieraNoRequiere = function () {
    var DTO = {};
    $.ajax({
        url: "../Planificacion/ListarNivelIngenieriaNoRequiere",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            setTimeout(function () {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    var contentSpan = "<span><strong>" + value.NINombre + "</strong></span>";
                    $("#ContenedorNivelIngenieria2").html(contentSpan);
                    localStorage.setItem("CAPEX_NIVEL_INGENIERIA_ALTERNATIVO", value.NIAcronimo);
                });
            }, 200);
        }
    });
}

FNObtenerListadoNivelIngenieraNoRequiereCorregido = function (append) {
    var DTO = {};
    $.ajax({
        url: "../Planificacion/ListarNivelIngenieriaNoRequiere",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            setTimeout(function () {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                $('#NivelIngenieria').val("-1");
                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    if (append) {
                        var encontrado = false;
                        $("#NivelIngenieria option").each(function (i) {
                            if ("-1" == $(this).val()) {
                                encontrado = true;
                            }
                        });
                        if (encontrado) {
                            $("#NivelIngenieria option[value='-1']").remove();
                        }
                        encontrado = false;
                        $("#NivelIngenieria option").each(function (i) {
                            if (value.NIAcronimo == $(this).val()) {
                                encontrado = true;
                            }
                        });
                        if (encontrado) {
                            $("#NivelIngenieria option[value='" + value.NIAcronimo + "']").remove();
                        }
                        $("#NivelIngenieria").append(new Option(value.NINombre, value.NIAcronimo, false, false));
                        $("#NivelIngenieria").append('<option value="-1" selected="true">Seleccionar..</option>');
                    } else {
                        var encontrado = false;
                        $("#NivelIngenieria option").each(function (i) {
                            if (value.NIAcronimo == $(this).val()) {
                                encontrado = true;
                            }
                        });
                        if (encontrado) {
                            $("#NivelIngenieria option[value='" + value.NIAcronimo + "']").remove();
                        }
                    }
                });
            }, 200);
        }
    });
}


//
// LISTAR OPCIONES SSO
//
FNObtenerListadoClasificacionSSO = function (CSToken) {
    //PREPARAR
    var ClasificacionSSO = $('#ClasificacionSSO');
    ClasificacionSSO.empty();
    ClasificacionSSO.append('<option value="-1" selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../Planificacion/ListarClasificacionSSO",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            ClasificacionSSO.empty();
            setTimeout(function () {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    if (value.CSToken == CSToken) {
                        ClasificacionSSO.append(new Option(value.CSNombre, value.CSToken, false, false))
                        cuantos++;
                    }
                });
                if (cuantos == 1) {
                    ClasificacionSSO.prop('selectedIndex', 0);
                } else if (cuantos == 0 || cuantos > 1) {
                    ClasificacionSSO.append('<option value="-1" selected="true">Seleccionar..</option>')
                }
                if (cuantos == 0 || cuantos == 1) {
                    FNProcesarOpcionSSO();
                }
                FNCheckFormDinamyc();
            }, 200);
        }
    });
}

//
// LISTAR OPCIONES SSO
//
FNObtenerListadoClasificacionSSOHSEC = function () {
    //PREPARAR
    var ClasificacionSSO = $('#ClasificacionSSO');
    ClasificacionSSO.empty();
    ClasificacionSSO.append('<option value="-1" selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../Planificacion/ListarClasificacionSSO",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            ClasificacionSSO.empty();
            setTimeout(function () {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    if (value.CSToken != "029FA2E7-58FA-4E86-B93D-8ED5A9DA28ED") {
                        ClasificacionSSO.append(new Option(value.CSNombre, value.CSToken, false, false))
                        cuantos++;
                    }
                });
                if (cuantos == 1) {
                    ClasificacionSSO.prop('selectedIndex', 0);
                }
                else if (cuantos > 1) {
                    ClasificacionSSO.append('<option value="-1" selected="true">Seleccionar..</option>')
                }
            }, 200);
        }
    });
}
//
// AUX SSO
//
FNProcesarOpcionSSO = function () {
    /**
     * REGLA DE NEGOCIO 5
     * RN005
     */
    var glosa = jQuery("#ClasificacionSSO option:selected").text();
    if (glosa == "Ley" || glosa == "Mejoras SSO" || glosa == "No Aplica" || glosa == "RCA" || glosa == "EIA") {
        $("#ContenedorSSO").hide();
        $("#ContenedorInformacionLateralIzquierdo").hide();
    } else {
        $("#ContenedorSSO").show();
        $("#ContenedorInformacionLateralIzquierdo").show();
    }
    return
}
//
// LISTAR ESTANDARES DE SEGURIDAD
//
FNObtenerEstandarSeguridad = function () {
    //PREPARAR
    var EstandarSeguridad = $('#EstandarSeguridad');
    EstandarSeguridad.empty();
    EstandarSeguridad.append('<option value="-1" selected="true">Buscando..</option>');
    //LISTO
    var EssComToken = localStorage.getItem("CAPEX_PLAN_COMPANIA_TOKEN");
    var EssCSToken = localStorage.getItem("CAPEX_PLAN_SSO");
    if (EssComToken == "" || EssComToken == null) {
        swal("", "Debe seleccionar 'Compañía' en etapa de Identificación.", "info");
        return;
    }
    else {
        var DTO = { "EssComToken": EssComToken, "EssCSToken": EssCSToken };
        var cuantos = 0
        $.ajax({
            url: "../Planificacion/ListarEstandarSeguridad",
            type: "GET",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                EstandarSeguridad.empty();
                setTimeout(function () {
                    $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                        EstandarSeguridad.append(new Option(value.EssNombre, value.EssToken, false, false))
                        cuantos++;
                    });
                    if (cuantos == 1) {
                        EstandarSeguridad.prop('selectedIndex', 0);
                    }
                    else if (cuantos > 1) {
                        EstandarSeguridad.append('<option value="-1" selected="true">Seleccionar..</option>')
                    }
                }, 200);
            }
        });
    }
}
//
// CICLO INVERSIONAL (CI)
// - CAMBIO DE ETAPA
// - CAMBIO DE NIVEL
// - ANALISIS DE BAJA COMPLEJIDAD
//

//
// ANALISIS CI
//
FnAnalizarEtapaCicloInversional = function (opcion) {
    var etapa = localStorage.getItem("CAPEX_PLAN_ETAPA");
    if (etapa == "" || etapa == null) {
        swal("Error", "Debe seleccionar 'Etapa' en paso anterior de Identificación.", "error");
        $("#NivelIngenieria option:last").prop("selected", "selected");
        return false;
    } else {
        var tipo = localStorage.getItem("CAPEX_TIPO_INICIATIVA");
        if (FNEvaluarRequerimientoNivelIngeneria() || tipo == "CB" || tipo == "CD") {
            return false;
        }
        $("#ContenedorAnalisisBajaComplejidad").hide();
        FNQuitarSeleccionPreviaDeOpcionesSolucionCicloInversional();
        switch (etapa) {
            case "PERF":
                if (opcion == "PERF") {
                    swal("Mensaje", "No se puede validar ciclo inversional debido a que la etapa es la misma que nivel de ingenieria.", "info");
                    $("#NivelIngenieria option:last").prop("selected", "selected");
                    return false;
                }
                else if (opcion == "PREF") {
                    swal("Mensaje", "No se puede validar ciclo inversional debido a que la etapa seleccionada es anterior a la que usted está seleccionando en nivel de ingenieria.", "info");
                    $("#NivelIngenieria option:last").prop("selected", "selected");
                    return false;
                }
                else if (opcion == "FACT") {
                    swal("Mensaje", "No se puede validar ciclo inversional debido a que la etapa seleccionada es anterior a la que usted está seleccionando en nivel de ingenieria.", "info");
                    $("#NivelIngenieria option:last").prop("selected", "selected");
                    return false;
                }
                else if (opcion == "EJEC") {
                    swal("Mensaje", "No se puede validar ciclo inversional debido a que la etapa seleccionada es anterior a la que usted está seleccionando en nivel de ingenieria.", "info");
                    $("#NivelIngenieria option:last").prop("selected", "selected");
                    return false;
                }
                break;
            case "PREF":
                if (opcion == "PREF") {
                    swal("Mensaje", "No se puede validar ciclo inversional debido a que la etapa es la misma que nivel de ingenieria.", "info");
                    $("#NivelIngenieria option:last").prop("selected", "selected");
                    return false;
                }
                else if (opcion == "FACT") {
                    swal("Mensaje", "No se puede validar ciclo inversional debido a que la etapa seleccionada es anterior a la que usted está seleccionando en nivel de ingenieria.", "info");
                    $("#NivelIngenieria option:last").prop("selected", "selected");
                    return false;
                }
                else if (opcion == "EJEC") {
                    swal("Mensaje", "No se puede validar ciclo inversional debido a que la etapa seleccionada es anterior a la que usted está seleccionando en nivel de ingenieria.", "info");
                    $("#NivelIngenieria option:last").prop("selected", "selected");
                    return false;
                }
                break;
            case "FACT":
                if (opcion == "PERF") {
                    FNMostrarModalCicloInversional();
                    return false;
                }
                else if (opcion == "FACT") {
                    swal("Mensaje", "No se puede validar ciclo inversional debido a que la etapa es la misma que nivel de ingenieria.", "info");
                    $("#NivelIngenieria option:last").prop("selected", "selected");
                    return false;
                }
                else if (opcion == "EJEC") {
                    swal("Mensaje", "No se puede validar ciclo inversional debido a que la etapa seleccionada es anterior a la que usted está seleccionando en nivel de ingenieria.", "info");
                    $("#NivelIngenieria option:last").prop("selected", "selected");
                    return false;
                }
                break;
            case "EJEC":
                if (opcion == "PERF") {
                    FNMostrarModalCicloInversional();
                    return false;
                }
                else if (opcion == "PREF") {
                    FNMostrarModalCicloInversional();
                    return false;
                }
                else if (opcion == "EJEC") {
                    swal("Mensaje", "No se puede validar ciclo inversional debido a que la etapa es la misma que nivel de ingenieria.", "info");
                    $("#NivelIngenieria option:last").prop("selected", "selected");
                    return false;
                }
                break;
        }
    }
}
//
// AUXILIAR CI-0
//

//
//INICIO  - CONTROL DE CAMBIO, 24/07/2019
//
FNMostrarModalCicloInversional = function () {
    var Categoria = $("#Categoria").val();
    if (Categoria.trim() == "DC77FD26-10C4-4B09-83DC-04E926162A72") {
        $("#opci1").prop('checked', false); $("#opci1").prop('disabled', true);
        $("#opci3").prop('checked', false); $("#opci3").prop('disabled', true);
        $("#opci2").click(); $("#opci2").prop('checked', true);
        $('#ModalCicloInversional').modal('show');
    }
    else {
        $('#ModalCicloInversional').modal('show');
    }
}
//
//FIN  - CONTROL DE CAMBIO, 24/07/2019
//

//
// AUXILIAR CI-1
//
FNFijarOpcionSolucionCicloInversional = function (opcion) {
    $('#BotonAceptarOpcionCicloInversional').prop('disabled', false);
    localStorage.setItem("CAPEX_OPCION_SOLUCION_CICLOINVERSIONAL", opcion);
}
//
// AUXILIAR CI-2
//
FNQuitarSeleccionPreviaDeOpcionesSolucionCicloInversional = function () {
    //OPCIONES
    $("#opci1").prop('checked', false);
    $("#opci2").prop('checked', false);
    $("#opci3").prop('checked', false);
    $("#opci4").prop('checked', false);
    //NOTAS
    $("#n11").prop('checked', true);
    $("#n21").prop('checked', true);

    $("#n31").prop('checked', true);;
    $("#n41").prop('checked', true);

    $("#n51").prop('checked', true);
    $("#n61").prop('checked', true);

    //TOTALES
    $("#TotalNota1").val("1");
    $("#TotalNota2").val("1");
    $("#TotalNota3").val("1");
    $("#TotalNota4").val("1");
    $("#TotalNota5").val("1");
    $("#TotalNota6").val("1");
    $("#TotalAnalisis").val("6");
    //DESCRIPCIONES
    $("#NotaInterferencia").val("");
    $("#NotaRiesgo").val("");
    $("#NotaSustentabilidad").val("");
    $("#NotaComplejidad").val("");
    $("#NotaEmplazamiento").val("");
    $("#NotaSolucion").val("");
    //CONTENEDORES
    $("#ValorRecomendacion").html("Esperando ingreso de notas..")
    //HABILITAR BOTON GUARDAR O ACTUALIZAR SEGUN CORRESPONDA
    var capexIniciativaCategorizacionEstado = localStorage.getItem("CAPEX_INICIATIVA_CATEGORIZACION_ESTADO");
    if (!capexIniciativaCategorizacionEstado || capexIniciativaCategorizacionEstado == undefined || capexIniciativaCategorizacionEstado == "") {
        $("#BotonGuardarCategorizacion").prop('disabled', false);
    } else if (capexIniciativaCategorizacionEstado == "Guardado" || capexIniciativaCategorizacionEstado == "Actualizado") {
        $("#BotonActualizarCategorizacion").prop('disabled', false);
    }
}
//
// AUXILIAR CI - 3
//
//INICIO  - CONTROL DE CAMBIO, 24/07/2019
FNProcesarResetDeControles = function () {
    var Categoria = $("#Categoria").val();
    if (Categoria.trim() == "DC77FD26-10C4-4B09-83DC-04E926162A72") {
        $("#Categoria option:last").prop("selected", "selected");
        $("#NivelIngenieria option:last").prop("selected", "selected");
    }
    else {
        $("#NivelIngenieria option:last").prop("selected", "selected");
    }
}
//FIN  - CONTROL DE CAMBIO, 24/07/2019
//
// AUXILIAR CI - 4
//
FNProcesarOpcionSolucionCicloInversional = function () {
    /**
     * REGLA DE NEGOCIO 4
     * RN004
     */
    var opcion = localStorage.getItem("CAPEX_OPCION_SOLUCION_CICLOINVERSIONAL");
    switch (opcion) {
        case "CAMBIO-NIVEL":
            var etapa = localStorage.getItem("CAPEX_PLAN_ETAPA");
            switch (etapa) {
                case "FACT":
                    $('#NivelIngenieria>option:eq(1)').prop('selected', true);
                    break;
                case "EJEC":
                    $('#NivelIngenieria>option:eq(2)').prop('selected', true);
                    break;
            }
            $('#ModalCicloInversional').modal('hide');
            swal("", "Nivel de ingeniería modificado a valor permitido por ciclo inversional.", "success");
            break;
        case "CAMBIO-ETAPA":
            var nivel = $("#NivelIngenieria").val();
            switch (nivel) {
                case "PREF":
                    localStorage.setItem("CAPEX_PLAN_ETAPA", "FACT");
                    $('#Etapa>option:eq(2)').prop('selected', true);
                    break;
                case "PERF":
                    localStorage.setItem("CAPEX_PLAN_ETAPA", "PREF");
                    $('#Etapa>option:eq(1)').prop('selected', true);
                    break;
                case "FACT":
                    localStorage.setItem("CAPEX_PLAN_ETAPA", "EJEC");
                    $('#Etapa>option:eq(3)').prop('selected', true);
                    break;
            }
            FNActualizarEtapaAlmacenada();
            $('#ModalCicloInversional').modal('hide');
            swal("", "Etapa modificada a valor permitido por ciclo inversional.", "success");

            break;
        case "ANALISIS":
            localStorage.setItem("CAPEX_PLAN_ANALISIS_BAJA_COMPLEJIDAD", "SI");
            $("#ContenedorRecomendacion").hide();
            $("#ContenedorAnalisisBajaComplejidad").show();
            FNObtenerTextoAyuda('BAJA_COMPLEJIDAD', '');
            break;
        case "ADMIN":
            break;
    }
}
//
// AUXILIAR CI - 5
//
FNCalcularSubtotal = function (cual, nota) {
    if (parseInt(nota) == 3) {
        switch (cual) {
            case "INTERFERENCIA":
                $("#TotalNota1").val(nota);
                break;
            case "RIESGO":
                $("#TotalNota2").val(nota);
                break;
            case "SUSTENTABILIDAD":
                $("#TotalNota3").val(nota);
                break;
            case "COMPLEJIDAD":
                $("#TotalNota4").val(nota);
                break;
            case "EMPLAZAMIENTO":
                $("#TotalNota5").val(nota);
                break;
            case "SOLUCION":
                $("#TotalNota6").val(nota);
                break;
        }
        FNCalcularNotaFinal("FLUJO-0", cual);
        return true;
    }
    else {
        switch (cual) {
            case "INTERFERENCIA":
                $("#TotalNota1").val(nota);
                break;
            case "RIESGO":
                $("#TotalNota2").val(nota);
                break;
            case "SUSTENTABILIDAD":
                $("#TotalNota3").val(nota);
                break;
            case "COMPLEJIDAD":
                $("#TotalNota4").val(nota);
                break;
            case "EMPLAZAMIENTO":
                $("#TotalNota5").val(nota);
                break;
            case "SOLUCION":
                $("#TotalNota6").val(nota);
                break;
        }
        FNCalcularNotaFinal("FLUJO-1", cual);
        return true;
    }
}
//
// AUXILIAR CI - 6
//
FNCalcularNotaFinal = function (flujo, cual) {

    var r = parseInt(0);
    var ri = parseInt(0);
    var no1 = $("#TotalNota1").val();
    if (no1 == "") {
        no1 = parseInt(0);
    }
    var no2 = $("#TotalNota2").val();
    if (no2 == "") {
        no2 = parseInt(0);
    }
    var no3 = $("#TotalNota3").val();
    if (no3 == "") {
        no3 = parseInt(0);
    }
    var no4 = $("#TotalNota4").val();
    if (no4 == "") {
        no4 = parseInt(0);
    }
    var no5 = $("#TotalNota5").val();
    if (no5 == "") {
        no5 = parseInt(0);
    }
    var no6 = $("#TotalNota6").val();
    if (no6 == "") {
        no6 = parseInt(0);
    }

    var n1 = parseInt(no1);
    var n2 = parseInt(no2);
    var n3 = parseInt(no3);
    var n4 = parseInt(no4);
    var n5 = parseInt(no5);
    var n6 = parseInt(no6);
    r = parseInt(n1 + n2 + n3 + n4 + n5 + n6);
    ri = parseInt(r);
    /**
     * REGLA DE NEGOCIO 7
     * RN007
     */

    var TextoAnalisis = "Se recomienda el ingreso del proyecto a la etapa de ";
    if (ri == 9 || ri < 9) {
        if (!FNRevisionOpcionesSeleccionadas()) {
            if (flujo == "FLUJO-1") {
                $("#TotalAnalisis").val(r);
                $("#ValorRecomendacion").html(TextoAnalisis + " EJECUCION");
                $("#ContenedorRecomendacion").show();
                var capexIniciativaCategorizacionEstado = localStorage.getItem("CAPEX_INICIATIVA_CATEGORIZACION_ESTADO");
                if (!capexIniciativaCategorizacionEstado || capexIniciativaCategorizacionEstado == undefined || capexIniciativaCategorizacionEstado == "") {
                    $("#BotonGuardarCategorizacion").prop('disabled', false);
                } else if (capexIniciativaCategorizacionEstado == "Guardado" || capexIniciativaCategorizacionEstado == "Actualizado") {
                    $("#BotonActualizarCategorizacion").prop('disabled', false);
                }
            }
            else if (flujo == "FLUJO-0") {
                $("#TotalAnalisis").val(r);
                $("#ValorRecomendacion").html("No se puede recomendar etapa de EJECUCION con los parámetros seleccionados.");
                $("#ContenedorRecomendacion").show();
                var capexIniciativaCategorizacionEstado = localStorage.getItem("CAPEX_INICIATIVA_CATEGORIZACION_ESTADO");
                if (!capexIniciativaCategorizacionEstado || capexIniciativaCategorizacionEstado == undefined || capexIniciativaCategorizacionEstado == "") {
                    $("#BotonGuardarCategorizacion").prop('disabled', true);
                } else if (capexIniciativaCategorizacionEstado == "Guardado" || capexIniciativaCategorizacionEstado == "Actualizado") {
                    $("#BotonActualizarCategorizacion").prop('disabled', true);
                }
            }
        }
        else {
            $("#TotalAnalisis").val(r);
            $("#ValorRecomendacion").html("No se puede recomendar etapa de EJECUCION con los parámetros seleccionados.");
            $("#ContenedorRecomendacion").show();
            var capexIniciativaCategorizacionEstado = localStorage.getItem("CAPEX_INICIATIVA_CATEGORIZACION_ESTADO");
            if (!capexIniciativaCategorizacionEstado || capexIniciativaCategorizacionEstado == undefined || capexIniciativaCategorizacionEstado == "") {
                $("#BotonGuardarCategorizacion").prop('disabled', true);
            } else if (capexIniciativaCategorizacionEstado == "Guardado" || capexIniciativaCategorizacionEstado == "Actualizado") {
                $("#BotonActualizarCategorizacion").prop('disabled', true);
            }
        }

    }
    else if (ri > 9 && ri < 13) {
        $("#TotalAnalisis").val(r);
        $("#ValorRecomendacion").html(TextoAnalisis + " FACTIBILIDAD");
        $("#ContenedorRecomendacion").show();
        var capexIniciativaCategorizacionEstado = localStorage.getItem("CAPEX_INICIATIVA_CATEGORIZACION_ESTADO");
        if (!capexIniciativaCategorizacionEstado || capexIniciativaCategorizacionEstado == undefined || capexIniciativaCategorizacionEstado == "") {
            $("#BotonGuardarCategorizacion").prop('disabled', false);
        } else if (capexIniciativaCategorizacionEstado == "Guardado" || capexIniciativaCategorizacionEstado == "Actualizado") {
            $("#BotonActualizarCategorizacion").prop('disabled', false);
        }
    }
    else if (ri > 12) {
        $("#TotalAnalisis").val(r);
        $("#ValorRecomendacion").html(TextoAnalisis + " PRE-FACTILIDAD");
        $("#ContenedorRecomendacion").show();
        var capexIniciativaCategorizacionEstado = localStorage.getItem("CAPEX_INICIATIVA_CATEGORIZACION_ESTADO");
        if (!capexIniciativaCategorizacionEstado || capexIniciativaCategorizacionEstado == undefined || capexIniciativaCategorizacionEstado == "") {
            $("#BotonGuardarCategorizacion").prop('disabled', false);
        } else if (capexIniciativaCategorizacionEstado == "Guardado" || capexIniciativaCategorizacionEstado == "Actualizado") {
            $("#BotonActualizarCategorizacion").prop('disabled', false);
        }
    }
    localStorage.setItem("CAPEX_NOTA_ANALISIS_BAJA_COMPLEJIDAD", r);
}
//
// AUX CI -7
//
FNRevisionOpcionesSeleccionadas = function () {
    if ($('#n13').is(":checked") || $('#n23').is(":checked") || $('#n33').is(":checked") || $('#n43').is(":checked") || $('#n53').is(":checked") || $('#n63').is(":checked")) {
        return true;
    }
    return false;
}

FNCheckFormDinamyc = function () {
    if ($('#ContenedorEstadoNoSeleccionable').is(':visible')) {
        console.log("ContenedorEstadoNoSeleccionable es visible");
        $('#EstadoProyecto1').rules('remove', 'required');
        $('#EstadoProyecto1').rules("add", { required: { depends: function () { $(this).val($.trim($(this).val())); return true; } }, messages: { required: "Por favor, ingrese estado del proyecto" } });
    } else {
        console.log("ContenedorEstadoNoSeleccionable no es visible");
        $('#EstadoProyecto1').rules('remove', 'required');
    }
    if ($('#ContenedorEstadoSeleccionable1').is(':visible')) {
        console.log("ContenedorEstadoSeleccionable1 es visible");
        $('#EstadoProyecto2').rules('remove', 'valueNotEquals');
        $('#EstadoProyecto2').rules("add", { valueNotEquals: "-1", messages: { valueNotEquals: "Por favor, selecciona estado del proyecto" } });
    } else {
        console.log("ContenedorEstadoSeleccionable1 no es visible");
        $('#EstadoProyecto2').rules('remove', 'valueNotEquals');
    }
    if ($('#ContenedorEstadoSeleccionable2').is(':visible')) {
        console.log("ContenedorEstadoSeleccionable2 es visible");
        $('#EstadoProyecto3').rules('remove', 'valueNotEquals');
        $('#EstadoProyecto3').rules("add", { valueNotEquals: "-1", messages: { valueNotEquals: "Por favor, selecciona estado del proyecto" } });
    } else {
        console.log("ContenedorEstadoSeleccionable2 no es visible");
        $('#EstadoProyecto3').rules('remove', 'valueNotEquals');
    }

    if ($('#ContenedorSSO').is(':visible')) {
        console.log("ContenedorSSO es visible");
        $('#EstandarSeguridad').rules("add", { valueNotEquals: "-1", messages: { valueNotEquals: "Por favor, selecciona estándar de Seguridad" } });
    } else {
        console.log("ContenedorSSO no es visible");
        $('#EstandarSeguridad').rules('remove', 'valueNotEquals');
    }

    if (!$('#form_categorizacion').valid()) {
        console.log("validacion incorrecta");
        return false;
    }
    console.log("validacion correcta");
    return true;
}

//
// GUARDAR CATEGORIZACION
//
FNGuardarCategorizacion = function () {

    var estado = localStorage.getItem("CAPEX_INICIATIVA_CATEGORIZACION_ESTADO");
    var IniToken = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    if (IniToken == "") {
        swal("Error", "Debe identificar una iniciativa para poder guardar este paso.", "error");
        return false;
    }
    if (estado == "Guardado") {
        swal("Error", "Esta categorización ya se encuentra guardada.", "error");
        return false;
    }
    if (!FNCheckFormDinamyc()) {
        return false;
    } else {
        var IniUsuario = $("#CAPEX_H_USERNAME").val();
        var EstadoProyecto = "";
        var estado_proyecto = localStorage.getItem("CAPEX_PLAN_ESTADO_PROYECTO");
        if (estado_proyecto == "") {
            EstadoProyecto = $("#EstadoProyecto1").val();
        } else {
            EstadoProyecto = estado_proyecto;
        }

        var Categoria = $("#Categoria").val();
        var TipoCotizacion = $("#TipoCotizacion").val();

        var NivelIngenieria = $("#NivelIngenieria").val();


        var EstandarSeguridad = "";
        var EstandarSeguridadAux = "NA";

        var ClasificacionSSO = $("#ClasificacionSSO").val();

        var CatAgrega = localStorage.getItem("CAPEX_PLAN_REQUIERE_CAPACIDAD");
        var CatClase = $("#Clase").val();
        var CatMacroCategoria = localStorage.getItem("CAPEX_CLASIFICA_MACRO");
        var CatAnalisis = localStorage.getItem("CAPEX_PLAN_ANALISIS_BAJA_COMPLEJIDAD");
        var CatArcBc = localStorage.getItem("CAPEX_INICIATIVA_CATEGORIZACION_ARCH_BC_SUBIDO");

        var Categoria = $("#Categoria").val();

        if (CatAnalisis == "" || CatAnalisis != "SI") {
            CatAnalisis = "NO";
        }
        if (CatAnalisis == "NO" || CatAnalisis == "") {
            var CatACNota1 = "0";
            var CatACNota2 = "0";
            var CatACNota3 = "0";
            var CatACNota4 = "0";
            var CatACNota5 = "0";
            var CatACNota6 = "0";
            var TotalAnalisis = "0";
            var CatACObs1 = "";
            var CatACObs2 = "";
            var CatACObs3 = "";
            var CatACObs4 = "";
            var CatACObs5 = "";
            var CatACObs6 = "";
        } else {
            var CatACNota1 = $("#TotalNota1").val();
            var CatACNota2 = $("#TotalNota2").val();
            var CatACNota3 = $("#TotalNota3").val();
            var CatACNota4 = $("#TotalNota4").val();
            var CatACNota5 = $("#TotalNota5").val();
            var CatACNota6 = $("#TotalNota6").val();
            var TotalAnalisis = $("#TotalAnalisis").val();
            var CatACObs1 = $("#NotaInterferencia").val();
            var CatACObs2 = $("#NotaRiesgo").val();
            var CatACObs3 = $("#NotaSustentabilidad").val();
            var CatACObs4 = $("#NotaComplejidad").val();
            var CatACObs5 = $("#NotaEmplazamiento").val();
            var CatACObs6 = $("#NotaSolucion").val();
        }

        if (ClasificacionSSO != "241DB703-0882-4C58-995B-D1247EC0FF8A" && ClasificacionSSO != "42BDF69C-F9BD-4752-9D60-FFE843447318"
            && ClasificacionSSO != "029FA2E7-58FA-4E86-B93D-8ED5A9DA28ED" && ClasificacionSSO != "DCA08674-9F1C-43FF-BE98-C52DA2F0CD62"
            && ClasificacionSSO != "68D47AED-406A-4200-A94D-7BA960E30142") {
            EstandarSeguridad = $("#EstandarSeguridad").val();
            EstandarSeguridadAux = $("#EstandarSeguridad").val();
        }

        if (EstadoProyecto == "-1" || EstadoProyecto == "" || CatMacroCategoria == "" || Categoria == "-1" || Categoria == "" || CatAgrega == "" || CatClase == "-1" || CatClase == "" || ClasificacionSSO == "" | ClasificacionSSO == "-1" || EstandarSeguridadAux == "" || EstandarSeguridadAux == "-1" || NivelIngenieria == "" || NivelIngenieria == "-1") {
            swal("", "Todos los campos son requeridos, favor completar.", "info");
            return false;
        } else if (CatArcBc != "SI" && CatAnalisis == "SI") {
            swal("", "Debe subir archivo de respaldo de baja complejidad", "info");
            return false;
        } else {

            if (EstadoProyecto && EstadoProyecto != undefined && EstadoProyecto == "IMPORTAR") {
                var DTOValidacionIdentificacion = {
                    'PidToken': IniToken
                };
                var responseValidacionIdentificacion = FNRealizarValidacionIniciativaCodigoProyecto(DTOValidacionIdentificacion);
                if (!responseValidacionIdentificacion || responseValidacionIdentificacion == undefined || responseValidacionIdentificacion == "" || responseValidacionIdentificacion == "false") {
                    swal("", "El Código del Proyecto en Identificación es un campo requerido cuando Estado del Proyecto es Remanente.", "info");
                    return false;
                }
            }

            var DTO = {
                "IniToken": IniToken,
                "IniUsuario": IniUsuario,
                "CatEstadoProyecto": EstadoProyecto,
                "CatCategoria": Categoria,
                "CatNivelIngenieria": NivelIngenieria,
                "CatAgrega": CatAgrega,
                "CatTipoCotizacion": TipoCotizacion,
                "CatClasificacionSSO": ClasificacionSSO,
                "CatEstandarSeguridad": EstandarSeguridad,
                "CatClase": CatClase,
                "CatMacroCategoria": CatMacroCategoria,
                "CatAnalisis": CatAnalisis,
                "CatACNota1": CatACNota1,
                "CatACNota2": CatACNota2,
                "CatACNota3": CatACNota3,
                "CatACNota4": CatACNota4,
                "CatACNota5": CatACNota5,
                "CatACNota6": CatACNota6,
                "CatACTotal": TotalAnalisis,
                "CatACObs1": CatACObs1,
                "CatACObs2": CatACObs2,
                "CatACObs3": CatACObs3,
                "CatACObs4": CatACObs4,
                "CatACObs5": CatACObs5,
                "CatACObs6": CatACObs6
            }
            //OPERACION
            $.ajax({
                url: "../Planificacion/GuardarCategorizacion",
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
                        swal("Error", "No es posible almacenar categorización.", "error");
                        return false;
                    } else if (estado == "Guardado") {
                        /*localStorage.setItem("CAPEX_NIVEL_INGENIERIA_ALTERNATIVO_FINAL", "");
                        if (nivelIngenieriaAlternativo) {
                            localStorage.setItem("CAPEX_NIVEL_INGENIERIA_ALTERNATIVO_FINAL", NivelIngenieria);
                        }*/
                        localStorage.setItem("CAPEX_INICIATIVA_CATEGORIZACION_ESTADO", estado);
                        $("#BotonGuardarCategorizacion").prop("disabled", "true");
                        $("#BotonActualizarCategorizacion").show();
                        swal("Exito", "Categorización Guardada.", "success");
                        return false;
                    }
                }
            });
        }
    }
}
//
// ACTUALIZAR CATEGORIZACION
//
FNActualizarCategorizacion = function () {
    var estado = localStorage.getItem("CAPEX_INICIATIVA_CATEGORIZACION_ESTADO");
    var IniToken = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    if (IniToken == "") {
        swal("Error", "Debe identificar una iniciativa para poder actualizar este paso.", "error");
        return false;
    }
    if (!FNCheckFormDinamyc()) {
        return false;
    } else {

        var IniUsuario = $("#CAPEX_H_USERNAME").val();

        var EstadoProyecto = "";
        var estado_proyecto = localStorage.getItem("CAPEX_PLAN_ESTADO_PROYECTO");
        if (estado_proyecto == "") {
            EstadoProyecto = $("#EstadoProyecto1").val();
        }
        else {
            EstadoProyecto = estado_proyecto;
        }

        var Categoria = $("#Categoria").val();
        var TipoCotizacion = $("#TipoCotizacion").val();
        var NivelIngenieria = $("#NivelIngenieria").val();

        var EstandarSeguridad = "";
        var EstandarSeguridadAux = "NA";

        var ClasificacionSSO = $("#ClasificacionSSO").val();
        var CatAgrega = localStorage.getItem("CAPEX_PLAN_REQUIERE_CAPACIDAD");
        var CatClase = $("#Clase").val();
        var CatMacroCategoria = localStorage.getItem("CAPEX_CLASIFICA_MACRO");
        var CatAnalisis = localStorage.getItem("CAPEX_PLAN_ANALISIS_BAJA_COMPLEJIDAD");
        if (CatAnalisis == "" || CatAnalisis != "SI") {
            CatAnalisis = "NO";
        }
        if (CatAnalisis == "NO" || CatAnalisis == "") {
            var CatACNota1 = "0";
            var CatACNota2 = "0";
            var CatACNota3 = "0";
            var CatACNota4 = "0";
            var CatACNota5 = "0";
            var CatACNota6 = "0";
            var TotalAnalisis = "0";
            var CatACObs1 = "";
            var CatACObs2 = "";
            var CatACObs3 = "";
            var CatACObs4 = "";
            var CatACObs5 = "";
            var CatACObs6 = "";
        } else {
            var CatACNota1 = $("#TotalNota1").val();
            var CatACNota2 = $("#TotalNota2").val();
            var CatACNota3 = $("#TotalNota3").val();
            var CatACNota4 = $("#TotalNota4").val();
            var CatACNota5 = $("#TotalNota5").val();
            var CatACNota6 = $("#TotalNota6").val();
            var TotalAnalisis = $("#TotalAnalisis").val();
            var CatACObs1 = $("#NotaInterferencia").val();
            var CatACObs2 = $("#NotaRiesgo").val();
            var CatACObs3 = $("#NotaSustentabilidad").val();
            var CatACObs4 = $("#NotaComplejidad").val();
            var CatACObs5 = $("#NotaEmplazamiento").val();
            var CatACObs6 = $("#NotaSolucion").val();
        }

        if (ClasificacionSSO != "241DB703-0882-4C58-995B-D1247EC0FF8A" && ClasificacionSSO != "42BDF69C-F9BD-4752-9D60-FFE843447318"
            && ClasificacionSSO != "029FA2E7-58FA-4E86-B93D-8ED5A9DA28ED" && ClasificacionSSO != "DCA08674-9F1C-43FF-BE98-C52DA2F0CD62"
            && ClasificacionSSO != "68D47AED-406A-4200-A94D-7BA960E30142") {
            EstandarSeguridad = $("#EstandarSeguridad").val();
            EstandarSeguridadAux = $("#EstandarSeguridad").val();
        }

        if (EstadoProyecto == "-1" || EstadoProyecto == "" || CatMacroCategoria == "" || Categoria == "-1" || Categoria == "" || CatAgrega == "" || CatClase == "-1" || CatClase == "" || ClasificacionSSO == "" || ClasificacionSSO == "-1" || EstandarSeguridadAux == "" || EstandarSeguridadAux == "-1" || NivelIngenieria == "" || NivelIngenieria == "-1") {
            swal("", "Todos los campos son requeridos.", "info");
            return false;
        }
        else {

            if (EstadoProyecto && EstadoProyecto != undefined && EstadoProyecto == "IMPORTAR") {
                var DTOValidacionIdentificacion = {
                    'PidToken': IniToken
                };
                var responseValidacionIdentificacion = FNRealizarValidacionIniciativaCodigoProyecto(DTOValidacionIdentificacion);
                if (!responseValidacionIdentificacion || responseValidacionIdentificacion == undefined || responseValidacionIdentificacion == "" || responseValidacionIdentificacion == "false") {
                    swal("", "El Código del Proyecto en Identificación es un campo requerido cuando Estado del Proyecto es Remanente.", "info");
                    return false;
                }
            }

            var DTO = {
                "IniToken": IniToken,
                "IniUsuario": IniUsuario,
                "CatEstadoProyecto": EstadoProyecto,
                "CatCategoria": Categoria,
                "CatNivelIngenieria": NivelIngenieria,
                "CatAgrega": CatAgrega,
                "CatTipoCotizacion": TipoCotizacion,
                "CatClasificacionSSO": ClasificacionSSO,
                "CatEstandarSeguridad": EstandarSeguridad,
                "CatClase": CatClase,
                "CatMacroCategoria": CatMacroCategoria,
                "CatAnalisis": CatAnalisis,
                "CatACNota1": CatACNota1,
                "CatACNota2": CatACNota2,
                "CatACNota3": CatACNota3,
                "CatACNota4": CatACNota4,
                "CatACNota5": CatACNota5,
                "CatACNota6": CatACNota6,
                "CatACTotal": TotalAnalisis,
                "CatACObs1": CatACObs1,
                "CatACObs2": CatACObs2,
                "CatACObs3": CatACObs3,
                "CatACObs4": CatACObs4,
                "CatACObs5": CatACObs5,
                "CatACObs6": CatACObs6
            }
            //OPERACION
            $.ajax({
                url: "../Planificacion/ActualizarCategorizacion",
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
                        swal("Error", "No es posible actualizar esta categorización.", "error");
                        return false;
                    }
                    else if (estado == "Actualizado") {
                        /*localStorage.setItem("CAPEX_NIVEL_INGENIERIA_ALTERNATIVO_FINAL", "");
                        if (nivelIngenieriaAlternativo) {
                            localStorage.setItem("CAPEX_NIVEL_INGENIERIA_ALTERNATIVO_FINAL", NivelIngenieria);
                        }*/
                        localStorage.setItem("CAPEX_INICIATIVA_CATEGORIZACION_ESTADO", estado);
                        swal("Exito", "Categorización Actualizada.", "success");
                        return false;
                    }
                }
            });
        }
    }
}

FNRealizarValidacionIniciativaCodigoProyecto = function (DTO) {
    var response = '';
    $.ajax({
        url: "../Planificacion/ValidarCodigoProyectoRemanenteCategorizacion",
        type: "POST",
        async: false,
        dataType: "json",
        data: (DTO),
        success: function (r) {
            if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                document.getElementById('linkToLogout').click();
                return;
            }
            if (r && r.Mensaje) {
                response = r.Mensaje;
            }
        }
    });
    return response;
}


FNPreviousCallBackIngresoCategorizacionCheckToken = function () {
    var iniciativa_token = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    if (iniciativa_token == null || iniciativa_token == "") {
        swal("", "Debe identificar iniciativa.", "info");
        return false;
    }
    return true;
}

FNPreviousCallBackIngresoCategorizacion = function () {
    console.log("FNPreviousCallBackIngresoCategorizacion");
    jQuery("#AppLoaderContainer").show();
}

FNCallBackIngresoCategorizacion = function (paramJson) {
    jQuery("#AppLoaderContainer").hide();
    if (paramJson.Data.code == "0") {
        swal("Exito", "Archivo de desarrollo subido correctamente", "success");
        FNPoblarDocumentos();
    } else {
        swal("Error", "No es posible guardar el archivo de desarrollo", "error");
    }
}

//
// ARCHIVO DE DESARROLLO /RESPADO DE INGENIERIA
//
/*document.getElementById('form_categorizacion_desarrollo').onsubmit = function (e) {
    e.preventDefault();
    var iniciativa_token = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    if (iniciativa_token == null || iniciativa_token == "") {
        swal("", "Debe identificar iniciativa.", "info");
        return false;
    }
    else {

        if (document.getElementById("ArchivoDesarrollo").files.length == 0) {
            return false;
        }
        else
        {
            var formdata = new FormData();
            var fileInput = document.getElementById('ArchivoDesarrollo');
            var nombreArchivo = fileInput.files[0].name;
            $("#NombreArchivoDesarrollo").html(nombreArchivo);
            var tamano = fileInput.files[0].size;

            var extension = nombreArchivo.substr((nombreArchivo.lastIndexOf('.') + 1));
            formdata.append(fileInput.files[0].name, fileInput.files[0]);

            jQuery("#AppLoaderContainer").show();
            var ajaxRequest = $.ajax({
                type: "POST",
                url: "../Planificacion/SubirArchivoDesarrollo",
                contentType: false,
                processData: false,
                data: formdata
            });

            ajaxRequest.done(function (xhr, textStatus) {
                var parusuario = $("#CAPEX_H_USERNAME").val();
                var DTO = { "IniToken": iniciativa_token, "ParUsuario": parusuario, "ParNombre": nombreArchivo, "ParPaso": "Categorizacion", "ParCaso": "Desarrollo Ingenieria" };
                $.ajax({
                    url: "../Planificacion/RegistrarArchivo",
                    type: "GET",
                    dataType: "json",
                    data: (DTO),
                    success: function (r) {
                        jQuery("#AppLoaderContainer").hide();
                        swal("Exito", "Archivo de desarrollo subido correctamente", "success");
                        FNPoblarDocumentos();
                    },
                    error: function (xhr, error, status) {
                        jQuery("#AppLoaderContainer").hide();
                        swal("Exito", "Archivo de desarrollo subido correctamente", "success");
                        return false;
                    }
                });
            });
        }
    }
}*/
//
// ARCHIVO DE BAJA COMPLEJIDAD
//
document.getElementById('form_categorizacion_baja_complejidad').onsubmit = function (e) {
    e.preventDefault();
    var iniciativa_token = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    if (iniciativa_token == null || iniciativa_token == "") {
        swal("", "Debe identificar iniciativa.", "info");
        return false;
    }
    else {

        if (document.getElementById("ArchivoBajaComplejidad").files.length == 0) {
            return false;
        }
        else {
            var formdata = new FormData();
            var fileInput = document.getElementById('ArchivoBajaComplejidad');
            var nombreArchivo = fileInput.files[0].name;
            $("#NombreArchivoBajaComplejidad").html(nombreArchivo);
            var tamano = fileInput.files[0].size;
            var extension = nombreArchivo.substr((nombreArchivo.lastIndexOf('.') + 1));
            formdata.append(fileInput.files[0].name, fileInput.files[0]);

            jQuery("#AppLoaderContainer").show();
            var ajaxRequest = $.ajax({
                type: "POST",
                url: "../Planificacion/SubirArchivoBajaComplejidad",
                contentType: false,
                processData: false,
                data: formdata
            });

            ajaxRequest.done(function (xhr, textStatus) {
                var parusuario = $("#CAPEX_H_USERNAME").val();
                var DTO = { "IniToken": iniciativa_token, "ParUsuario": parusuario, "ParNombre": nombreArchivo, "ParPaso": "Categorizacion", "ParCaso": "Desarrollo Baja Complejidad" };
                $.ajax({
                    url: "../Planificacion/RegistrarArchivo",
                    type: "GET",
                    dataType: "json",
                    data: (DTO),
                    success: function (r) {
                        if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                            document.getElementById('linkToLogout').click();
                            return;
                        }
                        jQuery("#AppLoaderContainer").hide();
                        swal("Exito", "Archivo de desarrollo subido correctamente", "success");
                        localStorage.setItem("CAPEX_INICIATIVA_CATEGORIZACION_ARCH_BC_SUBIDO", "SI");
                        FNPoblarDocumentos();
                    },
                    error: function (xhr, error, status) {
                        jQuery("#AppLoaderContainer").hide();
                        swal("Exito", "Archivo de desarrollo subido correctamente", "success");
                        localStorage.setItem("CAPEX_INICIATIVA_CATEGORIZACION_ARCH_BC_SUBIDO", "NO");
                        return false;
                    }
                });
            });

        }
    }
}
//
// ACTUALIZAR ETAPA
//
FNActualizarEtapaAlmacenada = function () {
    //PREPARAR
    var token = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    var etapa = localStorage.getItem("CAPEX_PLAN_ETAPA")
    //LISTO
    var DTO = {
        "token": token, "etapa": etapa
    };
    var cuantos = 0
    $.ajax({
        url: "../Planificacion/ActualizarEtapa",
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
                swal("Error", "No es posible actualizar 'etapa' de iniciativa.", "error");
                return false;
            }
        }
    });
}
//
// VER MODAL GRADO COMPLEJIDAD
//
FNMostrarModalGradoComplejidad = function () {
    $("#ModalGradoComplejidad").show();
}
//
// CERRAR MODAL GRADO BAJA COMPLEJIDAD
//
FNCerrarModalBajaComplejidad = function () {
    $("#ModalGradoComplejidad").hide();
}

