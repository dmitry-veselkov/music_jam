import {Component} from "../core/Component.js";
import {Logo, Button} from "../components/UI.js";
import {loadUserInfoOrRedirect} from "../services/AccountServices.js";
import {generateEmptyGame, getAllUserGames} from "../services/GamesServices.js";
import {ButtonLoader} from "../components/ButtonLoader.js";
import {GamesList} from "../components/GamesList.js";
import {redirectTo} from "../services/RouteServices.js";

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
            this.container.innerHtml = this.render(userInfo);
            this._addEventListeners();
        }
    }

    async _setState(userInfo) {
        this.state.userName = userInfo.name;
        this.state.games = await getAllUserGames();
        sessionStorage.setItem("loadedUserGames", JSON.stringify(this.state.games));
    }

    render(userInfo) {
        return `
            <div class="page-layout" style="overflow-y: auto; height: 100vh;">
                <div class="header-top">${Logo()}</div>
                
                <main class="dashboard-container">
                    <div class="dashboard-header mt-3">
                        <h1>Добро пожаловать, ${this.state.userName}!</h1>
                        <div class="mt-3">${Button(this._createNewGameButtonSetts)}</div>
                    </div>

                    ${GamesList(this.state.games)}                   
                </main>
            </div>
        `;
    }

    _addEventListeners() {
        const createNewGameButton = document.querySelector("#create-game-btn");

        if (createNewGameButton) {
            createNewGameButton.addEventListener("click", async () => {
                await ButtonLoader.wrap(createNewGameButton, async () => {
                    await this._createNewGame();
                });
            });
        }

        const editGameButtons = document.querySelectorAll("[data-edit-id]");
        if (editGameButtons) {
            editGameButtons.forEach(button => {
                button.addEventListener("click", (event) => {
                    const gameId = event.currentTarget.dataset.gameCode;
                    redirectTo(`/room/game_settings/${gameId}`);
                });
            });
        }

        const startButtons = document.querySelectorAll("[data-start-id]");
        if (startButtons) {
            startButtons.forEach(button => {
                button.addEventListener("click", async (event) => {
                    sessionStorage.removeItem('teams');
                    const gameId = event.currentTarget.dataset.gameCode;
                    redirectTo(`/room/admin_waiting/${gameId}`);
                });
            });
        }
    }

    async _createNewGame() {
        /**
         * Запрос на бэк. Если успешно код успешно вернулся, значит, что комната создалась
         */
        const code = await generateEmptyGame();
        if (code) {
            redirectTo(`/room/game_settings/${code}`, {isNew: true});
        }
    }

    _createNewGameButtonSetts = {
        id: "create-game-btn",
        text: "+ Создать новую игру",
        variant: "primary"
    }
}