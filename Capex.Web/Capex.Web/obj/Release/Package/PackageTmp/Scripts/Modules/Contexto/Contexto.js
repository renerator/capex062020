// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : PLANIFICACION
// FECHA            : MAYO 2018
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// METODOS          : CAMBIO DE CLAVE

FNAbrirModalCambioClave = function () {
    $("#ModalActualizarClave").show();
    return false;
}
//
// ACTUALIZADO
//
FNActualizarClave = function () {
    //INGRESO
    var clave1 = $("#Clave1").val();
    var clave2 = $("#Clave2").val();
    var usuario = $("#CAPEX_H_USERNAME").val();
    //VALIDACION
    if (clave1 == "" || clave2 == "") {
        swal("", "Debe ingresar su nueva clave en ambos campos.", "info");
        $("#Clave1").val("");
        $("#Clave2").val("");
        return false;
    }
    else if (clave1 != clave2) {
        swal("", "Las contraseñas propocionadas deben coincidir.", "info");
        $("#Clave1").val("");
        $("#Clave2").val("");
        return false;
    }
    else {
        //PREPARAR
        var DTO = {
            'Clave1': clave1,
            'Clave2': clave2,
            'Usuario': usuario
        };
        $.ajax({
            url: window.location.origin + '/Login/ActualizarClave',
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {

                //PARSEAR RESPUESTA
                var estructura = JSON.parse(JSON.stringify(r));
                //PROCESAR RESPUESTA
                if (estructura.Mensaje == "Error") {
                    swal("", "No es posible actualizar la contraseña en estos momentos.", "error");
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                }
                else if (estructura.Mensaje == "Actualizado") {
                    swal("", "La contraseña fue actualizada correctamente.", "success");
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                }
            }
        });
    }
}
// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : PANEL
// FECHA            : MAYO 2018
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// METODOS          : AUXILIARES
//
//
FNRegistrarEventoIngresoIniciativa = function () {
    localStorage.setItem("CAPEX_MENU_INGRESAR_INICIATIVA", "SI");
    document.location = window.location.origin + '/Panel/Index';
    return;
}

