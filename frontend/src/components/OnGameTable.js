export const OnGameTable = (gameSettings, state, options = {}) => {
    return `
    <div class="scroll-container">
        <table class="edit-table game-table">
            <thead>
                <tr>
                    <th class="corner-cell">Категория / Цена</th>
                    ${Costs(gameSettings.costs)}
                </tr>
            </thead>

            <tbody>
                ${TableBody(gameSettings, state, options)}
            </tbody>
        </table>
    </div>
    `;
}

const Costs = (costs) => {
    const row = (cost) => {
        return `
                <th class="cost-th">
                    <div class="th-content readonly-th">
                        <span>${cost}</span>
                    </div>
                </th>
        `;
    }
    return costs.map(cost => row(cost)).join('');
}

const TableBody = (gameSettings, state, options) => {
    return gameSettings.categories
        .map((category, rIdx) => `
                    <tr>
                        <td class="cat-td">
                            <div class="td-content readonly-td">
                                <span>${category}</span>
                            </div>
                        </td>
                        
                        ${RowSongs(gameSettings, state, options, rIdx)}
                    </tr>`)
        .join('');
}

const RowSongs = (gameSettings, state, options, rIdx) => {
    const {
        showFilledState = true,
        showActiveState = true,
        emptyText = '-',
        useLaunchText = false
    } = options;

    return gameSettings.costs
        .map((cost, cIdx) => {

            const cell = gameSettings.cells?.[rIdx]?.[cIdx] || {};
            const isActive = state.activeCell?.row === rIdx && state.activeCell?.col === cIdx;

            const filledClass = showFilledState && cell.song ? 'cell-filled' : '';
            const activeClass = showActiveState && isActive ? 'cell-active' : '';

            const notPlayedText = useLaunchText ? '▶ Запустить' : emptyText;

            const text = isActive ? '🔊 Играет...' : !cell.played
                ? notPlayedText
                : 'Нет трека';

            return `<td>${CellButton(filledClass, activeClass, rIdx, cIdx, text)}</td>`;
        })
        .join('');
}

const CellButton = (filledClass, activeClass, rIdx, cIdx, text) => {
    return `
        <button
            class="preview-cell track-cell organizer-track-btn ${filledClass} ${activeClass}"
            data-row="${rIdx}"
            data-col="${cIdx}"
            style="width: 100%;">
            <div class="cell-sub">
                ${text}
            </div>
        </button>
    `;
}