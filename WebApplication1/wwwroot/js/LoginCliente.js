document.getElementById('loginForm').addEventListener('submit', function (e) {
    // Validación básica del lado del cliente
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;

    if (!username || !password) {
        e.preventDefault();
        alert('Por favor completa todos los campos');
    }
});

// Manejar el clic en "Regístrate"
document.querySelector('.register-link').addEventListener('click', function (e) {
    e.preventDefault();
    alert('Redirigiendo a página de registro');
    // window.location.href = "/Auth/Register"; // Puedes crear esta acción más tarde
});

// Manejar el clic en "¿Olvidaste tu contraseña?"
document.querySelector('.forgot-password').addEventListener('click', function (e) {
    e.preventDefault();
    alert('Redirigiendo a recuperación de contraseña');
    // window.location.href = "/Auth/ForgotPassword"; // Puedes crear esta acción más tarde
});