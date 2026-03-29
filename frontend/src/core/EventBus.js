export class EventBus {
    constructor() {
        this.listeners = {};
    }

    subscribe(event, callback) {
        if (!this.listeners[event]) {
            this.listeners[event] = [];
        }
        this.listeners[event].push(callback);
    }

    unsubscribe(event, callback) {
        this.listeners[event] = this.listeners[event].filter(x => x !== callback);
    }

    emit(event, data) {
        if (this.listeners[event]) {
            this.listeners[event].forEach(callback => callback(data));
        }
    }
}