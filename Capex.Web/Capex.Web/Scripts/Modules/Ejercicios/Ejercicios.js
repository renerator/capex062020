// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : GESTION
// FECHA            : FEBRERO 2019
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// SECCION          : CAPEX - EJERCICIOS OFICIALES
//
// PAGINACION 
//


var banderaMostrarOcultarFiltros = 0;
var filtroGetData = {};
var filtrosSeleccionados = [];
var filtroUtilizado = false;
var iniciativasSeleccionadas = [];

FNDescargaMasiva = function () {
    console.log("Descargar el pdf");
    $("#idDescargaMasiva").prop("disabled", true);
    var numFiles = 0;
    if (iniciativasSeleccionadas && iniciativasSeleccionadas.length > 0) {
        var files = [];
        iniciativasSeleccionadas.forEach(function (entry) {
            console.log("FNDescargaMasiva entry=", entry);
            files.push('../../../Planificacion/descargaPdfPresupuesto?token=' + entry);
            numFiles++;
        });
        downloadAll(files);
    }
    setTimeout(function () {
        $("#idDescargaMasiva").prop("disabled", false);
    }, (numFiles * 3000));
    console.log("Descargar el pdf final");
}

function downloadAll(files) {
    if (files.length == 0) {
        return;
    }
    file = files.pop();
    var theAnchor = $('<a />')
        .attr('href', file)
        .attr('download', file)
        // Firefox does not fires click if the link is outside
        // the DOM
        .appendTo('body');

    theAnchor[0].click();
    theAnchor.remove();
    downloadAll(files);
}

deseleccionarTodoAlSalir = function () {
    $("#idDescargaMasiva").prop("disabled", false);
    $('#botonDescargarMasivaPdf').hide();
    iniciativasSeleccionadas = [];
    $("#defaultCheckedAll").prop("indeterminate", false);
    $("#defaultCheckedAll").prop("checked", false);
}

deseleccionarTodo = function (page) {
    page = page || 1;
    var pageChange = false;
    var start = (page - 1) * 10;
    var end = start + 10;
    console.log("deseleccionarTodo page=" + page + ", start=" + start + ", end=" + end);
    $("#ejercicios tr").each(function (index, value) {
        var item = $(this);
        if (index > 0) {
            if (index <= start || index > end) {
                console.log("deseleccionarTodo evaluando fila index=", index);
                var arrayValues = $(this).find("td:eq(0)").find('input[type=hidden]').val().split(",");
                var selectorValue = $('#' + arrayValues[0]).is(':checked');
                if (selectorValue != undefined && selectorValue === true) {
                    console.log("deseleccionarTodo fila seleccionada index=", index);
                    pageChange = true;
                    return false;
                }
            }
        }
    });
    console.log("deseleccionarTodo pageChange=" + pageChange);
    if (pageChange) {
        $('#botonDescargarMasivaPdf').hide();
        iniciativasSeleccionadas = [];
        $("#defaultCheckedAll").prop("indeterminate", false);
        $("#defaultCheckedAll").prop("checked", false);
        $('.styled').each(function (index, value) {
            this.checked = false;
        });
    }
}

seleccionarTodo = function (param) {
    $("#ejercicios tr").each(function (index, value) {
        var tr = $(this);
        if (value.style.display === '' && index > 0) {
            var arrayValues = $(this).find("td:eq(0)").find('input[type=hidden]').val().split(",");
            $("#" + arrayValues[0]).prop('checked', param);
            seleccionarIniciativaPdf(arrayValues[0], arrayValues[1], "1");
        }
    });
}

seleccionarIniciativaPdf = function (selector, token, masiva) {
    console.log("seleccionarIniciativaPdf selector=" + selector + ", token=" + token + ", masiva=" + masiva);
    var selectorValue = $('#' + selector).is(':checked');
    console.log("selectorValue=", selectorValue);
    if (selectorValue != undefined) {
        if (selectorValue === true) {
            iniciativasSeleccionadas.push(token);
        } else if (selectorValue === false) {
            var filtered = iniciativasSeleccionadas.filter(function (value, index, arr) { return value != token; });
            iniciativasSeleccionadas = filtered;
        }
    }
    if (!masiva || masiva == undefined || masiva == "") {
        if (iniciativasSeleccionadas && iniciativasSeleccionadas.length > 0) {
            $("#defaultCheckedAll").prop("indeterminate", true);
        } else if (iniciativasSeleccionadas && iniciativasSeleccionadas.length == 0) {
            $("#defaultCheckedAll").prop("indeterminate", false);
            $("#defaultCheckedAll").prop("checked", false);
        }
    }

    if (iniciativasSeleccionadas && iniciativasSeleccionadas.length > 0) {
        $('#botonDescargarMasivaPdf').show();
    } else if (iniciativasSeleccionadas && iniciativasSeleccionadas.length == 0) {
        $('#botonDescargarMasivaPdf').hide();
    } else {
        $('#botonDescargarMasivaPdf').hide();
    }
}


FNResizeScroll = function () {
    var rowLength = $('#ejercicios >tbody >tr').filter(function () {
        return !$(this).attr('style');
    }).length;
    console.log("rowLength=", rowLength);
    if (rowLength > 6) {
        $('#accordion').attr("style", "width:300px;height:800px;overflow:auto");
    } else {
        $('#accordion').attr("style", "width:300px;height:400px;overflow:auto");
    }
}

FNGotoPage = function (page, perPage) {
    exitFilterPanel();
    deseleccionarTodo(page);
    var startAt = (page - 1) * perPage,
        endOn = startAt + perPage;

    startAt++;
    endOn++;
    console.log("FNGotoPage startAt=" + startAt + ", endOn=" + endOn);
    $("#ejercicios tr").each(function (index, value) {
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
    FNResizeScroll();
}

FNRemoverFiltro = function (index) {
    console.log("FNRemoverFiltro index=", index);
    if (filtrosSeleccionados && filtrosSeleccionados.length > 0) {
        if (index >= 0 && index < filtrosSeleccionados.length) {
            var filtroSeleccionado = filtrosSeleccionados[index];
            $("#" + filtroSeleccionado.selector).prop("checked", false);
            var arrValues = filtroGetData[filtroSeleccionado.type];
            var filtered = arrValues.filter(function (_value, _index, _arr) {
                return _value != filtroSeleccionado.id;
            });
            if (!filtered || filtered == undefined) {
                filtered = [];
            }
            filtroGetData[filtroSeleccionado.type] = filtered;
            filtrosSeleccionados.splice(index, 1);
            if (filtrosSeleccionados.length < 5) {
                $("#accordion").find("input").prop("disabled", false);
            }
            FNDrawSelectedFilters();
            if (filtroUtilizado) {
                FNFilterActionGetData(false);
            }
        }
    }
    FNResizeScroll();
}

FNDrawSelectedFilters = function () {
    console.log("FNDrawSelectedFilters");
    $("#contenedor_refinement").html('');
    var filtersSpan = "";
    if (filtrosSeleccionados && filtrosSeleccionados.length > 0) {
        filtersSpan = "<ul class='list-group'>";
        for (var i = 0; i < filtrosSeleccionados.length; i++) {
            filtersSpan += "<li class='list-group-item d-flex justify-content-between align-items-center pt-0 pb-0' style='background-color:#003a71;color:#fff;'>";
            filtersSpan += filtrosSeleccionados[i].name;
            filtersSpan += "<span style='cursor:pointer' class='badge text-white badge-pill' onclick='FNRemoverFiltro(" + i + ")'><h5>&times;</h5></span>";
            filtersSpan += "</li>";
        }
        filtersSpan += "</ul>";
    }
    if (filtersSpan.length > 0) {
        $("#contenedor_refinement").html(filtersSpan);
        $('#buttonFilterAction').prop('disabled', false);
    } else {
        $('#buttonFilterAction').prop('disabled', true);
    }
}

FNGetDataRadio = function (id, sigla, key, value, itemName) {
    console.log('llamando FNGetDataRadio');
    var radioValue = $("input[name='" + sigla + "']:checked").val();
    console.log('FNGetDataRadio radioValue=', radioValue);
    var arrValues = [value];
    filtroGetData[sigla] = arrValues;
    var matchOk = false;
    for (var i = 0; i < filtrosSeleccionados.length; i++) {
        if (filtrosSeleccionados[i].type == sigla) {
            matchOk = true;
            filtrosSeleccionados[i] = { id: value, name: itemName, type: sigla, selector: id };
            break;
        }
    }
    if (!matchOk) {
        filtrosSeleccionados.push({ id: value, name: itemName, type: sigla, selector: id });
    }
    FNDrawSelectedFilters();
    if (!filtrosSeleccionados || filtrosSeleccionados == undefined || filtrosSeleccionados.length == 0) {
        if (filtroUtilizado) {
            FNFilterActionGetData(false);
        }
    } else {
        if (filtrosSeleccionados.length == 5) {
            $("#accordion").find("input").prop("disabled", true);
        }
    }
    FNResizeScroll();
}

FNGetData = function (id, sigla, key, value, itemName) {
    console.log('llamando FNGetData');
    var checkValue = $('#' + id).is(":checked");
    if (checkValue) {
        filtrosSeleccionados.push({ id: value, name: itemName, type: sigla, selector: id });
        if (filtroGetData[sigla] === undefined) {
            var arrValues = [value];
            filtroGetData[sigla] = arrValues;
        } else {
            var arrValues = filtroGetData[sigla];
            arrValues.push(value);
            filtroGetData[sigla] = arrValues;
        }
    } else {
        var filtrosFilter = filtrosSeleccionados.filter(function (filtro) {
            return filtro.id != value;
        });
        filtrosSeleccionados = filtrosFilter;
        var arrValues = filtroGetData[sigla];
        var filtered = arrValues.filter(function (_value, _index, _arr) {
            return _value != value;
        });
        filtroGetData[sigla] = filtered;
    }
    FNDrawSelectedFilters();
    if (!filtrosSeleccionados || filtrosSeleccionados == undefined || filtrosSeleccionados.length == 0) {
        if (filtroUtilizado) {
            FNFilterActionGetData(false);
        }
    } else {
        if (filtrosSeleccionados.length == 5) {
            $("#accordion").find("input").prop("disabled", true);
        }
    }
    FNResizeScroll();
}

FNFilterActionGetData = function (load) {
    console.log('FNFilterActionGetData');
    deseleccionarTodoAlSalir();
    filtroUtilizado = true;
    if (load) {
        $('#AppLoaderContainer').show();
    }
    $.ajaxSetup({ cache: false });
    $.ajax({
        url: "Ejercicios/getData",
        method: "POST",
        data: { "filtroGetData": JSON.stringify(filtroGetData) }
    }).done(function (r) {
        if (load) {
            $('#AppLoaderContainer').hide();
        }
        var obj = JSON.parse(JSON.stringify(r));
        console.log('done', obj);
        if (r && r.success && r.tableTrs) {
            $("#TBodyIdSummary").html(r.tableTrs);
            $("#paginador").html(r.paginator);
        } else {
            $("#paginador").html('');
        }
        if (!filtrosSeleccionados || filtrosSeleccionados == undefined || filtrosSeleccionados.length == 0) {
            $("#iconFilter").html("");
        } else {
            $("#iconFilter").html("<span class='glyphicon glyphicon-filter' style='font-size:15px;color:#1296ff'></span>");
        }
        FNResizeScroll();
        console.log('FNFilterActionGetData done');
    }).fail(function (xhr) {
        if (load) {
            $('#AppLoaderContainer').hide();
        }
        console.log('FNFilterActionGetData fail');
        console.log('error', xhr);
        $("#TBodyIdSummary").html('');
        $("#paginador").html('');
        $("#iconFilter").html("");
    });
}

FNFilterData = function () {
    console.log('llamando FNFilterData');
    $('#idMostrarOcultarFiltros').prop('disabled', true);
    $.ajaxSetup({ cache: false });
    $.ajax({
        url: "Ejercicios/getTreeFilter",
        method: "GET"
    }).done(function (r) {
        //var obj = JSON.parse(JSON.stringify(r));
        //console.log('done', obj);
        if (r && r.message && r.message.length > 0) {
            $('#idMostrarOcultarFiltros').prop('disabled', false);
            $('#accordion').html(r.message);
        } else {
            $('#accordion').html('');
        }
        FNResizeScroll();
    }).fail(function (xhr) {
        var error = JSON.parse(JSON.stringify(xhr));
        console.log('error', error);
        $('#accordion').html('');
    });
}

FNChangeIcon = function (idSelector, current, total) {
    console.log('FNChangeIcon = ', idSelector);
    for (var i = 1; i <= total; i++) {
        if (i != current) {
            $('#' + idSelector + i).removeClass('glyphicon-minus');
            $('#' + idSelector + i).addClass('glyphicon-plus');
        }
    }
    if ($('#' + idSelector + current).hasClass('glyphicon-plus')) {
        $('#' + idSelector + current).removeClass('glyphicon-plus');
        $('#' + idSelector + current).addClass('glyphicon-minus');
    } else {
        $('#' + idSelector + current).removeClass('glyphicon-minus');
        $('#' + idSelector + current).addClass('glyphicon-plus');
    }
}

FNMostrarOcultarFiltros = function () {
    console.log('FNMostrarOcultarFiltros');
    if (banderaMostrarOcultarFiltros == 0) {
        showFilterPanel();
    } else {
        exitFilterPanel();
    }
}

showFilterPanel = function () {
    banderaMostrarOcultarFiltros = 1;
    $("#menuFilter").css("opacity", "0");
    $("#lgMenuFilter").addClass("enter");
    $("#idMostrarOcultarFiltros").html("Ocultar Filtros");
}

exitFilterPanel = function () {
    banderaMostrarOcultarFiltros = 0;
    $("#lgMenuFilter").removeClass("enter");
    $("#menuFilter").css("opacity", "1");
    $("#idMostrarOcultarFiltros").html("Mostrar Filtros");
}

FNPaginar = function () {
    $('td', 'table').each(function (i) { });
    $('table.paginated').each(function () {
        var currentPage = 0;
        var numPerPage = 10;
        var $table = $(this);
        $table.bind('repaginate', function () {
            $table.find('tbody tr').hide().slice(currentPage * numPerPage, (currentPage + 1) * numPerPage).show();
        });
        $table.trigger('repaginate');
        var numRows = $table.find('tbody tr').length;
        var numPages = Math.ceil(numRows / numPerPage);
        var $pager = $('<div id="paginador" class="pager"></div>');
        for (var page = 0; page < numPages; page++) {
            $('<span class="page-number"></span>').text(page + 1).bind('click', {
                newPage: page
            }, function (event) {
                $("#buscador").val('');
                currentPage = event.data['newPage'];
                $table.trigger('repaginate');
                $(this).addClass('active').siblings().removeClass('active');
            }).appendTo($pager).addClass('clickable');
        }
        $pager.insertAfter($table).find('span.page-number:first').addClass('active');
    });
}
//
// FILTRO BUSQUEDA TABLA USUARIOS
//
FNBuscarEnTabla = function (value) {
    $('#ejercicios tr').each(function () {
        var found = 'false';
        $(this).each(function () {
            if ($(this).text().toLowerCase().indexOf(value.toLowerCase()) >= 0) {
                found = 'true';
            }
        });
        if (found == 'true') {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });
}

FNEvaluarEstadoFiltro = function (valor) {
    var contenido = valor;
    if (contenido.length > 1) {
        console.log(contenido);
        $("#paginador").hide();
    }
    else {
        console.log(contenido);
        $("#paginador").html("");
        FNPaginar();
        $("#paginador").show();
    }
}
//
// ACCIONES SOBRE ESTADO DEL WORKFLOW
//
FNEvaluarAccion = function (accion, token) {
    var iniciativa = token;
    var usuario = $("#CAPEX_H_USERNAME").val();
    localStorage.setItem("CAPEX_GESTION_INICIATIVA_USUARIO", usuario);
    switch (accion) {
        case "0":
            event.preventDefault();
            localStorage.setItem("CAPEX_GESTION_INICIATIVA_TOKEN", iniciativa)
            var url = '../../Gestion/VerIniciativa/' + iniciativa;
            window.location.href = url;

            break;
        case "2":
            $.ajaxSetup({ cache: false });
            $.ajax({
                url: "../../Gestion/VerAdjuntos",
                method: "GET",
                data: { "token": iniciativa }
            }).done(function (request) {
                $("#ContenedorElementosAdjuntos").html(request);
                $("#ModalAdjuntos").show();
            }).fail(function (xhr) { console.log('error', xhr); });
            break;
        case "3":
            event.preventDefault();
            //document.location.href = '../../Planificacion/PdfPresupuesto?token=' + iniciativa;
            document.location.href = '../../Planificacion/descargaPdfPresupuesto?token=' + iniciativa;
            //document.location.reload();
            break;
        case "23":
            $('#AppLoaderContainer').show();
            event.preventDefault();
            var url = 'Orientacion/ListarIniciativas/' + iniciativa;
            window.location.href = url;
            break;
        case "24":
            swal({
                title: 'Está seguro?',
                text: "Se generará Ejercicio Oficial con la versión de parámetros comerciales " + $("#versionSeleccionada").val() + ". Está seguro que desea continuar?",
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
                    /*setTimeout(function () {
                        $('#AppLoaderContainer').hide();
                        $(".orderselect").val('-1').prop('selected', true);
                        swal("", "Ejercicio oficial generado correctamente para la versión " + $("#versionSeleccionada").val(), "success");
                        setTimeout(function () {
                            window.location.reload(true);
                        }, 3000);
                    }, 3000);*/
                    $.ajaxSetup({ cache: false });
                    $.ajax({
                        url: "/Orientacion/GenerarEjercicionOficial",
                        method: "POST",
                        data: { "ParametroVNToken": token }
                    }).done(function (r) {
                        $('#AppLoaderContainer').hide();
                        $(".orderselect").val('-1').prop('selected', true);
                        if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                            document.getElementById('linkToLogout').click();
                            return;
                        }
                        console.log("Done generar ejercicio oficial JSON.stringify(r)=", JSON.stringify(r));
                        var obj = JSON.parse(JSON.stringify(r));
                        if (obj.Mensaje.startsWith("Guardado")) {
                            swal("", "Ejercicio Oficial generada con la versión " + $("#versionSeleccionada").val() + ".", "success");
                            setTimeout(function () {
                                window.location.reload(true);
                            }, 3000);
                        } else {
                            swal("", "Problemas al generar el ejercicio oficial.", "error");
                            setTimeout(function () {
                                window.location.reload(true);
                            }, 3000);
                        }
                    }).fail(function (xhr) {
                        console.log("fail GenerarParametroV0");
                        $('#AppLoaderContainer').hide();
                        $(".orderselect").val('-1').prop('selected', true);
                        console.log('error', xhr);
                    });
                } else {
                    $(".orderselect").val('-1').prop('selected', true);
                    /*setTimeout(function () {
                        document.location.reload();
                    }, 2500);*/
                    return false;
                }
            });
            break;
    }
}
// CERRAR MODAL ADJUNTOS
FNCerrarModalAdjuntos = function () {
    $("#ModalAdjuntos").hide();
    document.location.reload(true);
}
// RESETEAR FORM
FNResetear = function () {
    document.EjercicioFormIndex.reset();
}

Actualizar = function () {
    $("#AppLoaderContainer").show();
    document.location.reload(true);
}

FNDescargarAdjuntoFinal = function (token) {
    var link = document.createElement("a");
    console.info("token=", token);
    $.ajax({
        url: "/Documentacion/DescargarDocumentoAdjuntoFinal/" + token,
        method: "GET",
        data: { "token": token },
        async: false
    }).done(function (r) {
        if (r && r.IsSuccess && r.ResponseData) {
            console.log("r.ResponseData=", r.ResponseData);
            document.location.href = r.ResponseData;
        }
    }).fail(function (xhr) {
        console.log('fail error', xhr);
    });
    return;
}

//
// INICIALIZADOR
//
$(document).ready(function () {
    /********************************** ALMACENAMIENTO LOCAL *****************************/
    localStorage.setItem("CAPEX_UI_PANELFILTRO", "");
    localStorage.setItem("CAPEX_GESTION_INICIATIVA_TOKEN", "");
    localStorage.setItem("CAPEX_GESTION_INICIATIVA_USUARIO", "");
    localStorage.setItem("CAPEX_GESTION_CONTENEDOR_VER_SELECCIONADO", "");
    /**********************************         LOADER       *****************************/
    jQuery("#AppLoaderContainer").hide();
    /********************************** CONTROL DE EVENTO SUBMIT *****************************/
    $("#EjercicioFormIndex").submit(function (e) {
        return false;
    });
    /********************************** PRE-CARGA DE DATOS *****************************/
    FNPaginar();
    /********************************** FILTRAR TABLA USUARIOS *****************************/
    $('#buscador').keyup(function () {
        FNBuscarEnTabla($(this).val());
        $("#paginador").hide();
    });


});
