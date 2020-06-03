// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : DOCUMENTOS
// FECHA            : FEBRERO 2019
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// SECCION          : CAPEX REPOSITORIO DOCUMENTAL
// METODOS          :

//
// FILTRAR VISTA
//
FNFiltrarVista = function (Categoria) {

    localStorage.setItem("CAPEX_DOCUMENTACION_CATEGORIA", Categoria);
    document.location.href = window.origin + "/Documentacion/" + Categoria;
    return;
}
//
// DESCARGAR DOCUMENTO
//
FNDescargarDocumento = function (token) {
    //document.location.href = window.origin + "/Documentacion/DescargarDocumentoAdjunto/" + Documento + "/";
    var link = document.createElement("a");
    console.info("token=", token);
    $.ajax({
        url: "/Documentacion/DescargarDocumentoBiblitecaFinal/" + token,
        method: "GET",
        data: { "token": token }
    }).done(function (r) {
        if (r && r.IsSuccess && r.ResponseData) {
            document.location.href = r.ResponseData;
        }
    }).fail(function (xhr) {
        console.log('error', xhr);
    });
    return;
}
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
    $('#documentos tr').each(function () {
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
// ACTUALIZAR VISTA
//
FNDocumentosActualizar = function () {
    document.location.reload();
    return false;
}
//
// ABRIR MODAL NUEVA CATEGORIA
//
FNAbrirPopUpCrearCategoria = function () {
    $("#ModalCrearCategoria").show();
    return false;
}
//
// CERRAR MODAL NUEVA CATEGORIA
//
FNCerrarPopUpCrearCategoria = function () {
    $("#ModalCrearCategoria").hide();
    document.location.reload();
    return false;
}
//
// GUARDAR CATEGORIA
//
FNCrearCategoriaDocumental = function (TokenCom) {
    var DocCatNombre = $("#DocCatNombre").val();
    if (DocCatNombre == "" || DocCatNombre == null || TokenCom == "" || TokenCom == null) {
        swal("", "Debe proveer un nombre de categoría.", "info");
        return false;
    }
    else {
        //LISTO
        var DTO = {
            'CatNombre': DocCatNombre,
            'ComToken': TokenCom
        };
        var cuantos = 0
        $.ajax({
            url: window.origin + "/Documentacion/Accion/CrearCategoria",
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                swal("", "Nueva categoría creada exitosamente.", "success");
                setTimeout(function () {
                    document.location.reload();
                }, 2000);
                return false;
            }
            ,
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                return false;
            }
        });
    }
}
//
// MODAL NUEVO DOCUMENTO
//
FNMostrarModalNuevoDocumento = function () {
    $("#ModalNuevoDocumento").show();
    return;
}
FNCerrarModalNuevoDocumento = function () {
    $("#ModalNuevoDocumento").hide();
    document.location.reload();
    return false;
}
//
//  MODAL ELIMINAR DOCUMENTO
//
FNModalEliminarDocumento = function (token) {
    var token = token.trim();
    localStorage.setItem("CAPEX_TOKEN_DOCUMENTO", token);
    $("#ModalEliminarDocumento").show();
    return;
}
FNCerrarModalEliminarDocumento = function () {
    $("#ModalEliminarDocumento").hide();
    document.location.reload();
    return false;
}

FNObtenerCategoriaSeleccionada = function () {
    return $('#Categoria').val();
}

FNErrorCategoria = function () {
    swal("", "Seleccione una categoria.", "info");
}

FNInicioSubidaDocumento = function () {
    $("#AppLoaderContainer").show();
}

FNErrorTamanioArchivo = function () {
    swal("", "EL archivo no debe superar los 10MB de tamaño,.", "info");
}

FNCallBackSubidaDocumento = function (paramJson) {
    $("#AppLoaderContainer").hide();
    if (paramJson.Data.code == "0") {
        swal("", "Documento subido y registrado correctamente.", "success");
        setTimeout(function () {
            parent.location.href = parent.location.href;
        }, 3000);
    } else {
        swal("", "No es posible subir documento.", "info");
        setTimeout(function () {
            parent.location.href = parent.location.href;
        }, 3000);
    }
}
//
// CARGAR CATEGORIAS
//
FNObtenerCategorias = function () {
    //PREPARAR
    var Categoria = $('#Categoria');
    Categoria.empty();
    Categoria.append('<option selected="true">Buscando..</option>');
    //LISTO
    var DTO = {};
    var cuantos = 0
    $.ajax({
        url: window.origin + "/Documentacion/Accion/ListarCategoria",
        type: "GET",
        dataType: "json",
        data: (DTO),
        success: function (r) {
            Categoria.empty();
            var estructura = JSON.parse(JSON.stringify(r));
            estructura = estructura.Resultado;

            for (i in estructura) {
                $('#Categoria').append(new Option(estructura[i].DocCatNombre, estructura[i].DocCatToken, false, false));
            }
        }
        ,
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            return false;
        }
    });
}

//
// ELIMINAR DOCUMENTO
//
FNEliminarDocumento = function () {
    var token = localStorage.getItem("CAPEX_TOKEN_DOCUMENTO");

    //PREPARAR
    var DTO = { "Token": token };
    if (token == "" || token == null) {
        swal("", "Identificador de documento no disponible, no se puede eliminar el documento seleccionado.", "info");
        return false;
    }
    else {
        $.ajax({
            url: window.origin + "/Documentacion/Accion/EliminarDocumento",
            type: "POST",
            dataType: "json",
            data: (DTO),
            success: function (r) {
                var respuesta = JSON.parse(JSON.stringify(r));
                if (respuesta.Mensaje == "Eliminado") {
                    swal("", "Documento Eliminado.", "success");
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                }
                else {
                    swal("", "No es posible eliminar el documento.", "error");
                    setTimeout(function () {
                        document.location.reload();
                    }, 3000);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                swal("", "No es posible eliminar el documento.", "error");
                setTimeout(function () {
                    document.location.reload();
                }, 3000);
            }
        });
    }
}

//
// SUBIR Y REGISTRAR DOCUMENTO
//
document.getElementById('form_nuevodocumento').onsubmit = function (e) {
    e.preventDefault();
    if (document.getElementById("Documento").files.length == 0) {
        return false;
    }
    else {
        var formdata = new FormData();
        var fileInput = document.getElementById('Documento');
        var nombreArchivo = fileInput.files[0].name;
        var tamano = fileInput.files[0].size;
        tamano = Math.round(parseInt(tamano));
        tamano = (tamano ^ 0);

        var extension = nombreArchivo.substr((nombreArchivo.lastIndexOf('.') + 1));
        formdata.append(fileInput.files[0].name, fileInput.files[0]);
        $("#NombreDocumento").html(nombreArchivo);
        if (tamano > 10485760) {
            swal("", "EL archivo no debe superar los 10MB de tamaño,.", "info");
            return false;
        }
        else {
            jQuery("#AppLoaderContainer").show();
            var ajaxRequest = $.ajax({
                type: "POST",
                url: window.origin + "/Documentacion/Accion/SubirDocumento",
                contentType: false,
                processData: false,
                data: formdata
            });

            ajaxRequest.done(function (xhr, textStatus) {
                var usuario = $("#CAPEX_H_USERNAME").val();
                var categoria = $("#Categoria").val();
                if (categoria == "" || categoria == null) {
                    swal("", "Seleccione una categoria.", "info");
                    return false;
                }
                else {
                    var tipo = null;
                    switch (extension) {
                        case "xls": tipo = "Excel"; break
                        case "xlsx": tipo = "Excel"; break;
                        case "ppt": tipo = "Power Point"; break
                        case "pptx": tipo = "Power Point"; break;
                        case "pdf": tipo = "Acrobat Reader"; break
                        case "doc": tipo = "Word"; break;
                        case "docx": tipo = "Word"; break;
                        case "mpp": tipo = "Project"; break;
                        case "mppx": tipo = "Project"; break;
                        case "txt": tipo = "Texto"; break;
                        case "zip": tipo = "Comprimido ZIP"; break;
                        case "rar": tipo = "Comprimido RAR"; break;
                    }
                    var DTO = { "Documento": nombreArchivo, "Tamano": tamano, "Extension": extension, "Tipo": tipo, "Categoria": categoria };
                    $.ajax({
                        url: window.origin + "/Documentacion/Accion/RegistrarDocumento",
                        type: "POST",
                        dataType: "json",
                        data: (DTO),
                        success: function (r) {
                            var respuesta = JSON.parse(JSON.stringify(r));
                            if (respuesta.Mensaje == "Registrado") {
                                jQuery("#AppLoaderContainer").hide();
                                swal("", "Documento subido y registrado correctamente.", "success");
                                setTimeout(function () {
                                    document.location.reload();
                                }, 3000);
                            }
                        },
                        error: function (xhr, error, status) {
                            jQuery("#AppLoaderContainer").hide();
                            swal("", "No es posible subir documento.", "info");
                            setTimeout(function () {
                                document.location.reload();
                            }, 3000);
                        }
                    });
                }
            });
        }
    }
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
// METODOS          : INICIALIZADOR

$(document).ready(function () {
    jQuery("#AppLoaderContainer").hide();
    /******************************** IMPORTACION CONTROL DE EVENTOS    *************************/
    $('#NombreDocumento').on('change', function () {
        var file = document.getElementById('NombreDocumento');
        if (file.files.length > 0) {
            document.getElementById('NombreDocumento').innerHTML = file.files[0].name;
        }
    });
    $('#buscador').keyup(function () {
        FNBuscarEnTabla($(this).val());
        $("#paginador").hide();
    });
    /********************************** CONTROL DE EVENTO SUBMIT    *****************************/
    $("#form_nuevodocumento").submit(function (e) {
        return false;
    });
    /**********************************CARGAR ELEMENTOS POR DEFECTO *****************************/
    FNObtenerCategorias();
    /********************************** PAGINAR                     *****************************/
    FNPaginar();
    /********************************** RESALTAR CATEGORIA SELECCIONAD **************************/
    var cat = localStorage.getItem("CAPEX_DOCUMENTACION_CATEGORIA");
    if (cat) {
        $("#" + cat).css({ "color": "orange", "font-size": "12px", "font-weigth": "bold", "opacity": "0.9" });
    }
    else {
        $("#3A22E755-E8B2-42AE-A868-55F08B731580").css({ "color": "orange", "font-size": "12px", "font-weigth": "bold", "opacity": "0.9" });
    }
    localStorage.setItem("CAPEX_DOCUMENTACION_CATEGORIA", "");

});
