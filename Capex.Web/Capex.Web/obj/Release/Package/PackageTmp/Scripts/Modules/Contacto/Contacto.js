// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : PLANIFICACION
// FECHA            : MAYO 2018
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// METODOS          : CONTACTO

//
// GUARDAR SELECCION
//
FNGuardarSeleccion = function (tipo, valor) {
    switch (tipo) {
        case "COMBO_GERENCIA": localStorage.setItem("CAPEX_CONTACTO_GERENCIA_ID", valor);
            FNObtenerListadoSuperintendencias();
            break;
        case "COMBO_SUPER": localStorage.setItem("CAPEX_CONTACTO_SUPER_ID", valor);
            break;
        case "COMBO_TIPO": localStorage.setItem("CAPEX_CONTACTO_TIPO", valor);
            break;
    }
    return;
}

//
// LISTADO DE GERENCIAS
//
FNObtenerListadoGerencia = function () {
    //PREPARAR
    var Gerencia = $('#Gerencia');
    Gerencia.empty();
    Gerencia.append('<option selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "Planificacion/ListarGerencias",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            Gerencia.empty();
            setTimeout(function () {
                if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                    document.getElementById('linkToLogout').click();
                    return;
                }
                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    $('#Gerencia').append(new Option(value.GerNombre, value.IdGerencia, false, false));
                    cuantos++;
                });
                if (cuantos == 1) {
                    Gerencia.prop('selectedIndex', 0);
                }
                else if (cuantos > 1) {
                    Gerencia.append('<option selected="true">Seleccionar..</option>')
                }

            }, 500);
        }
    });
}
//
// LISTADO DE SUPER
//
FNObtenerListadoSuperintendencias = function () {
    //PREPARAR
    var IdGerencia = localStorage.getItem("CAPEX_CONTACTO_GERENCIA_ID");
    var Superitendencia = $('#Superintendencia');
    Superitendencia.empty();
    Superitendencia.append('<option selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0;
    var encontrado = 1;
    $.ajax({
        url: "Planificacion/ListarSuperintendencias",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            Superitendencia.empty();
            setTimeout(function () {
                Superitendencia.append('<option value="-1" selected="true">Seleccionar..</option>')
                $.each(JSON && JSON.parse(r) || $.parseJSON(r), function (key, value) {
                    if (parseInt(value.IdGerencia) === parseInt(IdGerencia)) {
                        $('#Superintendencia').append(new Option(value.SuperNombre, value.IdSuper, false, true));
                        encontrado = 1;
                    } else {
                        $('#Superintendencia').append(new Option(value.SuperNombre, value.IdSuper, false, false));
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
                    Superitendencia.append('<option selected="true">Seleccionar..</option>')
                }*/
            }, 500);
        }
    });
}
//
// GUARDAR SOLICITUD
//
FnEnviarSolicitud = function () {
    /*********************************** PARAMETROS BASE ***********************************/
    var GerenciaId = localStorage.getItem("CAPEX_CONTACTO_GERENCIA_ID");
    var SuperintendenciaId = localStorage.getItem("CAPEX_CONTACTO_SUPER_ID");
    var SolTipo = localStorage.getItem("CAPEX_CONTACTO_TIPO");
    var SolNomSolicitante = $("#NombreSolicitante").val();
    var SolApeSolicitante = $("#ApellidoSolicitante").val();
    var SolEmailSolicitante = $("#EmailSolicitante").val();
    var SolOtroTelefono = $("#OtroTelefono").val();
    var SolComentario = $("#Comentarios").val();
    var PidUsuario = $("#CAPEX_H_USERNAME").val();

    /*********************************** VALIDACIONES ***********************************/
    if (SolTipo == "" || SolComentario == "") {
        swal("Error", "Debe completar todos los campos.", "error");
        return false;
    }
    else if (SolComentario.length > 600) {
        swal("Error", "El comentario no puede superar los 600 caracteres.", "error");
        return false;
    }
    else {
        if (GerenciaId == "1") {
            SuperintendenciaId = "1";
        }

        var DTO = {
            'GerenciaId': GerenciaId,
            'SuperintendenciaId': SuperintendenciaId,
            'SolTipo': SolTipo,
            'SolNomSolicitante': SolNomSolicitante,
            'SolApeSolicitante': SolApeSolicitante,
            'SolEmailSolicitante': SolEmailSolicitante,
            'SolOtroTelefono': SolOtroTelefono,
            'SolComentario': SolComentario,
            'PidUsuario': PidUsuario
        };
        $.ajax({
            url: "Contacto/GuardarSolicitud",
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                var estado = JSON.parse(r);
                if (estado == "ERROR") {
                    swal("Error", "No es posible procesar la solicitud.", "error");
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                }
                else if (estado == "GUARDADO") {
                    swal("Exito", "Solicitud enviada exitosamente.", "success");
                    setTimeout(function () {
                        document.location.href = "/Panel";
                    }, 3000)
                }
            }
        });

    }

}
//
// INICIALIZAR
//
$(document).ready(function () {
    //INICIALIZAR
    localStorage.setItem("CAPEX_CONTACTO_GERENCIA_ID", "");
    localStorage.setItem("CAPEX_CONTACTO_GERENCIA_ID", "");
    localStorage.setItem("CAPEX_CONTACTO_SUPER_ID", "");
    localStorage.setItem("CAPEX_CONTACTO_TIPO", "");

    //NOTIFICACIONES
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "showDuration": "400",
        "hideDuration": "1000",
        "timeOut": "7000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
    //CARGA DE DATOS POR DEFECTO
    FNObtenerListadoGerencia();
});
