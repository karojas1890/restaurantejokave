document.addEventListener("DOMContentLoaded", () => {

    function createSalesChart() {
        const canvas = document.getElementById("salesChart")
        if (!canvas) return

        const ctx = canvas.getContext("2d")
        const centerX = canvas.width / 2
        const centerY = canvas.height / 2
        const radius = 120


        ctx.clearRect(0, 0, canvas.width, canvas.height)

        const data = [
            { label: "Pizza Margarita", value: 35, color: "#2196F3" },
            { label: "Pizza Pepperoni", value: 25, color: "#4CAF50" },
            { label: "Pizza Vegetariana", value: 15, color: "#FF9800" },
            { label: "Pizza Hawaiana", value: 10, color: "#FF5722" },
            { label: "Otras", value: 15, color: "#9C27B0" },
        ]

        let currentAngle = -Math.PI / 2


        data.forEach((item) => {
            const sliceAngle = (item.value / 100) * 2 * Math.PI

            ctx.beginPath()
            ctx.moveTo(centerX, centerY)
            ctx.arc(centerX, centerY, radius, currentAngle, currentAngle + sliceAngle)
            ctx.closePath()
            ctx.fillStyle = item.color
            ctx.fill()


            ctx.strokeStyle = "#fff"
            ctx.lineWidth = 2
            ctx.stroke()

            currentAngle += sliceAngle
        })


        currentAngle = -Math.PI / 2
        data.forEach((item) => {
            const sliceAngle = (item.value / 100) * 2 * Math.PI
            const labelAngle = currentAngle + sliceAngle / 2

            const innerX = centerX + Math.cos(labelAngle) * (radius + 10)
            const innerY = centerY + Math.sin(labelAngle) * (radius + 10)
            const outerX = centerX + Math.cos(labelAngle) * (radius + 40)
            const outerY = centerY + Math.sin(labelAngle) * (radius + 40)

            ctx.beginPath()
            ctx.moveTo(innerX, innerY)
            ctx.lineTo(outerX, outerY)
            ctx.strokeStyle = item.color
            ctx.lineWidth = 2
            ctx.stroke()

            const labelText = `${item.value}%`
            ctx.font = "bold 12px Arial"
            const textWidth = ctx.measureText(labelText).width
            const textHeight = 16

            ctx.fillStyle = item.color
            ctx.fillRect(outerX - textWidth / 2 - 4, outerY - textHeight / 2 - 2, textWidth + 8, textHeight + 4)


            ctx.fillStyle = "#fff"
            ctx.textAlign = "center"
            ctx.textBaseline = "middle"
            ctx.fillText(labelText, outerX, outerY)

            currentAngle += sliceAngle
        })
    }


    const tabs = document.querySelectorAll(".sales-view .tab")
    tabs.forEach((tab) => {
        tab.addEventListener("click", function () {
            tabs.forEach((t) => t.classList.remove("active"))
            this.classList.add("active")

            const period = this.getAttribute("data-tab")
            console.log("Switched to:", period)


            updateSalesMetrics(period)


            setTimeout(() => {
                createSalesChart()
            }, 100)
        })
    })


    function updateSalesMetrics(period) {
        const metrics = {
            diario: {
                total: "₡120,000",
                orders: "89",
                average: "₡1,500",
            },
            semanal: {
                total: "₡1,200,000",
                orders: "573",
                average: "₡15,000",
            },
            mensual: {
                total: "₡4,800,000",
                orders: "2,156",
                average: "₡18,500",
            },
        }

        const data = metrics[period] || metrics.semanal

        const metricCards = document.querySelectorAll(".sales-metric-card .metric-value")
        if (metricCards.length >= 3) {
            metricCards[0].textContent = data.total
            metricCards[1].textContent = data.orders
            metricCards[2].textContent = data.average
        }
    }


    setTimeout(() => {
        createSalesChart()
    }, 500)


    const canvas = document.getElementById("salesChart")
    if (canvas) {
        canvas.addEventListener("mousemove", (event) => {
            const rect = canvas.getBoundingClientRect()
            const x = event.clientX - rect.left
            const y = event.clientY - rect.top

            const centerX = canvas.width / 2
            const centerY = canvas.height / 2
            const distance = Math.sqrt((x - centerX) ** 2 + (y - centerY) ** 2)

            if (distance <= 120) {
                canvas.style.cursor = "pointer"


                const angle = Math.atan2(y - centerY, x - centerX)
                const normalizedAngle = (angle + Math.PI / 2 + 2 * Math.PI) % (2 * Math.PI)


                let currentAngle = 0
                const data = [35, 25, 15, 10, 15]
                const labels = ["Pizza Margarita", "Pizza Pepperoni", "Pizza Vegetariana", "Pizza Hawaiana", "Otras"]

                for (let i = 0; i < data.length; i++) {
                    const sliceAngle = (data[i] / 100) * 2 * Math.PI
                    if (normalizedAngle >= currentAngle && normalizedAngle <= currentAngle + sliceAngle) {
                        canvas.title = `${labels[i]}: ${data[i]}%`
                        break
                    }
                    currentAngle += sliceAngle
                }
            } else {
                canvas.style.cursor = "default"
                canvas.title = ""
            }
        })

        canvas.addEventListener("click", (event) => {
            const rect = canvas.getBoundingClientRect()
            const x = event.clientX - rect.left
            const y = event.clientY - rect.top

            const centerX = canvas.width / 2
            const centerY = canvas.height / 2
            const distance = Math.sqrt((x - centerX) ** 2 + (y - centerY) ** 2)

            if (distance <= 120) {
                const angle = Math.atan2(y - centerY, x - centerX)
                const normalizedAngle = (angle + Math.PI / 2 + 2 * Math.PI) % (2 * Math.PI)

                let currentAngle = 0
                const data = [35, 25, 15, 10, 15]
                const labels = ["Pizza Margarita", "Pizza Pepperoni", "Pizza Vegetariana", "Pizza Hawaiana", "Otras"]

                for (let i = 0; i < data.length; i++) {
                    const sliceAngle = (data[i] / 100) * 2 * Math.PI
                    if (normalizedAngle >= currentAngle && normalizedAngle <= currentAngle + sliceAngle) {
                        showProductDetails(labels[i], data[i])
                        break
                    }
                    currentAngle += sliceAngle
                }
            }
        })
    }


    function showProductDetails(product, percentage) {
        const details = {
            "Pizza Margarita": { sales: "₡420,000", units: 200 },
            "Pizza Pepperoni": { sales: "₡300,000", units: 143 },
            "Pizza Vegetariana": { sales: "₡180,000", units: 86 },
            "Pizza Hawaiana": { sales: "₡120,000", units: 57 },
            Otras: { sales: "₡180,000", units: 87 },
        }

        const info = details[product] || { sales: "₡0", units: 0 }

        alert(`${product}\n\nParticipación: ${percentage}%\nVentas: ${info.sales}\nUnidades vendidas: ${info.units}`)
    }


    function animateChart() {
        const canvas = document.getElementById("salesChart")
        if (!canvas) return

        canvas.style.opacity = "0"
        canvas.style.transform = "scale(0.8)"
        canvas.style.transition = "all 0.5s ease"

        setTimeout(() => {
            canvas.style.opacity = "1"
            canvas.style.transform = "scale(1)"
        }, 200)
    }

    function updateSalesData() {

        const metricValues = document.querySelectorAll(".sales-metric-card .metric-value")

        metricValues.forEach((value, index) => {
            if (Math.random() > 0.8) {

                const currentText = value.textContent

                if (currentText.includes("₡")) {
                    const currentAmount = Number.parseInt(currentText.replace(/[₡,]/g, ""))
                    const newAmount = currentAmount + Math.floor(Math.random() * 50000)
                    value.textContent = "₡" + newAmount.toLocaleString()
                } else if (!currentText.includes("₡") && index === 1) {

                    const currentOrders = Number.parseInt(currentText)
                    const newOrders = currentOrders + Math.floor(Math.random() * 5)
                    value.textContent = newOrders.toString()
                }
            }
        })


        if (Math.random() > 0.95) {
            setTimeout(createSalesChart, 100)
        }
    }

    setInterval(updateSalesData, 30000)

    setTimeout(animateChart, 100)


    const legendItems = document.querySelectorAll(".legend-item")
    legendItems.forEach((item, index) => {
        item.addEventListener("click", () => {
            const labels = ["Pizza Margarita", "Pizza Pepperoni", "Pizza Vegetariana", "Pizza Hawaiana", "Otras"]
            const percentages = [35, 25, 15, 10, 15]

            if (labels[index] && percentages[index]) {
                showProductDetails(labels[index], percentages[index])
            }
        })

        // Add hover effect
        item.addEventListener("mouseenter", function () {
            this.style.backgroundColor = "#f0f0f0"
            this.style.cursor = "pointer"
            this.style.borderRadius = "4px"
            this.style.padding = "4px"
        })

        item.addEventListener("mouseleave", function () {
            this.style.backgroundColor = ""
            this.style.padding = ""
        })
    })


})