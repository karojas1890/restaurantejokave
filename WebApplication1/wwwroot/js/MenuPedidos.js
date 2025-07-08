// Funcionalidad para agregar/quitar cantidades
document.querySelectorAll('.qty-btn').forEach(btn => {
    btn.addEventListener('click', function () {
        const quantitySpan = this.parentNode.querySelector('.quantity');
        let quantity = parseInt(quantitySpan.textContent);

        if (this.textContent === '+') {
            quantity++;
        } else if (this.textContent === '−' && quantity > 0) {
            quantity--;
        }

        quantitySpan.textContent = quantity;

        // Actualizar precio 
        updateItemPrice(this.closest('.order-item'));
    });
});

// Funcionalidad para remover items
document.querySelectorAll('.remove-btn').forEach(btn => {
    btn.addEventListener('click', function () {
        this.parentNode.remove();
        updateTotal();
    });
});

function updateItemPrice(orderItem) {
    // Lógica básica para actualizar precios
    const quantity = parseInt(orderItem.querySelector('.quantity').textContent);
    const itemName = orderItem.querySelector('.item-name').textContent;
    const priceElement = orderItem.querySelector('.item-price');

    let basePrice = 0;
    switch (itemName) {
        case 'Carbonara': basePrice = 8000; break;
        case 'Margarita': basePrice = 5000; break;
        case 'Limonada': basePrice = 1500; break;
    }

    priceElement.textContent = (basePrice * quantity).toLocaleString();
}

function updateTotal() {
    let total = 0;
    document.querySelectorAll('.item-price').forEach(price => {
        total += parseInt(price.textContent.replace(/[.,]/g, ''));
    });
    document.querySelector('.total-amount').textContent = '₡' + total.toLocaleString();
}
