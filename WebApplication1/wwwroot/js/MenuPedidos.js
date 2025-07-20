// Agregar item al pedido al hacer clic en el menú
document.querySelectorAll('.menu-item').forEach(item => {
    item.addEventListener('click', function () {
        const id = this.dataset.id;
        const name = this.querySelector('.menu-item-name')?.textContent || 'Producto';
        const price = parseInt(this.dataset.price);

        const container = document.getElementById('orderItemsContainer');
        let existingItem = container.querySelector(`.order-item[data-id='${id}']`);

        if (existingItem) {
            // Aumentar cantidad si ya existe
            const qtySpan = existingItem.querySelector('.quantity');
            qtySpan.textContent = parseInt(qtySpan.textContent) + 1;
            updateItemPrice(existingItem);
        } else {
            // Crear nuevo item
            const orderItem = document.createElement('div');
            orderItem.className = 'order-item';
            orderItem.dataset.id = id;
            orderItem.dataset.price = price;

            orderItem.innerHTML = `
                <span class="item-name">${name}</span>
                <div class="qty-controls">
                    <button class="qty-btn">−</button>
                    <span class="quantity">1</span>
                    <button class="qty-btn">+</button>
                </div>
                <span class="item-price">${price.toLocaleString()}</span>
                <button class="remove-btn"><i class="fas fa-trash"></i></button>
            `;

            container.appendChild(orderItem);
        }

        updateTotal();
    });
});

// Delegación de eventos para manejar clicks dinámicos (cantidad y eliminar)
document.getElementById('orderItemsContainer').addEventListener('click', function (e) {
    const btn = e.target.closest('button');
    if (!btn) return;

    const orderItem = btn.closest('.order-item');
    if (!orderItem) return;

    // Botones de cantidad
    if (btn.classList.contains('qty-btn')) {
        const qtySpan = orderItem.querySelector('.quantity');
        let quantity = parseInt(qtySpan.textContent);

        if (btn.textContent === '+') {
            quantity++;
        } else if (btn.textContent === '−' && quantity > 1) {
            quantity--;
        }

        qtySpan.textContent = quantity;
        updateItemPrice(orderItem);
        updateTotal();
    }

    // Botón de eliminar
    if (btn.classList.contains('remove-btn')) {
        orderItem.remove();
        updateTotal();
    }
});

// Actualiza el precio individual según cantidad
function updateItemPrice(orderItem) {
    const price = parseInt(orderItem.dataset.price);
    const quantity = parseInt(orderItem.querySelector('.quantity').textContent);
    const priceElement = orderItem.querySelector('.item-price');
    priceElement.textContent = (price * quantity).toLocaleString();
}

// Suma el total general del pedido
function updateTotal() {
    let total = 0;
    document.querySelectorAll('.order-item .item-price').forEach(price => {
        total += parseInt(price.textContent.replace(/[.,]/g, ''));
    });

    document.querySelector('.total-amount').textContent = '₡' + total.toLocaleString();
}

//desde aqui se gestiona el controlador para la orden 
document.getElementById('btnOrder').addEventListener('click', async () => {
    const orderItemsContainer = document.getElementById('orderItemsContainer');
    const orderItemsDivs = orderItemsContainer.querySelectorAll('.order-item');

    
    const items = Array.from(orderItemsDivs).map(div => {
        const idProducto = parseInt(div.dataset.id);
        const nombre = div.querySelector('.item-name').textContent;
        const cantidad = parseInt(div.querySelector('.quantity').textContent);
        const precioUnitario = parseInt(div.dataset.price);
        const precioTotal = cantidad * precioUnitario;

        return {
            IdProducto: idProducto,
            Nombre: nombre,
            Cantidad: cantidad,
            PrecioUnitario: precioUnitario,
            PrecioTotal: precioTotal
        };
    });

    
    const pedido = {
        idMesa: parseInt(document.getElementById('orderNumber').textContent), 
        items: items
    };
    
    try {
        const response = await fetch('/Menu/RealizarPedido', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(pedido)
        });

        if (!response.ok) {
            const errorData = await response.json();
            mostrarModal("Error", errorData.message || 'Error en el pedido');
            return;
        }

        const data = await response.json();
        mostrarModal("Éxito", `Pedido realizado con éxito. ID orden: ${data.ordenId}`);
        
    } catch (error) {
        mostrarModal("Error", 'Error al enviar el pedido: ' + error.message);
    }
});
