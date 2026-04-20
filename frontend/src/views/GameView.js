import {Component} from "../core/Component.js";
import {Button, Logo} from "../components/UI.js";
import {get404} from "../services/RouteServices.js";
import {tryGetGameSettings, saveGameSettings} from "../services/GamesServices.js";
import {loadUserInfoOrRedirect} from "../services/AccountServices.js";

export class GameView extends Component {
    constructor(container, data) {
        super(container, data);
        this.state = {
            settings: {
                name: '',
                author: '',
                description: '',
                maxTeams: 4
            },
            categories: ['Категория 1', 'Категория 2'],
            costs: [100, 200, 300],
            tracks: {},
            activeCell: null,
            playedTracks: {},
            players: {}
        };
    }

    _applyRoomInfo(roomInfo) {
        this.state.settings = {
            name: roomInfo.name ?? '',
            author: roomInfo.author ?? '',
            description: roomInfo.description ?? '',
            maxTeams: roomInfo.maxTeams ?? 4
        };

        this.state.categories = roomInfo.categories ?? ['Категория 1', 'Категория 2'];
        this.state.costs = roomInfo.costs ?? [100, 200, 300];
        this.state.tracks = roomInfo.tracks ?? {};
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

    renderTable(){
        return `
        <div class="scroll-container">
                    <table class="edit-table game-table">
                        <thead>
                            <tr>
                                <th class="corner-cell">Категория / Цена</th>
                                ${this.state.costs.map(cost => `
                                    <th class="cost-th">
                                        <div class="th-content readonly-th">
                                            <span>${cost}</span>
                                        </div>
                                    </th>
                                `).join('')}
                            </tr>
                        </thead>

                        <tbody>
                            ${this.state.categories.map((cat, rIdx) => `
                                <tr>
                                    <td class="cat-td">
                                        <div class="td-content readonly-td">
                                            <span>${cat}</span>
                                        </div>
                                    </td>

                                     ${this.state.costs.map((cost, cIdx) => {
                                         const cat = this.state.categories[cIdx];
                                        const trackValue = this.state.tracks[cat]?.[cost];

                                        return `
                                            <td>
                                                <button
                                                    class="preview-cell track-cell organizer-track-btn"
                                                    data-row="${rIdx}"
                                                    data-col="${cIdx}"
                                                    data-track="${trackValue ? trackValue : ''}"
                                                    style="width: 100%;"
                                                >
                                                    <div class="cell-sub" style="width: 100%">
                                                        ${trackValue}
                                                    </div>
                                                </button>
                                            </td>
                                        `;}).join('')}
                                </tr>`).join('')}
                        </tbody>
                    </table>
                </div>`;
        }

    render() {
        return `
            <div class="logo-corner">${Logo()}</div>

                <div class="editor-layout player-mode">
                    <aside class="editor-sidebar">
                        <div class="card">
                            <h2 class="card-title">Информация об игре</h2>

                            <div class="game-meta">
                                <p><strong>Название:</strong> ${this.state.settings.name || '—'}</p>
                                <p><strong>Автор:</strong> ${this.state.settings.author || '—'}</p>
                                <p><strong>Описание:</strong> ${this.state.settings.description || '—'}</p>
                            </div>

                            <div class="player-status-card">
                                <div class="modal-badge">🎧 Сейчас играет</div>
                                <div class="track-preview-subtitle">
                                    ${this.state.activeCell
                                        ? 'Организатор запустил вопрос. Можно отвечать.'
                                        : 'Дождитесь, пока организатор выберет категорию.'}
                                </div>
                            </div>

                            ${this.state.activeCell && this.state.canBuzz
                            ? Button({ text: 'Ответить', id: 'buzz-btn', extraClass: 'w-100' })
                                : `<button class="btn btn-secondary w-100" disabled>
                                    ${this.state.activeCell ? 'Кто-то уже отвечает' : 'Ожидание вопроса'}
                                </button>`}
                        </div>
                    </aside>

        <main class="editor-main">
            <div class="table-header-actions">
                <div class="badge">Код комнаты: ${this.data.roomCode}</div>
            </div>
            ${this.renderTable()}
        </main>
    </div>
    `;
    }

    _connectSocket() {
        if (this.ws) return;
        const protocol = window.location.protocol === 'https:' ? 'wss' : 'ws';
        this.ws = new WebSocket(
            `${protocol}://${window.location.host}/api/ws/room/${this.data.roomCode}`
        );
        this.ws.onmessage = (event) => {
            const data = JSON.parse(event.data);
            if (data.type === 'track_started') {
                this.state.activeCell = { row: data.row, col: data.col };
                this.state.canBuzz = true;
                this.updateDOM();
            }
            if (data.type === 'player_buzzed') {
                this.state.canBuzz = false;
                this.updateDOM();
            }
        };
    }

    attachEvents() {
        const buzzBtn = this.container.querySelector('#buzz-btn');
        if (buzzBtn) {
            buzzBtn.addEventListener('click', () => {
                const team = localStorage.getItem('team-name') ?? 'Неизвестная команда';
                this.ws.send(JSON.stringify({
                    type: 'player_buzzed',
                    team
                }));
                this.state.canBuzz = false;
                this.updateDOM();
            });
        }
    }
}