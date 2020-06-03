FNEvaluarRequerimientoNivelIngeneria = function () {
    var categoria = $('#Categoria').val();
    var superIntendencia = $('#Superitendencia').val();
    if ((categoria && categoria != undefined && categoria == "DC77FD26-10C4-4B09-83DC-04E926162A72")
        || (superIntendencia && superIntendencia != undefined && superIntendencia == "42")) {
        return true;
    }
    return false;
}

FNMostrarDefaultNivelIngenieria = function (resetNivelIngenieria) {
    if (resetNivelIngenieria) {
        $('#NivelIngenieria').val("-1");
    }
    $('#ContenedorNivelIngenieria1').show();
    $('#ContenedorNivelIngenieria2').hide();
}

FNObtenerSuperIntendenciaPorGerencia = function (valor) {
    var Superitendencia = $('#Superitendencia');
    Superitendencia.empty();
    if (valor == "-1") {
        Superitendencia.append('<option value="-1" selected="true">Seleccionar..</option>')
        return;
    }
    Superitendencia.append('<option selected="true">Buscando..</option>');
    //LISTO
    var DTO = { GerToken: valor };
    $.ajax({
        url: "../Planificacion/ListarSuperintendenciasPorGerencia",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            Superitendencia.empty();
            setTimeout(function () {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                Superitendencia.append('<option value="-1" selected="true">Seleccionar..</option>')
                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    Superitendencia.append(new Option(value.SuperNombre, value.CodigoSuper, false, false));
                });
            }, 500);
        }
    });
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
// SECCION          : IDENTIFICACION 
// METODOS          :
//
//
// GUARDAR SELECCION
//
FNGuardarSeleccion = function (tipo, valor) {
    switch (tipo) {
        case "COMBO_PROCESO": localStorage.setItem("CAPEX_PLAN_PROCESO", valor); break;
        case "OBJETO": localStorage.setItem("CAPEX_PLAN_OBJETO", valor); break;
        case "COMBO_AREA": localStorage.setItem("CAPEX_PLAN_AREA", valor); break;
        case "COMBO_COMPANIA": localStorage.setItem("CAPEX_PLAN_COMPANIA", valor); break;
        case "COMBO_ETAPA":
            /**
             * REGLA DE NEGOCIO 2.2
             * RN002.1
             * CONTROL DE CAMBIO 20-02-2019 : SOPORTAR DOS ETAPAS PARA CONFIGURACION
             * DE RESPONSABLE DE EJECUCION AUTOMATICO Y RESET RESPUESTA PREGUNTA
             */
            $("#requiere1").prop('checked', false);
            $("#requiere2").prop('checked', false);
            $("#GerenciaEjecucion option:last").prop("selected", "selected");
            var Superitendencia = $('#Superitendencia');
            Superitendencia.empty();
            Superitendencia.append('<option value="-1" selected="true">Seleccionar..</option>');
            $("#GerenteEjecucion").val("");
            $("#SuperIntendente").val("");
            $("#EncargadoControl").val("");
            $('#Encargado').val('');
            localStorage.setItem("CAPEX_PLAN_SUPERINTENDENCIA", "");
            localStorage.setItem("CAPEX_PLAN_ETAPA", valor);
            localStorage.setItem("CAPEX_PLAN_REQUIERE_EJECUCION", "");
            /**
             * ACTUAIZAR ETAPA EN CASO DE QUE LA 
             * INICIATIVA HAYA SIDO GUARDADA
             * 
             * */
            var token_iniciativa = localStorage.getItem("CAPEX_INICIATIVA_TOKEN")
            if (token_iniciativa != "" || token_iniciativa != "") {
                FNActualizarEtapaAlmacenada();
            }
            break;
        case "COMBO_GERENCIAINVERSION": localStorage.setItem("CAPEX_PLAN_GERENCIAINVERSION", valor); break;
        case "COMBO_GERENCIAEJECUCION":
            localStorage.setItem("CAPEX_PLAN_GERENCIAEJECUCION", valor);
            localStorage.setItem("CAPEX_PLAN_SUPERINTENDENCIA", "");
            FNObtenerGerenteEjecucion(valor);
            FNObtenerSuperIntendenciaPorGerencia(valor);
            $("#SuperIntendente").val("");
            $("#Encargado").val("");
            break;
        case "COMBO_SUPERINTENDENCIA":
            $("#SuperIntendente").val("");
            $("#Encargado").val("");
            console.log("tipo=" + tipo + ", valor=" + valor);
            localStorage.setItem("CAPEX_PLAN_SUPERINTENDENCIA", valor); break;
        case "COMBO_ESTADO_PROYECTO":
            localStorage.setItem("CAPEX_PLAN_ESTADO_PROYECTO", valor);
            if (valor == "IMPORTAR") {
                var codigoProyecto = $("#CodigoProyecto").val();
                if (!codigoProyecto || codigoProyecto == undefined || codigoProyecto.trim() == "") {
                    swal("", "Debe ingresar Código de Proyecto en Identificación.", "info");
                }
            }
            break;
        case "COMBO_SSO":
            localStorage.setItem("CAPEX_PLAN_SSO", valor);
            FNObtenerEstandarSeguridad();
            break;
        case "RADIO_REQUIERE_EJECUCION":
            /**
           * REGLA DE NEGOCIO 2
           * RN002
           */
            localStorage.setItem("CAPEX_PLAN_REQUIERE_EJECUCION", valor);
            var etapa = localStorage.getItem("CAPEX_PLAN_ETAPA");
            /**
             * REGLA DE NEGOCIO 2.1
             * RN002.1
             * CONTROL DE CAMBIO 20-02-2019 : SOPORTAR DOS ETAPAS PARA CONFIGURACION
             * DE RESPONSABLE DE EJECUCION AUTOMATICO Y RESET EN CASO DE SELECCIONAR "NO" COMO RESPUESTA
             */
            if (valor == "NO") {
                $("#GerenciaEjecucion option:last").prop("selected", "selected");
                $("#Superitendencia option:last").prop("selected", "selected");
                var Superitendencia = $('#Superitendencia');
                Superitendencia.empty();
                Superitendencia.append('<option value="-1" selected="true">Seleccionar..</option>');
                localStorage.setItem("CAPEX_PLAN_GERENCIAEJECUCION", "");
                $("#GerenteEjecucion").val("");
                $("#SuperIntendente").val("");
                $("#Encargado").val("");
                return false;
            } else if (valor == "SI" && (etapa == "FACT" || etapa == "EJEC")) {

                /**
                 * REGLA DE NEGOCIO 2
                 * RN002
                 */
                //$('#GerenciaEjecucion>option:eq(4)').prop('selected', true);

                var valueSelectedGcia = "-1";
                $("#GerenciaEjecucion > option").each(function () {
                    //console.log("RADIO_REQUIERE_EJECUCION this.text=" + this.text + ', this.value=' + this.value);
                    if ("Gerencia Proyectos" == this.text) {
                        valueSelectedGcia = this.value;
                        return false;
                    }
                });

                if (valueSelectedGcia != "-1") {
                    $("#GerenciaEjecucion").val(valueSelectedGcia);
                    localStorage.setItem("CAPEX_PLAN_GERENCIAEJECUCION", valueSelectedGcia);
                    $('#form_identificacion').valid();
                }
                $("#SuperIntendente").val("");
                $("#Encargado").val("");
                var token = $("#GerenciaEjecucion").val();
                //CAPEX_GERENTE_EJECUCION_NOMBRE
                //CAPEX_GERENTE_EJECUCION_TOKEN
                FNObtenerGerenteEjecucion(token);
                FNObtenerSuperIntendenciaPorGerencia(token);
                return;
            }
            break;
        case "RADIO_REQUIERE_CAPACIDAD":
            localStorage.setItem("CAPEX_PLAN_REQUIERE_CAPACIDAD", valor);
            /**
             * REGLA DE NEGOCIO 3
             * RN003
             */
            if (valor == "SI") {
                //localStorage.setItem("CAPEX_NIVEL_INGENIERIA_ALTERNATIVO", "");
                $('#ContenedorMacro2').hide();
                $('#ContenedorMacro1').show();
                localStorage.setItem("CAPEX_CLASIFICA_MACRO", "DEVELOPMENT");
                FNObtenerListadoCategorias("DEVELOPMENT");
                //FNMostrarDefaultNivelIngenieria(false);
                FNObtenerListadoNivelIngenieraNoRequiereCorregido(false);
                return;
            } else if (valor == "NO") {
                $('#ContenedorMacro1').hide();
                $('#ContenedorMacro2').show();
                localStorage.setItem("CAPEX_CLASIFICA_MACRO", "SUSTAINING");
                FNObtenerListadoCategorias("SUSTAINING");
                return;
            }
            break;
        case "COMBO_ALIMENTACION":
            localStorage.setItem("CAPEX_DOTACION_HOTELERIA", valor);
            break;

        case "COMBO_HOTELERIA":
            localStorage.setItem("CAPEX_DOTACION_ALIMENTACION", valor);
            break;

    }
    return;
}
//
// GENERAR NOMBRE DE PROYECTO
//
FNResolverNombreProyecto = function () {
    var t = setInterval(FNContruirNombreProyecto, 1500);
}

//
//VERIFICAR SI SE ESTA LLENANDO EL FORMULARIO
//
FNVerificarIngresoInfoEtapaIdentificacion = function (paso) {

    switch (paso) {
        case 'IDENTIFICACION':
            var estado = localStorage.getItem("CAPEX_INICIATIVA_ESTADO");
            var ee1 = $("#NombreProyecto").val();
            var ee2 = $("#CodigoProyecto").val();
            var ee3 = $("#GerenteInversion").val();
            if (estado != "Guardado") {
                if ((ee1 != "///CEN/" && ee1 != "") || ee2 != "" || ee3 != "") {
                    swal("", "No olvide completar el formulario y 'GUARDAR', de lo contrario perderá todos los cambios.", "warning");
                }
            }
            break;

        case 'CATEGORIZACION':
            var estado = localStorage.getItem("CAPEX_INICIATIVA_CATEGORIZACION_ESTADO");
            var ee1 = $("#TipoCotizacion").val();
            var ee2 = $("#Categoria").val();
            var ee3 = $("#ClasificacionSSO").val();
            if (estado != "Guardado") {
                if (ee1 != "" || ee2 != "" || ee3 != "") {
                    swal("", "No olvide completar el formulario y 'GUARDAR', de lo contrario perderá todos los cambios.", "warning");
                }
            }
            break;
        case 'PRESUPUESTO':
            var estado = localStorage.getItem("CAPEX_INICIATIVA_CATEGORIZACION_ESTADO");
            var ee1 = $("#TipoCotizacion").val();
            var ee2 = $("#Categoria").val();
            var ee3 = $("#ClasificacionSSO").val();
            if (estado != "Guardado") {
                if (ee1 != "" || ee2 != "" || ee3 != "") {
                    swal("", "No olvide completar el formulario y 'GUARDAR', de lo contrario perderá todos los cambios.", "warning");
                }
            }
            break;
    }
}

//
// ARMAR NOMBRE DE PROYECTO
//
FNContruirNombreProyecto = function () {
    /**
    * REGLA DE NEGOCIO 1
    * RN001
    */
    var proceso = localStorage.getItem("CAPEX_PLAN_PROCESO");
    var objeto = localStorage.getItem("CAPEX_PLAN_OBJETO");
    var area = localStorage.getItem("CAPEX_PLAN_AREA");
    var compania = localStorage.getItem("CAPEX_PLAN_COMPANIA");
    var etapa = localStorage.getItem("CAPEX_PLAN_ETAPA");
    var tipo = localStorage.getItem("CAPEX_TIPO_INICIATIVA");
    var periodo = localStorage.getItem("CAPEX_PERIODO_EP");
    var actual = periodo.toString().substr(-2);
    var siguiente = parseInt(actual) + 1;
    $("#NombreProyecto").val(proceso + "/" + objeto + "/" + area + "/" + compania + "/" + etapa);
    var folioIniciativaGuardado = localStorage.getItem("CAPEX_INICIATIVA_FOLIO");
    if (!folioIniciativaGuardado || folioIniciativaGuardado == undefined) {
        folioIniciativaGuardado = "";
    }

    if (tipo == "IMPORTAR") {
        $("#CodigoIniciativa").val(tipo + siguiente + compania + area + etapa + ((folioIniciativaGuardado == "") ? "" : ("-" + folioIniciativaGuardado)));
    }
    else {
        $("#CodigoIniciativa").val(tipo + actual + compania + area + etapa + ((folioIniciativaGuardado == "") ? "" : ("-" + folioIniciativaGuardado)));
    }

    var nomproy = $("#NombreProyecto").val();
    if (nomproy == "" || nomproy == null) {
        localStorage.setItem("CAPEX_PLAN_NOMBRE_PROYECTO", nomproy);
    }
}
//
// CARGAR ESTADO DEL PROYECTO
//
FNResolverEstadoDelProyecto = function () {
    var estado = localStorage.getItem("CAPEX_TIPO_EJERCICIO");
    var tipo = localStorage.getItem("CAPEX_TIPO_INICIATIVA");

    if ((tipo == "CB" || tipo == "CD") && (estado == "CREAR" || estado == "IMPORTAR")) {
        $("#ContenedorEstadoNoSeleccionable").hide();
        $("#ContenedorEstadoSeleccionable2").hide();
        $("#ContenedorEstadoSeleccionable1").show();
    }
    else if ((tipo == "PP") && (estado == "CREAR" || estado == "IMPORTAR")) {
        $("#ContenedorEstadoNoSeleccionable").hide();
        $("#ContenedorEstadoSeleccionable2").show();
        $("#ContenedorEstadoSeleccionable1").hide();
    }
    else if ((tipo == "EX") && (estado == "CREAR" || estado == "IMPORTAR")) {
        $("#ContenedorEstadoNoSeleccionable").show();
        $("#ContenedorEstadoSeleccionable2").hide();
        $("#ContenedorEstadoSeleccionable1").hide();
        $("#EstadoProyecto1").val("EXTRAORDINARIO");
    }
    return;
}
//
// LISTADO DE PROCESOS
//
FNObtenerListadoProcesos = function () {
    //PREPARAR
    var Proceso = $('#Proceso');
    Proceso.empty();
    Proceso.append('<option selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../Planificacion/ListarProcesos",
        cache: true,
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            Proceso.empty();
            setTimeout(function () {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    $('#Proceso').append(new Option(value.ProcNombre, value.ProcAcronimo, false, false));
                    cuantos++;
                });
                if (cuantos == 1) {
                    Proceso.prop('selectedIndex', 0);
                }
                else if (cuantos > 1) {
                    Proceso.append('<option value="-1" selected="true">Seleccionar..</option>')
                }
            }, 500);
        }
    });
}
//
// LISTADO DE AREAS
//
FNObtenerListadoAreas = function () {
    //PREPARAR
    var Area = $('#Area');
    Area.empty();
    Area.append('<option selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../Planificacion/ListarAreas",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            Area.empty();
            setTimeout(function () {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    $('#Area').append(new Option(value.AreaNombre, value.AreaAcronimo, false, false));
                    cuantos++;
                });
                if (cuantos == 1) {
                    Area.prop('selectedIndex', 0);
                }
                else if (cuantos > 1) {
                    Area.append('<option value="-1" selected="true">Seleccionar..</option>')
                }
            }, 500);
        }
    });
}
//
// LISTADO DE COMPANIAS
//
FNObtenerListadoCompanias = function () {
    //PREPARAR
    var Compania = $('#Compania');
    Compania.empty();
    Compania.append('<option selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../Planificacion/ListarCompanias",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            Compania.empty();
            setTimeout(function () {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    if (value.ComAcronimo == "CEN") {
                        $('#Compania').append(new Option(value.ComNombre, value.ComAcronimo, false, true));
                        localStorage.setItem("CAPEX_PLAN_COMPANIA", value.ComAcronimo);
                        FNObtenerTokenCompania(value.ComAcronimo);
                    }
                    else {
                        $('#Compania').append(new Option(value.ComNombre, value.ComAcronimo, false, false));
                    }
                    cuantos++;
                });
                //if (cuantos == 1) {
                //    Compania.prop('selectedIndex', 0);
                //}
                //else if (cuantos > 1) {
                //    Compania.append('<option selected="true">Compania..</option>')
                //}
            }, 500);
        }
    });
}
//
// LISTADO DE ETAPAS
//
FNObtenerListadoEtapas = function () {
    //PREPARAR
    var Etapa = $('#Etapa');
    Etapa.empty();
    Etapa.append('<option selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../Planificacion/ListarEtapas",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            Etapa.empty();
            setTimeout(function () {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    $('#Etapa').append(new Option(value.NINombre, value.NIAcronimo, false, false));
                    cuantos++;
                });
                if (cuantos == 1) {
                    Etapa.prop('selectedIndex', 0);
                }
                else if (cuantos > 1) {
                    Etapa.append('<option value="-1" selected="true">Seleccionar..</option>')
                }
            }, 500);
        }
    });
}
//
// LISTADO DE GERENCIAS
//
FNObtenerListadoGerencia = function () {
    //PREPARAR
    var GerenciaInversion = $('#GerenciaInversion');
    GerenciaInversion.empty();
    GerenciaInversion.append('<option value="-1" selected="true">Buscando..</option>');

    var GerenciaEjecucion = $('#GerenciaEjecucion');
    GerenciaEjecucion.empty();
    GerenciaEjecucion.append('<option value="-1" selected="true">Buscando..</option>');

    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../Planificacion/ListarGerencias",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                document.getElementById('linkToLogout').click();
                return;
            }
            GerenciaInversion.empty();
            GerenciaEjecucion.empty();
            setTimeout(function () {
                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    $('#GerenciaInversion').append(new Option(value.GerNombre, value.GerToken, false, false));
                    cuantos++;
                });

                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    $('#GerenciaEjecucion').append(new Option(value.GerNombre, value.GerToken, false, false));
                    cuantos++;
                });

                if (cuantos == 1) {
                    GerenciaInversion.prop('selectedIndex', 0);
                }
                else if (cuantos > 1) {
                    GerenciaInversion.append('<option value="-1" selected="true">Seleccionar..</option>')
                }

                if (cuantos == 1) {
                    GerenciaEjecucion.prop('selectedIndex', 0);
                }
                else if (cuantos > 1) {
                    GerenciaEjecucion.append('<option value="-1" selected="true">Seleccionar..</option>')
                }

            }, 500);
        }
    });
}
//
// OBTENER INFORMACION GERENTE INVERSION
//
FNObtenerGerenteInversion = function (token) {
    //PREPARAR
    var param_token = token;
    //LISTO
    var DTO = { 'token': param_token };
    $.ajax({
        url: "../Planificacion/ObtenerGerente",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                document.getElementById('linkToLogout').click();
                return;
            }
            $("#GerenteInversion").val("");
            localStorage.setItem("CAPEX_GERENTE_INVERSION_NOMBRE", "");
            localStorage.setItem("CAPEX_GERENTE_INVERSION_TOKEN", "");
            $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                if (key == "GteNombre") {
                    $("#GerenteInversion").val(value);
                    localStorage.setItem("CAPEX_GERENTE_INVERSION_NOMBRE", value);
                }
                if (key == "GteToken") {
                    localStorage.setItem("CAPEX_GERENTE_INVERSION_TOKEN", value);
                }
            });
        }
    });
}
//
// OBTENER INFORMACION GERENTE EJECUCION
//
FNObtenerGerenteEjecucion = function (token) {
    if (token == "-1") {
        localStorage.setItem("CAPEX_GERENTE_EJECUCION_NOMBRE", "");
        localStorage.setItem("CAPEX_GERENTE_EJECUCION_TOKEN", "");
        localStorage.setItem("CAPEX_PLAN_GERENCIA_ID", "");
        return;
    }
    //PREPARAR
    var param_token = token;
    //LISTO
    var DTO = { 'token': param_token };
    $.ajax({
        url: "../Planificacion/ObtenerGerente",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                document.getElementById('linkToLogout').click();
                return;
            }
            $("#GerenteEjecucion").val("");
            localStorage.setItem("CAPEX_GERENTE_EJECUCION_NOMBRE", "");
            localStorage.setItem("CAPEX_GERENTE_EJECUCION_TOKEN", "");
            localStorage.setItem("CAPEX_PLAN_GERENCIA_ID", "");
            $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                console.log("FNObtenerGerenteEjecucion key=" + key + ", value=" + value);
                if (key == "GteNombre") {
                    console.log("FNObtenerGerenteEjecucion GteNombre key=" + key + ", value=" + value);
                    $("#GerenteEjecucion").val(value);
                    localStorage.setItem("CAPEX_GERENTE_EJECUCION_NOMBRE", value);
                }
                if (key == "GteToken") {
                    console.log("FNObtenerGerenteEjecucion GteToken key=" + key + ", value=" + value);
                    localStorage.setItem("CAPEX_GERENTE_EJECUCION_TOKEN", value);
                }
                if (key == "IdGerencia") {
                    console.log("FNObtenerGerenteEjecucion IdGerencia key=" + key + ", value=" + value);
                    localStorage.setItem("CAPEX_PLAN_GERENCIA_ID", value);
                    // FNObtenerListadoSuperintendencias(value);
                }
            });
        }
    });
}
//
// LISTADO DE SUPERINTENDENCIAS
//
FNObtenerListadoSuperintendencias = function (IdGerencia) {
    if (IdGerencia != null || IdGerencia != "") {
        //PREPARAR
        var Superitendencia = $('#Superitendencia');
        Superitendencia.empty();
        Superitendencia.append('<option selected="true">Buscando..</option>');
        //LISTO
        var DTO = {};
        var cuantos = 0;
        var encontrado = 0;
        $.ajax({
            url: "../Planificacion/ListarSuperintendencias",
            type: "GET",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                Superitendencia.empty();
                setTimeout(function () {
                    if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                        document.getElementById('linkToLogout').click();
                        return;
                    }
                    Superitendencia.append('<option value="-1" selected="true">Seleccionar..</option>')
                    $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                        if (parseInt(value.IdGerencia) === parseInt(IdGerencia)) {
                            Superitendencia.append(new Option(value.SuperNombre, value.CodigoSuper, false, true));
                            encontrado = 1;
                        } else {
                            Superitendencia.append(new Option(value.SuperNombre, value.CodigoSuper, false, false));
                        }
                        cuantos++;
                    });
                    if (!encontrado) {
                        // Superitendencia.prop('selectedIndex', 0);
                        $("#Superitendencia").val($("#Superitendencia option:first").val());
                    }
                    /*if (cuantos == 1) {
                        Superitendencia.prop('selectedIndex', 0);
                    }
                    else if (cuantos > 1) {
                        Superitendencia.append('<option selected="true">Superitendencia..</option>')
                    }*/
                }, 500);
            }
        });
    }
    else {
        return true;
    }
}
//
// OBTENER INFORMACION SUPERINTENDENTE
//
FNObtenerIntendente = function (token) {
    //PREPARAR
    var param_token = token;
    //LISTO
    var DTO = { 'token': param_token };
    $.ajax({
        url: "../Planificacion/ObtenerIntendente",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                document.getElementById('linkToLogout').click();
                return;
            }
            localStorage.setItem("CAPEX_INTENDENTE_NOMBRE", "");
            localStorage.setItem("CAPEX_INTENDENTE_TOKEN", "");
            $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                if (key == "IntNombre") {
                    $("#SuperIntendente").val(value);
                    localStorage.setItem("CAPEX_INTENDENTE_NOMBRE", value);
                }
                if (key == "IntToken") {
                    localStorage.setItem("CAPEX_INTENDENTE_TOKEN", value);
                }
            });
            var gerencia = localStorage.getItem("CAPEX_PLAN_GERENCIA_ID");
            FNObtenerEncargado(gerencia, token);
        }
    });
}
//
// OBTENER INFORMACION ENCARGADO
//
FNObtenerEncargado = function (gerencia, superintendencia) {
    if (!gerencia || gerencia == undefined || gerencia == "" || gerencia == "-1" || gerencia == 0 || gerencia == -1) {
        localStorage.setItem("CAPEX_ENCARGADO_NOMBRE", "");
        localStorage.setItem("CAPEX_ENCARGADO_TOKEN", "");
        return;
    }
    console.log("FNObtenerEncargado gerencia=" + gerencia + ", superintendencia=" + superintendencia);
    var DTO = { 'IdGerencia': gerencia, 'CodigoSuper': superintendencia };
    $.ajax({
        url: "../Planificacion/ObtenerEncargado",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                document.getElementById('linkToLogout').click();
                return;
            }
            localStorage.setItem("CAPEX_ENCARGADO_NOMBRE", "");
            localStorage.setItem("CAPEX_ENCARGADO_TOKEN", "");
            $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                console.log("FNObtenerEncargado key=" + key + ", value=" + value);
                if (key == "EncNombre") {
                    console.log("FNObtenerEncargado EncNombre key=" + key + ", value=" + value);
                    $("#Encargado").val(value);
                    localStorage.setItem("CAPEX_ENCARGADO_NOMBRE", value);
                }
                if (key == "EncToken") {
                    console.log("FNObtenerEncargado EncToken key=" + key + ", value=" + value);
                    localStorage.setItem("CAPEX_ENCARGADO_TOKEN", value);
                }
            });
        }
    });
}

//
// GUARDAR IDENTIFICACION
//

FNGuardarIdentificacion = function () {
    if (!$('#form_identificacion').valid()) {
        console.log("validacion incorrecta");
        return false;
    }
    console.log("validacion correcta");

    /*********************************** PARAMETROS BASE ***********************************/
    var estado = localStorage.getItem("CAPEX_INICATIVA_ESTADO");

    var PidTipoIniciativa = localStorage.getItem("CAPEX_TIPO_INICIATIVA");
    var PidTipoEjercicio = localStorage.getItem("CAPEX_TIPO_EJERCICIO");
    var PidPeriodo = localStorage.getItem("CAPEX_PERIODO_EP");
    var PidUsuario = $("#CAPEX_H_USERNAME").val();

    /*********************************** PARAMETROS ENTRADA**********************************/
    var PidProceso = localStorage.getItem("CAPEX_PLAN_PROCESO");
    var PidObjeto = localStorage.getItem("CAPEX_PLAN_OBJETO");
    var PidArea = localStorage.getItem("CAPEX_PLAN_AREA");
    var PidCompania = localStorage.getItem("CAPEX_PLAN_COMPANIA");
    var PidEtapa = localStorage.getItem("CAPEX_PLAN_ETAPA");
    var PidNombreProyecto = $("#NombreProyecto").val();

    var PidCodigoIniciativa = $("#CodigoIniciativa").val();
    var PidCodigoProyecto = $("#CodigoProyecto").val();

    if ("____-_______" == PidCodigoProyecto) {
        PidCodigoProyecto = "";
    }
    PidCodigoProyecto = PidCodigoProyecto.replace('_', '');
    if ("-" == PidCodigoProyecto) {
        PidCodigoProyecto = "";
    }

    var PidGerenciaInversion = localStorage.getItem("CAPEX_PLAN_GERENCIAINVERSION");
    var PidGerenteInversion = localStorage.getItem("CAPEX_GERENTE_INVERSION_TOKEN");

    var PidRequiere = localStorage.getItem("CAPEX_PLAN_REQUIERE_EJECUCION");

    var PidGerenciaEjecucion = localStorage.getItem("CAPEX_PLAN_GERENCIAEJECUCION");
    var PidGerenteEjecucion = localStorage.getItem("CAPEX_GERENTE_EJECUCION_TOKEN");

    var PidSuperintendencia = localStorage.getItem("CAPEX_PLAN_SUPERINTENDENCIA");
    var PidSuperintendente = localStorage.getItem("CAPEX_INTENDENTE_TOKEN");

    var PidEncargado = localStorage.getItem("CAPEX_ENCARGADO_TOKEN");

    var PidNombreProyectoAlias = $("#NombreProyectoAlias").val();

    /*********************************** VALIDAR ENTRADA**********************************/
    if (estado == "Guardado") {
        swal("", "La identificación se encuentra guardada, utilice la opción 'Actualizar' para realizar cambios.", "info");
        return false;
    }
    else if (!PidNombreProyectoAlias || PidNombreProyectoAlias == "" || PidNombreProyectoAlias == undefined || PidRequiere == "" || PidProceso == "" || PidObjeto == "" || PidArea == "" || PidCompania == "" || PidEtapa == "" || PidNombreProyecto == "" || PidCodigoIniciativa == "" || PidGerenciaInversion == "" || !PidSuperintendencia || PidSuperintendencia == "" || PidSuperintendencia == undefined || PidSuperintendencia == "-1") {
        swal("", "Todos los campos son requeridos, por favor complete el formulario y vuelva a intentar.", "info");
        return false;
    }
    else {
        if (estado == "Guardado") {
            swal("", "La identificación se encuentra guardada, utilice la opción 'Actualizar' para realizar cambios.", "info");
            return false;
        }
        if (PidCodigoProyecto && PidCodigoProyecto != undefined && PidCodigoProyecto.trim() != "" && PidCodigoProyecto.trim().length != 12) {
            swal("", "El formato del campo Código del Proyecto no es correcto.", "info");
            return false;
        }
        FNLimpiarCategorizacion();
        //PREPARAR
        var DTO = {
            'PidTipoIniciativa': PidTipoIniciativa,
            'PidTipoEjercicio': PidTipoEjercicio,
            'PidPeriodo': PidPeriodo,
            'PidUsuario': PidUsuario,
            'PidProceso': PidProceso,
            'PidObjeto': PidObjeto,
            'PidArea': PidArea,
            'PidCompania': PidCompania,
            'PidEtapa': PidEtapa,
            'PidNombreProyecto': PidNombreProyecto,
            'PidNombreProyectoAlias': PidNombreProyectoAlias,
            'PidCodigoIniciativa': PidCodigoIniciativa,
            'PidCodigoProyecto': PidCodigoProyecto,
            'PidGerenciaInversion': PidGerenciaInversion,
            'PidGerenteInversion': PidGerenteInversion,
            'PidRequiere': PidRequiere,
            'PidGerenciaEjecucion': PidGerenciaEjecucion,
            'PidGerenteEjecucion': PidGerenteEjecucion,
            'PidSuperintendencia': PidSuperintendencia,
            'PidSuperintendente': PidSuperintendente,
            'PidGerenciaControl': 'ND',
            'PidGerenteControl': 'ND',
            'PidEncargadoControl': PidEncargado
        };

        $.ajax({
            url: "../Planificacion/GuardarIdentificacion",
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                //PARSEAR RESPUESTA
                var estructura = JSON.parse(JSON.stringify(r));
                estructura = estructura.Mensaje;
                //DESCOMPONER RESPUESTA
                var parte = estructura.split("|");
                var estado = parte[0];
                var folio = parte[1];
                var token = parte[2];
                var PidCodigoIniciativaGenerado = PidCodigoIniciativa + '-' + folio;
                //SETTEAR VARIABLES DEL SISTEMA
                localStorage.setItem("CAPEX_INICIATIVA_ESTADO", estado);
                localStorage.setItem("CAPEX_INICIATIVA_CODIGO", PidCodigoIniciativaGenerado);
                localStorage.setItem("CAPEX_INICIATIVA_FOLIO", folio);
                localStorage.setItem("CAPEX_INICIATIVA_TOKEN", token);
                //PROCESAR RESPUESTA
                if (estado == "Error") {
                    swal("Error", "No es posible almacenar esta inicativa.", "error");
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                } else if (estado == "Guardado") {
                    var fechaCreacionIniciativa = parte[3];
                    swal("Exito", "Identificación Guardada, Código iniciativa <br>" + PidCodigoIniciativaGenerado, "success");
                    $("#BotonGuardarIdentificacion").prop("disabled", "true");
                    $("#BotonActualizarIdentificacion").show();
                    $("#contenedorNuevaIniciativa").show();
                    $("#contenedorNuevaIniciativaCodigoIniciativa").html(PidCodigoIniciativaGenerado);
                    $("#contenedorNuevaIniciativaFechaIniciativa").html(fechaCreacionIniciativa);
                }
            }
        });
    }
}
//
// ACTUALIZAR IDENTIFICACION
//
FNActualizarIdentificacion = function () {
    if (!$('#form_identificacion').valid()) {
        console.log("validacion incorrecta");
        return false;
    }
    console.log("validacion correcta");
    /*********************************** PARAMETROS BASE ***********************************/
    var estado = localStorage.getItem("CAPEX_INICATIVA_ESTADO");
    var PidToken = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    var PidTipoIniciativa = localStorage.getItem("CAPEX_TIPO_INICIATIVA");
    var PidTipoEjercicio = localStorage.getItem("CAPEX_TIPO_EJERCICIO");
    var PidPeriodo = localStorage.getItem("CAPEX_PERIODO_EP");
    var PidUsuario = $("#CAPEX_H_USERNAME").val();

    /*********************************** PARAMETROS ENTRADA**********************************/
    var PidProceso = localStorage.getItem("CAPEX_PLAN_PROCESO");
    var PidObjeto = localStorage.getItem("CAPEX_PLAN_OBJETO");
    var PidArea = localStorage.getItem("CAPEX_PLAN_AREA");
    var PidCompania = localStorage.getItem("CAPEX_PLAN_COMPANIA");
    var PidEtapa = localStorage.getItem("CAPEX_PLAN_ETAPA");
    var PidNombreProyecto = $("#NombreProyecto").val();

    var PidCodigoIniciativa = $("#CodigoIniciativa").val();
    var PidCodigoProyecto = $("#CodigoProyecto").val();

    if ("____-_______" == PidCodigoProyecto) {
        PidCodigoProyecto = "";
    }
    PidCodigoProyecto = PidCodigoProyecto.replace('_', '');
    if ("-" == PidCodigoProyecto) {
        PidCodigoProyecto = "";
    }
    var PidGerenciaInversion = localStorage.getItem("CAPEX_PLAN_GERENCIAINVERSION");
    var PidGerenteInversion = localStorage.getItem("CAPEX_GERENTE_INVERSION_TOKEN");

    var PidRequiere = localStorage.getItem("CAPEX_PLAN_REQUIERE_EJECUCION");

    var PidGerenciaEjecucion = localStorage.getItem("CAPEX_PLAN_GERENCIAEJECUCION");
    var PidGerenteEjecucion = localStorage.getItem("CAPEX_GERENTE_EJECUCION_TOKEN");

    var PidSuperintendencia = localStorage.getItem("CAPEX_PLAN_SUPERINTENDENCIA");
    var PidSuperintendente = localStorage.getItem("CAPEX_INTENDENTE_TOKEN");

    var PidEncargado = localStorage.getItem("CAPEX_ENCARGADO_TOKEN");
    var PidNombreProyectoAlias = $("#NombreProyectoAlias").val();

    /*********************************** VALIDAR ENTRADA**********************************/
    if (!PidNombreProyectoAlias || PidNombreProyectoAlias == "" || PidNombreProyectoAlias == undefined || PidRequiere == "" || PidProceso == "" || PidObjeto == "" || PidArea == "" || PidCompania == "" || PidEtapa == "" || PidNombreProyecto == "" || PidCodigoIniciativa == "" || PidGerenciaInversion == "" || !PidSuperintendencia || PidSuperintendencia == "" || PidSuperintendencia == undefined || PidSuperintendencia == "-1") {
        swal("", "Todos los campos son requeridos, por favor complete el formulario y vuelva a intentar.", "info");
        return false;
    } else {
        if (PidCodigoProyecto && PidCodigoProyecto != undefined && PidCodigoProyecto.trim() != "" && PidCodigoProyecto.trim().length != 12) {
            swal("", "El formato del Código del Proyecto no es correcto.", "info");
            return false;
        }

        if (!PidCodigoProyecto || PidCodigoProyecto == undefined || PidCodigoProyecto.trim() == "") {
            var DTOValidacionCategorizacion = {
                'PidToken': PidToken
            };
            var responseValidacionCategorizacion = FNRealizarValidacionIniciativaEstadoProyecto(DTOValidacionCategorizacion);
            if (responseValidacionCategorizacion && responseValidacionCategorizacion != undefined && responseValidacionCategorizacion != "" && responseValidacionCategorizacion == "true") {
                swal("", "El Código del Proyecto es un campo requerido cuando en Categorización el Estado del Proyecto es Remanente.", "info");
                return false;
            }
        }

        var flujoNormal = true;
        var capexIniciativaCategorizacionEstado = localStorage.getItem("CAPEX_INICIATIVA_CATEGORIZACION_ESTADO");
        if (capexIniciativaCategorizacionEstado && capexIniciativaCategorizacionEstado != undefined && capexIniciativaCategorizacionEstado != "" && (capexIniciativaCategorizacionEstado == "Guardado" || capexIniciativaCategorizacionEstado == "Actualizado")) {

            //PREPARAR
            var DTO = {
                'PidToken': PidToken,
                'PidTipoIniciativa': PidTipoIniciativa,
                'PidTipoEjercicio': PidTipoEjercicio,
                'PidPeriodo': PidPeriodo,
                'PidUsuario': PidUsuario,
                'PidProceso': PidProceso,
                'PidObjeto': PidObjeto,
                'PidArea': PidArea,
                'PidCompania': PidCompania,
                'PidEtapa': PidEtapa,
                'PidNombreProyecto': PidNombreProyecto,
                'PidNombreProyectoAlias': PidNombreProyectoAlias,
                'PidCodigoIniciativa': PidCodigoIniciativa,
                'PidCodigoProyecto': PidCodigoProyecto,
                'PidGerenciaInversion': PidGerenciaInversion,
                'PidGerenteInversion': PidGerenteInversion,
                'PidRequiere': PidRequiere,
                'PidGerenciaEjecucion': PidGerenciaEjecucion,
                'PidGerenteEjecucion': PidGerenteEjecucion,
                'PidSuperintendencia': PidSuperintendencia,
                'PidSuperintendente': PidSuperintendente,
                'PidGerenciaControl': 'ND',
                'PidGerenteControl': 'ND',
                'PidEncargadoControl': PidEncargado
            };
            var responseValidacion = FNRealizarValidacionIniciativa(DTO);
            if (responseValidacion && responseValidacion != "" && responseValidacion != undefined) {
                if (responseValidacion == "1") {
                    flujoNormal = false;
                    swal({
                        title: 'Esta seguro?',
                        text: "Al guardar estos cambios, perderá lo establecido en Categorización!",
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
                            $.ajax({
                                url: "../Planificacion/ActualizarIdentificacionCategorizacion",
                                type: "POST",
                                dataType: "json",
                                data: (DTO),
                                success: function (r) {
                                    if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                                        document.getElementById('linkToLogout').click();
                                        return;
                                    }
                                    //PARSEAR RESPUESTA
                                    var estructura = JSON.parse(JSON.stringify(r));
                                    estructura = estructura.Mensaje;
                                    //DESCOMPONER RESPUESTA
                                    var parte = estructura.split("|");
                                    var estado = parte[0];
                                    var folio = parte[1];
                                    var token = parte[2];
                                    //SETTEAR VARIABLES DEL SISTEMA
                                    localStorage.setItem("CAPEX_INICIATIVA_ESTADO", estado);
                                    localStorage.setItem("CAPEX_INICIATIVA_FOLIO", folio);
                                    localStorage.setItem("CAPEX_INICIATIVA_TOKEN", token);
                                    localStorage.setItem("CAPEX_INICIATIVA_CODIGO", PidCodigoIniciativa);
                                    //PROCESAR RESPUESTA
                                    if (estado == "Error") {
                                        swal("Error", "No es posible actualizar esta inicativa.", "error");
                                        setTimeout(function () {
                                            document.location.reload();
                                        }, 3000);
                                    } else if (estado == "Actualizado") {
                                        swal("Exito", "Identificación Actualizada.", "success");
                                        //localStorage.setItem("CAPEX_NIVEL_INGENIERIA_ALTERNATIVO", "");
                                        localStorage.setItem("CAPEX_CLASIFICA_MACRO", "");
                                        var Categoria = $('#Categoria');
                                        Categoria.empty();
                                        Categoria.append('<option selected="true">Seleccionar..</option>');
                                        $("#RequiereCapacidad1").prop('checked', false);
                                        $("#RequiereCapacidad2").prop('checked', false);
                                        $('#ContenedorMacro2').hide();
                                        $('#ContenedorMacro1').hide();
                                        /*$('#ContenedorNivelIngenieria1').show();
                                        $('#ContenedorNivelIngenieria2').hide();
                                        $("#NivelIngenieria option:last").prop("selected", "selected");*/
                                        FNObtenerListadoNivelIngenieraNoRequiereCorregido(false);
                                        $("#contenedorNuevaIniciativaCodigoIniciativa").html(PidCodigoIniciativa);
                                    }
                                    return false;
                                }
                            });
                        } else {
                            return false;
                        }
                    });
                }
            } else {
                swal("Error", "Error al actualizar identificación. Por favor inténtalo de nuevo más tarde!", "error");
                /*Swal.fire({
                    icon: 'error',
                    title: 'Error al actualizar identificación',
                    text: 'Por favor inténtalo de nuevo más tarde!'
                });*/
                //mensaje error y return
                return false;
            }
        }

        if (!capexIniciativaCategorizacionEstado || capexIniciativaCategorizacionEstado == undefined || capexIniciativaCategorizacionEstado == "") {
            FNLimpiarCategorizacion();
        }

        if (flujoNormal) {

            //PREPARAR
            var DTO = {
                'PidToken': PidToken,
                'PidTipoIniciativa': PidTipoIniciativa,
                'PidTipoEjercicio': PidTipoEjercicio,
                'PidPeriodo': PidPeriodo,
                'PidUsuario': PidUsuario,
                'PidProceso': PidProceso,
                'PidObjeto': PidObjeto,
                'PidArea': PidArea,
                'PidCompania': PidCompania,
                'PidEtapa': PidEtapa,
                'PidNombreProyecto': PidNombreProyecto,
                'PidNombreProyectoAlias': PidNombreProyectoAlias,
                'PidCodigoIniciativa': PidCodigoIniciativa,
                'PidCodigoProyecto': PidCodigoProyecto,
                'PidGerenciaInversion': PidGerenciaInversion,
                'PidGerenteInversion': PidGerenteInversion,
                'PidRequiere': PidRequiere,
                'PidGerenciaEjecucion': PidGerenciaEjecucion,
                'PidGerenteEjecucion': PidGerenteEjecucion,
                'PidSuperintendencia': PidSuperintendencia,
                'PidSuperintendente': PidSuperintendente,
                'PidGerenciaControl': 'ND',
                'PidGerenteControl': 'ND',
                'PidEncargadoControl': PidEncargado
            };

            $.ajax({
                url: "../Planificacion/ActualizarIdentificacion",
                type: "POST",
                dataType: "json",
                data: (DTO),
                success: function (r) {
                    if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                        document.getElementById('linkToLogout').click();
                        return;
                    }
                    //PARSEAR RESPUESTA
                    var estructura = JSON.parse(JSON.stringify(r));
                    estructura = estructura.Mensaje;
                    //DESCOMPONER RESPUESTA
                    var parte = estructura.split("|");
                    var estado = parte[0];
                    var folio = parte[1];
                    var token = parte[2];
                    //SETTEAR VARIABLES DEL SISTEMA
                    localStorage.setItem("CAPEX_INICIATIVA_ESTADO", estado);
                    localStorage.setItem("CAPEX_INICIATIVA_FOLIO", folio);
                    localStorage.setItem("CAPEX_INICIATIVA_TOKEN", token);
                    localStorage.setItem("CAPEX_INICIATIVA_CODIGO", PidCodigoIniciativa);
                    //PROCESAR RESPUESTA
                    if (estado == "Error") {
                        swal("Error", "No es posible actualizar esta inicativa.", "error");
                        setTimeout(function () {
                            document.location.reload();
                        }, 3000);
                    }
                    else if (estado == "Actualizado") {
                        swal("Exito", "Identificación Actualizada.", "success");
                        $("#contenedorNuevaIniciativaCodigoIniciativa").html(PidCodigoIniciativa);
                        return false;
                    }
                }
            });
        }
    }
}

FNRealizarValidacionIniciativaEstadoProyecto = function (DTO) {
    var response = '';
    $.ajax({
        url: "../Planificacion/ValidarEstadoProyectoRemanenteIdentificacion",
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

FNRealizarValidacionIniciativa = function (DTO) {
    var response = '';
    $.ajax({
        url: "../Planificacion/ValidarIdentificacion",
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

//
// OBTENER Y ALMACENAR TOKEN DE COMPAÑIA
//
FNObtenerTokenCompania = function (valor) {
    //LISTO
    var DTO = {
        "Tipo": "Compania", "Valor": valor
    };
    var cuantos = 0
    $.ajax({
        url: "../Planificacion/ObtenerTokenCompania",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                document.getElementById('linkToLogout').click();
                return;
            }
            //PARSEAR RESPUESTA
            var token = JSON.parse(JSON.stringify(r));
            token = token.Mensaje;
            localStorage.setItem("CAPEX_PLAN_COMPANIA_TOKEN", token);
        }
    });
}

FNLimpiarCategorizacion = function () {
    if ($('#ContenedorEstadoNoSeleccionable').is(':visible')) {
        $('#EstadoProyecto1').val("-1");
    } else if ($('#ContenedorEstadoSeleccionable1').is(':visible')) {
        $('#EstadoProyecto2').val("-1");
    } else if ($('#ContenedorEstadoSeleccionable2').is(':visible')) {
        $('#EstadoProyecto3').val("-1");
    }
    $('#TipoCotizacion').val("-1");
    $("#RequiereCapacidad1").prop('checked', false);
    $("#RequiereCapacidad2").prop('checked', false);
    $('#ContenedorMacro2').hide();
    $('#ContenedorMacro1').hide();
    $('#Clase').val("-1");
    var Categoria = $('#Categoria');
    Categoria.empty();
    Categoria.append('<option value="-1" selected="true">Seleccionar..</option>');
    var ClasificacionSSO = $('#ClasificacionSSO');
    ClasificacionSSO.empty();
    ClasificacionSSO.append('<option value="-1" selected="true">Seleccionar..</option>');
    var encontrado = false;
    $("#NivelIngenieria option").each(function (i) {
        if ("NREQ" == $(this).val()) {
            encontrado = true;
        }
    });
    if (encontrado) {
        $("#NivelIngenieria option[value='" + "NREQ" + "']").remove();
    }
    $("#NivelIngenieria").val("-1");
    var EstandarSeguridad = $('#EstandarSeguridad');
    EstandarSeguridad.empty();
    EstandarSeguridad.append('<option value="-1" selected="true">Seleccionar..</option>');
}
