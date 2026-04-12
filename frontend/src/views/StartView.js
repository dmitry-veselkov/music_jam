import {Component} from "../core/Component.js";
import {Button, Header, Footer} from "../components/UI.js";
import {tryGetUserInfo} from "../services/AccountServices.js";

export class StartView extends Component {
    async mount() {
        /**
         * Получаем информацию о пользователе и отображаем главную страницу.
         * Если пользователь авторизован, то в правом верхнем углу будет ссылка на личный кабинет.
         * Иначе будут кнопки Входа/Регистрации
         */
        window.currentUser = await tryGetUserInfo();
        this.container.innerHTML = this.render();
        this._addEventListeners();
    }

    render() {
        return `
            <div class="start-page page-layout">
                <div class="header-top">
                   ${Header(window.currentUser !== null)}
                </div>
                
                <main class="hero-section main-content">
                    <h1 class="hero-title">Музыкальный свояк</h1>
                    <p class="hero-description">
                        Соревнуйся в знании любимых треков вместе с друзьями!
                    </p>
                    
                    <div class="action-buttons">
                        ${Button(this._connectButtonSettings)}
                    </div>
                </main>
                ${Footer()}
            </div>
        `;
    }

    _addEventListeners() {
        const buttonRoutes = {
            '/lobby': this.container.querySelector('#join-game-btn'),
            '/login': this.container.querySelector('#login-btn'),
            '/register': this.container.querySelector('#register-btn'),
        };

        for (const [route, obj] of Object.entries(buttonRoutes)) {
            if (obj) {
                obj.addEventListener('click', () => {
                    window.history.pushState({}, '', route);
                    window.dispatchEvent(new Event('popstate'));
                });
            }
        }
    }

    _connectButtonSettings = {
        text: 'Подключиться к игре',
        id: 'join-game-btn',
        variant: 'primary',
        extraClass: 'btn-large'
    }
}