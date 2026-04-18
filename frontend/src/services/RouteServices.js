export function redirectTo(path, data = {}) {
    /**
     * Перенаправить на страницу path
     */
    window.history.pushState(data, '', path);
    window.dispatchEvent(new Event('popstate'));
    console.log('redirect to', path);
}

export function get404() {
    /**
     * Струтура страницы 404, которая будет отображаться,
     * когда страница была не найдена
     */
    return `
            <div class="page-layout">
                <main class="main-content">
                    <h1>404</h1>
                    <p>Страница не найдена</p>
                    <a href="/" class="btn btn-primary">На главную</a>
                </main>
            </div>
        `;
}