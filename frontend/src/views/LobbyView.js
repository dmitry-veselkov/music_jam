import {Component} from "../core/Component.js";
import {Button, Input, Logo} from "../components/UI.js";
import {checkRoomInfo} from "../services/RoomService.js";


export class LobbyView extends Component {
    render() {
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
                        ${Input({id: 'room-code', placeholder: 'Код (например, X7B9)', maxLength: 6, type: 'text'})}
                        ${Button({text: 'Подключиться', id: 'join-btn', variant: 'primary', extraClass: 'btn-full'})}
                    </div>

                    <a href="/" class="back-link">← Назад</a>
                </main>
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