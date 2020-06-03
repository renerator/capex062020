// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : GESTION
// FECHA            : FEBRERO 2019
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// SECCION          : CAPEX - COMUN GESTION


//
// PAGINACION 
//
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
    $('#iniciativas tr').each(function () {
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
// OBTENER NNUMERO DE INICIATIVAS POR TIPO
//
FNObtenernNumIniciativas = function (cual) {
    //PREPARAR
    var usuario = $("#CAPEX_H_USERNAME").val();
    var DTO = { "usuario": usuario, "cual": cual };
    var cuantos = 0
    $.ajax({
        url: "../../Gestion/ObtenerContador",
        cache: false,
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            var obj = JSON.parse(JSON.stringify(r));
            var cantidad = obj.Resultado[0].Value;
            switch (cual) {
                case "RESUMEN":
                    if (cantidad != null || cantidad != "") {
                        $("#ContenedorContResumenIni").html(cantidad);
                    }
                    else {
                        $("#ContenedorContResumenIni").html("0");
                    }
                    break;
                case "DESARROLLO":
                    if (cantidad != null || cantidad != "") {
                        $("#ContenedorContDesaIni").html(cantidad);
                    }
                    else {
                        $("#ContenedorContDesaIni").html("0");
                    }
                    break;
                case "VISACION":
                    if (cantidad != null || cantidad != "") {
                        $("#ContenedorContVisIni").html(cantidad);
                    }
                    else {
                        $("#ContenedorContVisIni").html("0");
                    }
                    break;
                case "INGRESO":
                    if (cantidad != null || cantidad != "") {
                        $("#ContenedorContIngresadaIni").html(cantidad);
                    }
                    else {
                        $("#ContenedorContIngresadaIni").html("0");
                    }
                    break;
                case "OBSERVADAS":
                    if (cantidad != null || cantidad != "") {
                        $("#ContenedorContComent").html(cantidad);
                    }
                    else {
                        $("#ContenedorContComent").html("0");
                    }
                    break;
                case "REVISION AMSA":
                    if (cantidad != null || cantidad != "") {
                        $("#ContenedorContRevisionIni").html(cantidad);
                    }
                    else {
                        $("#ContenedorContRevisionIni").html("0");
                    }
                    break;
                case "APROBADA AMSA":
                    if (cantidad != null || cantidad != "") {
                        $("#ContenedorContAprobadaAMSAIni").html(cantidad);
                    }
                    else {
                        $("#ContenedorContAprobadaAMSAIni").html("0");
                    }
                    break;
                case "NO APROBADA GAF":
                    if (cantidad != null || cantidad != "") {
                        $("#ContenedorContNoAprobadaGAFIni").html(cantidad);
                    }
                    else {
                        $("#ContenedorContNoAprobadaGAFIni").html("0");
                    }
                    break;
                case "NO APROBADA CE":
                    if (cantidad != null || cantidad != "") {
                        $("#ContenedorContNoAprobadaCEIni").html(cantidad);
                    }
                    else {
                        $("#ContenedorContNoAprobadaCEIni").html("0");
                    }
                    break;
            }
        }
    });
}
//
// REGISTRAR INICIATIVA
//
FNRegistrarIniciativa = function (token) {
    localStorage.setItem("CAPEX_GESTION_INICIATIVA_TOKEN", token);
    return true;
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

            event.preventDefault();
            localStorage.setItem("CAPEX_GESTION_INICIATIVA_TOKEN", iniciativa)
            var url = 'Gestion/VerIniciativa/' + iniciativa;
            window.location.href = url;

            break;
        case "1":
            event.preventDefault();
            localStorage.setItem("CAPEX_GESTION_INICIATIVA_TOKEN", iniciativa)
            var url = 'Gestion/ModificarIniciativa/' + iniciativa;
            window.location.href = url
            break;
        case "2":
            $.ajaxSetup({ cache: false });
            $.ajax({
                url: "Gestion/VerAdjuntos",
                method: "GET",
                data: { "token": iniciativa }
            }).done(function (request) {
                $("#ContenedorElementosAdjuntos").html(request);
                $("#ModalAdjuntos").show();

            }).fail(function (xhr) { console.log('error', xhr); });
            break;
        case "3":
            //document.location.href = '../../Planificacion/PdfPresupuesto?token=' + iniciativa;
            document.location.href = '../../Planificacion/descargaPdfPresupuesto?token=' + iniciativa;
            break;
        case "4":
            $("#Comentario").val("");
            $('#Prioridad>option:eq(0)').prop('selected', true);
            $("#ModalComentar").show();
            break;
        case "5":
            FNAprobarIniciativaSponsor(iniciativa, usuario);
            return true;
            break;
        case "6":
            FNAbrirRechazoSponsor(iniciativa);
            return true;
            break;
        case "7":
            $("#ModalAsignacion").show();
            break;
        case "8":
            FNReconsiderarSponsor(iniciativa);
            return true;
            break;
        case "9":

            break;
        case "10":
            FNAprobarIniciativaAdmin1(iniciativa, usuario);
            return true;
            break;
        case "11":
            $("#ModalRechazoAdmin1").show();
            return true;
            break;
        case "12":
            $("#ModalEliminarIniciativa").show();
            return true;
            break;
        case "14":
            FNAsignarmela(iniciativa);
            return true;
            break;
        case "15":
            FNObtenerUltimoComentario(iniciativa)
            return true;
            break;
        case "16":
            FNObtenerUltimoComentarioCE(iniciativa)
            return true;
            break;
        case "17":
            FNObtenerUltimoComentarioCAMSA(iniciativa)
            return true;
            break;
        case "18":
            FNNoAprobarIniciativaCE(iniciativa);
            return true;
            break;
        case "19":
            FNNoAprobarIniciativaVisacion(iniciativa);
            return true;
            break;
        case "20":
            FNReintegrarIniciativaDesarrollo(iniciativa);
            return true;
            break;
        case "21":
            FNReintegrarIniciativaVisacion(iniciativa);
            return true;
            break;
        case "22":
            FNReintegrarIniciativaVisacionAMSA(iniciativa);
            return true;
            break;
        default:
            alert("todavia no esta lista");
            return true;
            break;
    }
}

//
// ABRIR "VISTA MODIFICAR INICIATIVA"
//
FNModificarIniciativa = function (PidToken) {
    event.preventDefault();
    localStorage.setItem("CAPEX_GESTION_INICIATIVA_TOKEN", PidToken)
    var url = 'Gestion/ModificarIniciativa/' + PidToken;
    window.location.href = url;
}
//
//  VER DOCUMENTO PDF GENERADO
//
VerPdf = function (token) {
    var link = document.createElement('a');
    link.href = 'Files/Downloads/';
    link.download = 'Iniciativa.pdf';
    link.dispatchEvent(new MouseEvent('click'));
}
//
// ACTUALIZAR VISTA 
//
Actualizar = function () {
    document.location.reload(true);
}
//
// CERRAR MODALES 
//
FNCerrarModalAsignar = function () {
    $("#ModalAsignar").hide();
    document.location.reload(true);
}
// CERRAR MODAL ADJUNTOS
FNCerrarModalAdjuntos = function () {
    $("#ModalAdjuntos").hide();
    document.location.reload(true);
}
// CERRAR MODAL ELIMINAR INICIATIVA
FNCerrarModalEliminarIniciativa = function () {
    $("#ModalEliminarIniciativa").hide();
    document.location.reload(true);
}
//
// ASIGNAR REVISORES
//
FNAsignarRevisores = function () {
    swal("", "Revisores asignados", "success");
    setTimeout(function () {
        window.location.reload(true);
    }, 3000)
}
// RESETEAR FORM
FNResetear = function () {
    if (document.GestionFormIndex)
        document.GestionFormIndex.reset();
}
//
//
// INICIALIZADOR
//
$(document).ready(function () {
    //INICIALIZAR VARIABLES
    localStorage.setItem("CAPEX_UI_PANELFILTRO", "");
    localStorage.setItem("CAPEX_GESTION_INICIATIVA_TOKEN", "");
    localStorage.setItem("CAPEX_GESTION_INICIATIVA_USUARIO", "");
    localStorage.setItem("CAPEX_GESTION_CONTENEDOR_VER_SELECCIONADO", "");

    /********************************** CONTABILIZAR INICIATIVAS POR ETAPA**********************/
    setTimeout(function () {
        FNObtenernNumIniciativas('RESUMEN');
    }, 500);
    setTimeout(function () {
        FNObtenernNumIniciativas('DESARROLLO');
    }, 600);
    setTimeout(function () {
        FNObtenernNumIniciativas('VISACION');
    }, 700);
    setTimeout(function () {
        FNObtenernNumIniciativas('INGRESO');
    }, 800);
    setTimeout(function () {
        FNObtenernNumIniciativas('OBSERVADAS');
    }, 900);
    setTimeout(function () {
        FNObtenernNumIniciativas('REVISION AMSA');
    }, 1000);
    setTimeout(function () {
        FNObtenernNumIniciativas('APROBADA AMSA');
    }, 1100);
    setTimeout(function () {
        FNObtenernNumIniciativas('NO APROBADA GAF');
    }, 1200);
    setTimeout(function () {
        FNObtenernNumIniciativas('NO APROBADA CE');
    }, 1300);
    /********************************** FILTRAR TABLA INICIATIVAS *****************************/
    FNPaginar();
    $('#buscador').val('');
    $('#buscador').keyup(function () {
        FNBuscarEnTabla($(this).val());
        $("#paginador").hide();
    });
    /********************************** OCULTAR INDICADOR DE CARGA *****************************/
    jQuery("#AppLoaderContainer").remove();
});
