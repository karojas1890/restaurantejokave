﻿// Funcionalidad para selección de estado de ánimo
document.querySelectorAll('.mood-btn').forEach(btn => {
    btn.addEventListener('click', function () {
        document.querySelectorAll('.mood-btn').forEach(b => b.classList.remove('selected'));
        this.classList.add('selected');
    });
});

// Funcionalidad corregida para calificación con estrellas
let currentRating = 0;

document.querySelectorAll('.star').forEach((star, index) => {
    star.addEventListener('click', function () {
        const rating = parseInt(this.dataset.rating);

        // Si se hace clic en la misma estrella que ya está seleccionada, deseleccionar
        if (currentRating === rating) {
            currentRating = 0;
            updateStars(0);
        } else {
            currentRating = rating;
            updateStars(rating);
        }
    });

    // Efecto hover
    star.addEventListener('mouseenter', function () {
        const rating = parseInt(this.dataset.rating);
        updateStars(rating);
    });
});

// Restaurar colores al salir del hover
document.querySelector('.star-rating').addEventListener('mouseleave', function () {
    updateStars(currentRating);
});

// Función para actualizar el estado visual de las estrellas
function updateStars(rating) {
    document.querySelectorAll('.star').forEach((star, index) => {
        star.classList.remove('active');
        if (index < rating) {
            star.classList.add('active');
        }
    });
}

// Inicializar estrellas en gris al cargar la página
document.addEventListener('DOMContentLoaded', function () {
    updateStars(0);
});

// Funcionalidad para botones de acción
document.querySelectorAll('.action-btn').forEach(btn => {
    btn.addEventListener('click', function () {
        const action = this.textContent.trim();
       
    });
});

// Funcionalidad para botones de emergencia
document.querySelectorAll('.emergency-btn, .chaos-btn').forEach(btn => {
    btn.addEventListener('click', function () {
        const emergency = this.textContent.trim();
        if (confirm(`¿Estás seguro de que quieres activar: ${emergency}?`)) {
            
        }
    });
});



//menu hamburguesa
const btn = document.getElementById('hamburgerBtn');
const menu = document.getElementById('menuHamburguesa');

btn.addEventListener('click', () => {
    menu.classList.toggle('show');
});


window.addEventListener('click', function (e) {
    if (!menu.contains(e.target) && !btn.contains(e.target)) {
        menu.classList.remove('show');
    }
});


window.addEventListener('click', function (e) {
    if (!menu.contains(e.target) && !btn.contains(e.target)) {
        menu.classList.remove('show');
    }
});


