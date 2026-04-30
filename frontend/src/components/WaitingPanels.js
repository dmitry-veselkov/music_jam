import {Button, Input} from "./UI.js";

export const WaitingInfoPanel = (state, data) => {
    const inputSettings = {
        id: "team-name-input",
        placeholder: "Например: Команда 1",
        maxLength: 60,
        value: state.myTeamName,
        disabled: state.isNameSaved
    }

    const buttonSettings = {
        id: "save-team-btn",
        text: state.isNameSaved ? "Изменить" : "Войти в игру",
        variant: state.isNameSaved ? "outline" : "primary"
    }

    return `<aside class="waiting-info-panel">
                    <div class="card info-card">
                        <div class="badge room-badge">Код комнаты: ${data.roomCode}</div>
                        <h1 class="game-title">${state.gameName}</h1>
                        <p class="game-author">Создатель: <strong>${state.creator}</strong></p>
                        
                        <div class="team-form-container">
                            <h3>Ваша команда</h3>
                            <p class="text-muted">Придумайте название, чтобы другие вас узнали.</p>
                            
                            <div class="team-input-group">
                                ${Input(inputSettings)}
                            </div>
                            
                            <div class="btn-wrapper mt-3">
                                ${Button(buttonSettings)}
                            </div>
                        </div>
                    </div>
                </aside>`
}

export const WaitingTeamsPanel = (state) => {
    const teamsListLogic = state.teams.length === 0
        ? `<li class="empty-state">Пока никого нет. Будьте первыми!</li>`
        : state.teams
            .map(team => `
                    <li class="team-item ${team === state.myTeamName ? 'my-team' : ''}">
                        <span class="team-icon">👥</span>
                        <span class="team-name">${team}</span>
                        ${team === state.myTeamName ? '<span class="badge-you">Вы</span>' : ''}
                    </li>`)
            .join('');

    return `<main class="waiting-teams-panel">
                    <div class="card teams-card">
                        <div class="teams-header">
                            <h2>Уже в лобби 
                                <span class="team-count">
                                    (${state.teams.length})
                                </span>
                            </h2>
                            <div class="pulse-indicator">Ожидание...</div>
                        </div>
                        
                        <ul class="teams-list">
                            ${teamsListLogic}
                        </ul>
                    </div>
                </main>
    `;
}