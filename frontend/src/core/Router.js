import {get404} from "../services/RoomService.js";

export class Router {
    constructor(routes, container) {
        this.routes = routes;
        this.container = container;
        this.currentView = null;

        window.addEventListener('popstate', () => this.handleRoute());

        document.addEventListener('click', (e) => {
            const link = e.target.closest('a');
            if (link && link.getAttribute('href').startsWith('/')) {
                e.preventDefault();
                this.navigate(link.getAttribute('href'));
            }
        });

        this.handleRoute();
    }

    handleRoute() {
        const fullpath = window.location.pathname;
        let path = fullpath;
        let data = {};

        if (path.includes('room')) {
            const [roomCode] = path.split('/').slice(-1);
            path = path.slice(0, path.length - roomCode.length - 1);
            data.roomCode = roomCode;
        }

        const historyState = window.history.state || {};
        const combinedData = {...historyState, ...data};
        const View = this.routes[path];

        if (View) {
            if (this.currentView) {
                this.currentView.unmount();
            }

            this.currentView = new View(this.container, combinedData);
            console.log(this.currentView);
            this.currentView.mount();
        } else {
            this.container.innerHTML = get404();
        }
    }

    navigate(path) {
        window.history.pushState(null, '', path);
        this.handleRoute();
    }
}