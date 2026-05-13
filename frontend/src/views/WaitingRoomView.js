import {Component} from "../core/Component.js";
import {tryGetRoomInfo, setTeamName, tryGetTeamInfo, checkKicked} from "../services/RoomService.js";
import {get404, redirectTo} from "../services/RouteServices.js";
import {Logo} from "../components/UI.js";
import {WaitingInfoPanel, WaitingTeamsPanel} from "../components/WaitingPanels.js";

export class WaitingRoomView extends Component {
    /**
     * Представление для комнаты ожидания. Игроки попадают в нее после ввода
     * кода комнаты или перехода по ссылке. Можно указать название для команды,
     * исправить его, подключиться к лобби и увидеть уже подключенные команды
     */
    constructor(container, data) {
        super(container, data);

        this.ws = null;
        this.state = {
            gameId: null,
            roomCode: null,
            gameName: 'Загрузка...',
            creator: 'Загрузка...',
            myTeamName: '',
            isNameSaved: false,
            teams: []
        };
    }

    async mount() {
        const roomData = await tryGetRoomInfo(this.data.roomCode);
        if (!roomData || roomData.status !== 'waiting') {
            this.container.innerHTML = get404();
            return;
        }

        this._connectSocket();

        this.state = {
            gameId: roomData.id,
            roomCode: roomData.code,
            gameName: roomData.title || 'Без названия',
            creator: roomData.author || 'неизвестный...',
            myTeamName: '',
            isNameSaved: false,
            teams: roomData.teams || []
        }

        this.updateDOM();
    }

    updateDOM() {
        this.container.innerHTML = this.render();
        this.attachEvents();
    }

    render() {
        return `
            <div class="logo-corner">${Logo()}</div>
            <div class="waiting-layout">
                ${WaitingInfoPanel(this.state, this.data)}
                ${WaitingTeamsPanel(this.state)}
            </div>
        `;
    }

    attachEvents() {
        const input = this.container.querySelector('#team-name-input');
        const saveBtn = this.container.querySelector('#save-team-btn');

        input.addEventListener('input', (e) => {
            this.state.myTeamName = e.target.value;
        });

        input.addEventListener('keypress', async (e) => {
            if (e.key === 'Enter') {
                await this._saveTeamName();
            }
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
        } else {
            this.state.isNameSaved = true;
            await setTeamName(this.state.gameId, this.state.roomCode, this.state.myTeamName);
        }

        this.updateDOM();
    }

    _connectSocket() {
        if (this.ws) return;
        const protocol = window.location.protocol === "https:" ? "wss" : "ws";
        this.ws = new WebSocket(`${protocol}://${window.location.host}/api/ws/room/${this.data.roomCode}`);

        this.ws.onmessage = async (event) => {
            const data = JSON.parse(event.data);
            switch (data.type) {
                case 'init':
                    await this.initRoom(data.teams);
                    break;
                case 'update':
                    await this.updateRoom(data.teams, data.kicked);
                    break;
                case 'game_started':
                    redirectTo(`/room/active/${this.data.roomCode}`);
                    break;
            }
        };
    }

    async initRoom(teams) {
        this.state.teams = teams;
        const roomInfo = await tryGetTeamInfo(this.state.roomCode);
        if (!roomInfo) {
            return;
        }
        this.state.myTeamName = roomInfo.name;
        this.state.isNameSaved = true;
        this.updateDOM();
    }

    async updateRoom(teams, kicked) {
        this.state.teams = teams;
        const isIKicked = await checkKicked(this.state.roomCode);
        if (isIKicked) {
            this.ws.close();
            redirectTo(`/`);
            alert("Вас забанили за читы!");
            return;
        }
        this.updateDOM();
    }
}