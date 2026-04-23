import {Component} from "../core/Component.js";
import {Logo} from "../components/UI.js";
import {tryGetGameSettings, saveGameSettings} from "../services/GamesServices.js";
import {get404} from "../services/RouteServices.js";
import {loadUserInfoOrRedirect} from "../services/AccountServices.js";
import {GameSettings} from "../domain/GameSettings.js";
import {Song} from "../domain/Song.js";
import {ButtonLoader} from "../components/ButtonLoader.js";

export class GameSettingsView extends Component {
    constructor(container, data) {
        super(container, data);
        this.gameSettings = new GameSettings();
        this.currentCell = null;
    }

    async mount() {
        const userInfo = await loadUserInfoOrRedirect();
        if (!userInfo) return;

        const roomInfo = await tryGetGameSettings(this.data.roomCode);
        if (!roomInfo || !roomInfo.exists) {
            this.container.innerHTML = get404();
            return;
        }

        this.gameSettings.init(roomInfo, userInfo);
        this.updateDOM();
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
                        <input type="text"
                               class="ui-input sync-input"
                               data-key="title"
                               value="${this.gameSettings.title}"
                               placeholder="Название свояка">
                    </div>

                    <div class="form-group">
                        <label>Описание</label>
                        <textarea class="ui-input sync-input"
                                  data-key="description"
                                  rows="3"
                                  placeholder="О чем эта игра?">${this.gameSettings.description}</textarea>
                    </div>
                    
                    <div class="form-group">
                        <label>Режим игры</label>
                            <div class="mode-toggle">
                                <label class="mode-option">
                                    <input type="radio"
                                        name="game-mode"
                                        value="text"
                                        ${!this.gameSettings.mode ? 'checked' : ''}>
                                    <span>📝 Текстовый</span>
                                </label>
                                <label class="mode-option">
                                    <input type="radio"
                                       name="game-mode"
                                       value="voice"
                                       ${this.gameSettings.mode ? 'checked' : ''}>
                                    <span>🔊 Озвучка</span>
                                </label>
                            </div>
                    </div>

                    <div class="d-flex flex-column">
                        <button class="btn btn-primary w-100" id="save-changes" style="margin-top: 8px;">
                            Сохранить изменения
                        </button>
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

                                ${this.gameSettings.costs.map((cost, idx) => `
                                    <th class="cost-th">
                                        <div class="th-content">
                                            <input type="number"
                                                   class="cost-edit"
                                                   data-idx="${idx}"
                                                   value="${cost}">

                                            <button class="remove-btn remove-col"
                                                    data-idx="${idx}">
                                                ×
                                            </button>
                                        </div>
                                    </th>
                                `).join('')}
                            </tr>
                        </thead>

                        <tbody>
                            ${this.gameSettings.cells.map((row, rIdx) => `
                                <tr>
                                    <td class="cat-td">
                                        <input type="text"
                                               class="cat-edit"
                                               data-idx="${rIdx}"
                                               value="${this.gameSettings.categories[rIdx]}">
                                    </td>

                                    ${row.map((cell, cIdx) => {
            const song = cell?.song;
            const hasSong = !!song;
            return `
                                            <td>
                                                <div class="preview-cell ${hasSong ? 'cell-filled' : ''}"
                                                     data-row="${rIdx}"
                                                     data-col="${cIdx}">
                                    
                                                    ${
                hasSong
                    ? `<div class="cell-title">🎵 ${song.title}</div>`
                    : `<div class="cell-empty">+</div>`
            }
                                    
                                                </div>
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

        ${this.renderModal()}
    `;
    }

    renderModal() {
        return `
            <div id="track-modal" class="modal hidden">
                <div class="modal-backdrop"></div>
    
                <div class="modal-dialog">
                    <button class="modal-close" id="close-modal" aria-label="Закрыть">×</button>
    
                    <div class="modal-header">
                        <div class="modal-badge">🎵 Трек</div>
                        <h3 class="modal-title">Добавить трек</h3>
                    </div>
    
                    <div class="modal-body">
                        <label for="track-title-input" class="modal-label">Название трека</label>
                        <input
                            type="text"
                            id="track-title-input"
                            class="modal-input"
                            placeholder="Введите название трека"
                        >
    
                            <label for="track-artist-input" class="modal-label">Исполнитель</label>
                            <input
                                type="text"
                                id="track-artist-input"
                                class="modal-input"
                                placeholder="Введите исполнителя"
                            >
    
                                <label for="question-track-input" class="modal-label">Аудио для вопроса</label>
                                <input
                                    type="file"
                                    id="question-track-input"
                                    class="modal-input"
                                >
    
                                    <label for="answer-track-input" class="modal-label">Ответ</label>
                                    <input
                                        type="file"
                                        id="answer-track-input"
                                        class="modal-input"
                                    >
                    </div>
    
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
                this.gameSettings[key] = e.target.value;
                console.log(this.gameSettings[key], key);
            });
        });

        this.container.querySelector('#add-row').onclick = () => {
            const newCategory = `Категория ${this.gameSettings.categories.length + 1}`;
            this.gameSettings.addRow(newCategory);
            this.updateDOM();
        };

        this.container.querySelector('#add-col').onclick = () => {
            const last = this.gameSettings.costs.at(-1) || 0;
            const newCost = last + 100;
            this.gameSettings.addCol(newCost);
            this.updateDOM();
        };

        this.container.querySelectorAll('.remove-row').forEach(btn => {
            btn.onclick = () => {
                const idx = +btn.dataset.idx;
                this.gameSettings.removeRow(idx);
                this.updateDOM();
            };
        });

        this.container.querySelectorAll('.remove-col').forEach(btn => {
            btn.onclick = () => {
                const idx = +btn.dataset.idx;
                this.gameSettings.removeCol(idx);
                this.updateDOM();
            };
        });

        this.container.querySelectorAll('.cost-edit').forEach(input => {
            input.onchange = (e) => {
                const idx = +e.target.dataset.idx;
                this.gameSettings.costs[idx] = Number(e.target.value);
                this.updateDOM();
            };
        });

        this.container.querySelectorAll('.cat-edit').forEach(input => {
            input.oninput = (e) => {
                const idx = +e.target.dataset.idx;
                this.gameSettings.categories[idx] = e.target.value;
            };
        });

        this.container.querySelectorAll('.preview-cell').forEach(cell => {
            cell.onclick = (e) => {
                this.currentCell = {
                    row: +e.currentTarget.dataset.row,
                    col: +e.currentTarget.dataset.col
                };
                this.openModal();
            };
        });

        const cancelBtn = this.container.querySelector('#cancel-track');
        if (cancelBtn) {
            cancelBtn.onclick = () => this.closeModal();
        }

        const closeBtn = this.container.querySelector('#close-modal');
        if (closeBtn) {
            closeBtn.onclick = () => this.closeModal();
        }

        const saveTrackBtn = this.container.querySelector('#save-track');
        if (saveTrackBtn) {
            saveTrackBtn.onclick = async () => {
                await ButtonLoader.wrap(saveTrackBtn, async () => {
                    await this.uploadSong();
                    this.closeModal();
                    this.updateDOM();
                });
            };
        }

        const saveChangesBtn = this.container.querySelector('#save-changes');
        if (saveChangesBtn) {
            saveChangesBtn.onclick = async () => {
                await ButtonLoader.wrap(saveChangesBtn, async () => {
                    await saveGameSettings(this.getPayload());
                    this.updateDOM();
                });
            };
        }

        this.container.querySelectorAll('input[name="game-mode"]').forEach(radio => {
            radio.onchange = (e) => {
                this.gameSettings.mode = e.target.value === 'voice';
            };
        });
    }

    openModal() {
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
            title: this.gameSettings.title,
            mode: this.gameSettings.mode,
            description: this.gameSettings.description,
            categories: this.gameSettings.categories,
            costs: this.gameSettings.costs,

            tracks: this.gameSettings.cells.map(row =>
                row.map(cell => cell.song ? {
                    title: cell.song.title,
                    artist: cell.song.artist,
                    question_url: cell.song.questionUrl,
                    answer_url: cell.song.answerUrl
                } : null)
            )
        };
    }

    async uploadSong() {
        const {row, col} = this.currentCell;
        const cell = this.gameSettings.cells[row][col];

        const title = this.container.querySelector('#track-title-input').value;
        const artist = this.container.querySelector('#track-artist-input').value;
        const question = this.container.querySelector('#question-track-input').files[0];
        const answer = this.container.querySelector('#answer-track-input').files[0];

        const formData = new FormData();
        formData.append("title", title);
        formData.append("artist", artist);
        formData.append("category", this.gameSettings.categories[row]);
        formData.append("cost", cell.cost);

        formData.append("question", question);
        formData.append("answer", answer);

        const res = await fetch("/api/upload_music", {
            method: "POST",
            body: formData
        });

        const data = await res.json();

        if (data.status !== "ok") {
            alert(data.error);
            return;
        }

        const song = new Song(
            data.song.title,
            data.song.artist,
            data.song.questionUrl,
            data.song.answerUrl
        );

        this.gameSettings.setSong(row, col, song);

        this.closeModal();
        this.updateDOM();
    }
}