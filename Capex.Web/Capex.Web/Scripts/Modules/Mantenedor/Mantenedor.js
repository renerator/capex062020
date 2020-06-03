// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : GESTION
// FECHA            : FEBRERO 2019
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// SECCION          : CAPEX MODULO - MANTENEDOR
// METODOS          : USUARIO
//


//
// ACTUALIZAR VISTA
//
Actualizar = function () {
    document.location.reload(true);
}
//
// LISTADO DE COMPANIAS
//
FNObtenerListadoCompanias = function () {
    //PREPARAR
    var Compania = $('#ComToken');
    Compania.empty();
    Compania.append('<option selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "/Planificacion/ListarCompanias",
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
                        $('#ComToken').append(new Option(value.ComNombre, value.ComToken, false, true));
                    }
                    else {
                        $('#ComToken').append(new Option(value.ComNombre, value.ComToken, false, false));
                    }
                    cuantos++;
                });
                if (cuantos == 1) {
                    Compania.prop('selectedIndex', 0);
                }
                else if (cuantos > 1) {
                    Compania.append('<option selected="true">Compania..</option>')
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
    var Area = $('#AreaToken');
    Area.empty();
    Area.append('<option selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: "/Planificacion/ListarAreas",
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
                    $('#AreaToken').append(new Option(value.AreaNombre, value.AreaToken, false, false));
                    cuantos++;
                });
                if (cuantos == 1) {
                    Area.prop('selectedIndex', 0);
                }
                else if (cuantos > 1) {
                    Area.append('<option selected="true">Area..</option>')
                }
            }, 500);
        }
    });
}
//
// GUARDAR NOMBRE DE AREA REVISORA
//
FNGuardarNombreAreaRevisora = function (valor) {
    var nombre = $("#ArevToken").find('option:selected').text();
    localStorage.setItem("CAPEX_USR_NOMBRE_AREA_REVISORA", nombre);
    return;
}
//
// CREAR USUARIO
//
FNCrearUsuario = function () {

    /*********************************** PARAMETROS BASE ***********************************/
    var UserName = $("#UserName").val();
    var Email = $("#Email").val();
    var Password = $("#Password").val();
    var Status = "1"
    var ComToken = $("#ComToken").val();
    var AreaToken = $("#AreaToken").val();
    var IdEmpresa = "6"
    var UsuTipo = $("#RoleID").val();
    var UsuRut = $("#UsuRut").val();
    var UsuNombre = $("#UsuNombre").val();
    var UsuApellido = $("#UsuApellido").val();
    var UsuEmail = $("#Email").val();
    var UsuTelefono = $("#UsuTelefono").val();
    var UsuMovil = $("#UsuMovil").val();
    var UsuImagen = "";
    var GrvUser = $("#UserName").val();
    var GrvUserToken = ""; //PROVISTO POR NEGOCIO
    var GrvAreaRevToken = $("#ArevToken").val();
    var GrvAreaRevNombre = localStorage.getItem("CAPEX_USR_NOMBRE_AREA_REVISORA");
    var UserID = "";//PROVISTO POR NEGOCIO
    var RoleID = $("#RoleID").val();

    /*********************************** VALIDAR ENTRADA**********************************/
    if (GrvAreaRevNombre == "Seleccionar.." || GrvAreaRevNombre == "" || RoleID == "-1" || RoleID == "" || ComToken == "" || ComToken == "-1" || AreaToken == "-1" || AreaToken == "") {
        swal("", "Debe seleccionar y copletar todos los campos.", "info");
        return false;
    }
    else {
        //PREPARAR
        var DTO = {
            'UserName': UserName,
            'Email': Email,
            'Password': Password,
            'Status': Status,
            'ComToken': ComToken,
            'AreaToken': AreaToken,
            'IdEmpresa': IdEmpresa,
            'UsuTipo': UsuTipo,
            'UsuRut': UsuRut,
            'UsuNombre': UsuNombre,
            'UsuApellido': UsuApellido,
            'UsuEmail': UsuEmail,
            'UsuTelefono': UsuTelefono,
            'UsuMovil': UsuMovil,
            'UsuImagen': UsuImagen,
            'GrvUser': GrvUser,
            'GrvUserToken': GrvUserToken,
            'GrvAreaRevToken': GrvAreaRevToken,
            'GrvAreaRevNombre': GrvAreaRevNombre,
            'UserID': UserID,
            'RoleID': RoleID
        };

        $.ajax({
            url: "../../Usuario/Accion/Crear",
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                //PARSEAR RESPUESTA
                var respuesta = JSON.parse(JSON.stringify(r));
                respuesta = respuesta.Mensaje;
                //PROCESAR RESPUESTA
                if (respuesta == "ERROR") {
                    swal("", "Error al intentar crear el usuario.", "error");
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                }
                else if (respuesta == "EXISTE") {
                    swal("", "El usuario ya existe.", "error");
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                }
                else if (respuesta == "CREADO") {
                    swal("", "Usuario creado con éxito", "success");
                    setTimeout(function () {
                        document.location.href = '../../Usuario';
                    }, 3000);
                }
            }
        });
    }
}
//
//  CAMBIO DE ESTADO
//
FNRealizarAccionCuentaUsuario = function (token, opcion) {
    localStorage.setItem("CAPEX_USU_TOKEN", token);
    var recurso;
    var DTO = {
        'UsuToken': token,
        'Status': opcion
    }
    switch (opcion) {
        case "1": recurso = "Activar"; break;
        case "2": recurso = "Bloquear"; break;
    }
    if (opcion != "" && (opcion == "1" || opcion == "2")) {
        $.ajax({
            url: "Usuario/Accion/" + recurso,
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                //PARSEAR RESPUESTA
                var estructura = JSON.parse(JSON.stringify(r));
                estructura = estructura.Mensaje;
                //PROCESAR RESPUESTA
                if (estructura == "Error") {
                    swal("", "No es posible realizar la acción solicitada.", "error");
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                }
                else if (estructura == "Activado" || estructura == "Bloqueado") {
                    swal("", "Cambios aplicados exitosamente.", "success");
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                }
            }
        });
    }
    else if (opcion == "3") {
        $("#ModalConfirmarEliminarCuentaUsuario").show();
        return false;
    }
    else if (opcion == "4") {
        document.location.href = "Usuario/Vista/Modificar/" + token;
        return false;
    }
}
//
// ELIMINAR CUENTA (CLOSE /CERRAR)
//
FNRealizarEliminacionCuentaUsuario = function () {
    var UsuToken = localStorage.getItem("CAPEX_USU_TOKEN");
    var DTO = {
        'UsuToken': UsuToken,
        'Status': '3'
    }

    $.ajax({
        url: "Usuario/Accion/Eliminar",
        type: "POST",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            //PARSEAR RESPUESTA
            var estructura = JSON.parse(JSON.stringify(r));
            estructura = estructura.Mensaje;
            //PROCESAR RESPUESTA
            if (estructura == "Error") {
                swal("", "No es posible realizar la acción solicitada.", "error");
                setTimeout(function () {
                    document.location.reload();
                }, 3000);
            }
            else if (estructura == "Eliminado") {
                swal("", "Cuenta de usuario eliminada (cerrada) exitosamente.", "success");
                setTimeout(function () {
                    document.location.reload();
                }, 3000);
            }
        }
    });

}
//
// CERRAR MODAL CONFIRMACION ACCION ELIMINAR
//
FNCerrarModalEliminacionCuentaUsuario = function () {
    $("#ModalConfirmarEliminarCuentaUsuario").hide();
    return false;
}

// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : GESTION
// FECHA            : FEBRERO 2019
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// SECCION          : CAPEX MODULO - MANTENEDOR
// METODOS          : GERENTE
//



//
//  CAMBIO DE ESTADO CUENTA GERENTE
//
FNRealizarAccionCuentaGerente = function (token, opcion) {
    localStorage.setItem("CAPEX_GTE_TOKEN", token.trim());
    var recurso;
    var DTO = {
        'GteToken': token.trim(),
        'Status': opcion
    }
    switch (opcion) {
        case "1": recurso = "Activar"; break;
        case "2": recurso = "Bloquear"; break;
    }
    if (opcion != "" && (opcion == "1" || opcion == "2")) {
        $.ajax({
            url: "Gerente/Accion/" + recurso,
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                //PARSEAR RESPUESTA
                var estructura = JSON.parse(JSON.stringify(r));
                estructura = estructura.Mensaje;
                //PROCESAR RESPUESTA
                if (estructura == "Error") {
                    swal("", "No es posible realizar la acción solicitada.", "error");
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                }
                else if (estructura == "Activado" || estructura == "Bloqueado") {
                    swal("", "Cambios aplicados exitosamente.", "success");
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                }
            }
        });
    }
    else if (opcion == "4") {
        document.location.href = "Gerente/Vista/Modificar/" + token;
        return false;
    }

}
//
// CREAR GERENTE
//
FNCrearGerente = function () {

    /*********************************** PARAMETROS BASE ***********************************/
    var GteNombre = $("#GteNombre").val();
    var IdGerencia = $("#IdGerencia").val();
    var GteDescripcion = $("#GteDescripcion").val();

    /*********************************** VALIDAR ENTRADA**********************************/
    if (IdGerencia == "Seleccionar.." || IdGerencia == "" || IdGerencia == "-1" || GteNombre == "" || GteDescripcion == "") {
        swal("", "Debe seleccionar y copletar todos los campos.", "info");
        return false;
    }
    else {
        //PREPARAR
        var DTO = {
            'GteNombre': GteNombre,
            'IdGerencia': IdGerencia,
            'GteDescripcion': GteDescripcion
        };

        $.ajax({
            url: "../../Gerente/Accion/Crear",
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                //PARSEAR RESPUESTA
                var respuesta = JSON.parse(JSON.stringify(r));
                respuesta = respuesta.Mensaje;
                //PROCESAR RESPUESTA
                if (respuesta == "ERROR") {
                    swal("", "Error al intentar crear el usuario.", "error");
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                }
                else if (respuesta == "EXISTE") {
                    swal("", "El Gerente ya existe.", "error");
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                }
                else if (respuesta == "CREADO") {
                    swal("", "Gerente creado con éxito", "success");
                    setTimeout(function () {
                        document.location.href = '../../Gerente';
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
// MODULO           : GESTION
// FECHA            : FEBRERO 2019
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// SECCION          : CAPEX MODULO - MANTENEDOR
// METODOS          : SUPERINTENDENTE
//



//
//  CAMBIO DE ESTADO CUENTA SUPERINTENDENTE
//
FNRealizarAccionCuentaSuperintendente = function (token, opcion) {
    localStorage.setItem("CAPEX_SUP_TOKEN", token.trim());
    var recurso;
    var DTO = {
        'IntToken': token.trim(),
        'Status': opcion
    }
    switch (opcion) {
        case "1": recurso = "Activar"; break;
        case "2": recurso = "Bloquear"; break;
    }
    if (opcion != "" && (opcion == "1" || opcion == "2")) {
        $.ajax({
            url: "Superintendente/Accion/" + recurso,
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                //PARSEAR RESPUESTA
                var estructura = JSON.parse(JSON.stringify(r));
                estructura = estructura.Mensaje;
                //PROCESAR RESPUESTA
                if (estructura == "Error") {
                    swal("", "No es posible realizar la acción solicitada.", "error");
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                }
                else if (estructura == "Activado" || estructura == "Bloqueado") {
                    swal("", "Cambios aplicados exitosamente.", "success");
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                }
            }
        });
    }
    else if (opcion == "4") {
        document.location.href = "Superintendente/Vista/Modificar/" + token;
        return false;
    }

}
//
// CREAR SUPERINTENDENTE
//
FNCrearSuperintendente = function () {

    /*********************************** PARAMETROS BASE ***********************************/
    var IntNombre = $("#IntNombre").val();
    var IdSuper = $("#IdSuper").val();
    var IntDescripcion = $("#IntDescripcion").val();

    /*********************************** VALIDAR ENTRADA**********************************/
    if (IdSuper == "Seleccionar.." || IdSuper == "" || IdSuper == "-1" || IntNombre == "" || IntDescripcion == "") {
        swal("", "Debe seleccionar y copletar todos los campos.", "info");
        return false;
    }
    else {
        //PREPARAR
        var DTO = {
            'GteNombre': GteNombre,
            'IdGerencia': IdGerencia,
            'GteDescripcion': GteDescripcion
        };

        $.ajax({
            url: "../../Superintendente/Accion/Crear",
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                //PARSEAR RESPUESTA
                var respuesta = JSON.parse(JSON.stringify(r));
                respuesta = respuesta.Mensaje;
                //PROCESAR RESPUESTA
                if (respuesta == "ERROR") {
                    swal("", "Error al intentar crear el usuario.", "error");
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                }
                else if (respuesta == "EXISTE") {
                    swal("", "El Superintendente ya existe.", "error");
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                }
                else if (respuesta == "CREADO") {
                    swal("", "Superintendente creado con éxito", "success");
                    setTimeout(function () {
                        document.location.href = '../../Superintendente';
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
// MODULO           : GESTION
// FECHA            : FEBRERO 2019
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// SECCION          : CAPEX MODULO - MANTENEDOR
// METODOS          : COMUNES
//

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
    $('#usuarios tr').each(function () {
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
    /********************************** ALMACENAMIENTO LOCAL *****************************/
    localStorage.setItem("CAPEX_USR_NOMBRE_AREA_REVISORA", "");
    localStorage.setItem("CAPEX_USU_TOKEN", "");
    localStorage.setItem("CAPEX_GTE_TOKEN", "");
    /**********************************         LOADER       *****************************/
    jQuery("#AppLoaderContainer").hide();
    /********************************** CONTROL DE EVENTO SUBMIT *****************************/
    $("#form1").submit(function (e) {
        return false;
    });
    /********************************** PRE-CARGA DE DATOS *****************************/
    FNObtenerListadoCompanias();
    FNObtenerListadoAreas();
    FNPaginar();
    /********************************** FILTRAR TABLA USUARIOS *****************************/
    $('#buscador').keyup(function () {
        FNBuscarEnTabla($(this).val());
        $("#paginador").hide();
    });


});
