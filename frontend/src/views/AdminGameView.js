import {Component} from "../core/Component.js";
import {get404} from "../services/RouteServices.js";
import {tryGetGameSettings, updateTeamScores} from "../services/GamesServices.js";
import {loadUserInfoOrRedirect} from "../services/AccountServices.js";
import {Logo, Button} from "../components/UI.js";
import {GameSettings} from "../domain/GameSettings.js";

export class AdminGameView extends Component {
    constructor(container, data) {
        super(container, data);
        const teams = JSON.parse(localStorage.getItem('teams') || '[]');

        this.gameSettings = new GameSettings();

        this.state = {
            activeCell: null,
            buzzedTeam: null,
            teamAnswer : null,
            audio: null,
            players: Object.fromEntries(
                teams.map(team => [team, 0]))
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

    renderPlayerTable(){
        const players = this.state.players;
        const entries = Object.entries(players);
        const sorted = [...entries].sort((a, b) => b[1] - a[1]);
        return `
            <div class="player-table-wrapper">
                <table class="player-score-table">
                    <thead>
                        <tr>
                            <th>#</th>
                            <th>Команда</th>
                            <th>Очки</th>
                         </tr>
                    </thead>
                    <tbody>
                        ${sorted.map(([team, score], idx) => `
                            <tr class="${idx === 0 ? 'rank-first' : ''}">
                                <td class="rank-cell">${idx + 1}</td>
                                <td class="team-cell">${team}</td>
                                <td class="score-cell">${score}</td>
                            </tr>
                        `).join('')}
                    </tbody>
                </table>
            </div>`;
    }

    renderCalculator() {
        if (!this.state.buzzedTeam) return '';

        const defaultPoints = this.state.activeCell
            ? this.gameSettings.costs[this.state.activeCell.col]
            : 0;

        return `
        <div class="card calculator-card">
            <h3>Отвечает: <strong>${this.state.buzzedTeam}</strong></h3>
            ${this.state.teamAnswer ? `
                <div class="form-group">
                    <p>🎤 <strong>${this.state.teamAnswer.artist}</strong></p>
                    <p>🎵 <strong>${this.state.teamAnswer.title}</strong></p>
                </div>
                ` : '<p class="muted">Ожидание ответа команды...</p>'}
            
            <div class="points-stepper">
                 <input class="stepper-value" id="points-value" type="number" value="${defaultPoints}">
            </div>
            
            <div class="btn-group-horizontal">
                ${Button({text: '', id: 'award-btn', extraClass: 'w-100'})}
                ${Button({text: '', id: 'wrong-btn', variant: 'outline', extraClass: 'w-100'})}
            </div>
        </div>
        `;
    }

    renderTable() {
        return `
        <div class="scroll-container">
            <table class="edit-table game-table">
                <thead>
                    <tr>
                        <th class="corner-cell">Категория / Цена</th>

                        ${this.gameSettings.costs.map(cost => `
                            <th class="cost-th">
                                <div class="th-content readonly-th">
                                    <span>${cost}</span>
                                </div>
                            </th>
                        `).join('')}
                    </tr>
                </thead>

                <tbody>
                    ${this.gameSettings.categories.map((category, rIdx) => `
                        <tr>
                            <td class="cat-td">
                                <div class="td-content readonly-td">
                                    <span>${category}</span>
                                </div>
                            </td>

                            ${this.gameSettings.costs.map((cost, cIdx) => {
            const track = this.gameSettings.cells?.[rIdx]?.[cIdx];

            return `
                                <td>
                                    <button
                                        class="preview-cell track-cell organizer-track-btn"
                                        data-row="${rIdx}"
                                        data-col="${cIdx}"
                                        style="width: 100%;"
                                    >
                                        <div class="cell-sub">
                                            ${track ? '▶ Запустить' : 'Нет трека'}
                                        </div>
                                    </button>
                                </td>
                                `;
        }).join('')}
                        </tr>
                    `).join('')}
                </tbody>
            </table>
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
                        ${this.renderCalculator()}

                        ${Button({
                            text: 'Завершить игру',
                            id: 'end-btn',
                            extraClass: 'w-100'
                        })}
                    </div>
                </div>
            </aside>

            <main class="editor-main">
                <div class="table-header-actions">
                    <div class="badge">Код комнаты: ${this.data.roomCode}</div>
                    <div class="badge">Режим организатора</div>
                </div>

                ${this.renderTable()}
                ${this.renderPlayerTable()}
            </main>
        </div>
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

        const endBtn = this.container.querySelector('#end-btn');
        if (endBtn) {
            endBtn.addEventListener('click', () => {
                this.ws.send(JSON.stringify({
                    type: 'game_ended'
                }));
                localStorage.removeItem('teams');
                new Promise(r => setTimeout(r, 200));
                window.history.pushState({}, '', '/account');
                window.dispatchEvent(new Event('popstate'));
            });
        }
        if (valueEl)
        {
            valueEl.addEventListener('input', () => {
                const v = +valueEl.value || 0;
                awardBtn.textContent = `+${v}`;
                wrongBtn.textContent = `−${v}`;
            });
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
            const song = this.gameSettings.cells?.[row]?.[col]?.song;
            if (song) {
                this.ws.send(JSON.stringify({
                    type : 'show_answer',
                    title : song.title,
                    artist : song.artist,
                }));
            }
            this.state.activeCell = null;
            this.state.buzzedTeam = null;
            this.state.teamAnswer = null;
            this.state.audio.playCorrectAnswer();
            this.state.audio = null;
        } else {
            this.state.buzzedTeam = null;
            this.state.teamAnswer = null;
            this.state.audio.play();
        }

        this.updateDOM();
    }



    _startSong(e) {
        const row = +e.currentTarget.dataset.row;
        const col = +e.currentTarget.dataset.col;

        const cell = this.gameSettings.cells?.[row]?.[col];
        if (!cell) return;

        this.state.activeCell = {row, col};

        cell.song.play(this.state);

        this.ws.send(JSON.stringify({
            type: 'track_started',
            row,
            col
        }));

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
                console.log(data);
                this.state.buzzedTeam = data.team;
                if (this.state.audio) {
                    this.state.audio.pause();
                }
                this.updateDOM();
            }

            if (data.type === 'team_answer'){
                console.log('Получил team_answer:', data);
                this.state.teamAnswer = {
                    artist: data.artist,
                    title: data.title,
                };
                this.updateDOM();
            }

            if (data.type === 'add_points'){
                console.log(data);
                const team = data.team;
                const points = data.points;
                this.state.players[team]+=points;
                this.updateDOM();
            }
        };
    }
}