function adjustNavbarVisibility() {
    const zoomLevel = Math.round(window.devicePixelRatio * 100);
    const windowWidth = window.innerWidth;
    const screenWidth = screen.width;
    const navbarToggler = document.getElementById('navbarToggler');
    const navbarNav = document.querySelector('.navbar-nav');
    const iconElement = document.getElementById('iconElement');

    if (!navbarToggler || !navbarNav || !iconElement) {
        console.error('Elementos del navbar no encontrados.');
        return;
    }

    // Mostrar opciones del menú si la ventana está completamente abierta y el zoom está entre 25% y 67%
    if (windowWidth >= screenWidth && zoomLevel >= 25 && zoomLevel <= 67) {
        navbarToggler.style.display = 'none';
        navbarNav.style.display = 'flex';
    } else {
        navbarToggler.style.display = 'block';
        navbarNav.style.display = 'none';
    }

    // Mostrar el ícono si el ancho es <= 810 o el zoom está entre 100% y 500%
    if (windowWidth <= 810 || (zoomLevel >= 100 && zoomLevel <= 500)) {
        iconElement.style.display = 'block';
    } else {
        iconElement.style.display = 'none';
    }
}
window.addEventListener('load', adjustNavbarVisibility);
window.addEventListener('resize', adjustNavbarVisibility);
