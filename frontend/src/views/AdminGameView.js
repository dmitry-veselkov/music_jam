import {Component} from "../core/Component.js";
import {get404} from "../services/RouteServices.js";
import {tryGetGameSettings, updateTeamScores} from "../services/GamesServices.js";
import {loadUserInfoOrRedirect} from "../services/AccountServices.js";
import {Logo, Button} from "../components/UI.js";
import {GameSettings} from "../domain/GameSettings.js";
import {OnGameRating} from "../components/OnGameRating.js";
import {OnGameTable} from "../components/OnGameTable.js";

export class AdminGameView extends Component {
    constructor(container, data) {
        super(container, data);
        const teams = JSON.parse(sessionStorage.getItem('teams') || '[]');

        this.gameSettings = new GameSettings();

        this.state = {
            activeCell: null,
            buzzedTeam: null,
            disabledTeams: [],
            teamAnswer: null,
            showCalculatorModal: false,
            audio: null,
            correctAudio: null,
            players: Object.fromEntries(
                teams.map(team => [team, 0])),
            playedCells: [],
            currentAnswer: null,
            isShowingAnswer: false,
        };
    }

    _applyRoomInfo(roomInfo) {
        this.gameSettings.init(roomInfo);
    }

    async mount() {
        const userInfo = await loadUserInfoOrRedirect();
        if (!userInfo) return;

        const roomInfo = await tryGetGameSettings(this.data.roomCode);

        if (!roomInfo?.exists) {
            this.container.innerHTML = get404();
            return;
        }

        this._connectSocket();
        this._applyRoomInfo(roomInfo);

        this.updateDOM();
    }

    updateDOM() {
        this.container.innerHTML = this.render();
        this.attachEvents();
    }

    renderCorrectAnswer(){
        if (!this.state.currentAnswer) return '';
        return `
        <div class="organizer-hint">
            <div>🎵 ${this.state.currentAnswer.title}</div>
            <div>🎤 ${this.state.currentAnswer.artist}</div>
        </div>
    `;
    }

    renderCalculatorModal() {
        if (!this.state.buzzedTeam) return '';

        const defaultPoints = this.state.activeCell
            ? this.gameSettings.costs[this.state.activeCell.col]
            : 0;


        return `
        <div id="calculator-modal" class="modal">
            <div class="modal-backdrop"></div>

            <div class="modal-dialog">
                <button class="modal-close" id="close-calculator-modal" aria-label="Закрыть">×</button>

                <div class="modal-header">
                    <div class="modal-badge">🧮 Калькулятор</div>
                    <h3 class="modal-title">Отвечает: <strong>${this.state.buzzedTeam}</strong></h3>
                </div>

                <div class="modal-body">
                    ${this.state.teamAnswer ? `
                        <div class="form-group">
                            <p>🎤 <strong>${this.state.teamAnswer.artist}</strong></p>
                            <p>🎵 <strong>${this.state.teamAnswer.title}</strong></p>
                        </div>
                    ` : '<p class="muted">Ожидание ответа команды...</p>'}

                    <div class="points-stepper">
                        <input
                            class="stepper-value"
                            id="points-value"
                            type="number"
                            value="${defaultPoints}"
                            min="0"
                            placeholder="${defaultPoints}">
                    </div>
                </div>

                <div class="modal-actions">
                    ${Button({text: '+', id: 'award-btn', extraClass: 'w-100'})}
                    ${Button({text: '-', id: 'wrong-btn', variant: 'outline', extraClass: 'w-100'})}
                </div>
            </div>
        </div>
    `;
    }

    render() {
        return `
        <div class="logo-corner">${Logo()}</div>

        <div class="editor-layout organizer-mode">
            <aside class="editor-sidebar">
                <div class="card">
                    <h2 class="card-title">Параметры игры</h2>

                    <div class="form-group">
                        <label>Название</label>
                        <div class="ui-input readonly-field">
                            ${this.gameSettings.title || '—'}
                        </div>
                    </div>

                    <div class="form-group">
                        <label>Описание</label>
                        <div class="ui-input readonly-field readonly-textarea">
                            ${this.gameSettings.description || '—'}
                        </div>
                    </div>

                    <div class="btn-group-vertical">
                        ${Button({
                            text: 'Завершить вопрос',
                            id: 'end-question-btn',
                            extraClass: 'w-100'
                        })}
                        <div style="margin-top: 0.75rem;">
                            ${Button({
                                text: 'Завершить игру',
                                id: 'end-btn',
                                extraClass: 'w-100'
                            })}
                        </div>
                    </div>
                </div>
            </aside>

            <main class="editor-main">
                <div class="table-header-actions">
                    <div class="badge">Код комнаты: ${this.data.roomCode}</div>
                    <div class="badge">Режим организатора</div>
                </div>
                ${this.renderCorrectAnswer()}
                ${OnGameTable(this.gameSettings, this.state, this._tableOptions)}
                ${OnGameRating(this.state.players)}
            </main>
            ${Button({
            text: '❚❚',
            id: 'music-btn',
        })}
            ${this.state.isShowingAnswer ? Button({ 
            text: '✕ Завершить ответ', 
            id: 'clear-audio-btn', 
            variant: 'outline' }) : ''} 
        </div>
        
        ${this.renderCalculatorModal()}
        `;
    }

    async attachEvents() {
        const valueEl = this.container.querySelector('#points-value');

        this.container
            .querySelectorAll('.organizer-track-btn')
            .forEach(btn => btn.addEventListener('click', this._startSong.bind(this)));

        const awardBtn = this.container.querySelector('#award-btn');
        if (awardBtn) {
            awardBtn.addEventListener('click', async () => {
                await this._processTeamAnswer(true, +valueEl.value);
            });
        }

        const wrongBtn = this.container.querySelector('#wrong-btn');
        if (wrongBtn) {
            wrongBtn.addEventListener('click', async () => {
                await this._processTeamAnswer(false, -valueEl.value);
            });
        }
        const endQuestionBtn = this.container.querySelector('#end-question-btn');
        if (endQuestionBtn) {
            endQuestionBtn.addEventListener('click', async () => {
                if (this.state.activeCell) {
                    const {row, col} = this.state.activeCell;
                    const cell = this.gameSettings.cells?.[row]?.[col];
                    if (cell) {
                        this.ws.send(JSON.stringify({
                            type: 'show_answer',
                            title: cell.song.title,
                            artist: cell.song.artist,
                        }));
                        this._semiCorrectAnswer(cell);
                    }
                }
            })
        }
        const endBtn = this.container.querySelector('#end-btn');
        if (endBtn) {
            endBtn.addEventListener('click', () => {
                this.ws.send(JSON.stringify({
                    type: 'game_ended'
                }));
                sessionStorage.removeItem('teams');
                new Promise(r => setTimeout(r, 200));
                window.history.pushState({}, '', '/account');
                window.dispatchEvent(new Event('popstate'));
            });
        }
        if (valueEl) {
            valueEl.addEventListener('input', () => {
                const v = +valueEl.value || 0;
                awardBtn.textContent = `+${v}`;
                wrongBtn.textContent = `−${v}`;
            });
        }

        const musicBtn = this.container.querySelector('#music-btn');
        if (musicBtn) {
            musicBtn.addEventListener('click', () => {
                if (!this.state.audio) return;
                if (this.state.audio.paused) {
                    this.state.audio.play();
                    musicBtn.textContent = '❚❚';
                }
                else{
                    this.state.audio.pause();
                    musicBtn.textContent = '▶';
                }

            })
        }

        const clearAudioBtn = this.container.querySelector('#clear-audio-btn');
        if (clearAudioBtn) {
            clearAudioBtn.addEventListener('click', () => {
                if (this.state.audio.paused) {
                    this.state.audio.pause();
                    this.state.audio = null;
                }
                this.state.isShowingAnswer = false;
                this.updateDOM();
            })
        }
    }

    async _processTeamAnswer(isCorrect, points) {
        this.state.audio.pause();

        this.ws.send(JSON.stringify({
            type: 'add_points',
            team: this.state.buzzedTeam,
            points: points,
        }));

        if (isCorrect) {
            const {row, col} = this.state.activeCell;
            const cell = this.gameSettings.cells?.[row]?.[col];
            if (cell) {
                this.ws.send(JSON.stringify({
                    type: 'show_answer',
                    title: cell.song.title,
                    artist: cell.song.artist,
                }));
            }
            this._semiCorrectAnswer(cell);
        } else {
            this.state.disabledTeams.push(this.state.buzzedTeam);
            this.state.buzzedTeam = null;
            this.state.teamAnswer = null;
            this.state.audio.play();
        }

        this.ws.send(JSON.stringify({
            type: 'reset_answer_btn',
            disabledTeams: this.state.disabledTeams,
        }));

        this.updateDOM();
    }

    _semiCorrectAnswer(cell){
        this.state.activeCell = null;
        this.state.buzzedTeam = null;
        this.state.teamAnswer = null;
        this.state.isShowingAnswer = true;
        cell.song.playCorrectAnswer(this.state);
        this.state.currentAnswer = null;
        this.updateDOM();
    }

    _startSong(e) {
        const row = +e.currentTarget.dataset.row;
        const col = +e.currentTarget.dataset.col;
        if (this.state.playedCells.some(c => c.row === row && c.col === col)) return;
        const cell = this.gameSettings.cells?.[row]?.[col];
        if (!cell) return;

        this.state.activeCell = {row, col};
        this.state.currentAnswer = {
            title : cell.song.title,
            artist : cell.song.artist,
        };

        cell.song.play(this.state);
        cell.played = true;
        this.ws.send(JSON.stringify({
            type: 'track_started',
            row,
            col
        }));
        this.state.playedCells.push(this.state.activeCell);
        this.updateDOM();
    }

    _connectSocket() {
        if (this.ws) return;

        const protocol = window.location.protocol === 'https:' ? 'wss' : 'ws';

        this.ws = new WebSocket(
            `${protocol}://${window.location.host}/api/ws/room/${this.data.roomCode}`
        );

        this.ws.onmessage = (event) => {
            const data = JSON.parse(event.data);

            if (data.type === 'player_buzzed') {
                this.state.buzzedTeam = data.team;
                if (this.state.audio) {
                    this.state.audio.pause();
                }
                this.updateDOM();
            }

            if (data.type === 'team_answer') {
                this.state.teamAnswer = {
                    artist: data.artist,
                    title: data.title,
                };
                this.updateDOM();
            }

            if (data.type === 'add_points') {
                const team = data.team;
                const points = data.points;
                this.state.players[team] += points;
                this.updateDOM();
            }
        };
    }

    _tableOptions = {
        showFilledState: false,
        showActiveState: false,
        useLaunchText: true
    }


}