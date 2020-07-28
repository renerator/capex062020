// ----------------------------------------------
//
// SISTEMA          : CAPEX
// CLIENTE          : ANTOFAGASTA MINERALS S.A.
// MODULO           : PLANIFICACION
// FECHA            : MAYO 2018
// DESARROLLADO POR : PMO 360 SpA
//
// ----------------------------------------------
// SECCION          : COMUN 
// METODOS          :
//

//
//TEXTOS DE AYUDA
//

FNObtenerFechaBloqueo = function () {
    //PREPARAR
    $.ajaxSetup({ cache: false });
    $.ajax({
        url: "/Planificacion/obtenerFechaBloqueo",
        type: "GET",
        dataType: "json",
        success: function (r) {
            console.log("FNObtenerFechaBloqueo r=" + r);
            if (r && r.redirectUrlLogout && r.redirectUrlLogout == "true") {
                document.getElementById('linkToLogout').click();
                return;
            }
            if (r == "0") {
                $('#BotonGuardarEnviarIniciativa').show();
            } else {
                $('#BotonGuardarEnviarIniciativa').hide();
            }
        }
    });
}



FNObtenerTextoAyuda = function (etapa, seccion) {
    var seccion = seccion.trim();
    //alert(seccion);
    $("#ContenedorInformacionLateralIzquierdo").html("");
    switch (etapa) {
        case "IDENTIFICACION":
            $("#ContenedorInformacionLateralIzquierdo").html("<h7>Identificación</h7><br><div style='margin-top:5px;text-align:justify;'><strong>Proceso:</strong> Se llama lo 'que se va a hacer'. <br> Es la acción que caracteriza la naturaleza de la inversión. <br><br><strong>Objeto:</strong> Se denomina al “sobre qué”, que es la materia o motivo del proceso.<br><br> <strong>Área:</strong> Es la ubicación del proyecto. <br><br><strong>Etapa:</strong> Corresponde a la etapa del ciclo de vida del proyecto que se está ejecutando.<br><br><strong> Nombre de Proyecto:</strong> Nombre asociado al código asignado por la elección de Proceso, Objeto, Area, Compañía y Etapa. <br><br><strong>Código Iniciativa:</strong> Código establecido por acrónimos del Tipo de ejercicio, el año en curso del ejercicio, la compañía, el área y la etapa.Todo este código ira separado con un guion para posteriormente indicar el correlativo que contiene ese código.Un ejemplo del Código de iniciativa es: CB20CENPC4 - 22. <br><br><strong>Código del Proyecto:</strong> campo que permitirá el ingreso de código con el cual queda el proyecto en sistema SAP, una vez que es aprobado o se encuentra en cartera, ingresando un máximo de 20 caracteres.</div>");
        case "CATEGORIZACION":
            if (seccion == "PERF") {
                $("#ContenedorInformacionLateralIzquierdo").html("<h7>Estudio de Perfil</h7><br><div style='margin-top:5px;text-align:justify;'>Estudio de Perfil (Scope – identificación y valorización de la oportunidad)<br><br> Estudio de Prefactibilidad (Ingeniería Conceptual – generación y selección alternativas), Estudio de Factibilidad (Ingeniería Básica)<br><br>Estudio Inversional (Ingeniería de Detalle), No Requiere. Cabe mencionar que esto está asociado al Ciclo Inversional del Proyecto.</div>");
            }
            else if (seccion == "PREF") {
                $("#ContenedorInformacionLateralIzquierdo").html("<h7>Estudio de Pre factibilidad </h7><br><div style='margin-top:5px;text-align:justify;'>Estudio de Pre factibilidad (Ingeniería Conceptual – generación y selección alternativas) <br> Es la etapa que desarrolla la ingeniería conceptual del proyecto con el fin de generar, evaluar y seleccionar la mejor alternativa técnica económica para el proyecto.Esta etapa la desarrolla el ejecutor.</div>");
            }
            else if (seccion == "FACT") {
                $("#ContenedorInformacionLateralIzquierdo").html("<h7>Estudio de Factibilidad  </h7><br><div style='margin-top:5px;text-align:justify;'>Estudio de Factibilidad (Ingeniería Básica) <br>Es la etapa que desarrolla la ingeniería básica de la alternativa seleccionada en la etapa anterior, completando el diseño detallado del activo que se va a construir.En esta etapa se desarrollan los estudios de instalaciones, revisión de planos, normas y estándares relacionados al proyecto, estimación de costos Clase 3, cotizaciones actualizadas u otra fuente confiable.En esta etapa se debe determinar situación actual y situación con proyecto en operación.Esta etapa la desarrolla el ejecutor.</div>");
            }
            else if (seccion == "EJEC") {
                $("#ContenedorInformacionLateralIzquierdo").html("<h7>Estudio Inversional </h7><br><div style='margin-top:5px;text-align:justify;'>Estudio Inversional (Ingeniería de Detalle) <br> Es la etapa que incluye el desarrollo de la ingeniería de detalles, la construcción, montaje y puesta en marcha del nuevo activo, donde se busca capturar la promesa ofrecida, privilegiando los aspectos de sustentabilidad, plazo, costo y calidad.Esta etapa la desarrolla el ejecutor.</div>");
            }
            else if (seccion == "EF29B2EA-603E-4631-87C9-D04F867D39CE") {
                $("#ContenedorInformacionLateralIzquierdo").html("<h7>ERFP</h7><br><div style='margin-top:5px;text-align:justify;'>ERFP: Estándar de Riesgos de Fatalidad Particulares<br>1. Operaciones portuarias - buceo <br>2. Operaciones portuarias - corte de espía <br>3. Operaciones portuarias - embarcaciones menores <br>4.	Operaciones portuarias - caída al mar <br>5. Operaciones de perforación y sondaje <br>6. Operaciones en condiciones climáticas adversas <br>7. Operaciones ferroviarias <br></div>");
            }
            else if (seccion == "3EA8EDBB-13CF-477B-B13E-CEE19966ABFA") {

                $("#ContenedorInformacionLateralIzquierdo").html("<h7>ERFT</h7><br><div style='margin-top:5px;text-align:justify;'>ERFT: Estándar de Riesgos de Fatalidad Transversales <br> 1.	Pérdida control del vehículo <br>2.	Pérdida control del equipo <br>3. Interacción personas, equipos y vehículos <br> 4. Caída de roca / falla de terreno <br> 5. Pérdida de control en maniobras de izaje<br> 6. Pérdida de control en manejo de explosivos <br>7. Pérdida de equilibrio / caída desde altura <br>8. Falla estructural <br> 9. Caída de objeto <br>10. Contacto con energía eléctrica <br>11. Liberación descontrolada de energía <br>12. Espacio confinado <br>13. Atrapamiento con partes móviles <br>14. Contacto con sustancias peligrosas <br>15. Incendio <br></div>");

            }
            else if (seccion == "CB2CEDC3-AE5A-46CD-A962-656DEAD2823C") {
                $("#ContenedorInformacionLateralIzquierdo").html("<h7>ESO</h7><br><div style='margin-top:5px;text-align:justify;'>ESO: Estándares de Salud Ocupacional<br>1. Estándar de salud compatible <br>2. Estándar de higiene ocupacional <br>3. Estándar de ergonomía <br>4. Estándar psicosocial <br>5. Estándar de vigilancia médica ocupacional <br>6. Estándar de gestión de casos de salud <br>7. Estándar de fatiga y somnolencia <br>8. Estándar de maternidad <br>9. Estándar de alcohol, drogas y tabaco <br>10. Estándar de promoción de conductas saludables y calidad de vida <br></div>");
            }
            else if (seccion == "Clase 1") {
                $("#ContenedorInformacionLateralIzquierdo").html("<h7>Clase 1</h7><br><div style='margin-top:5px;text-align:justify;'>Inversiones que reducen riesgos HSEC altos o estratégicos altos.<br><strong>Condición.</strong> <br>a. Debe buscar mitigar un riesgo declarado por la compañía.<br>b.	Efectividad del control: Debe bajar el nivel de riesgo de tal manera que el riesgo residual sea moderado o bajo.<br><br><strong>Criterio de Priorización. </strong><br>Priorización es solo relevante para asignar capacidad de ejecución.<br><br><strong>Sub-clase 1.</strong><br>Inversiones que mitigan riesgos moderados para las personas<br><br><strong>Criterio de Priorización.</strong><br>Ordenarlas de acuerdo a criterios de la compañía<br></div> ");
            }
            else if (seccion == "Clase 2") {
                $("#ContenedorInformacionLateralIzquierdo").html("<h7>Clase 2</h7><br><div style='margin-top:5px;text-align:justify;'>Inversiones que habilitan el caso base <br><strong>Condición.</strong> <br>a. Esta clase está compuesta por aquellas inversiones rentables (*) y que de no hacerse afectarían materialmente el Caso Base de las compañías<br><br><strong>Criterio de Priorización.</strong><br>Indicador que considera la materialidad de la pérdida que se tendría si el proyecto no se hace y el Capex asociado a la inversión:<br>Indicador de ranking: VA (perdida mitigada) - VA (Capex)<br><br><strong>Sub-clase 2.</strong><br>Inversiones que mitigan riesgos HSEC (excluyendo los riesgos a las personas)<br><br><strong>Criterio de Priorización.</strong><br>Delta VA (MFL) / Capex<br></div > ");
            }
            else if (seccion == "Clase 3") {
                $("#ContenedorInformacionLateralIzquierdo").html("<h7>Clase 3</h7><br><div style='margin-top:5px;text-align:justify;'>Inversiones que reducen riesgos económicos altos.<br><br><strong>Condición.</strong> <br>a.	Debe buscar mitigar un riesgo declarado por la compañía<br>b.	Efectividad de control: Debe disminuir el nivel de riesgo de tal manera que el riesgo residual sea moderado o bajo.<br><br><strong>Criterio de Priorización.</strong> <br>El primer criterio es ordenar estas inversiones por nivel de riesgo (prob.x impacto). El segundo criterio se deberá utilizar cuando dos inversiones tienen igual nivel de riesgo, en este caso se deberá priorizar el que tenga un mayor valor del siguiente indicador:<br><br>Indicador de ranking: Delta VA (MFL) / Capex<br><br><strong>Sub-clase 3.</strong><br>Inversiones que mitigan riesgos económicos moderados<br><br><strong>Criterio de Priorización. </strong><br>Delta VA (MFL) / Capex<br></div>");
            }
            else if (seccion == "Clase 4") {
                $("#ContenedorInformacionLateralIzquierdo").html("<h7>Clase 4</h7><br><div style='margin-top:5px;text-align:justify;'>Inversiones que agregan valor (marginal) o inversiones que reducen riesgos moderados.<br><br><strong>Condición.</strong><br>a. Aquellas inversiones que buscan mitigar riesgo, deben dejar un riesgo residual bajos.<br><br>b. Aquellas inversiones que agregan valor deberán tener un mínimo nivel de rentabilidad, el cual será definido por el Excom.<br><strong>Criterio de Priorización.</strong><br>No tiene<br><br><strong>Sub-clase 4.</strong><br>Inversiones que agregan valor marginal<br><br><strong>Criterio de Priorización. </strong><br>IVAN</div > ");
            }
            break;
        case "BAJA_COMPLEJIDAD":
            $("#ContenedorInformacionLateralIzquierdo").html("<div style='margin-top:5px;text-align:justify;'><h7>Interferencias</h7><br>Se refiere a la existencia de interferencias con instalaciones existentes de operaciones u otras, con otros proyectos a ejecutarse en paralelo o con condiciones climáticas adversas.<br>" +
                "<h7>Riesgo para las personas</h7><br> Se refiere a analizar si existen riesgos a las personas, ya sea durante la ejecución del proyecto o durante la operación en régimen.<br>" +
                "<h7>Sustentabilidad</h7><br> Se refiere al análisis de la existencia de riesgos al medio ambiente, a la comunidad y a los Bienes Físicos producto de la ejecución del proyecto.<br>" +
                "<h7>Complejidad Tecnológica</h7><br> En este aspecto se debe analizar el conocimiento que se tiene del lugar de emplazamiento del proyecto, en cuanto a mecánica de suelos, geotécnica, condiciones ambientales y otros aspectos relevantes.<br>" +
                "<h7>Solución Técnica</h7><br> En este aspecto se debe analizar el grado de definición de la solución para cumplir con el objetivo del proyecto, en cuánto que ésta sea la mejor desde el punto de vista técnico - económico.<br></div><br><div><a href='#' onclick='FNMostrarModalGradoComplejidad()' style='text-decoration:underline;'>Ver tabla Grado de Complejidad</a></div>");
            break;
        case "PRESUPUESTO":
            $("#ContenedorInformacionLateralIzquierdo").html("<h7>Presupuesto</h7><br><div style='margin-top:5px;text-align:justify;'>Avance Financiero: En este avance se debe entregar el valor de cada ìtem en KUS$, respetando ciertos parámetros máximos establecidos como el '% de costo del dueño' o '% de contingencia'.<br><br>Avance Físico: En este avance se debe entregar el avance físico acumulado de cada fase, el cual por medio de la ponderación de cada una de ellas logra el vector acumulado del proyecto.<br><br>Dotación Promedio: Se debe establecer el vector de las dotaciones en base a los contratos que considera cada proyecto.La información a entregar debe ser asociada a la dotación 'CONTRATADA' <br></div > ");
            break;
        case "DESCRIPCION":
            $("#ContenedorInformacionLateralIzquierdo").html("<h7>Descripciuón Detallada</h7><br><div style='margin-top:5px;text-align:justify;'><br> General: Se debe declarar el objetivo de la inversión, en donde se debe argumentar el proyecto o iniciativa en construcción.Se debe establecer alcance y justificar en forma concreta el desarrollo.<br><br>Fecha Post Evaluación: 	Existe un campo denominado fecha de Post Evaluación, la cual establece la fecha de evaluación de la iniciativa. Fecha en la que, una vez terminado el proyecto, se evalúan el resultado de las promesas de éste y se determina el éxito del proyecto en la operación.<br><br>Cuadro KPI: Indica descripción y detalle de lo que se logrará con la iniciativa en desarrollo. Corresponden a las promesas del proyecto que serán postevaluadas. Considera la descripción, la unidad de medida, el valor actual y la promesa (valor que se obtiene gracias a la ejecución del proyecto).</div > ");
            break;
        case "ECONOMICA":
            $("#ContenedorInformacionLateralIzquierdo").html("<h7>Evaluación Económica</h7><br><div style='margin-top:5px;text-align:justify;'><br>VAN:  El valor actual neto, también conocido como valor actualizado neto o valor presente neto, cuyo acrónimo es VAN, es un procedimiento que permite calcular el valor presente de un determinado número de flujos de caja futuros, originados por una inversión.<br><br>TIR: Es la tasa de interés o rentabilidad que ofrece una inversión. También se define como el valor de la tasa de descuento que hace que el VAN sea igual a cero, para un proyecto de inversión dado.<br><br>IVAN: Indicador para rankear o priorización de proyectos, corresponde al VAN dividico por la Inversión.<br><br>Payback: También denominado 'plazo de recuperación' es un criterio estático de valoración de inversiones que permite seleccionar un determinado proyecto sobre la base de cuánto tiempo se tardará en recuperar la inversión inicial mediante los flujos de caja.</div > ");
            break;
        case "RIESGO":
            $("#ContenedorInformacionLateralIzquierdo").html("<h7>Evaluación de Riesgo</h7><br><div style='margin-top:5px;text-align:justify;'><br>MFL: Pérdida Máxima Posible o Pérdida Máxima Previsible</div>");
            break;
        case "HITO":
            $("#ContenedorInformacionLateralIzquierdo").html("<h7>Hitos /CAPEX</h7><br><div style='margin-top:5px;text-align:justify;'><br>Se visualiza Resumen financiero de la iniciativa propuesta.<br><br>Se visualiza Ruta de aprobación, estableciendo validación y gráfica de ésta. </div>");
            break;
        case "ARCHIVO":
            $("#ContenedorInformacionLateralIzquierdo").html("<h7>Archivos Adjuntos</h7><br><div style='margin-top:5px;text-align:justify;'><br>Adjunto establecidos en construcción de iniciativa. <br><br>Estos archivos podrán ser eliminado y descargados por parte del usuario que construye iniciativa.</div>");
            break;
    }
}
//
//  LIMPIAR TEXTO DE AYUDA
//
FNLimpiarTextoAyuda = function () {
    $("#ContenedorInformacionLateralIzquierdo").html("");
}
//
// VERIFICAR ESTADO DE ALMACENAMIENTO DE ETAPA
//
FNVerificarEstadoGuardadoDatos = function (etapa) {

    localStorage.setItem("CAPEX_IDENTIFICACION_ETAPA_ANTERIOR", etapa);
    var cambio_identificacion = localStorage.getItem("CAPEX_IDENTIFICACION_CAMBIO");
    var cambio_categorizacion = localStorage.getItem("CAPEX_CATEGORIZACION_CAMBIO");
    var cambio_categorizacion1 = localStorage.getItem("CAPEX_CATEGORIZACION_DESARROLLO_CAMBIO");
    var cambio_categorizacion2 = localStorage.getItem("CAPEX_CATEGORIZACION_ABC_CAMBIO");
    var cambio_descripcion = localStorage.getItem("CAPEX_DESCRIPCION_CAMBIO");
    var cambio_evaleco = localStorage.getItem("CAPEX_EVALECO_CAMBIO");
    var cambio_evalriesgo = localStorage.getItem("CAPEX_EVALRIESGO_CAMBIO");
    var cambio_gantt = localStorage.getItem("CAPEX_GANTT_CAMBIO");
    var cambio_template = localStorage.getItem("CAPEX_TEMPLATE_CAMBIO");
    var cambio_hitos = localStorage.getItem("CAPEX_HITOS_CAMBIO");

    var estado_identificacion = localStorage.getItem("CAPEX_INICIATIVA_ESTADO");
    var estado_categorizacion = localStorage.getItem("CAPEX_INICIATIVA_CATEGORIZACION_ESTADO");
    var estado_descripcion = localStorage.getItem("CAPEX_INICIATIVA_DESCRIPCION_ESTADO");
    var estado_evaluacion_eco = localStorage.getItem("CAPEX_INICIATIVA_EVALUACION_ECONOMICA_ESTADO");
    var estado_evaluacion_rie = localStorage.getItem("CAPEX_INICIATIVA_EVALUACION_RIESGO_ESTADO");
    var estado_hitos = localStorage.getItem("CAPEX_INICIATIVA_HITO_ESTADO");

    if (estado_identificacion == "" && cambio_identificacion == "SI") {
        $("#ModalAdvertenciaPerdidaCambios").show();
    }
    else if (estado_categorizacion == "" && (cambio_categorizacion == "SI" || cambio_categorizacion1 == "SI" || cambio_categorizacion2 == "SI")) {
        $("#ModalAdvertenciaPerdidaCambios").show();
    }
    else if (estado_descripcion == "" && cambio_descripcion == "SI") {
        $("#ModalAdvertenciaPerdidaCambios").show();
    }
    else if (estado_evaluacion_eco == "" && cambio_evaleco == "SI") {
        $("#ModalAdvertenciaPerdidaCambios").show();
    }
    else if (estado_evaluacion_rie == "" && cambio_evalriesgo == "SI") {
        $("#ModalAdvertenciaPerdidaCambios").show();
    }
    else if (estado_hitos == "" && cambio_hitos == "SI") {
        $("#ModalAdvertenciaPerdidaCambios").show();
    }
}
FNRelocalizarEtapa = function () {
    $("#ModalAdvertenciaPerdidaCambios").hide();
    var etapa = localStorage.getItem("CAPEX_IDENTIFICACION_ETAPA_ANTERIOR");
    var cambio_identificacion = localStorage.getItem("CAPEX_IDENTIFICACION_CAMBIO");
    var cambio_categorizacion = localStorage.getItem("CAPEX_CATEGORIZACION_CAMBIO");
    var cambio_categorizacion1 = localStorage.getItem("CAPEX_CATEGORIZACION_DESARROLLO_CAMBIO");
    var cambio_categorizacion2 = localStorage.getItem("CAPEX_CATEGORIZACION_ABC_CAMBIO");
    var cambio_descripcion = localStorage.getItem("CAPEX_DESCRIPCION_CAMBIO");
    var cambio_evaleco = localStorage.getItem("CAPEX_EVALECO_CAMBIO");
    var cambio_evalriesgo = localStorage.getItem("CAPEX_EVALRIESGO_CAMBIO");
    var cambio_gantt = localStorage.getItem("CAPEX_GANTT_CAMBIO");
    var cambio_template = localStorage.getItem("CAPEX_TEMPLATE_CAMBIO");
    var cambio_hitos = localStorage.getItem("CAPEX_HITOS_CAMBIO");


    var estado_identificacion = localStorage.getItem("CAPEX_INICIATIVA_ESTADO");
    var estado_categorizacion = localStorage.getItem("CAPEX_INICIATIVA_CATEGORIZACION_ESTADO");
    var estado_descripcion = localStorage.getItem("CAPEX_INICIATIVA_DESCRIPCION_ESTADO");
    var estado_evaluacion_eco = localStorage.getItem("CAPEX_INICIATIVA_EVALUACION_ECONOMICA_ESTADO");
    var estado_evaluacion_rie = localStorage.getItem("CAPEX_INICIATIVA_EVALUACION_RIESGO_ESTADO");
    var estado_hitos = localStorage.getItem("CAPEX_INICIATIVA_HITO_ESTADO");

    if (cambio_identificacion == "SI" && estado_identificacion == "") {
        $('#navegacion li:nth-child(1) a').tab('show');
        return false;
    }
    else if ((cambio_categorizacion == "SI" || cambio_categorizacion1 == "SI" || cambio_categorizacion2 == "SI") && (estado_categorizacion == "")) {
        $('#navegacion li:nth-child(2) a').tab('show');
        return false;
    }
    else if (cambio_descripcion == "SI" && estado_descripcion == "") {
        $('#navegacion li:nth-child(4) a').tab('show');
        return false;
    }
    else if (cambio_evaleco == "SI" && estado_evaluacion_eco == "") {
        $('#navegacion li:nth-child(5) a').tab('show');
        return false;
    }
    else if (cambio_evalriesgo == "SI" && estado_evaluacion_rie == "") {
        $('#navegacion li:nth-child(6) a').tab('show');
        return false;
    }
    else if (cambio_hitos == "SI" && estado_hitos == "") {
        $('#navegacion li:nth-child(7) a').tab('show');
        return false;
    }
}
//
// CERRAR MODAL ADVERTENCIA
//
FNCerrarModalAdvertenciaPerdidaCambios = function () {
    $("#ModalAdvertenciaPerdidaCambios").hide();
    return false;
}
//
// GENERAR TOKEN
//
FNGenerarUUID = function () {
    var d = new Date().getTime();
    if (Date.now) {
        d = Date.now(); //high-precision timer
    }
    var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = (d + Math.random() * 16) % 16 | 0;
        d = Math.floor(d / 16);
        return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16).toUpperCase();
    });
    return uuid;
};
//
// FORZAR SOLO NUMERICOS
//
jQuery.fn.ForceNumericOnly =
    function () {
        return this.each(function () {
            $(this).keydown(function (e) {
                var key = e.charCode || e.keyCode || 0;
                return (
                    key == 8 ||
                    key == 9 ||
                    key == 13 ||
                    key == 46 ||
                    key == 110 ||
                    key == 190 ||
                    (key >= 35 && key <= 40) ||
                    (key >= 48 && key <= 57) ||
                    (key >= 96 && key <= 105));
            });
        });
    };
