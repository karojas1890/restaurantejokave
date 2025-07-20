document.addEventListener("DOMContentLoaded", function () {
    const select = document.getElementById("NumeroTarjeta");
    const mensaje = document.querySelector('.success-message');
    const botonPagar = document.querySelector('.btn-pay');
    const infoTarjeta = document.getElementById("infoTarjeta");
    const saldoDisponibleElem = document.getElementById("saldoDisponible");
    const montoPagarElem = document.getElementById("montoPagar");

    const inputMes = document.getElementById('inputMes');
    const inputAnio = document.getElementById('inputAnio');
    const inputCVV = document.getElementById('inputCVV');
    const inputMonto = document.getElementById('inputMonto');

    // Obtener el monto desde el span (quitar símbolo ₡ y comas)
    const montoTexto = montoPagarElem.textContent.replace(/[₡,]/g, '').trim();
    const totalAPagar = parseFloat(montoTexto) || 0;

    botonPagar.disabled = true;

    function actualizarInformacionTarjeta() {
        const selected = select.options[select.selectedIndex];
        const saldo = parseFloat(selected.dataset.saldo);

        const mes = selected.getAttribute('data-mes');
        const anio = selected.getAttribute('data-anio');
        const cvv = selected.getAttribute('data-cvv');

        inputMes.value = mes;
        inputAnio.value = anio;
        inputCVV.value = cvv;
        inputMonto.value = totalAPagar;

        if (!isNaN(saldo)) {
            saldoDisponibleElem.textContent = `₡${saldo.toLocaleString()}`;
            infoTarjeta.style.display = "block";
            verificarSaldo(saldo);
        } else {
            infoTarjeta.style.display = "none";
            mensaje.innerHTML = '';
            botonPagar.disabled = true;
        }
    }

    function verificarSaldo(saldoDisponible) {
        if (saldoDisponible >= totalAPagar && totalAPagar > 0) {
            mensaje.innerHTML = `
                <div class="success-message" style="background: #7ED321;">
                    <span class="success-icon" style="color:green;">✓</span>
                    <span style="color:green;">Saldo suficiente para procesar el pago</span>
                </div>`;
            botonPagar.disabled = false;
        } else {
            mensaje.innerHTML = `
                <div class="success-message" style="background: linear-gradient(90deg, #fff3b0, #ffc971, #ff6961); padding: 10px; border-radius: 8px; color: #000; display: flex; align-items: center; gap: 10px;">
                    <span class="error-icon">✗</span> 
                    <span>Saldo insuficiente. <a style=" color: #000;" href="/Tarjetas/TraerTarjetas">Cargar saldo</a></span>
                </div>`;
            botonPagar.disabled = true;
        }
    }

    // Ejecutar al cargar la página si hay un valor seleccionado
    if (select.selectedIndex > 0) {
        actualizarInformacionTarjeta();
    }

    // Ejecutar al cambiar el valor del select
    select.addEventListener("change", actualizarInformacionTarjeta);
});
