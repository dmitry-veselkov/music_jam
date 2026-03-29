import {Component} from "../core/Component.js";
import {Button, Header, Footer} from "../components/UI.js";

export class StartView extends Component {
    // TODO Доделать header: кнопки должы перекидывать на соответсвующие страницы
    // TODO После входа или регистрации в углу должен отображаться пользователь (логин, ава?)
    // TODO Переписать под нас все тексты на странице
    render() {
        return `
            <div class="start-page page-layout">
                <div class="header-top">
                   ${Header()}
                </div>
                
                <main class="hero-section main-content">
                    <h1 class="hero-title">Угадывай мелодии быстрее всех</h1>
                    <p class="hero-description">
                        Музыкальный свояк в реальном времени. Создавай приватные комнаты, 
                        зови друзей и соревнуйся в знании любимых треков.
                    </p>
                    
                    <div class="action-buttons">
                        ${Button({
            text: 'Подключиться к игре',
            id: 'join-game-btn',
            variant: 'primary',
            extraClass: 'btn-large'
        })}
                    </div>
                </main>

                ${Footer()}
            </div>
        `;
    }

    mount() {
        this.container.innerHTML = this.render();

        const joinBtn = this.container.querySelector('#join-game-btn');
        if (joinBtn) {
            joinBtn.addEventListener('click', () => {
                window.history.pushState({}, '', '/lobby');
                window.dispatchEvent(new Event('popstate'));
            });
        }
    }
}