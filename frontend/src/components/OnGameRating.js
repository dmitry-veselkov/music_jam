export const OnGameRating = (players, myTeam = null) => {
    const entries = Object.entries(players);
    const sorted = [...entries]
        .sort((a, b) => b[1] - a[1]);
    return `
            <div class="player-table-wrapper">
                <table class="player-score-table">
                    <thead>
                        <tr>
                            <th>#</th>
                            <th>Команда</th>
                            <th>Очки</th>
                         </tr>
                    </thead>
                    <tbody>
                        ${sorted.map(([team, score], idx) => `
                            <tr class="${idx === 0 ? 'rank-first' : ''}">
                                <td class="rank-cell">${idx + 1}</td>
                                <td class="team-cell ${myTeam === team ? 'my-team' : ''}">${team}</td>
                                <td class="score-cell ${myTeam === team ? 'my-team' : ''}">${score}</td>
                            </tr>
                        `).join('')}
                    </tbody>
                </table>
            </div>`;
}

export const TopPlayer = (players) => {
    const entries = Object.entries(players);
    const sorted = [...entries]
        .sort((a, b) => b[1] - a[1]);
    if (sorted.length === 0) return ``;
    const [team, score] = sorted[0];
    return `
        <div class="top-player-wrapper">
            <h2 class="top-player-title">Победитель игры!</h2>
            <div class="top-player-card">
                <div class="top-player-team">${team}</div>
                <div class="top-player-score">${score} очков</div>
            </div>
        </div>
    `;
}