// Variables globales
let form;
let createBtn;
let inputs;
// Inicializa validaciones
document.addEventListener('DOMContentLoaded', function () {
    form = document.getElementById('registrationForm');
    createBtn = document.getElementById('createBtn');
    inputs = document.querySelectorAll('.form-input');

    initializeValidation();
});

// Configura validaciones para cada campo
function initializeValidation() {
    setupFieldValidation('DocumentoIdentificacion', validateIdentificacion);
    setupFieldValidation('Nombre', validateNombre);
    setupFieldValidation('Apellido1', validateApellido);
    setupFieldValidation('Apellido2', validateApellidoOpcional);
    setupFieldValidation('Telefono', validateTelefono);
    setupFieldValidation('Email', validateCorreo);
    setupFieldValidation('Usuario1', validateUsuario);
    setupFieldValidation('Password', validateContrasena);
    setupFieldValidation('confirmarContrasena', validateConfirmarContrasena);
}

// Configura validacion para un campos especificos
function setupFieldValidation(fieldId, validationFunction) {
    const field = document.getElementById(fieldId);

    field.addEventListener('blur', function () {
        validationFunction(this.value, fieldId);
    });

    field.addEventListener('input', function () {
        clearFieldMessages(fieldId);
        if (fieldId === 'Contraseña') {
            updatePasswordStrength(this.value);
        }
    });
}

// Funciones de validacion
function validateIdentificacion(value, fieldId) {
    if (!value.trim()) {
        showFieldError(fieldId, 'La identificación es requerida');
        return false;
    }
    if (!/^\d{9}$/.test(value)) {
        showFieldError(fieldId, 'Debe contener 9 digitos');
        return false;
    }
    showFieldSuccess(fieldId, 'Identificación válida');
    return true;
}

function validateNombre(value, fieldId) {
    if (!value.trim()) {
        showFieldError(fieldId, 'El nombre es requerido');
        return false;
    }
    if (value.length < 2) {
        showFieldError(fieldId, 'Debe tener al menos 2 caracteres');
        return false;
    }
    showFieldSuccess(fieldId, 'Nombre válido');
    return true;
}
function validateApellido(value, fieldId) {
    const trimmedValue = value.trim();

    if (trimmedValue) {
        if (trimmedValue.length < 2) {
            showFieldError(fieldId, 'Debe tener al menos 2 caracteres');
            return false;
        }
        if (/\d/.test(trimmedValue)) {
            showFieldError(fieldId, 'No debe contener números');
            return false;
        }

        showFieldSuccess(fieldId, 'Apellido válido');
    }

    return true; // válido si está vacío
}
function validateApellidoOpcional(value, fieldId) {
    const trimmedValue = value.trim();

    if (trimmedValue) {
        if (trimmedValue.length < 2) {
            showFieldError(fieldId, 'Debe tener al menos 2 caracteres');
            return false;
        }
        if (/\d/.test(trimmedValue)) {
            showFieldError(fieldId, 'No debe contener números');
            return false;
        }

        showFieldSuccess(fieldId, 'Apellido válido');
    }

    return true;
}

function validateTelefono(value, fieldId) {
    if (!value.trim()) {
        showFieldError(fieldId, 'El teléfono es requerido');
        return false;
    }
    if (!/^\d{8}$/.test(value)) {
        showFieldError(fieldId, 'Debe contener 8 dígitos numéricos');
        return false;
    }
    showFieldSuccess(fieldId, 'Teléfono válido');
    return true;
}


function validateCorreo(value, fieldId) {
    if (!value.trim()) {
        showFieldError(fieldId, 'El correo es requerido');
        return false;
    }
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(value)) {
        showFieldError(fieldId, 'Formato de correo inválido');
        return false;
    }
    showFieldSuccess(fieldId, 'Correo válido');
    return true;
}

function validateUsuario(value, fieldId) {
    if (!value.trim()) {
        showFieldError(fieldId, 'El usuario es requerido');
        return false;
    }
    if (value.length < 4) {
        showFieldError(fieldId, 'Debe tener al menos 4 caracteres');
        return false;
    }
    if (!/^[a-zA-Z0-9_]+$/.test(value)) {
        showFieldError(fieldId, 'Solo letras, números');
        return false;
    }
    showFieldSuccess(fieldId, 'Usuario válido');
    return true;
}

function validateContrasena(value, fieldId) {
    if (!value) {
        showFieldError(fieldId, 'La contraseña es requerida');
        return false;
    }
    if (value.length < 8) {
        showFieldError(fieldId, 'Debe tener al menos 8 caracteres');
        return false;
    }
    if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(value)) {
        showFieldError(fieldId, 'Debe incluir mayúscula, minúscula y número');
        return false;
    }
    showFieldSuccess(fieldId, 'Contraseña segura');
    return true;
}

function validateConfirmarContrasena(value, fieldId) {
    const contrasena = document.getElementById('Password').value;
    if (!value) {
        showFieldError(fieldId, 'Confirma tu contraseña');
        return false;
    }
    if (value !== contrasena) {
        showFieldError(fieldId, 'Las contraseñas no coinciden');
        return false;
    }
    showFieldSuccess(fieldId, 'Contraseñas coinciden');
    return true;
}

// Actualizaa indicador de fortaleza de contraseña
function updatePasswordStrength(password) {
    const strengthIndicator = document.getElementById('passStrength');
    const strengthBar = document.getElementById('passStrengthBar');

    if (!password) {
        strengthIndicator.style.display = 'none';
        return;
    }

    strengthIndicator.style.display = 'block';

    let strength = 0;
    if (password.length >= 8) strength++;
    if (/[a-z]/.test(password)) strength++;
    if (/[A-Z]/.test(password)) strength++;
    if (/\d/.test(password)) strength++;
    if (/[^a-zA-Z\d]/.test(password)) strength++;

    const percentage = (strength / 5) * 100;
    strengthBar.style.width = percentage + '%';

    strengthBar.className = 'password-strength-bar';
    if (strength <= 2) {
        strengthBar.classList.add('strength-weak');
    } else if (strength <= 3) {
        strengthBar.classList.add('strength-medium');
    } else {
        strengthBar.classList.add('strength-strong');
    }
}

// Muestra mensaje de error
function showFieldError(fieldId, message) {
    const field = document.getElementById(fieldId);
    const errorElement = document.getElementById(fieldId + 'Error');
    const successElement = document.getElementById(fieldId + 'Success');

    field.classList.add('error');
    field.classList.remove('success');
    errorElement.textContent = message;
    errorElement.style.display = 'block';
    successElement.style.display = 'none';
}

// Mostrar mensaje de exito
function showFieldSuccess(fieldId, message) {
    const field = document.getElementById(fieldId);
    const errorElement = document.getElementById(fieldId + 'Error');
    const successElement = document.getElementById(fieldId + 'Success');

    field.classList.add('success');
    field.classList.remove('error');
    successElement.textContent = message;
    successElement.style.display = 'block';
    errorElement.style.display = 'none';
}

// Limpia mensajes de un campo
function clearFieldMessages(fieldId) {
    const field = document.getElementById(fieldId);
    const errorElement = document.getElementById(fieldId + 'Error');
    const successElement = document.getElementById(fieldId + 'Success');

    field.classList.remove('error', 'success');
    errorElement.style.display = 'none';
    successElement.style.display = 'none';
}

// Maneja envio del formulario
function handleFormSubmit(e) {
    e.preventDefault();

    // Valida todos los campos
    const validations = [
        validateIdentificacion(document.getElementById('DocumentoIdentificacion').value, 'DocumentoIdentificacion'),
        validateNombre(document.getElementById('Nombre').value, 'Nombre'),
        validateApellido(document.getElementById('Apellido1').value, 'Apellido1'),
        validateApellidoOpcional(document.getElementById('Apellido2').value, 'Apellido2'),
        validateTelefono(document.getElementById('Telefono').value, 'Telefono'),
        validateCorreo(document.getElementById('Email').value, 'Email'),
        validateUsuario(document.getElementById('Usuario1').value, 'Usuario1'),
        validateContrasena(document.getElementById('Password').value, 'Password'),
        validateConfirmarContrasena(document.getElementById('confirmarContrasena').value, 'confirmarContrasena')
    ];


    const isValid = validations.every(validation => validation);

    if (isValid) {
        submitForm();
    } else {
        alert('Por favor corrige los errores en el formulario');
    }
}

// Enviar formulario
function submitForm() {
    createBtn.classList.add('loading');
    createBtn.disabled = true;

    // Armar objeto con datos del formulario (usando FormData)
    const formData = new FormData(form);

    // Enviar datos con fetch al backend
    fetch('/Usuarios/CrearCuenta', {
        method: 'POST',
        body: formData,
    })
        .then(response => {
            if (response.ok) {
                return response.text(); // O JSON si el backend devuelve JSON
            }
            throw new Error('Error en la respuesta del servidor');
        })
        .then(data => {
            alert('Cuenta creada exitosamente.');
            form.reset();
            inputs.forEach(input => clearFieldMessages(input.id));
            document.getElementById('passwordStrength').style.display = 'none';
        })
        .catch(error => {
            alert('Error al enviar los datos: ' + error.message);
        })
        .finally(() => {
            createBtn.classList.remove('loading');
            createBtn.disabled = false;
        });
}

