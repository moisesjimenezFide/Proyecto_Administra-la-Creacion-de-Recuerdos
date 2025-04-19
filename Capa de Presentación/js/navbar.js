function adjustNavbarVisibility() {
    const zoomLevel = Math.round(window.devicePixelRatio * 100);
    const windowWidth = window.innerWidth;
    const screenWidth = screen.width; // Ancho de la pantalla
    const navbarToggler = document.getElementById('navbarToggler');
    const navbarNav = document.querySelector('.navbar-nav');
    const iconElement = document.getElementById('iconElement');

    // Mostrar opciones del menú si la ventana está completamente abierta y el zoom está entre 25% y 67%
    if (windowWidth >= screenWidth && zoomLevel >= 25 && zoomLevel <= 67) {
        navbarToggler.style.display = 'none';
        navbarNav.style.display = 'flex';
    }
    // Mostrar ícono del menú en otras condiciones
    else if (windowWidth <= 768 ||
        (zoomLevel >= 25 && zoomLevel <= 90 && windowWidth >= 1080) ||
        (zoomLevel >= 100 && zoomLevel <= 500) ||
        windowWidth > 768) {
        navbarToggler.style.display = 'block';
        navbarNav.style.display = 'none';
    }
    // Mostrar opciones del menú por defecto
    else {
        navbarToggler.style.display = 'none';
        navbarNav.style.display = 'flex';
    }

    // Mostrar el ícono si el ancho es <= 810 o el zoom está entre 100% y 500%
    if (windowWidth <= 810 || (zoomLevel >= 100 && zoomLevel <= 500)) {
        iconElement.style.display = 'block'; // Muestra el ícono
    } else {
        iconElement.style.display = 'none'; // Oculta el ícono
    }

    // Lógica adicional para el navbar
    if (windowWidth <= 768 || (zoomLevel >= 100 && zoomLevel <= 500)) {
        navbarToggler.style.display = 'block';
        navbarNav.style.display = 'none';
    } else {
        navbarToggler.style.display = 'none';
        navbarNav.style.display = 'flex';
    }
}
window.addEventListener('load', adjustNavbarVisibility);
window.addEventListener('resize', adjustNavbarVisibility);