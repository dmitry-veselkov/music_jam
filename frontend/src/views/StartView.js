import {Component} from "../core/Component.js";
import {Button, Header, Footer} from "../components/UI.js";

export class StartView extends Component {
    // TODO ЛИЗА Доделать header: кнопки должы перекидывать на соответсвующие страницы
    // TODO ЛИЗА После входа или регистрации в углу должен отображаться пользователь (логин, ава?)
    render() {
        const buttonSettings = {
            text: 'Подключиться к игре',
            id: 'join-game-btn',
            variant: 'primary',
            extraClass: 'btn-large'
        }

        return `
            <div class="start-page page-layout">
                <div class="header-top">
                   ${Header()}
                </div>
                
                <main class="hero-section main-content">
                    <h1 class="hero-title">Музыкальный свояк</h1>
                    <p class="hero-description">
                        Соревнуйся в знании любимых треков вместе с друзьями!
                    </p>
                    
                    <div class="action-buttons">
                        ${Button(buttonSettings)}
                    </div>
                </main>

                ${Footer()}
            </div>
        `;
    }

    mount() {
        this.container.innerHTML = this.render();
        const registerBtn = this.container.querySelector('#register-btn');
        const loginBtn = this.container.querySelector('#login-btn');
        const joinBtn = this.container.querySelector('#join-game-btn');

        if (joinBtn) {
            joinBtn.addEventListener('click', () => {
                window.history.pushState({}, '', '/lobby');
                window.dispatchEvent(new Event('popstate'));
            });
        }

        if (registerBtn) {
            registerBtn.addEventListener('click', () => {
                alert(1);
                window.history.pushState({}, '', '/register');
                window.dispatchEvent(new Event('popstate'));
            });
        }

        if (loginBtn) {
            loginBtn.addEventListener('click', () => {
                alert(1);
                window.history.pushState({}, '', '/login');
                window.dispatchEvent(new Event('popstate'));
            });
        }
    }
}