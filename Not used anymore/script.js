document.addEventListener('DOMContentLoaded', () => {
    const loginForm = document.getElementById('loginForm');
    const errorMessage = document.getElementById('errorMessage');

    loginForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        const username = document.getElementById('username').value;
        const password = document.getElementById('password').value;

        const response = await fetch('/api/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, password })
        });

        const data = await response.json();
        if (response.ok) {
            window.location.href = 'dashboard.html';
            
        } else {
            errorMessage.textContent = data.message || 'Login failed';
        }
    });

    
    const registerBtn = document.querySelector('.register-btn');
    registerBtn.addEventListener('click', () => {
        window.location.href = 'register.html';
    });
});