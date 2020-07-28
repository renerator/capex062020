
// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : PLANIFICACION
// FECHA            : MAYO 2018
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// METODOS          : PLANIFICACION

(function ($) {
    $.validator.addMethod("valueNotEquals", function (value, element, arg) {
        console.log("valueNotEquals value=" + value + ", element=" + element + ", arg=" + arg);
        return arg !== value;
    }, "Value must not equal arg.");

    $("#form_identificacion").validate({
        rules: {
            Proceso: { valueNotEquals: "-1" },
            Objeto: { required: { depends: function () { $(this).val($.trim($(this).val())); return true; } } },
            Area: { valueNotEquals: "-1" },
            Compania: { valueNotEquals: "-1" },
            Etapa: { valueNotEquals: "-1" },
            NombreProyecto: 'required',
            CodigoIniciativa: 'required',
            NombreProyectoAlias: { required: { depends: function () { $(this).val($.trim($(this).val())); return true; } } },
            GerenciaInversion: { valueNotEquals: "-1" },
            GerenciaEjecucion: { valueNotEquals: "-1" },
            Superitendencia: { valueNotEquals: "-1" },
            requiere: 'required'
        },
        messages: {
            Proceso: { valueNotEquals: "Por favor, selecciona proceso" },
            Objeto: "Por favor, ingrese objeto",
            Area: { valueNotEquals: "Por favor, selecciona area" },
            Compania: { valueNotEquals: "Por favor, selecciona compañia" },
            Etapa: { valueNotEquals: "Por favor, selecciona etapa" },
            NombreProyecto: "Por favor, ingresa nombre del proyecto",
            CodigoIniciativa: "Por favor, ingresa código  de la iniciativa",
            NombreProyectoAlias: "Por favor, ingresa alias del proyecto",
            GerenciaInversion: { valueNotEquals: "Por favor, selecciona gerencia inversión" },
            GerenciaEjecucion: { valueNotEquals: "Por favor, selecciona gerencia ejecución" },
            Superitendencia: { valueNotEquals: "Por favor, selecciona superintendencia" },
            requiere: 'Por favor, responder pregunta'
        },
        debug: true
    });

    $("#form_categorizacion").validate({
        rules: {
            TipoCotizacion: { valueNotEquals: "-1" },
            RequiereCapacidad: 'required',
            Clase: { valueNotEquals: "-1" },
            Categoria: { valueNotEquals: "-1" },
            ClasificacionSSO: { valueNotEquals: "-1" },
            NivelIngenieria: { valueNotEquals: "-1" }
        },
        messages: {
            TipoCotizacion: { valueNotEquals: "Por favor, selecciona tipo de cotización" },
            RequiereCapacidad: 'Por favor, responder pregunta',
            Clase: { valueNotEquals: "Por favor, selecciona clase" },
            Categoria: { valueNotEquals: "Por favor, selecciona categoria" },
            ClasificacionSSO: { valueNotEquals: "Por favor, selecciona clasificación HSEC" },
            NivelIngenieria: "Por favor, ingresa nivel de ingenieria"
        },
        debug: true
    });

    $("#form_descripcion").validate({
        rules: {
            PddObjetivo: { required: { depends: function () { $(this).val($.trim($(this).val())); return true; } } },
            PddAlcance: { required: { depends: function () { $(this).val($.trim($(this).val())); return true; } } },
            PddJustificacion: { required: { depends: function () { $(this).val($.trim($(this).val())); return true; } } }
        },
        messages: {
            PddObjetivo: 'Por favor, ingresa el objetivo de la inversión',
            PddAlcance: 'Por favor, ingresa el alcance de la inversión',
            PddJustificacion: 'Por favor, ingresa la justificación de la inversión'
        },
        debug: true
    });

    $("#form_eval_eco").validate({
        rules: {
            EveVan: { required: { depends: function () { $(this).val($.trim($(this).val())); return true; } } },
            EveTir: { required: { depends: function () { $(this).val($.trim($(this).val())); return true; } } },
            EveIvan: { required: { depends: function () { $(this).val($.trim($(this).val())); return true; } } },
            EvePayBack: { required: { depends: function () { $(this).val($.trim($(this).val())); return true; } } },
            EveVidaUtil: { required: { depends: function () { $(this).val($.trim($(this).val())); return true; } } },
            EveTipoCambio: { required: { depends: function () { $(this).val($.trim($(this).val())); return true; } } }
        },
        messages: {
            EveVan: 'Por favor, ingresa el VAN',
            EveTir: 'Por favor, ingresa el TIR',
            EveIvan: 'Por favor, ingresa el IVAN',
            EvePayBack: 'Por favor, ingresa el PayBack',
            EveVidaUtil: 'Por favor, ingresa la Vida Util',
            EveTipoCambio: 'Por favor, ingresa el Tipo de Cambio',
        },
        debug: true
    });

    $("#form_eval_riesgo").validate({
        rules: {
            EvrProbabilidad1: { required: { depends: function () { $(this).val($.trim($(this).val())); return true; } } },
            EvrImpacto1: { required: { depends: function () { $(this).val($.trim($(this).val())); return true; } } },
            EvrProbabilidad2: { required: { depends: function () { $(this).val($.trim($(this).val())); return true; } } },
            EvrImpacto2: { required: { depends: function () { $(this).val($.trim($(this).val())); return true; } } }
        },
        messages: {
            EvrProbabilidad1: 'probabilidad',
            EvrImpacto1: 'impacto',
            EvrProbabilidad2: 'probabilidad',
            EvrImpacto2: 'impacto'
        },
        debug: true
    });

    /*********************************** LIMPIAR ALMACENAMIENTO LOCAL *******************************/
    localStorage.setItem("CAPEX_PLAN_PROCESO", "");
    localStorage.setItem("CAPEX_PLAN_OBJETO", "");
    localStorage.setItem("CAPEX_PLAN_AREA", "");
    localStorage.setItem("CAPEX_PLAN_COMPANIA", "");
    localStorage.setItem("CAPEX_PLAN_ETAPA", "");
    localStorage.setItem("CAPEX_PLAN_NOMBRE_PROYECTO", "");

    localStorage.setItem("CAPEX_PLAN_GERENCIAINVERSION", "");
    localStorage.setItem("CAPEX_PLAN_GERENCIAEJECUCION", "");
    localStorage.setItem("CAPEX_PLAN_SUPERINTENDENCIA", "");
    localStorage.setItem("CAPEX_PLAN_GERENCIA_ID", "");
    localStorage.setItem("CAPEX_PLAN_SUPER_ID", "");

    localStorage.setItem("CAPEX_GERENTE_INVERSION_NOMBRE", "");
    localStorage.setItem("CAPEX_GERENTE_INVERSION_TOKEN", "");
    localStorage.setItem("CAPEX_GERENTE_EJECUCION_NOMBRE", "");
    localStorage.setItem("CAPEX_GERENTE_EJECUCION_TOKEN", "");
    localStorage.setItem("CAPEX_INTENDENTE_NOMBRE", "");
    localStorage.setItem("CAPEX_INTENDENTE_TOKEN", "");
    localStorage.setItem("CAPEX_ENCARGADO_NOMBRE", "");
    localStorage.setItem("CAPEX_ENCARGADO_TOKEN", "");

    localStorage.setItem("CAPEX_PLAN_REQUIERE_EJECUCION", "");
    localStorage.setItem("CAPEX_PLAN_REQUIERE_CAPACIDAD", "");
    localStorage.setItem("CAPEX_CLASIFICA_MACRO", "");
    localStorage.setItem("CAPEX_OPCION_SOLUCION_CICLOINVERSIONAL", "");
    localStorage.setItem("CAPEX_NOTA_ANALISIS_BAJA_COMPLEJIDAD", "");
    localStorage.setItem("CAPEX_PLAN_ANALISIS_BAJA_COMPLEJIDAD", "");
    localStorage.setItem("CAPEX_PLAN_SSO", "");
    localStorage.setItem("CAPEX_PLAN_INICIAL", "");

    localStorage.setItem("CAPEX_DOTACION_HOTELERIA", "");
    localStorage.setItem("CAPEX_DOTACION_ALIMENTACION", "");
    localStorage.setItem("CAPEX_DOTACION_DATA", "");
    localStorage.setItem("CAPEX_DOTACION_MES", "");

    localStorage.setItem("CAPEX_INICIATIVA_CODIGO", "");
    localStorage.setItem("CAPEX_INICIATIVA_ESTADO", "");
    localStorage.setItem("CAPEX_INICIATIVA_CATEGORIZACION_ESTADO", "");
    localStorage.setItem("CAPEX_INICIATIVA_CATEGORIZACION_ARCH_BC_SUBIDO", "");
    localStorage.setItem("CAPEX_INICIATIVA_PRESUPUESTO_ESTADO", "");
    localStorage.setItem("CAPEX_INICIATIVA_DESCRIPCION_ESTADO", "");
    localStorage.setItem("CAPEX_INICIATIVA_EVALUACION_ECONOMICA_ESTADO", "");
    localStorage.setItem("CAPEX_INICIATIVA_EVALUACION_RIESGO_ESTADO", "");
    localStorage.setItem("CAPEX_INICIATIVA_EVALUACION_HITO_ESTADO", "");


    localStorage.setItem("CAPEX_FECHA_POSTEVAL_EN_USO", "");

    localStorage.setItem("CAPEX_MATRIZ_RIESGO_ITEM", "");
    localStorage.setItem("CAPEX_HITO_FECHA_INICIO", "");
    localStorage.setItem("CAPEX_HITO_TOTAL", "");
    localStorage.setItem("CAPEX_INICIATIVA_HITO_ESTADO", "");

    localStorage.setItem("CAPEX_INICIATIVA_FOLIO", "");
    localStorage.setItem("CAPEX_INICIATIVA_TOKEN", "");
    localStorage.setItem("CAPEX_PLAN_COMPANIA_TOKEN", "");

    localStorage.setItem("CAPEX_IDENTIFICACION_ETAPA_ANTERIOR", "");
    localStorage.setItem("CAPEX_IDENTIFICACION_CAMBIO", "");
    localStorage.setItem("CAPEX_CATEGORIZACION_CAMBIO", "");
    localStorage.setItem("CAPEX_CATEGORIZACION_DESARROLLO_CAMBIO", "");
    localStorage.setItem("CAPEX_CATEGORIZACION_ABC_CAMBIO", "");
    localStorage.setItem("CAPEX_DESCRIPCION_CAMBIO", "");
    localStorage.setItem("CAPEX_EVALECO_CAMBIO", "");
    localStorage.setItem("CAPEX_EVALRIESGO_CAMBIO", "");
    localStorage.setItem("CAPEX_GANTT_CAMBIO", "");
    localStorage.setItem("CAPEX_TEMPLATE_CAMBIO", "");
    localStorage.setItem("CAPEX_HITOS_CAMBIO", "");

    $('#EvrProbabilidad1').on('change keyup', function () {
        console.log("EvrProbabilidad1 change keyup $(this).val()=", $(this).val());
        FNCalculoRiesgoSinProyecto();
    });

    $('#EvrProbabilidad1').on('click', function () {
        console.log("EvrProbabilidad1 click");
        FNCalculoRiesgoSinProyecto();
        $('#form_eval_riesgo').valid();
    });

    $('#EvrImpacto1').on('change keyup', function () {
        console.log("EvrImpacto1 change keyup $(this).val()=", $(this).val());
        FNCalculoRiesgoSinProyecto();
    });

    $('#EvrImpacto1').on('click', function () {
        console.log("EvrImpacto1 click");
        FNCalculoRiesgoSinProyecto();
        $('#form_eval_riesgo').valid();
    });

    $('#EvrProbabilidad2').on('change keyup', function () {
        console.log("EvrProbabilidad2 change keyup $(this).val()=", $(this).val());
        FNCalculoRiesgoConProyecto();
    });

    $('#EvrProbabilidad2').on('click', function () {
        console.log("EvrProbabilidad2 click");
        FNCalculoRiesgoConProyecto();
        $('#form_eval_riesgo').valid();
    });

    $('#EvrImpacto2').on('change keyup', function () {
        console.log("EvrImpacto2 change keyup $(this).val()=", $(this).val());
        FNCalculoRiesgoConProyecto();
    });

    $('#EvrImpacto2').on('click', function () {
        console.log("EvrImpacto2 click");
        FNCalculoRiesgoConProyecto();
        $('#form_eval_riesgo').valid();
    });

    $('#EvrMFL1').number(true, 2, ',', '.');
    $('#EvrMFL2').number(true, 2, ',', '.');

    $("#EvrMFL1").on("click", function () {
        $(this).select();
    });
    $("#EvrMFL1").focus(function () {
        $(this).select();
    });
    $("#EvrMFL1").focusin(function () {
        $(this).select();
    });

    $("#EvrMFL2").on("click", function () {
        $(this).select();
    });
    $("#EvrMFL2").focus(function () {
        $(this).select();
    });
    $("#EvrMFL2").focusin(function () {
        $(this).select();
    });

    /**************************** DETECCION DE CAMBIOS                     ***********************/

    $("#form_identificacion").change(function () {
        localStorage.setItem("CAPEX_IDENTIFICACION_CAMBIO", "SI");
    });
    $("#form_categorizacion").change(function () {
        localStorage.setItem("CAPEX_CATEGORIZACION_CAMBIO", "SI");
    });
    $("#form_categorizacion_desarrollo").change(function () {
        localStorage.setItem("CAPEX_CATEGORIZACION_DESARROLLO_CAMBIO", "SI");
    });
    $("#form_categorizacion_baja_complejidad").change(function () {
        localStorage.setItem("CAPEX_CATEGORIZACION_ABC_CAMBIO", "SI");
    });
    $("#form_descripcion").change(function () {
        localStorage.setItem("CAPEX_DESCRIPCION_CAMBIO", "SI");
    });
    $("#form_eval_eco").change(function () {
        localStorage.setItem("CAPEX_EVALECO_CAMBIO", "SI");
    });
    $("#form_eval_riesgo").change(function () {
        localStorage.setItem("CAPEX_EVALRIESGO_CAMBIO", "SI");
    });
    $("#form_importar_gantt").change(function () {
        localStorage.setItem("CAPEX_GANTT_CAMBIO", "SI");
    });
    $("#form_importar_template").change(function () {
        localStorage.setItem("CAPEX_TEMPLATE_CAMBIO", "SI");
    });
    $("#form_hitos").change(function () {
        localStorage.setItem("CAPEX_HITOS_CAMBIO", "SI");
    });


    /**************************** CARGA DE VALORES PARA TAB-IDENTIFICACION ***********************/

    setTimeout(function () {
        FNObtenerListadoProcesos();
        setTimeout(function () {
            FNObtenerListadoAreas();
            setTimeout(function () {
                FNObtenerListadoCompanias();
                setTimeout(function () {
                    FNObtenerListadoEtapas();
                    setTimeout(function () {
                        FNObtenerListadoGerencia();
                    }, 200);
                }, 200);
            }, 200);
        }, 200);
    }, 200);


    /**************************** CARGA DE VALORES PARA TAB-CATEGORIZACION *************************/
    setTimeout(function () {
        FNObtenerListadoCategorias();
        setTimeout(function () {
            FNObtenerListadoNivelIngeniera();
            setTimeout(function () {
                FNObtenerListadoClasificacionSSO("");
            }, 200);
        }, 200);
    }, 200);

    /**************************** CARGA DE VALORES PARA TAB-PRESUPUESTO-DOTACION ******************/
    setTimeout(function () {
        FNObtenerUbicaciones();
        setTimeout(function () {
            FNObtenerTipoEECC();
            setTimeout(function () {
                FNObtenerClasificaciones();
                setTimeout(function () {
                    FNObtenerDepartamentoDotacion();
                    setTimeout(function () {
                        FNObtenerTurnosDotacion();
                    }, 200);
                }, 200);
            }, 200);
        }, 200);
    }, 200);
    /*********************************** CARGA DE TEXTOS DE AYUDA ***************************/
    FNObtenerTextoAyuda("IDENTIFICACION", "");

    /*********************************** NOMBRE DEL PROYECTO **********************************/
    FNResolverNombreProyecto();
    /*********************************** ESTADO DEL PROYECTO **********************************/
    FNResolverEstadoDelProyecto();
    /*********************************** RIESGO DEL PROYECTO **********************************/
    //FNResolverRiesgo();
    /*********************************** TIPO  DE INICIATIVA **********************************/
    var tipo_iniciativa = localStorage.getItem("CAPEX_TIPO_INICIATIVA");
    var iniPeriodo = localStorage.getItem("CAPEX_PERIODO_EP");
    if (iniPeriodo && iniPeriodo != undefined && iniPeriodo != "") {
        $(".iniPeridioHito").text(iniPeriodo);
    }
    switch (tipo_iniciativa) {
        case "CB": $("#TipoIniTit").html("Caso Base"); break;
        case "CD": $("#TipoIniTit").html("Caso Desarrollo"); break;
        case "PP": $("#TipoIniTit").html("Presupuesto"); break;
        case "EX": $("#TipoIniTit").html("Extraordinario"); break;
    }
    if (tipo_iniciativa == "CB" || tipo_iniciativa == "CD") {
        $("#ContenedorBotoneraComprobacion").hide();
        $("#ContenedorDescargaTemplateCB").show();
        $("#ContenedorTablaCasoBase1").show();
        $("#ContenedorTablaCasoBase2").show();

        var anioCasoBase = iniPeriodo;
        var contador = 0;
        $(".presupuestoCB2").each(function () {
            $(this).text((parseInt(anioCasoBase) + contador));
            contador++;
        });

        anioCasoBase = iniPeriodo;
        contador = 0;
        $(".presupuestoCB2").each(function () {
            $(this).text((parseInt(anioCasoBase) + contador));
            contador++;
        });

        $("#ContenedorTabla1").hide();
        $("#ContenedorTabla2").hide();
    } else {
        $("#ContenedorBotoneraComprobacion").show();
        $("#ContenedorDescargaTemplate").show();
        $("#ContenedorTabla1").show();
        $("#ContenedorTabla2").show();

        var anioPresupuesto = iniPeriodo;
        var contador = 0;
        $(".presupuestoPP1").each(function () {
            $(this).text((parseInt(anioPresupuesto) + contador));
            contador++;
        });

        anioPresupuesto = iniPeriodo;
        contador = 0;
        $(".presupuestoPP2").each(function () {
            $(this).text((parseInt(anioPresupuesto) + contador));
            contador++;
        });

        $("#ContenedorTablaCasoBase1").hide();
        $("#ContenedorTablaCasoBase2").hide();
    }
    FNObtenerExcelTemplateFinal();

    /******************************** IMPORTACION CONTROL DE EVENTOS *************************/

    $('#ArchivoDesarrollo').on('change', function () {
        var file = document.getElementById('ArchivoDesarrollo');
        if (file.files.length > 0) {
            document.getElementById('NombreArchivoDesarrollo').innerHTML = file.files[0].name;
        }
    });

    $('#ImportarTemplate').on('change', function () {
        var file = document.getElementById('ImportarTemplate');
        if (file.files.length > 0) {
            document.getElementById('filename').innerHTML = file.files[0].name;
        }
    });

    $('#ArchivoGantt').on('change', function () {
        var file = document.getElementById('ArchivoGantt');
        if (file.files.length > 0) {
            document.getElementById('NombreArchivoGantt').innerHTML = file.files[0].name;
        }
    });

    $('#ArchivoDesc').on('change', function () {
        var file = document.getElementById('ArchivoDesc');
        if (file.files.length > 0) {
            document.getElementById('NombreArchivoDesc').innerHTML = file.files[0].name;
        }
    });

    $('#ArchivoEvalEco').on('change', function () {
        var file = document.getElementById('ArchivoEvalEco');
        if (file.files.length > 0) {
            document.getElementById('NombreArchivoEvalEco').innerHTML = file.files[0].name;
        }
    });

    $('#ArchivoEvalRiesgo').on('change', function () {
        var file = document.getElementById('ArchivoEvalRiesgo');
        if (file.files.length > 0) {
            document.getElementById('NombreArchivoRiesgo').innerHTML = file.files[0].name;
        }
    });


    /********************************** CONTROL DE EVENTO SUBMIT *****************************/
    $("#form_identificacion").submit(function (e) {
        return false;
    });
    $("#form_categorizacion").submit(function (e) {
        return false;
    });
    $("#form_categorizacion_desarrollo").submit(function (e) {
        return false;
    });
    $("#form_categorizacion_baja_complejidad").submit(function (e) {
        return false;
    });
    $("#form_descripcion").submit(function (e) {
        return false;
    });
    $("#form_eval_eco").submit(function (e) {
        return false;
    });
    $("#form_eval_riesgo").submit(function (e) {
        return false;
    });

    $("#form_importar_gantt").submit(function (e) {
        return false;
    });

    $("#form_importar_template").submit(function (e) {
        return false;
    });

    $("#form_hitos").submit(function (e) {
        return false;
    });

    /**********************************  EVAL ECO  *****************************/
    /*$("#EveVan").ForceNumericOnly();
    $("#EveTIR").ForceNumericOnly();
    $("#EveIvan").ForceNumericOnly();
    $("#EvePayBack").ForceNumericOnly();
    $("#EveVidaUtil").ForceNumericOnly();
    $("#EveTipoCambio").ForceNumericOnly();*/

    /**********************************  EVAL RIESGO  *****************************/
    //$("#EvrProbabilidad1").ForceNumericOnly();
    //$("#EvrImpacto1").ForceNumericOnly();
    //$("#EvrRiesgo1").ForceNumericOnly();
    //$("#EvrClasificacion1").ForceNumericOnly();
    //$("#EvrMFL1").ForceNumericOnly();

    //$("#EvrProbabilidad2").ForceNumericOnly();
    //$("#EvrImpacto2").ForceNumericOnly();
    //$("#EvrRiesgo2").ForceNumericOnly();
    //$("#EvrClasificacion2").ForceNumericOnly();
    //$("#EvrMFL2").ForceNumericOnly();



    /**********************************  FECHA POST EVALUACION *****************************/

    var hoy = new Date();
    if (hoy) {
        var futuro = 6;
        var actual = moment(hoy);
        var fecha_futura = moment(actual).add(futuro, 'months');
        var PddFechaPostEvalBD = moment(fecha_futura).format('DD-MM-YYYY');
        $('#datepickerFPE').datepicker({
            weekStart: 1,
            daysOfWeekHighlighted: "6,0",
            autoclose: true,
            todayHighlight: true,
            format: 'dd-mm-yyyy',
            language: 'es'
        });
        $('#datepickerFPE').datepicker("setDate", new Date(PddFechaPostEvalBD.split("-")[2], (PddFechaPostEvalBD.split("-")[1] - 1), PddFechaPostEvalBD.split("-")[0]));
    }


    /**********************************  ARBOL DE ARCHIVOS *****************************/

    $(".file-tree").filetree();
    $("#AppLoaderContainer").hide();

    $('#EveTir').mask('##0,00%', { reverse: true });

    $("#EveTir").on("click", function () {
        $(this).select();
    });
    $("#EveTir").focus(function () {
        $(this).select();
    });
    $("#EveTir").focusin(function () {
        $(this).select();
    });
    $("#EveVidaUtil").on("click", function () {
        $(this).select();
    });
    $("#EveVidaUtil").focus(function () {
        $(this).select();
    });
    $("#EveVidaUtil").focusin(function () {
        $(this).select();
    });
    $("#EveVan").on("click", function () {
        $(this).select();
    });
    $("#EveVan").focus(function () {
        $(this).select();
    });
    $("#EveVan").focusin(function () {
        $(this).select();
    });
    $("#EveIvan").on("click", function () {
        $(this).select();
    });
    $("#EveIvan").focus(function () {
        $(this).select();
    });
    $("#EveIvan").focusin(function () {
        $(this).select();
    });
    $("#EvePayBack").on("click", function () {
        $(this).select();
    });
    $("#EvePayBack").focus(function () {
        $(this).select();
    });
    $("#EvePayBack").focusin(function () {
        $(this).select();
    });
    $("#EveTipoCambio").on("click", function () {
        $(this).select();
    });
    $("#EveTipoCambio").focus(function () {
        $(this).select();
    });
    $("#EveTipoCambio").focusin(function () {
        $(this).select();
    });

    $('#EveTir').on('change', function () {
        console.log('Change event.');
        var val = $('#EveTir').val();
        console.log('Change event. val=', val);
    });

    $('#EveVidaUtil').on('change', function () {
        console.log('Change event.');
        var val = $('#EveVidaUtil').val();
        console.log('Change event. val=', val);
    });

    $('#EveVan').on('change', function () {
        console.log('Change event.');
        var val = $('#EveVan').val();
        console.log('Change event. val=', val);
    });
    $('#EveIvan').on('change', function () {
        console.log('Change event.');
        var val = $('#EveIvan').val();
        console.log('Change event. val=', val);
    });
    $('#EvePayBack').on('change', function () {
        console.log('Change event.');
        var val = $('#EvePayBack').val();
        console.log('Change event. val=', val);
    });
    $('#EveTipoCambio').on('change', function () {
        console.log('Change event.');
        var val = $('#EveTipoCambio').val();
        console.log('Change event. val=', val);
    });
    $('#EveVidaUtil').change(function () {
        console.log('Second change event...');
    });
    $('#EveVan').change(function () {
        console.log('Second change event...');
    });
    $('#EveIvan').change(function () {
        console.log('Second change event...');
    });
    $('#EvePayBack').change(function () {
        console.log('Second change event...');
    });
    $('#EveTipoCambio').change(function () {
        console.log('Second change event...');
    });
    $('#EveVidaUtil').number(true, 1, ',', '.');
    $('#EveVan').number(true, 2, ',', '.');
    $('#EveIvan').number(true, 2, ',', '.');
    $('#EvePayBack').number(true, 2, ',', '.');
    $('#EveTipoCambio').number(true, 2, ',', '.');

    $('#CodigoProyecto').inputmask({ mask: "****-*******" });  //static mask

}(jQuery));