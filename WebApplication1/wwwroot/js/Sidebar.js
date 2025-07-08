
document.addEventListener("DOMContentLoaded", () => {
    
    const navItems = document.querySelectorAll(".nav-item")
    const contentSections = document.querySelectorAll(".content-section")

    navItems.forEach((item) => {
        item.addEventListener("click", function () {
            const targetSection = this.getAttribute("data-section")

          
            navItems.forEach((nav) => nav.classList.remove("active"))
           
            this.classList.add("active")

            
            contentSections.forEach((section) => section.classList.remove("active"))
            
            const targetElement = document.getElementById(targetSection + "-section")
            if (targetElement) {
                targetElement.classList.add("active")
            }
        })
    })
})