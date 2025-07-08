// Variables globales
let resendTimer = 0;
let resendInterval;

// Inicializar cuando la página cargue
document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('verificationForm');
    const codeInput = document.getElementById('verificationCode');
    const resendBtn = document.getElementById('resendBtn');
    const backBtn = document.getElementById('backBtn');
    const loginLink = document.getElementById('loginLink');
    const errorMessage = document.getElementById('errorMessage');
    const successMessage = document.getElementById('successMessage');

 
    codeInput.addEventListener('input', function () {
        const value = this.value.replace(/\D/g, ''); // Solo números
        this.value = value;

        // Limpia  mensajes
        hideMessages();
        codeInput.classList.remove('error');

        // Valida la longitud
        if (value.length === 6) {
            showSuccessMessage();
        }
    });

    // Envío del formulario
    
    form.addEventListener('submit', function (e) {
        e.preventDefault();
        const code = codeInput.value;

        if (validateCode(code)) {
            verifyCode(code);
        } else {
            showErrorMessage();
        }
    });

    // Botón reenviar código
    resendBtn.addEventListener('click', function () {
        if (resendTimer === 0) {
            resendCode();
        }
    });

    // Botón volver al registro
    backBtn.addEventListener('click', function () {
        
            window.history.back();
        
    });

  
});

// Función para validar el código
function validateCode(code) {
    return /^\d{6}$/.test(code);
}


//aqui va la logica para verificar con el controlador 
function verifyCode(code) {
    // Simular verificación
    const verifyBtn = document.querySelector('.verify-btn');
    verifyBtn.textContent = 'Verificando...';
    verifyBtn.disabled = true;

    setTimeout(() => {
        // Simular respuesta del servidor
        if (code === '123456') {
            alert('¡Código verificado correctamente! Redirigiendo...');
            // Aqui redirige a la siguiente pantalla
        } else {
            showErrorMessage('Código incorrecto. Inténtalo de nuevo.');
        }

        verifyBtn.textContent = 'Verificar código';
        verifyBtn.disabled = false;
    }, 2000);
}

// Funcion para reenviar codigo
function resendCode() {
    const resendBtn = document.getElementById('resendBtn');
    resendTimer = 60; // 60 segundos

    // Deshabilita boton y mostrar contador
    resendBtn.disabled = true;
    updateResendButton();

    
    // Iniciar contador regresivo
    resendInterval = setInterval(() => {
        resendTimer--;
        updateResendButton();

        if (resendTimer === 0) {
            clearInterval(resendInterval);
            resendBtn.disabled = false;
        }
    }, 1000);
}

// Actualizar texto del boton de reenvio
function updateResendButton() {
    const resendBtn = document.getElementById('resendBtn');
    const span = resendBtn.querySelector('span:last-child');

    if (resendTimer > 0) {
        span.textContent = `Reenviar código (${resendTimer}s)`;
    } else {
        span.textContent = 'Reenviar código';
    }
}

// Mostrar mensaje de error
function showErrorMessage(message = 'Código inválido. Debe contener 6 dígitos.') {
    const errorMessage = document.getElementById('errorMessage');
    const codeInput = document.getElementById('verificationCode');

    errorMessage.textContent = message;
    errorMessage.style.display = 'block';
    codeInput.classList.add('error');
    hideSuccessMessage();
}



// Ocultar mensajes
function hideMessages() {
    hideErrorMessage();
    hideSuccessMessage();
}

function hideErrorMessage() {
    document.getElementById('errorMessage').style.display = 'none';
}

function hideSuccessMessage() {
    document.getElementById('successMessage').style.display = 'none';
}

// Auto-focus en el input al cargar
window.addEventListener('load', function () {
    document.getElementById('verificationCode').focus();
});