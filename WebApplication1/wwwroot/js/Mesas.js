
const grupos = document.querySelectorAll('.mesa-grupo');
const totalGrupos = grupos.length;

let grupoActual = 0;

function cambiarGrupo(direccion) {
    const grupos = document.querySelectorAll('.mesa-grupo');
    grupos[grupoActual].classList.remove('activo');
    grupoActual += direccion;

    if (grupoActual < 0) grupoActual = 0;
    if (grupoActual >= grupos.length) grupoActual = grupos.length - 1;

    grupos[grupoActual].classList.add('activo');

    document.getElementById('btnAnterior').disabled = grupoActual === 0;
    document.getElementById('btnSiguiente').disabled = grupoActual === grupos.length - 1;
}

// Inicializar botones al cargar
window.onload = () => {
    document.getElementById('btnAnterior').disabled = true;
    if (document.querySelectorAll('.mesa-grupo').length <= 1) {
        document.getElementById('btnSiguiente').disabled = true;
    }
}

// Asegura que se muestre el primer grupo al cargar
document.addEventListener("DOMContentLoaded", () => {
    if (grupos.length > 0) grupos[0].classList.add('activo');
});

function seleccionarSilla(event, sillaId) {
    event.preventDefault();
    if (confirm("¿Deseas reservar esta silla?")) {
        fetch(`/Sillas/Reservar/${sillaId}`, {
            method: "POST"
        })
            .then(res => {
                if (res.ok) {
                    alert("Silla reservada con éxito.");
                    location.reload();
                } else {
                    alert("No se pudo reservar la silla.");
                }
            });
    }
}