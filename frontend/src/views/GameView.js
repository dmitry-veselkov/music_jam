import {Component} from "../core/Component.js";
import {Logo} from "../components/UI.js";
import {get404, redirectTo} from "../services/RouteServices.js";
import {getRoomState, tryGetGameSettings} from "../services/GamesServices.js";
import {tryGetTeamInfo} from "../services/RoomService.js";
import {GameSettings} from "../domain/GameSettings.js";
import {OnGameRating} from "../components/OnGameRating.js";
import {OnGameTable} from "../components/OnGameTable.js";
import {AnswerInput, CorrectAnswerModalForPlayer, IncorrectAnswerModal} from "../components/AnswerModals.js";
import {PlayerGameSidebar} from "../components/PlayerGameSidebar.js";

export class GameView extends Component {
    constructor(container, data) {
        super(container, data);
        this.gameSettings = new GameSettings();
        this.state = {
            myTeamName: '',
            activeCell: null,
            hadWrongAnswer: false,
            canBuzz: false,
            answer: null,
            showAnswerInput: false,
            whoAnswers : null,
            players: {},
            playedCells: []
        };
    }

    _applyRoomInfo(roomInfo) {
        this.gameSettings.init(roomInfo);
    }

    async mount() {
        const roomInfo = await tryGetGameSettings(this.data.roomCode);

        if (!roomInfo.exists) {
            this.container.innerHTML = get404();
            return;
        }

        // Игрок должен быть зарегистрирован — иначе на главную
        const myTeam = await tryGetTeamInfo(this.data.roomCode);
        if (!myTeam?.name) {
            redirectTo('/');
            return;
        }
        this.state.myTeamName = myTeam.name;

        this._onKeyDown = (e) => {
            if (e.key === 'Enter') {
                const buzzBtn = this.container.querySelector('#buzz-btn');
                if (buzzBtn && !buzzBtn.disabled) {
                    buzzBtn.click();
                }
            }
        };

        document.addEventListener('keydown', this._onKeyDown);

        const roomState = await getRoomState(this.data.roomCode);
        if (roomState) {
            this.state.players = roomState.teams;
            this.state.playedCells = roomState.played_tracks;
        }

        this._connectSocket();
        this._applyRoomInfo(roomInfo);
        this.updateDOM();
    }

    updateDOM() {
        this.container.innerHTML = this.render();
        this.attachEvents();
    }

    render() {
        return `
            <div class="logo-corner">${Logo()}</div>

            <div class="editor-layout player-mode">
                ${PlayerGameSidebar(this.state, this.gameSettings)}
                
                <main class="editor-main">
                    <div class="table-header-actions">
                        <div class="badge">Код комнаты: ${this.data.roomCode}</div>
                    </div>

                    ${OnGameTable(this.gameSettings, this.state, this._tableOptions)}
                    ${OnGameRating(this.state.players, this.state.myTeamName)}
                </main>
            </div>
            ${CorrectAnswerModalForPlayer(this.state.answer)}
            ${AnswerInput(this.state.showAnswerInput)}
            ${IncorrectAnswerModal(this.state.hadWrongAnswer)}
        `;
    }

    _connectSocket() {
        if (this.ws) {
            return;
        }

        const protocol = window.location.protocol === 'https:' ? 'wss' : 'ws';

        this.ws = new WebSocket(
            `${protocol}://${window.location.host}/api/ws/room/${this.data.roomCode}`
        );

        this.ws.onmessage = (event) => {
            const data = JSON.parse(event.data);
            console.log(data);

            if (data.type === 'track_started') {
                const row = data.row;
                const col = data.col;
                this.state.activeCell = {
                    row: row,
                    col: col,
                };
                const cell = this.gameSettings.cells?.[row]?.[col];
                cell.played = true;
                this.state.canBuzz = true;

                this.updateDOM();
            }

            if (data.type === 'reset_answer_btn') {
                const myTeam = this.state.myTeamName;
                this.state.hadWrongAnswer = data.disabledTeams.includes(myTeam);
                this.state.canBuzz = !this.state.hadWrongAnswer;
                this.state.whoAnswers = null;
                this.updateDOM();

            }

            if (data.type === 'player_buzzed') {
                this.state.canBuzz = false;
                const myTeam = this.state.myTeamName;
                this.state.whoAnswers = data.team;
                if (data.team === myTeam) {
                    this.state.showAnswerInput = this.gameSettings.mode === false;
                }
                this.updateDOM();
            }

            if (data.type === 'show_answer') {
                const cell = this.state.activeCell;
                const row = cell?.row;
                const col = cell?.col;
                this.state.playedCells.push([row, col]);
                this.state.answer = {
                    title: data.title,
                    artist: data.artist,
                }
                this.state.whoAnswers = null;
                this.state.canBuzz = true;
                this.state.showAnswerInput = false;
                this.state.activeCell = null;
                this.state.hadWrongAnswer = false;
                this.updateDOM();
                setTimeout(() => {
                    this.state.answer = null
                    this.updateDOM();
                }, 5000);
            }

            if (data.type === 'game_ended') {
                const players = data.players ?? this.state.players ?? {};
                redirectTo(`/room/finish/${this.data.roomCode}`, {
                    players,
                    myTeamName: this.state.myTeamName,
                });
            }

            if (data.type === 'add_points') {
                const team = data.team;
                const points = data.points;
                this.state.players[team] = (this.state.players[team] || 0) + points;
                this.updateDOM();
            }
        };
    }

    attachEvents() {
        const buzzBtn = this.container.querySelector('#buzz-btn');
        if (buzzBtn) {
            buzzBtn.onclick = () => {
                const team = this.state.myTeamName;

                this.ws.send(JSON.stringify({
                    type: 'player_buzzed',
                    team
                }));

                this.state.canBuzz = false;
                this.updateDOM();
            };
        }
        if (this.gameSettings.mode === false) {
            const submitBtn = this.container.querySelector('#submit-answer-btn');
            if (submitBtn) {
                submitBtn.onclick = () => {
                    const artist = this.container.querySelector('#answer-artist').value.trim();
                    const title = this.container.querySelector('#answer-title').value.trim();

                    this.ws.send(JSON.stringify({
                        type: 'team_answer',
                        team: this.state.myTeamName,
                        artist,
                        title
                    }));
                    this.state.showAnswerInput = false;
                    this.updateDOM();
                };
            }
        }

        const closeCorrectAnswer = this.container.querySelector('#close-answer-btn');
        if (closeCorrectAnswer) {
            closeCorrectAnswer.onclick = () => {
                closeCorrectAnswer.closest('.modal-overlay').remove();
            }
        }

        const closeCorrectAnswerBackground =  document.querySelector('#answer-overlay');
        if (closeCorrectAnswerBackground) {
            closeCorrectAnswerBackground.onclick = (e) => {
                if (e.target.id === 'answer-overlay')
                    e.target.remove();
            }
        }
    }

    _tableOptions = {
        showFilledState: true,
        showActiveState: true,
        emptyText: '',
        useLaunchText: false
    }
}
