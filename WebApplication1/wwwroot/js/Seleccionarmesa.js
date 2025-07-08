// Funcionalidad para seleccionar mesa
document.querySelectorAll('.table-card').forEach(card => {
    card.addEventListener('click', function () {
        const tableNumber = this.querySelector('.table-number').textContent;
        const capacity = this.querySelector('.table-capacity').textContent;

        // Remover selección previa
        document.querySelectorAll('.table-card').forEach(c => c.classList.remove('selected'));

        // Agregar selección actual
        this.classList.add('selected');

        
    });
});

// Funcionalidad de búsqueda
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