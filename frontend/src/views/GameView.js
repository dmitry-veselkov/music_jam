import {Component} from "../core/Component.js";
import {Button, Logo} from "../components/UI.js";
import {get404} from "../services/RouteServices.js";
import {tryGetGameSettings} from "../services/GamesServices.js";
import {GameSettings} from "../domain/GameSettings.js";

export class GameView extends Component {
    constructor(container, data) {
        super(container, data);

        this.gameSettings = new GameSettings();
        this.audio = null;

        const teams = JSON.parse(localStorage.getItem('teams') || '[]');

        this.state = {
            activeCell: null,
            canBuzz: false,
            answer : null,
            showAnswerInput: false,
            players: Object.fromEntries(
                teams.map(team => [team, 0])
            )
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

                            const cell = this.gameSettings.cells?.[rIdx]?.[cIdx];

                            const isActive = this.state.activeCell?.row === rIdx && this.state.activeCell?.col === cIdx;
                            
                            return `
                                <td>
                                    <button
                                        class="preview-cell track-cell organizer-track-btn
                                               ${cell.song ? 'cell-filled' : ''}
                                               ${isActive ? 'cell-active' : ''}"
                                        data-row="${rIdx}"
                                        data-col="${cIdx}"
                                        style="width: 100%;"
                                    >
                                        <div class="cell-sub">
                                            ${isActive ? '🔊 Играет...' : !cell.played ? '-' : 'Нет трека'}
                                        </div>
                                    </button>
                                </td>`;}).join('')}
                    </tr>`).join('')}
            </tbody>
        </table>
    </div>
    `;
    }

    render() {
        return `
            <div class="logo-corner">${Logo()}</div>

            <div class="editor-layout player-mode">
                <aside class="editor-sidebar">
                    <div class="card">
                        <h2 class="card-title">Информация об игре</h2>

                        <div class="game-meta">
                            <p><strong>Название:</strong> ${this.gameSettings.title || '—'}</p>
                            <p><strong>Автор:</strong> ${this.gameSettings.author || '—'}</p>
                            <p><strong>Описание:</strong> ${this.gameSettings.description || '—'}</p>
                        </div>

                        <div class="player-status-card" style="margin-top: 1rem; margin-bottom: 1rem;"">
                            <div class="modal-badge">🎧 Сейчас играет</div>
                            <div class="track-preview-subtitle">
                                ${
                                    this.state.activeCell
                                        ? 'Организатор запустил вопрос. Можно отвечать.'
                                        : 'Дождитесь, пока организатор выберет категорию.'
                                }
                            </div>
                        </div>

                        ${
                            this.state.activeCell && this.state.canBuzz
                            ? Button({ text: 'Ответить', id: 'buzz-btn', extraClass: 'w-100' })
                            : `<button class="btn btn-secondary w-100" disabled>
                               ${this.state.activeCell ? 'Кто-то уже отвечает' : 'Ожидание вопроса'}
                               </button>`
                        }
                    </div>
                </aside>

                <main class="editor-main">
                    <div class="table-header-actions">
                        <div class="badge">Код комнаты: ${this.data.roomCode}</div>
                    </div>

                    ${this.renderTable()}
                    ${this.renderPlayerTable()}
                </main>
            </div>
            ${this.renderCorrectAnswer()}
            ${this.renderAnswerInput()}
        `;
    }

    renderCorrectAnswer() {
        if (!this.state.answer) return '';

        return `
        <div class="modal-overlay">
            <div class="card modal-card answer-card">
                <div class="answer-icon">🎵</div>
                <div class="answer-label">Правильный ответ</div>
                <p class="answer-title">${this.state.answer.title}</p>
                <p class="answer-artist">${this.state.answer.artist}</p>
            </div>
        </div>`;
    }

    renderAnswerInput(){
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
                            ${Button({ text: 'Отправить', id: 'submit-answer-btn', extraClass: 'w-100' })}
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

            if (data.type === 'track_started') {
                const row = data.row;
                const col = data.col;
                this.state.activeCell = {
                    row : row,
                    col: col,
                };
                const cell = this.gameSettings.cells?.[row]?.[col];
                cell.played = true;
                this.state.canBuzz = true;

                // ❗ звук НЕ запускаем здесь
                this.updateDOM();
            }

            if (data.type === 'player_buzzed') {
                this.state.canBuzz = false;
                const myTeam = localStorage.getItem('team-name') ?? 'Неизвестная команда';
                if (data.team === myTeam) {
                    this.state.showAnswerInput = this.gameSettings.mode === false;
                }
                this.updateDOM();
            }

            if (data.type === 'show_answer') {
                this.state.answer = {
                    title : data.title,
                    artist : data.artist,
                }
                this.state.canBuzz = true;
                this.state.showAnswerInput = false;
                this.state.activeCell = null;
                this.updateDOM();
                setTimeout(() => {
                    this.state.answer = null
                    this.updateDOM();
                }, 3000);
            }

            if (data.type === 'game_ended') {
                localStorage.removeItem('teams');
                window.history.pushState({}, '', '/');
                window.dispatchEvent(new Event('popstate'));
            }

            if (data.type === 'add_points'){
                const team = data.team;
                const points = data.points;
                this.state.players[team]+=points;
                this.updateDOM();
            }
        };
    }

    attachEvents() {
        const buzzBtn = this.container.querySelector('#buzz-btn');
        if (buzzBtn) {
            buzzBtn.onclick = () => {
                const team = localStorage.getItem('team-name') ?? 'Неизвестная команда';

                this.ws.send(JSON.stringify({
                    type: 'player_buzzed',
                    team
                }));

                this.state.canBuzz = false;
                this.updateDOM();
            };
        }
        if (this.gameSettings.mode === false)
        {
            const submitBtn = this.container.querySelector('#submit-answer-btn');
            if (submitBtn) {
                submitBtn.onclick = () => {
                    const artist = this.container.querySelector('#answer-artist').value.trim();
                    const title = this.container.querySelector('#answer-title').value.trim();

                    this.ws.send(JSON.stringify({
                        type: 'team_answer',
                        team: localStorage.getItem('team-name') ?? 'Неизвестная команда',
                        artist,
                        title
                    }));
                    this.state.showAnswerInput = false;
                    this.updateDOM();
                };
            }
        }
    }
}