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

        this.state = {
            activeCell: null,
            canBuzz: false,
            answer : null,
            showAnswerInput: false,
            players: {}
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
                ${this.gameSettings.categories.map((category, rowIdx) => `
                    <tr>
                        <td class="cat-td">
                            <div class="td-content readonly-td">
                                <span>${category}</span>
                            </div>
                        </td>

                        ${this.gameSettings.costs.map((cost, colIdx) => {

            const track =
                this.gameSettings.tracks?.[category]?.[cost];

            const isActive =
                this.state.activeCell &&
                this.state.activeCell.row === rowIdx &&
                this.state.activeCell.col === colIdx;

            return `
                                <td>
                                    <button
                                        class="preview-cell track-cell organizer-track-btn
                                               ${track ? 'cell-filled' : ''}
                                               ${isActive ? 'cell-active' : ''}
                                        data-row="${rowIdx}"
                                        data-col="${colIdx}"
                                        style="width: 100%;"
                                    >
                                        <div class="cell-sub">
                                            ${track ? `🎵 ${track.title}` : '—'}
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

            <div class="editor-layout player-mode">
                <aside class="editor-sidebar">
                    <div class="card">
                        <h2 class="card-title">Информация об игре</h2>

                        <div class="game-meta">
                            <p><strong>Название:</strong> ${this.gameSettings.title || '—'}</p>
                            <p><strong>Автор:</strong> ${this.gameSettings.author || '—'}</p>
                            <p><strong>Описание:</strong> ${this.gameSettings.description || '—'}</p>
                        </div>

                        <div class="player-status-card">
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
                <div class="card modal-card">
                    <h3>Правильный ответ</h3>
                        <p><strong>${this.state.answer.title}</strong></p>
                        <p>${this.state.answer.artist}</p>
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
                this.state.activeCell = {
                    row: data.row,
                    col: data.col
                };

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
                window.history.pushState({}, '', '/');
                window.dispatchEvent(new Event('popstate'));
            }
        };
    }

    attachEvents() {
        this.container.querySelectorAll('.organizer-track-btn').forEach(btn => {
            btn.onclick = () => {
                if (!this.isHost) return;

                const row = +btn.dataset.row;
                const col = +btn.dataset.col;

                const cell = this.gameSettings.cells[row][col];

                if (!cell?.song) return;

                this.playSong(cell.song);

                this.state.activeCell = { row, col };

                this.ws.send(JSON.stringify({
                    type: 'track_started',
                    row,
                    col
                }));

                this.updateDOM();
            };
        });

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