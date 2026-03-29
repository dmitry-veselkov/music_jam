export class Component {
    constructor(container, data) {
        this.container = container;
        this.data = data;
    }

    render() {
        return '';
    }

    mount() {
        this.container.innerHTML = this.render();
    }

    unmount() {
        this.container.innerHTML = '';
    }
}