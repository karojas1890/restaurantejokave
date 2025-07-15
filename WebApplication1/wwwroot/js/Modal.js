
    function mostrarModal(titulo, mensaje) {
        document.getElementById("modalTitulo").innerText = titulo;
    document.getElementById("modalTexto").innerText = mensaje;
    document.getElementById("modalPersonalizado").style.display = "flex";
    }

    function cerrarModal() {
        document.getElementById("modalPersonalizado").style.display = "none";
    }

