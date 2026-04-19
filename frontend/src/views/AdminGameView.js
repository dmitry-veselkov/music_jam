import {Component} from "../core/Component.js";
import {Logo} from "../components/UI.js";
import {get404} from "../services/RouteServices.js";
import {tryGetGameSettings, saveGameSettings} from "../services/GamesServices.js";
import {loadUserInfoOrRedirect} from "../services/AccountServices.js";

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
        const userInfo = await loadUserInfoOrRedirect();
        if (!userInfo) {
            return;
        }
        const roomInfo = await tryGetGameSettings(this.data.roomCode);
        if (!roomInfo.exists) {
            this.container.innerHTML = get404();
            return;
        }
        this._applyRoomInfo(roomInfo);
        this.updateDOM();
    }

    updateDOM() {
        this.container.innerHTML = this.render();
        this.attachEvents();
    }

    render() {
        const activeCell = this.state.activeCell || null;
        const playedTracks = this.state.playedTracks || {};

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

                    <div class="form-group">
                        <label>Команд (макс)</label>
                        <div class="ui-input readonly-field">${this.state.settings.maxTeams || '—'}</div>
                    </div>

                    <div class="btn-group-vertical">
                        <button class="btn btn-primary w-100" id="end-btn">Завершить игру</button>
                    </div>
                </div>
            </aside>

            <main class="editor-main">
                <div class="table-header-actions">
                    <div class="badge">Код комнаты: ${this.data.roomCode}</div>
                    <div class="badge">Режим организатора</div>
                </div>

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
            const trackKey = `${rIdx}-${cIdx}`;
            const trackValue = this.state.tracks[trackKey];
            const isActive =
                activeCell &&
                Number(activeCell.row) === rIdx &&
                Number(activeCell.col) === cIdx;
            const isPlayed = !!playedTracks[trackKey];

            return `
                                            <td>
                                                <button
                                                    class="preview-cell track-cell organizer-track-btn ${isActive ? 'active' : ''} ${isPlayed ? 'played' : ''}"
                                                    data-row="${rIdx}"
                                                    data-col="${cIdx}"
                                                    data-track="${trackValue ? trackValue : ''}"
                                                >
                                                    <div class="cell-main">
                                                        ${cost}
                                                    </div>
                                                    <div class="cell-sub" style="width: 100%">
                                                        ${trackValue ? '▶ Запустить' : 'Нет трека'}
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
            </main>
        </div>
    `;
    }
}