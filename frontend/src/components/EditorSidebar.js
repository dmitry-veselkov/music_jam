import {Button} from "./UI.js";

export const EditorSidebar = (state, gameSettings) => {
    const stopQuestionBtn = state.activeCell ? Button(stopQuestionBtnSettings) : ''
    const stopAnswerBtn = state.isShowingAnswer ? Button(stopAnswerBtnSettings) : ''

    return `
        <aside class="editor-sidebar">
                <div class="card">
                    <h2 class="card-title">Параметры игры</h2>

                    <div class="form-group">
                        <label>Название</label>
                        <div class="ui-input readonly-field">
                            ${gameSettings.title || '—'}
                        </div>
                    </div>

                    <div class="form-group">
                        <label>Описание</label>
                        <div class="ui-input readonly-field readonly-textarea">
                            ${gameSettings.description || '—'}
                        </div>
                    </div>

                    <div class="btn-group-vertical">
                        <div style="margin-top: 0.75rem;">
                            ${Button(endGameBtnSettings)}
                        </div>
                    </div>
                </div>
                
                <div class="audio-panel" style="margin-top: 1.5rem;">
                    <span class="audio-panel__label">🎵 Управление</span>
                        <div class="audio-panel__controls">
                            ${Button({text: '❚❚', id: 'music-btn'})}
                            ${stopQuestionBtn}
                            ${stopAnswerBtn}
                        </div>
                </div>
            </aside>
    `;
}

const stopQuestionBtnSettings = {
    text: 'Завершить вопрос',
    id: 'end-question-btn',
    variant: 'outline'
}

const stopAnswerBtnSettings = {
    text: '✕ Завершить ответ',
    id: 'clear-audio-btn',
    variant: 'outline'
}

const endGameBtnSettings = {
    text: 'Завершить игру',
    id: 'end-btn',
    extraClass: 'w-100'
}