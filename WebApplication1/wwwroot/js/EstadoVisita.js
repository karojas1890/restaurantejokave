// Variables globales

//nuevo
let timerInterval = null;
let tiempoSegundos = 0;

let EstadoActual = null;

// Textos y colores por estado
const estados = [
    { id: 1, nombre: "En espera", clase: "en-espera" },
    { id: 2, nombre: "Asignado a mesa", clase: "asignado" },
    { id: 3, nombre: "Ordenando", clase: "ordenando" },
    { id: 4, nombre: "Esperando Orden", clase: "esperando-cuenta" },
    { id: 5, nombre: "Consumiendo", clase: "consumiendo" },
    { id: 6, nombre: "Solicitando cuenta", clase: "solicitando-cuenta" },
    { id: 7, nombre: "Pagando", clase: "pagando" },
    { id: 8, nombre: "Se retira después de pagar", clase: "retira-pagado" },
    { id: 9, nombre: "Se retira sin pagar", clase: "retira-sin-pago" }
];

// Función principal para obtener y actualizar estado de visita
function actualizarEstadoVisita() {
    fetch(`/Ordenes/EstadoVisita`)
        .then(res => res.json())
        .then(data => {
            if (data.error) {
                limpiarEstado();
                console.warn(data.error);
                return;
            }
            
            if (data.estadoActual && data.estadoActual !== EstadoActual) {
                EstadoActual = data.estadoActual;
                cambiarEstado(EstadoActual); 
            }

            // Actualizar tiempo si se proporciona desde el servidor
            if (data.tiempoEspera) {
                actualizarTiempoEspera(data.tiempoEspera);
            }
        })
        .catch(err => {
            console.error('Error al obtener el estado de visita:', err);
            limpiarEstado();
        });
}

// Crear solo el elemento del estado actual en el timeline
function actualizarTimelineEstados(estadoActual) {
    const timeline = document.getElementById("statusTimeline");
    if (!timeline) {
        console.warn("No se encontró el elemento #statusTimeline");
        return;
    }

    const estadoInfo = estados.find(e => e.id === estadoActual);
    if (!estadoInfo) {
        console.warn("Estado no encontrado:", estadoActual);
        return;
    }

    // Verifica si ya se añadió ese estado
    if (document.getElementById(`estado-${estadoInfo.id}`)) {
        return; // Ya existe, no lo agregues de nuevo
    }

    const estadoDiv = document.createElement('div');
    estadoDiv.className = 'status-item';
    estadoDiv.id = `estado-${estadoInfo.id}`;

    estadoDiv.innerHTML = `
        <div class="status-item-title">${estadoInfo.nombre}</div>
        <div class="status-time">
            <div class="clock-icon">🕐</div>
            <span id="tiempo-${estadoInfo.id}">0:00</span>
        </div>
    `;

    timeline.appendChild(estadoDiv);
}


// Actualizar solo el estado actual (sin recargar todo)
function cambiarEstado(nuevoEstado) {
    // Detener timer anterior
    if (timerInterval) {
        clearInterval(timerInterval);
    }

    tiempoSegundos = 0;
    estadoActual = nuevoEstado;

    // Actualizar solo el texto de estado actual
    const statusText = document.getElementById("textoEstado");
    if (statusText) {
        statusText.textContent = obtenerTextoEstado(nuevoEstado);
    }

    // Actualizar solo el indicador de color
    actualizarIndicadorColor(nuevoEstado);

    // Crear solo el elemento del estado actual en el timeline
    actualizarTimelineEstados(nuevoEstado);

    // Iniciar timer solo para el estado actual
    timerInterval = setInterval(() => {
        tiempoSegundos++;
        const tiempo = document.getElementById(`tiempo-${nuevoEstado}`);
        if (tiempo) {
            tiempo.textContent = formatearTiempo(tiempoSegundos);
        }
    }, 1000);

    console.log(`Estado actualizado a: ${obtenerTextoEstado(nuevoEstado)}`);
}

// Actualizar tiempo específico sin recargar
function actualizarTiempoEspera(tiempoEnSegundos) {
    if (estadoActual && tiempoEnSegundos) {
        tiempoSegundos = tiempoEnSegundos;
        const tiempo = document.getElementById(`tiempo-${estadoActual}`);
        if (tiempo) {
            tiempo.textContent = formatearTiempo(tiempoSegundos);
        }
    }
}

// Formato mm:ss
function formatearTiempo(segundos) {
    const mins = Math.floor(segundos / 60);
    const segs = segundos % 60;
    return `${mins}:${segs.toString().padStart(2, '0')}`;
}

// Obtener texto de estado
function obtenerTextoEstado(id) {
    const estado = estados.find(e => e.id === id);
    return estado ? estado.nombre : "Estado desconocido";
}

// Actualizar solo el color del indicador
function actualizarIndicadorColor(estadoId) {
    const indicador = document.querySelector("#estado-container .status-indicator");
    if (!indicador) {
        console.warn("No se encontró el indicador de estado");
        return;
    }

    // Remover clases de estado anteriores
    estados.forEach(e => indicador.classList.remove(e.clase));

    // Agregar clase del estado actual
    const estado = estados.find(e => e.id === estadoId);
    if (estado) {
        indicador.classList.add(estado.clase);
    }
}

// Limpiar solo la sección de estado
function limpiarEstado() {
    if (timerInterval) {
        clearInterval(timerInterval);
        timerInterval = null;
    }

    tiempoSegundos = 0;
    estadoActual = null;

    // Limpiar solo elementos dentro del estado-container
    const statusText = document.getElementById("textoEstado");
    if (statusText) {
        statusText.textContent = "Sin información";
    }

    const indicador = document.querySelector("#estado-container .status-indicator");
    if (indicador) {
        estados.forEach(e => indicador.classList.remove(e.clase));
    }

    // Limpiar el timeline
    const timeline = document.getElementById("statusTimeline");
    if (timeline) {
        timeline.innerHTML = "";
    }
}

// Inicializar solo la sección de estado
function inicializarSeccionEstado() {
    console.log("Inicializando sección de estado de visita...");

    // Verificar que existe el contenedor
    const container = document.getElementById("estado-container");
    if (!container) {
        console.error("No se encontró el contenedor #estado-container");
        return;
    }

    // Obtener estado inicial (esto creará el timeline cuando se obtenga el estado)
    actualizarEstadoVisita();

    // Configurar actualización automática cada 10 segundos
    setInterval(actualizarEstadoVisita, 10000);
}

// Inicializar cuando el DOM esté listo

    inicializarSeccionEstado();



    inicializarSeccionEstado();
