import { Component } from "../core/Component.js";
import { Logo, Button } from "../components/UI.js";
import { get404, tryGetRoomSettings } from "../services/RoomService.js";
import { loadUserInfoOrRedirect } from "../services/AccountServices.js";

export class HostAdminView extends Component {
    constructor(container, data) {
        super(container, data);
        this.state = {
            settings: {},
            categories: [],
            costs: [],
            tracks: {},
            teams: [], // [{id, name, score}]
            activeCell: null,
            isAnswerRevealed: false
        };
    }

    _applyRoomInfo(roomInfo) {
        this.state.settings = roomInfo.settings || {};
        this.state.categories = roomInfo.categories || [];
        this.state.costs = roomInfo.costs || [];
        this.state.tracks = roomInfo.tracks || {};
        // В реальном приложении это придет через Socket.io
        this.state.teams = roomInfo.teams || [
            { id: 1, name: "Команда Альфа", score: 1200 },
            { id: 2, name: "Бета-Тестеры", score: 800 }
        ];
    }

    async mount() {
        const userInfo = await loadUserInfoOrRedirect();
        if (!userInfo) return;

        const roomInfo = await tryGetRoomSettings(this.data.roomCode);
        if (!roomInfo.exists) {
            this.container.innerHTML = get404();
            return;
        }

        this._applyRoomInfo(roomInfo);
        this.updateDOM();
        this._initSocket();
    }

    _initSocket() {
        // Здесь будет ваша логика Socket.IO для обновления рейтинга в реальном времени
        // и получения уведомлений, когда кто-то нажал кнопку "Ответить"
    }

    render() {
        const { activeCell, isAnswerRevealed, teams } = this.state;
        const currentTrack = activeCell ? this.state.tracks[`${activeCell.row}-${activeCell.col}`] : null;

        return `
        <div class="logo-corner">${Logo()}</div>
        <div class="editor-layout admin-mode">
            
            <aside class="editor-sidebar">
                <div class="card shadow-lg">
                    <h2 class="card-title">Рейтинг команд</h2>
                    <div class="teams-control-list">
                        ${teams.map(team => `
                            <div class="team-admin-card">
                                <div class="team-info">
                                    <span class="team-name">${team.name}</span>
                                    <span class="team-score ${team.score < 0 ? 'text-danger' : 'text-primary'}">
                                        ${team.score}
                                    </span>
                                </div>
                                <div class="score-actions">
                                    <button class="btn-score minus" data-team-id="${team.id}">-</button>
                                    <input type="number" class="score-input" value="100" id="adj-${team.id}">
                                    <button class="btn-score plus" data-team-id="${team.id}">+</button>
                                </div>
                            </div>
                        `).join('')}
                    </div>
                </div>

                <div class="card mt-3 highlight-card">
                    <h3 class="card-title">Текущий трек</h3>
                    <div class="track-status-box">
                        ${activeCell ? `
                            <div class="active-info">
                                <div class="badge-sm">Цена: ${this.state.costs[activeCell.col]}</div>
                                <div class="correct-answer-box mt-2">
                                    <label>Правильный ответ:</label>
                                    <div class="answer-text">${currentTrack || "Не задан"}</div>
                                </div>
                                <div class="admin-controls mt-3">
                                    <button class="btn btn-secondary btn-sm w-100" id="play-reveal">
                                        ${isAnswerRevealed ? '⏸ Стоп' : '▶ Играть ответ'}
                                    </button>
                                    <button class="btn btn-outline btn-sm w-100 mt-2" id="reset-cell">Сбросить вопрос</button>
                                </div>
                            </div>
                        ` : `
                            <p class="text-muted text-center">Выберите ячейку на поле</p>
                        `}
                    </div>
                </div>
            </aside>

            <main class="editor-main">
                <div class="table-header-actions">
                    <div class="badge">Комната: ${this.data.roomCode}</div>
                    <div class="status-indicator">● В ЭФИРЕ</div>
                </div>

                <div class="scroll-container">
                    <table class="edit-table admin-game-table">
                        <thead>
                            <tr>
                                <th class="corner-cell">Ряд / Столбец</th>
                                ${this.state.costs.map(cost => `<th>${cost}</th>`).join('')}
                            </tr>
                        </thead>
                        <tbody>
                            ${this.state.categories.map((cat, rIdx) => `
                                <tr>
                                    <td class="cat-td">${cat}</td>
                                    ${this.state.costs.map((cost, cIdx) => {
            const isPlayed = false; // Добавить логику из state
            const isActive = activeCell && activeCell.row == rIdx && activeCell.col == cIdx;
            return `
                                            <td>
                                                <button class="admin-cell ${isActive ? 'active' : ''} ${isPlayed ? 'played' : ''}" 
                                                        data-row="${rIdx}" data-col="${cIdx}">
                                                    ${cost}
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

    attachEvents() {
        // Управление счетом
        this.container.querySelectorAll('.btn-score').forEach(btn => {
            btn.onclick = (e) => {
                const teamId = e.target.dataset.teamId;
                const isPlus = e.target.classList.contains('plus');
                const amount = parseInt(this.container.querySelector(`#adj-${teamId}`).value);
                this._updateScore(teamId, isPlus ? amount : -amount);
            };
        });

        // Выбор трека на поле
        this.container.querySelectorAll('.admin-cell').forEach(cell => {
            cell.onclick = (e) => {
                const { row, col } = e.target.dataset;
                this.state.activeCell = { row, col };
                this.state.isAnswerRevealed = false;
                // socket.emit('launch_track', {row, col});
                this.updateDOM();
            };
        });

        // Кнопка воспроизведения ответа
        const revealBtn = this.container.querySelector('#play-reveal');
        if (revealBtn) {
            revealBtn.onclick = () => {
                this.state.isAnswerRevealed = !this.state.isAnswerRevealed;
                // socket.emit('play_answer_fragment');
                this.updateDOM();
            };
        }
    }

    _updateScore(teamId, delta) {
        const team = this.state.teams.find(t => t.id == teamId);
        if (team) {
            team.score += delta;
            // socket.emit('update_score', {teamId, newScore: team.score});
            this.updateDOM();
        }
    }

    updateDOM() {
        this.container.innerHTML = this.render();
        this.attachEvents();
    }
}