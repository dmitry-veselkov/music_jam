export const GamesList = (games) => {
    const gamesList = games.map(game => GameCard(game)).join('');

    const panel = games.length === 0
        ? `<p class="empty-state">У вас пока нет созданных игр.</p>`
        : `<div class="games-grid">${gamesList}</div>`

    return `
            <div class="games-list-section mt-3">
                <h2>Ваши игры</h2>
                ${panel}
            </div>`
}

const GameCard = (game) => {
    return `
            <div class="card game-card">
                <h3>${game.title}</h3>
                <p class="room-code">Код лобби: <strong>${game.join_code}</strong></p>
                <div class="game-actions mt-3">
                    <button
                        class="btn btn-outline btn-sm"
                        data-edit-id="${game.id}"
                        data-game-code="${game.join_code}">
                        Редактировать
                    </button>
                    <button
                        class="btn btn-primary btn-sm"
                        data-start-id="${game.id}"
                        data-game-code="${game.join_code}">
                        Запустить
                    </button>
                </div>
            </div>`;
}