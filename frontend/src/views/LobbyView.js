import {Component} from "../core/Component.js";
import {Button, Footer, Header, Input, Logo} from "../components/UI.js";
import {checkRoomInfo} from "../services/RoomService.js";


export class LobbyView extends Component {
    render() {
            return `
            <div class="lobby-page">
                <div class="header-top">
                   ${Header({})}
                </div>
                
                <main class="lobby-container">
                    <label for="room-code"><span class="participate-block">Введите код комнаты</span></label>
                    <div>
                        ${Input({id: 'room-code', placeholder: 'Ваш код...', maxLength: 6, type: 'text'})}
                        ${Button({text: 'Присоединиться', id: 'join-btn', variant: 'code', textClass: 'participate-text'})}
                    </div>

                    <a href="/" class="back-link"><span class="participate-text">← Назад</span></a>
                </main>

                ${Footer()}
            </div>
        `;
    }

    mount() {
        this.container.innerHTML = this.render();

        const joinBtn = this.container.querySelector('#join-btn');
        const codeInput = this.container.querySelector('#room-code');

        const handleJoin = async () => {
            const code = codeInput.value.trim().toUpperCase();

            if (code.length < 3) {
                alert("Код комнаты слишком короткий!");
                return;
            }

            const data = await checkRoomInfo(code);
            if (!data['exists']) {
                alert("Комната не найдена!")
            } else if (data['status'] === 'waiting') {
                window.history.pushState({ roomCode: code }, '', `/room/waiting/${code}`);
                window.dispatchEvent(new Event('popstate'));
            } else {
                alert("Рановато зашел ты, приятель...")
            }
        };

        joinBtn.addEventListener('click', handleJoin);
        codeInput.addEventListener('keypress', async (e) => {
            if (e.key === 'Enter') {
                await handleJoin();
            }
        });

        codeInput.addEventListener('input', (e) => {
            e.target.value = e.target.value.toUpperCase();
        });

        codeInput.focus();
    }
}