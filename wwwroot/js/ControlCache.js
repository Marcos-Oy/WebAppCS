// Control de caché y navegación profesional
(function() {
    // 1. Prevenir caché del navegador
    if (window.performance && window.performance.navigation.type === 2) {
        window.location.reload(true);
    }

    // 2. Control de navegación (evitar retroceso)
    const originalPushState = history.pushState;
    history.pushState = function(state) {
        originalPushState.apply(history, arguments);
        window.dispatchEvent(new Event('locationchange'));
    };

    window.addEventListener('popstate', function() {
        // Redirigir al inicio si intentan retroceder
        if (!document.referrer || document.referrer.indexOf(window.location.host) === -1) {
            history.pushState(null, null, window.location.href);
            window.location.href = '@Url.Action("Index", "Auth")';
        }
    });

    // 3. Forzar recarga en páginas cacheadas
    window.onpageshow = function(event) {
        if (event.persisted) {
            window.location.reload();
        }
    };
})();