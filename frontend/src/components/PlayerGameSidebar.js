import {Button} from "./UI.js";

export const PlayerGameSidebar = (state, gameSettings) => {
    const aboutQuestionText = state.activeCell
        ? 'Организатор запустил вопрос. Можно отвечать.'
        : 'Дождитесь, пока организатор выберет категорию.';

    const onAnswerButton = state.activeCell && state.canBuzz && !state.hadWrongAnswer
        ? Button(onAnswerBtnSettings)
        : `<button class="btn btn-secondary w-100" disabled>${getDisabledReason(state)}</button>`

    return `
        <aside class="editor-sidebar">
                    <div class="card">
                        <h2 class="card-title">Информация об игре</h2>

                        <div class="game-meta">
                            <p><strong>Название:</strong> ${gameSettings.title || '—'}</p>
                            <p><strong>Автор:</strong> ${gameSettings.author || '—'}</p>
                            <p><strong>Описание:</strong> ${gameSettings.description || '—'}</p>
                        </div>

                        <div class="player-status-card" style="margin-top: 1rem; margin-bottom: 1rem;"">
                            <div class="track-preview-subtitle">${aboutQuestionText}</div>
                        </div>

                        ${onAnswerButton}
                    </div>
                </aside>
    `;
}

const onAnswerBtnSettings = {
    text: 'Ответить',
    id: 'buzz-btn',
    extraClass: 'w-100'
}

function getDisabledReason(state) {
    return state.hadWrongAnswer
        ? 'Вы ответили неправильно'
        : state.activeCell
            ? 'Кто-то уже отвечает'
            : 'Ожидание вопроса';
}