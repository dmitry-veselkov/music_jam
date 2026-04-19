import {get404} from "../services/RouteServices.js";

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
        let dataForView = {};
        const path = this._updateRoomPath(window.location.pathname, dataForView);

        const historyState = window.history.state || {};
        const combinedData = {...historyState, ...dataForView};

        const View = this.routes[path];
        if (View) {
            this._showView(View, combinedData);
        } else {
            this.container.innerHTML = get404();
        }
    }

    _updateRoomPath(path, dataForView) {
        if (path.includes('room')) {
            const [roomCode] = path.split('/').slice(-1);
            dataForView.roomCode = roomCode;
            return path.slice(0, path.length - roomCode.length - 1);
        }
        return path;
    }

    _showView(View, data) {
        if (this.currentView) {
            this.currentView.unmount();
        }

        this.currentView = new View(this.container, data);
        this.currentView.mount();
    }

    navigate(path) {
        window.history.pushState(null, '', path);
        this.handleRoute();
    }
}