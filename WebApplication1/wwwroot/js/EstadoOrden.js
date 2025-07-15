
const idOrden = 2;//hay que ver como se recoje el id

function actualizarEstadoOrden() {
    fetch(`/Ordenes/EstadoOrden?id=${idOrden}`)
        .then(res => res.json())
        .then(data => {
            if (data.error) {
                // Orden no encontrada
                limpiarEstado();
                console.warn(data.error);
                return;
            }


            actualizarBarra(data.progreso);
        })
        .catch(err => {
            console.error('Error fetching estado:', err);
            limpiarEstado();
        });
}

function limpiarEstado() {
    // O resetear la barra
    document.querySelector('.progress-bar').style.width = '0%';


    document.querySelectorAll('.progress-dot').forEach(dot => {
        dot.classList.remove('dot-yellow', 'dot-green', 'dot-magenta', 'dot-cyan');
        dot.style.backgroundColor = '#ccc';
    });


    const statusText = document.querySelector('.order-status-header span:last-child');
    if (statusText) statusText.textContent = 'Orden no encontrada';
}

function actualizarBarra(progreso) {


    const progressBar = document.querySelector('.progress-bar');
    const dots = document.querySelectorAll('.progress-dot');
    const statusText = document.querySelector('.order-status-header span:last-child');

    // Limpia clases
    dots.forEach(dot => {
        dot.classList.remove('dot-yellow', 'dot-green', 'dot-magenta', 'dot-cyan');
        dot.style.backgroundColor = '';
    });

    switch (progreso) {
        case 1:
            progressBar.style.width = '25%';
            dots[0].classList.add('dot-yellow');
            if (statusText) statusText.textContent = 'Mesero tomando orden';
            break;
        case 2:
            progressBar.style.width = '50%';
            dots[0].classList.add('dot-yellow');
            dots[1].classList.add('dot-green');
            if (statusText) statusText.textContent = 'Pedido enviado a cocina';
            break;
        case 3:
            progressBar.style.width = '75%';
            dots[0].classList.add('dot-yellow');
            dots[1].classList.add('dot-green');
            dots[2].classList.add('dot-magenta');
            if (statusText) statusText.textContent = 'Cocina preparando orden';
            break;
        case 4:
            progressBar.style.width = '100%';
            dots[0].classList.add('dot-yellow');
            dots[1].classList.add('dot-green');
            dots[2].classList.add('dot-magenta');
            dots[3].classList.add('dot-cyan');
            if (statusText) statusText.textContent = 'Entregando orden al cliente';
            break;
        default:

            limpiarEstado();
    }
}

// Smart polling cada 1 minuto
setInterval(actualizarEstadoOrden, 10000);

// carga la 
actualizarEstadoOrden();
