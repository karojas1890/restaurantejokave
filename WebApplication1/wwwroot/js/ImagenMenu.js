class MenuViewer {
    constructor() {
        this.currentZoom = 1;
        this.maxZoom = 3;
        this.minZoom = 0.5;
        this.zoomStep = 0.2;
        this.isDragging = false;
        this.startX = 0;
        this.startY = 0;
        this.translateX = 0;
        this.translateY = 0;

        this.init();
    }

    init() {
        this.bindEvents();
        this.setupImageDragging();
        this.addKeyboardShortcuts();
    }

    bindEvents() {
        // Zoom buttons
        document.getElementById('zoomBtn').addEventListener('click', () => {
            this.toggleZoom();
        });

        document.getElementById('zoomInBtn').addEventListener('click', () => {
            this.zoomIn();
        });

        document.getElementById('zoomOutBtn').addEventListener('click', () => {
            this.zoomOut();
        });

        document.getElementById('resetZoomBtn').addEventListener('click', () => {
            this.resetZoom();
        });

        // Fullscreen modal
        document.getElementById('fullscreenBtn').addEventListener('click', () => {
            this.openFullscreen();
        });

        document.getElementById('closeModal').addEventListener('click', () => {
            this.closeFullscreen();
        });

        // Close modal on background click
        document.getElementById('fullscreenModal').addEventListener('click', (e) => {
            if (e.target.id === 'fullscreenModal') {
                this.closeFullscreen();
            }
        });

        // Mouse wheel zoom
        const menuImage = document.getElementById('menuImage');
        menuImage.addEventListener('wheel', (e) => {
            e.preventDefault();
            if (e.deltaY < 0) {
                this.zoomIn();
            } else {
                this.zoomOut();
            }
        });

        // Double click to reset
        menuImage.addEventListener('dblclick', () => {
            this.resetZoom();
        });
    }

    setupImageDragging() {
        const menuImage = document.getElementById('menuImage');

        // Mouse events
        menuImage.addEventListener('mousedown', (e) => {
            if (this.currentZoom > 1) {
                this.startDragging(e.clientX, e.clientY);
            }
        });

        document.addEventListener('mousemove', (e) => {
            if (this.isDragging) {
                this.drag(e.clientX, e.clientY);
            }
        });

        document.addEventListener('mouseup', () => {
            this.stopDragging();
        });

        // Touch events for mobile
        menuImage.addEventListener('touchstart', (e) => {
            if (this.currentZoom > 1 && e.touches.length === 1) {
                e.preventDefault();
                const touch = e.touches[0];
                this.startDragging(touch.clientX, touch.clientY);
            }
        });

        document.addEventListener('touchmove', (e) => {
            if (this.isDragging && e.touches.length === 1) {
                e.preventDefault();
                const touch = e.touches[0];
                this.drag(touch.clientX, touch.clientY);
            }
        });

        document.addEventListener('touchend', () => {
            this.stopDragging();
        });
    }

    startDragging(x, y) {
        this.isDragging = true;
        this.startX = x - this.translateX;
        this.startY = y - this.translateY;
        document.body.style.cursor = 'grabbing';
    }

    drag(x, y) {
        if (!this.isDragging) return;

        this.translateX = x - this.startX;
        this.translateY = y - this.startY;

        this.updateImageTransform();
    }

    stopDragging() {
        this.isDragging = false;
        document.body.style.cursor = 'default';
    }

    updateImageTransform() {
        const menuImage = document.getElementById('menuImage');
        menuImage.style.transform = `scale(${this.currentZoom}) translate(${this.translateX / this.currentZoom}px, ${this.translateY / this.currentZoom}px)`;
    }

    zoomIn() {
        if (this.currentZoom < this.maxZoom) {
            this.currentZoom = Math.min(this.currentZoom + this.zoomStep, this.maxZoom);
            this.updateImageTransform();
            this.updateZoomButtons();
        }
    }

    zoomOut() {
        if (this.currentZoom > this.minZoom) {
            this.currentZoom = Math.max(this.currentZoom - this.zoomStep, this.minZoom);
            this.updateImageTransform();
            this.updateZoomButtons();

            // Reset position if zoomed out too much
            if (this.currentZoom <= 1) {
                this.translateX = 0;
                this.translateY = 0;
            }
        }
    }

    resetZoom() {
        this.currentZoom = 1;
        this.translateX = 0;
        this.translateY = 0;
        this.updateImageTransform();
        this.updateZoomButtons();
    }

    toggleZoom() {
        if (this.currentZoom === 1) {
            this.currentZoom = 1.5;
        } else {
            this.resetZoom();
        }
        this.updateImageTransform();
        this.updateZoomButtons();
    }

    updateZoomButtons() {
        const zoomInBtn = document.getElementById('zoomInBtn');
        const zoomOutBtn = document.getElementById('zoomOutBtn');

        zoomInBtn.disabled = this.currentZoom >= this.maxZoom;
        zoomOutBtn.disabled = this.currentZoom <= this.minZoom;

        // Update main zoom button text
        const zoomBtn = document.getElementById('zoomBtn');
        const icon = zoomBtn.querySelector('i');
        if (this.currentZoom > 1) {
            icon.className = 'fas fa-search-minus';
            zoomBtn.innerHTML = '<i class="fas fa-search-minus"></i> Reducir Zoom';
        } else {
            icon.className = 'fas fa-search-plus';
            zoomBtn.innerHTML = '<i class="fas fa-search-plus"></i> Ampliar Menú';
        }
    }

    openFullscreen() {
        const modal = document.getElementById('fullscreenModal');
        modal.style.display = 'block';
        document.body.style.overflow = 'hidden';

        // Add fade in animation
        setTimeout(() => {
            modal.style.opacity = '1';
        }, 10);
    }

    closeFullscreen() {
        const modal = document.getElementById('fullscreenModal');
        modal.style.opacity = '0';

        setTimeout(() => {
            modal.style.display = 'none';
            document.body.style.overflow = 'auto';
        }, 300);
    }

    addKeyboardShortcuts() {
        document.addEventListener('keydown', (e) => {
            switch (e.key) {
                case 'Escape':
                    this.closeFullscreen();
                    break;
                case '+':
                case '=':
                    e.preventDefault();
                    this.zoomIn();
                    break;
                case '-':
                    e.preventDefault();
                    this.zoomOut();
                    break;
                case '0':
                    e.preventDefault();
                    this.resetZoom();
                    break;
                case 'f':
                case 'F':
                    if (e.ctrlKey || e.metaKey) {
                        e.preventDefault();
                        this.openFullscreen();
                    }
                    break;
            }
        });
    }
}

// Utility functions
function showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.textContent = message;

    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: ${type === 'success' ? '#4CAF50' : '#2196F3'};
        color: white;
        padding: 1rem 1.5rem;
        border-radius: 8px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        z-index: 1000;
        animation: slideIn 0.3s ease;
    `;

    document.body.appendChild(notification);

    setTimeout(() => {
        notification.style.animation = 'slideOut 0.3s ease';
        setTimeout(() => {
            document.body.removeChild(notification);
        }, 300);
    }, 3000);
}

// Add CSS animations for notifications
const style = document.createElement('style');
style.textContent = `
    @keyframes slideIn {
        from {
            transform: translateX(100%);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    
    @keyframes slideOut {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(100%);
            opacity: 0;
        }
    }
`;
document.head.appendChild(style);

// Initialize the menu viewer when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    const menuViewer = new MenuViewer();

    // Show welcome message
    setTimeout(() => {
        showNotification('¡Bienvenido al Restaurante JOKAVE! Usa los controles para explorar nuestro menú.', 'success');
    }, 1000);

    // Add loading effect
    const menuImage = document.getElementById('menuImage');
    menuImage.addEventListener('load', () => {
        menuImage.style.opacity = '1';
    });

    // Add smooth scroll behavior
    document.documentElement.style.scrollBehavior = 'smooth';
});

// Performance optimization: Lazy loading for images
if ('IntersectionObserver' in window) {
    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src;
                img.classList.remove('lazy');
                imageObserver.unobserve(img);
            }
        });
    });

    document.querySelectorAll('img[data-src]').forEach(img => {
        imageObserver.observe(img);
    });
}