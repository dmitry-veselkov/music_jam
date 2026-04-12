export function redirectTo(path) {
    window.history.pushState({}, '', path);
    window.dispatchEvent(new Event('popstate'));
    console.log('redirect to', path);
}