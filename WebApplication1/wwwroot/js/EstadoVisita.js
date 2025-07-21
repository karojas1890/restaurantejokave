let timerInterval = null;
let tiempoSegundos = 0;

let EstadoActual = null;


const estados = [
    { id: 1, nombre: "En espera", clase: "en-espera", style: "red" },
    { id: 2, nombre: "Asignado a mesa", clase: "asignado", style: "Black" },
    { id: 3, nombre: "Ordenando", clase: "ordenando" },
    { id: 4, nombre: "Esperando Orden", clase: "esperando-cuenta" },
    { id: 5, nombre: "Consumiendo", clase: "consumiendo" },
    { id: 6, nombre: "Solicitando cuenta", clase: "solicitando-cuenta" },
    { id: 7, nombre: "Pagando", clase: "pagando" },
    { id: 8, nombre: "Se retira después de pagar", clase: "retira-pagado" },
    { id: 9, nombre: "Se retira sin pagar", clase: "retira-sin-pago", style: "red" }
];

const STORAGE_KEY = "estadosVisita";


function guardarEstadoTiempo(idEstado, tiempo) {
    let estadosGuardados = JSON.parse(localStorage.getItem(STORAGE_KEY)) || [];

    const index = estadosGuardados.findIndex(e => e.idEstado === idEstado);
    if (index >= 0) {
        estadosGuardados[index].tiempo = tiempo;
    } else {
        estadosGuardados.push({ idEstado, tiempo });
    }
    localStorage.setItem(STORAGE_KEY, JSON.stringify(estadosGuardados));
}


function obtenerEstadosGuardados() {
    return JSON.parse(localStorage.getItem(STORAGE_KEY)) || [];
}


function limpiarEstadosGuardados() {
    localStorage.removeItem(STORAGE_KEY);
}


function actualizarEstadoVisita() {
    fetch(`/Ordenes/EstadoVisita`, {
        method: 'GET',
        credentials: 'include'
    })
        .then(res => {
            if (!res.ok) throw new Error(`HTTP error! status: ${res.status}`);

            const contentType = res.headers.get('content-type') || '';
            if (!contentType.includes('application/json')) {
                throw new Error('Respuesta no es JSON');
            }

            return res.json();
        })
        .then(data => {
            if (data.error) {
                limpiarEstado();
                console.warn(data.error);
                return;
            }

            if (data.estadoActual && data.estadoActual !== EstadoActual) {
                cambiarEstado(data.estadoActual);
            }

            if (data.tiempoEspera) {
                actualizarTiempoEspera(data.tiempoEspera);
            }
        })
        .catch(err => {
            console.error('Error al obtener el estado de visita:', err);
            limpiarEstado();
        });
}



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

    if (document.getElementById(`estado-${estadoInfo.id}`)) {
        return; 
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


function cambiarEstado(nuevoEstado) {
    
    if (EstadoActual !== null) {
        guardarEstadoTiempo(EstadoActual, tiempoSegundos);
    }

   
    if (timerInterval) {
        clearInterval(timerInterval);
    }

    EstadoActual = nuevoEstado;

    const estadosGuardados = obtenerEstadosGuardados();
    const estadoGuardado = estadosGuardados.find(e => e.idEstado === nuevoEstado);
    tiempoSegundos = estadoGuardado ? estadoGuardado.tiempo : 0;

   
    const statusText = document.getElementById("textoEstado");
    if (statusText) {
        statusText.textContent = obtenerTextoEstado(nuevoEstado);
    }
    actualizarIndicadorColor(nuevoEstado);

    
    actualizarTimelineEstados(nuevoEstado);

   
    const tiempoElem = document.getElementById(`tiempo-${nuevoEstado}`);
    if (tiempoElem) {
        tiempoElem.textContent = formatearTiempo(tiempoSegundos);
    }

    
    guardarEstadoTiempo(nuevoEstado, tiempoSegundos);

   
    timerInterval = setInterval(() => {
        tiempoSegundos++;
        guardarEstadoTiempo(nuevoEstado, tiempoSegundos);
        const tiempo = document.getElementById(`tiempo-${nuevoEstado}`);
        if (tiempo) {
            tiempo.textContent = formatearTiempo(tiempoSegundos);
        }
    }, 1000);

    console.log(`Estado actualizado a: ${obtenerTextoEstado(nuevoEstado)}`);
}


function actualizarTiempoEspera(tiempoEnSegundos) {
    if (EstadoActual && tiempoEnSegundos) {
        tiempoSegundos = tiempoEnSegundos;
        const tiempo = document.getElementById(`tiempo-${EstadoActual}`);
        if (tiempo) {
            tiempo.textContent = formatearTiempo(tiempoSegundos);
        }
        
        guardarEstadoTiempo(EstadoActual, tiempoSegundos);
    }
}


function reconstruirTimeline() {
    const estadosGuardados = obtenerEstadosGuardados();
    estadosGuardados.forEach(({ idEstado, tiempo }) => {
        actualizarTimelineEstados(idEstado);
        const tiempoElem = document.getElementById(`tiempo-${idEstado}`);
        if (tiempoElem) {
            tiempoElem.textContent = formatearTiempo(tiempo);
        }
    });
}


function formatearTiempo(segundos) {
    const mins = Math.floor(segundos / 60);
    const segs = segundos % 60;
    return `${mins}:${segs.toString().padStart(2, '0')}`;
}


function obtenerTextoEstado(id) {
    const estado = estados.find(e => e.id === id);
    return estado ? estado.nombre : "Estado desconocido";
}


function actualizarIndicadorColor(estadoId) {
    const indicador = document.querySelector("#estado-container .status-indicator");
    if (!indicador) {
        console.warn("No se encontró el indicador de estado");
        return;
    }

    estados.forEach(e => indicador.classList.remove(e.clase));

  
    const estado = estados.find(e => e.id === estadoId);
    if (estado) {
        indicador.classList.add(estado.clase);
    }
}


function limpiarEstado() {
    if (timerInterval) {
        clearInterval(timerInterval);
        timerInterval = null;
    }

    tiempoSegundos = 0;
    EstadoActual = null;

    
    const statusText = document.getElementById("textoEstado");
    if (statusText) {
        statusText.textContent = "Sin información";
    }

    const indicador = document.querySelector("#estado-container .status-indicator");
    if (indicador) {
        estados.forEach(e => indicador.classList.remove(e.clase));
    }

   
    const timeline = document.getElementById("statusTimeline");
    if (timeline) {
        timeline.innerHTML = "";
    }

    limpiarEstadosGuardados();
}


function inicializarSeccionEstado() {
    console.log("Inicializando sección de estado de visita...");

    const container = document.getElementById("estado-container");
    if (!container) {
        console.error("No se encontró el contenedor #estado-container");
        return;
    }

    
    reconstruirTimeline();

    
    actualizarEstadoVisita();

 
    setInterval(actualizarEstadoVisita, 10000);
}


inicializarSeccionEstado();
