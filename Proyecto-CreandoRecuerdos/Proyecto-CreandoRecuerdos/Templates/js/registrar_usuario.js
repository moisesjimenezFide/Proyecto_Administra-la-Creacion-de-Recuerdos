// Función para inicializar todo el comportamiento
function initFormHandlers() {
    // Cargar SweetAlert desde CDN
    const script = document.createElement('script');
    script.src = 'https://cdn.jsdelivr.net/npm/sweetalert2@11';
    document.head.appendChild(script);

    // Cambio entre formularios
    document.querySelectorAll('input[name="formSwitch"]').forEach(radio => {
        radio.addEventListener('change', function () {
            const loginForm = document.getElementById('loginForm');
            const registerForm = document.getElementById('registerForm');
            const loginLabel = document.querySelector('label[for="loginSwitch"]');
            const registerLabel = document.querySelector('label[for="registerSwitch"]');

            loginForm.style.transition = 'all 0.4s ease-out';
            registerForm.style.transition = 'all 0.4s ease-out';

            loginForm.classList.toggle('active-form', this.id === 'loginSwitch');
            registerForm.classList.toggle('active-form', this.id === 'registerSwitch');

            loginLabel.classList.toggle('active', this.id === 'loginSwitch');
            registerLabel.classList.toggle('active', this.id === 'registerSwitch');
        });
    });

    /* Validación de coincidencia de contraseñas
    const registerForm = document.getElementById('registerForm');
    if (registerForm) {
        registerForm.addEventListener('submit', function (e) {
            const password = document.querySelector("input[name='password']").value;
            const confirmPassword = document.querySelector("input[name='contrasenna']").value;

            if (password !== confirmPassword) {
                e.preventDefault();
                showSweetAlert('error', '¡Error!', 'Las contraseñas no coinciden. Por favor, verifica.');
            }
        });
    }*/

    // Mostrar mensajes del servidor
    showServerMessages();
}
// Función para mostrar alertas SweetAlert
async function showSweetAlert(icon, title, text) {
    // Aseguramos que SweetAlert esté cargado
    if (typeof Swal === 'undefined') {
        await loadSweetAlert();
    }

    return Swal.fire({
        icon: icon,
        title: title,
        text: text,
        confirmButtonColor: '#B54885'
    });
}

// Cargar SweetAlert dinámicamente
function loadSweetAlert() {
    return new Promise((resolve) => {
        if (typeof Swal !== 'undefined') {
            resolve();
            return;
        }

        const script = document.createElement('script');
        script.src = 'https://cdn.jsdelivr.net/npm/sweetalert2@11';
        script.onload = resolve;
        document.head.appendChild(script);
    });
}

// Función para mostrar mensajes del servidor
async function showServerMessages() {
    const body = document.body;
    const errorMessage = body.getAttribute('data-error-message');
    const successMessage = body.getAttribute('data-success-message');

    if (errorMessage) {
        await showSweetAlert('error', 'Error', errorMessage);
        body.removeAttribute('data-error-message');
    }

    if (successMessage) {
        const result = await showSweetAlert('success', '¡Éxito!', successMessage);
        if (result.isConfirmed) {
            // Redirigir después de cerrar el mensaje de éxito
            window.location.href = window.location.href; // Recarga la página
        }
    }
}

// Función principal
async function initFormHandlers() {
    // Cargar SweetAlert primero
    await loadSweetAlert();

    // Cambio entre formularios
    document.querySelectorAll('input[name="formSwitch"]').forEach(radio => {
        radio.addEventListener('change', function () {
            const loginForm = document.getElementById('loginForm');
            const registerForm = document.getElementById('registerForm');
            const loginLabel = document.querySelector('label[for="loginSwitch"]');
            const registerLabel = document.querySelector('label[for="registerSwitch"]');

            loginForm.style.transition = registerForm.style.transition = 'all 0.4s ease-out';
            loginForm.classList.toggle('active-form', this.id === 'loginSwitch');
            registerForm.classList.toggle('active-form', this.id === 'registerSwitch');
            loginLabel.classList.toggle('active', this.id === 'loginSwitch');
            registerLabel.classList.toggle('active', this.id === 'registerSwitch');
        });
    });

    /* Validación de contraseñas
    const registerForm = document.getElementById('registerForm');
    if (registerForm) {
        registerForm.addEventListener('submit', async function (e) {
            const password = this.querySelector("input[name='password']").value;
            const confirmPassword = this.querySelector("input[name='contrasenna']").value;

            if (password !== confirmPassword) {
                e.preventDefault();
                await showSweetAlert('error', '¡Error!', 'Las contraseñas no coinciden. Por favor, verifica.');
            }
        });
    }*/

    // Mostrar mensajes del servidor
    await showServerMessages();
}

// Inicialización mejorada
if (document.readyState === 'complete' || document.readyState === 'interactive') {
    setTimeout(initFormHandlers, 1);
} else {
    document.addEventListener('DOMContentLoaded', initFormHandlers);
}