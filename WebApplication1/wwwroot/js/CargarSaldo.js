const amountButtons = document.querySelectorAll('.amount-btn');
const amountInput = document.querySelector('.amount-input');

amountButtons.forEach(button => {
    button.addEventListener('click', () => {
        const amount = button.textContent;
        amountInput.value = amount;
    });
});