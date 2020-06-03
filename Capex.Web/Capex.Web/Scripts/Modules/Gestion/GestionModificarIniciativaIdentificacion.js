// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : GESTION - MODIFICAR INICIATIVA
// FECHA            : MAYO 2018
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// SECCION          : IDENTIFICACION
// METODOS          :
//
// CATEGORIA Y MACROCATEGORIA (REFACTORIZACION)
// CONTIENE REGLA DE NEGOCIO #3, MANEJO DE ESTADO PARA INGRESO y MODIFICACION
//
//

FNResolverNombreProyecto = function () {
    var t = setInterval(FNContruirNombreProyecto, 1500);
}
//
// ARMAR NOMBRE DE PROYECTO
//

FNContruirNombreProyecto = function () {
    /*var nomproy = $("#NombreProyecto").val();
    var proceso = $("#Proceso").val();//localStorage.getItem("CAPEX_PLAN_PROCESO");
    var objeto = $("#Objeto").val();//localStorage.getItem("CAPEX_PLAN_OBJETO");
    var area = $("#Area").val();//localStorage.getItem("CAPEX_PLAN_AREA");
    var compania = $("#Compania").val();//localStorage.getItem("CAPEX_PLAN_COMPANIA");
    var etapa = $("#Etapa").val();//localStorage.getItem("CAPEX_PLAN_ETAPA");
    var tipo = localStorage.getItem("CAPEX_TIPO_INICIATIVA");
    var periodo = '@Html.Raw(@ViewBag.BaseIniPeriodo)';*/
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
    } else {
        $("#CodigoIniciativa").val(tipo + actual + compania + area + etapa + ((folioIniciativaGuardado == "") ? "" : ("-" + folioIniciativaGuardado)));
    }
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
// CARGAR ESTADO DEL PROYECTO
//
FNResolverEstadoDelProyecto = function () {
    var estado = localStorage.getItem("CAPEX_TIPO_EJERCICIO");
    var estadoProyecto = localStorage.getItem("CAPEX_PLAN_ESTADO_PROYECTO");
    var tipo = localStorage.getItem("CAPEX_TIPO_INICIATIVA");
    console.log("FNResolverEstadoDelProyecto estado=" + estado + ", estadoProyecto=" + estadoProyecto + ", tipo=" + tipo);
    if ((tipo == "CB" || tipo == "CD") && (estado == "CREAR" || estado == "IMPORTAR")) {
        $("#ContenedorEstadoNoSeleccionable").hide();
        $("#ContenedorEstadoSeleccionable2").hide();
        $("#ContenedorEstadoSeleccionable1").show();
        var indexSelectOption = 0;
        var iterSelectOption = 0;
        $("#EstadoProyecto2 > option").each(function () {
            console.info('this.text=' + this.text + ', this.value=' + this.value);
            if (estadoProyecto.toUpperCase() == this.value.toUpperCase()) {
                indexSelectOption = iterSelectOption;
            }
            iterSelectOption++;
        });
        console.info("indexSelectOption=", indexSelectOption);
        $('#EstadoProyecto2>option:eq(' + indexSelectOption + ')').prop('selected', true);
        /*if (estado == "CREAR") {
            $('#EstadoProyecto2>option:eq(1)').prop('selected', true);
        }
        else if (estado == "IMPORTAR") {
            $('#EstadoProyecto2>option:eq(2)').prop('selected', true);
        }
        else if (estado == "FUTURO") {
            $('#EstadoProyecto2>option:eq(3)').prop('selected', true);
        }*/
    }
    else if ((tipo == "PP") && (estado == "CREAR" || estado == "IMPORTAR")) {
        $("#ContenedorEstadoNoSeleccionable").hide();
        $("#ContenedorEstadoSeleccionable2").show();
        $("#ContenedorEstadoSeleccionable1").hide();

        var indexSelectOption = 0;
        var iterSelectOption = 0;
        $("#EstadoProyecto3 > option").each(function () {
            console.info('this.text=' + this.text + ', this.value=' + this.value);
            if (estadoProyecto.toUpperCase() == this.value.toUpperCase()) {
                indexSelectOption = iterSelectOption;
            }
            iterSelectOption++;
        });
        console.info("indexSelectOption=", indexSelectOption);
        $('#EstadoProyecto3>option:eq(' + indexSelectOption + ')').prop('selected', true);

        /*if (estado == "CREAR") {
            $('#EstadoProyecto3>option:eq(1)').prop('selected', true);
        }
        else if (estado == "IMPORTAR") {
            $('#EstadoProyecto3>option:eq(2)').prop('selected', true);
        }*/
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
FNObtenerListadoProcesos = function (ProcesoGuardado) {
    //PREPARAR
    var Proceso = $('#Proceso');
    Proceso.empty();
    Proceso.append('<option value="-1" selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../../Planificacion/ListarProcesos",
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
                    if (ProcesoGuardado == value.ProcNombre) {
                        $('#Proceso').append(new Option(value.ProcNombre, value.ProcAcronimo, false, true));
                        localStorage.setItem("CAPEX_PLAN_PROCESO", value.ProcAcronimo);
                    } else {
                        $('#Proceso').append(new Option(value.ProcNombre, value.ProcAcronimo, false, false));
                    }
                    cuantos++;
                });
            }, 500);
        }
    });
}
//
// LISTADO DE AREAS
//
FNObtenerListadoAreas = function (AreaGuardado) {
    //PREPARAR
    var Area = $('#Area');
    Area.empty();
    Area.append('<option value="-1" selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../../Planificacion/ListarAreas",
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
                    if (AreaGuardado == value.AreaNombre) {
                        $('#Area').append(new Option(value.AreaNombre, value.AreaAcronimo, false, true));
                        localStorage.setItem("CAPEX_PLAN_AREA", value.AreaAcronimo);
                    }
                    else {
                        $('#Area').append(new Option(value.AreaNombre, value.AreaAcronimo, false, false));
                    }
                    cuantos++;
                });

            }, 500);
        }
    });
}
//
// LISTADO DE COMPANIAS
//
FNObtenerListadoCompanias = function (CompaniaGuardado) {
    //PREPARAR

    var Compania = $('#Compania');
    Compania.empty();
    Compania.append('<option value="-1" selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../../Planificacion/ListarCompanias",
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
                    if (CompaniaGuardado == value.ComNombre) {
                        $('#Compania').append(new Option(value.ComNombre, value.ComAcronimo, false, true));
                        localStorage.setItem("CAPEX_PLAN_COMPANIA", value.ComAcronimo);
                        FNObtenerTokenCompania(value.ComAcronimo);
                    }
                    else {
                        $('#Compania').append(new Option(value.ComNombre, value.ComAcronimo, false, false));
                        localStorage.setItem("CAPEX_PLAN_COMPANIA", value.ComAcronimo);
                    }
                    cuantos++;
                });

            }, 500);
        }
    });
}
//
// LISTADO DE ETAPAS
//
FNObtenerListadoEtapas = function (EpataGuardado) {
    //PREPARAR
    var Etapa = $('#Etapa');
    Etapa.empty();
    Etapa.append('<option value="-1" selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "../../Planificacion/ListarEtapas",
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
                    if (EpataGuardado == value.NINombre) {
                        $('#Etapa').append(new Option(value.NINombre, value.NIAcronimo, false, true));
                    }
                    else {
                        $('#Etapa').append(new Option(value.NINombre, value.NIAcronimo, false, false));
                    }
                    cuantos++;
                });
            }, 500);
        }
    });
}
//
// LISTADO DE GERENCIAS
//
FNObtenerListadoGerencia = function () {
    //PREPARAR

    var GerInversionGuardado = '@Html.Raw(@ViewBag.IdenGerInver)';
    var GerEjecucionGuardado = '@Html.Raw(@ViewBag.IdenGerEjec)';
    var IdGerenciaEjecGuardada = '@Html.Raw(@ViewBag.IdenIdGeren)';
    //
    // PRIMERO: VERIFICAR ESTADO /SI TIENE ALMACENADO CAMBIOS
    //
    if (GerInversionGuardado == "" || GerEjecucionGuardado == "") {

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
            url: "Planificacion/ListarGerencias",
            type: "GET",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                GerenciaInversion.empty();
                GerenciaEjecucion.empty();
                setTimeout(function () {
                    if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                        document.getElementById('linkToLogout').click();
                        return;
                    }
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
                        GerenciaInversion.append('<option selected="true">Seleccionar..</option>')
                    }

                    if (cuantos == 1) {
                        GerenciaEjecucion.prop('selectedIndex', 0);
                    }
                    else if (cuantos > 1) {
                        GerenciaEjecucion.append('<option selected="true">Seleccionar..</option>')
                    }

                }, 500);
            }
        });

    }
    else {
        var GerInversionGuardado = '@Html.Raw(@ViewBag.IdenGerInver)';
        var GerenciaInversion = $('#GerenciaInversion');
        GerenciaInversion.empty();
        GerenciaInversion.append('<option value="-1" selected="true">Buscando..</option>');

        var GerEjecucionGuardado = '@Html.Raw(@ViewBag.IdenGerEjec)';
        var GerenciaEjecucion = $('#GerenciaEjecucion');
        GerenciaEjecucion.empty();
        GerenciaEjecucion.append('<option value="-1" selected="true">Buscando..</option>');
        //LISTO
        var DTO = {};
        var cuantos1 = 0
        $.ajax({
            url: "../../Planificacion/ListarGerencias",
            type: "GET",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                GerenciaInversion.empty();
                setTimeout(function () {
                    if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                        document.getElementById('linkToLogout').click();
                        return;
                    }
                    $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                        if (GerInversionGuardado == value.GerNombre) {
                            $('#GerenciaInversion').append(new Option(value.GerNombre, value.GerToken, false, true));
                        }
                        else {
                            $('#GerenciaInversion').append(new Option(value.GerNombre, value.GerToken, false, false));
                        }
                        cuantos1++;
                    });
                }, 500);
                GerenciaInversion.append('<option value="-1" selected="true">Seleccionar..</option>');
            }
        });
        var cuantos2 = 0
        $.ajax({
            url: "../../Planificacion/ListarGerencias",
            type: "GET",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                GerenciaEjecucion.empty();
                setTimeout(function () {
                    if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                        document.getElementById('linkToLogout').click();
                        return;
                    }
                    $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                        if (GerEjecucionGuardado == value.GerNombre) {
                            $('#GerenciaEjecucion').append(new Option(value.GerNombre, value.GerToken, false, true));
                            if (IdGerenciaEjecGuardada != "") {
                                FNObtenerListadoSuperintendencias(IdGerenciaEjecGuardada);
                                FNObtenerGerenteEjecucion(IdGerenciaEjecGuardada);
                            }
                        } else {
                            $('#GerenciaEjecucion').append(new Option(value.GerNombre, value.GerToken, false, false));
                        }
                        cuantos2++;
                    });
                }, 500);
                GerenciaEjecucion.append('<option value="-1" selected="true">Seleccionar..</option>');
            }
        });
    }
}

FNObtenerListadoGerenciaModificacion = function (GerInversionGuardado, GerEjecucionGuardado) {
    //PREPARAR

    // var GerInversionGuardado = '@Html.Raw(@ViewBag.IdenGerInver)';
    //var GerEjecucionGuardado = '@Html.Raw(@ViewBag.IdenGerEjec)';
    //var IdGerenciaEjecGuardada = '@Html.Raw(@ViewBag.IdenIdGeren)';

    var GerenciaInversion = $('#GerenciaInversion');
    GerenciaInversion.empty();
    GerenciaInversion.append('<option value="-1" selected="true">Buscando..</option>');

    var GerenciaEjecucion = $('#GerenciaEjecucion');
    GerenciaEjecucion.empty();
    GerenciaEjecucion.append('<option value="-1" selected="true">Buscando..</option>');

    //LISTO
    var DTO = {};
    var cuantos = 0
    var cuantos1 = 0
    $.ajax({
        url: "../../Planificacion/ListarGerencias",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            GerenciaInversion.empty();
            GerenciaEjecucion.empty();
            setTimeout(function () {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                localStorage.setItem("CAPEX_PLAN_GERENCIAINVERSION", "");

                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    GerenciaInversion.append(new Option(value.GerNombre, value.GerToken, false, false));
                    cuantos++;
                });

                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    GerenciaEjecucion.append(new Option(value.GerNombre, value.GerToken, false, false));
                    cuantos1++;
                });

                GerenciaInversion.append('<option value="-1" selected="true">Seleccionar..</option>');
                GerenciaEjecucion.append('<option value="-1" selected="true">Seleccionar..</option>');

                if (cuantos > 0) {
                    if (GerInversionGuardado && GerInversionGuardado != '') {
                        $('#GerenciaInversion').val(GerInversionGuardado);
                        localStorage.setItem("CAPEX_PLAN_GERENCIAINVERSION", GerInversionGuardado);
                    } else {
                        GerenciaInversion.prop('selectedIndex', 0);
                    }
                } else {
                    GerenciaInversion.prop('selectedIndex', 0);
                }

                if (cuantos1 > 0) {
                    if (GerEjecucionGuardado && GerEjecucionGuardado != '') {
                        $('#GerenciaEjecucion').val(GerEjecucionGuardado);
                        localStorage.setItem("CAPEX_PLAN_GERENCIAEJECUCION", GerEjecucionGuardado);
                    } else {
                        GerenciaEjecucion.prop('selectedIndex', 0);
                    }
                } else {
                    GerenciaEjecucion.prop('selectedIndex', 0);
                }

                /* if (cuantos == 1) {
                     GerenciaInversion.prop('selectedIndex', 0);
                 }
                 else if (cuantos > 1) {
                     GerenciaInversion.append('<option selected="true">Gerencia..</option>')
                 }
 
                 if (cuantos1 == 1) {
                     GerenciaEjecucion.prop('selectedIndex', 0);
                 }
                 else if (cuantos1 > 1) {
                     GerenciaEjecucion.append('<option selected="true">Gerencia..</option>')
                 }*/

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
    localStorage.setItem("CAPEX_GERENTE_INVERSION_NOMBRE", "");
    localStorage.setItem("CAPEX_GERENTE_INVERSION_TOKEN", '');
    $("#GerenteInversion").val('');
    //LISTO
    var DTO = { 'token': param_token };
    $.ajax({
        url: "../../Planificacion/ObtenerGerente",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                document.getElementById('linkToLogout').click();
                return;
            }
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
        url: "../../Planificacion/ObtenerGerente",
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
                if (key == "GteNombre") {
                    $("#GerenteEjecucion").val(value);
                    localStorage.setItem("CAPEX_GERENTE_EJECUCION_NOMBRE", value);
                }
                if (key == "GteToken") {
                    localStorage.setItem("CAPEX_GERENTE_EJECUCION_TOKEN", value);
                }
                if (key == "IdGerencia") {
                    console.log("FNObtenerGerenteEjecucion value=", value);
                    localStorage.setItem("CAPEX_PLAN_GERENCIA_ID", value);
                    //FNObtenerListadoSuperintendencias(value);
                }
            });
        }
    });
}
//
// LISTADO DE SUPERINTENDENCIAS
//
FNObtenerListadoSuperintendencias1 = function (IdGerencia, SuperIntendenciaGuardada) {
    console.log("FNObtenerListadoSuperintendencias1 IdGerencia=" + IdGerencia + ", SuperIntendenciaGuardada=" + SuperIntendenciaGuardada);
    if (IdGerencia != null || IdGerencia != "") {
        //PREPARAR
        var Superitendencia = $('#Superitendencia');
        localStorage.setItem("CAPEX_PLAN_GERENCIA_ID", IdGerencia);
        Superitendencia.empty();
        Superitendencia.append('<option value="-1" selected="true">Buscando..</option>');

        //LISTO
        var DTO = {};
        var cuantos = 0
        var encontrado = 0;
        $.ajax({
            url: "../../Planificacion/ListarSuperintendencias",
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
                    Superitendencia.append('<option value="-1" selected="true">Seleccionar..</option>');
                    localStorage.setItem("CAPEX_PLAN_SUPERINTENDENCIA", "");
                    $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                        if (IdGerencia == value.IdGerencia) {
                            if (SuperIntendenciaGuardada == value.CodigoSuper) {
                                Superitendencia.append(new Option(value.SuperNombre, value.CodigoSuper, false, true));
                                FNObtenerIntendente(value.CodigoSuper);
                                localStorage.setItem("CAPEX_PLAN_SUPERINTENDENCIA", value.CodigoSuper);
                                encontrado = 1;
                            } else {
                                Superitendencia.append(new Option(value.SuperNombre, value.CodigoSuper, false, false));
                            }
                            cuantos++;
                        }
                    });
                    if (!encontrado) {
                        //Superitendencia.prop('selectedIndex', 0);
                        $("#Superitendencia").val($("#Superitendencia option:first").val());
                    }
                    console.log("FNObtenerListadoSuperintendencias1 Fin 1 IdGerencia=" + IdGerencia + ", SuperIntendenciaGuardada=" + SuperIntendenciaGuardada);
                }, 500);

            }
        });
    }
    else {
        console.log("FNObtenerListadoSuperintendencias Fin 2 IdGerencia=" + IdGerencia + ", SuperIntendenciaGuardada=" + SuperIntendenciaGuardada);
        return true;
    }
}
//
// LISTADO DE SUPERINTENDENCIAS
//
FNObtenerListadoSuperintendencias = function (IdGerencia) {
    console.log("FNObtenerListadoSuperintendencias IdGerencia=", IdGerencia);
    if (IdGerencia != null || IdGerencia != "") {
        //PREPARAR
        var Superitendencia = $('#Superitendencia');
        Superitendencia.empty();
        Superitendencia.append('<option selected="true">Buscando..</option>');
        //LISTO
        var DTO = {};
        var cuantos = 0
        var encontrado = 0;
        $.ajax({
            url: "../../Planificacion/ListarSuperintendencias",
            type: "GET",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                Superitendencia.empty();
                setTimeout(function () {
                    Superitendencia.append('<option value="-1" selected="true">Seleccionar..</option>')
                    $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                        if (value.IdGerencia == IdGerencia) {
                            Superitendencia.append(new Option(value.SuperNombre, value.CodigoSuper, false, true));
                            encontrado = 1;
                        } else {
                            Superitendencia.append(new Option(value.SuperNombre, value.CodigoSuper, false, false));
                        }
                        cuantos++;
                    });
                    if (!encontrado) {
                        //Superitendencia.prop('selectedIndex', 0);
                        $("#Superitendencia").val($("#Superitendencia option:first").val());
                    }
                    /*if (cuantos == 1) {
                        Superitendencia.prop('selectedIndex', 0);
                    }
                    else if (cuantos > 1) {
                       
                    }*/
                    console.log("FNObtenerListadoSuperintendencias Fin IdGerencia=", IdGerencia);
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
        url: "../../Planificacion/ObtenerIntendente",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                document.getElementById('linkToLogout').click();
                return;
            }
            $("#SuperIntendente").val('');
            $("#Encargado").val("");
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
    var DTO = { 'IdGerencia': gerencia, 'CodigoSuper': superintendencia };
    $.ajax({
        url: "../../Planificacion/ObtenerEncargado",
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
                if (key == "EncNombre") {
                    $("#Encargado").val(value);
                    localStorage.setItem("CAPEX_ENCARGADO_NOMBRE", value);
                }
                if (key == "EncToken") {
                    localStorage.setItem("CAPEX_ENCARGADO_TOKEN", value);
                }
            });
        }
    });
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
        url: "../../Planificacion/ObtenerTokenCompania",
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

FNRealizarValidacionIniciativa = function (DTO) {
    var response = '';
    $.ajax({
        url: "../../Planificacion/ValidarIdentificacion",
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
// ACTUALIZAR IDENTIFICACION
//
FNActualizarIdentificacion = function () {
    if (!$('#form_identificacion').valid()) {
        console.log("validacion incorrecta");
        return false;
    }
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
    if (!PidObjeto || PidObjeto == '') {
        PidObjeto = $("#Objeto").val();
    }
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
    if (PidGerenciaEjecucion == "") {
        PidGerenciaEjecucion = $("#GerenciaEjecucion").val();
    }
    var PidGerenteEjecucion = localStorage.getItem("CAPEX_GERENTE_EJECUCION_TOKEN");

    var PidSuperintendencia = localStorage.getItem("CAPEX_PLAN_SUPERINTENDENCIA");
    var PidSuperintendente = localStorage.getItem("CAPEX_INTENDENTE_TOKEN");

    var PidEncargado = localStorage.getItem("CAPEX_ENCARGADO_TOKEN");

    var PidNombreProyectoAlias = $("#NombreProyectoAlias").val();

    /*********************************** VALIDAR ENTRADA**********************************/
    if (!PidNombreProyectoAlias || PidNombreProyectoAlias == "" || PidNombreProyectoAlias == undefined || PidRequiere == "" || PidProceso == "" || PidObjeto == "" || PidArea == "" || PidCompania == "" || PidEtapa == "" || PidNombreProyecto == "" || PidCodigoIniciativa == "" || PidGerenciaInversion == "" || PidGerenciaEjecucion == "" || !PidSuperintendencia || PidSuperintendencia == "" || PidSuperintendencia == undefined || PidSuperintendencia == "-1") {
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
                                url: "/Planificacion/ActualizarIdentificacionCategorizacion",
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
                                    localStorage.setItem("CAPEX_INICIATIVA_CODIGO", PidCodigoIniciativa);
                                    localStorage.setItem("CAPEX_INICIATIVA_FOLIO", folio);
                                    localStorage.setItem("CAPEX_INICIATIVA_TOKEN", token);
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
                                        FNObtenerListadoNivelIngenieraNoRequiereCorregido(false, "", "");
                                        return false;
                                    }
                                }
                            });
                        } else {
                            return false;
                        }
                    });
                }
            } else {
                swal("Error", "Error al actualizar identificación. Por favor inténtalo de nuevo más tarde!", "error");
                return false;
            }
        }
        if (!capexIniciativaCategorizacionEstado || capexIniciativaCategorizacionEstado == undefined || capexIniciativaCategorizacionEstado == "") {
            FNLimpiarCategorizacion();
        }
        if (flujoNormal) {
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
            console.info("DTO=" + JSON.stringify(DTO));

            $.ajax({
                url: "../../Planificacion/ActualizarIdentificacion",
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
                    localStorage.setItem("CAPEX_INICIATIVA_CODIGO", PidCodigoIniciativa);
                    localStorage.setItem("CAPEX_INICIATIVA_FOLIO", folio);
                    localStorage.setItem("CAPEX_INICIATIVA_TOKEN", token);
                    //PROCESAR RESPUESTA
                    if (estado == "Error") {
                        swal("Error", "No es posible actualizar esta inicativa.", "error");
                        setTimeout(function () {
                            document.location.reload();
                        }, 3000);
                    }
                    else if (estado == "Actualizado") {
                        swal("Exito", "Identificación Actualizada.", "success");
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
        url: "../../Planificacion/ValidarEstadoProyectoRemanenteIdentificacion",
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

FNAprobarVisacion = function () {
    var PidToken = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    var PidUsuario = $("#CAPEX_H_USERNAME").val();

    var DTO = {
        'PidToken': PidToken,
        'PidUsuario': PidUsuario
    };

    console.info("DTO=" + JSON.stringify(DTO));
    swal({
        title: 'Esta seguro?',
        text: "Desea aprobar y enviar la iniciativa al comité ejecutivo?",
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
                url: "/Planificacion/AprobarVisacion",
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
                    //PROCESAR RESPUESTA
                    if (estado == "Error") {
                        swal("Error", "No es posible aprobar esta inicativa.", "error");
                        return false;
                    }
                    else if (estado == "Aprobado") {
                        swal("Exito", "Iniciativa aprobada.", "success");
                        setTimeout(function () {
                            window.location.href = location.protocol + '//' + location.host + "/GestionVisacion";
                        }, 1500);
                    }
                }
            });
        } else {
            return false;
        }
    });
}

FNAprobarRevisionCE = function () {
    var PidToken = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    var PidUsuario = $("#CAPEX_H_USERNAME").val();

    var DTO = {
        'PidToken': PidToken,
        'PidUsuario': PidUsuario
    };

    console.info("DTO=" + JSON.stringify(DTO));
    swal({
        title: 'Esta seguro?',
        text: "Desea aprobar y enviar la iniciativa a revisión amsa?",
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
                url: "/Planificacion/AprobarCE",
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
                    //PROCESAR RESPUESTA
                    if (estado == "Error") {
                        swal("Error", "No es posible aprobar esta inicativa.", "error");
                        return false;
                    }
                    else if (estado == "Aprobado") {
                        swal("Exito", "Iniciativa aprobada.", "success");
                        setTimeout(function () {
                            window.location.href = location.protocol + '//' + location.host + "/GestionIngresada";
                        }, 1500);
                    }
                }
            });
        } else {
            return false;
        }
    });
}

FNAprobarRevisionAMSA = function () {
    var PidToken = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    var PidUsuario = $("#CAPEX_H_USERNAME").val();

    var DTO = {
        'PidToken': PidToken,
        'PidUsuario': PidUsuario
    };

    console.info("DTO=" + JSON.stringify(DTO));
    swal({
        title: 'Esta seguro?',
        text: "Desea aprobar y enviar la iniciativa a aprobada amsa?",
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
                url: "/Planificacion/AprobarAMSA",
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
                    //PROCESAR RESPUESTA
                    if (estado == "Error") {
                        swal("Error", "No es posible aprobar esta inicativa.", "error");
                        return false;
                    }
                    else if (estado == "Aprobado") {
                        swal("Exito", "Iniciativa aprobada.", "success");
                        setTimeout(function () {
                            window.location.href = location.protocol + '//' + location.host + "/GestionEnRevision";
                        }, 1500);
                    }
                }
            });
        } else {
            return false;
        }
    });
}

FNRechazoIniciativaAdmin1 = function () {
    $("#ModalRechazoAdmin1").hide();

    var PidToken = localStorage.getItem("CAPEX_INICIATIVA_TOKEN");
    var PidUsuario = $("#CAPEX_H_USERNAME").val();
    var Observacion = $("#ObservacionRechazoAdmin1").val();

    if (!Observacion || Observacion == undefined || Observacion == "") {
        swal("", "Debe completar el formulario.", "info");
        $("#ModalRechazoAdmin1").show();
        return false;
    }

    var DTO = {
        'PidToken': PidToken,
        'PidUsuario': PidUsuario,
        'Comentario': Observacion
    };
    var opcionRechazo = $("#rechazoAdmin").val();
    var urlController = "/Planificacion/RechazarVisacion"
    var txtController = "Desea rechazar y enviar la iniciativa al gestor?";
    var contexto = "/GestionVisacion";
    if (opcionRechazo == "1") {
        urlController = "/Planificacion/RechazarVisacion";
        txtController = "Desea rechazar y enviar la iniciativa al gestor?";
        contexto = "/GestionVisacion";
    } else if (opcionRechazo == "3") {
        urlController = "/Planificacion/RechazarCE";
        txtController = "Desea rechazar y enviar la iniciativa al administrador?";
        contexto = "/GestionIngresada";
    } else if (opcionRechazo == "4") {
        urlController = "/Planificacion/RechazarAMSA";
        txtController = "Desea rechazar y enviar la iniciativa al administrador?";
        contexto = "/GestionEnRevision";
    }

    console.info("DTO=" + JSON.stringify(DTO));
    swal({
        title: 'Esta seguro?',
        text: txtController,
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
                url: urlController,
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
                    //PROCESAR RESPUESTA
                    if (estado == "Error") {
                        swal("Error", "No es posible rechazar esta inicativa.", "error");
                        return false;
                    }
                    else if (estado == "Rechazado") {
                        swal("Exito", "Iniciativa rechazada.", "success");
                        setTimeout(function () {
                            window.location.href = location.protocol + '//' + location.host + contexto;
                        }, 1500);
                    }
                }
            });
        } else {
            $("#ModalRechazoAdmin1").show();
            return false;
        }
    });
}

FNRechazarVisacion = function () {
    $("#rechazoAdmin").val("1");
    $("#ModalRechazoAdmin1").show();
}

FNRechazarRevisionCE = function () {
    $("#rechazoAdmin").val("3");
    $("#ModalRechazoAdmin1").show();
}

FNRechazarRevisionAMSA = function () {
    $("#rechazoAdmin").val("4");
    $("#ModalRechazoAdmin1").show();
}

FNCerrarRechazoAdmin1 = function () {
    $("#ModalRechazoAdmin1").hide();
    setTimeout(function () {
        window.location.reload(true);
    }, 200);
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
