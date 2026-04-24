import {Component} from "../core/Component.js";
import {Button, Input, Logo} from "../components/UI.js";
import {tryGetRoomInfo} from "../services/RoomService.js";
import {redirectTo} from "../services/RouteServices.js";
import {WrongText} from "../components/Error.js";
import {ButtonLoader} from "../components/ButtonLoader.js";


export class EnterInviteCodeView extends Component {
    mount() {
        /**
         * Отображаем поле для ввода пригласительного кода
         */
        this.container.innerHTML = this.render();

        this.joinButton = this.container.querySelector('#join-btn');
        this.codeInput = this.container.querySelector('#room-code');

        this.wrongText = new WrongText(this.container);
        this.wrongText.hideWrongText();

        this._setEventListeners();
        this.codeInput.focus();
    }

    _setEventListeners() {
        /**
         * Обрабатываем нажатия. Кнопка Подключиться и Enter в поле ввода
         * выполняются одинаковый функционал: запускают обработку введенного кода
         */
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
        /**
         * Обработка введенного кода. Если он валидный и по нему нашлась
         * соответствующая комната, пользователя перекинет на нее
         */
        this.wrongText.hideWrongText();
        const code = this.codeInput.value.trim().toUpperCase();
        if (code.length !== 6) {
            this.wrongText.showWrongText('Корректный код состоит из 6 символов!');
            return;
        }

        let data;
        await ButtonLoader.wrap(this.joinButton, async () => {
            data = await tryGetRoomInfo(code);
        })

        if (data && data.status === 'waiting') {
            redirectTo(`/room/waiting/${code}`, {roomCode: code})
        } else {
            this.wrongText.showWrongText('Комната не найдена!');
        }
    }

    render() {
        return `
            <div class="lobby-page">
                ${Logo('logo-corner')}
                
                <main class="lobby-container">
                    <h2 class="lobby-title">Вход в комнату</h2>
                    <p class="lobby-description">Введите короткий код, чтобы присоединиться к свояку.</p>
                    
                    <div class="join-form">
                        ${Input(this._inputSettings)}
                        ${Button(this._buttonSettings)}
                    </div>
                    
                    ${WrongText.html}
                    
                    <a href="/" class="back-link">← Назад</a>
                </main>
            </div>
        `;
    }

    _inputSettings = {
        id: 'room-code',
        placeholder: 'Код (например, X74FB9)',
        maxLength: 6,
        type: 'text'
    }

    _buttonSettings = {
        text: 'Подключиться',
        id: 'join-btn',
        variant: 'primary',
        extraClass: 'btn-full'
    }
}