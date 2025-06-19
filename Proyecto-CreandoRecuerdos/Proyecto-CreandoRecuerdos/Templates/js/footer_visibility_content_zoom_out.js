function adjustZoom() {
    const zoomLevel = Math.round(window.devicePixelRatio * 100);
    const scale = 67 / zoomLevel;
    const inverseScale = 1 / scale;
    const content = document.getElementById('scaled-wrapper');
    const navbar = document.getElementById('navbar');

    if (zoomLevel === 25 || zoomLevel === 33 || zoomLevel === 50) {
        // Escala todo el contenido
        if (content) {
            content.style.transform = `scale(${scale})`;
            content.style.transformOrigin = 'top center';
            content.style.overflow = 'visible';
            content.style.width = `${100 / scale}%`;
            content.style.margin = '0 auto';
            content.style.overflow = 'visible';
        }
        // Escala inversa para navbar y footer
        [navbar].forEach(el => {
            if (el) {
                el.style.transform = `scale(${inverseScale})`;
                el.style.transformOrigin = 'top center';
            }
        });
    } else {
        if (content) {
            content.style.transform = '';
            content.style.transformOrigin = '';
            content.style.overflow = '';
            content.style.width = '';
            content.style.margin = '';
            content.style.overflow = '';
            content.style.height = '';

        }
        [navbar].forEach(el => {
            if (el) {
                el.style.transform = '';
                el.style.transformOrigin = '';
            }
        });
    }
}
window.addEventListener('resize', adjustZoom);
window.addEventListener('load', adjustZoom);

function adjustNavbarZoom() {
    const zoomLevel = Math.round(window.devicePixelRatio * 100);
    const navbar = document.getElementById('navbar');
    
    if (!navbar) return;
    if (zoomLevel === 50 || zoomLevel === 33 || zoomLevel === 25) {
        const scale = 67 / zoomLevel;

        // Escalar navbar
        navbar.style.transform = `scale(${scale})`;
        navbar.style.transformOrigin = 'top center';

        navbar.style.overflow = 'visible';
    } else {
        // Reset estilos
        navbar.style.transform = '';
        navbar.style.transformOrigin = '';
        navbar.style.width = '';
        navbar.style.margin = '';
        navbar.style.overflow = 'visible';
    }
}
window.addEventListener('resize', adjustNavbarZoom);
window.addEventListener('load', adjustNavbarZoom);

function adjustFooterSpacing() {
    const zoomLevel = Math.round(window.devicePixelRatio * 100);
    const windowWidth = window.innerWidth;
    const screenWidth = screen.width;
    const content = document.getElementById('scaled-wrapper');
    const footer = document.getElementById('footer');

    if (!content || !footer) return;

    let scale = null;

    // Escalas personalizadas por zoom
    if (zoomLevel === 50) {
        scale = 67 / 50;
    } else if (zoomLevel === 33) {
        scale = 11 / 33.3;
    } else if (zoomLevel === 25) {
        scale = 5 / 25.5;
    }

    if (scale && windowWidth >= 810 && windowWidth >= screenWidth) {
        requestAnimationFrame(() => {
            const adjustedMarginBottom = footer.offsetHeight / scale;
            content.style.marginBottom = `${adjustedMarginBottom}px`;
        });
    } else {
        content.style.marginBottom = '';
    }
}
window.addEventListener('resize', adjustFooterSpacing);
window.addEventListener('load', adjustFooterSpacing);