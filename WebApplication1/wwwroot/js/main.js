document.addEventListener("DOMContentLoaded", () => {
    // Navigation functionality
    const navItems = document.querySelectorAll(".nav-item[data-view]")
    const views = document.querySelectorAll(".view")

    navItems.forEach((item) => {
        item.addEventListener("click", function () {
            const targetView = this.getAttribute("data-view")

            // Remove active class from all nav items
            navItems.forEach((nav) => nav.classList.remove("active"))
            // Add active class to clicked item
            this.classList.add("active")

            // Hide all views
            views.forEach((view) => view.classList.remove("active"))
            // Show target view
            document.getElementById(targetView + "-view").classList.add("active")
        })
    })

    // Tab functionality for personal view
    const tabs = document.querySelectorAll(".tab")
    tabs.forEach((tab) => {
        tab.addEventListener("click", function () {
            tabs.forEach((t) => t.classList.remove("active"))
            this.classList.add("active")

            // Here you could add logic to switch between chefs and meseros content
            console.log("Switched to:", this.getAttribute("data-tab"))
        })
    })

    // Assignment button functionality
    const assignButtons = document.querySelectorAll(".assign-btn")
    assignButtons.forEach((button) => {
        button.addEventListener("click", function () {
            const area = this.textContent
            const waiterName = this.closest(".waiter-item").querySelector("strong").textContent
            alert(`Asignando ${waiterName} al ${area}`)
        })
    })

    // Update date and time
    function updateDateTime() {
        const now = new Date()
        const options = {
            weekday: "long",
            year: "numeric",
            month: "long",
            day: "numeric",
            hour: "2-digit",
            minute: "2-digit",
        }
        const dateTimeString = now.toLocaleDateString("es-ES", options)
        document.querySelector(".date-time").textContent = dateTimeString
    }

    // Update time every minute
    updateDateTime()
    setInterval(updateDateTime, 60000)

    // Tab functionality for inventory view
    const inventoryTabs = document.querySelectorAll("#inventory-view .tab")
    inventoryTabs.forEach((tab) => {
        tab.addEventListener("click", function () {
            // Remove active class from all inventory tabs
            inventoryTabs.forEach((t) => t.classList.remove("active"))
            this.classList.add("active")

            // Hide all tab contents
            document.querySelectorAll("#inventory-view .tab-content").forEach((content) => {
                content.classList.remove("active")
            })

            // Show selected tab content
            const tabName = this.getAttribute("data-tab")
            document.getElementById(tabName + "-content").classList.add("active")
        })
    })

    // Sales chart functionality
    function createSalesChart() {
        const canvas = document.getElementById("salesChart")
        if (!canvas) return

        const ctx = canvas.getContext("2d")
        const centerX = canvas.width / 2
        const centerY = canvas.height / 2
        const radius = 120

        const data = [
            { label: "Pizza Margarita", value: 35, color: "#2196F3" },
            { label: "Pizza Pepperoni", value: 25, color: "#4CAF50" },
            { label: "Pizza Vegetariana", value: 15, color: "#FF9800" },
            { label: "Pizza Hawaiana", value: 10, color: "#FF5722" },
            { label: "Otras", value: 15, color: "#9C27B0" },
        ]

        let currentAngle = -Math.PI / 2 // Start from top

        data.forEach((item) => {
            const sliceAngle = (item.value / 100) * 2 * Math.PI

            // Draw slice
            ctx.beginPath()
            ctx.moveTo(centerX, centerY)
            ctx.arc(centerX, centerY, radius, currentAngle, currentAngle + sliceAngle)
            ctx.closePath()
            ctx.fillStyle = item.color
            ctx.fill()

            // Draw label
            const labelAngle = currentAngle + sliceAngle / 2
            const labelX = centerX + Math.cos(labelAngle) * (radius + 30)
            const labelY = centerY + Math.sin(labelAngle) * (radius + 30)

            ctx.fillStyle = item.color
            ctx.font = "12px Arial"
            ctx.textAlign = "center"
            ctx.fillText(`${item.label} ${item.value}%`, labelX, labelY)

            currentAngle += sliceAngle
        })
    }

    // Initialize chart when sales view is shown
    const salesNavItem = document.querySelector('[data-view="sales"]')
    if (salesNavItem) {
        salesNavItem.addEventListener("click", () => {
            setTimeout(createSalesChart, 100) // Small delay to ensure canvas is visible
        })
    }

    // Simulate real-time data updates
    function simulateDataUpdates() {
        // Update random progress bars
        const progressBars = document.querySelectorAll(".progress-fill")
        progressBars.forEach((bar) => {
            if (Math.random() > 0.8) {
                // 20% chance to update
                const currentWidth = Number.parseInt(bar.style.width)
                const newWidth = Math.max(5, Math.min(100, currentWidth + (Math.random() - 0.5) * 10))
                bar.style.width = newWidth + "%"
                const textElement = bar.nextElementSibling
                if (textElement && textElement.classList.contains("progress-text")) {
                    textElement.textContent = Math.round(newWidth) + "%"
                }
            }
        })

        // Update sales numbers
        const salesAmounts = document.querySelectorAll(".sale-amount")
        salesAmounts.forEach((amount) => {
            if (amount.textContent.includes("₡") && Math.random() > 0.9) {
                const currentAmount = Number.parseInt(amount.textContent.replace(/[₡,]/g, ""))
                const newAmount = currentAmount + Math.floor(Math.random() * 5000)
                amount.textContent = "₡" + newAmount.toLocaleString()
            }
        })
    }

    // Update data every 30 seconds
    setInterval(simulateDataUpdates, 30000)

    // Add click effects to cards
    const cards = document.querySelectorAll(".area-card, .staff-card, .summary-card")
    cards.forEach((card) => {
        card.addEventListener("click", function () {
            this.style.transform = "scale(0.98)"
            setTimeout(() => {
                this.style.transform = "scale(1)"
            }, 150)
        })
    })

    // Alert actions functionality
    const executeBtn = document.querySelector(".execute-btn")
    if (executeBtn) {
        executeBtn.addEventListener("click", () => {
            showNotification("Acción ejecutada correctamente", "success")
        })
    }

    // Crisis simulation
    function simulateCrisisUpdates() {
        const impactBar = document.querySelector(".crisis-detail .progress-fill")
        if (impactBar && Math.random() > 0.7) {
            const currentWidth = Number.parseInt(impactBar.style.width)
            const newWidth = Math.max(10, Math.min(90, currentWidth + (Math.random() - 0.5) * 20))
            impactBar.style.width = newWidth + "%"

            const progressText = impactBar.nextElementSibling
            if (progressText) {
                progressText.textContent = Math.round(newWidth) + "%"
            }
        }
    }

  
    setInterval(simulateCrisisUpdates, 45000)
})


function showNotification(message, type = "info") {
    const notification = document.createElement("div")
    notification.className = `notification ${type}`
    notification.textContent = message
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 15px 20px;
        background: ${type === "success" ? "#4CAF50" : type === "error" ? "#FF4444" : "#2196F3"};
        color: white;
        border-radius: 5px;
        z-index: 1000;
        animation: slideIn 0.3s ease;
    `

    document.body.appendChild(notification)

    setTimeout(() => {
        notification.remove()
    }, 3000)
}

// Add CSS 
const style = document.createElement("style")
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
    
    .area-card, .staff-card, .summary-card {
        transition: transform 0.15s ease;
        cursor: pointer;
    }
    
    .area-card:hover, .staff-card:hover, .summary-card:hover {
        box-shadow: 0 4px 8px rgba(0,0,0,0.1);
    }
`
document.head.appendChild(style)
