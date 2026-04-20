import {Component} from "../core/Component.js";
import {tryGetRoomInfo, setTeamName, getTeamNameByUUID} from "../services/RoomService.js";
import {get404} from "../services/RouteServices.js";
import {Logo, Input, Button} from "../components/UI.js";

export class WaitingRoomView extends Component {
    /**
     * Представление для комнаты ожидания. Игроки попадают в нее после ввода
     * кода комнаты или перехода по ссылке. Можно указать название для команды,
     * исправить его, подключиться к лобби и увидеть уже подключенные команды
     */
    constructor(container, data) {
        super(container, data);
        this._savedName = '';

        this.state = {
            gameName: 'Загрузка...',
            creator: 'Загрузка...',
            myTeamName: '',
            isNameSaved: false,
            teams: []
        };
    }

    // TODO ДИМА доделать ограничения на названия команд (уникальность)

    async mount() {
        // TODO делаем еще один запрос... надо фильтровать что уже в лобби нашли инфу всю
        const roomData = await tryGetRoomInfo(this.data.roomCode);
        if (!roomData || roomData.status !== 'waiting') {
            this.container.innerHTML = get404();
            return;
        }

        this._connectSocket();

        const uuid = this._setUUID();
        await this._loadTeamInfoByUUID(uuid);

        this._setState({
            gameId: roomData.id,
            roomCode: roomData.code,
            gameName: roomData.title || 'Без названия',
            creator: roomData.author || 'неизвестный...',
            teams: roomData.teams || []
        })
    }

    _setUUID() {
        const currentUUID = localStorage.getItem('team-uuid');
        if (!currentUUID) {
            localStorage.setItem('team-uuid', crypto.randomUUID());
        }
        return localStorage.getItem('team-uuid');
    }

    async _loadTeamInfoByUUID(uuid) {
        const teamName = await getTeamNameByUUID(uuid);
        if (teamName) {
            this.state.myTeamName = teamName;
            this.state.isNameSaved = true;
        }
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

    _renderWaitingInfoPanel() {
        const inputSettings = {
            id: "team-name-input",
            placeholder: "Например: Команда 1",
            maxLength: 60,
            value: this.state.myTeamName,
            disabled: this.state.isNameSaved
        }

        const buttonSettings = {
            id: "save-team-btn",
            text: this.state.isNameSaved ? "Изменить" : "Войти в игру",
            variant: this.state.isNameSaved ? "outline" : "primary"
        }

        return `<aside class="waiting-info-panel">
                    <div class="card info-card">
                        <div class="badge room-badge">Код комнаты: ${this.data.roomCode}</div>
                        <h1 class="game-title">${this.state.gameName}</h1>
                        <p class="game-author">Создатель: <strong>${this.state.creator}</strong></p>
                        
                        <div class="team-form-container">
                            <h3>Ваша команда</h3>
                            <p class="text-muted">Придумайте название, чтобы другие вас узнали.</p>
                            
                            <div class="team-input-group">
                                ${Input(inputSettings)}
                            </div>
                            
                            <div class="btn-wrapper mt-3">
                                ${Button(buttonSettings)}
                            </div>
                        </div>
                    </div>
                </aside>`
    }

    _renderWaitingTeamsPanel() {
        const teamsListLogic = this.state.teams.length === 0
            ? `<li class="empty-state">Пока никого нет. Будьте первыми!</li>`
            : this.state.teams
                .map(team => `
                    <li class="team-item ${team === this.state.myTeamName ? 'my-team' : ''}">
                        <span class="team-icon">👥</span>
                        <span class="team-name">${team}</span>
                        ${team === this.state.myTeamName ? '<span class="badge-you">Вы</span>' : ''}
                    </li>`)
                .join('')

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
                    </div>
                </main>`
    }

    render() {
        return `
            <div class="logo-corner">${Logo()}</div>
            <div class="waiting-layout">
                ${this._renderWaitingInfoPanel()}
                ${this._renderWaitingTeamsPanel()}
            </div>
        `;
    }

    attachEvents() {
        const input = this.container.querySelector('#team-name-input');
        const saveBtn = this.container.querySelector('#save-team-btn');

        input.addEventListener('input', (e) => {
            this.state.myTeamName = e.target.value;
        });

        saveBtn.addEventListener('click', () => this._saveTeamName());
    }

    async _saveTeamName() {
        if (!this.state.myTeamName.trim()) {
            alert("Название команды не может быть пустым!");
            return;
        }

        if (this.state.isNameSaved) {
            this.state.isNameSaved = false;
            this.updateDOM();
        } else {
            this.state.isNameSaved = true;

            const uuid = localStorage.getItem('team-uuid');
            const resp = await setTeamName(
                this.state.gameId,
                this.state.roomCode,
                uuid,
                this._savedName,
                this.state.myTeamName
            );
            if (resp.status !== 'ok') {
                alert("Имя не было сохранено. Повторите попытку!");
                return;
            }

            this._savedName = this.state.myTeamName;
            this.updateDOM();
        }
    }

    _connectSocket() {
        if (this.ws) return;
        const protocol = window.location.protocol === "https:" ? "wss" : "ws";
        this.ws = new WebSocket(
            `${protocol}://${window.location.host}/api/ws/room/${this.data.roomCode}`
        );

        this.ws.onmessage = (event) => {
            const data = JSON.parse(event.data);
            console.log('ws message:', data);

            if (data.type === "init" || data.type === "update") {
                this.state.teams = data.teams;
                this.updateDOM();
            }

            if (data.type === "game_started"){
                console.log("here");
                window.history.pushState({}, '', `/room/active/${this.data.roomCode}`);
                window.dispatchEvent(new Event('popstate'));
            }
        };
    }
}