// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : GESTION
// FECHA            : FEBRERO 2019
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// SECCION          : CAPEX MODULO - VISTA VISACION
//

//
// APROBACION INICIATIVA
//

var banderaMostrarOcultarFiltros = 0;
var filtroGetData = {};
var filtrosSeleccionados = [];
var filtroUtilizado = false;
var iniciativasSeleccionadas = [];

//
// REGISTRAR INICIATIVA
//
FNRegistrarIniciativa = function (token) {
    localStorage.setItem("CAPEX_GESTION_INICIATIVA_TOKEN", token);
    return true;
}

// CERRAR MODAL ADJUNTOS
FNCerrarModalAdjuntos = function () {
    $("#ModalAdjuntos").hide();
    document.location.reload(true);
}

//
// ACTUALIZAR VISTA 
//
Actualizar = function () {
    $("#AppLoaderContainer").show();
    document.location.reload(true);
}

//
// ACCIONES SOBRE ESTADO DEL WORKFLOW
//
FNEvaluarAccion = function (accion) {
    var iniciativa = localStorage.getItem("CAPEX_GESTION_INICIATIVA_TOKEN");
    var usuario = $("#CAPEX_H_USERNAME").val();
    localStorage.setItem("CAPEX_GESTION_INICIATIVA_USUARIO", usuario);
    switch (accion) {
        case "0":
            $('#AppLoaderContainer').show();
            event.preventDefault();
            localStorage.setItem("CAPEX_GESTION_INICIATIVA_TOKEN", iniciativa)
            var url = '/Gestion/VerIniciativa/' + iniciativa;
            window.location.href = url;
            break;
        case "2":
            $.ajaxSetup({ cache: false });
            $('#AppLoaderContainer').show();
            $.ajax({
                url: "/Gestion/VerAdjuntos",
                method: "GET",
                data: { "token": iniciativa }
            }).done(function (request) {
                $('#AppLoaderContainer').hide();
                $("#ContenedorElementosAdjuntos").html(request);
                $("#ModalAdjuntos").show();
            }).fail(function (xhr) {
                $('#AppLoaderContainer').hide();
                console.log('error', xhr);
            });
            break;
        case "3":
            $('#AppLoaderContainer').show();
            document.location.href = '/Planificacion/descargaPdfPresupuesto?token=' + iniciativa;
            setTimeout(function () {
                $('#AppLoaderContainer').hide();
                setTimeout(function () {
                    FNLimpiarSelect();
                }, 2000);
            }, 6000);
            break;
        case "4":
            $("#Comentario").val("");
            $('#Prioridad>option:eq(0)').prop('selected', true);
            $("#ModalComentar").show();
            break;
        default:
            alert("todavia no esta lista");
            return true;
            break;
    }
}

FNDescargaMasiva = function () {
    console.log("Descargar el pdf");
    $('#AppLoaderContainer').show();
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
        $('#AppLoaderContainer').hide();
    }, 7000);
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

FNDescargaMasivaXls = function () {
    console.log("Descargar el xls");
    $('#AppLoaderContainer').show();
    $("#idDescargaMasivaXLS").prop("disabled", true);
    var numFiles = 0;
    if (iniciativasSeleccionadas && iniciativasSeleccionadas.length > 0) {
        var files = [];
        iniciativasSeleccionadas.forEach(function (entry) {
            console.log("FNDescargaMasivaXls entry=", entry);
            files.push('../../../Planificacion/descargaExcelPresupuesto?token=' + entry);
            numFiles++;
        });
        downloadAllXls(files);
    }
    setTimeout(function () {
        $('#AppLoaderContainer').hide();
    }, 1000);
    setTimeout(function () {
        $("#idDescargaMasivaXLS").prop("disabled", false);
    }, (numFiles * 3000));
    console.log("Descargar el xls final");
}

function downloadAllXls(files) {
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
    downloadAllXls(files);
}

deseleccionarTodoAlSalir = function () {
    $("#idDescargaMasiva").prop("disabled", false);
    $("#idDescargaMasivaXLS").prop("disabled", false);
    $('#botonDescargarMasivaPdf').hide();
    $('#botonDescargarMasivaXLS').hide();
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
    $("#iniciativas tr").each(function (index, value) {
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
        $('#botonDescargarMasivaXLS').hide();
        iniciativasSeleccionadas = [];
        $("#defaultCheckedAll").prop("indeterminate", false);
        $("#defaultCheckedAll").prop("checked", false);
        $('.styled').each(function (index, value) {
            this.checked = false;
        });
    }
}

seleccionarTodo = function (param) {
    $("#iniciativas tr").each(function (index, value) {
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
        $('#botonDescargarMasivaXLS').show();
    } else if (iniciativasSeleccionadas && iniciativasSeleccionadas.length == 0) {
        $('#botonDescargarMasivaPdf').hide();
        $('#botonDescargarMasivaXLS').hide();
    } else {
        $('#botonDescargarMasivaPdf').hide();
        $('#botonDescargarMasivaXLS').hide();
    }
}

FNResizeScroll = function () {
    var rowLength = $('#iniciativas >tbody >tr').filter(function () {
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
    $("#iniciativas tr").each(function (index, value) {
        var item = $(this);
        if (index > 0) {
            if (index >= startAt && index < endOn) {
                var lastTd = item.find(':last-child');
                var valueStyle = "";
                console.log("FNGotoPage lastTd=", lastTd);
                if (lastTd && lastTd != undefined) {
                    var dropDown = lastTd.find('select');
                    console.log("FNGotoPage dropDown=", dropDown);
                    if (dropDown && dropDown != undefined) {
                        dropDown.find('option').each(function (index, element) {
                            if (element.value == "20") {
                                valueStyle = "background-color:#f4a90d;color:#ffffff";
                                return false;
                            } else if (element.value == "22") {
                                valueStyle = "background-color:#f4a90d;Red:#ffffff";
                                return false;
                            }
                        });
                    }
                }
                item.attr("style", valueStyle);
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
        url: "Orientacion/getData",
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
            $("#ContenedorContVisIni").html(r.countBadge);
        } else {
            $("#paginador").html('');
            $("#ContenedorContVisIni").html('0');
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
        $("#ContenedorContVisIni").html('0');
        $("#iconFilter").html("");
    });
}

FNFilterData = function () {
    console.log('llamando FNFilterData');
    $('#idMostrarOcultarFiltros').prop('disabled', true);
    $.ajaxSetup({ cache: false });
    $.ajax({
        url: "Orientacion/getTreeFilter",
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

FNLimpiarSelect = function () {
    console.log("FNLimpiarSelect desde Listar iniciativas");
    setTimeout(function () {
        console.log("setTimeout 500");
        $('#iniciativas > tbody  > tr').each(function (index, tr) {
            if (!$(tr).attr('style') || $(tr).attr('style').includes('background-color')) {
                var selectElm = $("td:last-child", tr).children("select");
                selectElm.val(-1);
            }
        });
    }, 500);
}