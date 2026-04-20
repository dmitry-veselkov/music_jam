import {Component} from "../core/Component.js";
import {tryGetRoomInfo} from "../services/RoomService.js";
import {get404} from "../services/RouteServices.js";

import {Logo, Input, Button} from "../components/UI.js";
import {loadUserInfoOrRedirect} from "../services/AccountServices.js";
import {tryRunGame} from "../services/GamesServices.js";

export class AdminWaitingRoomView extends Component {
    constructor(container, data) {
        super(container, data);
        this._savedName = '';

        this.state = {
            gameName: 'Загрузка...',
            creator: 'Загрузка...',
            teams: []
        };
    }

    async mount() {
        const userInfo = await loadUserInfoOrRedirect();
        if (!userInfo) {
            return;
        }

        const roomInfo = await tryRunGame(this.data.roomCode);
        if (!roomInfo) {
            this.container.innerHTML = get404();
            return;
        }

        this._connectSocket();

        this._setState({
            gameId: roomInfo.id,
            roomCode: roomInfo.code,
            gameName: roomInfo.title || 'Без названия',
            creator: roomInfo.author || 'неизвестный...',
            teams: roomInfo.teams || []
        })
    }

    _setState(newState) {
        this.state = {...this.state, ...newState};
        this._savedName = this.state.myTeamName;
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
                    </li>`)
                .join('')

        const startButton = Button({ text: 'Начать игру', id: 'btn-start-game', variant: 'primary'});
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
                        
                        <ul class="teams-list">
                            ${teamsListLogic}
                        </ul>
                        ${startButton}
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
    }

    _connectSocket() {
        const protocol = window.location.protocol === "https:" ? "wss" : "ws";
        this.ws = new WebSocket(
            `${protocol}://${window.location.host}/api/ws/room/${this.data.roomCode}`
        );

        this.ws.onmessage = (event) => {
            const data = JSON.parse(event.data);

            if (data.type === "init" || data.type === "update") {
                this.state.teams = data.teams;
                this.updateDOM();
            }
        };
    }
}