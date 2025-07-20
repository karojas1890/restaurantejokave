let grupoActual = 0;
const grupos = document.querySelectorAll('.mesa-grupo');
const btnAnterior = document.getElementById('btnAnterior');
const btnSiguiente = document.getElementById('btnSiguiente');

function cambiarGrupo(direccion) {
    grupos[grupoActual].classList.remove('activo');
    grupoActual += direccion;

    if (grupoActual < 0) grupoActual = 0;
    if (grupoActual >= grupos.length) grupoActual = grupos.length - 1;

    grupos[grupoActual].classList.add('activo');

    btnAnterior.disabled = grupoActual === 0;
    btnSiguiente.disabled = grupoActual === grupos.length - 1;
}

btnAnterior.addEventListener('click', () => cambiarGrupo(-1));
btnSiguiente.addEventListener('click', () => cambiarGrupo(1));

// Inicializa estados de botones al cargar la página
btnAnterior.disabled = true;
if (grupos.length <= 1) btnSiguiente.disabled = true;
let sillaSeleccionada = null;

function seleccionarSilla(elemento) {
    if (!elemento.classList.contains("disponible")) {
        mostrarModal("Esta silla ya está ocupada.");
        return;
    }

    // Desmarcar silla previa
    if (sillaSeleccionada) {
        sillaSeleccionada.classList.remove("seleccionada");
    }

    // Marcar la nueva
    elemento.classList.add("seleccionada");
    sillaSeleccionada = elemento;

    const idSilla = elemento.dataset.sillaId;
    const idMesa = elemento.dataset.mesaId;

    console.log(`Silla seleccionada: ${idSilla}, Mesa: ${idMesa}`);

    fetch("/Mesa/ConfirmarSilla", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ idSilla, idMesa })
    })
        .then(response => {
            return response.json();
        })
        .then(data => {
            mostrarModal("Exito", data.message);

           
            setTimeout(() => {
                location.reload();
            }, 2000); 
        })
        .catch(error => {
            mostrarModal("Error", error.message);
        });


}


// Funcionalidad de busqueda
document.querySelector('.search-input').addEventListener('input', function (e) {
    const searchTerm = e.target.value.toLowerCase();
    const tables = document.querySelectorAll('.table-card');

    tables.forEach(table => {
        const tableNumber = table.querySelector('.table-number').textContent;
        if (tableNumber.includes(searchTerm)) {
            table.style.display = 'flex';
        } else {
            table.style.display = searchTerm === '' ? 'flex' : 'none';
        }
    });
});