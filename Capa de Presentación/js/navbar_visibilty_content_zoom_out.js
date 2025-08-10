function adjustNavbarVisibility() {
    const zoomLevel = Math.round(window.devicePixelRatio * 100);
    const windowWidth = window.innerWidth;
    const screenWidth = screen.width;
    const navbarToggler = document.getElementById('navbarToggler');
    const navbarNav = document.querySelector('.navbar-nav');
    const iconElement = document.getElementById('iconElement');

    // Caso 1: Mostrar opciones del menú si el zoom está entre 25% y 175% y la ventana está completamente abierta
    if (zoomLevel >= 25 && zoomLevel <= 175 && windowWidth >= screenWidth) {
        navbarToggler.style.display = 'none'; // Ocultar el botón de colapso
        navbarNav.style.display = 'flex'; // Mostrar las opciones del menú
        iconElement.style.display = 'none'; // Ocultar el ícono
    }

    // Caso 2: Mostrar el botón de colapso si el zoom está entre 25% y 175% y la ventana es de 810px o menos
    else if (zoomLevel >= 25 && zoomLevel <= 175 && windowWidth <= 810) {
        navbarToggler.style.display = 'block'; // Mostrar el botón de colapso
        navbarNav.style.display = 'none'; // Ocultar las opciones del menú
        iconElement.style.display = 'none'; // Ocultar el ícono
    }

    // Caso 3: Mostrar el ícono si el zoom está entre 200% y 500% y la ventana está completamente abierta
    else if (zoomLevel >= 200 && zoomLevel <= 500 && windowWidth >= screenWidth) {
        navbarToggler.style.display = 'none'; // Ocultar el botón de colapso
        navbarNav.style.display = 'flex'; // Mostrar las opciones del menú
        iconElement.style.display = 'none'; // Ocultar el ícono
    }

    // Caso 4: Mostrar el botón de colapso si el zoom está entre 200% y 500% y la ventana es de 810px o menos
    else if (zoomLevel >= 200 && zoomLevel <= 500 && windowWidth <= 810) {
        navbarToggler.style.display = 'block'; // Mostrar el botón de colapso
        navbarNav.style.display = 'none'; // Ocultar las opciones del menú
        iconElement.style.display = 'block'; // Mostrar el ícono
    }

    // Caso 5: Mostrar el ícono si el zoom está fuera del rango 25% a 500%
    else if (zoomLevel < 25 || zoomLevel > 500) {
        navbarToggler.style.display = 'block'; // Mostrar el botón de colapso
        navbarNav.style.display = 'none'; // Ocultar las opciones del menú
        iconElement.style.display = 'block'; // Mostrar el ícono
    }
}
window.addEventListener('load', adjustNavbarVisibility);
window.addEventListener('resize', adjustNavbarVisibility);

function adjustZoom() {
    const zoomLevel = Math.round(window.devicePixelRatio * 100);
    const content = document.getElementById('container-wrap');
    const footer = document.getElementById('footer');

    if (zoomLevel === 50) {
        const scale = 67 / zoomLevel;
        content.style.transform = `scale(${scale})`;
        content.style.transformOrigin = 'top center';
        content.style.width = '74.9%';
        content.style.marginLeft = '12.7%';
        const adjustedMarginBottom = `${footer.offsetHeight / scale}px`;
        content.style.marginBottom = adjustedMarginBottom;
        content.style.overflow = 'visible';
    }

    else if (zoomLevel === 33) {
        const scale = 67 / zoomLevel;
        content.style.transform = `scale(${scale})`;
        content.style.transformOrigin = 'top center';
        content.style.width = '49.5%';
        content.style.marginLeft = '25.5%';
        const adjustedMarginBottom = `${footer.offsetHeight / scale}px`;
        content.style.marginBottom = adjustedMarginBottom;
        content.style.overflow = 'visible';
    }

    else if (zoomLevel === 25) {
        const scale = 67 / zoomLevel;
        content.style.transform = `scale(${scale})`;
        content.style.transformOrigin = 'top center';
        content.style.width = '38%';
        content.style.marginLeft = '31.8%';
        const adjustedMarginBottom = `${footer.offsetHeight / scale}px`;
        content.style.marginBottom = adjustedMarginBottom;
        content.style.overflow = 'visible';
    }

    else {
        content.style.transform = '';
        content.style.transformOrigin = '';
        content.style.width = '100%';
        content.style.marginLeft = '';
        content.style.marginBottom = '';
        content.style.overflow = 'visible';
    }
}
window.addEventListener('resize', adjustZoom);
window.addEventListener('load', adjustZoom);
