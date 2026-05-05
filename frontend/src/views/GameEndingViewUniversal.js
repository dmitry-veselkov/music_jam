import {Component} from "../core/Component.js";
import {Button, Logo} from "../components/UI.js";
import {get404, redirectTo} from "../services/RouteServices.js";
import {OnGameRating} from "../components/OnGameRating.js";

export class GameEndingViewUniversal extends Component {
    constructor(container, data) {
        super(container, data);
    }

    async mount() {
        this.updateDOM()
    }

    updateDOM() {
        this.container.innerHTML = this.render();
        this.attachEvents();
    }

    render() {
        return `
            <div class="logo-corner">${Logo()}</div>
            <div class="editor-layout player-mode">
                <main class="editor-main">
                    ${OnGameRating(this.state.players, sessionStorage.getItem('team-name'))}
                    ${Button(`Выйти из игры`,
            )                   `close-btn`}}
                </main>
            </div>
        `;
    }

    attachEvents() {
            const closeBtn = this.container.querySelector('#close-btn');
            if (closeBtn) {
                closeBtn.onclick = () => {
                    sessionStorage.clear();
                    redirectTo("/");
                }
            }

        }
}