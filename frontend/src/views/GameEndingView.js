import {Component} from "../core/Component.js";
import {Button, Logo} from "../components/UI.js";
import {get404, redirectTo} from "../services/RouteServices.js";
import {tryGetGameSettings} from "../services/GamesServices.js";
import {GameSettings} from "../domain/GameSettings.js";
import {OnGameRating} from "../components/OnGameRating.js";
import {OnGameTable} from "../components/OnGameTable.js";

export class GameEndingView extends Component {
    constructor(container, data) {
        super(container, data);
    }

    _applyRoomInfo(roomInfo) {
        this.gameSettings.init(roomInfo);

    }

    async mount() {
    }

    updateDOM() {
        this.container.innerHTML = this.render();
        this.attachEvents();
    }

    render() {
        return `
            <div class="logo-corner">${Logo()}</div>

            <div class="editor-layout player-mode">
                <main class="editor-main">
                    <div class="table-header-actions">
                        <div class="badge">Код комнаты: ${this.data.roomCode}</div>
                    </div>

                    ${OnGameTable(this.gameSettings, this.state, this._tableOptions)}
                    ${OnGameRating(this.state.players, sessionStorage.getItem('team-name'))}
                </main>
            </div>
        `;
    }

    renderCorrectAnswer() {
        if (!this.state.answer) return '';

        return `
        <div class="modal-overlay" id="answer-overlay">
            <div class="card modal-card answer-card">
                <button class="modal-close" id="close-answer-btn">×</button>
                <div class="answer-icon">🎵</div>
                <div class="answer-label">Правильный ответ</div>
                <p class="answer-title">${this.state.answer.title}</p>
                <p class="answer-artist">${this.state.answer.artist}</p>
            </div>
        </div>`;
    }

    renderIncorrectAnswer() {
        if (!this.state.hadWrongAnswer) return '';
        return `
        <div class="modal-overlay answer-fade">
            <div class="card modal-card answer-card">
                <div class="answer-icon">🎵</div>
                <div class="answer-label">Вы дали неправильный ответ!</div>
            </div>
        </div>`;
    }

    renderAnswerInput() {
        if (!this.state.showAnswerInput) return '';

        return `
            <div class="modal-overlay">
                <div class="card modal-card">
                    <h3 class="card-title">Ваш ответ</h3>
                        <div class="form-group">
                            <label>Исполнитель</label>
                            <input class="ui-input" id="answer-artist" placeholder="Введите исполнителя" />
                        </div>
                        <div class="form-group">
                            <label>Название трека</label>
                            <input class="ui-input" id="answer-title" placeholder="Введите название" />
                        </div>
                        <div class="btn-group-vertical">
                            ${Button({text: 'Отправить', id: 'submit-answer-btn', extraClass: 'w-100'})}
                        </div>
                </div>
            </div>`;
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
                const myTeam = sessionStorage.getItem('team-name') ?? 'Неизвестная команда';
                this.state.hadWrongAnswer = data.disabledTeams.includes(myTeam);
                this.state.canBuzz = !this.state.hadWrongAnswer;
                this.state.whoAnswers = null;
                this.updateDOM();

                if (this.state.hadWrongAnswer) {
                    setTimeout(() => {
                        this.state.hadWrongAnswer = false;
                        this.updateDOM();
                    }, 3000);
                }
            }

            if (data.type === 'player_buzzed') {
                this.state.canBuzz = false;
                const myTeam = sessionStorage.getItem('team-name') ?? 'Неизвестная команда';
                this.state.whoAnswers = myTeam;
                if (data.team === myTeam) {
                    this.state.showAnswerInput = this.gameSettings.mode === false;
                }
                this.updateDOM();
            }

            if (data.type === 'show_answer') {
                this.state.answer = {
                    title: data.title,
                    artist: data.artist,
                }
                this.state.whoAnswers = null;
                this.state.canBuzz = false;
                this.state.showAnswerInput = false;
                this.state.activeCell = null;
                this.updateDOM();
                setTimeout(() => {
                    this.state.answer = null
                    this.updateDOM();
                }, 5000);
            }

            if (data.type === 'game_ended') {
                sessionStorage.removeItem('teams');
                sessionStorage.removeItem('team-name');
                redirectTo('/');
            }

            if (data.type === 'add_points') {
                console.log('add_points');
                const team = data.team;
                const points = data.points;
                this.state.players[team] += points;
                this.updateDOM();
            }
        };
    }

    attachEvents() {
        const buzzBtn = this.container.querySelector('#buzz-btn');
        if (buzzBtn) {
            buzzBtn.onclick = () => {
                const team = sessionStorage.getItem('team-name') ?? 'Неизвестная команда';

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
                        team: sessionStorage.getItem('team-name') ?? 'Неизвестная команда',
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
                this.container.querySelector('#answer-overlay').remove();
            }
        }

        const closeCorrectAnswerBackground = this.container.querySelector('#answer-overlay');
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
        emptyText: '-',
        useLaunchText: false
    }
}