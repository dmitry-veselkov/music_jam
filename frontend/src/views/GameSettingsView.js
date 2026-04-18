import {Component} from "../core/Component.js";
import {Logo} from "../components/UI.js";
import {get404, tryGetRoomInfo, tryGetGameSettings, saveGameSettings} from "../services/RoomService.js";
import {loadUserInfoOrRedirect} from "../services/AccountServices.js";

export class GameSettingsView extends Component {
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
            tracks: {}
        };
    }

    async mount() {
        const userInfo = await loadUserInfoOrRedirect();
        if (!userInfo) {
            return;
        }

        const historyState = window.history.state;
        const roomInfo = await tryGetGameSettings(this.data.roomCode);
        if (!roomInfo.exists && !(historyState && historyState.isNew)) {
            this.container.innerHTML = get404();
            return;
        }
        this._applyRoomInfo(roomInfo);
        this.updateDOM();
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

    updateDOM() {
        this.container.innerHTML = this.render();
        this.attachEvents();
    }

    render() {
        return `
            <div class="logo-corner">${Logo()}</div>
            <div class="editor-layout">
                
                <aside class="editor-sidebar">
                    <div class="card">
                        <h2 class="card-title">Параметры игры</h2>
                        <div class="form-group">
                            <label>Название</label>
                            <input type="text" class="ui-input sync-input" data-key="name" value="${this.state.settings.name}" placeholder="Название квиза">
                        </div>
                        <div class="form-group">
                            <label>Автор</label>
                            <input type="text" class="ui-input sync-input" data-key="author" value="${this.state.settings.author}" placeholder="Ваше имя">
                        </div>
                        <div class="form-group">
                            <label>Описание</label>
                            <textarea class="ui-input sync-input" data-key="description" rows="3" placeholder="О чем эта игра?">${this.state.settings.description}</textarea>
                        </div>
                        <div class="form-group">
                            <label>Команд (макс): <span id="team-val">${this.state.settings.maxTeams}</span></label>
                            <input type="range" class="sync-input" data-key="maxTeams" min="2" max="8" value="${this.state.settings.maxTeams}">
                        </div>
                        <div class="btn-group-vertical">
                            <button class="btn btn-primary w-100" id="save-btn">Создать игру</button>
                            <button class="btn btn-primary w-100" id="save-changes">Сохранить изменения</button>
                        </div>
                    </div>
                </aside>

                <main class="editor-main">
                    <div class="table-header-actions">
                        <div class="badge">Код комнаты: ${this.data.roomCode}</div>
                        <div class="btn-group">
                            <button class="btn btn-outline btn-sm" id="add-row">+ Категория</button>
                            <button class="btn btn-outline btn-sm" id="add-col">+ Стоимость</button>
                        </div>
                    </div>
                    
                    <div class="scroll-container">
                        <table class="edit-table">
                            <thead>
                                <tr>
                                    <th class="corner-cell">Категория / Цена</th>
                                    ${this.state.costs.map((cost, idx) => `
                                        <th class="cost-th">
                                            <div class="th-content">
                                                <input type="number" class="cost-edit" data-idx="${idx}" value="${cost}">
                                                <button class="remove-btn remove-col" data-idx="${idx}">×</button>
                                            </div>
                                        </th>
                                    `).join('')}
                                </tr>
                            </thead>
                            <tbody>
                                ${this.state.categories.map((cat, rIdx) => `
                                    <tr>
                                        <td class="cat-td">
                                            <div class="td-content">
                                                <input type="text" class="cat-edit" data-idx="${rIdx}" value="${cat}">
                                                <button class="remove-btn remove-row" data-idx="${rIdx}">×</button>
                                            </div>
                                        </td>
                                        ${this.state.costs.map((cost, cIdx) => {
            const trackKey = `${rIdx}-${cIdx}`;
            const trackValue = this.state.tracks[trackKey];
            return `
                                            <td><div class="preview-cell"
                                                     data-row="${rIdx}"
                                                     data-col="${cIdx}">${trackValue ? trackValue : cost}</div></td>`
        }).join('')}
                                        </tr>
                                    `).join('')}
                            </tbody>
                        </table>
                    </div>
                </main>
            </div>
            <div id="track-modal" class="modal hidden">
                <div class="modal-backdrop"></div>

                    <div class="modal-dialog">
                        <button class="modal-close" id="close-modal" aria-label="Закрыть">×</button>

                    <div class="modal-header">
                        <div class="modal-badge">🎵 Трек</div>
                            <h3 class="modal-title">Добавить трек</h3>
                                <p class="modal-subtitle">
                                    Укажи название композиции, чтобы привязать её к выбранной ячейке.
                                </p>
                        </div>

                        <div class="modal-body">
                            <label for="track-input" class="modal-label">Аудио для вопроса</label>
                                <input
                                    type="file"
                                    id="track-input"
                                    class="modal-input"
                                    placeholder="Аудио для вопроса"
                                >
                                
                        <div class="modal-body">
                            <label for="track-input" class="modal-label">Ответ</label>
                                <input
                                    type="file"
                                    id="track-input"
                                    class="modal-input"
                                    placeholder="Ответ"
                                >        

                        <div class="modal-actions">
                            <button class="btn btn-secondary" id="cancel-track">Отмена</button>
                            <button class="btn btn-primary" id="save-track">Сохранить</button>
                        </div>
                </div>
            </div>
        `;
    }

    attachEvents() {
        this.container.querySelectorAll('.sync-input').forEach(input => {
            input.addEventListener('input', (e) => {
                const key = e.target.dataset.key;
                this.state.settings[key] = e.target.value;
                if (key === 'maxTeams') {
                    this.container.querySelector('#team-val').textContent = e.target.value;
                }
            });
        });

        this.container.querySelector('#add-row').onclick = () => {
            this.state.categories.push(`Категория ${this.state.categories.length + 1}`);
            this.updateDOM();
        };

        this.container.querySelector('#add-col').onclick = () => {
            const last = this.state.costs[this.state.costs.length - 1] || 0;
            this.state.costs.push(last + 100);
            this.updateDOM();
        };

        this.container.querySelectorAll('.remove-row').forEach(btn => {
            btn.onclick = () => {
                this.state.categories.splice(parseInt(btn.dataset.idx), 1);
                this.updateDOM();
            };
        });

        this.container.querySelectorAll('.remove-col').forEach(btn => {
            btn.onclick = () => {
                this.state.costs.splice(parseInt(btn.dataset.idx), 1);
                this.updateDOM();
            };
        });

        this.container.querySelectorAll('.cost-edit').forEach(input => {
            input.onchange = (e) => {
                this.state.costs[e.target.dataset.idx] = Number(e.target.value);
                this.updateDOM();
            };
        });

        this.container.querySelectorAll('.cat-edit').forEach(input => {
            input.oninput = (e) => {
                this.state.categories[e.target.dataset.idx] = e.target.value;
            };
        });

        this.container.querySelectorAll('.preview-cell').forEach(cell => {
            cell.addEventListener('click', (e) => {
                const row = e.currentTarget.dataset.row;
                const col = e.currentTarget.dataset.col;

                this.openModal(row, col);
            });
        });

        const closeBtn = this.container.querySelector('#cancel-track');
        if (closeBtn) {
            closeBtn.onclick = () => this.closeModal();
        }

        const closeBtnKrest = this.container.querySelector('#close-modal');
        if (closeBtnKrest) {
            closeBtnKrest.onclick = () => this.closeModal();
        }

        const saveBtn = this.container.querySelector('#save-track');
        if (saveBtn) {
            saveBtn.onclick = async () => {
                const input = this.container.querySelector('#track-input');
                const value = input.value;
                if (!value || !this.currentCell) return;

                const key = `${this.currentCell.row}-${this.currentCell.col}`;
                this.state.tracks[key] = value;
                await saveGameSettings(this.getPayload());
                this.closeModal();
                this.updateDOM();
            };
        }

        const saveChangesBtn = this.container.querySelector('#save-changes');
        if (saveChangesBtn) {
            saveChangesBtn.onclick = async () => {
                await saveGameSettings(this.getPayload());
                this.updateDOM();
            };
        }
    }

    openModal(row, col) {
        this.currentCell = {row, col};

        const modal = this.container.querySelector('#track-modal');
        modal.classList.remove('hidden');
    }

    closeModal() {
        const modal = this.container.querySelector('#track-modal');
        modal.classList.add('hidden');
    }

    getPayload() {
        return {
            roomCode: this.data.roomCode,
            name: this.state.settings.name,
            author: this.state.settings.author,
            description: this.state.settings.description,
            maxTeams: Number(this.state.settings.maxTeams),
            categories: this.state.categories,
            costs: this.state.costs,
            tracks: this.state.tracks
        };
    }
}