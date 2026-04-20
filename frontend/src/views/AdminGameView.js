import {Component} from "../core/Component.js";
import {get404} from "../services/RouteServices.js";
import {tryGetGameSettings, addPoints, updateTeamScores} from "../services/GamesServices.js";
import {loadUserInfoOrRedirect} from "../services/AccountServices.js";
import {Logo, Button} from "../components/UI.js";
import {GameSettings} from "../domain/GameSettings.js";

export class AdminGameView extends Component {
    constructor(container, data) {
        super(container, data);

        this.gameSettings = new GameSettings();

        this.state = {
            activeCell: null,
            buzzedTeam: null,
            audio: null
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

    renderCalculator() {
        if (!this.state.buzzedTeam) return '';

        return `
        <div class="card calculator-card">
            <h3>Отвечает: <strong>${this.state.buzzedTeam}</strong></h3>
            <div class="btn-group-vertical">
                ${Button({text: 'Верно', id: 'award-btn', extraClass: 'w-100'})}
                ${Button({text: 'Неверно', id: 'wrong-btn', variant: 'outline', extraClass: 'w-100'})}
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
            </main>
        </div>
        `;
    }

    attachEvents() {
        this.container
            .querySelectorAll('.organizer-track-btn')
            .forEach(btn => btn.addEventListener('click', this._startSong.bind(this)));

        this.container
            .querySelector('#award-btn')
            .addEventListener('click', async () => {

            });

        const wrongBtn = this.container.querySelector('#wrong-btn');
        if (wrongBtn) {
            wrongBtn.addEventListener('click', async () => {
                const points = Number(
                    this.container.querySelector('#points-input').value
                );

                await addPoints(
                    this.data.roomCode,
                    points,
                    this.state.buzzedTeam
                );

                this.state.buzzedTeam = null;
                this.state.activeCell = null;

                this.updateDOM();
            });
        }

        const endBtn = this.container.querySelector('#end-btn');
        if (endBtn) {
            endBtn.addEventListener('click', () => {
                // завершение игры
            });
        }
    }

    async _processTeamAnswer(isCorrect) {
        this.state.audio.pause();

        const points = this.state.activeCell ? this.gameSettings.costs[this.state.activeCell.col] : 0;
        await updateTeamScores(this.data.roomCode, points, isCorrect, this.state.buzzedTeam);
        this.state.buzzedTeam = null;

        if (isCorrect) {
            this.state.activeCell = null;
            this.state.audio = null;
            // Показать правильный ответ
        } else {
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
                this.state.buzzedTeam = data.team;
                this.updateDOM();
            }
        };
    }
}