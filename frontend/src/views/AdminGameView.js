import {Component} from "../core/Component.js";
import {get404} from "../services/RouteServices.js";
import {tryGetGameSettings, saveGameSettings, addPoints} from "../services/GamesServices.js";
import {loadUserInfoOrRedirect} from "../services/AccountServices.js";
import {Logo, Input, Button} from "../components/UI.js";

export class AdminGameView extends Component {
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
            name: roomInfo['title'] ?? '',
        };

        this.state.categories = roomInfo.categories ?? ['Категория 1', 'Категория 2'];
        this.state.costs = roomInfo.costs ?? [100, 200, 300];
        this.state.tracks = roomInfo.tracks ?? {};
    }

    async mount() {
        const userInfo = await loadUserInfoOrRedirect();
        if (!userInfo) {
            return;
        }
        const roomInfo = await tryGetGameSettings(this.data.roomCode);
        console.log(roomInfo);
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

    renderCalculator() {
        if (!this.state.buzzedTeam) return '';
        return `
        <div class="card calculator-card">
            <h3>Отвечает: <strong>${this.state.buzzedTeam}</strong></h3>
            <div class="form-group">
                <label>Очки</label>
                <input class="ui-input" id="points-input" type="number" 
                       value="${this.state.activeCell ? this.state.costs[this.state.activeCell.col] : 0}"/>
            </div>
            <div class="btn-group-vertical">
                ${Button({ text: 'Верно', id: 'award-btn', extraClass: 'w-100' })}
                ${Button({ text: 'Неверно', id: 'wrong-btn', variant: 'outline', extraClass: 'w-100' })}
            </div>
        </div>
    `;
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
                                                        ${trackValue ? '▶ Запустить' : 'Нет трека'}
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
        const endGameBtn = Button({ text: 'Завершить игру', id: 'end-btn', extraClass: 'w-100' });
        return `
        <div class="logo-corner">${Logo()}</div>

        <div class="editor-layout organizer-mode">
            <aside class="editor-sidebar">
                <div class="card">
                    <h2 class="card-title">Параметры игры</h2>

                    <div class="form-group">
                        <label>Название</label>
                        <div class="ui-input readonly-field">${this.state.settings.name || '—'}</div>
                    </div>

                    <div class="form-group">
                        <label>Автор</label>
                        <div class="ui-input readonly-field">${this.state.settings.author || '—'}</div>
                    </div>

                    <div class="form-group">
                        <label>Описание</label>
                        <div class="ui-input readonly-field readonly-textarea">
                            ${this.state.settings.description || '—'}
                        </div>
                    </div>
                    
                    <div class="btn-group-vertical">
                        ${this.renderCalculator()}
                        ${endGameBtn}
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
        this.container.querySelectorAll('.organzer-track-btn').forEach((btn) => {
            btn.addEventListener('click', e => {
                const row = e.currentTarget.dataset.row;
                const col = e.currentTarget.dataset.col;
                const track = e.currentTarget.dataset.track;
                if (!track) return;

                this.state.activeCell = {row, col};
                this.ws.send(JSON.stringify({
                    type : 'track_started',
                    row, col, track
                }));
                this.updateDOM();
            })
        });

        const awardCalc = this.container.querySelector('#award-btn');
        if (awardCalc) {
            awardCalc.addEventListener('click', async (event) => {
                const points = Number(this.container.querySelector('#points-input').value);
                await addPoints(this.data.roomCode, points, this.state.buzzedTeam, this.state.gameId);
                this.state.buzzedTeam = null;
                this.state.activeCell = null;
                this.updateDOM();
            })
        }

        const wrongBtn = this.container.querySelector('#wrong-btn');
        if (wrongBtn) {
            wrongBtn.addEventListener('click', async (event) => {
                const points = Number(this.container.querySelector('#points-input').value);
                await addPoints(this.data.roomCode, points, this.state.buzzedTeam, this.state.gameId);
                this.state.buzzedTeam = null;
                this.state.activeCell = null;
                this.updateDOM();
            });
        }

        const endBtn = this.container.querySelector('#end-btn');
        if (endBtn) {
            endBtn.addEventListener('click', (event) => {

            })
        }
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