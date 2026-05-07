import {Component} from "../core/Component.js";
import {tryGetRoomInfo, setTeamName} from "../services/RoomService.js";
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
        this._savedName = '';

        sessionStorage.removeItem('teams');

        this.state = {
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

        // const uuid = this._setUUID();
        // await this._loadTeamInfoByUUID(uuid);

        this._setState({
            gameId: roomData.id,
            roomCode: roomData.code,
            gameName: roomData.title || 'Без названия',
            creator: roomData.author || 'неизвестный...',
            teams: roomData.teams || []
        })
    }

    _setState(newState) {
        this.state = {...this.state, ...newState};
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
            this.updateDOM();
            return;
        }

        this.state.isNameSaved = true;
        const resp = await setTeamName(
            this.state.gameId,
            this.state.roomCode,
            this._savedName,
            this.state.myTeamName
        );
        if (resp.status !== 'ok') {
            // TODO заменить alert
            alert("Имя не было сохранено. Повторите попытку!");
            return;
        }

        this._savedName = this.state.myTeamName;
        sessionStorage.setItem('team-name', this.state.myTeamName);
        this.updateDOM();
    }

    _connectSocket() {
        if (this.ws) return;
        const protocol = window.location.protocol === "https:" ? "wss" : "ws";
        this.ws = new WebSocket(
            `${protocol}://${window.location.host}/api/ws/room/${this.data.roomCode}`
        );

        this.ws.onmessage = (event) => {
            const data = JSON.parse(event.data);
            console.log(data.type);

            if (data.type === "init") {
                this.state.teams = data.teams;
                let storageName = sessionStorage.getItem('team-name');
                if (!this.state.teams.includes(storageName)) {
                    sessionStorage.removeItem('team-name');
                }
                storageName = sessionStorage.getItem('team-name');
                if (storageName) {
                    this.state.myTeamName = storageName;
                    this.state.isNameSaved = true;
                } else {
                    this.state.isNameSaved = '';
                    this.state.isNameSaved = false;
                }
                this._savedName = this.state.myTeamName;
                this.updateDOM();
            } else if (data.type === "update") {
                if (!data.teams.includes(this.state.myTeamName)) {
                    redirectTo(`/`);
                    sessionStorage.removeItem('team-name');
                    alert("Вас забанили за читы!");
                    return;
                }
                this.state.teams = data.teams;
                this.updateDOM();
            } else if (data.type === "game_started") {
                sessionStorage.setItem('teams', JSON.stringify(data.teams));
                redirectTo(`/room/active/${this.data.roomCode}`);
            }
        };
    }
}