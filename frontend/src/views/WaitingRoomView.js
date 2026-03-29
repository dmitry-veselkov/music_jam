import {Component} from "../core/Component.js";
import {get404, tryEnterToRoom} from "../services/RoomService.js";
import {Logo} from "../components/UI.js";

export class WaitingRoomView extends Component {
    async mount() {
        if (await tryEnterToRoom(this.data)) {
            this.container.innerHTML = this.render();
        } else {
            this.container.innerHTML = get404();
        }
    }

    render() {
        return `
            <div class="page-layout">
                <div class="logo-corner">${Logo()}</div>
                <main class="main-content">
                    <h1>Зал ожидания: ${this.data.roomCode}</h1>
                    <p>Ожидаем начала битвы...</p>
                </main>
            </div>
        `;
    }
}