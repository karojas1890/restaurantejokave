const tarjetaVisible = document.getElementById("NumeroTarjetaVisible");
const tarjetaReal = document.getElementById("NumeroTarjeta");
let valorReal = "";

// Formatea mientras escribe
tarjetaVisible.addEventListener("input", (e) => {
    let numeros = e.target.value.replace(/\D/g, '');
    valorReal = numeros;

    // Formateo visual
    e.target.value = numeros.replace(/(.{4})/g, '$1 ').trim();

    // Actualiza el campo oculto
    tarjetaReal.value = valorReal;
});

// Enmascarar al salir
tarjetaVisible.addEventListener("blur", () => {
    if (valorReal.length >= 4) {
        tarjetaVisible.value = "•••• •••• •••• " + valorReal.slice(-4);
    }
});
