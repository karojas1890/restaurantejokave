class PedidoManager {
    constructor() {
        this.products = {};
        this.initialize();
    }

    initialize() {
        this.loadProducts();
        this.setupEventListeners();
        this.updateOrderNumber();
    }

    loadProducts() {
        document.querySelectorAll('.menu-item').forEach(item => {
            const productId = item.dataset.id;
            this.products[productId] = {
                id: productId,
                name: item.querySelector('.menu-item-name').textContent,
                price: parseFloat(item.dataset.price),
                image: item.querySelector('img').src,
                description: item.querySelector('.menu-item-desc')?.textContent || ''
            };
        });
    }

    setupEventListeners() {
        // Menú items
        document.querySelectorAll('.menu-item').forEach(item => {
            item.addEventListener('click', () => this.addItemToOrder(item.dataset.id));
        });

        // Búsqueda
        document.querySelector('.search-input').addEventListener('input', (e) => this.handleSearch(e));
        document.querySelector('.search-btn').addEventListener('click', () => this.performSearch());

        // Botones de orden
        document.getElementById('btnOrder').addEventListener('click', () => this.submitOrder());
        document.getElementById('btnTotal').addEventListener('click', () => this.showTotal());

        // Delegación de eventos para los items del pedido
        document.getElementById('orderItemsContainer').addEventListener('click', (e) => {
            if (e.target.classList.contains('remove-btn')) {
                this.removeOrderItem(e.target.closest('.order-item'));
            } else if (e.target.classList.contains('qty-btn')) {
                this.adjustQuantity(e.target);
            }
        });
    }

    addItemToOrder(productId) {
        const product = this.products[productId];
        const orderContainer = document.getElementById('orderItemsContainer');
        const existingItem = orderContainer.querySelector(`.order-item[data-id="${productId}"]`);

        if (existingItem) {
            this.increaseQuantity(existingItem);
        } else {
            this.createNewOrderItem(product);
        }

        this.updateTotal();
    }

    createNewOrderItem(product) {
        const orderContainer = document.getElementById('orderItemsContainer');

        const newOrderItem = document.createElement('div');
        newOrderItem.className = 'order-item';
        newOrderItem.dataset.id = product.id;
        newOrderItem.innerHTML = `
            <button class="remove-btn"><i class="fas fa-times"></i></button>
            <img src="${product.image}" alt="${product.name}" onerror="this.src='@Url.Content("~/images/default.png")'">
            <div class="item-info">
                <span class="item-name">${product.name}</span>
                <div class="quantity-controls">
                    <button class="qty-btn"><i class="fas fa-minus"></i></button>
                    <span class="quantity">1</span>
                    <button class="qty-btn"><i class="fas fa-plus"></i></button>
                </div>
            </div>
            <span class="item-price">₡${product.price.toLocaleString()}</span>
        `;

        orderContainer.appendChild(newOrderItem);
    }

    increaseQuantity(orderItem) {
        const quantityElement = orderItem.querySelector('.quantity');
        let currentQuantity = parseInt(quantityElement.textContent);
        quantityElement.textContent = currentQuantity + 1;
        this.updateItemPrice(orderItem);
    }

    adjustQuantity(button) {
        const orderItem = button.closest('.order-item');
        const quantityElement = orderItem.querySelector('.quantity');
        let quantity = parseInt(quantityElement.textContent);

        if (button.querySelector('.fa-plus')) {
            quantity++;
        } else if (button.querySelector('.fa-minus') && quantity > 1) {
            quantity--;
        }

        quantityElement.textContent = quantity;
        this.updateItemPrice(orderItem);
        this.updateTotal();
    }

    removeOrderItem(orderItem) {
        orderItem.remove();
        this.updateTotal();
    }

    updateItemPrice(orderItem) {
        const quantity = parseInt(orderItem.querySelector('.quantity').textContent);
        const productId = orderItem.dataset.id;
        const priceElement = orderItem.querySelector('.item-price');
        const product = this.products[productId];
        const totalPrice = product.price * quantity;

        priceElement.textContent = `₡${totalPrice.toLocaleString()}`;
    }

    updateTotal() {
        let total = 0;
        document.querySelectorAll('.order-item .item-price').forEach(priceElement => {
            const priceText = priceElement.textContent.replace(/[^0-9]/g, '');
            total += parseInt(priceText);
        });

        document.querySelector('.total-amount').textContent = `₡${total.toLocaleString()}`;
    }

    handleSearch(e) {
        const term = e.target.value.trim();
        if (term.length > 2) {
            this.performSearch(term);
        } else {
            document.getElementById('searchResults').innerHTML = '';
            document.getElementById('searchResults').style.display = 'none';
        }
    }

    performSearch(term = null) {
        const searchTerm = term || document.querySelector('.search-input').value.trim();

        if (searchTerm.length < 1) {
            // Mostrar todos los productos si la búsqueda está vacía
            document.querySelectorAll('.menu-item').forEach(item => {
                item.style.display = 'block';
            });
            return;
        }

        fetch(`/Menu/Buscar?term=${encodeURIComponent(searchTerm)}`)
            .then(response => response.json())
            .then(data => {
                const resultsContainer = document.getElementById('searchResults');
                resultsContainer.innerHTML = '';

                if (data.length > 0) {
                    data.forEach(item => {
                        const resultItem = document.createElement('div');
                        resultItem.className = 'search-result-item';
                        resultItem.textContent = item.nombre;
                        resultItem.addEventListener('click', () => {
                            this.addItemToOrder(item.idProducto.toString());
                            resultsContainer.style.display = 'none';
                        });
                        resultsContainer.appendChild(resultItem);
                    });
                    resultsContainer.style.display = 'block';
                } else {
                    const noResults = document.createElement('div');
                    noResults.className = 'search-no-results';
                    noResults.textContent = 'No se encontraron resultados';
                    resultsContainer.appendChild(noResults);
                    resultsContainer.style.display = 'block';
                }
            });
    }

    submitOrder() {
        const orderItems = document.querySelectorAll('.order-item');
        if (orderItems.length === 0) {
            alert('No hay items en el pedido');
            return;
        }

        const mesaSelect = document.getElementById('mesaSelect');
        const pedido = {
            IdMesa: parseInt(mesaSelect.value),
            Items: Array.from(orderItems).map(item => {
                const productId = item.dataset.id;
                const product = this.products[productId];
                const quantity = parseInt(item.querySelector('.quantity').textContent);
                const priceText = item.querySelector('.item-price').textContent.replace(/[^0-9]/g, '');

                return {
                    IdProducto: parseInt(productId),
                    Nombre: product.name,
                    Cantidad: quantity,
                    PrecioUnitario: product.price,
                    PrecioTotal: parseInt(priceText)
                };
            })
        };

        fetch('/Menu/RealizarPedido', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(pedido)
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    alert(`¡Pedido #${data.ordenId} enviado a la cocina!`);
                    document.getElementById('orderItemsContainer').innerHTML = '';
                    this.updateTotal();
                    this.updateOrderNumber();
                } else {
                    alert('Error al procesar el pedido');
                }
            })
            .catch(error => {
                console.error('Error:', error);
                alert('Error al enviar el pedido');
            });
    }

    showTotal() {
        const total = document.querySelector('.total-amount').textContent;
        alert(`Total del pedido: ${total}`);
    }

    updateOrderNumber() {
        const orderNumberElement = document.getElementById('orderNumber');
        orderNumberElement.textContent = Math.floor(Math.random() * 900) + 100;
    }
}

// Inicializar cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', () => {
    new PedidoManager();
});
