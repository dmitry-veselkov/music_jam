import {Component} from "../core/Component.js";
import {Button, Logo} from "../components/UI.js";
import {redirectTo} from "../services/RouteServices.js";
import {OnGameRating, TopPlayer} from "../components/OnGameRating.js";

export class GameEndingViewUniversal extends Component {
    constructor(container, data) {
        super(container, data);
        this.state = {
            players: data?.players ?? {},
            myTeamName: data?.myTeamName ?? null,
        };
    }

    async mount() {
        this.updateDOM();
    }

    updateDOM() {
        this.container.innerHTML = this.render();
        this.attachEvents();
    }

    render() {
        return `
            <div class="logo-corner">${Logo()}</div>
            <div style="max-width:520px;margin:0 auto;padding:100px 1.5rem 2rem;">
                  ${TopPlayer(this.state.players)}
                  ${OnGameRating(this.state.players, this.state.myTeamName)}
                  <div style="text-align:center;margin-top:1.5rem;">
                    ${Button({text: 'Выйти в меню', id: 'close-btn'})}
                  </div>
            </div>
        `;
    }

    attachEvents() {
        const closeBtn = this.container.querySelector('#close-btn');
        if (closeBtn) {
            closeBtn.onclick = () => {
                redirectTo("/");
            }
        }
    }
}
