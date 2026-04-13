import {Component} from "../core/Component.js";
import {Logo, Button} from "../components/UI.js";
import {loadUserInfoOrRedirect} from "../services/AccountServices.js";
import {getAllUserGames} from "../services/GamesServices.js";

export class AccountView extends Component {
    constructor(container, data) {
        super(container, data);
        this.state = {
            userName: "",
            games: [],
        };
    }

    async mount() {
        const userInfo = await loadUserInfoOrRedirect();
        if (userInfo) {
            await this._setState(userInfo);
            this.container.innerHTML = this.render(userInfo);
            this._addEventListeners();
        }
    }

    async _setState(userInfo) {
        this.state.userName = userInfo.name;
        this.state.games = await getAllUserGames(userInfo.email);
        console.log(this.state.games);
    }

    _renderGamesList() {
        const gamesList = this.state.games
            .map(game => `
                <div class="card game-card">
                    <h3>${game.title}</h3>
                    <p class="text-muted">Создана: ${game.date}</p>
                    <p class="room-code">Код лобби: <strong>${game.code}</strong></p>
                    <div class="game-actions mt-3">
                        <button 
                            class="btn btn-outline btn-sm" 
                            data-edit-id="${game.id}"
                            data-game-code="${game.code}">
                        Редактировать
                        </button>
                        <button 
                            class="btn btn-primary btn-sm" 
                            data-start-id="${game.id}"
                            data-game-code="${game.code}"
                            >Запустить</button>
                    </div>
                </div>
            `)
            .join('');

        const panel = this.state.games.length === 0
            ? `<p class="empty-state">У вас пока нет созданных игр.</p>`
            : `<div class="games-grid">${gamesList}</div>`

        return `<div class="games-list-section mt-3">
                    <h2>Ваши игры</h2>
                    ${panel}
                </div>`
    }

    render(userInfo) {
        const createNewGameButtonSetts = {
            id: "create-game-btn",
            text: "+ Создать новую игру",
            variant: "primary"
        }

        return `
            <div class="page-layout">
                <div class="header-top">${Logo()}</div>
                
                <main class="dashboard-container">
                    <div class="dashboard-header mt-3">
                        <h1>Добро пожаловать, ${this.state.userName}!</h1>
                        <div class="mt-3">${Button(createNewGameButtonSetts)}</div>
                    </div>

                    ${this._renderGamesList()}                    
                </main>
            </div>
        `;
    }

    _addEventListeners() {
        const createNewGameButton = document.querySelector("#create-game-btn");

        if (createNewGameButton) {
            createNewGameButton.addEventListener("click", () => {
                window.history.pushState({}, '', '/room/game_settings/12345');
                window.dispatchEvent(new Event('popstate'));
            })
        }

        const editGameButtons = document.querySelectorAll("[data-edit-id]");
        if (editGameButtons) {
            editGameButtons.forEach(button => {
                button.addEventListener("click", (event) => {
                    const gameId = event.currentTarget.dataset.gameCode;
                    window.history.pushState({}, '', `/room/game_settings/${gameId}`);
                    window.dispatchEvent(new Event('popstate'));
                });
            });
        }

        const startButtons = this.container.querySelectorAll("[data-start-id]");
        if (startButtons) {
            startButtons.forEach(button => {
                button.addEventListener("click", (event) => {
                    const gameId = event.currentTarget.dataset.gameCode;
                    window.history.pushState({}, '', `/room/active/${gameId}`);
                    window.dispatchEvent(new Event('popstate'));
                });
            });
        }
    }
}