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

    // Mostrar el ícono si:
    // 1. La ventana está en tamaño completo y el zoom está entre 200% y 500%.
    // 2. O si la ventana no está en tamaño completo (<= 810 píxeles) y el zoom está entre 100% y 500%.
    if (
        (windowWidth >= screenWidth && zoomLevel >= 200 && zoomLevel <= 500) || // Ventana completa y zoom alto
        (windowWidth <= 810 && zoomLevel >= 100 && zoomLevel <= 500) // Ventana no completa y zoom medio-alto
        || (zoomLevel < 67 || zoomLevel > 175)

    ) {
        iconElement.style.display = 'block';
    } else {
        iconElement.style.display = 'none';
    }
}
window.addEventListener('load', adjustNavbarVisibility);
window.addEventListener('resize', adjustNavbarVisibility);

function adjustZoom() {
    const zoomLevel = Math.round(window.devicePixelRatio * 100);
    const content = document.getElementById('container-wrap'); // Selecciona el contenedor del contenido principal incluyendo el navbar y footer
    const footer = document.getElementById('footer'); // Selecciona el footer

    if (zoomLevel === 50) {
        const scale = 67 / zoomLevel; // Escala inversa para simular 67%
        content.style.transform = `scale(${scale})`; // Aplica el escalado al contenido principal
        content.style.transformOrigin = 'top center'; // Ajusta el punto de origen del escalado
        content.style.width = '74.9%'; // Asegúrate de que el ancho sea del 100%
        content.style.marginLeft = '12.7%'; // Asegúrate de que el ancho sea del 100%
        const adjustedMarginBottom = `${footer.offsetHeight / scale}px`; // Calcula el margen inferior dinámico basado en el footer
        content.style.marginBottom = adjustedMarginBottom; // Ajusta el margen inferior para que el footer esté visible
        content.style.overflow = 'hidden'; // Oculta cualquier contenido fuera de los límites
    } else if (zoomLevel === 33) {
        const scale = 67 / zoomLevel;
        content.style.transform = `scale(${scale})`;
        content.style.transformOrigin = 'top center';
        content.style.width = '49.5%'; // Asegúrate de que el ancho sea del 100%
        content.style.marginLeft = '25.5%'; // Asegúrate de que el ancho sea del 100%
        const adjustedMarginBottom = `${footer.offsetHeight / scale}px`; // Calcula el margen inferior dinámico basado en el footer
        content.style.marginBottom = adjustedMarginBottom; // Ajusta el margen inferior para que el footer esté visible
        content.style.overflow = 'hidden'; // Oculta cualquier contenido fuera de los límites
    } else if (zoomLevel === 25) {
        const scale = 67 / zoomLevel;
        content.style.transform = `scale(${scale})`;
        content.style.transformOrigin = 'top center';
        content.style.width = '38%'; // Asegúrate de que el ancho sea del 100%
        content.style.marginLeft = '31.8%'; // Asegúrate de que el ancho sea del 100%
        const adjustedMarginBottom = `${footer.offsetHeight / scale}px`; // Calcula el margen inferior dinámico basado en el footer
        content.style.marginBottom = adjustedMarginBottom; // Ajusta el margen inferior para que el footer esté visible
        content.style.overflow = 'hidden'; // Oculta cualquier contenido fuera de los límites
    } else {
        content.style.transform = ''; // Restablece el escalado al valor original
        content.style.transformOrigin = ''; // Ajusta el punto de origen del escalado
        content.style.width = '100%'; // Asegúrate de que el ancho sea del 100%
        content.style.marginLeft = ''; // Asegúrate de que el ancho sea del 100%
        content.style.marginBottom = ''; // Restablece el margen inferior al valor predeterminado
        content.style.overflow = 'hidden'; // Oculta cualquier contenido fuera de los límites
    }
}
window.addEventListener('resize', adjustZoom);
window.addEventListener('load', adjustZoom);
