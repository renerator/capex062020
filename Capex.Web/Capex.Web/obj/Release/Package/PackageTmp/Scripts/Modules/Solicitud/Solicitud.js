// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : PLANIFICACION
// FECHA            : MAYO 2018
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// METODOS          : SOLICITUD

//
//VER SOLICITUD
//

FNVerSolicitud = function(SolComentario, SolNomSolicitante, SolApeSolicitante, SolTelefono, SolMovil, SolOtroTelefono, SolEmailSolicitante, SolFecha) {
    document.getElementById('ContenedorFormVersolicitud').reset();
    $("#ModalVerSolicitud").show();
    $("#Nombre").html(SolNomSolicitante);
    $("#Apellido").html(SolApeSolicitante);
    $("#Telefono").html(SolTelefono);
    $("#OtroTelefono").html(SolOtroTelefono);
    $("#Movil").html(SolMovil);
    $("#Email").html(SolEmailSolicitante);
    $("#Fecha").html(SolFecha);
    $("#Solicitud").html(SolComentario);

}
//
//CERRAR SOLICITUD
//
FNCerrarModalVerSolicitud = function () {
    $("#ModalVerSolicitud").hide();
}

//
// COMUNES PAGIACION Y FILTRO
//
FNPaginar = function () {
    $('td', 'table').each(function (i) { });
    $('table.paginated').each(function () {
        var currentPage = 0;
        var numPerPage = 6;
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
    $('#solicitudes tr').each(function () {
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
// INICIALIZADOR
//
$(document).ready(function () {
    jQuery("#AppLoaderContainer").hide();
    FNPaginar();
    $('#buscador').keyup(function () {
        FNBuscarEnTabla($(this).val());
        $("#paginador").hide();
    });
});