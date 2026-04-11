import { Component } from "../core/Component.js";
import {Header, Footer, Button} from "../components/UI";


export class LoginView extends Component {
    // TODO ЛИЗА Доделать header: кнопки должы перекидывать на соответсвующие страницы
    // TODO ЛИЗА После входа или регистрации в углу должен отображаться пользователь (логин, ава?)
    render() {
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