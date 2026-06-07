export const AsideSettings = (gameSettings) => {
    return `
            <aside class="editor-sidebar">
                <div class="card">
                    <h2 class="card-title">Параметры игры</h2>
                    
                    ${TitleForm(gameSettings.title)}
                    ${DescriptionForm(gameSettings.description)}
                    ${ModeForm(gameSettings.mode)}

                    <div class="d-flex flex-column">
                        <button class="btn btn-primary w-100" id="save-changes" style="margin-top: 8px;">
                            Сохранить изменения
                        </button>
                    </div>
                </div>
            </aside>
    `;
}

export const MainEditor = (gameSettings, roomCode) => {
    return `
            <main class="editor-main">

                ${TableHeader(roomCode)}

                <div class="scroll-container">
                    <table class="edit-table">
                        <thead>
                            <tr>
                                <th class="corner-cell">Категория / Цена</th>
                                ${CostsRow(gameSettings.costs)}
                                
                            </tr>
                        </thead>

                        <tbody>
                            ${gameSettings.cells.map((row, rIdx) => `
                                <tr>
                                    <td class="cat-td">
                                        <input type="text"
                                               class="cat-edit"
                                               data-idx="${rIdx}"
                                               value="${gameSettings.categories[rIdx]}">
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
                                        `;}).join('')}
                                </tr>`).join('')}
                        </tbody>
                    </table>
                </div>
            </main>
    `;
}

const CostsRow = (costs) => {
    const row = (idx, cost) => {
        return `
                <th class="cost-th">
                    <div class="th-content">
                        <input type="number"
                               class="cost-edit"
                               data-idx="${idx}"
                               value="${cost}"
                               min="0"
                               max="3000">
            
                        <button class="remove-btn remove-col"
                                data-idx="${idx}">
                            ×
                        </button>
                    </div>
                </th>`
            ;
    }

    return costs.map((cost, idx) => row(idx, cost)).join('');
}

const TableHeader = (roomCode) => {
    return `
            <div class="table-header-actions">
                <div class="badge">Код комнаты: ${roomCode}</div>

                <div class="btn-group">
                    <button class="btn btn-outline btn-sm" id="add-row">+ Категория</button>
                    <button class="btn btn-outline btn-sm" id="add-col">+ Стоимость</button>
                </div>
            </div>
    `;
}

const TitleForm = (title) => {
    return `
            <div class="form-group">
                <label>Название</label>
                <input type="text"
                       class="ui-input sync-input"
                       data-key="title"
                       value="${title}"
                       placeholder="Название свояка">
            </div>
    `;
}

const DescriptionForm = (description) => {
    return `
            <div class="form-group">
                <label>Описание</label>
                <textarea class="ui-input sync-input"
                          data-key="description"
                          rows="3"
                          placeholder="О чем эта игра?">${description?.trim()}</textarea>
            </div>
    `;
}

const ModeForm = (mode) => {
    return `
            <div class="form-group">
                <label style="display: flex; align-items: center; gap: 0.5rem;">
                    Режим игры
                    <button class="remove-btn" 
                            id="open-mode-explanation" 
                            title="Описание режимов?" 
                            style="width: 22px; height: 22px; font-size: 0.85rem; color: var(--color-text-muted);">?</button>
                </label>
                    <div class="mode-toggle">
                        <label class="mode-option">
                            <input type="radio"
                                name="game-mode"
                                value="text"
                                ${!mode ? 'checked' : ''}>
                            <span>📝 Текстовый</span>
                        </label>
                        <label class="mode-option">
                            <input type="radio"
                               name="game-mode"
                               value="voice"
                               ${mode ? 'checked' : ''}>
                            <span>🔊 Озвучка</span>
                        </label>
                    </div>
            </div>
    `;
}


export const Toast = (text = 'Изменения сохранены') => {
    return `
        <div class="toast" id="save-toast">
            ${text}
        </div>
    `;
}

export const ModeExplanationModal = () => {
    return `
        <div class="modal-overlay hidden" id="mode-explanation-overlay">
            <div class="card modal-card" style="position: relative; padding: 1.5rem;">
                <button class="modal-close" id="close-mode-explanation">×</button>
                
                <h2 class="card-title" style="margin-bottom: 1rem;">Режимы игры</h2>

                <div class="track-preview-card" style="margin-bottom: 0.75rem;">
                    <div class="track-preview-icon">🔊</div>
                    <div class="track-preview-text">
                        <div class="track-preview-title">Озвучка</div>
                        <div class="track-preview-subtitle">
                            Игроки отвечают вслух. Подходящий режим для очной игры
                        </div>
                    </div>
                </div>

                <div class="track-preview-card">
                    <div class="track-preview-icon">📝</div>
                    <div class="track-preview-text">
                        <div class="track-preview-title">Текстовый режим</div>
                        <div class="track-preview-subtitle">
                            Игроки отвечают в формочке. Подходящий режим для онлайн-игры
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `;
}