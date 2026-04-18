import {Component} from "../core/Component.js";
import {Button, Input, Logo} from "../components/UI.js";
import {tryGetRoomInfo} from "../services/RoomService.js";


export class LobbyView extends Component {
    mount() {
        this.container.innerHTML = this.render();

        this.joinButton = this.container.querySelector('#join-btn');
        this.codeInput = this.container.querySelector('#room-code');

        this._setEventListeners();
        this.codeInput.focus();
    }

    _setEventListeners() {
        this.joinButton.addEventListener('click', () => this._handleJoin());
        this.codeInput.addEventListener('keypress', async (e) => {
            if (e.key === 'Enter') {
                await this._handleJoin();
            }
        });

        this.codeInput.addEventListener('input', (e) => {
            e.target.value = e.target.value.toUpperCase();
        });
    }

    async _handleJoin() {
        const code = this.codeInput.value.trim().toUpperCase();

        if (code.length < 3) {
            // TODO ДИМА заменить alert на сообщение красным текстом под полем ввода
            alert("Код комнаты слишком короткий!");
            return;
        }

        const data = await tryGetRoomInfo(code);
        if (data && data.status === 'waiting') {
            window.history.pushState({roomCode: code}, '', `/room/waiting/${code}`);
            window.dispatchEvent(new Event('popstate'));
        } else {
            // TODO ДИМА заменить alert на сообщение красным текстом под полем ввода
            alert("Комната не найдена!");
        }
    }

    render() {
        const inputSettings = {
            id: 'room-code',
            placeholder: 'Код (например, X7B9)',
            maxLength: 6,
            type: 'text'
        }

        const buttonSettings = {
            text: 'Подключиться',
            id: 'join-btn',
            variant: 'primary',
            extraClass: 'btn-full'
        }

        return `
            <div class="lobby-page">
                ${Logo('logo-corner')}
                
                <main class="lobby-container">
                    <h2 class="lobby-title">Вход в комнату</h2>
                    <p class="lobby-description">
                        Введите короткий код, который вам дал создатель игры, 
                        чтобы присоединиться к музыкальному свояку.
                    </p>
                    
                    <div class="join-form">
                        ${Input(inputSettings)}
                        ${Button(buttonSettings)}
                    </div>

                    <a href="/" class="back-link">← Назад</a>
                </main>
            </div>
        `;
    }
}