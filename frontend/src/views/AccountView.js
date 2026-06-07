import {Component} from "../core/Component.js";
import {Logo, Button} from "../components/UI.js";
import {checkAuthorizedOrRedirect, exit} from "../services/AccountServices.js";
import {generateEmptyGame, getAllUserGames, deleteGame} from "../services/GamesServices.js";
import {ButtonLoader} from "../components/ButtonLoader.js";
import {GamesList} from "../components/GamesList.js";
import {redirectTo} from "../services/RouteServices.js";
import {ConfirmModal} from "../components/ConfirmModal.js";

export class AccountView extends Component {
    constructor(container, data) {
        super(container, data);
        this.state = {
            userName: "",
            games: [],
        };
    }

    async mount() {
        const userInfo = await checkAuthorizedOrRedirect();
        if (userInfo) {
            await this._setState(userInfo);
            this.updateDOM();
        }
    }

    updateDOM() {
        this.container.innerHTML = this.render();
        this._addEventListeners();
    }

    async _setState(userInfo) {
        this.state.userName = userInfo.name;
        this.state.games = await getAllUserGames();
        sessionStorage.setItem("loadedUserGames", JSON.stringify(this.state.games));
    }

    render() {
        return `
            <div class="page-layout" style="overflow-y: auto; height: 100vh;">
                <div class="header-top">
                    <header class="start-header">
                        ${Logo()}
                        <button id="exit-btn" class="btn btn-outline btn-sm">Выйти</button>
                    </header>
                </div>
                
                <main class="dashboard-container">
                    <div class="dashboard-header mt-3">
                        <h1>Добро пожаловать, ${this.state.userName}!</h1>
                        <div class="mt-3">${Button(this._createNewGameButtonSetts)}</div>
                    </div>

                    ${GamesList(this.state.games)}
                </main>
                
                ${ConfirmModal(this.state.showDeleteConfirm, 'Удалить игру?', 'Удалить')}
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
                    const gameId = event.currentTarget.dataset.gameCode;
                    redirectTo(`/room/admin_waiting/${gameId}`);
                });
            });
        }

        const deleteGameButtons = document.querySelectorAll("[data-delete-id]");
        if (deleteGameButtons) {
            deleteGameButtons.forEach(button => {
                button.addEventListener("click", async (event) => {
                    this.state.showDeleteConfirm = true;
                    this.state.deleteTarget = event.currentTarget;
                    this.updateDOM()
                });
            });
        }

        const confirmDeleteBtn = this.container.querySelector('#confirm-end-btn');
        if (confirmDeleteBtn) {
            confirmDeleteBtn.addEventListener('click', async () => {
                await this._deleteGame();

                const gameId = this.state.deleteTarget.dataset.gameCode;
                const index = this.state.games.findIndex(g => g.join_code === gameId);
                if (index !== -1) {
                    this.state.games.splice(index, 1);
                }

                this.state.deleteTarget = null;
                this.state.showDeleteConfirm = false;
                this.updateDOM();
            });
        }

        const cancelEndBtn = this.container.querySelector('#cancel-end-btn');
        if (cancelEndBtn) {
            cancelEndBtn.addEventListener('click', () => {
                this.state.showDeleteConfirm = false;
                this.state.deleteTarget = null;
                this.updateDOM();
            });
        }

        document
            .querySelector("#exit-btn")
            .addEventListener("click", async () => {
                await exit();
            })
    }

    async _deleteGame() {
        const target = this.state.deleteTarget;
        if (!target) {
            return;
        }

        const gameId = target.dataset.gameCode;
        try {
            await deleteGame(gameId);
            const card = target.closest(".game-card");
            if (card) {
                card.remove();
            }
        } catch (error) {
            console.error("Ошибка при удалении:", error);
            alert("Не удалось удалить игру");
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