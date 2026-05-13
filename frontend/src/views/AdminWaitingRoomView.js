import {Component} from "../core/Component.js";
import {get404, redirectTo} from "../services/RouteServices.js";

import {Logo, Button} from "../components/UI.js";
import {authenticationToGame} from "../services/AccountServices.js";
import {startGame, trySetWaitingStatus} from "../services/GamesServices.js";
import {removeTeam, tryGetRoomInfo} from "../services/RoomService.js";

export class AdminWaitingRoomView extends Component {
    constructor(container, data) {
        super(container, data);

        this.state = {
            roomCode: null,
            gameName: '',
            creator: '',
            teams: []
        };
    }

    async mount() {
        // TODO: тут теперь дублируются запросы на получение информации об игре. Нужно исправить
        const authData = await authenticationToGame(this.container, this.data.roomCode);
        if (!authData) {
            return;
        }

        await trySetWaitingStatus(this.data.roomCode);
        const roomInfo = await tryGetRoomInfo(this.data.roomCode);
        console.log(roomInfo);

        this._connectSocket();

        this.state = {
            roomCode: roomInfo.code,
            gameName: roomInfo.title || 'Без названия',
            creator: roomInfo.author || 'неизвестный...',
            teams: roomInfo.teams || [],
        }

        this.updateDOM();
    }

    updateDOM() {
        this.container.innerHTML = this.render();
        this.attachEvents();
    }

    _renderTeamsPanel() {
        const teamsListLogic = this.state.teams.length === 0
            ? `<li class="empty-state">Пока никого нет. Поделитесь кодом, чтобы игроки присоединились!</li>`
            : this.state.teams
                .map(team => `
                    <li class="team-item">
                        <span class="team-icon">👥</span>
                        <span class="team-name">${team}</span>
                        <button class="remove-team" data-name="${team}" aria-label="Выгнать">×</button>
                    </li>`)
                .join('')

        const startButton = Button({text: 'Начать игру', id: 'start-game', variant: 'primary'});
        return `<main class="waiting-teams-panel">
                    <div class="card teams-card">
                        <div class="teams-header">
                            <h2>Уже в лобби 
                                <span class="team-count">
                                    (${this.state.teams.length})
                                </span>
                            </h2>
                            <div class="pulse-indicator">Ожидание...</div>
                        </div>
                        <div class="room-code-panel">
                            <span>Код комнаты</span>
                                <div class="room-code-row">
                                    <span>${this.state.roomCode}</span>
                                    <button class="copy-btn" id="copy-code-btn" title="Скопировать">📋</button>
                                </div>
                        </div>
                        <ul class="teams-list">
                            ${teamsListLogic}
                        </ul>
                        <div class="start-btn-wrapper">
                            ${startButton}
                        </div>
                    </div>
                </main>`
    }

    render() {
        return `
            <div class="logo-corner">${Logo()}</div>
            <div class="waiting-layout">
                ${this._renderTeamsPanel()}
            </div>
        `;
    }

    attachEvents() {
        const startGameBtn = document.getElementById("start-game");
        startGameBtn.addEventListener("click", async (event) => {
            const gameId = this.data.roomCode;
            await startGame(gameId);
            redirectTo(`/room/admin_active/${gameId}`);
        });

        const copyBtn = document.getElementById("copy-code-btn");
        if (copyBtn) {
            copyBtn.addEventListener("click", () => {
                navigator.clipboard.writeText(this.state.roomCode);
                copyBtn.textContent = '✅';
                setTimeout(() => copyBtn.textContent = '📋', 2000);
            });
        }

        document
            .querySelector('.teams-list')
            .addEventListener('click', async (e) => {
                if (e.target.classList.contains('remove-team')) {
                    const teamName = event.target.dataset.name;
                    await removeTeam(teamName, this.state.roomCode);
                }
            });
    }

    _connectSocket() {
        if (this.ws) return;
        const protocol = window.location.protocol === "https:" ? "wss" : "ws";
        this.ws = new WebSocket(`${protocol}://${window.location.host}/api/ws/room/${this.data.roomCode}`);

        this.ws.onmessage = (event) => {
            const data = JSON.parse(event.data);

            if (data.type === "init" || data.type === "update") {
                this.state.teams = data.teams;
                this.updateDOM();
            }
        };
    }
}