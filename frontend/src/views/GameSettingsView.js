import { Component } from "../core/Component.js";
import { Logo } from "../components/UI.js";
import {get404, tryGetRoomInfo} from "../services/RoomService.js";

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
            costs: [100, 200, 300]
        };
    }

    async mount() {
        const roomInfo = await tryGetRoomInfo(this.data.roomCode);
        if (!roomInfo.exists) {
            this.container.innerHTML = get404();
            return;
        }

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
                        <button class="btn btn-primary w-100" id="save-btn">Создать игру</button>
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
                                        ${this.state.costs.map(cost => `
                                            <td><div class="preview-cell">${cost}</div></td>
                                        `).join('')}
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
    }
}