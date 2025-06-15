function adjustZoom() {
    const zoomLevel = Math.round(window.devicePixelRatio * 100);
    const scale = 67 / zoomLevel;
    const elementsToScale = [
        document.getElementById('scaled-wrapper'),
        document.getElementById('navbar')
    ];

    if (zoomLevel === 25 || zoomLevel === 33 || zoomLevel === 50) {
        elementsToScale.forEach(el => {
            if (el) {
                el.style.transform = `scale(${scale})`;
                el.style.transformOrigin = 'top center';
                el.style.overflow = 'visible';
                el.style.height = 'auto';
            }

            if (window.innerHeight >= screen.height) {
                elementsToScale.forEach(el => {
                    if (el) {
                        el.style.width = 'auto';
                        el.style.height = 'auto';
                    }
                });
            }
        });
    } else {
        elementsToScale.forEach(el => {
            if (el) {
                el.style.transform = '';
                el.style.transformOrigin = '';
                el.style.overflow = 'visible';
                el.style.width = '';
                el.style.height = '';
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

        // Centrar automáticamente
        navbar.style.width = `${100 / scale}%`;  // inverso del escalado para compensar reducción
        navbar.style.margin = '0 auto'; // centrado horizontal automático

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